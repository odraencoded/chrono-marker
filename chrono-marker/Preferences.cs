//
//  Preferences.cs
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
using System.IO;

namespace Chrono
{
	/// <summary>
	/// Stores all of the program preferences.
	/// </summary>
	public sealed class Preferences
	{
		private Preferences() { }

		public bool CreateWatchOnStartup;
		public String StartupWatchName;

		public bool WatchDockedByDefault;
		public bool WatchCompactByDefault;

		public TimeFormatSettings TimeDisplaySettings { get; private set; }

		public void SaveTo(string filename)
		{
			ConfigFile configFile = new ConfigFile();

			ConfigSection rootSection = configFile.RootSection;
			
			rootSection.SetValue("Compact by Default", WatchCompactByDefault);
			rootSection.SetValue("Docked by Default", WatchDockedByDefault);

			rootSection.SetValue("Create Stopwatch on Startup", CreateWatchOnStartup);
			rootSection.SetValue("Startup Stopwatch Name", StartupWatchName);

			ConfigSection formattingSection = configFile["Formatting"];

			formattingSection.SetValue("Show Hours", TimeDisplaySettings.ShowHours);
			formattingSection.SetValue("Show Minutes", TimeDisplaySettings.ShowMinutes);
			formattingSection.SetValue("Show Seconds", TimeDisplaySettings.ShowSeconds);
			formattingSection.SetValue("Show Milliseconds", TimeDisplaySettings.ShowMilliseconds);
			formattingSection.SetValue("Show Plus Symbol", TimeDisplaySettings.ShowPlusSymbol);
			formattingSection.SetValue("Show Minus Symbol", TimeDisplaySettings.ShowMinusSymbol);
			formattingSection.SetValue("Show Leading Zeroes", TimeDisplaySettings.ShowLeadingZeroes);
			formattingSection.SetValue("Show Separators", TimeDisplaySettings.ShowSeparators);

			configFile.Save(filename);
		}

		public static Preferences Load(string filename)
		{
			// Due to how these classes are structured
			// Any kind of "normal" failure will result in default settings
			Preferences result = new Preferences();

			ConfigFile configFile = new ConfigFile();

			if(File.Exists(filename))
				configFile.Load( filename );

			ConfigSection rootSection = configFile.RootSection;

			result.WatchCompactByDefault = rootSection.GetBoolean("Compact by Default", false);
			result.WatchDockedByDefault = rootSection.GetBoolean("Docked by Default", true);

			result.CreateWatchOnStartup = rootSection.GetBoolean("Create Stopwatch on Startup", true);
			result.StartupWatchName = rootSection.GetString("Startup Stopwatch Name", "Chrono Marker");

			result.TimeDisplaySettings = new TimeFormatSettings();

			ConfigSection clockDefaultsSection = configFile["Formatting"];

			result.TimeDisplaySettings.ShowHours = clockDefaultsSection.GetBoolean("Show Hours", true);
			result.TimeDisplaySettings.ShowMinutes = clockDefaultsSection.GetBoolean("Show Minutes", true);
			result.TimeDisplaySettings.ShowSeconds = clockDefaultsSection.GetBoolean("Show Seconds", true);
			result.TimeDisplaySettings.ShowMilliseconds = clockDefaultsSection.GetBoolean("Show Milliseconds", true);
			result.TimeDisplaySettings.ShowPlusSymbol = clockDefaultsSection.GetBoolean("Show Plus Symbol", false);
			result.TimeDisplaySettings.ShowMinusSymbol = clockDefaultsSection.GetBoolean("Show Minus Symbol", true);
			result.TimeDisplaySettings.ShowLeadingZeroes = clockDefaultsSection.GetBoolean("Show Leading Zeroes", true);
			result.TimeDisplaySettings.ShowSeparators = clockDefaultsSection.GetBoolean("Show Separators", true);

			return result;
		}
	}
}

