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
using Mono.Unix;

namespace Chrono
{
	public partial class ClockPropertiesWindow : Window
	{
		public ClockPropertiesWindow(Program program) : 
				base(Gtk.WindowType.Toplevel)
		{
			this.Build( );

			this.Program = program;

			watchSpeedEntry.Alignment = 1;

			_comboNameIters = new Dictionary<LoggingHandler, TreeIter>();

			_handlerListStore = new ListStore(typeof(LoggingHandler), typeof(string));
			watchNameCombo.Model = _handlerListStore;

			List<LoggingHandler> manyHandlers = Logger.GetClockList();

			watchNameCombo.Entry.ActivatesDefault = true;
			watchNameCombo.SetCellDataFunc(watchNameCombo.Cells[0], new CellLayoutDataFunc(renderComboBoxCells));

			watchNameCombo.TextColumn = 1;

			// Startup refresh
			foreach( LoggingHandler handler in manyHandlers ) {
				TreeIter iter =  _handlerListStore.AppendValues(handler, handler.Name);
				
				_comboNameIters.Add(handler, iter);
			}
			if(manyHandlers.Count > 0)
			{
				ChangeHandler(manyHandlers[0]);
				watchNameCombo.Active = 0;
			}

			// Events
			watchNameCombo.Entry.Changed += clockNameEntryChanged_event;
			Logger.ClockAdded += loggerClockAdded_event;
			Logger.ClockRemoved += loggerClockRemoved_event;
			Logger.ClockRenamed += loggerClockRenamed_event;

			RefreshTexts();
		}

		public Program Program { get; private set; }

		public TimeLogger Logger { get { return Program.TimeLogger; } }
		public LoggerWindow LoggerWindow { get { return Program.LoggerWindow; } }

		private LoggingHandler _currentHandler;
		private StopwatchWindow _clockWindow;

		private Dictionary<LoggingHandler, TreeIter> _comboNameIters;
		private ListStore _handlerListStore;

		// This supresses errors
		// When RefreshControls is called
		private bool _refreshing;

		public void RefreshTexts()
		{
			Title = Catalog.GetString("Stopwatches");

			watchNameLabel.Text = Catalog.GetString("Name");
			watchNameContainer.TooltipMarkup = Catalog.GetString("Type a stopwatch name to edit it or to create a new stopwatch");

			RefreshWatchButtonText();

			// General tab

			generalTabLabel.Text = Catalog.GetString("General");

			logTheFollowingLabel.Text = Catalog.GetString("Log the following events");
			logStartsCheck.Label = Catalog.GetString("When the Start button is clicked");
			logStopsCheck.Label = Catalog.GetString("When the Stop button is clicked");
			deleteBtn.TooltipMarkup = Catalog.GetString("Click here to delete this stopwatch, this action cannot be undone");

			// Counting tab
			countingTabLabel.Text = Catalog.GetString("Counting");
			countingDirectionLabel.Text = Catalog.GetString("Choose the counting direction");
			countForwardOption.Label = Catalog.GetString("Count Forward");
			countBackwardOption.Label = Catalog.GetString("Count Backward");

			watchSpeedLabel.Text = Catalog.GetString("Counting Speed");
			watchSpeedContainer.TooltipMarkup = Catalog.GetString("How many seconds are elapsed in this stopwatch every second");

			applySpeedBtn.Label = Catalog.GetString("Apply Settings");
			applySpeedBtn.TooltipMarkup = Catalog.GetString("Click here to apply the changes made to this stopwatch counting settings");

			// Window tab
			windowTabLabel.Text = Catalog.GetString("Window");

			configureLayoutLabel.Text = Catalog.GetString("Configure the window layout");
			dockedCheck.Label = Catalog.GetString("Dock in Log Window");
			dockedCheck.TooltipMarkup = Catalog.GetString("Check to dock this stopwatch's window inside the log window");

			compactCheck.Label = Catalog.GetString("Compact Mode");
			compactCheck.TooltipMarkup = Catalog.GetString("Check to show only one button and a smaller display in this stopwatch's window");

			windowVisibleBtn.Label = Catalog.GetString("Visible");
			windowVisibleBtn.TooltipMarkup = Catalog.GetString("Click here to show or hide this stopwatch's window");

			keepAboveCheck.Label = Catalog.GetString("Keep Above");
			keepAboveCheck.TooltipMarkup = Catalog.GetString("Check to keep this stopwatch's window on top of other windows");
		}

