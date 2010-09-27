//=================================================================================================
//
//		OpenNETCF.ComponentModel.InvalidEnumArgumentException
//		Copyright (C) 2005, OpenNETCF.org
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
//=================================================================================================

using System;

namespace OpenNETCF.ComponentModel
{
	/// <summary>
	/// The exception thrown when using invalid arguments that are enumerators.
	/// </summary>
	public class InvalidEnumArgumentException : System.ArgumentException
	{
		/// <summary>
		/// Initializes a new instance of the InvalidEnumArgumentException class without a message.
		/// </summary>
		public InvalidEnumArgumentException() {	}

		/// <summary>
		/// Initializes a new instance of the InvalidEnumArgumentException class with the specified message.
		/// </summary>
		/// <param name="message">The message to display with this exception.</param>
		public InvalidEnumArgumentException(string message) : base(message) {	}
	}
}