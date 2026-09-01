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

public sealed class NightVisionImageEffect : ImageEffectBase
{
    public override string Id => "night_vision";
    public override string Name => "Night vision";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.scan_eye;
    public override string Description => "Simulates a night vision green phosphor display effect.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<NightVisionImageEffect>("intensity", "Intensity", 0, 100, 78, (e, v) => e.Intensity = v),
        EffectParameters.FloatSlider<NightVisionImageEffect>("glow", "Glow", 0, 100, 42, (e, v) => e.Glow = v),
        EffectParameters.FloatSlider<NightVisionImageEffect>("noise", "Noise", 0, 100, 18, (e, v) => e.Noise = v),
        EffectParameters.FloatSlider<NightVisionImageEffect>("vignette", "Vignette", 0, 100, 45, (e, v) => e.Vignette = v),
        EffectParameters.FloatSlider<NightVisionImageEffect>("scanlines", "Scanlines", 0, 100, 35, (e, v) => e.Scanlines = v),
    ];

    public float Intensity { get; set; } = 78f; // 0..100
    public float Glow { get; set; } = 42f; // 0..100
    public float Noise { get; set; } = 18f; // 0..100
    public float Vignette { get; set; } = 45f; // 0..100
    public float Scanlines { get; set; } = 35f; // 0..100

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float intensity = Math.Clamp(Intensity, 0f, 100f) / 100f;
        float glow = Math.Clamp(Glow, 0f, 100f) / 100f;
        float noise = Math.Clamp(Noise, 0f, 100f) / 100f;
        float vignette = Math.Clamp(Vignette, 0f, 100f) / 100f;
        float scanlines = Math.Clamp(Scanlines, 0f, 100f) / 100f;

        if (intensity <= 0.0001f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        float centerX = (width - 1) * 0.5f;
        float centerY = (height - 1) * 0.5f;
        float maxDist = MathF.Sqrt((centerX * centerX) + (centerY * centerY));
        maxDist = MathF.Max(1f, maxDist);

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        Parallel.For(0, height, y =>
        {
            int row = y * width;

            for (int x = 0; x < width; x++)
            {
                SKColor src = srcPixels[row + x];
                float lum = GetLuminance(src) / 255f;

                float avg = lum;
                avg += GetLuminance(srcPixels[row + Math.Max(0, x - 1)]) / 255f;
                avg += GetLuminance(srcPixels[row + Math.Min(width - 1, x + 1)]) / 255f;
                avg += GetLuminance(srcPixels[(Math.Max(0, y - 1) * width) + x]) / 255f;
                avg += GetLuminance(srcPixels[(Math.Min(height - 1, y + 1) * width) + x]) / 255f;
                avg /= 5f;

                float phosphor = ProceduralEffectHelper.Lerp(lum, avg, glow * 0.45f) + (MathF.Pow(lum, 0.82f) * glow * 0.26f);
                phosphor *= 0.72f + (intensity * 0.68f);

                float dx = x - centerX;
                float dy = y - centerY;
                float dist = MathF.Sqrt((dx * dx) + (dy * dy)) / maxDist;
                float vignetteMask = 1f - (vignette * dist * dist);
                vignetteMask = ProceduralEffectHelper.Clamp01(vignetteMask) * (1.02f - (dist * 0.08f));

                float scan = 1f - (scanlines * 0.16f * (0.5f + (0.5f * MathF.Sin(y * MathF.PI))));
                float grain = ((ProceduralEffectHelper.Hash01(x, y, 1229) * 2f) - 1f) * noise * 0.12f;

                float r = ProceduralEffectHelper.Clamp01(((phosphor * 0.12f) + (grain * 0.35f)) * vignetteMask * scan);
                float g = ProceduralEffectHelper.Clamp01(((phosphor * 1.08f) + grain) * vignetteMask * scan);
                float b = ProceduralEffectHelper.Clamp01(((phosphor * 0.18f) + (grain * 0.28f)) * vignetteMask * scan);

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