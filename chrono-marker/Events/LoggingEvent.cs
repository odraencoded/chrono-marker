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
using System.Collections.ObjectModel;
using System.Text;

namespace Chrono
{
    public delegate void LoggingEventHandler(object sender, LoggingEventArgs e);

    public class LoggingEventArgs : EventArgs
    {
        public LoggingEventArgs(TimeLogger logger, params LogEntry[] manyEntries)
        {
            Logger = logger;
            _entries = manyEntries;
        }

		public TimeLogger Logger { get; private set; }
		public ReadOnlyCollection<LogEntry> Entries {get { return Array.AsReadOnly(_entries); } }

		private readonly LogEntry[] _entries;
	}
}
