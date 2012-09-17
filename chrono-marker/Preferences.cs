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

		private const string Version = "c-m-pref-0";

		public StartupSettings Startup;
		public TimeFormatSettings TimeDisplaySettings { get; private set; }
		public bool ClockCompactByDefault;

		public void SaveTo(string filename)
		{
			ConfigFile configFile = new ConfigFile();

			ConfigSection startupSection = configFile["Startup"];

			startupSection.SetValue("Create Clock", Startup.CreateClock);
			startupSection.SetValue("First Clock Name", Startup.FirstClockName);
			startupSection.SetValue("Start Docked", Startup.StartDocked);

			ConfigSection clockDefaultsSection = configFile["Clock Defaults"];

			clockDefaultsSection.SetValue("Show Hours", TimeDisplaySettings.ShowHours);
			clockDefaultsSection.SetValue("Show Minutes", TimeDisplaySettings.ShowMinutes);
			clockDefaultsSection.SetValue("Show Seconds", TimeDisplaySettings.ShowSeconds);
			clockDefaultsSection.SetValue("Show Milliseconds", TimeDisplaySettings.ShowMilliseconds);
			clockDefaultsSection.SetValue("Show Plus Symbol", TimeDisplaySettings.ShowPlusSymbol);
			clockDefaultsSection.SetValue("Show Minus Symbol", TimeDisplaySettings.ShowMinusSymbol);
			clockDefaultsSection.SetValue("Show Leading Zero", TimeDisplaySettings.ShowLeadingZero);
			clockDefaultsSection.SetValue("Show Separators", TimeDisplaySettings.ShowSeparators);
			clockDefaultsSection.SetValue("Starts Compact", ClockCompactByDefault);

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

			ConfigSection startupSection = configFile["Startup"];

			result.Startup.CreateClock = startupSection.GetBoolean("Create Clock", true);
			result.Startup.FirstClockName = startupSection.GetString("First Clock Name", "Sundial");
			result.Startup.StartDocked = startupSection.GetBoolean("Start Docked", true);

			result.TimeDisplaySettings = new TimeFormatSettings(TimeFormatFlags.Nothing);

			ConfigSection clockDefaultsSection = configFile["Clock Defaults"];

			result.TimeDisplaySettings.ShowHours = clockDefaultsSection.GetBoolean("Show Hours", true);
			result.TimeDisplaySettings.ShowMinutes = clockDefaultsSection.GetBoolean("Show Minutes", true);
			result.TimeDisplaySettings.ShowSeconds = clockDefaultsSection.GetBoolean("Show Seconds", true);
			result.TimeDisplaySettings.ShowMilliseconds = clockDefaultsSection.GetBoolean("Show Milliseconds", true);
			result.TimeDisplaySettings.ShowPlusSymbol = clockDefaultsSection.GetBoolean("Show Plus Symbol", false);
			result.TimeDisplaySettings.ShowMinusSymbol = clockDefaultsSection.GetBoolean("Show Minus Symbol", true);
			result.TimeDisplaySettings.ShowLeadingZero = clockDefaultsSection.GetBoolean("Show Leading Zero", true);
			result.TimeDisplaySettings.ShowSeparators = clockDefaultsSection.GetBoolean("Show Separators", true);
			result.ClockCompactByDefault = clockDefaultsSection.GetBoolean("Starts Compact", false);

			return result;
		}
	}
}
