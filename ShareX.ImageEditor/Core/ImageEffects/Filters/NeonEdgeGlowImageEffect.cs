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

using ShareX.ImageEditor.Core.ImageEffects.Helpers;
using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using ShareX.AvaloniaUI.Theming;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public sealed class NeonEdgeGlowImageEffect : ImageEffectBase
{
    public override string Id => "neon_edge_glow";
    public override string Name => "Neon edge glow";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.zap;
    public override string Description => "Detects edges and applies a neon glow effect.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<NeonEdgeGlowImageEffect>("edge_strength", "Edge strength", 0.1, 6, 2.2, (e, v) => e.EdgeStrength = v),
        EffectParameters.IntSlider<NeonEdgeGlowImageEffect>("threshold", "Threshold", 0, 255, 36, (e, v) => e.Threshold = v),
        EffectParameters.FloatSlider<NeonEdgeGlowImageEffect>("glow_radius", "Glow radius", 0, 40, 8, (e, v) => e.GlowRadius = v),
        EffectParameters.FloatSlider<NeonEdgeGlowImageEffect>("glow_intensity", "Glow intensity", 0, 250, 120, (e, v) => e.GlowIntensity = v),
        EffectParameters.FloatSlider<NeonEdgeGlowImageEffect>("base_dim", "Base dim", 0, 100, 30, (e, v) => e.BaseDim = v),
        EffectParameters.Color<NeonEdgeGlowImageEffect>("neon_color", "Neon color", new SKColor(0, 240, 255, 255), (e, v) => e.NeonColor = v),
    ];

    public float EdgeStrength { get; set; } = 2.2f; // 0.1..6
    public int Threshold { get; set; } = 36; // 0..255
    public float GlowRadius { get; set; } = 8f; // 0..40
    public float GlowIntensity { get; set; } = 120f; // 0..250
    public float BaseDim { get; set; } = 30f; // 0..100
    public SKColor NeonColor { get; set; } = new SKColor(0, 240, 255, 255);

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float edgeStrength = Math.Clamp(EdgeStrength, 0.1f, 6f);
        int threshold = Math.Clamp(Threshold, 0, 255);
        float glowRadius = Math.Clamp(GlowRadius, 0f, 40f);
        float glowIntensity = Math.Clamp(GlowIntensity, 0f, 250f) / 100f;
        float baseDim = Math.Clamp(BaseDim, 0f, 100f) / 100f;

        if (edgeStrength <= 0f && glowIntensity <= 0f && baseDim <= 0f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        int right = width - 1;
        int bottom = height - 1;
        float neonAlphaFactor = NeonColor.Alpha / 255f;

        SKColor[] srcPixels = source.Pixels;
        byte[] luma = new byte[srcPixels.Length];
        SKColor[] edgeCorePixels = new SKColor[srcPixels.Length];
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        for (int i = 0; i < srcPixels.Length; i++)
        {
            SKColor c = srcPixels[i];
            luma[i] = (byte)(((c.Red * 77) + (c.Green * 150) + (c.Blue * 29)) >> 8);
        }

        for (int y = 0; y < height; y++)
        {
            int row = y * width;
            int y0 = Clamp(y - 1, 0, bottom);
            int y1 = y;
            int y2 = Clamp(y + 1, 0, bottom);

            int row0 = y0 * width;
            int row1 = y1 * width;
            int row2 = y2 * width;

            for (int x = 0; x < width; x++)
            {
                int x0 = Clamp(x - 1, 0, right);
                int x1 = x;
                int x2 = Clamp(x + 1, 0, right);

                int p00 = luma[row0 + x0];
                int p01 = luma[row0 + x1];
                int p02 = luma[row0 + x2];
                int p10 = luma[row1 + x0];
                int p12 = luma[row1 + x2];
                int p20 = luma[row2 + x0];
                int p21 = luma[row2 + x1];
                int p22 = luma[row2 + x2];

                int gx = (-p00 + p02) + (-2 * p10 + 2 * p12) + (-p20 + p22);
                int gy = (-p00 - 2 * p01 - p02) + (p20 + 2 * p21 + p22);

                float magnitude = MathF.Sqrt((gx * gx) + (gy * gy)) * edgeStrength;
                float edgeMask = ToEdgeMask(magnitude, threshold) * neonAlphaFactor;

                SKColor src = srcPixels[row + x];
                byte coreAlpha = ProceduralEffectHelper.ClampToByte(src.Alpha * edgeMask);

                edgeCorePixels[row + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(NeonColor.Red * edgeMask),
                    ProceduralEffectHelper.ClampToByte(NeonColor.Green * edgeMask),
                    ProceduralEffectHelper.ClampToByte(NeonColor.Blue * edgeMask),
                    coreAlpha);
            }
        }

        using SKBitmap edgeCoreBitmap = new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = edgeCorePixels
        };
        using SKBitmap glowBitmap = ApplyBlurWithClamp(edgeCoreBitmap, glowRadius);

        SKColor[] glowPixels = glowBitmap.Pixels;

        float baseFactor = 1f - baseDim;
        float coreGain = 0.85f + (0.35f * MathF.Min(glowIntensity, 1.6f));
        float glowGain = glowIntensity;

        for (int i = 0; i < srcPixels.Length; i++)
        {
            SKColor src = srcPixels[i];
            SKColor core = edgeCorePixels[i];
            SKColor glow = glowPixels[i];

            float r = (src.Red * baseFactor) + (core.Red * coreGain) + (glow.Red * glowGain);
            float g = (src.Green * baseFactor) + (core.Green * coreGain) + (glow.Green * glowGain);
            float b = (src.Blue * baseFactor) + (core.Blue * coreGain) + (glow.Blue * glowGain);

            dstPixels[i] = new SKColor(
                ProceduralEffectHelper.ClampToByte(r),
                ProceduralEffectHelper.ClampToByte(g),
                ProceduralEffectHelper.ClampToByte(b),
                src.Alpha);
        }

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }

    private static float ToEdgeMask(float magnitude, int threshold)
    {
        if (magnitude <= threshold)
        {
            return 0f;
        }

        float denom = Math.Max(1f, 255f - threshold);
        float t = (magnitude - threshold) / denom;
        t = ProceduralEffectHelper.Clamp01(t);

        // Smooth ramp for less noisy edge activation.
        return t * t * (3f - (2f * t));
    }

    private static SKBitmap ApplyBlurWithClamp(SKBitmap source, float radius)
    {
        if (radius <= 0.01f)
        {
            return source.Copy();
        }

        int padding = Math.Max(2, (int)MathF.Ceiling(radius * 2f));
        int expandedWidth = source.Width + (padding * 2);
        int expandedHeight = source.Height + (padding * 2);

        using SKBitmap expanded = new SKBitmap(expandedWidth, expandedHeight, source.ColorType, source.AlphaType);
        using (SKCanvas expandCanvas = new SKCanvas(expanded))
        {
            using SKShader shader = SKShader.CreateBitmap(
                source,
                SKShaderTileMode.Clamp,
                SKShaderTileMode.Clamp,
                SKMatrix.CreateTranslation(padding, padding));
            using SKPaint paint = new SKPaint { Shader = shader };
            expandCanvas.DrawRect(new SKRect(0, 0, expandedWidth, expandedHeight), paint);
        }

        float sigma = Math.Max(0.001f, radius / 3f);

        using SKBitmap blurredExpanded = new SKBitmap(expandedWidth, expandedHeight, source.ColorType, source.AlphaType);
        using (SKCanvas blurCanvas = new SKCanvas(blurredExpanded))
        {
            using SKPaint blurPaint = new SKPaint
            {
                ImageFilter = SKImageFilter.CreateBlur(sigma, sigma)
            };
            blurCanvas.DrawBitmap(expanded, 0, 0, blurPaint);
        }

        SKBitmap result = new SKBitmap(source.Width, source.Height, source.ColorType, source.AlphaType);
        using (SKCanvas resultCanvas = new SKCanvas(result))
        {
            resultCanvas.DrawBitmap(
                blurredExpanded,
                new SKRect(padding, padding, padding + source.Width, padding + source.Height),
                new SKRect(0, 0, source.Width, source.Height));
        }

        return result;
    }

    private static int Clamp(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}