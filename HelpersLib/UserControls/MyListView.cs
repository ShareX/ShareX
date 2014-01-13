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

using System.ComponentModel;
using System.Diagnostics;
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

        public MyListView()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.EnableNotifyMessage, true);

            AutoFillColumn = false;
            AutoFillColumnIndex = -1;
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
    }
}