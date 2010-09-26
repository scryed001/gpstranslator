/*=======================================================================================

    OpenNETCF.Net
    Copyright 2003, OpenNETCF.org

    This library is free software; you can redistribute it and/or modify it under 
    the terms of the OpenNETCF.org Shared Source License.

    This library is distributed in the hope that it will be useful, but 
    WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
    FITNESS FOR A PARTICULAR PURPOSE. See the OpenNETCF.org Shared Source License 
    for more details.

    You should have received a copy of the OpenNETCF.org Shared Source License 
    along with this library; if not, email licensing@opennetcf.org to request a copy.

    If you wish to contact the OpenNETCF Advisory Board to discuss licensing, please 
    email licensing@opennetcf.org.

    For general enquiries, email enquiries@opennetcf.org or visit our website at:
    http://www.opennetcf.org

=======================================================================================*/
using System;
using System.Data;
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using OpenNETCF.Win32;
using OpenNETCF.IO;
using OpenNETCF.Runtime.InteropServices;

namespace OpenNETCF.Net
{
	#region -------------- Internal-only Classes --------------

	internal class NDISUIO_QUERY_OID	
	{
		protected byte[]	data;
		protected int		ourSize;
		public int Size
		{
			get { return ourSize; }
		}

		protected const int NDISUIO_QUERY_OID_SIZE = 12;
		public NDISUIO_QUERY_OID( int extrasize )
		{
			// Most of the cases we'll use will have a size
			// of just sizeof( DWORD ), but you never know.
			ourSize = NDISUIO_QUERY_OID_SIZE + extrasize;
			data = new byte[ ourSize ];
		}

		protected const int OidOffset = 0;
		public uint Oid
		{
			get { return BitConverter.ToUInt32( data, OidOffset ); }
			set 
			{ 
				byte[]	bytes = BitConverter.GetBytes( value );
				Buffer.BlockCopy( bytes, 0, data, OidOffset, 4 );
			}
		}

		protected const int ptcDeviceNameOffset = OidOffset + 4;
		public unsafe byte *ptcDeviceName
		{
			get 
			{ 
				return (byte*)BitConverter.ToUInt32( data, ptcDeviceNameOffset ); 
			}
			set 
			{ 
				byte[]	bytes = BitConverter.GetBytes( (UInt32)value );
				Buffer.BlockCopy( bytes, 0, data, ptcDeviceNameOffset, 4 );
			}
		}

		protected const int DataOffset = ptcDeviceNameOffset + 4;
		public byte[] Data
		{
			get
			{
				byte[]	b = new byte[ ourSize - DataOffset ];
				Array.Copy( data, DataOffset, b, 0, ourSize - DataOffset );
				return b;
			}
		}

		public byte[] getBytes()
		{
			return data;
		}

		public static implicit operator byte[](NDISUIO_QUERY_OID qoid)
		{
			return qoid.data;
		}
	}

	internal class IP_ADAPTER_INFO
	{
		protected byte[]	data;
		protected uint		firstnextIndex = 0;
		protected uint		firstnextOffset = 0;
		protected uint		ourSize = 0;
		protected uint		ourBase = 0;

		// Main constructor.  This figures out how much space
		// is needed to hold the list of adapters, allocates
		// a byte array for that space, and gets the list
		// from GetAdaptersInfo().
		unsafe public IP_ADAPTER_INFO()
		{
			// Find out how much space we need to store the
			// adapter list.
			int	size = 0;
			int	err = AdapterPInvokes.GetAdaptersInfo( null, ref size ); 
			if ( err != 0 )
			{
				// This is what we'd expect: there is not enough room in the
				// buffer, so the size is set and an error is returned.
				if ( err == 111 )
				{
					// ToDo: Handle buffer-too-small.
				}
			}

			// Check for size = 0 (no adapters, presumably).
			ourSize = (uint)size;
			if ( ourSize == 0 )
			{
				data = null;
			}
			else
			{
				data = new byte[ size ];

				// We need to lock this in memory until we can
				// get its address.  Since GetAdaptersInfo will
				// be storing Next pointers from adapter information
				// to adapter information, we need to know what
				// the base for those pointers is.  We can then
				// calculate the offset into the byte array of
				// IP_ADAPTER_INFO from that.
				// Fix the data array in memory.  We need to do
				// this to store the base address of the array.
				// The GetAdaptersInfo call will put various Next
				// pointers in the structure and we need to know
				// what the base address against which those are
				// measured is.  With that, we can figure out what
				// offset in the data array they reference.
				fixed( byte *b = &data[ 0 ] )
				{
					// Save the base address.
					ourBase = (uint)b;

					// Actually call GetAdaptersInfo.
					size = (int)ourSize;
					err = AdapterPInvokes.GetAdaptersInfo( data, ref size );
				}

				if ( err != 0 )
					data = null;
			}
		}

		protected const int IP_ADAPTER_INFO_SIZE = 640;
		protected IP_ADAPTER_INFO( byte[] datain, uint offset )
		{
			// Create an internal-only copy of this structure,
			// making it easy to get to the fields of one
			// of the items in the linked list, based on its
			// offset within the byte[] of the main
			// instance of an IP_ADAPTER_INFO.
			ourSize = IP_ADAPTER_INFO_SIZE;
			data = new byte[ IP_ADAPTER_INFO_SIZE ];
			Array.Copy( datain, (int)offset, data, 0, IP_ADAPTER_INFO_SIZE );
		}

		public Adapter FirstAdapter()
		{
			if ( data == null )
				return null;

			// Reset the indexing.
			firstnextIndex = 0;
			if ( this.Next == 0 )	// !!!!
				firstnextOffset = 0;	// !!!!
			else						// !!!!
				firstnextOffset = this.Next - ourBase;

			// Since we are creating this adapter based on
			// the first entry in our table, we can just pass
			// 'this' to do it.
			return new Adapter( this );
		}
		public Adapter NextAdapter()
		{
			// Starting at the current offset in our 'data'
			// member, get the Next field, subtrace its pointer
			// from the initial pointer value to find the
			// new offset, and create an adapter starting
			// at that point in the 'data' member.

			if ( data == null )
				return null;

			// Handle no more case.
			if ( firstnextOffset == 0 )
				return null;

			IP_ADAPTER_INFO	newinfo = new IP_ADAPTER_INFO( data, 
				firstnextOffset );

			firstnextIndex++;

			// Now, use the Next field of the new info 
			// structure to update where we find the next
			// one after that in our list.
			if ( newinfo.Next == 0 )
				firstnextOffset = 0;
			else
				firstnextOffset = newinfo.Next - ourBase;

			return new Adapter( newinfo );
		}

		// Accessors for fields of the item.
		protected const int NextOffset = 0;
		public uint Next
		{
			get { return BitConverter.ToUInt32( data, NextOffset ); }
		}

		protected const int ComboIndexOffset = NextOffset + 4;
		public int ComboIndex
		{
			get { return BitConverter.ToInt32( data, ComboIndexOffset ); }
		}

		protected const int MAX_ADAPTER_DESCRIPTION_LENGTH = 128;
		protected const int MAX_ADAPTER_NAME_LENGTH = 256;
		protected const int MAX_ADAPTER_ADDRESS_LENGTH = 8;

		protected const int AdapterNameOffset = ComboIndexOffset + 4;
		public String AdapterName
		{
			get 
			{ 
				String	s = Encoding.ASCII.GetString(data, AdapterNameOffset, MAX_ADAPTER_NAME_LENGTH);
				int		l = s.IndexOf( '\0' );
				if ( l != -1 )
					return s.Substring( 0, l ); 
				return s;
			}
		}

		protected const int DescriptionOffset = AdapterNameOffset + MAX_ADAPTER_NAME_LENGTH + 4;
		public String Description
		{
			get 
			{ 
				String	s = Encoding.ASCII.GetString(data, DescriptionOffset, MAX_ADAPTER_DESCRIPTION_LENGTH);
				int		l = s.IndexOf( '\0' );
				if ( l != -1 )
					return s.Substring( 0, l ); 
				return s;
			}
		}

		protected const int PhysAddressLengthOffset = DescriptionOffset + MAX_ADAPTER_DESCRIPTION_LENGTH + 4;
		public int PhysAddressLength
		{
			get { return BitConverter.ToInt32( data, PhysAddressLengthOffset ); }
		}

		protected const int PhysAddressOffset = PhysAddressLengthOffset + 4;
		public byte[] PhysAddress
		{
			get 
			{ 
				byte[]	b = new byte[ MAX_ADAPTER_ADDRESS_LENGTH ];
				Array.Copy( data, PhysAddressOffset, b, 0, MAX_ADAPTER_ADDRESS_LENGTH ); 
				return b;
			}
		}

		protected const int IndexOffset = PhysAddressOffset + MAX_ADAPTER_ADDRESS_LENGTH;
		public int Index
		{
			get { return BitConverter.ToInt32( data, IndexOffset ); }
		}

		protected const int TypeOffset = IndexOffset + 4;
		public int Type
		{
			get { return BitConverter.ToInt32( data, TypeOffset ); }
		}

		protected const int DHCPEnabledOffset = TypeOffset + 4;
		public bool DHCPEnabled
		{
			get { return BitConverter.ToBoolean( data, DHCPEnabledOffset ); }
		}

