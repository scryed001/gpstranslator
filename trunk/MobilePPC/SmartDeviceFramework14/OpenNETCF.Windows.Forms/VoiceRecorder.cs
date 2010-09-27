//==========================================================================================
//
//		OpenNETCF.Windows.Forms.VoiceRecorder
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
using System.Drawing;
using System.ComponentModel;
using System.Runtime.InteropServices;
using OpenNETCF.Runtime.InteropServices;
using OpenNETCF.Win32;
using OpenNETCF.Windows.Forms;

namespace OpenNETCF.Windows.Forms
{

	/// <summary>
	/// Specifies values representing possible states for an <see cref="VoiceRecorder">Voice Recorder</see> control.
	/// </summary>
	[Flags]
	public enum VoiceRecorderStyles : int
	{
		/// <summary>
		/// The <b>OK/CANCEL</b> is not displayed. 
		/// </summary>
		NoOKCancel	= 0x0001,
		/// <summary>
		/// The <b>OK</b> is not displayed. 
		/// </summary>
		NoOK		= 0x0008,
		/// <summary>
		/// No <b>RECORD</b> button displayed.
		/// </summary>
		NoRecord	= 0x0010,
		/// <summary>
		/// Immediatly play supplied file when launched.
		/// </summary>
		PlayMode	= 0x0020,
		/// <summary>
		/// Immediately record when launched.
		/// </summary>
		RecordMode	= 0x0080,
		/// <summary>
		/// Dismiss control when stopped.
		/// </summary>
		StopDismiss	= 0x0100
	}

#if DESIGN
    [DefaultProperty("FileName")]
#endif
	/// <summary>
	/// Allows voice recording and playback.
	/// <para><b>New in v1.1</b></para>
	/// </summary>
	public class VoiceRecorder : ControlEx
	{
		private VoiceRecorderData _vrd = new VoiceRecorderData();
		private string _filename = null;
		private VoiceRecorderStyles _styles;

		/// <summary>
		/// Creates a new <see cref="VoiceRecorder">Voice Recorder</see> control.
		/// </summary>
		public VoiceRecorder() : base(true)
		{
		}

#if !NDOC && !DESIGN
		IntPtr _vr;

		// if host control erase background message has been set 
		// then VoiceRecorder probably was closed.
		internal override void OnEraseBkgndMessage(ref Microsoft.WindowsCE.Forms.Message m)
		{
            Hide();	
			CleanFileName();
		}
#endif

		/// <summary>
		/// Shows <see cref="VoiceRecorder">Voice Recorder</see> control for a specific filename.
		/// </summary>
		/// <param name="fileName"></param>
		public void Show(string fileName)
		{
			if (fileName == null) throw new ArgumentNullException(fileName, "FileName must be set");

			base.Show();

#if !NDOC && !DESIGN
			_filename = fileName;
			CleanFileName();

			_vrd.lpszRecordFileName = MarshalEx.StringToHGlobalUni(_filename);
			_vrd.hwndParent = HostHandle;
			_vrd.Styles = _styles;

			_vr = VoiceRecorder_Create(_vrd);
			ShowWindow(_vr, 1);
			UpdateWindow(_vr);
			
			Rectangle r = Win32.Win32Window.GetWindowRect(_vr);
			Width = r.Width;
			Height = r.Height;
#else
			Width = 127;
			Height = 26;
#endif			
		}

		/// <summary>
		/// Show Voice Recorder with filename from <see cref="FileName" />.
		/// </summary>
		public new void Show()
		{
			Show(_filename);
		}

		/// <summary>
		/// Gets or sets the filename to which the voice recording is to be saved.
		/// </summary>
#if DESIGN
		[Browsable(true),
		Category("Behavior"),
		Description("The filename to which the voice recording is to be saved."),
		EditorBrowsable(EditorBrowsableState.Always)]
#endif
		public string FileName
		{
			get
			{
				return _filename;
			}
			set
			{
                _filename = value;				
			}
		}

		/// <summary>
		/// Gets or sets styles how the <see cref="VoiceRecorder">Voice Recorder</see> behaves.
		/// </summary>
#if DESIGN
		[Browsable(true),
		Category("Behavior"),
		Description("Describes how the Voice Recorder behaves."),
		EditorBrowsable(EditorBrowsableState.Always)]
#endif
		public VoiceRecorderStyles Styles
		{
			get
			{
				return _styles;
			}
			set
			{
				_styles = value;				
			}
		}

		private void CleanFileName()
		{
			//dispose native string
			if(_vrd.lpszRecordFileName != IntPtr.Zero)
			{
				MarshalEx.FreeHLocal(_vrd.lpszRecordFileName);
			}
		}

		/// <summary>
		/// Releases the resources used by the control.
		/// </summary>
		/// <param name="disposing">True to release both managed and unmanaged resources; False to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			CleanFileName();
			base.Dispose (disposing);
		}

		#region Events
		/// <summary>
		/// Occurs when started playback.
		/// </summary>
		public event EventHandler Play;
		/// <summary>
		/// Raises the <see cref="Play"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnPlay(EventArgs e)
		{
			if(this.Play!=null)
			{
				this.Play(this, e);
			}
		}

		/// <summary>
		/// Occurs when started recording.
		/// </summary>
		public event EventHandler Record;
		/// <summary>
		/// Raises the <see cref="Record"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnRecord(EventArgs e)
		{
			if(this.Record!=null)
			{
				this.Record(this, e);
			}
		}
		
