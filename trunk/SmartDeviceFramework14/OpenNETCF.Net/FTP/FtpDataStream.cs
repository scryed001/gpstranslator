/*=======================================================================================

    OpenNETCF.Net.FTP
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
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace OpenNETCF.Net.Ftp
{
	/// <summary>
	/// An FTP connection has got two streams, one for sending control commands an another one for "data" retrieval and "sending"
	/// This stream is the Control Stream of the FTP connection, meaning it will be used to transmit control commands and
	/// also for checking if transmissions of those control commands was ok.
	/// </summary>
	public class FtpDataStream : FtpStream, IDisposable
	{
		internal FtpDataStream( Socket inSocket )
			: base( inSocket )
		{
		}

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				// Need to be able to write this stream to upload files
				// to the server.
				return true;
			}
		}

/*        
		#region IDisposable Members

		public void Dispose()
		{
			base.Dispose();
		}

		#endregion
*/
	}
}













