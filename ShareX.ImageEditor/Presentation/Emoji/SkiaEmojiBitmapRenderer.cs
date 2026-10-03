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

using SkiaSharp;
using SkiaSharp.HarfBuzz;

namespace ShareX.ImageEditor.Presentation.Emoji;

/// <summary>Draws shaped emoji with an installed color font, including joined and skin-tone sequences.</summary>
internal static class SkiaEmojiBitmapRenderer
{
    private static readonly string[] ColorFontFamilies = ["Noto Color Emoji", "Apple Color Emoji", "Segoe UI Emoji"];
    private static readonly Lazy<HashSet<string>> InstalledFamilies = new(() =>
        new HashSet<string>(SKFontManager.Default.FontFamilies, StringComparer.OrdinalIgnoreCase));

    public static SKBitmap? Render(string glyph, int canvasSize)
    {
        if (string.IsNullOrWhiteSpace(glyph) || canvasSize <= 0)
        {
            return null;
        }

        foreach (string family in ColorFontFamilies)
        {
            // MatchFamily may silently substitute the default font for a missing family.
            if (!InstalledFamilies.Value.Contains(family)) continue;
            using SKTypeface? typeface = SKFontManager.Default.MatchFamily(family);
            if (typeface == null) continue;
            SKBitmap? bitmap = Render(glyph, canvasSize, typeface);
            if (bitmap != null) return bitmap;
        }

        // Systems without a color font can still draw supported monochrome emoji.
        using SKTypeface? fallback = SKFontManager.Default.MatchCharacter(glyph.EnumerateRunes().First().Value);
        return fallback == null ? null : Render(glyph, canvasSize, fallback);
    }

    internal static SKBitmap? Render(string glyph, int canvasSize, SKTypeface typeface)
    {
        using var font = new SKFont(typeface, canvasSize * 0.68f);
        using var shaper = new SKShaper(typeface);
        SKShaper.Result shaped = shaper.Shape(glyph, font);
        if (shaped.Codepoints.Length == 0 || shaped.Codepoints.Any(glyphId => glyphId == 0)) return null;

        ushort[] glyphs = Array.ConvertAll(shaped.Codepoints, glyphId => checked((ushort)glyphId));
        using var builder = new SKTextBlobBuilder();
        builder.AddPositionedRun(glyphs, font, shaped.Points);
        using SKTextBlob? blob = builder.Build();
        if (blob == null || blob.Bounds.IsEmpty) return null;

        var bitmap = new SKBitmap(new SKImageInfo(canvasSize, canvasSize, SKColorType.Bgra8888, SKAlphaType.Premul));
        using var canvas = new SKCanvas(bitmap);
        using var paint = new SKPaint { IsAntialias = true, Color = SKColors.White };
        canvas.Clear(SKColors.Transparent);

        SKRect bounds = blob.Bounds;
        float scale = Math.Min(1, Math.Min(canvasSize * 0.9f / bounds.Width, canvasSize * 0.9f / bounds.Height));
        canvas.Translate(canvasSize / 2f, canvasSize / 2f);
        canvas.Scale(scale);
        canvas.DrawText(blob, -bounds.MidX, -bounds.MidY, paint);
        canvas.Flush();
        return bitmap;
    }
}
