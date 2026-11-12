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

/// <summary>
/// Pixels lifted off a cel that can be moved, scaled, rotated and flipped before they are dropped again.
/// The cel always shows the base image with the transformed pixels drawn on top.
/// </summary>
public sealed class Floating
{
    public required PixelBuffer Source { get; init; }

    /// <summary>Which pixels of the source belong to the selection; null when the whole source does.</summary>
    public bool[]? SourceMask { get; init; }

    /// <summary>The cel without the lifted pixels.</summary>
    public required PixelBuffer Base { get; init; }

    public required PixelLayer Layer { get; init; }

    public int Frame { get; init; }

    public double X { get; set; }

    public double Y { get; set; }

    public double Width { get; set; }

    public double Height { get; set; }

    public double Angle { get; set; }

    public bool FlipX { get; set; }

    public bool FlipY { get; set; }

    /// <summary>Whether the dropped pixels stay selected.</summary>
    public bool Selects { get; init; }

    public required string HistoryName { get; init; }

    public double CenterX => X + Width / 2;

    public double CenterY => Y + Height / 2;
}

public sealed class MoveTool : ToolBase
{
    private enum Drag
    {
        None,
        Move,
        Scale,
        Rotate
    }

    private static readonly (int X, int Y)[] Handles = [(-1, -1), (0, -1), (1, -1), (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0)];

    private Drag _drag;
    private int _handle;
    private double _startX, _startY, _startW, _startH, _startAngle, _pointerX, _pointerY;

    public MoveTool(PixelEditorWindow editor) : base(editor, ToolId.Move, Strings.Tool_Move, Strings.Tool_Move_Hint, PixelEditorIcons.Move, Key.V)
    {
    }

    public override StandardCursorType Cursor => StandardCursorType.SizeAll;

    private static (double X, double Y) ToLocal(Floating f, double x, double y)
    {
        double radians = f.Angle * Math.PI / 180, cos = Math.Cos(radians), sin = Math.Sin(radians), dx = x - f.CenterX, dy = y - f.CenterY;
        return (dx * cos + dy * sin, -dx * sin + dy * cos);
    }

    private static (double X, double Y) ToDocument(Floating f, double lx, double ly)
    {
        double radians = f.Angle * Math.PI / 180, cos = Math.Cos(radians), sin = Math.Sin(radians);
        return (f.CenterX + lx * cos - ly * sin, f.CenterY + lx * sin + ly * cos);
    }

    private (Drag Drag, int Handle) HitTest(Floating f, ToolPointer p)
    {
        double zoom = Editor.View?.Zoom ?? 1, tolerance = 7 / zoom;
        (double lx, double ly) = ToLocal(f, p.Fx, p.Fy);
        double hw = f.Width / 2, hh = f.Height / 2;
        if (Math.Abs(lx) <= tolerance && Math.Abs(ly - (-hh - 22 / zoom)) <= tolerance) return (Drag.Rotate, 0);
        for (int i = 0; i < Handles.Length; i++)
        {
            if (Math.Abs(lx - Handles[i].X * hw) <= tolerance && Math.Abs(ly - Handles[i].Y * hh) <= tolerance) return (Drag.Scale, i);
        }
        return Math.Abs(lx) <= hw && Math.Abs(ly) <= hh ? (Drag.Move, 0) : (Drag.None, 0);
    }

    public override void Down(ToolPointer p)
    {
        Floating? f = Editor.Floating;
        if (f == null)
        {
            // Alt lifts a copy and leaves the original pixels in place.
            if (!Editor.BeginFloating(p.Alt)) return;
            f = Editor.Floating!;
            _drag = Drag.Move;
        }
        else
        {
            (_drag, _handle) = HitTest(f, p);
            if (_drag == Drag.None)
            {
                Editor.CommitFloating();
                return;
            }
        }
        _startX = f.X;
        _startY = f.Y;
        _startW = f.Width;
        _startH = f.Height;
        _startAngle = f.Angle;
        _pointerX = p.Fx;
        _pointerY = p.Fy;
    }

