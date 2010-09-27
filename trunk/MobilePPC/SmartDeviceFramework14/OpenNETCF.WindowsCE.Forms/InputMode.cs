//==========================================================================================
//
//		OpenNETCF.WindowsCE.Forms.InputMode
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

// Preliminary code 02/10/2004 Peter Foot

using System;

namespace OpenNETCF.WindowsCE.Forms
{
	/// <summary>
	/// Specifies the input modes that can be set on the Smartphone using an <see cref="InputModeEditor"/>.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	public enum InputMode
	{
		/// <summary>
		/// This is the conventional method for entering text, called multi-tap, which requires up to four key presses depending on the letter being entered.
		/// </summary>
		AlphaABC = 0,
		/// <summary>
		/// Maintains the current T9 or ABC selection for entering alpha characters as set by the user by holding down the star key (*) on a Smartphone.
		/// This mode is shared across the system so that any text box set with this mode, including text box controls in native Windows CE applications, will adhere to the last T9 or ABC choice as set by the user.
		/// This ability to match user preferences makes this the preferred input mode for text boxes that accept alpha characters. 
		/// <para>Note that if the user selects a numeric input mode (123) with the star key for a text box set to AlphaCurrent, the text box will only accept numeric characters but will be set to the current alpha input mode (T9 or ABC) when the text box loses and regains focus.</para>
		/// </summary>
		AlphaCurrent = 3,
		/// <summary>
		/// This input mode is the T9 predictive text input feature on the Smartphone.
		/// </summary>
		AlphaT9 = 1,
		/// <summary>
		/// This input mode accepts only numeric characters and symbols.
		/// </summary>
		Numeric = 2,
	}
}
