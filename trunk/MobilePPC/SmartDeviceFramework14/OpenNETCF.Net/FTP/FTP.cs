/*=======================================================================================

    OpenNETCF.Net.FTP
    Copyright 2003-2005, OpenNETCF.org

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
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using OpenNETCF;
//using OpenNETCF.Threading;
using OpenNETCF.Net;

/*	
FTP Command set:
 
Note that commands marked with a * are not implemented in a number of FTP servers. 


Common commands
ABOR - abort a file transfer 
CWD - change working directory 
DELE - delete a remote file 
LIST - list remote files 
MDTM - return the modification time of a file 
MKD - make a remote directory 
NLST - name list of remote directory 
PASS - send password 
PASV - enter passive mode 
PORT - open a data port 
PWD - print working directory 
QUIT - terminate the connection 
RETR - retrieve a remote file 
RMD - remove a remote directory 
RNFR - rename from 
RNTO - rename to 
SITE - site-specific commands 
SIZE - return the size of a file 
STOR - store a file on the remote host 
TYPE - set transfer type 
USER - send username 
Less common commandsACCT* - send account information 
APPE - append to a remote file 
CDUP - CWD to the parent of the current directory 
HELP - return help on using the server 
MODE - set transfer mode 
NOOP - do nothing 
REIN* - reinitialize the connection 
STAT - return server status 
STOU - store a file uniquely 
STRU - set file transfer structure 
SYST - return system type 
*/

namespace OpenNETCF.Net.Ftp
{
	/// <summary>
	/// This class can be used for FTP operations
	/// </summary>
	public class FTP
	{
		private static int BUFFER_SIZE = 512;

		private bool				m_connected		= false;
		private FTPMode				m_mode			= FTPMode.Passive;
//		private int					m_port			= 21;
//		private string				m_host			= "";
		private FTPTransferType		m_type			= FTPTransferType.Binary;
		private string				m_uid			= "";
		private string				m_pwd			= "";
		private Socket				m_cmdsocket		= null;
		private bool				m_exceptions	= true;
		private byte[]				m_buffer		= new byte[BUFFER_SIZE];
		private FTPServerType		m_server		= FTPServerType.Unknown;

#if CF
		private EventWaitHandle		m_connectwait	= new EventWaitHandle(false, EventResetMode.ManualReset, "FTPConnect");		
#else
		private ManualResetEvent		m_connectwait	= new ManualResetEvent(false);
#endif
		private int					m_timeout		= 5000;

		//// begin new code
		Stream			m_ftpRequestStream;
		FtpWebRequest	m_request;
		Uri				m_uri;
	
		/// <summary>
		/// Event raised when the FTP server sends a response
		/// </summary>
		public event FTPResponseHandler ResponseEvent;
		/// <summary>
		/// Event raised when a command is sent to the FTP server
		/// </summary>
		public event FTPCommandHandler  CommandSentEvent;
		/// <summary>
		/// Event raised when a connection is made with the FTP server
		/// </summary>
		public event FTPConnectedHandler	ConnectedEvent;

		#region ctors / dtor
		/// <summary>
		/// Creates an FTP object
		/// </summary>
		/// <param name="RemoteHost">Hostname that will be connected to.  The port will default to 21.</param>
		public FTP(string RemoteHost) : this(RemoteHost, 21)
		{
		}

		/// <summary>
		/// Creates an FTP object
		/// </summary>
		/// <param name="remoteHost">Hostname that will be connected to.</param>
		/// <param name="port">Port on which to connect</param>
		public FTP(string remoteHost, int port)
		{
			FtpRequestCreator creator = new FtpRequestCreator();
			WebRequest.RegisterPrefix( "ftp:", creator );

			// Building our URI object
			if( remoteHost.IndexOf( "ftp:" ) != 0 )
				m_uri = new Uri("ftp://" + remoteHost + ":" + port.ToString());
			else
				m_uri = new Uri(remoteHost + ":" + port.ToString());

		}
		#endregion

		#region methods
		/// <summary>
		/// When set, if an error is encountered while using the class instance, an exception will be thrown
		/// </summary>
		public bool ExceptionOnError
		{
			set
			{
				m_exceptions = value;
			}
			get
			{
				return m_exceptions;
			}
		}

