//
//  WatchConfigWindow.cs
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
using System.Collections.Generic;
using Gtk;

namespace Chrono
{
	public partial class ClockPropertiesWindow : Gtk.Window
	{
		public ClockPropertiesWindow(Program program) : 
				base(Gtk.WindowType.Toplevel)
		{
			Program = program;

			Program.ClockWindowReceivedFocus += AnyClockWindowFocused_event;

			Logger.ClockAdded += loggerClockAdded_event;
			Logger.ClockRemoved += loggerClockRemoved_event;

			this.Build( );

			_comboNameIters = new Dictionary<LoggingHandler, TreeIter>();

			LoggingHandler[] manyHandlers = Logger.Handlers;

			clockNameBox.Model = _comboEntryList = new ListStore(typeof(LoggingHandler));

			foreach( LoggingHandler handler in manyHandlers ) {
				TreeIter iter =  _comboEntryList.AppendValues(handler);

				_comboNameIters.Add(handler, iter);
			}

			clockNameBox.Entry.ActivatesDefault = true;
			clockNameBox.Entry.Changed += clockNameEntryChanged_event;

			clockNameBox.SetCellDataFunc(clockNameBox.Cells[0], new CellLayoutDataFunc(renderComboBoxCells));

			if(manyHandlers.Length > 0)
			{
				ChangeHandler(manyHandlers[0]);
				clockNameBox.Active = 0;
			}
		}

		public Program Program { get; private set; }

		public TimeLogger Logger { get { return Program.TimeLogger; } }
		public LoggerWindow LoggerWindow { get { return Program.LoggerWindow; } }

		private LoggingHandler _currentHandler;
		private StopwatchWindow _clockWindow;

		private Dictionary<LoggingHandler, TreeIter> _comboNameIters;
		private ListStore _comboEntryList;

		// This helps supress false positive errors
		// When RefreshControls is called
		private bool _refreshing;

		private void RefreshHandler()
		{
			string clockName = clockNameBox.Entry.Text;

			LoggingHandler newHandler;

			if( ! Logger.TryGetHandler( clockName, out newHandler ) )
				newHandler = null;

			ChangeHandler(newHandler);
		}

		private void ChangeHandler(LoggingHandler newHandler)
		{
			if( newHandler == _currentHandler )
				return;

			if( _clockWindow != null ) {
				_currentHandler.Renamed -= handlerRenamed_event;
				_currentHandler.Clock.SpeedChanged -= clockSpeedChanged_event;

				_clockWindow.Shown -= clockWindowVisibilityChanged_event;
				_clockWindow.Hidden -= clockWindowVisibilityChanged_event;
			}

			_currentHandler = newHandler;
			if( _currentHandler != null ) {
				_currentHandler.Renamed += handlerRenamed_event;
				_currentHandler.Clock.SpeedChanged += clockSpeedChanged_event;

				Program.GetClockWindow( _currentHandler, out _clockWindow );
				_clockWindow.Shown += clockWindowVisibilityChanged_event;
				_clockWindow.Hidden += clockWindowVisibilityChanged_event;
			} else {
				_clockWindow = null;
			}
		}

		private void RefreshControls()
		{
			_refreshing = true;

			// This disables every sort of configuration if
			// there is no clock selected
			bool valid = _currentHandler != null;
			detailBook.Sensitive = valid;

			// This loads the GUI data from the actual class
			// data. The "else" part sets defaults.
			if( valid ) {
				logStartsCheck.Active = _currentHandler.LogStarts;
				logStopsCheck.Active = _currentHandler.LogStops;

				if( _currentHandler.Clock.Speed >= 0 )
					tickForwardOption.Active = true;
				else
					tickBackwardOption.Active = true;

				speedEntry.Value = Math.Abs( _currentHandler.Clock.Speed );

				if(_clockWindow.Compact)
					compactWindowOption.Active = true;
				else normalWindowOption.Active = true;

				onTopCheck.Active = _clockWindow.OnTop;
				displayStopwatchBtn.Active = _clockWindow.Visible;

				// When the name is valid, createBtn becomes renameBtn!!!
				clockNameBtn.Label = "Rename";
			}else {
				logStartsCheck.Active = false;
				logStopsCheck.Active = false;

				tickForwardOption.Active = true;
				speedEntry.Value = 1.0;

				normalWindowOption.Active = true;

				onTopCheck.Active = false;
				displayStopwatchBtn.Active = false;

				clockNameBtn.Label = "Create";
			}

			_refreshing = false;
		}

		#region Dynamic events
		private void loggerClockAdded_event(object sender, ClockHandlerEventArgs e)
		{
			TreeIter iter = _comboEntryList.AppendValues(e.LoggingHandler);

			_comboNameIters.Add(e.LoggingHandler, iter);
		}
		private void loggerClockRemoved_event(object sender, ClockHandlerEventArgs e)
		{
			TreeIter removedIter = _comboNameIters [e.LoggingHandler];

			_comboEntryList.Remove( ref removedIter );

			if( _currentHandler == e.LoggingHandler ) {
				ChangeHandler( null );

				TreeIter firstIter;
				if( _comboEntryList.GetIterFirst( out firstIter ) )
					clockNameBox.Entry.Text = _comboEntryList.GetValue( firstIter, 0 ) as String;

				RefreshControls();
			}
		}
		private void handlerRenamed_event(object sender, EventArgs e)
		{
			_refreshing = true;
			if( _currentHandler != null ) {
				clockNameBox.Entry.Text = _currentHandler.Name;
			}

			_refreshing = false;
		}

