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
    // Stores data about an event raised by a watch
    public class ClockEventArgs : EventArgs
    {
        public ClockEventArgs(Clock watch) : this(watch, watch.ElapsedTime, watch.Speed, DateTime.Now) { }

        public ClockEventArgs(Clock watch, TimeSpan displayTime, double speed, DateTime timestamp)
        {
            _watch = watch;
            _displayTime = displayTime;
            _timestamp = timestamp;
            _speed = speed;
        }

        // Watch that raised this event
		public Clock Watch { get { return _watch; } }

        // Time on the watch when the event was rised
		public TimeSpan DisplayTime { get { return _displayTime; } }

        // Speed of the watch when the event was rised
		public double Speed { get { return _speed; } }

        // Time when the event was raised
		public DateTime Timestamp { get { return _timestamp; } }

		#region Detail
		private Clock _watch;
		private TimeSpan _displayTime;
		private double _speed;
		private DateTime _timestamp;
		#endregion
	}

    public delegate void ClockEventHandler(object sender, ClockEventArgs e);
}