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
	//previous to this override it was done with sessionless cookies in the URL
	//or using the HttpWebRequest directly and building your own soap messages		
	//ultimately it should be done in a SoapHeader, as demonstated in KeithBa's WS book
	public class SessionCookiesWrap : SoapHttpClientProtocol
	{
		public SessionCookiesWrap() : base()
		{}

		//this will work on RTM
		//http://www.alexfeinman.com/download.asp?doc=SessionAwareWebSvc.zip

		/*
		private static string sessCookie = null; //TODO expired sessions
		
		//only with SP1 bits
		protected override System.Net.WebRequest GetWebRequest(Uri uri)
		{
			System.Net.HttpWebRequest hwr = (System.Net.HttpWebRequest) base.GetWebRequest (uri);
			if(sessCookie != null)
			{
				hwr.Headers.Add("Cookie", sessCookie);
			}
			return hwr;
		}
	
		protected override System.Net.WebResponse GetWebResponse(System.Net.WebRequest request)
		{
			System.Net.HttpWebResponse hwr = (System.Net.HttpWebResponse) base.GetWebResponse (request);
			if(hwr.Headers["Set-Cookie"] != null)
			{
				sessCookie = hwr.Headers["Set-Cookie"];
				//"ASP.NET_SessionId=g3exyaihxpmbkr55rhfxwq45; path=/"
			}
			return hwr;
		}
		*/
	}
}
