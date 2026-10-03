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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform.Windows;

/// <summary>Win32 clipboard. Images are published as PNG (keeps transparency) and as a DIB for older applications.</summary>
public sealed unsafe class WindowsClipboardService : IClipboardService
{
    private static readonly uint PngFormat = Win32.RegisterClipboardFormat("PNG");

    public FeatureSupport Support => FeatureSupport.Supported;

    public Task<bool> SetTextAsync(string text, CancellationToken cancellationToken = default) => Task.FromResult(Write(() =>
    {
        byte[] bytes = new byte[(text.Length + 1) * 2];
        MemoryMarshal.AsBytes(text.AsSpan()).CopyTo(bytes);
        return SetData(Win32.CF_UNICODETEXT, bytes);
    }));

    public Task<string?> GetTextAsync(CancellationToken cancellationToken = default) => Task.FromResult(Read<string?>(() =>
    {
        IntPtr handle = Win32.GetClipboardData(Win32.CF_UNICODETEXT);

        if (handle == IntPtr.Zero)
        {
            return null;
        }

        IntPtr pointer = Win32.GlobalLock(handle);

        try
        {
            return pointer != IntPtr.Zero ? Marshal.PtrToStringUni(pointer) : null;
        }
        finally
        {
            Win32.GlobalUnlock(handle);
        }
    }));

    public Task<bool> SetImageAsync(byte[] png, CancellationToken cancellationToken = default)
    {
        byte[] dib = CreateDib(PngCodec.Decode(png));
        return Task.FromResult(Write(() => SetData(PngFormat, png) & SetData(Win32.CF_DIB, dib)));
    }

    public Task<byte[]?> GetImageAsync(CancellationToken cancellationToken = default) => Task.FromResult(Read<byte[]?>(() =>
    {
        byte[]? png = GetData(PngFormat);

        if (png != null && PngCodec.IsPng(png))
        {
            return png;
        }

        byte[]? dib = GetData(Win32.CF_DIBV5) ?? GetData(Win32.CF_DIB);
        PixelBuffer? pixels = dib != null ? ReadDib(dib) : null;
        return pixels != null ? PngCodec.Encode(pixels) : null;
    }));

    public Task<bool> SetFilesAsync(IReadOnlyList<string> paths, CancellationToken cancellationToken = default) => Task.FromResult(Write(() =>
    {
        // DROPFILES header followed by a double null terminated UTF-16 list.
        string list = string.Join("\0", paths) + "\0\0";
        int headerSize = sizeof(Win32.DROPFILES);
        byte[] data = new byte[headerSize + list.Length * 2];

        fixed (byte* pointer = data)
        {
            *(Win32.DROPFILES*)pointer = new Win32.DROPFILES { pFiles = (uint)headerSize, fWide = 1 };
        }

        MemoryMarshal.AsBytes(list.AsSpan()).CopyTo(data.AsSpan(headerSize));
        return SetData(Win32.CF_HDROP, data);
    }));

    public Task<IReadOnlyList<string>> GetFilesAsync(CancellationToken cancellationToken = default) => Task.FromResult(Read<IReadOnlyList<string>>(() =>
    {
        IntPtr drop = Win32.GetClipboardData(Win32.CF_HDROP);

        if (drop == IntPtr.Zero)
        {
            return Array.Empty<string>();
        }

        uint count = Win32.DragQueryFile(drop, uint.MaxValue, null, 0);
        List<string> files = new List<string>((int)count);

        for (uint i = 0; i < count; i++)
        {
            uint length = Win32.DragQueryFile(drop, i, null, 0);
            char[] buffer = new char[length + 1];

            fixed (char* chars = buffer)
            {
                Win32.DragQueryFile(drop, i, chars, (uint)buffer.Length);
            }

            files.Add(new string(buffer, 0, (int)length));
        }

        return files;
    }) ?? Array.Empty<string>());

    public Task<bool> ClearAsync(CancellationToken cancellationToken = default) => Task.FromResult(Write(() => true));

