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

			this.Program = program;

			// Setting up GUI
			// Main build
			this.Build();

			// Log view columns and setup
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

			_logListStore = new ListStore(
				typeof( LogEntry ), typeof( string ), typeof( string ), typeof( string ));

			logView.Model = _logListStore;
			
			logView.Selection.Mode = SelectionMode.Multiple;
			
			clockColumn.SortColumnId = 1;
			descColumn.SortColumnId = 2;
			timeColumn.SortColumnId = 3;
			
			clockColumn.MinWidth = 40;
			descColumn.MinWidth = 120;
			timeColumn.MinWidth = 40;
			
			_logListStore.SetSortColumnId(3, SortType.Descending); 
			_logEntryRows = new Dictionary<LogEntry, TreeIter>();

			// Log view popup menu
			logViewMenu = new Menu();
			logViewMenu.Append( copyAction.CreateMenuItem( ) );
			logViewMenu.Append( deleteAction.CreateMenuItem( ) );

			// Setup events
			Logger.EntryAdded += loggerEntryAdded_event;
			Logger.EntryDeleted += loggerEntryDelete_event;
			Program.History.Changed += historyChanged_event;
			logView.Selection.Changed += logViewSelectionChanged_event;

			// Load exporters
			Gtk.MenuItem exportMenuItem = new MenuItem("Export");
			Gtk.Menu exportMenu = new Menu();
			exportMenuItem.Submenu = exportMenu;
			((menubar1.Children[0] as MenuItem).Submenu as Menu).Insert(exportMenuItem, 0);

			foreach(Files.ILogExporter exporter in program.LogExporters) {
				MenuItem exporterMenuItem = new MenuItem(exporter.Label);
				exporterMenuItem.Activated += exportAction_event;
				exporterMenuItem.Data["exporter"] = exporter;

				exportMenu.Append(exporterMenuItem);
			}
			exportMenuItem.ShowAll();
			
			// Load log entries
			List<LogEntry> manyLogEntries = Logger.GetLogList();

			foreach(LogEntry logEntry in manyLogEntries)
				AddLogEntry(logEntry);

			RefreshTexts();
		}

		public Program Program { get; private set; }
		public TimeLogger Logger { get { return Program.TimeLogger; } }

		private Dictionary<LogEntry, TreeIter> _logEntryRows;

		private Menu logViewMenu;
		private ListStore _logListStore;

		public void RefreshTexts()
		{
			// Menu bar
			FileAction.ShortLabel = FileAction.Label = Catalog.GetString("File");
			EditAction.ShortLabel = EditAction.Label = Catalog.GetString("Edit");
			ViewAction.ShortLabel = ViewAction.Label = Catalog.GetString("View");
			HelpAction.ShortLabel = HelpAction.Label = Catalog.GetString("Help");

			// File menu
			ExportAction.ShortLabel = ExportAction.Label = Catalog.GetString("Export...");
			quitAction.ShortLabel = quitAction.Label = Catalog.GetString("Quit");

			// Edit menu
			undoAction.ShortLabel = Catalog.GetString("Undo");
			redoAction.ShortLabel = Catalog.GetString("Redo");
			RefreshHistoryTexts();

			copyAction.ShortLabel = copyAction.Label = Catalog.GetString("Copy");
			deleteAction.ShortLabel = deleteAction.Label = Catalog.GetString("Delete");

			SelectAllAction.ShortLabel = SelectAllAction.Label = Catalog.GetString("Select All");
			stopwatchesAction.ShortLabel = stopwatchesAction.Label = Catalog.GetString("Stopwatches...");
			preferencesAction.ShortLabel = preferencesAction.Label = Catalog.GetString("Preferences...");

			// View menu
			KeepAboveAction.ShortLabel = KeepAboveAction.Label = Catalog.GetString("Keep Above");

			// Help menu
			HelpAction.ShortLabel = HelpAction.Label = Catalog.GetString("Help");

			RefreshLogCount();
		}

		public void RefreshHistoryTexts()
		{
			if( undoAction.Sensitive = Program.History.CanUndo )
				undoAction.Label = Program.History.Undoable.UndoText;
			else undoAction.Label = Catalog.GetString("Undo");
			
			if( redoAction.Sensitive = Program.History.CanRedo )
				redoAction.Label = Program.History.Redoable.RedoText;
			else redoAction.Label = Catalog.GetString("Redo");
		}

		private void RefreshLogCount()
		{
			string logCountText;

			int logCount = Logger.EntryCount;

			if( logCount > 0 ) {
				int selectedLogCount = logView.Selection.CountSelectedRows( );
				
				logCountText = String.Format(Catalog.GetPluralString(
					"{0} Log", "{0} Logs", logCount), logCount);
				if(selectedLogCount > 0)
				{
					logCountText = String.Format(Catalog.GetPluralString(
						"{0} Selected", "{0} Selected", selectedLogCount), selectedLogCount)
						+ " : " + logCountText;
				}
			}
			else 
			{
				logCountText = Catalog.GetString("No Logs");
			}

			logCountLabel.Text = logCountText;
		}

		private List<LogEntry> GetSelectedLogs()
		{
			TreePath[] manyPaths = logView.Selection.GetSelectedRows();
			List<LogEntry> result = new List<LogEntry>(manyPaths.Length);

			foreach( TreePath selectedPath in manyPaths ) {
				TreeIter iter;

				if( _logListStore.GetIter( out iter, selectedPath ) ) {
					LogEntry selectedEntry = _logListStore.GetValue( iter, 0 ) as LogEntry;

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

		private void AddLogEntry(LogEntry entry)
		{
			TreeIter entryRow = _logListStore.AppendValues(
				entry,
				entry.ClockName,
				entry.Description,
				entry.Timestamp.ToString("HH:mm:ss.fff"));
			
			_logEntryRows.Add(entry, entryRow);
		}

		private void RemoveLogEntry(LogEntry entry)
		{
			TreeIter iter;
			if( !_logEntryRows.TryGetValue( entry, out iter ) )
				return;
			
			_logListStore.Remove( ref iter );
			_logEntryRows.Remove( entry );
		}

		#region Dynamic events
		private void loggerEntryAdded_event(object sender, LoggingEventArgs e)
		{
			foreach( LogEntry entry in e.Entries ) {
				AddLogEntry(entry);
			}

			RefreshLogCount();
		}

		private void loggerEntryDelete_event(object sender, LoggingEventArgs e)
		{
			foreach( LogEntry entry in e.Entries ) {
				RemoveLogEntry(entry);
			}

			RefreshLogCount();
		}

		private void historyChanged_event(object sender, HistoryChangedArgs e)
		{
			RefreshHistoryTexts();
		}
		#endregion

		#region GUI events
		#region Log treeview events
		protected void logViewSelectionChanged_event (object sender, EventArgs e)
		{
			copyAction.Sensitive = deleteAction.Sensitive =
				!(logView.Selection.CountSelectedRows( ) == 0 );

			RefreshLogCount();
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
			// Tries to get the data to export
			Gtk.Widget itemSender = sender as Gtk.Widget;
			if( itemSender == null ) {
				Console.WriteLine( "Could not convert export widget!" );
				return;
			}

			Files.ILogExporter exporter = itemSender.Data ["exporter"] as Files.ILogExporter;
			if( itemSender == null ) {
				Console.WriteLine("Could not get exporter from widget!");
				return;
			}


			string dialogTitle = Catalog.GetString(exporter.Title);

			FileChooserDialog saveDialog = new FileChooserDialog(
				dialogTitle, this, FileChooserAction.Save,
				Stock.Cancel, ResponseType.Cancel,
				Stock.Save, ResponseType.Accept,
				null);

			saveDialog.Modal = true;
			saveDialog.TransientFor = this;
			saveDialog.DoOverwriteConfirmation = true;

			if(saveDialog.Run() == (int)ResponseType.Accept) {
				Logger.Sort();
				exporter.Export(Logger.GetLogs(false), saveDialog.Filename);
			}

			saveDialog.Destroy();
		}

		protected void quitAction_event (object sender, EventArgs e)
		{
			this.Destroy();
		}

		protected void editUndo_event (object sender, EventArgs e)
		{
			Program.History.Undo();
		}
		protected void editRedo_event (object sender, EventArgs e)
		{
			Program.History.Redo();
		}
		protected void editCopy_event(object sender, EventArgs e)
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
		protected void editDelete_event(object sender, EventArgs e)
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

		protected void keepAbove_event(object sender, EventArgs e)
		{
			KeepAbove = KeepAboveAction.Active;
		}
		protected void stopwatches_event(object sender, EventArgs e)
		{
			Program.ClockPropertiesWindow.Present();
		}

		protected void preferences_event (object sender, EventArgs e)
		{
			Program.PreferencesWindow.Present();
		}

		protected void aboutAction_event(object sender, EventArgs e)
		{
			AboutDialog about = new AboutDialog();

			about.Run();
			about.Destroy( );
		}

		protected void shown_event(object sender, EventArgs e)
		{
			KeepAbove = KeepAboveAction.Active;
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