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

using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace ShareX.PixelEditor.UI;

/// <summary>A ruler along the top or left edge of the canvas, measured in sprite pixels. Dragging from it adds a guide.</summary>
public sealed class Ruler : Control
{
    public const double Thickness = 18;

    private static readonly int[] Steps = [1, 2, 5, 10, 20, 25, 50, 100, 200, 250, 500, 1000, 2000, 5000];
    private static readonly IBrush Background = new SolidColorBrush(Color.FromRgb(0x2B, 0x2B, 0x2B));
    private static readonly IBrush SpriteArea = new SolidColorBrush(Color.FromRgb(0x36, 0x36, 0x36));
    private static readonly Pen Tick = new(new SolidColorBrush(Color.FromRgb(0x80, 0x80, 0x80)), 1);
    private static readonly Pen Marker = new(Brushes.White, 1);
    private static readonly Typeface Font = new("Segoe UI");

    private readonly PixelEditorWindow _editor;
    private readonly bool _vertical;
    private Guide? _dragging;
    private double? _marker;

    public Ruler(PixelEditorWindow editor, bool vertical)
    {
        _editor = editor;
        _vertical = vertical;
        ClipToBounds = true;
        Cursor = new Cursor(vertical ? StandardCursorType.SizeWestEast : StandardCursorType.SizeNorthSouth);
        ToolTip.SetTip(this, Strings.Ruler_Tip);
        if (vertical) Width = Thickness;
        else Height = Thickness;
    }

    /// <summary>Marks the pointer position on the ruler, in sprite pixels.</summary>
    public void SetMarker(double? position)
    {
        if (_marker == position) return;
        _marker = position;
        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        Rect bounds = new(Bounds.Size);
        context.FillRectangle(Background, bounds);
        SpriteView? view = _editor.View;
        if (view == null) return;
        double zoom = view.Zoom, pan = _vertical ? view.PanY : view.PanX;
        double length = _vertical ? bounds.Height : bounds.Width;
        int size = _vertical ? view.Sprite.Height : view.Sprite.Width;

        // The extent of the sprite.
        double start = pan, end = pan + size * zoom;
        context.FillRectangle(SpriteArea, _vertical ? new Rect(0, start, Thickness, end - start) : new Rect(start, 0, end - start, Thickness));

        int step = Steps.FirstOrDefault(s => s * zoom >= 48, Steps[^1]);
        int minor = step >= 10 ? step / (step % 25 == 0 && step >= 25 ? 5 : 10) : step >= 5 ? 1 : 0;
        if (minor > 0 && minor * zoom < 4) minor = step / 2 * zoom >= 4 ? step / 2 : 0;
        double first = Math.Floor(-pan / zoom / step) * step, last = (length - pan) / zoom;
        for (double value = first; value <= last; value += step)
        {
            double at = Math.Round(pan + value * zoom) + 0.5;
            Line(context, at, 0, Thickness);
            FormattedText label = new(((int)value).ToString(CultureInfo.CurrentCulture), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, Font, 9, Tick.Brush);
            if (_vertical)
            {
                using (context.PushTransform(Matrix.CreateRotation(-Math.PI / 2) * Matrix.CreateTranslation(2, at - 2)))
                    context.DrawText(label, new Point(0, 0));
            }
            else
            {
                context.DrawText(label, new Point(at + 2, 0));
            }
            if (minor <= 0) continue;
            for (double sub = value + minor; sub < value + step - 0.001; sub += minor)
            {
                double subAt = Math.Round(pan + sub * zoom) + 0.5;
                Line(context, subAt, Thickness * (Math.Abs(sub - value - step / 2.0) < 0.001 ? 0.45 : 0.7), Thickness);
            }
        }

        foreach (Guide guide in view.Guides.Where(g => g.Vertical != _vertical))
        {
            double at = Math.Round(pan + guide.Position * zoom) + 0.5;
            Pen pen = new(GuideBrush, 1);
            if (_vertical) context.DrawLine(pen, new Point(0, at), new Point(Thickness, at));
            else context.DrawLine(pen, new Point(at, 0), new Point(at, Thickness));
        }
        if (_marker is double marker)
        {
            double at = Math.Round(pan + marker * zoom) + 0.5;
            if (_vertical) context.DrawLine(Marker, new Point(0, at), new Point(Thickness, at));
            else context.DrawLine(Marker, new Point(at, 0), new Point(at, Thickness));
        }
        context.FillRectangle(Ui.Gutter, _vertical ? new Rect(bounds.Width - 1, 0, 1, bounds.Height) : new Rect(0, bounds.Height - 1, bounds.Width, 1));
    }

    public static readonly IBrush GuideBrush = new SolidColorBrush(Color.FromRgb(0x2E, 0xD4, 0xF0));

    private void Line(DrawingContext context, double at, double from, double to)
    {
        if (_vertical) context.DrawLine(Tick, new Point(from, at), new Point(to, at));
        else context.DrawLine(Tick, new Point(at, from), new Point(at, to));
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        SpriteView? view = _editor.View;
        if (view == null || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
        // A horizontal ruler makes horizontal guides, which are not vertical lines.
        _dragging = new Guide { Vertical = _vertical, Position = 0 };
        view.Guides.Add(_dragging);
        e.Pointer.Capture(this);
        Drag(e);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (_dragging != null) Drag(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (_dragging == null) return;
        Guide guide = _dragging;
        _dragging = null;
        e.Pointer.Capture(null);
        // Releasing over the ruler, or outside the canvas, drops the guide.
        Point p = e.GetPosition(_editor.Canvas);
        if (p.X < 0 || p.Y < 0 || p.X > _editor.Canvas.Bounds.Width || p.Y > _editor.Canvas.Bounds.Height) _editor.View?.Guides.Remove(guide);
        _editor.InvalidateRulers();
    }

    private void Drag(PointerEventArgs e)
    {
        SpriteView? view = _editor.View;
        if (view == null || _dragging == null) return;
        (double x, double y) = view.ToDocument(e.GetPosition(_editor.Canvas));
        _dragging.Position = (int)Math.Round(_dragging.Vertical ? x : y);
        _editor.SetStatus(string.Format(Strings.Ruler_GuideAt, _dragging.Position));
        _editor.InvalidateRulers();
    }
}
