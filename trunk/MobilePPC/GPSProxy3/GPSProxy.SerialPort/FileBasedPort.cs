/*
$Id: FileBasedPort.cs,v 1.1 2006/05/23 09:27:00 andrew_klopper Exp $

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
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using GPSProxy.Extension;

namespace GPSProxy.SerialPort
{
	/// <summary>
	/// Summary description for FileBasedPort.
	/// </summary>
	public class FileBasedPort : IPort, IDisposable
	{
		private bool disposed = false;

		private string fileName;
		private bool readMode;
		private Timer readTimer;
		private int lastTimestamp = -1;
		private string cachedLines = "";

		private Regex timeRegex =
			// M5 sends time in the format 'hhmmss.x.y'. Don't know what the x and y stand for
			// (can't be fractions of a second) so we ignore anything after the first '.'.
			new Regex(@"^(?<hour>[0-9]{2})(?<min>[0-9]{2})(?<sec>[0-9]{2})(?:\.|$)");

		private FileStream stream;
		private StreamReader reader;

		public FileBasedPort(string fileName, string fileMode)
		{
			if (fileMode == "read")
				readMode = true;
			else if (fileMode == "write")
				readMode = false;
			else
				throw new Exception("Invalid file mode: " + fileMode);

			this.fileName = fileName;

			readTimer = new Timer(new TimerCallback(ReadTimerCallback), null, Timeout.Infinite, Timeout.Infinite);
		}

		~FileBasedPort()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (! disposed)
			{
				if (disposing)
				{
					Close();
					readTimer.Dispose();
				}
				cachedLines = null;
				disposed = true;
			}
		}

		private void ReadTimerCallback(object state)
		{
			byte[] data = null;
			lock(this)
			{
				if (reader == null)
					return;

				// This is geared exclusively to NMEA. In the future, provide a configuration option
				// to read other types of input file (assuming the protocols aren't bidirectional).
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					if (line.StartsWith("$"))
					{
						// Appears to be a line of NMEA data. Split it into its component parts after
						// removing the leading $.
						string[] parts = line.Remove(0, 1).Split(new char[] {','});
						if (((parts[0] == "GPGGA") || (parts[0] == "GPRMC")) && (parts.Length > 1)) 
						{
							Match match = timeRegex.Match(parts[1]);
							if (match.Success)
							{
								GroupCollection gc = match.Groups;
								int timestamp = int.Parse(gc["hour"].Value) * 3600 + int.Parse(gc["min"].Value) * 60 +
									int.Parse(gc["sec"].Value);
								if (timestamp != lastTimestamp)
								{
									// Output the lines so far.
									if (cachedLines != "")
										data = System.Text.Encoding.ASCII.GetBytes(cachedLines);

									// Set the timer to the appropriate interval and return.
									cachedLines = line + "\r\n";
									int interval = 1;
									if (lastTimestamp >= 0)
									{
										interval = timestamp - lastTimestamp;
										if (interval <= 0)
											interval = 1;
										else if (interval > 10)
											interval = 10;
									}
									readTimer.Change(interval * 1000, Timeout.Infinite);
									lastTimestamp = timestamp;
									break;
								}
							}
						}

						// Cache the line.
						cachedLines = cachedLines + line + "\r\n";
					}
				}
			}

			// If we have data at this point then we broke out of the loop.
			if (data != null)
			{
				if (Read != null)
					Read(this, data);
			}
			else
			{
				// Send the remaining cached lines as we have reached the end of the file.
				lock(this)
				{
					if (cachedLines != "")
					{
						data = System.Text.Encoding.ASCII.GetBytes(cachedLines);
						cachedLines = "";
					}
				}
				if ((data != null) && (Read != null))
					Read(this, data);
			}
		}

		#region IPort Members

		public event GPSProxy.Extension.PortReadEvent Read;

		public event GPSProxy.Extension.PortErrorEvent Error;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public bool Open()
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);

			lock(this)
			{
				if (stream == null)
				{
					stream = new FileStream(fileName, readMode ? FileMode.Open : FileMode.Create,
						readMode ? FileAccess.Read : FileAccess.Write);
					if (readMode) 
					{
						reader = new StreamReader(stream, System.Text.Encoding.ASCII);
						lastTimestamp = -1;
						cachedLines = "";
						readTimer.Change(0, Timeout.Infinite);
					}
				}
			}
			return true;
		}

		public bool Close()
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);

			readTimer.Change(Timeout.Infinite, Timeout.Infinite);
			lock(this)
			{
				if (reader != null)
				{
					reader.Close();
					reader = null;
				}
				if (stream != null)
				{
					stream.Close();
					stream = null;
				}
			}
			return true;
		}

		public bool IsOpen()
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);

			lock(this)
			{
				return stream != null;
			}
		}

		public int Write(byte[] data)
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);

			lock(this)
			{
				if (stream != null)
				{
					if (! readMode)
						stream.Write(data, 0, data.Length);
				}
			}
			return data.Length;
		}

		#endregion
	}
}