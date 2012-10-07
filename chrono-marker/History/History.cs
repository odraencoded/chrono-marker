//
//  History.cs
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

using System.Collections.Generic;

namespace Chrono
{
	public sealed class History
	{
		public History(int capacity)
		{
			manyDoables = new LinkedList<Doable>();

			Capacity = capacity;
			currentDoableNode = manyDoables.AddFirst((Doable)null);
		}

		public int Capacity { get; set; }

		// Yep, it was only for the sake of this pun.
		public Doable Undoable { get { return currentDoableNode.Value; } }
		public Doable Redoable {get { return currentDoableNode.Next.Value; } }

		private LinkedList<Doable> manyDoables;
		private LinkedListNode<Doable> currentDoableNode;

		public void Clear()
		{
			manyDoables.Clear( );
			currentDoableNode = manyDoables.AddFirst( ( Doable )null );
		}

		public bool CanRedo { 
			get { return currentDoableNode != manyDoables.Last; }
		}

		public bool CanUndo {
			get { return currentDoableNode != manyDoables.First; }
		}

		public void Undo()
		{
			currentDoableNode.Value.Undo();
			currentDoableNode = currentDoableNode.Previous;

			OnHistoryChanged();
		}

		public void Redo()
		{
			currentDoableNode.Next.Value.Redo();
			currentDoableNode = currentDoableNode.Next;

			OnHistoryChanged();
		}

		public void Register(Doable doable)
		{
			currentDoableNode = manyDoables.AddAfter( currentDoableNode, doable );

			while(currentDoableNode != manyDoables.Last) manyDoables.RemoveLast();

			if(manyDoables.Count > Capacity + 1)
			{
				do // Do, doables, get it? Get it?
			 	{
					manyDoables.RemoveFirst();
				} while( manyDoables.Count > Capacity + 1);

				manyDoables.First.Value = null;
			}

			OnHistoryChanged();
		}

		private void OnHistoryChanged()
		{
			if( Changed != null )
				Changed( this, new HistoryChangedArgs(this) );
		}
	
		public event HistoryChangedHandler Changed;
	}
}

