#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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
using System.Windows.Forms;

namespace ShareX
{
    public partial class SimpleActionsForm : Form
    {
        public List<HotkeyType> Actions { get; set; }

        public SimpleActionsForm()
        {
            InitializeComponent();

            tsMain.Renderer = new CustomToolStripProfessionalRenderer();

            // https://www.medo64.com/2014/01/scaling-toolstrip-with-dpi/
            using (Graphics g = CreateGraphics())
            {
                double scale = Math.Max(g.DpiX, g.DpiY) / 96.0;
                double newScale = ((int)Math.Floor(scale * 100) / 25 * 25) / 100.0;
                if (newScale > 1)
                {
                    int newWidth = (int)(tsMain.ImageScalingSize.Width * newScale);
                    int newHeight = (int)(tsMain.ImageScalingSize.Height * newScale);
                    tsMain.ImageScalingSize = new Size(newWidth, newHeight);
                }
            }

            Actions = new List<HotkeyType>() { HotkeyType.RectangleRegion, HotkeyType.PrintScreen, HotkeyType.LastRegion, HotkeyType.FileUpload, HotkeyType.ClipboardUploadWithContentViewer };

            foreach (HotkeyType action in Actions)
            {
                ToolStripButton tsb = new ToolStripButton();
                tsb.Text = action.GetLocalizedDescription();
                tsb.DisplayStyle = ToolStripItemDisplayStyle.Image;
                tsb.Image = TaskHelpers.GetHotkeyTypeIcon(action);

                tsMain.Items.Add(tsb);
            }
        }

        private void tslTitle_MouseEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.SizeAll;
        }

        private void tslTitle_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void tslTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                NativeMethods.ReleaseCapture();
                NativeMethods.DefWindowProc(Handle, (uint)WindowsMessages.SYSCOMMAND, (UIntPtr)NativeConstants.MOUSE_MOVE, IntPtr.Zero);
            }
        }

        private void tslTitle_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Close();
            }
        }
    }
}