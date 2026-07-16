#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ShareX.AvaloniaUI.Imaging;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace ShareX;

internal sealed class LucideNativeIconRenderer : IDisposable
{
    private readonly Dictionary<string, Bitmap> _cache = new();
    private readonly SKTypeface _typeface;

    public LucideNativeIconRenderer()
    {
        using Stream stream = AssetLoader.Open(new Uri("avares://ShareX.Avalonia/Assets/lucide.ttf"));
        _typeface = SKTypeface.FromStream(stream) ?? SKTypeface.Default;
    }

    public Bitmap Get(string glyph)
    {
        if (_cache.TryGetValue(glyph, out Bitmap? bitmap))
        {
            return bitmap;
        }

        using SKBitmap skBitmap = new(20, 20, SKColorType.Bgra8888, SKAlphaType.Premul);
        using SKCanvas canvas = new(skBitmap);
        using SKPaint paint = new()
        {
            IsAntialias = true,
            Color = new SKColor(80, 80, 80)
        };
        using SKFont font = new(_typeface, 16);

        canvas.Clear(SKColors.Transparent);
        SKFontMetrics metrics = font.Metrics;
        float y = 10 - ((metrics.Ascent + metrics.Descent) / 2);
        canvas.DrawText(glyph, 10, y, SKTextAlign.Center, font, paint);

        bitmap = BitmapConversionHelpers.ToAvaloniBitmap(skBitmap);
        _cache.Add(glyph, bitmap);
        return bitmap;
    }

    public void Dispose()
    {
        foreach (Bitmap bitmap in _cache.Values)
        {
            bitmap.Dispose();
        }

        _cache.Clear();
        _typeface.Dispose();
    }
}
