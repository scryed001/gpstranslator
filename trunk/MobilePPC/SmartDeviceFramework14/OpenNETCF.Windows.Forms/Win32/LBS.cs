//==========================================================================================
//
//		OpenNETCF.Win32.LBS
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
	/// Listbox control styles.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	[Flags()]
	public enum LBS : int
	{
			
		NOTIFY           = 0x0001,
		SORT             = 0x0002,
		NOREDRAW         = 0x0004,
		MULTIPLESEL      = 0x0008,
		HASSTRINGS       = 0x0040,
		USETABSTOPS      = 0x0080,
		NOINTEGRALHEIGHT = 0x0100,
		MULTICOLUMN      = 0x0200,
		WANTKEYBOARDINPUT= 0x0400,
		EXTENDEDSEL      = 0x0800,
		DISABLENOSCROLL  = 0x1000,
		NODATA           = 0x2000,
		STANDARD         = (NOTIFY | SORT | WS.VSCROLL | WS.BORDER)
	}

}
