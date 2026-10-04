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
using ShareX.Platform.MacOS.Native;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace ShareX.Platform.MacOS;

/// <summary>
/// Standard cursors from AppKit's NSCursor, at the resolution asked for. Emoji are drawn by ShareX with SkiaSharp and Apple Color
/// Emoji, as on Linux.
/// </summary>
[SupportedOSPlatform("macos")]
public sealed class AppKitGraphicsService : ISystemGraphicsService
{
    /// <summary>
    /// NSCursor class methods for each cursor, in order of preference. The underscored ones are the cursors macOS itself shows for
    /// window resizing, help and moving; they have no public name, so each is used only when NSCursor still responds to it.
    /// </summary>
    internal static readonly Dictionary<SystemCursor, string[]> CursorSelectors = new()
    {
        [SystemCursor.Arrow] = ["arrowCursor"],
        [SystemCursor.AppStarting] = ["_busyButClickableCursor"],
        [SystemCursor.Cross] = ["crosshairCursor"],
        [SystemCursor.Hand] = ["pointingHandCursor"],
        [SystemCursor.Help] = ["_helpCursor"],
        [SystemCursor.IBeam] = ["IBeamCursor"],
        [SystemCursor.No] = ["operationNotAllowedCursor"],
        [SystemCursor.SizeAll] = ["_moveCursor", "openHandCursor"],
        [SystemCursor.SizeNESW] = ["_windowResizeNorthEastSouthWestCursor"],
        [SystemCursor.SizeNS] = ["_windowResizeNorthSouthCursor", "resizeUpDownCursor"],
        [SystemCursor.SizeNWSE] = ["_windowResizeNorthWestSouthEastCursor"],
        [SystemCursor.SizeWE] = ["_windowResizeEastWestCursor", "resizeLeftRightCursor"],
        [SystemCursor.UpArrow] = ["resizeUpCursor"],
        // macOS shows the spinning wait cursor itself; applications cannot set or read it.
        [SystemCursor.Wait] = []
    };

    public FeatureSupport EmojiSupport { get; } = FeatureSupport.NotSupported("ShareX draws emoji with Apple Color Emoji on macOS.");

    public PixelBuffer? RenderEmoji(string text, int canvasSize, float fontSize) => null;

    public FeatureSupport CursorSupport => FeatureSupport.Supported;

    public SystemCursorImage? GetSystemCursor(SystemCursor cursor, int? size = null)
    {
        if (!CursorSelectors.TryGetValue(cursor, out string[]? selectors))
        {
            return null;
        }

        return ObjC.WithAutoreleasePool(() =>
        {
            IntPtr cursorClass = ObjC.GetClass("NSCursor");

            foreach (string selector in selectors)
            {
                if (ObjC.RespondsTo(cursorClass, selector) && ReadCursor(ObjC.Send(cursorClass, selector), size, null) is { } image)
                {
                    return image;
                }
            }

            return null;
        });
    }

    /// <summary>The image and hot spot of an NSCursor, with the hot spot moved from points to the chosen representation's pixels.</summary>
    /// <param name="pixelHeight">The image height wanted, in pixels; null for the largest.</param>
    /// <param name="displayScale">Instead of a height, the pixels per point of the display the cursor is drawn on.</param>
    internal static SystemCursorImage? ReadCursor(IntPtr cursor, int? pixelHeight, double? displayScale)
    {
        if (cursor == IntPtr.Zero)
        {
            return null;
        }

        IntPtr image = ObjC.Send(cursor, "image");

        if (displayScale is double pixelsPerPoint && image != IntPtr.Zero)
        {
            pixelHeight = (int)Math.Round(ObjC.SendPoint(image, "size").Y * pixelsPerPoint);
        }

        (IntPtr representation, double scale) = AppKitImages.GetRepresentation(image, pixelHeight);

        if (AppKitImages.ToPng(representation) is not { } png)
        {
            return null;
        }

        PixelBuffer pixels;

        try
        {
            pixels = PngCodec.Decode(png);
        }
        catch (Exception e) when (e is NotSupportedException or System.IO.InvalidDataException)
        {
            return null;
        }

        CoreGraphics.CGPoint hotspot = ObjC.SendPoint(cursor, "hotSpot");
        return new SystemCursorImage(pixels, new PlatformPoint((int)Math.Round(hotspot.X * scale), (int)Math.Round(hotspot.Y * scale)));
    }
}
