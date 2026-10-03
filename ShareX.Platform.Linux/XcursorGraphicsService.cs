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
using ShareX.Platform.Linux.Native;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ShareX.Platform.Linux;

/// <summary>Standard cursors from the user's cursor theme. Emoji are drawn by ShareX with SkiaSharp and the colour emoji font.</summary>
public sealed class XcursorGraphicsService : ISystemGraphicsService
{
    // Each cursor by its freedesktop name, then the older X11 core names that themes still ship.
    private static readonly Dictionary<SystemCursor, string[]> CursorNames = new()
    {
        [SystemCursor.Arrow] = ["default", "left_ptr", "arrow"],
        [SystemCursor.AppStarting] = ["progress", "left_ptr_watch", "half-busy"],
        [SystemCursor.Cross] = ["crosshair", "cross", "tcross"],
        [SystemCursor.Hand] = ["pointer", "hand2", "hand1", "pointing_hand"],
        [SystemCursor.Help] = ["help", "question_arrow", "whats_this", "left_ptr_help"],
        [SystemCursor.IBeam] = ["text", "xterm", "ibeam"],
        [SystemCursor.No] = ["not-allowed", "crossed_circle", "forbidden", "no-drop"],
        [SystemCursor.SizeAll] = ["all-scroll", "move", "fleur", "size_all"],
        [SystemCursor.SizeNESW] = ["nesw-resize", "size_bdiag", "fd_double_arrow"],
        [SystemCursor.SizeNS] = ["ns-resize", "size_ver", "sb_v_double_arrow", "v_double_arrow"],
        [SystemCursor.SizeNWSE] = ["nwse-resize", "size_fdiag", "bd_double_arrow"],
        [SystemCursor.SizeWE] = ["ew-resize", "size_hor", "sb_h_double_arrow", "h_double_arrow"],
        [SystemCursor.UpArrow] = ["sb_up_arrow", "up_arrow", "n-resize", "top_side"],
        [SystemCursor.Wait] = ["wait", "watch"]
    };

    private readonly string? theme;
    private readonly int defaultSize;

    public XcursorGraphicsService()
        : this(Environment.GetEnvironmentVariable("XCURSOR_THEME"), Environment.GetEnvironmentVariable("XCURSOR_SIZE"))
    {
    }

    /// <param name="theme">Null uses the "default" theme, which links to the user's choice on most desktops.</param>
    public XcursorGraphicsService(string? theme, string? size)
    {
        this.theme = string.IsNullOrWhiteSpace(theme) ? null : theme;
        defaultSize = int.TryParse(size, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value) && value > 0 ? value : 24;
    }

    public FeatureSupport EmojiSupport { get; } = FeatureSupport.NotSupported("ShareX draws emoji with the colour emoji font on Linux.");

    public PixelBuffer? RenderEmoji(string text, int canvasSize, float fontSize) => null;

    public FeatureSupport CursorSupport => Xcursor.IsAvailable
        ? FeatureSupport.Supported
        : FeatureSupport.NotSupported("Cursor images need libXcursor (package libxcursor or libxcursor1).");

    public unsafe SystemCursorImage? GetSystemCursor(SystemCursor cursor, int? size = null)
    {
        if (!Xcursor.IsAvailable || !CursorNames.TryGetValue(cursor, out string[]? names))
        {
            return null;
        }

        foreach (string name in names)
        {
            Xcursor.XcursorImage* image = Xcursor.XcursorLibraryLoadImage(name, theme, size ?? defaultSize);

            if (image == null)
            {
                continue;
            }

            try
            {
                int width = (int)image->Width;
                int height = (int)image->Height;
                return new SystemCursorImage(FromPremultipliedArgb(new ReadOnlySpan<uint>(image->Pixels, width * height), width, height),
                    new PlatformPoint((int)image->XHot, (int)image->YHot));
            }
            finally
            {
                Xcursor.XcursorImageDestroy(image);
            }
        }

        return null;
    }

    internal static IReadOnlyList<string> GetCursorNames(SystemCursor cursor) => CursorNames[cursor];

    /// <summary>Xcursor pixels are premultiplied 0xAARRGGBB; PixelBuffer is straight BGRA.</summary>
    internal static PixelBuffer FromPremultipliedArgb(ReadOnlySpan<uint> pixels, int width, int height)
    {
        PixelBuffer buffer = new PixelBuffer(width, height);
        byte[] output = buffer.Pixels;

        for (int i = 0; i < width * height; i++)
        {
            uint argb = pixels[i];
            byte a = (byte)(argb >> 24);
            int o = i * 4;

            if (a == 0)
            {
                continue;
            }

            output[o] = Unpremultiply((byte)argb, a);
            output[o + 1] = Unpremultiply((byte)(argb >> 8), a);
            output[o + 2] = Unpremultiply((byte)(argb >> 16), a);
            output[o + 3] = a;
        }

        return buffer;
    }

    private static byte Unpremultiply(byte value, byte alpha) => alpha == 255 ? value : (byte)Math.Min(255, (value * 255 + alpha / 2) / alpha);
}
