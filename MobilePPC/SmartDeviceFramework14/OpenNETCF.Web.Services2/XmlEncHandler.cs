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
using System.Collections;
using System.Xml;

using OpenNETCF.Security.Cryptography;

namespace OpenNETCF.Web.Services2
{
	public class XmlEncHandler
	{
		public XmlEncHandler() {}

		public static XmlEncObject EncObj;
		public static XmlEncObject DecObj;
		public static SecConvObject SecConvObj;

		public static bool signFirst = true; //see what you sign
		[CLSCompliant(false)]
		public static bool SignFirst
		{
			get{return signFirst;}
			set{signFirst = value;}
		}

		public static XmlDocument EncryptXml(XmlDocument plainDoc)
		{
			if(EncObj == null)
				return plainDoc; //nothing to encrypt

			XmlElement envelope = plainDoc.DocumentElement;

			//add namespace
			//XmlAttribute xenc = xd.CreateAttribute(Pre.xmlns, Pre.xenc, Ns.xmlns);
			//xenc.Value = Ns.xenc;
			//envelope.Attributes.Append(xenc);

			XmlElement headerOrBody = (XmlElement) envelope.ChildNodes[0];
			XmlElement header;
			XmlElement body;
			if(headerOrBody.LocalName == Elem.Body)
			{
				header = plainDoc.CreateElement(headerOrBody.Prefix, Elem.Header, headerOrBody.NamespaceURI);
				envelope.InsertBefore(header, headerOrBody);
			}
			header = (XmlElement) envelope.ChildNodes[0];
			body = (XmlElement) envelope.ChildNodes[1];
			XmlNodeList headers = header.ChildNodes;
			XmlElement security = null;
			foreach(XmlNode xn in headers)
			{
				if(xn.LocalName == Elem.Security)
					security = (XmlElement) xn;
			}
			if(security == null)
			{
				//used to work for SymmetricEncryptionV1
				//if(EncObj.SecTokRef != null) //symmetric is older
				//	security = plainDoc.CreateElement(Pre.wsse, Elem.Security, Ns.wsse0207);
				//else //newest
				security = plainDoc.CreateElement(Pre.wsse, Elem.Security, Ns.wsseLatest);
				XmlAttribute mustUnderstand = plainDoc.CreateAttribute(Pre.soap, Attrib.mustUnderstand, Ns.soap);
				mustUnderstand.Value = "1";
				security.Attributes.Append(mustUnderstand);
				header.AppendChild(security);
			}
			XmlElement tokenElem = null;
			if(EncObj.UserTok != null)
			{
				XmlElement userTokElem = LameXpath.SelectSingleNode(security, Elem.UsernameToken);
				if(userTokElem == null)
					EncObj.UserTok.WriteXml(plainDoc, security);
				tokenElem = userTokElem;
				//secTokId = SigObj.UserTok.Id;
				//sigAlgVal = "http://www.w3.org/2000/09/xmldsig#hmac-sha1";
			}
			/*
			<wsse:Security soap:mustUnderstand="1">
			<wsse:BinarySecurityToken ValueType="wsse:X509v3" 
				EncodingType="wsse:Base64Binary" 
				xmlns:wsu="http://schemas.xmlsoap.org/ws/2002/07/utility" 
				wsu:Id="SecurityToken-b2adaba3-09f7-45a0-aa0d-0c4da15d0725">
					MIIBxDCCAW6...==
			</wsse:BinarySecurityToken> 
			</wsse:Security>
			*/
			if(EncObj.BinSecTok != null)
			{
				XmlElement binSecTok = LameXpath.SelectSingleNode(security, Elem.BinarySecurityToken);
				if(binSecTok == null)
					EncObj.BinSecTok.WriteXml(plainDoc, security);

				tokenElem = binSecTok;
			}
			/*
			<wsse:Security soap:mustUnderstand="1" xmlns:wsse="http://schemas.xmlsoap.org/ws/2002/07/secext">
			<xenc:ReferenceList xmlns:xenc="http://www.w3.org/2001/04/xmlenc#">
			<xenc:DataReference URI="#EncryptedContent-c163b16f-44c7-4eea-ac65-a6ce744e2651" /> 
			</xenc:ReferenceList>
			</wsse:Security>
			*/
			if(EncObj.SecTokRef != null) // || EncObj.ClearPassword != null
			{
				//security.Attributes["xmlns"].Value = Ns.wsse0207;

				XmlElement referenceList = plainDoc.CreateElement(Pre.xenc, Elem.ReferenceList, Ns.xenc);
				XmlElement dataReference = plainDoc.CreateElement(Pre.xenc, Elem.DataReference, Ns.xenc);
				XmlAttribute uri = plainDoc.CreateAttribute(Attrib.URI);
				uri.Value = "#" + EncObj.Id;
				dataReference.Attributes.Append(uri);
				referenceList.AppendChild(dataReference);
				if(SignFirst == false)
					security.AppendChild(referenceList); //just append
				else
					security.InsertAfter(referenceList, tokenElem); //after token
			}
			/*
			<wsse:Security soap:mustUnderstand="1">
			  <xenc:EncryptedKey xmlns:xenc="http://www.w3.org/2001/04/xmlenc#">
			    <xenc:EncryptionMethod Algorithm="http://www.w3.org/2001/04/xmlenc#rsa-1_5" /> 
			    <KeyInfo xmlns="http://www.w3.org/2000/09/xmldsig#">
			      <wsse:SecurityTokenReference>
			        <wsse:KeyIdentifier ValueType="wsse:X509v3">gBfo0147lM6cKnTbbMSuMVvmFY4=</wsse:KeyIdentifier> 
			      </wsse:SecurityTokenReference>
			    </KeyInfo>
			    <xenc:CipherData>
			      <xenc:CipherValue>CKc0qzMkc...==</xenc:CipherValue> 
			    </xenc:CipherData>
			    <xenc:ReferenceList>
			      <xenc:DataReference URI="#EncryptedContent-702cd57e-c5ca-44c6-9bd8-b8639762b036" /> 
			    </xenc:ReferenceList>
			  </xenc:EncryptedKey>
			</wsse:Security>
			*/
			if(EncObj.EncKey != null)
			{
				XmlElement encKeyElem = plainDoc.CreateElement(Pre.xenc, Elem.EncryptedKey, Ns.xenc);
				
				XmlElement encMethElem = plainDoc.CreateElement(Pre.xenc, Elem.EncryptionMethod, Ns.xenc);
				XmlAttribute alg = plainDoc.CreateAttribute(Attrib.Algorithm);
				alg.Value = Alg.rsa15;
				encMethElem.Attributes.Append(alg);
				encKeyElem.AppendChild(encMethElem);
				
				XmlElement keyInfoElem = plainDoc.CreateElement(Pre.ds, Elem.KeyInfo, Ns.ds);				
				XmlElement secTokRefElem = plainDoc.CreateElement(Pre.wsse, Elem.SecurityTokenReference, Ns.wsseLatest);
				XmlElement keyIdElem = plainDoc.CreateElement(Pre.wsse, Elem.KeyIdentifier, Ns.wsseLatest);
				XmlAttribute valueType = plainDoc.CreateAttribute(Attrib.ValueType);
				//valueType.Value = "wsse:X509v3";
				valueType.Value = Misc.tokenProfX509 + "#X509SubjectKeyIdentifier";
				keyIdElem.Attributes.Append(valueType);
				keyIdElem.InnerText = EncObj.KeyId;
				secTokRefElem.AppendChild(keyIdElem);
				keyInfoElem.AppendChild(secTokRefElem);
				encKeyElem.AppendChild(keyInfoElem);
				
				//encrypt key
				byte [] baSessKey = EncObj.SymmAlg.Key;
				byte [] baEncSessKey = EncObj.RSACSP.EncryptValue(baSessKey);
				XmlElement ciphDataElem = plainDoc.CreateElement(Pre.xenc, Elem.CipherData, Ns.xenc);
				XmlElement ciphValElem = plainDoc.CreateElement(Pre.xenc, Elem.CipherValue, Ns.xenc);
				ciphValElem.InnerText = OpenNETCF.Security.Cryptography.NativeMethods.Format.GetB64(baEncSessKey);
				ciphDataElem.AppendChild(ciphValElem);
				encKeyElem.AppendChild(ciphDataElem);

				XmlElement refListElem = plainDoc.CreateElement(Pre.xenc, Elem.ReferenceList, Ns.xenc);
				XmlElement dataRefElem = plainDoc.CreateElement(Pre.xenc, Elem.DataReference, Ns.xenc);
				XmlAttribute uri = plainDoc.CreateAttribute(Attrib.URI);
				uri.Value = "#" + EncObj.Id;
				dataRefElem.Attributes.Append(uri);
				refListElem.AppendChild(dataRefElem);
				encKeyElem.AppendChild(refListElem);

				//security.PrependChild(encKeyElem);
				if(SignFirst == false)
					security.AppendChild(encKeyElem); //just append
				else
					security.InsertAfter(encKeyElem, tokenElem); //after token
			}
			//SecurityContextToken - add here, or with Signature
			string secTokId = null;
			if(EncObj.securityContextToken != null)
			{
				XmlNode sctNode = LameXpath.SelectSingleNode(header, Elem.SecurityContextToken);
				
				if(sctNode == null)
				{
					//i need to import this node 1st
					sctNode = plainDoc.ImportNode(EncObj.securityContextToken, true);
					string dupeId = sctNode.Attributes[Attrib.Id, Ns.wsuLatest].Value;
					XmlElement dupeElem = LameXpath.SelectSingleNode(dupeId, security);
					if(dupeElem == null)
						security.AppendChild(sctNode);
					else
						sctNode = LameXpath.SelectSingleNode(dupeId, security);
				}
				//<wsse:SecurityContextToken wsu:Id=\"SecurityToken-feb27552-6eb5-4a27-a831-e1bdfca326e2\">
				secTokId = sctNode.Attributes[Attrib.Id, Ns.wsuLatest].Value;

				//add ReferenceList too for SecureConversation
				//<xenc:ReferenceList xmlns:xenc="http://www.w3.org/2001/04/xmlenc#">
				//  <xenc:DataReference URI="#EncryptedContent-cb7efc1c-e4dd-4737-9214-aec967789d2d" />
				//</xenc:ReferenceList>
				XmlElement referenceListElem = plainDoc.CreateElement(Pre.xenc, Elem.ReferenceList, Ns.xenc);
				//security.AppendChild(referenceListElem);
				XmlElement dataReferenceElem = plainDoc.CreateElement(Pre.xenc, Elem.DataReference, Ns.xenc);
				XmlAttribute uriAttrib = plainDoc.CreateAttribute(Attrib.URI);
				uriAttrib.Value = "#" + EncObj.Id;
				dataReferenceElem.Attributes.Append(uriAttrib);
				referenceListElem.AppendChild(dataReferenceElem);
				security.InsertAfter(referenceListElem, sctNode);

				if(EncObj.derKeyTok != null)
				{
					XmlNode idElem = LameXpath.SelectSingleNode(sctNode, Elem.Identifier);
					if(idElem != null)
						EncObj.derKeyTok.secTokRef.Reference.URI = idElem.InnerText;

					XmlElement derKeyTokElem = EncObj.derKeyTok.WriteXml(plainDoc, security, (XmlElement) sctNode);
					secTokId = EncObj.derKeyTok.id;

					EncObj.SymmAlg.Key = EncObj.derKeyTok.derKey;
				}
			}
			
			if(EncObj.UserTok != null)
			{
				int numBytes = EncObj.SymmAlg.Key.Length;
				byte [] derKey = P_SHA1.DeriveKey(EncObj.ClearPassword, XmlSigHandler.StrKeyLabel, EncObj.UserTok.Nonce.Text, EncObj.UserTok.Created, numBytes);
				EncObj.SymmAlg.Key = derKey;
			}

			//possibly add BinSecTok, but dont encrypt
			if(EncObj.SymmAlg == null)
				return plainDoc;
			if(EncObj.RSACSP == null && EncObj.UserTok == null && EncObj.securityContextKey == null && EncObj.derKeyTok.derKey == null)
				return plainDoc;

			XmlElement plainElement = LameXpath.SelectSingleNode(envelope, EncObj.TargetElement);
			if(plainElement == null)
				throw new Exception("element not found to encrypt");
			
			byte [] baPlain;
			if(EncObj.Type == PlainTextType.Element)
				baPlain = OpenNETCF.Security.Cryptography.NativeMethods.Format.GetBytes(plainElement.OuterXml);
			else if(EncObj.Type == PlainTextType.Content)
				baPlain = OpenNETCF.Security.Cryptography.NativeMethods.Format.GetBytes(plainElement.InnerXml);
			else
				throw new Exception("only support #Element and #Content");

			//diff algorithms
			SymmetricAlgorithm sa = EncObj.SymmAlg;
			sa.IV = OpenNETCF.Security.Cryptography.NativeMethods.Rand.GetRandomBytes(sa.IV.Length);
			byte [] baCipher = sa.EncryptValue(baPlain);
			byte [] baCipherIv = new byte[baCipher.Length + sa.IV.Length];
			Array.Copy(sa.IV, 0, baCipherIv, 0, sa.IV.Length);
			Array.Copy(baCipher, 0, baCipherIv, sa.IV.Length, baCipher.Length);

			//byte [] baEncTest = Format.GetB64("JgiIRPjmEMW/QVXB6/KRICQrO5B9CSWo8pgqWoAA4TL2BFSkeuerfumauP6lneK8eRHz+iSG2Bcvu+4FXnYjRkQO4We7MdEL5C9HB9hFu7+GTnmTcL+aVh3Ue3bfrJq0");
			//byte [] baTest = tCsp.DecryptValue(baEncTest);
			//string strEncText = Format.GetString(baTest);
			
			/*
			<xenc:EncryptedData Id="EncryptedContent-c163b16f-44c7-4eea-ac65-a6ce744e2651" Type="http://www.w3.org/2001/04/xmlenc#Content" xmlns:xenc="http://www.w3.org/2001/04/xmlenc#">
			<xenc:EncryptionMethod Algorithm="http://www.w3.org/2001/04/xmlenc#tripledes-cbc" /> 
			  <KeyInfo xmlns="http://www.w3.org/2000/09/xmldsig#">
			    <KeyName>WSE Sample Symmetric Key</KeyName> //NORMAL
			    <wsse:SecurityTokenReference> //SecureConversation
                  <wsse:Reference URI="#SecurityToken-be84969f-41c7-4dff-95a4-7319a3122142" />
                </wsse:SecurityTokenReference>			    
			  </KeyInfo>
			  <xenc:CipherData>
			    <xenc:CipherValue>1+uBlSL/pxXyl2FdeT/EVM6TZgW9cv1AjwlJ9LZyKejet9TgjK37QoURZklglS9z+yGd5XooIDhtWPLaw3ApuhRCky6Y8eP1+3mT6v+t3o28idscfYOrkFmVaI25AwHK</xenc:CipherValue> 
			  </xenc:CipherData>
			</xenc:EncryptedData>
			*/
			XmlElement encryptedData = plainDoc.CreateElement(Pre.xenc, Elem.EncryptedData, Ns.xenc);
			XmlAttribute id = plainDoc.CreateAttribute(Attrib.Id);
			id.Value = EncObj.Id;
			encryptedData.Attributes.Append(id);
			XmlAttribute type = plainDoc.CreateAttribute(Attrib.Type);
			type.Value = Misc.plainTextTypeContent; //xeo.Type.ToString();
			encryptedData.Attributes.Append(type);
            XmlElement encryptionMethod = plainDoc.CreateElement(Pre.xenc, Elem.EncryptionMethod, Ns.xenc);
			XmlAttribute algorithm = plainDoc.CreateAttribute(Attrib.Algorithm);
			if(EncObj.SymmAlg is TripleDES)
				algorithm.Value = Alg.tripledesCbc; //xeo.AlgorithmEnum.ToString();
			else
				algorithm.Value = Alg.aes128cbc;
			encryptionMethod.Attributes.Append(algorithm);
			encryptedData.AppendChild(encryptionMethod);
			if((EncObj.KeyName != null && EncObj.KeyName != String.Empty) || EncObj.securityContextToken != null || EncObj.ClearPassword != null)
			{
				XmlElement keyInfo = plainDoc.CreateElement(Pre.ds, Elem.KeyInfo, Ns.ds);
				if(EncObj.KeyName != null && EncObj.KeyName != String.Empty)
				{
					XmlElement keyName = plainDoc.CreateElement(Pre.ds, Elem.KeyName, Ns.ds);
					keyName.InnerText = EncObj.KeyName;
					keyInfo.AppendChild(keyName);
				}
				if(EncObj.securityContextToken != null || EncObj.ClearPassword != null)
				{
					XmlElement securityTokenReferenceElem = plainDoc.CreateElement(Pre.wsse, Elem.SecurityTokenReference, Ns.wsseLatest);
					keyInfo.AppendChild(securityTokenReferenceElem);
					XmlElement referenceElem = plainDoc.CreateElement(Pre.wsse, Elem.Reference, Ns.wsseLatest);
					securityTokenReferenceElem.AppendChild(referenceElem);
					if(EncObj.securityContextToken != null)
					{
						XmlAttribute uriAttrib = plainDoc.CreateAttribute(Attrib.URI);
						uriAttrib.Value = "#" + secTokId;
						referenceElem.Attributes.Append(uriAttrib);
					}
					if(EncObj.UserTok != null)
					{
						XmlAttribute uriAttrib = plainDoc.CreateAttribute(Attrib.URI);
						uriAttrib.Value = "#" + EncObj.UserTok.Id;
						referenceElem.Attributes.Append(uriAttrib);

						XmlAttribute valueTypeAttrib = plainDoc.CreateAttribute(Attrib.ValueType);
						valueTypeAttrib.Value = Misc.tokenProfUsername + "#UsernameToken";
						referenceElem.Attributes.Append(valueTypeAttrib);
					}
				}
				encryptedData.AppendChild(keyInfo);
			}
			XmlElement cipherData = plainDoc.CreateElement(Pre.xenc, Elem.CipherData, Ns.xenc);
			XmlElement cipherValue = plainDoc.CreateElement(Pre.xenc, Elem.CipherValue, Ns.xenc);
			cipherValue.InnerText = OpenNETCF.Security.Cryptography.NativeMethods.Format.GetB64(baCipherIv);
			cipherData.AppendChild(cipherValue);
			encryptedData.AppendChild(cipherData);

			if(EncObj.Type == PlainTextType.Element)
				plainElement.ParentNode.InnerXml = encryptedData.OuterXml;
			else //content
				plainElement.InnerXml = encryptedData.OuterXml;

			SecConvObj = null;
			EncObj = null;
			return plainDoc;
		}

