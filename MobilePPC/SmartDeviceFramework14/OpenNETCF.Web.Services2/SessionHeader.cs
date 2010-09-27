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
 
namespace OpenNETCF.Web.Services2
{
	[XmlRoot(Namespace="http://keithba.com/2003/05/Session")]
	public class SessionHeader : SoapHeader
	{
		public SessionHeader(){}

		[XmlText()]
		public string Id;
		[XmlElement()]
		public Initiate Initiate;
		[XmlElement()]
		public Terminate Terminate;

		//any
		[XmlAnyElement()]
		public XmlElement [] anyElements;
		[XmlAnyAttribute()]
		public XmlAttribute [] anyAttributes;
	}
 
	public class Initiate
	{
		public Initiate(){}
 
		[XmlText()]
		public string Expires;
 
		//any
		[XmlAnyElement()]
		public XmlElement [] anyElements;
		[XmlAnyAttribute()]
		public XmlAttribute [] anyAttributes;
	}
 
	public class Terminate
	{
		public Terminate() {}
 
		//any
		[XmlAnyElement()]
		public XmlElement [] anyElements;
		[XmlAnyAttribute()]
		public XmlAttribute [] anyAttributes;
	}
}