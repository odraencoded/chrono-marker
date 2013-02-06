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

namespace Chrono
{
    public class LogEntry : IComparable<LogEntry>, IEquatable<LogEntry>
    {
        public LogEntry(string clockName, string description, DateTime timestamp)
		{
			_clockName = clockName;
			_description = description;
			_timestamp = timestamp;
		}

		public string ClockName { get { return _clockName; } }
		public string Description { get { return _description; } }
		public DateTime Timestamp{ get { return _timestamp; } }

		private readonly string _clockName;
		private readonly string _description;
		private readonly DateTime _timestamp;

        public int CompareTo(LogEntry other)
        {
            return Timestamp.CompareTo(other.Timestamp);
        }

        public override string ToString()
        {
            return "[" + _timestamp.ToString("HH:mm:ss.fff") + "] " + _clockName + " - " + _description;
        }

		public bool Equals(LogEntry obj)
		{
			return
				_timestamp.Equals( obj._timestamp ) &&
				_clockName.Equals( obj._clockName ) &&
				_description.Equals( obj.Description );
		}

		public enum Properties {
			ClockName, Description, Timestamp
		}
    }
}
