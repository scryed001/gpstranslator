//==========================================================================================
//
//		OpenNETCF.Win32.LVM
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
	/// ListView Messages.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	public enum LVM: int
	{
		FIRST = 0x1000,
		GETHEADER = FIRST + 31,
		SETICONSPACING = FIRST + 53,
		GETSUBITEMRECT = FIRST + 56,
		GETITEMSTATE = FIRST + 44,
		GETITEMTEXTW = FIRST + 115, 
		INSERTITEMA = FIRST + 7,
		INSERTITEMW = FIRST + 77,
		INSERTCOLUMNA = FIRST + 27,
		INSERTCOLUMNW = FIRST + 97,
		DELETECOLUMN = FIRST + 28, 
		GETCOLUMNA = FIRST + 25,
		GETCOLUMNW = FIRST + 95, 
		SETEXTENDEDLISTVIEWSTYLE = FIRST + 54, 
		SETITEMA = FIRST + 6,
		SETITEMW = FIRST + 76,
		EDITLABELA = FIRST + 23,
		EDITLABELW = FIRST + 118,
		DELETEITEM = FIRST + 8,
		SETBKCOLOR = FIRST + 1,
		GETBKCOLOR = FIRST + 0, 
		GETTEXTBKCOLOR = FIRST + 37,
		SETTEXTBKCOLOR = FIRST + 38,
		DELETEALLITEMS = FIRST + 9,
		GETNEXTITEM = FIRST + 12,
		SETITEMCOUNT = FIRST + 47,
		GETITEMCOUNT = FIRST + 4,
		SETCOLUMNWIDTH = FIRST + 30,
		GETITEMRECT = FIRST + 14,
		EDITLABEL = FIRST + 23,
	}

}
