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

using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public class CursorDrawingShape : BaseDrawingShape
    {
        public override ShapeType ShapeType { get; } = ShapeType.DrawingCursor;

        public Cursor Cursor { get; private set; }

        public CursorDrawingShape()
        {
        }

        public void SetCursor(CursorData cursorData)
        {
            Dispose();
            Cursor = new Cursor(cursorData.IconHandle);
            Rectangle = new Rectangle(cursorData.Position, Cursor.Size);
        }

        public override void ShowNodes()
        {
        }

        public override void OnCreating()
        {
            Manager.IsMoving = true;
            Dispose();
            Cursor = new Cursor(Cursors.Arrow.CopyHandle());
            Point pos = InputManager.MousePosition0Based;
            Rectangle = new Rectangle(pos, Cursor.Size);
        }

        public override void OnDraw(Graphics g)
        {
            if (Cursor != null)
            {
                Cursor.Draw(g, Rectangle);

                if (!Manager.IsRenderingOutput && Manager.CurrentShapeType == ShapeType.DrawingCursor)
                {
                    Manager.DrawRegionArea(g, Rectangle, false);
                }
            }
        }

        public override void Resize(int x, int y, bool fromBottomRight)
        {
            Move(x, y);
        }

        public override void Dispose()
        {
            if (Cursor != null)
            {
                Cursor.Dispose();
            }
        }
    }
}