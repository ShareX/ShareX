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
using System.Xml.Linq;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace ShareX.Icons;

/// <summary>
/// A full-colour icon read from an SVG file. Only the small part of SVG that the legacy icon files use is
/// understood: basic shapes and paths, solid and gradient fills and strokes, group opacity and transforms.
/// <c>currentColor</c> is drawn in the Foreground the icon is drawn with.
/// </summary>
internal sealed class SvgIconDrawing
{
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";

    private readonly SvgNode[] _nodes;

    private SvgIconDrawing(double width, double height, SvgNode[] nodes)
    {
        Width = width;
        Height = height;
        _nodes = nodes;
    }

    public double Width { get; }

    public double Height { get; }

    public static SvgIconDrawing Load(Stream stream)
    {
        XElement root = XDocument.Load(stream).Root ?? throw new FormatException("The SVG file is empty.");
        double[] viewBox = ParseNumbers(root.Attribute("viewBox")?.Value ?? throw new FormatException("The SVG file has no viewBox."));

        if (viewBox.Length != 4 || viewBox[2] <= 0 || viewBox[3] <= 0)
        {
            throw new FormatException("The SVG viewBox is not valid.");
        }

        Dictionary<string, XElement> gradients = root.Descendants()
            .Where(element => element.Name == Svg + "linearGradient" || element.Name == Svg + "radialGradient")
            .Where(element => element.Attribute("id") != null)
            .ToDictionary(element => element.Attribute("id")!.Value, StringComparer.Ordinal);

        Parser parser = new(gradients);
        SvgNode[] nodes = parser.ReadChildren(root, SvgStyle.Default);
        Matrix offset = Matrix.CreateTranslation(-viewBox[0], -viewBox[1]);
        return new SvgIconDrawing(viewBox[2], viewBox[3], [new SvgGroup(offset, 1, nodes)]);
    }

    /// <summary>Draws the icon centered in the largest area of its own shape that fits <paramref name="bounds"/>.</summary>
    public void Draw(DrawingContext context, Rect bounds, IBrush currentColor)
    {
        double scale = Math.Min(bounds.Width / Width, bounds.Height / Height);
        Matrix transform = Matrix.CreateScale(scale, scale) * Matrix.CreateTranslation(
            bounds.X + (bounds.Width - Width * scale) / 2,
            bounds.Y + (bounds.Height - Height * scale) / 2);

        using (context.PushTransform(transform))
        {
            foreach (SvgNode node in _nodes)
            {
                node.Draw(context, currentColor);
            }
        }
    }

    private static double[] ParseNumbers(string text)
    {
        List<double> numbers = [];
        int i = 0;

        while (i < text.Length)
        {
            char c = text[i];

            if (char.IsWhiteSpace(c) || c == ',')
            {
                i++;
                continue;
            }

            int start = i;
            bool seenDot = false;
            bool seenExponent = false;

            if (c is '+' or '-')
            {
                i++;
            }

            while (i < text.Length)
            {
                c = text[i];

                if (char.IsDigit(c))
                {
                    i++;
                }
                else if (c == '.' && !seenDot && !seenExponent)
                {
                    seenDot = true;
                    i++;
                }
                else if ((c is 'e' or 'E') && !seenExponent)
                {
                    seenExponent = true;
                    i++;

                    if (i < text.Length && text[i] is '+' or '-')
                    {
                        i++;
                    }
                }
                else
                {
                    break;
                }
            }

            if (i == start)
            {
                throw new FormatException($"Unexpected character '{c}' in \"{text}\".");
            }

            numbers.Add(double.Parse(text.AsSpan(start, i - start), NumberStyles.Float, CultureInfo.InvariantCulture));
        }

        return [.. numbers];
    }

