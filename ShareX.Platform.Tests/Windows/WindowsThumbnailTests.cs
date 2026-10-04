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
using ShareX.Platform.Windows.Native;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;
using Xunit;

namespace ShareX.Platform.Tests;

[Collection("Windows platform services")]
public sealed class WindowsThumbnailTests
{
    [WindowsFact]
    public void ShellThumbnailsPreserveImageAspectAndRowColors() => RunSta(() =>
    {
        using ImageFiles files = new();
        WindowsThumbnailService service = new();
        foreach (string path in new[] { files.Png, files.Jpeg })
        {
            byte[]? thumbnail = service.GetThumbnail(path, 24, 24);

            // CI's Windows Server image runs no thumbnail handlers, so there is nothing to compare; desktops must produce one.
            if (thumbnail == null && Environment.GetEnvironmentVariable("CI") == "true")
            {
                return;
            }

            PixelBuffer image = PngCodec.Decode(Assert.IsType<byte[]>(thumbnail));
            Assert.InRange(image.Width, 1, 24);
            Assert.InRange(image.Height, 1, 24);
            Assert.InRange((double)image.Width / image.Height, 1.9, 2.1);
            int top = (image.Width / 2) * 4;
            int bottom = ((image.Height - 1) * image.Width + image.Width / 2) * 4;
            Assert.InRange(image.Pixels[top + 2], 200, 255);
            Assert.InRange(image.Pixels[top], 0, 30);
            Assert.InRange(image.Pixels[bottom], 200, 255);
            Assert.InRange(image.Pixels[bottom + 2], 0, 30);
            Assert.Equal(255, image.Pixels[top + 3]);
            Assert.Equal(255, image.Pixels[bottom + 3]);
        }
    });

    [WindowsFact]
    public void MissingFilesAndDirectoriesReturnNoThumbnail() => RunSta(() =>
    {
        using ImageFiles files = new();
        WindowsThumbnailService service = new();
        Assert.Null(service.GetThumbnail(Path.Combine(files.DirectoryPath, "missing.png"), 24, 24));
        Assert.Null(service.GetThumbnail(Path.Combine(files.DirectoryPath, "missing", "image.png"), 24, 24));
    });

    [WindowsFact]
    public void NativeBitmapReaderPreservesRowsAndStraightAlpha() => RunSta(() =>
    {
        byte[] premultiplied =
        [
            0, 0, 128, 128,   0, 255, 0, 255,   255, 0, 0, 255,
            128, 64, 32, 128,   0, 0, 0, 0,   0, 255, 255, 255
        ];
        byte[] straight =
        [
            0, 0, 255, 128,   0, 255, 0, 255,   255, 0, 0, 255,
            255, 127, 63, 128,   0, 0, 0, 0,   0, 255, 255, 255
        ];
        foreach (bool topDown in new[] { true, false })
        {
            using NativeBitmap bitmap = new(premultiplied, 32, topDown);
            PixelBuffer actual = Assert.IsType<PixelBuffer>(WindowsThumbnailService.ReadPixels(bitmap.Handle));
            Assert.Equal(3, actual.Width);
            Assert.Equal(2, actual.Height);
            Assert.Equal(straight, actual.Pixels);
            bitmap.AssertUnchanged();
        }
        Assert.Null(WindowsThumbnailService.ReadPixels(IntPtr.Zero));
    });

    [WindowsFact]
    public void NativeRgbBitmapsProduceOpaqueThumbnails() => RunSta(() =>
    {
        // Three 24-bit pixels leave three padding bytes in each scan line.
        byte[] rgb =
        [
            0, 0, 255,   0, 255, 0,   255, 0, 0,   17, 18, 19,
            255, 0, 255,   255, 255, 0,   0, 255, 255,   20, 21, 22
        ];
        byte[] expected =
        [
            0, 0, 255, 255,   0, 255, 0, 255,   255, 0, 0, 255,
            255, 0, 255, 255,   255, 255, 0, 255,   0, 255, 255, 255
        ];
        foreach (bool topDown in new[] { true, false })
        {
            using NativeBitmap bitmap = new(rgb, 24, topDown);
            PixelBuffer actual = Assert.IsType<PixelBuffer>(WindowsThumbnailService.ReadPixels(bitmap.Handle));
            Assert.Equal(expected, actual.Pixels);
            bitmap.AssertUnchanged();
        }
    });

