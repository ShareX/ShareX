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

using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace ShareX.AvaloniaUI.Theming;

/// <summary>
/// Draws tray and menu icons with SkiaSharp so they look the same on Windows, macOS and Linux:
/// Lucide glyphs from the bundled font, and the upload or encoding progress square.
/// </summary>
public static class TrayIconRenderer
{
    private static readonly Uri LucideFontUri = new("avares://ShareX.Avalonia/Assets/lucide.ttf");
    private static readonly int[] IconSizes = [16, 20, 24, 32, 40, 48, 64];
    private static readonly Lazy<SKTypeface> LucideTypeface = new(LoadTypeface);

    /// <summary>A PNG of the glyph centred in a transparent square.</summary>
    public static byte[] RenderGlyphPng(string glyph, SKColor color, int size)
    {
        if (string.IsNullOrEmpty(glyph))
        {
            throw new ArgumentException("A Lucide glyph is required.", nameof(glyph));
        }

        using SKBitmap bitmap = new(new SKImageInfo(size, size, SKColorType.Bgra8888, SKAlphaType.Premul));
        using SKCanvas canvas = new(bitmap);
        using SKFont font = new(LucideTypeface.Value, size);
        using SKPaint paint = new() { Color = color, IsAntialias = true };

        canvas.Clear(SKColors.Transparent);
        font.MeasureText(glyph, out SKRect bounds, paint);
        float x = ((size - bounds.Width) / 2f) - bounds.Left;
        float y = ((size - bounds.Height) / 2f) - bounds.Top;
        canvas.DrawText(glyph, x, y, font, paint);
        canvas.Flush();

        return Encode(bitmap);
    }

    /// <summary>
    /// A square that fills from the bottom with the percentage written on top, the look ShareX has always used for progress.
    /// </summary>
    public static byte[] RenderProgressPng(int percentage, SKColor color, int size = 32)
    {
        percentage = Math.Clamp(percentage, 0, 100);

        using SKBitmap bitmap = new(new SKImageInfo(size, size, SKColorType.Bgra8888, SKAlphaType.Premul));
        using SKCanvas canvas = new(bitmap);
        using SKPaint background = new() { Color = new SKColor(39, 39, 39) };
        canvas.Clear(SKColors.Transparent);
        canvas.DrawRect(0, 0, size, size, background);

        int filled = (int)(size * (percentage / 100f));

        if (filled > 0)
        {
            using SKPaint fill = new() { Color = color };
            canvas.DrawRect(0, size - filled, size, filled, fill);

            if (filled < size)
            {
                color.ToHsl(out float h, out float s, out float l);
                using SKPaint edge = new() { Color = SKColor.FromHsl(h, s, Math.Min(100, l + 30)), StrokeWidth = Math.Max(1, size / 16f) };
                canvas.DrawLine(0, size - filled, size, size - filled, edge);
            }
        }

        using SKFont font = new(SKTypeface.Default, size * 0.6f);
        using SKPaint text = new() { Color = SKColors.White, IsAntialias = true };
        string label = Math.Min(percentage, 99).ToString(System.Globalization.CultureInfo.InvariantCulture);
        font.MeasureText(label, out SKRect bounds, text);
        canvas.DrawText(label, ((size - bounds.Width) / 2f) - bounds.Left, ((size - bounds.Height) / 2f) - bounds.Top, font, text);
        canvas.Flush();

        return Encode(bitmap);
    }

    /// <summary>A multi-resolution .ico of the glyph, for Windows APIs that take an icon.</summary>
    public static byte[] RenderGlyphIco(string glyph, SKColor color)
    {
        List<byte[]> images = new(IconSizes.Length);

        foreach (int size in IconSizes)
        {
            images.Add(RenderGlyphPng(glyph, color, size));
        }

        using MemoryStream stream = new();
        using BinaryWriter writer = new(stream);

        writer.Write((ushort)0); // Reserved
        writer.Write((ushort)1); // Icon
        writer.Write((ushort)images.Count);

        int imageOffset = 6 + (16 * images.Count);

        for (int index = 0; index < images.Count; index++)
        {
            int size = IconSizes[index];
            byte[] image = images[index];
            writer.Write((byte)size);
            writer.Write((byte)size);
            writer.Write((byte)0); // Color palette
            writer.Write((byte)0); // Reserved
            writer.Write((ushort)1); // Color planes
            writer.Write((ushort)32); // Bits per pixel
            writer.Write(image.Length);
            writer.Write(imageOffset);
            imageOffset += image.Length;
        }

        foreach (byte[] image in images)
        {
            writer.Write(image);
        }

        writer.Flush();
        return stream.ToArray();
    }

    /// <summary>A tray icon for Avalonia's TrayIcon, which Windows, macOS and Linux (StatusNotifierItem) all show.</summary>
    public static WindowIcon CreateWindowIcon(string glyph, SKColor color, int size = 64)
    {
        using MemoryStream stream = new(RenderGlyphPng(glyph, color, size));
        return new WindowIcon(stream);
    }

    public static WindowIcon CreateProgressWindowIcon(int percentage, SKColor color, int size = 64)
    {
        using MemoryStream stream = new(RenderProgressPng(percentage, color, size));
        return new WindowIcon(stream);
    }

    /// <summary>A small image for a menu item.</summary>
    public static Bitmap CreateMenuBitmap(string glyph, SKColor color, int size = 32)
    {
        using MemoryStream stream = new(RenderGlyphPng(glyph, color, size));
        return new Bitmap(stream);
    }

    private static SKTypeface LoadTypeface()
    {
        using Stream stream = AssetLoader.Open(LucideFontUri);
        return SKTypeface.FromStream(stream) ?? throw new InvalidOperationException("Unable to load the bundled Lucide font.");
    }

    private static byte[] Encode(SKBitmap bitmap)
    {
        using SKImage image = SKImage.FromBitmap(bitmap);
        using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }
}
