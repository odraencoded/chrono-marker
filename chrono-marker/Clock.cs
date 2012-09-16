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
using System.Diagnostics;

namespace Chrono
{
	/// <summary>
	/// It times stuff.
	/// </summary>
    public sealed class Clock
    {
        public Clock()
        {
            IsTicking = false;
            Speed = 1.0;
            _startMark = _clockedTicks = 0;
        }

		public bool IsTicking { get; private set; }
		public double Speed { get; private set; }

		public TimeSpan ElapsedTime {
			get {
				return TimeSpan.FromSeconds( TickRatio * ElapsedTicks );
			}
			set { 
				ElapsedTicks = (long) Math.Round(
					value.TotalSeconds * Stopwatch.Frequency);
			}
		}

		public long ElapsedTicks {
			get {
				if( IsTicking )
					return _clockedTicks + ( long )Math.Round(
                    ( Stopwatch.GetTimestamp( ) - _startMark ) * Speed );
				else
					return _clockedTicks;
			}
			set {
				_clockedTicks = value;

				if( IsTicking )
					_clockedTicks -= (long)Math.Round(
						( Stopwatch.GetTimestamp() - _startMark) * Speed);
			}
		}
        
        public event ClockEventHandler Started;
        public event ClockEventHandler Stopped;
		public event ClockEventHandler SpeedChanged;

		#region Fields
        private long _startMark, _clockedTicks;
		#endregion
		
        #region Main stopwatch interface
		/// <summary>
		/// Toggles the stopwatch. Stops if ticking, starts ticking if stopped.
		/// </summary>
		/// <returns>
		/// Whether the stopwatch is ticking after toggling.
		/// </returns>
		public bool Toggle()
		{
			// The following statements are beautiful.
			if( Start() ) return true;
			else
			{
				Stop();
				return false;
			}
		}

		public bool Stop()
		{
			// Can't stop the stopped.
			if(!IsTicking)
				return false;

			// Haha! In your face, stopped!
			// Oh wait...
			IsTicking = false;
			DateTime eventTime = DateTime.Now;

			long timeStamp = Stopwatch.GetTimestamp(); 

			_clockedTicks += ( long )Math.Round( ( timeStamp - _startMark ) * Speed );
			TimeSpan displayTime = ElapsedTime;
			// The order of the previous events must be preserved.

			if( Stopped != null )
				Stopped( this, new ClockEventArgs(this, displayTime, Speed, eventTime) );

			return true;
		}

		public bool Start()
		{
			// Can't start the started.
			if(IsTicking)
				return false;

			DateTime eventTime = DateTime.Now;

			long timeStamp = Stopwatch.GetTimestamp(); 

			// First get the elapsed time
			TimeSpan displayTime = ElapsedTime;

			// Then you start ticking
			IsTicking = true;
			_startMark = timeStamp;
			
			if( Started != null )
				Started( this, new ClockEventArgs(this, displayTime, Speed, eventTime));

			return true;
		}

		/// <summary>
		/// Changes the timing speed of the stopwatch.
		/// </summary>
		/// <param name='value'>
		/// New timing speed.
		/// </param>
		public void ChangeSpeed(double value)
		{
			if( IsTicking ) {
				long timeStamp = Stopwatch.GetTimestamp();

				_clockedTicks += ( long )Math.Round( ( timeStamp - _startMark ) * Speed );
				_startMark = timeStamp;
                Speed = value;
			}
            else
            {
				Speed = value;
			}

			if( SpeedChanged != null )
					SpeedChanged( this, new ClockEventArgs(this) );
		}
        #endregion

        public static readonly double TickRatio = 1.0 / Stopwatch.Frequency;

        public override string ToString()
        {
            return (IsTicking ? "Clock ticking at " + Speed.ToString("0.##") + "x" : "A stopped clock");
        }
	}
}