    public override void Move(ToolPointer p)
    {
        Floating? f = Editor.Floating;
        if (f == null || _drag == Drag.None) return;
        switch (_drag)
        {
            case Drag.Move:
                double dx = Math.Round(p.Fx - _pointerX), dy = Math.Round(p.Fy - _pointerY);
                if (p.Shift)
                {
                    // Shift keeps the move on one axis.
                    if (Math.Abs(dx) >= Math.Abs(dy)) dy = 0;
                    else dx = 0;
                }
                f.X = _startX + dx;
                f.Y = _startY + dy;
                if (Settings.SnapToGrid && f.Angle == 0)
                {
                    int gw = Math.Max(1, Settings.GridWidth), gh = Math.Max(1, Settings.GridHeight);
                    f.X = Math.Round(f.X / gw) * gw;
                    f.Y = Math.Round(f.Y / gh) * gh;
                }
                break;
            case Drag.Rotate:
                double start = Math.Atan2(_pointerY - (_startY + _startH / 2), _pointerX - (_startX + _startW / 2));
                double now = Math.Atan2(p.Fy - (_startY + _startH / 2), p.Fx - (_startX + _startW / 2));
                double angle = _startAngle + (now - start) * 180 / Math.PI;
                if (p.Shift) angle = Math.Round(angle / 15) * 15;
                f.Angle = ((Math.Round(angle) % 360) + 360) % 360;
                break;
            case Drag.Scale:
                Scale(f, p);
                break;
        }
        Editor.RenderFloating();
        Editor.SetStatus(string.Format(Strings.Status_Transform, Math.Round(f.X), Math.Round(f.Y), Math.Round(f.Width), Math.Round(f.Height), Math.Round(f.Angle)));
    }

    private void Scale(Floating f, ToolPointer p)
    {
        // Work in the frame of the rectangle as it was when the drag began.
        f.X = _startX;
        f.Y = _startY;
        f.Width = _startW;
        f.Height = _startH;
        (double lx, double ly) = ToLocal(f, p.Fx, p.Fy);
        double left = -_startW / 2, right = _startW / 2, top = -_startH / 2, bottom = _startH / 2;
        (int hx, int hy) = Handles[_handle];
        if (hx > 0) right = Math.Max(left + 1, Math.Round(lx - left) + left);
        if (hx < 0) left = Math.Min(right - 1, right - Math.Round(right - lx));
        if (hy > 0) bottom = Math.Max(top + 1, Math.Round(ly - top) + top);
        if (hy < 0) top = Math.Min(bottom - 1, bottom - Math.Round(bottom - ly));
        if (p.Shift && hx != 0 && hy != 0)
        {
            // Shift keeps the proportions when dragging a corner.
            double scale = Math.Max((right - left) / _startW, (bottom - top) / _startH);
            double w = Math.Max(1, Math.Round(_startW * scale)), h = Math.Max(1, Math.Round(_startH * scale));
            if (hx > 0) right = left + w;
            else left = right - w;
            if (hy > 0) bottom = top + h;
            else top = bottom - h;
        }
        (double cx, double cy) = ToDocument(f, (left + right) / 2, (top + bottom) / 2);
        f.Width = right - left;
        f.Height = bottom - top;
        f.X = cx - f.Width / 2;
        f.Y = cy - f.Height / 2;
        if (f.Angle == 0)
        {
            f.X = Math.Round(f.X);
            f.Y = Math.Round(f.Y);
        }
    }

    public override void Up(ToolPointer p)
    {
        if (_drag == Drag.None) return;
        _drag = Drag.None;
        Editor.SetStatus(null);
        Editor.RefreshOptions();
    }

