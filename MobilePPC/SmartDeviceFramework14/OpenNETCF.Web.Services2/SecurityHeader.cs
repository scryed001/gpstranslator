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
	<wsse:Security soap:mustUnderstand="1" xmlns:wsse="http://schemas.xmlsoap.org/ws/2002/07/secext">
	</wsse:Security>
	*/

namespace OpenNETCF.Web.Services2
{
	//add public member to the auto gen'd proxy 
	//public SecurityHeader securityHeader; 
	//add to WebMethod that will be called on auto gen'd proxy 
	//[System.Web.Services.Protocols.SoapHeaderAttribute("securityHeader")]
	[XmlRoot(Namespace=Ns.wsse, ElementName="Security")] 
	public class SecurityHeader : SoapHeader 
	{ 
		public SecurityHeader() 
		{
			this.MustUnderstand = true;
			//this.UsernameToken = new UsernameToken();
		} 
  
		//optional
		[CLSCompliant(false)]
		[XmlAttribute(Namespace=Ns.S)]
		public string actor;

		//typed
		[XmlElement(Namespace=Ns.wsse)] 
		public UsernameToken UsernameToken;
		[XmlElement(Namespace=Ns.wsse)] 
		public BinarySecurityToken BinarySecurityToken;
		
		[XmlElement(Namespace=Ns.xenc)] 
		public ReferenceList ReferenceList;
		[XmlElement(Namespace=Ns.xenc)] 
		public EncryptedKey EncryptedKey;

		[XmlElement(Namespace=Ns.wsu)] 
		public TimestampHeader Timestamp;

		//any
		//[XmlAnyElement]
		//public XmlElement [] anyElements;
		//[XmlAnyAttribute]
		//public XmlAttribute [] anyAttributes;
	}

	public enum EncodingType
	{
		Base64Binary,
		HexBinary,
	}

	public enum Errors
	{
		//Unsupported
		UnsupportedSecurityToken, //An unsupported token was provided
		UnsupportedAlgorithm, //An unsupported signature or encryption algorithm was used
		//Failure
		InvalidSecurity, //An error was discovered processing the <Security> header.  
		InvalidSecurityToken, //An invalid security token was provided 
		FailedAuthentication, //The security token could not be authenticated or authorized  
		FailedCheck, //The signature or decryption was invalid 
		SecurityTokenUnavailable, //Referenced security token could not be retrieved 
	}
}