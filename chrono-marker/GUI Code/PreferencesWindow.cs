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
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;

namespace Chrono
{
	public partial class PreferencesWindow : Gtk.Window
	{
		public PreferencesWindow(Program program) : 
				base(Gtk.WindowType.Toplevel)
		{
			this.Build( );

			this.Program = program;

			previewTime = new TimeSpan(0, 3, 14, 15, 926);
			previewDisplaySettings = new TimeFormatSettings(TimeFormatFlags.Nothing);

			LoadPreferences();
		}

		public Program Program { get; private set; }
		public Preferences Preferences { get { return Program.Settings; } }

		private TimeSpan previewTime;
		private TimeFormatSettings previewDisplaySettings;
		private bool supressValidation;

		private void LoadPreferences()
		{
			supressValidation = true;

			showHoursCheck.Active = Preferences.TimeDisplaySettings.ShowHours;
			showMinutesCheck.Active = Preferences.TimeDisplaySettings.ShowMinutes;
			showSecondsCheck.Active = Preferences.TimeDisplaySettings.ShowSeconds;
			showMilisecondsCheck.Active = Preferences.TimeDisplaySettings.ShowMilliseconds;
			showPlusSymbolCheck.Active = Preferences.TimeDisplaySettings.ShowPlusSymbol;
			showMinusSymbolCheck.Active = Preferences.TimeDisplaySettings.ShowMinusSymbol;
			showLeadingZeroCheck.Active = Preferences.TimeDisplaySettings.ShowLeadingZero;

			if( Preferences.TimeDisplaySettings.ShowSeparators )
				showSeparatorsOption.Active = true;
			else showAbbreviationsOption.Active = true;


			if(Preferences.ClockCompactByDefault)
				compactModeOption.Active = true;
			else normalModeOption.Active = true;

			firstClockNameEntry.Text = Preferences.Startup.FirstClockName;
			createClockOnStartupCheck.Active = Preferences.Startup.CreateClock;
			startDockedCheck.Active = Preferences.Startup.StartDocked;

			supressValidation = false;

			RefreshTimeFormatControls();
		}

		private void ApplyChanges()
		{
			TimeFormatSettings timeDisplaySettings = Program.Settings.TimeDisplaySettings;

			timeDisplaySettings.ShowHours = showHoursCheck.Active;
			timeDisplaySettings.ShowMinutes = showMinutesCheck.Active;
			timeDisplaySettings.ShowSeconds = showSecondsCheck.Active;
			timeDisplaySettings.ShowMilliseconds = showMilisecondsCheck.Active;
			timeDisplaySettings.ShowLeadingZero = showLeadingZeroCheck.Active;
			timeDisplaySettings.ShowMinusSymbol = showMinusSymbolCheck.Active;
			timeDisplaySettings.ShowPlusSymbol = showPlusSymbolCheck.Active;
			timeDisplaySettings.ShowSeparators = showSeparatorsOption.Active;

			StartupSettings startupSettings;

			startupSettings.CreateClock = createClockOnStartupCheck.Active;
			startupSettings.FirstClockName = firstClockNameEntry.Text;
			startupSettings.StartDocked = startDockedCheck.Active;

			Preferences.Startup = startupSettings;

			Preferences.ClockCompactByDefault = compactModeOption.Active;

			foreach( LoggingHandler handler in Program.TimeLogger.Handlers ) {
				StopwatchWindow clockWindow;

				if(Program.GetClockWindow(handler, out clockWindow))
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
			showMilisecondsCheck.Sensitive = true;
			
			if( showHoursCheck.Active ) {
				if( showMilisecondsCheck.Active )
				{
					showSecondsCheck.Active = true;
					showSecondsCheck.Sensitive = false;
				}
				
				if( showSecondsCheck.Active )
				{
					showMinutesCheck.Active = true;
					showMinutesCheck.Sensitive = false;
				}
				
				if(!(showMilisecondsCheck.Active | showSecondsCheck.Active | showMinutesCheck.Active))
					showHoursCheck.Sensitive = false;
			}
			else if( showMinutesCheck.Active ) {
				if( showMilisecondsCheck.Active )
				{
					showSecondsCheck.Active = true;
					showSecondsCheck.Sensitive = false;
				}
				
				if(!(showMilisecondsCheck.Active | showSecondsCheck.Active | showHoursCheck.Active))
					showMinutesCheck.Sensitive = false;
			}
			else if(showSecondsCheck.Active
			        && !(showMilisecondsCheck.Active | showMinutesCheck.Active | showHoursCheck.Active))
			{
				showSecondsCheck.Sensitive = false;
			}
			else if(showMilisecondsCheck.Active
			        && !(showSecondsCheck.Active | showMinutesCheck.Active | showHoursCheck.Active))
			{
				showMilisecondsCheck.Sensitive = false;
			}
			
			previewDisplaySettings.ShowHours = showHoursCheck.Active;
			previewDisplaySettings.ShowMinutes = showMinutesCheck.Active;
			previewDisplaySettings.ShowSeconds = showSecondsCheck.Active;
			previewDisplaySettings.ShowMilliseconds = showMilisecondsCheck.Active;
			previewDisplaySettings.ShowLeadingZero = showLeadingZeroCheck.Active;
			previewDisplaySettings.ShowMinusSymbol = showMinusSymbolCheck.Active;
			previewDisplaySettings.ShowPlusSymbol = showPlusSymbolCheck.Active;
			previewDisplaySettings.ShowSeparators = showSeparatorsOption.Active;

			previewDisplayBox.Text = previewDisplaySettings.ToString(previewTime);

			supressValidation = false;
		}

		protected void WindowShown_event(object sender, EventArgs e)
		{
			TimeFormatSettings timeSettings = Program.Settings.TimeDisplaySettings;

			showHoursCheck.Active = timeSettings.ShowHours;
			showMinutesCheck.Active = timeSettings.ShowMinutes;
			showSecondsCheck.Active = timeSettings.ShowSeconds;
			showMilisecondsCheck.Active = timeSettings.ShowMilliseconds;
			showLeadingZeroCheck.Active = timeSettings.ShowLeadingZero;
			showMinusSymbolCheck.Active = timeSettings.ShowMinusSymbol;

			StartupSettings startupSettings = Program.Settings.Startup;

			createClockOnStartupCheck.Active = startupSettings.CreateClock;
			firstClockNameEntry.Text = startupSettings.FirstClockName;

			compactModeOption.Active = Program.Settings.ClockCompactByDefault;
		}

		protected void WindowDelete_event(object o, Gtk.DeleteEventArgs args)
		{
			// This avoids deleting the window
			args.RetVal = true;
			HideOnDelete();
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

		protected void displayCheckToggled_event(object sender, EventArgs e)
		{
			if( supressValidation )
				return;

			RefreshTimeFormatControls();
		}
	}
}