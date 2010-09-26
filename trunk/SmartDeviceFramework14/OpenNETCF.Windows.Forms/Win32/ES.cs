//==========================================================================================
//
//		OpenNETCF.Win32.ES
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
	/// Edit control styles.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	[Flags()]
	public enum ES: int
	{
		LEFT        = 0x0000,
		CENTER      = 0x0001,
		RIGHT       = 0x0002,
		MULTILINE   = 0x0004,
		UPPERCASE   = 0x0008,
		LOWERCASE   = 0x0010,
		PASSWORD    = 0x0020,
		AUTOVSCROLL = 0x0040,
		AUTOHSCROLL = 0x0080,
		NOHIDESEL   = 0x0100,
		COMBOBOX    = 0x0200,
		OEMCONVERT  = 0x0400,
		READONLY    = 0x0800,
		WANTRETURN  = 0x1000,
		NUMBER      = 0x2000,
	}
}
