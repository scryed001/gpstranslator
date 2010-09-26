//==========================================================================================
//
//		OpenNETCF.Win32.DTM
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
	/// DateTimePicker Messages.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	internal enum DTM : int
	{
	GETSYSTEMTIME = 0x1001,
	SETSYSTEMTIME = 0x1002,
	GETRANGE = 0x1003,
	SETRANGE = 0x1004,
	SETFORMATA = 0x1005,
	SETFORMATW = 0x1032,
	SETMCCOLOR = 0x1006,
	GETMCCOLOR = 0x1007,
	GETMONTHCAL = 0x1008,
	SETMCFONT = 0x1009,
	GETMCFONT = 0x100A,
	}
}
