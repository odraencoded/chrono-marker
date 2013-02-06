
// This file has been generated by the GUI designer. Do not modify.
namespace Chrono
{
	public partial class AboutDialog
	{
		private global::Gtk.Alignment alignment2;
		private global::Gtk.VBox vbox2;
		private global::Gtk.HBox hbox1;
		private global::Gtk.Image image3;
		private global::Gtk.Label label2;
		private global::Gtk.Frame frame2;
		private global::Gtk.Alignment GtkAlignment1;
		private global::Gtk.Label label1;
		private global::Gtk.Label GtkLabel1;
		private global::Gtk.Label label3;
		private global::Gtk.Button buttonCancel;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Chrono.AboutDialog
			this.Name = "Chrono.AboutDialog";
			this.Title = "About Chrono Marker";
			this.TypeHint = ((global::Gdk.WindowTypeHint)(1));
			this.WindowPosition = ((global::Gtk.WindowPosition)(3));
			this.AllowGrow = false;
			// Internal child Chrono.AboutDialog.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.alignment2 = new global::Gtk.Alignment (0.5F, 0.5F, 1F, 1F);
			this.alignment2.Name = "alignment2";
			this.alignment2.LeftPadding = ((uint)(10));
			this.alignment2.RightPadding = ((uint)(10));
			// Container child alignment2.Gtk.Container+ContainerChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			// Container child vbox2.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 20;
			// Container child hbox1.Gtk.Box+BoxChild
			this.image3 = new global::Gtk.Image ();
			this.image3.Name = "image3";
			this.image3.Xalign = 1F;
			this.image3.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "chrono-marker", global::Gtk.IconSize.Dialog);
			this.hbox1.Add (this.image3);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.image3]));
			w2.Position = 0;
			// Container child hbox1.Gtk.Box+BoxChild
			this.label2 = new global::Gtk.Label ();
			this.label2.Name = "label2";
			this.label2.LabelProp = "<span size=\"x-large\" font_weight=\"heavy\">Chrono Marker</span>\nStopwatch and time " +
				"logger gadget\n\nCopyright &#169; 2012 Leonardo Augusto Pereira";
			this.label2.UseMarkup = true;
			this.label2.Wrap = true;
			this.label2.Justify = ((global::Gtk.Justification)(2));
			this.hbox1.Add (this.label2);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.label2]));
			w3.Position = 1;
			this.vbox2.Add (this.hbox1);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hbox1]));
			w4.Position = 0;
			w4.Expand = false;
			w4.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.frame2 = new global::Gtk.Frame ();
			this.frame2.Name = "frame2";
			// Container child frame2.Gtk.Container+ContainerChild
			this.GtkAlignment1 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment1.Name = "GtkAlignment1";
			this.GtkAlignment1.LeftPadding = ((uint)(5));
			this.GtkAlignment1.RightPadding = ((uint)(5));
			this.GtkAlignment1.BottomPadding = ((uint)(5));
			this.GtkAlignment1.BorderWidth = ((uint)(3));
			// Container child GtkAlignment1.Gtk.Container+ContainerChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.LabelProp = @"Chrono Marker is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

Chrono Marker  is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Chrono Marker. If not, see <http://www.gnu.org/licenses/>.";
			this.label1.Wrap = true;
			this.GtkAlignment1.Add (this.label1);
			this.frame2.Add (this.GtkAlignment1);
			this.GtkLabel1 = new global::Gtk.Label ();
			this.GtkLabel1.Name = "GtkLabel1";
			this.GtkLabel1.LabelProp = global::Mono.Unix.Catalog.GetString ("License");
			this.GtkLabel1.UseMarkup = true;
			this.frame2.LabelWidget = this.GtkLabel1;
			this.vbox2.Add (this.frame2);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.frame2]));
			w7.Position = 1;
			w7.Expand = false;
			w7.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.LabelProp = "http://code.google.com/p/chrono-marker/";
			this.label3.UseMarkup = true;
			this.label3.UseUnderline = true;
			this.label3.Selectable = true;
			this.label3.SingleLineMode = true;
			this.vbox2.Add (this.label3);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.label3]));
			w8.Position = 2;
			w8.Expand = false;
			w8.Fill = false;
			this.alignment2.Add (this.vbox2);
			w1.Add (this.alignment2);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(w1 [this.alignment2]));
			w10.Position = 0;
			w10.Expand = false;
			w10.Fill = false;
			// Internal child Chrono.AboutDialog.ActionArea
			global::Gtk.HButtonBox w11 = this.ActionArea;
			w11.Name = "dialog1_ActionArea";
			w11.Spacing = 10;
			w11.BorderWidth = ((uint)(5));
			w11.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseStock = true;
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "gtk-close";
			this.AddActionWidget (this.buttonCancel, -7);
			global::Gtk.ButtonBox.ButtonBoxChild w12 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w11 [this.buttonCancel]));
			w12.Expand = false;
			w12.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 448;
			this.DefaultHeight = 427;
			this.Show ();
			this.buttonCancel.Clicked += new global::System.EventHandler (this.closeClicked_event);
		}
	}
}
