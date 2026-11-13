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
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Input;
using ShareX.HelpersLib;
using ShareX.Platform;
using System;
using System.Threading.Tasks;
using DrawingPoint = System.Drawing.Point;
using DrawingRectangle = System.Drawing.Rectangle;

namespace ShareX.ScreenCaptureLib.Presentation.RegionCapture;

/// <summary>
/// Selects a rectangle over the live desktop instead of a frozen screenshot, so the screen keeps updating
/// while the region is drawn. The caller captures the screen after the window closes.
/// </summary>
public sealed class RegionCaptureTransparentWindow : Window
{
    private const int MinimumRectangleSize = 5;

    private readonly TaskCompletionSource<DrawingRectangle?> _completionSource =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly DrawingRectangle _screenBounds;
    private readonly SelectionSurface _surface;
    private DrawingPoint _startPoint;
    private bool _isSelecting;
    private bool _suppressRightRelease;
    private bool _closing;
    private DrawingRectangle? _result;

    public RegionCaptureTransparentWindow(DrawingRectangle screenBounds, bool lockCursorToWindow)
    {
        _screenBounds = screenBounds;

        Title = "ShareX - Region capture (Transparent)";
        WindowDecorations = WindowDecorations.None;
        CanResize = false;
        ShowInTaskbar = false;
#if !DEBUG
        Topmost = true;
#endif
        TransparencyLevelHint = [WindowTransparencyLevel.Transparent];
        // Fully transparent pixels let clicks through to the windows below, so keep an invisible alpha of 1.
        Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));

        _surface = new SelectionSurface { IsHitTestVisible = false };
        Content = _surface;

        if (lockCursorToWindow)
        {
            Helpers.LockCursorToWindow(this);
        }

        Opened += OnOpened;
    }

    /// <summary>Shows the window and returns the selected screen rectangle, or null when cancelled.</summary>
    public Task<DrawingRectangle?> SelectAsync()
    {
        // Set Position once while the window is still small, on the monitor the system takes the spanning window's DPI from.
        // Keep that DPI while the window spans monitors with different scaling, as region capture does.
        RegionCaptureDpi.PositionAtAnchor(this, new PlatformRectangle(_screenBounds.X, _screenBounds.Y, _screenBounds.Width, _screenBounds.Height));
        ApplyPixelSize();
        Helpers.EnsureWindowPosition(this, _screenBounds.X, _screenBounds.Y);
        Show();
        return _completionSource.Task;
    }

    protected override void OnClosed(EventArgs e)
    {
        _completionSource.TrySetResult(_result);
        base.OnClosed(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Key == Key.Escape)
        {
            e.Handled = true;
            Complete(null);
        }
    }

    // A button pressed while another is held arrives as a pointer move, so every handler goes by which
    // button changed rather than by the kind of event.
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        HandlePointerUpdate(e);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (!HandlePointerUpdate(e) && _isSelecting && !_closing)
        {
            UpdateSelection(ToScreenPixel(e.GetPosition(this)));
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        HandlePointerUpdate(e);
    }

    /// <summary>Handles a button press or release. Returns false when no button changed.</summary>
    private bool HandlePointerUpdate(PointerEventArgs e)
    {
        if (_closing)
        {
            return false;
        }

        PointerPoint point = e.GetCurrentPoint(this);

        switch (point.Properties.PointerUpdateKind)
        {
            case PointerUpdateKind.LeftButtonPressed:
                _startPoint = ToScreenPixel(point.Position);
                _isSelecting = true;
                _suppressRightRelease = false;
                e.Pointer.Capture(this);
                UpdateSelection(_startPoint);
                break;
            case PointerUpdateKind.LeftButtonReleased:
                e.Pointer.Capture(null);

                if (_isSelecting)
                {
                    DrawingRectangle selection = GetSelectionRectangle(ToScreenPixel(point.Position));

                    if (IsValidSelection(selection))
                    {
                        Complete(selection);
                        break;
                    }
                }

                ResetSelection();
                break;
            case PointerUpdateKind.RightButtonPressed:
                // Right click while the left button is held cancels the selection being drawn; releasing the
                // right button afterwards must not also cancel the capture.
                if (_isSelecting)
                {
                    e.Pointer.Capture(null);
                    ResetSelection();
                    _suppressRightRelease = true;
                }
                break;
            case PointerUpdateKind.RightButtonReleased:
                if (_suppressRightRelease)
                {
                    _suppressRightRelease = false;
                }
                else if (_isSelecting)
                {
                    e.Pointer.Capture(null);
                    ResetSelection();
                }
                else
                {
                    Complete(null);
                }
                break;
            default:
                return false;
        }

        e.Handled = true;
        return true;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        ApplyToolWindowStyle();
        ApplyPixelSize();
        Helpers.EnsureWindowPosition(this, _screenBounds.X, _screenBounds.Y);
        WindowGeometryLog.Record(this, "Region selection (transparent)", _screenBounds, trackDpiChanges: true);
        DisableWindowTransitions();
        Cursor = CursorAssetLoader.GetCrosshairCursor(GetScaling());

        Activate();
        Focus();
    }

    private void UpdateSelection(DrawingPoint position)
    {
        DrawingRectangle selection = GetSelectionRectangle(position);
        double scaling = GetScaling();

        _surface.Scaling = scaling;
        _surface.Selection = IsValidSelection(selection)
            ? new Rect(
                (selection.X - _screenBounds.X) / scaling,
                (selection.Y - _screenBounds.Y) / scaling,
                selection.Width / scaling,
                selection.Height / scaling)
            : null;
    }

    private void ResetSelection()
    {
        _isSelecting = false;
        _surface.Selection = null;
    }

    private DrawingRectangle GetSelectionRectangle(DrawingPoint position)
    {
        return CaptureHelpers.CreateRectangle(_startPoint, position);
    }

    /// <summary>Converts a window position to the screen pixel under it, clamped to the screen bounds.</summary>
    private DrawingPoint ToScreenPixel(Point position)
    {
        double scaling = GetScaling();
        int x = _screenBounds.X + (int)Math.Floor(position.X * scaling + 0.001);
        int y = _screenBounds.Y + (int)Math.Floor(position.Y * scaling + 0.001);
        return new DrawingPoint(
            Math.Clamp(x, _screenBounds.Left, _screenBounds.Right - 1),
            Math.Clamp(y, _screenBounds.Top, _screenBounds.Bottom - 1));
    }

    private static bool IsValidSelection(DrawingRectangle selection)
    {
        return selection.Width > MinimumRectangleSize && selection.Height > MinimumRectangleSize;
    }

    private void Complete(DrawingRectangle? result)
    {
        if (_closing)
        {
            return;
        }

        _closing = true;
        _result = result;
        _isSelecting = false;
        _surface.Selection = null;

        // Let the cleared overlay render before the window goes away, so no border is left on screen.
        Dispatcher.UIThread.Post(Close, DispatcherPriority.Background);
    }

    private void ApplyPixelSize()
    {
        double scaling = GetScaling();
        Width = _screenBounds.Width / scaling;
        Height = _screenBounds.Height / scaling;
    }

    private double GetScaling()
    {
        return double.IsFinite(RenderScaling) && RenderScaling > 0 ? RenderScaling : 1;
    }

    /// <summary>
    /// Chrome treats a window covering it as occluding unless it is a tool window, and stops showing video while
    /// occluded, so without this a YouTube video loaded under the selection does not appear.
    /// </summary>
    private void ApplyToolWindowStyle()
    {
        IntPtr handle = TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;

        if (OperatingSystem.IsWindows() && handle != IntPtr.Zero && PlatformServices.IsInitialized)
        {
            // Adds WS_EX_TOOLWINDOW on Windows.
            PlatformServices.Current.Windows.SetOverlayStyle(handle, clickThrough: false);
        }
    }

    private void DisableWindowTransitions()
    {
        IntPtr handle = TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;

        if (handle != IntPtr.Zero && PlatformServices.IsInitialized)
        {
            // A fade out animation would still be on screen when the capture is taken.
            PlatformServices.Current.Windows.DisableTransitions(handle);
        }
    }

    /// <summary>Draws the selection as a white line with black dashes, one physical pixel wide.</summary>
    private sealed class SelectionSurface : Control
    {
        private static readonly IPen WhitePen = new ImmutablePen(Brushes.White, 1);
        private static readonly IPen BlackDashPen = new ImmutablePen(Brushes.Black, 1, new ImmutableDashStyle([5, 5], 0));

        private Rect? _selection;
        private double _scaling = 1;

        public Rect? Selection
        {
            get => _selection;
            set
            {
                if (_selection != value)
                {
                    _selection = value;
                    InvalidateVisual();
                }
            }
        }

        public double Scaling
        {
            get => _scaling;
            set => _scaling = value > 0 ? value : 1;
        }

        public override void Render(DrawingContext context)
        {
            if (_selection is not Rect selection)
            {
                return;
            }

            // Draw through the centers of the edge pixels, in physical pixel units.
            double pixel = 1 / _scaling;
            Rect outline = selection.Deflate(pixel / 2);

            using (context.PushTransform(Matrix.CreateScale(pixel, pixel)))
            {
                Rect pixelOutline = new Rect(outline.X * _scaling, outline.Y * _scaling,
                    outline.Width * _scaling, outline.Height * _scaling);
                context.DrawRectangle(null, WhitePen, pixelOutline);
                context.DrawRectangle(null, BlackDashPen, pixelOutline);
            }
        }
    }
}
