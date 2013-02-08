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
using Mono.Unix;

namespace Chrono
{
	public partial class StopwatchWindow : Window
	{
		public StopwatchWindow(Program program, LoggingHandler logHandler) : 
				base(Gtk.WindowType.Toplevel)
		{
			this.Build();

			_forwardIcon = Stetic.IconLoader.LoadIcon(this, "chrono-forward", IconSize.Button);
			_backwardIcon = Stetic.IconLoader.LoadIcon(this, "chrono-backward", IconSize.Button);
			_pauseIcon = Stetic.IconLoader.LoadIcon(this, "chrono-pause", IconSize.Button);
			_undoIcon = Stetic.IconLoader.LoadIcon(this, "chrono-undo", IconSize.Button);

			// Sets the focus chain AKA tab order
			Container frame = (Container)Children[0];
			frame.FocusChain = new Widget[]
			{
				timeDisplayBox,
				bottomBtn,
				compactBtn,
				forwardBtn,
				backwardBtn
			};

			this.Program = program;
			this.LogHandler = logHandler;

			LogHandler.Renamed += ClockRenamed_event;

			Clock.Started += ClockStarted_event;
			Clock.Stopped += ClockStopped_event;
			Clock.SpeedChanged += ClockChangedSpeed_event;

			_clockButtonMode = ClockButtonMode.Start;

			_displayBoxParser = new TimeParser(LogHandler.LowerCap, LogHandler.UpperCap);
			timeDisplayBox.Validator = _displayBoxParser;

			if( Clock.IsTicking ) {
				timeDisplayBox.DoValidation = false;
				program.TextRefresher.Handlers += refreshDisplayText;
			}

			_hasEditedTime = false;

			_normalFont = FontDescription.FromString( "monospace 32" );
			_compactFont = FontDescription.FromString( "monospace 16" );

			// Refresh...
			// ..all the things!
			RefreshLayout();
			RefreshTitle();
			RefreshControls( );
			RefreshTimeDisplay( );

			RefreshTexts();
		}

		#region Properties
		public Program Program { get; private set; }
		public Clock Clock { get { return LogHandler.Clock; } }
		public LoggingHandler LogHandler { get; private set; }

		// This is a wrapper around keep above that
		// stores the current value of "KeepAbove"
		public bool KeepVisible {
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
				if(_compact == value)
					return;

				_compact = value;

				RefreshLayout();
				RefreshTitle();
			}
		}

		public bool Docked {
			get
			{
				return _docked;
			}
			set
			{
				if(value) Dock();
				else Undock();
			}
		}

		public bool DisplayVisible {
			get {
				if( _docked )
					return _dockExpander.Expanded;
				else
					return Visible;
			}
			set {
				if( _docked )
					_dockExpander.Expanded = value;
				else {
					if( value )
						this.Present( );
					else
						this.Hide( );
				}

				OnDisplayVisibilityChanged(value);
			}
		}
		#endregion

		#region Fields
		private TimeParser _displayBoxParser;
		private bool _hasEditedTime;

		private bool _onTop;
		private bool _compact;
		private bool _docked;
		private Expander _dockExpander;

		private bool _supressVisibilityNotification;
		private FontDescription _normalFont;
		private FontDescription _compactFont;

		private Gdk.Pixbuf _forwardIcon, _backwardIcon, _pauseIcon, _undoIcon;

		private enum ClockButtonMode
		{ Start, Stop, Undo };
		
		private ClockButtonMode _clockButtonMode;
		#endregion

		public event EventHandler DisplayVisibilityChanged;

		public void Dock()
		{
			if( _docked )
				return;

			_supressVisibilityNotification = true;

			bool hasDefault = bottomBtn.HasDefault || compactBtn.HasDefault;

			if( _dockExpander == null ) {
				_dockExpander = Program.LoggerWindow.CreateDockExpander( );
				_dockExpander.Activated += expanderActivated_event;
				Program.LoggerWindow.FocusInEvent += windowFocusIn_event;
			}

			_dockExpander.Expanded = DisplayVisible;

			_dockExpander.Show();
			this.Hide();

			_docked = true;

			topContainer.Reparent( _dockExpander );

			if( hasDefault ) {
				if( _compact )
					compactBtn.HasDefault = true;
				else
					bottomBtn.HasDefault = true;
			}

			RefreshLayout( );
			RefreshTitle( );

			_supressVisibilityNotification = false;
		}

