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
using System.Web.Services;
using System.Web.Services.Protocols;
using System.IO;
using System.Xml;
using System.Collections;

namespace OpenNETCF.Web.Services2
{
	public class XmlSigExtension : SoapExtension 
	{
		Stream oldStream;
		Stream newStream;

		public override Stream ChainStream( Stream stream )
		{
			oldStream = stream;
			newStream = new MemoryStream();
			return newStream;
		}

		public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute) 
		{
			return null;
		}

		public override object GetInitializer(Type WebServiceType) 
		{
			return null;
		}

		public override void Initialize(object initializer) 
		{

		}

		public override void ProcessMessage(SoapMessage message) 
		{
			switch (message.Stage) 
			{
				case SoapMessageStage.BeforeSerialize:
					BeforeSerialize(message);
					break;
				case SoapMessageStage.AfterSerialize:
					AfterSerialize(message);
					break;
				case SoapMessageStage.BeforeDeserialize:
					BeforeDeserialize(message);
					break;
				case SoapMessageStage.AfterDeserialize:
					AfterDeserialize(message);
					break;
				default:
					throw new Exception("invalid SoapExtension.ProcessMessage stage");
			}
		}

		public void BeforeSerialize(SoapMessage message) //ObjectOut
		{
			//NOTE this only works if you handle StreamOut/In too
			//NOTE this only works on .NETfx
			return;
		}

		public void AfterSerialize(SoapMessage message) //StreamOut
		{
			newStream.Position = 0;
			//Hack MemoryStream below because of XmlDocument bug closing stream
			byte [] tempBa = new byte[newStream.Length];
			int bytesRead = newStream.Read(tempBa, 0, (int) newStream.Length);
			MemoryStream ms = new MemoryStream(tempBa);
			//BUG NOTE this closes the underlying stream on .NETcf
			XmlDocument xd = new XmlDocument();
			xd.Load(ms); //MemoryStream will be closed now
			
			//outgoing
			//timestamp and routing
			HeadersHandler.AddHeaders(xd, message.Action, message.Url); 
			//XmlEncHandler.EncryptXml(xd);
			XmlSigHandler.SignXml(xd); //or before XmlEnc.
			//Trace
			
			newStream.Position = 0;
			XmlTextWriter xtw = new XmlTextWriter(newStream, System.Text.Encoding.UTF8);
			xtw.Namespaces = true;
			xd.WriteTo(xtw);
			xtw.Flush();
			//xtw.Close();

			//these 2 lines have be called at a minimum
			newStream.Position = 0;
			Copy(newStream, oldStream); //for next guy
			return;
		}

		/*
		ChainStream
		BeforeSerial
		AfterSerial
		ChainStream
		BeforeDeserial
		AfterDeserial
		*/

		public void BeforeDeserialize(SoapMessage message) //StreamIn
		{
			Copy(oldStream, newStream);
			newStream.Position = 0;
			
			//Hack MemoryStream below because of XmlDocument bug closing stream
			byte [] tempBa = new byte[newStream.Length];
			int bytesRead = newStream.Read(tempBa, 0, (int) newStream.Length);
			MemoryStream ms = new MemoryStream(tempBa);
			//BUG NOTE this closes the underlying stream on .NETcf
			XmlDocument xd = new XmlDocument();
			xd.Load(ms); //MemoryStream will be closed now
			
			//incoming
			//Trace
			XmlSigHandler.VerifySig(xd); //or after XmlEnc?
			//XmlEncHandler.DecryptXml(xd);
			HeadersHandler.CheckHeaders(xd); 
			//timestamp and routing

			//newStream is too big now
			newStream.SetLength(0); //doing a new MemoryStream here was bad
			XmlTextWriter xtw = new XmlTextWriter(newStream, System.Text.Encoding.UTF8);
			xtw.Namespaces = true;
			xd.WriteTo(xtw);
			xtw.Flush();
			//xtw.Close();
			newStream.Position = 0;
			return;
		}

		public void AfterDeserialize(SoapMessage message) //ObjectIn
		{
			SoapMessage mess = message;
			return;
		}

		void Copy(Stream from, Stream to) 
		{
			TextReader reader = new StreamReader(from);
			TextWriter writer = new StreamWriter(to);
			//string strRead = reader.ReadToEnd();
			//writer.WriteLine(strRead);
			writer.WriteLine(reader.ReadToEnd());
			writer.Flush();
		}
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class XmlSigExtensionAttribute : SoapExtensionAttribute 
	{
		public override Type ExtensionType 
		{
			get { return typeof(XmlSigExtension); }
		}

		private int priority;
		public override int Priority 
		{
			get { return priority; }
			set { priority = value; }
		}
	}
}