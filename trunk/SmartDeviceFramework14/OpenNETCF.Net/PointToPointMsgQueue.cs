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

namespace OpenNETCF.Net
{
	/// <summary>
	/// The PointToPointMsgQueue class wraps the Windows CE point-to-
	/// point message queue object type.  It is used primarily for
	/// interprocess communication between device drivers, who send
	/// notifications of change of status, etc. and applications, who
	/// might display status information or take action when the status
	/// changes.
	/// </summary>
	public class PointToPointMsgQueue
	{
		internal IntPtr		hMsgQueue;
		internal bool		created;

		/// <summary>
		/// Create a new point-to-point message queue object.
		/// </summary>
		/// <param name="name">
		/// Indicates the name of a named queue.  Set to null to create
		/// an unnamed queue.
		/// </param>
		/// <param name="maxMessages">
		/// Indicates the length of the queue in messages.
		/// </param>
		/// <param name="maxMessageSize">
		/// Indicates the maximum size of each message in the queue.
		/// </param>
		/// <param name="readAccess">
		/// Set to true if messages will be read from the queue, false
		/// if messages will be written to the queue.  A read/write
		/// queue cannot be created.
		/// </param>
		/// <param name="flags">
		/// Flags indicating how the queue will operate.
		/// </param>
		public PointToPointMsgQueue( string name, 
			int maxMessages, int maxMessageSize, 
			bool readAccess, PointToPointMsgQueueFlags flags )
		{
			hMsgQueue = IntPtr.Zero;
			created = false;

			// Set up the descriptor for the queue, based on the
			// parameters.
			MSGQUEUEOPTIONS	opt = new MSGQUEUEOPTIONS();
			opt.dwFlags = (int)flags;
			opt.dwMaxMessages = maxMessages;
			opt.cbMaxMessage = maxMessageSize;
			opt.bReadAccess = ( readAccess ? 1 : 0 );

			// Actually create the queue.
			hMsgQueue = PointToPointMsgQueuePInvokes.CreateMsgQueue( name, opt );
			if ( hMsgQueue != IntPtr.Zero )
			{
				// Get indication of whether we created the queue or
				// if we just attached to an existing queue.
				uint	err = PointToPointMsgQueuePInvokes.GetLastError();
				if ( err == 0 )
				{
					created = true;
				}
				else
				{
					created = false;
				}
			}
		}

		/// <summary>
		/// The queue is closed.
		/// </summary>
		~PointToPointMsgQueue()
		{
			this.Close();
		}

		/// <summary>
		/// Closes the message queue.
		/// </summary>
		public void Close()
		{
			if ( hMsgQueue != IntPtr.Zero )
			{
				PointToPointMsgQueuePInvokes.CloseMsgQueue( hMsgQueue );
				hMsgQueue = IntPtr.Zero;
			}
		}

		/// <summary>
		/// Read the next item from the message queue.
		/// </summary>
		/// <param name="buffer">
		/// Caller-allocated byte array for the data.
		/// </param>
		/// <param name="bufferSize">
		/// Number of bytes in the caller-allocated buffer.
		/// </param>
		/// <param name="bytesRead">
		/// Return value indicating number of bytes actually read and
		/// stored in the buffer.
		/// </param>
		/// <param name="timeout">
		/// Time-out value in ms for the read.  If no messages arrive
		/// before the time-out occurs, the number of bytes read will
		/// be zero.
		/// </param>
		/// <param name="readFlags">
		/// Will be set to AlertMsg if the message was written to the
		/// queue with that flag set.  Zero, otherwise.
		/// </param>
		/// <returns>
		/// True is returned on a successful read; false otherwise.
		/// </returns>
		public bool ReadMsgQueue( byte[] buffer, int bufferSize, 
			ref int bytesRead, int timeout, 
			ref PointToPointMsgQueueFlags readFlags )
		{
			int		fl = 0;
			bool	result =  PointToPointMsgQueuePInvokes.ReadMsgQueue( 
						hMsgQueue,
						buffer, bufferSize, ref bytesRead, timeout, 
						ref fl );

			readFlags = (PointToPointMsgQueueFlags)fl;

			// Check the result and throw exceptions on errors.
			// ????

			return result;
		}

		/// <summary>
		/// Write a new item to the message queue.
		/// </summary>
		/// <param name="buffer">
		/// Caller-allocated byte array for the data.
		/// </param>
		/// <param name="bufferSize">
		/// Number of bytes in the caller-allocated buffer.
		/// </param>
		/// <param name="timeout">
		/// Time-out value in ms for the read.  If no messages arrive
		/// before the time-out occurs, the number of bytes read will
		/// be zero.
		/// </param>
		/// <param name="writeFlags">
		/// Can be set to AlertMsg, if an 'alert message' is to be posted
		/// to the queue.  Otherwise, set to zero.
		/// </param>
		/// <returns>
		/// True is returned on a successful write; false otherwise.
		/// </returns>
		public bool WriteMsgQueue( byte[] buffer, int bufferSize, 
			int timeout, PointToPointMsgQueueFlags writeFlags )
		{
			bool	result =  PointToPointMsgQueuePInvokes.WriteMsgQueue( 
						hMsgQueue, buffer, bufferSize, 
						timeout, (int)writeFlags );

			// Check the result and throw exceptions on errors.
			// ????

			return result;
		}

