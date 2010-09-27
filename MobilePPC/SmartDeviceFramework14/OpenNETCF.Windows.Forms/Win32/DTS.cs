//==========================================================================================
//
//		OpenNETCF.Win32.DTS
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
	/// DateTimePicker Styles.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	[Flags()]
	internal enum DTS : int
	{
		UPDOWN = 0x0001, // use UPDOWN instead of MONTHCAL
		SHOWNONE = 0x0002, // allow a NONE or checkbox selection
		SHORTDATEFORMAT = 0x0000, // use the short date format (app must forward WM_WININICHANGE messages)
		LONGDATEFORMAT = 0x0004, // use the long date format (app must forward WM_WININICHANGE messages)
		TIMEFORMAT = 0x0009, // use the time format (app must forward WM_WININICHANGE messages)
		APPCANPARSE = 0x0010, // allow user entered strings (app MUST respond to DTN_USERSTRING)
		RIGHTALIGN = 0x0020, // right-align popup instead of left-align it
		NONEBUTTON = 0x0080, // use NONE button instead of checkbox
	}
}
