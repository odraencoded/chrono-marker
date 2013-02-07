//
//  HTMLExporter.cs
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
using Mono.Unix;

namespace Chrono.Files
{
	public class HTMLExporter : ILogExporter
	{
		public string Label { get { return "HTML Table"; } }
		public string Title { get { return "Export Logs as HTML Table"; } }

		public void Export(IEnumerable<LogEntry> logs, string filename) {
			using (StreamWriter writer =
    			      new StreamWriter(filename, false, Encoding.Unicode)) {

				writer.WriteLine("<!doctype html>");
				writer.WriteLine("<title>{0}</title>", Catalog.GetString("Chrono Marker Logs"));
				writer.WriteLine("<meta charset=\"utf8\">");

				writer.WriteLine("<table>");

				// Write table head
				writer.WriteLine("\t<thead>");
				writer.WriteLine("\t<tr>");
				writer.WriteLine("\t\t<th>{0}", Catalog.GetString("Clock Name"));
				writer.WriteLine("\t\t<th>{0}", Catalog.GetString("Description"));
				writer.WriteLine("\t\t<th>{0}", Catalog.GetString("Timestamp"));

				// Write table body
				bool firstRow = true;
				foreach(LogEntry logEntry in logs){
					if(firstRow) {
						writer.WriteLine("\t<tbody>");
						firstRow = false;
					}

					writer.WriteLine("\t<tr>");
					writer.WriteLine("\t\t<td>{0}", XMLExporter.EscapeXML(logEntry.ClockName));
					writer.WriteLine("\t\t<td>{0}", XMLExporter.EscapeXML(logEntry.Description));
					writer.WriteLine("\t\t<td><time datetime=\"{0}\" >{1}</time>",
					                 XMLExporter.EscapeXML(logEntry.Timestamp.ToString("u")),
					                 XMLExporter.EscapeXML(logEntry.Timestamp.ToLongTimeString()));
				}

				writer.WriteLine("</table>");
			}
		}
	}
}