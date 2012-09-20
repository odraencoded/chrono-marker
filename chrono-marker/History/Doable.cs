//
//  Doable.cs
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
	/// Undo and Redo abstract class.
	/// </summary>
	public abstract class Doable
	{
		public abstract string RedoText { get; }
		public abstract string UndoText { get; }

		public abstract void Redo();
		public abstract void Undo();
	}

	public class NoCanDoException : Exception
	{
		NoCanDoException(string why) : base(why)
		{
		}
	}
}

