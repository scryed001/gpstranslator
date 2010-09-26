/*=======================================================================================

    OpenNETCF.Net
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
using System.Runtime.InteropServices;
using System.Threading;

using OpenNETCF.IO;
using OpenNETCF.Threading;

namespace OpenNETCF.Net
{
	#region ---------- Device Notifications -----------

	internal class NDISUIO_REQUEST_NOTIFICATION
	{
		public const int Size = 8;
		byte[]	data = new byte[ Size ];

		public IntPtr	hMsgQueue
		{
			get 
			{ 
				return (IntPtr)BitConverter.ToUInt32( data, 0 ); 
			}
			set 
			{ 
				byte[]	bytes = BitConverter.GetBytes( (UInt32)value );
				Buffer.BlockCopy( bytes, 0, data, 0, 4 );
			}
		}

		public uint		dwNotificationTypes
		{
			get 
			{ 
				return BitConverter.ToUInt32( data, 4 ); 
			}
			set 
			{ 
				byte[]	bytes = BitConverter.GetBytes( value );
				Buffer.BlockCopy( bytes, 0, data, 4, 4 );
			}
		}

		public byte[] getBytes()
		{
			return data;
		}
	}

	internal class NDISUIO_DEVICE_NOTIFICATION
	{
		public const int Size = 532;

		internal byte[] data = new byte[ Size ];

		public NDISUIO_DEVICE_NOTIFICATION()
		{
		}

		public int dwNotificationType
		{
			get
			{
				return BitConverter.ToInt32( data, 0 ); 
			}
		}

		public string ptcDeviceName
		{
			get
			{
				String	s = System.Text.Encoding.Unicode.GetString( data, 4, Size-4 );
				int		l = s.IndexOf( '\0' );
				if ( l != -1 )
					return s.Substring( 0, l );
				return s;
			}
		}

		public byte[] getBytes()
		{
			return data;
		}
	}

	/// <summary>
	/// The NdisNotificationType enumeration defines the types
	/// of notifications which can be requested from NDIS.
	/// </summary>
	public enum NdisNotificationType
	{
		/// <summary>
		/// NdisResetStart is set when an adapter is being
		/// reset.
		/// </summary>
		NdisResetStart = 0x00000001,

		/// <summary>
		/// NdisResetEnd is sent when the reset process on an
		/// adapter is complete and the adapter is ready to be
		/// rebound.
		/// </summary>
		NdisResetEnd = 0x00000002,

		/// <summary>
		/// NdisMediaConnect is set when the communications
		/// media, an Ethernet cable for example, is connected
		/// to the adapter.
		/// </summary>
		NdisMediaConnect = 0x00000004,

		/// <summary>
		/// NdisMediaDisconnect is set when the communciations
		/// media, an Ethernet cable for example, is disconnected
		/// from the adapter.
		/// </summary>
		NdisMediaDisconnect = 0x00000008,

		/// <summary>
		/// NdisBind is set when one or more protocols, TCP/IP
		/// typically, is bound to the adapter.
		/// </summary>
		NdisBind = 0x00000010,

		/// <summary>
		/// NdisUnbind is set when the adapter is unbound from
		/// one or more protocols.
		/// </summary>
		NdisUnbind = 0x00000020,

		/// <summary>
		/// NdisMediaSpecific is set when some notification not
		/// generally defined for all adapter types occurred.
		/// </summary>
		NdisMediaSpecific = 0x00000040,
	}

	public class AdapterNotificationArgs : System.EventArgs
	{
		public	string					AdapterName;
		public	NdisNotificationType	NotificationType;

		public AdapterNotificationArgs( string aName, 
			NdisNotificationType nType )
		{
			AdapterName = aName;
			NotificationType = nType;
		}
	}

	#endregion

	/// <summary>
	/// Class giving the ability to monitor NDIS adapters for changes
	/// in their state.  When a change is detected, an event is fired
	/// to all interested parties.  The parameters of the event indicate
	/// the type of status change and the name of the adapter which
	/// changed.
	/// </summary>
	public class AdapterStatusMonitor
	{
		public static AdapterStatusMonitor NDISMonitor = new AdapterStatusMonitor();

		private AdapterStatusMonitor()
		{
		}

		~AdapterStatusMonitor()
		{
			this.StopStatusMonitoring();
		}

		private PointToPointMsgQueue	mq = null;
		private ThreadEx				mqt = null;

		/// <summary>
		/// Initiates a worker thread to listen for NDIS-reported
		/// changes to the status of the adapter.  Listeners can
		/// register for notification of these changes, which the
		/// thread will send.
		/// </summary>
		public void StartStatusMonitoring()
		{
			if ( !Active )
			{
				// Create a point-to-point message queue to get the 
				// notifications.
				mq = new PointToPointMsgQueue( 
					null,	// unnamed
					25, NDISUIO_DEVICE_NOTIFICATION.Size,
					true, 0 );

				// Start monitoring thread.
				mqt = new ThreadEx( new ThreadStart( this.StatusThread ) );
				mqt.Start();
			}
		}

		/// <summary>
		/// Stops the worker thread which monitors for changes of status
		/// of the adapter.  This must be done, if monitoring has been
		/// started, before the object is destroyed.
		/// </summary>
		public void StopStatusMonitoring()
		{
			if ( Active )
			{
				if ( mq != null )
				{
					// Close the point-to-point message queue.  This should
					// cause any waits on it to return, allowing the thread
					// to exit.
					PointToPointMsgQueue	q = mq;
					mq = null;
					q.Close();

					// Wait for the monitoring thread to exit.
					mqt.Join( 5000 );
					mqt = null;
				}
			}
		}

		/// <summary>
		/// The Active property is true when the status is
		/// being monitored.  If status monitoring is not
		/// occurring, Active is false.
		/// </summary>
		public bool Active
		{
			get
			{
				return ( mqt != null );
			}
		}

		internal void StatusThread()
		{
			// Ask NDISUIO to send us notification messages for our
			// adapter.
			IntPtr	ndisAccess = FileEx.CreateFile( 
						NDISUIOPInvokes.NDISUIO_DEVICE_NAME, 
						FileAccess.All, 
						FileShare.None,
						FileCreateDisposition.OpenExisting,
						NDISUIOPInvokes.FILE_ATTRIBUTE_NORMAL | NDISUIOPInvokes.FILE_FLAG_OVERLAPPED );
			if ( (int)ndisAccess == FileEx.InvalidHandle )
				return;

			NDISUIO_REQUEST_NOTIFICATION ndisRequestNotification = 
				new NDISUIO_REQUEST_NOTIFICATION();
			ndisRequestNotification.hMsgQueue = mq.Handle;
			ndisRequestNotification.dwNotificationTypes = 
				NDISUIOPInvokes.NDISUIO_NOTIFICATION_MEDIA_SPECIFIC_NOTIFICATION |
				NDISUIOPInvokes.NDISUIO_NOTIFICATION_MEDIA_CONNECT |
				NDISUIOPInvokes.NDISUIO_NOTIFICATION_MEDIA_DISCONNECT |
				NDISUIOPInvokes.NDISUIO_NOTIFICATION_BIND |
				NDISUIOPInvokes.NDISUIO_NOTIFICATION_UNBIND;

			UInt32	xcount = 0;
			if (!NDISUIOPInvokes.DeviceIoControl(ndisAccess, 
				NDISUIOPInvokes.IOCTL_NDISUIO_REQUEST_NOTIFICATION,
				ndisRequestNotification.getBytes(),
				NDISUIO_REQUEST_NOTIFICATION.Size,
				null, 0, ref xcount, IntPtr.Zero))
			{
				System.Diagnostics.Debug.WriteLine( this, "Error in DeviceIoControl to request notifications!" );
			}

			// Each notification will be of this type.
			NDISUIO_DEVICE_NOTIFICATION ndisDeviceNotification = 
				new NDISUIO_DEVICE_NOTIFICATION();

			// Wait for the queue to be signaled.  When it is, decode
			// the message and call any listeners for it.  If the
			// queue closes suddenly, that's our cue to exit, shutting
			// down NDISUIO notifications before we leave.
			PointToPointMsgQueue	q = mq;
			while ( mq != null )	// Check the global object here.
			{
				// Wait.  On return, true indicates that the queue is
				// signaled; false is a chance to check whether we should
				// exit.
				if ( q.Wait( -1 ) )
				{
					// Read the event data.
					int							bytes = 0;
					PointToPointMsgQueueFlags	readFlags = 0;
					if ( q.ReadMsgQueue( ndisDeviceNotification.getBytes(), 
						NDISUIO_DEVICE_NOTIFICATION.Size, 
						ref bytes, -1, ref readFlags ) )
					{
						// Handle the event.
						OnAdapterNotification( new AdapterNotificationArgs( 
							ndisDeviceNotification.ptcDeviceName, 
							(NdisNotificationType)ndisDeviceNotification.dwNotificationType ) );
					}
				}
				else
				{
					System.Diagnostics.Debug.WriteLine( this, "mq is null in monitoring thread.  Exiting..." );
				}
			}

			System.Diagnostics.Debug.WriteLine( this, "exiting AdapterStatusMonitor thread." );

			if (!NDISUIOPInvokes.DeviceIoControl(ndisAccess, 
				NDISUIOPInvokes.IOCTL_NDISUIO_CANCEL_NOTIFICATION,
				null, 0,
				null, 0, ref xcount, IntPtr.Zero))
			{
				System.Diagnostics.Debug.WriteLine( this, "Error in DeviceIoControl to stop notifications!" );
				// ????
			}

			// Don't forget to close our handle to NDISUIO.
			FileEx.CloseHandle( ndisAccess );
		}

		/// <summary>
		/// Event fired when some aspect of the adapter's configuration
		/// or state has changed.
		/// </summary>
		public event System.EventHandler AdapterNotification;

		/// <summary>
		/// Raises the AdapterNotification event.
		/// </summary>
		/// <param name="e">
		/// An EventArgs that contains the event data.
		/// </param>
		protected virtual void OnAdapterNotification(System.EventArgs e)
		{
			if (AdapterNotification != null)
			{
				AdapterNotification(this, e);
			}
		}
	}
}
