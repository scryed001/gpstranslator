//==========================================================================================
//
//		OpenNETCF.Net.Network
//		Copyright (c) 2003, OpenNETCF.org
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
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;

using OpenNETCF.Runtime.InteropServices;

namespace OpenNETCF.Net
{
	/// <summary>
	/// Summary description for Network.
	/// </summary>
	public class Network
	{

		/// <summary>
		/// Maps the network resouce to the specified share name
		/// </summary>
		/// <param name="hwnd">Owner window handle</param>
		/// <param name="netRes">Network resource to connect</param>
		/// <param name="shareName">Share name</param>
		/// <param name="userName">User name</param>
		/// <param name="password">Password</param>
		public static void MapDrive(IntPtr hwnd, string netRes, string shareName, string userName, string password)
		{
			NETRESOURCE NetRes = new NETRESOURCE();			
			NetRes.dwScope = RESOURCE_GLOBALNET | RESOURCE_REMEMBERED;
			NetRes.dwType = RESOURCETYPE_DISK;
			NetRes.dwDisplayType = RESOURCEDISPLAYTYPE_SHARE;
			NetRes.dwUsage = RESOURCEUSAGE_CONNECTABLE;
			NetRes.lpRemoteName = MarshalEx.StringToHGlobalUni(netRes);
			NetRes.lpLocalName = MarshalEx.StringToHGlobalUni(shareName);
			NetRes.lpComment = IntPtr.Zero;
			NetRes.lpProvider = IntPtr.Zero;
				
			int ret = WNetAddConnection3(hwnd, NetRes, password, userName, 1);

			if (ret != 0)
			{
				throw new System.ComponentModel.Win32Exception(ret, ((NetworkErrors)ret).ToString());
			}
				
		}

		/// <summary>
		/// Disconnects the network resource
		/// </summary>
		/// <param name="shareName">Local share or remote name</param>
		/// <param name="force">Force disconnect</param>
		public static void Disconnect(string shareName, bool force)
		{
			if ((shareName!=null) && (shareName != String.Empty))
			{
				int ret = WNetCancelConnection2(shareName, 1, (force)?1:0);

				if (ret != 0)
				{
					throw new System.ComponentModel.Win32Exception(ret, ((NetworkErrors)ret).ToString());
				}
			}

		}

		/// <summary>
		/// Returns name of the network resource
		/// </summary>
		/// <param name="shareName"></param>
		/// <returns>Network resource</returns>
		public static string GetRemoteName(string shareName)
		{
			StringBuilder sb = new StringBuilder(255);
			int lenBuff = 255;
			
			int ret = WNetGetConnection(shareName, sb, ref lenBuff);
			
			if (ret != 0)
			{
				throw new System.ComponentModel.Win32Exception(ret, ((NetworkErrors)ret).ToString());
			}

			return sb.ToString();
		}

		/// <summary>
		/// Enumerates and returns all connected network resources.
		/// </summary>
		/// <returns>Array of NetworkResource class</returns>
		public static NetworkResource[] GetConnectedResources()
		{
			NETRESOURCE netRes = new NETRESOURCE();	
			IntPtr hEnum = IntPtr.Zero;

			int ret = WNetOpenEnum(RESOURCE_CONNECTED, RESOURCETYPE_ANY, 0, IntPtr.Zero, ref hEnum);
			
			if (ret != 0)
			{
				throw new System.ComponentModel.Win32Exception(ret, ((NetworkErrors)ret).ToString());
			}
			
			//Allocate memory for NETRESOURCE array
			int bufferSize = 16384;
			IntPtr buffer = MarshalEx.AllocHLocal(bufferSize);

			if (buffer == IntPtr.Zero)
			{
				throw new OutOfMemoryException("There's not enough native memory.");
			}
			
			uint c = 0xFFFFFFFF;

			int count = (int)c;
			int size = Marshal.SizeOf(typeof(NETRESOURCE));
			ArrayList arrList = new ArrayList();

			ret = WNetEnumResource(hEnum, ref count, buffer, ref bufferSize);
			if (ret == 0)
			{
				IntPtr currPtr = buffer;
				for(int i=0;i<count;i++)
				{
					netRes = (NETRESOURCE)Marshal.PtrToStructure(currPtr, typeof(NETRESOURCE));
					NetworkResource res = new NetworkResource(Marshal.PtrToStringUni(netRes.lpLocalName), Marshal.PtrToStringUni(netRes.lpRemoteName));
					//res.RemoteName = Marshal.PtrToStringUni(netRes.lpRemoteName);
					//res.ShareName = Marshal.PtrToStringUni(netRes.lpLocalName);
					arrList.Add(res);
					currPtr = new IntPtr((int)currPtr + size);

				}
			}
			else
			{
				//clean up
				MarshalEx.FreeHLocal(buffer);
			}

			//clean up
			MarshalEx.FreeHLocal(buffer);

			return (NetworkResource[])arrList.ToArray(typeof(NetworkResource));


		}

