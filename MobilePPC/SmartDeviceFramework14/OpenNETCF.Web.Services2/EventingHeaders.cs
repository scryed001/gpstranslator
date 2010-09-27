//==========================================================================================
//
//		OpenNETCF.Windows.Forms.PasswordDeriveBytes
//		Copyright (c) 2003, OpenNETCF.org
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
//		author: casey chesnut
//		http://www.brains-N-brawn.com
//
//==========================================================================================

using System;
using System.Xml;
using System.Xml.Serialization;
using System.Web.Services.Protocols; 

/*
(12)	<wse:SubscriptionEnd>
(13)       <wse:Id>uuid:22e8a584-0d18-4228-b2a8-3716fa2097fa</wse:Id>
(14)       <wse:Code>wse:SourceCanceling</wse:Code>
(15)       <wse:Reason xml:lang='en-US' >
(16)         Event source going off line.
(17)       </wse:Reason>
(18)     </wse:SubscriptionEnd>
*/

namespace OpenNETCF.Web.Services2
{
	[XmlRoot(Namespace=Ns.wsa, ElementName="SubscriptionEnd")]
	public class SubscriptionEndHeader : SoapHeader
	{
		public SubscriptionEndHeader() {}

		public SubscriptionEndHeader(string id)
		{
			/*
			this.Address = new Address();
			if(addressText==null || addressText==String.Empty)
				addressText = "http://schemas.xmlsoap.org/ws/2003/03/addressing/role/anonymous";
			this.Address.text = addressText;
			*/
		}

		[XmlElement(Namespace=Ns.wse)]
		public string Id;
		[XmlElement(Namespace=Ns.wse)]
		public string Code;
		[XmlElement(Namespace=Ns.wse)]
		public Reason Reason;
	}

	public class Reason
	{
		public Reason() {}

		//[XmlAttribute(Namespace=Ns.xmlns)]
		[XmlAttribute()] //TODO
		public string lang = "en-US";
		[XmlText()]
		public string text;
	}

	public enum Code
	{
		//wse:SourceCanceling
		Unsubscribed,
		Expired,
		NotifyToFailure,
		SourceCanceling,
	}
}
