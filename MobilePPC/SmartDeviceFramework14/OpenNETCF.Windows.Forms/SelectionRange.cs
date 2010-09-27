//==========================================================================================
//
//		OpenNETCF.Windows.Forms.SelectionRange
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
using OpenNETCF.Win32;

namespace OpenNETCF.Windows.Forms
{
	/// <summary>
	/// Represents a date selection range in a month calendar control.
	/// <para><b>New in v1.1</b></para>
	/// </summary>
	public class SelectionRange
	{
		byte[] m_data;

		/// <summary>
		/// Initializes a new instance of the <see cref="SelectionRange"/> class.
		/// </summary>
		public SelectionRange()
		{
			m_data = new byte[32];
			//start at minimum
			this.Start = new DateTime(1753, 1, 1);
			//end at max
			this.End = DateTime.MaxValue;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="SelectionRange"/> class with the specified beginning and ending dates.
		/// </summary>
		/// <param name="lower">The starting date in the <see cref="SelectionRange"/>.</param>
		/// <param name="upper">The ending date in the <see cref="SelectionRange"/>.</param>
		public SelectionRange(DateTime lower, DateTime upper) : this()
		{
			this.Start = lower;
			this.End = upper;
		}

		internal byte[] ToByteArray()
		{
			return m_data;
		}

		/// <summary>
		/// Gets or sets the starting date and time of the selection range.
		/// </summary>
		public DateTime Start
		{
			get
			{
				SystemTime st = new SystemTime(m_data, 0);
				return st.ToDateTime();
			}
			set
			{
				SystemTime st = new SystemTime(value);
				st.ToByteArray().CopyTo(m_data, 0);
			}
		}

		/// <summary>
		/// Gets or sets the ending date and time of the selection range.
		/// </summary>
		public DateTime End
		{
			get
			{
				SystemTime st = new SystemTime(m_data, 16);
				return st.ToDateTime();
			}
			set
			{
				SystemTime st = new SystemTime(value);
				st.ToByteArray().CopyTo(m_data, 16);
			}
		}
	}
}
