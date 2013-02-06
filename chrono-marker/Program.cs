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
using Mono.Unix;

namespace Chrono
{
	/// <summary>
	/// The whole thing.
	/// <remarks>Also a SPOF.</remarks>
	/// </summary>
    public sealed class Program
    {
		private const string settingsFilename = "config.ini";
		private const int historySteps = 128;

		private Program()
		{
			Gtk.Window.DefaultIconList = new Pixbuf[]
			{
				Pixbuf.LoadFromResource("chrono-marker-16.png"),
				Pixbuf.LoadFromResource("chrono-marker-24.png"),
				Pixbuf.LoadFromResource("chrono-marker-32.png"),
				Pixbuf.LoadFromResource("chrono-marker-48.png"),
				Pixbuf.LoadFromResource("chrono-marker-64.png"),
			};

			// Load settings. This is important.
			Settings = Preferences.Load( settingsFilename );

			TimeLogger = new TimeLogger(Settings.TimeDisplaySettings);

			TimeLogger.ClockAdded += loggerClockAdded_event;
			TimeLogger.ClockRemoved += loggerClockRemoved_event;

			History = new History(historySteps);

			// Creates exporters
			LogExporters = new List<Chrono.Files.ILogExporter>();
			LogExporters.Add(new Files.HTMLExporter());
			LogExporters.Add(new Files.XMLExporter());
			LogExporters.Add(new Files.PlainTextExporter());

			clockWindows = new Dictionary<LoggingHandler, StopwatchWindow>();
			LoggerWindow = new LoggerWindow(this);
			ClockPropertiesWindow = new ClockPropertiesWindow(this);
			PreferencesWindow = new PreferencesWindow(this);

			// Creates the first clock if configured to do so
			if( Settings.CreateWatchOnStartup && TimeLogger.CanCreateClock( Settings.StartupWatchName ) ) {
				LoggingHandler firstClockHandler =
					TimeLogger.CreateClock( Settings.StartupWatchName );

				StopwatchWindow clockWindow = clockWindows [firstClockHandler];

				clockWindow.DisplayVisible = true;
			}
		}
		public List<Files.ILogExporter> LogExporters { get; private set; }

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <remarks>Yo dawg, I herd u liek programs...</remarks>
        public static void Main(string[] args)
		{
			Application.Init();

			Catalog.Init("chrono-marker", "./i18n");

			Program myself = new Program();

			GLib.ExceptionManager.UnhandledException += myself.unhandledException_event;

			try
			{
				myself.LoggerWindow.Present();

				Application.Run();
			}
			finally
			{
				myself.Finish();
			}
		}
		
		private void unhandledException_event(GLib.UnhandledExceptionArgs args)
		{
			Finish();

			Exception theProblem = args.ExceptionObject as Exception;
			string theError;

			if( theProblem != null ) {
				string theLocalizable = Catalog.GetString("Chrono Marker is crashing due to the following error \"{0}\"");

				theError = string.Format(theLocalizable, theProblem);
			}
			else
			{
				theError = Catalog.GetString("Chrono Marker is crashing due to an unknown error.");
			}

			ShowError(null, theError);
		}
		
		private void Finish()
		{
			if(finished) return;

			Settings.SaveTo(settingsFilename);

			finished = true;
		}

		#region Properties & Fields
		public Preferences Settings { get; private set; }
		public TimeLogger TimeLogger { get; private set; }
		public History History { get; private set; }

		public ClockPropertiesWindow ClockPropertiesWindow { get; private set; }
		public LoggerWindow LoggerWindow { get; private set; }
		public PreferencesWindow PreferencesWindow { get; private set; }

		private Dictionary<LoggingHandler, StopwatchWindow> clockWindows;
		private bool finished;

		public bool TryGetClockWindow(LoggingHandler handler, out StopwatchWindow window)
		{
			return clockWindows.TryGetValue(handler, out window);
		}
		#endregion

		#region Events
		void loggerClockAdded_event(object sender, ClockHandlerEventArgs e)
		{
			StopwatchWindow newWindow = new StopwatchWindow(this, e.LoggingHandler);

			newWindow.Compact = Settings.WatchCompactByDefault;
			newWindow.Docked = Settings.WatchDockedByDefault;

			clockWindows.Add(e.LoggingHandler, newWindow);
		}

		void loggerClockRemoved_event(object sender, ClockHandlerEventArgs e)
		{
			StopwatchWindow obsoleteWindow;

			if(!clockWindows.TryGetValue(e.LoggingHandler, out obsoleteWindow))
				return;

			obsoleteWindow.Destroy();
			clockWindows.Remove(e.LoggingHandler);
		}
		#endregion

		public static void ShowError(Gtk.Window parent, string format, params object[] args)
		{
			string message = string.Format(format, args);

			MessageDialog errorDialog = new MessageDialog(
				parent, DialogFlags.Modal, MessageType.Error,
				ButtonsType.Ok, message);

			errorDialog.Title = "Oopz!";

			errorDialog.TransientFor = parent;

			errorDialog.Run();
			errorDialog.Destroy();
		}
	}	
}
