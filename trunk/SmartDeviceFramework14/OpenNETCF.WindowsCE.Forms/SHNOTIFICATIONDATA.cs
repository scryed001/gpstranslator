//==========================================================================================
//
//		OpenNETCF.WindowsCE.Forms.SHNOTIFICATIONDATA
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

// Preliminary code 02/10/2004 Peter Foot

using System;
using System.Runtime.InteropServices;
using OpenNETCF.Runtime.InteropServices;

namespace OpenNETCF.WindowsCE.Forms
{
	
	internal struct SHNOTIFICATIONDATA
	{
		// for verification and versioning
		public int     cbStruct;
		// identifier for this particular notification
		public int      dwID;
		// priority
		public SHNP		npPriority;
		/// <summary>
		/// Duration to display the bubble (in seconds).
		/// </summary>
		public int		csDuration;
		// the icon for the notification
		public IntPtr	hicon;
		/// <summary>
		/// Flags that affect the behaviour of the Notification bubble
		/// </summary>
		public SHNF	grfFlags;
		// unique identifier for the notification class
		public Guid		clsid;
		// window to receive command choices, dismiss, etc.
		public IntPtr	hwndSink;
		// HTML content for the bubble
		public IntPtr	pszHTML;
		// Optional title for bubble
		public IntPtr	pszTitle;
		/// <summary>
		/// User defined parameter
		/// </summary>
		public int		lParam;
	}

	internal enum SHNP :int
	{
		/// <summary>
		/// Bubble shown for duration, then goes away
		/// </summary>
		INFORM = 0x1B1,
		/// <summary>
		/// No bubble, icon shown for duration then goes away
		/// </summary>
		ICONIC = 0,
	}

	/// <summary>
	/// Flags that affect the display behaviour of the Notification
	/// </summary>
	internal enum SHNF : int
	{
		/// <summary>
		/// For SHNP_INFORM priority and above, don't display the notification bubble when it's initially added;
		/// the icon will display for the duration then it will go straight into the tray.
		/// The user can view the icon / see the bubble by opening the tray.
		/// </summary>
		STRAIGHTTOTRAY  = 0x00000001,
		/// <summary>
		/// Critical information - highlights the border and title of the bubble.
		/// </summary>
		CRITICAL        = 0x00000002,
		/// <summary>
		/// Force the message (bubble) to display even if settings says not to.
		/// </summary>
		FORCEMESSAGE    = 0x00000008,
		/// <summary>
		/// Force the display to turn on for notification. Added for Windows Mobile 2003.
		/// </summary>
		DISPLAYON       = 0x00000010,
		/// <summary>
		/// Force the notification to be silent and not vibrate, regardless of Settings. Added for Windows Mobile 2003.
		/// </summary>
		SILENT          = 0x00000020,

	}

	internal struct NMSHN
	{
		public IntPtr hwndFrom; 
		public int idFrom; 
		public SHNN code; 
		public int lParam;
		public int dwReturn;
		public int union1;
		public int union2;
	}
}