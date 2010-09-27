/*======================================================================================= {
OpenNETCF.Threading.EventWaitHandle

Copyright © 2005, OpenNETCF.org

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

// Daniel Moth

namespace OpenNETCF.Threading{

	/// <summary>
	/// Indicates whether an <see cref="EventWaitHandle"/> is reset automatically or manually.
    /// <para><b>New in v1.3</b></para>
	/// </summary>
	public enum EventResetMode{
		/// <summary>
		/// When signaled, the <see cref="EventWaitHandle"/> resets automatically after releasing a single thread.
        /// If no threads are waiting, the EventWaitHandle remains signaled until a thread blocks, and resets after releasing the thread.
		/// </summary>
		AutoReset = 0,
		/// <summary>
		/// When signaled, the <see cref="EventWaitHandle"/> releases all waiting threads, and remains signaled until it is manually reset.
		/// </summary>
		ManualReset = 1,
	}

	/// <summary>
	/// Represents a thread synchronization event.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	public class EventWaitHandle : WaitHandle {
		/// <summary>
		/// Opens an existing named synchronization event.
		/// </summary>
		/// <param name="name">The name of a system event.</param>
		/// <returns>A <see cref="EventWaitHandle"/> object that represents the named system event.</returns>
		public static EventWaitHandle OpenExisting(string name){
			return new EventWaitHandle(NativeMethods.OpenEvent(NativeMethods.EVENT_ALL_ACCESS, false, name));
		}

        /// <summary>
		/// Sets the state of the event to nonsignaled, causing threads to block.
		/// </summary>
		/// <returns>true if the function succeeds; otherwise, false.</returns>
		public bool Reset(){
			return NativeMethods.EventModify(this.Handle, NativeMethods.EVENT.RESET);
		}

		/// <summary>
		/// Sets the state of the event to signaled, allowing one or more waiting threads to proceed.
		/// </summary>
		/// <returns>true if the function succeeds; otherwise, false.</returns>
		public bool Set(){
			return NativeMethods.EventModify(this.Handle, NativeMethods.EVENT.SET);
		}

		/// <summary>
		/// Initializes a newly created <see cref="EventWaitHandle"/> object, specifying whether the wait 
		/// handle is initially signaled, and whether it resets automatically or manually.
		/// </summary>
		/// <param name="initialState">true to set the initial state to signaled, false to set it to nonsignaled.</param>
		/// <param name="mode">An EventResetMode value that determines whether the event resets automatically or manually.</param>
		public EventWaitHandle(bool initialState, EventResetMode mode):this(initialState, mode, null){}

		/// <summary>
		/// Initializes a newly created <see cref="EventWaitHandle"/> object, specifying whether the wait handle is initially signaled, whether it resets automatically or manually, and the name of a system synchronization event.
		/// </summary>
		/// <param name="initialState">true to set the initial state to signaled, false to set it to nonsignaled.</param>
		/// <param name="mode">An Threading.EventResetMode value that determines whether the event resets automatically or manually.</param>
		/// <param name="name">The name of a system-wide synchronization event.</param>
		public EventWaitHandle(bool initialState, EventResetMode mode, string name):this(NativeMethods.CreateEvent(IntPtr.Zero, mode == EventResetMode.ManualReset, initialState, name)){}

		/// <summary>
		/// Initializes a newly created <see cref="EventWaitHandle"/> object, specifying whether the wait handle is initially signaled, whether it resets automatically or manually, the name of a system synchronization event, and a bool variable whose value after the call indicates whether the named system event was created.
		/// </summary>
		/// <param name="initialState">true to set the initial state to signaled, false to set it to nonsignaled.</param>
		/// <param name="mode">An Threading.EventResetMode value that determines whether the event resets automatically or manually.</param>
		/// <param name="name">The name of a system-wide synchronization event.</param>
		/// <param name="createdNew">When this method returns, contains true if the calling thread was granted initial ownership of the named system event; otherwise, false. This parameter is passed uninitialized.</param>
		public EventWaitHandle(bool initialState, EventResetMode mode, string name, out bool createdNew){
			IntPtr h = NativeMethods.CreateEvent(IntPtr.Zero, mode == EventResetMode.ManualReset, initialState, name);
			if (h.Equals(IntPtr.Zero)){
				throw new ApplicationException("Cannot create " + name);
			}
			createdNew = (Marshal.GetLastWin32Error() == NativeMethods.ERROR_ALREADY_EXISTS);
			this.Handle = h;
		}

		/// <summary>
		/// When overridden in a derived class, blocks the current thread until the current <see cref="WaitHandle"/> receives a signal.
		/// </summary>
		/// <returns>true if the current instance receives a signal. if the current instance is never signaled, <see cref="WaitOne(Int32,bool)"/> never returns.</returns>
		public override bool WaitOne(){
			return WaitOne(-1, false);
		}

		/// <summary>
        /// When overridden in a derived class, blocks the current thread until the current <see cref="WaitHandle"/> receives a signal, using 32-bit signed integer to measure the time interval and specifying whether to exit the synchronization domain before the wait.
		/// </summary>
		/// <param name="millisecondsTimeout">The number of milliseconds to wait, or Threading.Timeout.Infinite (-1) to wait indefinitely.</param>
		/// <param name="exitContext">Not Supported - Just pass false.</param>
		/// <returns>true if the current instance receives a signal; otherwise, false.</returns>
		public bool WaitOne(Int32 millisecondsTimeout, bool exitContext){
			return (NativeMethods.WaitForSingleObject(this.Handle, millisecondsTimeout) != NativeMethods.WAIT_TIMEOUT);
		}

		/// <summary>
		/// When overridden in a derived class, blocks the current thread until the current instance receives a signal, using a <see cref="TimeSpan"/> to measure the time interval and specifying whether to exit the synchronization domain before the wait.
		/// </summary>
		/// <param name="timeout">A TimeSpan that represents the number of milliseconds to wait, or a TimeSpan that represents -1 milliseconds to wait indefinitely.</param>
		/// <param name="exitContext">Not Supported - Just pass false.</param>
		/// <returns>true if the current instance receives a signal; otherwise, false.</returns>
		public bool WaitOne(TimeSpan timeout, bool exitContext){
			//corrected bug - now passing TotalMilliseconds - Peter Foot 11/07/2005
			return (NativeMethods.WaitForSingleObject(this.Handle, (int)timeout.TotalMilliseconds) != NativeMethods.WAIT_TIMEOUT);
		}

		//			'Summary:
		//		'When overridden in a derived class, releases all resources held by the current <see cref="WaitHandle"/>.
		public override void Close(){
			GC.SuppressFinalize(this);
			NativeMethods.CloseHandle(this.Handle);
			this.Handle = new IntPtr(-1);
		}

		private EventWaitHandle(IntPtr aHandle):base(){
			if (aHandle.Equals(IntPtr.Zero)){
				throw new ApplicationException("CreateEvent failed");
			}
			this.Handle = aHandle;
		}

		~EventWaitHandle(){
			NativeMethods.CloseHandle(this.Handle);
		}
	}
}
