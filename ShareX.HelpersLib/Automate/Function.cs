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

using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class Function
    {
        public string Name { get; set; }
        public string[] Parameters { get; set; }
        public int Loop { get; set; }

        public FunctionManager FunctionManager { protected get; set; }

        public Function()
        {
            Loop = 1;
        }

        public virtual void Prepare()
        {
        }

        public virtual void Method()
        {
        }
    }

    public class Function_Call : Function
    {
        public override void Prepare()
        {
            int index;

            if (!int.TryParse(Parameters[0], out index))
            {
                index = FunctionManager.FindFunctionIndex(Parameters[0]);

                if (index > -1)
                {
                    Parameters[0] = (index + 1).ToString();
                }
            }
        }

        public override void Method()
        {
            int lineIndex;

            if (int.TryParse(Parameters[0], out lineIndex) && lineIndex > 0)
            {
                FunctionManager.Run(lineIndex);
            }
        }
    }

    public class Function_Wait : Function
    {
        public override void Method()
        {
            int delay;

            if (int.TryParse(Parameters[0], out delay))
            {
                Thread.Sleep(delay);
            }
        }
    }

    public class Function_KeyDown : Function
    {
        public override void Method()
        {
            VirtualKeyCode keyCode = (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), Parameters[0], true);
            InputHelpers.SendKeyDown(keyCode);
        }
    }

    public class Function_KeyUp : Function
    {
        public override void Method()
        {
            VirtualKeyCode keyCode = (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), Parameters[0], true);
            InputHelpers.SendKeyUp(keyCode);
        }
    }

    public class Function_KeyPress : Function
    {
        public override void Method()
        {
            VirtualKeyCode keyCode = (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), Parameters[0], true);
            InputHelpers.SendKeyPress(keyCode);
        }
    }

    public class Function_KeyPressText : Function
    {
        public override void Method()
        {
            InputHelpers.SendKeyPressText(Parameters[0]);
        }
    }

    public class Function_MouseDown : Function
    {
        public override void Method()
        {
            MouseButtons button = (MouseButtons)Enum.Parse(typeof(MouseButtons), Parameters[0], true);
            InputHelpers.SendMouseDown(button);
        }
    }

    public class Function_MouseUp : Function
    {
        public override void Method()
        {
            MouseButtons button = (MouseButtons)Enum.Parse(typeof(MouseButtons), Parameters[0], true);
            InputHelpers.SendMouseUp(button);
        }
    }

    public class Function_MouseClick : Function
    {
        public override void Method()
        {
            MouseButtons button;

            if (Parameters.Length == 3)
            {
                int x, y;

                if (int.TryParse(Parameters[0], out x) && int.TryParse(Parameters[1], out y))
                {
                    button = (MouseButtons)Enum.Parse(typeof(MouseButtons), Parameters[2], true);
                    InputHelpers.SendMouseClick(x, y, button);
                }
            }
            else
            {
                button = (MouseButtons)Enum.Parse(typeof(MouseButtons), Parameters[0], true);
                InputHelpers.SendMouseClick(button);
            }
        }
    }

    public class Function_MouseMove : Function
    {
        public override void Method()
        {
            int x, y;

            if (int.TryParse(Parameters[0], out x) && int.TryParse(Parameters[1], out y))
            {
                InputHelpers.SendMouseMove(x, y);
            }
        }
    }

    public class Function_MouseWheel : Function
    {
        public override void Method()
        {
            int delta;

            if (int.TryParse(Parameters[0], out delta))
            {
                InputHelpers.SendMouseWheel(delta);
            }
        }
    }
}