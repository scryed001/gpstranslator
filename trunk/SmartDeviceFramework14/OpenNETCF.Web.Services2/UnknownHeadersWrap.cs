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
using System.Web.Services.Protocols;

namespace OpenNETCF.Web.Services2
{
	public class UnknownHeaderWrap : SoapHttpClientProtocol
	{
		public UnknownHeaderWrap() : base()
		{}

		//declare member and attribute in auto gen'd proxy 
		//public SoapUnknownHeader [] unknownHeaders; 
		//[SoapHeaderAttribute("unknownHeaders",Direction=SoapHeaderDirection.Out)] 
  
		/*
		//and in client code with 'webRef' being the proxy 
		SoapUnknownHeader [] suha = webRef.unknownHeaders; 
		foreach(SoapUnknownHeader suh in suha) 
		{ 
			MessageBox.Show(suh.Element.Name); 
		} 
		*/
	}
}
