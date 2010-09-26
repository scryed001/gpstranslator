/*
$Id: IExtension.cs,v 1.2 2006/05/25 10:14:36 andrew_klopper Exp $

Copyright 2005-2006 Andrew Rowland Klopper (http://gpsproxy.sourceforge.net/)

This file is part of GPSProxy.

GPSProxy is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

GPSProxy is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with GPSProxy; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System;

namespace GPSProxy.Extension
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
	public sealed class GPSProxyExtensionAttribute : Attribute
	{
		private string m_ExtensionName;
		private string m_ExtensionDescription;
		private string m_ExtensionVersion;
		private string m_MinimumGPSProxyVersion;
		private string m_MaximumGPSProxyVersion;
		private bool m_HasUserInterface;
		private bool m_RequiresConfiguration;
		private bool m_IsProvider;
		private bool m_AllowMultipleInstances;
		private int m_Precedence;

		public string ExtensionName
		{
			get { return m_ExtensionName; }
		}

		public string ExtensionDescription
		{
			get { return m_ExtensionDescription; }
		}

		public string ExtensionVersion
		{
			get { return m_ExtensionVersion; }
		}

		public string MinimumGPSProxyVersion
		{
			get { return m_MinimumGPSProxyVersion; }
		}

		public string MaximumGPSProxyVersion
		{
			get { return m_MaximumGPSProxyVersion; }
		}

		public bool HasUserInterface
		{
			get { return m_HasUserInterface; }
		}

		public bool RequiresConfiguration
		{
			get { return m_RequiresConfiguration; }
		}

		public bool IsProvider
		{
			get { return m_IsProvider; }
		}

		public int Precedence
		{
			get { return m_Precedence; }
		}

		public bool AllowMultipleInstances
		{
			get { return m_AllowMultipleInstances; }
		}

		public GPSProxyExtensionAttribute(string extensionName, string extensionDescription,
			string extensionVersion, string minimumGPSProxyVersion, string maximumGPSProxyVersion,
			bool hasUserInterface, bool requiresConfiguration, int precedence,
			bool allowMultipleInstances, bool isProvider)
		{
			m_ExtensionName = extensionName;
			m_ExtensionDescription = extensionDescription;
			m_ExtensionVersion = extensionVersion;
			m_MinimumGPSProxyVersion = minimumGPSProxyVersion;
			m_MaximumGPSProxyVersion = maximumGPSProxyVersion;
			m_HasUserInterface = hasUserInterface;
			m_RequiresConfiguration = requiresConfiguration;
			m_Precedence = precedence;
			m_IsProvider = isProvider;
			m_AllowMultipleInstances = allowMultipleInstances;
		}
	}

	public delegate void GPSFixEvent(IExtension sender, IGPSFix fix);
	public delegate void GPSSatelliteDataEvent(IExtension sender, IGPSSatelliteVehicle[] vehicles);

	/// <summary>
	/// Summary description for IExtension.
	/// </summary>
	public interface IExtension
	{
		void ExtensionInit(IApplication application, int extensionID, IConfig config);
		void ExtensionDispose();

		System.Windows.Forms.Control GetUserInterface();
		void ResizeUserInterface(System.Drawing.Size size);

		void ShowConfigurationDialog();

		void Start();
		void Stop();
		void Wakeup();

		// Providers
		event GPSFixEvent NewGPSFix;
		event GPSSatelliteDataEvent NewGPSSatelliteData;

		// Consumers
		void ProcessGPSFix(IGPSFix fix);
		void ProcessGPSSatelliteData(IGPSSatelliteVehicle[] satelliteVehicles);
	}
}
