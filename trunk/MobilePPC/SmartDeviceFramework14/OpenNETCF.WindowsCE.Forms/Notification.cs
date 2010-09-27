//==========================================================================================
//
//		OpenNETCF.WindowsCE.Forms.Notification
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

// 02/10/2004 Peter Foot

using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using OpenNETCF.Runtime.InteropServices;

namespace OpenNETCF.WindowsCE.Forms
{
	/// <summary>
	/// Implements Windows CE functionality for displaying and responding to user notifications.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	/// <remarks>This class provides a managed implementation of the Windows CE notification functions.
	/// This class is supported only on the Pocket PC.
	/// <para>You can create notifications and then display them as needed using the <see cref="Visible"/> property.
	/// The <see cref="InitialDuration"/> property sets the time the message balloon initially displays.
	/// If you set <see cref="InitialDuration"/> to zero and Visible to true, the message balloon does not appear but its icon is available in the title bar for reactivation when clicked.
	/// The <see cref="BalloonChanged"/> event occurs whenever the balloon is shown or hidden, either programmatically using the <see cref="Visible"/> property or through user interaction.
	/// In addition to plain text, you can create a user notification with HTML content in the message balloon.
	/// The HTML is rendered by the Pocket PC HTML control, and you can respond to values in an HTML form by parsing a response string provided by the <see cref="ResponseSubmittedEventArgs"/> class, through the <see cref="ResponseSubmittedEventArgs.Response"/> property.</para>
	/// <b>Cmd:2 Identifier</b>
	/// <para>The identifier "cmd:2" has a special purpose in Windows CE and is used to dismiss notifications.
	/// If cmd:2 is the name of an HTML button or other element in a message balloon, the <see cref="ResponseSubmitted"/> event is not raised.
	/// The notification is dismissed, but its icon is placed on the title bar to be responded to at a later time.</para></remarks>
#if DESIGN
	[ToolboxItemFilter("NETCF",ToolboxItemFilterType.Require),
	ToolboxItemFilter("System.CF.Windows.Forms", ToolboxItemFilterType.Custom)]
#endif
	public class Notification : Component
	{

#if !DESIGN
		private static NotificationMessageWindow msgwnd;
		private static Hashtable notifications;
		private static int id = 0;
		private static IntPtr hIcon;
		private static Guid clsid = GuidEx.NewGuid();

		
		private SHNOTIFICATIONDATA m_data;
		private IntPtr mIcon = hIcon;
#endif
		private int mDuration = 10;
		private string mText = string.Empty;
		private string mCaption = string.Empty;
		private bool mCritical = false;
		private bool mVisible = false;


		static Notification()
		{
#if !DESIGN
			notifications = new Hashtable();

			msgwnd = new NotificationMessageWindow(notifications);

			//grab the icon from the calling EXE
			ExtractIconEx(System.Reflection.Assembly.GetCallingAssembly().GetModules()[0].FullyQualifiedName, 0, 0, ref hIcon, 1);
#endif
		}

		
#if !DESIGN
		/// <summary>
		/// Initializes a new instance of the <see cref="Notification"/> class.
		/// </summary>
		/// <remarks>This class is not supported on the Smartphone or other Windows CE devices that are not Pocket PCs.
		/// You can create multiple notifications, such as an array of notifications, and display them as needed with the Visible property.</remarks>
		public Notification()
		{

			m_data = new SHNOTIFICATIONDATA();
			m_data.clsid = clsid;

			m_data.hwndSink = msgwnd.Hwnd;
			m_data.dwID = id;
			m_data.cbStruct = Marshal.SizeOf(m_data);
			notifications.Add(id, this);
			id++;

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			//remove from notification collection
			Visible = false;
			notifications.Remove(m_data.dwID);

			

			base.Dispose(disposing);
		}

		private void Show()
		{
			m_data.pszHTML = MarshalEx.StringToHGlobalUni(mText);
			m_data.pszTitle = MarshalEx.StringToHGlobalUni(mCaption);
			m_data.hicon = mIcon;
			m_data.csDuration = mDuration;

			if(mDuration==0)
			{
				m_data.npPriority = SHNP.ICONIC;
			}
			else
			{
				m_data.npPriority = SHNP.INFORM;
			}

			if(mCritical)
			{
				m_data.grfFlags |= SHNF.CRITICAL;
			}
			else
			{
				m_data.grfFlags ^= (m_data.grfFlags & SHNF.CRITICAL);
			}

			int hresult = SHNotificationAdd(ref m_data);

			if(m_data.pszTitle!=IntPtr.Zero)
			{
				MarshalEx.FreeHGlobal(m_data.pszTitle);
				m_data.pszTitle = IntPtr.Zero;
			}
			if(m_data.pszHTML!=IntPtr.Zero)
			{
				MarshalEx.FreeHGlobal(m_data.pszHTML);
				m_data.pszHTML = IntPtr.Zero;
			}
		}

