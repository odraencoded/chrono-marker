/* Copyright (C) 2013 Leonardo Augusto Pereira
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
using GLib;
using System.Threading;
using System.Collections.Generic;

namespace Chrono
{
	/// <summary>
	/// This class manages multiple TimeoutHandlers
	/// Under a single frequency
	/// </summary>
	public sealed class FrequentCaller
	{
		public FrequentCaller(uint frequency)
		{
			Frequency = frequency;
			callbackQueue = new LinkedList<TimeoutHandler>();
		}

		public uint Frequency { get; set; }

		// Called once every frequency pulse
		public event TimeoutHandler Handlers {
			add {
				if(!callbackQueue.Contains(value)) {
					if(callbackQueue.Count == 0){
						// Restart timeout
						GLib.Timeout.Add(Frequency, CallbackLoop);
					}

					callbackQueue.AddLast(value);
				}
			}
			remove{
				callbackQueue.Remove(value);
			}
		}
		LinkedList<TimeoutHandler> callbackQueue;

		private bool CallbackLoop()
		{
			LinkedListNode<TimeoutHandler>  callbackIterator, currentNode;
			callbackIterator  = callbackQueue.First;

			while(callbackIterator != null){
				// Moves iterator
				currentNode = callbackIterator;
				callbackIterator = callbackIterator.Next;

				// If the value returned is false, remove the node
				if(!currentNode.Value()) {
					callbackQueue.Remove(currentNode);
				}
			}

			return (callbackQueue.Count > 0);
		}
	}
}

