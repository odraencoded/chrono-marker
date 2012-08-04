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
using System.Collections;
using System.Text;
using System.Windows.Forms;

namespace Chrono
{
    class ListViewComparer : IComparer, IComparer<ListViewItem>
    {
        public ListViewComparer() : this(0, SortOrder.None) { }

        public ListViewComparer(int column, SortOrder sorting)
        {
            this.Column = column;
            this.Sorting = sorting;
        }

        public int Column { get; set; }
        public SortOrder Sorting { get; set; }

        public int Compare(ListViewItem x, ListViewItem y)
        {
            switch (Sorting)
            {
                case SortOrder.Ascending:
                    return string.Compare(x.SubItems[Column].Text, y.SubItems[Column].Text);

                case SortOrder.Descending:
                    return string.Compare(y.SubItems[Column].Text, x.SubItems[Column].Text);

                default: return 0;
            }
        }

        public int Compare(object x, object y)
        {
            ListViewItem lx = x as ListViewItem;
            ListViewItem ly = y as ListViewItem;
            if (lx != null && ly != null)
                return Compare(lx, ly);
            else return 0;
        }
    }
}
