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
using Avalonia.Media;
using System;
using System.Collections.Generic;

namespace ShareX.ScreenCaptureLib.Presentation.RegionCapture;

/// <summary>
/// Event-driven region chrome. The frozen screenshot and annotations are rendered by the
/// shared editor workspace; this control only redraws when hover or selection state changes.
/// </summary>
public sealed class RegionSelectionOverlay : Control
{
    public static readonly StyledProperty<IBrush> AccentBrushProperty =
        AvaloniaProperty.Register<RegionSelectionOverlay, IBrush>(nameof(AccentBrush), Brushes.DodgerBlue);

    private static readonly IPen AntDashPen = new Pen(
        new SolidColorBrush(Color.FromArgb(230, 0, 0, 0)),
        1,
        new DashStyle([5, 5], 0));
    private Rect _selectionRectangle;
    private Rect _hoverRectangle;
    private IReadOnlyList<Rect> _regions = [];
    private bool _showHoverWithSelection;
    private bool _showCenterCrosshair;
    private bool _showCursorCrosshair;
    private Point _cursorPosition;
    private byte _dimAlpha = 51;
    private IBrush _dimBrush = new SolidColorBrush(Color.FromArgb(51, 0, 0, 0));
    private bool _dimAllExceptSelection;
    private bool _isSelectionPending;

    static RegionSelectionOverlay()
    {
        AffectsRender<RegionSelectionOverlay>(AccentBrushProperty);
    }

    public IBrush AccentBrush
    {
        get => GetValue(AccentBrushProperty);
        set => SetValue(AccentBrushProperty, value);
    }

    public Rect SelectionRectangle
    {
        get => _selectionRectangle;
        set
        {
            if (_selectionRectangle != value)
            {
                _selectionRectangle = value;
                InvalidateVisual();
            }
        }
    }

    public Rect HoverRectangle
    {
        get => _hoverRectangle;
        set
        {
            if (_hoverRectangle != value)
            {
                _hoverRectangle = value;
                InvalidateVisual();
            }
        }
    }

    /// <summary>
    /// Regions that were already added in multi region mode. They stay undimmed and outlined next to the
    /// selection, which is the region currently being drawn or edited.
    /// </summary>
    public IReadOnlyList<Rect> Regions
    {
        get => _regions;
        set
        {
            _regions = value ?? [];
            InvalidateVisual();
        }
    }

    /// <summary>
    /// When enabled, a hovered window is outlined even while a selection exists, so another region can be
    /// picked from it. Otherwise the selection hides the hover.
    /// </summary>
    public bool ShowHoverWithSelection
    {
        get => _showHoverWithSelection;
        set
        {
            if (_showHoverWithSelection != value)
            {
                _showHoverWithSelection = value;
                InvalidateVisual();
            }
        }
    }

    public bool ShowCenterCrosshair
    {
        get => _showCenterCrosshair;
        set
        {
            if (_showCenterCrosshair != value)
            {
                _showCenterCrosshair = value;
                InvalidateVisual();
            }
        }
    }

    public bool ShowCursorCrosshair
    {
        get => _showCursorCrosshair;
        set
        {
            if (_showCursorCrosshair != value)
            {
                _showCursorCrosshair = value;
                InvalidateVisual();
            }
        }
    }

    public Point CursorPosition
    {
        get => _cursorPosition;
        set
        {
            if (_cursorPosition != value)
            {
                _cursorPosition = value;
                if (ShowCursorCrosshair)
                {
                    InvalidateVisual();
                }
            }
        }
    }

    public byte DimAlpha
    {
        get => _dimAlpha;
        set
        {
            if (_dimAlpha != value)
            {
                _dimAlpha = value;
                _dimBrush = new SolidColorBrush(Color.FromArgb(value, 0, 0, 0));
                InvalidateVisual();
            }
        }
    }

    /// <summary>
    /// When enabled, a hovered window or control is only outlined and stays dimmed, including while a
    /// press on it is pending; only a non-pending selection is shown undimmed, whether it is being
    /// dragged or has been released.
    /// </summary>
    public bool DimAllExceptSelection
    {
        get => _dimAllExceptSelection;
        set
        {
            if (_dimAllExceptSelection != value)
            {
                _dimAllExceptSelection = value;
                InvalidateVisual();
            }
        }
    }

    /// <summary>
    /// True while a press has picked the hovered window as the selection but the user has not yet
    /// released or started dragging a new region.
    /// </summary>
    public bool IsSelectionPending
    {
        get => _isSelectionPending;
        set
        {
            if (_isSelectionPending != value)
            {
                _isSelectionPending = value;
                InvalidateVisual();
            }
        }
    }

