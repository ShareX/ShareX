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

using Avalonia.Input;
using ShareX.PixelEditor.UI;

namespace ShareX.PixelEditor.Tools;

/// <summary>Base of the tools that drag out a shape; the shape is redrawn from the untouched cel on every move.</summary>
public abstract class ShapeToolBase : PaintToolBase
{
    protected ShapeToolBase(PixelEditorWindow editor, ToolId id, string name, string hint, string icon, Key shortcut) : base(editor, id, name, hint, icon, shortcut)
    {
    }

    protected SKPointI Start { get; set; }

    protected SKPointI End_ { get; set; }

    protected bool Dragging { get; private set; }

    protected abstract void Draw(Painter painter, SKPointI from, SKPointI to);

    /// <summary>Applies the Shift and Ctrl constraints to the dragged end point.</summary>
    protected virtual (SKPointI From, SKPointI To) Constrain(SKPointI from, SKPointI to, ToolPointer p) => (from, to);

    public override void Down(ToolPointer p)
    {
        if (!Begin(p)) return;
        Start = End_ = SnapToGrid(p.Pixel);
        Dragging = true;
        Redraw(p);
    }

    public override void Move(ToolPointer p)
    {
        if (Painter == null || !Dragging) return;
        End_ = SnapToGrid(p.Pixel);
        Redraw(p);
    }

    private void Redraw(ToolPointer p)
    {
        (SKPointI from, SKPointI to) = Constrain(Start, End_, p);
        Painter!.RestoreAll();
        Draw(Painter, from, to);
        Flush();
        Editor.SetStatus($"{Math.Abs(to.X - from.X) + 1} × {Math.Abs(to.Y - from.Y) + 1} px");
    }

    public override void Up(ToolPointer p)
    {
        Dragging = false;
        Editor.SetStatus(null);
        End();
    }

    public override void Cancel()
    {
        Dragging = false;
        base.Cancel();
    }

    protected void ShapeOptions(OptionsBuilder options, bool fill)
    {
        if (fill)
        {
            options.Choice(
            [
                (PixelEditorIcons.ShapeOutline, Strings.Options_Outline, false),
                (PixelEditorIcons.ShapeFilled, Strings.Options_Filled, true)
            ], Settings.ShapeFilled, v => Settings.ShapeFilled = v);
            options.Separator();
        }
        BrushOptions(options);
    }
}

public sealed class LineTool : ShapeToolBase
{
    public LineTool(PixelEditorWindow editor) : base(editor, ToolId.Line, Strings.Tool_Line, Strings.Tool_Line_Hint, PixelEditorIcons.Line, Key.L)
    {
    }

    protected override (SKPointI, SKPointI) Constrain(SKPointI from, SKPointI to, ToolPointer p) => (from, p.Shift ? ConstrainAngle(from, to) : to);

    protected override void Draw(Painter painter, SKPointI from, SKPointI to) => painter.Line(from.X, from.Y, to.X, to.Y);

    public override void BuildOptions(OptionsBuilder options) => ShapeOptions(options, false);
}

/// <summary>Drag a line, then move the pointer to bend it and click to finish.</summary>
public sealed class CurveTool : PaintToolBase
{
    private SKPointI _start, _end, _control;
    private int _phase;

    public CurveTool(PixelEditorWindow editor) : base(editor, ToolId.Curve, Strings.Tool_Curve, Strings.Tool_Curve_Hint, PixelEditorIcons.Curve, Key.L)
    {
    }

    public override void Down(ToolPointer p)
    {
        if (_phase == 2 && Painter != null)
        {
            _control = p.Pixel;
            Redraw();
            _phase = 0;
            End();
            return;
        }
        if (!Begin(p)) return;
        _start = _end = _control = p.Pixel;
        _phase = 1;
        Redraw();
    }

    public override void Move(ToolPointer p)
    {
        if (Painter == null || _phase != 1) return;
        _end = p.Shift ? ConstrainAngle(_start, p.Pixel) : p.Pixel;
        _control = new SKPointI((_start.X + _end.X) / 2, (_start.Y + _end.Y) / 2);
        Redraw();
    }

    public override void Up(ToolPointer p)
    {
        if (Painter == null || _phase != 1) return;
        if (_start == _end)
        {
            _phase = 0;
            End();
            return;
        }
        _phase = 2;
        Editor.SetStatus(Strings.Tool_Curve_Bend);
    }

    public override void Hover(ToolPointer p)
    {
        if (Painter == null || _phase != 2) return;
        _control = p.Pixel;
        Redraw();
    }

