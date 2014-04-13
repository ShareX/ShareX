#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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

namespace HelpersLib
{
    public class MyListView : ListView
    {
        private const int WM_PAINT = 0xF;
        private const int WM_ERASEBKGND = 0x14;

        [DefaultValue(false)]
        public bool AutoFillColumn { get; set; }

        [DefaultValue(-1)]
        public int AutoFillColumnIndex { get; set; }

        [DefaultValue(-1)]
        public int LineIndex { get; set; }

        private ListViewItem SelectedItem = null;
        private ListViewItem DragToItem = null;

        public MyListView()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.EnableNotifyMessage, true);

            this.ApplyDefaultPropertyValues();
            FullRowSelect = true;
            View = View.Details;
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

            if (m.Msg == WM_PAINT && LineIndex >= 0)
            {
                Rectangle rc = Items[LineIndex < Items.Count ? LineIndex : LineIndex - 1].GetBounds(ItemBoundsPortion.Entire);
                DrawInsertionLine(rc.Left, rc.Right, LineIndex < Items.Count ? rc.Top : rc.Bottom);
            }
        }

        protected override void OnItemDrag(ItemDragEventArgs e)
        {
            if (AllowDrop)
            {
                base.OnItemDrag(e);
                if (this.SelectedItems.Count > 0)
                    this.SelectedItem = this.SelectedItems[0];
                this.DoDragDrop(this.SelectedItem, DragDropEffects.Move);
            }
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);
            drgevent.Effect = DragDropEffects.Move;

            Point cp = this.PointToClient(new Point(drgevent.X, drgevent.Y));
            DragToItem = this.GetItemAt(cp.X, cp.Y);

            this.LineIndex = DragToItem != null ? DragToItem.Index : this.Items.Count;
            this.Invalidate();
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);

            if (this.SelectedItem != null)
            {
                ListViewItem insertItem = (ListViewItem)SelectedItem.Clone();
                this.Items.Insert(DragToItem != null ? DragToItem.Index : Items.Count, insertItem);
                this.Items.Remove(SelectedItem);
                this.LineIndex = -1;
            }
        }

        private void DrawInsertionLine(int X1, int X2, int Y)
        {
            using (Graphics g = this.CreateGraphics())
            {
                g.DrawLine(Pens.LightBlue, X1, Y, X2 - 1, Y);

                Point[] leftTriangle = new Point[3] {
                            new Point(X1,      Y-4),
                            new Point(X1 + 7,  Y),
                            new Point(X1,      Y+4)
                        };
                Point[] rightTriangle = new Point[3] {
                            new Point(X2,     Y-4),
                            new Point(X2 - 8, Y),
                            new Point(X2,     Y+4)
                        };
                g.FillPolygon(Brushes.LightBlue, leftTriangle);
                g.FillPolygon(Brushes.LightBlue, rightTriangle);
            }
        }
    }
}