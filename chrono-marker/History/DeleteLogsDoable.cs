//
//  DeleteLogDoable.cs
//
//  Author:
//       Leonardo Augusto Pereira <http://code.google.com/p/chrono-marker/>
//
//  Copyright (c) 2012 Leonardo Augusto Pereira
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
using Mono.Unix;

namespace Chrono
{
	public class DeleteLogsDoable : Doable
	{
		public DeleteLogsDoable(TimeLogger logger, params LogEntry[] logsDeleted)
		{
			if(logger == null)
				throw new ArgumentNullException("logger");

			if(logsDeleted.Length < 0)
				throw new ArgumentException("logsDeleted");

			_logger = logger;
			_logsDeleted = logsDeleted;
		}

		private TimeLogger _logger;
		private LogEntry[] _logsDeleted;

		public override string RedoText {
			get {
				string translatable = Catalog.GetPluralString("Redelete a log", "Redelete {0} logs", _logsDeleted.Length);

				return string.Format(translatable, _logsDeleted.Length);
			}
		}

		public override string UndoText {
			get {
				string translatable = Catalog.GetPluralString("Undelete a log", "Undelete {0} logs", _logsDeleted.Length);

				return string.Format(translatable, _logsDeleted.Length);
			}
		}

		public override void Undo()
		{
			_logger.AddEntry(_logsDeleted);
		}

		public override void Redo()
		{
			_logger.DeleteEntry(_logsDeleted);
		}
	}
}

