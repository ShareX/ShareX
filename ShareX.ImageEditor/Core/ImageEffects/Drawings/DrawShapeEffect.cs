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

using ShareX.AvaloniaUI.Theming;
using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Drawings;

public sealed class DrawShapeEffect : ImageEffectBase
{
    public override string Id => "draw_shape";
    public override string Name => "Shape";
    public override ImageEffectCategory Category => ImageEffectCategory.Drawings;
    public override string IconKey => LucideIcons.shapes;
    public override string Description => "Draws a shape on the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.Enum<DrawShapeEffect, DrawingShapeType>(
            "shape", "Shape", DrawingShapeType.Rectangle, (e, v) => e.Shape = v,
            new (string Label, DrawingShapeType Value)[]
            {
                ("Rectangle", DrawingShapeType.Rectangle),
                ("Rounded rectangle", DrawingShapeType.RoundedRectangle),
                ("Ellipse", DrawingShapeType.Ellipse),
                ("Triangle", DrawingShapeType.Triangle),
                ("Diamond", DrawingShapeType.Diamond)
            }),
        EffectParameters.Enum<DrawShapeEffect, DrawingPlacement>(
            "placement", "Placement", DrawingPlacement.TopLeft, (e, v) => e.Placement = v,
            new (string Label, DrawingPlacement Value)[]
            {
                ("Top left", DrawingPlacement.TopLeft),
                ("Top center", DrawingPlacement.TopCenter),
                ("Top right", DrawingPlacement.TopRight),
                ("Middle left", DrawingPlacement.MiddleLeft),
                ("Middle center", DrawingPlacement.MiddleCenter),
                ("Middle right", DrawingPlacement.MiddleRight),
                ("Bottom left", DrawingPlacement.BottomLeft),
                ("Bottom center", DrawingPlacement.BottomCenter),
                ("Bottom right", DrawingPlacement.BottomRight)
            }),
        EffectParameters.IntNumeric<DrawShapeEffect>("offset_x", "Offset X", -10000, 10000, 0, (e, v) => e.Offset = new SKPointI(v, e.Offset.Y)),
        EffectParameters.IntNumeric<DrawShapeEffect>("offset_y", "Offset Y", -10000, 10000, 0, (e, v) => e.Offset = new SKPointI(e.Offset.X, v)),
        EffectParameters.IntNumeric<DrawShapeEffect>("size_width", "Size width", -1, 10000, 100, (e, v) => e.Size = new SKSizeI(v, e.Size.Height)),
        EffectParameters.IntNumeric<DrawShapeEffect>("size_height", "Size height", -1, 10000, 100, (e, v) => e.Size = new SKSizeI(e.Size.Width, v)),
        EffectParameters.Color<DrawShapeEffect>("color", "Color", new SKColor(255, 255, 255, 255), (e, v) => e.Color = v)
    ];

    public DrawingShapeType Shape { get; set; } = DrawingShapeType.Rectangle;

    public DrawingPlacement Placement { get; set; } = DrawingPlacement.TopLeft;

    public SKPointI Offset { get; set; } = new SKPointI(0, 0);

    public SKSizeI Size { get; set; } = new SKSizeI(100, 100);

    public SKColor Color { get; set; } = new SKColor(255, 255, 255, 255);

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        int width = ResolveDimension(Size.Width, source.Width);
        int height = ResolveDimension(Size.Height, source.Height);

        if (width <= 0 || height <= 0 || Color.Alpha == 0)
        {
            return source.Copy();
        }

        SKSizeI shapeSize = new SKSizeI(width, height);
        SKPointI shapePosition = DrawingEffectHelpers.GetPosition(
            Placement,
            Offset,
            new SKSizeI(source.Width, source.Height),
            shapeSize);

        SKRect shapeRect = new SKRect(
            shapePosition.X,
            shapePosition.Y,
            shapePosition.X + shapeSize.Width,
            shapePosition.Y + shapeSize.Height);

        SKBitmap result = source.Copy();
        using SKCanvas canvas = new SKCanvas(result);
        using SKPaint paint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Color = Color
        };

        DrawShape(canvas, paint, shapeRect);
        return result;
    }

    private static int ResolveDimension(int value, int fullSize)
    {
        return value == -1 ? fullSize : value;
    }

    private void DrawShape(SKCanvas canvas, SKPaint paint, SKRect rect)
    {
        switch (Shape)
        {
            case DrawingShapeType.RoundedRectangle:
                {
                    float radius = MathF.Max(1f, MathF.Min(rect.Width, rect.Height) * 0.2f);
                    canvas.DrawRoundRect(rect, radius, radius, paint);
                    break;
                }
            case DrawingShapeType.Ellipse:
                canvas.DrawOval(rect, paint);
                break;
            case DrawingShapeType.Triangle:
                using (SKPath trianglePath = CreateTrianglePath(rect))
                {
                    canvas.DrawPath(trianglePath, paint);
                }
                break;
            case DrawingShapeType.Diamond:
                using (SKPath diamondPath = CreateDiamondPath(rect))
                {
                    canvas.DrawPath(diamondPath, paint);
                }
                break;
            default:
                canvas.DrawRect(rect, paint);
                break;
        }
    }

    private static SKPath CreateTrianglePath(SKRect rect)
    {
        SKPath path = new SKPath();
        path.MoveTo(rect.MidX, rect.Top);
        path.LineTo(rect.Right, rect.Bottom);
        path.LineTo(rect.Left, rect.Bottom);
        path.Close();
        return path;
    }

    private static SKPath CreateDiamondPath(SKRect rect)
    {
        SKPath path = new SKPath();
        path.MoveTo(rect.MidX, rect.Top);
        path.LineTo(rect.Right, rect.MidY);
        path.LineTo(rect.MidX, rect.Bottom);
        path.LineTo(rect.Left, rect.MidY);
        path.Close();
        return path;
    }
}