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

using ShareX.Platform.Diagnostics;
using ShareX.Platform.MacOS.Native;
using System;
using System.IO;
using System.Runtime.Versioning;

namespace ShareX.Platform.MacOS;

/// <summary>
/// The main screen's wallpaper from NSWorkspace. macOS wallpapers are often HEIC, which ShareX does not decode, so they are
/// converted once to PNG with sips (part of macOS) in ShareX's cache.
/// </summary>
[SupportedOSPlatform("macos")]
public sealed class MacDesktopWallpaperService(ICommandRunner runner, string cacheDirectory) : IDesktopWallpaperService
{
    private readonly object sync = new object();
    private DesktopWallpaper? cached;
    private string? cachedSource;

    public FeatureSupport Support => FeatureSupport.Supported;

    public bool RequiresPrewarm => true;

    public void Prewarm() => GetWallpaper();

    public DesktopWallpaper? GetWallpaper()
    {
        string? source = ObjC.WithAutoreleasePool(() =>
        {
            IntPtr workspace = ObjC.Send(ObjC.GetClass("NSWorkspace"), "sharedWorkspace");
            IntPtr screen = ObjC.Send(ObjC.GetClass("NSScreen"), "mainScreen");
            IntPtr url = screen != IntPtr.Zero ? ObjC.Send(workspace, "desktopImageURLForScreen:", screen) : IntPtr.Zero;
            return url != IntPtr.Zero ? CoreFoundation.ToManagedString(ObjC.Send(url, "path")) : null;
        });

        if (string.IsNullOrEmpty(source) || !File.Exists(source))
        {
            return null;
        }

        lock (sync)
        {
            if (cached != null && cachedSource == source)
            {
                return cached;
            }

            string path = source;
            string extension = Path.GetExtension(source).ToLowerInvariant();

            if (extension is not (".png" or ".jpg" or ".jpeg" or ".bmp" or ".gif" or ".webp"))
            {
                Directory.CreateDirectory(cacheDirectory);
                path = Path.Combine(cacheDirectory, "wallpaper.png");
                CommandResult result = runner.RunAsync("sips", ["-s", "format", "png", source, "--out", path], timeout: TimeSpan.FromSeconds(30))
                    .GetAwaiter().GetResult();

                if (!result.Success || !File.Exists(path))
                {
                    return null;
                }
            }

            cachedSource = source;
            // macOS fills the screen by default.
            cached = new DesktopWallpaper(path, DesktopWallpaperLayout.Fill);
            return cached;
        }
    }
}
