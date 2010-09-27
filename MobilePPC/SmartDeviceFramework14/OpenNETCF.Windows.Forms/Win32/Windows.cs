//==========================================================================================
//
//		OpenNETCF.Win32.Windows
//		Copyright (c) 2003-2005, OpenNETCF.org
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
using System.Runtime.InteropServices;

namespace OpenNETCF.Win32
{
	/// <summary>
	/// Contains Windows P/Invokes.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	/// <remarks>Previously contained in <see cref="OpenNETCF.Win32.Core"/> and <see cref="OpenNETCF.Win32.Win32Window"/>.</remarks>
	public class Windows
	{
		private Windows(){}

		#region Find Window
		/// <summary>
		/// Find a Window
		/// </summary>
		/// <param name="lpClassName">Can be empty</param>
		/// <param name="lpWindowName">Caption or text of Window to find</param>
		/// <returns>Handle to specified window.</returns>
		public static IntPtr FindWindow(string lpClassName, string lpWindowName)
		{
			IntPtr ptr = IntPtr.Zero;

			ptr = FindWindowCE(lpClassName, lpWindowName);

			if(ptr == IntPtr.Zero)
			{
				throw new WinAPIException("Failed to find Window");
			}

			return ptr;
		}

		[DllImport("coredll.dll", EntryPoint="FindWindowW", SetLastError=true)]
		private static extern IntPtr FindWindowCE(string lpClassName, string lpWindowName);
		#endregion
	}
}
