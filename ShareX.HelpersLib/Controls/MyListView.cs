#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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
        private const int WM_PAINT = 0xF;
        private const int WM_ERASEBKGND = 0x14;

        [DefaultValue(false)]
        public bool AutoFillColumn { get; set; }

        [DefaultValue(-1)]
        public int AutoFillColumnIndex { get; set; }

        [DefaultValue(false)]
        public bool AllowColumnSort { get; set; }

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

            if (AllowDrop && e.Button == MouseButtons.Left)
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
                lineIndex = dragOverItem != null ? dragOverItem.Index : Items.Count;

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

            if (lvi != null && lvi.ListView == this)
            {
                ListViewItem insertItem = (ListViewItem)lvi.Clone();
                Items.Insert(dragOverItem != null ? dragOverItem.Index : Items.Count, insertItem);
                Items.Remove(lvi);
            }

            lineIndex = lastLineIndex = -1;
            Invalidate();
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

        private void DrawInsertionLine(int x1, int x2, int y)
        {
            using (Graphics g = CreateGraphics())
            {
                g.DrawLine(Pens.LightBlue, x1, y, x2 - 1, y);

                Point[] leftTriangle = new Point[] { new Point(x1, y - 4), new Point(x1 + 7, y), new Point(x1, y + 4) };
                g.FillPolygon(Brushes.LightBlue, leftTriangle);

                Point[] rightTriangle = new Point[] { new Point(x2, y - 4), new Point(x2 - 8, y), new Point(x2, y + 4) };
                g.FillPolygon(Brushes.LightBlue, rightTriangle);
            }
        }
    }
}