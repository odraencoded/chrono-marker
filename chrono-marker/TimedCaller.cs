//
//  TimedCall.cs
//
//  Author:
//       Leonardo Augusto Pereira <http://code.google.com/p/chrono-marker/>
//
//  Copyright (c) 2012 Leonardo Augusto Pereira
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using GLib;

namespace Chrono
{
	/// <summary>This is class handles timed event calls</summary>
	public class TimedCaller
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Chrono.TimedCaller"/> class.
		/// </summary>
		/// <param name='frequency'>
		/// TimedOut even call frequency in miliseconds
		/// </param>
		/// <param name='delayedStart'>
		/// When set to true, the TimedOut calls won't start until <see cref="Chrono.TimeCalled.Start()"> is called
		/// </param>
		public TimedCaller(uint frequency, bool delayedStart = false)
		{
			this.Frequency = frequency;
			this.Cancelled = false;
			this.HasStarted = false;

			if(!delayedStart)
				Start();
		}

		public uint Frequency { get; private set; }
		public bool HasStarted { get; private set; }
		public bool Cancelled { get; private set; }


		public void Start()
		{
			if( !HasStarted ) {
				HasStarted = true;
				Timeout.Add( Frequency, new GLib.TimeoutHandler(glibTimeout));
			}
		}

		public void Cancel()
		{
			Cancelled = true;
		}

		public event TimedCallEventHandler TimeOut;

		private bool glibTimeout()
		{
			// This check is necessary because you can't remove the callback from glib when its first cancelled.
			if( Cancelled ) 
				return false;

			if( TimeOut != null )
				TimeOut( this, new TimedCallEventArgs(this) );

			// This check is for when the TimedCall has been cancelled in an TimedOut delegate.
			return !Cancelled;
		}
	}

	public delegate void TimedCallEventHandler(TimedCaller sender, TimedCallEventArgs e);

	public class TimedCallEventArgs : EventArgs
	{
		public TimedCallEventArgs(TimedCaller caller)
		{
			this.Caller = caller;
		}

		public TimedCaller Caller{get; private set;}
	}
}

