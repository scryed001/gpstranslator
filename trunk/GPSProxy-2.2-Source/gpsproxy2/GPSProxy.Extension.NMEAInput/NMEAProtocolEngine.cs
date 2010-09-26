/*
$Id: NMEAProtocolEngine.cs,v 1.1 2006/05/23 09:27:09 andrew_klopper Exp $

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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using GPSProxy.Extension;
using GPSProxy.Common;

namespace GPSProxy.Extension.NMEAInput
{
	/// <summary>
	/// Summary description for NMEAProtocolEngine.
	/// </summary>
	public class NMEAProtocolEngine
	{
		private enum NMEASentenceType {Unknown, GGA, RMC};

		private class GSAData 
		{
			public DateTime timeReceived;
			public double HDOP;
			public double VDOP;
			public double PDOP;
			public GPSFixTypes fixType;
			public int[] satellitesInFix;
		}

		private const int savedFixValiditySeconds = 10;
		private const int gsaDataValiditySeconds = 10;

		private double movementThreshold;

		private string readBuffer = "";
		private Regex timeRegex =
			// M5 sends time in the format 'hhmmss.x.y'. Don't know what the x and y stand for
			// (can't be fractions of a second) so we ignore anything after the first '.'.
			//new Regex(@"^(?<hour>[0-9]{2})(?<min>[0-9]{2})(?<sec>[0-9]{2}(?:\.[0-9]+))$");
			new Regex(@"^(?<hour>[0-9]{2})(?<min>[0-9]{2})(?<sec>[0-9]{2})(?:\.|$)");

		private GPSFix partialFix;
		private NMEASentenceType partialFixSentenceType = NMEASentenceType.Unknown;
		private bool haveGGA = false;
		private bool haveRMC = false;
		private DateTime lastGGATimestamp;
		private DateTime lastRMCTimestamp;
		private NMEASentenceType fixTriggerSentence = NMEASentenceType.Unknown;
		
		private int lastGSVMessageNumber = 0;

		private GSAData gsaData;
		
		private GPSFix savedFix;
		private GPSFix m_MostRecentGPSFix;

		private int partialSatelliteVehicleIndex;
		private GPSSatelliteVehicle[] partialSatelliteVehicles;
		private GPSSatelliteVehicle[] m_MostRecentSatelliteVehicles;
		
		public delegate void LogEvent(string logMessage);
		public event LogEvent OnLogEvent;

		private void CallOnLogEvent(string logMessage)
		{
			if (OnLogEvent != null) 
			{
				OnLogEvent(logMessage);
			}
		}

		public IGPSFix MostRecentGPSFix 
		{
			get
			{
				GPSFix ret = m_MostRecentGPSFix;
				m_MostRecentGPSFix = null;
				return ret;
			}
		}

		public IGPSSatelliteVehicle[] MostRecentSatelliteData
		{
			get
			{
				IGPSSatelliteVehicle[] ret = m_MostRecentSatelliteVehicles;
				m_MostRecentSatelliteVehicles = null;
				return ret;
			}
		}

		public NMEAProtocolEngine(double movementThreshold)
		{
			this.movementThreshold = movementThreshold;
		}

		public void Reset()
		{
			readBuffer = "";

			partialFix = null;
			partialFixSentenceType = NMEASentenceType.Unknown;
			haveGGA = false;
			haveRMC = false;
			lastGGATimestamp = new DateTime(0);
			lastRMCTimestamp = new DateTime(0);
			fixTriggerSentence = NMEASentenceType.Unknown;

			savedFix = null;
			m_MostRecentGPSFix = null;

			gsaData = null;

			partialSatelliteVehicles = null;
			partialSatelliteVehicleIndex = 0;
			m_MostRecentSatelliteVehicles = null;

			lastGSVMessageNumber = 0;
		}

		private DateTime ParseNMEATimestamp(string timestamp)
		{
			DateTime ret = DateTime.UtcNow;

			Match match = timeRegex.Match(timestamp);
			if (match.Success)
			{
				GroupCollection gc = match.Groups;
				double sec = 0;
				if (! ParseDouble(gc["sec"].Value, ref sec))
					throw new Exception("Invalid seconds value (should be impossible): " + gc["sec"].Value);
				int millisec = (int)((sec - (int)sec) * 1000);
				ret = new DateTime(ret.Year, ret.Month, ret.Day, int.Parse(gc["hour"].Value),
					int.Parse(gc["min"].Value), (int)sec, millisec);
			}

			return ret;
		}

		public void ProcessReceivedData(byte[] rawData)
		{
			const string hexChars = "0123456789ABCDEF";

			// Add the data to the read buffer.
			Encoding ascii = Encoding.ASCII; 
			readBuffer += ascii.GetString(rawData, 0, rawData.Length).ToUpper();

			// Process received lines. The last entry in the array (which may be an empty string)
			// is always an incomplete line as it wasn't followed by a "\n". Therefore it needs
			// to go back into the read buffer.
			string[] lines = readBuffer.Split(new char[] {'\n'});
			readBuffer = lines[lines.Length - 1];
			int i;
			for (i = 0; i < lines.Length - 1; i++)
			{
				// Split the checksum off from the line. If we don't find one, ignore the line.
				int hi, lo;
				string[] elements = lines[i].Trim().Split(new char[] {'*'});
				if ((elements.Length == 2) && elements[0].StartsWith("$") && (elements[1].Length == 2) &&
					((hi = hexChars.IndexOf(elements[1][0])) >= 0) &&
					((lo = hexChars.IndexOf(elements[1][1])) >= 0))
				{
					// Verify the checksum.
					byte checksum = (byte)(hi << 4 | lo);
					byte c = 0;
					int j;
					for (j = 1; j < elements[0].Length; j++)
						c ^= (byte)elements[0][j];
					if (c == checksum) 
					{
						// Valid NMEA sentence. Split it into its components and process it after
						// removing the leading '$'. There will always be at least one part, even
						// if it is empty.
						string[] parts = elements[0].Remove(0, 1).Split(new char[] {','});

						// Decide whether to use GGA or RMC sentences, or both.
						if (fixTriggerSentence == NMEASentenceType.Unknown) 
						{
							if (parts[0] == "GPGGA") 
							{
								haveGGA = true;
								if (! haveRMC && (lastGGATimestamp.Ticks != 0)) 
								{
									// This is the second GGA fix we have seen without an intervening RMC fix.
									// Assume no RMC packets.
									fixTriggerSentence = NMEASentenceType.GGA;
								}
								lastGGATimestamp = ParseNMEATimestamp(parts.Length > 0 ? parts[1] : "");
								if (haveRMC)
								{
									if (lastRMCTimestamp == lastGGATimestamp)
										fixTriggerSentence = NMEASentenceType.GGA;
									else
										fixTriggerSentence = NMEASentenceType.RMC;
								}
							}
							else if (parts[0] == "GPRMC")
							{
								haveRMC = true;
								if (! haveGGA && (lastRMCTimestamp.Ticks != 0)) 
								{
									// This is the second RMC fix we have seen without an intervening GGA fix.
									// Assume no GGA packets.
									fixTriggerSentence = NMEASentenceType.RMC;
								}
								lastRMCTimestamp = ParseNMEATimestamp(parts.Length > 0 ? parts[1] : "");
								if (haveGGA)
								{
									if (lastGGATimestamp == lastRMCTimestamp)
										fixTriggerSentence = NMEASentenceType.RMC;
									else
										fixTriggerSentence = NMEASentenceType.GGA;
								}
							}
						}

						// Process specific sentence types.
						switch (parts[0])
						{
							case "GPGGA":
								ProcessGGA(parts);
								break;

							case "GPGSA":
								ProcessGSA(parts);
								break;

							case "GPGSV":
								ProcessGSV(parts);
								break;

							case "GPRMC":
								ProcessRMC(parts);
								break;
						}
					}
				}
			}
		}

		private void ProcessGGA(string[] parts)
		{
			// Make sure we have the right number of parts.
			if (parts.Length != 15)
			{
				CallOnLogEvent("Invalid GGA sentence.");
				return;
			}

			// Create a new fix object.
			GPSFix fix = new GPSFix();

			// Determine the fix type from this sentence. We can't reliably uses the
			// presence of an altitude to distinguish between 2D and 3D fixes,
			// so assume that all fixes are 2D in the absence of GSA information.
			fix.FixType = (parts[6].Equals("1") || parts[6].Equals("2")) ?
				GPSFixTypes.Fix2D : GPSFixTypes.Invalid;

			// Extract the HDOP, leaving it set to zero if this fails.
			double hdop = 0;
			if (! parts[8].Equals(String.Empty)) 
				ParseDouble(parts[8], ref hdop);
			fix.HDOP = hdop;

			// Determine if we have a differential fix.
			fix.IsDifferential = parts[6].Equals("2");

			// Extract the UTC time, if present.
			fix.UTCFixTime = ParseNMEATimestamp(parts[1]);
			
			// Extract the latitude, converting the minutes portion to a fraction of a degree.
			if (parts[2].Equals(String.Empty)) 
			{
				// Invalidate the fix.
				fix.FixType = GPSFixTypes.Invalid;
			}
			else 
			{
				double temp = 0;
				if (! ParseDouble(parts[2], ref temp))
					throw new Exception("Invalid latitude: " + parts[2]);
				temp /= 100;
				double lat = (int)temp;
				lat = (lat + (temp - lat) / 0.6) * (parts[3].Equals("N") ? 1 : -1);
				fix.LatitudeDegrees = lat;
			}

			// Extract the longitude.
			if (parts[4].Equals(String.Empty)) 
			{
				// Invalidate the fix.
				fix.FixType = GPSFixTypes.Invalid;
			}
			else
			{
				double temp = 0;
				if (! ParseDouble(parts[4], ref temp))
					throw new Exception("Invalid longitude: " + parts[4]);
				temp /= 100;
				double lon = (int)temp;
				lon = (lon + (temp - lon) / 0.6) * (parts[5].Equals("E") ? 1 : -1);
				fix.LongitudeDegrees = lon;
			}

			// Check if we have an altitude value.
			if (! parts[9].Equals(String.Empty))
			{
				// Parse the altitude.
				double alt = 0;
				if (! ParseDouble(parts[9], ref alt))
					throw new Exception("Invalid altitude: " + parts[9]);
				switch (parts[10]) 
				{
					case "M":
						break;
					case "f":
						alt *= 0.3048;
						break;
					default:
						throw new Exception("Invalid unit of measurement: " + parts[10]);
				}
				fix.AltitudeAboveMSL = alt;

				// Extract the MSL height above WGS84. If not present, implies previous altitude
				// is relative to WGS84 and not MSL (for Garmin receivers, anyway).
				double mslAltitude = 0;
				if (parts[11].Equals(String.Empty))
				{
					// TODO: calculate correct MSLAltitudeAboveWGS84
					mslAltitude = 0;

					// Convert from Altitude above WGS84 to Altitude above MSL;
					fix.AltitudeAboveMSL -= mslAltitude;
				}
				else
				{
					if (! ParseDouble(parts[11], ref mslAltitude))
						throw new Exception("Invalid MSL altitude: " + parts[11]);
					switch (parts[12]) 
					{
						case "M":
							break;
						case "f":
							mslAltitude *= 0.3048;
							break;
						default:
							throw new Exception("Invalid unit of measurement: " + parts[12]);
					}
				}
				fix.MSLAltitudeAboveWGS84 = mslAltitude;
			}

			// Update the fix based on the contents of the last GSA sentence, if any.
			if ((gsaData != null) && (DateTime.Now.Subtract(gsaData.timeReceived).TotalSeconds < gsaDataValiditySeconds))
			{
				// Bump the fix type up to 3D if the GSA fix was 3D.
				if ((fix.FixType == GPSFixTypes.Fix2D) && (gsaData.fixType == GPSFixTypes.Fix3D))
					fix.FixType = GPSFixTypes.Fix3D;

				// Add the VDOP information.
				fix.VDOP = gsaData.VDOP;
			}

			// Publish the fix.
			PublishFix(fix, NMEASentenceType.GGA);
		}

		private void ProcessGSA(string[] parts)
		{
			// Check that we have the correct number of parts.
			if (parts.Length != 18)
			{
				CallOnLogEvent("Invalid GSA sentence.");
				return;
			}

			// Record the information from the packet in a new object.
			GSAData newGsaData = new GSAData();
			newGsaData.timeReceived = DateTime.Now;

			// Extract the PDOP leaving it set to 0 if this fails.
			if (! parts[15].Equals(String.Empty))
			{
				double temp = 0;
				if (ParseDouble(parts[15], ref temp))
					newGsaData.PDOP = temp;
			}

			// Extract the HDOP leaving it set to 0 if this fails.
			if (! parts[16].Equals(String.Empty)) 
			{
				double temp = 0;
				if (ParseDouble(parts[16], ref temp))
					newGsaData.HDOP = temp;
			}

			// Extract the VDOP leaving it set to 0 if this fails.
			if (! parts[17].Equals(String.Empty))
			{
				double temp = 0;
				if (ParseDouble(parts[17], ref temp))
					newGsaData.VDOP = temp;
			}

			// Determine the fix type.
			newGsaData.fixType = GPSFixTypes.Invalid;
			switch (parts[2]) 
			{
				case "2":
					newGsaData.fixType = GPSFixTypes.Fix2D;
					break;
				case "3":
					newGsaData.fixType = GPSFixTypes.Fix3D;
					break;
			}

			// Count the number of satellites in the fix.
			int numSatellitesInFix = 0;
			for (int i = 3; i <= 14; i++) 
			{
				if (! parts[i].Equals(String.Empty))
					numSatellitesInFix++;
			}

			// If there were any satellites, record the vehicle IDs.
			newGsaData.satellitesInFix = new int[numSatellitesInFix];
			if (numSatellitesInFix > 0) 
			{
				int offset = 0;
				for (int i = 3; i <= 14; i++) 
				{
					if (! parts[i].Equals(String.Empty)) 
					{
						try 
						{
							newGsaData.satellitesInFix[offset++] = int.Parse(parts[i]);
						}
						catch
						{
							throw new Exception("Invalid satellite vehicle ID: " + parts[i]);
						}
					}
				}
			}
 
			// Make the data available.
			gsaData = newGsaData;
		}

		private void ProcessGSV(string[] parts)
		{
			// Check that we have the correct number of parts.
			int numMessages, messageNumber, numSatellitesInView, numSatellitesInSentence;
			if (parts.Length < 4) 
			{
				CallOnLogEvent("Invalid GSV sentence.");
				return;
			}
			try
			{
				numMessages = int.Parse(parts[1]);
				messageNumber = int.Parse(parts[2]);
				numSatellitesInView = int.Parse(parts[3]);
				numSatellitesInSentence = messageNumber < numMessages ?
					4 : numSatellitesInView - 4 * (numMessages - 1);
			}
			catch
			{
				CallOnLogEvent("Invalid GSV sentence.");
				return;
			}

			// Need to allow for cases where the exact number of satellites are present,
			// and where placeholder commas are used to pad missing satellite data in the
			// last sentence.
			if ((parts.Length != numSatellitesInSentence * 4 + 4) && (parts.Length != 20))
			{
				CallOnLogEvent("Invalid GSV sentence.");
				return;
			}

			// Create a new satellite data instance if this is the first record.
			if (messageNumber == 1) 
			{
				partialSatelliteVehicles = new GPSSatelliteVehicle[numSatellitesInView];
				partialSatelliteVehicleIndex = 0;
			}

			// Check that the message isn't out of sequence.
			if (messageNumber != lastGSVMessageNumber + 1) 
			{
				partialSatelliteVehicles = null;
				partialSatelliteVehicleIndex = 0;
			}
			if (messageNumber == numMessages)
				lastGSVMessageNumber = 0;
			else
				lastGSVMessageNumber = messageNumber;

			// Sanity check.
			if ((partialSatelliteVehicles != null) &&
				(numSatellitesInView != partialSatelliteVehicles.Length))
			{
				partialSatelliteVehicles = null;
				partialSatelliteVehicleIndex = 0;
			}

			// Check if we have a satellite data instance in case we didn't see the
			// first record or we invalidated the satellite data above.
			if (partialSatelliteVehicles != null)
			{
				// Add the satellite data to the list, if there is space.
				for (int i = 0; i < numSatellitesInSentence; i++)
				{
					// Sanity check.
					if (partialSatelliteVehicleIndex >= numSatellitesInView)
						throw new Exception("Too many satellite vehicles in GSV sentence.");

					// Create a new vehicle entry.
					GPSSatelliteVehicle vehicle = new GPSSatelliteVehicle();
					partialSatelliteVehicles[partialSatelliteVehicleIndex++] = vehicle;

					// Populate the entry.
					try
					{
						int partOffset = i * 4 + 4;
						vehicle.PRN = int.Parse(parts[partOffset]);
						vehicle.ElevationDegrees = parts[partOffset + 1].Equals(String.Empty) ? 0 : int.Parse(parts[partOffset + 1]);
						vehicle.AziumthDegrees = parts[partOffset + 2].Equals(String.Empty) ? 0 : int.Parse(parts[partOffset + 2]);
						vehicle.SNRdB = parts[partOffset + 3].Equals(String.Empty) ? 0 : int.Parse(parts[partOffset + 3]);
						vehicle.HaveEphemerisData = true; // Assumed.
					}
					catch
					{
						throw new Exception("Invalid satellite vehicle data in GSV sentence.");
					}
				}

				// If this is the last message then publish the data.
				if (messageNumber == numMessages) 
				{
					// Flag which satellites are used in the fix if this information is
					// available.
					if ((gsaData != null) && (DateTime.Now.Subtract(gsaData.timeReceived).TotalSeconds < gsaDataValiditySeconds))
					{
						for (int i = 0; i < gsaData.satellitesInFix.Length; i++)
						{
							for (int j = 0; j < partialSatelliteVehicleIndex; j++)
							{
								if (gsaData.satellitesInFix[i] == partialSatelliteVehicles[j].PRN)
								{
									partialSatelliteVehicles[j].UsedInFix = true;
									break;
								}
							}
						}
					}

					// Sort the vehicles in order of PRN.
					Array.Sort(partialSatelliteVehicles);

					// Publish the data.
					m_MostRecentSatelliteVehicles = partialSatelliteVehicles;
				
					// Tidy up.
					partialSatelliteVehicles = null;
					partialSatelliteVehicleIndex = 0;
				}
			}
		}

		private void ProcessRMC(string[] parts)
		{
			// Make sure we have the right number of parts.
			if (parts.Length < 6) 
			{
				CallOnLogEvent("Invalid RMC sentence.");
				return;
			}

			// Create a new fix object.
			GPSFix fix = new GPSFix();

			// Assume a 2D fix type unless the fix is invalid.
			fix.FixType = parts[2].Equals("A") ? GPSFixTypes.Fix2D : GPSFixTypes.Invalid;

			// Extract the UTC time, if present.
			fix.UTCFixTime = ParseNMEATimestamp(parts[1]);
			
			// Extract the latitude, converting the minutes portion to a fraction of a degree.
			if (parts[3].Equals(String.Empty)) 
			{
				// Invalidate the fix.
				fix.FixType = GPSFixTypes.Invalid;
			}
			else 
			{
				double temp = 0;
				if (! ParseDouble(parts[3], ref temp))
					throw new Exception("Invalid latitude: " + parts[3]);
				temp /= 100;
				double lat = (int)temp;
				lat = (lat + (temp - lat) / 0.6) * (parts[4].Equals("N") ? 1 : -1);
				fix.LatitudeDegrees = lat;
			}

			// Extract the longitude.
			if (parts[5].Equals(String.Empty)) 
			{
				// Invalidate the fix.
				fix.FixType = GPSFixTypes.Invalid;
			}
			else
			{
				double temp = 0;
				if (! ParseDouble(parts[5], ref temp))
					throw new Exception("Invalid longitude: " + parts[5]);
				temp /= 100;
				double lon = (int)temp;
				lon = (lon + (temp - lon) / 0.6) * (parts[6].Equals("E") ? 1 : -1);
				fix.LongitudeDegrees = lon;
			}

			// Extract the speed and heading. If parsing fails, the values will be left as 0.
			double speed = 0, track = 0;
			ParseDouble(parts[7], ref speed);
			ParseDouble(parts[8], ref track);
			speed *= 1852.0 / 3600;
			fix.HeadingDegrees = track;
			fix.SpeedNorth = speed * Math.Cos(fix.HeadingRadians);
			fix.SpeedEast = speed * Math.Sin(fix.HeadingRadians);

			// Fake an EPE.
			fix.EPH = 5;

			// Publish the fix.
			PublishFix(fix, NMEASentenceType.RMC);
		}

		private void CalculateSpeedAndHeading(GPSFix fix, bool verticalOnly)
		{
			// Only calculate speed between valid fixes, and only save valid fixes for next time. This
			// allows the occassinal invalid fix in between valid fixes without disrupting the speed
			// calculations for the valid fixes (the last valid fix will hang around for a while).
			if (fix.FixType != GPSFixTypes.Invalid) 
			{
				double saveAltitude = fix.AltitudeAboveMSL;
				if (savedFix != null)
				{
					double seconds = fix.UTCFixTime.Subtract(savedFix.UTCFixTime).TotalSeconds;
					if ((seconds > 0) && (seconds <= savedFixValiditySeconds)) 
					{
						// Only calculate speed and heading if the total position change,
						// which may occur across multiple fixes, is greater than a
						// predefined threshold. We obtain the total position change by
						// not updating the position in the saved fix until the threshold
						// has been reached, at which point we adjust the saved fix to
						// the new position. Since we report speeds of 0 until the
						// threshold has been reached, we always calculate the speed from
						// the saved position from the time of the last fix, not the
						// original time of the saved fix, so that a speed * time
						// calculation will return the correct distance for this position
						// change.
						if (verticalOnly) 
						{
							// Calculate the vertical speed based on the elevation and time differences, but only if
							// both fixes are 3D.
							if ((fix.FixType == GPSFixTypes.Fix3D) && (savedFix.FixType == GPSFixTypes.Fix3D))
							{
								if (Math.Abs(fix.AltitudeAboveMSL - savedFix.AltitudeAboveMSL) <=
									movementThreshold)
								{
									// Change in altitude is insignificant, so set vertical speed to zero and ensure that
									// the altitude of the saved fix is preserved below.
									fix.SpeedUp = 0;
									saveAltitude = savedFix.AltitudeAboveMSL;
								}
								else
								{
									fix.SpeedUp = (fix.AltitudeAboveMSL - savedFix.AltitudeAboveMSL) / seconds;
								}
							}
							else
							{
								fix.SpeedUp = 0;
							}

							// Always update the saved fix.
							savedFix = null;
						}
						else
						{
							double distance;
							SetSpeedAndHeading(fix, savedFix, out distance);
							if (Math.Abs(fix.AltitudeAboveMSL - savedFix.AltitudeAboveMSL) <=
								movementThreshold)
							{
								// Change in altitude is insignificant, so set vertical speed to zero and ensure that
								// the altitude of the saved fix is preserved below.
								fix.SpeedUp = 0;
								saveAltitude = savedFix.AltitudeAboveMSL;
							}
							if (distance > movementThreshold)
							{
								// We have moved enough for a reliable speed calculation, so save the current fix
								// for next time.
								savedFix = null;
							}
							else
							{
								// Haven't moved enough to exclude the effects of position error, so set all
								// speeds to zero and update the time of the saved fix so that we can test against
								// the same saved position next time around.
								fix.SpeedEast = 0;
								fix.SpeedNorth = 0;
								fix.HeadingRadians = savedFix.HeadingRadians;
								savedFix.UTCFixTime = fix.UTCFixTime;
							}
						}
					}
					else
					{
						// Too long in between fixes, so invalidate the saved fix.
						savedFix = null;
					}
				}

				// Only save the current fix if we haven't updated the existing saved fix.
				if (savedFix == null)
					savedFix = new GPSFix(fix);
				savedFix.AltitudeAboveMSL = saveAltitude;
			}
		}

		private void PublishFix(GPSFix fix, NMEASentenceType fixSource)
		{
			if (fixSource == fixTriggerSentence) 
			{
				if (haveGGA && haveRMC) 
				{
					// Output a combined fix if the partial fix is valid.
					if ((partialFix != null) && (partialFix.UTCFixTime == fix.UTCFixTime) &&
						(fixSource != partialFixSentenceType))
					{
						GPSFix fixRMC = fixSource == NMEASentenceType.RMC ? fix : partialFix;
						GPSFix fixGGA = fixSource == NMEASentenceType.GGA ? fix : partialFix;

						m_MostRecentGPSFix = fixGGA;
						m_MostRecentGPSFix.HeadingRadians = fixRMC.HeadingRadians;
						m_MostRecentGPSFix.SpeedEast = fixRMC.SpeedEast;
						m_MostRecentGPSFix.SpeedNorth = fixRMC.SpeedNorth;

						// Still need to calculate vertical speed. Sigh.
						CalculateSpeedAndHeading(m_MostRecentGPSFix, true);
					}
					partialFix = null;
					partialFixSentenceType = NMEASentenceType.Unknown;
				}
				else if (fixSource == NMEASentenceType.GGA)
				{
					// Don't have speed information, so calculate it.
					m_MostRecentGPSFix = fix;
					CalculateSpeedAndHeading(m_MostRecentGPSFix, false);
				}
				else if (fixSource == NMEASentenceType.RMC) 
				{
					// Publish the fix as is.
					m_MostRecentGPSFix = fix;
				}
			}
			else
			{
				// Save the fix until we hit the trigger sentence.
				partialFix = fix;
				partialFixSentenceType = fixSource;
			}
		}

		private bool ParseDouble(string str, ref double val)
		{
			// Avoid exceptions having to be raised and caught for obvious cases.
			if (str == "")
				return false;
			try
			{
				// val will remain unchanged if an exception is thrown in the Parse method.
				val = double.Parse(str, System.Globalization.CultureInfo.InvariantCulture);
				return true;
			}
			catch
			{
				return false;
			}
		}

		// <summary>
		// Calculates the speed and heading of the specified current fix based on the distance and bearing to the
		// specified origin point, and the time taken to travel from the origin point to here. Elevation is not
		// taken into account in the geodesic distance calculation (it is assumed that elevation does not
		// significantly affect geodesic distance calculations, which may or may not be true), but elevation
		// differens are taken into account when calculating vertical speed. This probably needs looking at
		// in the future.
		// </summary>
		private void SetSpeedAndHeading(GPSFix current, GPSFix origin, out double distance)
		{
			// Check that we are dealing with valid fixes, and that the time difference between this fix and
			// the origin fix is positive.
			if ((origin.FixType == GPSFixTypes.Invalid) || (current.FixType == GPSFixTypes.Invalid))
				throw new Exception("Invalid current or origin fix for speed calculation.");
			if (current.UTCFixTime.CompareTo(origin.UTCFixTime) <= 0)
				throw new Exception("Negative or zero time difference from origin to current fix for speed calculation.");

			// Calculate the distance and bearing from the current point to the origin point.
			double bearing;
			GeoidUtils.GetDistanceAndBearing(current.LatitudeRadians, current.LongitudeRadians,
				origin.LatitudeRadians, origin.LongitudeRadians, out distance, out bearing);

			// Rotate the bearing 180 degress to get the heading.
			current.HeadingRadians = bearing >= 0 ? bearing - Math.PI : bearing + Math.PI;

			// Calculate the north and east components of the speed based on the distance, time difference
			// and heading.
			double seconds = current.UTCFixTime.Subtract(origin.UTCFixTime).TotalSeconds;
			double groundSpeed = distance / seconds;
			current.SpeedNorth = groundSpeed * Math.Cos(current.HeadingRadians);
			current.SpeedEast = groundSpeed * Math.Sin(current.HeadingRadians);

			// Calculate the vertical speed based on the elevation and time differences, but only if
			// both fixes are 3D.
			if ((current.FixType == GPSFixTypes.Fix3D) && (origin.FixType == GPSFixTypes.Fix3D))
				current.SpeedUp = (current.AltitudeAboveMSL - origin.AltitudeAboveMSL) / seconds;
			else
				current.SpeedUp = 0;
		}
	}
}