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
using System.Collections;

namespace OpenNETCF.Web.Services2
{
	public abstract class SoapOutputFilter
	{
		protected SoapOutputFilter(){}
		//protected void AddEnvelopeNamespaceDeclaration(SoapEnvelope envelope, string prefix, string namespaceUri);
		public abstract void ProcessMessage(SoapEnvelope envelope);
	} 

	public abstract class SoapInputFilter
	{
		protected SoapInputFilter(){}
		//protected virtual bool CanProcessHeader(XmlElement header, SoapContext context);
		public abstract void ProcessMessage(SoapEnvelope envelope); 
	}

	public class SoapEnvelope : XmlDocument
	{
		public SoapEnvelope()
		{
		}

		//private XmlDocument xd;
		//public SoapEnvelope(XmlDocument xmlDoc)
		//{
		//	xd = xmlDoc;
		//}

		private Hashtable ht = new Hashtable();
		public Hashtable Context
		{
			get
			{
				return ht;
			}
			set
			{
				ht = value;
			}
		}

		public XmlElement Body 
		{ 
			get
			{
				//XmlElement envelope = xd.DocumentElement;
				XmlElement envelope = this.DocumentElement;

				XmlElement headerOrBody = (XmlElement) envelope.ChildNodes[0];
				XmlElement body = null;
				if(headerOrBody.LocalName == Elem.Header)
					body = (XmlElement) envelope.ChildNodes[1];
				if(headerOrBody.LocalName == Elem.Body)
					body = headerOrBody;
				return body;
			}
		}

		public XmlElement Header 
		{ 
			get
			{
				//XmlElement envelope = xd.DocumentElement;
				XmlElement envelope = this.DocumentElement;
				XmlElement headerOrBody = (XmlElement) envelope.ChildNodes[0];
				if(headerOrBody.LocalName == Elem.Body)
					return headerOrBody;
				else
					return null;				
			}
		}
		
		public XmlElement CreateHeader()
		{
			//XmlElement envelope = xd.DocumentElement;
			XmlElement envelope = this.DocumentElement;
			XmlElement headerOrBody = (XmlElement) envelope.ChildNodes[0];
			XmlElement header;
			XmlElement body;
			if(headerOrBody.LocalName == Elem.Body)
			{
				//header = xd.CreateElement(headerOrBody.Prefix, Elem.Header, headerOrBody.NamespaceURI);
				header = this.CreateElement(headerOrBody.Prefix, Elem.Header, headerOrBody.NamespaceURI);
				envelope.InsertBefore(header, headerOrBody);
			}
			header = (XmlElement) envelope.ChildNodes[0];
			body = (XmlElement) envelope.ChildNodes[1];
			return header;
		}


	}
}
