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

using ShareX.AvaloniaUI.Windows;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Theming;
using ShareX.Platform;
using ShareX.Tools.Controls;
using ShareX.Tools.Ruler;

namespace ShareX.Tools;

public partial class RulerWindow : Window
{
    private RulerOverlayControl _overlay = null!;
    private PixelRect? _captureBounds;
    private bool _closed;

    public RulerWindow()
    {
        AvaloniaXamlLoader.Load(this);
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
        _overlay = this.FindControl<RulerOverlayControl>("RulerOverlay")!;

        KeyDown += OnKeyDown;
        AddHandler(PointerReleasedEvent, OnWindowPointerReleased);
        Opened += (_, _) =>
        {
            if (_closed) return;
            ApplyCaptureBounds();
            Activate();
            _overlay.Focus();
        };
        ScalingChanged += (_, _) => Dispatcher.UIThread.Post(ApplyCaptureBounds, DispatcherPriority.Loaded);
        Closed += (_, _) => _closed = true;
    }

    private void OnWindowPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton == MouseButton.Right)
        {
            Close();
            e.Handled = true;
        }
    }

    /// <summary>Captures before showing the overlay, including when the platform needs an asynchronous permission prompt.</summary>
    public async Task ShowRulerAsync(CancellationToken cancellationToken = default)
    {
        if (_closed) return;
        IReadOnlyList<Screen> screens = Screens.All;
        if (screens.Count == 0)
        {
            return;
        }

        int left = screens.Min(screen => screen.Bounds.X);
        int top = screens.Min(screen => screen.Bounds.Y);
        int right = screens.Max(screen => screen.Bounds.Right);
        int bottom = screens.Max(screen => screen.Bounds.Bottom);
        PixelRect bounds = new(left, top, right - left, bottom - top);
        ScreenPixelBuffer screenPixelBuffer = await ScreenPixelBuffer.CaptureAsync(
            PlatformServices.Current.ScreenCapture, bounds, cancellationToken);
        if (_closed) return;
        _overlay.SetScreenPixelBuffer(screenPixelBuffer);

        _captureBounds = screenPixelBuffer.Bounds;
        ApplyCaptureBounds();
        Show();
    }

    private void ApplyCaptureBounds()
    {
        if (_closed || _captureBounds is not PixelRect bounds) return;
        double scaling = WindowScaling.GetPositionScaling(RenderScaling);
        Position = bounds.Position;
        Width = bounds.Width / scaling;
        Height = bounds.Height / scaling;
        _overlay.InvalidateVisual();
    }

    private async Task CopyMeasurementAsync()
    {
        string? text = _overlay.MeasurementText;
        if (!string.IsNullOrEmpty(text) && Clipboard != null)
        {
            await Clipboard.SetTextAsync(text);
        }

        _overlay.Focus();
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        int amount = e.KeyModifiers.HasFlag(KeyModifiers.Shift) ? 10 : 1;

        switch (e.Key)
        {
            case Key.Left:
                _overlay.Nudge(-amount, 0);
                e.Handled = true;
                break;
            case Key.Right:
                _overlay.Nudge(amount, 0);
                e.Handled = true;
                break;
            case Key.Up:
                _overlay.Nudge(0, -amount);
                e.Handled = true;
                break;
            case Key.Down:
                _overlay.Nudge(0, amount);
                e.Handled = true;
                break;
            case Key.H:
                _overlay.SetHorizontal();
                e.Handled = true;
                break;
            case Key.V:
                _overlay.SetVertical();
                e.Handled = true;
                break;
            case Key.Space:
                _overlay.ToggleAxis();
                e.Handled = true;
                break;
            case Key.Delete:
                _overlay.Clear();
                e.Handled = true;
                break;
            case Key.C when e.KeyModifiers.HasFlag(KeyModifiers.Control):
                _ = CopyMeasurementAsync();
                e.Handled = true;
                break;
            case Key.Escape:
                Close();
                e.Handled = true;
                break;
        }
    }
}
