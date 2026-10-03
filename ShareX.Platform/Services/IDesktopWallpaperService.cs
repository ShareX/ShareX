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

namespace ShareX.Platform;

/// <summary>How the desktop presents its wallpaper image.</summary>
public enum DesktopWallpaperLayout
{
    Fill,
    Fit,
    Stretch,
    Center,
    Tile,
    Span
}

/// <param name="Path">An absolute path to an image ShareX can decode (converted to PNG where the desktop uses another format).</param>
public sealed record DesktopWallpaper(string Path, DesktopWallpaperLayout Layout);

/// <summary>The current desktop wallpaper, which the image editor can use as a background.</summary>
public interface IDesktopWallpaperService
{
    FeatureSupport Support { get; }

    /// <summary>Whether the first lookup is slow (it may convert the image) and is worth starting early in the background.</summary>
    bool RequiresPrewarm { get; }

    /// <summary>The wallpaper, or null when it cannot be found.</summary>
    DesktopWallpaper? GetWallpaper();

    /// <summary>Does the slow part of the first lookup ahead of time.</summary>
    void Prewarm();
}
