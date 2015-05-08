Chrono Marker is a free and open source stopwatch program developed in C# using Mono and GTK#. It aims to provide an highly interactive, highly convenient, and highly intuitive graphical user interface for a timing, logging and measuring user created events.

Not designed for high precision time measurement but instead quick, simple and easy time logging. Most of this project code is UI related. Little of it is reusable if anything at all. But if it can be of any help, feel free to take a peek.

![http://i.imgur.com/1UmJ1.png](http://i.imgur.com/1UmJ1.png)

Check the Downloads section for the most recent version of the program. The Wiki section contains a Quick Guide which should cover everything you might want to question about the gadget. Any bugs found or features request should be sent through the issue tracker.

Features of Chrono Marker v0.5:
  * Editable stopwatch display, including real time warning of bad formatting, also allows negative time
  * Logs starts/stops and show them in the log window, the logs can be exported as a text file, reordered, copied, deleted, undeleted and reedeleted
  * Multiple stopwatches can be created, configured, renamed and deleted
  * Stopwatches can count down or up in a speed up to 60 seconds per second
  * Closing a stopwatch window doesn't delete a stopwatch, it isn't a bug it's a feature!
  * Can dock stopwatches inside the log window or allow them inside their own window
  * Can keep stopwatches' windows and the log window above other windows, allowing them to be always visible
  * Supports localization using gettext
  * Supports a few time formatting options, including hiding time units, leading zeros, abbreviations and others
  * A compact mode which makes stopwatches as small and simple as they need to be.

This project is a timer application able to log time events. Not designed to be an high precision profiling tool but instead a gadget for logging how long it takes to do whatever.

Planned features:
  * Triggering sounds or scripts after counting down/up to zero
  * Editable log entries
  * Exporting as HTML
  * Save and load stopwatch configurations
  * Loading logs
  * Remembering column settings