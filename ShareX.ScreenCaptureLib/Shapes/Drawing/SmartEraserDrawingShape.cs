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

using System.Drawing;

namespace ShareX.ScreenCaptureLib
{
    public class SmartEraserDrawingShape : BaseDrawingShape
    {
        public override ShapeType ShapeType { get; } = ShapeType.DrawingSmartEraser;

        private Color eraserColor;
        private Color eraserDimmedColor;

        public override void OnConfigLoad()
        {
        }

        public override void OnConfigSave()
        {
        }

        public override void OnCreating()
        {
            base.OnCreating();

            eraserColor = Manager.GetCurrentColor();

            if (eraserColor.IsEmpty)
            {
                eraserColor = Color.White;
            }

            if (Manager.Form.DimmedCanvas != null)
            {
                eraserDimmedColor = Manager.GetCurrentColor(Manager.Form.DimmedCanvas);
            }
        }

        public override void OnDraw(Graphics g)
        {
            Color color;

            if (!Manager.IsRenderingOutput && !eraserDimmedColor.IsEmpty)
            {
                color = eraserDimmedColor;
            }
            else
            {
                color = eraserColor;
            }

            using (Brush brush = new SolidBrush(color))
            {
                g.FillRectangle(brush, Rectangle);
            }
        }
    }
}