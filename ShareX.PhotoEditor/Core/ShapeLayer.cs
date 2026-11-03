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

namespace ShareX.PhotoEditor.Core;

public sealed class ShapeLayer : Layer
{
    public ShapeLayer(string name, VectorPath path) : base(name)
    {
        Path = path;
    }

    public override LayerKind Kind => LayerKind.Shape;

    public VectorPath Path { get; set; }

    public SKColor? FillColor { get; set; } = SKColors.Gray;

    public SKColor? StrokeColor { get; set; }

    public float StrokeWidth { get; set; } = 3;

    public StrokePosition StrokeAlignment { get; set; } = StrokePosition.Center;

    public bool Dashed { get; set; }

    public SKStrokeCap StrokeCap { get; set; } = SKStrokeCap.Butt;

    public SKStrokeJoin StrokeJoin { get; set; } = SKStrokeJoin.Miter;

    protected override Layer CreateShallowCopy() => new ShapeLayer(Name, Path.Clone())
    {
        FillColor = FillColor,
        StrokeColor = StrokeColor,
        StrokeWidth = StrokeWidth,
        StrokeAlignment = StrokeAlignment,
        Dashed = Dashed,
        StrokeCap = StrokeCap,
        StrokeJoin = StrokeJoin
    };

    public override void Translate(float dx, float dy) => Path.Offset(new SKPoint(dx, dy));

    public override SKRect GetContentBounds()
    {
        SKRect bounds = Path.GetBounds();
        float inflate = StrokeColor != null ? StrokeWidth + 2 : 1;
        bounds.Inflate(inflate, inflate);
        return bounds;
    }

    public override void RenderContent(SKCanvas canvas, RenderContext context)
    {
        using SKPath path = Path.ToSKPath();
        if (FillColor is SKColor fill)
        {
            using SKPaint paint = new() { Color = fill, IsAntialias = true, Style = SKPaintStyle.Fill };
            canvas.DrawPath(path, paint);
        }
        if (StrokeColor is SKColor stroke && StrokeWidth > 0)
        {
            float width = StrokeAlignment == StrokePosition.Center ? StrokeWidth : StrokeWidth * 2;
            using SKPaint paint = new()
            {
                Color = stroke,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = width,
                StrokeCap = StrokeCap,
                StrokeJoin = StrokeJoin,
                PathEffect = Dashed ? SKPathEffect.CreateDash([width * 2, width * 2], 0) : null
            };
            canvas.Save();
            if (StrokeAlignment == StrokePosition.Inside) canvas.ClipPath(path, SKClipOperation.Intersect, true);
            else if (StrokeAlignment == StrokePosition.Outside) canvas.ClipPath(path, SKClipOperation.Difference, true);
            canvas.DrawPath(path, paint);
            canvas.Restore();
        }
    }
}
