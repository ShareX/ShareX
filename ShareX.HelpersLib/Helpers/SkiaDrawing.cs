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

using SkiaSharp;
using System;
using System.Drawing;
using System.Linq;

namespace ShareX.HelpersLib;

public static class SkiaDrawing
{
    public static SKRect ToSKRect(this Rectangle rectangle) => new(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
    public static SKRect ToSKRect(this RectangleF rectangle) => new(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
    public static RectangleF ToRectangleF(this SKRect rectangle) => new(rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height);
    public static SKPaint Fill(Color color) => new() { Color = color.ToSKColor(), IsAntialias = true };
    public static SKPaint Stroke(Color color, float width = 1) => new()
    {
        Color = color.ToSKColor(), Style = SKPaintStyle.Stroke, StrokeWidth = width,
        IsAntialias = true, StrokeJoin = SKStrokeJoin.Round
    };
    public static SKPaint Stroke(SKPaint brush, float width = 1)
    {
        SKPaint paint = brush.Clone();
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = width;
        paint.StrokeJoin = SKStrokeJoin.Round;
        return paint;
    }

    public static SKPaint Texture(SKBitmap bitmap, ImageTileMode mode = ImageTileMode.Tile)
    {
        SKShaderTileMode xMode = mode == ImageTileMode.Clamp ? SKShaderTileMode.Clamp : mode is ImageTileMode.TileFlipX or ImageTileMode.TileFlipXY ? SKShaderTileMode.Mirror : SKShaderTileMode.Repeat;
        SKShaderTileMode yMode = mode == ImageTileMode.Clamp ? SKShaderTileMode.Clamp : mode is ImageTileMode.TileFlipY or ImageTileMode.TileFlipXY ? SKShaderTileMode.Mirror : SKShaderTileMode.Repeat;
        return new SKPaint { Shader = SKShader.CreateBitmap(bitmap, xMode, yMode), IsAntialias = true };
    }

    public static SKPaint Gradient(Rectangle rectangle, Color first, Color second, ImageGradientMode mode)
        => Gradient(rectangle, new[] { first, second }, new[] { 0f, 1f }, mode);

    public static SKPaint Gradient(Rectangle rectangle, Color[] colors, float[] positions, ImageGradientMode mode)
    {
        if (colors.Length == 0) return Fill(Color.Transparent);
        if (colors.Length == 1) return Fill(colors[0]);
        SKPoint start = new(rectangle.Left, rectangle.Top);
        SKPoint end = mode switch
        {
            ImageGradientMode.Horizontal => new(rectangle.Right, rectangle.Top),
            ImageGradientMode.Vertical => new(rectangle.Left, rectangle.Bottom),
            ImageGradientMode.BackwardDiagonal => new(rectangle.Left, rectangle.Bottom),
            _ => new(rectangle.Right, rectangle.Bottom)
        };
        if (mode == ImageGradientMode.BackwardDiagonal) start = new(rectangle.Right, rectangle.Top);
        return new SKPaint
        {
            IsAntialias = true,
            Shader = SKShader.CreateLinearGradient(start, end, colors.Select(color => color.ToSKColor()).ToArray(),
                positions, SKShaderTileMode.Clamp)
        };
    }

    public static SKPathEffect DashEffect(ImageDashStyle style, float width) => style switch
    {
        ImageDashStyle.Dash => SKPathEffect.CreateDash(new[] { 3 * width, width }, 0),
        ImageDashStyle.Dot => SKPathEffect.CreateDash(new[] { width, width }, 0),
        ImageDashStyle.DashDot => SKPathEffect.CreateDash(new[] { 3 * width, width, width, width }, 0),
        ImageDashStyle.DashDotDot => SKPathEffect.CreateDash(new[] { 3 * width, width, width, width, width, width }, 0),
        _ => null
    };

    public static void Translate(this SKPaint paint, float x, float y)
    {
        using SKShader shader = paint.Shader;
        paint.Shader = shader.WithLocalMatrix(SKMatrix.CreateTranslation(x, y));
    }

    public static void Clear(this SKCanvas canvas, Color color) => canvas.Clear(color.ToSKColor());
    public static void FillRectangle(this SKCanvas canvas, SKPaint paint, RectangleF rectangle) => canvas.DrawRect(rectangle.ToSKRect(), paint);
    public static void FillRectangle(this SKCanvas canvas, SKPaint paint, float x, float y, float width, float height)
        => canvas.DrawRect(x, y, width, height, paint);
    public static void DrawRectangleProper(this SKCanvas canvas, SKPaint paint, RectangleF rectangle)
    {
        float inset = paint.StrokeWidth / 2;
        canvas.DrawRect(SKRect.Create(rectangle.X + inset, rectangle.Y + inset,
            Math.Max(0, rectangle.Width - paint.StrokeWidth), Math.Max(0, rectangle.Height - paint.StrokeWidth)), paint);
    }
    public static void DrawRectangleProper(this SKCanvas canvas, SKPaint paint, int x, int y, int width, int height)
        => canvas.DrawRectangleProper(paint, new RectangleF(x, y, width, height));
    public static void FillPath(this SKCanvas canvas, SKPaint paint, SKPath path) => canvas.DrawPath(path, paint);
    public static void DrawPath(this SKCanvas canvas, SKPaint paint, SKPath path) => canvas.DrawPath(path, paint);
    public static void DrawLine(this SKCanvas canvas, SKPaint paint, float x1, float y1, float x2, float y2)
        => canvas.DrawLine(x1, y1, x2, y2, paint);
    public static void DrawLine(this SKCanvas canvas, SKPaint paint, PointF first, PointF second)
        => canvas.DrawLine(first.X, first.Y, second.X, second.Y, paint);
    public static void DrawPolygon(this SKCanvas canvas, SKPaint paint, PointF[] points)
        => canvas.FillPolygon(paint, points);
    public static void FillEllipse(this SKCanvas canvas, SKPaint paint, RectangleF rectangle) => canvas.DrawOval(rectangle.ToSKRect(), paint);
    public static void FillEllipse(this SKCanvas canvas, SKPaint paint, float x, float y, float width, float height)
        => canvas.DrawOval(SKRect.Create(x, y, width, height), paint);
    public static void DrawEllipse(this SKCanvas canvas, SKPaint paint, RectangleF rectangle) => canvas.DrawOval(rectangle.ToSKRect(), paint);
    public static void DrawEllipse(this SKCanvas canvas, SKPaint paint, float x, float y, float width, float height)
        => canvas.DrawOval(SKRect.Create(x, y, width, height), paint);

    public static void DrawImage(this SKCanvas canvas, SKBitmap bitmap, float x, float y, float width, float height)
        => canvas.DrawImage(bitmap, new SKRect(0, 0, bitmap.Width, bitmap.Height), SKRect.Create(x, y, width, height));
    public static void DrawImage(this SKCanvas canvas, SKBitmap bitmap, RectangleF rectangle)
        => canvas.DrawImage(bitmap, new SKRect(0, 0, bitmap.Width, bitmap.Height), rectangle.ToSKRect());
    public static void DrawImage(this SKCanvas canvas, SKBitmap bitmap, RectangleF destination, RectangleF source)
        => canvas.DrawImage(bitmap, source.ToSKRect(), destination.ToSKRect());
    public static void DrawImage(this SKCanvas canvas, SKBitmap bitmap, Rectangle destination,
        float x, float y, float width, float height, SKPaint paint = null)
        => canvas.DrawImage(bitmap, SKRect.Create(x, y, width, height), destination.ToSKRect(), paint);
    public static void DrawImageUnscaled(this SKCanvas canvas, SKBitmap bitmap, float x, float y) => canvas.DrawBitmap(bitmap, x, y);
    public static void DrawImageUnscaled(this SKCanvas canvas, SKBitmap bitmap, Point point) => canvas.DrawBitmap(bitmap, point.X, point.Y);
    public static void DrawImage(this SKCanvas canvas, SKBitmap bitmap, Point[] points)
    {
        int save = canvas.Save();
        try
        {
            canvas.Concat(new SKMatrix((points[1].X - points[0].X) / (float)bitmap.Width,
                (points[2].X - points[0].X) / (float)bitmap.Height, points[0].X,
                (points[1].Y - points[0].Y) / (float)bitmap.Width,
                (points[2].Y - points[0].Y) / (float)bitmap.Height, points[0].Y, 0, 0, 1));
            canvas.DrawBitmap(bitmap, 0, 0);
        }
        finally { canvas.RestoreToCount(save); }
    }

    public static void AddRoundedRectangle(this SKPath path, RectangleF rectangle, float radius)
        => path.AddRoundRect(rectangle.ToSKRect(), Math.Max(0, radius), Math.Max(0, radius));
    public static void AddRoundedRectangleProper(this SKPath path, RectangleF rectangle, float radius, float penWidth = 1)
    {
        rectangle.Inflate(-penWidth / 2, -penWidth / 2);
        path.AddRoundedRectangle(rectangle, radius);
    }
    public static void FillPolygon(this SKCanvas canvas, SKPaint paint, PointF[] points)
    {
        using SKPath path = new();
        if (points.Length == 0) return;
        path.MoveTo(points[0].X, points[0].Y);
        foreach (PointF point in points.Skip(1)) path.LineTo(point.X, point.Y);
        path.Close();
        canvas.DrawPath(path, paint);
    }
    public static void FillPolygon(this SKCanvas canvas, SKPaint paint, Point[] points)
        => canvas.FillPolygon(paint, points.Select(point => new PointF(point.X, point.Y)).ToArray());
    public static void FillClosedCurve(this SKCanvas canvas, SKPaint paint, Point[] points)
        => canvas.FillClosedCurve(paint, points.Select(point => new PointF(point.X, point.Y)).ToArray());
    public static void FillClosedCurve(this SKCanvas canvas, SKPaint paint, PointF[] points)
    {
        if (points.Length < 3) { canvas.FillPolygon(paint, points); return; }
        using SKPath path = new();
        path.MoveTo(points[0].X, points[0].Y);
        for (int index = 0; index < points.Length; index++)
        {
            PointF previous = points[(index + points.Length - 1) % points.Length];
            PointF current = points[index];
            PointF next = points[(index + 1) % points.Length];
            PointF after = points[(index + 2) % points.Length];
            path.CubicTo(current.X + (next.X - previous.X) / 6, current.Y + (next.Y - previous.Y) / 6,
                next.X - (after.X - current.X) / 6, next.Y - (after.Y - current.Y) / 6, next.X, next.Y);
        }
        path.Close();
        canvas.DrawPath(path, paint);
    }

    public static SKBitmap CreateEmptyBitmap(this SKBitmap bitmap, int widthOffset = 0, int heightOffset = 0)
        => SkiaImageHelpers.CreateBitmap(bitmap.Width + widthOffset, bitmap.Height + heightOffset);

    public static Size MeasureText(string text, ImageFont settings)
    {
        using SKFont font = settings.CreateFont();
        string[] lines = text.Replace("\r", "").Split('\n');
        return new Size((int)Math.Ceiling(lines.Max(line => font.MeasureText(line))),
            (int)Math.Ceiling(lines.Length * font.Spacing));
    }

    public static void DrawText(this SKCanvas canvas, string text, PointF position, ImageFont settings,
        SKPaint paint, ImageTextRenderingMode renderingMode = ImageTextRenderingMode.AntiAliasGridFit)
    {
        using SKFont font = settings.CreateFont();
        font.Edging = renderingMode switch
        {
            ImageTextRenderingMode.SingleBitPerPixel or ImageTextRenderingMode.SingleBitPerPixelGridFit => SKFontEdging.Alias,
            ImageTextRenderingMode.ClearTypeGridFit => SKFontEdging.SubpixelAntialias,
            _ => SKFontEdging.Antialias
        };
        font.Hinting = renderingMode is ImageTextRenderingMode.SingleBitPerPixel or ImageTextRenderingMode.AntiAlias
            ? SKFontHinting.None : SKFontHinting.Normal;
        string[] lines = text.Replace("\r", "").Split('\n');
        float baseline = position.Y - font.Metrics.Ascent;
        foreach (string line in lines)
        {
            canvas.DrawText(line, position.X, baseline, font, paint);
            using SKPath decorations = TextDecorations(line, font, settings.Style, position.X, baseline);
            canvas.DrawPath(decorations, paint);
            baseline += font.Spacing;
        }
    }

    public static SKPath TextPath(string text, ImageFont settings)
    {
        using SKFont font = settings.CreateFont();
        SKPath result = new();
        float baseline = -font.Metrics.Ascent;
        foreach (string line in text.Replace("\r", "").Split('\n'))
        {
            using SKPath path = font.GetTextPath(line, new SKPoint(0, baseline));
            result.AddPath(path);
            using SKPath decorations = TextDecorations(line, font, settings.Style, 0, baseline);
            result.AddPath(decorations);
            baseline += font.Spacing;
        }
        return result;
    }
    internal static SKPath TextDecorations(string text, SKFont font, ImageFontStyle style, float x, float baseline)
    {
        SKPath path = new();
        float width = font.MeasureText(text);
        SKFontMetrics metrics = font.Metrics;
        if (style.HasFlag(ImageFontStyle.Underline))
            path.AddRect(SKRect.Create(x, baseline + (metrics.UnderlinePosition ?? font.Size / 10), width,
                metrics.UnderlineThickness ?? font.Size / 16));
        if (style.HasFlag(ImageFontStyle.Strikeout))
            path.AddRect(SKRect.Create(x, baseline + (metrics.StrikeoutPosition ?? -font.Size / 3), width,
                metrics.StrikeoutThickness ?? font.Size / 16));
        return path;
    }
}
