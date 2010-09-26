//==========================================================================================
//
//		OpenNETCF.Windows.Forms.PowerLineStatus
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
	/// Power line status.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	/// <remarks>Used by <see cref="PowerStatus"/> class.</remarks>
	public enum PowerLineStatus : byte
	{
		/// <summary>
		/// AC power is offline.
		/// </summary>
		Offline         = 0x00,
		/// <summary>
		/// AC power is online.
		/// </summary>
		Online			= 0x01,
		/// <summary>
		/// Unit is on backup power.
		/// </summary>
		BackupPower		= 0x02,
		/// <summary>
		/// AC line status is unknown.
		/// </summary>
		Unknown			= 0xFF,
	}
}