    [WindowsFact]
    public void RepeatedShellThumbnailsReleaseGdiObjectsAndFiles() => RunSta(() =>
    {
        using ImageFiles files = new();
        WindowsThumbnailService service = new();
        for (int i = 0; i < 4; i++) Assert.NotNull(service.GetThumbnail(files.Png, 24, 24));
        using Process process = Process.GetCurrentProcess();
        uint before = GetGuiResources(process.Handle, 0);
        for (int i = 0; i < 40; i++) Assert.NotNull(service.GetThumbnail(files.Png, 24, 24));
        Assert.Equal(before, GetGuiResources(process.Handle, 0));
        // On Windows this also verifies that extraction leaves no open file handle.
        using FileStream exclusive = File.Open(files.Png, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
    });

    private static void RunSta(Action action)
    {
        if (!OperatingSystem.IsWindows()) return;
        Exception? failure = null;
        Thread thread = new(() =>
        {
            try { action(); }
            catch (Exception exception) { failure = exception; }
        }) { IsBackground = true };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(30)), "Windows thumbnail verification did not complete.");
        if (failure != null) ExceptionDispatchInfo.Capture(failure).Throw();
    }

    private sealed class NativeBitmap : IDisposable
    {
        private readonly IntPtr pixels;
        private readonly byte[] source;
        public IntPtr Handle { get; }

        public NativeBitmap(byte[] topDownPixels, ushort bitCount, bool topDown)
        {
            source = (byte[])topDownPixels.Clone();
            if (!topDown)
            {
                int stride = source.Length / 2;
                Buffer.BlockCopy(topDownPixels, stride, source, 0, stride);
                Buffer.BlockCopy(topDownPixels, 0, source, stride, stride);
            }
            Win32.BITMAPINFOHEADER header = new()
            {
                biSize = (uint)Marshal.SizeOf<Win32.BITMAPINFOHEADER>(),
                biWidth = 3,
                biHeight = topDown ? -2 : 2,
                biPlanes = 1,
                biBitCount = bitCount,
                biCompression = Win32.BI_RGB
            };
            Handle = CreateDIBSection(IntPtr.Zero, ref header, Win32.DIB_RGB_COLORS, out pixels, IntPtr.Zero, 0);
            Assert.NotEqual(IntPtr.Zero, Handle);
            Marshal.Copy(source, 0, pixels, source.Length);
        }

        public void AssertUnchanged()
        {
            byte[] actual = new byte[source.Length];
            Marshal.Copy(pixels, actual, 0, actual.Length);
            Assert.Equal(source, actual);
        }

        public void Dispose() => Assert.True(Win32.DeleteObject(Handle));
    }

    private sealed class ImageFiles : IDisposable
    {
        public string DirectoryPath { get; } = Path.Combine(Path.GetTempPath(), "sharex-thumbnails-" + Guid.NewGuid().ToString("N"));
        public string Png => Path.Combine(DirectoryPath, "synthetic.png");
        public string Jpeg => Path.Combine(DirectoryPath, "synthetic.jpg");

        public ImageFiles()
        {
            Directory.CreateDirectory(DirectoryPath);
            try
            {
                PixelBuffer image = new(96, 48);
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        int offset = (y * image.Width + x) * 4;
                        image.Pixels[offset + (y < image.Height / 2 ? 2 : 0)] = 255;
                        image.Pixels[offset + 3] = 255;
                    }
                }
                File.WriteAllBytes(Png, PngCodec.Encode(image));
                using Bitmap jpeg = new(image.Width, image.Height, PixelFormat.Format24bppRgb);
                using (Graphics graphics = Graphics.FromImage(jpeg))
                {
                    graphics.FillRectangle(Brushes.Red, 0, 0, image.Width, image.Height / 2);
                    graphics.FillRectangle(Brushes.Blue, 0, image.Height / 2, image.Width, image.Height / 2);
                }
                jpeg.Save(Jpeg, ImageFormat.Jpeg);
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            File.Delete(Png);
            File.Delete(Jpeg);
            Directory.Delete(DirectoryPath);
        }
    }

    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateDIBSection(IntPtr dc, ref Win32.BITMAPINFOHEADER info, uint usage, out IntPtr pixels, IntPtr section, uint offset);

    [DllImport("user32.dll")]
    private static extern uint GetGuiResources(IntPtr process, uint flag);
}
