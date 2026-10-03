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

using Microsoft.Win32;
using ShareX.Platform.Windows.Native;
using System;
using System.IO;
using System.Text;

namespace ShareX.Platform.Windows;

/// <summary>
/// Windows wallpaper lookup, preserving the editor's SPI and TranscodedImageCache fallback behavior.
/// </summary>
public sealed class WindowsDesktopWallpaperService : IDesktopWallpaperService
{
    private const uint SpiGetDesktopWallpaper = 0x0073;
    private const int MaxWallpaperPath = short.MaxValue;
    private const string DesktopRegistrySubKey = @"Control Panel\Desktop";
    private const string TranscodedImageCacheValueName = "TranscodedImageCache";

    private readonly Func<string?> readSystemWallpaper;
    private readonly Func<byte[]?> readTranscodedCache;
    private readonly Func<string, bool> fileExists;

    public FeatureSupport Support => FeatureSupport.Supported;
    public bool RequiresPrewarm => false;

    public WindowsDesktopWallpaperService() : this(ReadSystemWallpaper, ReadTranscodedCache, File.Exists)
    {
    }

    internal WindowsDesktopWallpaperService(Func<string?> readSystemWallpaper, Func<byte[]?> readTranscodedCache,
        Func<string, bool> fileExists)
    {
        this.readSystemWallpaper = readSystemWallpaper;
        this.readTranscodedCache = readTranscodedCache;
        this.fileExists = fileExists;
    }

    public DesktopWallpaper? GetWallpaper()
    {
        string? wallpaperPath = readSystemWallpaper();
        if (TryCreateWallpaper(wallpaperPath) is DesktopWallpaper wallpaper)
        {
            return wallpaper;
        }

        // A nonempty SPI path takes precedence even if its file is no longer available.
        if (!string.IsNullOrWhiteSpace(wallpaperPath)) return null;
        byte[]? cache = readTranscodedCache();
        return cache is { Length: > 0 } ? TryCreateWallpaper(TryExtractWallpaperPathFromTranscodedCache(cache)) : null;
    }

    internal static unsafe string? ReadSystemWallpaper()
    {
        char[] buffer = new char[MaxWallpaperPath];
        fixed (char* characters = buffer)
        {
            if (!Win32.SystemParametersInfo(SpiGetDesktopWallpaper, (uint)buffer.Length, characters, 0)) return null;
        }

        int terminator = Array.IndexOf(buffer, '\0');
        return new string(buffer, 0, terminator >= 0 ? terminator : buffer.Length);
    }

    private static byte[]? ReadTranscodedCache()
    {
        using RegistryKey? desktopKey = Registry.CurrentUser.OpenSubKey(DesktopRegistrySubKey);
        return desktopKey?.GetValue(TranscodedImageCacheValueName) as byte[];
    }

    private DesktopWallpaper? TryCreateWallpaper(string? wallpaperPath)
    {
        return !string.IsNullOrWhiteSpace(wallpaperPath) && fileExists(wallpaperPath)
            ? new DesktopWallpaper(wallpaperPath, DesktopWallpaperLayout.Fill)
            : null;
    }

    private static string? TryExtractWallpaperPathFromTranscodedCache(byte[] transcodedImageCache)
    {
        string decoded = Encoding.Unicode.GetString(transcodedImageCache);
        int pathStart = FindWallpaperPathStart(decoded);
        if (pathStart < 0)
        {
            return null;
        }

        int terminatorIndex = decoded.IndexOf('\0', pathStart);
        string candidate = terminatorIndex >= 0
            ? decoded[pathStart..terminatorIndex]
            : decoded[pathStart..];

        return candidate.Trim();
    }

    private static int FindWallpaperPathStart(string decoded)
    {
        for (int index = 0; index < decoded.Length - 2; index++)
        {
            if (char.IsLetter(decoded[index]) && decoded[index + 1] == ':' && decoded[index + 2] == '\\')
            {
                return index;
            }

            if (decoded[index] == '\\' && decoded[index + 1] == '\\')
            {
                return index;
            }
        }

        return -1;
    }

    public void Prewarm()
    {
        // Windows exposes the original wallpaper file directly, so there is no conversion cache to prewarm.
    }

}