		private void AnyClockWindowFocused_event(object sender, ClockHandlerEventArgs e)
		{
			clockNameBox.Entry.Text = e.LoggingHandler.Name;
		}

		private void clockSpeedChanged_event(object sender, ClockEventArgs args)
		{
			_refreshing = true;
			if(args.Speed >= 0)
				tickForwardOption.Active = true;
			else tickBackwardOption.Active = true;

			speedEntry.Value = Math.Abs(args.Speed);

			_refreshing = false;
		}

		private void clockWindowVisibilityChanged_event(object sender, EventArgs args)
		{
			_refreshing = true;
			displayStopwatchBtn.Active = _clockWindow.Visible;
			_refreshing = false;
		}
		#endregion

		#region GUI events
		#region Watch name handling
		private void renderComboBoxCells(CellLayout cell_layout, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			LoggingHandler logHandler = model.GetValue( iter, 0 ) as LoggingHandler;
			if( logHandler != null )
				( cell as CellRendererText ).Text = logHandler.Name;
		}

		protected void clockNameChanged_event(object sender, EventArgs e)
		{
			TreeIter activeIter;
			if( clockNameBox.GetActiveIter( out activeIter ) ) {
				LoggingHandler selectedHandler =
					_comboEntryList.GetValue(activeIter, 0) as LoggingHandler;

				if(selectedHandler != null)
					clockNameBox.Entry.Text = selectedHandler.Name;
			}
		}

		protected void clockNameEntryChanged_event(object sender, EventArgs e)
		{
			if( _refreshing )
				return;

			RefreshHandler();
			RefreshControls();
		}

		protected void clockNameBtn_event(object sender, EventArgs e)
		{
			string clockName = clockNameBox.Entry.Text;

			if( _currentHandler == null ) {
				LoggingHandler newHandler = Logger.CreateClock(clockName );
				ChangeHandler( newHandler );

				_clockWindow.Show();
				Present();

				RefreshControls( );

			} else {
				RenameClockDialog renameDiag = new RenameClockDialog(_currentHandler.Name);

				renameDiag.TransientFor = this;
			RunRenameDialog:
				if(renameDiag.Run() == (int)ResponseType.Ok)
				{
					string newName = renameDiag.NewName;
					if(newName != _currentHandler.Name)
					{
						if(!string.Equals(newName, _currentHandler.Name, TimeLogger.HandlerNameComparison)
						   && _currentHandler.Logger.HasClock( newName ) ) {

							MessageDialog errorDiag =  new MessageDialog(
								this, DialogFlags.DestroyWithParent, MessageType.Error, ButtonsType.Ok,
								"A clock with the name \"{0}\" already exists!"
								+Environment.NewLine+"Please choose another name.", newName);

							errorDiag.TransientFor = renameDiag;
							errorDiag.Run();
							errorDiag.Destroy();

							goto RunRenameDialog;
						}

						_currentHandler.Name = newName;

						RefreshControls();
					}
				}

				renameDiag.Destroy();
			}
		}

		protected void deleteClock_event(object sender, EventArgs e)
		{
			if( _currentHandler == null )
				return;

			Logger.RemoveClock( _currentHandler.Name );
		}
		#endregion

		#region Config stuff
		protected void logStarts_event (object sender, EventArgs e)
		{
			if( _refreshing || _currentHandler == null )
				return;

			_currentHandler.LogStarts = logStartsCheck.Active;
		}
		protected void logStops_event (object sender, EventArgs e)
		{
			if( _refreshing || _currentHandler == null )
				return;

			_currentHandler.LogStops = logStopsCheck.Active;
		}

		protected void setSpeed_event(object sender, EventArgs e)
		{
			if( _currentHandler == null )
				return;

			double newSpeed = speedEntry.Value;
			if( tickBackwardOption.Active )
				newSpeed *= -1;

			_currentHandler.Clock.ChangeSpeed( newSpeed );
		}

		protected void windowMode_event(object sender, EventArgs e)
		{
			if(_refreshing || _clockWindow == null)
				return;

			_clockWindow.Compact = compactWindowOption.Active;
		}

		protected void onTop_event (object sender, EventArgs e)
		{
			if( _refreshing || _clockWindow == null )
				return;

			_clockWindow.OnTop = onTopCheck.Active;
		}

		protected void displayStopwatch_event(object sender, EventArgs e)
		{
			if(_refreshing || _clockWindow == null )
				return;

			if( displayStopwatchBtn.Active ) {
				_clockWindow.Show( );
			} else {
				_clockWindow.Hide( );
			}
		}
		#endregion

		protected void closeWindow_event(object sender, EventArgs e)
		{
			Hide();
		}

		protected void windowDelete_event(object o, DeleteEventArgs args)
		{
			ChangeHandler(null);
			Hide( );

			// Prevents from destroying
			args.RetVal = true;
		}
		#endregion

		#region Overrides
		protected override void OnShown()
		{
			base.OnShown();

			RefreshHandler();
			RefreshControls();
		}

		protected override void OnDestroyed()
		{
			ChangeHandler(null);

			base.OnDestroyed();
		}


		#endregion
	}
}