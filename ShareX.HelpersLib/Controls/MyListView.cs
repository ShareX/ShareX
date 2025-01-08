#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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

        public event ListViewItemMovedEventHandler ItemMoving;
        public event ListViewItemMovedEventHandler ItemMoved;

        [DefaultValue(false)]
        public bool AutoFillColumn { get; set; }

        [DefaultValue(-1)]
        public int AutoFillColumnIndex { get; set; }

        [DefaultValue(false)]
        public bool AllowColumnSort { get; set; }

        // Note: AllowDrag also need to be true.
        [DefaultValue(false)]
        public bool AllowItemDrag { get; set; }

        [DefaultValue(true)]
        public bool AllowSelectAll { get; set; }

        [DefaultValue(false)]
        public bool DisableDeselect { get; set; }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
                UnselectAll();

                if (value > -1)
                {
                    ListViewItem lvi = Items[value];
                    lvi.EnsureVisible();
                    lvi.Selected = true;
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
            AllowSelectAll = true;
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

        public void SelectSingle(ListViewItem lvi)
        {
            UnselectAll();

            if (lvi != null)
            {
                lvi.Selected = true;
            }
        }

        public void SelectAll()
        {
            if (AllowSelectAll && MultiSelect)
            {
                foreach (ListViewItem lvi in Items)
                {
                    lvi.Selected = true;
                }
            }
        }

        public void UnselectAll()
        {
            if (MultiSelect)
            {
                SelectedItems.Clear();
            }
        }

        public void EnsureSelectedVisible()
        {
            if (SelectedItems.Count > 0)
            {
                SelectedItems[0].EnsureVisible();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.A))
            {
                SelectAll();
            }

            base.OnKeyDown(e);
        }

        [DebuggerStepThrough]
        protected override void WndProc(ref Message m)
        {
            if (AutoFillColumn && m.Msg == (int)WindowsMessages.PAINT && !DesignMode)
            {
                FillColumn(AutoFillColumnIndex);
            }

            if (m.Msg == (int)WindowsMessages.ERASEBKGND)
            {
                return;
            }

            if (DisableDeselect && m.Msg >= (int)WindowsMessages.LBUTTONDOWN && m.Msg <= (int)WindowsMessages.MBUTTONDBLCLK)
            {
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                ListViewHitTestInfo hit = HitTest(pos);
                switch (hit.Location)
                {
                    case ListViewHitTestLocations.AboveClientArea:
                    case ListViewHitTestLocations.BelowClientArea:
                    case ListViewHitTestLocations.LeftOfClientArea:
                    case ListViewHitTestLocations.RightOfClientArea:
                    case ListViewHitTestLocations.None:
                        return;
                }
            }

            base.WndProc(ref m);

            if (m.Msg == (int)WindowsMessages.PAINT && lineIndex >= 0)
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

            if (drgevent.Data.GetData(typeof(ListViewItem)) is ListViewItem lvi && lvi.ListView == this)
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

            if (drgevent.Data.GetData(typeof(ListViewItem)) is ListViewItem lvi && lvi.ListView == this && lvi != dragOverItem)
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

                OnItemMoving(oldIndex, newIndex);

                Items.RemoveAt(oldIndex);
                Items.Insert(newIndex, lvi);

                OnItemMoved(oldIndex, newIndex);
            }

            lineIndex = lastLineIndex = -1;
            Invalidate();
        }

        protected void OnItemMoving(int oldIndex, int newIndex)
        {
            ItemMoving?.Invoke(this, oldIndex, newIndex);
        }

        protected void OnItemMoved(int oldIndex, int newIndex)
        {
            ItemMoved?.Invoke(this, oldIndex, newIndex);
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

                // If the column is tagged as a DateTime, then sort by date
                lvwColumnSorter.SortByDate = Columns[e.Column].Tag is DateTime;

                Cursor.Current = Cursors.WaitCursor;
                Sort();
                Cursor.Current = Cursors.Default;
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

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            base.ScaleControl(factor, specified);

            foreach (ColumnHeader column in Columns)
            {
                column.Width = (int)Math.Round(column.Width * factor.Width);
            }
        }
    }
}