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

public sealed class CrystalPrismImageEffect : ImageEffectBase
{
    public override string Id => "crystal_prism";
    public override string Name => "Crystal prism";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.gem;
    public override string Description => "Refracts and disperses light through crystal-like facets.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<CrystalPrismImageEffect>("facetSize", "Facet size", 6, 96, 24, (e, v) => e.FacetSize = v),
        EffectParameters.FloatSlider<CrystalPrismImageEffect>("refraction", "Refraction", 0f, 30f, 8f, (e, v) => e.Refraction = v),
        EffectParameters.FloatSlider<CrystalPrismImageEffect>("dispersion", "Dispersion", 0f, 20f, 4f, (e, v) => e.Dispersion = v),
        EffectParameters.FloatSlider<CrystalPrismImageEffect>("sparkle", "Sparkle", 0f, 100f, 25f, (e, v) => e.Sparkle = v),
    ];

    public int FacetSize { get; set; } = 24; // 6..96
    public float Refraction { get; set; } = 8f; // 0..30
    public float Dispersion { get; set; } = 4f; // 0..20
    public float Sparkle { get; set; } = 25f; // 0..100
    public int Seed { get; set; } = 4771;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int facetSize = ProceduralEffectHelper.ClampInt(FacetSize, 6, 96);
        float refraction = Math.Clamp(Refraction, 0f, 30f);
        float dispersion = Math.Clamp(Dispersion, 0f, 20f);
        float sparkle = Math.Clamp(Sparkle, 0f, 100f) / 100f;

        if (refraction <= 0f && dispersion <= 0f && sparkle <= 0f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        const float twoPi = MathF.PI * 2f;
        float blendStrength = 0.58f + (0.25f * (refraction / 30f));

        for (int y = 0; y < height; y++)
        {
            int row = y * width;

            for (int x = 0; x < width; x++)
            {
                int cellX = x / facetSize;
                int cellY = y / facetSize;

                float h0 = ProceduralEffectHelper.Hash01(cellX, cellY, Seed);
                float h1 = ProceduralEffectHelper.Hash01(cellX, cellY, Seed ^ 149);
                float h2 = ProceduralEffectHelper.Hash01(cellX, cellY, Seed ^ 313);
                float h3 = ProceduralEffectHelper.Hash01(cellX, cellY, Seed ^ 341);

                float centerX = (cellX + 0.20f + (0.60f * h0)) * facetSize;
                float centerY = (cellY + 0.20f + (0.60f * h1)) * facetSize;

                float dx = x - centerX;
                float dy = y - centerY;
                float dist = MathF.Sqrt((dx * dx) + (dy * dy));
                float radial = 1f - ProceduralEffectHelper.Clamp01(dist / (facetSize * 0.85f));

                float angle = h2 * twoPi;
                float dirX = MathF.Cos(angle);
                float dirY = MathF.Sin(angle);

                float offsetMag = refraction * (0.30f + (0.70f * radial));
                float sampleX = x + (dirX * offsetMag);
                float sampleY = y + (dirY * offsetMag);

                float disp = dispersion * (0.25f + (0.75f * radial));

                SKColor cR = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, sampleX + (dirY * disp), sampleY - (dirX * disp));
                SKColor cG = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, sampleX, sampleY);
                SKColor cB = ProceduralEffectHelper.BilinearSample(srcPixels, width, height, sampleX - (dirY * disp), sampleY + (dirX * disp));

                float crystalShade = 0.88f + ((h3 - 0.5f) * 0.30f);
                float pr = (cR.Red / 255f) * crystalShade;
                float pg = (cG.Green / 255f) * crystalShade;
                float pb = (cB.Blue / 255f) * crystalShade;

                float fx = (x % facetSize) / (float)facetSize;
                float fy = (y % facetSize) / (float)facetSize;
                float border = MathF.Min(MathF.Min(fx, 1f - fx), MathF.Min(fy, 1f - fy));
                float edge = 1f - ProceduralEffectHelper.SmoothStep(0.04f, 0.28f, border);
                float sparkleMask = edge * edge * sparkle;

                pr += sparkleMask * 0.32f;
                pg += sparkleMask * 0.34f;
                pb += sparkleMask * 0.38f;

                SKColor original = srcPixels[row + x];
                float or = original.Red / 255f;
                float og = original.Green / 255f;
                float ob = original.Blue / 255f;

                float r = ProceduralEffectHelper.Lerp(or, pr, blendStrength);
                float g = ProceduralEffectHelper.Lerp(og, pg, blendStrength);
                float b = ProceduralEffectHelper.Lerp(ob, pb, blendStrength);

                dstPixels[row + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(r * 255f),
                    ProceduralEffectHelper.ClampToByte(g * 255f),
                    ProceduralEffectHelper.ClampToByte(b * 255f),
                    original.Alpha);
            }
        }

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }
}