#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2023 ShareX Team

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
            rtbDecodeResult.AddContextMenu();
            ShareXResources.ApplyTheme(this);

            if (!string.IsNullOrEmpty(text))
            {
                txtQRCode.Text = text;
            }
        }

        public static QRCodeForm EncodeClipboard()
        {
            string text = ClipboardHelpers.GetText(true);

            if (!string.IsNullOrEmpty(text) && TaskHelpers.CheckQRCodeContent(text))
            {
                return new QRCodeForm(text);
            }

            return new QRCodeForm();
        }

        public static QRCodeForm OpenFormDecodeFromFile(string filePath)
        {
            QRCodeForm form = new QRCodeForm();
            form.tcMain.SelectedTab = form.tpDecode;
            form.DecodeFromFile(filePath);
            return form;
        }

        public static QRCodeForm OpenFormDecodeFromScreen()
        {
            QRCodeForm form = Instance;
            form.tcMain.SelectedTab = form.tpDecode;
            form.DecodeFromScreen();
            return form;
        }

        private void QRCodeForm_Shown(object sender, EventArgs e)
        {
            isReady = true;

            txtQRCode.SetWatermark(Resources.QRCodeForm_InputTextToEncode);

            EncodeText(txtQRCode.Text);
        }

        private void ClearQRCode()
        {
            if (pbQRCode.Image != null)
            {
                Image temp = pbQRCode.Image;
                pbQRCode.Image = null;
                temp.Dispose();
            }
        }

        private void EncodeText(string text)
        {
            if (isReady)
            {
                ClearQRCode();

                int size = Math.Min(pbQRCode.Width, pbQRCode.Height);
                pbQRCode.Image = TaskHelpers.CreateQRCode(text, size);
                pbQRCode.BackColor = Color.White;
            }
        }

        private void DecodeImage(Bitmap bmp)
        {
            string output = "";

            string[] results = TaskHelpers.BarcodeScan(bmp);

            if (results != null)
            {
                output = string.Join(Environment.NewLine + Environment.NewLine, results);
            }

            rtbDecodeResult.Text = output;
        }

        private void DecodeFromFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                using (Bitmap bmp = ImageHelpers.LoadImage(filePath))
                {
                    if (bmp != null)
                    {
                        DecodeImage(bmp);
                    }
                }
            }
        }

        private void DecodeFromScreen()
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
                        DecodeImage(bmp);
                    }
                }
            }
            finally
            {
                this.ForceActivate();
            }
        }

        private void QRCodeForm_Resize(object sender, EventArgs e)
        {
            EncodeText(txtQRCode.Text);
        }

        private void txtQRCode_TextChanged(object sender, EventArgs e)
        {
            EncodeText(txtQRCode.Text);
        }

        private void tsmiCopy_Click(object sender, EventArgs e)
        {
            if (pbQRCode.Image != null)
            {
                ClipboardHelpers.CopyImage(pbQRCode.Image);
            }
        }

        private void tsmiSaveAs_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtQRCode.Text))
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = @"PNG (*.png)|*.png|JPEG (*.jpg)|*.jpg|Bitmap (*.bmp)|*.bmp|SVG (*.svg)|*.svg";
                    sfd.FileName = txtQRCode.Text;
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
                            SvgRenderer.SvgImage svgImage = writer.Write(txtQRCode.Text);
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

        private void tsmiUpload_Click(object sender, EventArgs e)
        {
            if (pbQRCode.Image != null)
            {
                Bitmap bmp = (Bitmap)pbQRCode.Image.Clone();
                UploadManager.UploadImage(bmp);
            }
        }

        private void tsmiDecode_Click(object sender, EventArgs e)
        {
            if (pbQRCode.Image != null)
            {
                tcMain.SelectedTab = tpDecode;

                DecodeImage((Bitmap)pbQRCode.Image);
            }
        }

        private void btnDecodeFromScreen_Click(object sender, EventArgs e)
        {
            DecodeFromScreen();
        }

        private void btnDecodeFromFile_Click(object sender, EventArgs e)
        {
            string filePath = ImageHelpers.OpenImageFileDialog();

            DecodeFromFile(filePath);
        }

        private void rtbDecodeResult_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            URLHelpers.OpenURL(e.LinkText);
        }
    }
}