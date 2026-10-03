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
using System;

namespace ShareX.Platform;

/// <summary>A page's size and printable area in hundredths of an inch, as System.Drawing.Printing reports them.</summary>
public sealed record PrintPageSetup(int Width, int Height, PlatformRectangle MarginBounds)
{
    /// <summary>Letter with one inch margins, used when no printer is known.</summary>
    public static PrintPageSetup Letter { get; } = new PrintPageSetup(850, 1100, new PlatformRectangle(100, 100, 650, 900));

    /// <summary>A4 with one inch margins.</summary>
    public static PrintPageSetup A4 { get; } = new PrintPageSetup(827, 1169, new PlatformRectangle(100, 100, 627, 969));
}

/// <summary>One rendered page: an image covering the whole page, and whether more pages follow.</summary>
public sealed record PrintedPage(PixelBuffer Image, bool HasMorePages);

/// <summary>The printer, copies and similar choices for a print job. Filled by <see cref="IPrintService.ShowPrintDialog"/>.</summary>
public sealed class PrintOptions
{
    /// <summary>Null for the default printer.</summary>
    public string? PrinterName { get; set; }

    public int Copies { get; set; } = 1;

    /// <summary>The window that owns the print dialog, or 0.</summary>
    public long OwnerWindowHandle { get; set; }

    /// <summary>Settings the platform keeps between the dialog and printing, such as the Windows DEVMODE. Callers do not read it.</summary>
    public object? PlatformData { get; set; }
}

/// <summary>Sending pages to a printer. ShareX renders the pages itself, so printouts look the same everywhere.</summary>
public interface IPrintService
{
    /// <summary>Windows printing on Windows; CUPS (the lp command) on Linux and macOS.</summary>
    FeatureSupport Support { get; }

    /// <summary>Whether <see cref="ShowPrintDialog"/> shows a dialog. Where it does not, jobs go to the chosen or default printer.</summary>
    FeatureSupport DialogSupport { get; }

    bool IsPrinterInstalled(string printerName);

    /// <summary>The page the job will print on, from the printer's settings, or a default paper size.</summary>
    PrintPageSetup GetPageSetup(PrintOptions options);

    /// <summary>Lets the user choose a printer and its settings. Call it on the UI thread. False when the user cancelled.</summary>
    bool ShowPrintDialog(PrintOptions options);

    /// <summary>Prints pages until one reports no more pages.</summary>
    /// <param name="renderPage">Renders the page with the given zero based index for the page setup in use.</param>
    void Print(PrintOptions options, string documentName, Func<PrintPageSetup, int, PrintedPage?> renderPage);
}