		public void Undock()
		{
			if( !_docked )
				return;

			_supressVisibilityNotification = true;

			bool hasDefault = bottomBtn.HasDefault || compactBtn.HasDefault;

			this.Visible = DisplayVisible;
			_dockExpander.Hide();
			topContainer.Reparent( this );
			_docked = false;

			if( hasDefault ) {
				if( _compact )
					compactBtn.HasDefault = true;
				else
					bottomBtn.HasDefault = true;
			}

			RefreshLayout( );
			RefreshTitle( );

			_supressVisibilityNotification = false;
		}

		public void RefreshTexts()
		{
			RefreshTitle( );

			timeDisplayBox.TooltipMarkup = Catalog.GetString( "You can edit the time in this box" );
			forwardBtn.TooltipMarkup = Catalog.GetString( "Count forward" );
			backwardBtn.TooltipMarkup = Catalog.GetString( "Count backward" );

			RefreshMainButton();
		}

		public void RefreshMainButton()
		{
			string mainButtonTooltip;
			
			switch( _clockButtonMode ) {
			case ClockButtonMode.Start:
				bottomBtn.Label = Catalog.GetString( "Start" );

				if( LogHandler.Clock.Speed >= 0 ) {
					compactBtnImage.Pixbuf = _forwardIcon;
					mainButtonTooltip = Catalog.GetString( "Begins counting forward" );
				} else {
					compactBtnImage.Pixbuf = _backwardIcon;
					mainButtonTooltip = Catalog.GetString( "Begins counting backward" );
				}

				break;

			case ClockButtonMode.Stop:
				bottomBtn.Label = Catalog.GetString( "Stop" );
				compactBtnImage.Pixbuf = _pauseIcon;
				mainButtonTooltip = Catalog.GetString( "Stops counting" );
				break;

			case ClockButtonMode.Undo:
				bottomBtn.Label = Catalog.GetString("Undo");
				compactBtnImage.Pixbuf = _undoIcon;
				mainButtonTooltip = Catalog.GetString( "Revert changes made to the display" );
				break;

			default:
				mainButtonTooltip = "";
				break;
			}

			compactBtn.TooltipMarkup = bottomBtn.TooltipMarkup = mainButtonTooltip;
		}

		public void RefreshLayout()
		{
			bool hasDefault = bottomBtn.HasDefault || compactBtn.HasDefault;

			if( _compact ) {
				forwardBtn.Visible = false;
				backwardBtn.Visible = false;
				bottomBtn.Visible = false;
				compactBtn.Visible = true;

				timeDisplayBox.ModifyFont( _compactFont );

				if(!Docked || hasDefault) compactBtn.HasDefault = true;
			} else {
				forwardBtn.Visible = true;
				backwardBtn.Visible = true;
				bottomBtn.Visible = true;
				compactBtn.Visible = false;

				timeDisplayBox.ModifyFont( _normalFont );

				if(!Docked || hasDefault) bottomBtn.HasDefault = true;
			}
		}

		public void RefreshTitle()
		{
			if( _compact ) {
				Title = LogHandler.Name;
			}
			else {
				Title = string.Format(
					Catalog.GetString( "Stopwatch - {0}" ),
					LogHandler.Name );
			}

			if(_dockExpander != null)
				_dockExpander.Label = LogHandler.Name;
		}

		public void RefreshTimeDisplay()
		{
			// Cache time
			TimeSpan clockTime = Clock.ElapsedTime;

			// Apply capping
			bool inRange = false;

			if( clockTime > LogHandler.UpperCap ) {	
				clockTime = LogHandler.UpperCap;
			} else if( clockTime < LogHandler.LowerCap ) {
				clockTime = LogHandler.LowerCap;
			}
			else inRange = true;

			if(!inRange)
			{
				Clock.Stop();
				Clock.ElapsedTime = clockTime;
			}

			// Cancels refreshing the box itself while editing
			if(_hasEditedTime)
				return;

			timeDisplayBox.Text = LogHandler.TimeFormatSettings.ToString(clockTime);
			timeDisplayBox.WidthChars = GetDisplayWidth(LogHandler.TimeFormatSettings);
		}

		public void RefreshControls()
		{
			if( Clock.IsTicking == true ) {
				_clockButtonMode = ClockButtonMode.Stop;

				timeDisplayBox.Sensitive = false;
			} else {
				timeDisplayBox.Sensitive = true;

				if(timeDisplayBox.IsValid) {
					_clockButtonMode = ClockButtonMode.Start;
				} else {
					_clockButtonMode = ClockButtonMode.Undo;
				}
			}

			RefreshMainButton();

			if( Clock.Speed >= 0 ) {
				forwardBtn.Sensitive = false;
				backwardBtn.Sensitive = true;
			} else {
				forwardBtn.Sensitive = true;
				backwardBtn.Sensitive = false;
			}
		}

