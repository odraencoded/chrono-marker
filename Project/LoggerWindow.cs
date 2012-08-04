/* Copyright (C) 2012 Leonardo Augusto Pereira
 * 
 * This file is part of Chrono Marker 
 * 
 * Chrono Marker is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Chrono Marker is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Chrono Marker.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace Chrono
{
    partial class LoggerWindow : Form
    {
        public LoggerWindow(Logger logger)
        {
            InitializeComponent();

            this.Logger = logger;

            logger.WatchAdded += logger_WatchAdded;
            logger.WatchRemoved += logger_WatchRemoved;

            logger.EntryAdded += logger_EntryAdded;
            logger.EntryDeleted += logger_EntryDeleted;

            logComparer = new ListViewComparer(2, SortOrder.Descending);
            logView.ListViewItemSorter = logComparer;

            logViewEntries = new Dictionary<LogEntry, ListViewItem>();
            watchDialogs = new Dictionary<Watch, StopwatchDialog>();

            trayIcon.Icon = Icon = Properties.Resources.Icon;
        }

        public Logger Logger { get; private set; }

        public void DisplayStopwatch()
        {
            if (currentDialog == null)
            {
                currentDialog = new StopwatchDialog(currentWatch);
                currentDialog.FormClosed += currentDialog_FormClosed;
            }

            currentDialog.Show();
            currentDialog.WindowState = FormWindowState.Normal;
            currentDialog.BringToFront();
        }
        private Watch currentWatch;
        private StopwatchDialog currentDialog;
        private ListViewComparer logComparer;
        private Dictionary<LogEntry, ListViewItem> logViewEntries;
        private Dictionary<Watch, StopwatchDialog> watchDialogs;

        private void deleteSelectedLogs()
        {
            foreach (ListViewItem selectedItem in logView.SelectedItems)
                Logger.RemoveEntry(selectedItem.Tag as LogEntry);
        }

        #region Dynamic events
        void currentDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            currentDialog = null;
            if(this.WindowState == FormWindowState.Minimized)
                trayIcon.ShowBalloonTip(5000, "Chrono Marker is here!",
                        "Right click if you want to open or close it", ToolTipIcon.Info);
        }

        void logger_WatchAdded(object sender, LoggerWatchEventArgs e)
        {
            /*
            if (watchDialogs.ContainsKey(e.Watch)) return;

            watchDialogs.Add(e.Watch, new StopwatchDialog(e.Watch));*/
            currentWatch = e.Watch;
            //currentDialog = new StopwatchDialog(e.Watch);
        }
        void logger_WatchRemoved(object sender, LoggerWatchEventArgs e)
        {
            /*
            if (!watchDialogs.ContainsKey(e.Watch)) return;

            watchDialogs[e.Watch].Close();

            watchDialogs.Remove(e.Watch);
             */
        }

        void logger_EntryAdded(object sender, LoggingEventArgs e)
        {
            LogEntry entry = e.Entry;

            if (logViewEntries.ContainsKey(entry)) return; // No action needed

            ListViewItem newItem = new ListViewItem();

            newItem.Text = entry.ClockName; // First item is clock name
            newItem.SubItems.Add(entry.Description); // Then desc
            newItem.SubItems.Add(entry.Timestamp.ToString("HH:mm:ss.fff")); // Then this

            newItem.Tag = entry;

            logView.Items.Add(newItem);
            logViewEntries.Add(entry, newItem);
        }
        void logger_EntryDeleted(object sender, LoggingEventArgs e)
        {
            LogEntry entry = e.Entry;
            ListViewItem logViewItem;
            if (!logViewEntries.TryGetValue(entry, out logViewItem)) return; // No action needed

            logView.Items.Remove(logViewItem);
            logViewEntries.Remove(entry);
        }
        #endregion

        #region GUI events

        #region Window display handling
        private void trayShowLogger(object sender, EventArgs e)
        {
            if (this.Visible) this.Hide();
            else
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.BringToFront();
            }
        }
        private void showWatchTrayItem_Click(object sender, EventArgs e)
        {
            DisplayStopwatch();
        }
        private void exitMenuItem_click(object sender, EventArgs e)
        {
            Close();
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutDialog().ShowDialog();
        }
        #endregion

        #region Logs
        private void logView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (logComparer.Column == e.Column)
            {
                if (logComparer.Sorting == SortOrder.Descending)
                    logComparer.Sorting = SortOrder.Ascending;
                else logComparer.Sorting = SortOrder.Descending;
            }
            else
            {
                logComparer.Column = e.Column;

                if (logComparer.Sorting == SortOrder.None)
                    logComparer.Sorting = SortOrder.Descending;
            }

            logView.Sort();
        }

        private void copyLogs_event(object sender, EventArgs e)
        {
            if (logView.SelectedItems.Count == 0) return;

            string copyText = "";
            bool multiLine = false;

            foreach (ListViewItem selectedItem in logView.SelectedItems)
            {
                if (multiLine) copyText += Environment.NewLine;
                copyText += "[" + selectedItem.SubItems[2].Text + "] ";
                copyText += selectedItem.SubItems[0].Text + " - ";
                copyText += selectedItem.SubItems[1].Text;

                multiLine = true;
            }

            Clipboard.SetText(copyText, TextDataFormat.UnicodeText);
        }

        private void deleteSelectedLogs_event(object sender, EventArgs e)
        {
            deleteSelectedLogs();
        }

        private void selectAllLogs_event(object sender, EventArgs e)
        {

            logView.SelectedIndices.Clear();

            for (int i = 0; i < logView.Items.Count; i++)
                logView.SelectedIndices.Add(i);
        }

        private void logMenu_Opening(object sender, CancelEventArgs e)
        {
            copyContextMenuItem.Enabled =
                deleteContextMenuItem.Enabled = (logView.SelectedItems.Count > 0);
        }
        private void logToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            deleteMenuItem.Enabled =
                copyMenuItem.Enabled = (logView.SelectedItems.Count > 0);
        }
        private void logToolStripMenuItem_DropDownClosed(object sender, EventArgs e)
        {
            deleteMenuItem.Enabled =
                copyMenuItem.Enabled = true;
        }
        #endregion


        private void trayMenu_Opening(object sender, CancelEventArgs e)
        {
            if (this.Visible)
            {
                showLoggerTrayItem.Text = "Minimize to tray";
            }
            else
            {
                showLoggerTrayItem.Text = "Show logger";
            }
        }

        private void LoggerWindow_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                if (currentDialog == null)
                    trayIcon.ShowBalloonTip(5000, "Chrono Marker is here!",
                        "Right click if you want to open or close it", ToolTipIcon.Info);
            }
        }
        
        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult exportResult = exportTextDialog.ShowDialog();
        }

        private void exportTextDialog_FileOk(object sender, CancelEventArgs e)
        {
            Logger.ExportTo(exportTextDialog.FileName);
        }

        #endregion
    }
}
