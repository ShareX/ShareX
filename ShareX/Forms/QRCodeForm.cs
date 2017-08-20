#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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
using System.IO;
using System.Text;
using System.Windows.Forms;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;
using static ZXing.Rendering.SvgRenderer;

namespace ShareX
{
    public partial class QRCodeForm : Form
    {
        public bool EditMode { get; set; }

        public QRCodeForm(string text = null)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;
            ClientSize = new Size(400, 400);

            if (!string.IsNullOrEmpty(text))
            {
                pbQRCode.Cursor = Cursors.Hand;
                Text += ": " + text;
                txtQRCode.Text = text;
            }
            else
            {
                EditMode = true;
                txtQRCode.Visible = true;

                if (Clipboard.ContainsText())
                {
                    text = Clipboard.GetText();

                    if (URLHelpers.IsValidURL(text))
                    {
                        txtQRCode.Text = text;
                    }
                    else
                    {
                        SetDefaultText();
                    }
                }
                else
                {
                    SetDefaultText();
                }

                txtQRCode.SelectAll();
            }
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
            ClearQRCode();

            if (!string.IsNullOrEmpty(text))
            {
                try
                {
                    BarcodeWriter writer = new BarcodeWriter
                    {
                        Format = BarcodeFormat.QR_CODE,
                        Options = new EncodingOptions
                        {
                            Width = pbQRCode.Width,
                            Height = pbQRCode.Height
                        },
                        Renderer = new BitmapRenderer()
                    };

                    pbQRCode.Image = writer.Write(text);
                }
                catch (Exception e)
                {
                    e.ShowError();
                }
            }
        }

        private void SetDefaultText()
        {
            txtQRCode.Text = "Input text to convert";
        }

        private void QRCodeForm_Resize(object sender, EventArgs e)
        {
            EncodeText(txtQRCode.Text);
        }

        private void txtQRCode_TextChanged(object sender, EventArgs e)
        {
            EncodeText(txtQRCode.Text);
        }

        private void pbQRCode_Click(object sender, EventArgs e)
        {
            if (!EditMode)
            {
                Close();
            }
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
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = @"PNG (*.png)|*.png|JPEG (*.jpg)|*.jpg|Bitmap (*.bmp)|*.bmp|SVG (*.svg)|*.svg";
                    saveFileDialog.FileName = txtQRCode.Text;
                    saveFileDialog.DefaultExt = "png";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = saveFileDialog.FileName;

                        if (filePath.EndsWith("svg", StringComparison.InvariantCultureIgnoreCase))
                        {
                            BarcodeWriterSvg writer = new BarcodeWriterSvg
                            {
                                Format = BarcodeFormat.QR_CODE,
                                Options = new EncodingOptions
                                {
                                    Width = pbQRCode.Width,
                                    Height = pbQRCode.Height
                                }
                            };
                            SvgImage svgImage = writer.Write(txtQRCode.Text);
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
    }
}