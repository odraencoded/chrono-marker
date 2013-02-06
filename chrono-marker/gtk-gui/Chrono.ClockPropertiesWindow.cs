
// This file has been generated by the GUI designer. Do not modify.
namespace Chrono
{
	public partial class ClockPropertiesWindow
	{
		private global::Gtk.VBox vbox2;
		private global::Gtk.HBox hbox1;
		private global::Gtk.HBox watchNameContainer;
		private global::Gtk.Label watchNameLabel;
		private global::Gtk.ComboBoxEntry watchNameCombo;
		private global::Gtk.Button watchNameButton;
		private global::Gtk.Notebook detailBook;
		private global::Gtk.Alignment alignment3;
		private global::Gtk.VBox vbox6;
		private global::Gtk.Label logTheFollowingLabel;
		private global::Gtk.Alignment alignment2;
		private global::Gtk.VBox vbox5;
		private global::Gtk.CheckButton logStartsCheck;
		private global::Gtk.CheckButton logStopsCheck;
		private global::Gtk.HButtonBox hbuttonbox3;
		private global::Gtk.Button deleteBtn;
		private global::Gtk.HSeparator hseparator4;
		private global::Gtk.Label generalTabLabel;
		private global::Gtk.Alignment alignment5;
		private global::Gtk.VBox vbox4;
		private global::Gtk.Label countingDirectionLabel;
		private global::Gtk.Alignment alignment6;
		private global::Gtk.VBox vbox8;
		private global::Gtk.RadioButton countForwardOption;
		private global::Gtk.RadioButton countBackwardOption;
		private global::Gtk.HSeparator hseparator2;
		private global::Gtk.HBox watchSpeedContainer;
		private global::Gtk.Label watchSpeedLabel;
		private global::Gtk.SpinButton watchSpeedEntry;
		private global::Gtk.Button applySpeedBtn;
		private global::Gtk.HSeparator hseparator3;
		private global::Gtk.Label countingTabLabel;
		private global::Gtk.Alignment alignment1;
		private global::Gtk.VBox vbox7;
		private global::Gtk.Label configureLayoutLabel;
		private global::Gtk.Alignment alignment4;
		private global::Gtk.VBox vbox9;
		private global::Gtk.CheckButton dockedCheck;
		private global::Gtk.CheckButton compactCheck;
		private global::Gtk.HBox hbox3;
		private global::Gtk.ToggleButton windowVisibleBtn;
		private global::Gtk.CheckButton keepAboveCheck;
		private global::Gtk.HSeparator hseparator5;
		private global::Gtk.Label windowTabLabel;
		private global::Gtk.HButtonBox hbuttonbox2;
		private global::Gtk.Button closeBtn;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Chrono.ClockPropertiesWindow
			this.Name = "Chrono.ClockPropertiesWindow";
			this.Title = "Stopwatches";
			this.WindowPosition = ((global::Gtk.WindowPosition)(1));
			this.BorderWidth = ((uint)(6));
			this.Resizable = false;
			this.AllowGrow = false;
			// Container child Chrono.ClockPropertiesWindow.Gtk.Container+ContainerChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			// Container child vbox2.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 10;
			// Container child hbox1.Gtk.Box+BoxChild
			this.watchNameContainer = new global::Gtk.HBox ();
			this.watchNameContainer.Name = "watchNameContainer";
			this.watchNameContainer.Spacing = 20;
			// Container child watchNameContainer.Gtk.Box+BoxChild
			this.watchNameLabel = new global::Gtk.Label ();
			this.watchNameLabel.Name = "watchNameLabel";
			this.watchNameLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("Name");
			this.watchNameContainer.Add (this.watchNameLabel);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.watchNameContainer [this.watchNameLabel]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;
			// Container child watchNameContainer.Gtk.Box+BoxChild
			this.watchNameCombo = new global::Gtk.ComboBoxEntry ();
			this.watchNameCombo.WidthRequest = 160;
			this.watchNameCombo.Name = "watchNameCombo";
			this.watchNameContainer.Add (this.watchNameCombo);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.watchNameContainer [this.watchNameCombo]));
			w2.Position = 1;
			this.hbox1.Add (this.watchNameContainer);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.watchNameContainer]));
			w3.Position = 0;
			// Container child hbox1.Gtk.Box+BoxChild
			this.watchNameButton = new global::Gtk.Button ();
			this.watchNameButton.WidthRequest = 80;
			this.watchNameButton.CanDefault = true;
			this.watchNameButton.CanFocus = true;
			this.watchNameButton.Name = "watchNameButton";
			this.watchNameButton.UseUnderline = true;
			this.watchNameButton.Label = global::Mono.Unix.Catalog.GetString ("Create");
			this.hbox1.Add (this.watchNameButton);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.watchNameButton]));
			w4.Position = 1;
			w4.Expand = false;
			w4.Fill = false;
			this.vbox2.Add (this.hbox1);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hbox1]));
			w5.Position = 0;
			w5.Expand = false;
			w5.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.detailBook = new global::Gtk.Notebook ();
			this.detailBook.CanFocus = true;
			this.detailBook.Name = "detailBook";
			this.detailBook.CurrentPage = 1;
			this.detailBook.BorderWidth = ((uint)(1));
			// Container child detailBook.Gtk.Notebook+NotebookChild
			this.alignment3 = new global::Gtk.Alignment (0.5F, 0.5F, 1F, 1F);
			this.alignment3.Name = "alignment3";
			this.alignment3.LeftPadding = ((uint)(24));
			this.alignment3.TopPadding = ((uint)(16));
			this.alignment3.RightPadding = ((uint)(24));
			this.alignment3.BottomPadding = ((uint)(16));
			// Container child alignment3.Gtk.Container+ContainerChild
			this.vbox6 = new global::Gtk.VBox ();
			this.vbox6.Name = "vbox6";
			this.vbox6.Spacing = 6;
			// Container child vbox6.Gtk.Box+BoxChild
			this.logTheFollowingLabel = new global::Gtk.Label ();
			this.logTheFollowingLabel.Name = "logTheFollowingLabel";
			this.logTheFollowingLabel.Xalign = 0F;
			this.logTheFollowingLabel.LabelProp = "Log the following events";
			this.vbox6.Add (this.logTheFollowingLabel);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox6 [this.logTheFollowingLabel]));
			w6.Position = 0;
			w6.Expand = false;
			w6.Fill = false;
			// Container child vbox6.Gtk.Box+BoxChild
			this.alignment2 = new global::Gtk.Alignment (0.5F, 0.5F, 1F, 1F);
			this.alignment2.Name = "alignment2";
			this.alignment2.LeftPadding = ((uint)(20));
			this.alignment2.TopPadding = ((uint)(5));
			// Container child alignment2.Gtk.Container+ContainerChild
			this.vbox5 = new global::Gtk.VBox ();
			this.vbox5.Name = "vbox5";
			this.vbox5.Spacing = 6;
			// Container child vbox5.Gtk.Box+BoxChild
			this.logStartsCheck = new global::Gtk.CheckButton ();
			this.logStartsCheck.CanFocus = true;
			this.logStartsCheck.Name = "logStartsCheck";
			this.logStartsCheck.Label = "When the Start button is clicked";
			this.logStartsCheck.DrawIndicator = true;
			this.logStartsCheck.UseUnderline = true;
			this.vbox5.Add (this.logStartsCheck);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.logStartsCheck]));
			w7.Position = 0;
			w7.Expand = false;
			w7.Fill = false;
			// Container child vbox5.Gtk.Box+BoxChild
			this.logStopsCheck = new global::Gtk.CheckButton ();
			this.logStopsCheck.CanFocus = true;
			this.logStopsCheck.Name = "logStopsCheck";
			this.logStopsCheck.Label = "When the Stop button is clicked";
			this.logStopsCheck.DrawIndicator = true;
			this.logStopsCheck.UseUnderline = true;
			this.vbox5.Add (this.logStopsCheck);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.logStopsCheck]));
			w8.Position = 1;
			w8.Expand = false;
			w8.Fill = false;
			this.alignment2.Add (this.vbox5);
			this.vbox6.Add (this.alignment2);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.vbox6 [this.alignment2]));
			w10.Position = 1;
			w10.Expand = false;
			w10.Fill = false;
			// Container child vbox6.Gtk.Box+BoxChild
			this.hbuttonbox3 = new global::Gtk.HButtonBox ();
			this.hbuttonbox3.Name = "hbuttonbox3";
			this.hbuttonbox3.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(3));
			// Container child hbuttonbox3.Gtk.ButtonBox+ButtonBoxChild
			this.deleteBtn = new global::Gtk.Button ();
			this.deleteBtn.CanFocus = true;
			this.deleteBtn.Name = "deleteBtn";
			this.deleteBtn.UseStock = true;
			this.deleteBtn.UseUnderline = true;
			this.deleteBtn.Label = "gtk-delete";
			this.hbuttonbox3.Add (this.deleteBtn);
			global::Gtk.ButtonBox.ButtonBoxChild w11 = ((global::Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox3 [this.deleteBtn]));
			w11.Expand = false;
			w11.Fill = false;
			this.vbox6.Add (this.hbuttonbox3);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.vbox6 [this.hbuttonbox3]));
			w12.PackType = ((global::Gtk.PackType)(1));
			w12.Position = 2;
			w12.Expand = false;
			w12.Fill = false;
			// Container child vbox6.Gtk.Box+BoxChild
			this.hseparator4 = new global::Gtk.HSeparator ();
			this.hseparator4.Name = "hseparator4";
			this.vbox6.Add (this.hseparator4);
			global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.vbox6 [this.hseparator4]));
			w13.PackType = ((global::Gtk.PackType)(1));
			w13.Position = 3;
			w13.Expand = false;
			w13.Fill = false;
			this.alignment3.Add (this.vbox6);
			this.detailBook.Add (this.alignment3);
			// Notebook tab
			this.generalTabLabel = new global::Gtk.Label ();
			this.generalTabLabel.Name = "generalTabLabel";
			this.generalTabLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("General");
			this.detailBook.SetTabLabel (this.alignment3, this.generalTabLabel);
			this.generalTabLabel.ShowAll ();
			// Container child detailBook.Gtk.Notebook+NotebookChild
			this.alignment5 = new global::Gtk.Alignment (0.5F, 0.5F, 1F, 1F);
			this.alignment5.Name = "alignment5";
			this.alignment5.LeftPadding = ((uint)(24));
			this.alignment5.TopPadding = ((uint)(16));
			this.alignment5.RightPadding = ((uint)(24));
			this.alignment5.BottomPadding = ((uint)(16));
			// Container child alignment5.Gtk.Container+ContainerChild
			this.vbox4 = new global::Gtk.VBox ();
			this.vbox4.Name = "vbox4";
			this.vbox4.Spacing = 6;
			// Container child vbox4.Gtk.Box+BoxChild
			this.countingDirectionLabel = new global::Gtk.Label ();
			this.countingDirectionLabel.Name = "countingDirectionLabel";
			this.countingDirectionLabel.Xalign = 0F;
			this.countingDirectionLabel.LabelProp = "Choose the counting direction";
			this.vbox4.Add (this.countingDirectionLabel);
			global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.countingDirectionLabel]));
			w16.Position = 0;
			w16.Expand = false;
			w16.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.alignment6 = new global::Gtk.Alignment (0.5F, 0.5F, 1F, 1F);
			this.alignment6.Name = "alignment6";
			this.alignment6.LeftPadding = ((uint)(20));
			this.alignment6.TopPadding = ((uint)(5));
			// Container child alignment6.Gtk.Container+ContainerChild
			this.vbox8 = new global::Gtk.VBox ();
			this.vbox8.Name = "vbox8";
			this.vbox8.Spacing = 6;
			// Container child vbox8.Gtk.Box+BoxChild
			this.countForwardOption = new global::Gtk.RadioButton ("Count Forward");
			this.countForwardOption.CanFocus = true;
			this.countForwardOption.Name = "countForwardOption";
			this.countForwardOption.Active = true;
			this.countForwardOption.DrawIndicator = true;
			this.countForwardOption.UseUnderline = true;
			this.countForwardOption.Group = new global::GLib.SList (global::System.IntPtr.Zero);
			this.vbox8.Add (this.countForwardOption);
			global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.vbox8 [this.countForwardOption]));
			w17.Position = 0;
			w17.Expand = false;
			w17.Fill = false;
			// Container child vbox8.Gtk.Box+BoxChild
			this.countBackwardOption = new global::Gtk.RadioButton ("Count Backward");
			this.countBackwardOption.CanFocus = true;
			this.countBackwardOption.Name = "countBackwardOption";
			this.countBackwardOption.DrawIndicator = true;
			this.countBackwardOption.UseUnderline = true;
			this.countBackwardOption.Group = this.countForwardOption.Group;
			this.vbox8.Add (this.countBackwardOption);
			global::Gtk.Box.BoxChild w18 = ((global::Gtk.Box.BoxChild)(this.vbox8 [this.countBackwardOption]));
			w18.Position = 1;
			w18.Expand = false;
			w18.Fill = false;
			this.alignment6.Add (this.vbox8);
			this.vbox4.Add (this.alignment6);
			global::Gtk.Box.BoxChild w20 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.alignment6]));
			w20.Position = 1;
			w20.Expand = false;
			w20.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.hseparator2 = new global::Gtk.HSeparator ();
			this.hseparator2.Name = "hseparator2";
			this.vbox4.Add (this.hseparator2);
			global::Gtk.Box.BoxChild w21 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.hseparator2]));
			w21.Position = 2;
			w21.Expand = false;
			w21.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.watchSpeedContainer = new global::Gtk.HBox ();
			this.watchSpeedContainer.Name = "watchSpeedContainer";
			this.watchSpeedContainer.Spacing = 20;
			// Container child watchSpeedContainer.Gtk.Box+BoxChild
			this.watchSpeedLabel = new global::Gtk.Label ();
			this.watchSpeedLabel.Name = "watchSpeedLabel";
			this.watchSpeedLabel.LabelProp = "Counting Speed";
			this.watchSpeedContainer.Add (this.watchSpeedLabel);
			global::Gtk.Box.BoxChild w22 = ((global::Gtk.Box.BoxChild)(this.watchSpeedContainer [this.watchSpeedLabel]));
			w22.Position = 0;
			w22.Expand = false;
			w22.Fill = false;
			// Container child watchSpeedContainer.Gtk.Box+BoxChild
			this.watchSpeedEntry = new global::Gtk.SpinButton (0, 60, 1);
			this.watchSpeedEntry.CanFocus = true;
			this.watchSpeedEntry.Name = "watchSpeedEntry";
			this.watchSpeedEntry.Adjustment.PageIncrement = 6;
			this.watchSpeedEntry.ClimbRate = 1;
			this.watchSpeedEntry.Digits = ((uint)(3));
			this.watchSpeedEntry.Numeric = true;
			this.watchSpeedEntry.Value = 1;
			this.watchSpeedContainer.Add (this.watchSpeedEntry);
			global::Gtk.Box.BoxChild w23 = ((global::Gtk.Box.BoxChild)(this.watchSpeedContainer [this.watchSpeedEntry]));
			w23.Position = 1;
			this.vbox4.Add (this.watchSpeedContainer);
			global::Gtk.Box.BoxChild w24 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.watchSpeedContainer]));
			w24.Position = 3;
			w24.Expand = false;
			w24.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.applySpeedBtn = new global::Gtk.Button ();
			this.applySpeedBtn.WidthRequest = 40;
			this.applySpeedBtn.CanFocus = true;
			this.applySpeedBtn.Name = "applySpeedBtn";
			this.applySpeedBtn.UseUnderline = true;
			this.applySpeedBtn.Label = "Apply Settings";
			this.vbox4.Add (this.applySpeedBtn);
			global::Gtk.Box.BoxChild w25 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.applySpeedBtn]));
			w25.PackType = ((global::Gtk.PackType)(1));
			w25.Position = 4;
			w25.Expand = false;
			w25.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.hseparator3 = new global::Gtk.HSeparator ();
			this.hseparator3.Name = "hseparator3";
			this.vbox4.Add (this.hseparator3);
			global::Gtk.Box.BoxChild w26 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.hseparator3]));
			w26.PackType = ((global::Gtk.PackType)(1));
			w26.Position = 5;
			w26.Expand = false;
			w26.Fill = false;
			this.alignment5.Add (this.vbox4);
			this.detailBook.Add (this.alignment5);
			global::Gtk.Notebook.NotebookChild w28 = ((global::Gtk.Notebook.NotebookChild)(this.detailBook [this.alignment5]));
			w28.Position = 1;
			// Notebook tab
			this.countingTabLabel = new global::Gtk.Label ();
			this.countingTabLabel.Name = "countingTabLabel";
			this.countingTabLabel.LabelProp = "Counting";
			this.detailBook.SetTabLabel (this.alignment5, this.countingTabLabel);
			this.countingTabLabel.ShowAll ();
			// Container child detailBook.Gtk.Notebook+NotebookChild
			this.alignment1 = new global::Gtk.Alignment (0.5F, 0.5F, 1F, 1F);
			this.alignment1.Name = "alignment1";
			this.alignment1.LeftPadding = ((uint)(24));
			this.alignment1.TopPadding = ((uint)(16));
			this.alignment1.RightPadding = ((uint)(24));
			this.alignment1.BottomPadding = ((uint)(16));
			// Container child alignment1.Gtk.Container+ContainerChild
			this.vbox7 = new global::Gtk.VBox ();
			this.vbox7.Name = "vbox7";
			this.vbox7.Spacing = 6;
			// Container child vbox7.Gtk.Box+BoxChild
			this.configureLayoutLabel = new global::Gtk.Label ();
			this.configureLayoutLabel.Name = "configureLayoutLabel";
			this.configureLayoutLabel.Xalign = 0F;
			this.configureLayoutLabel.LabelProp = "Configure the window layout";
			this.vbox7.Add (this.configureLayoutLabel);
			global::Gtk.Box.BoxChild w29 = ((global::Gtk.Box.BoxChild)(this.vbox7 [this.configureLayoutLabel]));
			w29.Position = 0;
			w29.Expand = false;
			w29.Fill = false;
			// Container child vbox7.Gtk.Box+BoxChild
			this.alignment4 = new global::Gtk.Alignment (0.5F, 0.5F, 1F, 1F);
			this.alignment4.Name = "alignment4";
			this.alignment4.LeftPadding = ((uint)(20));
			this.alignment4.TopPadding = ((uint)(5));
			// Container child alignment4.Gtk.Container+ContainerChild
			this.vbox9 = new global::Gtk.VBox ();
			this.vbox9.Name = "vbox9";
			this.vbox9.Spacing = 6;
			// Container child vbox9.Gtk.Box+BoxChild
			this.dockedCheck = new global::Gtk.CheckButton ();
			this.dockedCheck.CanFocus = true;
			this.dockedCheck.Name = "dockedCheck";
			this.dockedCheck.Label = "Dock in Log Window";
			this.dockedCheck.DrawIndicator = true;
			this.dockedCheck.UseUnderline = true;
			this.vbox9.Add (this.dockedCheck);
			global::Gtk.Box.BoxChild w30 = ((global::Gtk.Box.BoxChild)(this.vbox9 [this.dockedCheck]));
			w30.Position = 0;
			w30.Expand = false;
			w30.Fill = false;
			// Container child vbox9.Gtk.Box+BoxChild
			this.compactCheck = new global::Gtk.CheckButton ();
			this.compactCheck.CanFocus = true;
			this.compactCheck.Name = "compactCheck";
			this.compactCheck.Label = global::Mono.Unix.Catalog.GetString ("Compact Mode");
			this.compactCheck.DrawIndicator = true;
			this.compactCheck.UseUnderline = true;
			this.vbox9.Add (this.compactCheck);
			global::Gtk.Box.BoxChild w31 = ((global::Gtk.Box.BoxChild)(this.vbox9 [this.compactCheck]));
			w31.Position = 1;
			w31.Expand = false;
			w31.Fill = false;
			this.alignment4.Add (this.vbox9);
			this.vbox7.Add (this.alignment4);
			global::Gtk.Box.BoxChild w33 = ((global::Gtk.Box.BoxChild)(this.vbox7 [this.alignment4]));
			w33.Position = 1;
			w33.Expand = false;
			w33.Fill = false;
			// Container child vbox7.Gtk.Box+BoxChild
			this.hbox3 = new global::Gtk.HBox ();
			this.hbox3.Name = "hbox3";
			this.hbox3.Spacing = 6;
			// Container child hbox3.Gtk.Box+BoxChild
			this.windowVisibleBtn = new global::Gtk.ToggleButton ();
			this.windowVisibleBtn.CanFocus = true;
			this.windowVisibleBtn.Name = "windowVisibleBtn";
			this.windowVisibleBtn.UseUnderline = true;
			this.windowVisibleBtn.Label = "Visible";
			this.hbox3.Add (this.windowVisibleBtn);
			global::Gtk.Box.BoxChild w34 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.windowVisibleBtn]));
			w34.Position = 0;
			// Container child hbox3.Gtk.Box+BoxChild
			this.keepAboveCheck = new global::Gtk.CheckButton ();
			this.keepAboveCheck.CanFocus = true;
			this.keepAboveCheck.Name = "keepAboveCheck";
			this.keepAboveCheck.Label = "Keep Above";
			this.keepAboveCheck.DrawIndicator = true;
			this.keepAboveCheck.UseUnderline = true;
			this.hbox3.Add (this.keepAboveCheck);
			global::Gtk.Box.BoxChild w35 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.keepAboveCheck]));
			w35.PackType = ((global::Gtk.PackType)(1));
			w35.Position = 1;
			w35.Expand = false;
			this.vbox7.Add (this.hbox3);
			global::Gtk.Box.BoxChild w36 = ((global::Gtk.Box.BoxChild)(this.vbox7 [this.hbox3]));
			w36.PackType = ((global::Gtk.PackType)(1));
			w36.Position = 2;
			w36.Expand = false;
			w36.Fill = false;
			// Container child vbox7.Gtk.Box+BoxChild
			this.hseparator5 = new global::Gtk.HSeparator ();
			this.hseparator5.Name = "hseparator5";
			this.vbox7.Add (this.hseparator5);
			global::Gtk.Box.BoxChild w37 = ((global::Gtk.Box.BoxChild)(this.vbox7 [this.hseparator5]));
			w37.PackType = ((global::Gtk.PackType)(1));
			w37.Position = 3;
			w37.Expand = false;
			w37.Fill = false;
			this.alignment1.Add (this.vbox7);
			this.detailBook.Add (this.alignment1);
			global::Gtk.Notebook.NotebookChild w39 = ((global::Gtk.Notebook.NotebookChild)(this.detailBook [this.alignment1]));
			w39.Position = 2;
			// Notebook tab
			this.windowTabLabel = new global::Gtk.Label ();
			this.windowTabLabel.Name = "windowTabLabel";
			this.windowTabLabel.LabelProp = "Window";
			this.detailBook.SetTabLabel (this.alignment1, this.windowTabLabel);
			this.windowTabLabel.ShowAll ();
			this.vbox2.Add (this.detailBook);
			global::Gtk.Box.BoxChild w40 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.detailBook]));
			w40.Position = 1;
			// Container child vbox2.Gtk.Box+BoxChild
			this.hbuttonbox2 = new global::Gtk.HButtonBox ();
			this.hbuttonbox2.Name = "hbuttonbox2";
			this.hbuttonbox2.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child hbuttonbox2.Gtk.ButtonBox+ButtonBoxChild
			this.closeBtn = new global::Gtk.Button ();
			this.closeBtn.CanFocus = true;
			this.closeBtn.Name = "closeBtn";
			this.closeBtn.UseStock = true;
			this.closeBtn.UseUnderline = true;
			this.closeBtn.Label = "gtk-close";
			this.hbuttonbox2.Add (this.closeBtn);
			global::Gtk.ButtonBox.ButtonBoxChild w41 = ((global::Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox2 [this.closeBtn]));
			w41.Expand = false;
			w41.Fill = false;
			this.vbox2.Add (this.hbuttonbox2);
			global::Gtk.Box.BoxChild w42 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hbuttonbox2]));
			w42.PackType = ((global::Gtk.PackType)(1));
			w42.Position = 2;
			w42.Expand = false;
			w42.Fill = false;
			this.Add (this.vbox2);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 328;
			this.DefaultHeight = 379;
			this.watchNameButton.HasDefault = true;
			this.Hide ();
			this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.windowDelete_event);
			this.watchNameCombo.Changed += new global::System.EventHandler (this.clockNameChanged_event);
			this.watchNameButton.Clicked += new global::System.EventHandler (this.clockNameBtn_event);
			this.logStartsCheck.Toggled += new global::System.EventHandler (this.logStarts_event);
			this.logStopsCheck.Toggled += new global::System.EventHandler (this.logStops_event);
			this.deleteBtn.Clicked += new global::System.EventHandler (this.deleteClock_event);
			this.applySpeedBtn.Clicked += new global::System.EventHandler (this.setSpeed_event);
			this.dockedCheck.Toggled += new global::System.EventHandler (this.docked_event);
			this.compactCheck.Toggled += new global::System.EventHandler (this.compact_event);
			this.windowVisibleBtn.Toggled += new global::System.EventHandler (this.visible_event);
			this.keepAboveCheck.Toggled += new global::System.EventHandler (this.keepVisible_event);
			this.closeBtn.Clicked += new global::System.EventHandler (this.closeWindow_event);
		}
	}
}