		public void RefreshWatchButtonText()
		{
			if( _currentHandler == null ) {
				watchNameButton.Label = Catalog.GetString( "Create" );
				watchNameButton.TooltipMarkup = Catalog.GetString("Click here to create a stopwatch with this name");
			}
			else
			{
				watchNameButton.Label = Catalog.GetString("Rename...");
				watchNameButton.TooltipMarkup = Catalog.GetString("Click here to rename this stopwatch");
			}
		}

		public void SetHandler(LoggingHandler handler)
		{
			watchNameCombo.Entry.Text = handler.Name;
		}

		private void RefreshHandler()
		{
			string clockName = watchNameCombo.Entry.Text;

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
				_currentHandler.Clock.SpeedChanged -= clockSpeedChanged_event;
				_clockWindow.DisplayVisibilityChanged -= clockWindowDisplayChanged_event;
			}

			_currentHandler = newHandler;
			if( _currentHandler != null ) {
				_currentHandler.Clock.SpeedChanged += clockSpeedChanged_event;

				Program.TryGetClockWindow( _currentHandler, out _clockWindow );
				_clockWindow.DisplayVisibilityChanged += clockWindowDisplayChanged_event;
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
					countForwardOption.Active = true;
				else
					countBackwardOption.Active = true;

				watchSpeedEntry.Value = Math.Abs( _currentHandler.Clock.Speed );

				compactCheck.Active = _clockWindow.Compact;
				windowVisibleBtn.Active = _clockWindow.DisplayVisible;
				dockedCheck.Active = _clockWindow.Docked;
				keepAboveCheck.Active = _clockWindow.KeepVisible;
				keepAboveCheck.Sensitive = !_clockWindow.Docked && _clockWindow.DisplayVisible;

				// When the name is valid, createBtn becomes renameBtn!!!
				watchNameButton.Label = Catalog.GetString("Rename");
				watchNameButton.Sensitive = true;
			} else {
				logStartsCheck.Active = false;
				logStopsCheck.Active = false;

				countForwardOption.Active = true;
				watchSpeedEntry.Value = 1.0;

				compactCheck.Active = false;
				dockedCheck.Active = false;
				windowVisibleBtn.Active = false;
				keepAboveCheck.Active = false;

				watchNameButton.Label = Catalog.GetString("Create");

				watchNameButton.Sensitive = Logger.CanCreateClock(watchNameCombo.Entry.Text);
			}

			_refreshing = false;
		}

		#region Dynamic events
		private void loggerClockAdded_event(object sender, ClockHandlerEventArgs e)
		{
			bool first = _comboNameIters.Count == 0;

			TreeIter iter = _handlerListStore.AppendValues( e.LoggingHandler, e.LoggingHandler.Name );

			_comboNameIters.Add( e.LoggingHandler, iter );

			if( first ) {
				ChangeHandler(e.LoggingHandler);
				watchNameCombo.Active = 0;
			}
		}
		private void loggerClockRemoved_event(object sender, ClockHandlerEventArgs e)
		{
			TreeIter removedIter;
			if(!_comboNameIters.TryGetValue(e.LoggingHandler, out removedIter))
			{
				Console.Error.WriteLine( "Failed to get clock treeiter name \"" + e.LoggingHandler.Name + "\"." );
				return;
			}

			_handlerListStore.Remove( ref removedIter );

			if( _currentHandler == e.LoggingHandler ) {
				ChangeHandler( null );

				// If there are any stopwatches left, change to the first one
				// Otherwise clear the stopwatch name entry
				TreeIter firstIter;
				if( _handlerListStore.GetIterFirst( out firstIter ) )
					watchNameCombo.Entry.Text = _handlerListStore.GetValue( firstIter, watchNameCombo.TextColumn ) as String;
				else watchNameCombo.Entry.Text = "";

				RefreshControls();
			}
		}
		private void loggerClockRenamed_event(object sender, ClockHandlerEventArgs e)
		{
			TreeIter renamedIter;
			if( !_comboNameIters.TryGetValue( e.LoggingHandler, out renamedIter ) ) {
				Console.Error.WriteLine( "Failed to get clock treeiter name \"" + e.LoggingHandler.Name + "\"." );
				return;
			}

			_handlerListStore.SetValue(renamedIter, 1, e.LoggingHandler.Name);

			_refreshing = true;

			if( _currentHandler == e.LoggingHandler ) {
				watchNameCombo.Entry.Text = _currentHandler.Name;
			}

			_refreshing = false;
		}

