//==========================================================================================
//
//		OpenNETCF.Win32.HS
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
	/// HTML control styles.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	[Flags()]
	internal enum HS : int
	{
		NOFITTOWINDOW        = 0x0001,
		CONTEXTMENU          = 0x0002,
		CLEARTYPE            = 0x0004,
		NOSCRIPTING          = 0x0008,
		INTEGRALPAGING       = 0x0010,
		NOSCROLL             = 0x0020,
		NOIMAGES             = 0x0040,
		NOSOUNDS             = 0x0080,
		NOACTIVEX            = 0x0100,
		NOSELECTION          = 0x0200,
		NOFOCUSRECT          = 0x0400,
	}
}
