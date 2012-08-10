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
	public partial class WatchConfigWindow : Gtk.Window
	{
		public WatchConfigWindow(LoggerWindow loggerWindow) : 
				base(Gtk.WindowType.Toplevel)
		{
			this.Build( );

			this.loggerWindow = loggerWindow;
			this.Logger = loggerWindow.Logger;

			this.Logger.WatchAdded += loggerWatchAdded_event;
			this.Logger.WatchRemoved += loggerWatchRemoved_event;

			loggerWindow.WatchWindowFocused += watchWindowFocused_event;

			comboNameIters = new Dictionary<LoggingHandler, TreeIter>();

			LoggingHandler[] manyHandlers = Logger.Handlers;

			comboEntryList = watchNameCombo.Model as ListStore;

			foreach( LoggingHandler handler in manyHandlers ) {
				TreeIter iter =  comboEntryList.AppendValues(handler.Name);

				comboNameIters.Add(handler, iter);
			}

			watchNameCombo.Entry.Changed+= watchNameEntrychanged_event;

			if(manyHandlers.Length > 0)
			{
				currentHandler=  manyHandlers[0];
				watchNameCombo.Active=0;
			}

			RefreshControls();
		}

		public Logger Logger { get; private set; }
		private LoggerWindow loggerWindow;
		private LoggingHandler currentHandler;

		private Dictionary<LoggingHandler, TreeIter> comboNameIters;
		private ListStore comboEntryList;

		#region Dynamic events
		private void loggerWatchAdded_event(object sender, LoggerWatchEventArgs e)
		{
			TreeIter iter = comboEntryList.AppendValues(e.LoggingHandler.Name);

			comboNameIters.Add(e.LoggingHandler, iter);
		}

		private void loggerWatchRemoved_event(object sender, LoggerWatchEventArgs e)
		{
			TreeIter removedIter = comboNameIters[e.LoggingHandler];

			comboEntryList.Remove(ref removedIter);

			if( currentHandler == e.LoggingHandler ) {
				currentHandler = null;

				TreeIter firstIter;
				if(comboEntryList.GetIterFirst(out firstIter))
					watchNameCombo.Entry.Text = comboEntryList.GetValue(firstIter, 0) as String;

				RefreshControls( );
			}
		}

		private void watchWindowFocused_event(object sender, FocusInEventArgs args)
		{
			StopwatchWindow watchWindow = ( sender as StopwatchWindow );

			if( watchWindow == null )
				return;

			watchNameCombo.Entry.Text = watchWindow.LogHandler.Name;
		}
		#endregion

		#region GUI events
		private void RefreshControls()
		{
			bool valid = currentHandler != null;

			detailBook.Sensitive = valid;

			if( valid ) {
				logStartsCheck.Active = currentHandler.LogStarts;
				logStopsCheck.Active = currentHandler.LogStops;

				if( currentHandler.Watch.Speed >= 0 )
					radioForward.Active = true;
				else
					radioBackward.Active = true;

				speedEntry.Value = Math.Abs( currentHandler.Watch.Speed );
			} else {
				logStartsCheck.Active = false;
				logStopsCheck.Active = false;

				radioForward.Active=true;
				speedEntry.Value=1.0;
			}
		}

		#region Watch name handling
		protected void watchNameEntrychanged_event(object sender, EventArgs e)
		{
			string watchName = watchNameCombo.Entry.Text;

			if( ! Logger.TryGetHandler( watchName, out currentHandler ) ) {
				currentHandler=null;
			}
			RefreshControls();
		}

		protected void newBtn_event(object sender, EventArgs e)
		{
			string watchName = watchNameCombo.Entry.Text;

			if( ! Logger.HasWatch( watchName)) {
				currentHandler=Logger.AddWatch(new Watch(), watchName);
				RefreshControls();
				loggerWindow.ShowWatch(currentHandler);
			}
			else 
			{
				MessageDialog oopz = new MessageDialog(
					this, DialogFlags.DestroyWithParent,
					MessageType.Error, ButtonsType.Close,
					"A watch with this name already exists. Please use another name.");

				oopz.Title = "Watch already exists";
				oopz.Run();

				oopz.Destroy();
			}
		}
		protected void deleteWatch_event(object sender, EventArgs e)
		{
			if( currentHandler == null )
				return;

			Logger.RemoveWatch( currentHandler.Name );
		}

		protected void showWatch_event(object sender, EventArgs e)
		{
			if( currentHandler == null )
				return;
			loggerWindow.ShowWatch( currentHandler );
		}
		#endregion

		#region Config stuff
		protected void logStartsToggled_event(object sender, EventArgs e)
		{
			if( currentHandler != null )
				currentHandler.LogStarts = logStartsCheck.Active;
		}
		protected void logStopsToggled_event(object sender, EventArgs e)
		{
			if( currentHandler != null )
				currentHandler.LogStops = logStopsCheck.Active;
		}

		protected void setSpeedClicked_event(object sender, EventArgs e)
		{
			if( currentHandler != null ) {
				double newSpeed = speedEntry.Value;
				if( radioBackward.Active )
					newSpeed *= -1;

				currentHandler.Watch.ChangeSpeed( newSpeed );
			}
		}
		#endregion
		protected void closeWindow_event(object sender, EventArgs e)
		{
			Hide();
		}

		protected void windowDelete_event(object o, DeleteEventArgs args)
		{
			Hide();

			// Prevents from destroying
			args.RetVal=true;
		}
		#endregion

		protected override void OnDestroyed()
		{
			loggerWindow.WatchWindowFocused -= watchWindowFocused_event;

			base.OnDestroyed();
		}
	}
}
