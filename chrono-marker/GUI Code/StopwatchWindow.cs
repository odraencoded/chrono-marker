//
//  StopwatchWindow.cs
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
using Gtk;
using Pango;

namespace Chrono
{
	public partial class StopwatchWindow : Window
	{
		private const int displayRefreshFrequency = 66;

		public StopwatchWindow(Program program, LoggingHandler logHandler) : 
				base(Gtk.WindowType.Toplevel)
		{
			this.Build( );

			// Sets the focus chain AKA tab order
			FocusChain = new Widget[]
			{
				timeDisplayBox,
				bottomBtn,
				compactBtn,
				forwardBtn,
				backwardBtn
			};

			// Selects the display bo by default
			timeDisplayBox.GrabFocus();
			timeDisplayBox.SelectRegion(0,0);

			this.Program = program;
			this.LogHandler = logHandler;

			LogHandler.Renamed += ClockRenamed_event;

			Clock.Started += ClockStarted_event;
			Clock.Stopped += ClockStopped_event;
			Clock.SpeedChanged += ClockChangedSpeed_event;

			_clockButtonMode = ClockButtonMode.Unset;

			if( Clock.IsTicking ) {
				_timedRefreshCaller = new TimedCaller(displayRefreshFrequency);
				_timedRefreshCaller.TimeOut += refreshTimeout_event;
			}

			_isEditValid = true;
			_hasEditedTime = false;

			_normalFont = FontDescription.FromString( "consolas 32" );
			_compactFont = FontDescription.FromString( "consolas 16" );

			RefreshControls( );
			RefreshTimeDisplay( );
		}

		public Program Program { get; private set; }
		public Clock Clock { get { return LogHandler.Clock; } }
		public LoggingHandler LogHandler { get; private set; }

		// This is a wrapper around keep above that
		// stores the current value of "KeepAbove"
		public bool OnTop {
			get { return _onTop;}
			set {
				_onTop = value;
				KeepAbove = _onTop;
			}
		}

		public bool Compact {
			get { return _compact; }
			set
			{
				_compact = value; 
				RefreshControls();
				QueueResize();
			}
		}

		private bool _hasEditedTime;
		private bool _isEditValid;
		private TimeSpan _timeInput;
		private TimeSpan _clockTime;
		private TimedCaller _timedRefreshCaller;

		private bool _onTop;
		private bool _compact;
		private bool _supressBoxValidation;



		private FontDescription _normalFont;
		private FontDescription _compactFont;

		private static Gdk.Pixbuf _forwardIcon = Gdk.Pixbuf.LoadFromResource("Chrono.forward.png");
		private static Gdk.Pixbuf _backwardIcon = Gdk.Pixbuf.LoadFromResource("Chrono.backward.png");
        private static Gdk.Pixbuf _stopIcon = Gdk.Pixbuf.LoadFromResource("Chrono.stop.png");
    	private static Gdk.Pixbuf _undoIcon = Gdk.Pixbuf.LoadFromResource("Chrono.undo.png");

		public void RefreshTimeDisplay()
		{
			// Cache time
			_clockTime = Clock.ElapsedTime;

			// Absolute cap is 7 days.
			TimeSpan timeCap = new TimeSpan(7, 0, 0, 0, 0);

			if(_clockTime.Duration() > timeCap)
		   	{	
				if(_clockTime.Ticks >= 0) _clockTime = timeCap;
				else _clockTime = timeCap.Negate();

				// Preserve the order!!!
				Clock.Stop();
				Clock.ElapsedTime = _clockTime;
			}

			// Cancels refreshing the box itself while editing
			if(_hasEditedTime)
				return;

			_supressBoxValidation = true;

			timeDisplayBox.Text = LogHandler.TimeFormatSettings.ToString(_clockTime);
			timeDisplayBox.WidthChars = GetDisplayWidth(LogHandler.TimeFormatSettings);

			_supressBoxValidation = false;
		}

