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

public sealed class DisposableCameraImageEffect : ImageEffectBase
{
    public override string Id => "disposable_camera";
    public override string Name => "Disposable camera";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.camera;
    public override string Description => "Simulates the look of a disposable film camera with flash, grain, and warmth.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<DisposableCameraImageEffect>("flash", "Flash", 0, 100, 55, (e, v) => e.Flash = v),
        EffectParameters.FloatSlider<DisposableCameraImageEffect>("softness", "Softness", 0, 24, 8, (e, v) => e.Softness = v),
        EffectParameters.FloatSlider<DisposableCameraImageEffect>("grain", "Grain", 0, 100, 24, (e, v) => e.Grain = v),
        EffectParameters.FloatSlider<DisposableCameraImageEffect>("vignette", "Vignette", 0, 100, 34, (e, v) => e.Vignette = v),
        EffectParameters.FloatSlider<DisposableCameraImageEffect>("warmth", "Warmth", 0, 100, 42, (e, v) => e.Warmth = v)
    ];

    public float Flash { get; set; } = 55f;
    public float Softness { get; set; } = 8f;
    public float Grain { get; set; } = 24f;
    public float Vignette { get; set; } = 34f;
    public float Warmth { get; set; } = 42f;
    public int Seed { get; set; } = 1996;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float flash = Math.Clamp(Flash, 0f, 100f) / 100f;
        float softness = Math.Clamp(Softness, 0f, 24f);
        float grain = Math.Clamp(Grain, 0f, 100f) / 100f;
        float vignette = Math.Clamp(Vignette, 0f, 100f) / 100f;
        float warmth = Math.Clamp(Warmth, 0f, 100f) / 100f;

        if (flash <= 0.0001f && softness <= 0.0001f && grain <= 0.0001f && vignette <= 0.0001f && warmth <= 0.0001f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        float centerX = (width - 1) * 0.56f;
        float centerY = (height - 1) * 0.44f;
        float maxDist = MathF.Sqrt((centerX * centerX) + (centerY * centerY));
        maxDist = MathF.Max(1f, maxDist);

        SKColor[] srcPixels = source.Pixels;
        using SKBitmap? blurBitmap = softness > 0.01f ? AnalogEffectHelper.CreateBlurredClamp(source, softness) : null;
        SKColor[] blurPixels = blurBitmap?.Pixels ?? srcPixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];
        float sigmaInv = 1f / MathF.Max(0.0001f, 2f * MathF.Pow(0.22f + (flash * 0.20f), 2f));
        float softnessMix = (softness / 24f) * 0.34f;
        float contrast = 1.08f + (flash * 0.16f) + (warmth * 0.08f);

        Parallel.For(0, height, y =>
        {
            int row = y * width;
            for (int x = 0; x < width; x++)
            {
                SKColor src = srcPixels[row + x];
                SKColor soft = blurPixels[row + x];

                float r = ProceduralEffectHelper.Lerp(src.Red / 255f, soft.Red / 255f, softnessMix);
                float g = ProceduralEffectHelper.Lerp(src.Green / 255f, soft.Green / 255f, softnessMix);
                float b = ProceduralEffectHelper.Lerp(src.Blue / 255f, soft.Blue / 255f, softnessMix);

                r = AnalogEffectHelper.ApplyContrast(r, contrast);
                g = AnalogEffectHelper.ApplyContrast(g, contrast);
                b = AnalogEffectHelper.ApplyContrast(b, contrast);

                float dx = (x - centerX) / Math.Max(1, width - 1);
                float dy = (y - centerY) / Math.Max(1, height - 1);
                float dist = MathF.Sqrt((dx * dx) + (dy * dy));
                float flashMask = flash * MathF.Exp(-(dist * dist) * sigmaInv);

                r = AnalogEffectHelper.Screen(r, flashMask * (0.22f + (warmth * 0.10f)));
                g = AnalogEffectHelper.Screen(g, flashMask * 0.18f);
                b = AnalogEffectHelper.Screen(b, flashMask * (0.11f - (warmth * 0.02f)));

                if (warmth > 0.0001f)
                {
                    float warmMix = warmth * (0.14f + (flashMask * 0.30f));
                    r = ProceduralEffectHelper.Lerp(r, MathF.Min(1f, r + 0.22f), warmMix);
                    g = ProceduralEffectHelper.Lerp(g, MathF.Min(1f, g + 0.10f), warmMix * 0.75f);
                    b = ProceduralEffectHelper.Lerp(b, MathF.Max(0f, b - 0.05f), warmMix * 0.70f);
                }

                float radial = MathF.Sqrt(MathF.Pow(x - ((width - 1) * 0.5f), 2f) + MathF.Pow(y - ((height - 1) * 0.5f), 2f)) / maxDist;
                float vignetteMask = 1f - (vignette * MathF.Pow(radial, 1.85f) * 0.62f);
                vignetteMask = ProceduralEffectHelper.Clamp01(vignetteMask);

                float noise = ((ProceduralEffectHelper.Hash01(x, y, Seed) * 2f) - 1f) * grain * 0.11f;
                r = ProceduralEffectHelper.Clamp01((r * vignetteMask) + (noise * 1.02f));
                g = ProceduralEffectHelper.Clamp01((g * vignetteMask) + noise);
                b = ProceduralEffectHelper.Clamp01((b * vignetteMask) + (noise * 0.92f));

                dstPixels[row + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(r * 255f),
                    ProceduralEffectHelper.ClampToByte(g * 255f),
                    ProceduralEffectHelper.ClampToByte(b * 255f),
                    src.Alpha);
            }
        });

        return AnalogEffectHelper.CreateBitmap(source, dstPixels);
    }
}