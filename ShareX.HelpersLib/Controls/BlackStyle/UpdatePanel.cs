#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

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

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public partial class UpdatePanel : UserControl
    {
        public UpdatePanel()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rect = ClientRectangle;

            using (Brush backgroundBrush = new LinearGradientBrush(rect, Color.FromArgb(100, 100, 100), Color.FromArgb(70, 70, 70), LinearGradientMode.Vertical))
            {
                g.FillRectangle(backgroundBrush, rect);
            }

            using (Pen topLinePen = new Pen(Color.FromArgb(30, 30, 30)))
            {
                g.DrawLine(topLinePen, rect.Location, new Point(rect.Width, rect.Y));
            }

            using (Pen topLinePen2 = new Pen(Color.FromArgb(125, 125, 125)))
            {
                g.DrawLine(topLinePen2, new Point(rect.X, rect.Y + 1), new Point(rect.Width, rect.Y + 1));
            }

            using (Font font = new Font("Arial", 12))
            {
                string text = "ShareX update is available!";
                Rectangle textRect = rect.LocationOffset(10, 0);
                TextRenderer.DrawText(g, text, font, textRect.LocationOffset(0, 1), Color.Black, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                TextRenderer.DrawText(g, text, font, textRect, Color.White, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }
        }
    }
}