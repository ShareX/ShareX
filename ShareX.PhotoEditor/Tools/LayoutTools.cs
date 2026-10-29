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
using Avalonia.Input;
using Avalonia.Media;
using ShareX.PhotoEditor.UI;

namespace ShareX.PhotoEditor.Tools;

public sealed class CropTool : ToolBase
{
    private SKPoint _center;
    private SKSize _size;
    private float _angle;
    private bool _active;
    private int _handle = -1;
    private SKPoint _dragStart;
    private SKPoint _startCenter;
    private SKSize _startSize;
    private float _startAngle;
    private SKPoint[] _startCorners = new SKPoint[4];

    public CropTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.Crop;

    public override string Name => "Crop Tool";

    public override string Hint => "Drag to define the crop area; drag outside it to rotate. Enter commits, Esc cancels.";

    public override bool HasPendingOperation => _active;

    public float RatioWidth { get; set; }

    public float RatioHeight { get; set; }

    public bool DeleteCroppedPixels { get; set; } = true;

    public bool ShowThirds { get; set; } = true;

    public override void Activate()
    {
        if (Doc != null) Reset();
    }

    private void Reset()
    {
        if (Doc == null) return;
        _center = new SKPoint(Doc.Width / 2f, Doc.Height / 2f);
        _size = new SKSize(Doc.Width, Doc.Height);
        _angle = 0;
        _active = true;
        Host.InvalidateOverlay();
        Host.RefreshOptions();
    }

    private SKMatrix BoxMatrix => SKMatrix.CreateRotationDegrees(_angle).PostConcat(SKMatrix.CreateTranslation(_center.X, _center.Y));

    private SKPoint[] Corners()
    {
        float w = _size.Width / 2, h = _size.Height / 2;
        return BoxMatrix.MapPoints([new(-w, -h), new(w, -h), new(w, h), new(-w, h)]);
    }

    private SKPoint ToLocal(SKPoint doc) => BoxMatrix.Invert().MapPoint(doc);

    private int HitHandle(ToolInput input)
    {
        if (Host.View == null) return -1;
        SKPoint[] c = Corners();
        double tolerance = 9;
        for (int i = 0; i < 4; i++) if (Dist(Host.View.DocToScreen(c[i]), input.Screen) < tolerance) return i;
        for (int i = 0; i < 4; i++)
        {
            SKPoint mid = new((c[i].X + c[(i + 1) % 4].X) / 2, (c[i].Y + c[(i + 1) % 4].Y) / 2);
            if (Dist(Host.View.DocToScreen(mid), input.Screen) < tolerance) return 4 + i;
        }
        SKPoint local = ToLocal(input.Doc);
        if (MathF.Abs(local.X) <= _size.Width / 2 && MathF.Abs(local.Y) <= _size.Height / 2) return 8;
        float outside = Math.Max(MathF.Abs(local.X) - _size.Width / 2, MathF.Abs(local.Y) - _size.Height / 2) * (float)Host.View.Zoom;
        return outside < 40 ? 9 : 10;
    }

