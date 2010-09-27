//==========================================================================================
//
//		OpenNETCF.Phone.AddressType
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
using System;

namespace OpenNETCF.Phone
{
	/// <summary>
	/// Identifies the phone number type specified in the <see cref="Sms.SmsAddress"/> structure.
	/// <para><b>New in v1.1</b></para>
	/// </summary>
	public enum AddressType 
	{
		/// <summary>
		/// Unknown phone number type.
		/// </summary>
		Unknown = 0,
		/// <summary>
		/// Number is expressed in full with country code.
		/// </summary>
		International,
		/// <summary>
		/// Number is expressed without country code.
		/// </summary>
		National,
		/// <summary>
		/// 
		/// </summary>
		NetworkSpecific,
		/// <summary>
		/// 
		/// </summary>
		Subscriber,
		/// <summary>
		/// 
		/// </summary>
		Alphanumeric,
		/// <summary>
		/// 
		/// </summary>
		Abbreviated,
	}
}