		/// <summary>
		/// Wait for the queue to be signaled, indicating, for a read
		/// queue, that items are in the queue or, for a write queue,
		/// that the queue is not full and can be written.
		/// </summary>
		/// <param name="timeout">
		/// Time to wait in ms.  Set to -1 for infinite wait.
		/// </param>
		/// <returns>
		/// True if the queue is signaled; false if the time-out passed
		/// before the queue was ready.
		/// </returns>
		public bool Wait( int timeout )
		{
			// Wait for the queue to be signaled.  Return true if it
			// is signaled, false if the time-out passes.
			int	res = PointToPointMsgQueuePInvokes.WaitForSingleObject( 
					hMsgQueue, timeout );
			if ( res == 0 )
				return true;
			return false;
		}

		/// <summary>
		/// Access the raw message queue handle.
		/// </summary>
		public IntPtr Handle
		{
			get
			{
				return hMsgQueue;
			}
		}

		/// <summary>
		/// Access indicator of whether the queue was created new
		/// during construction (true) or if an existing queue was 
		/// connected (false).
		/// </summary>
		public bool CreatedNew
		{
			get
			{
				return created;
			}
		}

		/// <summary>
		/// Return indicator of valid queue connection.
		/// </summary>
		public bool Valid
		{
			get
			{
				if ( hMsgQueue != IntPtr.Zero )
					return true;
				return false;
			}
		}
	}

	/// <summary>
	/// The PointToPointMsgQueueFlags enumeration gives the values which
	/// can be passed in the flags parameter when creating a 
	/// PointToPointMsgQueue instance.
	/// </summary>
	[Flags]
	public enum PointToPointMsgQueueFlags : int
	{
		/// <summary>
		/// Set when the length of the message queue is not limited
		/// at creation time.  Set maxMessages to zero, also, when
		/// setting this flag.
		/// </summary>
		NoPrecommit = 0x00000001,

		/// <summary>
		/// Set to allow sending and receiving, even when there is
		/// no complete connection between a writer and one or more
		/// readers.
		/// </summary>
		AllowBroken = 0x00000002,

		/// <summary>
		/// Set during a read/write operation to indicate an 'alert message'
		/// in the queue.
		/// </summary>
		AlertMsg = 0x00000001,
	}

	[StructLayout(LayoutKind.Sequential)]
	internal class MSGQUEUEOPTIONS
	{
		public Int32 dwSize;			// size of the structure
		public Int32 dwFlags;           // behavior of message queue
		public Int32 dwMaxMessages;     // max # of msgs in queue
		public Int32 cbMaxMessage;      // max size of msg
		public Int32  bReadAccess;      // read access requested

		public MSGQUEUEOPTIONS()
		{
			dwSize = 20;
		}
	}

	internal class PointToPointMsgQueuePInvokes
	{
		[DllImport ("coredll.dll", SetLastError=true)]
		public static extern IntPtr CreateMsgQueue( string name, 
			MSGQUEUEOPTIONS opt );

		//		HANDLE WINAPI OpenMsgQueue( HANDLE hSrcProc, HANDLE hMsgQ, LPMSGQUEUEOPTIONS lpOptions);

		[DllImport ("coredll.dll", SetLastError=true)]
		public static extern bool ReadMsgQueue( IntPtr hMsgQ, 
			byte[] lpBuffer, int cbBufferSize,
			ref int lpNumberOfBytesRead, int dwTimeout, 
			ref int pdwFlags );

		[DllImport ("coredll.dll", SetLastError=true)]
		public static extern bool WriteMsgQueue( IntPtr hMsgQ, 
			byte[] lpBuffer, int cbDataSize,
			int dwTimeout, int dwFlags);

		//		BOOL WINAPI GetMsgQueueInfo(HANDLE hMsgQ, LPMSGQUEUEINFO lpInfo);

		[DllImport ("coredll.dll", SetLastError=true)]
		public static extern bool CloseMsgQueue( IntPtr hMsgQ );

		[DllImport("coredll.dll")]
		public static extern uint GetLastError();

		[DllImport("coredll.dll", SetLastError = true)]
		public static extern int WaitForSingleObject(IntPtr hHandle, 
			int dwMilliseconds); 
	}
}
