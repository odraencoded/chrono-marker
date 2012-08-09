/* Copyright (C) 2012 Leonardo Augusto Pereira
 * 
 * This file is part of Chrono Marker 
 * 
 * Chrono Marker is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Chrono Marker is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Chrono Marker.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Chrono
{
	/// <summary>This class is responsible for logging Sandwatch events</summary>
	public sealed class Logger
	{
		public Logger()
		{
			logEntryNodes = new Dictionary<LogEntry, LinkedListNode<LogEntry>>();
			manyLogEntries = new LinkedList<LogEntry>();
			namedHandlers = new Dictionary<string, LoggingHandler>();
		}

        #region Watch list interface
		public LoggingHandler this [string name] {
			get { return namedHandlers [name]; }
		}

		public bool TryGetHandler(string name, out LoggingHandler result) {
			return namedHandlers.TryGetValue(name, out result);
		}

		public LoggingHandler AddWatch(Watch watch, string name)
		{
			if( namedHandlers.ContainsKey( name ) )
				throw new ArgumentException("The name of this watch was already registered.", "watch");

			LoggingHandler result = new LoggingHandler(this, watch, name);
			namedHandlers.Add( name, result );

			if( WatchAdded != null )
				WatchAdded( this, new LoggerWatchEventArgs(this, result, watch) );

			return result;
		}
		public bool HasWatch(string watchName)
		{
			return namedHandlers.ContainsKey(watchName);
		}
		public void RemoveWatch(string watchName)
		{
			LoggingHandler logHandler;

			if( !namedHandlers.TryGetValue( watchName, out logHandler ) )
				return;

			namedHandlers.Remove( watchName );

			if( WatchRemoved != null )
				WatchRemoved( this, new LoggerWatchEventArgs(this, logHandler, logHandler.Watch) );
		}

		public LoggingHandler GetWatchHandler(string name)
		{
			LoggingHandler result;
			if( !namedHandlers.TryGetValue( name, out result ) ) {
				result = AddWatch( new Watch() , name);
			}

			return result;
		}

		public LoggingHandler[] Handlers {
			get {
				LoggingHandler[] result = new LoggingHandler[namedHandlers.Count];

				namedHandlers.Values.CopyTo(result, 0);

				return result;
			}
		}
        #endregion

        #region Log entry manipulation
		public void AddEntry(LogEntry entry)
		{
			if( logEntryNodes.ContainsKey( entry ) )
				throw new ArgumentException("Duplicated entry", "entry");

			LinkedListNode<LogEntry> logNode = manyLogEntries.AddLast( entry );
			logEntryNodes.Add( entry, logNode );

			if( EntryAdded != null )
				EntryAdded( this, new LoggingEventArgs(this, entry) );
		}
		public void RemoveEntry(LogEntry entry)
		{
			LinkedListNode<LogEntry> logNode;

			if( !logEntryNodes.TryGetValue( entry, out logNode ) )
				throw new ArgumentException("Entry does not exist on this logger", "entry");

			manyLogEntries.Remove( logNode );
			logEntryNodes.Remove( entry );

			if( EntryDeleted != null )
				EntryDeleted( this, new LoggingEventArgs(this, entry) );
		}
        #endregion

        #region Events
		public event LoggingEventHandler EntryAdded;
		public event LoggingEventHandler EntryDeleted;
        
		public event LoggerWatchEventHandler WatchAdded;
		public event LoggerWatchEventHandler WatchRemoved;
        #endregion

        #region Detail
		private Dictionary<string, LoggingHandler> namedHandlers;
		private Dictionary<LogEntry, LinkedListNode<LogEntry>> logEntryNodes;
		private LinkedList<LogEntry> manyLogEntries;
        #endregion

		public void ExportTo(string filename)
		{
			using( StreamWriter writer = new StreamWriter(filename, false, Encoding.Unicode) ) {
				List<LogEntry> exportEntries = new List<LogEntry>(manyLogEntries);

				exportEntries.Sort( );
				exportEntries.Reverse( );

				foreach( LogEntry logEntry in exportEntries )
					writer.WriteLine( logEntry );
			}
		}

		#region String conversion
		public static string TimeToString(TimeSpan timespan)
		{
			string result = "";

			if( timespan.Ticks < 0 )
				result += "-";
			timespan = timespan.Duration( );
			result += string.Format( "{0:00}:{1:00}:{2:00}.{3:000}",
                timespan.Hours, timespan.Minutes, timespan.Seconds, timespan.Milliseconds );

			return result;
		}

		public static TimeSpan StringToTime(string input)
		{
			input = input.Trim();

			bool negative;

			int miliseconds = 0;
			int seconds = 0;
			int minutes = 0;
			int hours = 0;

			int miliStart;
			int secondStart;
			int minStart;
			int hourStart;

			int minuteSeparator = input.LastIndexOf( ':' );
			int hourSeparator;
			if( minuteSeparator < 0 )
				hourSeparator = -1;
			else
				hourSeparator = input.IndexOf( ':', 0, minuteSeparator);
			int miliSeparator = input.IndexOf( '.' );

			if(input.StartsWith("-"))
			{
				hourStart=1;
				negative=true;
			}
			else
			{hourStart=0;
				negative=false;
			}

			if( hourSeparator >= 0 ) {
				hours = int.Parse( input.Substring( hourStart, hourSeparator-hourStart ) );
				minStart = hourSeparator + 1;
			} else
				minStart = hourStart;

			if( minuteSeparator >= 0 ) {
				minutes = int.Parse(input.Substring(minStart, minuteSeparator-minStart));
				secondStart = minuteSeparator + 1;
			}else secondStart=minStart;

			if( miliSeparator >= 0 ) {
				seconds=int.Parse(input.Substring(secondStart, miliSeparator-secondStart));

				miliStart=miliSeparator+1;
				miliseconds = int.Parse(input.Substring(miliStart));

				if(miliseconds<10)miliseconds*=100;
				else if(miliseconds<100)miliseconds*=10;
			}
			else seconds=int.Parse(input.Substring(secondStart));

			TimeSpan result = new TimeSpan(0, hours, minutes, seconds, miliseconds);
			if(negative)result=result.Negate();

			return result;
		}

		public static bool IsStringValid(string input)
		{
			input = input.Trim();

			if( string.IsNullOrEmpty( input ) )
				return false;

			bool isNegative=false;
			bool hasHours=false;
			bool hasMinutes=false;
			bool hasMiliseconds=false;

			for(int i =0; i<input.Length; i++)
			{
				char c=input[i];
				if(c=='.')
				{
					if(hasMiliseconds)return false;
					else hasMiliseconds=true;
				}
				else if (c==':')
				{
					if(hasMiliseconds)return false;
					if(hasMinutes)
					{
						if(hasHours)return false;
						else hasHours=true;
					}
					else hasMinutes=true;
				}
				else if(c=='-')
				{
					if(i > 0)return false;
					else isNegative=true;
				}
				else if(char.IsDigit(c) == false) return false;
			}

			if(isNegative)input=input.Substring(1);
			string[] sections = input.Split(':', '.');

			int sectionCount = (hasHours?1:0) + (hasMinutes?1:0) + (hasMiliseconds?1:0) + 1;

			if(sections.Length != sectionCount)return false;

			int hourSection = hasHours? 0 : -1;
			int minuteSection = hasMinutes? hourSection + 1 : -1;
			int secondSection = minuteSection+1;
			int miliSection = hasMiliseconds? secondSection+1 : -1;

			for(int i =0; i<sections.Length; i++)
			{
				string sectionStr = sections[i];
				if(string.IsNullOrWhiteSpace(sectionStr))return false;

				int sectionVal;

				if(!int.TryParse(sectionStr, out sectionVal))return false;

				if(i>0)
				{
					if(i==minuteSection || i==secondSection)
					{
						if(sectionStr.Length != 2)return false;
						if(sectionVal >= 60) return false;
					}

					if(i==miliSection)
					{
						if(sectionStr.Length > 3)return false;
					}
				}
			}

			return true;
		}
		#endregion
	}
}