    private static double ParseNumber(string? text, double fallback)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return fallback;
        }

        text = text.Trim();
        bool percent = text.EndsWith('%');
        double value = double.Parse(percent ? text[..^1] : text, NumberStyles.Float, CultureInfo.InvariantCulture);
        return percent ? value / 100 : value;
    }

    private static Color ParseColor(string text, double opacity)
    {
        Color color = Color.Parse(text.Trim());
        return Color.FromArgb((byte)Math.Round(color.A * Math.Clamp(opacity, 0, 1)), color.R, color.G, color.B);
    }

    private static Matrix ParseTransform(string? text)
    {
        Matrix result = Matrix.Identity;

        if (string.IsNullOrWhiteSpace(text))
        {
            return result;
        }

        int i = 0;

        while (i < text.Length)
        {
            int open = text.IndexOf('(', i);

            if (open < 0)
            {
                break;
            }

            int close = text.IndexOf(')', open);
            string name = text[i..open].Trim(' ', ',', '\t', '\r', '\n');
            double[] a = ParseNumbers(text[(open + 1)..close]);
            Matrix m = name switch
            {
                "translate" => Matrix.CreateTranslation(a[0], a.Length > 1 ? a[1] : 0),
                "scale" => Matrix.CreateScale(a[0], a.Length > 1 ? a[1] : a[0]),
                "rotate" when a.Length >= 3 => Matrix.CreateTranslation(-a[1], -a[2]) *
                    Matrix.CreateRotation(a[0] * Math.PI / 180) * Matrix.CreateTranslation(a[1], a[2]),
                "rotate" => Matrix.CreateRotation(a[0] * Math.PI / 180),
                "matrix" => new Matrix(a[0], a[1], a[2], a[3], a[4], a[5]),
                _ => throw new FormatException($"Unsupported transform \"{name}\".")
            };

            // In SVG the transform written last applies to the shape first.
            result = m * result;
            i = close + 1;
        }

        return result;
    }

    /// <summary>The painting properties that SVG passes from a group to its children.</summary>
    private sealed record SvgStyle(
        string Fill, string Stroke, double StrokeWidth, PenLineCap LineCap, PenLineJoin LineJoin, double MiterLimit,
        double[]? Dashes, double FillOpacity, double StrokeOpacity, FillRule FillRule)
    {
        public static readonly SvgStyle Default = new("#000", "none", 1, PenLineCap.Flat, PenLineJoin.Miter, 4, null, 1, 1, FillRule.NonZero);

        public SvgStyle With(XElement element)
        {
            string? Get(string name) => element.Attribute(name)?.Value;

            return new SvgStyle(
                Get("fill") ?? Fill,
                Get("stroke") ?? Stroke,
                ParseNumber(Get("stroke-width"), StrokeWidth),
                Get("stroke-linecap") switch { "round" => PenLineCap.Round, "square" => PenLineCap.Square, "butt" => PenLineCap.Flat, _ => LineCap },
                Get("stroke-linejoin") switch { "round" => PenLineJoin.Round, "bevel" => PenLineJoin.Bevel, "miter" => PenLineJoin.Miter, _ => LineJoin },
                ParseNumber(Get("stroke-miterlimit"), MiterLimit),
                Get("stroke-dasharray") is string dashes ? (dashes.Trim() == "none" ? null : ParseNumbers(dashes)) : Dashes,
                ParseNumber(Get("fill-opacity"), FillOpacity),
                ParseNumber(Get("stroke-opacity"), StrokeOpacity),
                Get("fill-rule") switch { "evenodd" => FillRule.EvenOdd, "nonzero" => FillRule.NonZero, _ => FillRule });
        }
    }

    private sealed class Parser(Dictionary<string, XElement> gradients)
    {
        public SvgNode[] ReadChildren(XElement parent, SvgStyle style)
        {
            List<SvgNode> nodes = [];

            foreach (XElement element in parent.Elements())
            {
                if (element.Name.Namespace != Svg)
                {
                    continue;
                }

                SvgNode? node = Read(element, style);

                if (node != null)
                {
                    nodes.Add(node);
                }
            }

            return [.. nodes];
        }

        private SvgNode? Read(XElement element, SvgStyle inherited)
        {
            string name = element.Name.LocalName;

            if (name is "defs" or "linearGradient" or "radialGradient" or "title" or "desc" or "metadata")
            {
                return null;
            }

            SvgStyle style = inherited.With(element);
            Matrix transform = ParseTransform(element.Attribute("transform")?.Value);
            double opacity = ParseNumber(element.Attribute("opacity")?.Value, 1);

            if (name == "g")
            {
                return new SvgGroup(transform, opacity, ReadChildren(element, style));
            }

            Geometry? geometry = CreateGeometry(element, style.FillRule);

            if (geometry == null)
            {
                return null;
            }

            // A line has no area to fill.
            Paint fill = name == "line" ? Paint.None : CreatePaint(style.Fill, style.FillOpacity);
            Paint stroke = CreatePaint(style.Stroke, style.StrokeOpacity);
            return new SvgShape(geometry, fill, stroke, style, transform, opacity);
        }

        private static Geometry? CreateGeometry(XElement element, FillRule fillRule)
        {
            double Number(string attribute, double fallback = 0) => ParseNumber(element.Attribute(attribute)?.Value, fallback);

            switch (element.Name.LocalName)
            {
                case "path":
                    string? data = element.Attribute("d")?.Value;
                    return string.IsNullOrWhiteSpace(data) ? null : Geometry.Parse((fillRule == FillRule.EvenOdd ? "F0 " : "F1 ") + data);

                case "rect":
                    double width = Number("width"), height = Number("height");

                    if (width <= 0 || height <= 0)
                    {
                        return null;
                    }

                    double rx = Number("rx", -1), ry = Number("ry", -1);
                    rx = rx < 0 ? Math.Max(ry, 0) : rx;
                    ry = ry < 0 ? rx : ry;
                    return new RectangleGeometry(new Rect(Number("x"), Number("y"), width, height),
                        Math.Min(rx, width / 2), Math.Min(ry, height / 2));

                case "circle":
                    double r = Number("r");
                    return r <= 0 ? null : new EllipseGeometry(new Rect(Number("cx") - r, Number("cy") - r, r * 2, r * 2));

                case "ellipse":
                    double erx = Number("rx"), ery = Number("ry");
                    return erx <= 0 || ery <= 0 ? null : new EllipseGeometry(new Rect(Number("cx") - erx, Number("cy") - ery, erx * 2, ery * 2));

                case "line":
                    return CreatePolyline([Number("x1"), Number("y1"), Number("x2"), Number("y2")], closed: false, fillRule);

                case "polyline":
                case "polygon":
                    double[] points = ParseNumbers(element.Attribute("points")?.Value ?? "");
                    return points.Length < 4 ? null : CreatePolyline(points, element.Name.LocalName == "polygon", fillRule);

                default:
                    throw new FormatException($"Unsupported SVG element <{element.Name.LocalName}>.");
            }
        }

        private static Geometry CreatePolyline(double[] points, bool closed, FillRule fillRule)
        {
            StreamGeometry geometry = new();

            using (StreamGeometryContext context = geometry.Open())
            {
                context.SetFillRule(fillRule);
                context.BeginFigure(new Point(points[0], points[1]), isFilled: true);

                for (int i = 2; i + 1 < points.Length; i += 2)
                {
                    context.LineTo(new Point(points[i], points[i + 1]));
                }

                context.EndFigure(closed);
            }

            return geometry;
        }

        private Paint CreatePaint(string value, double opacity)
        {
            value = value.Trim();

            if (value == "none" || opacity <= 0)
            {
                return Paint.None;
            }

            if (value == "currentColor")
            {
                return new Paint(null, opacity, UsesCurrentColor: true);
            }

            if (value.StartsWith("url(#", StringComparison.Ordinal))
            {
                string id = value[5..value.IndexOf(')')];
                return gradients.TryGetValue(id, out XElement? gradient)
                    ? new Paint(CreateGradient(gradient, opacity), 1, UsesCurrentColor: false)
                    : Paint.None;
            }

            return new Paint(new ImmutableSolidColorBrush(ParseColor(value, opacity)), 1, UsesCurrentColor: false);
        }

        private static IBrush CreateGradient(XElement element, double opacity)
        {
            double Number(string attribute, double fallback) => ParseNumber(element.Attribute(attribute)?.Value, fallback);

            GradientStops stops = [];

            foreach (XElement stop in element.Elements(Svg + "stop"))
            {
                double offset = Math.Clamp(ParseNumber(stop.Attribute("offset")?.Value, 0), 0, 1);
                double stopOpacity = ParseNumber(stop.Attribute("stop-opacity")?.Value, 1);
                Color color = ParseColor(stop.Attribute("stop-color")?.Value ?? "#000", stopOpacity);

                // SVG clamps each offset to at least the one before it.
                if (stops.Count > 0)
                {
                    offset = Math.Max(offset, stops[^1].Offset);
                }

                stops.Add(new GradientStop(color, offset));
            }

            if (element.Name.LocalName == "linearGradient")
            {
                return new LinearGradientBrush
                {
                    GradientStops = stops,
                    Opacity = opacity,
                    StartPoint = new RelativePoint(Number("x1", 0), Number("y1", 0), RelativeUnit.Relative),
                    EndPoint = new RelativePoint(Number("x2", 1), Number("y2", 0), RelativeUnit.Relative)
                }.ToImmutable();
            }

            double cx = Number("cx", .5), cy = Number("cy", .5), r = Number("r", .5);
            return new RadialGradientBrush
            {
                GradientStops = stops,
                Opacity = opacity,
                Center = new RelativePoint(cx, cy, RelativeUnit.Relative),
                GradientOrigin = new RelativePoint(Number("fx", cx), Number("fy", cy), RelativeUnit.Relative),
                RadiusX = new RelativeScalar(r, RelativeUnit.Relative),
                RadiusY = new RelativeScalar(r, RelativeUnit.Relative)
            }.ToImmutable();
        }
    }

    /// <summary>How a shape is filled or stroked: a brush, or the Foreground at an opacity for currentColor.</summary>
    private sealed record Paint(IBrush? Brush, double Opacity, bool UsesCurrentColor)
    {
        public static readonly Paint None = new(null, 0, false);

        public bool IsNone => Brush == null && !UsesCurrentColor;

        public IBrush? Resolve(IBrush currentColor)
        {
            if (!UsesCurrentColor)
            {
                return Brush;
            }

            if (Opacity >= 1)
            {
                return currentColor;
            }

            return currentColor is ISolidColorBrush solid
                ? new ImmutableSolidColorBrush(solid.Color, solid.Opacity * Opacity)
                : currentColor;
        }
    }

    private abstract class SvgNode
    {
        public abstract void Draw(DrawingContext context, IBrush currentColor);
    }

    private sealed class SvgGroup(Matrix transform, double opacity, SvgNode[] children) : SvgNode
    {
        public override void Draw(DrawingContext context, IBrush currentColor)
        {
            using (context.PushTransform(transform))
            using (context.PushOpacity(opacity))
            {
                foreach (SvgNode child in children)
                {
                    child.Draw(context, currentColor);
                }
            }
        }
    }

    private sealed class SvgShape(Geometry geometry, Paint fill, Paint stroke, SvgStyle style, Matrix transform, double opacity) : SvgNode
    {
        private readonly IDashStyle? _dashStyle = style.Dashes is { Length: > 0 } dashes && style.StrokeWidth > 0
            ? new ImmutableDashStyle(dashes.Select(length => length / style.StrokeWidth), 0)
            : null;

        private IPen? _pen;

        public override void Draw(DrawingContext context, IBrush currentColor)
        {
            using (context.PushTransform(transform))
            using (context.PushOpacity(opacity))
            {
                if (!fill.IsNone)
                {
                    context.DrawGeometry(fill.Resolve(currentColor), null, geometry);
                }

                if (!stroke.IsNone && style.StrokeWidth > 0)
                {
                    IPen pen = stroke.UsesCurrentColor
                        ? CreatePen(stroke.Resolve(currentColor)!)
                        : _pen ??= CreatePen(stroke.Brush!);
                    context.DrawGeometry(null, pen, geometry);
                }
            }
        }

        private IPen CreatePen(IBrush brush) =>
            new Pen(brush, style.StrokeWidth, _dashStyle, style.LineCap, style.LineJoin, style.MiterLimit);
    }
}
