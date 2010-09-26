//==========================================================================================
//
//		OpenNETCF.Phone.Sim.Record
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
	/// Describes a single record in the SIM card storage.
	/// </summary>
	public class Record
	{
		/// <summary>
		/// Raw binary data
		/// </summary>
		private byte[] m_data;

		/// <summary>
		/// Length of the SimRecord data.
		/// </summary>
		private const int Length = 20;

		/// <summary>
		/// Creates a new <see cref="Record"/>.
		/// </summary>
		public Record()
		{
			m_data = new byte[Length];

			//write size to first four bytes
			BitConverter.GetBytes(Length).CopyTo(m_data, 0);
		}

		internal byte[] ToByteArray()
		{
			return m_data;
		}

		/// <summary>
		/// Defines which fields are valid.
		/// </summary>
		private RecordFlags Flags
		{
			get
			{
				return (RecordFlags)BitConverter.ToInt32(m_data, 4);
			}
			set
			{
				BitConverter.GetBytes((int)value).CopyTo(m_data, 4);
			}
		}

		/// <summary>
		/// Gets or Sets the type of record.
		/// </summary>
		/// <seealso cref="RecordType"/>
		public RecordType Type
		{
			get
			{
				return (RecordType)BitConverter.ToInt32(m_data, 8);
			}
			set
			{
				BitConverter.GetBytes((int)value).CopyTo(m_data, 8);
			}
		}

		/// <summary>
		/// Gets or Sets the number of items contained in the record.
		/// </summary>
		public int ItemCount
		{
			get
			{
				return BitConverter.ToInt32(m_data, 12);
			}
			set
			{
				BitConverter.GetBytes(value).CopyTo(m_data, 12);
			}
		}

		/// <summary>
		/// Gets or Sets the size, in bytes, of each item.
		/// </summary>
		public int Size
		{
			get
			{
				return BitConverter.ToInt32(m_data, 16);
			}
			set
			{
				BitConverter.GetBytes(value).CopyTo(m_data, 16);
			}
		}

		private enum RecordFlags :int
		{
			/// <summary>
			/// RecordType field is valid.
			/// </summary>
			RecordType = 0x00000001,
			/// <summary>
			/// ItemCount field is valid.
			/// </summary>
			ItemCount  = 0x00000002,
			/// <summary>
			/// Size field is valid.
			/// </summary>
			Size       = 0x00000004,
			/// <summary>
			/// All fields are valid.
			/// </summary>
			All        = 0x00000007,
		}
	}
}
