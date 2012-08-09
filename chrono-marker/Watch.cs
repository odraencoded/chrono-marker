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
    public sealed class Watch
    {
        public Watch()
        {
            this.IsRunning = false;
            this.Speed = 1.0;
            startMark = clockedTicks = 0;
        }

        //public string Name { get; private set; }
        public bool IsRunning { get; private set; }
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
				if( IsRunning )
					return clockedTicks + ( long )Math.Round(
                    ( Stopwatch.GetTimestamp( ) - startMark ) * Speed );
				else
					return clockedTicks;
			}
			set {
				clockedTicks = value;

				if( IsRunning )
					clockedTicks -= (long)Math.Round(
						( Stopwatch.GetTimestamp() - startMark) * Speed);
			}
		}
        
        #region Main stopwatch interface
		// Starts or stops the watch. Returns whether it is running after changing mode.
		public bool StartStop()
		{
			if( IsRunning ) {
				IsRunning = false; // Watch is running, stop it.

				clockedTicks += ( long )Math.Round( ( Stopwatch.GetTimestamp( ) - startMark ) * Speed );

				// Creating it here solves possible miliseconds differences
				// in the event creation due to clockedTicks set lag
				if( Stopped != null )
					Stopped( this, new WatchEventArgs(this) );
			} else {
				// Creating it here solves possible milisecond differences
				// in the event creation to the timestamp it should store
				WatchEventArgs eventArgs = new WatchEventArgs(this);

				IsRunning = true;// Watch is stopped, start it.
				startMark = Stopwatch.GetTimestamp( );

				if( Started != null )
					Started( this, eventArgs );
			}

			return IsRunning;
		}

		public void ChangeSpeed(double value)
		{
            if (IsRunning)
            {
				clockedTicks += ( long )Math.Round( ( Stopwatch.GetTimestamp( ) - startMark ) * Speed );
				startMark=Stopwatch.GetTimestamp();
                Speed = value;
            }
            else
            {
                Speed = value;
            }

			if( ChangedSpeed != null )
					ChangedSpeed( this, new WatchEventArgs(this) );
        }
        #endregion

        public event WatchEventHandler Started;
        public event WatchEventHandler Stopped;
		public event WatchEventHandler ChangedSpeed;

        private long startMark, clockedTicks;

        public static readonly double TickRatio = 1.0 / Stopwatch.Frequency;

        public override string ToString()
        {
            return (IsRunning ? "Watch running at " + Speed.ToString("0.##") + "x" : " stopped watch");
        }
    }
}
