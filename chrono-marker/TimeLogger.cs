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
	/// <summary>
	/// This class stores log entries containing data about
	/// stopwatch events. It also stores logging handlers which
	/// Create the log entries.
	/// </summary>
	public sealed class TimeLogger
	{
		public TimeLogger(TimeFormatSettings defaultFormat)
		{
			_logEntryNodes = new Dictionary<LogEntry, LinkedListNode<LogEntry>>();
			_manyLogEntries = new LinkedList<LogEntry>();
			_namedHandlers = new Dictionary<string, LoggingHandler>(
				StringComparer.CurrentCultureIgnoreCase);

			DefaultFormatSettings = defaultFormat;
		}

		#region Properties
		public LoggingHandler[] Handlers {
			get {
				LoggingHandler[] result = new LoggingHandler[_namedHandlers.Count];

				_namedHandlers.Values.CopyTo(result, 0);

				return result;
			}
		}

		public TimeFormatSettings DefaultFormatSettings { get; set; }

		private Dictionary<string, LoggingHandler> _namedHandlers;
		private Dictionary<LogEntry, LinkedListNode<LogEntry>> _logEntryNodes;
		private LinkedList<LogEntry> _manyLogEntries;
        #endregion

		#region Events
		public event LoggingEventHandler EntryAdded;
		public event LoggingEventHandler EntryDeleted;
        
		public event ClockHandlerEventHandler ClockAdded;
		public event ClockHandlerEventHandler ClockRemoved;
        #endregion

        #region Clock handler interface
		public bool CanCreateClock(string name)
		{
			if( string.IsNullOrWhiteSpace( name ) ) return false;

			if(HasClock(name)) return false;

			return true;
		}

		/// <summary>
		/// Creates a new clock logging handler with the given name
		/// </summary>
		/// <returns>
		/// The newly created handler
		/// </returns>
		/// <param name='name'>
		/// Name for the handler
		/// </param>
		public LoggingHandler CreateClock(string name)
		{

			if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Blank name");

			if( _namedHandlers.ContainsKey( name ) )
				throw new ArgumentException(string.Format(
					"Handler \"{0}\" already exists", name));

			LoggingHandler result = new LoggingHandler(this, new Clock(), name);
			_namedHandlers.Add( name, result );

			if( ClockAdded != null )
				ClockAdded( this, new ClockHandlerEventArgs(result) );

			return result;
		}
		/// <summary>
		/// Removes a logging handler with the given name
		/// </summary>
		/// <param name='clockName'>
		/// The handler name
		/// </param>
		public void RemoveClock(string clockName)
		{
			LoggingHandler logHandler;

			if( !_namedHandlers.TryGetValue( clockName, out logHandler ) )
				throw new ArgumentException(string.Format(
					"Handler \"{0}\" does not exist", clockName));

			_namedHandlers.Remove( clockName );

			if( ClockRemoved != null )
				ClockRemoved( this, new ClockHandlerEventArgs(logHandler) );
		}

		/// <summary>
		/// Determines whether this instance has a clock handler with the specified clockName.
		/// </summary>
		/// <returns>
		/// <c>true</c> if this instance has a clock handler the specified clockName; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='clockName'>
		/// Name of the clock handler to check.
		/// </param>
		public bool HasClock(string clockName)
		{
			return _namedHandlers.ContainsKey(clockName);
		}

		public bool TryGetHandler(string name, out LoggingHandler result) {
			return _namedHandlers.TryGetValue(name, out result);
		}

		// Tries to move a logHandler from an obsolete key
		// To the current one
		public void RefreshClock(string clockName)
		{
			LoggingHandler logHandler;
			if( !_namedHandlers.TryGetValue( clockName, out logHandler ) )
				throw new ArgumentException(string.Format(
					"Logging handler \"{0}\" does not exist", clockName )
				);

			// Nothing to be done
			if(string.Equals(logHandler.Name, clockName, HandlerNameComparison) )
				return;

			if( _namedHandlers.ContainsKey( logHandler.Name ) )
				throw new ArgumentException(string.Format(
					"Logging handler \"{0}\" already exists", logHandler.Name )
				);

			_namedHandlers.Remove( clockName );
			_namedHandlers.Add( logHandler.Name, logHandler );
		}

		public LoggingHandler GetClockHandler(string name)
		{
			LoggingHandler result;
			if( !_namedHandlers.TryGetValue( name, out result ) ) {
				result = CreateClock(name);
			}

			return result;
		}
        #endregion

        #region Log entry manipulation
		public void AddEntry(LogEntry entry)
		{
			if( _logEntryNodes.ContainsKey( entry ) )
				throw new ArgumentException("Duplicated entry", "entry");

			LinkedListNode<LogEntry> logNode = _manyLogEntries.AddLast( entry );
			_logEntryNodes.Add( entry, logNode );

			if( EntryAdded != null )
				EntryAdded( this, new LoggingEventArgs(this, entry) );
		}
		public void RemoveEntry(LogEntry entry)
		{
			LinkedListNode<LogEntry> logNode;

			if( !_logEntryNodes.TryGetValue( entry, out logNode ) )
				throw new ArgumentException("Entry does not exist on this logger", "entry");

			_manyLogEntries.Remove( logNode );
			_logEntryNodes.Remove( entry );

			if( EntryDeleted != null )
				EntryDeleted( this, new LoggingEventArgs(this, entry) );
		}
        #endregion

		public void ExportTo(string filename)
		{
			using( StreamWriter writer = new StreamWriter(filename, false, Encoding.Unicode) ) {
				List<LogEntry> exportEntries = new List<LogEntry>(_manyLogEntries);

				exportEntries.Sort( );
				exportEntries.Reverse( );

				foreach( LogEntry logEntry in exportEntries )
					writer.WriteLine( logEntry );
			}
		}

		#region String conversion
		/*
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
			input = input.Trim( );

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
			{
				hourStart=0;
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
			input = input.Trim( );

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
				if(string.IsNullOrEmpty(sectionStr))return false;

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
		*/
		public static StringComparison HandlerNameComparison { get { return StringComparison.CurrentCultureIgnoreCase; } }
		#endregion
	}
}