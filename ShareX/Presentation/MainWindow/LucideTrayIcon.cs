#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.
*/

#endregion License Information (GPL v3)

#nullable enable

using Avalonia.Platform;
using Microsoft.Win32;
using ShareX.HelpersLib;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ShareX;

/// <summary>
/// Creates DPI-friendly tray icons from the bundled Lucide font.
/// </summary>
internal static class LucideTrayIcon
{
    private const string PersonalizeRegistryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";
    private const string SystemUsesLightThemeRegistryValue = "SystemUsesLightTheme";
    private static readonly Uri LucideFontUri = new("avares://ShareX.Avalonia/Assets/lucide.ttf");
    private static readonly int[] IconSizes = [16, 20, 24, 32, 40, 48, 64];
    private static readonly Lazy<SKTypeface> LucideTypeface = new(LoadTypeface);

    /// <summary>
    /// Assigns a Lucide glyph to a tray icon and keeps its color in sync with the
    /// Windows taskbar theme. Dispose the returned binding before the tray icon.
    /// </summary>
    public static IDisposable Bind(NotifyIcon trayIcon, string glyph)
    {
        ArgumentNullException.ThrowIfNull(trayIcon);

        if (string.IsNullOrEmpty(glyph))
        {
            throw new ArgumentException("A Lucide glyph is required.", nameof(glyph));
        }

        return new ThemeBinding(trayIcon, glyph);
    }

    /// <summary>
    /// Creates a multi-resolution icon using the color appropriate for the
    /// current Windows taskbar theme.
    /// </summary>
    public static Icon Create(string glyph)
    {
        if (string.IsNullOrEmpty(glyph))
        {
            throw new ArgumentException("A Lucide glyph is required.", nameof(glyph));
        }

        SKColor color = IsLightTaskbarTheme() ? SKColors.Black : SKColors.White;
        byte[] iconData = CreateIconData(glyph, color);

        using MemoryStream stream = new(iconData, writable: false);
        using Icon icon = new(stream, SystemInformation.SmallIconSize);
        return (Icon)icon.Clone();
    }

    private static bool IsLightTaskbarTheme()
    {
        int? value = RegistryHelpers.GetValueDWord(
            PersonalizeRegistryPath,
            SystemUsesLightThemeRegistryValue,
            RegistryHive.CurrentUser);

        return value != 0;
    }

    private static SKTypeface LoadTypeface()
    {
        using Stream stream = AssetLoader.Open(LucideFontUri);
        return SKTypeface.FromStream(stream) ??
            throw new InvalidOperationException("Unable to load the bundled Lucide font.");
    }

    private static byte[] CreateIconData(string glyph, SKColor color)
    {
        List<byte[]> images = new(IconSizes.Length);

        foreach (int size in IconSizes)
        {
            images.Add(RenderGlyph(glyph, color, size));
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

    private static byte[] RenderGlyph(string glyph, SKColor color, int size)
    {
        using SKBitmap bitmap = new(new SKImageInfo(size, size, SKColorType.Bgra8888, SKAlphaType.Premul));
        using SKCanvas canvas = new(bitmap);
        using SKFont font = new(LucideTypeface.Value, size * 0.875f);
        using SKPaint paint = new()
        {
            Color = color,
            IsAntialias = true
        };

        canvas.Clear(SKColors.Transparent);

        font.MeasureText(glyph, out SKRect bounds, paint);
        float x = ((size - bounds.Width) / 2f) - bounds.Left;
        float y = ((size - bounds.Height) / 2f) - bounds.Top;
        canvas.DrawText(glyph, x, y, font, paint);
        canvas.Flush();

        using SKImage image = SKImage.FromBitmap(bitmap);
        using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    private sealed class ThemeBinding : IDisposable
    {
        private readonly NotifyIcon _trayIcon;
        private readonly string _glyph;
        private readonly SynchronizationContext? _synchronizationContext;
        private Icon? _ownedIcon;
        private bool _disposed;

        public ThemeBinding(NotifyIcon trayIcon, string glyph)
        {
            _trayIcon = trayIcon;
            _glyph = glyph;
            _synchronizationContext = SynchronizationContext.Current;

            RefreshIcon();
            SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
            SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
        }

        private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            QueueRefresh();
        }

        private void OnDisplaySettingsChanged(object? sender, EventArgs e)
        {
            QueueRefresh();
        }

        private void QueueRefresh()
        {
            if (_disposed)
            {
                return;
            }

            if (_synchronizationContext != null)
            {
                _synchronizationContext.Post(_ => RefreshIcon(), null);
            }
            else
            {
                RefreshIcon();
            }
        }

        private void RefreshIcon()
        {
            if (_disposed)
            {
                return;
            }

            Icon replacement = Create(_glyph);
            Icon? previous = _ownedIcon;
            _trayIcon.Icon = replacement;
            _ownedIcon = replacement;
            previous?.Dispose();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
            SystemEvents.DisplaySettingsChanged -= OnDisplaySettingsChanged;
            _trayIcon.Icon = null;
            _ownedIcon?.Dispose();
            _ownedIcon = null;
        }
    }
}
