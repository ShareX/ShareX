#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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

using HelpersLib;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX
{
    public partial class ScreenRegionForm : Form
    {
        private Color color = Color.Red;

        public ScreenRegionForm(Rectangle regionRectangle)
        {
            InitializeComponent();

            Location = new Point(regionRectangle.X - 1, regionRectangle.Y - 1);
            Size = new Size(regionRectangle.Width + 2, regionRectangle.Height + 2);

            Rectangle rect = ClientRectangle;
            Region region = new Region(rect);
            rect.Inflate(-1, -1);
            region.Exclude(rect);
            Region = region;
        }

        public void ChangeColor(Color color)
        {
            this.color = color;
            Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (Pen pen1 = new Pen(Color.Black) { DashPattern = new float[] { 5, 5 } })
            using (Pen pen2 = new Pen(color) { DashPattern = new float[] { 5, 5 }, DashOffset = 5 })
            {
                e.Graphics.DrawRectangleProper(pen1, ClientRectangle);
                e.Graphics.DrawRectangleProper(pen2, ClientRectangle);
            }

            base.OnPaint(e);
        }
    }
}