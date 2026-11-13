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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DrawingPoint = System.Drawing.Point;
using DrawingRectangle = System.Drawing.Rectangle;

namespace ShareX;

/// <summary>
/// Draws the monitors where Windows places them, like the arrangement in Windows display settings: the primary monitor in
/// the accent colour, space the taskbar or app bars reserve shaded, and each monitor's resolution and scale.
/// </summary>
internal sealed class MonitorLayoutView : Control
{
    private const double LayoutPadding = 8;
    private const double MonitorGap = 3;
    private const double CornerRadius = 6;
    private const double CursorRadius = 4;
    private const double MinimumStrip = 4;

    private IReadOnlyList<DisplayDiagnosticsMonitor> _monitors = [];
    private DrawingPoint? _cursorPosition;

    public void SetMonitors(IReadOnlyList<DisplayDiagnosticsMonitor> monitors, DrawingPoint? cursorPosition)
    {
        _monitors = monitors;
        _cursorPosition = cursorPosition;
        InvalidateVisual();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ThemeVariantScope.ActualThemeVariantProperty)
        {
            InvalidateVisual();
        }
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (_monitors.Count == 0 || Bounds.Width <= 2 * LayoutPadding || Bounds.Height <= 2 * LayoutPadding)
        {
            return;
        }

        DrawingRectangle desktop = _monitors.Select(x => x.Bounds).Aggregate(DrawingRectangle.Union);
        if (desktop.Width <= 0 || desktop.Height <= 0)
        {
            return;
        }

        double scale = Math.Min((Bounds.Width - 2 * LayoutPadding) / desktop.Width, (Bounds.Height - 2 * LayoutPadding) / desktop.Height);
        double offsetX = (Bounds.Width - desktop.Width * scale) / 2;
        double offsetY = (Bounds.Height - desktop.Height * scale) / 2;

        Rect ToView(DrawingRectangle rectangle) => new(
            offsetX + (rectangle.X - desktop.X) * scale,
            offsetY + (rectangle.Y - desktop.Y) * scale,
            rectangle.Width * scale,
            rectangle.Height * scale);

        // The theme's panel and control colours are close together, so monitors are drawn in the hover shade and their
        // outlines and reserved strips are tints of the text colour, which stays readable in light and dark themes.
        IBrush background = FindBrush("ShareX.Brush.Control.Background.Hover") ?? Brushes.DimGray;
        IBrush panel = FindBrush("ShareX.Brush.Background.Panel") ?? Brushes.Black;
        IBrush accent = FindBrush("ShareX.Brush.Accent") ?? Brushes.DodgerBlue;
        IBrush text = FindBrush("ShareX.Brush.Text") ?? Brushes.White;
        IBrush border = WithOpacity(text, 0.28);
        IBrush reserved = WithOpacity(text, 0.3);
        IBrush secondaryText = FindBrush("ShareX.Brush.Text.Secondary") ?? Brushes.LightGray;
        FontFamily fontFamily = FindResource<FontFamily>("ShareX.FontFamily.Default") ?? FontFamily.Default;

        foreach (DisplayDiagnosticsMonitor monitor in _monitors)
        {
            Rect area = ToView(monitor.Bounds).Deflate(MonitorGap / 2);
            if (area.Width <= 0 || area.Height <= 0)
            {
                continue;
            }

            double radius = Math.Min(CornerRadius, Math.Min(area.Width, area.Height) / 4);
            RoundedRect shape = new(area, radius);
            context.DrawRectangle(background, null, shape);

            // Shade the parts of the monitor outside its working area: taskbars and app bars.
            using (context.PushClip(shape))
            {
                Rect work = ToView(monitor.WorkingArea);
                foreach (Rect strip in GetReservedStrips(area, work))
                {
                    context.DrawRectangle(reserved, null, strip);
                }
            }

            double borderThickness = monitor.Primary ? 2 : 1;
            context.DrawRectangle(null, new Pen(monitor.Primary ? accent : border, borderThickness),
                new RoundedRect(area.Deflate(borderThickness / 2), Math.Max(0, radius - borderThickness / 2)));

            DrawLabels(context, monitor, area, fontFamily, text, secondaryText);
        }

        if (_cursorPosition is DrawingPoint cursor && desktop.Contains(cursor))
        {
            Point center = new(offsetX + (cursor.X - desktop.X) * scale, offsetY + (cursor.Y - desktop.Y) * scale);
            context.DrawEllipse(accent, new Pen(panel, 2), center, CursorRadius, CursorRadius);
        }
    }

    // Reserved strips are a few pixels at this scale, so they are drawn at least MinimumStrip thick to stay visible.
    private static IEnumerable<Rect> GetReservedStrips(Rect area, Rect work)
    {
        if (work.Left > area.Left) yield return new Rect(area.Left, area.Top, Thickness(work.Left - area.Left), area.Height);
        if (work.Top > area.Top) yield return new Rect(area.Left, area.Top, area.Width, Thickness(work.Top - area.Top));
        if (work.Right < area.Right)
        {
            double size = Thickness(area.Right - work.Right);
            yield return new Rect(area.Right - size, area.Top, size, area.Height);
        }
        if (work.Bottom < area.Bottom)
        {
            double size = Thickness(area.Bottom - work.Bottom);
            yield return new Rect(area.Left, area.Bottom - size, area.Width, size);
        }
    }

    private static double Thickness(double size) => Math.Max(MinimumStrip, size);

    private static void DrawLabels(DrawingContext context, DisplayDiagnosticsMonitor monitor, Rect area, FontFamily fontFamily,
        IBrush text, IBrush secondaryText)
    {
        double numberSize = Math.Clamp(Math.Min(area.Width, area.Height) * 0.28, 11, 30);
        FormattedText number = CreateText(monitor.Number.ToString(CultureInfo.CurrentCulture), fontFamily, FontWeight.SemiBold,
            numberSize, text);

        FormattedText caption = CreateText(
            $"{monitor.Bounds.Width} × {monitor.Bounds.Height}  ·  {monitor.ScalePercent}%",
            fontFamily, FontWeight.Normal, 11, secondaryText);

        bool showCaption = caption.Width + 12 <= area.Width && number.Height + caption.Height + 8 <= area.Height;
        double totalHeight = number.Height + (showCaption ? caption.Height + 2 : 0);
        double top = area.Center.Y - totalHeight / 2;

        if (number.Width <= area.Width && number.Height <= area.Height)
        {
            context.DrawText(number, new Point(area.Center.X - number.Width / 2, top));
        }

        if (showCaption)
        {
            context.DrawText(caption, new Point(area.Center.X - caption.Width / 2, top + number.Height + 2));
        }
    }

    private static FormattedText CreateText(string value, FontFamily fontFamily, FontWeight weight, double size, IBrush brush) =>
        new(value, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(fontFamily, FontStyle.Normal, weight), size, brush);

    private static IBrush WithOpacity(IBrush brush, double opacity) =>
        brush is ISolidColorBrush solid ? new SolidColorBrush(solid.Color, solid.Opacity * opacity) : brush;

    private IBrush? FindBrush(string key) => FindResource<IBrush>(key);

    private T? FindResource<T>(string key) where T : class =>
        this.TryFindResource(key, ActualThemeVariant, out object? value) ? value as T : null;
}
