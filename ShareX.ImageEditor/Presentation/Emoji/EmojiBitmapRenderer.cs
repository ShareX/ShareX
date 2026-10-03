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

using Avalonia.Media.Imaging;
using SkiaSharp;
using ShareX.ImageEditor.Presentation.Rendering;
using ShareX.Platform;

namespace ShareX.ImageEditor.Presentation.Emoji;

public static class EmojiBitmapRenderer
{
    private const int PreviewPadding = 6;
    private const int StickerPadding = 14;
    private const int MaxStickerCacheEntries = 256;
    private const float RawCanvasScale = 2.0f;
    private const float RawFontScale = 1.32f;

    private static readonly object SyncRoot = new();
    private static readonly Dictionary<string, WeakReference<Bitmap>> PreviewCache = new(StringComparer.Ordinal);
    private static readonly Dictionary<string, SKBitmap> StickerCache = new(StringComparer.Ordinal);
    private static ISystemGraphicsService? cachedGraphics;

    private static void RefreshGraphicsCache()
    {
        ISystemGraphicsService? graphics = PlatformServices.IsInitialized ? PlatformServices.Current.Graphics : null;
        if (ReferenceEquals(cachedGraphics, graphics)) return;
        cachedGraphics = graphics;
        PreviewCache.Clear();
        ClearStickerCache();
    }

    public static Bitmap? RenderPreviewBitmap(string unicodeSequence, int size)
    {
        if (string.IsNullOrWhiteSpace(unicodeSequence) || size <= 0)
        {
            return null;
        }

        string cacheKey = $"{unicodeSequence}:{size}";

        lock (SyncRoot)
        {
            RefreshGraphicsCache();
            if (PreviewCache.TryGetValue(cacheKey, out var reference) && reference.TryGetTarget(out Bitmap? cached))
            {
                return cached;
            }

            using SKBitmap? bitmap = RenderSquareBitmap(unicodeSequence, size, PreviewPadding);
            if (bitmap == null)
            {
                return null;
            }

            Bitmap preview = BitmapConversionHelpers.ToAvaloniBitmap(bitmap);
            // Controls borrow previews; evict references without disposing images still in use.
            if (PreviewCache.Count >= 256) PreviewCache.Clear();
            PreviewCache[cacheKey] = new WeakReference<Bitmap>(preview);
            return preview;
        }
    }

    public static SKBitmap? RenderStickerBitmap(string unicodeSequence, int size = 160)
    {
        if (string.IsNullOrWhiteSpace(unicodeSequence) || size <= 0)
        {
            return null;
        }

        return RenderStickerBitmapCore(unicodeSequence, size);
    }

    public static SKBitmap? RenderInteractiveStickerBitmap(string unicodeSequence, int size = 160)
    {
        if (string.IsNullOrWhiteSpace(unicodeSequence) || size <= 0)
        {
            return null;
        }

        return RenderStickerBitmapCore(unicodeSequence, GetInteractiveStickerSize(size));
    }

    public static int GetInteractiveStickerSize(int size)
    {
        size = Math.Max(1, size);

        if (size <= 64)
        {
            return size;
        }

        int step = size <= 128 ? 4 : size <= 256 ? 8 : 12;
        return Math.Max(64, (int)Math.Round(size / (double)step) * step);
    }

    private static SKBitmap? RenderSquareBitmap(string unicodeSequence, int size, int padding)
    {
        string glyph = EmojiCatalogService.ToGlyph(unicodeSequence);
        if (string.IsNullOrEmpty(glyph))
        {
            return null;
        }

        try
        {
            int rawCanvasSize = Math.Max(64, (int)Math.Ceiling(size * RawCanvasScale));
            float rawFontSize = Math.Max(24, size * RawFontScale);

            using SKBitmap? rawBitmap = RenderWithPlatform(glyph, rawCanvasSize, rawFontSize) ?? RenderWithSkiaFallback(glyph, rawCanvasSize);
            if (rawBitmap == null)
            {
                return null;
            }

            using SKBitmap trimmedBitmap = TrimTransparentBounds(rawBitmap) ?? rawBitmap.Copy();
            return FitIntoSquare(trimmedBitmap, size, padding);
        }
        catch
        {
            return RenderWithSkiaFallback(glyph, size);
        }
    }