		private void clockSpeedChanged_event(object sender, ClockEventArgs args)
		{
			_refreshing = true;
			if(args.Speed >= 0)
				countForwardOption.Active = true;
			else countBackwardOption.Active = true;

			watchSpeedEntry.Value = Math.Abs(args.Speed);

			_refreshing = false;
		}

		private void clockWindowDisplayChanged_event(object sender, EventArgs args)
		{
			_refreshing = true;
			windowVisibleBtn.Active = _clockWindow.DisplayVisible;
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
			if( watchNameCombo.GetActiveIter( out activeIter ) ) {
				LoggingHandler selectedHandler =
					_handlerListStore.GetValue(activeIter, 0) as LoggingHandler;

				if(selectedHandler != null)
					watchNameCombo.Entry.Text = selectedHandler.Name;
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
			string clockName = watchNameCombo.Entry.Text;

			if( _currentHandler == null ) {
				if(!Logger.CanCreateClock(clockName))
				{
					string localError = Catalog.GetString("Failed to create stopwatch \"{0}\".");

					Program.ShowError(this, localError, clockName);
					return;
				}

				LoggingHandler newHandler = Logger.CreateClock(clockName );
				ChangeHandler( newHandler );

				_clockWindow.DisplayVisible = true;

				// This avoids having the newly created window appear
				// Over the properties window
				this.Present(); 

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
						if(string.IsNullOrWhiteSpace(newName))
						{
							string localError = Catalog.GetString("Blank names are not valid.");

							Program.ShowError(renameDiag, localError);

							goto RunRenameDialog;
						}
						// This name comparison allows the user to rename a clock using different casing
						else if(!string.Equals(newName, _currentHandler.Name, TimeLogger.HandlerNameComparison)
						   && _currentHandler.Logger.HasClock( newName ) ) {

							string localError = Catalog.GetString("A stopwatch with named \"{0}\" already exists!\n" +
							                  "Please choose another name.");

							Program.ShowError(renameDiag, localError , newName);

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

			double newSpeed = watchSpeedEntry.Value;
			if( countBackwardOption.Active )
				newSpeed *= -1;

			_currentHandler.Clock.ChangeSpeed( newSpeed );
		}
		#region Window settings
		protected void docked_event(object sender, EventArgs e)
		{
			if( _refreshing || _clockWindow == null )
				return;

			_clockWindow.Docked = dockedCheck.Active;

			keepAboveCheck.Sensitive = !dockedCheck.Active && windowVisibleBtn.Active;
		}

		protected void compact_event(object sender, EventArgs e)
		{
			if( _refreshing || _clockWindow == null )
				return;

			_clockWindow.Compact = compactCheck.Active;
		}

		protected void keepVisible_event (object sender, EventArgs e)
		{
			if( _refreshing || _clockWindow == null )
				return;

			_clockWindow.KeepVisible = keepAboveCheck.Active;
		}

		protected void visible_event(object sender, EventArgs e)
		{
			if(_refreshing || _clockWindow == null )
				return;

			_clockWindow.DisplayVisible = windowVisibleBtn.Active;

			keepAboveCheck.Sensitive = !dockedCheck.Active && windowVisibleBtn.Active;
		}
		#endregion
		#endregion

		protected void closeWindow_event(object sender, EventArgs e)
		{
			Hide();
		}

		protected void windowDelete_event(object o, DeleteEventArgs args)
		{
			// Prevents from destroying
			args.RetVal = true;
			HideOnDelete();
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