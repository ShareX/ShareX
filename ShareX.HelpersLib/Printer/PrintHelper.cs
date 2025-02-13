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

using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class PrintHelper : IDisposable
    {
        public PrintType PrintType { get; private set; }
        public Image Image { get; private set; }
        public string Text { get; private set; }
        public PrintSettings Settings { get; set; }

        public bool Printable
        {
            get
            {
                return Settings != null && ((PrintType == PrintType.Image && Image != null) ||
                    (PrintType == PrintType.Text && !string.IsNullOrEmpty(Text) && Settings.TextFont != null));
            }
        }

        private PrintDocument printDocument;
        private PrintDialog printDialog;
        private PrintPreviewDialog printPreviewDialog;
        private PrintTextHelper printTextHelper;

        public PrintHelper(Image image)
        {
            PrintType = PrintType.Image;
            Image = image;
            InitPrint();
        }

        public PrintHelper(string text)
        {
            PrintType = PrintType.Text;
            Text = text;
            printTextHelper = new PrintTextHelper();
            printTextHelper.Text = Text;
            InitPrint();
        }

        private void InitPrint()
        {
            printDocument = new PrintDocument();
            printDocument.BeginPrint += printDocument_BeginPrint;
            printDocument.PrintPage += printDocument_PrintPage;
            printDialog = new PrintDialog();
            printDialog.Document = printDocument;
            printDialog.UseEXDialog = true;
            printPreviewDialog = new PrintPreviewDialog();
            printPreviewDialog.Document = printDocument;
        }

        public void Dispose()
        {
            if (printDocument != null) printDocument.Dispose();
            if (printDialog != null) printDialog.Dispose();
            if (printPreviewDialog != null) printPreviewDialog.Dispose();
        }

        public void ShowPreview()
        {
            if (Printable)
            {
                printPreviewDialog.ShowDialog();
            }
        }

        public void TryDefaultPrinterOverride()
        {
            string defaultPrinterName = printDocument.PrinterSettings.PrinterName;

            if (!string.IsNullOrEmpty(Settings.DefaultPrinterOverride))
            {
                printDocument.PrinterSettings.PrinterName = Settings.DefaultPrinterOverride;
            }

            if (!printDocument.PrinterSettings.IsValid)
            {
                printDocument.PrinterSettings.PrinterName = defaultPrinterName;

                MessageBox.Show("Printer \"" + Settings.DefaultPrinterOverride + "\" does not exist. Continuing with Windows default printer, you can set the default printer override in application settings.",
                    "Invalid printer name", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public bool Print()
        {
            if (Printable && (!Settings.ShowPrintDialog || printDialog.ShowDialog() == DialogResult.OK))
            {
                if (PrintType == PrintType.Text)
                {
                    printTextHelper.Font = Settings.TextFont;
                }

                TryDefaultPrinterOverride();
                printDocument.Print();
                return true;
            }

            return false;
        }

        private void printDocument_BeginPrint(object sender, PrintEventArgs e)
        {
            if (PrintType == PrintType.Text)
            {
                printTextHelper.BeginPrint();
            }
        }

        private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (PrintType == PrintType.Image)
            {
                PrintImage(e);
            }
            else if (PrintType == PrintType.Text)
            {
                printTextHelper.Font = Settings.TextFont;
                printTextHelper.PrintPage(e);
            }
        }

        private void PrintImage(PrintPageEventArgs e)
        {
            Rectangle rect = e.PageBounds;
            rect.Inflate(-Settings.Margin, -Settings.Margin);

            Image img;

            if (Settings.AutoRotateImage && ((rect.Width > rect.Height && Image.Width < Image.Height) ||
                (rect.Width < rect.Height && Image.Width > Image.Height)))
            {
                img = (Image)Image.Clone();
                img.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }
            else
            {
                img = Image;
            }

            if (Settings.AutoScaleImage)
            {
                DrawAutoScaledImage(e.Graphics, img, rect, Settings.AllowEnlargeImage, Settings.CenterImage);
            }
            else
            {
                e.Graphics.DrawImage(img, rect, new Rectangle(0, 0, rect.Width, rect.Height), GraphicsUnit.Pixel);
            }
        }

        private void DrawAutoScaledImage(Graphics g, Image img, Rectangle rect, bool allowEnlarge = false, bool centerImage = false)
        {
            double ratio;
            int newWidth, newHeight;

            if (!allowEnlarge && img.Width <= rect.Width && img.Height <= rect.Height)
            {
                ratio = 1.0;
                newWidth = img.Width;
                newHeight = img.Height;
            }
            else
            {
                double ratioX = (double)rect.Width / img.Width;
                double ratioY = (double)rect.Height / img.Height;
                ratio = ratioX < ratioY ? ratioX : ratioY;
                newWidth = (int)(img.Width * ratio);
                newHeight = (int)(img.Height * ratio);
            }

            int newX = rect.X;
            int newY = rect.Y;

            if (centerImage)
            {
                newX += (int)((rect.Width - (img.Width * ratio)) / 2);
                newY += (int)((rect.Height - (img.Height * ratio)) / 2);
            }

            g.SetHighQuality();
            g.DrawImage(img, newX, newY, newWidth, newHeight);
        }
    }
}