//  Author:
//       Leonardo Augusto Pereira <http://code.google.com/p/chrono-marker/>
//
//  Copyright (c) 2013 Leonardo Augusto Pereira
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
	/// This is an entry box that can be validated.
	/// </summary>
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ValidatableEntry : Gtk.Entry
	{
		public ValidatableEntry() : this(null) {}
		public ValidatableEntry(IValidator validator)
		{
			Validator = validator;
			DoValidation = true;

			if(Validator != null)
				IsValid = Validator.Validate(Text);
			else IsValid = true;
		}

		public bool IsValid { get; private set; }
		public bool DoValidation { get; set; }
		public IValidator Validator { get; set; }

		protected override void OnChanged()
		{
			if( DoValidation && Validator != null ) {
				IsValid = Validator.Validate(this.Text);
			}

			base.OnChanged();
		}
	}

	/// <summary>
	/// This interface validates text.
	/// </summary>
	public interface IValidator {
		/// <summary>
		/// Checks whether the text is valid.
		/// </summary>
		bool Validate(string text);
	}
}


