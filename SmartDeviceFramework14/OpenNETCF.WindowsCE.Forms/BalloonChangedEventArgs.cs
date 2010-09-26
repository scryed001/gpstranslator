//==========================================================================================
//
//		OpenNETCF.WindowsCE.Forms.BalloonChangedEventArgs
//		Copyright (c) 2003-2004, OpenNETCF.org
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
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OpenNETCF.WindowsCE.Forms
{
	/// <summary>
	/// Provides data for the <see cref="Notification.BalloonChanged"/> event.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	public class BalloonChangedEventArgs : EventArgs
	{
		private bool m_visible;

		/// <summary>
		/// Initializes a new instance of the <see cref="BalloonChangedEventArgs"/> class.
		/// </summary>
		/// <param name="visible">true to indicate the notification is visible; otherwise, false.</param>
		public BalloonChangedEventArgs(bool visible)
		{
			m_visible = visible;
		}

		/// <summary>
		/// Gets or sets a value indicating whether the message balloon was changed to visible.
		/// </summary>
		/// <value>true if the Visible property on the Notification is true; otherwise, false.</value>
		/// <remarks>You can use this property to track whenever a notification is displayed or hidden.</remarks>
		public bool Visible
		{
			get
			{
				return m_visible;
			}
		}

	}

	/// <summary>
	/// Represents the method that will handle the <see cref="Notification.BalloonChanged"/> event of a <see cref="Notification"/>.
	/// </summary>
	public delegate void BalloonChangedEventHandler(object sender, BalloonChangedEventArgs balevent);
}
