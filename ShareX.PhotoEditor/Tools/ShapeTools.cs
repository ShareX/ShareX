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

using ShareX.PhotoEditor.UI;

namespace ShareX.PhotoEditor.Tools;

public enum ShapeToolMode
{
    Shape,
    Path,
    Pixels
}

public enum ShapeKind
{
    Rectangle,
    Ellipse,
    Triangle,
    Polygon,
    Line,
    Custom
}

public enum PathCombine
{
    NewLayer,
    Combine,
    Subtract,
    Intersect,
    Exclude
}

/// <summary>A named custom shape defined in a 100x100 unit box.</summary>
public sealed record CustomShape(string Name, string SvgPath)
{
    public static List<CustomShape> Defaults { get; } =
    [
        new("Heart", "M50 88 C20 66 4 48 4 30 C4 16 15 6 28 6 C38 6 46 12 50 20 C54 12 62 6 72 6 C85 6 96 16 96 30 C96 48 80 66 50 88 Z"),
        new("Star", "M50 4 L61 37 L96 37 L68 58 L79 92 L50 71 L21 92 L32 58 L4 37 L39 37 Z"),
        new("Arrow", "M4 36 L58 36 L58 14 L96 50 L58 86 L58 64 L4 64 Z"),
        new("Check Mark", "M8 54 L22 40 L40 58 L80 14 L94 28 L40 86 Z"),
        new("Speech Bubble", "M12 10 L88 10 Q96 10 96 18 L96 62 Q96 70 88 70 L44 70 L24 90 L28 70 L12 70 Q4 70 4 62 L4 18 Q4 10 12 10 Z"),
        new("Lightning", "M58 4 L18 56 L44 56 L36 96 L82 38 L54 38 Z"),
        new("Cloud", "M26 80 C12 80 4 70 4 60 C4 48 14 40 26 42 C28 26 40 16 56 16 C72 16 84 28 84 44 C92 46 96 54 96 62 C96 72 88 80 78 80 Z"),
        new("Cross", "M36 4 L64 4 L64 36 L96 36 L96 64 L64 64 L64 96 L36 96 L36 64 L4 64 L4 36 L36 36 Z"),
        new("Gear", "M44 4 L56 4 L58 16 Q64 18 68 21 L78 14 L86 22 L79 32 Q82 36 84 42 L96 44 L96 56 L84 58 Q82 64 79 68 L86 78 L78 86 L68 79 Q64 82 58 84 L56 96 L44 96 L42 84 Q36 82 32 79 L22 86 L14 78 L21 68 Q18 64 16 58 L4 56 L4 44 L16 42 Q18 36 21 32 L14 22 L22 14 L32 21 Q36 18 42 16 Z M50 34 A16 16 0 1 0 50.1 34 Z"),
        new("Crown", "M6 80 L12 26 L34 50 L50 14 L66 50 L88 26 L94 80 Z"),
        new("Droplet", "M50 4 C50 4 16 46 16 64 C16 82 31 96 50 96 C69 96 84 82 84 64 C84 46 50 4 50 4 Z"),
        new("Puzzle", "M8 30 L36 30 C32 22 34 10 46 10 C58 10 60 22 56 30 L84 30 L84 54 C92 50 96 56 96 64 C96 72 92 78 84 74 L84 96 L8 96 Z")
    ];

    public SKPath CreatePath(SKRect bounds)
    {
        SKPath path = SKPath.ParseSvgPathData(SvgPath) ?? new SKPath();
        path.Transform(SKMatrix.CreateScale(bounds.Width / 100f, bounds.Height / 100f).PostConcat(SKMatrix.CreateTranslation(bounds.Left, bounds.Top)));
        return path;
    }
}

/// <summary>Rectangle, ellipse, triangle, polygon, line and custom shape tools.</summary>
public sealed class ShapeTool : ToolBase
{
    private readonly ShapeKind _kind;
    private SKPoint _start;
    private SKPoint _current;
    private bool _dragging;
    private bool _square;
    private bool _center;

    public ShapeTool(IEditorHost host, ShapeKind kind) : base(host)
    {
        _kind = kind;
    }

    public override ToolId Id => _kind switch
    {
        ShapeKind.Rectangle => ToolId.Rectangle,
        ShapeKind.Ellipse => ToolId.Ellipse,
        ShapeKind.Triangle => ToolId.Triangle,
        ShapeKind.Polygon => ToolId.Polygon,
        ShapeKind.Line => ToolId.Line,
        _ => ToolId.CustomShape
    };

    public override string Name => _kind == ShapeKind.Custom ? "Custom Shape Tool" : $"{_kind} Tool";

    public override string Hint => _kind == ShapeKind.Line
        ? "Drag to draw a line. Shift constrains to 45 degrees."
        : "Drag to draw the shape. Shift keeps proportions, Alt draws from the center.";

    public static ShapeToolMode Mode { get; set; } = ShapeToolMode.Shape;

    public static PathCombine Combine { get; set; } = PathCombine.NewLayer;

    public static SKColor? Fill { get; set; }

    public static SKColor? Stroke { get; set; }

