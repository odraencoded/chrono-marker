//
//  RenameClockDialog.cs
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
using Gtk;
using Mono.Unix;

namespace Chrono
{
	public partial class RenameClockDialog : Dialog
	{
		public RenameClockDialog(string previousName)
		{
			this.Build( );

			newNameEntry.Text = this.PreviousName = previousName;

			RefreshTexts();
		}

		public void RefreshTexts()
		{
			Title = string.Format(Catalog.GetString("Rename {0}"), PreviousName);
			newNameLabel.Text = Catalog.GetString("New Name");
			newNameContainer.TooltipMarkup = Catalog.GetString("Type a new name for the stopwatch here");
		}

		public string NewName { get { return newNameEntry.Text; } }
		public string PreviousName { get; private set; }
	}
}

