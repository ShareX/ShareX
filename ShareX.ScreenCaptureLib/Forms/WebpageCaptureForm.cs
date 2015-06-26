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

using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public partial class WebpageCaptureForm : Form
    {
        public bool IsBusy { get; private set; }

        private WebpageCapture webpageCapture;

        public WebpageCaptureForm()
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;
            CheckClipboard();
            nudWebpageWidth.Value = Screen.PrimaryScreen.Bounds.Width.Between((int)nudWebpageWidth.Minimum, (int)nudWebpageWidth.Maximum);
            nudWebpageHeight.Value = Screen.PrimaryScreen.Bounds.Height.Between((int)nudWebpageHeight.Minimum, (int)nudWebpageHeight.Maximum);
            webpageCapture = new WebpageCapture();
            webpageCapture.CaptureCompleted += webpageCapture_CaptureCompleted;
        }

        private void CheckClipboard()
        {
            if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText();

                if (!string.IsNullOrEmpty(text) && URLHelpers.IsValidURLRegex(text))
                {
                    txtURL.Text = text;
                }
            }
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            IsBusy = true;
            btnCapture.Enabled = txtURL.Enabled = !IsBusy;
            if (pbResult.Image != null)
            {
                pbResult.Image.Dispose();
                pbResult.Image = null;
            }

            webpageCapture.CapturePage(txtURL.Text, new Size((int)nudWebpageWidth.Value, (int)nudWebpageWidth.Value));
        }

        private void webpageCapture_CaptureCompleted(Bitmap bmp)
        {
            pbResult.Image = bmp;

            IsBusy = false;
            btnCapture.Enabled = txtURL.Enabled = !IsBusy;
        }

        private void txtURL_TextChanged(object sender, EventArgs e)
        {
            btnCapture.Enabled = txtURL.TextLength > 0;
        }
    }
}