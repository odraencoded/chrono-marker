//
//  ConfigSection.cs
//
//  Author:
//       Leonardo Augusto Pereira <http://code.google.com/p/chrono-marker/>
//
//  Copyright (c) 2012 Leonardo Augusto Pereira
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;

namespace Chrono
{
	public class ConfigSection
	{
		public ConfigSection()
		{
			keys = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
		}

		public void SetRawString(string key, string value)
		{
			keys[key] = value.ToString();
		}

		public void SetValue(string key, object value)
		{
			keys[key] = value.ToString();
		}

		public void SetValue(string key, string value)
		{
			keys[key] = "\"" + value + "\"";
		}

		public bool GetBoolean(string key)
		{
			string value = keys[key];

			return bool.Parse(value);
		}

		public bool GetBoolean(string key, bool @default)
		{
			string value;
			if(!keys.TryGetValue(key, out value)) return @default;

			bool parsedValue;
			if(bool.TryParse(value, out parsedValue)) return parsedValue;
			else return @default;
		}

		public string GetString(string key)
		{
			string value = keys[key];

			if( value.StartsWith( "\"" ) && value.EndsWith( "\"" ) ) {
				return value.Substring(1, value.Length - 2);
			}
			else throw new FormatException();
		}

		public string GetString(string key, string @default)
		{
			string value;
			if( !keys.TryGetValue( key, out value ) )
				return @default;

			if( value.StartsWith( "\"" ) && value.EndsWith( "\"" ) ) {
				return value.Substring(1, value.Length - 2);
			}
			else return @default;
		}

		public List<string> KeyValueAsLines()
		{
			List<string> result = new List<string>(keys.Count);

			foreach( KeyValuePair<string, string> keyValue in keys ) {
				string line = keyValue.Key + " = " + keyValue.Value;

				result.Add(line);
			}

			return result;
		}

		private Dictionary<string, string> keys;
	}
}