		#region Dynamic Events
		private void ClockRenamed_event(object sender, EventArgs e)
		{
			RefreshTitle();
		}

		private void ClockStarted_event(object sender, ClockEventArgs e)
		{
			timeDisplayBox.DoValidation = false;

			if(DisplayVisible)
				Program.TextRefresher.Handlers += refreshDisplayText;

			RefreshControls( );
		}
		private void ClockStopped_event(object sender, ClockEventArgs e)
		{
			timeDisplayBox.DoValidation = true;

			RefreshTimeDisplay( );
			RefreshControls( );
		}
		private void ClockChangedSpeed_event(object sender, ClockEventArgs e)
		{
			RefreshControls();
		}

		private bool refreshDisplayText()
		{
			bool refreshing = DisplayVisible && Clock.IsTicking;

			if(refreshing)
				RefreshTimeDisplay();

			return refreshing;
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
			handleClockButtonAction();
		}

		private void handleClockButtonAction()
		{
			switch( _clockButtonMode ) {
			case ClockButtonMode.Start:
				startAction();
				break;
			case ClockButtonMode.Stop:
				stopAction();
				break;
			case ClockButtonMode.Undo:
				undoAction();
				break;
			default:
				break;
			}
		}

		private void startAction()
		{
			if( _hasEditedTime && timeDisplayBox.IsValid ) {
					Clock.ElapsedTime = _displayBoxParser.LastValidated;
			}

			_hasEditedTime = false;
			Clock.Toggle();
		}
		private void stopAction ()
		{
			Clock.Toggle( );

			timeDisplayBox.GrabFocus();
		}
		private void undoAction()
		{
			_hasEditedTime = false;

			timeDisplayBox.ModifyText( StateType.Normal, new Gdk.Color(0, 0, 0) );

			RefreshTimeDisplay( );
			RefreshControls( );

			timeDisplayBox.GrabFocus();
		}
		#endregion

		protected void displayBoxChanged_event(object sender, EventArgs e)
		{
			if( !timeDisplayBox.DoValidation || Clock.IsTicking )
				return;

			_hasEditedTime = true;

			if( timeDisplayBox.IsValid ) {
				timeDisplayBox.ModifyText( StateType.Normal, new Gdk.Color(0, 0, 0) );
				_clockButtonMode = ClockButtonMode.Start;

			} else {
				timeDisplayBox.ModifyText( StateType.Normal, new Gdk.Color(255, 0, 0) );
				_clockButtonMode = ClockButtonMode.Undo;
			}

			RefreshControls( );
		}

		// Sets the default button, this avoids mistaken default buttons
		// on compact mode.
		protected void displayBoxFocused_event (object o, EventArgs args)
		{
			if(Compact)	
				compactBtn.HasDefault = true;
			else
				bottomBtn.HasDefault = true;

			if(Docked)
				Program.ClockPropertiesWindow.SetHandler( LogHandler );
		}

		protected void windowFocusIn_event(object o, FocusInEventArgs args)
		{
			if( Docked ) {
				// In this case, the focus in event is raised from LoggerWindow
				if( timeDisplayBox.HasFocus )
					Program.ClockPropertiesWindow.SetHandler( LogHandler );
			} else {
				// In this case, the focus in event is raised from this.
				Program.ClockPropertiesWindow.SetHandler( LogHandler );
			}
		}
		
		void windowVisibilityChanged_event(object o, EventArgs args)
		{
			if(_docked)
				return;

			OnDisplayVisibilityChanged(Visible);
		}

		void expanderActivated_event(object o, EventArgs args)
		{
			if(!_docked)
				return;

			OnDisplayVisibilityChanged(_dockExpander.Expanded);
		}
		private void OnDisplayVisibilityChanged(bool visible){
			if(visible && Clock.IsTicking)
					Program.TextRefresher.Handlers += refreshDisplayText;

			if(_supressVisibilityNotification)
				return;

			if(DisplayVisibilityChanged != null)
				DisplayVisibilityChanged(this, new EventArgs());
		}

		protected void windowDelete_event(object o, DeleteEventArgs args)
		{
			Hide();

			// This should stop the window from being destroyed
			args.RetVal = true;
		}
		#region Overrides
		protected override void OnShown()
		{
			base.OnShown();

			// Necessary for whatever reason
			KeepAbove = KeepVisible;
			QueueResize();
		}

		protected override void OnDestroyed()
		{
			Program.TextRefresher.Handlers -= refreshDisplayText;

			if( _dockExpander != null ) {
				Program.LoggerWindow.FocusInEvent -= windowFocusIn_event;
				_dockExpander.Destroy( );
			}

			base.OnDestroyed();
		}
		#endregion
		#endregion

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