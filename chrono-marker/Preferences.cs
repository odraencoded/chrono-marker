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

			ConfigSection timeFormatSection = configFile["Time Format"];

			timeFormatSection.SetValue("Show Hours", TimeDisplaySettings.ShowHours);
			timeFormatSection.SetValue("Show Minutes", TimeDisplaySettings.ShowMinutes);
			timeFormatSection.SetValue("Show Seconds", TimeDisplaySettings.ShowSeconds);
			timeFormatSection.SetValue("Show Milliseconds", TimeDisplaySettings.ShowMilliseconds);
			timeFormatSection.SetValue("Show Plus Symbol", TimeDisplaySettings.ShowPlusSymbol);
			timeFormatSection.SetValue("Show Minus Symbol", TimeDisplaySettings.ShowMinusSymbol);
			timeFormatSection.SetValue("Show Leading Zero", TimeDisplaySettings.ShowLeadingZero);
			timeFormatSection.SetValue("Show Separators", TimeDisplaySettings.ShowSeparators);

			configFile.Save(filename);
		}

		public static Preferences Load(string filename)
		{
			Preferences result = new Preferences();

			ConfigFile configFile = new ConfigFile();

			configFile.Load( filename );

			ConfigSection startupSection = configFile["Startup"];

			result.Startup.CreateClock = startupSection.GetBoolean("Create Clock", true);
			result.Startup.FirstClockName = startupSection.GetString("First Clock Name", "Sundial");

			result.TimeDisplaySettings = new TimeFormatSettings(TimeFormatFlags.Nothing);

			ConfigSection timeFormatSection = configFile["Time Format"];

			result.TimeDisplaySettings.ShowHours = timeFormatSection.GetBoolean("Show Hours", true);
			result.TimeDisplaySettings.ShowMinutes = timeFormatSection.GetBoolean("Show Minutes", true);
			result.TimeDisplaySettings.ShowSeconds = timeFormatSection.GetBoolean("Show Seconds", true);
			result.TimeDisplaySettings.ShowMilliseconds = timeFormatSection.GetBoolean("Show Milliseconds", true);
			result.TimeDisplaySettings.ShowPlusSymbol = timeFormatSection.GetBoolean("Show Plus Symbol", false);
			result.TimeDisplaySettings.ShowMinusSymbol = timeFormatSection.GetBoolean("Show Minus Symbol", true);
			result.TimeDisplaySettings.ShowLeadingZero = timeFormatSection.GetBoolean("Show Leading Zero", true);
			result.TimeDisplaySettings.ShowSeparators = timeFormatSection.GetBoolean("Show Separators", true);

			return result;
		}

		public static Preferences CreateDefault()
		{
			Preferences result = new Preferences();

			result.TimeDisplaySettings = 
				new TimeFormatSettings(TimeFormatFlags.AllTimeUnits |
				                       TimeFormatFlags.MinusSymbol |
				                       TimeFormatFlags.LeadingZero |
				                       TimeFormatFlags.Separators);

			result.Startup = new StartupSettings()
			{
				CreateClock = true,
				FirstClockName = "Sundial",
			};

			return result;
		}
	}
}

