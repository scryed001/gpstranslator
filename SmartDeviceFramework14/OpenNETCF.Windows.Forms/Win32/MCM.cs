//==========================================================================================
//
//		OpenNETCF.Win32.MCM
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
	/// MonthCalendar Messages.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	internal enum MCM : int
	{
		FIRST				= 0x1000,
		GETCURSEL			= (FIRST + 1),
		SETCURSEL			= (FIRST + 2),
		GETMAXSELCOUNT		= (FIRST + 3),
		SETMAXSELCOUNT		= (FIRST + 4),
		GETSELRANGE			= (FIRST + 5),
		SETSELRANGE			= (FIRST + 6),
		GETMONTHRANGE		= (FIRST + 7),
		SETDAYSTATE			= (FIRST + 8),
		GETMINREQRECT		= (FIRST + 9),
		SETCOLOR            = (FIRST + 10),
		GETCOLOR            = (FIRST + 11),
		SETTODAY			= (FIRST + 12),
		GETTODAY			= (FIRST + 13),
		HITTEST				= (FIRST + 14),
		SETFIRSTDAYOFWEEK	= (FIRST + 15),
		GETFIRSTDAYOFWEEK	= (FIRST + 16),
		GETRANGE			= (FIRST + 17),
		SETRANGE			= (FIRST + 18),
		GETMONTHDELTA		= (FIRST + 19),
		SETMONTHDELTA		= (FIRST + 20),
		GETMAXTODAYWIDTH	= (FIRST + 21),
		GETMAXNONEWIDTH		= (FIRST + 22),
	}
}
