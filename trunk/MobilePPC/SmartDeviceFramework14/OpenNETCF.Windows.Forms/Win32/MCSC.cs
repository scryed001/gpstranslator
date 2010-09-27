//==========================================================================================
//
//		OpenNETCF.Win32.MCSC
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
	/// MonthCalendar Colors.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	internal enum MCSC :int
	{
		BACKGROUND   = 0,   // the background color (between months)
		TEXT         = 1,   // the dates
		TITLEBK      = 2,   // background of the title
		TITLETEXT    = 3,
		MONTHBK      = 4,   // background within the month cal
		TRAILINGTEXT = 5,   // the text color of header & trailing days
	}
}