		/// <summary>
		/// Connect to the FTP server anonymously, using the supplied email address for a password
		/// </summary>
		/// <param name="EmailAddress">String passwed as the password</param>
		public void Connect(string EmailAddress)
		{
			Connect("anonymous", EmailAddress);
		}

		/// <summary>
		/// Connect to the FTP server using the supplied username and password
		/// </summary>
		/// <param name="Username">Username</param>
		/// <param name="Password">Password</param>
		public void Connect(string username, string password)
		{
			// Creating a new FtpRequest object
			m_request = (FtpWebRequest)WebRequest.Create( m_uri );
			m_request.Credentials = new NetworkCredential( username, password );

			// Getting the Request stream
			m_ftpRequestStream = m_request.GetRequestStream();

			StreamReader reader = new StreamReader( m_ftpRequestStream );

/*
			// get the server type
			response = SendCommand("SYST", false);
			if(response.Text.ToUpper().IndexOf("UNIX") > -1)
			{
				m_server = FTPServerType.Unix;
			}
			else if(response.Text.ToUpper().IndexOf("WINDOWS") > -1)
			{
				m_server = FTPServerType.Windows;
			}
			else
			{
				m_server = FTPServerType.Unknown;
			}

			m_connected = true;
			m_connectwait.Set();

			//			m_response = reader.ReadToEnd();

*/
		}

		/// <summary>
		/// Disconnect from the FTP server
		/// </summary>
		public void Disconnect()
		{
			if(m_cmdsocket != null)
			{
				FTPResponse response = SendCommand("QUIT");
				m_cmdsocket.Close();
				m_cmdsocket = null;
			}
			
			m_server	= FTPServerType.Unknown;
			m_connected = false;
		
		}

		/// <summary>
		/// retrieve a file from the FTP server
		/// </summary>
		/// <param name="RemoteFileName">Name of the file to get</param>
		/// <param name="LocalFileName">Local name under which to save the retrieved file</param>
		/// <param name="Overwrite">Overwrite the local file if it already exists</param>
		public void GetFile(string RemoteFileName, string LocalFileName, bool Overwrite)
		{
/*
			int			bytesrecvd	= 0;
			FTPResponse response;

			// make sure we're connected
			CheckConnect();

			if(!File.Exists(LocalFileName))
			{
				if(Overwrite)
				{
					System.IO.File.Delete(LocalFileName);
				}
				else
				{
					throw new FTPException("Local File already exists");
				}
			}
			
			FileStream output = File.Create(LocalFileName);

			Socket socket = OpenDataSocket();

			response = SendCommand("RETR " + RemoteFileName);

			if( !((response.ID == 150) || (response.ID == 125)) )
			{
				if(!m_exceptions)
				{
					return;
				}
				else
				{
					throw new IOException(response.Text);
				}
			}

			// get the data
			while(true)
			{
				bytesrecvd = socket.Receive(m_buffer, m_buffer.Length, 0);
				output.Write(m_buffer, 0, bytesrecvd);

				if(bytesrecvd <= 0)
				{
					break;
				}
			}

			output.Close();
			if(socket.Connected)
			{
				socket.Close();
			}

			response = ReadResponse();

			if(response.ID == 0)
			{
				return;
			}

			if( !((response.ID == 226) || (response.ID == 250)) )
			{
				if(!m_exceptions)
				{
					return;
				}
				else
				{
					throw new FTPException(response.Text);
				}
			}
*/			
		}

		/// <summary>
		/// Send a file to the FTP server
		/// </summary>
		/// <param name="LocalFile">Name of the local file to send</param>
		/// <param name="RemoteFileName">Name to save the remote file as</param>
		public void SendFile(FileStream LocalFile, string RemoteFileName)
		{
			/*
			FTPResponse response;
			int			bytesSent	= 0;

			// make sure we're connected
			CheckConnect();

			Socket socket = OpenDataSocket();

			// send the STOR command
			response = SendCommand("STOR " + Path.GetFileName(RemoteFileName));

			if( !((response.ID == 125) || (response.ID == 150)) )
			{
				if(!m_exceptions)
				{
					return;
				}
				else
				{
					throw new FTPException(response.Text);
				}
			}

			// seek to the start of the file
			LocalFile.Seek(0, SeekOrigin.Begin);
			
			// send the data
			while((bytesSent = LocalFile.Read(m_buffer, 0, m_buffer.Length)) > 0)
			{
				socket.Send(m_buffer, bytesSent, 0);
			}

			if (socket.Connected)
			{
				socket.Close();
			}

			response = ReadResponse();
			if( !((response.ID == 226) || (response.ID == 250)) )
			{
				if(!m_exceptions)
				{
					return;
				}
				else
				{
					throw new FTPException(response.Text);
				}
			}
			*/
		}

