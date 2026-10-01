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
using System.Windows.Forms;

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
    public static Icon CreateIcon(string glyph)
    {
        return CreateIcon(glyph, GetThemeIconColor());
    }

    /// <summary>
    /// Creates a multi-resolution icon using a specific glyph color.
    /// </summary>
    public static Icon CreateIcon(string glyph, System.Drawing.Color color)
    {
        if (string.IsNullOrEmpty(glyph))
        {
            throw new ArgumentException("A Lucide glyph is required.", nameof(glyph));
        }

        SKColor skColor = new(color.R, color.G, color.B, color.A);
        byte[] iconData = CreateIconData(glyph, skColor);

        using MemoryStream stream = new(iconData, writable: false);
        using Icon icon = new(stream, SystemInformation.SmallIconSize);
        return (Icon)icon.Clone();
    }

    /// <summary>
    /// Creates a small bitmap using the color appropriate for the current
    /// Windows taskbar theme.
    /// </summary>
    public static Image CreateImage(string glyph)
    {
        return CreateImage(glyph, GetThemeIconColor());
    }

    /// <summary>
    /// Creates a small bitmap using a specific glyph color.
    /// </summary>
    public static Image CreateImage(string glyph, System.Drawing.Color color)
    {
        if (string.IsNullOrEmpty(glyph))
        {
            throw new ArgumentException("A Lucide glyph is required.", nameof(glyph));
        }

        SKColor skColor = new(color.R, color.G, color.B, color.A);
        int size = Math.Max(SystemInformation.SmallIconSize.Width, SystemInformation.SmallIconSize.Height);
        byte[] imageData = RenderGlyph(glyph, skColor, size);

        using MemoryStream stream = new(imageData, writable: false);
        using Image image = Image.FromStream(stream);
        return new Bitmap(image);
    }

    private static System.Drawing.Color GetThemeIconColor()
    {
        return IsLightTaskbarTheme() ?
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

            Icon replacement = CreateIcon(_glyph);
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