    /// <summary>Converts to a bottom up 32 bit BI_RGB DIB, the most widely understood clipboard bitmap.</summary>
    internal static byte[] CreateDib(PixelBuffer image)
    {
        int headerSize = sizeof(Win32.BITMAPINFOHEADER);
        byte[] dib = new byte[headerSize + image.Pixels.Length];

        fixed (byte* pointer = dib)
        {
            *(Win32.BITMAPINFOHEADER*)pointer = new Win32.BITMAPINFOHEADER
            {
                biSize = (uint)headerSize,
                biWidth = image.Width,
                biHeight = image.Height,
                biPlanes = 1,
                biBitCount = 32,
                biCompression = Win32.BI_RGB,
                biSizeImage = (uint)image.Pixels.Length
            };
        }

        for (int y = 0; y < image.Height; y++)
        {
            Buffer.BlockCopy(image.Pixels, y * image.Stride, dib, headerSize + (image.Height - 1 - y) * image.Stride, image.Stride);
        }

        return dib;
    }

    /// <summary>Reads 24 and 32 bit uncompressed or bitfield DIBs, the formats screenshot tools and browsers put on the clipboard.</summary>
    internal static PixelBuffer? ReadDib(byte[] dib)
    {
        if (dib.Length < sizeof(Win32.BITMAPINFOHEADER))
        {
            return null;
        }

        Win32.BITMAPINFOHEADER header;

        fixed (byte* pointer = dib)
        {
            header = *(Win32.BITMAPINFOHEADER*)pointer;
        }

        if ((header.biBitCount != 32 && header.biBitCount != 24) || (header.biCompression != Win32.BI_RGB && header.biCompression != Win32.BI_BITFIELDS))
        {
            return null;
        }

        int width = header.biWidth;
        int height = Math.Abs(header.biHeight);
        bool bottomUp = header.biHeight > 0;
        int bytesPerPixel = header.biBitCount / 8;
        int stride = (width * bytesPerPixel + 3) & ~3;
        // BITMAPINFOHEADER followed by three DWORD masks when BI_BITFIELDS is used with the plain header size.
        int offset = (int)header.biSize + (header.biCompression == Win32.BI_BITFIELDS && header.biSize == 40 ? 12 : 0) + (int)header.biClrUsed * 4;

        if (width <= 0 || height <= 0 || offset + (long)stride * height > dib.Length)
        {
            return null;
        }

        PixelBuffer result = new PixelBuffer(width, height);
        bool hasAlpha = false;

        for (int y = 0; y < height; y++)
        {
            int sourceRow = offset + (bottomUp ? height - 1 - y : y) * stride;

            for (int x = 0; x < width; x++)
            {
                int s = sourceRow + x * bytesPerPixel;
                int d = y * result.Stride + x * 4;
                result.Pixels[d] = dib[s];
                result.Pixels[d + 1] = dib[s + 1];
                result.Pixels[d + 2] = dib[s + 2];
                byte alpha = bytesPerPixel == 4 ? dib[s + 3] : (byte)255;
                result.Pixels[d + 3] = alpha;
                hasAlpha |= alpha != 0;
            }
        }

        // Many applications leave the fourth byte of a 32 bit DIB at zero. Treat that as opaque.
        if (!hasAlpha)
        {
            result.MakeOpaque();
        }

        return result;
    }

    // The names WinForms' DataFormats uses for the predefined formats, so the clipboard viewer shows what it always did.
    private static readonly Dictionary<uint, string> StandardFormatNames = new Dictionary<uint, string>
    {
        [1] = "Text",
        [2] = "Bitmap",
        [3] = "MetaFilePict",
        [4] = "SymbolicLink",
        [5] = "DataInterchangeFormat",
        [6] = "TaggedImageFileFormat",
        [7] = "OEMText",
        [8] = "DeviceIndependentBitmap",
        [9] = "Palette",
        [10] = "PenData",
        [11] = "RiffAudio",
        [12] = "WaveAudio",
        [13] = "UnicodeText",
        [14] = "EnhancedMetafile",
        [15] = "FileDrop",
        [16] = "Locale",
        [17] = "Format17"
    };

