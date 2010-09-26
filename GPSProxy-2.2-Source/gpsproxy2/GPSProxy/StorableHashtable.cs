/*
$Id: StorableHashtable.cs,v 1.1 2006/05/23 09:27:05 andrew_klopper Exp $

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
using System.Collections;
using System.Text.RegularExpressions;

namespace GPSProxy
{
	/// <summary>
	/// Summary description for StorableHashtable.
	/// </summary>
	public class StorableHashtable: Hashtable
	{
		public StorableHashtable(): base()
		{
		}

		public StorableHashtable(string storedValue): base()
		{
			if (storedValue != "") 
			{
				Regex itemDelimiterRegex = new Regex(@"\|");
				Regex equalsRegex = new Regex(@"=");
				string[] items = itemDelimiterRegex.Split(storedValue);
				for (int i = 0; i < items.Length; i++) 
				{
					string[] parts = equalsRegex.Split(items[i], 2);
					if (parts.Length != 2)
						throw new Exception("Invalid hash table item: " + items[i]);
					Add(parts[0], parts[1]);
				}
			}
		}

		public override string ToString() 
		{
			string ret = "";
			foreach (DictionaryEntry entry in this) 
			{
				if (ret.Length > 0)
					ret += "|";
				ret += entry.Key.ToString() + "=" + entry.Value.ToString();
			}
			return ret;
		}
	}
}
