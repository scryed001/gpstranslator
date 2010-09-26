//==========================================================================================
//
//		OpenNETCF.WindowsCE.Forms.InkX
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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using OpenNETCF.Runtime.InteropServices;
using OpenNETCF.WindowsCE.Forms;
using OpenNETCF.Windows.Forms; 

#if !DESIGN
using OpenNETCF.Win32;
#if !NDOC
using Microsoft.WindowsCE.Forms;
#endif
#endif

namespace OpenNETCF.WindowsCE.Forms
{
	/// <summary>
	/// Displays rich ink and voice content.
	/// <para><b>Revised in v1.3</b></para>
	/// </summary>
	/// <remarks>This class was previously named <b>OpenNETCF.Windows.Forms.InkX</b></remarks>
#if DESIGN
	[DefaultEvent("HotSpotClick"),
	DefaultProperty("Url")]
#endif
	public class InkX : ControlEx
	{

		//current settings
		private bool m_voicebar;
		private IntPtr mHInk = IntPtr.Zero;

		#region Constructor
		/// <summary>
		/// Creates a new instance of RichInk.
		/// </summary>
		public InkX() : base(true)
		{
#if !DESIGN
			
			//init inkx control
			InitInkX();
			
			
#endif
			
		}
		#endregion

#if !DESIGN
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ClassName = "InkX";
				return cp;
			}
		}

		private IntPtr hInk
		{
			get
			{
				if(mHInk==IntPtr.Zero)
				{
					mHInk = Win32Window.GetWindow(ChildHandle, GW.CHILD);
				}
				return mHInk;
			}
		}
#endif

		#region Enable Voice Bar Property
		/// <summary>
		/// Gets or sets a value which determines whether the VoiceBar is displayed.
		/// </summary>
		/// <remarks>Setting this property to true will display the VoiceBar.</remarks>
#if DESIGN
		[Category("Behavior"),
		Description("Indicates whether the voice recording bar is displayed.")]
#endif
		public bool EnableVoiceBar
		{
			get
			{
				return m_voicebar;
			}
			set
			{
				m_voicebar = value;
#if !DESIGN
				//send to control
				int intval = 0;
				if(value)
					intval = -1;

				Win32Window.SendMessage(ChildHandle, (int)InkXMessage.VoiceBar, intval, 0);
#endif
			}
		}
		#endregion
		
		#region Data Length Property
		/// <summary>
		/// Gets the length in bytes of the ink data in the main compose window.
		/// </summary>
#if DESIGN
		[Browsable(false)]
#endif
		public int DataLength
		{
			get
			{
#if !DESIGN
				return (int)Win32Window.SendMessage(ChildHandle,(int)InkXMessage.GetDataLen, 0, 0);
#else
				return 0;
#endif
			}
		}
		#endregion

		#region Data Property
		/// <summary>
		/// Gets or Sets the raw data contained in the compose window.
		/// </summary>
#if DESIGN
		[Browsable(false)]
#endif
		public byte[] Data
		{
			get
			{
#if !DESIGN
				//get data length
				int len = this.DataLength;
				byte[] data = new byte[len];
				//create native buffer
				IntPtr ptr = MarshalEx.AllocHGlobal(len);
				//send message to control
				Win32Window.SendMessage(ChildHandle,(int)InkXMessage.GetData, len, (int)ptr);
				//marshal data to byte array
				Marshal.Copy(ptr, data, 0, len);
				//free native memory
				MarshalEx.FreeHGlobal(ptr);

				return data;
#else
				return new byte[0];
#endif
			}
			set
			{
				int len = value.Length;
				IntPtr ptr = MarshalEx.AllocHGlobal(len);
				//copy to native memory
				Marshal.Copy(value, 0, ptr, len);
#if !DESIGN
				//send message to control
				Win32Window.SendMessage(ChildHandle,(int)InkXMessage.SetData, len, (int)ptr);
#endif
				//free native memory
				MarshalEx.FreeHGlobal(ptr);
			}
		}
		#endregion

		#region IsPlaying Property
		/// <summary>
		/// Gets a value indicating whether the control is currently playing audio.
		/// </summary>
		/// <value>Returns TRUE if the control is playing voice audio, else returns FALSE.</value>
#if DESIGN
		[Browsable(false)]
#endif
		public bool IsPlaying
		{
			get
			{
#if !DESIGN
				return Convert.ToBoolean((int)Win32Window.SendMessage(ChildHandle,(int)InkXMessage.IsVoicePlaying, 0, 0));
#else 
				return false;
#endif
			}
		}
		#endregion

		#region IsRecording Property
		/// <summary>
		/// Gets a value indicating whether the control is currently recording audio.
		/// </summary>
		/// <value>Returns TRUE if the control is recording voice audio, else returns FALSE.</value>
