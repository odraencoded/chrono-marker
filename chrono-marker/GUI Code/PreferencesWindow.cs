//
//  SettingsWindow.cs
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
//  MERCHAstartupClockNameEntryFOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using Pango;
using Mono.Unix;

namespace Chrono
{
	public partial class PreferencesWindow : Gtk.Window
	{
		public PreferencesWindow(Program program) : 
				base(Gtk.WindowType.Toplevel)
		{
			this.Build( );

			this.Program = program;

			FontDescription monospace = FontDescription.FromString( "monospace" );
			previewDisplayBox1.ModifyFont(monospace);
			previewDisplayBox2.ModifyFont(monospace);

			previewDisplaySettings = new TimeFormatSettings();

			RefreshTexts();
		}

		public Program Program { get; private set; }
		public Preferences Preferences { get { return Program.Settings; } }

		private TimeFormatSettings previewDisplaySettings;
		private bool supressValidation;

		private readonly TimeSpan[] previewTimes = new TimeSpan[] 
		{
			new TimeSpan(0,113,59,15,250),
			new TimeSpan(0,0,-3,-14,-159)
		};

		public void RefreshTexts()
		{
			Title = Catalog.GetString("Preferences");

			// General
			generalTabLabel.Text = Catalog.GetString("General");

			createWatchOnStartupCheck.Label = Catalog.GetString( "Create a stopwatch on startup" );
			startupClockNameLabel.Text = Catalog.GetString( "Name" );
			startupClockNameContainer.TooltipText = Catalog.GetString( "Name of the stopwatch to create on startup" );

			byDefaultLabel.Text = Catalog.GetString("By default stopwatches are");

			compactByDefaultCheck.Label = Catalog.GetString( "Compact" );
			compactByDefaultCheck.TooltipMarkup = Catalog.GetString( "Stopwatches will be created with a single button and a small display" );

			dockedByDefaultCheck.Label = Catalog.GetString("Docked");
			dockedByDefaultCheck.TooltipMarkup = Catalog.GetString("Stopwatches will be created docked in the log window");

			// Formatting
			formattingTabLabel.Text = Catalog.GetString("Formatting");
			formattingTopLabel.TooltipMarkup = Catalog.GetString("Show the following in the stopwatch display");

			showPlusSymbolCheck.Label = Catalog.GetString("Plus Symbol");
			showPlusSymbolCheck.TooltipMarkup = Catalog.GetString("Prefix a + when the time is positive");

			showMinusSymbolCheck.Label = Catalog.GetString("Minus Symbol");
			showMinusSymbolCheck.TooltipMarkup = Catalog.GetString("Prefix a - when the time is negative");

			showSeparatorsOption.Label = Catalog.GetString("Separators");
			showSeparatorsOption.TooltipMarkup = Catalog.GetString("Show separators between time units");

			showAbbreviationsOption.Label = Catalog.GetString("Abbreviations");
			showAbbreviationsOption.TooltipMarkup = Catalog.GetString("Show abbreviations next to time units");

			showLeadingZerosCheck.Label = Catalog.GetString("Leading Zeros");
			showLeadingZerosCheck.TooltipMarkup = Catalog.GetString("Uncheck to hide any left side zeros");

			showHoursCheck.Label = Catalog.GetString("Hours");
			showHoursCheck.TooltipMarkup = Catalog.GetString("Uncheck to convert hours to minutes");

			showMinutesCheck.Label = Catalog.GetString("Minutes");
			showMinutesCheck.TooltipMarkup = Catalog.GetString("Uncheck to convert minutes to seconds");

			showSecondsCheck.Label = Catalog.GetString("Seconds");
			showSecondsCheck.TooltipMarkup = Catalog.GetString("Uncheck to convert seconds to milliseconds");

			showMillisecondsCheck.Label = Catalog.GetString("Milliseconds");
			showMillisecondsCheck.TooltipMarkup = Catalog.GetString("Uncheck to hide fractions of second");

			previewLabel.Text = Catalog.GetString("Preview");
			previewContainer.TooltipMarkup = Catalog.GetString("For previewing purposes only");
		}

		private void LoadPreferences()
		{
			supressValidation = true;

			showHoursCheck.Active = Preferences.TimeDisplaySettings.ShowHours;
			showMinutesCheck.Active = Preferences.TimeDisplaySettings.ShowMinutes;
			showSecondsCheck.Active = Preferences.TimeDisplaySettings.ShowSeconds;
			showMillisecondsCheck.Active = Preferences.TimeDisplaySettings.ShowMilliseconds;
			showPlusSymbolCheck.Active = Preferences.TimeDisplaySettings.ShowPlusSymbol;
			showMinusSymbolCheck.Active = Preferences.TimeDisplaySettings.ShowMinusSymbol;
			showLeadingZerosCheck.Active = Preferences.TimeDisplaySettings.ShowLeadingZeroes;

			if( Preferences.TimeDisplaySettings.ShowSeparators )
				showSeparatorsOption.Active = true;
			else showAbbreviationsOption.Active = true;

			createWatchOnStartupCheck.Active = Preferences.CreateWatchOnStartup;
			startupClockNameEntry.Text = Preferences.StartupWatchName;

			compactByDefaultCheck.Active = Preferences.WatchCompactByDefault;
			dockedByDefaultCheck.Active = Preferences.WatchDockedByDefault;

			supressValidation = false;

			RefreshTimeFormatControls();
		}

