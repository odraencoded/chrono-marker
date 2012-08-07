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
		public LoggerWindow(Logger logger) : 
				base(Gtk.WindowType.Toplevel)
		{
			this.Build( );

			this.Logger = logger;
			Logger.WatchAdded += loggerWatchAdded_event;

			logger.EntryAdded += loggerEntryAdded_event;
			logger.EntryDeleted += loggerEntryDelete_event;

			CellRendererText cellRenderer = new CellRendererText();

			TreeViewColumn watchColumn = new TreeViewColumn();
			watchColumn.Title = "Watch";
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

			logView.Model = logStore = new ListStore(
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

			timeColumn.SortOrder = SortType.Descending;

			logEntryRows = new Dictionary<LogEntry, TreeIter>();
		}

		private Dictionary<LogEntry, TreeIter> logEntryRows;

		private ListStore logStore;
		public Logger Logger {get; private set;}
		private Watch currentWatch;
		private StopwatchWindow stopwatchWindow;

		List<LogEntry> GetSelectedLogs()
		{
			List<LogEntry> result = new List<LogEntry>();

			TreePath[] manyPaths = logView.Selection.GetSelectedRows();
			result.Capacity = manyPaths.Length;

			foreach( TreePath selectedPath in manyPaths ) {
				TreeIter iter;

				if( logStore.GetIter( out iter, selectedPath ) ) {
					LogEntry selectedEntry = logStore.GetValue( iter, 0 ) as LogEntry;

					if( selectedEntry != null )
						result.Add( selectedEntry );
				}
			}

			return result;
		}

		public void ShowWatch()
		{
			if( stopwatchWindow == null ) {
				stopwatchWindow = new StopwatchWindow(currentWatch);
				stopwatchWindow.DeleteEvent +=  watchWindowClosed_event;
			}
			stopwatchWindow.Show();
		}

		#region Dynamic events
		private void loggerWatchAdded_event(object sender, LoggerWatchEventArgs e)
		{
			currentWatch = e.Watch;
			stopwatchWindow = new StopwatchWindow(currentWatch);
			stopwatchWindow.DeleteEvent += watchWindowClosed_event;
		}

		private void loggerEntryAdded_event(object sender, LoggingEventArgs e)
		{
			TreeIter entryRow = logStore.AppendValues(
				e.Entry,
				e.Entry.ClockName,
				e.Entry.Description,
				e.Entry.Timestamp.ToString("HH:mm:ss.fff"));

			logEntryRows.Add(e.Entry, entryRow);
		}

		private void loggerEntryDelete_event(object sender, LoggingEventArgs e)
		{
			TreeIter iter;
			if(!logEntryRows.TryGetValue(e.Entry, out iter))return;

			logStore.Remove(ref iter);

			logEntryRows.Remove(e.Entry);
		}

		private void watchWindowClosed_event(object sender, EventArgs e)
		{
			stopwatchWindow = null;
		}
		#endregion

		#region GUI events
		protected void viewStopwatch_event(object sender, EventArgs e)
		{
			ShowWatch();
		}

		protected void copyAction_event(object sender, EventArgs e)
		{
			List<LogEntry> selectedEntries = GetSelectedLogs();

			selectedEntries.Sort();
			selectedEntries.Reverse();

			if( selectedEntries.Count == 0 )
				return;

			string copyData = "";

			for( int i = 0; i<selectedEntries.Count; i++ ) {
				copyData += 
					( i > 0? Environment.NewLine : "" )
					+ selectedEntries [i];
			}

			logView.GetClipboard( Gdk.Selection.Clipboard ).Text = copyData;
		}

		protected void deleteAction_event(object sender, EventArgs e)
		{
			List<LogEntry> selectedEntries = GetSelectedLogs( );

			foreach( LogEntry entry in selectedEntries )
				Logger.RemoveEntry( entry );
		}

		protected void selectAllAction_event (object sender, EventArgs e)
		{
			logView.Selection.SelectAll();
		}

		protected void helpAbout_event(object sender, EventArgs e)
		{
			AboutDialog about = new AboutDialog();

			about.Show();
		}

		protected void exportAction_event(object sender, EventArgs e)
		{
			FileChooserDialog dialog = new FileChooserDialog(
				"Export Log", this, FileChooserAction.Save,
				Stock.Cancel, ResponseType.Cancel,
				Stock.Save, ResponseType.Accept,
				null);

			dialog.DoOverwriteConfirmation = true;
			FileFilter txtFilter = new FileFilter();
			txtFilter.AddPattern( "*.txt" );
			txtFilter.Name = "Text file";
			dialog.AddFilter( txtFilter );

			if( dialog.Run( ) == (int)ResponseType.Accept) {
				Logger.ExportTo(dialog.Filename);
			}

			dialog.Destroy();
		}

		protected void editMenuOpen_event(object sender, EventArgs e)
		{
			CopyAction.Sensitive = DeleteAction.Sensitive =
				!(logView.Selection.CountSelectedRows( ) == 0 );
		}

		protected void actionQuit_event (object sender, EventArgs e)
		{
			this.Hide();
		}

		protected void windowHidden_event (object sender, EventArgs e)
		{
			Console.Write("Cake");
			Application.Quit();
		}
		#endregion
	}
}