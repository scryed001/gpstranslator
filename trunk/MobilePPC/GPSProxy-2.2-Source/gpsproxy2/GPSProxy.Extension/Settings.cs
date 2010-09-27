/*
$Id: Settings.cs,v 1.1 2006/05/23 09:27:07 andrew_klopper Exp $

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
	// Currently understood types and associated parameters are:
	//
	//   - boolean
	//   - string (maxlength)
	//   - integer (maxlength)
	//   - long (maxlength)
	//   - double (maxlength)
	//   - updown (maxlength, min, max)
	//   - comportin
	//	 - comportout
	//   - combobox (items)
	//   - openfile (filter, filterindex, initialdir)
	//   - savefile (filter, filterindex, initialdir)
	//   - exefile
	//
	// Parameters must be passed in the Params array in the form {name, value, name, value}. In other words,
	// each parameter results in 2 entries in the Params array.

	public class Setting
	{
		public string Name;
		public string Caption;
		public string Group;
		public string Type;
		public object[] Params;
		public object Value;

		public Setting(string Name, string Caption, string Group, string Type, object Value, params object[] Params)
		{
			this.Name = Name;
			this.Caption = Caption;
			this.Group = Group;
			this.Type = Type;
			this.Value = Value;
			this.Params = Params;
		}
	}

	public class Settings
	{
		private Setting[] settings;

		public Setting this[string index]
		{
			get
			{
				int i;
				for(i = 0; i < settings.Length; i++) 
				{
					if (settings[i].Name == index) 
					{
						return settings[i];
					}
				}
				throw new Exception("Invalid setting name: " + index);
			}
		}

		public Setting this[int index]
		{
			get
			{
				return settings[index];
			}
		}

		public int Count
		{
			get
			{
				return settings.Length;
			}
		}

		public Settings(Setting[] settings)
		{
			this.settings = settings;
		}
	}

	public delegate bool SettingsValidator(Settings settings, ref string errorMessage);
}
