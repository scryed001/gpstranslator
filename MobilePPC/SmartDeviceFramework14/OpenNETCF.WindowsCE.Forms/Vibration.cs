//==========================================================================================
//
//		OpenNETCF.WindowsCE.Forms.Vibrate
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

using System;
using System.Runtime.InteropServices;

namespace OpenNETCF.WindowsCE.Forms
{
	/// <summary>
	/// Controls vibration on the device.
	/// <para><b>Revised in v1.3</b></para>
	/// </summary>
	/// <remarks>Supported only on Smartphone. Previously named <b>OpenNETCF.Notification.Vibrate</b>. Vibration equipped Pocket PC devices emulate the Vibration motorl as a notification Led.</remarks>
	/// <platform><os>Windows Mobile 2003 for Smartphone</os></platform>
	public class Vibrate
	{
		private Vibrate(){}

		/// <summary>
		/// Plays the default looping vibration on the device.
		/// </summary>
		/// <returns>TRUE if vibration started successfully else FALSE.</returns>
		/// <platform><os>Pocket PC Phone Edition, Windows Mobile 2003 for Smartphone</os></platform>
		/// <preliminary/>
		public static bool Play()
		{
			int result = VibratePlay(0, IntPtr.Zero, 0xffffffff, 0xffffffff);

			if(result!=0)
			{

				return false;
			}
			return true;
		}

		[DllImport("aygshell.dll", EntryPoint="Vibrate", SetLastError=true)]
		private static extern int VibratePlay(
			int cvn,
			IntPtr rgvn,
			uint fRepeat,
			uint dwTimeout);


		/// <summary>
		/// Stops any current vibration.
		/// </summary>
		/// <returns>TRUE on success else FALSE.</returns>
		/// <platform><os>Pocket PC Phone Edition, Windows Mobile 2003 for Smartphone</os></platform>
		/// <preliminary/>
		public static bool Stop()
		{
			int result = VibrateStop();

			if(result!=0)
			{
				return false;
			}
			return true;
		}

		[DllImport("aygshell.dll", SetLastError=true)]
		private static extern int VibrateStop();


		/// <summary>
		/// Gets the current device vibration capabilities.
		/// </summary>
		/// <param name="caps">Member of the VibrationCapabilities enumeration identifying what capability to query.</param>
		/// <returns>0 if the capability is not supported, 1 if a single level is supported or a value between 2 and 7 if multiple levels are supported.</returns>
		/// <remarks>This function returns the number of vibration steps (a number from 0 to 7) that the device hardware supports for the requested vibration capability.</remarks>
		/// <platform><os>Pocket PC Phone Edition, Windows Mobile 2003 for Smartphone</os></platform>
		/// <preliminary/>
		public static int GetDeviceCaps(VibrationCapabilities caps)
		{
			return VibrateGetDeviceCaps(caps);
		}

		[DllImport("aygshell.dll", SetLastError=true)]
		private static extern int VibrateGetDeviceCaps(
			VibrationCapabilities caps);


		/// <summary>
		/// Used by the GetDeviceCaps function to query the device vibration capabilities.
		/// <para><b>New in v1.1</b></para>
		/// </summary>
		/// <platform><os>Pocket PC Phone Edition, Windows Mobile 2003 for Smartphone</os></platform>
		/// <preliminary/>
		public enum VibrationCapabilities: int
		{
			/// <summary>
			/// Query the amplitude that the device supports.
			/// </summary>
			Amplitude,
			/// <summary>
			/// Query the frequency that the device supports.
			/// </summary>
			Frequency,

			//Last
		}


		/*public struct VibrateNote
		{
			public short Duration;
			public byte Amplitude;
			public byte Frequency;
		}*/
	}
}
