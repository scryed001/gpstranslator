/*
$Id: GarminProtocolOutputExtension.cs,v 1.2 2006/05/25 10:14:36 andrew_klopper Exp $

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
using System.Windows.Forms;
using System.Drawing;
using GPSProxy.Extension;

namespace GPSProxy.Extension.GarminProtocolOutput
{
	/// <summary>
	/// Summary description for GarminProtocolOutput.
	/// </summary>
	[GPSProxyExtension("Garmin Protocol Output",
		"Allows output of the Garmin GPS Protocol to an external application", "1.1", "2.2", "2.2",
		 false, true, 0, true, false)]
	public class GarminProtocolOutputExtension : IExtension, IDisposable
	{
		private delegate void EventThreadCallback();

		private class EventThread
		{
			private bool shuttingDown = false;
			private Thread thread;
			private AutoResetEvent threadEvent = new AutoResetEvent(false);
			private EventThreadCallback callback;

			public AutoResetEvent ThreadEvent
			{
				get { return threadEvent; }
			}
			
			public ThreadPriority Priority
			{
				get { return thread.Priority; }
				set { thread.Priority = Priority; }
			}

			public EventThread(EventThreadCallback callback)
			{
				this.callback = callback;
				thread = new Thread(new ThreadStart(StartThread));
			}

			public void Start()
			{
				thread.Start();
			}

			public void Abort()
			{
				lock (this)
				{
					shuttingDown = true;
				}
				threadEvent.Set();
			}

			private void StartThread()
			{
				while (true)
				{
					if (threadEvent.WaitOne()) 
					{
						lock (this)
						{
							if (shuttingDown)
								break;
						}
						callback();
					}
				}
			}
		}

		private bool disposed = false;

		private bool started = false;

		private Settings settings =
			new Settings(new Setting[] {
										   new Setting("appPort", "Application COM Port", "Communication", "comportout", ""),
										   new Setting("logGarminProtocol", "Log Garmin Protocol", "Communication", "boolean", false),
										   new Setting("gpsProductId", "GPS Product ID", "Garmin Protocol", "integer", 9999, "maxlength", 5),
										   new Setting("gpsSoftwareVersion", "GPS Software Version", "Garmin Protocol", "integer", 100, "maxlength", 5),
										   new Setting("gpsProductDescription", "GPS Product Description", "Garmin Protocol", "string", "GPSProxy"),
										   new Setting("gpsUnitId", "GPS Unit ID", "Garmin Protocol", "long", (long)1234567890, "maxlength", 10),
										   new Setting("snrMin", "SNRdB Min", "Garmin Que", "updown", 30, "maxlength", 2, "min", 10, "max", 69),
										   new Setting("snrMax", "SNRdB Max", "Garmin Que", "updown", 50, "maxlength", 2, "min", 11, "max", 70)
									   });

		private IApplication application;
		private IConfig config;
		private IPort appPort;
		private bool logGarminProtocol;

		private GarminProtocolEngine engine;
		private Mutex engineMutex = new Mutex();
		private Mutex appPortMutex = new Mutex();
		private System.Threading.Timer protocolTimer;
		private EventThread outputThread;

		public GarminProtocolOutputExtension()
		{
			protocolTimer = new System.Threading.Timer(new TimerCallback(ProtocolTimerTick), null,
				Timeout.Infinite, Timeout.Infinite);
		}

		~GarminProtocolOutputExtension()
		{
			Dispose(false);
		}

		protected void Dispose(bool disposing)
		{
			if (! disposed)
			{
				if (disposing)
				{
					if (started)
						Stop();
					protocolTimer.Dispose();
				}

				application = null;
				config = null;
				engine = null;
				appPort = null;
				outputThread = null;

				disposed = true;
			}
		}

		private bool ValidateSettings(Settings settings, ref string errorMessage)
		{
			bool ret = true;

			if (((int)settings["gpsProductId"].Value < 0) || ((int)settings["gpsProductId"].Value > Int16.MaxValue))
			{
				errorMessage += "Invalid GPS Product ID.\r\n";
				ret = false;
			}
			if (((int)settings["gpsSoftwareVersion"].Value < 0) || ((int)settings["gpsSoftwareVersion"].Value > Int16.MaxValue))
			{
				errorMessage += "Invalid GPS Software Version.\r\n";
				ret = false;
			}
			if ((string)settings["gpsProductDescription"].Value == "")
			{
				errorMessage += "Invalid GPS Product Description.\r\n";
				ret = false;
			}
			if (((long)settings["gpsUnitId"].Value < 0) || ((long)settings["gpsUnitId"].Value > UInt32.MaxValue))
			{
				errorMessage += "Invalid GPS Unit ID.\r\n";
				ret = false;
			}
			if ((int)settings["snrMin"].Value >= (int)settings["snrMax"].Value)
			{
				errorMessage += "SNRdB minimum value must be less than the maximum value.\r\n";
				ret = false;
			}

			return ret;
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

			settings["appPort"].Value = config.GetString("Application COM Port", (string)settings["appPort"].Value);
			settings["logGarminProtocol"].Value = config.GetBoolean("Log Garmin Protocol", (bool)settings["logGarminProtocol"].Value);
			settings["gpsProductId"].Value = config.GetInt("GPS Product ID", (int)settings["gpsProductId"].Value);
            settings["gpsSoftwareVersion"].Value = config.GetInt("GPS Software Version", (int)settings["gpsSoftwareVersion"].Value);
			settings["gpsProductDescription"].Value = config.GetString("GPS Product Description", (string)settings["gpsProductDescription"].Value);
			settings["gpsUnitId"].Value = (long)config.GetLong("GPS Unit ID", (long)settings["gpsUnitId"].Value);
			settings["snrMin"].Value = config.GetInt("SNRdB Min", (int)settings["snrMin"].Value);
			settings["snrMax"].Value = config.GetInt("SNRdB Max", (int)settings["snrMax"].Value);

			logGarminProtocol = (bool)settings["logGarminProtocol"].Value;
		}

		public void ExtensionDispose()
		{
			Dispose(true);
		}

		public void ShowConfigurationDialog()
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);

			if (application.ShowSettingsDialog(this, settings, null))
			{
				logGarminProtocol = (bool)settings["logGarminProtocol"].Value;

				config.Set("Application COM Port", (string)settings["appPort"].Value);
				config.Set("Log Garmin Protocol", (bool)settings["logGarminProtocol"].Value);
				config.Set("GPS Product ID", (int)settings["gpsProductId"].Value);
				config.Set("GPS Software Version", (int)settings["gpsSoftwareVersion"].Value);
				config.Set("GPS Product Description", (string)settings["gpsProductDescription"].Value);
				config.Set("GPS Unit ID", (long)settings["gpsUnitId"].Value);
				config.Set("SNRdB Min", (int)settings["snrMin"].Value);
				config.Set("SNRdB Max", (int)settings["snrMax"].Value);
			}
		}

		public Control GetUserInterface()
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);
			return null;
		}

		public void ResizeUserInterface(Size size)
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);
		}

		public void Start()
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);

			if (engineMutex.WaitOne())
			{
				try
				{
					if (appPortMutex.WaitOne())
					{
						try 
						{
							try 
							{
								engine = new GarminProtocolEngine((short)((int)settings["gpsProductId"].Value),
									(short)((int)settings["gpsSoftwareVersion"].Value),
									(string)settings["gpsProductDescription"].Value,
									(uint)((long)settings["gpsUnitId"].Value), (int)settings["snrMin"].Value,
									(int)settings["snrMax"].Value);

								appPort = application.GetPort(this, (string)settings["appPort"].Value);
								appPort.Read += new PortReadEvent(appPort_Read);
								appPort.Error += new PortErrorEvent(appPort_Error);
								appPort.Open();

								outputThread = new EventThread(new EventThreadCallback(OutputThreadCallback));
								outputThread.Priority = ThreadPriority.Normal;
								outputThread.Start();

								protocolTimer.Change(0, 1000);
							}
							catch
							{
								protocolTimer.Change(Timeout.Infinite, Timeout.Infinite);
								if (outputThread != null)
								{
									outputThread.Abort();
									outputThread = null;
								}
								if (appPort != null)
								{
									appPort.Close();
									appPort.Dispose();
									appPort = null;
								}
								engine = null;
								throw;
							}
						}
						finally
						{
							appPortMutex.ReleaseMutex();
						}
					}
					else
						throw new Exception("Failed to lock application port mutex");
				}
				finally
				{
					engineMutex.ReleaseMutex();
				}
			}
			else
				throw new Exception("Failed to lock protocol engine mutex");

			started = true;
		}

		public void Stop()
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);

			started = false;

			protocolTimer.Change(Timeout.Infinite, Timeout.Infinite);

			if (engineMutex.WaitOne())
			{
				try
				{
					if (appPortMutex.WaitOne())
					{
						try
						{
							outputThread.Abort();
							outputThread = null;

							appPort.Close();
							appPort.Dispose();
							appPort = null;

							engine = null;
						}
						finally
						{
							appPortMutex.ReleaseMutex();
						}
					}
					else 
						throw new Exception("Failed to lock application port mutex");
				}
				finally
				{
					engineMutex.ReleaseMutex();
				}
			}
			else
				throw new Exception("Failed to lock protocol engine mutex");
		}

		public void Wakeup()
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);

			// Let the application handle automatic stopping and starting after wakeup.
		}

		public void ProcessGPSFix(IGPSFix fix)
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);

			if (engineMutex.WaitOne())
			{
				try
				{
					if (engine != null)
					{
						engine.NewGPSFix(fix);
						outputThread.ThreadEvent.Set();
					}
				}
				finally
				{
					engineMutex.ReleaseMutex();
				}
			}
			else
			{
				application.LogMessage(this, "Failed to lock protocol engine mutex");
			}
		}

		public void ProcessGPSSatelliteData(IGPSSatelliteVehicle[] satelliteVehicles)
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);

			if (engineMutex.WaitOne())
			{
				try
				{
					if (engine != null)
					{
						engine.NewSatelliteData(satelliteVehicles);
						outputThread.ThreadEvent.Set();
					}
				}
				finally
				{
					engineMutex.ReleaseMutex();
				}
			}
			else
			{
				application.LogMessage(this, "Failed to lock protocol engine mutex");
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		private string HexFormatPacket(byte[] packet)
		{
			return HexFormatPacket(packet, 0, packet.Length);
		}

		private string HexFormatPacket(byte[] packet, int offset, int length)
		{
			char[] hexChars = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};
			string ret = "";
			int i;
			for (i = offset; i < offset + length; i++) 
			{
				ret += hexChars[packet[i] >> 4];
				ret += hexChars[packet[i] & 15];
			}
			return ret;
		}

		private void appPort_Read(IPort sender, byte[] data)
		{
			if (engineMutex.WaitOne())
			{
				try
				{
					if (engine != null)
					{
						if (logGarminProtocol)
							application.LogMessage(this, "in: " + HexFormatPacket(data));
						engine.ProcessReceivedData(data);
						outputThread.ThreadEvent.Set();
					}
				}
				finally
				{
					engineMutex.ReleaseMutex();
				}
			}
			else
			{
				application.LogMessage(this, "Failed to lock protocol engine mutex");
			}
		}

		private void appPort_Error(IPort sender, string error)
		{
			application.LogMessage(this, "App Port Error: " + error);
		}

		private void ProtocolTimerTick(object state)
		{
			if (engineMutex.WaitOne())
			{
				try
				{
					if (engine != null)
					{
						engine.Tick();
						outputThread.ThreadEvent.Set();
					}
				}
				finally
				{
					engineMutex.ReleaseMutex();
				}
			}
			else
			{
				application.LogMessage(this, "Failed to lock protocol engine mutex");
			}
		}

		private void OutputThreadCallback()
		{
			// All this trouble just to stop the user interface thread from blocking if the app port
			// blocks...
			byte[] output = null;
			if (engineMutex.WaitOne())
			{
				try
				{
					if (engine != null)
						output = engine.Output;
				}
				finally
				{
					engineMutex.ReleaseMutex();
				}
			}
			else
			{
				application.LogMessage(this, "Failed to lock protocol engine mutex");
			}
			if ((output != null) && (output.Length > 0))
			{
				if (appPortMutex.WaitOne())
				{
					try
					{
						if (appPort != null)
						{
							if (logGarminProtocol)
								application.LogMessage(this, "out: " + HexFormatPacket(output));

							appPort.Write(output);
						}
					}
					finally
					{
						appPortMutex.ReleaseMutex();
					}
				}
				else
				{
					application.LogMessage(this, "Failed to lock application port mutex");
				}
			}
		}
	}
}