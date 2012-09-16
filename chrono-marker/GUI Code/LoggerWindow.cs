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
		public LoggerWindow(Program program) : 
				base(Gtk.WindowType.Toplevel)
		{
			if(program == null)
				throw new ArgumentNullException("program");

			this.Build();

			Program = program;

			Logger.EntryAdded += loggerEntryAdded_event;
			Logger.EntryDeleted += loggerEntryDelete_event;

			CellRendererText cellRenderer = new CellRendererText();

			TreeViewColumn clockColumn = new TreeViewColumn();
			clockColumn.Title = "Source";
			clockColumn.PackStart( cellRenderer, true );
			clockColumn.AddAttribute( cellRenderer, "text", 1 );

			TreeViewColumn descColumn = new TreeViewColumn();
			descColumn.Title = "Event";
			descColumn.PackStart( cellRenderer, true );
			descColumn.AddAttribute( cellRenderer, "text", 2 );

			TreeViewColumn timeColumn = new TreeViewColumn();
			timeColumn.Title = "Occurred";
			timeColumn.PackStart( cellRenderer, true );
			timeColumn.AddAttribute( cellRenderer, "text", 3 );

			logView.Model = _logStore = new ListStore(
				typeof( LogEntry ), typeof( string ), typeof( string ), typeof( string ));

			logView.Selection.Mode = SelectionMode.Multiple;

			logView.AppendColumn( clockColumn );
			logView.AppendColumn( descColumn );
			logView.AppendColumn( timeColumn );

			clockColumn.SortColumnId = 1;
			descColumn.SortColumnId = 2;
			timeColumn.SortColumnId = 3;

			clockColumn.SortIndicator = true;
			descColumn.SortIndicator = true;
			timeColumn.SortIndicator = true;

			clockColumn.Resizable = true;
			descColumn.Resizable = true;
			timeColumn.Resizable = true;

			_logStore.SetSortColumnId(3,SortType.Descending); 

			_logEntryRows = new Dictionary<LogEntry, TreeIter>();
			logView.Selection.Changed += logViewSelectionChanged_event;

			_logViewMenu = new Menu();
			_logViewMenu.Append(copyAction.CreateMenuItem());
			_logViewMenu.Append(deleteAction.CreateMenuItem());
		}

		public Program Program { get; private set; }
		public TimeLogger Logger { get { return Program.TimeLogger; } }

		private Dictionary<LogEntry, TreeIter> _logEntryRows;

		private Menu _logViewMenu;
		private ListStore _logStore;

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
		#endregion

		#region GUI events
		#region Log treeview events
		protected void logViewSelectionChanged_event (object sender, EventArgs e)
		{
			copyAction.Sensitive = deleteAction.Sensitive =
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
				Logger.ExportTo(saveDialog.Filename);
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
				Logger.RemoveEntry( entry );
		}
		
		protected void editStopwatches_event(object sender, EventArgs e)
		{
			Program.ClockPropertiesWindow.Present();
		}

		protected void editPreferences_click (object sender, EventArgs e)
		{
			Program.PreferencesWindow.Present();
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