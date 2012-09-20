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
using Mono.Unix;

namespace Chrono
{
	public partial class LoggerWindow : Window
	{
		public LoggerWindow(Program program) : 
				base(Gtk.WindowType.Toplevel)
		{
			if( program == null )
				throw new ArgumentNullException("program");

			this.Build( );

			dockExpanderVBox = new VBox();
			vbox1.Add( dockExpanderVBox );

			Box.BoxChild vboxChild = ( Box.BoxChild )vbox1 [dockExpanderVBox];
			vboxChild.Expand = false;
			vboxChild.Fill = false;

			dockExpanderVBox.Show( );

			logViewMenu = new Menu();
			logViewMenu.Append( copyAction.CreateMenuItem( ) );
			logViewMenu.Append( deleteAction.CreateMenuItem( ) );

			Program = program;

			Logger.EntryAdded += loggerEntryAdded_event;
			Logger.EntryDeleted += loggerEntryDelete_event;

			CellRendererText cellRenderer = new CellRendererText();

			TreeViewColumn clockColumn = new TreeViewColumn();
			clockColumn.Title = Catalog.GetString("Source");
			clockColumn.PackStart( cellRenderer, true );
			clockColumn.AddAttribute( cellRenderer, "text", 1 );

			TreeViewColumn descColumn = new TreeViewColumn();
			descColumn.Title = Catalog.GetString("Event");
			descColumn.PackStart( cellRenderer, true );
			descColumn.AddAttribute( cellRenderer, "text", 2 );

			TreeViewColumn timeColumn = new TreeViewColumn();
			timeColumn.Title = Catalog.GetString("Occurred");
			timeColumn.PackStart( cellRenderer, true );
			timeColumn.AddAttribute( cellRenderer, "text", 3 );

			TreeViewColumn[] manyColumns = new TreeViewColumn[] 
			{clockColumn, descColumn, timeColumn};

			foreach(TreeViewColumn column in manyColumns)
			{
				logView.AppendColumn(column);

				column.Resizable = true;
				column.Reorderable = true;
				column.SortIndicator = true;
			}

			logView.Model = _logStore = new ListStore(
				typeof( LogEntry ), typeof( string ), typeof( string ), typeof( string ));

			logView.Selection.Mode = SelectionMode.Multiple;

			clockColumn.SortColumnId = 1;
			descColumn.SortColumnId = 2;
			timeColumn.SortColumnId = 3;

			clockColumn.MinWidth = 40;
			descColumn.MinWidth = 120;
			timeColumn.MinWidth = 40;

			_logStore.SetSortColumnId(3, SortType.Descending); 

			_logEntryRows = new Dictionary<LogEntry, TreeIter>();
			logView.Selection.Changed += logViewSelectionChanged_event;

			Program.History.Changed += historyChanged_event;
		}

		public Program Program { get; private set; }
		public TimeLogger Logger { get { return Program.TimeLogger; } }

		private Dictionary<LogEntry, TreeIter> _logEntryRows;

		private Menu logViewMenu;
		private ListStore _logStore;

		private VBox dockExpanderVBox;

		private List<LogEntry> GetSelectedLogs()
		{
			TreePath[] manyPaths = logView.Selection.GetSelectedRows();
			List<LogEntry> result = new List<LogEntry>(manyPaths.Length);

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

		public Expander CreateDockExpander()
		{
			Expander result = new Expander("Docked thing");
			dockExpanderVBox.Add(result);
			Box.BoxChild resultBoxChild = (Box.BoxChild)dockExpanderVBox[result];

			resultBoxChild.Expand = false;
			resultBoxChild.Fill = false;

			result.CanFocus = true;

			return result;
		}

		#region Dynamic events
		private void loggerEntryAdded_event(object sender, LoggingEventArgs e)
		{
			foreach( LogEntry entry in e.Entries ) {
				TreeIter entryRow = _logStore.AppendValues(
				entry,
				entry.ClockName,
				entry.Description,
				entry.Timestamp.ToString("HH:mm:ss.fff"));

				_logEntryRows.Add(entry, entryRow);
			}
		}

		private void loggerEntryDelete_event(object sender, LoggingEventArgs e)
		{
			foreach( LogEntry entry in e.Entries ) {
				TreeIter iter;
				if( !_logEntryRows.TryGetValue( entry, out iter ) )
					continue;

				_logStore.Remove( ref iter );

				_logEntryRows.Remove( entry );
			}
		}

		private void historyChanged_event(object sender, HistoryChangedArgs e)
		{
			if( undoAction.Sensitive = e.History.CanUndo )
				undoAction.Label = e.History.Undoable.UndoText;
			else undoAction.Label = Catalog.GetString("Undo");

			if( redoAction.Sensitive = e.History.CanRedo )
				redoAction.Label = e.History.Redoable.RedoText;
			else redoAction.Label = Catalog.GetString("Redo");
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
			showLogViewContextMenu();
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

				showLogViewContextMenu( );
			}
		}

		private void showLogViewContextMenu()
		{
			logViewMenu.Show();
			logViewMenu.Popup();
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

		protected void undo_event (object sender, EventArgs e)
		{
			Program.History.Undo();
		}
		protected void redo_event (object sender, EventArgs e)
		{
			Program.History.Redo();
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
				Logger.DeleteEntry( entry );

			Program.History.Register(new DeleteLogsDoable(Logger, selectedEntries.ToArray()));
		}
		protected void selectAllAction_event (object sender, EventArgs e)
		{
			logView.Selection.SelectAll();
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