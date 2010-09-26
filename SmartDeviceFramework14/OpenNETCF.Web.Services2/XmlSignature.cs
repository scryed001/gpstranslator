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
using System.Xml.Serialization;

namespace OpenNETCF.Web.Services2
{
	public class Signature
	{
		public Signature() {}
	}

	/*
	<ds:KeyInfo Id="..." xmlns:ds="http://www.w3.org/2000/09/xmldsig#">
		<ds:KeyName>CN=Hiroshi Maruyama, C=JP</ds:KeyName>
	</ds:KeyInfo> 
	*/

	//[XmlRoot(Namespace=Ns.ds)]
	public class KeyInfo
	{
		public KeyInfo() {}

		[XmlAttribute(Namespace=Ns.wsu)] 
		public string Id;

		[XmlElement(Namespace=Ns.ds)]
		public string KeyValue;
		[XmlElement(Namespace=Ns.ds)]
		public string KeyName;
		[XmlElement(Namespace=Ns.ds)]
		public string RetrievalMethod;

		//xmlEnc extensions
		[XmlElement(Namespace=Ns.xenc)]
		public EncryptedKey EncryptedKey;
		[XmlElement(Namespace=Ns.xenc)]
		public string AgreementMethod;

		[XmlElement(Namespace=Ns.wsse)]
		public SecurityTokenReference SecurityTokenReference;
	}

	/*
	<element name="RetrievalMethod" type="ds:RetrievalMethodType"/> 
   <complexType name="RetrievalMethodType">
	 <sequence>
	   <element name="Transforms" type="ds:TransformsType" minOccurs="0"/> 
	 </sequence>  
	 <attribute name="URI" type="anyURI"/>
	 <attribute name="Type" type="anyURI" use="optional"/>
   </complexType>
   */
	public class RetrievalMethod
	{
		public RetrievalMethod(){}
		
		//TODO Transforms here or in xenc: ?
		[XmlElement(Namespace=Ns.ds)]
		public Transform [] Transforms;

		[XmlAttribute()]
		public string URI;
		[XmlAttribute()]
		public string Type;
	}

	//ds:
	public class Transform
	{
		public Transform() {}

		//TODO
	}

	/*
	public class KeyName
	{
		public KeyName() {}

		[XmlText()]
		public string Text;
	}
	*/
}
