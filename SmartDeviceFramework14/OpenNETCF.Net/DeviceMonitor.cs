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
	#region ---------- Internal Classes -----------

	internal class DEVDETAIL
	{
		public const int MAX_DEVDETAIL_SIZE = 64 + 16 + 12;
		public const int MAX_DEVCLASS_NAMELEN = 64;

		internal byte[] data = new byte[ MAX_DEVDETAIL_SIZE ];

		public DEVDETAIL()
		{
		}

		public GuidEx guidDevClass
		{
			get
			{
				// Extract a 16-byte array from the structure and send that
				// to GuidEx to make a new Guid.
				byte[]	b = new byte[ 16 ];
				Array.Copy( data, 0, b, 0, b.Length );

				return new GuidEx( b );
			}
		}

		public int dwReserved
		{
			get
			{
				return BitConverter.ToInt32( data, 16 ); 
			}
		}

		public bool fAttached
		{
			get
			{
				return BitConverter.ToBoolean( data, 20 );
			}
		}

		public int cbName
		{
			get
			{
				return BitConverter.ToInt32( data, 24 ); 
			}
		}

		public string szName
		{
			get
			{
				String	s = System.Text.Encoding.Unicode.GetString( data, 28, cbName );
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
	#endregion

	#region ---------- Device Notifications -----------

	/// <summary>
	/// DeviceNotificationArgs passed to interested parties
	/// when a device notification is fired.  Contains the
	/// device class GUID, a flag indicating whether the
	/// device is attached or detached, and the device name.
	/// </summary>
	public class DeviceNotificationArgs : System.EventArgs
	{
		/// <summary>
		/// GUID of the interface/type/class of the device
		/// being attached or detached.
		/// </summary>
		public	GuidEx					DeviceInterfaceGUID;

		/// <summary>
		/// True if, after the latest event, the device is
		/// connected; false, otherwise.
		/// </summary>
		public	bool					DeviceAttached;

		/// <summary>
		/// The device name being attached/detached.  Might
		/// be a stream driver name like COM1:, or something
		/// more descriptive like Power Manager, depending
		/// on the GUID.
		/// </summary>
		public	string					DeviceName;

		/// <summary>
		/// Constructor for notification arguments.
		/// </summary>
		/// <param name="g">Device class GUID</param>
		/// <param name="att">Device attached, true or false</param>
		/// <param name="nam">Device name</param>
		public DeviceNotificationArgs( GuidEx g, 
			bool att, string nam )
		{
			DeviceInterfaceGUID = g;
			DeviceAttached = att;
			DeviceName = nam;			
		}
	}

	#endregion

	/// <summary>
	/// Class for receiving device notifications of all sorts (storage card
	/// insertions/removals, etc.)  When a change is detected, an event is 
	/// fired to all interested parties.  The parameters of the event 
	/// indicate the GUID of the device interface that changed, whether the
	/// device is now connected or disconnected from the system, and the
	/// name of the device (COM1:, for example), which changed.
	/// </summary>
	public class DeviceStatusMonitor
	{
		/// <summary>
		/// Constructor for DeviceStatusMonitor.  Specifies
		/// the class of notifications desired and whether
		/// notifications should be fired for already-attached
		/// devices.
		/// </summary>
		/// <param name="devclass">
		/// GUID of device class to monitor (or empty to 
		/// monitor *all* device notifications).
		/// </param>
		/// <param name="notifyAlreadyConnectedDevices">
		/// Set to true to receive notifiations for devices
		/// which were attached before we started monitoring;
		/// set to false to see new events only.
		/// </param>
		public DeviceStatusMonitor( GuidEx devclass,
			bool notifyAlreadyConnectedDevices )
		{
			devClass = devclass;
			fAll = notifyAlreadyConnectedDevices;
		}

		/// <summary>
		/// Destructor stops status monitoring.
		/// </summary>
		~DeviceStatusMonitor()
		{
			this.StopStatusMonitoring();
		}

		private	GuidEx					devClass = null;
		private bool					fAll = false;
		private PointToPointMsgQueue	mq = null;
		private ThreadEx				mqt = null;

		/// <summary>
		/// Initiates a worker thread to listen for reports of device
		/// changes.  Listeners can register for notification of these 
		/// changes, which the thread will send.
		/// </summary>
		public void StartStatusMonitoring()
		{
			if ( !Active )
			{
				// Create a point-to-point message queue to get the 
				// notifications.
				mq = new PointToPointMsgQueue( 
					null,	// unnamed
					25, DEVDETAIL.MAX_DEVDETAIL_SIZE,
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
			// Ask the system to notify our message queue when devices of
			// the indicated class are added or removed.
			IntPtr	h = RequestDeviceNotifications( 
						devClass.ToGuid().ToByteArray(), 
						mq.Handle, fAll );

			// Wait for the queue to be signaled.  When it is, decode
			// the message and call any listeners for it.  If the
			// queue closes suddenly, that's our cue to exit, shutting
			// down notifications before we leave.
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
					DEVDETAIL					devDetail = new DEVDETAIL();
					if ( q.ReadMsgQueue( devDetail.getBytes(), 
						DEVDETAIL.MAX_DEVDETAIL_SIZE,
						ref bytes, -1, ref readFlags ) )
					{
						// Handle the event.
						OnDeviceNotification( new DeviceNotificationArgs( 
							devDetail.guidDevClass, devDetail.fAttached,
							devDetail.szName ) );
					}
				}
				else
				{
					System.Diagnostics.Debug.WriteLine( this, "mq is null in monitoring thread.  Exiting..." );
				}
			}

			System.Diagnostics.Debug.WriteLine( this, "exiting DeviceStatusMonitor thread." );

			// Stop notifications to us.
			StopDeviceNotifications( h );
		}

		/// <summary>
		/// Event fired when some aspect of the device's connected status
		/// has changed.
		/// </summary>
		public event System.EventHandler DeviceNotification;

		/// <summary>
		/// Raises the DeviceNotification event.
		/// </summary>
		/// <param name="e">
		/// An EventArgs that contains the event data.
		/// </param>
		protected virtual void OnDeviceNotification(System.EventArgs e)
		{
			if (DeviceNotification != null)
			{
				DeviceNotification(this, e);
			}
		}

		// Global constant devclass values for use with the monitor.
		#region ---------- Notification GUIDs -----------
		/// <summary>
		/// Fired when the block driver for a Storage Manager
		/// device is loaded.
		/// </summary>
		public static GuidEx BLOCK_DRIVER_GUID = new GuidEx( "{A4E7EDDA-E575-4252-9D6B-4195D48BB865}" );

		/// <summary>
		/// Fired when the store, managed by the Storage
		/// Manager is ready.
		/// </summary>
		public static GuidEx STORE_MOUNT_GUID  = new GuidEx( "{C1115848-46FD-4976-BDE9-D79448457004}" );

		/// <summary>
		/// Fired when a FAT filesystem is loaded for a device.
		/// </summary>
		public static GuidEx FATFS_MOUNT_GUID = new GuidEx( "{169E1941-04CE-4690-97AC-776187EB67CC}" );

		/// <summary>
		/// Fired when a CDFS filesystem is loaded.
		/// </summary>
		public static GuidEx CDFS_MOUNT_GUID = new GuidEx( "{72D75746-D54A-4487-B7A1-940C9A3F259A}" );

		/// <summary>
		/// Fired when a UDFS filesystem is loaded.
		/// </summary>
		public static GuidEx UDFS_MOUNT_GUID = new GuidEx( "{462FEDA9-D478-4b00-86BB-51A8E3D10890}" );

		/// <summary>
		/// Fired when a CDDA filesystem is loaded.
		/// </summary>
		public static GuidEx CDDA_MOUNT_GUID = new GuidEx( "{BA6B1343-7980-4d0c-9290-762D527B33AB}" );
		#endregion

		// P/Invoke declarations.
		[DllImport ("coredll.dll", SetLastError=true)]
		private static extern IntPtr RequestDeviceNotifications( 
			byte[] devclass, IntPtr hMsgQ,
			bool fAll );

		[DllImport ("coredll.dll", SetLastError=true)]
		private static extern bool StopDeviceNotifications( 
			IntPtr h );
	}
}
