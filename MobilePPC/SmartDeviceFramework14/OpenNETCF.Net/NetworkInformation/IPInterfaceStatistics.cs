//==========================================================================================
//
//		OpenNETCF.Net.NetworkInformation.IPInterfaceStatistics
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
	/// Provides statistical data for a network interface on the local computer.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	public abstract class IPInterfaceStatistics
	{
		protected IPInterfaceStatistics()
		{
		}

		/// <summary>
		/// Gets the number of bytes received on the interface.
		/// </summary>
		public abstract long BytesReceived { get; }
		/// <summary>
		/// Gets the number of bytes sent on the interface.
		/// </summary>
		public abstract long BytesSent { get; }
		/// <summary>
		/// Gets the number of incoming packets discarded.
		/// </summary>
		public abstract long IncomingPacketsDiscarded { get; }
		/// <summary>
		/// Gets the number of incoming packets with errors.
		/// </summary>
		public abstract long IncomingPacketsWithErrors { get; }
		/// <summary>
		/// Gets the number of incoming packets with an unknown protocol.
		/// </summary>
		public abstract long IncomingUnknownProtocolPackets { get; }
		/// <summary>
		/// Gets the number of non-unicast packets received on the interface.
		/// </summary>
		public abstract long NonUnicastPacketsReceived { get; }
		/// <summary>
		/// Gets the number of non-unicast packets sent on the interface.
		/// </summary>
		public abstract long NonUnicastPacketsSent { get; }
		/// <summary>
		/// Gets the number of outgoing packets that were discarded.
		/// </summary>
		public abstract long OutgoingPacketsDiscarded { get; }
		/// <summary>
		/// Gets the number of outgoing packets with errors.
		/// </summary>
		public abstract long OutgoingPacketsWithErrors { get; }
		/// <summary>
		/// Gets the length of the output queue.
		/// </summary>
		public abstract long OutputQueueLength { get; }
		/// <summary>
		/// Gets the number of unicast packets received on the interface.
		/// </summary>
		public abstract long UnicastPacketsReceived { get; }
		/// <summary>
		/// Gets the number of unicast packets sent on the interface.
		/// </summary>
		public abstract long UnicastPacketsSent { get; }
	}
}
