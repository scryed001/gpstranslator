//==========================================================================================
//
//		OpenNETCF.Win32.SafeHandles.SafeWaitHandle
//		Copyright (c) 2005, OpenNETCF.org
//
//		This library is free software; you can redistribute it and/or modify it under 
//		the terms of the OpenNETCF.org Shared Source License.
//
//		This library is distributed in the hope that it will be useful, but 
//		WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
//		FITNESS FOR A PARTICULAR PURPOSE. See the OpenNETCF.org Shared Source License 
//		for more details.
//
//		You should have received a copy of the OpenNETCF.org Shared Source License 
//		along with this library; if not, email licensing@opennetcf.org to request a copy.
//
//		If you wish to contact the OpenNETCF Advisory Board to discuss licensing, please 
//		email licensing@opennetcf.org.
//
//		For general enquiries, email enquiries@opennetcf.org or visit our website at:
//		http://www.opennetcf.org
//
//==========================================================================================
using System;
using OpenNETCF.Runtime.InteropServices;

namespace OpenNETCF.Win32.SafeHandles
{
	/// <summary>
	/// Represents a wrapper class for wait handles.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	public class SafeWaitHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SafeWaitHandle"/> class.
		/// </summary>
		/// <param name="existingHandle">The pre-existing handle to use.</param>
		/// <param name="ownsHandle">true to reliably release the handle during the finalization phase; otherwise, false (not recommended).</param>
		public SafeWaitHandle(IntPtr existingHandle, bool ownsHandle) : base(ownsHandle)
		{
			base.SetHandle(existingHandle);
		}

		/// <summary>
		/// Executes the code required to free a handle.
		/// </summary>
		/// <returns>true if the handle is released successfully; false if a catastrophic failure occurs.</returns>
		protected override bool ReleaseHandle()
		{
			return Core.CloseHandle(handle);
		}

	}
}
