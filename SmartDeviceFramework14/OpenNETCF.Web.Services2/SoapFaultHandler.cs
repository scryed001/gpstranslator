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

namespace OpenNETCF.Web.Services2
{
	public class SoapFaultHandler
	{
		public SoapFaultHandler() {}

		/*
		<env:Body>
			<env:Fault>
				<env:Code>
				<env:Value>env:Sender</env:Value>
				<env:Subcode>
					<env:Value>rpc:BadArguments</env:Value>
				</env:Subcode>
				</env:Code>
				<env:Reason>
				<env:Text xml:lang="en-US">Processing error</env:Text>
				<env:Text xml:lang="cs">Chyba zpracování</env:Text>
				</env:Reason>
				<env:Detail>
				<e:myFaultDetails 
					xmlns:e="http://travelcompany.example.org/faults">
					<e:message>Name does not match card number</e:message>
					<e:errorcode>999</e:errorcode>
				</e:myFaultDetails>
				</env:Detail>
			</env:Fault>
		</env:Body> 
		*/
		public static void CheckXml(XmlDocument plainDoc)
		{
			XmlElement envelope = plainDoc.DocumentElement;

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
			
		}
	}
}
