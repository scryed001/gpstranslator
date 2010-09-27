//==========================================================================================
//
//		OpenNETCF.WindowsCE.Forms.InputMethod
//		Copyright (c) 2004, OpenNETCF.org
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

// 05/12/2004 Peter Foot

using System;
using OpenNETCF.Win32;

namespace OpenNETCF.WindowsCE.Forms
{
	/// <summary>
	/// Provides the name and class ID for a soft input panel method.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	public class InputMethod
	{
		private Guid m_clsid;
		private string m_name;

		internal InputMethod(Guid clsid, string name)
		{
			m_clsid = clsid;
			m_name = name;
		}

		/// <summary>
		/// Returns an instance of InputMethod from a specific Clsid.
		/// </summary>
		/// <param name="clsid">Class identifier of SIP class.</param>
		/// <returns></returns>
		public static InputMethod FromClsid(Guid clsid)
		{
			string name = "";

			//try and get a name for the SIP
			RegistryKey rksip = Registry.ClassesRoot.OpenSubKey("CLSID\\" + clsid.ToString("B"));
			
			if(rksip != null)
			{
				name = rksip.GetValue("").ToString();
				rksip.Close();
			}

			InputMethod im = new InputMethod(clsid, name);

			return im;
		}

		/// <summary>
		/// Gets a GUID value that contains a GUID that identifies a specific input method.
		/// </summary>
		public Guid Clsid
		{
			get
			{
				return m_clsid;
			}
		}

		/// <summary>
		/// Gets the name of an input method.
		/// </summary>
		public string Name
		{
			get
			{
				return m_name;
			}
		}

		
	}
}
