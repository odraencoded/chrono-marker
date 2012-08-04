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
using System.Drawing;

namespace Chrono
{
    /// <summary>Handles event logging and logging data storage</summary>
    class LoggingHandler
    {
        public LoggingHandler(Logger logger, Watch clock)
        {
            this.Logger = logger;
            this.Watch = clock;

            Logging = true;

            clock.Started += clockStarted_event;
            clock.Stopped += clockStopped_event;
        }

        public string Name { get; private set; }
        public Color MarkerColor { get; set; }

        public Logger Logger { get; private set; }
        public Watch Watch { get; private set; }

        public bool Logging { get; set; }

        void clockStarted_event(object sender, WatchEventArgs e)
        {
            LogEntry logEntry = new LogEntry(e.Timestamp);
            logEntry.ClockName = e.Watch.Name;

            string description = "Started ";

            if (e.Speed >= 0) description += "timing";
            else description += "counting down";

            if (e.DisplayTime != TimeSpan.Zero) description += " with " + Logger.TimeToString(e.DisplayTime);
            description += ".";

            logEntry.Description = description;

            Logger.AddEntry(logEntry);
        }

        void clockStopped_event(object sender, WatchEventArgs e)
        {
            LogEntry logEntry = new LogEntry(e.Timestamp);
            logEntry.ClockName = e.Watch.Name;

            string description = "Stopped ";

            if (e.Speed >= 0) description += "timing";
            else description += "counting down";

            if (e.DisplayTime != TimeSpan.Zero) description += " with " + Logger.TimeToString(e.DisplayTime);
            description += ".";

            logEntry.Description = description;

            Logger.AddEntry(logEntry);
        }
    }
}
