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
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class WindowState
    {
        public Point Location { get; set; }
        public Size Size { get; set; }
        public bool IsMaximized { get; set; }

        public void ApplyFormState(Form form)
        {
            if (!Location.IsEmpty && !Size.IsEmpty && CaptureHelpers.GetScreenWorkingArea().Contains(new Rectangle(Location, Size)))
            {
                form.StartPosition = FormStartPosition.Manual;
                form.Location = Location;
                form.Size = Size;
            }

            if (IsMaximized)
            {
                form.WindowState = FormWindowState.Maximized;
            }
        }

        public void UpdateFormState(Form form)
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
            ApplyFormState(form);
            form.FormClosing += (sender, e) => UpdateFormState(form);
        }
    }
}