		private void ApplyChanges()
		{
			TimeFormatSettings timeDisplaySettings = Program.Settings.TimeDisplaySettings;

			timeDisplaySettings.ShowHours = showHoursCheck.Active;
			timeDisplaySettings.ShowMinutes = showMinutesCheck.Active;
			timeDisplaySettings.ShowSeconds = showSecondsCheck.Active;
			timeDisplaySettings.ShowMilliseconds = showMillisecondsCheck.Active;
			timeDisplaySettings.ShowLeadingZeroes = showLeadingZerosCheck.Active;
			timeDisplaySettings.ShowMinusSymbol = showMinusSymbolCheck.Active;
			timeDisplaySettings.ShowPlusSymbol = showPlusSymbolCheck.Active;
			timeDisplaySettings.ShowSeparators = showSeparatorsOption.Active;

			Preferences.CreateWatchOnStartup =createWatchOnStartupCheck.Active;
			Preferences.StartupWatchName = startupClockNameEntry.Text;

			Preferences.WatchDockedByDefault = dockedByDefaultCheck.Active;
			Preferences.WatchCompactByDefault = compactByDefaultCheck.Active;

			List<LoggingHandler> clockList = Program.TimeLogger.GetClockList();

			foreach( LoggingHandler handler in  clockList) {
				StopwatchWindow clockWindow;

				if(Program.TryGetClockWindow(handler, out clockWindow))
				{
					clockWindow.RefreshTimeDisplay();
					clockWindow.RefreshControls();
				}
			}
		}

		private void RefreshTimeFormatControls()
		{	
			supressValidation = true;
			
			showHoursCheck.Sensitive = true;
			showMinutesCheck.Sensitive = true;
			showSecondsCheck.Sensitive = true;
			showMillisecondsCheck.Sensitive = true;
			
			if( showHoursCheck.Active ) {
				if( showMillisecondsCheck.Active )
				{
					showSecondsCheck.Active = true;
					showSecondsCheck.Sensitive = false;
				}
				
				if( showSecondsCheck.Active )
				{
					showMinutesCheck.Active = true;
					showMinutesCheck.Sensitive = false;
				}
				
				if(!(showMillisecondsCheck.Active || showSecondsCheck.Active || showMinutesCheck.Active))
					showHoursCheck.Sensitive = false;
			}
			else if( showMinutesCheck.Active ) {
				if( showMillisecondsCheck.Active )
				{
					showSecondsCheck.Active = true;
					showSecondsCheck.Sensitive = false;
				}
				
				if(!(showMillisecondsCheck.Active || showSecondsCheck.Active))
					showMinutesCheck.Sensitive = false;
			}
			else if(showSecondsCheck.Active    && !(showMillisecondsCheck.Active || showMinutesCheck.Active || showHoursCheck.Active))
			{
				showSecondsCheck.Sensitive = false;
			}
			else if(showMillisecondsCheck.Active
			        && !(showSecondsCheck.Active || showMinutesCheck.Active || showHoursCheck.Active))
			{
				showMillisecondsCheck.Sensitive = false;
			}
			
			previewDisplaySettings.ShowHours = showHoursCheck.Active;
			previewDisplaySettings.ShowMinutes = showMinutesCheck.Active;
			previewDisplaySettings.ShowSeconds = showSecondsCheck.Active;
			previewDisplaySettings.ShowMilliseconds = showMillisecondsCheck.Active;
			previewDisplaySettings.ShowLeadingZeroes = showLeadingZerosCheck.Active;
			previewDisplaySettings.ShowMinusSymbol = showMinusSymbolCheck.Active;
			previewDisplaySettings.ShowPlusSymbol = showPlusSymbolCheck.Active;
			previewDisplaySettings.ShowSeparators = showSeparatorsOption.Active;

			previewDisplayBox1.Text = previewDisplaySettings.ToString(previewTimes[0]);
			previewDisplayBox2.Text = previewDisplaySettings.ToString(previewTimes[1]);

			supressValidation = false;
		}

		#region GUI events
		protected void displayCheckToggled_event(object sender, EventArgs e)
		{
			if( supressValidation )
				return;
			
			RefreshTimeFormatControls( );
		}

		protected void createOnStartupToggled_event(object sender, EventArgs e)
		{
			startupClockNameEntry.Sensitive = createWatchOnStartupCheck.Active;
		}

		protected void ok_event (object sender, EventArgs e)
		{
			ApplyChanges();
			Hide();
		}
		
		protected void cancel_event (object sender, EventArgs e)
		{
			Hide();
		}
		
		protected void apply_event (object sender, EventArgs e)
		{
			ApplyChanges();
		}

		protected void windowShown_event(object sender, EventArgs e)
		{
			LoadPreferences();
		}

		protected void windowDelete_event(object o, Gtk.DeleteEventArgs args)
		{
			// This avoids deleting the window
			args.RetVal = true;

			HideOnDelete();
		}
		#endregion
	}
}