    public static float StrokeWidth { get; set; } = 3;

    public static StrokePosition StrokeAlign { get; set; } = StrokePosition.Center;

    public float CornerRadius { get; set; }

    public int Sides { get; set; } = 5;

    /// <summary>Star inset ratio; 100 means a regular polygon.</summary>
    public float StarRatio { get; set; } = 100;

    public float LineWeight { get; set; } = 4;

    public bool ArrowStart { get; set; }

    public bool ArrowEnd { get; set; }

    public CustomShape Custom { get; set; } = CustomShape.Defaults[0];

    public override void PointerDown(ToolInput input)
    {
        if (Doc == null || !input.LeftButton) return;
        _start = _current = input.Doc;
        _dragging = true;
    }

    public override void PointerMove(ToolInput input)
    {
        if (!_dragging) return;
        _current = _kind == ShapeKind.Line && input.Shift ? Constrain45(_start, input.Doc) : input.Doc;
        _square = input.Shift;
        _center = input.Alt;
        SKRect r = DragRect(_start, _current, _square, _center);
        Host.SetStatus(_kind == ShapeKind.Line ? $"Length: {SKPoint.Distance(_start, _current):0.0} px" : $"W: {r.Width:0} px  H: {r.Height:0} px");
        Host.InvalidateOverlay();
    }

    public override void PointerUp(ToolInput input)
    {
        if (!_dragging || Doc == null) return;
        _dragging = false;
        Host.InvalidateOverlay();
        using SKPath? path = BuildPath();
        if (path == null || path.Bounds.Width < 1 && path.Bounds.Height < 1) return;
        Apply(path);
    }

    /// <summary>Creates the geometry for the current drag.</summary>
    public SKPath? BuildPath()
    {
        SKRect r = DragRect(_start, _current, _square, _center);
        SKPath path = new();
        switch (_kind)
        {
            case ShapeKind.Rectangle:
                if (CornerRadius > 0) path.AddRoundRect(r, CornerRadius, CornerRadius);
                else path.AddRect(r);
                break;
            case ShapeKind.Ellipse:
                path.AddOval(r);
                break;
            case ShapeKind.Triangle:
            {
                SKPoint[] pts = [new(r.MidX, r.Top), new(r.Right, r.Bottom), new(r.Left, r.Bottom)];
                if (CornerRadius > 0) AddRounded(path, pts, CornerRadius);
                else path.AddPoly(pts, true);
                break;
            }
            case ShapeKind.Polygon:
            {
                int sides = Math.Clamp(Sides, 3, 100);
                bool star = StarRatio < 100;
                int count = star ? sides * 2 : sides;
                List<SKPoint> pts = [];
                for (int i = 0; i < count; i++)
                {
                    float angle = -MathF.PI / 2 + i * MathF.PI * 2 / count;
                    float scale = star && i % 2 == 1 ? StarRatio / 100f : 1;
                    pts.Add(new SKPoint(r.MidX + MathF.Cos(angle) * r.Width / 2 * scale, r.MidY + MathF.Sin(angle) * r.Height / 2 * scale));
                }
                if (CornerRadius > 0) AddRounded(path, pts, CornerRadius);
                else path.AddPoly(pts.ToArray(), true);
                break;
            }
            case ShapeKind.Line:
            {
                SKPoint a = _start, b = _current;
                SKPoint d = b - a;
                float length = d.Length;
                if (length < 0.5f) return null;
                SKPoint u = new(d.X / length, d.Y / length);
                SKPoint n = new(-u.Y * LineWeight / 2, u.X * LineWeight / 2);
                path.AddPoly([a + n, b + n, b - n, a - n], true);
                float head = Math.Max(10, LineWeight * 4);
                void Arrow(SKPoint tip, SKPoint dir)
                {
                    SKPoint back = tip - new SKPoint(dir.X * head, dir.Y * head);
                    SKPoint side = new(-dir.Y * head / 2, dir.X * head / 2);
                    path.AddPoly([tip, back + side, back - side], true);
                }
                if (ArrowEnd) Arrow(b, u);
                if (ArrowStart) Arrow(a, new SKPoint(-u.X, -u.Y));
                break;
            }
            default:
                path.AddPath(Custom.CreatePath(r));
                break;
        }
        return path;
    }

    private static void AddRounded(SKPath path, IReadOnlyList<SKPoint> pts, float radius)
    {
        int n = pts.Count;
        for (int i = 0; i < n; i++)
        {
            SKPoint prev = pts[(i - 1 + n) % n], cur = pts[i], next = pts[(i + 1) % n];
            SKPoint a = cur + Scale(prev - cur, radius), b = cur + Scale(next - cur, radius);
            if (i == 0) path.MoveTo(a);
            else path.LineTo(a);
            path.QuadTo(cur, b);
        }
        path.Close();
    }

    private static SKPoint Scale(SKPoint v, float length)
    {
        float l = Math.Max(1e-3f, v.Length);
        float k = Math.Min(length, l / 2) / l;
        return new SKPoint(v.X * k, v.Y * k);
    }

