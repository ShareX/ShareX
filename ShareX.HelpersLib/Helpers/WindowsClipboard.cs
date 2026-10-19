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

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace ShareX.HelpersLib;

// Writes image formats with the Win32 clipboard API. Avalonia synthesizes CF_DIB, CF_DIBV5 and CF_BITMAP
// from its own premultiplied, top-down 32 bpp bitmap, which Windows clipboard history and Office render
// incorrectly. Placing our own bottom-up CF_DIB lets Windows synthesize CF_BITMAP and CF_DIBV5 like
// System.Windows.Forms.Clipboard.SetImage did.
internal static class WindowsClipboard
{
    private const uint CF_DIB = 8;
    private const uint GMEM_MOVEABLE = 0x0002;
    private const int OpenRetryCount = 10;
    private const int OpenRetryDelay = 50;

    public static void SetImage(IntPtr owner, byte[] png, byte[] dib, string html)
    {
        // A null owner makes SetClipboardData fail after EmptyClipboard.
        if (owner == IntPtr.Zero) throw new InvalidOperationException("Clipboard owner window is unavailable.");
        Open(owner);
        try
        {
            if (!NativeMethods.EmptyClipboard()) throw new Win32Exception();
            SetData(CF_DIB, dib);
            if (png != null) SetData(RegisterFormat(ClipboardHelpers.FORMAT_PNG), png);
            if (!string.IsNullOrEmpty(html)) SetData(RegisterFormat(ClipboardDataFormats.Html), Encoding.UTF8.GetBytes(html + "\0"));
        }
        finally
        {
            NativeMethods.CloseClipboard();
        }
    }

    private static void Open(IntPtr owner)
    {
        // Another process may briefly hold the clipboard, for example a clipboard manager reading our last copy.
        for (int i = 0; ; i++)
        {
            if (NativeMethods.OpenClipboard(owner)) return;
            if (i >= OpenRetryCount) throw new Win32Exception();
            Thread.Sleep(OpenRetryDelay);
        }
    }

    private static uint RegisterFormat(string name)
    {
        uint format = NativeMethods.RegisterClipboardFormat(name);
        if (format == 0) throw new Win32Exception();
        return format;
    }

    private static void SetData(uint format, byte[] data)
    {
        IntPtr handle = NativeMethods.GlobalAlloc(GMEM_MOVEABLE, (UIntPtr)data.Length);
        if (handle == IntPtr.Zero) throw new Win32Exception();
        try
        {
            IntPtr pointer = NativeMethods.GlobalLock(handle);
            if (pointer == IntPtr.Zero) throw new Win32Exception();
            try { Marshal.Copy(data, 0, pointer, data.Length); }
            finally { NativeMethods.GlobalUnlock(handle); }
            if (NativeMethods.SetClipboardData(format, handle) == IntPtr.Zero) throw new Win32Exception();
        }
        catch
        {
            // The system owns the memory only after SetClipboardData succeeds.
            NativeMethods.GlobalFree(handle);
            throw;
        }
    }
}
