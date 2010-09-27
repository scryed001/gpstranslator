/*
$Id: GPSFix.cs,v 1.1 2006/05/23 09:27:06 andrew_klopper Exp $

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
using GPSProxy.Extension;

namespace GPSProxy.Common
{
	/// <summary>
	/// Summary description for GPSFix.
	/// </summary>
	public class GPSFix : IGPSFix
	{
		public const double DefaultDOPMultiplier = 8;
		public static double dopMultiplier = DefaultDOPMultiplier;

		private bool posChanged = true;
		private double ecef_x;
		private double ecef_y;
		private double ecef_z;

		private double m_LatitudeRadians;
		private double m_LongitudeRadians;
		private double m_AltitudeAboveMSL;
		private double m_MSLAltitudeAboveWGS84;
		private double m_HeadingRadians;

		private double m_HDOP = -1;
		private double m_VDOP = -1;
		private double m_EPH = -1;
		private double m_EPV = -1;

		private GPSFixTypes m_FixType = GPSFixTypes.Invalid;
		private bool m_IsDifferential = false;
		private DateTime m_UTCFixTime;
		private double m_SpeedEast;
		private double m_SpeedNorth;
		private double m_SpeedUp;

		public GPSFix()
		{
			UTCFixTime = DateTime.UtcNow;
		}

		public GPSFix(GPSFix fix)
		{
			posChanged = fix.posChanged;

			ecef_x = fix.ecef_x;
			ecef_y = fix.ecef_y;
			ecef_z = fix.ecef_z;

			m_LatitudeRadians = fix.m_LatitudeRadians;
			m_LongitudeRadians = fix.m_LongitudeRadians;
			m_AltitudeAboveMSL = fix.m_AltitudeAboveMSL;
			m_MSLAltitudeAboveWGS84 = fix.m_MSLAltitudeAboveWGS84;
			m_HeadingRadians = fix.m_HeadingRadians;

			m_HDOP = fix.m_HDOP;
			m_VDOP = fix.m_VDOP;
			m_EPH = fix.m_EPH;
			m_EPV = fix.m_EPV;
		
			FixType = fix.FixType;
			IsDifferential = fix.IsDifferential;
			UTCFixTime = fix.UTCFixTime;
			SpeedEast = fix.SpeedEast;
			SpeedNorth = fix.SpeedNorth;
			SpeedUp = fix.SpeedUp;
		}

		public GPSFixTypes FixType
		{
			get
			{
				return m_FixType;
			}
			set
			{
				m_FixType = value;
			}
		}

		public bool IsDifferential
		{
			get
			{
				return m_IsDifferential;
			}
			set
			{
				m_IsDifferential = value;
			}
		}

		public DateTime UTCFixTime
		{
			get
			{
				return m_UTCFixTime;
			}
			set
			{
				m_UTCFixTime = value;
			}
		}

		public double SpeedEast
		{
			get
			{
				return m_SpeedEast;
			}
			set
			{
				m_SpeedEast = value;
			}
		}

		public double SpeedNorth
		{
			get
			{
				return m_SpeedNorth;
			}
			set
			{
				m_SpeedNorth = value;
			}
		}

		public double SpeedUp
		{
			get
			{
				return m_SpeedUp;
			}
			set
			{
				m_SpeedUp = value;
			}
		}

		public double LatitudeRadians
		{
			get
			{
				return m_LatitudeRadians;
			}
			set
			{
				posChanged = true;
				m_LatitudeRadians = value;
			}
		}

		public double LongitudeRadians
		{
			get
			{
				return m_LongitudeRadians;
			}
			set
			{
				posChanged = true;
				m_LongitudeRadians = value;
			}
		}

		public double LatitudeDegrees
		{
			get
			{
				return LatitudeRadians * 180 / Math.PI;
			}
			set
			{
				LatitudeRadians = value / 180 * Math.PI;
			}
		}

		public double LongitudeDegrees
		{
			get
			{
				return LongitudeRadians * 180 / Math.PI;
			}
			set
			{
				LongitudeRadians = value / 180 * Math.PI;
			}
		}

		public double HeadingRadians
		{
			get
			{
				return m_HeadingRadians;
			}
			set
			{
				m_HeadingRadians = value;
			}
		}

		public double HeadingDegrees
		{
			get
			{
				return m_HeadingRadians * 180 / Math.PI;
			}
			set
			{
				HeadingRadians = value / 180 * Math.PI;
			}
		}

		public double AltitudeAboveMSL
		{
			get
			{
				return m_AltitudeAboveMSL;
			}
			set
			{
				posChanged = true;
				m_AltitudeAboveMSL = value;
			}
		}

		public double MSLAltitudeAboveWGS84
		{
			get
			{
				return m_MSLAltitudeAboveWGS84;
			}
			set
			{
				posChanged = true;
				m_MSLAltitudeAboveWGS84 = value;
			}
		}

		public double PDOP 
		{
			get
			{
				return Math.Sqrt(HDOP * HDOP + VDOP * VDOP);
			}
		}

		public double HDOP 
		{
			get
			{
				return m_HDOP >= 0 ? m_HDOP : (m_EPH >= 0 ? m_EPH / dopMultiplier : 0);
			}
			set
			{
				m_HDOP = value;
			}
		}

		public double VDOP 
		{
			get
			{
				return m_VDOP >= 0 ? m_VDOP : (m_EPV >= 0 ? m_EPV / dopMultiplier : 0);
			}
			set
			{
				m_VDOP = value;
			}
		}

		public double EPE 
		{
			get
			{
				return Math.Sqrt(EPH * EPH + EPV * EPV);
			}
		}

		public double EPH
		{
			get
			{
				return m_EPH >= 0 ? m_EPH : (m_HDOP >= 0 ? m_HDOP * dopMultiplier : 0);
			}
			set
			{
				m_EPH = value;
			}
		}

		public double EPV
		{
			get
			{
				return m_EPV >= 0 ? m_EPV : (m_VDOP >= 0 ? m_VDOP * dopMultiplier : 0);
			}
			set
			{
				m_EPV = value;
			}
		}
	}
}