    private void Apply(SKPath path)
    {
        PhotoDocument doc = Doc!;
        switch (Mode)
        {
            case ShapeToolMode.Pixels:
            {
                if (!RequireEditableLayer(out RasterLayer layer)) return;
                using PixelSurface mask = MaskOps.FromPath(path, doc.Width, doc.Height);
                uint color = ColorMath.FromColor(Host.Foreground);
                SKRectI changed = LayerOps.CompositeMasked(doc, layer, (_, _) => color, mask, 1, LayerBlendMode.Normal);
                PushHistory(Name.Replace(" Tool", string.Empty));
                Host.InvalidateCanvas(changed);
                return;
            }
            case ShapeToolMode.Path:
            {
                doc.WorkPath ??= new VectorPath();
                VectorPath target = doc.ActivePath?.Path ?? doc.WorkPath;
                target.SubPaths.AddRange(VectorPath.FromSKPath(path).SubPaths);
                if (doc.ActivePath == null) doc.ActivePath = new NamedPath("Work Path", doc.WorkPath);
                PushHistory(Name.Replace(" Tool", string.Empty));
                Host.InvalidateOverlay();
                return;
            }
        }

        if (Combine != PathCombine.NewLayer && doc.ActiveLayer is ShapeLayer existing)
        {
            using SKPath current = existing.Path.ToSKPath();
            SKPathOp op = Combine switch
            {
                PathCombine.Subtract => SKPathOp.Difference,
                PathCombine.Intersect => SKPathOp.Intersect,
                PathCombine.Exclude => SKPathOp.Xor,
                _ => SKPathOp.Union
            };
            using SKPath? combined = current.Op(path, op);
            if (combined != null) existing.Path = VectorPath.FromSKPath(combined);
            PushHistory(Name.Replace(" Tool", string.Empty));
            Host.InvalidateCanvas();
            return;
        }

        string name = _kind == ShapeKind.Custom ? Custom.Name : _kind.ToString();
        int number = doc.AllLayers.Count(l => l.Name.StartsWith(name, StringComparison.Ordinal)) + 1;
        ShapeLayer shape = new($"{name} {number}", VectorPath.FromSKPath(path))
        {
            FillColor = _kind == ShapeKind.Line ? Stroke ?? Fill ?? Host.Foreground : Fill ?? (Stroke == null ? Host.Foreground : null),
            StrokeColor = _kind == ShapeKind.Line ? null : Stroke,
            StrokeWidth = StrokeWidth,
            StrokeAlignment = StrokeAlign
        };
        if (Fill == null && Stroke == null) shape.FillColor = Host.Foreground;
        doc.ActivePath = null;
        doc.AddLayer(shape);
        PushHistory(Name.Replace(" Tool", string.Empty));
        Host.InvalidateCanvas();
    }

    public override void DrawOverlay(OverlayContext context)
    {
        if (!_dragging) return;
        using SKPath? path = BuildPath();
        if (path != null) context.Path(path);
    }

    public override void BuildOptions(OptionsBuilder options)
    {
        options.EnumCombo(null, () => Mode, v => { Mode = v; Host.RefreshOptions(); }, 80);
        if (Mode == ShapeToolMode.Shape)
        {
            options.Label("Fill:");
            options.ColorButton("Fill color (right-click for none)", () => Fill ?? Host.Foreground, c => Fill = c, true, () => Fill == null && Stroke != null, () => Fill = null);
            options.Label("Stroke:");
            options.ColorButton("Stroke color (right-click for none)", () => Stroke ?? SKColors.Black, c => Stroke = c, true, () => Stroke == null, () => Stroke = null);
            options.Number(string.Empty, 0, 1000, () => StrokeWidth, v => StrokeWidth = (float)v, "0.#", "px", 0.5, 56);
            options.EnumCombo(null, () => StrokeAlign, v => StrokeAlign = v, 80);
            options.EnumCombo("Path op:", () => Combine, v => Combine = v, 100);
        }
        options.Separator();
        switch (_kind)
        {
            case ShapeKind.Rectangle:
            case ShapeKind.Triangle:
                options.Number("Radius:", 0, 10000, () => CornerRadius, v => CornerRadius = (float)v, "0", "px");
                break;
            case ShapeKind.Polygon:
                options.Number("Sides:", 3, 100, () => Sides, v => Sides = (int)v);
                options.Number("Star Ratio:", 1, 100, () => StarRatio, v => StarRatio = (float)v, "0", "%");
                options.Number("Radius:", 0, 10000, () => CornerRadius, v => CornerRadius = (float)v, "0", "px");
                break;
            case ShapeKind.Line:
                options.Number("Weight:", 0.5, 1000, () => LineWeight, v => LineWeight = (float)v, "0.#", "px", 0.5);
                options.Check("Start Arrow", () => ArrowStart, v => ArrowStart = v);
                options.Check("End Arrow", () => ArrowEnd, v => ArrowEnd = v);
                break;
            case ShapeKind.Custom:
                options.Combo("Shape:", CustomShape.Defaults.Select(s => (s.Name, s)), () => Custom, v => Custom = v, 140);
                break;
        }
    }
}
