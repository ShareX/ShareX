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

public sealed class HalftoneImageEffect : ImageEffectBase
{
    public override string Id => "halftone";
    public override string Name => "Halftone";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.circle_dot;
    public override string Description => "Converts the image into a CMYK halftone dot pattern.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<HalftoneImageEffect>("cell_size", "Cell size", 3, 24, 8, (e, v) => e.CellSize = v),
        EffectParameters.FloatSlider<HalftoneImageEffect>("dot_softness", "Dot softness", 0, 100, 18, (e, v) => e.DotSoftness = v),
        EffectParameters.FloatSlider<HalftoneImageEffect>("ink_strength", "Ink strength", 0, 100, 90, (e, v) => e.InkStrength = v),
        EffectParameters.FloatSlider<HalftoneImageEffect>("source_blend", "Source blend", 0, 100, 20, (e, v) => e.SourceBlend = v),
        EffectParameters.FloatSlider<HalftoneImageEffect>("angle_offset", "Angle offset", -45, 45, 0, (e, v) => e.AngleOffset = v)
    ];

    public int CellSize { get; set; } = 8; // 3..24
    public float DotSoftness { get; set; } = 18f; // 0..100
    public float InkStrength { get; set; } = 90f; // 0..100
    public float SourceBlend { get; set; } = 20f; // 0..100
    public float AngleOffset { get; set; } = 0f; // -45..45

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int cell = Math.Clamp(CellSize, 3, 24);
        float softness = Math.Clamp(DotSoftness, 0f, 100f) / 100f;
        float inkStrength = Math.Clamp(InkStrength, 0f, 100f) / 100f;
        float sourceBlend = Math.Clamp(SourceBlend, 0f, 100f) / 100f;
        float angleOffsetRad = Math.Clamp(AngleOffset, -45f, 45f) * (MathF.PI / 180f);

        int width = source.Width;
        int height = source.Height;
        if (width <= 0 || height <= 0)
        {
            return source.Copy();
        }

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        GetRotation(15f * (MathF.PI / 180f) + angleOffsetRad, out float cCos, out float cSin);
        GetRotation(75f * (MathF.PI / 180f) + angleOffsetRad, out float mCos, out float mSin);
        GetRotation(0f + angleOffsetRad, out float yCos, out float ySin);
        GetRotation(45f * (MathF.PI / 180f) + angleOffsetRad, out float kCos, out float kSin);

        for (int y = 0; y < height; y++)
        {
            int row = y * width;
            for (int x = 0; x < width; x++)
            {
                SKColor src = srcPixels[row + x];

                float r = src.Red / 255f;
                float g = src.Green / 255f;
                float b = src.Blue / 255f;

                RgbToCmyk(r, g, b, out float c, out float m, out float yv, out float k);

                float cDot = ComputeDotCoverage(x, y, cell, cCos, cSin, c, softness);
                float mDot = ComputeDotCoverage(x, y, cell, mCos, mSin, m, softness);
                float yDot = ComputeDotCoverage(x, y, cell, yCos, ySin, yv, softness);
                float kDot = ComputeDotCoverage(x, y, cell, kCos, kSin, k, softness);

                float outR = 1f;
                float outG = 1f;
                float outB = 1f;

                BlendInk(ref outR, ref outG, ref outB, 0f, 1f, 1f, cDot * inkStrength); // Cyan
                BlendInk(ref outR, ref outG, ref outB, 1f, 0f, 1f, mDot * inkStrength); // Magenta
                BlendInk(ref outR, ref outG, ref outB, 1f, 1f, 0f, yDot * inkStrength); // Yellow
                BlendInk(ref outR, ref outG, ref outB, 0f, 0f, 0f, kDot * inkStrength * 1.1f); // Black

                outR = ProceduralEffectHelper.Lerp(outR, r, sourceBlend);
                outG = ProceduralEffectHelper.Lerp(outG, g, sourceBlend);
                outB = ProceduralEffectHelper.Lerp(outB, b, sourceBlend);

                dstPixels[row + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(outR * 255f),
                    ProceduralEffectHelper.ClampToByte(outG * 255f),
                    ProceduralEffectHelper.ClampToByte(outB * 255f),
                    src.Alpha);
            }
        }

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }

    private static void RgbToCmyk(float r, float g, float b, out float c, out float m, out float y, out float k)
    {
        k = 1f - MathF.Max(r, MathF.Max(g, b));

        float denom = 1f - k;
        if (denom <= 0.0001f)
        {
            c = 0f;
            m = 0f;
            y = 0f;
            return;
        }

        c = (1f - r - k) / denom;
        m = (1f - g - k) / denom;
        y = (1f - b - k) / denom;

        c = Math.Clamp(c, 0f, 1f);
        m = Math.Clamp(m, 0f, 1f);
        y = Math.Clamp(y, 0f, 1f);
        k = Math.Clamp(k, 0f, 1f);
    }

    private static float ComputeDotCoverage(
        int x,
        int y,
        int cell,
        float cosAngle,
        float sinAngle,
        float intensity,
        float softness01)
    {
        if (intensity <= 0.0001f)
        {
            return 0f;
        }

        float u = ((x * cosAngle) + (y * sinAngle)) / cell;
        float v = ((-x * sinAngle) + (y * cosAngle)) / cell;

        float fu = Fract(u) - 0.5f;
        float fv = Fract(v) - 0.5f;
        float distance = MathF.Sqrt((fu * fu) + (fv * fv));

        float radius = 0.5f * MathF.Sqrt(Math.Clamp(intensity, 0f, 1f));
        float softness = 0.01f + (softness01 * 0.14f);

        float inner = Math.Max(0f, radius - softness);
        float outer = radius + softness;

        return 1f - ProceduralEffectHelper.SmoothStep(inner, outer, distance);
    }

    private static void BlendInk(ref float r, ref float g, ref float b, float inkR, float inkG, float inkB, float alpha)
    {
        float t = Math.Clamp(alpha, 0f, 1f);
        r = ProceduralEffectHelper.Lerp(r, inkR, t);
        g = ProceduralEffectHelper.Lerp(g, inkG, t);
        b = ProceduralEffectHelper.Lerp(b, inkB, t);
    }

    private static float Fract(float value)
    {
        return value - MathF.Floor(value);
    }

    private static void GetRotation(float angle, out float cosAngle, out float sinAngle)
    {
        cosAngle = MathF.Cos(angle);
        sinAngle = MathF.Sin(angle);
    }
}