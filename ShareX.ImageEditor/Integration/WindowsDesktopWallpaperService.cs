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
using System.Runtime.InteropServices;
using System.Text;

namespace ShareX.ImageEditor.Integration;

/// <summary>
/// Default Windows wallpaper resolver used by Avalonia hosts that do not provide a custom implementation.
/// </summary>
internal sealed class WindowsDesktopWallpaperService : IDesktopWallpaperService
{
    private const int SpiGetDesktopWallpaper = 0x0073;
    private const int MaxWallpaperPath = short.MaxValue;
    private const string DesktopRegistrySubKey = @"Control Panel\Desktop";
    private const string TranscodedImageCacheValueName = "TranscodedImageCache";

    public bool IsSupported => OperatingSystem.IsWindows();
    public bool RequiresDesktopWallpaperPrewarm => false;

    public bool TryGetDesktopWallpaper(out DesktopWallpaperInfo? wallpaper)
    {
        wallpaper = null;

        if (!OperatingSystem.IsWindows())
        {
            return false;
        }

        StringBuilder buffer = new StringBuilder(MaxWallpaperPath);
        if (SystemParametersInfo(SpiGetDesktopWallpaper, buffer.Capacity, buffer, 0))
        {
            string wallpaperPath = buffer.ToString().TrimEnd('\0');
            if (TryCreateDesktopWallpaperInfo(wallpaperPath, out wallpaper))
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(wallpaperPath))
            {
                return false;
            }
        }

        return TryGetDesktopWallpaperFromRegistryCache(out wallpaper);
    }

    private static bool TryGetDesktopWallpaperFromRegistryCache(out DesktopWallpaperInfo? wallpaper)
    {
        wallpaper = null;

        if (!OperatingSystem.IsWindows())
        {
            return false;
        }

        using RegistryKey? desktopKey = Registry.CurrentUser.OpenSubKey(DesktopRegistrySubKey);
        if (desktopKey?.GetValue(TranscodedImageCacheValueName) is not byte[] transcodedImageCache || transcodedImageCache.Length == 0)
        {
            return false;
        }

        string? wallpaperPath = TryExtractWallpaperPathFromTranscodedCache(transcodedImageCache);
        return TryCreateDesktopWallpaperInfo(wallpaperPath, out wallpaper);
    }

    private static bool TryCreateDesktopWallpaperInfo(string? wallpaperPath, out DesktopWallpaperInfo? wallpaper)
    {
        wallpaper = null;

        if (string.IsNullOrWhiteSpace(wallpaperPath) || !File.Exists(wallpaperPath))
        {
            return false;
        }

        wallpaper = new DesktopWallpaperInfo
        {
            Path = wallpaperPath,
            Layout = DesktopWallpaperLayout.Fill
        };

        return true;
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

    public void PrewarmDesktopWallpaper()
    {
        // Windows exposes the original wallpaper file directly, so there is no conversion cache to prewarm.
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool SystemParametersInfo(int uiAction, int uiParam, StringBuilder pvParam, int fWinIni);
}