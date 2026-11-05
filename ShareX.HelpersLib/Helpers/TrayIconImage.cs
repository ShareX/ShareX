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

#nullable enable

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Microsoft.Win32;
using ShareX.Icons;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace ShareX.HelpersLib;

/// <summary>
/// Creates DPI-friendly tray icons from ShareX icons.
/// </summary>
public static class TrayIconImage
{
    private const string PersonalizeRegistryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";
    private const string SystemUsesLightThemeRegistryValue = "SystemUsesLightTheme";
    private static readonly int[] IconSizes = [16, 20, 24, 32, 40, 48, 64];

    /// <summary>
    /// Assigns an icon to a tray icon and keeps its color in sync with the
    /// Windows taskbar theme. Dispose the returned binding before the tray icon.
    /// </summary>
    public static IDisposable Bind(TrayIcon trayIcon, string icon)
    {
        ArgumentNullException.ThrowIfNull(trayIcon);

        if (string.IsNullOrEmpty(icon))
        {
            throw new ArgumentException("An icon key is required.", nameof(icon));
        }

        return new ThemeBinding(trayIcon, icon);
    }

    /// <summary>
    /// Creates a small bitmap using the color appropriate for the current
    /// Windows taskbar theme.
    /// </summary>
    public static SKBitmap CreateImage(string icon)
    {
        return CreateImage(icon, GetThemeIconColor());
    }

    /// <summary>
    /// Creates a small bitmap using a specific icon color.
    /// </summary>
    public static SKBitmap CreateImage(string icon, System.Drawing.Color color)
    {
        if (string.IsNullOrEmpty(icon))
        {
            throw new ArgumentException("An icon key is required.", nameof(icon));
        }

        SKColor skColor = new(color.R, color.G, color.B, color.A);
        int size = OperatingSystem.IsWindows() ? Math.Max(NativeMethods.GetSystemMetrics(SystemMetric.SM_CXSMICON),
            NativeMethods.GetSystemMetrics(SystemMetric.SM_CYSMICON)) : 16;
        byte[] imageData = RenderIcon(icon, skColor, size);

        using MemoryStream stream = new(imageData, writable: false);
        return SkiaImageHelpers.Decode(stream);
    }

    public static byte[] CreateIconBytes(string icon) => CreateIconBytes(icon, GetThemeIconColor());

    public static byte[] CreateIconBytes(string icon, System.Drawing.Color color)
        => CreateIconData(icon, color.ToSKColor());

    private static System.Drawing.Color GetThemeIconColor()
    {
        return (OperatingSystem.IsWindows() ? IsLightTaskbarTheme() :
            Application.Current?.PlatformSettings?.GetColorValues().ThemeVariant == PlatformThemeVariant.Light) ?
            System.Drawing.Color.Black :
            System.Drawing.Color.White;
    }

    private static bool IsLightTaskbarTheme()
    {
        int? value = RegistryHelpers.GetValueDWord(
            PersonalizeRegistryPath,
            SystemUsesLightThemeRegistryValue,
            RegistryHive.CurrentUser);

        return value != 0;
    }

    private static byte[] CreateIconData(string icon, SKColor color)
    {
        List<byte[]> images = new(IconSizes.Length);

        foreach (int size in IconSizes)
        {
            images.Add(RenderIcon(icon, color, size));
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

    private static byte[] RenderIcon(string icon, SKColor color, int size)
    {
        VectorIcon vectorIcon = IconCatalog.Find(icon) ?? throw new ArgumentException("Unknown icon key.", nameof(icon));
        using RenderTargetBitmap bitmap = new(new PixelSize(size, size), new Vector(96, 96));

        using (DrawingContext context = bitmap.CreateDrawingContext())
        {
            vectorIcon.Draw(context, new Rect(0, 0, size, size), new ImmutableSolidColorBrush(Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue)));
        }

        using MemoryStream stream = new();
        bitmap.Save(stream, PngBitmapEncoderOptions.Default);
        return stream.ToArray();
    }

    private sealed class ThemeBinding : IDisposable
    {
        private readonly TrayIcon _trayIcon;
        private readonly string _icon;
        private readonly SynchronizationContext? _synchronizationContext;
        private bool _disposed;

        public ThemeBinding(TrayIcon trayIcon, string icon)
        {
            _trayIcon = trayIcon;
            _icon = icon;
            _synchronizationContext = SynchronizationContext.Current;

            RefreshIcon();
            if (OperatingSystem.IsWindows())
            {
                SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
                SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
            }
            if (Application.Current?.PlatformSettings is { } platform) platform.ColorValuesChanged += OnColorsChanged;
        }

        private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            QueueRefresh();
        }

        private void OnDisplaySettingsChanged(object? sender, EventArgs e)
        {
            QueueRefresh();
        }

        private void OnColorsChanged(object? sender, PlatformColorValues e) => QueueRefresh();

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

            using MemoryStream stream = new(CreateIconBytes(_icon), writable: false);
            _trayIcon.Icon = new WindowIcon(stream);
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            if (OperatingSystem.IsWindows())
            {
                SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
                SystemEvents.DisplaySettingsChanged -= OnDisplaySettingsChanged;
            }
            if (Application.Current?.PlatformSettings is { } platform) platform.ColorValuesChanged -= OnColorsChanged;
            _trayIcon.Icon = null;
        }
    }
}