		public void RefreshControls()
		{
			if( Clock.IsTicking == true ) {
				_clockButtonMode = ClockButtonMode.Stop;

				timeDisplayBox.Sensitive = false;
			} else {
				timeDisplayBox.Sensitive = true;

				if( !_isEditValid ) 
					_clockButtonMode = ClockButtonMode.Undo;
				else _clockButtonMode = ClockButtonMode.Start;
			}

			if( _compact ) {
				forwardBtn.Visible = false;
				backwardBtn.Visible = false;
				bottomBtn.Visible = false;
				compactBtn.Visible = true;

				compactBtn.HasDefault = true;

				timeDisplayBox.ModifyFont( _compactFont );

				switch( _clockButtonMode ) {
				case ClockButtonMode.Start:
					if(Clock.Speed >= 0)
						compactBtnImage.Pixbuf = _forwardIcon;
					else compactBtnImage.Pixbuf = _backwardIcon;
					break;

				case ClockButtonMode.Stop:
					compactBtnImage.Pixbuf = _stopIcon;
					break;

				case ClockButtonMode.Undo:
					compactBtnImage.Pixbuf = _undoIcon;
					break;

				default:
					break;
				}

				Title = LogHandler.Name;
			}
			else
			{
				forwardBtn.Visible = true;
				backwardBtn.Visible = true;
				bottomBtn.Visible = true;
				compactBtn.Visible = false;

				bottomBtn.HasDefault = true;

				timeDisplayBox.ModifyFont( _normalFont );

				if( Clock.Speed >= 0 ) {
					forwardBtn.Sensitive = false;
					backwardBtn.Sensitive = true;
				} else {
					forwardBtn.Sensitive = true;
					backwardBtn.Sensitive = false;
				}

				switch( _clockButtonMode ) {
				case ClockButtonMode.Start:
					bottomBtn.Label = "Start";
					break;
				case ClockButtonMode.Stop:
					bottomBtn.Label = "Stop";
					break;
				case ClockButtonMode.Undo:
					bottomBtn.Label = "Undo";
					break;

				default:
					break;
				}

				Title = string.Format("Stopwatch - {0}", LogHandler.Name);
			}
		}

		private enum ClockButtonMode
		{ Unset = -1, Start, Stop, Undo };

		private ClockButtonMode _clockButtonMode;

		#region Dynamic Events
		private void ClockRenamed_event(object sender, EventArgs e)
		{
			RefreshControls();
		}

		private void ClockStarted_event(object sender, ClockEventArgs e)
		{
			if( _timedRefreshCaller != null ) {
				_timedRefreshCaller.Cancel( );
			}

			_timedRefreshCaller = new TimedCaller(displayRefreshFrequency);
			_timedRefreshCaller.TimeOut += refreshTimeout_event;

			RefreshControls( );
		}
		private void ClockStopped_event(object sender, ClockEventArgs e)
		{
			_timedRefreshCaller.Cancel();
			_timedRefreshCaller = null;

			RefreshTimeDisplay( );
			RefreshControls( );
		}
		private void ClockChangedSpeed_event(object sender, ClockEventArgs e)
		{
			RefreshControls();
		}

		private void refreshTimeout_event(object sender, TimedCallEventArgs e)
		{
			if( Visible )
				RefreshTimeDisplay( );
		}
		#endregion

		#region GUI events
		protected void forwardBtn_event(object sender, EventArgs e)
		{
			if( Clock.Speed < 0 )
				Clock.ChangeSpeed( Clock.Speed * -1 );
			else
				RefreshControls( );
		}
		protected void backwardBtn_event(object sender, EventArgs e)
		{
			if( Clock.Speed >= 0 )
				Clock.ChangeSpeed( Clock.Speed * -1 );
			else
				RefreshControls( );
		}

		#region Bottom Button handling
		protected void clockButton_event(object sender, EventArgs e)
		{
			switch( _clockButtonMode ) {
			case ClockButtonMode.Start:
				start_event(sender, e);
				break;
			case ClockButtonMode.Stop:
				stop_event(sender, e);
				break;
			case ClockButtonMode.Undo:
				undo_event(sender, e);
				break;
			default:
				break;
			}
		}

		protected void start_event(object sender, EventArgs e)
		{
			if( _hasEditedTime ) {
				if( _isEditValid )
					Clock.ElapsedTime = _timeInput;
				else throw new Exception();
			}

			_hasEditedTime = false;
			_isEditValid = true;

			Clock.Toggle( );
		}
		protected void stop_event (object sender, EventArgs e)
		{
			Clock.Toggle( );

			timeDisplayBox.GrabFocus();
		}
		protected void undo_event(object sender, EventArgs e)
		{
			_hasEditedTime = false;
			_isEditValid = true;

			timeDisplayBox.ModifyText( StateType.Normal, new Gdk.Color(0, 0, 0) );

			RefreshTimeDisplay( );
			RefreshControls( );

			timeDisplayBox.GrabFocus();
		}
		#endregion

