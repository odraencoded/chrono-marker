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
    public class LoggerClockEventArgs : EventArgs
    {
        public LoggerClockEventArgs(LoggingHandler loggingHandler)
		{
			_loggingHandler = loggingHandler;
		}

		public LoggingHandler LoggingHandler { get { return _loggingHandler; } }
		public TimeLogger Logger { get { return _loggingHandler.Logger; } }
		public Clock Watch { get { return _loggingHandler.Clock; } }

		private readonly LoggingHandler _loggingHandler;
    }

	public delegate void LoggerClockEventHandler(object sender, LoggerClockEventArgs e);
}