    private static SKBitmap? RenderWithPlatform(string glyph, int canvasSize, float fontSize)
    {
        if (cachedGraphics == null || !cachedGraphics.EmojiSupport.IsSupported) return null;
        return cachedGraphics.RenderEmoji(glyph, canvasSize, fontSize) is { } pixels
            ? SystemGraphicsBitmapConversion.ToSkBitmap(pixels) : null;
    }

    private static SKBitmap? RenderWithSkiaFallback(string glyph, int canvasSize)
    {
        return SkiaEmojiBitmapRenderer.Render(glyph, canvasSize);
    }

    private static SKBitmap? TrimTransparentBounds(SKBitmap source)
    {
        int minX = source.Width;
        int minY = source.Height;
        int maxX = -1;
        int maxY = -1;

        for (int y = 0; y < source.Height; y++)
        {
            for (int x = 0; x < source.Width; x++)
            {
                if (source.GetPixel(x, y).Alpha == 0)
                {
                    continue;
                }

                if (x < minX) minX = x;
                if (y < minY) minY = y;
                if (x > maxX) maxX = x;
                if (y > maxY) maxY = y;
            }
        }

        if (maxX < minX || maxY < minY)
        {
            return null;
        }

        int width = Math.Max(1, maxX - minX + 1);
        int height = Math.Max(1, maxY - minY + 1);
        var trimmed = new SKBitmap(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul));

        using var canvas = new SKCanvas(trimmed);
        canvas.Clear(SKColors.Transparent);
        canvas.DrawBitmap(
            source,
            new SKRectI(minX, minY, maxX + 1, maxY + 1),
            new SKRect(0, 0, width, height));

        return trimmed;
    }

    private static SKBitmap FitIntoSquare(SKBitmap source, int size, int padding)
    {
        var output = new SKBitmap(new SKImageInfo(size, size, SKColorType.Bgra8888, SKAlphaType.Premul));
        using var canvas = new SKCanvas(output);
        using var paint = new SKPaint
        {
            IsAntialias = true
        };

        canvas.Clear(SKColors.Transparent);

        float availableWidth = Math.Max(1, size - (padding * 2));
        float availableHeight = Math.Max(1, size - (padding * 2));
        float scale = Math.Min(availableWidth / source.Width, availableHeight / source.Height);
        float drawWidth = Math.Max(1, source.Width * scale);
        float drawHeight = Math.Max(1, source.Height * scale);
        float left = (size - drawWidth) / 2f;
        float top = (size - drawHeight) / 2f;

        using SKImage sourceImage = SKImage.FromBitmap(source);
        canvas.DrawImage(sourceImage, new SKRect(left, top, left + drawWidth, top + drawHeight), new SKSamplingOptions(SKCubicResampler.CatmullRom), paint);
        return output;
    }

    private static SKBitmap? RenderStickerBitmapCore(string unicodeSequence, int size)
    {
        string cacheKey = $"{unicodeSequence}:{size}";

        lock (SyncRoot)
        {
            RefreshGraphicsCache();
            if (StickerCache.TryGetValue(cacheKey, out SKBitmap? cachedBitmap))
            {
                return cachedBitmap.Copy();
            }

            SKBitmap? renderedBitmap = RenderSquareBitmap(unicodeSequence, size, StickerPadding);
            if (renderedBitmap == null)
            {
                return null;
            }

            if (StickerCache.Count >= MaxStickerCacheEntries)
            {
                ClearStickerCache();
            }

            StickerCache[cacheKey] = renderedBitmap.Copy();
            return renderedBitmap;
        }
    }

    private static void ClearStickerCache()
    {
        foreach (SKBitmap bitmap in StickerCache.Values)
        {
            bitmap.Dispose();
        }

        StickerCache.Clear();
    }
}
