/*
$Id: iQueM5API.cs,v 1.1 2006/05/23 09:27:09 andrew_klopper Exp $

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
using System.Threading;
using System.Runtime.InteropServices;
using GPSProxy.Extension;
using GPSProxy.Common;

namespace GPSProxy.Extension.iQueM5Input
{
	/// <summary>
	/// Summary description for iQueM5API.
	/// </summary>
	public class iQueM5API
	{
		private const byte gpsFixUnusable	= 0;
		private const byte gpsFixInvalid	= 1;
		private const byte gpsFix2D			= 2;
		private const byte gpsFix3D			= 3;
		private const byte gpsFix2DDiff		= 4;
		private const byte gpsFix3DDiff		= 5;

		private const byte gpsSatEphMask	= 1;
		private const byte gpsSatDifMask	= 2;
		private const byte gpsSatUsedMask	= 4;
		private const byte gpsSatRisingMask	= 8;

		[StructLayout(LayoutKind.Sequential)]
		private struct GPSSatDataType
		{
			public byte		svid;
			public byte		status;
			public short	snr;
			public float	azimuth;
			public float	elevation;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct GPSStatusDataType
		{
			public byte mode;
			public byte fix;
			public Int16 filler2;
			public float epe;
			public float eph;
			public float epv;
		};

		[StructLayout(LayoutKind.Sequential)]
		private struct GPSPositionDataType
		{
			public Int32 lat;
			public Int32 lon;
			public float altMSL;
			public float altWGS84;
		};

		[StructLayout(LayoutKind.Sequential)]
		private struct GPSVelocityDataType
		{
			public float east;
			public float north;
			public float up;
			public float track;
			public float speed;
		};

		[StructLayout(LayoutKind.Sequential)]
		private struct GPSTimeDataType
		{
			public UInt32 seconds;
			public UInt32 fracSeconds;
		};

		[StructLayout(LayoutKind.Sequential)]
		private struct GPSPVTDataType
		{
			public GPSStatusDataType status;
			public GPSPositionDataType position;
			public GPSVelocityDataType velocity;
			public GPSTimeDataType time;
		};

		// .NET Compact framework does not support callbacks.
		[DllImport("queapi.dll")]
		private static extern ushort QueAPIOpen(IntPtr callback);

		[DllImport("queapi.dll")]
		private static extern ushort QueAPIClose(IntPtr callback);

		[DllImport("queapi.dll")]
		private static extern ushort GPSGetPVT(out GPSPVTDataType pvt);

		[DllImport("queapi.dll")]
		private static extern byte GPSGetMaxSatellites();

		[DllImport("queapi.dll")]
		private static extern byte GPSGetSatellites(GPSSatDataType[] satData);

		private static Mutex openMutex = new Mutex();
		private static int openCount = 0;

		private bool isOpen = false;
		private byte maxSatellites;

		public iQueM5API()
		{
		}

		~iQueM5API()
		{
			Close();
		}

		public void Open()
		{
			if (! isOpen) 
			{
				if (openMutex.WaitOne())
				{
					try
					{
						if (openCount == 0)
						{
							ushort err = QueAPIOpen(IntPtr.Zero);
							if (err == 0)
								openCount = 1;
							else
								throw new Exception("Failed to open iQue M5 GPS library.");
						}
						else
						{
							openCount++;
						}
						isOpen = true;
						maxSatellites = GPSGetMaxSatellites();
					}
					finally
					{
						openMutex.ReleaseMutex();
					}
				}
				else
				{
					throw new Exception("Failed to lock M5 API open mutex.");
				}
			}
		}

		public void Close()
		{
			if (isOpen)
			{
				isOpen = false;
				if (openMutex.WaitOne())
				{
					try
					{
						if (openCount <= 0)
							throw new Exception("Reference count error in iQueM5API.");
						openCount--;
						if (openCount == 0)
							QueAPIClose(IntPtr.Zero);
					}
					finally
					{
						openMutex.ReleaseMutex();
					}
				}
				else
				{
					throw new Exception("Failed to lock M5 API open mutex.");
				}
			}
		}

		public IGPSFix GetGPSFix()
		{
			GPSFix fix = new GPSFix();
			GPSPVTDataType pvt;
			ushort err = GPSGetPVT(out pvt);
			if (err == 0)
			{
				// Convert the fix time.
				DateTime utc = DateTime.UtcNow;
				fix.UTCFixTime = new DateTime(utc.Year, utc.Month, utc.Day,
					(int)(pvt.time.seconds / 3600),
					(int)(pvt.time.seconds % 3600 / 60),
					(int)(pvt.time.seconds % 3600 % 60),
					(int)((double)pvt.time.fracSeconds / 4294967296 * 1000));

				// Determine the fix type.
				switch (pvt.status.fix)
				{
					case gpsFixInvalid:
					case gpsFixUnusable:
						fix.FixType = GPSFixTypes.Invalid;
						break;
					case gpsFix2D:
					case gpsFix2DDiff:
						fix.FixType = GPSFixTypes.Fix2D;
						fix.IsDifferential = pvt.status.fix == gpsFix2DDiff;
						break;
					case gpsFix3D:
					case gpsFix3DDiff:
						fix.FixType = GPSFixTypes.Fix3D;
						fix.IsDifferential = pvt.status.fix == gpsFix3DDiff;
						break;
				}

				// Populate the rest of the fields.
				fix.LatitudeRadians = pvt.position.lat * Math.PI / 2147483648;
				fix.LongitudeRadians = pvt.position.lon * Math.PI / 2147483648;
				fix.AltitudeAboveMSL = pvt.position.altMSL;
				fix.MSLAltitudeAboveWGS84 = pvt.position.altWGS84 - pvt.position.altMSL;
				fix.EPH = pvt.status.eph;
				fix.EPV = pvt.status.epv;
				fix.SpeedEast = pvt.velocity.east;
				fix.SpeedNorth = pvt.velocity.north;
				fix.SpeedUp = pvt.velocity.up;
				fix.HeadingRadians = Math.Atan2(fix.SpeedEast, fix.SpeedNorth);
			}
			else
			{
				fix.FixType = GPSFixTypes.Invalid;
			}
			return fix;
		}

		public IGPSSatelliteVehicle[] GetSatelliteData()
		{
			GPSSatDataType[] iQueSatData = new GPSSatDataType[maxSatellites];
			ushort err = GPSGetSatellites(iQueSatData);
			if (err == 0)
			{
				// Work out how many satellites are in view.
				int numSatellitesInView = 0;
				for (int i = 0; i < maxSatellites; i++)
					if (iQueSatData[i].svid != 255)
						numSatellitesInView++;

				// Construct the satellite data class.
				IGPSSatelliteVehicle[] satelliteVehicles = new IGPSSatelliteVehicle[numSatellitesInView];
				int index = 0;
				for (int i = 0; i < maxSatellites; i++)
				{
					if (iQueSatData[i].svid != 255)
					{
						GPSSatelliteVehicle vehicle = new GPSSatelliteVehicle();
						vehicle.PRN = iQueSatData[i].svid;
						vehicle.AziumthDegrees = (int)Math.Round(iQueSatData[i].azimuth * 180 / Math.PI);
						vehicle.ElevationDegrees = (int)Math.Round(iQueSatData[i].elevation * 180 / Math.PI);
						vehicle.SNRdB = iQueSatData[i].snr / 100;
						vehicle.HaveEphemerisData = (iQueSatData[i].status & gpsSatEphMask) > 0;
						vehicle.HaveDifferentialCorrection = (iQueSatData[i].status & gpsSatDifMask) > 0;
						vehicle.UsedInFix = (iQueSatData[i].status & gpsSatUsedMask) > 0;
						vehicle.IsRising = (iQueSatData[i].status & gpsSatRisingMask) > 0;
						satelliteVehicles[index++] = vehicle;
					}
				}

				// Return the result.
				return satelliteVehicles;
			}
			else
			{
				return null;
			}
		}
	}
}
