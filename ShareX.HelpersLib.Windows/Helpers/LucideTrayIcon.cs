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

using Avalonia.Platform;
using Microsoft.Win32;
using ShareX.AvaloniaUI.Theming;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using Avalonia.Controls;
using Avalonia;

namespace ShareX.HelpersLib;

/// <summary>
/// Creates DPI-friendly tray icons from the bundled Lucide font.
/// </summary>
public static class LucideTrayIcon
{
    private const string PersonalizeRegistryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";
    private const string SystemUsesLightThemeRegistryValue = "SystemUsesLightTheme";

    /// <summary>
    /// Assigns a Lucide glyph to a tray icon and keeps its color in sync with the
    /// Windows taskbar theme. Dispose the returned binding before the tray icon.
    /// </summary>
    public static IDisposable Bind(TrayIcon trayIcon, string glyph)
    {
        ArgumentNullException.ThrowIfNull(trayIcon);

        if (string.IsNullOrEmpty(glyph))
        {
            throw new ArgumentException("A Lucide glyph is required.", nameof(glyph));
        }

        return new ThemeBinding(trayIcon, glyph);
    }

    /// <summary>
    /// Creates a small bitmap using the color appropriate for the current
    /// Windows taskbar theme.
    /// </summary>
    public static SKBitmap CreateImage(string glyph)
    {
        return CreateImage(glyph, GetThemeIconColor());
    }

    /// <summary>
    /// Creates a small bitmap using a specific glyph color.
    /// </summary>
    public static SKBitmap CreateImage(string glyph, System.Drawing.Color color)
    {
        if (string.IsNullOrEmpty(glyph))
        {
            throw new ArgumentException("A Lucide glyph is required.", nameof(glyph));
        }

        SKColor skColor = new(color.R, color.G, color.B, color.A);
        int size = OperatingSystem.IsWindows() ? Math.Max(NativeMethods.GetSystemMetrics(SystemMetric.SM_CXSMICON),
            NativeMethods.GetSystemMetrics(SystemMetric.SM_CYSMICON)) : 16;
        byte[] imageData = RenderGlyph(glyph, skColor, size);

        using MemoryStream stream = new(imageData, writable: false);
        return SkiaImageHelpers.Decode(stream);
    }

    public static byte[] CreateIconBytes(string glyph) => CreateIconBytes(glyph, GetThemeIconColor());

    public static byte[] CreateIconBytes(string glyph, System.Drawing.Color color)
        => CreateIconData(glyph, color.ToSKColor());

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

    private static byte[] CreateIconData(string glyph, SKColor color) => TrayIconRenderer.RenderGlyphIco(glyph, color);

    private static byte[] RenderGlyph(string glyph, SKColor color, int size) => TrayIconRenderer.RenderGlyphPng(glyph, color, size);

    private sealed class ThemeBinding : IDisposable
    {
        private readonly TrayIcon _trayIcon;
        private readonly string _glyph;
        private readonly SynchronizationContext? _synchronizationContext;
        private bool _disposed;

        public ThemeBinding(TrayIcon trayIcon, string glyph)
        {
            _trayIcon = trayIcon;
            _glyph = glyph;
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

            using MemoryStream stream = new(CreateIconBytes(_glyph), writable: false);
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