		protected void displayBoxChanged_event(object sender, EventArgs e)
		{
			if( _supressBoxValidation || Clock.IsTicking )
				return;

			TimeSpan possibleTimeInput;

			_hasEditedTime = true;
			_isEditValid = TryParseTime(timeDisplayBox.Text, out possibleTimeInput);

			if(_isEditValid)
			{
				timeDisplayBox.ModifyText(StateType.Normal, new Gdk.Color(0, 0, 0) );
				_clockButtonMode = ClockButtonMode.Start;

				_timeInput = possibleTimeInput;
			}
			else
			{
				timeDisplayBox.ModifyText(StateType.Normal, new Gdk.Color(255, 0, 0) );
				_clockButtonMode = ClockButtonMode.Undo;

				_timeInput = _clockTime;
			}

			RefreshControls();
		}

		protected void windowDelete_event(object o, DeleteEventArgs args)
		{
			// This should stop the window from being destroyed
			args.RetVal = true;
			Hide();
		}
		#region Overrides
		protected override void OnShown()
		{
			base.OnShown( );

			// Necessary for whatever reason
			KeepAbove = OnTop;
		}

		protected override void OnDestroyed()
		{
			if( _timedRefreshCaller != null )
				_timedRefreshCaller.Cancel();

			base.OnDestroyed();
		}
		#endregion
		#endregion
	
