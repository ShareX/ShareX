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
using System;
using System.Drawing;

namespace ShareX.ScreenCaptureLib
{
    public class CursorDrawingShape : ImageDrawingShape
    {
        public override ShapeType ShapeType { get; } = ShapeType.DrawingCursor;

        public void UpdateCursor(IntPtr cursorHandle, Point position)
        {
            Icon icon = Icon.FromHandle(cursorHandle);
            Bitmap bmpCursor = icon.ToBitmap();
            UpdateCursor(bmpCursor, position);
        }

        public void UpdateCursor(Bitmap bmpCursor, Point position)
        {
            Dispose();
            Image = bmpCursor;
            Rectangle = new Rectangle(position, Image.Size);
        }

        public override void ShowNodes()
        {
        }

        public override void OnCreating()
        {
            Manager.IsMoving = true;
            UpdateCursor(Manager.GetSelectedCursor().Handle, Manager.Form.ScaledClientMousePosition.Round());
            OnCreated();
        }

        public override void OnDraw(Graphics g)
        {
            if (Image != null)
            {
                g.DrawImage(Image, Rectangle);

                if (!Manager.IsRenderingOutput && Manager.CurrentTool == ShapeType.DrawingCursor)
                {
                    Manager.DrawRegionArea(g, Rectangle.Round(), false);
                }
            }
        }

        public override void Resize(int x, int y, bool fromBottomRight)
        {
            Move(x, y);
        }
    }
}