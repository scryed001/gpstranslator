//==========================================================================================
//
//		OpenNETCF.Net.NetworkInformation.NetworkInformationException
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
//
//Submitted by Sergey Bogdanov
//
using System;

namespace OpenNETCF.Net.NetworkInformation
{
	/// <summary>
	/// The exception that is thrown when an error occurs while retrieving network information.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	public class NetworkInformationException : Exception
	{
		int errorCode;

		/// <summary>
		/// Initializes a new instance of the <see cref="NetworkInformationException"/> class.
		/// </summary>
		public NetworkInformationException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NetworkInformationException"/> class.
		/// </summary>
		/// <param name="errorCode">A Win32 error code.</param>
		public NetworkInformationException(int errorCode)
		{
			this.errorCode = errorCode;
		}

		/// <summary>
		/// Gets the Win32 error code for this exception.
		/// </summary>
		public int ErrorCode
		{
            get
            {
                return errorCode;            	
            }
		}
	}
}
