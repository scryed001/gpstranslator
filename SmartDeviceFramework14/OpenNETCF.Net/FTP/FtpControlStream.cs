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
	public class FtpControlStream : FtpStream, IDisposable
	{
		private int statusCode;
		private MemoryStream bufferMemoryStream;

		internal FtpControlStream( Socket inSocket )
			: base( inSocket )
		{
		}

		internal int StatusCode
		{
			get
			{
				return statusCode;
			}
		}

		private void SetStatus( byte[] bytes, int bytesRead )
		{
			if( bytesRead == 0 )
				return;
			if( bytesRead < 5 )
				return;
			string tmp = Encoding.ASCII.GetString( bytes, 0, bytesRead );
			if( tmp[tmp.Length-1] != '\n' || tmp[tmp.Length-2] != '\r' )
				return;
			else
			{
				try
				{
					tmp = tmp.Substring( 0, tmp.Length - 2 );
					int start = tmp.LastIndexOf( "\r\n" );
					if( start != -1 )
						tmp = tmp.Substring( start+2 );
					statusCode = Int32.Parse( tmp.Substring( 0, 3 ) );
				}
				catch( FormatException )
				{
					statusCode = -1;
					;//DO NOTHING
				}
				catch( OverflowException )
				{
					statusCode = -1;
					;//DO NOTHING
				}
				return;
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			base.Write( buffer, offset, count );
		}

		public override long Position
		{
			get
			{
				if( bufferMemoryStream != null )
					return bufferMemoryStream.Position;
				else
					throw new NotSupportedException();
			}
			set
			{
				if( bufferMemoryStream != null )
					bufferMemoryStream.Position = value;
				else
					throw new NotSupportedException();
			}
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
				return true;
			}
		}


		private int ReadFromSocket( byte[] buffer, int offset, int length )
		{
			int bytesRead = base.Read( buffer, offset, length );
			SetStatus( buffer, bytesRead );
			return bytesRead;
		}

		internal int ReadButDontClear( byte[] buffer, int offset, int length )
		{
			if( bufferMemoryStream == null )
				bufferMemoryStream = new MemoryStream();
			int bytesRead = ReadFromSocket( buffer, offset, length );
			bufferMemoryStream.Write( buffer, offset, bytesRead );
			return bytesRead;
		}

		public override int Read( byte[] buffer, int offset, int length )
		{
			int bytesRead = 0;
			if( bufferMemoryStream != null )
			{
				// We need to read from the "cache" first
				bytesRead = bufferMemoryStream.Read( buffer, offset, length );
				if( bufferMemoryStream.Position >= bufferMemoryStream.Length )
					bufferMemoryStream = null;
				if( bytesRead >= length )
					return bytesRead; // We got all we needed from the buffer, else we need to fill up from socket too
			}
			bytesRead += ReadFromSocket( buffer, offset, length );
			return bytesRead;
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