    public override bool KeyDown(Key key, KeyModifiers modifiers)
    {
        int step = modifiers.HasFlag(KeyModifiers.Shift) ? 8 : 1;
        (int dx, int dy) = key switch
        {
            Key.Left => (-step, 0),
            Key.Right => (step, 0),
            Key.Up => (0, -step),
            Key.Down => (0, step),
            _ => (0, 0)
        };
        if ((dx != 0 || dy != 0) && !modifiers.HasFlag(KeyModifiers.Control) && !modifiers.HasFlag(KeyModifiers.Alt))
        {
            if (Editor.Floating == null && !Editor.BeginFloating(false)) return true;
            Floating f = Editor.Floating!;
            f.X += dx;
            f.Y += dy;
            Editor.RenderFloating();
            return true;
        }
        if (Editor.Floating == null) return false;
        if (key == Key.Enter)
        {
            Editor.CommitFloating();
            return true;
        }
        if (key == Key.Escape)
        {
            Editor.CancelFloating();
            return true;
        }
        return false;
    }

    public override void Cancel() => _drag = Drag.None;

    public override void DrawOverlay(OverlayContext overlay)
    {
        Floating? f = Editor.Floating;
        if (f == null) return;
        double hw = f.Width / 2, hh = f.Height / 2;
        (double X, double Y)[] corners = [ToDocument(f, -hw, -hh), ToDocument(f, hw, -hh), ToDocument(f, hw, hh), ToDocument(f, -hw, hh)];
        for (int i = 0; i < 4; i++) overlay.Line(corners[i].X, corners[i].Y, corners[(i + 1) % 4].X, corners[(i + 1) % 4].Y);
        (double tx, double ty) = ToDocument(f, 0, -hh);
        (double rx, double ry) = ToDocument(f, 0, -hh - 22 / overlay.Zoom);
        overlay.Line(tx, ty, rx, ry);
        overlay.Handle(rx, ry, true);
        foreach ((int hx, int hy) in Handles)
        {
            (double x, double y) = ToDocument(f, hx * hw, hy * hh);
            overlay.Handle(x, y, false);
        }
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        Floating? f = Editor.Floating;
        if (f == null)
        {
            options.Label(Strings.Options_MoveHint);
            options.Separator();
            options.Button(Strings.Options_Transform, () => Editor.Execute("edit.transform"));
            return;
        }
        void Change(Action<Floating> change)
        {
            if (Editor.Floating is not Floating current) return;
            change(current);
            Editor.RenderFloating();
        }
        options.Number("X", -Sprite.MaxSize, Sprite.MaxSize, (int)Math.Round(f.X), v => Change(c => c.X = v));
        options.Number("Y", -Sprite.MaxSize, Sprite.MaxSize, (int)Math.Round(f.Y), v => Change(c => c.Y = v));
        options.Number(Strings.Options_Width, 1, Sprite.MaxSize, (int)Math.Round(f.Width), v => Change(c => c.Width = v));
        options.Number(Strings.Options_Height, 1, Sprite.MaxSize, (int)Math.Round(f.Height), v => Change(c => c.Height = v));
        options.Number(Strings.Options_Angle, 0, 359, (int)Math.Round(f.Angle), v => Change(c => c.Angle = v), "°");
        options.Separator();
        options.IconButton(PixelEditorIcons.FlipHorizontal, Strings.Command_FlipHorizontal, () => Change(c => c.FlipX = !c.FlipX));
        options.IconButton(PixelEditorIcons.FlipVertical, Strings.Command_FlipVertical, () => Change(c => c.FlipY = !c.FlipY));
        options.IconButton(PixelEditorIcons.RotateRight, Strings.Command_Rotate90CW, () =>
        {
            Change(c => c.Angle = (c.Angle + 90) % 360);
            Editor.RefreshOptions();
        });
        options.Separator();
        options.Button(Strings.Options_Apply, Editor.CommitFloating, true);
        options.Button(Strings.Options_Cancel, Editor.CancelFloating);
    }
}
