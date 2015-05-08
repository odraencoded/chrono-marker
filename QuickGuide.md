## Introduction ##

This is a quick guide through the 0.5 release of Chrono Marker. If something is not written here, someone probably forgot to write it.

## Requirements ##

Chrono Marker is developed in C#, it uses Gtk# for it's graphical user interface. In order to function, it needs .Net 4.0 or the Mono equivalent, the Gtk# library and the Gtk+ library.

The Gtk# library and Mono can be downloaded [from here](http://www.go-mono.com/mono-downloads/download.html).

For Windows users, the .Net framework can be downloaded [from here](http://www.microsoft.com/net)

## Log Window ##

The _Log Window_ is the main window of Chrono Marker. It contains the main menu of the program and a log of stopwatch events such as starting / stopping.

You can export the logs as a text file clicking on the menu _File_, and then on _Export_. Currently the export dialog doesn't append any extension to the filename. You must add ".txt" if you want it to be opened in windows with notepad by default.

The log list supports multi-selection, copying, deleting and sorting. If you delete something by mistake, you can undelete it, and if you undelete something by mistake, you can redelete it.

Clicking on the menu _Edit_, and then on _Stopwatches_ will open the _Stopwatch Properties dialog_ which lets you add, delete and customize stopwatches.

## Stopwatch Window ##

The _Stopwatch Window_ is composed of a textbox and three buttons. The textbox displays the current stopwatch time. The two side buttons can be used to switch between counting forward and backward. The bottom button can start and stop the stopwatch. In the compact mode only the textbox and an icon version of the bottom button are visible.

You can edit the time in the textbox before starting the stopwatch. If the input doesn't match that format the text will turn red and pressing the start button will undo the changes.

Examples of valid edits: -2:00:00.000, +2:00:00, -3:15, 3m 15s, 15s 3m, 8h20ms, 08:00:00.2

The hard limit of stopwatches are +-7 days in time. Once it reaches that point the stopwatch will stop. It is also impossible to set the stopwatch to more than 7 days, or 168 hours.

## Stopwatch Properties ##

The _Stopwatch Properties dialog_ can be used to create, rename, configure or delete stopwatches.

The _name combo box_ contains a dropdown list of all stopwatches created. Typing in the combo box will instantly change the stopwatch whose properties are displayed. Clicking on a _stopwatch window_ will automatically change the combo box to that window stopwatch name.

If you type something that is not a stopwatch name, pressing enter or clicking on the _create button_ will create a new stopwatch with that name. If you type something that is a stopwatch name, the _create button_ will turn into a _rename button_. Pressing it will display a dialog to rename the specified stopwatch.

The two checkboxes _log starts_ and _log stops_ define whether logs will be created when the _start button_ or _stop button_ are pressed in their stopwatch window.

In the counting tab you can change the counting speed and the counting direction of the stopwatch. After making any changes you must press the _Apply Settings_ button.

In the window tab you can choose to dock the stopwatch's window inside the log window, switch it to the compact mode, or make it be kept above other windows.