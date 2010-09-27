/*======================================================================================= 
OpenNETCF.Win32.Wave

Copyright © 2005, OpenNETCF.org

This library is free software; you can redistribute it and/or modify it under 
the terms of the OpenNETCF.org Shared Source License.

This library is distributed in the hope that it will be useful, but 
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
FITNESS FOR A PARTICULAR PURPOSE. See the OpenNETCF.org Shared Source License 
for more details.

You should have received a copy of the OpenNETCF.org Shared Source License 
along with this library; if not, email licensing@opennetcf.org to request a copy.

If you wish to contact the OpenNETCF Advisory Board to discuss licensing, please 
email licensing@opennetcf.org.

For general enquiries, email enquiries@opennetcf.org or visit our website at:
http://www.opennetcf.org

=======================================================================================*/

using System;
using OpenNETCF.Win32;
using System.Runtime.InteropServices;

namespace OpenNETCF.Win32
{
	/// <summary>
	/// Native wave in/out methods.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	/// <remarks>Previously part of <see cref="OpenNETCF.Win32.Core"/>.</remarks>
	public class Wave
	{
		private Wave(){}

		#region --------------- Multimedia API Calls ---------------
		

		/// <summary>
		/// Set the volume for the default waveOut device (device ID = 0)
		/// </summary>
		/// <param name="Volume"></param>
		public void SetVolume(int Volume)
		{
			WaveFormatEx	format		= new WaveFormatEx();
			IntPtr			hWaveOut	= IntPtr.Zero;

			waveOutOpen(out hWaveOut, 0, format, IntPtr.Zero, 0, 0);
			waveOutSetVolume(hWaveOut, Volume);
			waveOutClose(hWaveOut);
		}

		/// <summary>
		/// Set the volume for an already-open waveOut device
		/// </summary>
		/// <param name="hWaveOut"></param>
		/// <param name="Volume"></param>
		public void SetVolume(IntPtr hWaveOut, int Volume)
		{
			waveOutSetVolume(hWaveOut, Volume);
		}

		/// <summary>
		/// Get the current volume setting for the default waveOut device (device ID = 0)
		/// </summary>
		/// <returns></returns>
		public int GetVolume()
		{
			WaveFormatEx	format		= new WaveFormatEx();
			IntPtr			hWaveOut	= IntPtr.Zero;
			int			volume		= 0;

			waveOutOpen(out hWaveOut, 0, format, IntPtr.Zero, 0, 0);
			waveOutGetVolume(hWaveOut, ref volume);
			waveOutClose(hWaveOut);

			return volume;
		}

		/// <summary>
		/// Set the current volume setting for an already-open waveOut device
		/// </summary>
		/// <param name="hWaveOut"></param>
		/// <returns></returns>
		public int GetVolume(IntPtr hWaveOut)
		{
			int			volume		= 0;

			waveOutGetVolume(hWaveOut, ref volume);

			return volume;
		}
		#endregion ---------------------------------------------

		#region multimedia P/Invokes
		
		[DllImport ("coredll.dll", EntryPoint="waveOutGetNumDevs", SetLastError=true)]
		public static extern int waveOutGetNumDevs();

		[DllImport ("coredll.dll", EntryPoint="waveInGetNumDevs", SetLastError=true)]
		public static extern int waveInGetNumDevs();

		[DllImport ("coredll.dll", EntryPoint="waveOutOpen", SetLastError=true)]
		public static extern int waveOutOpen(out IntPtr t, int id, WaveFormatEx pwfx, IntPtr dwCallback, int dwInstance, int fdwOpen);

		[DllImport ("coredll.dll", EntryPoint="waveOutGetVolume", SetLastError=true)]
		public static extern int waveOutGetVolume(IntPtr hwo, ref int pdwVolume);

		[DllImport ("coredll.dll", EntryPoint="waveOutSetVolume", SetLastError=true)]
		public static extern int waveOutSetVolume(IntPtr hwo, int dwVolume);

