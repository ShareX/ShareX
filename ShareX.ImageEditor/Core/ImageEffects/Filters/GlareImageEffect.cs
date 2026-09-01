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

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public sealed class GlareImageEffect : ImageEffectBase
{
    public override string Id => "glare";
    public override string Name => "Glare";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.sparkle;
    public override string Description => "Adds a bright glare/light flare at a specified position.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<GlareImageEffect>("position_x", "Position X (%)", 0, 100, 50, (e, v) => e.PositionX = v),
        EffectParameters.FloatSlider<GlareImageEffect>("position_y", "Position Y (%)", 0, 100, 30, (e, v) => e.PositionY = v),
        EffectParameters.FloatSlider<GlareImageEffect>("radius", "Radius", 5, 100, 30, (e, v) => e.Radius = v),
        EffectParameters.FloatSlider<GlareImageEffect>("intensity", "Intensity", 0, 100, 80, (e, v) => e.Intensity = v),
        EffectParameters.IntSlider<GlareImageEffect>("ray_count", "Ray count", 0, 12, 6, (e, v) => e.RayCount = v),
        EffectParameters.FloatSlider<GlareImageEffect>("ray_length", "Ray length", 0, 100, 50, (e, v) => e.RayLength = v),
        EffectParameters.Color<GlareImageEffect>("color", "Color", SKColors.White, (e, v) => e.Color = v)
    ];

    public float PositionX { get; set; } = 50f;
    public float PositionY { get; set; } = 30f;
    public float Radius { get; set; } = 30f;
    public float Intensity { get; set; } = 80f;
    public int RayCount { get; set; } = 6;
    public float RayLength { get; set; } = 50f;
    public SKColor Color { get; set; } = SKColors.White;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float intensity = Math.Clamp(Intensity, 0f, 100f) / 100f;
        if (intensity <= 0f) return source.Copy();

        int w = source.Width;
        int h = source.Height;
        float cx = w * Math.Clamp(PositionX, 0f, 100f) / 100f;
        float cy = h * Math.Clamp(PositionY, 0f, 100f) / 100f;
        float diagonal = MathF.Sqrt(w * w + h * h);
        float radiusPx = diagonal * Math.Clamp(Radius, 5f, 100f) / 200f;
        float rayLengthPx = diagonal * Math.Clamp(RayLength, 0f, 100f) / 200f;

        SKBitmap result = source.Copy();
        using SKCanvas canvas = new(result);

        // Draw main glow
        byte glareAlpha = (byte)(255 * intensity);
        SKColor centerColor = new(Color.Red, Color.Green, Color.Blue, glareAlpha);
        SKColor edgeColor = new(Color.Red, Color.Green, Color.Blue, 0);

        using (SKPaint glarePaint = new()
        {
            IsAntialias = true,
            Shader = SKShader.CreateRadialGradient(
                new SKPoint(cx, cy),
                radiusPx,
                [centerColor, new SKColor(Color.Red, Color.Green, Color.Blue, (byte)(glareAlpha * 0.4f)), edgeColor],
                [0f, 0.4f, 1f],
                SKShaderTileMode.Clamp),
            BlendMode = SKBlendMode.Screen
        })
        {
            canvas.DrawRect(0, 0, w, h, glarePaint);
        }

        // Draw rays
        int rayCount = Math.Clamp(RayCount, 0, 12);
        if (rayCount > 0 && rayLengthPx > 1f)
        {
            float angleStep = 360f / rayCount;
            float rayWidth = radiusPx * 0.15f;

            for (int i = 0; i < rayCount; i++)
            {
                float angle = i * angleStep * MathF.PI / 180f;
                float endX = cx + MathF.Cos(angle) * rayLengthPx;
                float endY = cy + MathF.Sin(angle) * rayLengthPx;

                using SKPaint rayPaint = new()
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = rayWidth,
                    StrokeCap = SKStrokeCap.Round,
                    Shader = SKShader.CreateLinearGradient(
                        new SKPoint(cx, cy),
                        new SKPoint(endX, endY),
                        [centerColor, edgeColor],
                        [0f, 1f],
                        SKShaderTileMode.Clamp),
                    BlendMode = SKBlendMode.Screen
                };

                canvas.DrawLine(cx, cy, endX, endY, rayPaint);
            }
        }

        return result;
    }
}