//==========================================================================================
//
//		OpenNETCF.WindowsCE.Forms.ConfigurationManager
//		Copyright (c) 2005, OpenNETCF.org
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
//==========================================================================================
using System;
using System.Runtime.InteropServices;
using OpenNETCF.Runtime.InteropServices;
using System.Xml;

namespace OpenNETCF.WindowsCE.Forms
{
	/// <summary>
	/// Provides access to the Configuration Manager functionality of Smartphone and Pocket PC Phone Edition devices.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	public class ConfigurationManager
	{
		private ConfigurationManager(){}

		[DllImport("aygshell.dll", EntryPoint="DMProcessConfigXML", SetLastError=true)]
		private static extern int DMProcessConfigXML(string pszWXMLin, ConfigXmlFlags dwFlags, ref IntPtr ppszwXMLout);

		[DllImport("coredll.dll", SetLastError=true)]
		private static extern void free(IntPtr ptr); 
		/// <summary>
		/// Processes the specified Xml and returns a string containing the Xml response.
		/// </summary>
		/// <param name="xmlIn">Xml containing configuration settings.</param>
		/// <param name="flags">Specifies a combination of <see cref="ConfigXmlFlags"/>.</param>
		/// <returns></returns>
		/// <remarks>The Configuration Manager component is transactioned and must process only one configuration request at a time to prevent collisions.</remarks>
		public static string ProcessConfigXml(string xmlIn, ConfigXmlFlags flags)
		{
			IntPtr pOut = IntPtr.Zero;

			int hresult = DMProcessConfigXML(xmlIn, flags, ref pOut);

			if(hresult == 0)
			{
				string s = Marshal.PtrToStringUni(pOut);
				free(pOut);

				return s;
			}
			else
			{
				throw new ExternalException("Error " + hresult.ToString("X"));
			}
		}

		/// <summary>
		/// Processes the specified Xml document and returns the response as a new <see cref="XmlDocument"/>.
		/// </summary>
		/// <param name="xmlIn">Xml Document containing configuration settings.</param>
		/// <param name="flags">Specifies a combination of <see cref="ConfigXmlFlags"/>.</param>
		/// <returns></returns>
		public static XmlDocument ProcessConfigXml(XmlDocument xmlIn, ConfigXmlFlags flags)
		{
			string s = ProcessConfigXml(xmlIn.OuterXml, flags);

			//load string into an XmlDocument object.
			XmlDocument dOut = new XmlDocument();
			dOut.LoadXml(s);
			return dOut;
		}
	}

	/// <summary>
	/// Flags used with <see cref="ConfigurationManager.ProcessConfigXml"/> method.
	/// </summary>
	[Flags()]
	public enum ConfigXmlFlags
	{
		/// <summary>
		/// The configuration management service and the Configuration Service Providers (CSPs) test the input data but do not actually commit changes.
		/// </summary>
		Test = 0,
		/// <summary>
		/// The configuration management service and the Configuration Service Providers (CSPs) process the input data. 
		/// </summary>
		Process = 0x0001, 
		/// <summary>
		/// The configuration management service gathers and returns metadata for any XML parm elements it encounters. 
		/// </summary>
		MetaData = 0x0002, 
	}
}
