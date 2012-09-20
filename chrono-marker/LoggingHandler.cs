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
using Mono.Unix;

namespace Chrono
{
    /// <summary>
    /// This class relays clock events to a time logger
	/// In the form of LogEntry
    /// </summary>
    public class LoggingHandler
    {
        public LoggingHandler(TimeLogger logger, Clock clock, string name)
		{
			_name = name;
			Clock = clock;
			Logger = logger;

			LogStarts = true;
			LogStops = true;
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		/// <remarks>Also raises the Renamed event and
		/// calls Logger.RefreshClock</remarks>
		public string Name { 
			get { return _name; } 
			set
			{
				if(value == _name)
					return;

				string prevName = _name;
				_name = value;
				Logger.RefreshClock(prevName);

				if(Renamed != null)
					Renamed(this, null);
			}
		}

		private string _name;

		public TimeLogger Logger { get; private set; }
		public Clock Clock { get; private set; }

		public TimeFormatSettings TimeFormatSettings {
			get
			{
				return _timeFormatSettings == null ?
					Logger.DefaultFormatSettings : _timeFormatSettings;
			}
			set { _timeFormatSettings = value; }
		}

		private TimeFormatSettings _timeFormatSettings;

		public bool LogStarts {
			get { return _logStarts;}
			set {
				if( value == _logStarts )
					return;

				if( value == true ) {
					Clock.Started += clockStarted_event;
				} else {
					Clock.Started -= clockStarted_event;
				}

				_logStarts = value;
			}
		}
		public bool LogStops {
			get { return _logStops;}
			set {
				if( value == _logStops )
					return;

				if( value == true ) {
					Clock.Stopped += clockStopped_event;
				} else {
					Clock.Stopped -= clockStopped_event;
				}

				_logStops = value;
			}
		}

		private bool _logStarts;
		private bool _logStops;

		public TimeSpan UpperCap { get { return new TimeSpan(7, 0, 0, 0, 0); } }
		public TimeSpan LowerCap { get { return new TimeSpan(-7, 0, 0, 0, 0); } }

		public string CurrentTime {
			get
			{
				TimeSpan clockTime = Clock.ElapsedTime;

				if(clockTime > UpperCap) clockTime = UpperCap;
				else if(clockTime < LowerCap) clockTime = LowerCap;

				return TimeFormatSettings.ToString(clockTime);
			}
		}

		/// <summary>
		/// Occurs when the Name property is changed
		/// </summary>
		public event EventHandler Renamed;

        private void clockStarted_event(object sender, ClockEventArgs e)
		{
			string translatable, logDesc;

			if(e.Speed >= 0)
				translatable = Catalog.GetString("Started ticking with {0}.");
			else translatable = Catalog.GetString("Started counting down with {0}.");

			logDesc = string.Format(translatable, CurrentTime);

			Logger.AddEntry( new LogEntry(_name, logDesc, e.Timestamp) );
		}
        private void clockStopped_event(object sender, ClockEventArgs e)
		{
			string translatable, logDesc;

			if( e.Speed >= 0 )
				translatable = Catalog.GetString( "Stopped ticking at {0}." );
			else translatable = Catalog.GetString( "Stopped counting down at {0}." );

			logDesc = string.Format( translatable, CurrentTime);

			Logger.AddEntry( new LogEntry(_name, logDesc, e.Timestamp) );
        }

		public override int GetHashCode()
		{
			return Clock.GetHashCode( ) ^ Logger.GetHashCode( );
		}
    }
}
