//==========================================================================================
//
//		OpenNETCF.Windows.Forms.Day
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

namespace OpenNETCF.Windows.Forms
{
	/// <summary>
	/// Specifies the day of the week.
	/// <para><b>New in v1.1</b></para>
	/// </summary>
	/// <seealso cref="System.Windows.Forms.Day"/>
	public enum Day : short
	{
		/// <summary>
		/// A default day of the week specified by the application.
		/// </summary>
		Default		= -1,
		/// <summary>
		/// The day Monday.
		/// </summary>
		Monday		= 0,
		/// <summary>
		/// The day Tuesday.
		/// </summary>
		Tuesday		= 1,
		/// <summary>
		/// The day Wednesday.
		/// </summary>
		Wednesday	= 2,
		/// <summary>
		/// The day Thursday.
		/// </summary>
		Thursday	= 3,
		/// <summary>
		/// The day Friday.
		/// </summary>
		Friday		= 4,
		/// <summary>
		/// The day Saturday.
		/// </summary>
		Saturday	= 5,
		/// <summary>
		/// The day Sunday.
		/// </summary>
		Sunday		= 6,
	}
}
