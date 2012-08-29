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
            _isTicking = false;
            _speed = 1.0;
            _startMark = _clockedTicks = 0;
        }

        //public string Name { get; private set; }
		public bool IsTicking { get { return _isTicking; } }
		public double Speed { get { return _speed; } }

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
				if( _isTicking )
					return _clockedTicks + ( long )Math.Round(
                    ( Stopwatch.GetTimestamp( ) - _startMark ) * _speed );
				else
					return _clockedTicks;
			}
			set {
				_clockedTicks = value;

				if( IsTicking )
					_clockedTicks -= (long)Math.Round(
						( Stopwatch.GetTimestamp() - _startMark) * _speed);
			}
		}
        
        public event ClockEventHandler Started;
        public event ClockEventHandler Stopped;
		public event ClockEventHandler SpeedChanged;

		#region Fields
        private long _startMark, _clockedTicks;
		private bool _isTicking;
		private double _speed;
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
			// Cache time
			long timeStamp = Stopwatch.GetTimestamp(); 
			DateTime eventTime = DateTime.Now;
			TimeSpan displayTime;

			if( _isTicking ) {
				_isTicking = false; // Watch is running, stop it.

				_clockedTicks += ( long )Math.Round( ( timeStamp - _startMark ) * Speed );
				displayTime = ElapsedTime;

				// Creating it here solves possible miliseconds differences
				// in the event creation due to clockedTicks set lag
				if( Stopped != null )
					Stopped( this, new ClockEventArgs(this, displayTime, _speed, eventTime) );
			} else {
				// Setting displayTime here to avoid milisecond differences
				// between setting _isTicking and ElapsedTime.Get
				displayTime = ElapsedTime;

				_isTicking = true; // Watch is stopped, start it.
				_startMark = timeStamp;

				if( Started != null )
					Started( this, new ClockEventArgs(this, displayTime, _speed, eventTime));
			}

			return IsTicking;
		}

		/// <summary>
		/// Changes the timing speed of the stopwatch.
		/// </summary>
		/// <param name='value'>
		/// New timing speed.
		/// </param>
		public void ChangeSpeed(double value)
		{
			if( _isTicking ) {
				_clockedTicks += ( long )Math.Round( ( Stopwatch.GetTimestamp( ) - _startMark ) * _speed );
				_startMark=Stopwatch.GetTimestamp();
                _speed = value;
			}
            else
            {
                _speed = value;
			}

			if( SpeedChanged != null )
					SpeedChanged( this, new ClockEventArgs(this) );
		}
        #endregion

        public static readonly double TickRatio = 1.0 / Stopwatch.Frequency;

        public override string ToString()
        {
            return (IsTicking ? "Watch running at " + Speed.ToString("0.##") + "x" : " stopped watch");
        }
	}
}
