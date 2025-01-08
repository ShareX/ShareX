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
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ShareX
{
    public class TaskRoundedCornerPanel : RoundedCornerPanel
    {
        private bool selected;

        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                if (selected != value)
                {
                    selected = value;

                    Invalidate();
                }
            }
        }

        public Color StatusColor { get; private set; } = Color.Transparent;
        public ThumbnailTitleLocation StatusLocation { get; set; }

        public void UpdateStatusColor(TaskStatus status)
        {
            Color previousStatusColor = StatusColor;

            switch (status)
            {
                case TaskStatus.Completed:
                case TaskStatus.Stopped:
                    StatusColor = Color.CornflowerBlue;
                    break;
                case TaskStatus.Failed:
                    StatusColor = Color.Red;
                    break;
                case TaskStatus.History:
                    StatusColor = Color.Transparent;
                    break;
                default:
                    StatusColor = Color.PaleGreen;
                    break;
            }

            if (previousStatusColor != StatusColor)
            {
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            if (Selected)
            {
                g.PixelOffsetMode = PixelOffsetMode.Default;

                using (Pen pen = new Pen(ShareXResources.Theme.TextColor) { DashStyle = DashStyle.Dot })
                {
                    g.DrawRoundedRectangle(pen, ClientRectangle, Radius);
                }
            }

            if (StatusColor.A > 0)
            {
                g.PixelOffsetMode = PixelOffsetMode.Half;

                int y;

                if (StatusLocation == ThumbnailTitleLocation.Top)
                {
                    y = 0;
                }
                else
                {
                    y = ClientRectangle.Height;
                }

                using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, ClientRectangle.Width, 1), Color.Black, Color.Black,
                    LinearGradientMode.Horizontal))
                {
                    ColorBlend cb = new ColorBlend();
                    cb.Positions = new float[] { 0, 0.3f, 0.7f, 1 };
                    cb.Colors = new Color[] { Color.Transparent, StatusColor, StatusColor, Color.Transparent };
                    brush.InterpolationColors = cb;

                    using (Pen pen = new Pen(brush))
                    {
                        g.DrawLine(pen, new Point(0, y), new Point(ClientRectangle.Width - 1, y));
                    }
                }
            }
        }
    }
}