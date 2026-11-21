#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ShareX;

/// <summary>
/// One row of the main window's thumbnail view. The view virtualizes rows, so only the cards of rows on screen exist.
/// </summary>
internal sealed class ThumbnailRowViewModel
{
    public ObservableCollection<ThumbnailItemViewModel> Items { get; } = new();

    /// <summary>
    /// Changes the row to hold <paramref name="items"/> in order, keeping the cards of items that stay in it.
    /// A task added at the front moves one card into each row and one out, instead of rebuilding every row on screen.
    /// </summary>
    public void SetItems(IReadOnlyList<ThumbnailItemViewModel> items)
    {
        HashSet<ThumbnailItemViewModel> keep = new(items);

        for (int i = Items.Count - 1; i >= 0; i--)
        {
            if (!keep.Contains(Items[i]))
            {
                Items.RemoveAt(i);
            }
        }

        for (int i = 0; i < items.Count; i++)
        {
            if (i < Items.Count && ReferenceEquals(Items[i], items[i]))
            {
                continue;
            }

            int index = Items.IndexOf(items[i]);

            if (index >= 0)
            {
                Items.Move(index, i);
            }
            else
            {
                Items.Insert(i, items[i]);
            }
        }

        while (Items.Count > items.Count)
        {
            Items.RemoveAt(Items.Count - 1);
        }
    }
}
