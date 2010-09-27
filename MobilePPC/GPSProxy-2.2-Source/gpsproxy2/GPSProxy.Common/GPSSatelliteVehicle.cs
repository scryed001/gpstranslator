/*
$Id: GPSSatelliteVehicle.cs,v 1.1 2006/05/23 09:27:06 andrew_klopper Exp $

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
	public class GPSSatelliteVehicle: IGPSSatelliteVehicle, IComparable
	{
		private int m_PRN;
		private int m_ElevationDegrees;
		private int m_AzimuthDegrees;
		private int m_SNRdB;
		private bool m_HaveEphemerisData;
		private bool m_HaveDifferentialCorrection;
		private bool m_UsedInFix;
		private bool m_IsRising;

		public int PRN
		{
			get
			{
				return m_PRN;
			}
			set
			{
				m_PRN = value;
			}
		}

		public int ElevationDegrees
		{
			get
			{
				return m_ElevationDegrees;
			}
			set
			{
				m_ElevationDegrees = value;
			}
		}

		public int AziumthDegrees
		{
			get
			{
				return m_AzimuthDegrees;
			}
			set
			{
				m_AzimuthDegrees = value;
			}
		}

		public int SNRdB
		{
			get
			{
				return m_SNRdB;
			}
			set
			{
				m_SNRdB = value;
			}
		}

		public bool HaveEphemerisData
		{
			get
			{
				return m_HaveEphemerisData;
			}
			set
			{
				m_HaveEphemerisData = value;
			}
		}

		public bool HaveDifferentialCorrection
		{
			get
			{
				return m_HaveDifferentialCorrection;
			}
			set
			{
				m_HaveDifferentialCorrection = value;
			}
		}

		public bool UsedInFix
		{
			get
			{
				return m_UsedInFix;
			}
			set
			{
				m_UsedInFix = value;
			}
		}

		public bool IsRising
		{
			get
			{
				return m_IsRising;
			}
			set
			{
				m_IsRising = value;
			}
		}

		public GPSSatelliteVehicle()
		{
		}

		public GPSSatelliteVehicle(GPSSatelliteVehicle vehicle)
		{
			m_PRN = vehicle.m_PRN;
			m_ElevationDegrees = vehicle.m_ElevationDegrees;
			m_AzimuthDegrees = vehicle.m_AzimuthDegrees;
			m_SNRdB = vehicle.m_SNRdB;
			m_HaveEphemerisData = vehicle.m_HaveEphemerisData;
			m_HaveDifferentialCorrection = vehicle.m_HaveDifferentialCorrection;
			m_UsedInFix = vehicle.m_UsedInFix;
			m_IsRising = vehicle.m_IsRising;
		}

		#region IComparable Members

		public int CompareTo(object obj)
		{
			GPSSatelliteVehicle other = (GPSSatelliteVehicle)obj;
			return m_PRN.CompareTo(other.m_PRN);
		}

		#endregion
	}
}
