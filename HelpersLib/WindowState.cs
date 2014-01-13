#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HelpersLib
{
    public class WindowState
    {
        public Point Location { get; set; }
        public Size Size { get; set; }
        public bool IsMaximized { get; set; }

        public void SetFormState(Form form)
        {
            if (!Location.IsEmpty)
            {
                form.StartPosition = FormStartPosition.Manual;
                form.Location = Location;
            }

            if (!Size.IsEmpty) form.Size = Size;
            if (IsMaximized) form.WindowState = FormWindowState.Maximized;
        }

        public void GetFormState(Form form)
        {
            WINDOWPLACEMENT wp = new WINDOWPLACEMENT();
            wp.length = Marshal.SizeOf(wp);

            if (NativeMethods.GetWindowPlacement(form.Handle, ref wp))
            {
                Location = wp.rcNormalPosition.Location;
                Size = wp.rcNormalPosition.Size;
                IsMaximized = wp.showCmd == WindowShowStyle.Maximize;
            }
        }

        public void AutoHandleFormState(Form form)
        {
            SetFormState(form);
            form.FormClosing += (sender, e) => GetFormState(form);
        }
    }
}