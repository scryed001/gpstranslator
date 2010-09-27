//==========================================================================================
//
//		OpenNETCF.Phone.Sms.SmsMessageStatus
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

namespace OpenNETCF.Phone.Sms
{
	/// <summary>
	/// Describes the status of an SMS message.
	/// <para><b>New in v1.1</b></para>
	/// </summary>
	/// <preliminary/>
	public class SmsMessageStatus
	{
		internal const int Length = 288;
		private byte[] m_data;

		public SmsMessageStatus()
		{
			m_data = new byte[Length];
		}

		#region Byte Array Support
		public SmsMessageStatus(byte[] data)
		{
			if(data.Length == Length)
			{
				m_data = data;
			}
			else
			{
				throw new ArgumentException("Cannot create SmsMessageStatus object from specified bytes, length is incorrect");
			}
		}

		public byte[] ToByteArray()
		{
			return m_data;
		}
		public static implicit operator byte[](SmsMessageStatus sms)
		{
			return sms.ToByteArray();
		}

		public static implicit operator SmsMessageStatus(byte[] b)
		{
			return new SmsMessageStatus(b);
		}
		#endregion


		/// <summary>
		/// A message identifier returned when calling SmsSendMessage.
		/// </summary>
		/// <preliminary/>
		public int MessageID
		{
			get
			{
				return BitConverter.ToInt32(m_data, 0);
			}
			set
			{
				BitConverter.GetBytes(value).CopyTo(m_data, 0);
			}
		}

		/// <summary>
		/// The status of the message.
		/// </summary>
		/// <preliminary/>
		[CLSCompliant(false)]
		public MessageStatus Status
		{
			get
			{
				return (MessageStatus)BitConverter.ToUInt32(m_data, 4);
			}
		}

		/// <summary>
		/// The destination address of the message.
		/// </summary>
		/// <preliminary/>
		public SmsAddress RecipientAddress
		{
			get
			{
				//copy subset of array to sms address object
				SmsAddress sa = new SmsAddress();
				Buffer.BlockCopy(m_data, 12, sa, 0, SmsAddress.Length);
				return sa;
			}
		}

		/// <summary>
		/// The time when the service center received the sent message.
		/// </summary>
		/// <remarks>For a status information response resulting from a multipart message, this field contains the most recent timestamp of all the multipart messages.</remarks>
		/// <preliminary/>
		public DateTime ServiceCenterTimeStamp
		{
			get
			{
				return DateTime.FromFileTimeUtc(BitConverter.ToInt64(m_data, 272));
			}
		}

		/// <summary>
		/// The time pertaining to the particular outcome defined in <see cref="P:OpenNETCF.Phone.Sms.SmsMessageStatus.MessageStatus">MessageStatus</see>.
		/// </summary>
		/// <remarks>For a status information response resulting from a multipart message, this field contains the most recent discharge timestamp of all the multipart messages.</remarks>
		/// <preliminary/>
		public DateTime DischargeTime
		{
			get
			{
				return DateTime.FromFileTimeUtc(BitConverter.ToInt64(m_data, 280));
			}
		}
	}
}
