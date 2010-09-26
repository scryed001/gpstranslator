/*======================================================================================= {
OpenNETCF.Threading.NativeMethods

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

//Peter Foot

namespace OpenNETCF.Threading
{
	/// <summary>
	/// Contains native API calls for Threading related functionality.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	internal class NativeMethods
	{
		private NativeMethods(){}

		public const Int32 WAIT_FAILED = -1;
		public const Int32 WAIT_TIMEOUT = 0x102;
		public const Int32 EVENT_ALL_ACCESS = 0x3;
		public const Int32 ERROR_ALREADY_EXISTS = 183;

		//Events
		public enum EVENT
		{
			PULSE = 1,
			RESET = 2,
			SET = 3,
		}
		
		[DllImport("coredll.dll", SetLastError=true)] 
		public static extern bool EventModify(IntPtr hEvent, EVENT ef);

		[DllImport("coredll.dll", SetLastError=true)]
		public static extern Int32 WaitForSingleObject(IntPtr hHandle, Int32 dwMilliseconds);
		
		[DllImport("coredll.dll", SetLastError=true)]
		public static extern int WaitForMultipleObjects(uint nCount, IntPtr[] lpHandles, bool fWaitAll, uint dwMilliseconds); 
		
		[DllImport("coredll.dll", SetLastError=true)]
		public static extern int MsgWaitForMultipleObjectsEx(uint nCount, IntPtr[] lpHandles, uint dwMilliseconds, uint dwWakeMask, uint dwFlags); 
		
		[DllImport("coredll.dll", SetLastError=true)]
		public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);
		
		[DllImport("coredll.dll", SetLastError=true)]
		public static extern IntPtr OpenEvent(Int32 dwDesiredAccess, bool bInheritHandle, string lpName);
		
		//Handle
		[DllImport("coredll.dll", SetLastError=true)]
		public static extern bool CloseHandle(IntPtr hObject);

		//Semaphore
		[DllImport("coredll.dll", SetLastError=true)]
		public static extern IntPtr CreateSemaphore(IntPtr lpSemaphoreAttributes, Int32 lInitialCount, Int32 lMaximumCount, string lpName);
		[DllImport("coredll.dll", SetLastError=true)]
		public static extern bool ReleaseSemaphore(IntPtr handle, Int32 lReleaseCount, out Int32 previousCount);
		[DllImport("coredll.dll", SetLastError=true)]
		public static extern IntPtr OpenSemaphore(int desiredAccess, bool inheritHandle, string name);
		

		//Thread
		[DllImport("coredll.dll", EntryPoint="TerminateThread", SetLastError = true)]
		public static extern bool TerminateThread(IntPtr hThread, int dwExitCode);

		[DllImport("coredll.dll", EntryPoint="TerminateThread", SetLastError = true)]
		public static extern bool TerminateThread(uint hThread, int dwExitCode);

		[DllImport("coredll.dll", EntryPoint="SuspendThread", SetLastError = true)]
		public static extern uint SuspendThread(IntPtr hThread);

		[DllImport("coredll.dll", EntryPoint="ResumeThread", SetLastError = true)]
		public static extern uint ResumeThread(IntPtr hThread);

		[DllImport("coredll.dll", EntryPoint="CeGetThreadPriority", SetLastError = true)]
		public static extern int CeGetThreadPriority(IntPtr hThread);

		[DllImport("coredll.dll", EntryPoint="CeSetThreadPriority", SetLastError = true)]
		public static extern bool CeSetThreadPriority(IntPtr hThread, int nPriority);

		[DllImport("coredll.dll", EntryPoint="CeSetThreadQuantum", SetLastError = true)]
		public static extern bool CeSetThreadQuantum(IntPtr hThread, uint dwTime);

		//Mutex
		[DllImport("coredll.dll", EntryPoint="CreateMutex", SetLastError=true)]		
		public static extern IntPtr CreateMutex(
			IntPtr lpMutexAttributes, 
			bool InitialOwner, 
			string MutexName);		
		
		[DllImport("coredll.dll",EntryPoint="ReleaseMutex", SetLastError=true)]
		public static extern bool ReleaseMutex(IntPtr hMutex);
	}
}
