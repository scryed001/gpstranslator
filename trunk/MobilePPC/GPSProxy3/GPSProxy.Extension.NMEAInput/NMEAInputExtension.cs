/*
$Id: NMEAInputExtension.cs,v 1.2 2006/05/25 10:14:36 andrew_klopper Exp $

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
using GPSProxy.Extension;
using GPSProxy.Common;

namespace GPSProxy.Extension.NMEAInput
{
	/// <summary>
	/// Summary description for NMEAInputExtension.
	/// </summary>
	[GPSProxyExtensionAttribute("NMEA Input", "Allows GPS fixes and satellite data to be obtained from a GPS receiver that supports NMEA output",
		 "1.1", "2.2", "2.2", false, true, 0, false, true)]
	public class NMEAInputExtension : IExtension, IDisposable
	{
		private bool disposed = false;

		private NMEAProtocolEngine engine;

		private Settings settings = new Settings(new Setting[] {
			new Setting("in_port", "GPS COM Port", "NMEA", "comportin", ""),
			new Setting("log", "Log Input", "NMEA", "boolean", false),
			new Setting("mirror", "Mirror GPS Input to Output Port", "NMEA", "boolean", false),
			new Setting("out_port", "Output COM Port", "NMEA", "comportout", ""),
			new Setting("movement_threshold", "Movement Threshold (m)", "NMEA", "double", (double)1.0, "maxlength", 6),
			new Setting("dop_multiplier", "DOP to Accuracy Multiplier", "NMEA", "double", (double)GPSFix.DefaultDOPMultiplier, "maxlength", 6)
		});

		private IApplication application;
		private IConfig config;
		private IPort in_port;
		private IPort out_port;
		private bool log_input = false;

		public NMEAInputExtension()
		{
		}

		~NMEAInputExtension()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (! disposed) 
			{
				if (disposing)
				{
					if (in_port != null)
					{
						in_port.Dispose();
						in_port = null;
					}
					if (out_port != null)
					{
						out_port.Dispose();
						out_port = null;
					}
				}
				application = null;
				config = null;
				engine = null;

				disposed = true;
			}
		}

		#region IExtension Members

		public event GPSProxy.Extension.GPSFixEvent NewGPSFix;
		public event GPSProxy.Extension.GPSSatelliteDataEvent NewGPSSatelliteData;

		public void ExtensionInit(IApplication application, int extensionID, IConfig config)
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);

			this.application = application;
			this.config = config;

			settings["in_port"].Value = config.GetString("GPS COM Port", (string)settings["in_port"].Value);
			settings["log"].Value = config.GetBoolean("Log Input", (bool)settings["log"].Value);
			settings["mirror"].Value = config.GetBoolean("Mirror GPS Input to Output Port", (bool)settings["mirror"].Value);
			settings["out_port"].Value = config.GetString("Output COM Port", (string)settings["out_port"].Value);
			settings["movement_threshold"].Value = config.GetDouble("Movement Threshold", (double)settings["movement_threshold"].Value);
			settings["dop_multiplier"].Value = config.GetDouble("DOP to Accuracy Multiplier", (double)settings["dop_multiplier"].Value);

			log_input = (bool)settings["log"].Value;
			GPSFix.dopMultiplier = (double)settings["dop_multiplier"].Value;
		}

		public void ExtensionDispose()
		{
			Dispose(true);
		}

		public void ResizeUserInterface(System.Drawing.Size size)
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);
		}

		public System.Windows.Forms.Control GetUserInterface()
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);

			return null;
		}

		public void ShowConfigurationDialog()
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);

			if (application.ShowSettingsDialog(this, settings, new SettingsValidator(ValidateSettings)))
			{
				log_input = (bool)settings["log"].Value;
				GPSFix.dopMultiplier = (double)settings["dop_multiplier"].Value;

				config.Set("GPS COM Port", (string)settings["in_port"].Value);
				config.Set("Log Input", (bool)settings["log"].Value);
				config.Set("Mirror GPS Input to Output Port", (bool)settings["mirror"].Value);
				config.Set("Output COM Port", (string)settings["out_port"].Value);
				config.Set("Movement Threshold", (double)settings["movement_threshold"].Value);
			}
		}

		public void Start()
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);

			try
			{
				engine = new NMEAProtocolEngine((double)settings["movement_threshold"].Value);
				if ((bool)settings["mirror"].Value)
				{
					out_port = application.GetPort(this, (string)settings["out_port"].Value);
					out_port.Read += new PortReadEvent(out_port_Read);
					out_port.Error += new PortErrorEvent(out_port_Error);
					out_port.Open();
				}
				in_port = application.GetPort(this, (string)settings["in_port"].Value);
				in_port.Read += new PortReadEvent(in_port_Read);
				in_port.Error += new PortErrorEvent(in_port_Error);
				bool bSucc = in_port.Open();
                if (!bSucc)
                    application.LogMessage(this, "Error: Failed to open input port");
			}
			catch
			{
				engine = null;
				if (in_port != null)
				{
					in_port.Close();
					in_port.Dispose();
					in_port = null;
				}
				if (out_port != null)
				{
					out_port.Close();
					out_port.Dispose();
					out_port = null;
				}
				throw;
			}
		}

		public void Stop()
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);

			in_port.Close();
			in_port.Dispose();
			in_port = null;

			if (out_port != null)
			{
				out_port.Close();
				out_port.Dispose();
				out_port = null;
			}

			engine = null;
		}

		public void Wakeup()
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);

			// Let the application handle automatic stop and start after wakeup.
		}

		public void ProcessGPSSatelliteData(IGPSSatelliteVehicle[] satelliteVehicles)
		{
			// Not used in a provider.
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);
		}

		public void ProcessGPSFix(IGPSFix fix)
		{
			// Not used in a provider.
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);
		}

		#endregion

		private bool ValidateSettings(Settings settings, ref string errorMessage)
		{
			bool ret = true;

			if ((double)settings["movement_threshold"].Value < 0)
			{
				errorMessage += "Invalid Movement Threshold.\r\n";
				ret = false;
			}
			if ((double)settings["dop_multiplier"].Value < 0)
			{
				errorMessage += "Invalid DOP Multiplier.\r\n";
				ret = false;
			}

			return ret;
		}

		private void in_port_Read(IPort sender, byte[] data)
		{
			engine.ProcessReceivedData(data);
			IGPSFix fix = engine.MostRecentGPSFix;
			IGPSSatelliteVehicle[] vehicles = engine.MostRecentSatelliteData;
			if ((fix != null) && (NewGPSFix != null))
				NewGPSFix(this, fix);
			if ((vehicles != null) && (NewGPSSatelliteData != null))
				NewGPSSatelliteData(this, vehicles);
			if (out_port != null)
				out_port.Write(data);
			if (log_input)
				application.LogMessage(this, Encoding.ASCII.GetString(data, 0, data.Length));
		}

		private void in_port_Error(IPort sender, string error)
		{
			application.LogMessage(this, "GPS Port Error: " + error);
		}

		private void out_port_Read(IPort sender, byte[] data)
		{
			// Do nothing.
		}

		private void out_port_Error(IPort sender, string error)
		{
			application.LogMessage(this, "Output Port Error: " + error);
		}
	}
}
