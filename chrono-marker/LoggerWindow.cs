//
//  LoggerWindow.cs
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
using System.Collections.Generic;

namespace Chrono
{
	public partial class LoggerWindow : Window
	{
		public LoggerWindow(TimeLogger logger) : 
				base(Gtk.WindowType.Toplevel)
		{
			this.Build();

			_logger = logger;
			_logger.ClockAdded += loggerClockAdded_event;
			_logger.ClockRemoved += loggerClockRemoved_event;

			_logger.EntryAdded += loggerEntryAdded_event;
			_logger.EntryDeleted += loggerEntryDelete_event;

			CellRendererText cellRenderer = new CellRendererText();

			TreeViewColumn watchColumn = new TreeViewColumn();
			watchColumn.Title = "Clock";
			watchColumn.PackStart( cellRenderer, true );
			watchColumn.AddAttribute( cellRenderer, "text", 1 );

			TreeViewColumn descColumn = new TreeViewColumn();
			descColumn.Title = "Description";
			descColumn.PackStart( cellRenderer, true );
			descColumn.AddAttribute( cellRenderer, "text", 2 );

			TreeViewColumn timeColumn = new TreeViewColumn();
			timeColumn.Title = "Time";
			timeColumn.PackStart( cellRenderer, true );
			timeColumn.AddAttribute( cellRenderer, "text", 3 );

			logView.Model = _logStore = new ListStore(
				typeof( LogEntry ), typeof( string ), typeof( string ), typeof( string ));

			logView.Selection.Mode = SelectionMode.Multiple;

			logView.AppendColumn( watchColumn );
			logView.AppendColumn( descColumn );
			logView.AppendColumn( timeColumn );

			watchColumn.SortColumnId = 1;
			descColumn.SortColumnId = 2;
			timeColumn.SortColumnId = 3;

			watchColumn.SortIndicator = true;
			descColumn.SortIndicator = true;
			timeColumn.SortIndicator = true;

			_logStore.SetSortColumnId(3,SortType.Descending); 

			_logEntryRows = new Dictionary<LogEntry, TreeIter>();
			logView.Selection.Changed += logViewSelectionChanged_event;

			_logViewMenu = new Menu();
			_logViewMenu.Append(CopyAction.CreateMenuItem());
			_logViewMenu.Append(DeleteAction.CreateMenuItem());

			_manyClockWindows = new Dictionary<LoggingHandler, StopwatchWindow>();
		}

		public TimeLogger Logger { get { return _logger; } }
		public event FocusInEventHandler AnyClockWindowFocused;

		private ClockConfigWindow _configWindow;

		private Dictionary<LoggingHandler, StopwatchWindow> _manyClockWindows;
		private Dictionary<LogEntry, TreeIter> _logEntryRows;

		private Menu _logViewMenu;
		private ListStore _logStore;
		private readonly TimeLogger _logger;

		public StopwatchWindow ShowWatchWindow(LoggingHandler logHandler)
		{
			StopwatchWindow watchWindow = GetClockWindow(logHandler);

			watchWindow.Show( );
			watchWindow.Present( );

			return watchWindow;
		}

		public StopwatchWindow GetClockWindow(LoggingHandler logHandler)
		{
			StopwatchWindow clockWindow = _manyClockWindows [logHandler];

			if( clockWindow == null ) {
				clockWindow = new StopwatchWindow(logHandler);
				_manyClockWindows [logHandler] = clockWindow;

				clockWindow.FocusInEvent += clockWindowFocusIn_event;
				clockWindow.DestroyEvent += clockWindowDestroyed_event;
			}

			return clockWindow;
		}

		private List<LogEntry> GetSelectedLogs()
		{
			List<LogEntry> result = new List<LogEntry>();

			TreePath[] manyPaths = logView.Selection.GetSelectedRows();
			result.Capacity = manyPaths.Length;

			foreach( TreePath selectedPath in manyPaths ) {
				TreeIter iter;

				if( _logStore.GetIter( out iter, selectedPath ) ) {
					LogEntry selectedEntry = _logStore.GetValue( iter, 0 ) as LogEntry;

					if( selectedEntry != null )
						result.Add( selectedEntry );
				}
			}

			return result;
		}

		#region Dynamic events
		private void loggerClockAdded_event(object sender, LoggerClockEventArgs e)
		{
			_manyClockWindows.Add(e.LoggingHandler, null);
		}
		private void loggerClockRemoved_event(object sender, LoggerClockEventArgs e)
		{
			StopwatchWindow clockWindow;
			if( !_manyClockWindows.TryGetValue( e.LoggingHandler, out clockWindow ) )
				return;

			clockWindow.Destroy( );
			_manyClockWindows.Remove( e.LoggingHandler );
		}