    public Rect ActiveRectangle => IsValid(SelectionRectangle) ? SelectionRectangle : HoverRectangle;

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        Rect surface = new Rect(Bounds.Size);
        Rect active = Intersect(ActiveRectangle, surface);

        // In multi region mode a window can be hovered while another region is selected.
        Rect extraHover = ShowHoverWithSelection && IsValid(SelectionRectangle) && HoverRectangle != SelectionRectangle
            ? Intersect(HoverRectangle, surface)
            : default;

        if (DimAlpha > 0)
        {
            Rect undimmed = DimAllExceptSelection && (IsSelectionPending || !IsValid(SelectionRectangle)) ? default : active;

            if (Regions.Count > 0 || IsValid(extraHover))
            {
                List<Rect> clear = new(Regions.Count + 2);
                foreach (Rect region in Regions)
                {
                    clear.Add(region);
                }

                clear.Add(undimmed);

                if (!DimAllExceptSelection)
                {
                    clear.Add(extraHover);
                }

                foreach (Rect rectangle in GetUncoveredRectangles(surface, clear))
                {
                    context.DrawRectangle(_dimBrush, null, rectangle);
                }
            }
            else if (IsValid(undimmed))
            {
                DrawDimmedOutside(context, surface, undimmed, _dimBrush);
            }
            else
            {
                context.DrawRectangle(_dimBrush, null, surface);
            }
        }

        foreach (Rect region in Regions)
        {
            Rect visible = Intersect(region, surface);
            if (IsValid(visible))
            {
                DrawAntRectangle(context, GetAntRectangle(visible));
            }
        }

        if (IsValid(extraHover))
        {
            DrawAntRectangle(context, GetAntRectangle(extraHover));
        }

        if (IsValid(active))
        {
            DrawAntRectangle(context, GetAntRectangle(active));

            if (ShowCenterCrosshair && IsValid(SelectionRectangle))
            {
                DrawCenterCrosshair(context, SelectionRectangle);
            }
        }

