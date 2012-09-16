/* Copyright (C) 2012 Leonardo Augusto Pereira
 * 
 * This file is part of Chrono Marker 
 * 
 * Chrono Marker is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Chrono Marker is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Chrono Marker.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.IO;
using Gtk;
using Gdk;

namespace Chrono
{
	/// <summary>
	/// The whole thing.
	/// <remarks>Also a SPOF.</remarks>
	/// </summary>
    public sealed class Program
    {
		private const string settingsFilename = "config.ini";

		private Program()
		{
			clockWindows = new Dictionary<LoggingHandler, StopwatchWindow>();

			LoadPreferences();

			TimeLogger = new TimeLogger(Settings.TimeDisplaySettings);

			TimeLogger.ClockAdded += loggerClockAdded_event;
			TimeLogger.ClockRemoved += loggerClockRemoved_event;

			LoggerWindow = new LoggerWindow(this); // This
			ClockPropertiesWindow = new ClockPropertiesWindow(this); // And this
			PreferencesWindow = new PreferencesWindow(this); // And this too

			if(Settings.Startup.CreateClock)
			{
				LoggingHandler firstClockHandler =
					TimeLogger.CreateClock(Settings.Startup.FirstClockName);

				clockWindows[firstClockHandler].Present();
			}
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <remarks>Yo dawg, I herd u liek programs...</remarks>
        public static void Main(string[] args)
		{
			Application.Init();

			Program myself = new Program();

			try
			{
				myself.LoggerWindow.ShowAll();

				Application.Run();
			}
			finally
			{
				myself.Settings.SaveTo(settingsFilename);
			}
		}

		#region Properties & Fields
		public Preferences Settings { get; private set; }
		public TimeLogger TimeLogger { get; private set; }

		public ClockPropertiesWindow ClockPropertiesWindow { get; private set; }
		public LoggerWindow LoggerWindow { get; private set; }
		public PreferencesWindow PreferencesWindow { get; private set; }

		private Dictionary<LoggingHandler, StopwatchWindow> clockWindows;

		public bool GetClockWindow(LoggingHandler handler, out StopwatchWindow window)
		{
			return clockWindows.TryGetValue(handler, out window);
		}
		#endregion

		private void LoadPreferences()
		{
			bool loadedFromFile = false;

			if( File.Exists( settingsFilename ) ) {
				try
				{
					Settings = Preferences.Load(settingsFilename);
					loadedFromFile = true;
				}
				catch(IOException)
				{
					Console.Write("There was an error loading the preferences, creating defaults");
				}
			}

			if(!loadedFromFile)
				Settings = Preferences.CreateDefault();
		}	

		#region Events
		public event ClockHandlerEventHandler ClockWindowReceivedFocus;

		private void StopwatchFocusIn_event(object sender, FocusInEventArgs e)
		{
			// This relays a focus in event of a window to the rest of the application
			StopwatchWindow clockWindow = sender as StopwatchWindow;

			if(clockWindow != null && ClockWindowReceivedFocus != null)
				ClockWindowReceivedFocus(clockWindow, new ClockHandlerEventArgs(clockWindow.LogHandler));
		}

		void loggerClockAdded_event(object sender, ClockHandlerEventArgs e)
		{
			StopwatchWindow newWindow = new StopwatchWindow(this, e.LoggingHandler);

			newWindow.Compact = Settings.ClockCompactByDefault;
			newWindow.FocusInEvent += StopwatchFocusIn_event;

			clockWindows.Add(e.LoggingHandler, newWindow);
		}

		void loggerClockRemoved_event(object sender, ClockHandlerEventArgs e)
		{
			StopwatchWindow obsoleteWindow;

			if(!clockWindows.TryGetValue(e.LoggingHandler, out obsoleteWindow))
				return;

			obsoleteWindow.Destroy();
			obsoleteWindow.FocusInEvent -= StopwatchFocusIn_event;
			clockWindows.Remove(e.LoggingHandler);
		}
		#endregion
	}
}