    public Task<IReadOnlyList<string>> GetFormatsAsync(CancellationToken cancellationToken = default) => Task.FromResult(Read<IReadOnlyList<string>>(() =>
    {
        List<string> formats = new List<string>();
        char* name = stackalloc char[256];

        for (uint format = Win32.EnumClipboardFormats(0); format != 0; format = Win32.EnumClipboardFormats(format))
        {
            if (StandardFormatNames.TryGetValue(format, out string? standard))
            {
                formats.Add(standard);
            }
            else
            {
                int length = Win32.GetClipboardFormatName(format, name, 256);
                formats.Add(length > 0 ? new string(name, 0, length) : "Format" + format);
            }
        }

        return formats;
    }) ?? Array.Empty<string>());

    public Task<byte[]?> GetDataAsync(string format, CancellationToken cancellationToken = default) => Task.FromResult(Read(() =>
    {
        uint id = StandardFormatNames.FirstOrDefault(pair => pair.Value.Equals(format, StringComparison.OrdinalIgnoreCase)).Key;

        if (id == 0)
        {
            id = Win32.RegisterClipboardFormat(format);
        }

        // Bitmap, MetaFilePict, Palette and EnhancedMetafile hold GDI handles, not memory.
        return id is 2 or 3 or 9 or 14 ? null : GetData(id);
    }));

    private static bool SetData(uint format, byte[] data)
    {
        IntPtr memory = Win32.GlobalAlloc(Win32.GMEM_MOVEABLE, (nuint)data.Length);

        if (memory == IntPtr.Zero)
        {
            return false;
        }

        IntPtr pointer = Win32.GlobalLock(memory);
        Marshal.Copy(data, 0, pointer, data.Length);
        Win32.GlobalUnlock(memory);

        if (Win32.SetClipboardData(format, memory) == IntPtr.Zero)
        {
            // Ownership only passes to the system on success.
            Win32.GlobalFree(memory);
            return false;
        }

        return true;
    }

    private static byte[]? GetData(uint format)
    {
        IntPtr handle = Win32.GetClipboardData(format);

        if (handle == IntPtr.Zero)
        {
            return null;
        }

        IntPtr pointer = Win32.GlobalLock(handle);

        try
        {
            if (pointer == IntPtr.Zero)
            {
                return null;
            }

            byte[] data = new byte[(int)Win32.GlobalSize(handle)];
            Marshal.Copy(pointer, data, 0, data.Length);
            return data;
        }
        finally
        {
            Win32.GlobalUnlock(handle);
        }
    }

    private static bool Write(Func<bool> write)
    {
        // SetClipboardData fails when the clipboard was opened without an owner window, so use a message only window.
        IntPtr owner = Win32.CreateWindowEx(0, "STATIC", null, 0, 0, 0, 0, 0, Win32.HWND_MESSAGE, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

        try
        {
            if (!Open(owner))
            {
                return false;
            }

            try
            {
                Win32.EmptyClipboard();
                return write();
            }
            finally
            {
                Win32.CloseClipboard();
            }
        }
        finally
        {
            if (owner != IntPtr.Zero)
            {
                Win32.DestroyWindow(owner);
            }
        }
    }

    private static T? Read<T>(Func<T> read)
    {
        if (!Open(IntPtr.Zero))
        {
            return default;
        }

        try
        {
            return read();
        }
        finally
        {
            Win32.CloseClipboard();
        }
    }

    /// <summary>Another application may hold the clipboard briefly, so retry for up to a second.</summary>
    private static bool Open(IntPtr owner)
    {
        for (int attempt = 0; attempt < 20; attempt++)
        {
            if (Win32.OpenClipboard(owner))
            {
                return true;
            }

            Thread.Sleep(50);
        }

        return false;
    }
}
