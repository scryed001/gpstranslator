/*=======================================================================================

    OpenNETCF.Net.FTPFiles
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
using System.Collections;

namespace OpenNETCF.Net.Ftp
{
	/// <summary>
	/// This class encapsulates a list of files retrieves from an FTP server
	/// <seealso cref="FTP.EnumFiles"/>
	/// </summary>
	public class FTPFiles : CollectionBase
	{
		internal FTPFiles()
		{
		}

		internal void Add(FTPFile FileInfo)
		{
			List.Add(FileInfo);
		}

		/// <summary>
		/// Returns a FTPFile at a specific location in the FTPFiles
		/// <seealso cref="FTPFile"/>
		/// </summary>
		public FTPFile this[int index]
		{
			get
			{
				return (FTPFile)List[index];
			}
		}

		/// <summary>
		/// Determines whether the current FTPFiles contains the specified FTPFile
		/// <seealso cref="FTPFile"/>
		/// </summary>
		/// <param name="FileInfo"></param>
		/// <returns></returns>
		public bool Contains(FTPFile FileInfo)
		{
			return InnerList.Contains(FileInfo);
		}
		
	}
}