		private void loggerEntryAdded_event(object sender, LoggingEventArgs e)
		{
			TreeIter entryRow = _logStore.AppendValues(
				e.Entry,
				e.Entry.ClockName,
				e.Entry.Description,
				e.Entry.Timestamp.ToString("HH:mm:ss.fff"));

			_logEntryRows.Add(e.Entry, entryRow);
		}
		private void loggerEntryDelete_event(object sender, LoggingEventArgs e)
		{
			TreeIter iter;
			if(!_logEntryRows.TryGetValue(e.Entry, out iter))return;

			_logStore.Remove(ref iter);

			_logEntryRows.Remove(e.Entry);
		}

		private void clockWindowFocusIn_event(object sender, FocusInEventArgs args)
		{
			if(AnyClockWindowFocused != null)
				AnyClockWindowFocused(sender, args);
		}

		private void clockWindowDestroyed_event(object sender, DestroyEventArgs args)
		{
			StopwatchWindow clockWindow = sender as StopwatchWindow;
			if( clockWindow != null ) {
				clockWindow.FocusInEvent -= clockWindowFocusIn_event;
			}
		}
		#endregion

		#region GUI events
		#region Log treeview events
		protected void logViewSelectionChanged_event (object sender, EventArgs e)
		{
			CopyAction.Sensitive = DeleteAction.Sensitive =
				!(logView.Selection.CountSelectedRows( ) == 0 );
		}

		protected void logViewPopup_event(object o, PopupMenuArgs args)
		{
			logViewShowContextMenu();
		}

		// This makes this damn event work like it damn should!
		[GLib.ConnectBefore]
		protected void logViewButtonPress_event(object o, ButtonPressEventArgs args)
		{
			if( args.Event.Button == 3 ) {

				TreePath selected_path;

				if( logView.GetPathAtPos( (int)args.Event.X, (int)args.Event.Y, out selected_path ))
				{
					if(logView.Selection.PathIsSelected(selected_path))
						args.RetVal=true;
				}

				logViewShowContextMenu( );
			}
		}

		private void logViewShowContextMenu()
		{
			_logViewMenu.ShowAll();
			_logViewMenu.Popup();
		}
		#endregion

		#region Menu events
		protected void exportAction_event(object sender, EventArgs e)
		{
			FileChooserDialog saveDialog = new FileChooserDialog(
				"Export Log Text", this, FileChooserAction.Save,
				Stock.Cancel, ResponseType.Cancel,
				Stock.Save, ResponseType.Accept,
				null);

			saveDialog.TransientFor = this;
			saveDialog.DoOverwriteConfirmation = true;

			if( saveDialog.Run( ) == (int)ResponseType.Accept) {
				_logger.ExportTo(saveDialog.Filename);
			}

			saveDialog.Destroy();
		}
		protected void quitAction_event (object sender, EventArgs e)
		{
			this.Destroy();
		}

		protected void selectAllAction_event (object sender, EventArgs e)
		{
			logView.Selection.SelectAll();
		}
		protected void copyAction_event(object sender, EventArgs e)
		{
			List<LogEntry> selectedEntries = GetSelectedLogs( );

			selectedEntries.Sort( );
			selectedEntries.Reverse( );

			if( selectedEntries.Count == 0 )
				return;

			string copyData = "";

			for( int i = 0; i < selectedEntries.Count; i++ ) {
				copyData += 
					( i > 0? "\n" : "" )
					+ selectedEntries [i];
			}

			logView.GetClipboard( Gdk.Selection.Clipboard ).Text = copyData;
		}
		protected void deleteAction_event(object sender, EventArgs e)
		{
			List<LogEntry> selectedEntries = GetSelectedLogs( );

			foreach( LogEntry entry in selectedEntries )
				_logger.RemoveEntry( entry );
		}
		
		protected void editStopwatches_event(object sender, EventArgs e)
		{
			if( _configWindow == null ) {
				_configWindow = new ClockConfigWindow(this);
			}

			_configWindow.Show( );
			_configWindow.Present( );
		}

		protected void aboutAction_event(object sender, EventArgs e)
		{
			AboutDialog about = new AboutDialog();

			about.Run();
			about.Destroy( );
		}
		#endregion

		protected override void OnDestroyed()
		{
			Application.Quit( );

			base.OnDestroyed( );
		}
		#endregion
	}
}