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
using ShareX.HelpersLib;
using System;
using System.Runtime.InteropServices;
using DrawingRectangle = System.Drawing.Rectangle;

namespace ShareX.ScreenCaptureLib;

/// <summary>
/// Dashed one-pixel outline around a screen region, drawn by four opaque edge windows.
/// No window covers the region itself, so the outline does not rely on per-pixel window
/// transparency or window regions. Some systems ignore both, which turned a single
/// full-size overlay window into a solid rectangle over the area being recorded.
/// </summary>
public sealed class RegionFrame
{
    public const int BorderPixels = 1;

    private readonly EdgeWindow[] _edges;
    private DrawingRectangle _region;
    private IBrush _accentBrush;
    private bool _closed;

    public RegionFrame(DrawingRectangle region, IBrush accentBrush)
    {
        _region = region;
        _accentBrush = accentBrush;
        _edges =
        [
            new EdgeWindow(this, horizontal: true),
            new EdgeWindow(this, horizontal: true),
            new EdgeWindow(this, horizontal: false),
            new EdgeWindow(this, horizontal: false)
        ];
        UpdateBounds();
    }

    public string? Title
    {
        get => _edges[0].Title;
        set
        {
            foreach (EdgeWindow edge in _edges) edge.Title = value;
        }
    }

    public IBrush AccentBrush
    {
        get => _accentBrush;
        set
        {
            _accentBrush = value;
            foreach (EdgeWindow edge in _edges) edge.InvalidateDashes();
        }
    }

    /// <summary>Must be set before <see cref="Show"/>.</summary>
    public bool ExcludeFromCapture { get; set; }

    public void Show()
    {
        if (_closed) return;

        foreach (EdgeWindow edge in _edges)
        {
            if (!edge.IsVisible) edge.Show();
            edge.ApplyBounds();
        }
    }

    public void Hide()
    {
        if (_closed) return;

        foreach (EdgeWindow edge in _edges) edge.Hide();
    }

    public void Close()
    {
        if (_closed) return;

        _closed = true;
        foreach (EdgeWindow edge in _edges) edge.Close();
    }

    public void SetRegion(DrawingRectangle region)
    {
        if (_region == region) return;

        _region = region;
        UpdateBounds();
    }

    /// <summary>Places the outline directly below <paramref name="window"/> in z-order.</summary>
    public void PlaceBehind(IntPtr window)
    {
        if (window == IntPtr.Zero) return;

        foreach (EdgeWindow edge in _edges)
        {
            IntPtr handle = edge.Handle;
            if (handle != IntPtr.Zero)
            {
                NativeMethods.SetWindowPos(handle, window, 0, 0, 0, 0,
                    SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOACTIVATE);
            }
        }
    }

    private void UpdateBounds()
    {
        int left = _region.X - BorderPixels;
        int top = _region.Y - BorderPixels;
        int width = _region.Width + BorderPixels * 2;

        // Top and bottom span the corners; left and right fill the height between them.
        _edges[0].SetBounds(new PixelRect(left, top, width, BorderPixels), 0);
        _edges[1].SetBounds(new PixelRect(left, _region.Bottom, width, BorderPixels), 0);
        _edges[2].SetBounds(new PixelRect(left, _region.Y, BorderPixels, Math.Max(1, _region.Height)), BorderPixels);
        _edges[3].SetBounds(new PixelRect(_region.Right, _region.Y, BorderPixels, Math.Max(1, _region.Height)), BorderPixels);
    }

    private sealed class EdgeWindow : Window
    {
        private const int WDA_EXCLUDEFROMCAPTURE = 0x11;
        private const uint WS_POPUP = 0x80000000;

        private readonly RegionFrame _owner;
        private readonly EdgeDashes _dashes;
        private PixelRect _bounds;

