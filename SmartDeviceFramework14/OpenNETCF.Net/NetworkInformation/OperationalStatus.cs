//==========================================================================================
//
//		OpenNETCF.Net.NetworkInformation.OperationalStatus
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
//
//Submitted by Sergey Bogdanov
//
using System;

namespace OpenNETCF.Net.NetworkInformation
{
	/// <summary>
	/// Specifies the operational state of a network interface.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	public enum OperationalStatus
	{
		/// <summary>
		/// The network interface is not in a condition to transmit data packets; it is waiting for an external event.
		/// </summary>
		Dormant			= 5,
		/// <summary>
		/// The network interface is unable to transmit data packets.
		/// </summary>
		Down			= 2,
		/// <summary>
		/// The network interface is unable to transmit data packets because it runs on top of one or more other interfaces, and at least one of these "lower layer" interfaces is down.
		/// </summary>
		LowerLayerDown	= 7,
		/// <summary>
		/// The network interface is unable to transmit data packets because of a missing component, typically a hardware component.
		/// </summary>
		NotPresent		= 6,
		/// <summary>
		/// The network interface is running tests.
		/// </summary>
		Testing			= 3,
		/// <summary>
		/// The network interface status is not known.
		/// </summary>
		Unknown			= 4,
		/// <summary>
		/// The network interface is up; it can transmit data packets.
		/// </summary>
		Up				= 1
	}
}