		public static bool TryParseTime(string input, out TimeSpan result)
		{
			const int maxUnitCount = 4;
			
			input = input.Trim( );

			result = new TimeSpan();

			if( string.IsNullOrEmpty( input ) ) // This makes easier to undo
				return false;
			
			bool hasLeftSymbol; // This is a "+" or '-" symbol
			bool isNegative; // ...or is it?
			bool hasHours, hasMinutes, hasSeconds, hasMilli;
			hasLeftSymbol = isNegative =
				hasHours = hasMinutes = hasSeconds = hasMilli = false;

			int hoursIndex, minutesIndex, secondsIndex, milliIndex;
			hoursIndex = minutesIndex = secondsIndex = milliIndex = -1;

			int hoursVal, minutesVal, secondsVal, milliVal;
			hoursVal = minutesVal = secondsVal = milliVal = 0;

			bool knowSeparatorType, usesAbbreviations;
			knowSeparatorType = usesAbbreviations = false;

			int unitCount = 0;
			string[] manyUnitStrings = new string[maxUnitCount];
			
			for(int i = 0; i < input.Length;)
			{
				if(char.IsDigit(input[i]))
				{
					if(unitCount == maxUnitCount)
						return false;
					
					string timeUnit = "";

					do // Bet ya haven't seen one of these in a _while_
					{
						timeUnit += input[i];
						i++;
					} while(i < input.Length && char.IsDigit(input[i]));

					manyUnitStrings[unitCount] = timeUnit;
					unitCount++;
				}
				else
				{
					string nonDigitPart = "";
					
					do
					{
						nonDigitPart += input[i];
						i++;
					} while(i < input.Length && !char.IsDigit(input[i]));
					
					nonDigitPart = nonDigitPart.Trim().ToLowerInvariant();

					if(unitCount == 0)
					{
						if(!hasLeftSymbol)
						{
							isNegative = nonDigitPart == "-";

							if(isNegative) hasLeftSymbol = true;
							else if(nonDigitPart == "+") hasLeftSymbol = true;
							else return false;
						}
						else return false;

						continue;
					}

					if(!knowSeparatorType)
					{
						// Clearly, this will fail to check non valid separators.
						// Those are handled next.
						usesAbbreviations = !(nonDigitPart == ":" || nonDigitPart == ".");
						knowSeparatorType = true;
					}
					
					if(usesAbbreviations)
					{
						// While using abbreviations, order doesn't matter
						if(!hasHours && nonDigitPart == "h")
						{
							hasHours = true;
							hoursIndex = unitCount - 1;
						}
						else if(!hasMinutes && nonDigitPart == "m")
						{
							hasMinutes = true;
							minutesIndex = unitCount - 1;
						}
						else if(!hasSeconds && nonDigitPart == "s")
						{
							hasSeconds = true;
							secondsIndex = unitCount - 1;
						}
						else if(!hasMilli && nonDigitPart == "ms")
						{
							hasMilli = true;
							milliIndex = unitCount - 1;
						}
						else return false;
					}
					else
					{
						// There are no separators at the end of the string.
						// What would they separate if there were?
						if(i == input.Length)
							return false;

						if(nonDigitPart == ":" && !(hasHours && hasMinutes))
						{
							// The format is invalid if the "." separators comes
							// before the ":" separator.
							if(hasMilli)
								return false;

							if(!hasMinutes) hasMinutes = true;
							else hasHours = true;
						}
						else if(nonDigitPart == "." && !hasMilli)
						{
							hasMilli = true;
						}
						else return false;
					}
				}
			}

			if(unitCount == 0) 
				return false;

			// If there are neither separators or abbreviations
			// The program implies they are all seconds.
			if(!knowSeparatorType) // Implicit unitCount == 1
			{
				hasSeconds = true;
				usesAbbreviations = false;
			}

			if(!usesAbbreviations)
			{
				hasSeconds = true; // This is always true in this case.

				// Infer unit positioning from separator order here.
				if(hasHours) hoursIndex = 0;
				if(hasMinutes) minutesIndex = hasHours ? hoursIndex + 1 : 0;
				if(hasSeconds) secondsIndex = hasMinutes ? minutesIndex + 1 : 0;
				if(hasMilli) milliIndex = secondsIndex + 1;
			}

			for(int i = 0; i < unitCount; i++)
			{
				int unitVal;
				string unitString = manyUnitStrings[i];

				if(!usesAbbreviations)
				{
					if(i == milliIndex)
					{
						if(unitString.Length > 3)
							return false;
						
						unitString = unitString.PadRight(3, '0');
					}

					if(i != 0 && (i == secondsIndex || i == minutesIndex))
					{
						if(unitString.Length != 2)
							return false;
					}
				}

				// Even though the string is all digits, that won't prevent overflow errors
				if(!int.TryParse(unitString, out unitVal))
					return false;

				int maxUnitValue;
				if(i == milliIndex)
				{
					milliVal = unitVal; 

					if(hasSeconds || hasMinutes || hasHours)
						maxUnitValue = 999;
					// One week worth of milliseconds
					else maxUnitValue = 7 * 24 * 60 * 60 * 1000;
				}
				else if (i == secondsIndex)
				{
					secondsVal = unitVal;

					if(hasMinutes || hasHours)
						maxUnitValue = 59;
					else maxUnitValue = 7 * 24 * 60 * 60;
				}
				else if(i == minutesIndex)
				{
					minutesVal = unitVal;

					if(hasHours)
						maxUnitValue = 59;
					else maxUnitValue = 7 * 24 * 60;
				}
				else if(i == hoursIndex)
				{
					hoursVal = unitVal;

					maxUnitValue = 7 * 24; // Hours are only capped to a week
				}
				else return false; // This supposedly never happens though.

				if(unitVal > maxUnitValue)
					return false;
			}

			result = new TimeSpan(0, hoursVal, minutesVal, secondsVal, milliVal);

			if(result > new TimeSpan(7, 0, 0, 0, 0))
				return false;

			if(isNegative) result = result.Negate();

			return true;
		}

		public static int GetDisplayWidth(TimeFormatSettings settings)
		{
			int result = 0;
			
			if( settings.ShowPlusSymbol || settings.ShowMinusSymbol)
				result += 1; // + or -
			
			if( settings.ShowMilliseconds ) {
				if( settings.ShowSeconds)
				{
					if(settings.ShowSeparators)
						result += 4; // .999
					else result += 5; // 999ms
				}
				else
				{
					if(settings.ShowSeparators)
						result += 9; // 86400000
					else result += 11; // 86400000ms
				}
			}
			
			if( settings.ShowSeconds ) {
				if( settings.ShowMinutes )
					result += 3; // :59 or 59s
				else
				{
					if(settings.ShowSeparators)
						result += 5; // 86400
					else result += 6; // 86400s
				}
			}
			
			if( settings.ShowMinutes ) {
				if( settings.ShowHours )
					result += 3; // :59 or 59m
				else
				{
					if(settings.ShowSeparators)
						result += 4; // 1440
					else result += 5; // 1440m
				}
			}
			
			if( settings.ShowHours )
			{
				if(settings.ShowSeparators)
					result += 3; // 168
				else result += 4; // 168h
			}
			
			return result;
		}
	}
}