    private static double Dist(Point a, Point b) => Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        if (!_active) Reset();
        if (input.ClickCount >= 2 && HitHandle(input) == 8)
        {
            Commit();
            return;
        }
        _handle = HitHandle(input);
        _dragStart = input.Doc;
        _startCenter = _center;
        _startSize = _size;
        _startAngle = _angle;
        _startCorners = Corners();
        if (_handle == 10)
        {
            _angle = 0;
            _center = input.Doc;
            _size = new SKSize(0, 0);
            _startCenter = input.Doc;
        }
    }

    public override void PointerMove(ToolInput input)
    {
        if (_handle < 0) return;
        float ratio = RatioWidth > 0 && RatioHeight > 0 ? RatioWidth / RatioHeight : input.Shift ? (_startSize.Height > 0 ? _startSize.Width / _startSize.Height : 1) : 0;
        if (_handle == 10)
        {
            SKRect r = DragRect(_startCenter, input.Doc, false, input.Alt);
            if (ratio > 0)
            {
                float h = Math.Max(r.Height, r.Width / ratio);
                r = SKRect.Create(input.Doc.X >= _startCenter.X || input.Alt ? r.Left : r.Right - h * ratio, input.Doc.Y >= _startCenter.Y || input.Alt ? r.Top : r.Bottom - h, h * ratio, h);
            }
            _center = new SKPoint(r.MidX, r.MidY);
            _size = new SKSize(r.Width, r.Height);
        }
        else if (_handle == 8)
        {
            _center = _startCenter + (input.Doc - _dragStart);
        }
        else if (_handle == 9)
        {
            float a0 = MathF.Atan2(_dragStart.Y - _startCenter.Y, _dragStart.X - _startCenter.X);
            float a1 = MathF.Atan2(input.Doc.Y - _startCenter.Y, input.Doc.X - _startCenter.X);
            _angle = _startAngle + (a1 - a0) * 180 / MathF.PI;
        }
        else
        {
            // Resize in the box's local frame, keeping the opposite side fixed.
            SKMatrix startMatrix = SKMatrix.CreateRotationDegrees(_startAngle).PostConcat(SKMatrix.CreateTranslation(_startCenter.X, _startCenter.Y));
            SKPoint local = startMatrix.Invert().MapPoint(input.Doc);
            float left = -_startSize.Width / 2, right = _startSize.Width / 2, top = -_startSize.Height / 2, bottom = _startSize.Height / 2;
            bool moveLeft = _handle is 0 or 3 or 7, moveRight = _handle is 1 or 2 or 5, moveTop = _handle is 0 or 1 or 4, moveBottom = _handle is 2 or 3 or 6;
            if (moveLeft) left = Math.Min(local.X, right - 1);
            if (moveRight) right = Math.Max(local.X, left + 1);
            if (moveTop) top = Math.Min(local.Y, bottom - 1);
            if (moveBottom) bottom = Math.Max(local.Y, top + 1);
            if (ratio > 0)
            {
                float w = right - left, h = bottom - top;
                if (_handle >= 4 && (_handle == 4 || _handle == 6)) w = h * ratio;
                else h = w / ratio;
                if (moveLeft) left = right - w; else right = left + w;
                if (moveTop) top = bottom - h; else bottom = top + h;
            }
            SKPoint localCenter = new((left + right) / 2, (top + bottom) / 2);
            _center = startMatrix.MapPoint(localCenter);
            _size = new SKSize(right - left, bottom - top);
        }
        Host.SetStatus($"W: {_size.Width:0} px  H: {_size.Height:0} px  Angle: {_angle:0.0}°");
        Host.InvalidateOverlay();
    }

    public override void PointerUp(ToolInput input)
    {
        _handle = -1;
        if (_size.Width < 2 || _size.Height < 2) Reset();
        Host.RefreshOptions();
    }

    public override void Commit()
    {
        if (!_active || Doc == null) return;
        if (_angle == 0 && MathF.Abs(_size.Width - Doc.Width) < 0.5f && MathF.Abs(_size.Height - Doc.Height) < 0.5f &&
            MathF.Abs(_center.X - Doc.Width / 2f) < 0.5f && MathF.Abs(_center.Y - Doc.Height / 2f) < 0.5f)
        {
            return;
        }
        DocumentTransforms.CropRotated(Doc, _center, _size, _angle, DeleteCroppedPixels, Host.Background, Host.Settings.DefaultResample);
        PushHistory("Crop");
        Host.View?.FitOnScreen();
        Reset();
        Host.InvalidateCanvas();
    }

    public override void Cancel() => Reset();

    public override void DrawOverlay(OverlayContext context)
    {
        if (!_active || Doc == null) return;
        SKPoint[] c = Corners();
        // Dim the area outside the crop box.
        StreamGeometry shade = new();
        using (StreamGeometryContext g = shade.Open())
        {
            g.SetFillRule(FillRule.EvenOdd);
            SKPoint[] doc = [new(0, 0), new(Doc.Width, 0), new(Doc.Width, Doc.Height), new(0, Doc.Height)];
            g.BeginFigure(context.ToScreen(doc[0]), true);
            foreach (SKPoint p in doc.Skip(1)) g.LineTo(context.ToScreen(p));
            g.EndFigure(true);
            g.BeginFigure(context.ToScreen(c[0]), true);
            foreach (SKPoint p in c.Skip(1)) g.LineTo(context.ToScreen(p));
            g.EndFigure(true);
        }
        context.Drawing.DrawGeometry(new SolidColorBrush(Color.FromArgb(140, 0, 0, 0)), null, shade);
        context.Polygon(c, true, new Pen(Brushes.White, 1));
        if (ShowThirds)
        {
            Pen thin = new(new SolidColorBrush(Color.FromArgb(150, 255, 255, 255)), 0.75);
            for (int i = 1; i < 3; i++)
            {
                float t = i / 3f;
                context.Line(Lerp(c[0], c[1], t), Lerp(c[3], c[2], t), thin);
                context.Line(Lerp(c[0], c[3], t), Lerp(c[1], c[2], t), thin);
            }
        }
        for (int i = 0; i < 4; i++)
        {
            context.Handle(c[i]);
            context.Handle(Lerp(c[i], c[(i + 1) % 4], 0.5f));
        }
    }

    private static SKPoint Lerp(SKPoint a, SKPoint b, float t) => new(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t);

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Combo("Ratio:", [("Ratio", (0f, 0f)), ("Original Ratio", (-1f, -1f)), ("1 : 1 (Square)", (1f, 1f)), ("4 : 5 (8 : 10)", (4f, 5f)), ("5 : 7", (5f, 7f)), ("2 : 3 (4 : 6)", (2f, 3f)), ("16 : 9", (16f, 9f))],
            () => (RatioWidth, RatioHeight), v =>
            {
                if (v.Item1 < 0 && Doc != null) { RatioWidth = Doc.Width; RatioHeight = Doc.Height; }
                else { RatioWidth = v.Item1; RatioHeight = v.Item2; }
                Host.RefreshOptions();
            }, 140);
        options.Number("W:", 0, 100000, () => RatioWidth, v => RatioWidth = (float)v, "0.##", null, 1, 60);
        options.Button("⇄", () => { (RatioWidth, RatioHeight) = (RatioHeight, RatioWidth); Host.RefreshOptions(); }, "Swap width and height");
        options.Number("H:", 0, 100000, () => RatioHeight, v => RatioHeight = (float)v, "0.##", null, 1, 60);
        options.Button("Clear", () => { RatioWidth = RatioHeight = 0; Host.RefreshOptions(); });
        options.Separator();
        options.Check("Rule of Thirds", () => ShowThirds, v => { ShowThirds = v; Host.InvalidateOverlay(); });
        options.Check("Delete Cropped Pixels", () => DeleteCroppedPixels, v => DeleteCroppedPixels = v);
        options.Separator();
        options.Button("Cancel", Cancel);
        options.Button("Commit", Commit);
    }
}

