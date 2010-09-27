//==========================================================================================
//
//		OpenNETCF.WindowsCE.Forms.NotificationMessageWindow
//		Copyright (c) 2003-2004, OpenNETCF.org
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

// Preliminary code 02/10/2004 Peter Foot

using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace OpenNETCF.WindowsCE.Forms
{
	//exclude from documentation
#if !NDOC && !DESIGN
	using Microsoft.WindowsCE.Forms;

	/// <summary>
	/// Handles messages received from the Notification system and throws events in the parent NotificationEngine object
	/// </summary>
	internal class NotificationMessageWindow : MessageWindow
	{
		private Hashtable m_notifications;

		public NotificationMessageWindow(Hashtable notifications)
		{
			m_notifications = notifications;
		}

		protected override void WndProc(ref Message m)
		{
			switch(m.Msg)
			{
					//WM_NOTIFY
				case 78:
					NMSHN nm = (NMSHN)Marshal.PtrToStructure(m.LParam, typeof(NMSHN));
					Notification n = (Notification)m_notifications[nm.idFrom];
			
				switch(nm.code)
				{
					case SHNN.DISMISS:
						n.OnBalloonChanged(new BalloonChangedEventArgs(false));
						break;
					case SHNN.SHOW:
						n.OnBalloonChanged(new BalloonChangedEventArgs(true));
						break;
					case SHNN.LINKSEL:
						string link = Marshal.PtrToStringUni((IntPtr)nm.union1);
						n.OnResponseSubmitted(new ResponseSubmittedEventArgs(link));
						break;
				}
					break;
					//WM_COMMAND
				case 0x0111:
					//nm = new NotificationEventArgs((int)m.LParam, (int)m.WParam);
					//m_parent.OnNotificationSelect(nm);
					//Console.WriteLine(m.LParam.ToString() + " " + m.WParam.ToString());
					break;
			}
			
			//do base wndproc
			base.WndProc (ref m);
		}

	}
#endif


	internal enum SHNN : int
	{
		LINKSEL = -1000,
		DISMISS = -1001,
		SHOW = -1002,
	}
}