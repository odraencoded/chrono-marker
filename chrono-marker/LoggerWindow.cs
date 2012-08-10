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
			this.Build();

			this.Logger = logger;
			Logger.WatchAdded += loggerWatchAdded_event;
			Logger.WatchRemoved += loggerWatchRemoved_event;

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

			logStore.SetSortColumnId(3,SortType.Descending); 

			logEntryRows = new Dictionary<LogEntry, TreeIter>();
			logView.Selection.Changed += logViewSelectionChanged_event;

			logViewMenu = new Menu();
			logViewMenu.Append(CopyAction.CreateMenuItem());
			logViewMenu.Append(DeleteAction.CreateMenuItem());

			manyWatchWindows = new Dictionary<Watch, StopwatchWindow>();
		}

		public Logger Logger {get; private set;}

		public void ShowWatch(LoggingHandler logHandler)
		{
			StopwatchWindow watchWindow = manyWatchWindows [logHandler.Watch];

			if( watchWindow == null ) {
				watchWindow = new StopwatchWindow(logHandler);
				manyWatchWindows [logHandler.Watch] = watchWindow;

				watchWindow.FocusInEvent += watchWindowFocus_event;
			}

			watchWindow.Show( );
			watchWindow.Present( );
		}

		public event FocusInEventHandler WatchWindowFocused;

		private WatchConfigWindow configWindow;

		private Dictionary<Watch, StopwatchWindow> manyWatchWindows;
		private Dictionary<LogEntry, TreeIter> logEntryRows;

		private Menu logViewMenu;
		private ListStore logStore;

		private List<LogEntry> GetSelectedLogs()
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

		#region Dynamic events
		private void loggerWatchAdded_event(object sender, LoggerWatchEventArgs e)
		{
			manyWatchWindows.Add(e.LoggingHandler.Watch, null);
		}
		private void loggerWatchRemoved_event(object sender, LoggerWatchEventArgs e)
		{
			StopwatchWindow watchWindow;
			Watch watch = e.LoggingHandler.Watch;
			if( !manyWatchWindows.TryGetValue( watch, out watchWindow ) )
				return;

			watchWindow.Destroy( );
			manyWatchWindows.Remove( watch );
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

		private void watchWindowFocus_event(object sender, FocusInEventArgs args)
		{
			if(WatchWindowFocused !=null)
				WatchWindowFocused(sender, args);
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
			logViewMenu.ShowAll();
			logViewMenu.Popup();
		}
		#endregion

		#region Menu events
		protected void exportAction_event(object sender, EventArgs e)
		{
			FileChooserDialog dialog = new FileChooserDialog(
				"Export Log Text", this, FileChooserAction.Save,
				Stock.Cancel, ResponseType.Cancel,
				Stock.Save, ResponseType.Accept,
				null);

			dialog.DoOverwriteConfirmation = true;

			if( dialog.Run( ) == (int)ResponseType.Accept) {
				Logger.ExportTo(dialog.Filename);
			}

			dialog.Destroy();
		}
		protected void actionQuit_event (object sender, EventArgs e)
		{
			this.Destroy();
		}

		protected void selectAllAction_event (object sender, EventArgs e)
		{
			logView.Selection.SelectAll();
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
		
		protected void editStopwatches_event(object sender, EventArgs e)
		{
			if( configWindow == null ) {
				configWindow = new WatchConfigWindow(this);
			}

			configWindow.Show();
			configWindow.Present();
		}

		protected void helpAbout_event(object sender, EventArgs e)
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