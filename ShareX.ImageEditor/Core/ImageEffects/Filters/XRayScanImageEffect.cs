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

public sealed class XRayScanImageEffect : ImageEffectBase
{
    public override string Id => "x_ray_scan";
    public override string Name => "X-ray scan";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.scan_eye;
    public override string Description => "Simulates an X-ray scan with edge detection and glow effects.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<XRayScanImageEffect>("contrast", "Contrast", 0, 100, 70, (e, v) => e.Contrast = v),
        EffectParameters.FloatSlider<XRayScanImageEffect>("glow", "Glow", 0, 100, 60, (e, v) => e.Glow = v),
        EffectParameters.FloatSlider<XRayScanImageEffect>("edge_boost", "Edge boost", 0, 100, 68, (e, v) => e.EdgeBoost = v),
        EffectParameters.FloatSlider<XRayScanImageEffect>("scanlines", "Scanlines", 0, 100, 40, (e, v) => e.Scanlines = v),
        EffectParameters.FloatSlider<XRayScanImageEffect>("noise", "Noise", 0, 100, 14, (e, v) => e.Noise = v)
    ];

    public float Contrast { get; set; } = 70f; // 0..100
    public float Glow { get; set; } = 60f; // 0..100
    public float EdgeBoost { get; set; } = 68f; // 0..100
    public float Scanlines { get; set; } = 40f; // 0..100
    public float Noise { get; set; } = 14f; // 0..100

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float contrast = Math.Clamp(Contrast, 0f, 100f) / 100f;
        float glow = Math.Clamp(Glow, 0f, 100f) / 100f;
        float edgeBoost = Math.Clamp(EdgeBoost, 0f, 100f) / 100f;
        float scanlines = Math.Clamp(Scanlines, 0f, 100f) / 100f;
        float noise = Math.Clamp(Noise, 0f, 100f) / 100f;

        int width = source.Width;
        int height = source.Height;
        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        Parallel.For(0, height, y =>
        {
            int row = y * width;

            for (int x = 0; x < width; x++)
            {
                SKColor src = srcPixels[row + x];
                float lum = GetLuminance(src) / 255f;

                float lumLeft = GetLuminance(srcPixels[row + Math.Max(0, x - 1)]) / 255f;
                float lumRight = GetLuminance(srcPixels[row + Math.Min(width - 1, x + 1)]) / 255f;
                float lumTop = GetLuminance(srcPixels[(Math.Max(0, y - 1) * width) + x]) / 255f;
                float lumBottom = GetLuminance(srcPixels[(Math.Min(height - 1, y + 1) * width) + x]) / 255f;

                float edge = ProceduralEffectHelper.Clamp01((MathF.Abs(lumRight - lumLeft) + MathF.Abs(lumBottom - lumTop)) * (0.85f + (edgeBoost * 1.45f)));
                float density = 1f - lum;
                density = MathF.Pow(ProceduralEffectHelper.Clamp01(density), 1.85f - (contrast * 1.25f));

                float energy = ProceduralEffectHelper.Clamp01((density * 0.48f) + (edge * 0.92f));
                float glowMask = ProceduralEffectHelper.Clamp01((edge * (0.25f + (glow * 0.95f))) + (energy * glow * 0.18f));

                float scan = 1f - (scanlines * 0.16f * (0.5f + (0.5f * MathF.Sin(y * MathF.PI))));
                float grain = ((ProceduralEffectHelper.Hash01(x, y, 8849) * 2f) - 1f) * noise * 0.08f;

                float r = ProceduralEffectHelper.Clamp01(((energy * 0.14f) + (glowMask * 0.08f) + (grain * 0.30f)) * scan);
                float g = ProceduralEffectHelper.Clamp01(((energy * 0.72f) + (glowMask * 0.16f) + (grain * 0.40f)) * scan);
                float b = ProceduralEffectHelper.Clamp01(((energy * 1.00f) + (glowMask * 0.26f) + (grain * 0.48f)) * scan);

                dstPixels[row + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(r * 255f),
                    ProceduralEffectHelper.ClampToByte(g * 255f),
                    ProceduralEffectHelper.ClampToByte(b * 255f),
                    src.Alpha);
            }
        });

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }

    private static float GetLuminance(SKColor color)
    {
        return (0.2126f * color.Red) + (0.7152f * color.Green) + (0.0722f * color.Blue);
    }
}