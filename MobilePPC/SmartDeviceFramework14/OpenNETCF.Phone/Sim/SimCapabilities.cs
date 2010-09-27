//==========================================================================================
//
//		OpenNETCF.Phone.Sim.SimCapabilities
//		Copyright (c) 2004, OpenNETCF.org
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
using System.Collections;
using System.Runtime.InteropServices;

namespace OpenNETCF.Phone.Sim
{
	/// <summary>
	/// Summary description for SimCapabilities.
	/// </summary>
	public class SimCapabilities
	{
		private byte[] m_data;

		private const int Length = 124;

		private const int SIM_NUMLOCKFACILITIES = 10;
		
		/*DWORD cbSize;                           // @field Size of the structure in bytes
		DWORD dwParams;                         // @field Indicates valid parameter values
		DWORD dwPBStorages;                     // @field Supported phonebook storages
		DWORD dwMinPBIndex;                     // @field Minimum phonebook storages
		DWORD dwMaxPBIndex;                     // @field Maximum phonebook storages
		DWORD dwMaxPBEAddressLength;            // @field Maximum address length of phonebook entries
		DWORD dwMaxPBETextLength;               // @field Maximum text length of phonebook entries
		DWORD dwLockFacilities;                 // @field Supported locking facilities
		DWORD dwReadMsgStorages;                // @field Supported read message stores
		DWORD dwWriteMsgStorages;               // @field Supported write message stores
		DWORD dwNumLockingPwdLengths;           // @field Number of entries in rgLockingPwdLengths
		SIMLOCKINGPWDLENGTH rgLockingPwdLengths[SIM_NUMLOCKFACILITIES]; // @field Password lengths for each facility*/
		
		public SimCapabilities()
		{
			m_data = new byte[Length];

			//write length to first dword
			BitConverter.GetBytes(Length).CopyTo(m_data, 0);
		}

		internal byte[] ToByteArray()
		{
			return m_data;
		}

		

		
	}
}
