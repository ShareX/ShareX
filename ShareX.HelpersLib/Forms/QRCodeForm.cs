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

using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public partial class QRCodeForm : BaseForm
    {
        public bool EditMode { get; set; }

        public QRCodeForm(string text = null)
        {
            InitializeComponent();
            ClientSize = new Size(400, 400);

            if (!string.IsNullOrEmpty(text))
            {
                qrMain.Dock = DockStyle.Fill;
                qrMain.Cursor = Cursors.Hand;
                Text += ": " + text;
                qrMain.Text = text;
            }
            else
            {
                EditMode = true;
                txtQRCode.Visible = true;
                txtQRCode.Text = "Text";
                txtQRCode.SelectAll();
            }
        }

        private void txtQRCode_TextChanged(object sender, EventArgs e)
        {
            qrMain.Text = txtQRCode.Text;
        }

        private void QRCodeForm_Resize(object sender, EventArgs e)
        {
            qrMain.Refresh();
        }

        private void qrMain_Click(object sender, EventArgs e)
        {
            if (!EditMode)
            {
                Close();
            }
        }

        private void tsmiCopy_Click(object sender, EventArgs e)
        {
            GraphicsRenderer gRender = new GraphicsRenderer(new FixedModuleSize(20, QuietZoneModules.Two));
            BitMatrix matrix = qrMain.GetQrMatrix();
            using (MemoryStream stream = new MemoryStream())
            {
                gRender.WriteToStream(matrix, ImageFormat.Png, stream);

                using (Image img = Image.FromStream(stream))
                {
                    ClipboardHelpers.CopyImage(img);
                }
            }
        }

        private void tsmiSaveAs_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = @"PNG (*.png)|*.png|JPEG (*.jpg)|*.jpg|Bitmap (*.bmp)|*.bmp|Encapsuled PostScript (*.eps)|*.eps|SVG (*.svg)|*.svg";
                saveFileDialog.FileName = txtQRCode.Text;
                saveFileDialog.DefaultExt = "png";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    if (filePath.EndsWith("eps"))
                    {
                        // Initialize the EPS renderer
                        EncapsulatedPostScriptRenderer renderer = new EncapsulatedPostScriptRenderer(new FixedModuleSize(6, QuietZoneModules.Two), // Modules size is 6/72th inch (72 points = 1 inch)
                            new FormColor(Color.Black), new FormColor(Color.White));
                        BitMatrix matrix = qrMain.GetQrMatrix();
                        using (FileStream fs = new FileStream(filePath, FileMode.Create))
                        {
                            renderer.WriteToStream(matrix, fs);
                        }
                    }
                    else if (filePath.EndsWith("svg"))
                    {
                        // Initialize the EPS renderer
                        SVGRenderer renderer = new SVGRenderer(new FixedModuleSize(6, QuietZoneModules.Two), // Modules size is 6/72th inch (72 points = 1 inch)
                            new FormColor(Color.FromArgb(150, 200, 200, 210)), new FormColor(Color.FromArgb(200, 255, 155, 0)));
                        BitMatrix matrix = qrMain.GetQrMatrix();
                        using (FileStream fs = new FileStream(filePath, FileMode.Create))
                        {
                            renderer.WriteToStream(matrix, fs, false);
                        }
                    }
                    else
                    {
                        GraphicsRenderer gRender = new GraphicsRenderer(new FixedModuleSize(20, QuietZoneModules.Two));
                        BitMatrix matrix = qrMain.GetQrMatrix();
                        using (FileStream fs = new FileStream(filePath, FileMode.Create))
                        {
                            gRender.WriteToStream(matrix, ImageHelpers.GetImageFormat(filePath), fs);
                        }
                    }
                }
            }
        }
    }
}