		/// <summary>
		/// Retrieves the filelist string as the FTP server sends it
		/// <seealso cref="EnumFiles"/>
		/// </summary>
		/// <param name="Detailed">When set file sizes and dates are also retrieved (LIST)</param>
		/// <returns>The server file list as a string</returns>
		public string GetFileList(bool Detailed)
		{
			/*
			FTPResponse		response;
			byte[]			buffer = new byte[1024];

			Socket socket = OpenDataSocket();

			response = ReadResponse();

			if(Detailed)
			{
				response = SendCommand("LIST");
			}
			else
			{
				response = SendCommand("NLST");
			}

			if( !((response.ID == 150) || (response.ID == 125)) )
			{
				if(!m_exceptions)
				{
					return "";
				}
				else
				{
					throw new IOException(response.Text);
				}
			}

			StringBuilder dirInfo = new StringBuilder(buffer.Length);

			// get the data
			for( ; socket.Available > 0 ; )
			{
				int bytesrecvd = socket.Receive(buffer, buffer.Length, SocketFlags.None);

				dirInfo.Append(Encoding.ASCII.GetString(buffer, 0, bytesrecvd));
				
				buffer.Initialize();
			}

			if(socket.Connected)
			{
				socket.Close();
			}

			return dirInfo.ToString();
			*/

			return null;
		}

