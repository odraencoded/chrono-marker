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
using System.Diagnostics;

namespace Chrono
{
    sealed class Watch
    {
        public Watch(string name)
        {
            this.Name = name;
            this.IsRunning = false;
            this.Speed = 1.0;
            startMark = elapsedTicks = 0;
            Stopwatch a = new Stopwatch();
        }

        public string Name { get; private set; }
        public bool IsRunning { get; private set; }
        public double Speed { get; private set; }

        public TimeSpan GetElapsedTime()
        {
            long ticks = GetElapsedTicks();

            return TimeSpan.FromSeconds(TickRatio * ticks);
        }
        
        public void SetElapsedTime(TimeSpan value)
        {
            if (IsRunning) Stop();
            elapsedTicks = (long)Math.Round(value.TotalSeconds * Stopwatch.Frequency);
            Start();
        }

        public long GetElapsedTicks()
        {
            if (IsRunning)
                return elapsedTicks + (long)Math.Round(
                    (Stopwatch.GetTimestamp() - startMark) * Speed);
            else return elapsedTicks;
        }

        public void SetSpeed(double value)
        {
            if (IsRunning)
            {
                Stop();
                Speed = value;
                Start();
            }
            else
            {
                Speed = value;
            }
        }

        #region Main stopwatch interface
        public void Start()
        {
            if (IsRunning) return;
            IsRunning = true;

            startMark = Stopwatch.GetTimestamp();

            if (Started != null)
                Started(this, new WatchEventArgs(this));
        }
        public void Stop()
        {
            if (!IsRunning) return;
            IsRunning = false;

            elapsedTicks += (long)Math.Round((Stopwatch.GetTimestamp() - startMark) * Speed);

            if (Stopped != null)
                Stopped(this, new WatchEventArgs(this));
        }
        public void Snap()
        {
            if (Snapshot != null)
                Snapshot(this, new WatchEventArgs(this));
        }
        #endregion

        public event WatchEventHandler Started;
        public event WatchEventHandler Stopped;
        public event WatchEventHandler Snapshot;

        private long startMark, elapsedTicks;

        public static readonly double TickRatio = 1.0 / Stopwatch.Frequency;

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        public override string ToString()
        {
            return Name + (IsRunning ? " watch running at " + Speed.ToString("0.##") + "x" : " stopped watch");
        }
    }
}
