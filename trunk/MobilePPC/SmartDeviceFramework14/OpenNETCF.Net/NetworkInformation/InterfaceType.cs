//==========================================================================================
//
//		OpenNETCF.Net.NetworkInformation.InterfaceType
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
	/// Specifies types of network interfaces.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	public enum InterfaceType
	{
		AsymmetricDsl				= 0x5e,
		Atm							= 0x25,
		BasicIsdn					= 20,
		Ethernet					= 0x06,
		Ethernet3MegaBit			= 0x1a,
		FastEthernetFx				= 0x45,
		FastEthernetT				= 0x3e,
		Fddi						= 15,
		GenericModem				= 0x30,
		GigaBitEthernet				= 0x75,
		HighPerformanceSerialBus	= 0x90,
		IPOverAtm					= 0x72,
		Isdn						= 0x3f,
		Loopback					= 0x18,
		MultiRateSymmetricDsl		= 0x8f,
		Ppp							= 0x17,
		PrimaryIsdn					= 0x15,
		RateAdaptDsl				= 0x5f,
		Slip						= 0x1c,
		SymmetricDsl				= 0x60,
		TokenRing					= 0x09,
		Tunnel						= 0x83,
		Unknown						= 0x01,
		VeryHighSpeedDsl			= 0x61,
		Wireless80211				= 0x47
	}
}