        public EdgeWindow(RegionFrame owner, bool horizontal)
        {
            _owner = owner;
            _dashes = new EdgeDashes(owner, horizontal);

            WindowDecorations = WindowDecorations.None;
            CanResize = false;
            ShowInTaskbar = false;
            ShowActivated = false;
            Topmost = true;
            Focusable = false;
            MinWidth = 0;
            MinHeight = 0;
            Width = 1;
            Height = 1;
            Background = EdgeDashes.BackdropBrush;
            Content = _dashes;

            if (OperatingSystem.IsWindows())
            {
                // Windows keeps overlapped windows at least a caption tall (SM_CYMIN), which would make
                // the top and bottom edges dozens of pixels thick. Popup windows can be one pixel.
                Win32Properties.AddWindowStylesCallback(this, (style, exStyle) => (style | WS_POPUP, exStyle));
            }

            Opened += (_, _) =>
            {
                ApplyWindowStyles();
                ApplyBounds();
            };
        }

        public IntPtr Handle => TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;

        public void InvalidateDashes() => _dashes.InvalidateVisual();

        public void SetBounds(PixelRect bounds, int phasePixels)
        {
            _bounds = bounds;
            _dashes.PhasePixels = phasePixels;

            if (IsVisible)
            {
                ApplyBounds();
            }
            else
            {
                // Opens near the right place; Opened then sets the exact pixel bounds.
                Position = bounds.Position;
            }
        }

        public void ApplyBounds()
        {
            IntPtr handle = Handle;
            if (handle == IntPtr.Zero || _bounds.Width <= 0 || _bounds.Height <= 0) return;

            // Moving onto a monitor with a different DPI makes Avalonia resize the window to the
            // size Windows suggests, so place it again once it is on the new monitor.
            for (int attempt = 0; attempt < 3; attempt++)
            {
                NativeMethods.SetWindowPos(handle, IntPtr.Zero, _bounds.X, _bounds.Y, _bounds.Width, _bounds.Height,
                    SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_NOACTIVATE);

                System.Drawing.Rectangle actual = NativeMethods.GetWindowRect(handle);
                if (actual.X == _bounds.X && actual.Y == _bounds.Y && actual.Width == _bounds.Width && actual.Height == _bounds.Height)
                {
                    break;
                }
            }

            _dashes.InvalidateVisual();
        }

        private void ApplyWindowStyles()
        {
            IntPtr handle = Handle;
            if (handle == IntPtr.Zero) return;

            WindowInfo info = new(handle);
            info.ExStyle |= WindowStyles.WS_EX_TOOLWINDOW | WindowStyles.WS_EX_TRANSPARENT | WindowStyles.WS_EX_NOACTIVATE;

            if (_owner.ExcludeFromCapture && !NativeMethods.SetWindowDisplayAffinity(handle, WDA_EXCLUDEFROMCAPTURE))
            {
                DebugHelper.WriteLine("Could not exclude the region outline from capture. Win32 error: {0}", Marshal.GetLastWin32Error());
            }
        }
    }

    private sealed class EdgeDashes : Control
    {
        public static readonly IBrush BackdropBrush = new SolidColorBrush(Color.FromRgb(20, 20, 20));

        private const double DashLength = 5;

        private readonly RegionFrame _owner;
        private readonly bool _horizontal;

        public EdgeDashes(RegionFrame owner, bool horizontal)
        {
            _owner = owner;
            _horizontal = horizontal;
            IsHitTestVisible = false;
        }

        /// <summary>Offset of this edge's first pixel along the frame, so dashes line up at the corners.</summary>
        public int PhasePixels { get; set; }

        public override void Render(DrawingContext context)
        {
            Rect bounds = new(Bounds.Size);
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            context.FillRectangle(BackdropBrush, bounds);

            double scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1;
            double start = PhasePixels / scaling;
            double length = _horizontal ? bounds.Width : bounds.Height;
            double period = DashLength * 2;

            // Backdrop and accent dashes alternate every DashLength, with the accent in the second half of each period.
            double first = Math.Floor(start / period) * period + DashLength - start;
            for (double from = first; from < length; from += period)
            {
                double a = Math.Max(0, from);
                double b = Math.Min(length, from + DashLength);
                if (b <= a) continue;

                Rect dash = _horizontal ? new Rect(a, 0, b - a, bounds.Height) : new Rect(0, a, bounds.Width, b - a);
                context.FillRectangle(_owner.AccentBrush, dash);
            }
        }
    }
}
