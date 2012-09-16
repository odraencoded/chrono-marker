//
//  ConfigFile.cs
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
using System.IO;

namespace Chrono
{
	public class ConfigFile
	{
		public ConfigFile()
		{
			sections = new Dictionary<string, ConfigSection>(
				StringComparer.InvariantCultureIgnoreCase);

			RootSection = new ConfigSection();
		}

		public ConfigSection this [string name] {
			get
			{
				ConfigSection result;
				if(!sections.TryGetValue(name, out result))
				{
					sections[name] = result = new ConfigSection();
				}
				return result;
			}
		}

		public ConfigSection RootSection { get; private set; }

		public void Load(string filename)
		{
			using(StreamReader reader = new StreamReader(filename))
			{
				ConfigSection currentSection = RootSection;

				while(!reader.EndOfStream)
				{
					string line = reader.ReadLine().Trim();

					if(line == string.Empty) continue;

					if(line.StartsWith("[") && line.EndsWith("]"))
					{
						string sectionName = line.Substring(1, line.Length - 2);

						currentSection = this[sectionName];
					}
					else
					{
						int equalsIndex = line.IndexOf('=');

						if(equalsIndex == -1) continue;


						string key = line.Substring(0, equalsIndex).Trim();
						string value = line.Substring(equalsIndex + 1).Trim();

						currentSection.SetRawString(key, value);
					}
				}
			}
		}

		public void Save(string filename)
		{
			using(StreamWriter writer = new StreamWriter(filename))
			{

				List<string> lines;

				lines = RootSection.KeyValueAsLines();
				foreach(string line in lines) writer.WriteLine(line);

				foreach(KeyValuePair<string, ConfigSection> sectionKeyValue in sections)
				{
					writer.WriteLine("[" + sectionKeyValue.Key + "]");

					lines = sectionKeyValue.Value.KeyValueAsLines();
					foreach(string line in lines) writer.WriteLine(line);
				}
			}
		}

		Dictionary<string, ConfigSection> sections;
	}
}