public sealed class PerspectiveCropTool : ToolBase
{
    private SKPoint[]? _quad;
    private int _handle = -1;
    private SKPoint _dragStart;
    private SKPoint[] _startQuad = new SKPoint[4];
    private bool _creating;

    public PerspectiveCropTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.PerspectiveCrop;

    public override string Name => "Perspective Crop Tool";

    public override string Hint => "Drag to create a crop box, then drag its corners to match the perspective of an object. Enter crops and straightens.";

    public override bool HasPendingOperation => _quad != null;

    public bool ShowGrid { get; set; } = true;

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton || Host.View == null) return;
        _dragStart = input.Doc;
        if (_quad != null)
        {
            for (int i = 0; i < 4; i++)
            {
                if (SKPoint.Distance(_quad[i], input.Doc) * Host.View.Zoom < 9)
                {
                    _handle = i;
                    _startQuad = (SKPoint[])_quad.Clone();
                    return;
                }
            }
            using SKPath path = new();
            path.AddPoly(_quad, true);
            if (path.Contains(input.Doc.X, input.Doc.Y))
            {
                if (input.ClickCount >= 2)
                {
                    Commit();
                    return;
                }
                _handle = 8;
                _startQuad = (SKPoint[])_quad.Clone();
                return;
            }
        }
        _creating = true;
        _quad = [input.Doc, input.Doc, input.Doc, input.Doc];
    }

    public override void PointerMove(ToolInput input)
    {
        if (_quad == null) return;
        if (_creating)
        {
            SKRect r = DragRect(_dragStart, input.Doc, input.Shift, false);
            _quad = [new(r.Left, r.Top), new(r.Right, r.Top), new(r.Right, r.Bottom), new(r.Left, r.Bottom)];
        }
        else if (_handle >= 0 && _handle < 4)
        {
            _quad[_handle] = _startQuad[_handle] + (input.Doc - _dragStart);
        }
        else if (_handle == 8)
        {
            for (int i = 0; i < 4; i++) _quad[i] = _startQuad[i] + (input.Doc - _dragStart);
        }
        else return;
        Host.InvalidateOverlay();
    }

    public override void PointerUp(ToolInput input)
    {
        _handle = -1;
        if (_creating && _quad != null && SKPoint.Distance(_quad[0], _quad[2]) < 3) _quad = null;
        _creating = false;
        Host.InvalidateOverlay();
    }

    public override void Commit()
    {
        if (_quad == null || Doc == null) return;
        int width = (int)MathF.Round((SKPoint.Distance(_quad[0], _quad[1]) + SKPoint.Distance(_quad[3], _quad[2])) / 2);
        int height = (int)MathF.Round((SKPoint.Distance(_quad[0], _quad[3]) + SKPoint.Distance(_quad[1], _quad[2])) / 2);
        if (width < 1 || height < 1) return;
        DocumentTransforms.PerspectiveCrop(Doc, _quad, width, height, Host.Settings.DefaultResample);
        _quad = null;
        PushHistory("Perspective Crop");
        Host.View?.FitOnScreen();
        Host.InvalidateCanvas();
    }

    public override void Cancel()
    {
        _quad = null;
        Host.InvalidateOverlay();
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (_quad == null) return;
        context.Polygon(_quad, true, new Pen(Brushes.White, 1));
        if (ShowGrid)
        {
            Pen thin = new(new SolidColorBrush(Color.FromArgb(140, 255, 255, 255)), 0.75);
            SKMatrix m = ImageOps.RectToQuad(SKRect.Create(1, 1), _quad);
            for (int i = 1; i < 8; i++)
            {
                float t = i / 8f;
                context.Line(m.MapPoint(new SKPoint(t, 0)), m.MapPoint(new SKPoint(t, 1)), thin);
                context.Line(m.MapPoint(new SKPoint(0, t)), m.MapPoint(new SKPoint(1, t)), thin);
            }
        }
        foreach (SKPoint p in _quad) context.Handle(p);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Check("Show Grid", () => ShowGrid, v => { ShowGrid = v; Host.InvalidateOverlay(); });
        options.Button("Cancel", Cancel);
        options.Button("Commit", Commit);
    }
}

