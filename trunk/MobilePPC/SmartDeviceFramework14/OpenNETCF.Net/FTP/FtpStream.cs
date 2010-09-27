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
	/// Summary description for FtpStream.
	/// </summary>
	public abstract class FtpStream  : Stream, IDisposable
	{
		protected Socket socket;

		// We wait for ONE second trying to poll from socket before giving up and setting read stream in "CanRead = false" state
		protected int socketPollTime = 1000000;

		internal FtpStream( Socket inSocket )
		{
			socket = inSocket;
		}

		public override void Write( byte [] buffer, int offset, int length)
		{
			socket.Send( buffer, offset, length, SocketFlags.None );
		}

		public override int Read( byte[] buffer, int offset, int length )
		{
			if( socket.Available <= 0 )
			{
				if( !socket.Poll( socketPollTime, SelectMode.SelectRead ) )
				{
					return 0;
				}
			}
			int bytesRead = socket.Receive( buffer, offset, length, SocketFlags.None );
			return bytesRead;
		}

		public override void Close()
		{
			socket.Shutdown( SocketShutdown.Both );
			socket.Close();
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException("This stream cannot be seeked");
			}
		}

		public override long Position
		{
			get 
			{
				throw new NotSupportedException("This stream cannot be seeked");
			}

			set 
			{
				throw new NotSupportedException("This stream cannot be seeked");
			}
		}

		public override long Seek( long offset, SeekOrigin origin ) 
		{
			throw new NotSupportedException("This stream cannot be seeked");
		}

		public override void Flush() 
		{
		}

		public override void SetLength( long value ) 
		{
			throw new NotSupportedException("This stream cannot be seeked");
		}

		#region IDisposable Members

		public void Dispose()
		{
			socket.Close();
		}

		#endregion
	}
}
