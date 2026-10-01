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
using System;
using System.Drawing.Printing;
using System.Runtime.InteropServices;

namespace ShareX.HelpersLib;

// Native printer selection is separate from the Avalonia UI and Skia page rendering.
internal static class WindowsPrintDialog
{
    public static bool Show(PrintDocument document) => DesktopServices.Run(() =>
    {
        PrintDialogData data = new()
        {
            Size = (uint)Marshal.SizeOf<PrintDialogData>(),
            Owner = DesktopServices.GetWindow().TryGetPlatformHandle()?.Handle ?? IntPtr.Zero,
            Flags = 0x100 | 0x4 | 0x8 | 0x40000 | 0x800000, // PD_RETURNDC | PD_NOSELECTION | PD_NOPAGENUMS | PD_USEDEVMODECOPIESANDCOLLATE | PD_NOCURRENTPAGE
            MinPage = 1,
            MaxPage = 9999,
            Copies = (uint)document.PrinterSettings.Copies,
            StartPage = uint.MaxValue // START_PAGE_GENERAL
        };
        try
        {
            if (document.PrinterSettings.IsValid)
            {
                data.DevMode = document.PrinterSettings.GetHdevmode(document.DefaultPageSettings);
                data.DevNames = document.PrinterSettings.GetHdevnames();
            }
            int result = PrintDlgEx(ref data);
            if (result < 0) Marshal.ThrowExceptionForHR(result);
            if (data.ResultAction == 0) return false;
            if (data.DevNames != IntPtr.Zero) document.PrinterSettings.SetHdevnames(data.DevNames);
            if (data.DevMode != IntPtr.Zero)
            {
                document.PrinterSettings.SetHdevmode(data.DevMode);
                document.DefaultPageSettings.SetHdevmode(data.DevMode);
            }
            document.PrinterSettings.PrintToFile = (data.Flags & 0x20) != 0;
            return data.ResultAction == 1; // PD_RESULT_PRINT
        }
        finally
        {
            if (data.DevMode != IntPtr.Zero) GlobalFree(data.DevMode);
            if (data.DevNames != IntPtr.Zero) GlobalFree(data.DevNames);
            if (data.Dc != IntPtr.Zero) NativeMethods.DeleteDC(data.Dc);
        }
    });

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
}