		/// <summary>
		/// Occurs when ended playback/recording.
		/// </summary>
		public event EventHandler Stop;
		/// <summary>
		/// Raises the <see cref="Stop"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnStop(EventArgs e)
		{
			if(this.Stop!=null)
			{
				this.Stop(this, e);
			}
		}

		/// <summary>
		/// Occurs when user selected OK to save any recording file.
		/// </summary>
		public event EventHandler Ok;
		/// <summary>
		/// Raises the <see cref="Ok"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnOk(EventArgs e)
		{
			if(this.Ok!=null)
			{
				this.Ok(this, e);
			}
		}

		/// <summary>
		/// Occurs when user selected CANCEL.
		/// </summary>
		public event EventHandler Cancel;
		/// <summary>
		/// Raises the <see cref="Cancel"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnCancel(EventArgs e)
		{
			if(this.Cancel!=null)
			{
				this.Cancel(this, e);
			}
		}

		/// <summary>
		/// Occurs when error happend. 
		/// </summary>
		public event EventHandler Error;
		/// <summary>
		/// Raises the <see cref="Error"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnError(EventArgs e)
		{
			if(this.Error!=null)
			{
				this.Error(this, e);
			}
		}
		#endregion

#if !NDOC && !DESIGN
		/// <summary>
		/// Handles notification messages from the native control
		/// </summary>
		/// <param name="m">Message</param>
		protected override void OnNotifyMessage(ref Microsoft.WindowsCE.Forms.Message m)
		{
			//marshal data to custom nmhtml struct
			//Marshal.PtrToStructure doesn't work so marshalling items individually
			NM_VOICERECORDER nmvr = new NM_VOICERECORDER();
			nmvr.hwndFrom = (IntPtr)Marshal.ReadInt32(m.LParam, 0);
			nmvr.idFrom = Marshal.ReadInt32(m.LParam, 4);
			nmvr.code = (VoiceRecorderNotification)Marshal.ReadInt32(m.LParam, 8);
			nmvr.dwExtra = Marshal.ReadInt32(m.LParam, 12);

			//check the incoming message code and process as required
			switch(nmvr.code)
			{
				case VoiceRecorderNotification.VRN_PLAY_START:
					OnPlay(EventArgs.Empty);
					break;

				case VoiceRecorderNotification.VRN_PLAY_STOP:
				case VoiceRecorderNotification.VRN_RECORD_STOP:
					OnStop(EventArgs.Empty);
					break;

				case VoiceRecorderNotification.VRN_RECORD_START:
					OnRecord(EventArgs.Empty);
					break;

				case VoiceRecorderNotification.VRN_OK:
					OnOk(EventArgs.Empty);
					break;

				case VoiceRecorderNotification.VRN_CANCEL:
					OnCancel(EventArgs.Empty);
					break;

				case VoiceRecorderNotification.VRN_ERROR:
					OnError(EventArgs.Empty);
					break;
			}
		}
#endif

		[DllImport("voicectl.dll", EntryPoint="VoiceRecorder_Create")]
		private static extern IntPtr VoiceRecorder_Create(VoiceRecorderData lpVR);

		[DllImport("coredll.dll")]
		private static extern void ShowWindow(IntPtr hwnd, int shStatus);

		[DllImport("coredll.dll")]
		private static extern void UpdateWindow(IntPtr hwnd);

		[DllImport("coredll.dll")]
		private static extern bool IsWindow(IntPtr hwnd);

		/// <summary>
		/// Notification structure received from the voice control.
		/// </summary>
		private struct NM_VOICERECORDER
		{
			public IntPtr hwndFrom; 
			public int idFrom; 
			public VoiceRecorderNotification code;
			public int dwExtra;   
		}

		private enum VoiceRecorderNotification : int
		{
			VRN_FIRST           =   -860,    

			VRN_RECORD_START	=(VRN_FIRST-1),
			VRN_RECORD_STOP		=(VRN_FIRST-2),
			VRN_PLAY_START		=(VRN_FIRST-3),
			VRN_PLAY_STOP		=(VRN_FIRST-4),
			VRN_CANCEL			=(VRN_FIRST-5),
			VRN_OK				=(VRN_FIRST-6),
			VRN_ERROR			=(VRN_FIRST-7),
		}

		[Flags()]
		private enum PrivateVoiceRecorderStyle : int
		{
			VRS_NO_NOTIFY    = 0x0002,		// No parent Notifcation
			VRS_MODAL	     = 0x0004,		// Control is Modal     
			VRS_NO_MOVE		 = 0x0040		// Grip is removed and the control cannot be moved around by the user
		}

		private class VoiceRecorderData
		{
			private static int currentid = 1;

			private short						cb;                     
			private PrivateVoiceRecorderStyle	dwStyle;
			internal  int						xPos;
			internal  int						yPos;
			internal  IntPtr					hwndParent;
			private  int						id;
			internal  IntPtr					lpszRecordFileName;

			public VoiceRecorderStyles Styles
			{
				set
				{
                    dwStyle = (PrivateVoiceRecorderStyle)value | PrivateVoiceRecorderStyle.VRS_NO_MOVE;
				}
			}
			

			public VoiceRecorderData()
			{
				currentid++;

				cb = (short)Marshal.SizeOf(this);
				id = currentid;

				xPos = 0;
				yPos = 0;
				lpszRecordFileName = IntPtr.Zero;
			}

		}
	}
}
