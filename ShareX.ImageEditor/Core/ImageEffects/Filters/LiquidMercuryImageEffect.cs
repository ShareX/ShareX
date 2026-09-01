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

public sealed class LiquidMercuryImageEffect : ImageEffectBase
{
    public override string Id => "liquid_mercury";
    public override string Name => "Liquid mercury";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.droplet;
    public override string Description => "Transforms the image into a reflective liquid mercury surface.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<LiquidMercuryImageEffect>("reflection", "Reflection", 0, 100, 78, (e, v) => e.Reflection = v),
        EffectParameters.FloatSlider<LiquidMercuryImageEffect>("ripple", "Ripple", 0, 100, 42, (e, v) => e.Ripple = v),
        EffectParameters.FloatSlider<LiquidMercuryImageEffect>("shine", "Shine", 0, 100, 82, (e, v) => e.Shine = v),
        EffectParameters.FloatSlider<LiquidMercuryImageEffect>("fluidity", "Fluidity", 0, 100, 55, (e, v) => e.Fluidity = v),
        EffectParameters.FloatSlider<LiquidMercuryImageEffect>("depth", "Depth", 0, 100, 65, (e, v) => e.Depth = v)
    ];

    public float Reflection { get; set; } = 78f; // 0..100
    public float Ripple { get; set; } = 42f; // 0..100
    public float Shine { get; set; } = 82f; // 0..100
    public float Fluidity { get; set; } = 55f; // 0..100
    public float Depth { get; set; } = 65f; // 0..100

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        float reflection = Math.Clamp(Reflection, 0f, 100f) / 100f;
        float ripple = Math.Clamp(Ripple, 0f, 100f) / 100f;
        float shine = Math.Clamp(Shine, 0f, 100f) / 100f;
        float fluidity = Math.Clamp(Fluidity, 0f, 100f) / 100f;
        float depth = Math.Clamp(Depth, 0f, 100f) / 100f;

        int width = source.Width;
        int height = source.Height;
        if (width <= 0 || height <= 0)
        {
            return source.Copy();
        }

        float invWidth = 1f / Math.Max(1, width - 1);
        float invHeight = 1f / Math.Max(1, height - 1);

        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];

        Parallel.For(0, height, y =>
        {
            int row = y * width;
            float v = y * invHeight;

            for (int x = 0; x < width; x++)
            {
                SKColor src = srcPixels[row + x];
                float lum = GetLuminance(src) / 255f;

                float lumLeft = GetLuminance(srcPixels[row + Math.Max(0, x - 1)]) / 255f;
                float lumRight = GetLuminance(srcPixels[row + Math.Min(width - 1, x + 1)]) / 255f;
                float lumTop = GetLuminance(srcPixels[(Math.Max(0, y - 1) * width) + x]) / 255f;
                float lumBottom = GetLuminance(srcPixels[(Math.Min(height - 1, y + 1) * width) + x]) / 255f;

                float gradX = lumRight - lumLeft;
                float gradY = lumBottom - lumTop;

                float u = x * invWidth;
                float waveA = MathF.Sin((u * 12f) + (v * 6.5f) + 0.8f);
                float waveB = MathF.Cos((u * 7.5f) - (v * 14f) - 1.4f);
                float rippleField = ((waveA * 0.6f) + (waveB * 0.4f)) * ripple;

                float normalX = (-gradX * (1.35f + (depth * 3.8f))) + (rippleField * 0.45f);
                float normalY = (-gradY * (1.35f + (depth * 3.8f))) + (rippleField * 0.22f);
                float normalZ = 1f;
                Normalize(ref normalX, ref normalY, ref normalZ);

                float reflectU = u + (normalX * (0.55f + (fluidity * 0.55f))) + (waveB * fluidity * 0.05f);
                float reflectV = v - (normalY * (0.85f + (fluidity * 0.45f))) + (waveA * fluidity * 0.03f);

                float env = SampleEnvironment(reflectU, reflectV);
                float diffuse = ProceduralEffectHelper.Clamp01((normalZ * 0.65f) + 0.15f);
                float fresnel = MathF.Pow(1f - ProceduralEffectHelper.Clamp01(normalZ), 2.7f);
                float specular = MathF.Pow(ProceduralEffectHelper.Clamp01((normalX * -0.22f) + (normalY * -0.32f) + (normalZ * 0.92f)), 12f + (shine * 42f));

                float baseMetal = ProceduralEffectHelper.Clamp01((lum * 0.20f) + (diffuse * 0.26f) + (env * (0.32f + (reflection * 0.42f))));
                baseMetal = ProceduralEffectHelper.Clamp01(((baseMetal - 0.5f) * (1.15f + (depth * 1.55f))) + 0.5f);

                float silver = ProceduralEffectHelper.Lerp(0.06f, 0.94f, baseMetal);
                silver = ProceduralEffectHelper.Lerp(silver, 1f, specular * (0.34f + (shine * 0.46f)));
                silver = ProceduralEffectHelper.Lerp(silver, 1f, fresnel * (0.20f + (reflection * 0.22f)));

                float coolShift = 0.01f + (env * 0.03f);
                float r = ProceduralEffectHelper.Clamp01(silver - coolShift);
                float g = ProceduralEffectHelper.Clamp01(silver);
                float b = ProceduralEffectHelper.Clamp01(silver + (coolShift * 1.25f));

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

    private static void Normalize(ref float x, ref float y, ref float z)
    {
        float len = MathF.Sqrt((x * x) + (y * y) + (z * z));
        if (len <= 0.0001f)
        {
            x = 0f;
            y = 0f;
            z = 1f;
            return;
        }

        float inv = 1f / len;
        x *= inv;
        y *= inv;
        z *= inv;
    }

    private static float SampleEnvironment(float u, float v)
    {
        float waveA = MathF.Sin((u * 11.5f) + (v * 2.1f) + 0.7f);
        float waveB = MathF.Cos((u * 18.5f) - (v * 8.2f) - 1.1f);
        float waveC = MathF.Sin(((u * 0.82f) + (v * 1.28f)) * 12f + 2.6f);
        float env = (waveA * 0.42f) + (waveB * 0.36f) + (waveC * 0.22f);
        return (env * 0.5f) + 0.5f;
    }
}