public sealed class SliceTool : ToolBase
{
    private SKPoint _start;
    private SKPoint _current;
    private bool _dragging;

    public SliceTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.Slice;

    public override string Name => "Slice Tool";

    public override string Hint => "Drag to create a slice. Use File > Export > Save Slices to export them.";

    public float FixedWidth { get; set; }

    public float FixedHeight { get; set; }

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        _start = _current = input.Doc;
        _dragging = true;
    }

    public override void PointerMove(ToolInput input)
    {
        if (!_dragging) return;
        _current = input.Doc;
        Host.InvalidateOverlay();
    }

    private SKRectI Rect()
    {
        SKRect r = FixedWidth > 0 && FixedHeight > 0 ? SKRect.Create(_current.X, _current.Y, FixedWidth, FixedHeight) : DragRect(_start, _current, false, false);
        SKRectI i = SKRectI.Round(r);
        i.Intersect(Doc!.Bounds);
        return i;
    }

    public override void PointerUp(ToolInput input)
    {
        if (!_dragging || Doc == null) return;
        _dragging = false;
        SKRectI rect = Rect();
        Host.InvalidateOverlay();
        if (rect.Width < 2 || rect.Height < 2) return;
        Doc.Slices.Add(new SliceItem { Bounds = rect, Name = $"{Path.GetFileNameWithoutExtension(Doc.DisplayName.TrimEnd('*'))}_{Doc.Slices.Count + 1:00}" });
        PushHistory("Slice");
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (!_dragging) return;
        SKRectI r = Rect();
        context.Rect(SKRect.Create(r.Left, r.Top, r.Width, r.Height), OverlayContext.AccentPen);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Number("Fixed W:", 0, 30000, () => FixedWidth, v => FixedWidth = (float)v, "0", "px");
        options.Number("H:", 0, 30000, () => FixedHeight, v => FixedHeight = (float)v, "0", "px");
        options.Button("Slices From Guides", () =>
        {
            if (Doc == null) return;
            List<float> xs = Doc.Guides.Where(g => !g.Horizontal).Select(g => g.Position).Append(0).Append(Doc.Width).Distinct().OrderBy(x => x).ToList();
            List<float> ys = Doc.Guides.Where(g => g.Horizontal).Select(g => g.Position).Append(0).Append(Doc.Height).Distinct().OrderBy(y => y).ToList();
            Doc.Slices.Clear();
            for (int r = 0; r < ys.Count - 1; r++)
                for (int c = 0; c < xs.Count - 1; c++)
                    Doc.Slices.Add(new SliceItem { Bounds = new SKRectI((int)xs[c], (int)ys[r], (int)xs[c + 1], (int)ys[r + 1]), Name = $"slice_{r + 1:00}_{c + 1:00}" });
            PushHistory("Slices From Guides");
            Host.InvalidateOverlay();
        });
        options.Button("Clear Slices", () =>
        {
            if (Doc == null) return;
            Doc.Slices.Clear();
            PushHistory("Clear Slices");
            Host.InvalidateOverlay();
        });
    }
}

