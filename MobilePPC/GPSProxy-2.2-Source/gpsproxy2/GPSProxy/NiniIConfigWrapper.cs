/*
$Id: NiniIConfigWrapper.cs,v 1.1 2006/05/23 09:27:05 andrew_klopper Exp $

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

namespace GPSProxy
{
	/// <summary>
	/// Summary description for NiniIConfigWrapper.
	/// </summary>
	public class NiniIConfigWrapper : GPSProxy.Extension.IConfig
	{
		private Nini.Config.IConfig config;

		public NiniIConfigWrapper(Nini.Config.IConfig config)
		{
			this.config = config;
		}

		#region IConfig Members

		public long GetLong(string key, long defaultValue)
		{
			return config.GetLong(key, defaultValue);
		}

		public long GetLong(string key)
		{
			return config.GetLong(key);
		}

		public string[] GetValues()
		{
			return config.GetValues();
		}

		public void Remove(string key)
		{
			config.Remove(key);
		}

		public bool Contains(string key)
		{
			return config.Contains(key);
		}

		public double GetDouble(string key, double defaultValue)
		{
			return config.GetDouble(key, defaultValue);
		}

		public double GetDouble(string key)
		{
			return config.GetDouble(key);
		}

		public bool GetBoolean(string key, bool defaultValue)
		{
			return config.GetBoolean(key, defaultValue);
		}

		public bool GetBoolean(string key)
		{
			return config.GetBoolean(key);
		}

		public float GetFloat(string key, float defaultValue)
		{
			return config.GetFloat(key, defaultValue);
		}

		public float GetFloat(string key)
		{
			return config.GetFloat(key);
		}

		public string Get(string key, string defaultValue)
		{
			return config.Get(key, defaultValue);
		}

		public string Get(string key)
		{
			return config.Get(key);
		}

		public string[] GetKeys()
		{
			return config.GetKeys();
		}

		public void Set(string key, object value)
		{
			config.Set(key, value);
		}

		public int GetInt(string key, int defaultValue)
		{
			return config.GetInt(key, defaultValue);
		}

		public int GetInt(string key)
		{
			return config.GetInt(key);
		}

		public string GetString(string key, string defaultValue)
		{
			return config.GetString(key, defaultValue);
		}

		public string GetString(string key)
		{
			return config.GetString(key);
		}

		#endregion
	}
}
