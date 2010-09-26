//==========================================================================================
//
//		OpenNETCF.Phone.Sim.RecordType
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

namespace OpenNETCF.Phone.Sim
{
	/// <summary>
	/// Specifies different SIM file types.
	/// </summary>
	public enum RecordType : int
	{
		/// <summary>
		/// An unknown record type.
		/// </summary>
		Unknown          = 0x00000000,
		/// <summary>
		/// A single veriable lengthed record.
		/// </summary>
		Transparent      = 0x00000001,
		/// <summary>
		/// A cyclic set of records, each of the same length.
		/// </summary>
		Cyclic           = 0x00000002,
		/// <summary>
		/// A linear set of records, each of the same length.
		/// </summary>
		Linear           = 0x00000003, 
		/// <summary>
		/// Every SIM has a single master record, effectively the head node.
		/// </summary>
		Master           = 0x00000004,
		/// <summary>
		/// Effectively a "directory" file which is a parent of other records.
		/// </summary>
		Dedicated        = 0x00000005,
	}
}