		public static XmlDocument DecryptXml(XmlDocument cipherDoc)
		{
			SecConvObj = null;
			if(DecObj == null)
				return cipherDoc; //no keys to decrypt with

			XmlElement envelope = cipherDoc.DocumentElement;

			//add namespace
			//XmlAttribute xenc = xd.CreateAttribute(Pre.xmlns, Pre.xenc, Ns.xmlns);
			//xenc.Value = Ns.xenc;
			//envelope.Attributes.Append(xenc);

			XmlElement headerOrBody = (XmlElement) envelope.ChildNodes[0];
			XmlElement header = null;
			XmlElement body = null;
			if(headerOrBody.LocalName == Elem.Header)
			{
				header = (XmlElement) envelope.ChildNodes[0];
				body = (XmlElement) envelope.ChildNodes[1];
			}
			else //no header
			{
				body = (XmlElement) envelope.ChildNodes[0];
			}

			string encKeyMethod = null;
			byte [] baEncKey = null;
			string encKeyId = null;
			//UsernameToken encryption
			XmlElement nonce = null;
			XmlElement created = null;
			//search for Security in Header, remove MustUnderstand
			if(header != null)
			{
				XmlElement securityElem = LameXpath.SelectSingleNode(header, Elem.Security);
				if(securityElem != null)
				{
					XmlAttribute mustUndAtt = securityElem.Attributes[Attrib.mustUnderstand,Ns.soap];
					if(mustUndAtt != null)
						mustUndAtt.Value = "0";
					//securityElem.ParentNode.RemoveChild(securityElem);

					XmlElement encKeyElem = LameXpath.SelectSingleNode(securityElem, Elem.EncryptedKey);
					if(encKeyElem != null)
					{
						XmlElement encMethodElem = LameXpath.SelectSingleNode(encKeyElem, Elem.EncryptionMethod);
						if(encMethodElem != null)
						{
							encKeyMethod = encMethodElem.Attributes[Attrib.Algorithm].Value;
						}
						//ignore KeyInfo, use SecurityTokenReference instead

						XmlElement cipherValElem = LameXpath.SelectSingleNode(securityElem, Elem.CipherValue);
						if(cipherValElem != null)
						{
							baEncKey = OpenNETCF.Security.Cryptography.NativeMethods.Format.GetB64(cipherValElem.InnerText);
						}
					}

					XmlElement refListElem = LameXpath.SelectSingleNode(securityElem, Elem.ReferenceList);
					if(refListElem != null)
					{
						//ignore refList, just do straight to encData
					}
					XmlElement keyIdElem = LameXpath.SelectSingleNode(securityElem, Elem.KeyIdentifier);
					if(keyIdElem != null) //get keyId
					{
						string valueType = keyIdElem.Attributes[Attrib.ValueType].Value;
						//"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509SubjectKeyIdentifier
						if(valueType.EndsWith("#X509SubjectKeyIdentifier") == false && valueType != "wsse:X509v3")
							throw new Exception("only support X.509v3 certificates");
						encKeyId = keyIdElem.InnerText;
					}
					XmlElement refElem = LameXpath.SelectSingleNode(securityElem, Elem.Reference);
					if(refElem != null) //get keyUri
					{
						string refUri = refElem.Attributes[Attrib.URI].Value;
					}
					XmlElement userTokElem = LameXpath.SelectSingleNode(securityElem, Elem.UsernameToken);
					if(userTokElem != null)
					{
						nonce = LameXpath.SelectSingleNode(userTokElem, Elem.Nonce);
						created = LameXpath.SelectSingleNode(userTokElem, Elem.Created);
					}
				}
				//end header processing
			}

			byte [] baPlainKey = null;
			if(encKeyMethod != null) //decrypt key, assume RSA
			{
				baPlainKey = DecObj.RSACSP.DecryptValue(baEncKey);
				DecObj.SymmAlg.Key = baPlainKey;
			}
			//UsernameToken decryption
			if(DecObj.ClearPassword != null)
			{
				//use XmlSigHandler values, because will more than likely be signing
				int numKeyBytes = DecObj.SymmAlg.Key.Length;
				if(nonce==null || created==null)
					baPlainKey = P_SHA1.DeriveKey(DecObj.ClearPassword, XmlSigHandler.StrKeyLabel, DecObj.UserTok.Nonce.Text, DecObj.UserTok.Created, numKeyBytes);
				else
					baPlainKey = P_SHA1.DeriveKey(DecObj.ClearPassword, XmlSigHandler.StrKeyLabel, nonce.InnerText, created.InnerText, numKeyBytes);
				DecObj.SymmAlg.Key = baPlainKey;
			}

			//TODO EncryptedKey in body?, multiple EncryptedData in body

			string encBodMethod = null;
			string keyName = null;
			XmlElement cipherElem = LameXpath.SelectSingleNode(cipherDoc, Elem.EncryptedData);
			//if(cipherElem == null)
			//	return cipherDoc; //nothing to decrypt
			if(cipherElem != null)
			{
				XmlElement encMethodElemBod = LameXpath.SelectSingleNode(cipherElem, Elem.EncryptionMethod);
				if(encMethodElemBod != null)
				{
					encBodMethod = encMethodElemBod.Attributes[Attrib.Algorithm].Value;
					if(encBodMethod == Alg.aes128cbc)
					{
						if(DecObj.SymmAlg is TripleDES)
							throw new Exception("device expects TripleDES, not AES");
					}
					if(encBodMethod == Alg.tripledesCbc)
					{
						if((DecObj.SymmAlg is TripleDES) == false)
							throw new Exception("device expects AES, not TripleDES");
					}
				}
				XmlElement keyNameElem = LameXpath.SelectSingleNode(cipherElem, Elem.KeyName);
				if(keyNameElem != null)
					keyName = keyNameElem.InnerText;
			
				XmlElement cipherValueElem = LameXpath.SelectSingleNode(cipherElem, Elem.CipherValue);
				byte [] baCipherIv = OpenNETCF.Security.Cryptography.NativeMethods.Format.GetB64(cipherValueElem.InnerText);

				//should have encMethod, key, and cipherData now

				SymmetricAlgorithm sa = DecObj.SymmAlg;
				Array.Copy(baCipherIv, 0, sa.IV, 0, sa.IV.Length);
				byte [] baCipher = new byte [baCipherIv.Length - sa.IV.Length];
				Array.Copy(baCipherIv, sa.IV.Length, baCipher, 0, baCipher.Length);
				byte [] baClear = sa.DecryptValue(baCipher);

				/*
				PlainTextType ptType = PlainTextType.Content; //default
				if(cipherElem.Attributes["Type"] != null)
				{
					string strType = cipherElem.Attributes["Type"].Value;
					if(strType == "http://www.w3.org/2001/04/xmlenc#Element")
						ptType = PlainTextType.Element;
				}
				*/

				string strClear = OpenNETCF.Security.Cryptography.NativeMethods.Format.GetString(baClear); //for debugging
				cipherElem.ParentNode.InnerXml = strClear;
			}

			//MOD for SecureConversation
			//XmlElement rstrElem = LameXpath.SelectSingleNode(body, Elem.RequestSecurityToken); //temp for testing
			XmlElement rstrElem = LameXpath.SelectSingleNode(body, Elem.RequestSecurityTokenResponse);
			if(rstrElem != null)
			{
				SecConvObj = new SecConvObject();
				//<TokenType/>
				XmlElement ttElem = LameXpath.SelectSingleNode(rstrElem, Elem.TokenType);
				if(ttElem != null)
				{
					SecConvObj.tokenType = new TokenType();
					SecConvObj.tokenType.InnerText = ttElem.InnerText;
				}
				//ignore <AppliesTo/> for now

				//Entropy
				XmlElement entropyElem = LameXpath.SelectSingleNode(rstrElem, Elem.Entropy);
				if(entropyElem != null)
				{
					XmlElement encKeyElem = LameXpath.SelectSingleNode(entropyElem, Elem.EncryptedKey);
					if(encKeyElem != null)
					{
						XmlElement cipherValElem = LameXpath.SelectSingleNode(encKeyElem, Elem.CipherValue);
						if(cipherValElem != null)
						{
							baEncKey = OpenNETCF.Security.Cryptography.NativeMethods.Format.GetB64(cipherValElem.InnerText);
						}
						XmlElement encMethodElem = LameXpath.SelectSingleNode(encKeyElem, Elem.EncryptionMethod);
						if(encMethodElem != null)
						{
							encKeyMethod = encMethodElem.Attributes[Attrib.Algorithm].Value;
							if(encKeyMethod == Alg.kwTripledes)
							{
								throw new Exception("return Entropy with kw-TripleDes is not supported");
							}
							if(encKeyMethod == Alg.kwAes128)
							{
								XmlElement keyNameElem = LameXpath.SelectSingleNode(encKeyElem, Elem.KeyName);
								if(keyNameElem != null)
								{
									keyName = keyNameElem.InnerText;
								}
								if(DecObj.SymmAlg is TripleDES)
									throw new Exception("device expects TripleDES, not AES128");
								//the request entropy is encrypted with RSA
								//it passes a symmetric key
								//the response is encrypted with kw-aes128
								//the key for the kw-aes128 seems to be the symm key passed by RSA?
								SymmetricAlgorithm sa = DecObj.keyWrap;
								//key should have already been set
								byte [] unwrappedKey = sa.DecryptValue(baEncKey);
								SecConvObj.entropyKey = unwrappedKey;
							}
							if(encKeyMethod == Alg.rsa15)
							{
								//TODO - this scenario is not expected?
								XmlElement keyIdElem = LameXpath.SelectSingleNode(encKeyElem, Elem.KeyIdentifier);
								if(keyIdElem != null)
								{
									keyName = keyIdElem.InnerText;
								}
								baPlainKey = DecObj.RSACSP.DecryptValue(baEncKey);
								SecConvObj.secConvKey = baPlainKey;
								//went from 128 bytes to 16 decrypted - AES?
								//DecObj.SymmAlg.Key = baPlainKey;
							}
						}
						XmlElement carriedKeyNameElem = LameXpath.SelectSingleNode(encKeyElem, Elem.CarriedKeyName);
						if(carriedKeyNameElem != null)
						{
							keyName = carriedKeyNameElem.InnerText;
						}
					}
				}

				//RST
				XmlElement rstElem = LameXpath.SelectSingleNode(rstrElem, Elem.RequestedSecurityToken);
				if(rstElem != null)
					SecConvObj.requestedSecurityToken = rstElem;
				//RPT
				XmlElement rptElem = LameXpath.SelectSingleNode(rstrElem, Elem.RequestedProofToken);
				if(rptElem != null)
				{
					SecConvObj.requestedProofToken = rptElem;

					//figure out if key is computed
					//TODO use this later on
					bool computed = false;
					XmlElement compKeyElem = LameXpath.SelectSingleNode(rptElem, Elem.ComputedKey);
					if(compKeyElem != null)
						computed = true;
					if(computed == true)
					{
						//throw new Exception("not handling computed return keys yet");
						byte [] entropy1 = DecObj.keyWrap.Key;
						byte [] entropy2 = SecConvObj.entropyKey;
						byte [] concatEntropies = new byte[entropy1.Length + entropy2.Length];
						Array.Copy(entropy1, 0, concatEntropies, 0, entropy1.Length);
						Array.Copy(entropy2, 0, concatEntropies, entropy1.Length, entropy2.Length);
						SecConvObj.secConvKey = P_SHA1.DeriveKey(entropy1, entropy2, XmlSigHandler.NumKeyBytes);
					}

					XmlElement encMethodElemBod = LameXpath.SelectSingleNode(rptElem, Elem.EncryptionMethod);
					if(encMethodElemBod != null)
					{
						encBodMethod = encMethodElemBod.Attributes[Attrib.Algorithm].Value;
						if(encBodMethod == Alg.kwAes128)
						{
							//throw new Exception("only supports TripleDes, no AES on device");
							XmlElement cvElem = LameXpath.SelectSingleNode(rptElem, Elem.CipherValue);
							//byte [] baPKey = null;
							if(cvElem != null)
							{
								byte [] baCipher = OpenNETCF.Security.Cryptography.NativeMethods.Format.GetB64(cvElem.InnerText);

								int numKeyBytes = DecObj.SymmAlg.Key.Length;
								string tempLabel = XmlSigHandler.StrKeyLabel; //WS-Security
								
								baPlainKey = P_SHA1.DeriveKey(DecObj.ClearPassword, tempLabel, nonce.InnerText, created.InnerText, numKeyBytes);

								//TODO make TripleDES below work like this too - common codebase
								SymmetricAlgorithm sa = DecObj.keyWrap;
								sa.Key = baPlainKey;
								byte [] unwrappedKey = sa.DecryptValue(baCipher);

								SecConvObj.secConvKey = unwrappedKey;
							}
						}
						else if(encBodMethod == Alg.kwTripledes)
						{
							XmlElement cvElem = LameXpath.SelectSingleNode(rptElem, Elem.CipherValue);
							//byte [] baPKey = null;
							if(cvElem != null)
							{
								byte [] baCipher = OpenNETCF.Security.Cryptography.NativeMethods.Format.GetB64(cvElem.InnerText);

								int numKeyBytes = DecObj.SymmAlg.Key.Length;
								string tempLabel = XmlSigHandler.StrKeyLabel; //WS-Security
								//string tempLabel = "WS-SecureConversation";
								
								baPlainKey = P_SHA1.DeriveKey(DecObj.ClearPassword, tempLabel, nonce.InnerText, created.InnerText, numKeyBytes);

								//TODO make this work with KeyWrap interface
								//SymmetricAlgorithm sa = DecObj.SymmAlg;
								TripleDESCryptoServiceProvider sa = (TripleDESCryptoServiceProvider) DecObj.SymmAlg;
								sa.Key = baPlainKey;
								TripleDesKeyWrap tdkw = new TripleDesKeyWrap(sa);
								byte [] unwrappedKey = tdkw.DecryptValue(baCipher);

								SecConvObj.secConvKey = unwrappedKey;
							}
						}
						else //http://www.w3.org/2001/04/xmlenc#rsa-1_5
						{
							XmlElement cvElem = LameXpath.SelectSingleNode(rptElem, Elem.CipherValue);
							byte [] baPKey = null;
							if(cvElem != null)
							{
								byte [] baEKey = OpenNETCF.Security.Cryptography.NativeMethods.Format.GetB64(cvElem.InnerText);
								baPKey = DecObj.RSACSP.DecryptValue(baEKey);
								SecConvObj.secConvKey = baPKey;
							}
						}
					}
					//else
					//{
					//	throw new Exception("EncryptionMethod not specified");
					//}
				}
				//ignore <LifeTime/> for now
			}

			DecObj = null;
			return cipherDoc;
		}

		/*
		private static bool methodCalled = false;
		private static ArrayList encBag;
		public static void AddEncObject(object encObject)
		{
			if(encBag == null)
				encBag = new ArrayList();
			if(methodCalled == true)
			{
				encBag = new ArrayList();
				methodCalled = false;
			}
			encBag.Add(encObject);
		}

		public static ArrayList EncBag
		{
			get
			{
				if(encBag == null)
					encBag = new ArrayList();
				return encBag;
			}
		}

		public static void ClearEncBag()
		{
			if(encBag != null)
				encBag.Clear();
		}
		*/
	}
}
