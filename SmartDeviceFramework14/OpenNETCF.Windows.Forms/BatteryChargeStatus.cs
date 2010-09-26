//==========================================================================================
//
//		OpenNETCF.Windows.Forms.BatteryChargeStatus
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

namespace OpenNETCF.Windows.Forms
{

	/// <summary>
	/// Defines identifiers that indicate the current battery charge level or charging state information.
	/// </summary>
	[Flags()]
	public enum BatteryChargeStatus : byte
	{
		/// <summary>
		/// Indicates a high level of battery charge.
		/// </summary>
		High		= 0x01,
		/// <summary>
		/// Indicates a low level of battery charge.
		/// </summary>
		Low			= 0x02,
		/// <summary>
		/// Indicates a critically low level of battery charge.
		/// </summary>
		Critical	= 0x04,
		/// <summary>
		/// Indicates a battery is charging.
		/// </summary>
		Charging	= 0x08,
		/// <summary>
		/// Indicates that no battery is present.
		/// </summary>
		NoSystemBattery	= 0x80,
		/// <summary>
		/// Indicates an unknown battery condition.
		/// </summary>
		Unknown		= 0xFF,

	}
}
