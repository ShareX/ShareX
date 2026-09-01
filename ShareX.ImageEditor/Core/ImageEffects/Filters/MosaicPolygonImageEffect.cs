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
using ShareX.ImageEditor.Core.ImageEffects.Helpers;
using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public enum MosaicPolygonShape
{
    Hexagon,
    Triangle
}

public sealed class MosaicPolygonImageEffect : ImageEffectBase
{
    public override string Id => "mosaic_polygon";
    public override string Name => "Mosaic polygon";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.hexagon;
    public override string Description => "Creates a mosaic using hexagonal or triangular polygon tiles.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<MosaicPolygonImageEffect>("cell_size", "Cell size", 8, 80, 24, (e, v) => e.CellSize = v),
        EffectParameters.Enum<MosaicPolygonImageEffect, MosaicPolygonShape>("shape", "Shape", MosaicPolygonShape.Hexagon, (e, v) => e.Shape = v,
            new (string Label, MosaicPolygonShape Value)[] { ("Hexagon", MosaicPolygonShape.Hexagon), ("Triangle", MosaicPolygonShape.Triangle) }),
        EffectParameters.FloatSlider<MosaicPolygonImageEffect>("border_width", "Border width", 0, 6, 1, (e, v) => e.BorderWidth = v),
        EffectParameters.FloatSlider<MosaicPolygonImageEffect>("border_opacity", "Border opacity", 0, 100, 45, (e, v) => e.BorderOpacity = v),
        EffectParameters.FloatSlider<MosaicPolygonImageEffect>("randomness", "Randomness", 0, 100, 18, (e, v) => e.Randomness = v),
    ];

    public int CellSize { get; set; } = 24; // 8..80
    public MosaicPolygonShape Shape { get; set; } = MosaicPolygonShape.Hexagon;
    public float BorderWidth { get; set; } = 1f; // 0..6
    public float BorderOpacity { get; set; } = 45f; // 0..100
    public float Randomness { get; set; } = 18f; // 0..100
    public int Seed { get; set; } = 9031;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int width = source.Width;
        int height = source.Height;
        if (width <= 0 || height <= 0)
        {
            return source.Copy();
        }

        int cellSize = Math.Clamp(CellSize, 8, 80);
        float borderWidth = Math.Clamp(BorderWidth, 0f, 6f);
        float borderOpacity = Math.Clamp(BorderOpacity, 0f, 100f) / 100f;
        float randomness = Math.Clamp(Randomness, 0f, 100f) / 100f;

        SKColor[] srcPixels = source.Pixels;
        SKBitmap result = new SKBitmap(width, height, source.ColorType, source.AlphaType);

        using SKCanvas canvas = new SKCanvas(result);
        canvas.Clear(SKColors.Transparent);

        using SKPaint fillPaint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        using SKPaint strokePaint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeJoin = SKStrokeJoin.Round,
            StrokeWidth = borderWidth,
            Color = new SKColor(0, 0, 0, ProceduralEffectHelper.ClampToByte(255f * borderOpacity))
        };

        if (Shape == MosaicPolygonShape.Hexagon)
        {
            DrawHexagons(canvas, fillPaint, strokePaint, srcPixels, width, height, cellSize, randomness);
        }
        else
        {
            DrawTriangles(canvas, fillPaint, strokePaint, srcPixels, width, height, cellSize, randomness);
        }

        return result;
    }

    private void DrawHexagons(
        SKCanvas canvas,
        SKPaint fillPaint,
        SKPaint strokePaint,
        SKColor[] srcPixels,
        int width,
        int height,
        int cellSize,
        float randomness)
    {
        float radius = cellSize * 0.5f;
        float vertical = 1.7320508f * radius;
        float horizontal = 1.5f * radius;
        float jitter = randomness * radius * 0.45f;

        for (int col = -1; ; col++)
        {
            float centerX = radius + (col * horizontal);
            if (centerX > width + radius)
            {
                break;
            }

            float startY = radius + ((col & 1) != 0 ? vertical * 0.5f : 0f);
            for (float centerY = startY - vertical; centerY < height + radius; centerY += vertical)
            {
                int gx = col + 4096;
                int gy = (int)MathF.Floor((centerY + vertical) / Math.Max(vertical, 0.0001f));

                float ox = ((ProceduralEffectHelper.Hash01(gx, gy, Seed ^ 17) * 2f) - 1f) * jitter;
                float oy = ((ProceduralEffectHelper.Hash01(gx, gy, Seed ^ 73) * 2f) - 1f) * jitter;
                float cx = centerX + ox;
                float cy = centerY + oy;

                SKColor color = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, cx, cy);
                fillPaint.Color = color;

                using SKPath path = CreateHexagonPath(cx, cy, radius);
                canvas.DrawPath(path, fillPaint);

                if (strokePaint.StrokeWidth > 0f && strokePaint.Color.Alpha > 0)
                {
                    canvas.DrawPath(path, strokePaint);
                }
            }
        }
    }

    private void DrawTriangles(
        SKCanvas canvas,
        SKPaint fillPaint,
        SKPaint strokePaint,
        SKColor[] srcPixels,
        int width,
        int height,
        int cellSize,
        float randomness)
    {
        float side = cellSize;
        float triHeight = side * 0.8660254f;
        float halfSide = side * 0.5f;
        float jitter = randomness * side * 0.30f;

        int rows = (int)Math.Ceiling((height / triHeight)) + 2;
        int cols = (int)Math.Ceiling((width / halfSide)) + 2;

        for (int row = -1; row < rows; row++)
        {
            float baseY = row * triHeight;
            for (int col = -1; col < cols; col++)
            {
                float baseX = col * halfSide;
                bool up = ((row + col) & 1) == 0;

                SKPoint p1;
                SKPoint p2;
                SKPoint p3;

                if (up)
                {
                    p1 = new SKPoint(baseX, baseY + triHeight);
                    p2 = new SKPoint(baseX + halfSide, baseY);
                    p3 = new SKPoint(baseX + side, baseY + triHeight);
                }
                else
                {
                    p1 = new SKPoint(baseX, baseY);
                    p2 = new SKPoint(baseX + side, baseY);
                    p3 = new SKPoint(baseX + halfSide, baseY + triHeight);
                }

                float cx = (p1.X + p2.X + p3.X) / 3f;
                float cy = (p1.Y + p2.Y + p3.Y) / 3f;

                float ox = ((ProceduralEffectHelper.Hash01(col, row, Seed ^ 173) * 2f) - 1f) * jitter;
                float oy = ((ProceduralEffectHelper.Hash01(col, row, Seed ^ 719) * 2f) - 1f) * jitter;
                cx += ox;
                cy += oy;

                SKColor color = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, cx, cy);
                fillPaint.Color = color;

                using SKPath path = new SKPath();
                path.MoveTo(p1);
                path.LineTo(p2);
                path.LineTo(p3);
                path.Close();

                canvas.DrawPath(path, fillPaint);
                if (strokePaint.StrokeWidth > 0f && strokePaint.Color.Alpha > 0)
                {
                    canvas.DrawPath(path, strokePaint);
                }
            }
        }
    }

    private static SKPath CreateHexagonPath(float centerX, float centerY, float radius)
    {
        SKPath path = new SKPath();
        for (int i = 0; i < 6; i++)
        {
            float angle = ((MathF.PI / 3f) * i) - (MathF.PI / 6f);
            float x = centerX + (MathF.Cos(angle) * radius);
            float y = centerY + (MathF.Sin(angle) * radius);

            if (i == 0)
            {
                path.MoveTo(x, y);
            }
            else
            {
                path.LineTo(x, y);
            }
        }
        path.Close();
        return path;
    }
}