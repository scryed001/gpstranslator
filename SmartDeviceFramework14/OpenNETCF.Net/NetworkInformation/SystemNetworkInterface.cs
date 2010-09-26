//==========================================================================================
//
//		OpenNETCF.Net.NetworkInformation.SystemNetworkInterface
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
using System.Collections;
using System.Text;
using OpenNETCF.Net;

namespace OpenNETCF.Net.NetworkInformation
{
	internal class SystemNetworkInterface : NetworkInterface
	{
		internal uint index;
		SystemIPInterfaceProperties interfaceProperties;

		private SystemNetworkInterface(IpAdapterInfo adapterInfo)
		{
			this.index = adapterInfo.index;
			this.interfaceProperties = new SystemIPInterfaceProperties(adapterInfo);
		}

		internal static SystemNetworkInterface[] GetNetworkInterfaces()
		{
            ArrayList al = new ArrayList();

            AdapterCollection ac = Networking.GetAdapters();
			foreach(Adapter a in ac)
			{
				IpAdapterInfo i = new IpAdapterInfo();
				i.index = (uint)a.Index;
				al.Add(new SystemNetworkInterface(i));
			}

			return (SystemNetworkInterface[])al.ToArray(typeof(SystemNetworkInterface));
		}

		public override IPInterfaceProperties GetIPInterfaceProperties()
		{
			return this.interfaceProperties;
		}
	}
}