public sealed class SliceSelectTool : ToolBase
{
    private SliceItem? _selected;
    private int _handle = -1;
    private SKPoint _dragStart;
    private SKRectI _startBounds;

    public SliceSelectTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.SliceSelect;

    public override string Name => "Slice Select Tool";

    public override string Hint => "Click to select a slice, drag to move or resize it. Delete removes it, double-click renames it.";

    public override ToolCursor Cursor => ToolCursor.Arrow;

    public SliceItem? Selected => _selected;

    public override async void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton || Host.View == null) return;
        if (_selected != null)
        {
            SKRectI b = _selected.Bounds;
            SKPoint[] corners = [new(b.Left, b.Top), new(b.Right, b.Top), new(b.Right, b.Bottom), new(b.Left, b.Bottom)];
            for (int i = 0; i < 4; i++)
            {
                if (SKPoint.Distance(corners[i], input.Doc) * Host.View.Zoom < 8)
                {
                    _handle = i;
                    _dragStart = input.Doc;
                    _startBounds = b;
                    return;
                }
            }
        }
        _selected = Doc.Slices.LastOrDefault(s => s.Bounds.Contains(input.DocPixel.X, input.DocPixel.Y));
        Host.RefreshOptions();
        Host.InvalidateOverlay();
        if (_selected == null) return;
        if (input.ClickCount >= 2)
        {
            string? name = await Host.PromptTextAsync("Slice Options", "Name:", _selected.Name);
            if (name != null)
            {
                _selected.Name = name;
                PushHistory("Edit Slice Options");
            }
            return;
        }
        _handle = 8;
        _dragStart = input.Doc;
        _startBounds = _selected.Bounds;
    }

    public override void PointerMove(ToolInput input)
    {
        if (_selected == null || _handle < 0) return;
        int dx = (int)MathF.Round(input.Doc.X - _dragStart.X), dy = (int)MathF.Round(input.Doc.Y - _dragStart.Y);
        SKRectI b = _startBounds;
        switch (_handle)
        {
            case 8: b.Offset(dx, dy); break;
            case 0: b.Left += dx; b.Top += dy; break;
            case 1: b.Right += dx; b.Top += dy; break;
            case 2: b.Right += dx; b.Bottom += dy; break;
            case 3: b.Left += dx; b.Bottom += dy; break;
        }
        _selected.Bounds = b.Standardized;
        Host.InvalidateOverlay();
    }

    public override void PointerUp(ToolInput input)
    {
        if (_handle >= 0 && _selected != null && _selected.Bounds != _startBounds) PushHistory("Edit Slice");
        _handle = -1;
    }

    public override bool KeyDown(Key key, KeyModifiers modifiers)
    {
        if ((key == Key.Delete || key == Key.Back) && _selected != null && Doc != null)
        {
            Doc.Slices.Remove(_selected);
            _selected = null;
            PushHistory("Delete Slice");
            Host.InvalidateOverlay();
            return true;
        }
        return base.KeyDown(key, modifiers);
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (_selected == null) return;
        SKRectI b = _selected.Bounds;
        context.Rect(SKRect.Create(b.Left, b.Top, b.Width, b.Height), new Pen(Brushes.Orange, 2));
        foreach (SKPoint c in new SKPoint[] { new(b.Left, b.Top), new(b.Right, b.Top), new(b.Right, b.Bottom), new(b.Left, b.Bottom) }) context.Handle(c);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        if (_selected == null)
        {
            options.Label("No slice selected");
            return;
        }
        options.Text("Name:", () => _selected.Name, v => _selected.Name = v, 160);
        options.Text("URL:", () => _selected.Url, v => _selected.Url = v, 160);
        options.Button("Divide...", async () =>
        {
            if (Doc == null || _selected == null) return;
            string? text = await Host.PromptTextAsync("Divide Slice", "Rows x Columns (for example 2x3):", "2x2");
            if (text == null) return;
            string[] parts = text.ToLowerInvariant().Split('x');
            if (parts.Length != 2 || !int.TryParse(parts[0], out int rows) || !int.TryParse(parts[1], out int cols) || rows < 1 || cols < 1) return;
            SKRectI b = _selected.Bounds;
            Doc.Slices.Remove(_selected);
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    Doc.Slices.Add(new SliceItem
                    {
                        Bounds = new SKRectI(b.Left + b.Width * c / cols, b.Top + b.Height * r / rows, b.Left + b.Width * (c + 1) / cols, b.Top + b.Height * (r + 1) / rows),
                        Name = $"{_selected.Name}_{r + 1}_{c + 1}"
                    });
            _selected = null;
            PushHistory("Divide Slice");
            Host.RefreshOptions();
            Host.InvalidateOverlay();
        });
    }
}

