//==========================================================================================
//
//		OpenNETCF.Phone.Enumerations
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
	#region Call Type Enumeration
	/// <summary>
	/// The type of call represented in the <see cref="T:OpenNETCF.Phone.CallLogEntry"/>.
	/// <para><b>New in v1.1</b></para>
	/// </summary>
	public enum CallType : int
	{
		/// <summary>
		/// An unanswered (missed) incoming call.
		/// </summary>
		Missed,
		/// <summary>
		/// An answered incoming call.
		/// </summary>
		Incoming,
		/// <summary>
		/// An outgoing call.
		/// </summary>
		Outgoing,
	}
	#endregion

	#region CallerID Type Enumeration
	/// <summary>
	/// Specifies the availability of Caller ID.
	/// <para><b>New in v1.1</b></para>
	/// </summary>
	public enum CallerIDType : int
	{
		/// <summary>
		/// The Caller ID is unavailable.
		/// </summary>
		Unavailable,
		/// <summary>
		/// The Caller ID is blocked.
		/// </summary>
		Blocked,
		/// <summary>
		/// The Caller ID is available.
		/// </summary>
		Available,
	}
	#endregion

	#region Call Log Seek Enumeration
	/// <summary>
	/// Specifies the location within the <see cref="T:OpenNETCF.Phone.CallLog"/> where a search will begin.
	/// <para><b>New in v1.1</b></para>
	/// </summary>
	public enum CallLogSeek : int
	{ 
		/// <summary>
		/// The search will begin at the start of the call log.
		/// </summary>
		Beginning = 2,
		/// <summary>
		/// The search will begin at the end of the call log.
		/// </summary>
		End = 4
	}   
	#endregion

}
 

 