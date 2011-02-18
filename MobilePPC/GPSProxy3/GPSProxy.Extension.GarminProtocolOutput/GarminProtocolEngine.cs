/*
$Id: GarminProtocolEngine.cs,v 1.1 2006/05/23 09:27:08 andrew_klopper Exp $

Copyright 2005-2006 Andrew Rowland Klopper (http://gpsproxy.sourceforge.net/)

This file is part of GPSProxy.

GPSProxy is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

GPSProxy is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with GPSProxy; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
using GPSProxy.Extension;

namespace GPSProxy.Extension.GarminProtocolOutput
{
	/// <summary>
	/// Summary description for GarminProtocol.
	/// </summary>
	public class GarminProtocolEngine
	{
		private const byte DLE = 16;
		private const byte ETX = 3;
		private const int SEND_TIMEOUT = 2;
		private const int MAX_SEND_RETRIES = 2;

		private const byte satEphMask		= 1;
		private const byte satDifMask		= 2;
		private const byte satUsedMask		= 4;
		private const byte satRisingMask	= 8;

		private const int GarminSNRRangeMin = 30;
		private const int GarminSNRRangeMax = 50;

		private enum PacketId :byte 
		{
			ACK					= 6,
			COMMAND_DATA		= 10,
			XFER_COMPLT			= 12,
			DATE_TIME_DATA		= 14,
			NACK				= 21,
			RECORDS				= 27,
			UNIT_ID				= 38,
			PVT_DATA			= 51,
			UNLOCK_CODE			= 108,
			UNLOCK_RESPONSE		= 109,
			SAT_DATA			= 114,
			PROTOCOL_ARRAY		= 253,
			PRODUCT_RQST		= 254,
			PRODUCT_DATA		= 255
		};

		private enum CommandId :short 
		{
			TRANSFER_ALM		= 1,
			TRANSFER_TIME		= 5,
			TRANSFER_UNIT_ID	= 14,
			START_PVT_DATA		= 49,
			STOP_PVT_DATA		= 50,
			START_SAT_DATA		= 107, // ???
			START_DIFF_SAT_DATA	= 108
		};

		private enum FixTypes :byte
		{
			Unusable = 0,
			Invalid = 1,
			Fix2D = 2,
			Fix3D = 3,
			Diff2D = 4,
			Diff3D = 5
		};

		private enum ProtocolState 
		{
			Idle, NeedAck, SendProductData, SendAlmanacStart
		};

		private ProtocolState protocolState = ProtocolState.Idle;

		private byte[] readBuffer = new byte[1024];
		private int readBufferBytesUsed = 0;

		private byte[] unacknowledgedPacket;
		private DateTime lastSendTime;
		private int sendRetries;
		private MemoryStream sendStream = new MemoryStream();

		private bool sendPVTData = false;
		private byte[] pvtPacket;

		private bool sendSatData = false;
		private byte[] satDataPacket;

		private Int16 ProductId;
		private Int16 SoftwareVersion;
		private string ProductDescription;
		private UInt32 UnitId;
		private int SatelliteSNRdBMin;
		private int SatelliteSNRdBMax;

		public GarminProtocolEngine(Int16 productId, Int16 softwareVersion, string productDescription,
			UInt32 unitId, int satelliteSNRdBMin, int satelliteSNRdBMax)
		{
			ProductId = productId;
			SoftwareVersion = softwareVersion;
			ProductDescription = productDescription;
			UnitId = unitId;
			SatelliteSNRdBMin = satelliteSNRdBMin;
			SatelliteSNRdBMax = satelliteSNRdBMax;
		}

		public byte[] Output 
		{
			get 
			{
				byte[] ret = sendStream.ToArray();
				sendStream.SetLength(0);
				return ret;
			}
		}

		public void Reset()
		{
			// We don't clear the receive buffer since there may be queued data that is still relevent to us,
			// plus clearing it here messes up the loop in ProcessReceivedData.
			protocolState = ProtocolState.Idle;
			unacknowledgedPacket = null;
			lastSendTime = DateTime.MinValue;
			sendRetries = 0;
			sendPVTData = false;
			sendSatData = false;
			pvtPacket = null;
			satDataPacket = null;
			sendStream.SetLength(0);
		}

		public void Tick()
		{
			DoRetransmissionCheck();
		}

		public void ProcessReceivedData(byte[] rawData) 
		{
			// Make sure the raw data will fit into the read buffer. If not, remove bytes from the beginning
			// so that we are always looking at the last data received.
			int rawDataOffset = 0;
			int rawDataLength = rawData.Length;
			if (rawDataLength > readBuffer.Length) 
			{
				rawDataOffset = rawDataLength - readBuffer.Length;
				rawDataLength = readBuffer.Length;
			}

			// Add the new data to the read buffer, shifting old, unprocessed data out if necessary.
			int discardBytes = rawDataLength + readBufferBytesUsed - readBuffer.Length;
			if (discardBytes > 0) 
			{
				// We are shifting bytes down in memory, so the fact that we are dealing
				// with overlapping regions shouldn't matter even if BlockCopy doesn't
				// take this into account.
				Buffer.BlockCopy(readBuffer, discardBytes, readBuffer, 0, readBufferBytesUsed - discardBytes);
				readBufferBytesUsed -= discardBytes;
			}
			Buffer.BlockCopy(rawData, rawDataOffset, readBuffer, readBufferBytesUsed, rawDataLength);
			readBufferBytesUsed += rawDataLength;

			// Look for complete packets.
			MemoryStream packetStream = new MemoryStream();
			int packetStart = -1;
			int bytesProcessed = 0;
			int i = 0;
			while (i < readBufferBytesUsed) 
			{
				if (readBuffer[i] == DLE) 
				{
					// DLE characters have to be followed by something to be interpreted
					if (i == readBufferBytesUsed - 1) 
					{
						break;
					} 
					else 
					{
						if (readBuffer[i + 1] == DLE) 
						{
							// Interpret as a single DLE.
							if (packetStart >= 0)
								packetStream.WriteByte(DLE);
							i += 2;
						} 
						else if (readBuffer[i + 1] == ETX) 
						{
							// End of packet.
							if (packetStart < 0) 
							{
								i += 2;
							} 
							else 
							{
								// Check the validity of the packet.
								if (packetStream.Length >= 3) 
								{
									// We have at least the packet ID, length and checksum.
									packetStream.Position = 1;
									if (packetStream.ReadByte() == packetStream.Length - 3) 
									{
										// Packet is the right size so process it. Incorrect
										// checksums are handled later.
										ProcessPacket(packetStream.ToArray());
										i += 2;
										bytesProcessed = i;
									}
									else
									{
										// Packet length is wrong, so go back and try again.
										i = packetStart + 1;
									}
								} 
								else 
								{
									// Stream is too short to represent a valid packet, so go back and try again.
									i = packetStart + 1;
								}
						
								// Whatever happened we start again.
								packetStart = -1;
								packetStream.SetLength(0);
							}
						} 
						else 
						{
							// Possible start of packet.
							if (packetStart < 0) 
							{
								// Start recording packet data. Skip the DLE.
								packetStart = i;
								i++;
							} 
							else 
							{
								// Oops. We already started so something must be wrong.
								// Go back and look for a different starting point.
								i = packetStart + 1;
								packetStart = -1;
								packetStream.SetLength(0);
							}
						}
					}
				} 
				else 
				{
					// Just a regular character. Record it if necessary.
					if (packetStart >= 0) 
						packetStream.WriteByte(readBuffer[i]);
					i++;
				}
			}

			// Preserve any unprocessed data for the next time around.
			if (bytesProcessed > 0) 
			{
				readBufferBytesUsed -= bytesProcessed;
				if (readBufferBytesUsed > 0)
					Buffer.BlockCopy(readBuffer, bytesProcessed, readBuffer, 0, readBufferBytesUsed); 
			}
		}

		public void NewGPSFix(IGPSFix gpsFix)
		{
			if (sendPVTData) 
			{
				// Calculate the fix time in Garmin format.
				const long gpsDelta = 627666624000000000;
				double gpsDays = (double)(gpsFix.UTCFixTime.Ticks - gpsDelta) / 864000000000; // Fractional days since 31/12/1989 00:00:00 UTC
				Int32 wn_days = (Int32)(gpsDays / 7) * 7;
				double tow = (gpsDays - wn_days) * 86400;

				// Determine the Garmin fix type.
				FixTypes fixType = FixTypes.Invalid;
				switch (gpsFix.FixType)
				{
					case GPSFixTypes.Fix2D:
						fixType = gpsFix.IsDifferential ? FixTypes.Diff2D : FixTypes.Fix2D;
						break;
					case GPSFixTypes.Fix3D:
						fixType = gpsFix.IsDifferential ? FixTypes.Diff3D : FixTypes.Fix3D;
						break;
				}

				// Build the PVT packet.
				pvtPacket = BuildPVTDataPacket((float)gpsFix.AltitudeAboveMSL + (float)gpsFix.MSLAltitudeAboveWGS84,
					(float)gpsFix.EPE, (float)gpsFix.EPH, (float)gpsFix.EPV, (Int16)fixType, tow,
					gpsFix.LatitudeRadians, gpsFix.LongitudeRadians,
					(float)gpsFix.SpeedEast, (float)gpsFix.SpeedNorth, (float)gpsFix.SpeedUp,
					-(float)gpsFix.MSLAltitudeAboveWGS84, 0, wn_days);

				// If we are idle, send the packet immediately. Otherwise save it for later.
				if (protocolState == ProtocolState.Idle) 
				{
					SendPacket(pvtPacket);
					pvtPacket = null;
				}
			}
		}

		public void NewSatelliteData(IGPSSatelliteVehicle[] satelliteVehicles)
		{
			// Build an send a satellite data packet if required.
			if (sendSatData)
			{
				// Build the packet.
				satDataPacket = BuildSatDataPacket(satelliteVehicles);

				// If we are idle, send the packet immediately. Otherwise save it for later.
				if (protocolState == ProtocolState.Idle) 
				{
					SendPacket(satDataPacket);
					satDataPacket = null;
				}
			}
		}

		private void ProcessPacket(byte[] packet)
		{
			// Extract the packet ID and data length.
			byte packetId = packet[0];
			byte dataLength = packet[1];

			// Verify the checksum. We have previously checked that packets are at least 3 bytes long.
			int i;
			byte checksum = 0;
			for (i = 0; i < packet.Length - 1; i++)
				checksum -= packet[i];
			if (checksum != packet[packet.Length - 1])
			{
				// Corrupt packet. NACK it.
				SendPacket(BuildNACKPacket(packetId));
			}
			else if (packetId == (byte)PacketId.ACK)
			{
				// ACK. We can only have an unacknowledged packet if we are not in the Idle state.
				if ((protocolState != ProtocolState.Idle) && (dataLength > 0) && (packet[2] == unacknowledgedPacket[1]))
				{
					// Advance the protocol state. This is where multi-part communications are continued.
					switch (protocolState) 
					{
						case ProtocolState.SendProductData:
							SendPacket(BuildProtocolArrayPacket(), ProtocolState.NeedAck);
							break;
						case ProtocolState.SendAlmanacStart:
							// We specified D550 as the data type, so we don't actually have to send any data.
							SendPacket(BuildTransferCompletePacket((Int16)CommandId.TRANSFER_ALM), ProtocolState.NeedAck);
							break;
						default:
							// No more steps, or not a multi-part communication.
							unacknowledgedPacket = null;
							protocolState = ProtocolState.Idle;
							break;
					}

					// If we are now idle, check if there is any PVT or satellite data
					// that needs to be sent. If we were idle when the PVT or satellite
					// data arrived, it would have already been sent.
					if ((protocolState == ProtocolState.Idle) && (pvtPacket != null))
					{
						SendPacket(pvtPacket);
						pvtPacket = null;
					}
					if ((protocolState == ProtocolState.Idle) && (satDataPacket != null))
					{
						SendPacket(satDataPacket);
						satDataPacket = null;
					}
				}
			}
			else if (packetId == (byte)PacketId.NACK)
			{
				// We only need to retransmit if we aren't in the Idle state.
				if ((protocolState != ProtocolState.Idle) && (dataLength > 0) && (packet[2] == unacknowledgedPacket[1]))
					SendPacket(unacknowledgedPacket);
			}
			else if (protocolState == ProtocolState.Idle)
			{
				// Can only accept new commands if we are idle. The exception to this would be the interruption of
				// a record transfer, but we don't send any records so we will never have this.
				if (packetId != (byte)PacketId.PRODUCT_RQST)
					SendPacket(BuildACKPacket(packetId));
				switch (packetId) 
				{
					case (byte)PacketId.PRODUCT_RQST:
						// This is the first packet of a conversation, so start from scratch.
						Reset();

						// Need to send the ACK packet here as the reset will have cleared the send buffer.
						SendPacket(BuildACKPacket(packetId));
						SendPacket(BuildProductDataPacket(ProductId, SoftwareVersion, ProductDescription), ProtocolState.SendProductData);
						break;
					case (byte)PacketId.UNLOCK_CODE:
						SendPacket(BuildUnlockResponsePacket(true), ProtocolState.NeedAck);
						break;
					/*
					case (byte)PacketId.ENABLE_ASYNC_EVENTS:
						if (dataLength > 0)
						{
							// Enable or disable async satellite data depending on
							// which bits are set.
							if (packet[2] == 0) 
								sendAsyncSatData = false;
							else if ((packet[2] & 0x80) > 0)
								sendAsyncSatData = true;
						}
						break;
					*/
					case (byte)PacketId.COMMAND_DATA:
						if (dataLength > 0) 
						{
							switch (packet[2]) 
							{
								case (byte)CommandId.TRANSFER_ALM:
									// We always send an empty almanac.
									SendPacket(BuildRecordsPacket(0), ProtocolState.SendAlmanacStart);
									break;
								case (byte)CommandId.TRANSFER_TIME:
									DateTime utc = DateTime.UtcNow;
									SendPacket(BuildDateTimePacket((byte)utc.Month, (byte)utc.Day, (UInt16)utc.Year,
										(byte)utc.Hour, (byte)utc.Minute, (byte)utc.Second), ProtocolState.NeedAck);
									break;
								case (byte)CommandId.TRANSFER_UNIT_ID:
									// The Que software doesn't send an ACK for this packet, so don't expect one.
									SendPacket(BuildUnitIdPacket(UnitId));
									//SendPacket(BuildUnitIdPacket(UnitId), ProtocolState.NeedAck);
									break;
								case (byte)CommandId.START_PVT_DATA:
									sendPVTData = true;
									sendSatData = true;
									break;
								case (byte)CommandId.STOP_PVT_DATA:
									sendPVTData = false;
									sendSatData = false;
									break;
								case (byte)CommandId.START_SAT_DATA:
								case (byte)CommandId.START_DIFF_SAT_DATA:
									sendSatData = true;
									break;
							}
						}
						break;
				}
			}
			else
			{
				// We are not idle, and the packet wasn't relevent to changing that. That probably means that
				// the received packet is out of sequence, but we simply use this as a trigger to check if a
				// retransmit is necessary. This is in the hopes that the other end is capable of recovering
				// from this situation, since we certainly aren't.
				DoRetransmissionCheck();
			}
		}

		private void DoRetransmissionCheck()
		{
			TimeSpan diff = DateTime.Now - lastSendTime;
			if ((protocolState != ProtocolState.Idle) && (diff.Seconds > SEND_TIMEOUT))
			{
				sendRetries++;
				if (sendRetries > MAX_SEND_RETRIES) 
					Reset();
				else
					SendPacket(unacknowledgedPacket);
			}
		}

		private void SendPacket(byte[] packet, ProtocolState newProtocolState)
		{
			protocolState = newProtocolState;
			unacknowledgedPacket = packet;
			SendPacket(packet);
		}

		private void SendPacket(byte[] packet)
		{
			lastSendTime = DateTime.Now;
			sendStream.Write(packet, 0, packet.Length);
		}

		private byte[] BuildPacketFromArray(byte packetId, object[] members)
		{
			// Construct the packet based on the types of the members.
			int i;
			MemoryStream packetContents = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(packetContents);
			writer.Write(packetId);
			writer.Write((byte)0);
			for (i = 0; i < members.Length; i++)
			{
				string memberTypeName = members[i].GetType().Name;
				switch (memberTypeName) 
				{
					case "Byte":
						writer.Write((byte)members[i]);
						break;
					case "Int16":
						writer.Write((Int16)members[i]);
						break;
					case "UInt16":
						writer.Write((UInt16)members[i]);
						break;
					case "Int32":
						writer.Write((Int32)members[i]);
						break;
					case "UInt32":
						writer.Write((UInt32)members[i]);
						break;
					case "Single":
						writer.Write((float)members[i]);
						break;
					case "Double":
						writer.Write((double)members[i]);
						break;
					case "String":
						Encoding ascii = Encoding.ASCII; 
						writer.Write(ascii.GetBytes((string)members[i]));
						writer.Write((byte)0);
						break;
					default:
						throw new System.Exception("Invalid Garmin Packet field type: " + memberTypeName);
				}
			}
			packetContents.Position = 1;
			packetContents.WriteByte((byte)(packetContents.Length - 2));

			// Calculate the checksum and append it.
			byte checksum = 0;
			packetContents.Position = 0;
			while (packetContents.Position < packetContents.Length)
				checksum -= (byte)packetContents.ReadByte();
			writer.Write(checksum);

			// Escape the packet and add a DLE prefix and DLE-ETX suffix.
			MemoryStream packet = new MemoryStream();
			packet.WriteByte(DLE);
			packetContents.Position = 0;
			while (packetContents.Position < packetContents.Length)
			{
				byte b = (byte)packetContents.ReadByte();
				packet.WriteByte(b);
				if (b == DLE)
					packet.WriteByte(b);
			}
			packet.WriteByte(DLE);
			packet.WriteByte(ETX);

			// Return the resulting array of bytes.
			return packet.ToArray();
		}

		private byte[] BuildPacket(byte packetId, params object[] members)
		{
			return BuildPacketFromArray(packetId, members);
		}

		private byte[] BuildACKPacket(byte packetId)
		{
			return BuildPacket((byte)PacketId.ACK, packetId, (byte)0);
		}

		private byte[] BuildNACKPacket(byte packetId)
		{
			return BuildPacket((byte)PacketId.NACK, packetId, (byte)0);
		}

		private byte[] BuildProductDataPacket(Int16 productId, Int16 softwareVersion, string productDescription) 
		{
			return BuildPacket((byte)PacketId.PRODUCT_DATA, productId, softwareVersion, productDescription);
		}

		private byte[] BuildProtocolArrayPacket() 
		{
			return BuildPacket((byte)PacketId.PROTOCOL_ARRAY,
				(byte)'P', (UInt16)0,
				(byte)'L', (UInt16)1,
				(byte)'A', (UInt16)10,
				(byte)'A', (UInt16)500,
				(byte)'D', (UInt16)550,
				(byte)'A', (UInt16)600,
				(byte)'D', (UInt16)600,
				(byte)'A', (UInt16)800,
				(byte)'D', (UInt16)800,
				(byte)'A', (UInt16)902);
		}

		private byte[] BuildUnitIdPacket(UInt32 unitId)
		{
			return BuildPacket((byte)PacketId.UNIT_ID, (UInt32)unitId);
		}

		private byte[] BuildUnlockResponsePacket(bool validUnlockCode)
		{
			return BuildPacket((byte)PacketId.UNLOCK_RESPONSE, (Int16)(validUnlockCode ? 1 : 0));
		}

		private byte[] BuildRecordsPacket(Int16 numRecords)
		{
			return BuildPacket((byte)PacketId.RECORDS, numRecords);
		}

		private byte[] BuildTransferCompletePacket(Int16 commandId)
		{
			return BuildPacket((byte)PacketId.XFER_COMPLT, commandId);
		}

		private byte[] BuildDateTimePacket(byte month, byte day, UInt16 year, Int16 hour, byte minute, byte second)
		{
			return BuildPacket((byte)PacketId.DATE_TIME_DATA, month, day, year, hour, minute, second);
		}

		private byte[] BuildPVTDataPacket(float alt, float epe, float eph, float epv, Int16 fix, double tow,
			double lat, double lon, float east, float north, float up, float msl_hght, Int16 leap_scnds, Int32 wn_days)
		{
			return BuildPacket((byte)PacketId.PVT_DATA, alt, epe, eph, epv, fix, tow, lat, lon, east, north, up,
				msl_hght, leap_scnds, wn_days);
		}

		private byte[] BuildSatDataPacket(IGPSSatelliteVehicle[] satelliteVehicles)
		{
			Array.Sort(satelliteVehicles);
			ArrayList members = new ArrayList();
			for (int i = 0; i < 12; i++)
			{
				if (i < satelliteVehicles.Length) 
				{
					IGPSSatelliteVehicle vehicle = satelliteVehicles[i];

					// Copy the satellite data across.
					byte status = vehicle.HaveEphemerisData ? satEphMask : (byte)0;
					status |= vehicle.HaveDifferentialCorrection ? satDifMask : (byte)0;
					status |= vehicle.UsedInFix ? satUsedMask : (byte)0;
					status |= vehicle.IsRising ? satRisingMask : (byte)0;

					// Adjust the SNR so that the supplied satellite SNR window maps
					// to the Garmin visible SNR range.
					int snr = (int)Math.Round((double)(vehicle.SNRdB - SatelliteSNRdBMin) /
						(SatelliteSNRdBMax - SatelliteSNRdBMin) *
						(GarminSNRRangeMax - GarminSNRRangeMin)) +
						GarminSNRRangeMin;
					snr = snr < 0 ? 0 : snr;

					members.Add((byte)vehicle.PRN);
					members.Add((UInt16)(snr * 100));
					members.Add((byte)vehicle.ElevationDegrees);
					members.Add((UInt16)vehicle.AziumthDegrees);
					members.Add((byte)status);
				} 
				else
				{
					// Use empty values.
					members.Add((byte)255);
					members.Add((UInt16)0);
					members.Add((byte)0);
					members.Add((UInt16)0);
					members.Add((byte)0);
				}
			}

			return BuildPacketFromArray((byte)PacketId.SAT_DATA, members.ToArray());
		}
	}
}
