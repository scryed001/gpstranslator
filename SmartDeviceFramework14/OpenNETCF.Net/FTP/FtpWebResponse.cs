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
using System.Net;
using System.Net.Sockets;

namespace OpenNETCF.Net.Ftp
{
	/// <summary>
	/// Summary description for FtpWebResponse.
	/// </summary>
	public class FtpWebResponse : WebResponse, IDisposable
	{
		private FtpDataStream dataStream;
		internal FtpWebResponse( FtpDataStream stream )
		{
			dataStream = stream;
		}

		public override void Close()
		{
			throw new NotSupportedException();
		}

		public override Stream GetResponseStream()
		{
			return dataStream;
		}

		public string BannerMessage
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override long ContentLength
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public string ExitMessage
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override WebHeaderCollection Headers
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public DateTime LastModified
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override Uri ResponseUri
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public FtpStatusCode Status // TODO: Implement FtpStatusCode
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public string StatusDescription
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public string WelcomeMessage
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
}
