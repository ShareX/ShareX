#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2019 ShareX Team

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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class RoundedCornerPanel : Panel
    {
        public float Radius
        {
            get
            {
                return radius;
            }
            set
            {
                radius = value;

                Invalidate();
            }
        }

        private float radius;

        public Color PanelColor
        {
            get
            {
                return panelColor;
            }
            set
            {
                panelColor = value;

                Invalidate();
            }
        }

        private Color panelColor;

        public RoundedCornerPanel()
        {
            BackColor = Color.Transparent;

            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.SmoothingMode = SmoothingMode.HighQuality;

            using (SolidBrush brush = new SolidBrush(PanelColor))
            {
                g.DrawRoundedRectangle(brush, ClientRectangle.SizeOffset(1), Radius);
            }
        }
    }
}