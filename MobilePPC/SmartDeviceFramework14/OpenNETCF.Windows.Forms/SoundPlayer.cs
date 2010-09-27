//==========================================================================================
//
//		OpenNETCF.Windows.Forms.SoundPlayer
//		Copyright (c) 2004-2005, OpenNETCF.org
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
using System.IO;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;


namespace OpenNETCF.Windows.Forms
{
	/// <summary>
	/// Controls playback of a sound from a .wav file.
	/// <para><b>New in v1.2</b></para>
	/// </summary>
#if DESIGN
	[ToolboxItemFilter("NETCF",ToolboxItemFilterType.Require),
	ToolboxItemFilter("System.CF.Windows.Forms", ToolboxItemFilterType.Custom)]
#endif
	public class SoundPlayer : Component
	{
		//path to the file
		private string mSoundLocation = "";
		private Stream mStream;
		private byte[] mBuffer;

		/// <summary>
		/// Initializes a new instance of the <see cref="SoundPlayer"/> class.
		/// </summary>
		public SoundPlayer(){}

		/// <summary>
		/// Initializes a new instance of the <see cref="SoundPlayer"/> class, attaches the .wav file within the specified <see cref="Stream"/>.
		/// <para><b>New in v1.3</b></para>
		/// </summary>
		/// <param name="stream"></param>
		public SoundPlayer(Stream stream)
		{
			Stream = stream;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SoundPlayer"/> class and attaches the specified .wav file.
		/// <para><b>New in v1.2</b></para></summary>
		/// <param name="soundLocation">The location of a .wav file to load.</param>
		/// <remarks>The string passed to the soundLocation parameter must be a path to a .wav file.
		/// If the path is invalid, the <see cref="SoundPlayer"/> object will still be constructed, but subsequent calls to a load or play method will fail.</remarks>
		public SoundPlayer(string soundLocation)
		{
			//set the path
			SoundLocation = soundLocation;
		}

		/// <summary>
		/// Gets or sets the file path of the .wav file to load.
		/// <para><b>New in v1.2</b></para></summary>
		/// <value>The file path from which to load a .wav file, or <see cref="System.String.Empty"/> if no file path is present.
		/// The default is <see cref="System.String.Empty"/>.</value>
#if DESIGN
		[DefaultValue("")]
#endif
		public string SoundLocation
		{
			get
			{
				return mSoundLocation;
			}
			set
			{
				if(File.Exists(value))
				{
					mSoundLocation = value;
					Stream = null;
				}
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="Stream"/> from which to load the .wav file.
		/// <para><b>New in v1.3</b></para>
		/// </summary>
		/// <remarks>This property is set to null when the <see cref="SoundLocation"/> property is set to a new and valid sound location.</remarks>
#if DESIGN
		[Browsable(false)]
#endif
		public Stream Stream
		{
			get
			{
				return mStream;
			}
			set
			{
				mStream = value;

				if(value==null)
				{
					mBuffer = null;
				}
				else
				{	
					mBuffer = new byte[value.Length];
					value.Read(mBuffer, 0, mBuffer.Length);
					value.Close();
					mSoundLocation = string.Empty;
				}
			}
		}

		/// <summary>
		/// Plays the .wav file using a new thread.
		/// <para><b>New in v1.2</b></para></summary>
		/// <remarks>The Play method plays the sound using a new thread.
		/// If the .wav file has not been specified or it fails to load, the Play method will play the default beep sound.</remarks>
		public void Play()
		{
			//play async
			PlaySound(SoundFlags.Async);
		}

		/// <summary>
		/// Plays the .wav file using the UI thread.
		/// <para><b>New in v1.2</b></para></summary>
		/// <remarks>The PlaySync method uses the current thread to play a .wav file, preventing the thread from handling other messages until the load is complete.
		/// After a .wav file is successfully loaded from a Stream or URL path, future calls to playback methods for the SoundPlayer object will not need to reload the .wav file until the path for the sound changes.
		/// If the .wav file has not been specified or it fails to load, the PlaySync method will play the default beep sound.</remarks>
		public void PlaySync()
		{
			//play sync
			PlaySound(SoundFlags.Sync);
		}

		/// <summary>
		/// Plays and loops the .wav file using a new thread and loads the .wav file first if it has not been loaded.
		/// <para><b>New in v1.2</b></para></summary>
		/// <remarks>The PlayLooping method plays and loops the sound using a new thread.
		/// If the .wav file has not been specified or it fails to load, the PlayLooping method will play the default beep sound.</remarks>
		public void PlayLooping()
		{
			//play looping
			PlaySound(SoundFlags.Async | SoundFlags.Loop);
		}

		//helper function
		private void PlaySound(SoundFlags flags)
		{
			if(mBuffer!=null)
			{
				GCHandle wHandle = GCHandle.Alloc(mBuffer, GCHandleType.Pinned);
				PlaySound((IntPtr)(wHandle.AddrOfPinnedObject().ToInt32()+4),IntPtr.Zero, SoundFlags.Memory | flags);
				wHandle.Free();
			}
			else
			{
				PlaySound(mSoundLocation, IntPtr.Zero, SoundFlags.FileName | flags);
			}
		}

		[DllImport("coredll.dll", EntryPoint="PlaySoundW", SetLastError=true) ]
		private extern static bool PlaySound(string lpszName, IntPtr hModule, SoundFlags dwFlags);

		[DllImport("coredll.dll", EntryPoint="PlaySoundW", SetLastError=true) ]
		private extern static bool PlaySound(IntPtr lpszName, IntPtr hModule, SoundFlags dwFlags);


		/// <summary>
		/// Stops playback of the sound if playback is occurring.
		/// <para><b>New in v1.2</b></para></summary>
		public void Stop()
		{
			PlaySound(null, IntPtr.Zero, SoundFlags.NoDefault);
		}

		/// <summary>
		/// <para><b>New in v1.3</b></para>
		/// </summary>
#if DESIGN
		[Bindable(true), DefaultValue((string) null), TypeConverter(typeof(StringConverter))]
#endif
		public object Tag
		{
			get
			{
				return mTag;
			}
			set
			{
				mTag = value;
			}
		}
		private object mTag;


		#region Sound Flags
		/// <summary>
		/// PlaySound flags
		/// </summary>
		[Flags()]
		private enum SoundFlags : int
		{
			/// <summary>
			/// <b>name</b> is a WIN.INI [sounds] entry
			/// </summary>
			Alias      = 0x00010000,
			/// <summary>
			/// <b>name</b> is a file name
			/// </summary>
			FileName   = 0x00020000,
			/// <summary>
			/// <b>name</b> is a resource name or atom
			/// </summary>   
			Resource   = 0x00040004,   
			/// <summary>   
			/// Play synchronously (default)   
			/// </summary>   
			Sync       = 0x00000000,   
			/// <summary>   
			///  Play asynchronously   
			/// </summary>   
			Async      = 0x00000001,   
			/// <summary>   
			/// Silence not default, if sound not found   
			/// </summary>   
			NoDefault  = 0x00000002,   
			/// <summary>   
			/// <b>name</b> points to a memory file   
			/// </summary>   
			Memory     = 0x00000004,   
			/// <summary>   
			/// Loop the sound until next sndPlaySound   
			/// </summary>   
			Loop       = 0x00000008,   
			/// <summary>   
			/// Don't stop any currently playing sound   
			/// </summary>   
			NoStop     = 0x00000010,   
			/// <summary>   
			/// Don't wait if the driver is busy   
			/// </summary>   
			NoWait     = 0x00002000   
		}                  
		#endregion
	}
}
