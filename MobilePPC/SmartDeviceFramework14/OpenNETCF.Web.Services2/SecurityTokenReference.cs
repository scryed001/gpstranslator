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
using System.Text;

/*
<wsse:Security soap:mustUnderstand="1" xmlns:wsse="http://schemas.xmlsoap.org/ws/2002/07/secext">
  <xenc:ReferenceList xmlns:xenc="http://www.w3.org/2001/04/xmlenc#">
    <xenc:DataReference URI="#EncryptedContent-c163b16f-44c7-4eea-ac65-a6ce744e2651" /> 
  </xenc:ReferenceList>
</wsse:Security>
  
<wsse:SecurityTokenReference xmlns:wsse="http://schemas.xmlsoap.org/ws/2002/04/secext"> 
   <wsse:Reference URI="http://www.fabrikam123.com/tokens/Zoe#X509token"/>
</wsse:SecurityTokenReference>

<wsse:SecurityTokenReference>
   <wsse:KeyIdentifier wsu:Id="..." 
                       ValueType="..."
                       EncodingType="...">
      ...
   </wsse:KeyIdentifier>
</wsse:SecurityTokenReference>
*/

namespace OpenNETCF.Web.Services2
{
	//[XmlRoot(Namespace=Ns.wsse)]
	public class SecurityTokenReference
	{
		public SecurityTokenReference() {}

		[XmlAttribute(Namespace=Ns.wsu)] 
		public string Id;

		[XmlElement(Namespace=Ns.wsse)] //
		public Reference Reference;
		[XmlElement(Namespace=Ns.wsse)] //
		public KeyIdentifier KeyIdentifier;
		[XmlElement(Namespace=Ns.xenc)] //
		public ReferenceList ReferenceList;

		//any
		//[XmlAnyAttribute]
		//public XmlAttribute [] anyAttributes;
		//[XmlAnyElement]
		//public XmlElement [] anyElements;
	}

	//[XmlRoot(Namespace=Ns.wsse)]
	public class Reference
	{
		public Reference() {}

		[XmlAttribute()] 
		public string URI;
		[XmlAttribute()] 
		public string ValueType;
		
		[XmlText()] 
		public string text;
	}

	//[XmlRoot(Namespace=Ns.wsse)]
	public class KeyIdentifier
	{
		public KeyIdentifier() {}

		[XmlAttribute(Namespace=Ns.wsu)] 
		public string Id;
		[XmlAttribute()] 
		public string ValueType;
		[XmlAttribute()] 
		public string EncodingType;
		
		[XmlText()] 
		public string text;

		//any
		//[XmlAnyAttribute]
		//public XmlAttribute [] anyAttributes;
	}
}
