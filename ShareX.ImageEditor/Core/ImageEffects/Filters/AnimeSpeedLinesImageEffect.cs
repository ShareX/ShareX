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

public sealed class AnimeSpeedLinesImageEffect : ImageEffectBase
{
    public override string Id => "anime_speed_lines";
    public override string Name => "Anime speed lines";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.scan_line;
    public override string Description => "Overlays radial speed lines in a manga/anime style.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<AnimeSpeedLinesImageEffect>("density", "Density", 10, 100, 70, (e, v) => e.Density = v),
        EffectParameters.FloatSlider<AnimeSpeedLinesImageEffect>("strength", "Strength", 0, 100, 65, (e, v) => e.Strength = v),
        EffectParameters.FloatSlider<AnimeSpeedLinesImageEffect>("focus_radius", "Focus radius", 0, 80, 18, (e, v) => e.FocusRadius = v),
        EffectParameters.FloatSlider<AnimeSpeedLinesImageEffect>("center_x", "Center X", 0, 100, 50, (e, v) => e.CenterX = v),
        EffectParameters.FloatSlider<AnimeSpeedLinesImageEffect>("center_y", "Center Y", 0, 100, 50, (e, v) => e.CenterY = v),
        EffectParameters.FloatSlider<AnimeSpeedLinesImageEffect>("contrast", "Contrast", 0, 100, 35, (e, v) => e.Contrast = v)
    ];

    public float Density { get; set; } = 70f; // 10..100
    public float Strength { get; set; } = 65f; // 0..100
    public float FocusRadius { get; set; } = 18f; // 0..80
    public float CenterX { get; set; } = 50f; // 0..100
    public float CenterY { get; set; } = 50f; // 0..100
    public float Contrast { get; set; } = 35f; // 0..100
    public int Seed { get; set; } = 9911;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float density = Math.Clamp(Density, 10f, 100f) / 100f;
        float strength = Math.Clamp(Strength, 0f, 100f) / 100f;
        float focusRadius = Math.Clamp(FocusRadius, 0f, 80f) / 100f;
        float contrast = Math.Clamp(Contrast, 0f, 100f) / 100f;

        if (strength <= 0.0001f)
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        float centerX = (Math.Clamp(CenterX, 0f, 100f) / 100f) * (width - 1);
        float centerY = (Math.Clamp(CenterY, 0f, 100f) / 100f) * (height - 1);
        float minDim = MathF.Min(width, height);
        float focusPx = minDim * focusRadius;
        float featherPx = MathF.Max(8f, minDim * 0.22f);
        float freq = 40f + (density * 180f);

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        Parallel.For(0, height, y =>
        {
            int row = y * width;

            for (int x = 0; x < width; x++)
            {
                SKColor src = srcPixels[row + x];
                float r = src.Red;
                float g = src.Green;
                float b = src.Blue;
                float a = src.Alpha;

                float gray = (0.2126f * r) + (0.7152f * g) + (0.0722f * b);
                float baseDesat = 0.25f + (contrast * 0.45f);
                r = ProceduralEffectHelper.Lerp(r, gray, baseDesat);
                g = ProceduralEffectHelper.Lerp(g, gray, baseDesat);
                b = ProceduralEffectHelper.Lerp(b, gray, baseDesat);

                float dx = x - centerX;
                float dy = y - centerY;
                float dist = MathF.Sqrt((dx * dx) + (dy * dy));
                float radialMask = ProceduralEffectHelper.SmoothStep(focusPx, focusPx + featherPx, dist);

                float theta = MathF.Atan2(dy, dx);
                float bandPhase = (dist * 0.018f) + (ProceduralEffectHelper.Hash01((int)(dist * 0.12f), (int)(theta * 160f), Seed) * 0.85f);
                float stripe = 0.5f + (0.5f * MathF.Sin((theta * freq) + bandPhase));

                float brightLine = MathF.Pow(stripe, 36f);
                float darkLine = MathF.Pow(1f - stripe, 30f);
                float lineMask = radialMask * strength;

                float brightMix = brightLine * lineMask * 0.92f;
                float darkMix = darkLine * lineMask * 0.58f;

                r = ProceduralEffectHelper.Lerp(r, 255f, brightMix);
                g = ProceduralEffectHelper.Lerp(g, 255f, brightMix);
                b = ProceduralEffectHelper.Lerp(b, 255f, brightMix);

                r = ProceduralEffectHelper.Lerp(r, 0f, darkMix);
                g = ProceduralEffectHelper.Lerp(g, 0f, darkMix);
                b = ProceduralEffectHelper.Lerp(b, 0f, darkMix);

                dstPixels[row + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(r),
                    ProceduralEffectHelper.ClampToByte(g),
                    ProceduralEffectHelper.ClampToByte(b),
                    ProceduralEffectHelper.ClampToByte(a));
            }
        });

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }
}