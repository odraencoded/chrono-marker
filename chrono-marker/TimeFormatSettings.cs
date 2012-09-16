//
//  TimeDisplaySettings.cs
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

namespace Chrono
{
	public class TimeFormatSettings
	{
		public TimeFormatSettings(TimeFormatFlags flags)
		{
			TimeFormatOptions = flags;
		}

		#region Properties
		public bool ShowHours {
			get { return ( TimeFormatOptions & TimeFormatFlags.Hours ) != 0;}
			set {
				if( value )
					TimeFormatOptions |= TimeFormatFlags.Hours;
				else
					TimeFormatOptions &= ~TimeFormatFlags.Hours;
			}
		}

		public bool ShowMinutes {
			get { return ( TimeFormatOptions & TimeFormatFlags.Minutes ) != 0;}
			set {
				if( value )
					TimeFormatOptions |= TimeFormatFlags.Minutes;
				else
					TimeFormatOptions &= ~TimeFormatFlags.Minutes;
			}
		}

		public bool ShowSeconds {
			get { return ( TimeFormatOptions & TimeFormatFlags.Seconds ) != 0;}
			set {
				if( value )
					TimeFormatOptions |= TimeFormatFlags.Seconds;
				else
					TimeFormatOptions &= ~TimeFormatFlags.Seconds;
			}
		}

		public bool ShowMilliseconds {
			get { return ( TimeFormatOptions & TimeFormatFlags.Milliseconds ) != 0;}
			set {
				if( value )
					TimeFormatOptions |= TimeFormatFlags.Milliseconds;
				else
					TimeFormatOptions &= ~TimeFormatFlags.Milliseconds;
			}
		}
		
		public bool ShowPlusSymbol {
			get { return ( TimeFormatOptions & TimeFormatFlags.PlusSymbol ) != 0;}
			set {
				if( value )
					TimeFormatOptions |= TimeFormatFlags.PlusSymbol;
				else
					TimeFormatOptions &= ~TimeFormatFlags.PlusSymbol;
			}
		}
		
		public bool ShowMinusSymbol {
			get { return ( TimeFormatOptions & TimeFormatFlags.MinusSymbol ) != 0;}
			set {
				if( value )
					TimeFormatOptions |= TimeFormatFlags.MinusSymbol;
				else
					TimeFormatOptions &= ~TimeFormatFlags.MinusSymbol;
			}
		}

		public bool ShowLeadingZero {
			get { return ( TimeFormatOptions & TimeFormatFlags.LeadingZero ) != 0;}
			set {
				if( value )
					TimeFormatOptions |= TimeFormatFlags.LeadingZero;
				else
					TimeFormatOptions &= ~TimeFormatFlags.LeadingZero;
			}
		}

		public bool ShowSeparators {
			get { return ( TimeFormatOptions & TimeFormatFlags.Separators ) != 0;}
			set {
				if( value )
					TimeFormatOptions |= TimeFormatFlags.Separators;
				else
					TimeFormatOptions &= ~TimeFormatFlags.Separators;
			}
		}

		public TimeFormatFlags TimeFormatOptions { get; set; }
		#endregion

		public string ToString(TimeSpan timespan)
		{
			string leftSymbol;
			string milliseconds, seconds, minutes, hours;

			if( ShowMinusSymbol && timespan.Ticks < 0 )
				leftSymbol = "-";
			else if( ShowPlusSymbol && timespan.Ticks > 0 )
				leftSymbol = "+";
			else leftSymbol = "";

			timespan = timespan.Duration( );

			if( ShowMilliseconds )
			{
				if( ShowSeconds )
				{
					milliseconds = timespan.Milliseconds.ToString().PadLeft(3, '0');
					if(ShowSeparators) 
						milliseconds = "." + milliseconds;
				}
				else
				{
					milliseconds = ((int)Math.Floor(timespan.TotalMilliseconds)).ToString();

					if(ShowLeadingZero)
						milliseconds = milliseconds.PadLeft(3, '0');
				}

				if(!ShowSeparators)
					milliseconds = milliseconds + "ms";
			}
			else milliseconds = "";

			if( ShowSeconds )
			{
				if( ShowMinutes )
				{
					seconds = timespan.Seconds.ToString().PadLeft(2, '0');

					if(ShowSeparators) 
						seconds = ":" + seconds;
				}
				else
				{
					seconds = ((int)Math.Floor(timespan.TotalSeconds)).ToString();
					
					if(ShowLeadingZero)
						seconds = seconds.PadLeft(2, '0');
				}

				if(!ShowSeparators)
					seconds = seconds + "s";
			}
			else seconds = "";

			if( ShowMinutes )
			{
				if( ShowHours )
				{
					minutes = timespan.Minutes.ToString().PadLeft(2, '0');

					if(ShowSeparators) 
						minutes = ":" + minutes;
				}
				else
				{
					minutes = ((int)Math.Floor(timespan.TotalMinutes)).ToString();
					
					if(ShowLeadingZero)
						minutes = minutes.PadLeft(2, '0');
				}

				if(!ShowSeparators)
					minutes = minutes + "m";
			}
			else minutes = "";

			if( ShowHours )
			{
				hours = ((int)Math.Floor(timespan.TotalHours)).ToString();
				
				if(ShowLeadingZero)
					hours = hours.PadLeft(2, '0');

				if(!ShowSeparators)
					hours = hours + "h";
			}
			else hours = "";

			return leftSymbol + hours + minutes + seconds + milliseconds;
		}
	}
}

