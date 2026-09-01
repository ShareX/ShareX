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

public sealed class SnowflakesImageEffect : ImageEffectBase
{
    public override string Id => "snowflakes";
    public override string Name => "Snowflakes";
    public override ImageEffectCategory Category => ImageEffectCategory.Drawings;
    public override string IconKey => LucideIcons.snowflake;
    public override string Description => "Scatters snowflakes over the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<SnowflakesImageEffect>("count", "Count", 10, 500, 80, (e, v) => e.Count = v),
        EffectParameters.IntSlider<SnowflakesImageEffect>("min_size", "Min size", 2, 20, 3, (e, v) => e.MinSize = v),
        EffectParameters.IntSlider<SnowflakesImageEffect>("max_size", "Max size", 4, 60, 14, (e, v) => e.MaxSize = v),
        EffectParameters.FloatSlider<SnowflakesImageEffect>("opacity", "Opacity", 0, 100, 85, (e, v) => e.Opacity = v),
        EffectParameters.Bool<SnowflakesImageEffect>("random_angle", "Random angle", true, (e, v) => e.RandomAngle = v),
        EffectParameters.Bool<SnowflakesImageEffect>("background", "Background", false, (e, v) => e.Background = v)
    ];

    public int Count { get; set; } = 80;
    public int MinSize { get; set; } = 3;
    public int MaxSize { get; set; } = 14;
    public float Opacity { get; set; } = 85f;
    public bool RandomAngle { get; set; } = true;
    public bool Background { get; set; }

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (Count <= 0) return source.Copy();

        float alpha = Math.Clamp(Opacity / 100f, 0f, 1f);
        if (alpha <= 0f) return source.Copy();

        int minSize = Math.Max(2, Math.Min(MinSize, MaxSize));
        int maxSize = Math.Max(minSize, MaxSize);

        SKBitmap result = source.Copy();
        using SKCanvas canvas = new(result);

        if (Background)
        {
            canvas.Clear(SKColors.Transparent);
        }

        for (int i = 0; i < Count; i++)
        {
            float x = Random.Shared.Next(0, source.Width);
            float y = Random.Shared.Next(0, source.Height);
            float size = Random.Shared.Next(minSize, maxSize + 1);
            float flakeAlpha = alpha * (0.5f + Random.Shared.NextSingle() * 0.5f);
            float angle = RandomAngle ? Random.Shared.NextSingle() * 360f : 0f;

            DrawSnowflake(canvas, x, y, size, flakeAlpha, angle);
        }

        if (Background)
        {
            canvas.DrawBitmap(source, 0, 0);
        }

        return result;
    }

    private static void DrawSnowflake(SKCanvas canvas, float cx, float cy, float size, float alpha, float rotation)
    {
        byte a = (byte)(255 * alpha);

        canvas.Save();
        canvas.Translate(cx, cy);
        canvas.RotateDegrees(rotation);
        canvas.Translate(-cx, -cy);

        // Draw a glow behind the snowflake
        using (SKPaint glowPaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Color = new SKColor(200, 220, 255, (byte)(a * 0.3f)),
            MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, size * 0.4f)
        })
        {
            canvas.DrawCircle(cx, cy, size * 0.6f, glowPaint);
        }

        // Draw 6 branches
        using SKPaint linePaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            Color = new SKColor(240, 248, 255, a),
            StrokeWidth = Math.Max(1f, size * 0.1f),
            StrokeCap = SKStrokeCap.Round
        };

        float halfSize = size / 2f;
        for (int b = 0; b < 6; b++)
        {
            float angle = b * 60f * MathF.PI / 180f;
            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);

            float ex = cx + cos * halfSize;
            float ey = cy + sin * halfSize;
            canvas.DrawLine(cx, cy, ex, ey, linePaint);

            // Small side branches
            if (size >= 6)
            {
                float branchLen = halfSize * 0.35f;
                float midX = cx + cos * halfSize * 0.55f;
                float midY = cy + sin * halfSize * 0.55f;

                for (int side = -1; side <= 1; side += 2)
                {
                    float sideAngle = angle + side * MathF.PI / 3f;
                    float bex = midX + MathF.Cos(sideAngle) * branchLen;
                    float bey = midY + MathF.Sin(sideAngle) * branchLen;
                    canvas.DrawLine(midX, midY, bex, bey, linePaint);
                }
            }
        }

        // Center dot
        using SKPaint dotPaint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Color = new SKColor(255, 255, 255, a)
        };
        canvas.DrawCircle(cx, cy, Math.Max(1f, size * 0.08f), dotPaint);

        canvas.Restore();
    }
}