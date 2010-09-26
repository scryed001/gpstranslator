/*
$Id: IGPSSatelliteVehicle.cs,v 1.1 2006/05/23 09:27:07 andrew_klopper Exp $

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

namespace GPSProxy.Extension
{
	public interface IGPSSatelliteVehicle
	{
		int PRN { get; }
		int ElevationDegrees { get; }
		int AziumthDegrees { get; }
		int SNRdB { get; }
		bool HaveEphemerisData { get; }
		bool HaveDifferentialCorrection { get; }
		bool UsedInFix { get; }
		bool IsRising { get; }
	}
}
