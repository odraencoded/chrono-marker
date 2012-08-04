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
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Chrono
{
    /// <summary>This class is responsible for logging Sandwatch events</summary>
    sealed class Logger
    {
        public Logger()
        {
            logEntryNodes = new Dictionary<LogEntry, LinkedListNode<LogEntry>>();
            manyLogEntries = new LinkedList<LogEntry>();
            namedHandlers = new Dictionary<string, LoggingHandler>();
        }

        #region Watch list interface
        public LoggingHandler this[string name]
        {
            get { return namedHandlers[name]; }
        }

        public LoggingHandler AddWatch(Watch watch)
        {
            if (namedHandlers.ContainsKey(watch.Name))
                throw new ArgumentException("The name of this watch was already registered.", "watch");

            LoggingHandler result = new LoggingHandler(this, watch);
            namedHandlers.Add(watch.Name, result);

            if (WatchAdded != null)
                WatchAdded(this, new LoggerWatchEventArgs(this, watch));

            return result;
        }
        public void RemoveWatch(string watchName)
        {
            LoggingHandler logHandler;

            if (!namedHandlers.TryGetValue(watchName, out logHandler))
                return;

            namedHandlers.Remove(watchName);

            if (WatchRemoved != null)
                WatchRemoved(this, new LoggerWatchEventArgs(this, logHandler.Watch));
        }

        public Watch GetWatch(string name)
        {
            LoggingHandler result;
            if (!namedHandlers.TryGetValue(name, out result))
            {
                Watch newWatch = new Watch(name);

                result = AddWatch(newWatch);
            }

            return result.Watch;
        }
        #endregion

        #region Log entry manipulation
        public void AddEntry(LogEntry entry)
        {
            if (logEntryNodes.ContainsKey(entry))
                throw new ArgumentException("Duplicated entry", "entry");

            LinkedListNode<LogEntry> logNode = manyLogEntries.AddLast(entry);
            logEntryNodes.Add(entry, logNode);

            if (EntryAdded != null)
                EntryAdded(this, new LoggingEventArgs(this, entry));
        }
        public void RemoveEntry(LogEntry entry)
        {
            LinkedListNode<LogEntry> logNode;

            if (!logEntryNodes.TryGetValue(entry, out logNode))
                throw new ArgumentException("Entry does not exist on this logger", "entry");

            manyLogEntries.Remove(logNode);
            logEntryNodes.Remove(entry);

            if (EntryDeleted != null)
                EntryDeleted(this, new LoggingEventArgs(this, entry));
        }
        #endregion

        #region Events
        public event LoggingEventHandler EntryAdded;
        public event LoggingEventHandler EntryDeleted;
        
        public event LoggerWatchEventHandler WatchAdded;
        public event LoggerWatchEventHandler WatchRemoved;
        #endregion

        #region Detail
        private Dictionary<string, LoggingHandler> namedHandlers;
        private Dictionary<LogEntry, LinkedListNode<LogEntry>> logEntryNodes;
        private LinkedList<LogEntry> manyLogEntries;
        #endregion

        public void ExportTo(string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename, false, Encoding.Unicode))
            {
                List<LogEntry> exportEntries = new List<LogEntry>(manyLogEntries);

                exportEntries.Sort();
                exportEntries.Reverse();

                foreach (LogEntry logEntry in exportEntries)
                    writer.WriteLine(logEntry);
            }
        }

        public static string TimeToString(TimeSpan timespan)
        {
            string result="";

            if (timespan.Ticks < 0) result += "-";
            timespan = timespan.Duration();
            result += string.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                timespan.Hours, timespan.Minutes, timespan.Seconds, timespan.Milliseconds);

            return result;
        }
    }
}