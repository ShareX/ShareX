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
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public class InputManager
    {
        public Point MousePosition => mouseState.Position;

        public Point PreviousMousePosition => oldMouseState.Position;

        public Point ClientMousePosition => mouseState.ClientPosition;

        public Point PreviousClientMousePosition => oldMouseState.ClientPosition;

        public Point MouseVelocity => new Point(ClientMousePosition.X - PreviousClientMousePosition.X, ClientMousePosition.Y - PreviousClientMousePosition.Y);

        public bool IsMouseMoved => MouseVelocity.X != 0 || MouseVelocity.Y != 0;

        private MouseState mouseState = new MouseState();
        private MouseState oldMouseState;

        public void Update(Control control)
        {
            oldMouseState = mouseState;
            mouseState.Update(control);
        }

        public bool IsMouseDown(MouseButtons button)
        {
            return mouseState.Buttons.HasFlag(button);
        }

        public bool IsBeforeMouseDown(MouseButtons button)
        {
            return oldMouseState.Buttons.HasFlag(button);
        }

        public bool IsMousePressed(MouseButtons button)
        {
            return IsMouseDown(button) && !IsBeforeMouseDown(button);
        }

        public bool IsMouseReleased(MouseButtons button)
        {
            return !IsMouseDown(button) && IsBeforeMouseDown(button);
        }
    }
}