		[DllImport ("coredll.dll", EntryPoint="waveOutPrepareHeader", SetLastError=true)]
		public static extern int waveOutPrepareHeader(IntPtr hwo, byte[] pwh, int cbwh);
		[DllImport ("coredll.dll", EntryPoint="waveOutPrepareHeader", SetLastError=true)]
		public static extern int waveOutPrepareHeader(IntPtr hwo, IntPtr lpHdr, int cbwh);

		[DllImport ("coredll.dll", EntryPoint="waveOutWrite", SetLastError=true)]
		public static extern int waveOutWrite(IntPtr hwo, byte[] pwh, int cbwh);
		[DllImport ("coredll.dll", EntryPoint="waveOutWrite", SetLastError=true)]
		public static extern int waveOutWrite(IntPtr hwo, IntPtr lpHdr, int cbwh);

		[DllImport ("coredll.dll", EntryPoint="waveOutUnprepareHeader", SetLastError=true)]
		public static extern int waveOutUnprepareHeader(IntPtr hwo, byte[] pwh, int cbwh);
		[DllImport ("coredll.dll", EntryPoint="waveOutUnprepareHeader", SetLastError=true)]
		public static extern int waveOutUnprepareHeader(IntPtr hwo, IntPtr lpHdr, int cbwh);

		[DllImport ("coredll.dll", EntryPoint="waveOutClose", SetLastError=true)]
		public static extern int waveOutClose(IntPtr hwo);

		[DllImport ("coredll.dll", EntryPoint="waveOutPause", SetLastError=true)]
		public static extern int waveOutPause(IntPtr hwo);

		[DllImport ("coredll.dll", EntryPoint="waveOutRestart", SetLastError=true)]
		public static extern int waveOutRestart(IntPtr hwo);

		[DllImport ("coredll.dll", EntryPoint="waveOutReset", SetLastError=true)]
		public static extern int waveOutReset(IntPtr hwo);

		[DllImport ("coredll.dll", EntryPoint="waveInOpen", SetLastError=true)]
		internal static extern int waveInOpen(out IntPtr t, uint id, WaveFormatEx pwfx, IntPtr dwCallback, int dwInstance, int fdwOpen);

		[DllImport ("coredll.dll", EntryPoint="waveInClose", SetLastError=true)]
		public static extern int waveInClose(IntPtr hDev);

		[DllImport ("coredll.dll", EntryPoint="waveInPrepareHeader", SetLastError=true)]
		public static extern int waveInPrepareHeader(IntPtr hwi, byte[] pwh, int cbwh);
		[DllImport ("coredll.dll", EntryPoint="waveInPrepareHeader", SetLastError=true)]
		public static extern int waveInPrepareHeader(IntPtr hwi, IntPtr lpHdr, int cbwh);

		[DllImport ("coredll.dll", EntryPoint="waveInStart", SetLastError=true)]
		public static extern int waveInStart(IntPtr hwi);

		[DllImport ("coredll.dll", EntryPoint="waveInUnprepareHeader", SetLastError=true)]
		public static extern int waveInUnprepareHeader(IntPtr hwi, byte[] pwh, int cbwh);
		[DllImport ("coredll.dll", EntryPoint="waveInUnprepareHeader", SetLastError=true)]
		public static extern int waveInUnprepareHeader(IntPtr hwi, IntPtr lpHdr, int cbwh);

		[DllImport ("coredll.dll", EntryPoint="waveInStop", SetLastError=true)]
		public static extern int waveInStop(IntPtr hwi);

		[DllImport ("coredll.dll", EntryPoint="waveInReset", SetLastError=true)]
		public static extern int waveInReset(IntPtr hwi);

		[DllImport ("coredll.dll", EntryPoint="waveInGetDevCaps", SetLastError=true)]
		public static extern int waveInGetDevCaps(int uDeviceID, byte[] pwic, int cbwic);

		[DllImport ("coredll.dll", EntryPoint="waveInAddBuffer", SetLastError=true)]
		public static extern int waveInAddBuffer(IntPtr hwi, byte[] pwh, int cbwh);
		[DllImport ("coredll.dll", EntryPoint="waveInAddBuffer", SetLastError=true)]
		public static extern int waveInAddBuffer(IntPtr hwi, IntPtr lpHdr, int cbwh);

		#endregion
	}
}
