//==========================================================================================
//
//		OpenNETCF.Reflection.AssemblyEx
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
using System.Reflection;
using System.Runtime.InteropServices;

// Peter Foot

namespace OpenNETCF.Reflection
{
	/// <summary>
	/// Contains helper functions for the <see cref="System.Reflection.Assembly"/> class.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	/// <seealso cref="System.Reflection.Assembly"/>
	public sealed class AssemblyEx
	{
		private AssemblyEx(){}

		/// <summary>
		/// Gets the process executable.
		/// </summary>
		/// <returns>The <see cref="Assembly"/> that is the process executable.</returns>
		public static Assembly GetEntryAssembly()
		{
			byte[] buffer = new byte[256 * Marshal.SystemDefaultCharSize];
			int chars = GetModuleFileName(IntPtr.Zero, buffer, 255);

			if(chars > 0)
			{
				if(chars > 255)
				{
					throw new System.IO.PathTooLongException("Assembly name is longer than MAX_PATH characters.");
				}

				string assemblyPath = System.Text.Encoding.Unicode.GetString(buffer, 0, chars * Marshal.SystemDefaultCharSize);

				return Assembly.LoadFrom(assemblyPath);
			}
			else
			{
				return null;
			}

		}

		[DllImport("coredll.dll", SetLastError=true)]
		private static extern int GetModuleFileName(IntPtr hModule, byte[] lpFilename, int nSize);

	}
}
