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
		public StopwatchWindow(LoggingHandler logHandler) : 
				base(Gtk.WindowType.Toplevel)
		{
			this.Build( );

			Watch = logHandler.Watch;

			Watch.Started += WatchStarted_event;
			Watch.Stopped += WatchStopped_event;
			Watch.ChangedSpeed += WatchChangeSpeed_event;


			if( Watch.IsRunning ) {
				timedRefreshCaller = new TimedCaller(40);
				timedRefreshCaller.TimeOut += refreshTimeout_event;
			}

			Title += " - " + logHandler.Name;

			isEditValid = true;
			hasEditedTime = false;

			FontDescription timeFont =
				FontDescription.FromString( "consolas 32" );

			timeDisplayBox.ModifyFont( timeFont );
			timeDisplayBox.Text = Logger.TimeToString( Watch.ElapsedTime );

			RefreshControls( );
			RefreshDisplay( );
		}

		public TimedCaller timedRefreshCaller;
		public Watch Watch { get; private set; }
		private bool hasEditedTime;
		private bool isEditValid;

		public void RefreshDisplay()
		{
			if( !hasEditedTime ) {
				timeDisplayBox.Text = Logger.TimeToString( Watch.ElapsedTime );
			} else {
				if(!isEditValid) {
					timeDisplayBox.ModifyText(
						StateType.Normal, new Gdk.Color(255, 0, 0) );

					startBtn.Label = "Undo";
				} else{
					timeDisplayBox.ModifyText(
						StateType.Normal, new Gdk.Color(0, 0, 0) );

					startBtn.Label = "Start";
				}
			}
		}

		public void RefreshControls()
		{
			if( Watch.IsRunning == true ) {
				startBtn.Label = "Stop";
				timeDisplayBox.Sensitive = false;
			} else {
				timeDisplayBox.Sensitive = true;

				if( !isEditValid ) {
					startBtn.Label = "Undo";
				
				} else
					startBtn.Label = "Start";
			}

			if( Watch.Speed >= 0 ) {
				forwardBtn.Sensitive = false;
				backwardBtn.Sensitive = true;
			} else {
				forwardBtn.Sensitive = true;
				backwardBtn.Sensitive = false;
			}
		}

		#region Dynamic Events
		private void WatchStarted_event(object sender, WatchEventArgs e)
		{
			if( timedRefreshCaller != null ) {
				timedRefreshCaller.Cancel( );
			}

			timedRefreshCaller = new TimedCaller(40);
			timedRefreshCaller.TimeOut += refreshTimeout_event;

			RefreshControls( );
		}
		private void WatchStopped_event(object sender, WatchEventArgs e)
		{
			timedRefreshCaller.Cancel( );
			timedRefreshCaller = null;

			RefreshControls( );
		}
		private void WatchChangeSpeed_event(object sender, WatchEventArgs e)
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
			Watch.ChangeSpeed(1.0);
		}
		protected void backwardBtn_event(object sender, EventArgs e)
		{
			Watch.ChangeSpeed(-1.0);
		}

		protected void startBtn_event(object sender, EventArgs e)
		{
			if( hasEditedTime ) {
				if( isEditValid ) {
					Watch.ElapsedTime = Logger.StringToTime( timeDisplayBox.Text);
				} else {
					hasEditedTime = false;
					isEditValid = true;
					RefreshDisplay();
					timeDisplayBox.GrabFocus( );

					return;
				}
			}

			hasEditedTime = false;
			isEditValid = true;

			Watch.StartStop( );
		}

		protected void windowDelete_event(object o, DeleteEventArgs args)
		{
			// This should stop the window from being destroyed
			args.RetVal = true;
			Hide();
		}

		protected void displayBoxChanged_event(object sender, EventArgs e)
		{
			// Avoids infinite front and back calling
			if( Watch.IsRunning == false ) {
				hasEditedTime = true;
				isEditValid = Logger.IsStringValid( timeDisplayBox.Text );
				RefreshDisplay();
			}
		}
		#endregion GUI events

		protected override void OnDestroyed()
		{
			if( timedRefreshCaller != null )
				timedRefreshCaller.Cancel( );

			base.OnDestroyed();
		}
	}
}

