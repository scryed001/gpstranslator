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
using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Xml;

using OpenNETCF.Security.Cryptography;

namespace OpenNETCF.Web.Services2
{
	public class SecConvObject
	{
		public SecConvObject() {}

		public SecConvObject(string id, string address)
		{
			//defaults
			tokenType = new TokenType();
			tokenType.InnerText = Misc.secScSct;
			requestType = new RequestType();
			requestType.InnerText = Misc.secTrustIssue;
			base_ = new Base();
			base_.SecurityTokenReference = new SecurityTokenReferenceType();
			//if(keyId == null)
			//{
				base_.SecurityTokenReference.Reference = new ReferenceType();
				base_.SecurityTokenReference.Reference.URI = "#" + id;
				//http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509v3
				base_.SecurityTokenReference.Reference.ValueType = Misc.tokenProfUsername + "#UsernameToken";
			//}
			//else
			//{
			//	base_.SecurityTokenReference.KeyIdentifier = new KeyIdentifierType();
			//	base_.SecurityTokenReference.KeyIdentifier.URI = "#" + id;
			//	base_.SecurityTokenReference.KeyIdentifier.ValueType = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509SubjectKeyIdentifier";
			//	base_.SecurityTokenReference.KeyIdentifier.InnerText = keyId;
			//}
			appliesTo = new AppliesTo();
			appliesTo.EndpointReference = new EndpointReferenceType();
			appliesTo.EndpointReference.Address = new AddressType();
			appliesTo.EndpointReference.Address.InnerText = address;
			lifeTime = new LifeTime();
			lifeTime.Expires = new ExpiresType();
			lifeTime.Expires.InnerText = DateTime.UtcNow.AddHours(2).ToString(TimestampHeader.dateTimeFormat);
		}

		//request
		public RequestType requestType;
		public Base base_;
		public Entropy entropy;

		//RSA for Entropy
		public X509Certificate X509Cert;
		public string KeyId;
		public RSACryptoServiceProvider RSACSP; //only one that can encrypt
		public SymmetricAlgorithm SymmAlg; //3DES or AES
		public string CarriedKeyName;

		//AES for RPT
		//public Rijndael aes;
		public SymmetricAlgorithm keyWrap;

		//both request and response
		public TokenType tokenType;
		public AppliesTo appliesTo;
		public LifeTime lifeTime;

		//response
		public XmlElement requestedSecurityToken;
		public XmlElement requestedProofToken;
		public byte [] secConvKey;
		public byte [] entropyKey;
	}
}
