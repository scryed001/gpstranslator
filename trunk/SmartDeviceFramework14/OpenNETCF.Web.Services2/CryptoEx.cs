//==========================================================================================
//
//		OpenNETCF.Windows.Forms.PasswordDeriveBytes
//		Copyright (c) 2003, OpenNETCF.org
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
//		author: casey chesnut
//		http://www.brains-N-brawn.com
//
//==========================================================================================

using System;
using System.Text;

namespace OpenNETCF.Web.Services2
{
	/// <summary>
	/// for stuff that was in bNb.Sec and hid in OpenNET
	/// </summary>
	public class CryptoEx
	{
		public CryptoEx()
		{
		}

		public static string ByteArrayToString(byte [] data)
		{
			StringBuilder sb = new StringBuilder();
			foreach(byte b in data)
			{
				sb.Append(b.ToString());
				sb.Append(" ");
			}
			return sb.ToString().Trim();
		}
	}
}
