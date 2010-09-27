/*
$Id: VirtualComPort.cs,v 1.1 2006/05/23 09:27:05 andrew_klopper Exp $

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
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using OpenNETCF.Win32;

namespace GPSProxy
{
	/// <summary>
	/// Summary description for VirtualComPort.
	/// </summary>
	public sealed class VirtualComPort: IDisposable
	{
		// Allow for custom prefixes in future. Need to change driver to support this.
		private Regex portNameRegex = new Regex(@"^(COM)([0-9]+):$", RegexOptions.IgnoreCase);

		private bool disposed;

		private IntPtr DeviceHandle;

		private string m_PortName = "";

		public bool DriverLoaded
		{
			get {
				if (disposed)
					throw new ObjectDisposedException(GetType().Name);

				return DeviceHandle != IntPtr.Zero;
			}
		}

		public string PortName
		{
			get {
				if (disposed)
					throw new ObjectDisposedException(GetType().Name);

				return m_PortName == "" ? "unknown" : m_PortName;
			}
		}

		public VirtualComPort(RegistryKey rootKey, string dll, string friendlyName)
			: this(rootKey, dll, friendlyName, "")
		{
		}

		public VirtualComPort(RegistryKey rootKey, string dll, string friendlyName, string portName)
		{
			string prefix;
			int index;

			// Extract the prefix and index from the port name.
			m_PortName = portName;
			if (m_PortName != "")
			{
				Match match = portNameRegex.Match(m_PortName);
				if (! match.Success)
					throw new Exception("Invalid virtual COM port name: " + portName);
				prefix = match.Groups[1].Value;
				index = int.Parse(match.Groups[2].Value);
			}
			else
			{
				prefix = "COM";
				index = -1;
			}

			// Create a uniquely named subkey under the root key for this port.
			string[] subKeyNames = rootKey.GetSubKeyNames();
			int lastSubKeyIndex = -1;
			foreach (string keyName in subKeyNames)
			{
				int val;
				try
				{
					val = int.Parse(keyName);
				}
				catch
				{
					val = -1;
				}
				lastSubKeyIndex = val > lastSubKeyIndex ? val : lastSubKeyIndex;
			}
			string subKeyName = ((int)(lastSubKeyIndex + 1)).ToString();
			RegistryKey subKey = rootKey.CreateSubKey(subKeyName);
			try
			{
				// Populate the subkey.
				subKey.SetValue("Dll", dll, RegistryValueKind.String);
				subKey.SetValue("Prefix", prefix, RegistryValueKind.String);
				subKey.SetValue("FriendlyName", friendlyName +
					(m_PortName == "" ? "" : " on " + m_PortName), RegistryValueKind.String);
				if (index >= 0)
					subKey.SetValue("Index", index, RegistryValueKind.DWord);
				subKey.Flush();

				// Attempt to load the driver, remembering to strip the "HKEY_LOCAL_MACHINE\" prefix
				// from the sub key name.
				DeviceHandle = ActivateDevice(subKey.Name.Remove(0, subKey.Name.IndexOf(@"\") + 1), 0);
				if (DeviceHandle != IntPtr.Zero)
				{
					try 
					{
						// The driver loaded successfully. Retrieve the port name from the Active drivers
						// subkey if we don't have it already.
						if (m_PortName == "")
						{
							RegistryKey activeDrivers = Registry.LocalMachine.OpenSubKey(@"Drivers\Active", false);
							try
							{
								string[] driverSubKeyNames = activeDrivers.GetSubKeyNames();
								foreach (string keyName in driverSubKeyNames)
								{
									RegistryKey driverKey = activeDrivers.OpenSubKey(keyName, false);
									try
									{
										object HndValue = driverKey.GetValue("Hnd");
										object NameValue = driverKey.GetValue("Name");
										if ((HndValue != null) && (HndValue.ToString() == DeviceHandle.ToString()) &&
											(NameValue != null))
										{
											m_PortName = NameValue.ToString();
											break;
										}
									}
									finally
									{
										driverKey.Dispose();
									}
								}
							}
							finally
							{
								activeDrivers.Dispose();
							}

							// Verify the port name and update the friendly name if we can.
							if (m_PortName != "")
							{
								Match match = portNameRegex.Match(m_PortName);
								if (! match.Success)
									throw new Exception("Invalid virtual COM port name: " + portName);
								subKey.SetValue("FriendlyName", friendlyName + " on " + m_PortName,
									RegistryValueKind.String);
								subKey.Flush();
							}
							else
							{
								throw new Exception("Unable to locate active device driver details for device handle " + DeviceHandle.ToString());
							}
						}
					}
					catch
					{
						DeactivateDevice(DeviceHandle);
						DeviceHandle = IntPtr.Zero;
						throw;
					}
				}
			}
			finally
			{
				subKey.Dispose();
				if (DeviceHandle == IntPtr.Zero)
				{
					rootKey.DeleteSubKey(subKeyName);
					rootKey.Flush();
				}
			}
		}

		~VirtualComPort()
		{
			Dispose(false);
		}

		private void Dispose(bool disposing)
		{
			if (! disposed)
			{
				if (disposing)
				{
				}
				if (DeviceHandle != IntPtr.Zero)
					DeactivateDevice(DeviceHandle);
				disposed = true;
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		[DllImport("coredll.dll")]
		private static extern IntPtr ActivateDevice(string lpszDevKey, uint dwClientInfo);

		[DllImport("coredll.dll")]
		private static extern bool DeactivateDevice(IntPtr hDevice);
	}
}
