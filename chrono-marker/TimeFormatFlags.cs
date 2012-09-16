//
//  TimeDisplayFlags.cs
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
	/// <summary>
	/// What to show on time display.
	/// <remarks>If Hours is unset, the minutes digits should
	/// be able to exceed 60 minutes. Likewise, if both hours and
	/// minutes are unset, the seconds region can exceed 60 seconds.
	/// The value is still capped at 24 hours though.</remarks>
	/// </summary>
	[Flags]
	public enum TimeFormatFlags
	{
		/// <summary>
		/// Nothing will be shown.
		/// <remarks>This value is actually invalid.</remarks>
		/// </summary>
		Nothing = 0,
		/// <summary>
		/// Shows the hours.
		/// </summary>
		Hours = 1,
		/// <summary>
		/// Shows the minutes.
		/// <remarks>If this is unset and the seconds flag is set,
		/// the hours should be hidden.</remarks>
		/// </summary>
		Minutes = 2,
		/// <summary>
		/// Shows the seconds.
		/// <remarks>If this is unset and the minutes flag is set,
		/// the miliseconds should be hidden.</remarks>
		/// </summary>
		Seconds = 4,
		/// <summary>
		/// Shows the miliseconds.
		/// </summary>
		Milliseconds = 8,

		AllTimeUnits = Hours | Minutes | Seconds | Milliseconds,

		/// <summary>
		/// Shows the minus symbol.
		/// <remarks>This only affects negative timespans</remarks>
		/// </summary>
		MinusSymbol = 16,

		/// <summary>
		/// Shows the plus symbol.
		/// <remarks>This only affects positive timespans</remarks>
		/// </summary>
		PlusSymbol = 32,

		/// <summary>
		/// Shows the leading zero.
		/// <remarks>While this is set, the highest time unit (leftmost)
		/// will always be shown with two digits.</remarks>
		/// </summary>
		LeadingZero = 64,

		/// <summary>
		/// Shows separators between time units.
		/// <remarks>When turned off abbreviations will replace the separators.
		/// eg. 03h14m15s926ms</remarks>
		/// </summary>
		Separators = 128,
	}
}

