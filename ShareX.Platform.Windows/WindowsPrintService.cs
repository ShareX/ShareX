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
using ShareX.Platform.Windows.Native;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace ShareX.Platform.Windows;

/// <summary>Printing with System.Drawing.Printing and the PrintDlgEx dialog, as ShareX's PrintHelper did.</summary>
[SupportedOSPlatform("windows")]
public sealed class WindowsPrintService : IPrintService
{
    public FeatureSupport Support => FeatureSupport.Supported;

    public FeatureSupport DialogSupport => FeatureSupport.Supported;

    public bool IsPrinterInstalled(string printerName)
    {
        if (string.IsNullOrEmpty(printerName))
        {
            return false;
        }

        PrinterSettings settings = new PrinterSettings { PrinterName = printerName };
        return settings.IsValid;
    }

    public PrintPageSetup GetPageSetup(PrintOptions options)
    {
        PrintDocument document = GetDocument(options);

        if (!document.PrinterSettings.IsValid)
        {
            return PrintPageSetup.Letter;
        }

        PageSettings page = document.DefaultPageSettings;
        Rectangle bounds = page.Bounds;
        Margins margins = page.Margins;
        return new PrintPageSetup(bounds.Width, bounds.Height,
            new PlatformRectangle(margins.Left, margins.Top, bounds.Width - margins.Left - margins.Right, bounds.Height - margins.Top - margins.Bottom));
    }

    public bool ShowPrintDialog(PrintOptions options)
    {
        PrintDocument document = GetDocument(options);
        PrinterSettings printerSettings = document.PrinterSettings;

        PrintDialogData data = new()
        {
            Size = (uint)Marshal.SizeOf<PrintDialogData>(),
            Owner = (IntPtr)options.OwnerWindowHandle,
            Flags = 0x100 | 0x4 | 0x8 | 0x40000 | 0x800000, // PD_RETURNDC | PD_NOSELECTION | PD_NOPAGENUMS | PD_USEDEVMODECOPIESANDCOLLATE | PD_NOCURRENTPAGE
            MinPage = 1,
            MaxPage = 9999,
            Copies = (uint)printerSettings.Copies,
            StartPage = uint.MaxValue // START_PAGE_GENERAL
        };

        try
        {
            if (printerSettings.IsValid)
            {
                data.DevMode = printerSettings.GetHdevmode(document.DefaultPageSettings);
                data.DevNames = printerSettings.GetHdevnames();
            }

            int result = PrintDlgEx(ref data);
            if (result < 0) Marshal.ThrowExceptionForHR(result);
            if (data.ResultAction == 0) return false;
            if (data.DevNames != IntPtr.Zero) printerSettings.SetHdevnames(data.DevNames);

            if (data.DevMode != IntPtr.Zero)
            {
                printerSettings.SetHdevmode(data.DevMode);
                document.DefaultPageSettings.SetHdevmode(data.DevMode);
            }

            printerSettings.PrintToFile = (data.Flags & 0x20) != 0;
            options.PrinterName = printerSettings.PrinterName;
            options.Copies = printerSettings.Copies;
            return data.ResultAction == 1; // PD_RESULT_PRINT
        }
        finally
        {
            if (data.DevMode != IntPtr.Zero) GlobalFree(data.DevMode);
            if (data.DevNames != IntPtr.Zero) GlobalFree(data.DevNames);
            if (data.Dc != IntPtr.Zero) Win32.DeleteDC(data.Dc);
        }
    }

