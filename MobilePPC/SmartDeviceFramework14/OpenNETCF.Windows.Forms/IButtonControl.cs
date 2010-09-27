//=================================================================================================
//
//		OpenNETCF.Windows.Forms.IButtonControl
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
using System.Windows.Forms;

namespace OpenNETCF.Windows.Forms
{
	/// <summary>
	/// Allows a control to act like a button on a form.
	/// </summary>
	public interface IButtonControl
	{
		/// <summary>
		/// Gets or sets the value returned to the parent form when the button is clicked.
		/// </summary>
		/// <value>One of the <see cref="T:System.Windows.Forms.DialogResult"/> values.</value>
		DialogResult DialogResult {get; set;}

		/// <summary>
		/// Notifies a control that it is the default button so that its appearance and behavior is adjusted accordingly.
		/// </summary>
		/// <param name="value">True if the control should behave as a default button; otherwise, False.</param>
		void NotifyDefault(bool value);

		/// <summary>
		/// Generates a Click event for the control.
		/// </summary>
		void PerformClick();
	}
}