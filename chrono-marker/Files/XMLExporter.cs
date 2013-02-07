//
//  XMLExporter.cs
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
	public class XMLExporter : ILogExporter
	{
		public string Label { get { return "XML File"; } }
		public string Title { get { return "Export Logs as XML File"; } }

		public void Export(IEnumerable<LogEntry> logs, string filename)
		{
			using( StreamWriter writer =
			      new StreamWriter(filename, false, Encoding.Unicode) ) {
				string timelogsTag = Catalog.GetString( "timelogs" );
				string logTag = Catalog.GetString( "log" );
				string clockAttrib = Catalog.GetString( "clock" );
				string timestampTag = Catalog.GetString( "timestamp" );

				// Declare document
				writer.WriteLine( "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" );
				writer.WriteLine( "<{0}>", timelogsTag );

				foreach( LogEntry logEntry in logs ) {
					// Clock name is set as an attribute
					writer.WriteLine( "\t<{1} {2}=\"{0}\">", EscapeXML(logEntry.ClockName), logTag, clockAttrib );

					// Description goes as content
					writer.WriteLine( "\t\t{0}", EscapeXML(logEntry.Description));

					// The timestamp
					writer.WriteLine( "\t\t<{1}>{0}</{1}>", EscapeXML(logEntry.Timestamp.ToLongTimeString()), timestampTag );
					writer.WriteLine( "\t</{0}>", logTag );
				}

				writer.WriteLine( "</{0}>", timelogsTag );
			}
		}

		public static string EscapeXML(string input){
			return input.Replace("&", "&amp;").Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;");
		}
	}
}