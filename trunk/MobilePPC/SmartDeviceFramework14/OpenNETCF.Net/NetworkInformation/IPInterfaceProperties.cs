//==========================================================================================
//
//		OpenNETCF.Net.NetworkInformation.IPInterfaceProperties
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
	/// Provides information about network interfaces that support Internet Protocol version 4 (IPv4) or Internet Protocol version 6 (IPv6).
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	public abstract class IPInterfaceProperties
	{
		protected IPInterfaceProperties()
		{
		}

		/// <summary>
		/// Provides Internet Protocol (IP) statistical data for the network interface.
		/// </summary>
		/// <returns>An <see cref="IPInterfaceStatistics"/> object that provides traffic statistics for this network interface.</returns>
		public abstract IPInterfaceStatistics GetIPInterfaceStatistics();
	}
}
