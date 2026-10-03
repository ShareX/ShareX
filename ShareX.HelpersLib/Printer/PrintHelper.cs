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

using ShareX.AvaloniaUI.Integration;
using ShareX.Platform;
using SkiaSharp;
using System;
using System.Drawing;
using Image = SkiaSharp.SKBitmap;
using MessageBox = ShareX.AvaloniaUI.MessageBox;
using MessageBoxButtons = ShareX.AvaloniaUI.MessageBoxButtons;
using MessageBoxIcon = ShareX.AvaloniaUI.MessageBoxIcon;

namespace ShareX.HelpersLib
{
    /// <summary>Prints an image or text. Pages are rendered here with Skia; <see cref="IPrintService"/> picks the printer and prints them.</summary>
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

        private readonly PrintOptions printOptions = new PrintOptions();
        private PrintTextHelper printTextHelper;

        private static IPrintService PrintService => PlatformServices.Current.Printing;

        public PrintHelper(Image image)
        {
            PrintType = PrintType.Image;
            Image = image;
        }

        public PrintHelper(string text)
        {
            PrintType = PrintType.Text;
            Text = text;
            printTextHelper = new PrintTextHelper();
            printTextHelper.Text = Text;
        }

        public void Dispose() => (printOptions.PlatformData as IDisposable)?.Dispose();

        public void ShowPreview()
        {
            if (Printable) PrintPreviewWindow.ShowPreview(RenderPreviewPage, Print);
        }

        internal (SKBitmap Bitmap, bool HasMore) RenderPreviewPage(int pageIndex)
        {
            PrintPageSetup setup = PrintService.Support.IsSupported ? PrintService.GetPageSetup(printOptions) : PrintPageSetup.Letter;
            Size size = new(setup.Width, setup.Height);
            if (PrintType == PrintType.Image) return (RenderImagePage(Image, size, Settings), false);
            PrintTextHelper renderer = new() { Text = Text, Font = Settings.TextFont };
            renderer.BeginPrint();
            Rectangle margin = ToRectangle(setup.MarginBounds);
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
            if (string.IsNullOrEmpty(Settings.DefaultPrinterOverride))
            {
                return;
            }

            if (PrintService.IsPrinterInstalled(Settings.DefaultPrinterOverride))
            {
                printOptions.PrinterName = Settings.DefaultPrinterOverride;
            }
            else
            {
                MessageBox.Show(string.Format(Localization.Strings.PrintHelper_Invalid_printer_message, Settings.DefaultPrinterOverride),
                    Localization.Strings.PrintHelper_Invalid_printer_name, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public bool Print()
        {
            FeatureSupport support = PrintService.Support;
            if (!support.IsSupported) throw new PlatformNotSupportedException(support.Reason);

            if (Printable && (!Settings.ShowPrintDialog || ShowPrintDialog()))
            {
                if (PrintType == PrintType.Text)
                {
                    printTextHelper.Font = Settings.TextFont;
                }

                TryDefaultPrinterOverride();
                printTextHelper?.BeginPrint();
                PrintService.Print(printOptions, "ShareX", RenderPrintPage);
                return true;
            }

            return false;
        }

        private bool ShowPrintDialog() => DesktopServices.Run(() =>
        {
            printOptions.OwnerWindowHandle = DesktopServices.GetWindow()?.TryGetPlatformHandle()?.Handle.ToInt64() ?? 0;
            return PrintService.ShowPrintDialog(printOptions);
        });

        private PrintedPage RenderPrintPage(PrintPageSetup setup, int pageIndex)
        {
            Size size = new(setup.Width, setup.Height);

            if (PrintType == PrintType.Image)
            {
                using SKBitmap page = RenderImagePage(Image, size, Settings);
                return new PrintedPage(PlatformImageConverter.ToPixelBuffer(page), false);
            }

            printTextHelper.Font = Settings.TextFont;
            using SKBitmap textPage = printTextHelper.RenderPage(size, ToRectangle(setup.MarginBounds), out bool morePages);
            return textPage == null ? null : new PrintedPage(PlatformImageConverter.ToPixelBuffer(textPage), morePages);
        }

        private static Rectangle ToRectangle(PlatformRectangle rectangle) => new(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

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
