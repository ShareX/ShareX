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

using ShareX.HelpersLib;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public partial class ScrollingCaptureRegionForm : Form
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= (int)(WindowStyles.WS_EX_TOPMOST | WindowStyles.WS_EX_TRANSPARENT | WindowStyles.WS_EX_TOOLWINDOW);
                return createParams;
            }
        }

        private Rectangle borderRectangle;
        private Rectangle borderRectangleClient;

        public ScrollingCaptureRegionForm(Rectangle regionRectangle)
        {
            InitializeComponent();

            borderRectangle = regionRectangle.Offset(1);
            borderRectangleClient = new Rectangle(0, 0, borderRectangle.Width, borderRectangle.Height);

            Location = borderRectangle.Location;
            Size = borderRectangle.Size;

            BackColor = Color.Magenta;
            TransparencyKey = Color.Magenta;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (Pen pen1 = new Pen(ShareXResources.UseCustomTheme ? ShareXResources.Theme.BorderColor : Color.Black) { DashPattern = new float[] { 5, 5 } })
            using (Pen pen2 = new Pen(Color.Lime) { DashPattern = new float[] { 5, 5 }, DashOffset = 5 })
            {
                e.Graphics.DrawRectangleProper(pen1, borderRectangleClient);
                e.Graphics.DrawRectangleProper(pen2, borderRectangleClient);
            }

            base.OnPaint(e);
        }
    }
}