    private void Redraw()
    {
        Painter!.RestoreAll();
        // The curve passes through the control point: convert it to the matching quadratic Bezier handle.
        double hx = 2 * _control.X - (_start.X + _end.X) / 2.0, hy = 2 * _control.Y - (_start.Y + _end.Y) / 2.0;
        int steps = Math.Max(8, (Math.Abs(_end.X - _start.X) + Math.Abs(_end.Y - _start.Y) + Math.Abs(_control.X - _start.X) + Math.Abs(_control.Y - _start.Y)) / 2);
        SKPointI previous = _start;
        for (int i = 1; i <= steps; i++)
        {
            double t = i / (double)steps, u = 1 - t;
            SKPointI point = new((int)Math.Round(u * u * _start.X + 2 * u * t * hx + t * t * _end.X), (int)Math.Round(u * u * _start.Y + 2 * u * t * hy + t * t * _end.Y));
            if (point == previous) continue;
            Painter.Line(previous.X, previous.Y, point.X, point.Y);
            previous = point;
        }
        if (previous == _start) Painter.Stamp(_start.X, _start.Y);
        Flush();
    }

    public override void Cancel()
    {
        _phase = 0;
        Editor.SetStatus(null);
        base.Cancel();
    }

    public override bool KeyDown(Key key, KeyModifiers modifiers)
    {
        if (key == Key.Escape && Painter != null)
        {
            Cancel();
            return true;
        }
        return false;
    }

    public override void BuildOptions(OptionsBuilder options) => BrushOptions(options);
}

public sealed class RectangleTool : ShapeToolBase
{
    public RectangleTool(PixelEditorWindow editor) : base(editor, ToolId.Rectangle, Strings.Tool_Rectangle, Strings.Tool_Rectangle_Hint, PixelEditorIcons.Rectangle, Key.U)
    {
    }

    protected override (SKPointI, SKPointI) Constrain(SKPointI from, SKPointI to, ToolPointer p) => BoxConstrain(from, to, p);

    internal static (SKPointI, SKPointI) BoxConstrain(SKPointI from, SKPointI to, ToolPointer p)
    {
        if (p.Shift) to = ConstrainSquare(from, to);
        // Ctrl grows the box from its center.
        if (p.Control) from = new SKPointI(2 * from.X - to.X, 2 * from.Y - to.Y);
        return (from, to);
    }

    protected override void Draw(Painter painter, SKPointI from, SKPointI to) => painter.Rect(from.X, from.Y, to.X, to.Y, Settings.ShapeFilled);

    public override void BuildOptions(OptionsBuilder options) => ShapeOptions(options, true);
}

public sealed class EllipseTool : ShapeToolBase
{
    public EllipseTool(PixelEditorWindow editor) : base(editor, ToolId.Ellipse, Strings.Tool_Ellipse, Strings.Tool_Ellipse_Hint, PixelEditorIcons.Ellipse, Key.U)
    {
    }

    protected override (SKPointI, SKPointI) Constrain(SKPointI from, SKPointI to, ToolPointer p) => RectangleTool.BoxConstrain(from, to, p);

    protected override void Draw(Painter painter, SKPointI from, SKPointI to) => painter.Ellipse(from.X, from.Y, to.X, to.Y, Settings.ShapeFilled);

    public override void BuildOptions(OptionsBuilder options) => ShapeOptions(options, true);
}

/// <summary>Draws a freehand outline that is closed and filled on release.</summary>
public sealed class ContourTool : PaintToolBase
{
    private readonly List<SKPointI> _points = [];

    public ContourTool(PixelEditorWindow editor) : base(editor, ToolId.Contour, Strings.Tool_Contour, Strings.Tool_Contour_Hint, PixelEditorIcons.Contour, Key.U)
    {
    }

    public override void Down(ToolPointer p)
    {
        if (!Begin(p)) return;
        _points.Clear();
        _points.Add(p.Pixel);
        Painter!.Stamp(p.X, p.Y);
        Flush();
    }

    public override void Move(ToolPointer p)
    {
        if (Painter == null || _points.Count == 0 || _points[^1] == p.Pixel) return;
        SKPointI from = _points[^1];
        Painter.Line(from.X, from.Y, p.X, p.Y);
        _points.Add(p.Pixel);
        Flush();
    }

    public override void Up(ToolPointer p)
    {
        if (Painter == null) return;
        if (_points.Count > 1)
        {
            Painter.Line(_points[^1].X, _points[^1].Y, _points[0].X, _points[0].Y);
            if (Settings.ShapeFilled) Painter.Polygon(_points, true);
            Flush();
        }
        End();
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Choice(
        [
            (PixelEditorIcons.ShapeOutline, Strings.Options_Outline, false),
            (PixelEditorIcons.ShapeFilled, Strings.Options_Filled, true)
        ], Settings.ShapeFilled, v => Settings.ShapeFilled = v);
        options.Separator();
        BrushOptions(options);
    }
}
