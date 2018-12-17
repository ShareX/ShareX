#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

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
using ShareX.ScreenCaptureLib.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public partial class WebpageCaptureForm : Form
    {
        public event Action<Image> ImageUploadRequested;
        public event Action<Image> ImageCopyRequested;

        public WebpageCaptureOptions Options { get; set; }
        public bool IsBusy { get; private set; }

        private WebpageCapture webpageCapture;
        private bool stopRequested;

        public WebpageCaptureForm(WebpageCaptureOptions options)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;
            Options = options;
            LoadSettings();
            webpageCapture = new WebpageCapture();
            webpageCapture.CaptureCompleted += webpageCapture_CaptureCompleted;
        }

        private void LoadSettings()
        {
            CheckClipboardURL();

            Size browserSize = Options.BrowserSize;
            if (browserSize.Width == 0) browserSize.Width = Screen.PrimaryScreen.Bounds.Width;
            nudWebpageWidth.SetValue(browserSize.Width);
            if (browserSize.Height == 0) browserSize.Height = Screen.PrimaryScreen.Bounds.Height;
            nudWebpageHeight.SetValue(browserSize.Height);

            nudCaptureDelay.SetValue((decimal)Options.Delay);

            UpdateControls();
        }

        private void CheckClipboardURL()
        {
            if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText();

                if (URLHelpers.IsValidURL(text))
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

        private void txtURL_TextChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void nudWebpageWidth_ValueChanged(object sender, EventArgs e)
        {
            Options.BrowserSize = new Size((int)nudWebpageWidth.Value, Options.BrowserSize.Height);
        }

        private void nudWebpageHeight_ValueChanged(object sender, EventArgs e)
        {
            Options.BrowserSize = new Size(Options.BrowserSize.Width, (int)nudWebpageHeight.Value);
        }

        private void nudCaptureDelay_ValueChanged(object sender, EventArgs e)
        {
            Options.Delay = (float)nudCaptureDelay.Value;
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            if (IsBusy)
            {
                StopCapture();
            }
            else
            {
                StartCapture();
            }
        }

        private void StartCapture()
        {
            IsBusy = true;
            stopRequested = false;
            UpdateControls();

            lock (this)
            {
                CleanPictureBox();
            }

            webpageCapture.CaptureDelay = (int)nudCaptureDelay.Value * 1000;
            webpageCapture.CapturePage(txtURL.Text, new Size((int)nudWebpageWidth.Value, (int)nudWebpageWidth.Value));
        }

        private void StopCapture()
        {
            IsBusy = false;
            stopRequested = true;
            webpageCapture.Stop();
            UpdateControls();
        }

        private void webpageCapture_CaptureCompleted(Bitmap bmp)
        {
            if (!stopRequested && !IsDisposed)
            {
                lock (this)
                {
                    CleanPictureBox();
                    pbResult.Image = bmp;
                }

                IsBusy = false;
                UpdateControls();
            }
        }

        private void UpdateControls()
        {
            btnCapture.Text = IsBusy ? Resources.WebpageCaptureForm_UpdateControls_Stop : Resources.WebpageCaptureForm_UpdateControls_Capture;
            txtURL.Enabled = btnUpload.Enabled = btnCopy.Enabled = nudWebpageWidth.Enabled = nudWebpageHeight.Enabled = nudCaptureDelay.Enabled = !IsBusy;
            btnCapture.Enabled = txtURL.TextLength > 0;
        }

        protected void OnImageUploadRequested(Image img)
        {
            if (ImageUploadRequested != null)
            {
                ImageUploadRequested(img);
            }
        }

        protected void OnImageCopyRequested(Image img)
        {
            if (ImageCopyRequested != null)
            {
                ImageCopyRequested(img);
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            if (pbResult.Image != null)
            {
                Image img;

                lock (this)
                {
                    img = (Image)pbResult.Image.Clone();
                }

                OnImageUploadRequested(img);
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (pbResult.Image != null)
            {
                Image img;

                lock (this)
                {
                    img = (Image)pbResult.Image.Clone();
                }

                OnImageCopyRequested(img);
            }
        }
    }
}