		protected const int CurrentIpAddressOffset = DHCPEnabledOffset + 4;
		public String CurrentIpAddress
		{
			// The CurrentIpAddress field is a pointer to an 
			// IP_ADDRESS_STRING structure, not a string itself,
			// so we have to do some magic to make this work.
			get 
			{ 
				IntPtr p = new IntPtr( BitConverter.ToInt32( data, CurrentIpAddressOffset) );
				if ( p == IntPtr.Zero )
					return null;

				// Here, I'm going to extract the 16 bytes of
				// the IP address string from the data pointed
				// to by the CurrentIpAddress pointer.  The
				// IP address part of what's pointed to starts
				// at offset 4 from the pointer value (skip the
				// Next field).  From there, we just copy 16
				// bytes, the length of the IP address string,
				// to a local byte array, which can easily be
				// converted to a managed string below.
				byte[]	b = new byte[ 16 ];
				IntPtr	p4 = new IntPtr( p.ToInt32()+4 );
				MarshalEx.Copy( p4, b, 0, 16 );

				// The string itself is stored after the Next
				// field in the IP_ADDRESS_STRING structure
				// (offset 4).
				String	s = Encoding.ASCII.GetString(b, 0, 16);
				int		l = s.IndexOf( '\0' );
				if ( l != -1 )
					return s.Substring( 0, l );
				return s;
			}
		}

		// The current subnet mask is part of the same
		// IP_ADDRESS_STRING that contains the CurrentIpAddress.
		// We simply extract a different piece of that 
		// structure to get it.s
		public String CurrentSubnetMask
		{
			// The CurrentIpAddress field is a pointer to an 
			// IP_ADDRESS_STRING structure, not a string itself,
			// so we have to do some magic to make this work.
			get 
			{ 
				IntPtr p = new IntPtr( BitConverter.ToInt32( data, CurrentIpAddressOffset) );
				if ( p == IntPtr.Zero )
					return null;

				// Here, I'm going to extract the 16 bytes of
				// the subnet address string from the data pointed
				// to by the CurrentIpAddress pointer.  The
				// mask address part of what's pointed to starts
				// at offset 4+16 from the pointer value (skip 
				// the Next field and the IP address field, 
				// which has length 16).  From there, we just 
				// copy 16 bytes, the length of the mask 
				// string, to a local byte array, which can 
				// easily be converted to a managed string 
				// below.
				byte[]	b = new byte[ 16 ];
				IntPtr	p4 = new IntPtr( p.ToInt32()+4+16 );
				MarshalEx.Copy( p4, b, 0, 16 );

				// The string itself is stored after the Next
				// and IpAddresss fields in the 
				// IP_ADDRESS_STRING structure (offset 4 + 16).
				String	s = Encoding.ASCII.GetString(b, 0, 16);
				int		l = s.IndexOf( '\0' );
				if ( l != -1 )
					return s.Substring( 0, l ); 
				return s;
			}
		}

		protected const int IpAddressListOffset = CurrentIpAddressOffset + 4;
#if notdefined
		public string IpAddressList
		{
			get
			{
				return null;	// ????
			}
		}
#endif

		// The offset is the start of the address list plus the
		// size of the IP_ADDRESS_STRING structure which includes
		// the Next, IpAddress, IpMask, and Context fields.
		protected const int GatewayListOffset = IpAddressListOffset + 4 + 16 + 16 + 4;
		public String Gateway
		{
			// The GatewayList field is a structure of type
			// IP_ADDRESS_STRING.  We have to extract the bits
			// we want.
			get 
			{ 
				// The string itself is stored after the Next
				// field in the IP_ADDRESS_STRING structure
				// (offset 4).
				String	s = Encoding.ASCII.GetString(data, GatewayListOffset + 4, 16);
				int		l = s.IndexOf( '\0' );
				if ( l != -1 )
					return s.Substring( 0, l ); 
				return s;
			}
		}

		protected const int DHCPServerOffset = GatewayListOffset + 4 + 16 + 16 + 4;
		public String DHCPServer
		{
			// The DhcpServer field is a structure of type
			// IP_ADDRESS_STRING.  We have to extract the bits
			// we want.
			get 
			{ 
				// The string itself is stored after the Next
				// field in the IP_ADDRESS_STRING structure
				// (offset 4).
				String	s = Encoding.ASCII.GetString(data, DHCPServerOffset + 4, 16);
				int		l = s.IndexOf( '\0' );
				if ( l != -1 )
					return s.Substring( 0, l ); 
				return s;
			}
		}

		protected const int HaveWINSOffset = DHCPServerOffset + 4 + 16 + 16 + 4;
		public bool HaveWINS
		{
			get { return BitConverter.ToBoolean( data, HaveWINSOffset ); }
		}

		protected const int PrimaryWINSServerOffset = HaveWINSOffset + 4;
		public String PrimaryWINSServer
		{
			get 
			{ 
				String	s = Encoding.ASCII.GetString(data, PrimaryWINSServerOffset+4, 16);
				int		l = s.IndexOf( '\0' );
				if ( l != -1 )
					return s.Substring( 0, l ); 
				return s;
			}
		}

		protected const int SecondaryWINSServerOffset = PrimaryWINSServerOffset + 4 + 16 + 16 + 4;
		public String SecondaryWINSServer
		{
			get 
			{ 
				String s = Encoding.ASCII.GetString(data, SecondaryWINSServerOffset+4, 16);
				int		l = s.IndexOf( '\0' );
				if ( l != -1 )
					return s.Substring( 0, l ); 
				return s;
			}
		}

		protected const int LeaseObtainedOffset = SecondaryWINSServerOffset + 4 + 16 + 16 + 4;
		public DateTime LeaseObtained
		{
			get { return MarshalEx.Time_tToDateTime(BitConverter.ToUInt32( data, LeaseObtainedOffset)); }
		}

		protected const int LeaseExpiresOffset = LeaseObtainedOffset + 4;
		public DateTime LeaseExpires
		{
			get { return MarshalEx.Time_tToDateTime(BitConverter.ToUInt32( data, LeaseExpiresOffset)); }
		}

		/*
		IP_ADDR_STRING IpAddressList;
		*/

		public static implicit operator byte[](IP_ADAPTER_INFO ipinfo)
		{
			return ipinfo.data;
		}
	}

	/// <summary>
	/// Interface Entry for WZC
	/// </summary>
	internal struct INTF_ENTRY: IDisposable, ICloneable
	{
		private IntPtr wszGuid;
		private IntPtr wszDescr;
		public uint ulMediaState;
		public uint ulMediaType;
		public uint ulPhysicalMediaType;
		public int nInfraMode;
		public int nAuthMode;
		public int nWepStatus;
		/// <summary>
		/// control flags (see INTFCTL_* defines)
		/// </summary>
		public uint dwCtlFlags;     
		/// <summary>
		/// capabilities flags (see INTFCAP_* defines)
		/// </summary>
		public uint dwCapabilities; 
		/// <summary>
		/// encapsulates the SSID raw binary
		/// </summary>
		public RAW_DATA rdSSID;         
		/// <summary>
		/// encapsulates the BSSID raw binary
		/// </summary>
		public RAW_DATA rdBSSID;        
		/// <summary>
		/// encapsulates one WZC_802_11_CONFIG_LIST structure
		/// </summary>
		public RAW_DATA rdBSSIDList;   
		/// <summary>
		/// encapsulates one WZC_802_11_CONFIG_LIST structure
		/// </summary>
		public RAW_DATA rdStSSIDList;
		/// <summary>
		/// data for various control actions on the interface
		/// </summary>
		public RAW_DATA rdCtrlData;     
		/// <summary>
		/// To track caller that freeing the same structure more than one time..
		/// </summary>
		public int      bInitialized;   
										

		/// <summary>
		/// Creates a new entry with given name in memory 
		/// </summary>
		/// <param name="guid">Name</param>
		/// <returns>Entry</returns>
		public static INTF_ENTRY GetEntry(string guid)
		{
			INTF_ENTRY entry = new INTF_ENTRY();
			entry.Guid = guid;
			INTFFlags dwOutFlags;
			uint uret = WZCPInvokes.WZCQueryInterface(null, INTFFlags.INTF_ALL, ref entry, out dwOutFlags);
			if ( uret > 0 )
				throw new AdapterException(uret, "WZCQueryInterface");
			return entry;
		}

		/// <summary>
		/// SSID
		/// </summary>
		public string SSID 
		{ 
			get { return Encoding.ASCII.GetString(rdSSID.lpData, 0, rdSSID.lpData.Length); }
			set { rdSSID.lpData = Encoding.ASCII.GetBytes(value); }
		}
		public string BSSID 
		{ 
			get { return BitConverter.ToString( rdBSSID.lpData, 0); }
		}

		/// <summary>
		/// Entry name
		/// </summary>
		public string Guid
		{
			get 
			{
				return MarshalEx.PtrToStringUni(wszGuid);
			}
			set
			{
				if ( wszGuid != IntPtr.Zero )
				{
					MarshalEx.FreeHGlobal(wszGuid);
				}
				wszGuid = MarshalEx.AllocHGlobal((value.Length + 1) * 2);
				byte[] chars = Encoding.Unicode.GetBytes(value);
				MarshalEx.Copy(chars, 0, wszGuid, chars.Length);
			}
		}

		/// <summary>
		/// Entry description
		/// </summary>
		public string Description
		{
			get 
			{
				return MarshalEx.PtrToStringUni(wszDescr);
			}
			set
			{
				if ( wszDescr != IntPtr.Zero )
				{
					MarshalEx.FreeHGlobal(wszDescr);
				}
				wszDescr = MarshalEx.AllocHGlobal((value.Length + 1) * 2);
				byte[] chars = Encoding.Unicode.GetBytes(value);
				MarshalEx.Copy(chars, 0, wszDescr, chars.Length);
			}
		}
		/// <summary>
		/// Overriden
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Guid;
		}


		#region IDisposable Members

		public void Dispose()
		{
			if ( wszDescr != IntPtr.Zero )
			{
				MarshalEx.FreeHGlobal(this.wszDescr);
			}
			if ( wszGuid != IntPtr.Zero )
			{
				MarshalEx.FreeHGlobal(this.wszGuid);
			}
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			INTF_ENTRY entry = (INTF_ENTRY)MemberwiseClone();
			entry.rdBSSID.Clear();
			entry.rdBSSIDList.Clear();
			entry.rdCtrlData.Clear();
			entry.rdSSID.Clear();
			entry.rdStSSIDList.Clear();
			entry.rdBSSID.lpData = this.rdBSSID.lpData;
			entry.rdBSSIDList.lpData = this.rdBSSIDList.lpData;
			entry.rdCtrlData.lpData = this.rdCtrlData.lpData;
			entry.rdSSID.lpData = this.rdSSID.lpData;
			entry.rdStSSIDList.lpData = this.rdStSSIDList.lpData;
			return entry;
		}

