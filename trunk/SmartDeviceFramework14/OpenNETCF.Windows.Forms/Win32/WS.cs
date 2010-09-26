//==========================================================================================
//
//		OpenNETCF.Win32.WS
//		Copyright (c) 2005, OpenNETCF.org
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

namespace OpenNETCF.Win32
{
	/// <summary>
	/// Window style flags.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	[Flags()]
	public enum WS : int
	{
		/// <summary>
		/// Creates a window that has a thin-line border. 
		/// </summary>
		BORDER = 0x00800000,
 
		/// <summary>
		/// Creates a window that has a title bar (includes the BORDER style). 
		/// </summary>
		CAPTION = 0x00C00000,
 
		/// <summary>
		/// Creates a child window. This style cannot be used with the POPUP style. 
		/// </summary>
		CHILD = 0x40000000,
 
		/// <summary>
		/// Same as the CHILD style. 
		/// </summary>
		CHILDWINDOW = 0x40000000,
		/// <summary>
		/// Clips child windows relative to each other; that is, when a particular child window receives a WM_PAINT message, the CLIPSIBLINGS style clips all other overlapping child windows out of the region of the child window to be updated. If CLIPSIBLINGS is not specified and child windows overlap, it is possible, when drawing within the client area of a child window, to draw within the client area of a neighboring child window. 
		/// </summary>
		CLIPSIBLINGS = 0x04000000,
 
		/// <summary>
		/// Creates a window that is initially disabled. A disabled window cannot receive input from the user. 
		/// </summary>
		DISABLED = 0x08000000,
 
		/// <summary>
		/// Creates a window that has a border of a style typically used with dialog boxes. A window with this style cannot have a title bar. 
		/// </summary>
		DLGFRAME = 0x00400000,
 
		/// <summary>
		/// Specifies the first control of a group of controls. The group consists of this first control and all controls defined after it, up to the next control with the GROUP style. The first control in each group usually has the TABSTOP style so that the user can move from group to group. The user can subsequently change the keyboard focus from one control in the group to the next control in the group by using the direction keys. 
		/// </summary>
		GROUP = 0x00020000,
 
		/// <summary>
		/// Creates a window that has a horizontal scroll bar. 
		/// </summary>
		HSCROLL = 0x00100000,
 
		/// <summary>
		/// Creates a window that is initially minimized. Same as the MINIMIZE style. 
		/// </summary>
		ICONIC,
 
		/// <summary>
		/// Creates a window that is initially maximized. 
		/// </summary>
		MAXIMIZE,
 
		/// <summary>
		/// Creates a window that has a Maximize button. Cannot be combined with the EX_CONTEXTHELP style.  
		/// </summary>
		MAXIMIZEBOX = 0x00020000,
 
		/// <summary>
		/// Creates a window that is initially minimized. Same as the ICONIC style. 
		/// </summary>
		MINIMIZE,
 
		/// <summary>
		/// Creates a window that has a Minimize button. Cannot be combined with the EX_CONTEXTHELP style.  
		/// </summary>
		MINIMIZEBOX = 0x00010000,
 
		/// <summary>
		/// Creates an overlapped window. An overlapped window has a title bar and a border. Same as the TILED style. 
		/// </summary>
		OVERLAPPED = BORDER | CAPTION,
 
		/// <summary>
		/// Creates an overlapped window with the OVERLAPPED, CAPTION, SYSMENU, THICKFRAME, MINIMIZEBOX, and MAXIMIZEBOX styles. Same as the TILEDWINDOW style.  
		/// </summary>
		OVERLAPPEDWINDOW = OVERLAPPED,
 
		/// <summary>
		/// Creates a pop-up window. This style cannot be used with the CHILD style. 
		/// </summary>
		POPUP = unchecked((int)0x80000000),
 
		/// <summary>
		/// Creates a pop-up window with BORDER, POPUP, and SYSMENU styles. The CAPTION and POPUPWINDOW styles must be combined to make the window menu visible. 
		/// </summary>
		POPUPWINDOW = POPUP | BORDER | SYSMENU,
 
		/// <summary>
		/// Creates a window that has a sizing border. Same as the THICKFRAME style. 
		/// </summary>
		SIZEBOX = 0x00040000,
 
		/// <summary>
		/// Creates a window that has a Close (X) button in the non-client area. 
		/// </summary>
		SYSMENU = 0x00080000,
 
		/// <summary>
		/// Specifies a control that can receive the keyboard focus when the user presses the TAB key. Pressing the TAB key changes the keyboard focus to the next control with the TABSTOP style. 
		/// </summary>
		TABSTOP = 0x00010000,
 
		/// <summary>
		/// Creates a window that has a sizing border. Same as the SIZEBOX style. 
		/// </summary>
		THICKFRAME = 0x00040000,
 
		/// <summary>
		/// Creates an overlapped window. An overlapped window has a title bar and a border. Same as the OVERLAPPED style.  
		/// </summary>
		TILED = OVERLAPPED,
 
		/// <summary>
		/// Creates an overlapped window with the OVERLAPPED, CAPTION, SYSMENU, THICKFRAME, MINIMIZEBOX, and MAXIMIZEBOX styles. Same as the OVERLAPPEDWINDOW style.  
		/// </summary>
		TILEDWINDOW = OVERLAPPED | CAPTION | SYSMENU,
 
		/// <summary>
		/// Creates a window that is initially visible. 
		/// </summary>
		VISIBLE = 0x10000000,
 
		/// <summary>
		/// Creates a window that has a vertical scroll bar. 
		/// </summary>
		VSCROLL = 0x00200000
	}
}