public sealed class FrameTool : ToolBase
{
    private SKPoint _start;
    private SKPoint _current;
    private bool _dragging;

    public FrameTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.Frame;

    public override string Name => "Frame Tool";

    public override string Hint => "Drag to create a frame placeholder. Place or paste an image to fill the active frame.";

    public bool Ellipse { get; set; }

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        _start = _current = input.Doc;
        _dragging = true;
    }

    public override void PointerMove(ToolInput input)
    {
        if (!_dragging) return;
        _current = input.Doc;
        Host.InvalidateOverlay();
    }

    public override void PointerUp(ToolInput input)
    {
        if (!_dragging || Doc == null) return;
        _dragging = false;
        SKRect rect = DragRect(_start, input.Doc, input.Shift, input.Alt);
        Host.InvalidateOverlay();
        if (rect.Width < 4 || rect.Height < 4)
        {
            FrameLayer? hit = Doc.AllLayers.OfType<FrameLayer>().LastOrDefault(f => f.Frame.Contains(input.Doc));
            if (hit != null)
            {
                Doc.ActiveLayer = hit;
                Host.RefreshPanels();
            }
            return;
        }
        FrameLayer frame = new($"Frame {Doc.AllLayers.OfType<FrameLayer>().Count() + 1}", rect, Ellipse);
        Layer? active = Doc.ActiveLayer;
        if (active is RasterLayer raster && !raster.IsBackground && input.Ctrl)
        {
            // Ctrl-drag converts the active layer into the frame's content.
            SKRectI bounds = raster.GetOpaqueBounds();
            if (!bounds.IsEmpty)
            {
                frame.Content = new PixelSurface(ImageOps.Crop(raster.Surface.Bitmap, SKRectI.Create(bounds.Left - raster.Offset.X, bounds.Top - raster.Offset.Y, bounds.Width, bounds.Height)));
                frame.ContentRect = bounds;
                Doc.RemoveLayer(raster);
            }
        }
        Doc.AddLayer(frame);
        PushHistory("Frame");
        Host.InvalidateCanvas();
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (!_dragging) return;
        SKRect rect = DragRect(_start, _current, false, false);
        if (Ellipse) context.Ellipse(rect, OverlayContext.AccentPen);
        else context.Rect(rect, OverlayContext.AccentPen);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.IconButton("frame-rect", "Rectangular frame", () => { Ellipse = false; Host.RefreshOptions(); }, !Ellipse);
        options.IconButton("frame-ellipse", "Elliptical frame", () => { Ellipse = true; Host.RefreshOptions(); }, Ellipse);
        options.Separator();
        options.Button("Place Image...", () => UI.Commands.EditorCommands.PlaceIntoFrame(Host), "Place an image file into the active frame");
        options.Button("Clear Frame Content", () =>
        {
            if (Doc?.ActiveLayer is FrameLayer frame && frame.Content != null)
            {
                frame.Content = null;
                PushHistory("Clear Frame");
                Host.InvalidateCanvas();
            }
        });
    }
}

