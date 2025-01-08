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

using ShareX.HelpersLib;
using ShareX.Properties;
using ShareX.ScreenCaptureLib;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ZXing;
using ZXing.QrCode;
using ZXing.Rendering;

namespace ShareX
{
    public partial class QRCodeForm : Form
    {
        private static QRCodeForm instance;

        public static QRCodeForm Instance
        {
            get
            {
                if (instance == null || instance.IsDisposed)
                {
                    instance = new QRCodeForm();
                }

                return instance;
            }
        }

        private bool isReady;

        public QRCodeForm(string text = null)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            if (!string.IsNullOrEmpty(text))
            {
                txtText.Text = text;
            }
        }

        public static QRCodeForm GenerateQRCodeFromClipboard()
        {
            string text = ClipboardHelpers.GetText(true);

            if (!string.IsNullOrEmpty(text) && TaskHelpers.CheckQRCodeContent(text))
            {
                return new QRCodeForm(text);
            }

            return new QRCodeForm();
        }

        public static QRCodeForm OpenFormScanFromImageFile(string filePath)
        {
            QRCodeForm form = new QRCodeForm();
            form.ScanFromImageFile(filePath);
            return form;
        }

        public static QRCodeForm OpenFormScanFromScreen()
        {
            QRCodeForm form = Instance;
            form.ScanFromScreen();
            return form;
        }

        private void ClearQRCode()
        {
            if (pbQRCode.Image != null)
            {
                Image temp = pbQRCode.Image;
                pbQRCode.Reset();
                temp.Dispose();

                pbQRCode.PictureBoxBackColor = BackColor;
            }
        }

        private void GenerateQRCode(string text)
        {
            if (isReady)
            {
                ClearQRCode();

                if (!string.IsNullOrEmpty(text))
                {
                    int size;

                    if (nudQRCodeSize.Value > 0)
                    {
                        size = (int)nudQRCodeSize.Value;
                    }
                    else
                    {
                        size = Math.Min(pbQRCode.Width, pbQRCode.Height);
                    }

                    size = Math.Max(size, 64);

                    Image qrCode = TaskHelpers.GenerateQRCode(text, size);

                    pbQRCode.PictureBoxBackColor = Color.White;
                    pbQRCode.LoadImage(qrCode);
                }
            }
        }

        private void ScanImage(Bitmap bmp)
        {
            string output = "";

            string[] results = TaskHelpers.BarcodeScan(bmp);

            if (results != null)
            {
                output = string.Join(Environment.NewLine + Environment.NewLine, results);
            }

            txtText.Text = output;
        }

        private void ScanFromScreen()
        {
            try
            {
                if (Visible)
                {
                    Hide();
                    Thread.Sleep(250);
                }

                TaskSettings taskSettings = TaskSettings.GetDefaultTaskSettings();

                using (Bitmap bmp = RegionCaptureTasks.GetRegionImage(taskSettings.CaptureSettings.SurfaceOptions))
                {
                    if (bmp != null)
                    {
                        ScanImage(bmp);
                    }
                }
            }
            finally
            {
                this.ForceActivate();
            }
        }

        private void ScanFromImageFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                using (Bitmap bmp = ImageHelpers.LoadImage(filePath))
                {
                    if (bmp != null)
                    {
                        ScanImage(bmp);
                    }
                }
            }
        }

        private void QRCodeForm_Shown(object sender, EventArgs e)
        {
            isReady = true;

            txtText.SetWatermark(Resources.QRCodeForm_InputTextToEncode);

            GenerateQRCode(txtText.Text);
        }

        private void QRCodeForm_Resize(object sender, EventArgs e)
        {
            if (nudQRCodeSize.Value == 0)
            {
                GenerateQRCode(txtText.Text);
            }
        }

        private void btnScanQRCodeFromScreen_Click(object sender, EventArgs e)
        {
            txtText.ResetText();

            ScanFromScreen();
        }

        private void btnScanQRCodeFromImageFile_Click(object sender, EventArgs e)
        {
            txtText.ResetText();

            string filePath = ImageHelpers.OpenImageFileDialog();

            ScanFromImageFile(filePath);
        }

        private void txtText_TextChanged(object sender, EventArgs e)
        {
            GenerateQRCode(txtText.Text);
        }

        private void nudQRCodeSize_ValueChanged(object sender, EventArgs e)
        {
            GenerateQRCode(txtText.Text);
        }

        private void btnCopyImage_Click(object sender, EventArgs e)
        {
            if (pbQRCode.Image != null)
            {
                ClipboardHelpers.CopyImage(pbQRCode.Image);
            }
        }

        private void btnSaveImage_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtText.Text))
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = @"PNG (*.png)|*.png|JPEG (*.jpg)|*.jpg|Bitmap (*.bmp)|*.bmp|SVG (*.svg)|*.svg";
                    sfd.FileName = txtText.Text;
                    sfd.DefaultExt = "png";

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = sfd.FileName;

                        if (filePath.EndsWith("svg", StringComparison.OrdinalIgnoreCase))
                        {
                            BarcodeWriterSvg writer = new BarcodeWriterSvg()
                            {
                                Format = BarcodeFormat.QR_CODE,
                                Options = new QrCodeEncodingOptions()
                                {
                                    Width = pbQRCode.Width,
                                    Height = pbQRCode.Height,
                                    CharacterSet = "UTF-8"
                                }
                            };
                            SvgRenderer.SvgImage svgImage = writer.Write(txtText.Text);
                            File.WriteAllText(filePath, svgImage.Content, Encoding.UTF8);
                        }
                        else
                        {
                            if (pbQRCode.Image != null)
                            {
                                ImageHelpers.SaveImage(pbQRCode.Image, filePath);
                            }
                        }
                    }
                }
            }
        }

        private void btnUploadImage_Click(object sender, EventArgs e)
        {
            if (pbQRCode.Image != null)
            {
                Bitmap bmp = (Bitmap)pbQRCode.Image.Clone();
                TaskHelpers.MainFormUploadImage(bmp);
            }
        }
    }
}