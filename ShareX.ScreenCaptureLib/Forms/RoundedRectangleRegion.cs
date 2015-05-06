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

using ShareX.HelpersLib;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public class RoundedRectangleRegion : RectangleRegion
    {
        public float Radius { get; set; }
        public int RadiusIncrement { get; set; }

        public RoundedRectangleRegion()
        {
            Radius = 25;
            RadiusIncrement = 3;
            KeyDown += RoundedRectangleRegion_KeyDown;
        }

        private void RoundedRectangleRegion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Add)
            {
                Radius += RadiusIncrement;
            }
            else if (e.KeyData == Keys.Subtract)
            {
                Radius = Math.Max(0, Radius - RadiusIncrement);
            }
        }

        protected override void AddShapePath(GraphicsPath graphicsPath, Rectangle rect)
        {
            graphicsPath.AddRoundedRectangle(rect, Radius);
        }

        protected override void WriteTips(StringBuilder sb)
        {
            base.WriteTips(sb);

            sb.AppendLine("[Numpad +] [Numpad -] Change corner radius");
        }
    }
}