		private void Remove()
		{
			int hresult = SHNotificationRemove(ref clsid, m_data.dwID);
		}
#endif
		/// <summary>
		/// Gets or sets a string specifying the title text for the message balloon.
		/// </summary>
		/// <value>A string that specifies the caption text.
		/// The default value is an empty string ("").</value>
		/// <remarks>The background color of the caption is dependent on the current theme of the user.
		/// Use the <see cref="Text"/> property to modify the text in the body of the message balloon.</remarks>
#if DESIGN
		[DefaultValue(""),
		Description("Gets or sets a string specifying the title text for the message balloon.")]
#endif
		public string Caption
		{
			get
			{
				return mCaption;
			}
			set
			{
				mCaption = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the notification is of urgent importance.
		/// </summary>
		/// <value>true if the notification is critical; otherwise, false. The default is false.</value>
		/// <remarks>Critical notifications have a red background caption color, or other color, depending on the current Pocket PC theme.</remarks>
#if DESIGN
		[DefaultValue(false),
		Description("Gets or sets a value indicating whether the notification is of urgent importance.")]
#endif
		public bool Critical
		{
			get
			{
				return mCritical;
			}
			set
			{
				mCritical = value;
			}
		}

		/// <summary>
		/// Gets or sets the current native icon handle for the message balloon on the title bar.
		/// </summary>
#if !DESIGN

		public IntPtr IconHandle
		{
			get
			{
				return mIcon;
			}
			set
			{
				mIcon = value;
			}
		}
#endif

		/// <summary>
		/// Gets or sets the number of seconds the message balloon remains visible after initially shown.
		/// </summary>
		/// <remarks>The amount of time the balloon is visible, in seconds. The default is 10.</remarks>
#if DESIGN
		[DefaultValue(10),
		Description("Gets or sets the number of seconds the message balloon remains visible after initially shown.")]
#endif
		public int InitialDuration
		{
			get
			{
				return mDuration;
			}
			set
			{
				mDuration = value;
			}
		}

		/// <summary>
		/// Gets or sets the text for the message balloon.
		/// </summary>
#if DESIGN
		[DefaultValue(""),
		Description("Gets or sets the text for the message balloon.")]
#endif
		public string Text
		{
			get
			{
				return mText;
			}
			set
			{
				mText = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the message balloon is visible.
		/// </summary>
#if DESIGN
		[DefaultValue(false),
		Description("Gets or sets a value indicating whether the message balloon is visible.")]
#endif
		public bool Visible
		{
			get
			{
				return mVisible;
			}
			set
			{
#if !DESIGN
				if(mVisible != value)
				{
					mVisible = value;

					if(value)
					{
						Show();
					}
					else
					{
						Remove();
					}
				}
#endif
			}
		}

		/// <summary>
		/// Occurs when a message balloon is displayed or hidden.
		/// </summary>
#if DESIGN
		[Description("Occurs when a message balloon is displayed or hidden.")]
#endif
		public event BalloonChangedEventHandler BalloonChanged;

		internal void OnBalloonChanged(BalloonChangedEventArgs e)
		{
			//update visible state
			mVisible = e.Visible;

			if(this.BalloonChanged!=null)
			{
				this.BalloonChanged(this, e);
			}
		}
		/// <summary>
		/// Occurs when the user clicks a button or link in the message balloon.
		/// </summary>
#if DESIGN
		[Description("Occurs when the user clicks a button or link in the message balloon.")]
#endif
		public event ResponseSubmittedEventHandler ResponseSubmitted;

		internal void OnResponseSubmitted(ResponseSubmittedEventArgs e)
		{
			if (this.ResponseSubmitted != null)
			{
				this.ResponseSubmitted(this, e);
			}
		}

		#region API Declares

		//Add
		[DllImport("aygshell.dll", EntryPoint="#155", SetLastError=true)]
		private static extern int SHNotificationAdd(ref SHNOTIFICATIONDATA shinfo);
		
		//Remove
		[DllImport("aygshell.dll",EntryPoint="#157", SetLastError=true)]
		private static extern int SHNotificationRemove(ref Guid clsid, int dwID);
		
		//Update
		[DllImport("aygshell.dll", EntryPoint="#156", SetLastError=true)]
		private static extern int SHNotificationUpdate(int grnumUpdateMask, ref SHNOTIFICATIONDATA shinfo);
		
		//Get Data
		[DllImport("aygshell.dll", EntryPoint="#173", SetLastError=true)]
		private static extern int SHNotificationGetData(ref Guid clsid, int dwID, ref SHNOTIFICATIONDATA shinfo);
		
		//icon
		[DllImport("coredll.dll", SetLastError=true)]
		private static extern IntPtr ExtractIconEx(string fileName, int index, int hIconLarge, ref IntPtr hIconSmall, uint nIcons);
		
		#endregion

 

	}

}