		/// <summary>
		/// Enumerates network resources.
		/// </summary>
		/// <param name="remoteName">The name of the server</param>
		/// <returns>Array of NetworkResource class</returns>
		public static NetworkResource[] GetNetworkResources(string remoteName)
		{
			NETRESOURCE netRes = new NETRESOURCE();			
			netRes.dwScope = RESOURCE_GLOBALNET;
			netRes.dwType = RESOURCETYPE_DISK;
			netRes.dwUsage = RESOURCEUSAGE_CONTAINER;
			netRes.lpRemoteName = MarshalEx.StringToHGlobalUni(remoteName);
			netRes.lpLocalName = MarshalEx.StringToHGlobalUni("");
			netRes.lpComment = IntPtr.Zero;
			netRes.lpProvider = IntPtr.Zero;
			
			IntPtr hEnum = IntPtr.Zero;

			int ret = WNetOpenEnum(RESOURCE_GLOBALNET, RESOURCETYPE_ANY, 0, netRes, ref hEnum);
			
			if (ret != 0)
			{
				throw new System.ComponentModel.Win32Exception(ret, ((NetworkErrors)ret).ToString());
			}
			
			//Allocate memory for NETRESOURCE array
			int bufferSize = 16384;
			IntPtr buffer = MarshalEx.AllocHLocal(bufferSize);

			if (buffer == IntPtr.Zero)
			{
				throw new OutOfMemoryException("There's not enough native memory.");
			}
			
			uint c = 0xFFFFFFFF;

			int count = (int)c;
			int size = Marshal.SizeOf(typeof(NETRESOURCE));
			ArrayList arrList = new ArrayList();

			ret = WNetEnumResource(hEnum, ref count, buffer, ref bufferSize);
			if (ret == 0)
			{
				IntPtr currPtr = buffer;
				for(int i=0;i<count;i++)
				{
					netRes = (NETRESOURCE)Marshal.PtrToStructure(currPtr, typeof(NETRESOURCE));
					NetworkResource res = new NetworkResource("", Marshal.PtrToStringUni(netRes.lpRemoteName));
					//res.RemoteName = Marshal.PtrToStringUni(netRes.lpRemoteName);
					
					arrList.Add(res);
					currPtr = new IntPtr((int)currPtr + size);

				}
			}
			else
			{
				//clean up
				MarshalEx.FreeHLocal(buffer);
				throw new System.ComponentModel.Win32Exception(ret, ((NetworkErrors)ret).ToString());
			}

			//clean up
			MarshalEx.FreeHLocal(buffer);

			return (NetworkResource[])arrList.ToArray(typeof(NetworkResource));

		}


		

		public enum NetworkErrors
		{
			NoError = 0,
			AccessDenied = 5,
			InvalidHandle = 6,
			NotEnoughMemory = 8,
			NotSupported = 50,
			UnexpectedNetError	 = 59,
			InvalidPassword = 86,
			InvalidParameter = 87,
			InvalidLevel = 124,
			Busy = 170,
			MoreData = 234,
			InvalidAddress = 487,
			ConnectionUnavailable = 1201,
			DeviceAlreadyRemembered = 1202,
			ExtentedError = 1208,
			Cancelled = 1223,
			Retry = 1237,
			BadUsername = 2202,
			NoNetwork = 1222	
																	
		}

		#region P/Invokes

		const int RESOURCE_CONNECTED  =    0x00000001;
		const int RESOURCE_GLOBALNET  =    0x00000002;
		const int RESOURCE_REMEMBERED =    0x00000003;

		const int RESOURCETYPE_ANY   =     0x00000000;
		const int RESOURCETYPE_DISK  =     0x00000001;
		const int RESOURCETYPE_PRINT =     0x00000002;

		const int RESOURCEDISPLAYTYPE_GENERIC   =     0x00000000;
		const int RESOURCEDISPLAYTYPE_DOMAIN    =     0x00000001;
		const int RESOURCEDISPLAYTYPE_SERVER    =     0x00000002;
		const int RESOURCEDISPLAYTYPE_SHARE     =     0x00000003;
		const int RESOURCEDISPLAYTYPE_FILE      =     0x00000004;
		const int RESOURCEDISPLAYTYPE_GROUP     =     0x00000005;

		const int  RESOURCEUSAGE_CONNECTABLE =  0x00000001;
		const int RESOURCEUSAGE_CONTAINER   =  0x00000002;

		[DllImport("coredll")]
		private static extern int WNetAddConnection3(
			IntPtr hwndOwner, 
			NETRESOURCE lpNetResource, 
			string lpPassword, 
			string lpUserName, 
			int dwFlags);

		[DllImport("coredll")]
		private static extern int WNetCancelConnection2(
			string lpName, 
			int dwFlags, 
			int fForce);

		[DllImport("coredll")]
		private static extern int WNetGetConnection( 
			string lpLocalName, 
			StringBuilder lpRemoteName, 
			ref int lpnLength);

		[DllImport("coredll")]
		private static extern int WNetOpenEnum(
			int dwScope, 
			int dwType, 
			int dwUsage, 
			NETRESOURCE lpNetResource, 
			ref IntPtr lphEnum);

		[DllImport("coredll")]
		private static extern int WNetOpenEnum(
			int dwScope, 
			int dwType, 
			int dwUsage, 
			IntPtr lpNetResource, 
			ref IntPtr lphEnum);

		[DllImport("coredll")]
		private static extern int WNetEnumResource( 
			IntPtr hEnum, 
			ref int lpcCount, 
			IntPtr lpBuffer, 
			ref int lpBufferSize 
			);

		private class NETRESOURCE 
		{
			public int dwScope; 
			public int dwType; 
			public int dwDisplayType; 
			public int dwUsage; 
			public IntPtr lpLocalName; 
			public IntPtr lpRemoteName; 
			public IntPtr lpComment; 
			public IntPtr lpProvider; 
		}
		#endregion
	}
	
	/// <summary>
	/// Implements NetworkResouce class
	/// </summary>
	public class NetworkResource
	{
		private string shareName;
		private string remoteName;

		internal NetworkResource(string shareName, string remoteName)
		{
			this.shareName = shareName;
			this.remoteName = remoteName;
		}

		/// <summary>
		/// Gets ShareName
		/// </summary>
		public string ShareName
		{
			get
			{
				return shareName;
			}
		}
		
		/// <summary>
		/// Gets Remote name.
		/// </summary>
		public string RemoteName
		{
			get
			{
				return remoteName;
			}
		}
	}
}
