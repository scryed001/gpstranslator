//==========================================================================================
//
//		OpenNETCF.Windows.Forms.SystemSounds
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

// Replaces OpenNETCF.Win32.Sound
// Follows desktop .NET v2.0 model
// Peter Foot 12 July 2004

namespace OpenNETCF.Windows.Forms
{
	/// <summary>
	/// Retrieves sounds associated with a set of Windows system sound event types.
	/// <para><b>New in v1.2</b></para>
	/// </summary>
	public sealed class SystemSounds
	{
		private SystemSounds(){}

		/// <summary>
		/// Gets the sound associated with the Beep program event.
		/// </summary>
		public static SystemSound Beep
		{
			get
			{
				return new SystemSound(0);
			}
		}


		/// <summary>
		/// Gets the sound associated with the Asterisk program event.
		/// </summary>
		public static SystemSound Asterisk
		{
			get
			{
				return new SystemSound(0x00000040);
			}
		}

		/// <summary>
		/// Gets the sound associated with the Exclamation program event.
		/// </summary>
		public static SystemSound Exclamation
		{
			get
			{
				return new SystemSound(0x00000030);
			}
		}

		
		/// <summary>
		/// Gets the sound associated with the Question program event.
		/// </summary>
		public static SystemSound Question
		{
			get
			{
				return new SystemSound(0x00000020);
			}
		}
		
		/// <summary>
		/// Gets the sound associated with the Hand program event.
		/// </summary>
		public static SystemSound Hand
		{
			get
			{
				return new SystemSound(0x00000010);
			}
		}
	}

	/// <summary>
	/// Represents a standard system sound.
	/// <para><b>New in v1.2</b></para>
	/// </summary>
	public sealed class SystemSound
	{
		//type of sound
		private int mSoundType;

		internal SystemSound(int soundType)
		{
			//set type
			mSoundType = soundType;
		}

		public void Play()
		{
			//play
			MessageBeep(mSoundType);
		}

		[DllImport("coredll.dll", EntryPoint="MessageBeep", SetLastError=true)]
		private static extern void MessageBeep(int type);
		
	}
}
