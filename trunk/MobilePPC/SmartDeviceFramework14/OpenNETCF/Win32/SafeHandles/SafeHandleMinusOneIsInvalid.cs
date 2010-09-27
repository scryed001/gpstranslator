//==========================================================================================
//
//		OpenNETCF.Win32.SafeHandles.SafeHandleMinusOneIsInvalid
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
	/// Provides common functionality that supports safe Win32 handle types.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	public abstract class SafeHandleMinusOneIsInvalid : SafeHandle
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SafeHandleMinusOneIsInvalid"/> class.
		/// </summary>
		/// <param name="ownsHandle">true to reliably release the handle during the finalization phase; otherwise, false (not recommended).</param>
		protected SafeHandleMinusOneIsInvalid(bool ownsHandle) : base(new IntPtr(-1), ownsHandle){}

		/// <summary>
		/// Gets a value indicating whether a handle is invalid.
		/// </summary>
		public override bool IsInvalid
		{
			get
			{
				return(handle.ToInt32() == -1);
			}
		}
	}
}
