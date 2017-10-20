#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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

using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace ShareX.HistoryLib
{
    public class HistoryListView : HelpersLib.MyListView
    {
        protected override void OnItemDrag(ItemDragEventArgs e)
        {
            List<string> selection = new List<string>();

            foreach (ListViewItem item in SelectedItems)
            {
                HistoryItem hi = (HistoryItem)item.Tag;
                if (File.Exists(hi.Filepath))
                    selection.Add(hi.Filepath);
            }

            if (selection.Count == 0)
            {
                base.OnItemDrag(e);
                return;
            }

            DataObject data = new DataObject(DataFormats.FileDrop, selection.ToArray());
            DoDragDrop(data, DragDropEffects.Copy);
        }
    }
}
