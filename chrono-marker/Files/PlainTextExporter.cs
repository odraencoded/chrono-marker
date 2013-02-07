//
//  PlainTextExporter.cs
//
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
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Chrono.Files
{
	// Exports logs as plain text
	public class PlainTextExporter : ILogExporter
	{
		public PlainTextExporter() : this("[{2}].{0,-30}{1,30}") {}

		public PlainTextExporter(string format )
		{
			this.Format = format;
		}

		public string Format {get; set;}
		public string Label { get { return "Plain Text"; } }
		public string Title { get { return "Export Logs as Plain Text"; } }

		public void Export(IEnumerable<LogEntry> logs, string filename) {
			using(StreamWriter writer =
			      new StreamWriter(filename, false, Encoding.Unicode))
			{
				foreach(LogEntry logEntry in logs) {
					writer.WriteLine(this.Format, logEntry.ClockName, logEntry.Description, logEntry.Timestamp);
				}
			}
		}
	}
}
