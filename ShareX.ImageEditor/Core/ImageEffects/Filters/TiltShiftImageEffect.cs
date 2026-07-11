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

public enum TiltShiftMode
{
    Linear,
    Radial
}

public sealed class TiltShiftImageEffect : ImageEffectBase
{
    public override string Id => "tilt_shift";
    public override string Name => "Tilt-shift";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.focus;
    public override string Description => "Simulates a tilt-shift lens with selective focus.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.Enum<TiltShiftImageEffect, TiltShiftMode>("mode", "Mode", TiltShiftMode.Linear, (e, v) => e.Mode = v, new (string Label, TiltShiftMode Value)[]
        {
            ("Linear", TiltShiftMode.Linear),
            ("Radial", TiltShiftMode.Radial),
        }),
        EffectParameters.FloatSlider<TiltShiftImageEffect>("blur_radius", "Blur radius", 0f, 30f, 12f, (e, v) => e.BlurRadius = v),
        EffectParameters.FloatSlider<TiltShiftImageEffect>("focus_size", "Focus size", 5f, 90f, 30f, (e, v) => e.FocusSize = v),
        EffectParameters.FloatSlider<TiltShiftImageEffect>("focus_position_x", "Focus position X", 0f, 100f, 50f, (e, v) => e.FocusPositionX = v),
        EffectParameters.FloatSlider<TiltShiftImageEffect>("focus_position_y", "Focus position Y", 0f, 100f, 50f, (e, v) => e.FocusPositionY = v),
        EffectParameters.FloatSlider<TiltShiftImageEffect>("falloff", "Falloff", 1f, 60f, 24f, (e, v) => e.Falloff = v),
        EffectParameters.FloatSlider<TiltShiftImageEffect>("saturation_boost", "Saturation boost", 0f, 100f, 35f, (e, v) => e.SaturationBoost = v),
    ];

    public TiltShiftMode Mode { get; set; } = TiltShiftMode.Linear;
    public float BlurRadius { get; set; } = 12f; // 0..30
    public float FocusSize { get; set; } = 30f; // 5..90
    public float FocusPositionX { get; set; } = 50f; // 0..100
    public float FocusPositionY { get; set; } = 50f; // 0..100
    public float Falloff { get; set; } = 24f; // 1..60
    public float SaturationBoost { get; set; } = 35f; // 0..100

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int width = source.Width;
        int height = source.Height;
        if (width <= 0 || height <= 0)
        {
            return source.Copy();
        }

        float blur = Math.Clamp(BlurRadius, 0f, 30f);
        float focusSize = Math.Clamp(FocusSize, 5f, 90f) / 100f;
        float focusX = Math.Clamp(FocusPositionX, 0f, 100f) / 100f;
        float focusY = Math.Clamp(FocusPositionY, 0f, 100f) / 100f;
        float falloff = Math.Clamp(Falloff, 1f, 60f) / 100f;
        float saturation = Math.Clamp(SaturationBoost, 0f, 100f) / 100f;

        using SKBitmap blurred = blur > 0.01f ? CreateBlurred(source, blur) : source.Copy();

        SKColor[] srcPixels = source.Pixels;
        SKColor[] blurPixels = blurred.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        float cx = focusX * (width - 1);
        float cy = focusY * (height - 1);
        float minDimension = Math.Min(width, height);
        float focusHalfLinear = (height * focusSize) * 0.5f;
        float focusRadiusRadial = (minDimension * focusSize) * 0.5f;
        float falloffLinear = Math.Max(1f, height * falloff);
        float falloffRadial = Math.Max(1f, minDimension * falloff);

        for (int y = 0; y < height; y++)
        {
            int row = y * width;
            for (int x = 0; x < width; x++)
            {
                float mask = Mode == TiltShiftMode.Radial
                    ? ComputeRadialMask(x, y, cx, cy, focusRadiusRadial, falloffRadial)
                    : ComputeLinearMask(y, cy, focusHalfLinear, falloffLinear);

                SKColor src = srcPixels[row + x];
                SKColor blurColor = blurPixels[row + x];

                float r = ProceduralEffectHelper.Lerp(src.Red, blurColor.Red, mask);
                float g = ProceduralEffectHelper.Lerp(src.Green, blurColor.Green, mask);
                float b = ProceduralEffectHelper.Lerp(src.Blue, blurColor.Blue, mask);

                float localSatBoost = saturation * (1f - (mask * 0.7f));
                ApplySaturation(ref r, ref g, ref b, localSatBoost);

                dstPixels[row + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(r),
                    ProceduralEffectHelper.ClampToByte(g),
                    ProceduralEffectHelper.ClampToByte(b),
                    src.Alpha);
            }
        }

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }

    private static SKBitmap CreateBlurred(SKBitmap source, float radius)
    {
        SKBitmap blurred = new SKBitmap(source.Width, source.Height, source.ColorType, source.AlphaType);

        using SKCanvas canvas = new SKCanvas(blurred);
        using SKPaint paint = new SKPaint
        {
            IsAntialias = true,
            ImageFilter = SKImageFilter.CreateBlur(radius, radius)
        };
        canvas.DrawBitmap(source, 0, 0, paint);
        return blurred;
    }

    private static float ComputeLinearMask(float y, float centerY, float focusHalf, float falloff)
    {
        float distance = MathF.Abs(y - centerY);
        return ProceduralEffectHelper.SmoothStep(focusHalf, focusHalf + falloff, distance);
    }

    private static float ComputeRadialMask(float x, float y, float centerX, float centerY, float focusRadius, float falloff)
    {
        float dx = x - centerX;
        float dy = y - centerY;
        float distance = MathF.Sqrt((dx * dx) + (dy * dy));
        return ProceduralEffectHelper.SmoothStep(focusRadius, focusRadius + falloff, distance);
    }

    private static void ApplySaturation(ref float r, ref float g, ref float b, float amount01)
    {
        if (amount01 <= 0f)
        {
            return;
        }

        float gray = (r + g + b) / 3f;
        float factor = 1f + amount01;

        r = gray + ((r - gray) * factor);
        g = gray + ((g - gray) * factor);
        b = gray + ((b - gray) * factor);
    }
}