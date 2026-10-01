#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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

using SkiaSharp;
using System;
using System.Drawing;
using System.Drawing.Printing;
using Image = SkiaSharp.SKBitmap;
using MessageBox = ShareX.AvaloniaUI.MessageBox;
using MessageBoxButtons = ShareX.AvaloniaUI.MessageBoxButtons;
using MessageBoxIcon = ShareX.AvaloniaUI.MessageBoxIcon;

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
            if (OperatingSystem.IsWindows())
            {
                printDocument = new PrintDocument { PrintController = new StandardPrintController() };
                printDocument.BeginPrint += printDocument_BeginPrint;
                printDocument.PrintPage += printDocument_PrintPage;
            }
        }

        public void Dispose() => printDocument?.Dispose();

        public void ShowPreview()
        {
            if (Printable) PrintPreviewWindow.ShowPreview(RenderPreviewPage, Print);
        }

        internal (SKBitmap Bitmap, bool HasMore) RenderPreviewPage(int pageIndex)
        {
            PageSettings pageSettings = printDocument?.PrinterSettings.IsValid == true ? printDocument.DefaultPageSettings : null;
            Size size = pageSettings?.Bounds.Size ?? new Size(850, 1100);
            if (PrintType == PrintType.Image) return (RenderImagePage(Image, size, Settings), false);
            PrintTextHelper renderer = new() { Text = Text, Font = Settings.TextFont };
            renderer.BeginPrint();
            Margins margins = pageSettings?.Margins ?? new Margins(100, 100, 100, 100);
            Rectangle margin = new(margins.Left, margins.Top, size.Width - margins.Left - margins.Right, size.Height - margins.Top - margins.Bottom);
            SKBitmap page = null;
            bool hasMore = false;
            for (int index = 0; index <= pageIndex; index++)
            {
                page?.Dispose();
                page = renderer.RenderPage(size, margin, out hasMore);
                if (!hasMore) break;
            }
            return (page, hasMore);
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

                MessageBox.Show(string.Format(Localization.Strings.PrintHelper_Invalid_printer_message, Settings.DefaultPrinterOverride),
                    Localization.Strings.PrintHelper_Invalid_printer_name, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public bool Print()
        {
            if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException("A printing backend is required for this platform.");
            if (Printable && (!Settings.ShowPrintDialog || WindowsPrintDialog.Show(printDocument)))
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

        private void PrintImage(PrintPageEventArgs args)
        {
            using SKBitmap page = RenderImagePage(Image, args.PageBounds.Size, Settings);
            WindowsPrintInterop.DrawImage(args, page, args.PageBounds);
        }

        internal static SKBitmap RenderImagePage(SKBitmap source, Size pageSize, PrintSettings settings)
        {
            SKBitmap rotated = null;
            SKBitmap image = source;
            SKBitmap page = SkiaImageHelpers.CreateBitmap(pageSize.Width * 3, pageSize.Height * 3);
            Rectangle rectangle = new(0, 0, pageSize.Width, pageSize.Height);
            rectangle.Inflate(-settings.Margin, -settings.Margin);
            try
            {
                using SKCanvas canvas = new(page);
                canvas.Clear(SKColors.White);
                if (rectangle.Width <= 0 || rectangle.Height <= 0) return page;
                if (settings.AutoRotateImage && ((rectangle.Width > rectangle.Height && source.Width < source.Height) ||
                    (rectangle.Width < rectangle.Height && source.Width > source.Height)))
                {
                    rotated = source.Copy();
                    SkiaImageHelpers.RotateFlipInPlace(rotated, 1);
                    image = rotated;
                }
                canvas.Scale(3, 3);
                canvas.ClipRect(rectangle.ToSKRect());
                if (settings.AutoScaleImage)
                {
                    float scale = Math.Min(rectangle.Width / (float)image.Width, rectangle.Height / (float)image.Height);
                    if (!settings.AllowEnlargeImage) scale = Math.Min(1, scale);
                    float width = image.Width * scale, height = image.Height * scale;
                    float x = rectangle.X, y = rectangle.Y;
                    if (settings.CenterImage)
                    {
                        x += (rectangle.Width - width) / 2;
                        y += (rectangle.Height - height) / 2;
                    }
                    canvas.DrawImage(image, x, y, width, height);
                }
                else
                {
                    canvas.DrawBitmap(image, rectangle.X, rectangle.Y);
                }
                return page;
            }
            catch { page.Dispose(); throw; }
            finally { rotated?.Dispose(); }
        }
    }
}
