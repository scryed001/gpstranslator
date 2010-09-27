//==========================================================================================
//
//		OpenNETCF.Win32.CBS
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
	/// Combobox control styles.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	[Flags()]
	public enum CBS
	{
		DROPDOWN          = 0x0002,
		DROPDOWNLIST      = 0x0003,
		AUTOHSCROLL       = 0x0040,
		OEMCONVERT        = 0x0080,
		SORT              = 0x0100,
		HASSTRINGS        = 0x0200,
		NOINTEGRALHEIGHT  = 0x0400,
		DISABLENOSCROLL   = 0x0800,
		UPPERCASE         = 0x2000,
		LOWERCASE         = 0x4000,		
	}
}