		#endregion
	}

	/// <summary>
	/// Encapsulates generic data blob
	/// </summary>
	internal struct RAW_DATA:IDisposable
	{
		//byte[] m_data;
		private uint m_cbData;
		private IntPtr m_lpData;

		public RAW_DATA(byte[] data)
		{
			//m_data = new byte[0];
			m_lpData = IntPtr.Zero;
			m_cbData = (uint)data.Length;
			lpData = data;
		}
		public uint cbData { get { return m_cbData; } }
		public byte[] lpData 
		{
			get 
			{ 
				if ( m_lpData == IntPtr.Zero )
					return null;
				byte[] data = new byte[m_cbData];
				MarshalEx.Copy(m_lpData, data, 0, (int)m_cbData);
				return data;
			}
			set
			{
				FreeMemory();
				m_lpData = MarshalEx.AllocHGlobal(value.Length);
				MarshalEx.Copy(value, 0, m_lpData, value.Length);
			}
		}
		internal void Clear()
		{
			m_lpData = IntPtr.Zero;
			m_cbData = 0;
		}
		#region IDisposable Members

		public void Dispose()
		{
			FreeMemory();
		}

		private void FreeMemory()
		{
			if ( m_lpData != IntPtr.Zero )
			{
				MarshalEx.FreeHGlobal(m_lpData);
				m_lpData = IntPtr.Zero;
			}
		}
		#endregion
	}

	/// <summary>
	/// Wireless LAN config descriptor
	/// </summary>
	internal class WZC_WLAN_CONFIG: SelfMarshalledStruct
	{
		public WZC_WLAN_CONFIG():base(SizeOf)
		{
		}

		/// <summary>
		/// Length of this structure
		/// </summary>
		public int	Length { get { return GetInt32(0); } set { SetInt32(0, value); } }
		/// <summary>
		/// control flags (NON-NDIS) see WZC_WEPK* constants
		/// </summary>
		public WZCCTL	CtlFlags { get { return (WZCCTL)GetUInt32(4); } set { SetUInt32(4, (uint)value); }}
		/// <summary>
		/// MAC Address
		/// </summary>
		public byte[] MACAddr { get { byte[] ret = new byte[6]; Buffer.BlockCopy( data, 8, ret, 0, 6); return ret; } set { value.CopyTo(data, 8); } }
		/// <summary>
		/// SSID
		/// </summary>
		public string SSID 
		{ 
			get 
			{ 
				int cb = GetInt32(16); 
				return Encoding.ASCII.GetString(data, 20, cb); 
			} 
			set 
			{
				if ( value.Length > 32 ) value = value.Substring(32);
				SetInt32(16, value.Length);
				Buffer.BlockCopy( Encoding.ASCII.GetBytes(value), 0, data, 20, value.Length);
			}
		}
		/// <summary>
		/// WEP status
		/// </summary>
		public WEPStatus Privacy { get { return (WEPStatus)GetInt32(52); } set { SetInt32(52, (int)value); } }
		/// <summary>
		/// Receive signal strength in dBm
		/// </summary>
		public int Rssi { get { return GetInt32(56); } set { SetInt32(56, value); } }
		/// <summary>
		/// Network type
		/// </summary>
		public NetworkType NetworkTypeInUse { get { return (NetworkType)GetInt32(60); } set { SetInt32(60, (int)value); } }
		/// <summary>
		/// Infrastructure mode
		/// </summary>
		public InfrastructureMode InfrastructureMode { get { return (InfrastructureMode)GetInt32(96); } set { SetInt32(96, (int)value); } }
		/// <summary>
		/// Supported data rates
		/// </summary>
		public byte[] Rates { get { byte[] ret = new byte[8]; Buffer.BlockCopy(data, 100, ret, 0, 8); return ret; } }
		/// <summary>
		/// Data rates in Mbps
		/// </summary>
		public float[] RatesConverted 
		{
			get 
			{
				int cnt = 0;
				foreach( byte b in Rates ) cnt += b == 0? 0: 1;
				float[] arrRates = new float[cnt];
				for( int i = 0; i < cnt; i++ ) arrRates[i] = (float)(Rates[i] & 0x7f) / 2;
				return arrRates;
			}
		}
		/// <summary>
		/// Current connection speed (data rate)
		/// </summary>
		public float CurrentDataRate
		{
			get
			{
				float ret = 0;
				foreach( byte b in Rates)
					if ( (b & 0x80) != 0 )
						ret = (float)(b & 0x7f) / 2;
				return ret;
			}
		}
		/// <summary>
		/// Selected key index
		/// 0 is the per-client key, 1-N are the global keys
		/// </summary>
		public int KeyIndex { get { return GetInt32(108); } set { SetInt32(108, value); } }
		/// <summary>
		/// Key length
		/// </summary>
		public int KeyLength { get { return GetInt32(112); } set { SetInt32(112, value); } }
		/// <summary>
		/// Key data
		/// </summary>
		public byte[]  KeyMaterial
		{ 
			get 
			{ 
				byte[] ret = new byte[KeyLength];
				Buffer.BlockCopy(data, 116, ret, 0, KeyLength);
				return ret; 
			} 
			set 
			{
				KeyLength = Math.Min(WZCCTL_MAX_WEPK_MATERIAL, value.Length);
				Buffer.BlockCopy(value, 0, data, 116, KeyLength);
			}
		}

		/// <summary>
		/// Authentication mode
		/// </summary>
		public AuthenticationMode AuthenticationMode { get { return (AuthenticationMode)GetUInt32(148); } set { SetUInt32(148, (uint)value); }}

		//uint                               Length;             // Length of this structure
		//uint                               dwCtlFlags;         // control flags (NON-NDIS) see WZC_WEPK* constants
		// fields from the NDIS_WLAN_BSSID structure
		//NDIS_802_11_MAC_ADDRESS             MacAddress;         // BSSID //UCHAR[6]
		//UCHAR                               Reserved[2];
		//NDIS_802_11_SSID                    Ssid;               // SSID //36 bytes
		//uint                               Privacy;            // WEP encryption requirement
		//int                    Rssi;               // receive signal strength in dBm
		//NDIS_802_11_NETWORK_TYPE            NetworkTypeInUse;
		//NDIS_802_11_CONFIGURATION           Configuration;
		//		uint           Length;             // Length of structure
		//		uint           BeaconPeriod;       // units are Kusec
		//		uint           ATIMWindow;         // units are Kusec
		//		uint           DSConfig;           // Frequency, units are kHz
		//		uint           Length;             // Length of structure
		//		uint           HopPattern;         // As defined by 802.11, MSB set
		//		uint           HopSet;             // to one if non-802.11
		//		uint           DwellTime;          // units are Kusec
		//NDIS_802_11_NETWORK_INFRASTRUCTURE  InfrastructureMode;
		//NDIS_802_11_RATES                   SupportedRates; UCHAR[8]
		// fields from NDIS_802_11_WEP structure
		//ULONG   KeyIndex;                               // 0 is the per-client key, 1-N are the global keys
		//ULONG   KeyLength;                              // length of key in bytes
		//UCHAR   KeyMaterial[WZCCTL_MAX_WEPK_MATERIAL];  // variable length depending on above field
		// aditional field for the Authentication mode
		//NDIS_802_11_AUTHENTICATION_MODE     AuthenticationMode;//36
		//RAW_DATA                            rdUserData;         // upper level buffer, attached to this config // 2

		//WZC_EAPOL_PARAMS                    EapolParams;		// 802.1x parameters // 5

		public override string ToString()
		{
			return SSID;
		}

		public NDIS_WLAN_BSSID ToBssid()
		{
			// Create a new BSSID structure from the information
			// in our own structure.  It's in the right order and 
			// everything.  Note that the signal strength does not
			// represent the signal strength currently, but when
			// the SSID was added to the preferred list, if that's
			// what we're constructing here.

			// First, since our entry list doesn't perfectly match
			// the layout (the length field is not there), we have
			// to build a data array.  We'll copy everything, starting
			// where the length would be (it's a flags field, in our
			// case), then rewrite the Length.
			/*
			4	ULONG                               Length;             // Length of this structure
			6	NDIS_802_11_MAC_ADDRESS             MacAddress;         // BSSID
			2	UCHAR                               Reserved[2];
			36	NDIS_802_11_SSID                    Ssid;               // SSID
			4	ULONG                               Privacy;            // WEP encryption requirement
			4	NDIS_802_11_RSSI                    Rssi;               // receive signal
			// strength in dBm
			4	NDIS_802_11_NETWORK_TYPE            NetworkTypeInUse;
			32	NDIS_802_11_CONFIGURATION           Configuration;
					{
					ULONG           Length;             // Length of structure
					ULONG           BeaconPeriod;       // units are Kusec
					ULONG           ATIMWindow;         // units are Kusec
					ULONG           DSConfig;           // Frequency, units are kHz
					NDIS_802_11_CONFIGURATION_FH    FHConfig;
						{
							ULONG           Length;             // Length of structure
							ULONG           HopPattern;         // As defined by 802.11, MSB set
							ULONG           HopSet;             // to one if non-802.11
							ULONG           DwellTime;          // units are Kusec
						} 
					}
			4	NDIS_802_11_NETWORK_INFRASTRUCTURE  InfrastructureMode;
			16	NDIS_802_11_RATES                   SupportedRates;
			*/
			int		len = 4 + 6 + 2 + 36 + 4 + 4 + 4 + 32 + 4 + 8;
			byte[]	d = new byte[ len ];

			Buffer.BlockCopy( data, 4 /* MacAddressOffset-4, effectively */, 
				d, 0, d.Length );

			Buffer.BlockCopy( BitConverter.GetBytes( len ), 0, 
				d, 0, 4 );

			// Build the NDIS_WLAN_BSSID from this array we've just
			// created.
			return new NDIS_WLAN_BSSID( d, 0 );
		}

		public const int WZCCTL_MAX_WEPK_MATERIAL   = 32;
		public const int SizeOf = 180;
	} 

	[Flags]
	internal enum INTFFlags : uint
	{
		INTF_ALL           = 0xffffffff,

		INTF_ALL_FLAGS     = 0x0000ffff,
		/// <summary>
		/// mask for the configuration mode (NDIS_802_11_NETWORK_INFRASTRUCTURE value)
		/// </summary>
		INTF_CM_MASK       = 0x00000007,   
		/// <summary>
		/// zero conf enabled for this interface
		/// </summary>
		INTF_ENABLED       = 0x00008000,   
		/// <summary>
		/// attempt to connect to visible non-preferred networks also
		/// </summary>
		INTF_FALLBACK      = 0x00004000,   
		/// <summary>
		/// 802.11 OIDs are supported by the driver/firmware (can't be set)
		/// </summary>
		INTF_OIDSSUPP      = 0x00002000,   
		/// <summary>
		/// the service parameters are volatile.
		/// </summary>
		INTF_VOLATILE      = 0x00001000,   
		/// <summary>
		/// the service parameters are enforced by the policy.
		/// </summary>
		INTF_POLICY        = 0x00000800,   

		INTF_DESCR         = 0x00010000,
		INTF_NDISMEDIA     = 0x00020000,
		INTF_PREFLIST      = 0x00040000,
		INTF_CAPABILITIES  = 0x00080000,


		INTF_ALL_OIDS      = 0xfff00000,
		INTF_HANDLE        = 0x00100000,
		INTF_INFRAMODE     = 0x00200000,
		INTF_AUTHMODE      = 0x00400000,
		INTF_WEPSTATUS     = 0x00800000,
		INTF_SSID          = 0x01000000,
		INTF_BSSID         = 0x02000000,
		INTF_BSSIDLIST     = 0x04000000,
		INTF_LIST_SCAN     = 0x08000000,
		INTF_ADDWEPKEY     = 0x10000000,
		INTF_REMWEPKEY     = 0x20000000,
		/// <summary>
		/// reload the default WEP_KEY
		/// </summary>
		INTF_LDDEFWKEY     = 0x40000000,  
	}

	/// <summary>
	/// List of interface names
	/// </summary>
	internal struct INTFS_KEY_TABLE: IDisposable
	{
		private uint dwNumIntfs;
		public IntPtr pData;

		public INTFS_KEY_TABLE(uint size)
		{
			dwNumIntfs = size;
			if ( size != 0 )
			{
				pData = MarshalEx.AllocHGlobal(MarshalEx.SizeOf(typeof(IntPtr)) * (int)size);
			}
			else
				pData = IntPtr.Zero;
		}

		public string this[uint i]
		{
			get
			{
				return MarshalEx.PtrToStringUni((IntPtr)(MarshalEx.ReadInt32( pData, (int)i * 4)));
			}
		}

		public uint Count
		{
			get { return dwNumIntfs; }
		}

		#region IDisposable Members

		public void Dispose()
		{
			if ( pData != IntPtr.Zero )
			{
				MarshalEx.FreeHGlobal(pData);
			}
		}

		#endregion
	}

	#endregion

	/// <summary>
	/// Class representing a single instance of a network
	/// adapter, which might include PCMCIA cards, USB network
	/// cards, built-in Ethernet chips, etc.
	/// </summary>
	public class Adapter : StreamInterfaceDriver
	{
		private const string DEVICE_NAME = "UIO1:";

		private void Open()
		{
			base.Open(FileAccess.All, FileShare.None);
		}

		internal String	name;
		/// <summary>
		/// The NDIS/driver assigned adapter name.
		/// </summary>
		public String	Name
		{
			get { return name; }
		}
		internal String	description;
		/// <summary>
		/// The descriptive name of the adapter.
		/// </summary>
		public String	Description
		{
			get { return description; }
		}
		internal int	index;
		/// <summary>
		/// The index in NDIS' list of adapters where this
		/// adapter is found.
		/// </summary>
		public int	Index
		{
			get { return index; }
		}
		internal int	type;
		/// <summary>
		/// The adapter type.  Adapters can be standard
		/// Ethernet, RF Ethernet, loopback, dial-up, etc.
		/// </summary>
		public AdapterType	Type
		{
			get { return (AdapterType)type; }
		}
		internal byte[]	hwaddress;
		/// <summary>
		/// The hardware address associated with the adapter.
		/// For Ethernet-based adapters, including RF Ethernet
		/// adapters, this is the Ethernet address.
		/// </summary>
		public byte[]	MacAddress
		{
			get { return hwaddress; }
		}

		internal bool	dhcpenabled;
		/// <summary>
		/// Indicator of whether DHCP (for IP address 
		/// assignment from a server), is enabled for the
		/// adapter.
		/// </summary>
		public bool	DhcpEnabled
		{
			get { return dhcpenabled; }
			set
			{
				// Update the local copy of the state.
				// Well, on second thought, maybe we should have to
				// be refreshed to get this updated.
//				dhcpenabled = value;

				// Modify the registry keys associated with this
				// adapter to enable DHCP.  Once that is done, we
				// have to rebind the network adapter to actually
				// make the change from static to DHCP.  We try to
				// only do this if there has been a change.
				string	regName = "\\comm\\" + this.Name + "\\Parms\\Tcpip";

				// Open the base key for the adapter.
				RegistryKey tcpipkey = Registry.LocalMachine.OpenSubKey( regName, true );

				// Get the current value of the DHCPEnabled value.  If
				// it already matches the value we're trying to set, we
				// don't have to change it.
				UInt32		oldEnableDHCP = (UInt32)tcpipkey.GetValue( "EnableDHCP", 1 );
				bool		oldEnableDHCPB = ( oldEnableDHCP != 0 ) ? true : false;

				if ( oldEnableDHCPB != value )
				{
					// Make the change.
					tcpipkey.SetValue( "EnableDHCP", value ? (UInt32)1 : (UInt32)0 );
				}

				tcpipkey.Close();
			}
		}

		internal string	currentIp;
		/// <summary>
		/// The currently active IP address of the adapter.
		/// </summary>
		public string	CurrentIpAddress
		{
			get { return currentIp; }
			set
			{
				// Update the local copy of the state.
				// Well, on second thought, maybe we should have to
				// be refreshed to get this updated.
				//				currentIp = value;

				// Modify the registry keys associated with this
				// adapter to set the new static IP.  Once that is done, we
				// have to rebind the network adapter to actually
				// make the change.  
				string	regName = "\\comm\\" + this.Name + "\\Parms\\Tcpip";

				// Open the base key for the adapter.
				RegistryKey tcpipkey = Registry.LocalMachine.OpenSubKey( regName, true );

				// Get the current value of the IpAddress value.  If
				// it already matches the value we're trying to set, we
				// don't have to change it.
				string		oldIpAddress = (string)tcpipkey.GetValue( "IpAddress", "" );

				if ( oldIpAddress != value )
				{
					// Make the change.
					tcpipkey.SetValue( "IpAddress", value );
				}

				tcpipkey.Close();
			}
		}

		internal string	currentsubnet;
		/// <summary>
		/// The currently active subnet mask address of the 
		/// adapter.
		/// </summary>
		public string	CurrentSubnetMask
		{
			get { return currentsubnet; }
			set
			{
				// Update the local copy of the state.
				// Well, on second thought, maybe we should have to
				// be refreshed to get this updated.
				//				currentsubnet = value;

				// Modify the registry keys associated with this
				// adapter to set the new subnet mask.  Once that is done, we
				// have to rebind the network adapter to actually
				// make the change.  
				string	regName = "\\comm\\" + this.Name + "\\Parms\\Tcpip";

				// Open the base key for the adapter.
				RegistryKey tcpipkey = Registry.LocalMachine.OpenSubKey( regName, true );

				// Get the current value of the SubnetMask value.  If
				// it already matches the value we're trying to set, we
				// don't have to change it.
				string		oldMask = (string)tcpipkey.GetValue( "SubnetMask", "" );

				if ( oldMask != value )
				{
					// Make the change.
					tcpipkey.SetValue( "SubnetMask", value );
				}

				tcpipkey.Close();
			}
		}

		internal string	gateway;
		/// <summary>
		/// The active gateway address.
		/// </summary>
		public string	Gateway
		{
			get { return gateway; }
			set
			{
				// Update the local copy of the state.
				// Well, on second thought, maybe we should have to
				// be refreshed to get this updated.
				//				gateway = value;

				// Modify the registry keys associated with this
				// adapter to set the new gateway.  Once that is done, we
				// have to rebind the network adapter to actually
				// make the change.  
				string	regName = "\\comm\\" + this.Name + "\\Parms\\Tcpip";

				// Open the base key for the adapter.
				RegistryKey tcpipkey = Registry.LocalMachine.OpenSubKey( regName, true );

				// Get the current value of the DefaultGateway value.  If
				// it already matches the value we're trying to set, we
				// don't have to change it.
				string		oldGateway = (string)tcpipkey.GetValue( "DefaultGateway", "" );

				if ( oldGateway != value )
				{
					// Make the change.
					tcpipkey.SetValue( "DefaultGateway", value );
				}

				tcpipkey.Close();
			}
		}

		internal string	dhcpserver;
		/// <summary>
		/// The DHCP server from which the IP address was
		/// last assigned.
		/// </summary>
		public string	DhcpServer
		{
			get { return dhcpserver; }
		}

		/// <summary>
		/// Enables or disables WZC Fallback for the current adapter
		/// </summary>
		/// <returns>
		/// Returns true/false if WZC Fallback is enabled for the current adapter
		/// </returns>
		public bool WZCFallbackEnabled
		{
			set
			{
				if ( Type != AdapterType.Ethernet )
					return ;
				// Create the entry that will be passed to WZCSetInterface.
				INTF_ENTRY entry = new INTF_ENTRY();
				entry.Guid = Name;
				if (value == false)
					entry.dwCtlFlags &= ~(uint) INTFFlags.INTF_FALLBACK;
				else
					entry.dwCtlFlags |= (uint) INTFFlags.INTF_FALLBACK;
				uint uret = WZCPInvokes.WZCSetInterface(null, INTFFlags.INTF_FALLBACK, ref entry, null);
				if ( uret > 0 )
					throw new AdapterException(uret, "WZCSetInterface");
				entry.Dispose();
			}

			get
			{
				if ( Type != AdapterType.Ethernet )
					return false;
				INTF_ENTRY entry = new INTF_ENTRY();
				entry.Guid = Name;
				INTFFlags dwOutFlags;
				uint uret = WZCPInvokes.WZCQueryInterface(null,
					INTFFlags.INTF_FALLBACK, ref entry, out dwOutFlags);
				if (uret > 0 || (entry.dwCtlFlags & (uint) INTFFlags.INTF_FALLBACK) == 0)
					return false;
				return true;
			}
		}

		internal bool	havewins;
		/// <summary>
		/// Indicates the presence of WINS server addresses
		/// for the adapter.
		/// </summary>
		public bool	HasWins
		{
			get { return havewins; }
		}

		internal string	primarywinsserver;
		/// <summary>
		/// The IP address of the primary WINS server for the
		/// adapter.
		/// </summary>
		public string	PrimaryWinsServer
		{
			get { return primarywinsserver; }
		}
		internal string	secondarywinsserver;
		/// <summary>
		/// The IP address of the secondary WINS server for the
		/// adapter.
		/// </summary>
		public string	SecondaryWinsServer
		{
			get { return secondarywinsserver; }
		}

		internal DateTime	leaseobtained;
		/// <summary>
		/// The date/time at which the IP address lease was
		/// obtained from the DHCP server.
		/// </summary>
		public DateTime	LeaseObtained
		{
			get { return leaseobtained; }
		}
		internal DateTime	leaseexpires;
		/// <summary>
		/// The date/time at which the IP address lease from
		/// the DHCP server will expire (at which time the
		/// adapter will have to contact the server to renew
		/// the lease or stop using the IP address).
		/// </summary>
		public DateTime	LeaseExpires
		{
			get { return leaseexpires; }
		}

		/// <summary>
		/// Field, if set, is used, if the NDISUIO method
		/// fails, to get the RF signal strength.  You might 
		/// use this on an OS earlier than 4.0, when NDISUIO
		/// became available.  You'd usually create your own
		/// subclass of StrengthAddon, then assign an instance
		/// of that subclass to this property, then ask for
		/// the signal strength.
		/// </summary>
		internal StrengthAddon StrengthFetcher = null;

		/// <summary>
		/// Method called on unbound adapter (maybe when handling
		/// changing *both* the IP/subnet/gateway *and* the wireless
		/// settings).  This method notifies NDIS to bind the 
		/// adapter to all protocols indicated in the registry, in 
		/// effect causing the current registry settings to be 
		/// applied rather than those which the adapter is currently
		/// using.  Since we are binding, not *re*-binding the
		/// protocols, we are implying that the adapter is not
		/// currently bound to anything.  When making this call,
		/// we must refresh any adapter list that we might have,
		/// to retrieve the current state of all adapters.  
		/// Changes to things like the IP address, subnet mask, 
		/// etc. are not immediately returned.
		/// </summary>
		public void BindAdapter()
		{
			// Rather than telling NDISUIO to rebind, we actually open
			// NDIS itself and send the message there.  So, rather than
			// calling this.Open() to open NDISUIO, we need to open the
			// driver directly.
			IntPtr	ndisAccess = FileEx.CreateFile( 
				NDISPInvokes.NDIS_DEVICE_NAME, 
				FileAccess.All, 
				FileShare.None,
				FileCreateDisposition.OpenExisting,
				NDISUIOPInvokes.FILE_ATTRIBUTE_NORMAL | NDISUIOPInvokes.FILE_FLAG_OVERLAPPED );

			// Send the device command.
			UInt32	xcount = 0;
			byte[]	namebytes = new byte[ (this.Name.Length+1)*2 ];

			// Zero the byte array.  Since the default value for a byte
			// should be zero, this should not be necessary.

			// Get the bytes forming the Unicode string which is the
			// adapter name.
			Encoding.Unicode.GetBytes( this.Name, 0, this.Name.Length, 
				namebytes, 0);

			// Tell NDIS to bind the adapter.
			if ( !NDISUIOPInvokes.DeviceIoControl( ndisAccess, 
				NDISPInvokes.IOCTL_NDIS_BIND_ADAPTER,
				namebytes, namebytes.Length,
				null, 0, ref xcount, IntPtr.Zero ) )
			{
				// Handle error.
				throw new AdapterException(NDISUIOPInvokes.GetLastError(), 
					"DeviceIoControl( IOCTL_NDIS_BIND_ADAPTER )");
			}

			FileEx.CloseHandle( ndisAccess );
		}

		/// <summary>
		/// Method called after making some changes to the current
		/// IP address, subnet mask, etc.  This method notifies NDIS
		/// to rebind the adapter to all protocols, in effect causing
		/// the current registry settings to be applied rather than
		/// those which the current configuration represents.  Once you
		/// have rebound an adapter, to get its new configuration, you
		/// must regenerate the list of adapters.  Changes to things
		/// like the IP address, subnet mask, etc. are not immediately 
		/// returned.
		/// </summary>
		public void RebindAdapter()
		{
			// Open the NDIS driver.
			IntPtr	ndisAccess = FileEx.CreateFile( 
				NDISPInvokes.NDIS_DEVICE_NAME, 
				FileAccess.All, 
				FileShare.None,
				FileCreateDisposition.OpenExisting,
				NDISUIOPInvokes.FILE_ATTRIBUTE_NORMAL | NDISUIOPInvokes.FILE_FLAG_OVERLAPPED );

			// Send the device command.
			UInt32	xcount = 0;
			byte[]	namebytes = new byte[ (this.Name.Length+1)*2 ];

			// Zero the byte array.  Since the default value for a byte
			// should be zero, this should not be necessary.

			// Get the bytes forming the Unicode string which is the
			// adapter name.
			Encoding.Unicode.GetBytes( this.Name, 0, this.Name.Length, 
				namebytes, 0);

			// Tell NDIS to rebind the adapter.
			if ( !NDISUIOPInvokes.DeviceIoControl( ndisAccess, 
				NDISPInvokes.IOCTL_NDIS_REBIND_ADAPTER,
				namebytes, namebytes.Length,
				null, 0, ref xcount, IntPtr.Zero ) )
			{
				// Handle error.
				throw new AdapterException(NDISUIOPInvokes.GetLastError(), 
					"DeviceIoControl( IOCTL_NDIS_REBIND_ADAPTER )");
			}

			FileEx.CloseHandle( ndisAccess );
		}

		/// <summary>
		/// Method called to unbind a given adapter.  You might
		/// perform this operation before attempting to change
		/// *both* the protocol configuration of an adapter (IP,
		/// subnet, gateway), *and* the wireless configuration of
		/// the same adapter (WEP, SSID, etc.)  To do that, first
		/// unbind the adapter, then change the settings, then
		/// bind the adapter (UnbindAdapter(), make changes,
		/// BindAdapter()).  Once you have bound/unbound an 
		/// adapter, to get its new configuration, you must 
		/// regenerate the list of adapters.  Changes to things
		/// like the IP address, subnet mask, etc. are not 
		/// immediately returned.
		/// </summary>
		public void UnbindAdapter()
		{
			// Open the NDIS driver.
			IntPtr	ndisAccess = FileEx.CreateFile( 
				NDISPInvokes.NDIS_DEVICE_NAME, 
				FileAccess.All, 
				FileShare.None,
				FileCreateDisposition.OpenExisting,
				NDISUIOPInvokes.FILE_ATTRIBUTE_NORMAL | NDISUIOPInvokes.FILE_FLAG_OVERLAPPED );

			// Send the device command.
			UInt32	xcount = 0;
			byte[]	namebytes = new byte[ (this.Name.Length+1)*2 ];

			// Zero the byte array.  Since the default value for a byte
			// should be zero, this should not be necessary.

			// Get the bytes forming the Unicode string which is the
			// adapter name.
			Encoding.Unicode.GetBytes( this.Name, 0, this.Name.Length, 
				namebytes, 0);

			// Tell NDIS to unbind the adapter.
			if ( !NDISUIOPInvokes.DeviceIoControl( ndisAccess, 
				NDISPInvokes.IOCTL_NDIS_UNBIND_ADAPTER,
				namebytes, namebytes.Length,
				null, 0, ref xcount, IntPtr.Zero ) )
			{
				// Handle error.
				throw new AdapterException(NDISUIOPInvokes.GetLastError(), 
					"DeviceIoControl( IOCTL_NDIS_UNBIND_ADAPTER )");
			}

			FileEx.CloseHandle( ndisAccess );
		}

		internal void FromIP_ADAPTER_INFO( IP_ADAPTER_INFO info )
		{
			// Copy the name, description, index, etc.
			name = info.AdapterName;
			description = info.Description;
			index = info.Index;

			// The adapter type should not change, so we
			// can store that.
			type = info.Type;

			// The hardware address should not change, so
			// we can store that, too.
			hwaddress = info.PhysAddress;

			// Get the flag concerning whether DHCP is enabled
			// or not.
			dhcpenabled = info.DHCPEnabled;

			// Get the current IP address and subnet mask.
			currentIp = info.CurrentIpAddress;
			currentsubnet = info.CurrentSubnetMask;

			// Get the gateway address and the DHCP server.
			gateway = info.Gateway;
			dhcpserver = info.DHCPServer;

			// Get the WINS information.
			havewins = info.HaveWINS;
			primarywinsserver = info.PrimaryWINSServer;
			secondarywinsserver = info.SecondaryWINSServer;

			// DHCP lease information.
			leaseobtained = info.LeaseObtained;
			leaseexpires = info.LeaseExpires;
		}

		internal Adapter( IP_ADAPTER_INFO info ) : base(DEVICE_NAME)
		{
			this.FromIP_ADAPTER_INFO( info );
		}

		/// <summary>
		/// Returns a Boolean indicating if the adapter is
		/// an RF Ethernet adapter.
		/// </summary>
		/// <returns>
		/// true if adapter is RF Ethernet; false otherwise
		/// </returns>
		public bool IsWireless
		{
			// Deciding if the adapter is RF Ethernet is a
			// little more complicated than just looking at
			// a bit somewhere.
			// The original scheme is below:
			// get {return ( (Type == AdapterType.Ethernet) && (SignalStrengthInDecibels != 0) ); }
			// I figured that, if you can get a signal strength,
			// it's a wireless card.  However, there are a fair
			// number of errors that would cause that strength
			// to be zero.  So, now we try to use WZC to do this.
			get
			{
				if ( Type != AdapterType.Ethernet )
					return false;

				// Call WZC and ask it about this adapter name.
				// If we get something back, it's wireless.
				INTF_ENTRY entry = new INTF_ENTRY();
				entry.Guid = Name;
				INTFFlags dwOutFlags;
				uint uret = WZCPInvokes.WZCQueryInterface(null, INTFFlags.INTF_ALL, ref entry, out dwOutFlags);
				if ( uret > 0 )
				{
					// Returned error.  Seems not to be wireless.
					return false;
				}

				// Got something.  Seems to be wireless.  Note that,
				// even if the adapter is not associated with an
				// AP, this seems to work, which might not have been
				// the case with the old way.
				return true;
			}
		}

		/// <summary>
		/// Returns a Boolean indicating if the adapter is
		/// supported by WZC.
		/// </summary>
		/// <returns>
		/// true if adapter is supported by WZC; false otherwise
		/// </returns>
		public bool IsWirelessZeroConfigCompatible
		{
			// Deciding if the adapter is working with WZC
			// requires call call to WZCQueryInterface().
			get 
			{ 
				if ( !this.IsWireless )
					return false;
				
				// Attempt to get the status of the indicated
				// interface by calling WZCQueryInterface.  If
				// it works, we return true; if not, false.
				// Note that the first parameter, the WZC server,
				// is set to null, apparently indicating that the
				// local machine is the target.
				INTF_ENTRY entry = new INTF_ENTRY();
				entry.Guid = this.Name;
				INTFFlags dwOutFlags;
				uint uret = WZCPInvokes.WZCQueryInterface(null, 
						INTFFlags.INTF_ALL, 
						ref entry, out dwOutFlags);
				if ( uret > 0 )
					return false;
				return true;
			}
		}

		/// <summary>
		/// Converts the Adapter to a string representation.  We 
		/// use the adapter's name for this.
		/// </summary>
		/// <returns>
		/// string representing the adapter
		/// </returns>
		public override string ToString()
		{
			return this.Name;
		}

		#region -------------- Changing connection parameters --------------

		/// <summary>
		/// Modifies wireless settings associated with a given interface and AP
		/// </summary>
		/// <param name="SSID">Target SSID to connect</param>
		/// <param name="bInfrastructure">Is infrastructure</param>
		/// <param name="sKey">WEP key</param>
		/// <returns>True if successful</returns>
		public bool SetWirelessSettings(string SSID, bool bInfrastructure, string sKey)
		{
			if ( sKey.Length != 10 && sKey.Length != 26 ) 
				throw new ArgumentException("Key must contain 10 or 26 hexadecimal digits", "sKey");

			byte [] arrKey = new byte[sKey.Length >> 1];
			try
			{
				for ( int i = 0; i < sKey.Length >> 1; i++ )
					arrKey[i] = byte.Parse(sKey.Substring(i*2, 2), System.Globalization.NumberStyles.HexNumber);
			}
			catch
			{
				throw new ArgumentException("Key may contain hexadecimal digits only");
			}
			return SetWirelessSettings(SSID, bInfrastructure, arrKey);			
		}

		/// <summary>
		/// Modifies wireless settings associated with a given interface and AP
		/// </summary>
		/// <param name="SSID">Target SSID to connect</param>
		/// <param name="bInfrastructure">Is infrastructure</param>
		/// <param name="Key">binary wep key - 5 or 13 bytes</param>
		/// <returns>true if succeeded</returns>
		public bool SetWirelessSettings(string SSID, bool bInfrastructure, byte[] Key)
		{
			// First, we need to get an INTF_ENTRY for this adapter.
			INTF_ENTRY entry = new INTF_ENTRY();
			entry.Guid = this.Name;
			INTFFlags dwOutFlags;
			uint uret = WZCPInvokes.WZCQueryInterface(null, 
				INTFFlags.INTF_ALL, 
				ref entry, out dwOutFlags);
			if ( uret > 0 )
			{
				// As you can see, we presently don't support
				// total configuration of the adapter with no
				// WZC intervention at all.  Somehow, you have
				// to set things up, other than SSID value, 
				// infrastructure mode, and WEP key, so that we
				// have a starting place.
				throw new AdapterException(uret, "WZCQueryInterface");
			}
			else
			{
				// Perform the 'standard' WZC stuff to set the entry's 
				// configuration.
				int cConfig = BitConverter.ToInt32( entry.rdBSSIDList.lpData, 0 );
				int Index = 8;
				WZC_WLAN_CONFIG thisConfig = null;
				for( int i = 0; i < cConfig; i ++ )
				{
					WZC_WLAN_CONFIG cfg = new WZC_WLAN_CONFIG();
					int cbCfg = BitConverter.ToInt32( entry.rdBSSIDList.lpData, Index );
					Buffer.BlockCopy(entry.rdBSSIDList.lpData, Index, cfg.Data, 0, cbCfg);
					Index += cbCfg;
					if ( cfg.SSID == SSID )
						thisConfig = cfg;
					cfg = null;
				}

				// There are a couple of things going on here:
				//	1. It might be that the user is trying to associate
				//		with an access point that we don't know about.
				//		For now, we don't allow this.
				//	2. It is also possible that the adapter is to be 
				//		placed in ad hoc mode and it just so happens that
				//		this is the first adapter to be enabled with the
				//		SSID.  We need to allow this.
				if ( ( thisConfig == null ) && ( bInfrastructure ) )
					return false;

				// If the config is null, but we are going to continue,
				// we have to create a new one and set it up for us to
				// use.
				if ( thisConfig == null )
				{
					thisConfig = new WZC_WLAN_CONFIG();

					// Set the length.
					thisConfig.Length = thisConfig.Data.Length;

					// Set the MAC address.
					thisConfig.MACAddr = this.MacAddress;

					//				thisConfig.NetworkTypeInUse = NetworkType.????;

					// Set the SSID.
					thisConfig.SSID = SSID;
				}

				// Proceed with configuration.
				byte [] arrKey = null;
				if ( Key != null )
				{
					arrKey = Key.Clone() as byte[];
					thisConfig.KeyLength = arrKey.Length;
					thisConfig.CtlFlags |= WZCCTL.WZCCTL_WEPK_PRESENT | WZCCTL.WZCCTL_WEPK_XFORMAT;
					if ( arrKey.Length == 10 )
						thisConfig.CtlFlags |= WZCCTL.WZCCTL_WEPK_40BLEN;
					byte[] chFakeKeyMaterial = new byte[]{0x56, 0x09, 0x08, 0x98, 0x4D, 0x08, 0x11, 0x66, 0x42, 0x03, 0x01, 0x67, 0x66};
					for( int i = 0; i < arrKey.Length; i ++ )
						arrKey[i] ^= chFakeKeyMaterial[(7*i)%13];
					thisConfig.KeyMaterial = arrKey;
				}
				else
				{
					thisConfig.KeyLength = 0;
				}
				thisConfig.AuthenticationMode = AuthenticationMode.Ndis802_11AuthModeOpen;

				// If we have no key, we should probably set this to WEP Off.
				thisConfig.Privacy = ( thisConfig.KeyLength > 0 )? WEPStatus.Ndis802_11WEPEnabled : WEPStatus.Ndis802_11WEPDisabled;
				thisConfig.InfrastructureMode = bInfrastructure? InfrastructureMode.Infrastructure: InfrastructureMode.AdHoc;
				byte [] FullConfig = new byte[thisConfig.Data.Length + 8 ];
				FullConfig[0] = 1; 
				thisConfig.Data.CopyTo(FullConfig, 8);
				RAW_DATA dt = new RAW_DATA(FullConfig);
				entry.rdStSSIDList = dt;
				uret = WZCPInvokes.WZCSetInterface(null, INTFFlags.INTF_PREFLIST, ref entry, null);
				if ( uret > 0 )
					throw new AdapterException(uret, "WZCSetInterface");
				entry.Dispose();
				return true;
			}
		}

		/// <summary>
		/// Sets wireless settings associated with a given 
		/// interface and AP.  This version of the method is
		/// designed for the case where *all* of the options
		/// are going to be set, where no initial configuration
		/// exists at all.
		/// </summary>
		/// <param name="SSID">Target SSID to connect</param>
		/// <param name="bInfrastructure">Is infrastructure</param>
		/// <param name="sKey">wep key string representing hex string (each two characters are converted to a single byte)</param>
		/// <param name="authMode">Authentication mode for the connection</param>
		/// <returns>true if succeeded</returns>
		public bool SetWirelessSettingsEx(string SSID, bool bInfrastructure, 
			string sKey, AuthenticationMode authMode)
		{
			if ( sKey.Length != 10 && sKey.Length != 26 ) 
				throw new ArgumentException("Key must contain 10 or 26 hexadecimal digits", "sKey");

			byte [] arrKey = new byte[sKey.Length >> 1];
			try
			{
				for ( int i = 0; i < sKey.Length >> 1; i++ )
					arrKey[i] = byte.Parse(sKey.Substring(i*2, 2), System.Globalization.NumberStyles.HexNumber);
			}
			catch
			{
				throw new ArgumentException("Key may contain hexadecimal digits only");
			}
			return SetWirelessSettingsEx(SSID, bInfrastructure, arrKey, authMode);			
		}

		/// <summary>
		/// Sets wireless settings associated with a given 
		/// interface and AP.  This version of the method is
		/// designed for the case where *all* of the options
		/// are going to be set, where no initial configuration
		/// exists at all.
		/// </summary>
		/// <param name="SSID">Target SSID to connect</param>
		/// <param name="bInfrastructure">Is infrastructure</param>
		/// <param name="Key">binary wep key - 5 or 13 bytes</param>
		/// <param name="authMode">Authentication mode for the connection</param>
		/// <returns>true if succeeded</returns>
		public bool SetWirelessSettingsEx(string SSID, bool bInfrastructure, 
			byte[] Key, AuthenticationMode authMode)
		{
			// Unlike the other SetWirelessSettings versions,
			// we *don't* get the current configuration here;
			// our parameters will set that.
			uint			uret;
			WZC_WLAN_CONFIG thisConfig;

			thisConfig = new WZC_WLAN_CONFIG();

			// Set the length.
			thisConfig.Length = thisConfig.Data.Length;

			// Set the MAC address.
			thisConfig.MACAddr = this.MacAddress;

			// Set the SSID.
			thisConfig.SSID = SSID;

			// Proceed with configuration.
			byte [] arrKey = null;
			if ( Key != null )
			{
				arrKey = Key.Clone() as byte[];
				thisConfig.KeyLength = arrKey.Length;
				thisConfig.CtlFlags |= WZCCTL.WZCCTL_WEPK_PRESENT | WZCCTL.WZCCTL_WEPK_XFORMAT;
				if ( arrKey.Length == 10 )
					thisConfig.CtlFlags |= WZCCTL.WZCCTL_WEPK_40BLEN;
				byte[] chFakeKeyMaterial = new byte[]{0x56, 0x09, 0x08, 0x98, 0x4D, 0x08, 0x11, 0x66, 0x42, 0x03, 0x01, 0x67, 0x66};
				for( int i = 0; i < arrKey.Length; i ++ )
					arrKey[i] ^= chFakeKeyMaterial[(7*i)%13];
				thisConfig.KeyMaterial = arrKey;
			}
			else
			{
				thisConfig.KeyLength = 0;
			}
			thisConfig.AuthenticationMode = authMode;

			// ???? do the right thing, based on the mode.

			// If we have no key, we should probably set this to WEP Off.
			thisConfig.Privacy = ( thisConfig.KeyLength > 0 )? WEPStatus.Ndis802_11WEPEnabled : WEPStatus.Ndis802_11WEPDisabled;
			thisConfig.InfrastructureMode = bInfrastructure? InfrastructureMode.Infrastructure: InfrastructureMode.AdHoc;
			byte [] FullConfig = new byte[thisConfig.Data.Length + 8 ];
			FullConfig[0] = 1; 
			thisConfig.Data.CopyTo(FullConfig, 8);
			RAW_DATA dt = new RAW_DATA(FullConfig);

			// Create the entry that will be passed to WZCSetInterface.
			INTF_ENTRY entry = new INTF_ENTRY();
			entry.Guid = this.Name;
			entry.rdStSSIDList = dt;
			uret = WZCPInvokes.WZCSetInterface(null, INTFFlags.INTF_PREFLIST, ref entry, null);
			if ( uret > 0 )
				throw new AdapterException(uret, "WZCSetInterface");
			entry.Dispose();
			return true;
		}

		#endregion

		// ???? To really replace NetworkAdapter, we need to
		// implement the properties below.
#if notdefined
		internal IP_ADDR_STRING IpAddressList
		{
			get 
			{ 
				return new IP_ADDR_STRING(data, 428);
			}
		}
#endif

		/// <summary>
		/// Returns the currently-attached SSID for RF
		/// Ethernet adapters.
		/// </summary>
		/// <returns>
		/// Instance of SSID class (or null if not associated).
		/// </returns>
		public unsafe String AssociatedAccessPoint
		{
			get 
			{
				// Are we wireless?
				if(!IsWireless)
					throw new AdapterException("Wired NICs are not associated with Access Points");
				
				String	ssid = null;

				// If we are running on an OS version of 4.0 or
				// higher (Windows CE.NET), then attempt to use
				// NDISUIO to get the SSID.  If we are running on 
				// an earlier version of the OS, we call a virtual 
				// method to get it.  If you have a PPC or other 
				// 3.0-based device, you can override this method 
				// to get the SSID in some other way.
				if ( System.Environment.OSVersion.Version.Major >= 4 )
				{
					NDISUIO_QUERY_OID	queryOID;

					// Attach to NDISUIO.
					try
					{
						this.Open();
					}
					catch(Exception)
					{
						return null;
					}

					// Pin unsafely-accessed items in memory.
					byte[]	namestr = System.Text.Encoding.Unicode.GetBytes(this.Name+'\0');
					fixed (byte *name = &namestr[ 0 ])
					{
						// Get Signal strength
						queryOID = new NDISUIO_QUERY_OID( 36 );	// The data is a four-byte length plus 32-byte ASCII string
						queryOID.ptcDeviceName = name;
						queryOID.Oid = NDISUIOPInvokes.OID_802_11_SSID; // 0x0D010102

						try
						{
							this.DeviceIoControl(
								(int)NDISUIOPInvokes.IOCTL_NDISUIO_QUERY_OID_VALUE,	// 0x00120804
								queryOID, 
								queryOID);
						}
						catch(Exception)
						{
							return null;
						}
					}

					// Convert the data to a string.
					byte[]	ssdata = queryOID.Data;
					int		len	= BitConverter.ToInt32( ssdata, 0 );
					if ( len > 0 )
					{
						// Convert the string from ASCII to
						// Unicode.
						ssid = System.Text.Encoding.ASCII.GetString( ssdata, 4, len );
					}

					this.Close();
				}

				// If there is still no signal indication,
				// give the add-on method a chance.
				if ( ( ssid == null ) && ( this.StrengthFetcher != null ) )
					ssid = this.StrengthFetcher.RFSSID( this );

				return ssid;
			}
		}

		/// <summary>
		/// Returns the strength of the RF Ethernet signal
		/// being received by the adapter, in dB.
		/// </summary>
		/// <returns>
		/// integer strength in dB; zero, if adapter is not
		/// an RF adapter or an error occurred
		/// </returns>
		public unsafe int SignalStrengthInDecibels
		{
			get 
			{
				int	db = 0;	// 0 indicates not an RF adapter or error.

				// If we are running on an OS version of 4.0 or
				// higher (Windows CE.NET), then attempt to use
				// NDISUIO to get the RF signal strenth.  If we
				// are running on an earlier version of the OS, 
				// we call a virtual method to get it.  If you
				// have a PPC or other 3.0-based device, you can
				// override this method to get the signal
				// strength in some other way.
				if ( System.Environment.OSVersion.Version.Major >= 4 )
				{
					NDISUIO_QUERY_OID	queryOID;

					// Attach to NDISUIO.
					try
					{
						this.Open();
					}
					catch(Exception)
					{
						return 0;
					}

					// Pin unsafely-accessed items in memory.
					byte[]	namestr = System.Text.Encoding.Unicode.GetBytes(this.Name+'\0');
					fixed (byte *name = &namestr[ 0 ])
					{
						// Get Signal strength
						queryOID = new NDISUIO_QUERY_OID( 4 );
						queryOID.ptcDeviceName = name;
						queryOID.Oid = NDISUIOPInvokes.OID_802_11_RSSI; // 0x0d010206

						try
						{
							this.DeviceIoControl(
								(int)NDISUIOPInvokes.IOCTL_NDISUIO_QUERY_OID_VALUE,	// 0x00120804
								queryOID, 
								queryOID);
						}
						catch(Exception)
						{
							return 0;
						}						
					}

					byte[]	ssdata = queryOID.Data;
					db = BitConverter.ToInt32( ssdata, 0 );

					this.Close();
				}

				// If there is still no signal indication,
				// give the add-on method a chance.
				if ( ( db == 0 ) && ( this.StrengthFetcher != null ) )
					db = this.StrengthFetcher.RFSignalStrengthDB( this );

				return db;
			}
		}

		/// <summary>
		/// Returns a SignalStrength class representing the current strength
		/// of the signal.
		/// </summary>
		/// <returns>
		///	SignalStrength
		/// </returns>
		public SignalStrength SignalStrength
		{
			get 
			{
				// Check if its a 802.11 adapter first...
				if(!IsWireless)
					throw new AdapterException("Signal strength is not a property of a wired NIC adapter");

				// Get the signal strength code and just convert
				// it to a string.
				return ( new SignalStrength(this.SignalStrengthInDecibels) );
			}
		}

		/// <summary>
		/// Returns a list of the SSID values which the 
		/// adapter can currently 'hear'.
		/// </summary>
		/// <returns>
		/// SSIDList instance containing the SSIDs.
		/// </returns>
		public AccessPointCollection NearbyAccessPoints
		{
			get { return ( new AccessPointCollection( this ) ); }
		}

		/// <summary>
		/// Returns the list of preferred SSID values which the 
		/// adapter is currently assigned.  Note that none of
		/// these might be within range, however.
		/// </summary>
		/// <returns>
		/// SSIDList instance containing the preferred SSIDs.
		/// </returns>
		public AccessPointCollection PreferredAccessPoints
		{
			get { return ( new AccessPointCollection( this, false ) ); }
		}

		/// <summary>
		/// Returns the list of preferred SSID values which the 
		/// adapter is currently assigned, but also updates the
		/// signal strengths to their current values.  Otherwise,
		/// the signal strengths are not really valid.
		/// </summary>
		/// <returns>
		/// SSIDList instance containing the preferred SSIDs.
		/// </returns>
		public AccessPointCollection NearbyPreferredAccessPoints
		{
			get { return ( new AccessPointCollection( this, true ) ); }
		}

		/// <summary>
		/// Releases the Adapter's DHCP lease
		/// </summary>
		public void DhcpRelease()
		{
			IP_ADAPTER_INDEX_MAP map = new IP_ADAPTER_INDEX_MAP();
			map.Name = this.Name;
			map.Index = this.Index;

			AdapterPInvokes.IpReleaseAddress(map);
		}

		/// <summary>
		/// Renews the Adapter's DHCP lease
		/// </summary>
		public void DhcpRenew()
		{
			IP_ADAPTER_INDEX_MAP map = new IP_ADAPTER_INDEX_MAP();
			map.Name = this.Name;
			map.Index = this.Index;

			AdapterPInvokes.IpRenewAddress(map);
		}
	}

	#region ---------- P/Invokes ----------

	// P/Invoke declarations.
	internal class AdapterPInvokes
	{
		[DllImport ("iphlpapi.dll", SetLastError=true)]
		public static extern int GetAdaptersInfo( byte[] ip, ref int size );

		[DllImport ("iphlpapi.dll", SetLastError=true)]
		public static extern uint IpReleaseAddress(byte[] adapterInfo);

		[DllImport ("iphlpapi.dll", SetLastError=true)]
		public static extern uint IpRenewAddress(byte[] adapterInfo);
	}
	
	internal class IP_ADAPTER_INDEX_MAP
	{
		byte[] m_bytes = new byte[260]; // 4 + (128 * 2)

		public IP_ADAPTER_INDEX_MAP() {}
		
		public IP_ADAPTER_INDEX_MAP(byte[] data) 
		{
			m_bytes = data;
		}

		public static implicit operator byte[] (IP_ADAPTER_INDEX_MAP map)
		{
			return map.m_bytes;
		}

		public static implicit operator IP_ADAPTER_INDEX_MAP(byte[] data)
		{
			return new IP_ADAPTER_INDEX_MAP(data);
		}

		public int Index
		{
			get { return BitConverter.ToInt32(m_bytes, 0); }
			set { Buffer.BlockCopy(BitConverter.GetBytes(value), 0, m_bytes, 0, 4); }
		}

		public string Name
		{
			get { return BitConverter.ToString(m_bytes, 4); }
			set 
			{
				byte[] bytes = Encoding.Unicode.GetBytes(value);
				Buffer.BlockCopy(bytes, 0, m_bytes, 4, bytes.Length); 
			}
		}
	}

	internal class NDISPInvokes
	{
		// This name might be used when rebinding an adapter to all
		// of its protocols (when changing its IP address, for example).
		public const String NDIS_DEVICE_NAME = "NDS0:";

		// Pass this value, with the name of the adapter to be bound,
		// to bind an unbound adapter to all of its protocols, 
		// effectively grabbing its setup from the registry.
		public const uint IOCTL_NDIS_BIND_ADAPTER = 0x00170032;

		// Pass this value, with the name of the adapter to be rebound,
		// to rebind an adapter to all of its protocols, effectively
		// grabbing its setup again from the registry.
		public const uint IOCTL_NDIS_REBIND_ADAPTER = 0x0017002e;

		// Pass this value, with the name of the adapter to be unbound,
		// to unbind an unbound adapter from all of its protocols, 
		// effectively 'disconnecting' it.
		public const uint IOCTL_NDIS_UNBIND_ADAPTER = 0x00170036;

	}

	internal class NDISUIOPInvokes
	{
		public const String NDISUIO_DEVICE_NAME = "UIO1:";

		[DllImport("coredll.dll", SetLastError = true)]
		public static extern bool DeviceIoControl(
			IntPtr hDevice, UInt32 dwIoControlCode,
			byte[] lpInBuffer, Int32 nInBufferSize,
			byte[] lpOutBuffer, Int32 nOutBufferSize,
			ref UInt32 lpBytesReturned,
			IntPtr lpOverlapped);

		[DllImport("coredll.dll")]
		public static extern uint GetLastError();

		public const Int32 FILE_ATTRIBUTE_NORMAL = 0x00000080;
		public const Int32	FILE_FLAG_OVERLAPPED = 0x40000000;

		public const UInt32 ERROR_SUCCESS = 0x0;
		public const UInt32 E_FAIL = 0x80004005;

		public const uint OID_802_11_BSSID                        = 0x0D010101;
		public const uint OID_802_11_SSID                         = 0x0D010102;
		public const uint OID_802_11_NETWORK_TYPES_SUPPORTED      = 0x0D010203;
		public const uint OID_802_11_NETWORK_TYPE_IN_USE          = 0x0D010204;
		public const uint OID_802_11_TX_POWER_LEVEL               = 0x0D010205;
		public const uint OID_802_11_RSSI                         = 0x0D010206;
		public const uint OID_802_11_RSSI_TRIGGER                 = 0x0D010207;
		public const uint OID_802_11_INFRASTRUCTURE_MODE          = 0x0D010108;
		public const uint OID_802_11_FRAGMENTATION_THRESHOLD      = 0x0D010209;
		public const uint OID_802_11_RTS_THRESHOLD                = 0x0D01020A;
		public const uint OID_802_11_NUMBER_OF_ANTENNAS           = 0x0D01020B;
		public const uint OID_802_11_RX_ANTENNA_SELECTED          = 0x0D01020C;
		public const uint OID_802_11_TX_ANTENNA_SELECTED          = 0x0D01020D;
		public const uint OID_802_11_SUPPORTED_RATES              = 0x0D01020E;
		public const uint OID_802_11_DESIRED_RATES                = 0x0D010210;
		public const uint OID_802_11_CONFIGURATION                = 0x0D010211;
		public const uint OID_802_11_STATISTICS                   = 0x0D020212;
		public const uint OID_802_11_ADD_WEP                      = 0x0D010113;
		public const uint OID_802_11_REMOVE_WEP                   = 0x0D010114;
		public const uint OID_802_11_DISASSOCIATE                 = 0x0D010115;
		public const uint OID_802_11_POWER_MODE                   = 0x0D010216;
		public const uint OID_802_11_BSSID_LIST                   = 0x0D010217;
		public const uint OID_802_11_AUTHENTICATION_MODE          = 0x0D010118;
		public const uint OID_802_11_PRIVACY_FILTER               = 0x0D010119;
		public const uint OID_802_11_BSSID_LIST_SCAN              = 0x0D01011A;
		public const uint OID_802_11_WEP_STATUS                   = 0x0D01011B;
		// Renamed to support more than just WEP encryption
		public const uint OID_802_11_ENCRYPTION_STATUS            = OID_802_11_WEP_STATUS;
		public const uint OID_802_11_RELOAD_DEFAULTS              = 0x0D01011C;

		public const uint IOCTL_NDISUIO_QUERY_OID_VALUE = 0x120804;
		public const uint IOCTL_NDISUIO_SET_OID_VALUE = 0x120814;
		public const uint IOCTL_NDISUIO_REQUEST_NOTIFICATION = 0x12081c;
		public const uint IOCTL_NDISUIO_CANCEL_NOTIFICATION = 0x120820;

		// Adapter notification flags.
		public const uint NDISUIO_NOTIFICATION_RESET_START					= 0x00000001;
		public const uint NDISUIO_NOTIFICATION_RESET_END					= 0x00000002;
		public const uint NDISUIO_NOTIFICATION_MEDIA_CONNECT				= 0x00000004;
		public const uint NDISUIO_NOTIFICATION_MEDIA_DISCONNECT				= 0x00000008;			
		public const uint NDISUIO_NOTIFICATION_BIND							= 0x00000010;
		public const uint NDISUIO_NOTIFICATION_UNBIND						= 0x00000020;
		public const uint NDISUIO_NOTIFICATION_MEDIA_SPECIFIC_NOTIFICATION  = 0x00000040;
	}

	/// <summary>
	/// P/Invoke definitions for WZC API
	/// </summary>
	internal class WZCPInvokes
	{
		[DllImport("wzcsapi")]
		public static extern uint
			WZCQueryInterface(
			string              pSrvAddr,
			INTFFlags           dwInFlags,
			ref INTF_ENTRY      pIntf,
			out INTFFlags		pdwOutFlags);

		[DllImport("wzcsapi")]
		public static extern uint
			WZCEnumInterfaces(
			string           pSrvAddr,
			ref INTFS_KEY_TABLE pIntfs);

		[DllImport("wzcsapi")]
		public static extern uint
			WZCSetInterface(
			string              pSrvAddr,
			INTFFlags           dwInFlags,
			ref INTF_ENTRY      pIntf,
			object		pdwOutFlags);


		//---------------------------------------
		// WZCDeleteIntfObj: cleans an INTF_ENTRY object that is
		// allocated within any RPC call.
		// 
		// Parameters
		// pIntf
		//     [in] pointer to the INTF_ENTRY object to delete
		[DllImport("wzcsapi")]
		public static extern void
			WZCDeleteIntfObj(
			ref INTF_ENTRY Intf);

		[DllImport("wzcsapi")]
		public static extern void
			WZCDeleteIntfObj(
			IntPtr p);


		//---------------------------------------
		// WZCPassword2Key: Translates a user password (8 to 63 ascii chars)
		// into a 256 bit network key)
		[DllImport("wzcsapi")]
		public static extern void
			WZCPassword2Key(
			WZC_WLAN_CONFIG pwzcConfig,
			string cszPassword); 
	}

	#endregion

}
