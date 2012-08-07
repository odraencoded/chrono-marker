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
using Gtk;

namespace Chrono
{
    static class Program
    {
        public static void Main(string[] args)
        {
			Application.Init();
            
			Logger timeLogger = new Logger();
			LoggerWindow window = new LoggerWindow(timeLogger);

			timeLogger.AddWatch(new Watch("Beta Watch"));

			window.Show();
			window.ShowWatch();

			Application.Run();
        }
    }
}
