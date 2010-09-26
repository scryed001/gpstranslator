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
using System.Xml;

using OpenNETCF.Security.Cryptography;

namespace OpenNETCF.Web.Services2
{
	public class XmlEncObject
	{
		public XmlEncObject()
		{
			//defaults
			TargetElement = "Body";
			Id = "EncryptedContent-" + GuidEx.NewGuid().ToString("D");
			Type = PlainTextType.Content;
		}

		public XmlElement securityContextToken;
		public byte [] securityContextKey;
		//header
		public SecurityTokenReference SecTokRef; //symmetric
		public BinarySecurityToken BinSecTok; //response enc
		public EncryptedKey EncKey; 

		public UsernameToken UserTok;
		public string ClearPassword;
		//public string ValueType;
		//public string UserTokenId;

		public string TargetElement;
		public string Id;
		public PlainTextType Type;

		public X509Certificate X509Cert;
		public string KeyId;
		public RSACryptoServiceProvider RSACSP; //only one that can encrypt
		public SymmetricAlgorithm SymmAlg; //3DES or AES
		public string KeyName;

		public DerivedKeyToken derKeyTok;
		public SymmetricAlgorithm keyWrap;
	}
}