		/// <summary>
		/// Get a list of the files in the current remote directory
		/// <seealso cref="FTPFiles"/>
		/// <seealso cref="RemoteDirectory"/>
		/// </summary>
		/// <returns>An FTPFiles of the remote files</returns>
		public FTPFiles EnumFiles()
		{
			FTPFiles		files = new FTPFiles();			
	
			string fileList = GetFileList(true); 
			
			if(fileList.Length == 0)
			{
				return null;
			}


			string[] lines = fileList.Replace("\n", "").Split('\r');
			DateTime	filedate = new DateTime(0);
			int			filesize = 0;
			FTPFileType filetype = FTPFileType.File;
			int			pos		 = 0;

			switch(m_server)
			{
				case FTPServerType.Windows:
				{
					for(int l = 0 ; l < lines.Length ; l++)
					{
						if(lines[l].Trim().Length == 0)
						{
							continue;
						}

						// get file date
						try
						{
							filedate = Convert.ToDateTime(lines[l].Substring(0,17));
						}
						catch(Exception)
						{
							continue;
						}

						lines[l] = lines[l].Substring(18).Trim();
				
						// get type
						if(lines[l].IndexOf("<DIR>") > -1)
						{
							// IIS directory
							filetype = FTPFileType.Directory;
							lines[l] = lines[l].Replace("<DIR>", "").Trim();

							// no size
							filesize = -1;
						}
						else
						{
							filetype = FTPFileType.File;

							// get size
							pos = lines[l].IndexOf(' ');

							if(pos < 0)
							{
								continue;
							}

							filesize = Convert.ToInt32(lines[l].Substring(0, pos));
							lines[l] = lines[l].Substring(pos).Trim();
						}

						files.Add(new FTPFile(lines[l], filetype, filesize, filedate));
					}

					return files;
				}
				case FTPServerType.Unix:
				{
					for(int l = 0 ; l < lines.Length ; l++)
					{
						if(lines[l].Length == 0)
						{
							continue;
						}

						if(lines[l][0] == 'd')
						{
							filetype = FTPFileType.Directory;
						}
						else
						{
							filetype = FTPFileType.File;
						}

						// crop the permissions info, etc.
						lines[l] = lines[l].Substring(30).Trim();

						// get size
						pos = lines[l].IndexOf(' ');

						if(pos < 0)
						{
							continue;
						}

						try
						{
							filesize = Convert.ToInt32(lines[l].Substring(0, pos));
						}
						catch(Exception)
						{
							continue;
						}

						lines[l] = lines[l].Substring(pos).Trim();

						// get file date
						try
						{
							int y, m, d, h, n;

							switch(lines[l].Substring(0, 3).ToLower())
							{
								case "jan":
									m = 1;
									break;
								case "feb":
									m = 2;
									break;
								case "mar":
									m = 3;
									break;
								case "apr":
									m = 4;
									break;
								case "may":
									m = 5;
									break;
								case "jun":
									m = 6;
									break;
								case "jul":
									m = 7;
									break;
								case "aug":
									m = 8;
									break;
								case "sep":
									m = 9;
									break;
								case "oct":
									m = 10;
									break;
								case "nov":
									m = 11;
									break;
								case "dec":
									m = 12;
									break;
								default:
									m = 1;
									break;
							}

							d = int.Parse(lines[l].Substring(4, 2));

							if(lines[l].IndexOf(':') > -1)
							{
								// time

								h = int.Parse(lines[l].Substring(7, 2));
								n = int.Parse(lines[l].Substring(10, 2));
								y = DateTime.Now.Year;
							}
							else
							{
								// year
								h = 0;
								n = 0;

								y = int.Parse(lines[l].Substring(8, 4));
							}

							filedate = new DateTime(y, m, d, h, n, 0);
						}
						catch(Exception)
						{
							filedate = DateTime.MinValue;
						}

						lines[l] = lines[l].Substring(12).Trim();

						files.Add(new FTPFile(lines[l], filetype, filesize, filedate));
					}
					return files;
				}
				default:
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Change the remote working directory
		/// </summary>
		/// <param name="Directory">Name of directory to use</param>
		public void ChangeDirectory(string Directory)
		{
			FTPResponse response;

			// make sure we're connected
			CheckConnect();

			response = SendCommand("CWD " + Directory);

			if(response.ID != 250)
			{
				if(!m_exceptions)
				{
					return;
				}
				else
				{
					throw new FTPException(response.Text);
				}
			}
		}

		/// <summary>
		/// Delete a file on the FTP server
		/// </summary>
		/// <param name="FileName">Name of the file to delete</param>
		public void DeleteFile(string FileName)
		{
			FTPResponse response;

			// make sure we're connected
			CheckConnect();

			response = SendCommand("DELE " + FileName);

			if(response.ID != 250)
			{
				if(!m_exceptions)
				{
					return;
				}
				else
				{
					throw new FTPException(response.Text);
				}
			}
		}

		/// <summary>
		/// Delete an empty directory from the FTP server
		/// </summary>
		/// <param name="Directory">Directory to remove</param>
		public void DeleteDirectory(string Directory)
		{
			FTPResponse response;

			// make sure we're connected
			CheckConnect();

			response = SendCommand("RMD " + Directory);

			if(response.ID != 250)
			{
				if(!m_exceptions)
				{
					return;
				}
				else
				{
					throw new FTPException(response.Text);
				}
			}
		}

		/// <summary>
		/// Create a directory on the FTP server
		/// </summary>
		/// <param name="Directory">Name of the directory to create</param>
		public void CreateDirectory(string Directory)
		{
			FTPResponse response;

			// make sure we're connected
			CheckConnect();

			response = SendCommand("MKD " + Directory);
			if(response.ID != 257)
			{
				if(!m_exceptions)
				{
					return;
				}
				else
				{
					throw new FTPException(response.Text);
				}
			}
		}

		private FTPResponse SendCommand(string Command, bool CheckConnection)
		{
			// make sure we're connected
			if(CheckConnection)
				CheckConnect();

			// encode the command
			byte[] bytes = Encoding.ASCII.GetBytes((Command + "\r\n").ToCharArray());

			// send it
			m_cmdsocket.Send(bytes, bytes.Length, 0);

			if(CommandSentEvent != null)
			{
				foreach(FTPCommandHandler ch in CommandSentEvent.GetInvocationList())
				{
					ch(Command);
				}
			}
			
			return ReadResponse();
		}

		/// <summary>
		/// Send a generic or server-specific command to the FTP server
		/// <seealso cref="FTPResponse"/>
		/// </summary>
		/// <param name="Command">Command to send</param>
		/// <returns>The server's response to the command</returns>
		public FTPResponse SendCommand(string Command)
		{
			return SendCommand(Command, true);
		}

		/// <summary>
		/// Rename a file on the FTP server
		/// </summary>
		/// <param name="CurrentFileName">The file's current name</param>
		/// <param name="NewFileName">The new name for the file</param>
		public void RenameFile(string CurrentFileName, string NewFileName)
		{
			FTPResponse response;

			// make sure we're connected
			CheckConnect();

			response = SendCommand("RNFR " + CurrentFileName);
			if(response.ID != 350)
			{
				if(!m_exceptions)
				{
					return;
				}
				else
				{
					throw new FTPException(response.Text);
				}
			}

			// this will overwrite the newfilename if it exists!
			// we should add a check here
			response = SendCommand("RNTO " + NewFileName);
			if(response.ID != 250)
			{
				if(!m_exceptions)
				{
					return;
				}
				else
				{
					throw new FTPException(response.Text);
				}
			}
		}

		private FTPResponse ReadResponse()
		{
			FTPResponse response		= new FTPResponse();
			string		responsetext	= "";
			int			bytesrecvd		= 0;

			// make sure any command sent has enough time to respond
#if CF
			ThreadEx.Sleep(750);
#else
			Thread.Sleep(750);
#endif

			for( ; m_cmdsocket.Available > 0 ; )
			{
				bytesrecvd = m_cmdsocket.Receive(m_buffer, m_buffer.Length, 0);
				responsetext += Encoding.ASCII.GetString(m_buffer, 0, bytesrecvd);
			}

			if(responsetext.Length == 0)
			{
				response.ID = 0;
				response.Text = "";
				return response;
			}

			string[] message = responsetext.Replace("\r", "").Split('\n');

			// we may have multiple responses, 
			// particularly if retriving small amounts of data like directory listings
			// such as the command sent and transfer complete together
			// a response may also have multiple lines
			for(int m = 0 ; m < message.Length ; m++)
			{
				try
				{
					// is the first line a response?  If so, the first 3 characters
					// are the response ID number
					FTPResponse resp = new FTPResponse();
					try
					{
						resp.ID = int.Parse(message[m].Substring(0, 3));
					}
					catch(Exception)
					{
						resp.ID = 0;
					}

					resp.Text = message[m].Substring(4);

					if(ResponseEvent != null)
					{
						foreach(FTPResponseHandler rh in ResponseEvent.GetInvocationList())
						{
							rh(resp);
						}
					}

					if(m == 0)
					{
						response = resp;
					}
				}
				catch(Exception)
				{
					continue;
				}

				return response;
			}

			// return the first response received
			return response;
		}

		private void CheckConnect()
		{
			if(! m_connected)
			{
				throw new FTPException("Method only valid with an open connection");
			}
		}
/*
		private Socket OpenDataSocket()
		{
			FTPResponse response;
			int			port;
			string		ipAddress;

			if(m_mode == FTPMode.Passive)
			{
				response = SendCommand("PASV");

				if(response.ID != 227)
				{
					throw new FTPException(response.Text);
				}

				// get the remote server's IP address
				int start = response.Text.IndexOf('(');
				int end = response.Text.IndexOf(')');

				string ipstring = response.Text.Substring(start + 1, end - start - 1);
				int[] parts = new int[6];

				int part = 0;
				string buffer="";

				for (int i = 0 ;(i < ipstring.Length) && (part <= 6); i++)
				{
					char ch = ipstring[i];
					if (char.IsDigit(ch))
					{
						buffer += ch;
					}

					else if(ch != ',')
					{
						throw new FTPException("Malformed PASV reply: " + response.Text);
					}

					if( (ch == ',') || (i + 1 == ipstring.Length) )
					{

						try
						{
							parts[part++] = int.Parse(buffer);
							buffer="";
						}
						catch (Exception)
						{
							throw new FTPException("Malformed PASV reply: " + response.Text);
						}
					}
				} // for (int i = 0 ;(i < ipstring.Length) && (part <= 6); i++)

				ipAddress = parts[0] + "."+ parts[1]+ "." +
					parts[2] + "." + parts[3];

				port = (parts[4] << 8) + parts[5];
			}
			else
			{
				IPHostEntry ipHost = Dns.GetHostByName(Dns.GetHostName());

				long ip = ipHost.AddressList[0].Address;

				string dotip = (ip & 0xFF) + ", " 
					+ ((ip & (0xFF << 8)) >> 8) + ", "
					+ ((ip & (0xFF << 16)) >> 16) + ", "
					+ ((ip & (0xFF << 24)) >> 24);
				
				dotip += (", " + (m_port & (0xFF << 8)));
				dotip += (", " + (m_port & 0xFF));

				response = SendCommand("PORT " + dotip);

				if(response.ID != 200)
				{
					throw new FTPException(response.Text);
				}
				
				ipAddress = m_host;
				port = m_port;
			}

			Socket socket = new Socket(AddressFamily.InterNetwork, 
				SocketType.Stream,
				ProtocolType.Tcp);

//			socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 1000);

			IPEndPoint endpoint = new IPEndPoint(Dns.Resolve(ipAddress).AddressList[0], port);

			try
			{
				socket.Connect(endpoint);
			}
			catch(Exception)
			{
				throw new FTPException("Can't connect to remote server");
			}

			System.Threading.Thread.Sleep(1000);

			return socket;
		}
*/
		#endregion

		#region Properties

		/// <summary>
		/// Sets the size of the internal buffer used for FTP transfers.  Default is 512 bytes
		/// </summary>
		public int BufferSize
		{
			get 
			{ 
				return m_buffer.Length; 
			}
			set 
			{ 
				m_buffer = new byte[value]; 
			}
		}

		/// <summary>
		/// The remote server type
		/// </summary>
		public FTPServerType ServerType
		{
			get
			{
				return m_server;
			}
		}

		/// <summary>
		/// Gets the current connection state
		/// </summary>
		public bool IsConnected
		{
			get
			{
				return m_connected;
			}
		}
/*
		public FTPMode Mode 
		{
			get
			{
				return m_mode;
			}
			set
			{
				m_mode = value;		
			}
		}
*/
		/// <summary>
		/// The port on which to connect
		/// </summary>
		public int Port
		{
			get
			{
				return m_uri.Port;
			}
			/*
			set
			{
				if(m_connected)
				{
					throw new FTPException("Cannot change port while connected");
				}

				m_port = value;
			}
			*/
		}

		/// <summary>
		/// The connection's remot host
		/// </summary>
		public string RemoteHost
		{
			get
			{
				return m_uri.Host;
			}
/*
			set
			{
				if(m_connected)
				{
					throw new FTPException("Cannot change host while connected");
				}

				m_host = value;
			}
*/
		}

		/// <summary>
		/// The remote directory
		/// </summary>
		public string RemoteDirectory
		{
			get
			{
				string	dir = "";
				int		pos	= 0;

				if(!m_connected)
				{
					throw new FTPException("Cannot get remote directory without an open connection");
				}

				FTPResponse response = SendCommand("PWD");

				dir = response.Text;
				pos = dir.IndexOf("\"") + 1;
				dir = dir.Substring(pos);
				pos = dir.IndexOf("\"");
				dir = dir.Substring(0, pos);

				return dir;
			}
			set
			{
				if(value == "")
				{
					value = "\\";
				}

				if(m_connected)
				{
					FTPResponse response;

					response = SendCommand("CWD " + value);

					if(response.ID != 250)
					{
						if(!m_exceptions)
						{
							return;
						}
						else
						{
							throw new FTPException(response.Text);
						}
					}
				}
			}
		}

		/// <summary>
		/// The username used to make the connection
		/// </summary>
		public string Username
		{
			get
			{
				return m_uid;
			}
		}

		/// <summary>
		/// Timeout for FTP operations
		/// </summary>
		public int ConnectionTimeout
		{
			get
			{
				return m_timeout;
			}
			set
			{
				if(value <= 0)
				{
					throw new ArgumentOutOfRangeException("Timeout must be > 0");
				}

				m_timeout = value;
			}
		}
		
		/// <summary>
		/// The transfer type
		/// <seealso cref="FTPTransferType"/>
		/// </summary>
		public FTPTransferType TransferType
		{
			get
			{
				return m_type;
			}
			set
			{
				FTPResponse response;
				m_type = value;

				if(m_type == FTPTransferType.Binary)
				{
					response = SendCommand("TYPE I");
				}
				else
				{
					response = SendCommand("TYPE A");
				}

				if (response.ID != 200)
				{
					if(!m_exceptions)
					{
						return;
					}
					else
					{
						throw new FTPException(response.Text);
					}
				}
			}
		}

		#endregion Properties
	}
}
