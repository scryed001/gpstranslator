/*
$Id: OpenNETCFSerialPort.cs,v 1.1 2006/05/23 09:27:05 andrew_klopper Exp $

Copyright 2005-2006 Andrew Rowland Klopper (http://gpsproxy.sourceforge.net/)

This file is part of GPSProxy.

GPSProxy is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

GPSProxy is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with GPSProxy; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System;
using OpenNETCF.IO.Serial;
using GPSProxy.Extension;

namespace GPSProxy.SerialPort
{
	/// <summary>
	/// Summary description for OpenNETCFSerialPort.
	/// </summary>
	public class OpenNETCFSerialPort : IPort, IDisposable
	{
		private bool disposed = false;

		private const int portBufSize = 1024;
		private Port serialPort;

		public OpenNETCFSerialPort(string portName, int baudRate)
		{
			serialPort = new Port(portName, new HandshakeNone(), portBufSize, portBufSize);
			serialPort.Settings.BaudRate = (BaudRates)baudRate;
			serialPort.Settings.ByteSize = 8;
			serialPort.Settings.Parity = Parity.none;
			serialPort.Settings.StopBits = StopBits.one;
			serialPort.InputLen = 0;
			serialPort.RThreshold = 1;
			serialPort.SThreshold = 1;
			serialPort.DataReceived += new Port.CommEvent(DataReceived);
			serialPort.OnError += new Port.CommErrorEvent(CommError);
		}

		~OpenNETCFSerialPort()
		{
			Dispose(false);
		}

		private void DataReceived()
		{
			if (Read != null)
			{
				byte[] data = serialPort.Input;
				if ((data != null) && (data.Length > 0))
					Read(this, data);
			}
		}

		private void CommError(string error)
		{
			if (Error != null)
				Error(this, error);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if (! disposed)
			{
				if (disposing)
				{
					Close();
					serialPort.Dispose();
				}
				disposed = true;
			}
		}

		#region IComPort Members

		public event PortReadEvent Read;
		public event PortErrorEvent Error;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public bool IsOpen()
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);

			return serialPort.IsOpen;
		}

		public bool Open()
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);

			if (! serialPort.IsOpen)
				serialPort.Open();
			return serialPort.IsOpen;
		}

		public bool Close()
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);

			if (serialPort.IsOpen)
				serialPort.Close();
			return ! serialPort.IsOpen;
		}

		public int Write(byte[] data)
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);

			if (serialPort.IsOpen)
			{
				serialPort.Output = data;
				return data.Length;
			}
			return 0;
		}

		#endregion
	}
}
