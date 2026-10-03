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
using ShareX.Platform.Windows;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;

namespace ShareX.Platform.Tests;

[Collection("Windows platform services")]
public sealed class WindowsDesktopWallpaperTests
{
    [WindowsFact]
    public void LookupPreservesSystemPathPrecedenceAndCacheFallback()
    {
        if (!OperatingSystem.IsWindows()) return;
        const string systemPath = @"C:\fixtures\system wallpaper.png";
        const string cachePath = @"C:\fixtures\背景 image.png";
        int cacheReads = 0;
        byte[] cache = Encoding.Unicode.GetBytes("header\0" + cachePath + "\0ignored");
        byte[]? ReadCache() { cacheReads++; return cache; }

        WindowsDesktopWallpaperService service = new(() => systemPath, ReadCache, path => path == systemPath || path == cachePath);
        Assert.Equal(new DesktopWallpaper(systemPath, DesktopWallpaperLayout.Fill), service.GetWallpaper());
        Assert.Equal(0, cacheReads);

        // A stale, nonempty SPI result must not silently select a different registry image.
        service = new(() => systemPath, ReadCache, path => path == cachePath);
        Assert.Null(service.GetWallpaper());
        Assert.Equal(0, cacheReads);

        foreach (string? systemResult in new string?[] { null, "", " \t" })
        {
            service = new(() => systemResult, ReadCache, path => path == cachePath);
            Assert.Equal(new DesktopWallpaper(cachePath, DesktopWallpaperLayout.Fill), service.GetWallpaper());
        }
        Assert.Equal(3, cacheReads);

        foreach (string path in new[] { cachePath, @"\\server\share\背景 image.png" })
        {
            foreach (string suffix in new[] { "\0ignored", "" })
            {
                byte[] bytes = Encoding.Unicode.GetBytes("header\0" + path + suffix);
                byte[] unchanged = (byte[])bytes.Clone();
                service = new(() => null, () => bytes, candidate => candidate == path);
                Assert.Equal(new DesktopWallpaper(path, DesktopWallpaperLayout.Fill), service.GetWallpaper());
                Assert.Equal(unchanged, bytes);
            }
        }

        foreach (byte[]? bytes in new byte[]?[] { null, [], [0], Encoding.Unicode.GetBytes("no absolute image path\0") })
        {
            service = new(() => null, () => bytes, _ => throw new InvalidOperationException("No valid path to check."));
            Assert.Null(service.GetWallpaper());
        }
        service = new(() => null, () => cache, _ => false);
        Assert.Null(service.GetWallpaper());
    }

    [WindowsFact]
    public void NativeLookupMatchesTheLegacyBufferAndReadsOnlyMetadata()
    {
        if (!OperatingSystem.IsWindows()) return;
        StringBuilder legacyBuffer = new(short.MaxValue);
        string? expected = SystemParametersInfoW(0x0073, legacyBuffer.Capacity, legacyBuffer, 0)
            ? legacyBuffer.ToString().TrimEnd('\0') : null;
        Assert.Equal(expected, WindowsDesktopWallpaperService.ReadSystemWallpaper());

        WindowsDesktopWallpaperService service = new();
        Assert.True(service.Support.IsSupported);
        Assert.False(service.RequiresPrewarm);
        service.Prewarm();
        DesktopWallpaper? wallpaper = service.GetWallpaper();
        if (wallpaper != null)
        {
            Assert.True(File.Exists(wallpaper.Path));
            Assert.Equal(DesktopWallpaperLayout.Fill, wallpaper.Layout);
        }

        using WindowsPlatformServices platform = new();
        Assert.IsType<WindowsDesktopWallpaperService>(platform.Wallpaper);
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SystemParametersInfoW(int action, int count, StringBuilder value, int flags);
}