        if (ShowCursorCrosshair)
        {
            DrawCursorCrosshair(context, surface);
        }
    }

    private static Rect GetAntRectangle(Rect rectangle)
    {
        return new Rect(
            rectangle.X + 0.5,
            rectangle.Y + 0.5,
            Math.Max(0, rectangle.Width - 1),
            Math.Max(0, rectangle.Height - 1));
    }

    private void DrawAntRectangle(DrawingContext context, Rect rectangle)
    {
        context.DrawRectangle(null, new Pen(AccentBrush, 1), rectangle);
        context.DrawLine(AntDashPen, rectangle.TopLeft, rectangle.TopRight);
        context.DrawLine(AntDashPen, rectangle.TopLeft, rectangle.BottomLeft);
        context.DrawLine(AntDashPen, rectangle.TopRight, rectangle.BottomRight);
        context.DrawLine(AntDashPen, rectangle.BottomLeft, rectangle.BottomRight);
    }

    private void DrawCursorCrosshair(DrawingContext context, Rect surface)
    {
        const double cursorGap = 5;
        double x = Math.Clamp(Math.Floor(CursorPosition.X), surface.Left, Math.Max(surface.Left, surface.Right - 1)) + 0.5;
        double y = Math.Clamp(Math.Floor(CursorPosition.Y), surface.Top, Math.Max(surface.Top, surface.Bottom - 1)) + 0.5;

        DrawAntLineIfVisible(context, new Point(surface.Left, y), new Point(x - cursorGap, y));
        DrawAntLineIfVisible(context, new Point(x + cursorGap, y), new Point(surface.Right, y));
        DrawAntLineIfVisible(context, new Point(x, surface.Top), new Point(x, y - cursorGap));
        DrawAntLineIfVisible(context, new Point(x, y + cursorGap), new Point(x, surface.Bottom));
    }

    private void DrawAntLineIfVisible(DrawingContext context, Point start, Point end)
    {
        if (end.X <= start.X && end.Y <= start.Y)
        {
            return;
        }

        context.DrawLine(new Pen(AccentBrush, 1), start, end);
        context.DrawLine(AntDashPen, start, end);
    }

    private void DrawCenterCrosshair(DrawingContext context, Rect rectangle)
    {
        Point center = rectangle.Center;
        int centerX = (int)Math.Floor(center.X);
        int centerY = (int)Math.Floor(center.Y);
        DrawPixelCross(context, Brushes.Black, centerX - 1, centerY - 1);
        DrawPixelCross(context, AccentBrush, centerX, centerY);
    }

    private static void DrawPixelCross(DrawingContext context, IBrush brush, int centerX, int centerY)
    {
        const int radius = 10;
        const int diameter = radius * 2 + 1;
        context.DrawRectangle(brush, null, new Rect(centerX - radius, centerY, diameter, 1));
        context.DrawRectangle(brush, null, new Rect(centerX, centerY - radius, 1, diameter));
    }

    private static void DrawDimmedOutside(DrawingContext context, Rect surface, Rect clear, IBrush brush)
    {
        DrawIfValid(context, brush, new Rect(surface.Left, surface.Top, surface.Width, clear.Top - surface.Top));
        DrawIfValid(context, brush, new Rect(surface.Left, clear.Top, clear.Left - surface.Left, clear.Height));
        DrawIfValid(context, brush, new Rect(clear.Right, clear.Top, surface.Right - clear.Right, clear.Height));
        DrawIfValid(context, brush, new Rect(surface.Left, clear.Bottom, surface.Width, surface.Bottom - clear.Bottom));
    }

    /// <summary>
    /// Splits the part of <paramref name="surface"/> that none of <paramref name="covered"/> overlap into
    /// rectangles, so overlapping regions are never dimmed twice or left with dimmed seams.
    /// </summary>
    internal static List<Rect> GetUncoveredRectangles(Rect surface, IReadOnlyList<Rect> covered)
    {
        List<Rect> result = [];
        List<Rect> regions = [];
        SortedSet<double> columns = [surface.Left, surface.Right];

        foreach (Rect rectangle in covered)
        {
            Rect clipped = Intersect(rectangle, surface);
            if (IsValid(clipped))
            {
                regions.Add(clipped);
                columns.Add(clipped.Left);
                columns.Add(clipped.Right);
            }
        }

        if (regions.Count == 0)
        {
            result.Add(surface);
            return result;
        }

        double[] edges = [.. columns];
        List<(double Top, double Bottom)> spans = [];

        for (int i = 0; i < edges.Length - 1; i++)
        {
            double left = edges[i];
            double right = edges[i + 1];
            if (right <= left)
            {
                continue;
            }

            spans.Clear();
            foreach (Rect region in regions)
            {
                if (region.Left <= left && region.Right >= right)
                {
                    spans.Add((region.Top, region.Bottom));
                }
            }

            spans.Sort((a, b) => a.Top.CompareTo(b.Top));

            double y = surface.Top;
            foreach ((double top, double bottom) in spans)
            {
                if (top > y)
                {
                    result.Add(new Rect(left, y, right - left, top - y));
                }

                y = Math.Max(y, bottom);
            }

            if (surface.Bottom > y)
            {
                result.Add(new Rect(left, y, right - left, surface.Bottom - y));
            }
        }

        return result;
    }

    private static void DrawIfValid(DrawingContext context, IBrush brush, Rect rectangle)
    {
        if (IsValid(rectangle))
        {
            context.DrawRectangle(brush, null, rectangle);
        }
    }

    /// <summary>
    /// Creates the rectangle spanning two pointer positions. Each position addresses a whole pixel and
    /// both of those pixels are part of the result, so the first to the last pixel of a 1920x1080
    /// surface is 1920x1080 rather than 1919x1079.
    /// </summary>
    internal static Rect CreatePixelRectangle(Point first, Point second, Size bounds)
    {
        double firstX = GetPixel(first.X, bounds.Width);
        double firstY = GetPixel(first.Y, bounds.Height);
        double secondX = GetPixel(second.X, bounds.Width);
        double secondY = GetPixel(second.Y, bounds.Height);
        return new Rect(
            Math.Min(firstX, secondX),
            Math.Min(firstY, secondY),
            Math.Abs(secondX - firstX) + 1,
            Math.Abs(secondY - firstY) + 1);
    }

    // Positions are whole physical pixels but pick up floating point noise from the DPI transform,
    // so round them instead of flooring.
    private static double GetPixel(double position, double size)
    {
        return Math.Clamp(Math.Round(position), 0, Math.Max(0, size - 1));
    }

    internal static Rect Intersect(Rect rectangle, Rect bounds)
    {
        double left = Math.Max(rectangle.Left, bounds.Left);
        double top = Math.Max(rectangle.Top, bounds.Top);
        double right = Math.Min(rectangle.Right, bounds.Right);
        double bottom = Math.Min(rectangle.Bottom, bounds.Bottom);
        return right > left && bottom > top
            ? new Rect(left, top, right - left, bottom - top)
            : default;
    }

    internal static bool IsValid(Rect rectangle) => rectangle.Width > 0 && rectangle.Height > 0;
}
