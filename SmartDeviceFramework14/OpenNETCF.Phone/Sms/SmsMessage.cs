//==========================================================================================
//
//		OpenNETCF.Phone.Sms.SmsMessage
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
using System.Runtime.InteropServices;

namespace OpenNETCF.Phone.Sms
{
	/// <summary>
	/// Represents an individual SMS message.
	/// </summary>
	/// <preliminary/>
	public class SmsMessage
	{
		private SmsAddress m_destination;
		
		public SmsMessage()
		{
			
		}

		


		public SmsAddress PhoneNumber
		{
			get
			{
				SmsAddress buffer = new SmsAddress();

				int result = SmsGetPhoneNumber(buffer);

				if(result!=0)
				{
					throw new ExternalException("Error Retrieving Phone Number");
				}
				return buffer;
			}
		}

		[DllImport("sms.dll")]
		private static extern int SmsGetPhoneNumber(byte[] psmsaAddress);

		
	}
}