    public void Print(PrintOptions options, string documentName, Func<PrintPageSetup, int, PrintedPage?> renderPage)
    {
        PrintDocument document = GetDocument(options);
        document.DocumentName = documentName;
        int pageIndex = 0;

        void BeginPrint(object? sender, PrintEventArgs e) => pageIndex = 0;

        void PrintPage(object? sender, PrintPageEventArgs e)
        {
            Rectangle bounds = e.PageBounds;
            Rectangle margins = e.MarginBounds;
            PrintPageSetup setup = new PrintPageSetup(bounds.Width, bounds.Height,
                new PlatformRectangle(margins.X, margins.Y, margins.Width, margins.Height));
            PrintedPage? page = renderPage(setup, pageIndex++);

            if (page != null)
            {
                DrawImage(e, page.Image, bounds);
            }

            e.HasMorePages = page?.HasMorePages == true;
        }

        document.BeginPrint += BeginPrint;
        document.PrintPage += PrintPage;

        try
        {
            document.Print();
        }
        finally
        {
            document.BeginPrint -= BeginPrint;
            document.PrintPage -= PrintPage;
        }
    }

    /// <summary>The job's PrintDocument, kept in the options so dialog choices (paper, orientation, colour) reach the job.</summary>
    private static PrintDocument GetDocument(PrintOptions options)
    {
        if (options.PlatformData is not PrintDocument document)
        {
            document = new PrintDocument { PrintController = new StandardPrintController() };
            options.PlatformData = document;
        }

        if (!string.IsNullOrEmpty(options.PrinterName) && document.PrinterSettings.PrinterName != options.PrinterName)
        {
            document.PrinterSettings.PrinterName = options.PrinterName;
        }

        document.PrinterSettings.Copies = (short)Math.Clamp(options.Copies, 1, short.MaxValue);

        return document;
    }

    /// <summary>Transfers a rendered page to the printer's device context.</summary>
    internal static unsafe void DrawImage(PrintPageEventArgs args, PixelBuffer image, Rectangle rectangle)
    {
        Graphics graphics = args.Graphics ?? throw new InvalidOperationException("The page has no graphics.");
        BitmapInfoHeader header = new()
        {
            Size = (uint)sizeof(BitmapInfoHeader),
            Width = image.Width,
            Height = -image.Height, // top down
            Planes = 1,
            BitCount = 32
        };

        float scaleX = graphics.DpiX / 100f, scaleY = graphics.DpiY / 100f;
        rectangle = new Rectangle((int)Math.Round(rectangle.X * scaleX), (int)Math.Round(rectangle.Y * scaleY),
            (int)Math.Round(rectangle.Width * scaleX), (int)Math.Round(rectangle.Height * scaleY));
        IntPtr dc = IntPtr.Zero;

        try
        {
            dc = graphics.GetHdc();

            fixed (byte* pixels = image.Pixels)
            {
                int result = StretchDIBits(dc, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height,
                    0, 0, image.Width, image.Height, (IntPtr)pixels, ref header, 0, 0x00CC0020); // DIB_RGB_COLORS, SRCCOPY
                if (result == -1) throw new Win32Exception(); // GDI_ERROR
            }
        }
        finally
        {
            if (dc != IntPtr.Zero) graphics.ReleaseHdc(dc);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct BitmapInfoHeader
    {
        public uint Size;
        public int Width, Height;
        public ushort Planes, BitCount;
        public uint Compression, SizeImage;
        public int XPelsPerMeter, YPelsPerMeter;
        public uint ClrUsed, ClrImportant;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct PrintDialogData
    {
        public uint Size;
        public IntPtr Owner, DevMode, DevNames, Dc;
        public uint Flags, Flags2, ExclusionFlags, PageRangeCount, MaxPageRanges;
        public IntPtr PageRanges;
        public uint MinPage, MaxPage, Copies;
        public IntPtr Instance, TemplateName, Callback;
        public uint PropertyPageCount;
        public IntPtr PropertyPages;
        public uint StartPage, ResultAction;
    }

    [DllImport("comdlg32.dll", EntryPoint = "PrintDlgExW", CharSet = CharSet.Unicode)]
    private static extern int PrintDlgEx(ref PrintDialogData data);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GlobalFree(IntPtr memory);

    [DllImport("gdi32.dll", SetLastError = true)]
    private static extern int StretchDIBits(IntPtr dc, int x, int y, int width, int height,
        int sourceX, int sourceY, int sourceWidth, int sourceHeight, IntPtr pixels, ref BitmapInfoHeader info, uint usage, uint operation);
}
