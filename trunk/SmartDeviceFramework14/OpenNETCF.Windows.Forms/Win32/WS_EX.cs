//==========================================================================================
//
//		OpenNETCF.Win32.WS_EX
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
	/// Extended control styles.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	[Flags()]
	public enum WS_EX
	{
		/// <summary>
		/// Specifies that a window created with this style accepts drag-drop files. 
		/// </summary>
		ACCEPTFILES,

		/// <summary>
		/// Forces a top-level window onto the taskbar when the window is visible.  
		/// </summary>
		APPWINDOW,

		/// <summary>
		/// Specifies that a window has a border with a sunken edge. 
		/// </summary>
		CLIENTEDGE = 0x00000200, 
			
		/// <summary>
		/// Creates a window that has a double border; the window can, optionally, be created with a title bar by specifying the WS_CAPTION style in the dwStyle parameter. 
		/// </summary>
		DLGMODALFRAME = 0x00000001,
	
		/// <summary>
		/// Creates a window that has generic "left-aligned" properties. This is the default. 
		/// </summary>
		LEFT = 0,
	
		/// <summary>
		/// If the shell language is Hebrew, Arabic, or another language that supports reading order alignment, the vertical scroll bar (if present) is to the left of the client area. For other languages, the style is ignored. 
		/// </summary>
		LEFTSCROLLBAR = 0x00004000,
	
		/// <summary>
		/// The window text is displayed using left-to-right reading-order properties. This is the default. 
		/// </summary>
		LTRREADING = 0,
	
		/// <summary>
		/// Creates an MDI child window. 
		/// </summary>
		MDICHILD = 0x00000040,
	
		/// <summary>
		/// A top-level window created with this style cannot be activated. If a child window has this style, tapping it does not cause its top-level parent to be activated. A window that has this style receives stylus events, but neither it nor its child windows can get the focus. Supported in Windows CE versions 2.0 and later. 
		/// </summary>
		NOACTIVATE = 0x08000000,
	
		/// <summary>
		/// A window created with this style does not show animated exploding and imploding rectangles, and does not have a button on the taskbar. Supported in Windows CE versions 2.0 and later. 
		/// </summary>
		NOANIMATION = 0x04000000,
	
		/// <summary>
		/// Specifies that a child window created with this style does not send the WM_PARENTNOTIFY message to its parent window when it is created or destroyed. 
		/// </summary>
		NOPARENTNOTIFY = 0x00000004,
	
		/// <summary>
		/// Combines the CLIENTEDGE and WINDOWEDGE styles. 
		/// </summary>
		OVERLAPPEDWINDOW = WINDOWEDGE | CLIENTEDGE,
	
		/// <summary>
		/// Combines the WINDOWEDGE, TOOLWINDOW, and TOPMOST styles. 
		/// </summary>
		PALETTEWINDOW = WINDOWEDGE | TOOLWINDOW | TOPMOST,
	
		/// <summary>
		/// The window has generic "right-aligned" properties. This depends on the window class. This style has an effect only if the shell language is Hebrew, Arabic, or another language that supports reading-order alignment; otherwise, the style is ignored. 
		/// </summary>
		RIGHT = 0x00001000,
	
		/// <summary>
		/// Vertical scroll bar (if present) is to the right of the client area. This is the default. 
		/// </summary>
		RIGHTSCROLLBAR = 0, // Cannot find value in header file.
	
		/// <summary>
		/// If the shell language is Hebrew, Arabic, or another language that supports reading-order alignment, the window text is displayed using right-to-left reading-order properties. For other languages, the style is ignored. 
		/// </summary>
		RTLREADING = 0x00002000,
	
		/// <summary>
		/// Creates a window with a three-dimensional border style intended to be used for items that do not accept user input. 
		/// </summary>
		STATICEDGE = 0x00020000,
	
		/// <summary>
		/// Creates a tool window; that is, a window intended to be used as a floating toolbar. A tool window has a title bar that is shorter than a normal title bar, and the window title is drawn using a smaller font. A tool window does not appear in the taskbar or in the dialog that appears when the user presses ALT+TAB. If a tool window has a system menu, its icon is not displayed on the title bar. However, you can display the system menu by right-clicking or by typing ALT+SPACE.  
		/// </summary>
		TOOLWINDOW = 0x00000080,
	
		/// <summary>
		/// Specifies that a window created with this style should be placed above all non-topmost windows and should stay above them, even when the window is deactivated. To add or remove this style, use the SetWindowPos function. 
		/// </summary>
		TOPMOST = 0x00000008,
	
		/// <summary>
		/// Specifies that a window created with this style should not be painted until siblings beneath the window (that were created by the same thread) have been painted. The window appears transparent because the bits of underlying sibling windows have already been painted. To achieve transparency without these restrictions, use the SetWindowRgn function.
		/// </summary>
		TRANSPARENT = 0x00000020,
	
		/// <summary>
		/// Specifies that a window has a border with a raised edge. 
		/// </summary>
		WINDOWEDGE = 0x00000100,

		/// <summary>
		/// Additional value for Completeness
		/// </summary>
		NONE = 0x00000000
	}

}