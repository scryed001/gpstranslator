//==========================================================================================
//
//		OpenNETCF.Net.NetworkInformation.NetworkInterface
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
	/// Provides configuration and statistical information for a network interface.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	public abstract class NetworkInterface
	{
		protected NetworkInterface()
		{
			
		}

		/// <summary>
		/// Returns objects that describe the network interfaces on the local computer.
		/// </summary>
		/// <returns></returns>
		public static NetworkInterface[] GetAllNetworkInterfaces()
		{
			return SystemNetworkInterface.GetNetworkInterfaces();
		}

		/// <summary>
		/// Returns an object that describes the configuration of this network interface.
		/// </summary>
		/// <returns></returns>
		public abstract IPInterfaceProperties GetIPInterfaceProperties();

//		// Properties
//		public abstract string Description { get; }
//		public abstract bool IsReceiveOnly { get; }
//		public abstract string Name { get; }
//		public abstract OperationalStatus OperationalStatus { get; }
//		public abstract long Speed { get; }
//		public abstract bool SupportsMulticast { get; }
//		public abstract InterfaceType Type { get; }
	}
}
