/*=======================================================================================

    OpenNETCF.Net.FTPFile
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

namespace OpenNETCF.Net.Ftp
{
	/// <summary>
	/// Information about an FTP File
	/// <seealso cref="FTP.EnumFiles"/>
	/// </summary>
	public class FTPFile
	{
		private string		m_name;
		private int			m_size;
		private FTPFileType m_type;
		private DateTime	m_date;

		internal FTPFile(string name, FTPFileType type, int size, DateTime date)
		{
			m_name = name;
			m_size = size;
			m_type = type;
			m_date = date;

		}

		/// <summary>
		/// Filename
		/// </summary>
		public string Name
		{
			get
			{
				return m_name;
			}
		}

		/// <summary>
		/// File date
		/// </summary>
		public DateTime FileDate
		{
			get
			{
				return m_date;
			}
		}

		/// <summary>
		/// File size
		/// </summary>
		public int Size
		{
			get
			{
				return m_size;
			}
		}

		/// <summary>
		/// File type
		/// <seealso cref="FTPFileType"/>
		/// </summary>
		public FTPFileType Type
		{
			get
			{
				return m_type;
			}
		}
	}
}
