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

namespace ShareX.Platform;

/// <summary>The standard cursors an annotation can show. Each platform draws them from its own cursor set.</summary>
public enum SystemCursor
{
    Arrow,
    AppStarting,
    Cross,
    Hand,
    Help,
    IBeam,
    No,
    SizeAll,
    SizeNESW,
    SizeNS,
    SizeNWSE,
    SizeWE,
    UpArrow,
    Wait
}

/// <param name="Hotspot">The point of the image that is the cursor's position.</param>
public sealed record SystemCursorImage(PixelBuffer Image, PlatformPoint Hotspot);

/// <summary>Drawing that only the operating system can do the way users expect: its colour emoji and its cursors.</summary>
public interface ISystemGraphicsService
{
    /// <summary>
    /// Windows draws Segoe UI Emoji with DirectWrite. Elsewhere this is not supported and callers draw the system colour emoji font
    /// (Noto Color Emoji, Apple Color Emoji) with SkiaSharp themselves.
    /// </summary>
    FeatureSupport EmojiSupport { get; }

    /// <summary>Draws <paramref name="text"/> with the colour emoji font, centred on a transparent square. Null when it cannot.</summary>
    /// <param name="canvasSize">Width and height of the result in pixels.</param>
    /// <param name="fontSize">Font size in pixels.</param>
    PixelBuffer? RenderEmoji(string text, int canvasSize, float fontSize);

    /// <summary>Windows cursors on Windows; the user's cursor theme on Linux.</summary>
    FeatureSupport CursorSupport { get; }

    /// <summary>The image of a standard cursor, or null when this platform or theme has none.</summary>
    /// <param name="size">Nominal size in pixels; null for the system's default size.</param>
    SystemCursorImage? GetSystemCursor(SystemCursor cursor, int? size = null);
}
