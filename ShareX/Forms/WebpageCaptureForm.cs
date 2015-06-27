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
using ShareX.ScreenCaptureLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareX
{
    public partial class WebpageCaptureForm : Form
    {
        public event Action<Image> OnImageUploadRequested;
        public event Action<Image> OnImageCopyRequested;

        public bool IsBusy { get; private set; }

        private WebpageCapture webpageCapture;

        public WebpageCaptureForm()
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;
            LoadSettings();
            webpageCapture = new WebpageCapture();
            webpageCapture.CaptureCompleted += webpageCapture_CaptureCompleted;
        }

        private void LoadSettings()
        {
            CheckClipboardURL();

            Size browserSize = Program.Settings.WebpageCaptureBrowserSize;
            if (browserSize.Width == 0) browserSize.Width = Screen.PrimaryScreen.Bounds.Width;
            nudWebpageWidth.Value = browserSize.Width.Between((int)nudWebpageWidth.Minimum, (int)nudWebpageWidth.Maximum);
            if (browserSize.Height == 0) browserSize.Height = Screen.PrimaryScreen.Bounds.Height;
            nudWebpageHeight.Value = browserSize.Height.Between((int)nudWebpageHeight.Minimum, (int)nudWebpageHeight.Maximum);

            nudCaptureDelay.Value = (decimal)Program.Settings.WebpageCaptureDelay.Between((float)nudCaptureDelay.Minimum, (float)nudCaptureDelay.Maximum);
        }

        private void CheckClipboardURL()
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

        private void CleanPictureBox()
        {
            if (pbResult.Image != null)
            {
                pbResult.Image.Dispose();
                pbResult.Image = null;
            }
        }

        private void webpageCapture_CaptureCompleted(Bitmap bmp)
        {
            if (!IsDisposed)
            {
                CleanPictureBox();
                pbResult.Image = bmp;
                btnCapture.Enabled = txtURL.Enabled = btnUpload.Enabled = btnCopy.Enabled = true;
                IsBusy = false;
            }
        }

        private void txtURL_TextChanged(object sender, EventArgs e)
        {
            btnCapture.Enabled = txtURL.TextLength > 0;
        }

        private void nudWebpageWidth_ValueChanged(object sender, EventArgs e)
        {
            Program.Settings.WebpageCaptureBrowserSize.Width = (int)nudWebpageWidth.Value;
        }

        private void nudWebpageHeight_ValueChanged(object sender, EventArgs e)
        {
            Program.Settings.WebpageCaptureBrowserSize.Height = (int)nudWebpageHeight.Value;
        }

        private void nudCaptureDelay_ValueChanged(object sender, EventArgs e)
        {
            Program.Settings.WebpageCaptureDelay = (float)nudCaptureDelay.Value;
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            IsBusy = true;
            btnCapture.Enabled = txtURL.Enabled = btnUpload.Enabled = btnCopy.Enabled = false;
            CleanPictureBox();

            webpageCapture.CaptureDelay = (int)nudCaptureDelay.Value * 1000;
            webpageCapture.CapturePage(txtURL.Text, new Size((int)nudWebpageWidth.Value, (int)nudWebpageWidth.Value));
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            if (pbResult.Image != null)
            {
                Image img = (Image)pbResult.Image.Clone();
                OnImageUploadRequested(img);
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (pbResult.Image != null)
            {
                Image img = (Image)pbResult.Image.Clone();
                OnImageCopyRequested(img);
            }
        }
    }
}