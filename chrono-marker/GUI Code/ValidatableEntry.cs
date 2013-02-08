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


