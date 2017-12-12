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

        private ButtonObject confirmButton, cancelButton;
        private int buttonOffset = 12;

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (confirmButton != null)
            {
                confirmButton.Rectangle = new Rectangle(Rectangle.Right - cancelButton.Rectangle.Width - buttonOffset - confirmButton.Rectangle.Width,
                    Rectangle.Bottom + buttonOffset, confirmButton.Rectangle.Width, confirmButton.Rectangle.Height);

                cancelButton.Rectangle = new Rectangle(Rectangle.Right - cancelButton.Rectangle.Width,
                    Rectangle.Bottom + buttonOffset, cancelButton.Rectangle.Width, cancelButton.Rectangle.Height);
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
            confirmButton = new ButtonObject()
            {
                Text = "\u2714",
                ButtonColor = Color.ForestGreen,
                Rectangle = new Rectangle(0, 0, 80, 40),
                Visible = true
            };
            confirmButton.MousePressed += ConfirmButton_MousePressed;
            Manager.DrawableObjects.Add(confirmButton);

            cancelButton = new ButtonObject()
            {
                Text = "\u2716",
                ButtonColor = Color.FromArgb(227, 45, 45),
                Rectangle = new Rectangle(0, 0, 80, 40),
                Visible = true
            };
            cancelButton.MousePressed += CancelButton_MousePressed;
            Manager.DrawableObjects.Add(cancelButton);
        }

        private void ConfirmButton_MousePressed(object sender, MouseEventArgs e)
        {
            Manager.CropArea(RectangleInsideCanvas);
            Remove();
        }

        private void CancelButton_MousePressed(object sender, MouseEventArgs e)
        {
            Remove();
        }

        public override void Dispose()
        {
            base.Dispose();

            if (confirmButton != null)
            {
                Manager.DrawableObjects.Remove(confirmButton);
            }

            if (cancelButton != null)
            {
                Manager.DrawableObjects.Remove(cancelButton);
            }
        }
    }
}