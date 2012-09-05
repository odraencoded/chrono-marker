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
		private const int displayRefreshFrequency = 34;

		public StopwatchWindow(LoggingHandler logHandler) : 
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

			timeDisplayBox.GrabFocus();
			timeDisplayBox.SelectRegion(0,0);

			_logHandler = logHandler;

			Clock.Started += ClockStarted_event;
			Clock.Stopped += ClockStopped_event;
			Clock.SpeedChanged += ClockChangedSpeed_event;

			_logHandler.Renamed += ClockRenamed_event;

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
			RefreshDisplay( );
		}

		public Clock Clock { get { return _logHandler.Clock; } }
		public LoggingHandler LogHandler { get { return _logHandler; } }
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
			}
		}

		private readonly LoggingHandler _logHandler;
		private bool _hasEditedTime;
		private bool _isEditValid;
		private TimedCaller _timedRefreshCaller;

		private bool _onTop;
		private bool _compact;

		private FontDescription _normalFont;
		private FontDescription _compactFont;

		private static Gdk.Pixbuf _forwardIcon = Gdk.Pixbuf.LoadFromResource("Chrono.forward.png");
		private static Gdk.Pixbuf _backwardIcon = Gdk.Pixbuf.LoadFromResource("Chrono.backward.png");
        private static Gdk.Pixbuf _stopIcon = Gdk.Pixbuf.LoadFromResource("Chrono.stop.png");
    	private static Gdk.Pixbuf _undoIcon = Gdk.Pixbuf.LoadFromResource("Chrono.undo.png");

		public void RefreshDisplay()
		{
			if( !_hasEditedTime ) {
				timeDisplayBox.Text = TimeLogger.TimeToString( Clock.ElapsedTime );
			} else {
				if(!_isEditValid) {
					timeDisplayBox.ModifyText(
						StateType.Normal, new Gdk.Color(255, 0, 0) );
				} else{
					timeDisplayBox.ModifyText(
						StateType.Normal, new Gdk.Color(0, 0, 0) );
				}
			}
		}

		public void RefreshControls()
		{
			if( Clock.IsTicking == true ) {
				_clockButtonMode = ClockButtonMode.Stop;

				timeDisplayBox.Sensitive = false;
			} else {
				timeDisplayBox.Sensitive = true;

				if( !_isEditValid ) {
					_clockButtonMode = ClockButtonMode.Undo;
				
				} else 
					_clockButtonMode = ClockButtonMode.Start;
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
					{
						compactBtnImage.Pixbuf = _forwardIcon;
					}else{
						compactBtnImage.Pixbuf = _backwardIcon;
					}
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

				Title = _logHandler.Name;
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

				Title = string.Format("Stopwatch - {0}", _logHandler.Name);
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

			RefreshDisplay( );
			RefreshControls( );
		}
		private void ClockChangedSpeed_event(object sender, ClockEventArgs e)
		{
			RefreshControls();
		}

		private void refreshTimeout_event(object sender, TimedCallEventArgs e)
		{
			if( Visible )
				RefreshDisplay( );
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
					Clock.ElapsedTime = TimeLogger.StringToTime( timeDisplayBox.Text);
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
		protected void undo_event (object sender, EventArgs e)
		{
			_hasEditedTime = false;
			_isEditValid = true;

			RefreshDisplay();
			timeDisplayBox.GrabFocus( );
		}
		#endregion

		protected void windowDelete_event(object o, DeleteEventArgs args)
		{
			// This should stop the window from being destroyed
			args.RetVal = true;
			Hide();
		}

		protected void displayBoxChanged_event(object sender, EventArgs e)
		{
			// Avoids infinite front and back calling
			if( Clock.IsTicking == false ) {
				_hasEditedTime = true;
				_isEditValid = TimeLogger.IsStringValid( timeDisplayBox.Text );
				RefreshDisplay();
				RefreshControls();
			}
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
	}
}