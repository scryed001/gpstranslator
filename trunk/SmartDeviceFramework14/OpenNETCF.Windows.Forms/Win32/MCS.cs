//==========================================================================================
//
//		OpenNETCF.Win32.MCS
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
	/// MonthCalendar styles.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	internal enum MCS :int
	{
		DAYSTATE        = 0x0001,
		MULTISELECT     = 0x0002,
		WEEKNUMBERS     = 0x0004,
		SHOWNONE        = 0x0080,
		NOTODAYCIRCLE   = 0x0008,
		NOTODAY         = 0x0010,
	}
}
