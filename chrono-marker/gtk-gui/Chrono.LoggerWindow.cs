
// This file has been generated by the GUI designer. Do not modify.
namespace Chrono
{
	public partial class LoggerWindow
	{
		private global::Gtk.UIManager UIManager;
		private global::Gtk.Action FileAction;
		private global::Gtk.Action quitAction;
		private global::Gtk.Action helpAction;
		private global::Gtk.Action aboutAction;
		private global::Gtk.Action editAction;
		private global::Gtk.Action copyAction;
		private global::Gtk.Action deleteAction;
		private global::Gtk.Action selectAllAction;
		private global::Gtk.Action ExportAction;
		private global::Gtk.Action stopwatchesAction;
		private global::Gtk.Action preferencesAction;
		private global::Gtk.VBox vbox1;
		private global::Gtk.MenuBar menubar1;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView logView;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Chrono.LoggerWindow
			this.UIManager = new global::Gtk.UIManager ();
			global::Gtk.ActionGroup w1 = new global::Gtk.ActionGroup ("Default");
			this.FileAction = new global::Gtk.Action ("FileAction", global::Mono.Unix.Catalog.GetString ("File"), null, null);
			this.FileAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("File");
			w1.Add (this.FileAction, null);
			this.quitAction = new global::Gtk.Action ("quitAction", global::Mono.Unix.Catalog.GetString ("Quit"), null, "gtk-quit");
			this.quitAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Quit");
			w1.Add (this.quitAction, "<Control>q");
			this.helpAction = new global::Gtk.Action ("helpAction", global::Mono.Unix.Catalog.GetString ("Help"), null, null);
			this.helpAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Help");
			w1.Add (this.helpAction, null);
			this.aboutAction = new global::Gtk.Action ("aboutAction", global::Mono.Unix.Catalog.GetString ("About"), null, "gtk-about");
			this.aboutAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("About");
			w1.Add (this.aboutAction, "F1");
			this.editAction = new global::Gtk.Action ("editAction", global::Mono.Unix.Catalog.GetString ("Edit"), null, null);
			this.editAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Edit");
			w1.Add (this.editAction, null);
			this.copyAction = new global::Gtk.Action ("copyAction", global::Mono.Unix.Catalog.GetString ("Copy"), null, "gtk-copy");
			this.copyAction.Sensitive = false;
			this.copyAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Copy");
			w1.Add (this.copyAction, "<Control>c");
			this.deleteAction = new global::Gtk.Action ("deleteAction", global::Mono.Unix.Catalog.GetString ("Delete"), null, "gtk-delete");
			this.deleteAction.Sensitive = false;
			this.deleteAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Delete");
			w1.Add (this.deleteAction, "Delete");
			this.selectAllAction = new global::Gtk.Action ("selectAllAction", global::Mono.Unix.Catalog.GetString ("Select all"), null, null);
			this.selectAllAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Select all");
			w1.Add (this.selectAllAction, "<Control>a");
			this.ExportAction = new global::Gtk.Action ("ExportAction", global::Mono.Unix.Catalog.GetString ("Export"), null, null);
			this.ExportAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Export");
			w1.Add (this.ExportAction, null);
			this.stopwatchesAction = new global::Gtk.Action ("stopwatchesAction", global::Mono.Unix.Catalog.GetString ("Stopwatches..."), null, "gtk-properties");
			this.stopwatchesAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Stopwatches...");
			w1.Add (this.stopwatchesAction, null);
			this.preferencesAction = new global::Gtk.Action ("preferencesAction", global::Mono.Unix.Catalog.GetString ("Preferences..."), null, "gtk-preferences");
			this.preferencesAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Preferences...");
			w1.Add (this.preferencesAction, null);
			this.UIManager.InsertActionGroup (w1, 0);
			this.AddAccelGroup (this.UIManager.AccelGroup);
			this.Name = "Chrono.LoggerWindow";
			this.Title = global::Mono.Unix.Catalog.GetString ("Chrono Marker Logs");
			this.Icon = global::Gdk.Pixbuf.LoadFromResource ("Chrono.icon.ico");
			this.WindowPosition = ((global::Gtk.WindowPosition)(1));
			// Container child Chrono.LoggerWindow.Gtk.Container+ContainerChild
			this.vbox1 = new global::Gtk.VBox ();
			this.vbox1.Name = "vbox1";
			// Container child vbox1.Gtk.Box+BoxChild
			this.UIManager.AddUiFromString ("<ui><menubar name='menubar1'><menu name='FileAction' action='FileAction'><menuitem name='ExportAction' action='ExportAction'/><separator/><menuitem name='quitAction' action='quitAction'/></menu><menu name='editAction' action='editAction'><menuitem name='selectAllAction' action='selectAllAction'/><menuitem name='copyAction' action='copyAction'/><separator/><menuitem name='deleteAction' action='deleteAction'/><separator/><menuitem name='stopwatchesAction' action='stopwatchesAction'/><separator/><menuitem name='preferencesAction' action='preferencesAction'/></menu><menu name='helpAction' action='helpAction'><menuitem name='aboutAction' action='aboutAction'/></menu></menubar></ui>");
			this.menubar1 = ((global::Gtk.MenuBar)(this.UIManager.GetWidget ("/menubar1")));
			this.menubar1.Name = "menubar1";
			this.vbox1.Add (this.menubar1);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.menubar1]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.logView = new global::Gtk.TreeView ();
			this.logView.WidthRequest = 200;
			this.logView.HeightRequest = 200;
			this.logView.CanFocus = true;
			this.logView.Name = "logView";
			this.logView.EnableSearch = false;
			this.logView.Reorderable = true;
			this.GtkScrolledWindow.Add (this.logView);
			this.vbox1.Add (this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.GtkScrolledWindow]));
			w4.PackType = ((global::Gtk.PackType)(1));
			w4.Position = 1;
			this.Add (this.vbox1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 473;
			this.DefaultHeight = 373;
			this.Hide ();
			this.quitAction.Activated += new global::System.EventHandler (this.quitAction_event);
			this.aboutAction.Activated += new global::System.EventHandler (this.aboutAction_event);
			this.copyAction.Activated += new global::System.EventHandler (this.copyAction_event);
			this.deleteAction.Activated += new global::System.EventHandler (this.deleteAction_event);
			this.selectAllAction.Activated += new global::System.EventHandler (this.selectAllAction_event);
			this.ExportAction.Activated += new global::System.EventHandler (this.exportAction_event);
			this.stopwatchesAction.Activated += new global::System.EventHandler (this.editStopwatches_event);
			this.preferencesAction.Activated += new global::System.EventHandler (this.editPreferences_click);
			this.logView.PopupMenu += new global::Gtk.PopupMenuHandler (this.logViewPopup_event);
			this.logView.ButtonPressEvent += new global::Gtk.ButtonPressEventHandler (this.logViewButtonPress_event);
		}
	}
}
