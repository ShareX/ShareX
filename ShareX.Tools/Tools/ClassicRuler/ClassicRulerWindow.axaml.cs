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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.ScreenCaptureLib;
using ShareX.Tools.Controls;
using SkiaSharp;
using DrawingPoint = System.Drawing.Point;
using DrawingRectangle = System.Drawing.Rectangle;

namespace ShareX.Tools;

/// <summary>
/// Ruler that works like the ShareX 21 one: it freezes the screen and measures a freely drawn rectangle.
/// </summary>
public partial class ClassicRulerWindow : Window
{
    // Same as region capture: ignore keys briefly so the key-up of the hotkey that opened the ruler is not handled.
    private const int InputDelay = 500;

    private readonly ClassicRulerOverlayControl _overlay;
    private readonly LayoutTransformControl _pixelTransform;
    private readonly Image _screenshotImage;
    private readonly ClassicRulerOptions _options;
    private PixelRect _screenBounds;
    private Bitmap? _screenshot;
    private bool _keyboardInputEnabled;
    private bool _closed;

    public ClassicRulerWindow() : this(new ClassicRulerOptions())
    {
    }

    public ClassicRulerWindow(ClassicRulerOptions options)
    {
        AvaloniaXamlLoader.Load(this);
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
#if !DEBUG
        Topmost = true;
#endif
        _options = options;
        _overlay = this.FindControl<ClassicRulerOverlayControl>("RulerOverlay")!;
        _pixelTransform = this.FindControl<LayoutTransformControl>("PixelTransform")!;
        _screenshotImage = this.FindControl<Image>("ScreenshotImage")!;
        _overlay.CloseRequested += (_, _) => Close();

        CaptureScreen();

        Opened += OnOpened;
    }

    private void CaptureScreen()
    {
        IReadOnlyList<Screen> screens = Screens.All;
        if (screens.Count == 0)
        {
            return;
        }

        int left = screens.Min(screen => screen.Bounds.X);
        int top = screens.Min(screen => screen.Bounds.Y);
        int right = screens.Max(screen => screen.Bounds.Right);
        int bottom = screens.Max(screen => screen.Bounds.Bottom);
        _screenBounds = new PixelRect(left, top, right - left, bottom - top);

        using (SKBitmap? capture = OperatingSystem.IsWindows()
            ? WindowsImageInterop.Capture(new DrawingRectangle(left, top, _screenBounds.Width, _screenBounds.Height))
            : LinuxCaptureHelper.CaptureRectangle(new DrawingRectangle(left, top, _screenBounds.Width, _screenBounds.Height)))
        {
            if (capture != null)
            {
                _screenshot = BitmapConversionHelpers.ToAvaloniBitmap(capture);
            }
        }

        _screenshotImage.Source = _screenshot;
        _screenshotImage.Width = _screenBounds.Width;
        _screenshotImage.Height = _screenBounds.Height;

        PixelRect[] monitors = screens
            .Select(screen => new PixelRect(screen.Bounds.X - left, screen.Bounds.Y - top, screen.Bounds.Width, screen.Bounds.Height))
            .ToArray();
        _overlay.Initialize(_screenshot, _screenBounds.Position, monitors, _options);

        // Set the position once while the window is still small, so Windows picks the DPI of the monitor at
        // the desktop origin. Moving it again after it spans several monitors can change Avalonia's scale.
        double scaling = Screens.ScreenFromPoint(_screenBounds.Position)?.Scaling ?? screens[0].Scaling;
        Position = _screenBounds.Position;
        ApplyPixelSize(scaling);
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        if (_screenshot == null)
        {
            Close();
            return;
        }

        double scaling = GetRenderScaling();
        ApplyPixelSize(scaling);
        Helpers.EnsureWindowPosition(this, _screenBounds.X, _screenBounds.Y);
        _pixelTransform.LayoutTransform = new ScaleTransform(1 / scaling, 1 / scaling);
        _overlay.HudScale = scaling;

        Activate();
        _overlay.Focus();

        DrawingPoint cursor = CaptureHelpers.GetCursorPosition();
        _overlay.SetPointer(new PixelPoint(cursor.X - _screenBounds.X, cursor.Y - _screenBounds.Y));

        _ = LoadWindowRectanglesAsync();

        await Task.Delay(InputDelay);
        _keyboardInputEnabled = !_closed;
    }

    private async Task LoadWindowRectanglesAsync()
    {
        if (_options.GetWindowRectangles is not { } getWindowRectangles)
        {
            return;
        }

        try
        {
            IntPtr handle = TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
            IReadOnlyList<DrawingRectangle> windows = await Task.Run(() => getWindowRectangles(handle));

            if (!_closed)
            {
                _overlay.SetWindowRectangles(windows);
            }
        }
        catch (Exception ex)
        {
            DebugHelper.WriteException(ex);
        }
    }

    private void ApplyPixelSize(double scaling)
    {
        Width = _screenBounds.Width / scaling;
        Height = _screenBounds.Height / scaling;
    }

    private double GetRenderScaling()
    {
        double scaling = RenderScaling;
        return double.IsFinite(scaling) && scaling > 0 ? scaling : 1;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Handled || !_keyboardInputEnabled)
        {
            return;
        }

        if (e.Key == Key.C && e.KeyModifiers == KeyModifiers.Control)
        {
            _ = CopyMeasurementAsync();
            e.Handled = true;
        }
        else
        {
            e.Handled = _overlay.HandleKeyDown(e.Key, e.KeyModifiers);
        }
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);

        // Close on key up like region capture, so the release does not reach the window underneath.
        if (!e.Handled && e.Key == Key.Escape && e.KeyModifiers == KeyModifiers.None)
        {
            Close();
            e.Handled = true;
        }
    }

    private async Task CopyMeasurementAsync()
    {
        string text = _overlay.GetMeasurementText();
        if (!string.IsNullOrEmpty(text) && Clipboard != null)
        {
            await Clipboard.SetTextAsync(text);
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        _closed = true;
        base.OnClosed(e);
        _screenshotImage.Source = null;
        _screenshot?.Dispose();
        _screenshot = null;
    }
}