public sealed class ArtboardTool : ToolBase
{
    private SKPoint _start;
    private SKPoint _current;
    private bool _creating;
    private GroupLayer? _moving;
    private SKPoint _applied;

    public ArtboardTool(IEditorHost host) : base(host)
    {
    }

    public override ToolId Id => ToolId.Artboard;

    public override string Name => "Artboard Tool";

    public override string Hint => "Drag on empty canvas to create an artboard. Drag an artboard's title to move it with its contents.";

    public SKColor BackgroundColor { get; set; } = SKColors.White;

    public SKSizeI PresetSize { get; set; }

    private GroupLayer? HitArtboardTitle(ToolInput input)
    {
        if (Doc == null || Host.View == null) return null;
        float titleHeight = 20 / (float)Host.View.Zoom;
        return Doc.AllLayers.OfType<GroupLayer>().LastOrDefault(g => g.ArtboardBounds is SKRectI b &&
            input.Doc.X >= b.Left && input.Doc.X <= b.Right && input.Doc.Y >= b.Top - titleHeight && input.Doc.Y <= b.Bottom);
    }

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        _moving = HitArtboardTitle(input);
        _start = _current = input.Doc;
        _applied = default;
        if (_moving != null)
        {
            Doc.ActiveLayer = _moving;
            Host.RefreshPanels();
            return;
        }
        _creating = true;
    }

    public override void PointerMove(ToolInput input)
    {
        if (Doc == null) return;
        if (_moving != null)
        {
            SKPoint delta = new(MathF.Round(input.Doc.X - _start.X), MathF.Round(input.Doc.Y - _start.Y));
            SKPoint step = delta - _applied;
            _moving.Translate(step.X, step.Y);
            _applied = delta;
            Host.InvalidateCanvas();
            return;
        }
        if (!_creating) return;
        _current = input.Doc;
        Host.InvalidateOverlay();
    }

    public override void PointerUp(ToolInput input)
    {
        if (Doc == null) return;
        if (_moving != null)
        {
            _moving = null;
            if (_applied != default) PushHistory("Move Artboard");
            return;
        }
        if (!_creating) return;
        _creating = false;
        SKRect r = DragRect(_start, _current, input.Shift, false);
        if (r.Width < 4 || r.Height < 4)
        {
            if (PresetSize.Width <= 0) return;
            r = SKRect.Create(_start.X, _start.Y, PresetSize.Width, PresetSize.Height);
        }
        CreateArtboard(SKRectI.Round(r));
    }

    public void CreateArtboard(SKRectI bounds)
    {
        if (Doc == null) return;
        int number = Doc.AllLayers.OfType<GroupLayer>().Count(g => g.IsArtboard) + 1;
        GroupLayer artboard = new($"Artboard {number}") { ArtboardBounds = bounds, ArtboardBackground = BackgroundColor, BlendMode = LayerBlendMode.PassThrough };
        Layer? active = Doc.ActiveLayer;
        Doc.Root.Add(artboard);
        if (active != null && active is not GroupLayer { IsArtboard: true } && !(active is RasterLayer { IsBackground: true }) && active.Parent == Doc.Root &&
            SKRect.Intersect(active.GetContentBounds(), SKRect.Create(bounds.Left, bounds.Top, bounds.Width, bounds.Height)) != SKRect.Empty)
        {
            artboard.Add(active);
        }
        Doc.ActiveLayer = artboard;
        Doc.NotifyStructureChanged();
        // Grow the canvas if the artboard extends beyond it.
        SKRectI all = DocumentTransforms.ContentBounds(Doc);
        if (all != Doc.Bounds && (all.Left < 0 || all.Top < 0 || all.Right > Doc.Width || all.Bottom > Doc.Height))
        {
            DocumentTransforms.ApplyMatrix(Doc, SKMatrix.CreateTranslation(-Math.Min(0, all.Left), -Math.Min(0, all.Top)),
                Math.Max(Doc.Width, all.Right) - Math.Min(0, all.Left), Math.Max(Doc.Height, all.Bottom) - Math.Min(0, all.Top),
                ResampleMethod.NearestNeighbor, Host.Background, true);
        }
        PushHistory("New Artboard");
        Host.InvalidateCanvas();
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (!_creating) return;
        context.Rect(DragRect(_start, _current, false, false), OverlayContext.AccentPen);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.Combo("Size:", [("Custom", new SKSizeI(0, 0)), ("1920 × 1080 (HD)", new SKSizeI(1920, 1080)), ("1280 × 720", new SKSizeI(1280, 720)),
            ("1080 × 1080 (Square)", new SKSizeI(1080, 1080)), ("390 × 844 (Phone)", new SKSizeI(390, 844)), ("820 × 1180 (Tablet)", new SKSizeI(820, 1180)), ("595 × 842 (A4)", new SKSizeI(595, 842))],
            () => PresetSize, v => PresetSize = v, 170);
        options.ColorButton("Artboard background", () => BackgroundColor, c => BackgroundColor = c);
        options.Button("Add New Artboard", () =>
        {
            if (Doc == null) return;
            SKSizeI size = PresetSize.Width > 0 ? PresetSize : new SKSizeI(Math.Min(1000, Doc.Width), Math.Min(1000, Doc.Height));
            int right = Doc.AllLayers.OfType<GroupLayer>().Where(g => g.IsArtboard).Select(g => g.ArtboardBounds!.Value.Right).DefaultIfEmpty(-100).Max();
            CreateArtboard(SKRectI.Create(right + 100, 0, size.Width, size.Height));
        });
    }
}