#if DESIGN
		[Browsable(false)]
#endif
		public bool IsRecording
		{
			get
			{
#if !DESIGN
				return Convert.ToBoolean((int)Win32Window.SendMessage(ChildHandle,(int)InkXMessage.IsVoiceRecording, 0, 0));
#else
				return false;
#endif
			}
		}
		#endregion


		#region Clear Method
		/// <summary>
		/// Clears the contents of the InkX control.
		/// </summary>
		public void Clear()
		{
#if !DESIGN
			Win32Window.SendMessage(ChildHandle,(int)InkXMessage.ClearAll, 0, 0);
#endif
		}
		#endregion

		#region Play Method
		/// <summary>
		/// Plays the currently selected voice object.
		/// </summary>
		public void Play()
		{
#if !DESIGN
			Win32Window.SendMessage(ChildHandle,(int)InkXMessage.VoicePlay, 0, 0);
#endif
		}
		#endregion

		#region Stop Method
		/// <summary>
		/// Stops the currently playing audio (if applicable).
		/// </summary>
		public void Stop()
		{
#if !DESIGN
			Win32Window.SendMessage(ChildHandle,(int)InkXMessage.VoiceStop, 0, 0);
#endif
		}
		#endregion

		#region Record Method
		/// <summary>
		/// Starts recording a new voice clip.
		/// </summary>
		public void Record()
		{
#if !DESIGN
			//ensure audio is stopped first
			Stop();
			Win32Window.SendMessage(ChildHandle,(int)InkXMessage.VoiceRecord, 0, 0);
#endif
		}
		#endregion

	
		#region InkX Control Functions

#if !DESIGN
		[DllImport("inkx.dll", EntryPoint="InitInkX", SetLastError=true)] 
		private static extern void InitInkX();
#endif
		#endregion


		#region InkXMessage Enumeration

		private enum InkXMessage : int
		{
			/// <summary>
			/// Base message
			/// </summary>
			WM_USER = 1024,
			
			/// <summary>
			/// Required 
			/// </summary>
			WM_SETTEXT = 0x000C,
			
			/// <summary>
			/// Used to return the length of the data.
			/// </summary>
			GetDataLen  = (WM_USER + 514),

			/// <summary>
			/// Used to retrieve the data.
			/// </summary>
			GetData      = (WM_USER + 515),

			/// <summary>
			/// Used to set the data.
			/// Stores the Ink data from a previous IM_GETDATA call which will be inserted into the current compose window.
			/// </summary>
			SetData     = (WM_USER + 516),

			/// <summary>
			/// Used to erase all contents from the current compose window.
			/// </summary>
			ClearAll    = (WM_USER + 519),

			/// <summary>
			/// Sent to toggle the VoiceBar state
			/// </summary>
			VoiceBar    = (WM_USER + 530),

			/// <summary>
			/// Used to get the window handle to the RichInk control.
			/// </summary>
			GetRichInk   = (WM_USER + 532),

			/// <summary>
			/// Returns TRUE if we're currently recording
			/// </summary>
			IsVoiceRecording   = (WM_USER + 536),

			/// <summary>
			/// Causes the voicebar to halt playback or recording (if either is in progress).
			/// </summary>
			VoiceStop   = (WM_USER + 537),

			/// <summary>
			/// Plays a currently selected voice object
			/// </summary>
			VoicePlay  = (WM_USER + 540),

			/// <summary>
			/// This will begin recording to an inline embedded object.
			/// </summary>
			VoiceRecord = (WM_USER + 541),

			/// <summary>
			/// Returns TRUE if we're currently playing voice data.
			/// </summary>
			IsVoicePlaying = (WM_USER + 542),
		}
		#endregion

		#region InkXStyle Enumeration
		/// <summary>
		/// Window Styles for the InkX control.
		/// </summary>
		private enum InkXStyle : int
		{
			/// <summary>
			/// Don't create a VoiceBar.
			/// </summary>
			NoVoiceBar           = 0x0200,
			/// <summary>
			/// VoiceBar appears at bottom of the control.
			/// </summary>
			BottomVoiceBar       = 0x0400,
		}
		#endregion

		#region InkXNotification Enumeration
		/// <summary>
		/// Notifications received from the native control
		/// </summary>
		private enum InkXNotification : int
		{
			WM_USER = 1024,
		}
		#endregion


		#region INotifiable Members

#if !NDOC && !DESIGN
		protected override void OnNotifyMessage(ref Microsoft.WindowsCE.Forms.Message m)
		{
			if(m.Msg == 0x004E)
			{
				NMHDR nh = (NMHDR)Marshal.PtrToStructure(m.LParam, typeof(OpenNETCF.Win32.NMHDR));
			}

			base.OnNotifyMessage (ref m);
		}
#endif
		#endregion
	}
}
