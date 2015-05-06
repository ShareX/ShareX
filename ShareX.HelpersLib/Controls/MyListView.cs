#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright © 2007-2015 ShareX Developers

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

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class MyListView : ListView
    {
        public delegate void ListViewItemMovedEventHandler(object sender, int oldIndex, int newIndex);
        public event ListViewItemMovedEventHandler ItemMoved;

        private const int WM_PAINT = 0xF;
        private const int WM_ERASEBKGND = 0x14;

        [DefaultValue(false)]
        public bool AutoFillColumn { get; set; }

        [DefaultValue(-1)]
        public int AutoFillColumnIndex { get; set; }

        [DefaultValue(false)]
        public bool AllowColumnSort { get; set; }

        // Note: AllowDrag also need to be true.
        [DefaultValue(false)]
        public bool AllowItemDrag { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedIndex
        {
            get
            {
                if (SelectedIndices.Count > 0)
                {
                    return SelectedIndices[0];
                }

                return -1;
            }
            set
            {
                foreach (ListViewItem lvi in SelectedItems)
                {
                    lvi.Selected = false;
                }

                if (value > -1)
                {
                    Items[value].Selected = true;
                }
            }
        }

        private ListViewColumnSorter lvwColumnSorter;
        private int lineIndex = -1;
        private int lastLineIndex = -1;
        private ListViewItem dragOverItem;

        public MyListView()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.EnableNotifyMessage, true);

            AutoFillColumn = false;
            AutoFillColumnIndex = -1;
            AllowColumnSort = false;
            FullRowSelect = true;
            View = View.Details;

            lvwColumnSorter = new ListViewColumnSorter();
            ListViewItemSorter = lvwColumnSorter;
        }

        public void FillColumn(int index)
        {
            if (View == View.Details && Columns.Count > 0 && index >= -1 && index < Columns.Count)
            {
                if (index == -1)
                {
                    index = Columns.Count - 1;
                }

                int width = 0;

                for (int i = 0; i < Columns.Count; i++)
                {
                    if (i != index) width += Columns[i].Width;
                }

                int columnWidth = ClientSize.Width - width;

                if (columnWidth > 0 && Columns[index].Width != columnWidth)
                {
                    Columns[index].Width = columnWidth;
                }
            }
        }

        public void FillLastColumn()
        {
            FillColumn(-1);
        }

        public void Select(int index)
        {
            if (Items.Count > 0 && index > -1 && index < Items.Count)
            {
                SelectedIndex = index;
            }
        }

        public void SelectLast()
        {
            if (Items.Count > 0)
            {
                SelectedIndex = Items.Count - 1;
            }
        }

        [DebuggerStepThrough]
        protected override void OnNotifyMessage(Message m)
        {
            if (m.Msg == WM_PAINT && !DesignMode && AutoFillColumn)
            {
                FillColumn(AutoFillColumnIndex);
            }

            if (m.Msg != WM_ERASEBKGND)
            {
                base.OnNotifyMessage(m);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (MultiSelect && e.Control && e.KeyCode == Keys.A)
            {
                foreach (ListViewItem lvi in Items)
                {
                    lvi.Selected = true;
                }
            }

            base.OnKeyDown(e);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_PAINT && lineIndex >= 0)
            {
                Rectangle rc = Items[lineIndex < Items.Count ? lineIndex : lineIndex - 1].GetBounds(ItemBoundsPortion.Entire);
                DrawInsertionLine(rc.Left, rc.Right, lineIndex < Items.Count ? rc.Top : rc.Bottom);
            }
        }

        protected override void OnItemDrag(ItemDragEventArgs e)
        {
            base.OnItemDrag(e);

            if (AllowDrop && AllowItemDrag && e.Button == MouseButtons.Left)
            {
                DoDragDrop(e.Item, DragDropEffects.Move);
            }
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);

            ListViewItem lvi = drgevent.Data.GetData(typeof(ListViewItem)) as ListViewItem;

            if (lvi != null && lvi.ListView == this)
            {
                drgevent.Effect = DragDropEffects.Move;

                Point cp = PointToClient(new Point(drgevent.X, drgevent.Y));
                dragOverItem = GetItemAt(cp.X, cp.Y);

                if (dragOverItem != null)
                {
                    lineIndex = dragOverItem.Index;
                }
                else
                {
                    lineIndex = Items.Count;
                }

                if (lineIndex != lastLineIndex)
                {
                    Invalidate();
                }

                lastLineIndex = lineIndex;
            }
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);

            ListViewItem lvi = drgevent.Data.GetData(typeof(ListViewItem)) as ListViewItem;

            if (lvi != null && lvi.ListView == this && lvi != dragOverItem)
            {
                int oldIndex = lvi.Index;
                int newIndex;

                if (dragOverItem != null)
                {
                    newIndex = dragOverItem.Index;

                    if (newIndex > oldIndex)
                    {
                        newIndex--;
                    }
                }
                else
                {
                    newIndex = Items.Count - 1;
                }

                Items.RemoveAt(oldIndex);
                Items.Insert(newIndex, lvi);

                OnItemMoved(oldIndex, newIndex);
            }

            lineIndex = lastLineIndex = -1;
            Invalidate();
        }

        protected void OnItemMoved(int oldIndex, int newIndex)
        {
            if (ItemMoved != null)
            {
                ItemMoved(this, oldIndex, newIndex);
            }
        }

        protected override void OnDragLeave(EventArgs e)
        {
            base.OnDragLeave(e);

            lineIndex = lastLineIndex = -1;
            Invalidate();
        }

        protected override void OnColumnClick(ColumnClickEventArgs e)
        {
            base.OnColumnClick(e);

            if (AllowColumnSort)
            {
                if (e.Column == lvwColumnSorter.SortColumn)
                {
                    if (lvwColumnSorter.Order == SortOrder.Ascending)
                    {
                        lvwColumnSorter.Order = SortOrder.Descending;
                    }
                    else
                    {
                        lvwColumnSorter.Order = SortOrder.Ascending;
                    }
                }
                else
                {
                    lvwColumnSorter.SortColumn = e.Column;
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }

                Sort();
            }
        }

        private void DrawInsertionLine(int left, int right, int y)
        {
            using (Graphics g = CreateGraphics())
            {
                g.DrawLine(SystemPens.HotTrack, left, y, right - 1, y);

                Point[] leftTriangle = new Point[] { new Point(left, y - 4), new Point(left + 7, y), new Point(left, y + 4) };
                g.FillPolygon(SystemBrushes.HotTrack, leftTriangle);

                Point[] rightTriangle = new Point[] { new Point(right, y - 4), new Point(right - 8, y), new Point(right, y + 4) };
                g.FillPolygon(SystemBrushes.HotTrack, rightTriangle);
            }
        }
    }
}