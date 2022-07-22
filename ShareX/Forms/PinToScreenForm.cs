#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2022 ShareX Team

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
using System.Windows.Forms;

namespace ShareX
{
    public partial class PinToScreenForm : Form
    {
        public Image Image { get; private set; }

        private PinToScreenForm()
        {
            InitializeComponent();
        }

        public static void PinToScreen(Image image)
        {
            PinToScreenForm form = new PinToScreenForm();
            form.LoadImage(image);
            form.Show();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                Image?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void LoadImage(Image image)
        {
            pbImage.Reset();

            Image?.Dispose();
            Image = image;

            pbImage.LoadImage(Image);
            Size = Image.Size;
        }

        private void PinToScreenForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                NativeMethods.ReleaseCapture();
                NativeMethods.SendMessage(Handle, (uint)WindowsMessages.NCLBUTTONDOWN, (IntPtr)WindowHitTestRegions.HTCAPTION, IntPtr.Zero);
            }
        }

        private void PinToScreenForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Close();
            }
        }
    }
}