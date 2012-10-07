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
		#region Properties
		public bool ShowHours { get; set; }
		public bool ShowMinutes { get; set; }
		public bool ShowSeconds { get; set; }
		public bool ShowMilliseconds { get; set; }
		
		public bool ShowPlusSymbol { get; set; }		
		public bool ShowMinusSymbol { get; set; }

		public bool ShowLeadingZeroes { get; set; }
		public bool ShowSeparators { get; set; }
		#endregion

		public bool IsNonZero(TimeSpan timespan)
		{
			timespan = timespan.Duration();

			if(ShowMilliseconds && timespan.TotalMilliseconds >= 1)
				return true;
			if(ShowSeconds && timespan.TotalSeconds >= 1)
				return true;
			if(ShowMinutes && timespan.TotalMinutes >= 1)
				return true;
			if(ShowHours && timespan.TotalHours >= 1)
				return true;
			return false;
		}

		public string ToString(TimeSpan timespan)
		{
			string leftSymbol;
			string milliseconds, seconds, minutes, hours;
			bool hoursVisible, minutesVisible, secondsVisible, milliVisible;

			if( ShowMinusSymbol && timespan.Ticks < 0 && IsNonZero( timespan ) ) {
				leftSymbol="-";
			}
			else if(ShowPlusSymbol && timespan.Ticks > 0 && IsNonZero(timespan))
			{
				leftSymbol="+";
			}
			else leftSymbol = "";

			timespan = timespan.Duration( );

			// If a time unit is visible and ShowLeadingZeroes is not true,
			// it will be display if it's greater than zero or
			// if all lower units are hidden. This avoids empty strings.
			milliVisible = ShowMilliseconds;
			secondsVisible = ShowSeconds && (ShowLeadingZeroes || timespan.TotalSeconds >= 1 || !milliVisible);
			minutesVisible = ShowMinutes && (ShowLeadingZeroes || timespan.TotalMinutes >= 1 || (!milliVisible && !secondsVisible));
			hoursVisible = ShowHours && (ShowLeadingZeroes || timespan.TotalHours >= 1 || (!milliVisible && !secondsVisible && !minutesVisible));

			if( milliVisible )
			{
				if(secondsVisible)
				{
					milliseconds = timespan.Milliseconds.ToString().PadLeft(3, '0');
					if(ShowSeparators)
						milliseconds = "." + milliseconds;
					else milliseconds = milliseconds + "ms";
				}
				else
				{
					milliseconds = ((int)Math.Floor(timespan.TotalMilliseconds)).ToString();
					if(ShowLeadingZeroes)
						milliseconds = milliseconds.PadLeft(3, '0');
					if(!ShowSeparators)
						milliseconds = milliseconds + "ms";
				}


			}
			else milliseconds = "";

			if( secondsVisible)
			{
				if(minutesVisible)
				{
					seconds = timespan.Seconds.ToString().PadLeft(2, '0');
					if(ShowSeparators)
						seconds = ":" + seconds;
					else seconds = seconds + "s";
				}
				else
				{
					seconds = ((int)Math.Floor(timespan.TotalSeconds)).ToString();
					if(ShowLeadingZeroes)
						seconds = seconds.PadLeft(2, '0');
					if(!ShowSeparators)
						seconds = seconds + "s";
				}
			}
			else seconds = "";

			if( minutesVisible )
			{
				if(hoursVisible)
				{
					minutes = timespan.Minutes.ToString().PadLeft(2, '0');
					if(ShowSeparators)
						minutes = ":" + minutes;
					else minutes = minutes + "m";
				}
				else
				{
					minutes = ((int)Math.Floor(timespan.TotalMinutes)).ToString();
					if(ShowLeadingZeroes)
						minutes = minutes.PadLeft(2, '0');
					if(!ShowSeparators)
						minutes = minutes + "m";
				}
			}
			else minutes = "";

			if( hoursVisible )
			{
				hours = ((int)Math.Floor(timespan.TotalHours)).ToString();

				if(ShowLeadingZeroes)
					hours = hours.PadLeft(2, '0');
				if(!ShowSeparators)
					hours = hours + "h";
			}
			else hours = "";

			return leftSymbol + hours + minutes + seconds + milliseconds;
		}
	}
}

