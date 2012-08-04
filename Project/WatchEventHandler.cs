﻿/* Copyright (C) 2012 Leonardo Augusto Pereira
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
    /// <summary>Stores data about an event raised by a watch</summary>
    class WatchEventArgs : EventArgs
    {
        public WatchEventArgs(Watch watch) : this(watch, watch.GetElapsedTime(), watch.Speed, DateTime.Now) { }

        public WatchEventArgs(Watch watch, TimeSpan displayTime, double speed, DateTime timestamp)
        {
            this.Watch = watch;
            this.DisplayTime = displayTime;
            this.Timestamp = timestamp;
            this.Speed = speed;
        }

        /// <summary>Watch that raised this event</summary>
        public Watch Watch { get; private set; }

        /// <summary>Time on the watch when the event was rised</summary>
        public TimeSpan DisplayTime { get; private set; }

        /// <summary>Speed of the watch when the event was rised</summary>
        public double Speed { get; private set; }

        /// <summary>Time when the event was raised</summary>
        public DateTime Timestamp { get; private set; }
    }

    delegate void WatchEventHandler(object sender, WatchEventArgs e);
}