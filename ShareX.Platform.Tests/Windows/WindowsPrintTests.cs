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

using ShareX.Platform.Imaging;
using ShareX.Platform.Windows;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using Xunit;

namespace ShareX.Platform.Tests;

[Collection("Windows platform services")]
public sealed class WindowsPrintTests
{
    [WindowsFact]
    public void TopDownPagesKeepTheirColorsAndScaleWithPrinterDpi()
    {
        if (!OperatingSystem.IsWindows()) return;
        foreach (int width in new[] { 1, 3, 7 })
        {
            PixelBuffer page = CreatePage(width);
            byte[] expectedSource = (byte[])page.Pixels.Clone();
            foreach ((float dpiX, float dpiY) in new[] { (100f, 100f), (200f, 300f), (96f, 144f) })
            {
                using Bitmap output = new(32, 32, PixelFormat.Format24bppRgb);
                output.SetResolution(dpiX, dpiY);
                using Graphics graphics = Graphics.FromImage(output);
                graphics.Clear(Color.White);
                Rectangle bounds = new(0, 0, width, 2);
                PrintPageEventArgs args = new(graphics, bounds, bounds, new PageSettings());
                WindowsPrintService.DrawImage(args, page, bounds);
                int drawnWidth = (int)Math.Round(width * dpiX / 100);
                int drawnHeight = (int)Math.Round(2 * dpiY / 100);
                Assert.Equal(Color.Red.ToArgb(), output.GetPixel(0, 0).ToArgb());
                Assert.Equal(Color.Blue.ToArgb(), output.GetPixel(0, drawnHeight - 1).ToArgb());
                Assert.Equal((width == 1 ? Color.Red : Color.Green).ToArgb(), output.GetPixel(drawnWidth - 1, 0).ToArgb());
                Assert.Equal((width == 1 ? Color.Blue : Color.Yellow).ToArgb(), output.GetPixel(drawnWidth - 1, drawnHeight - 1).ToArgb());
                Assert.Equal(Color.White.ToArgb(), output.GetPixel(drawnWidth, 0).ToArgb());
                Assert.Equal(Color.White.ToArgb(), output.GetPixel(0, drawnHeight).ToArgb());
                Assert.Equal(expectedSource, page.Pixels);
            }
        }
    }

    [WindowsFact]
    public void DrawingPagesReleasesDeviceContexts()
    {
        if (!OperatingSystem.IsWindows()) return;
        PixelBuffer page = CreatePage(3);
        using Bitmap output = new(16, 16, PixelFormat.Format24bppRgb);
        output.SetResolution(100, 100);
        using Graphics graphics = Graphics.FromImage(output);
        Rectangle bounds = new(0, 0, 3, 2);
        PrintPageEventArgs args = new(graphics, bounds, bounds, new PageSettings());
        WindowsPrintService.DrawImage(args, page, bounds);
        using Process process = Process.GetCurrentProcess();
        uint before = GetGuiResources(process.Handle, 0);
        for (int i = 0; i < 100; i++) WindowsPrintService.DrawImage(args, page, bounds);
        Assert.Equal(before, GetGuiResources(process.Handle, 0));
        // Graphics can be used again only after the borrowed HDC has been released.
        graphics.FillRectangle(Brushes.Magenta, 4, 4, 1, 1);
        Assert.Equal(Color.Magenta.ToArgb(), output.GetPixel(4, 4).ToArgb());
    }

    [WindowsFact]
    public void RetainedPrinterSettingsAcceptChangingCopiesBackToOne()
    {
        if (!OperatingSystem.IsWindows()) return;
        WindowsPrintService service = new();
        string missingPrinter = "ShareX-test-missing-" + Guid.NewGuid().ToString("N");
        Assert.False(service.IsPrinterInstalled(""));
        Assert.False(service.IsPrinterInstalled(missingPrinter));
        PrintOptions options = new() { PrinterName = missingPrinter, Copies = 3 };
        Assert.Equal(PrintPageSetup.Letter, service.GetPageSetup(options));
        using PrintDocument document = Assert.IsType<PrintDocument>(options.PlatformData);
        Assert.IsType<StandardPrintController>(document.PrintController);
        Assert.Equal(missingPrinter, document.PrinterSettings.PrinterName);
        Assert.Equal(3, document.PrinterSettings.Copies);
        PageSettings settings = document.DefaultPageSettings;
        settings.Landscape = true;
        settings.Color = false;
        settings.PaperSize = new PaperSize("Synthetic", 400, 600);

        foreach (int copies in new[] { 1, int.MaxValue, 0 })
        {
            options.Copies = copies;
            Assert.Equal(PrintPageSetup.Letter, service.GetPageSetup(options));
            Assert.Same(document, options.PlatformData);
            Assert.Same(settings, document.DefaultPageSettings);
            Assert.Equal(Math.Clamp(copies, 1, short.MaxValue), document.PrinterSettings.Copies);
            Assert.True(settings.Landscape);
            Assert.False(settings.Color);
            Assert.Equal("Synthetic", settings.PaperSize.PaperName);
        }
    }

    private static PixelBuffer CreatePage(int width)
    {
        PixelBuffer page = new(width, 2);
        for (int row = 0; row < 2; row++)
        {
            for (int column = 0; column < width; column++)
            {
                Color color = row == 0 ? column == 0 ? Color.Red : Color.Green : column == 0 ? Color.Blue : Color.Yellow;
                int offset = (row * width + column) * 4;
                page.Pixels[offset] = color.B;
                page.Pixels[offset + 1] = color.G;
                page.Pixels[offset + 2] = color.R;
                page.Pixels[offset + 3] = 255;
            }
        }
        return page;
    }

    [DllImport("user32.dll")]
    private static extern uint GetGuiResources(IntPtr process, uint flag);
}
