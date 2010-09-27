/*======================================================================================= {
OpenNETCF.Threading.MutexEx

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
using System.Threading;
using System.Runtime.InteropServices;

// Peter Foot

namespace OpenNETCF.Threading
{
	/// <summary>
	/// A synchronization primitive than can also be used for interprocess synchronization.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	public class MutexEx : WaitHandle
	{
		private bool mCreatedNew;

		/// <summary>
		/// Initializes a new instance of the MutexEx class with a Boolean value indicating whether the calling thread should have initial ownership of the mutex, a string that is the name of the mutex, and a Boolean value that, when the method returns, will indicate whether the calling thread was granted initial ownership of the mutex.
		/// </summary>
		/// <param name="initiallyOwned">true to give the calling thread initial ownership of the mutex; otherwise, false.</param>
		/// <param name="name">The name of the Mutex.
		/// If the value is a null reference (Nothing in Visual Basic), the Mutex is unnamed.</param>
		/// <param name="createdNew">When this method returns, contains a Boolean that is true if the calling thread was granted initial ownership of the mutex; otherwise, false.
		/// This parameter is passed uninitialized.</param>
		public MutexEx(bool initiallyOwned, string name, ref bool createdNew) :this(initiallyOwned, name)
		{
			createdNew = mCreatedNew;
		}

		/// <summary>
		/// Initializes a new instance of the MutexEx class with a Boolean value indicating whether the calling thread should have initial ownership of the mutex, and a string that is the name of the mutex.
		/// </summary>
		/// <param name="initiallyOwned">true to give the calling thread initial ownership of the mutex; otherwise, false.</param>
		/// <param name="name">The name of the Mutex.
		/// If the value is a null reference (Nothing in Visual Basic), the Mutex is unnamed.</param>
		/// <exception cref="ApplicationException">Failed to create mutex.</exception>
		public MutexEx(bool initiallyOwned, string name)
		{
			IntPtr hMutex = NativeMethods.CreateMutex(IntPtr.Zero, initiallyOwned, name);
			if(hMutex == IntPtr.Zero)
			{
				throw new ApplicationException("Failure creating mutex: " + Marshal.GetLastWin32Error().ToString("X"));
			}
			if(Marshal.GetLastWin32Error() == NativeMethods.ERROR_ALREADY_EXISTS)
			{
				mCreatedNew = false;
			}
			else
			{
				mCreatedNew = true;
			}

			this.Handle = hMutex;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MutexEx"/> class with a Boolean value indicating whether the calling thread should have initial ownership of the mutex.
		/// </summary>
		/// <param name="initiallyOwned">true to give the calling thread initial ownership of the mutex; otherwise, false.</param>
		public MutexEx(bool initiallyOwned) : this(initiallyOwned, null){}

		/// <summary>
		/// Initializes a new instance of the MutexEx class with default properties.
		/// </summary>
		public MutexEx() : this(false, null){}

		/// <summary>
		/// Releases the <see cref="MutexEx"/> once.
		/// </summary>
		/// <exception cref="ApplicationException">The calling thread does not own the mutex.</exception>
		public void ReleaseMutex()
		{
			if(!NativeMethods.ReleaseMutex(this.Handle))
			{
				throw new ApplicationException("The calling thread does not own the mutex.");
			}
		}

		/// <summary>
		/// Blocks the current thread until the current <see cref="MutexEx"/> receives a signal.
		/// </summary>
		/// <returns>true if the current instance receives a signal. if the current instance is never signaled, <see cref="WaitOne()"/> never returns.</returns>
		public override bool WaitOne()
		{
			return WaitOne(-1, false);
		}

		/// <summary>
		/// When overridden in a derived class, blocks the current thread until the current <see cref="MutexEx"/> receives a signal, using 32-bit signed integer to measure the time interval and specifying whether to exit the synchronization domain before the wait.
		/// </summary>
		/// <param name="millisecondsTimeout">The number of milliseconds to wait, or Threading.Timeout.Infinite (-1) to wait indefinitely.</param>
		/// <param name="notApplicableOnCE">Just pass false</param>
		/// <returns>true if the current instance receives a signal; otherwise, false.</returns>
		public bool WaitOne(Int32 millisecondsTimeout, bool notApplicableOnCE)
		{
			return (NativeMethods.WaitForSingleObject(this.Handle, millisecondsTimeout) != NativeMethods.WAIT_TIMEOUT);
		}

		
		/// <summary>
		/// When overridden in a derived class, blocks the current thread until the current instance receives a signal, using a TimeSpan to measure the time interval and specifying whether to exit the synchronization domain before the wait.
		/// </summary>
		/// <param name="aTs">A TimeSpan that represents the number of milliseconds to wait, or a TimeSpan that represents -1 milliseconds to wait indefinitely.</param>
		/// <param name="notApplicableOnCE">Just pass false</param>
		/// <returns>true if the current instance receives a signal; otherwise, false.</returns>
		public bool WaitOne(TimeSpan aTs, bool notApplicableOnCE)
		{
			return (NativeMethods.WaitForSingleObject(this.Handle, aTs.Milliseconds) != NativeMethods.WAIT_TIMEOUT);
		}


		/// <summary>
		/// Releases all resources held by the current <see cref="WaitHandle"/>
		/// </summary>
		public override void Close()
		{
			GC.SuppressFinalize(this);
			NativeMethods.CloseHandle(this.Handle);
			this.Handle = new IntPtr(-1);
		}

		~MutexEx()
		{
			NativeMethods.CloseHandle(this.Handle);
		}
	}
}
