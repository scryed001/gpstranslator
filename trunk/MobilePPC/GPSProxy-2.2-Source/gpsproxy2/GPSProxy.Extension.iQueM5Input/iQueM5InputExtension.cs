/*
$Id: iQueM5InputExtension.cs,v 1.2 2006/05/25 10:14:37 andrew_klopper Exp $

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

namespace GPSProxy.Provider.iQueM5
{
	/// <summary>
	/// Summary description for iQueM5InputExtension.
	/// </summary>
	[GPSProxyExtensionAttribute("iQue M5 Input",
		"Allows GPS fix and satellite information to be obtained from the built in GPS receiver on the Garmin iQue M5",
		"1.0", "2.2", "2.2", false, false, 0, false, true)]
	public class iQueM5InputExtension : IExtension
	{
		public iQueM5InputExtension()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#region IExtension Members

		public void Start()
		{
			// TODO:  Add iQueM5InputExtension.Start implementation
		}

		public void ResizeUserInterface(System.Drawing.Size size)
		{
			// TODO:  Add iQueM5InputExtension.ResizeUserInterface implementation
		}

		public void ExtensionInit(IApplication application, int extensionID, IConfig config)
		{
			// TODO:  Add iQueM5InputExtension.ExtensionInit implementation
		}

		public void Wakeup()
		{
			// TODO:  Add iQueM5InputExtension.Wakeup implementation
		}

		public void ShowConfigurationDialog()
		{
			// TODO:  Add iQueM5InputExtension.ShowConfigurationDialog implementation
		}

		public System.Windows.Forms.Control GetUserInterface()
		{
			// TODO:  Add iQueM5InputExtension.GetUserInterface implementation
			return null;
		}

		public event GPSProxy.Extension.GPSFixEvent NewGPSFix;

		public void Stop()
		{
			// TODO:  Add iQueM5InputExtension.Stop implementation
		}

		public void ProcessGPSSatelliteData(IGPSSatelliteVehicle[] satelliteVehicles)
		{
			// TODO:  Add iQueM5InputExtension.ProcessGPSSatelliteData implementation
		}

		public void ProcessGPSFix(IGPSFix fix)
		{
			// TODO:  Add iQueM5InputExtension.ProcessGPSFix implementation
		}

		public event GPSProxy.Extension.GPSSatelliteDataEvent NewGPSSatelliteData;

		public void ExtensionDispose()
		{
			// TODO:  Add iQueM5InputExtension.ExtensionDispose implementation
		}

		#endregion
	}
}
