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

    /// <summary>The crop box counts as a pending operation only once it differs from the whole canvas.</summary>
    public override bool HasPendingOperation => _active && IsModified;

    private bool IsModified => Doc != null && (_angle != 0 || MathF.Abs(_size.Width - Doc.Width) >= 0.5f || MathF.Abs(_size.Height - Doc.Height) >= 0.5f ||
        MathF.Abs(_center.X - Doc.Width / 2f) >= 0.5f || MathF.Abs(_center.Y - Doc.Height / 2f) >= 0.5f);

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
