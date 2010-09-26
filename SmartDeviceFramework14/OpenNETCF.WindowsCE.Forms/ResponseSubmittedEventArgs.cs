//==========================================================================================
//
//		OpenNETCF.WindowsCE.Forms.ResponseSubmittedEventArgs
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
	/// Provides data for the <see cref="Notification.ResponseSubmitted"/> event.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	/// <remarks>The <see cref="Notification.ResponseSubmitted"/> event occurs when the message balloon is dismissed by user input.
	/// You can use the Response property to ascertain values entered into an HTML form in the message balloon or to determine the name of a button or link clicked by the user.</remarks>
	public class ResponseSubmittedEventArgs : EventArgs
	{
		private string m_response;

		/// <summary>
		/// Initializes a new instance of the <see cref="ResponseSubmittedEventArgs"/> class.
		/// </summary>
		/// <param name="response"></param>
		public ResponseSubmittedEventArgs(string response)
		{
			m_response = response;
		}

		/// <summary>
		/// Gets a string containing the name of a selected link or button or the results of a form embedded in the text of the message balloon.
		/// </summary>
		/// <remarks>The following table describes possible values for this property and the HTML elements that can create them.
		/// <list type="table"><listheader><term>Element</term><term></term>HTML example<term>Response value</term></listheader>
		/// <item><term>Name of a link</term><term>&lt;a href="helplink"&gt;Help&lt;/a&gt;</term><term>helplink</term></item>
		/// <item><term>Name of a button</term><term>&lt;input type=button name=OKbutton value="OK"&gt;</term><term>OKbutton</term></item>
		/// <item><term>HTML form results</term><term>&lt;form method=\"GET\" action=notify&gt;</term><term>action=notify</term></item>
		/// </list>
		/// Note that using cmd:#, where # is an integer, as the name for a HTML element prevents the <see cref="Notification.ResponseSubmitted"/> event from being raised.
		/// The cmd:2 identifier performs a special purpose: it displays the notification icon in the title bar making it suitable for Cancel buttons.</remarks>
		public string Response
		{
			get
			{
				return m_response;
			}
		}

	}

	/// <summary>
	/// Represents the method that will handle the <see cref="Notification.ResponseSubmitted"/> event of a <see cref="Notification"/>.
	/// </summary>
	public delegate void ResponseSubmittedEventHandler(object sender, ResponseSubmittedEventArgs respevent);
}
