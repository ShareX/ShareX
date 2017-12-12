#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public class CropTool : BaseTool
    {
        public override ShapeType ShapeType { get; } = ShapeType.ToolCrop;

        private Size buttonSize = new Size(100, 35);
        private int buttonOffset = 10;
        private ConfirmButton confirmButton;

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (confirmButton != null)
            {
                confirmButton.Rectangle = new Rectangle(Rectangle.Right - buttonSize.Width, Rectangle.Bottom + buttonOffset,
                    buttonSize.Width, buttonSize.Height);
            }
        }

        public override void OnDraw(Graphics g)
        {
            if (IsValidShape)
            {
                Manager.DrawRegionArea(g, RectangleInsideCanvas, true);
            }
        }

        public override void OnCreated()
        {
            confirmButton = new ConfirmButton()
            {
                Visible = true
            };
            confirmButton.MousePressed += ConfirmButton_MousePressed;
            Manager.DrawableObjects.Add(confirmButton);
        }

        private void ConfirmButton_MousePressed(object sender, MouseEventArgs e)
        {
            if (IsValidShape)
            {
                Rectangle = RectangleInsideCanvas;
                Manager.CropArea(Rectangle);
            }

            Remove();
        }

        public override void Dispose()
        {
            base.Dispose();

            if (confirmButton != null)
            {
                Manager.DrawableObjects.Remove(confirmButton);
            }
        }
    }
}