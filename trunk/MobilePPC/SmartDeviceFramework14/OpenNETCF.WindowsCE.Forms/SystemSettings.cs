//==========================================================================================
//
//		OpenNETCF.WindowsCE.Forms.SystemSettings
//		Copyright (c) 2004, OpenNETCF.org
//
//		This library is free software; you can redistribute it and/or modify it under 
//		the terms of the OpenNETCF.org Shared Source License.
//
//		This library is distributed in the hope that it will be useful, but 
//		WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
//		FITNESS FOR A PARTICULAR PURPOSE. See the OpenNETCF.org Shared Source License 
//		for more details.
//
//		You should have received a copy of the OpenNETCF.org Shared Source License 
//		along with this library; if not, email licensing@opennetcf.org to request a copy.
//
//		If you wish to contact the OpenNETCF Advisory Board to discuss licensing, please 
//		email licensing@opennetcf.org.
//
//		For general enquiries, email enquiries@opennetcf.org or visit our website at:
//		http://www.opennetcf.org
//
//==========================================================================================

// Tested with PPC2003SE Emulator 27/08/2004 Peter Foot 

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace OpenNETCF.WindowsCE.Forms
{
	/// <summary>
	/// Provides access to user interface and native Windows CE operating system settings on a device.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	public class SystemSettings
	{
		private SystemSettings(){}

		/// <summary>
		/// Gets or sets the current screen orientation of a device.
		/// </summary>
		public static ScreenOrientation ScreenOrientation
		{
			get
			{
				DEVMODE dm = new DEVMODE();
				dm.Fields = DM.DISPLAYQUERYORIENTATION;
				int result = ChangeDisplaySettingsEx(null, dm.ToByteArray(), IntPtr.Zero, CDS.TEST, IntPtr.Zero);

				if(result < 0)
				{
					throw new Win32Exception(result, "Error getting display orientation");
				}

				return (ScreenOrientation)dm.DisplayOrientation;
			}
			set
			{
				if(System.Environment.OSVersion.Version > new Version(4,21))
				{
					DEVMODE dm = new DEVMODE();
					dm.DisplayOrientation = (int)value;
					dm.Fields = DM.DISPLAYORIENTATION;

					int result = ChangeDisplaySettingsEx(null, dm.ToByteArray(), IntPtr.Zero, CDS.TEST, IntPtr.Zero);

					if(result < 0)
					{
						throw new Win32Exception(result, "Error setting display orientation");
					}
				}
				else
				{
					throw new PlatformNotSupportedException("Supported only on Windows Mobile 2003 Second Edition for Pocket PCs");
				}
			}
		}

		[DllImport("coredll.dll", SetLastError=true)]
		private static extern int ChangeDisplaySettingsEx(
			string lpszDeviceName,
			byte[] lpDevMode,
			IntPtr hwnd,
			CDS dwflags,
			IntPtr lParam);

		private enum CDS
		{
			ZERO = 0,
			TEST = 0x00000004,
			RESET = 0x40000000,
		}

	}
}
