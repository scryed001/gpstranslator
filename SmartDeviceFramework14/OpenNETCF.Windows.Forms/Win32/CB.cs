//==========================================================================================
//
//		OpenNETCF.Win32.CB
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
	/// ComboBox Messages.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	public enum CB: int
	{
		GETEDITSEL              = 0x0140,
		LIMITTEXT               = 0x0141,
		SETEDITSEL              = 0x0142,
		ADDSTRING               = 0x0143,
		DELETESTRING            = 0x0144,
		GETCOUNT                = 0x0146,
		GETCURSEL               = 0x0147,
		GETLBTEXT               = 0x0148,
		GETLBTEXTLEN            = 0x0149,
		INSERTSTRING            = 0x014A,
		RESETCONTENT            = 0x014B,
		FINDSTRING              = 0x014C,
		SELECTSTRING            = 0x014D,
		SETCURSEL               = 0x014E,
		SHOWDROPDOWN            = 0x014F,
		GETITEMDATA             = 0x0150,
		SETITEMDATA             = 0x0151,
		GETDROPPEDCONTROLRECT   = 0x0152,
		SETITEMHEIGHT           = 0x0153,
		GETITEMHEIGHT           = 0x0154,
		SETEXTENDEDUI           = 0x0155,
		GETEXTENDEDUI           = 0x0156,
		GETDROPPEDSTATE         = 0x0157,
		FINDSTRINGEXACT         = 0x0158,
		SETLOCALE               = 0x0159,
		GETLOCALE               = 0x015A,
		GETTOPINDEX             = 0x015b,
		SETTOPINDEX             = 0x015c,
		GETHORIZONTALEXTENT     = 0x015d,
		SETHORIZONTALEXTENT     = 0x015e,
		GETDROPPEDWIDTH         = 0x015f,
		SETDROPPEDWIDTH         = 0x0160,
		INITSTORAGE             = 0x0161
	}
}
