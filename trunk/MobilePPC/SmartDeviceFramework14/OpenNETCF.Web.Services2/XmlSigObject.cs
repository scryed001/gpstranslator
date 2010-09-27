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
	public class XmlSigObject
	{
		public XmlSigObject()
		{
			//defaults
			//For digital signing, the WSE signs the entire contents of the <Body> element, 
			//the Created and Expires elements of the WS-Timestamp SOAP header, 
			//and the Action, To, Id, and From elements of the WS-Routing SOAP header.
			Refs = new ArrayList();

			Refs.Add(Elem.Action);

			Refs.Add(Elem.MessageID);

			//Refs.Add(Elem.From);
			Refs.Add(Elem.ReplyTo);
			
			Refs.Add(Elem.To);

			//Refs.Add(Elem.Created);
			//Refs.Add(Elem.Expires);
			Refs.Add(Elem.Timestamp);

			Refs.Add(Elem.Body);
		}

		public XmlElement securityContextToken;
		public byte [] securityContextKey;
		public BinarySecurityToken BinSecTok;
		public UsernameToken UserTok;
		public string ClearPassword;

		public ArrayList Refs;

		public AsymmetricAlgorithm AsymmAlg; //for DSA or RSA signing

		public DerivedKeyToken derKeyTok;
	}
}
