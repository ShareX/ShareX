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

public sealed class BevelImageEffect : ImageEffectBase
{
    public override string Id => "bevel";
    public override string Name => "Bevel";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.layers_2;
    public override string Description => "Adds a beveled edge effect with highlights and shadows.";

    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<BevelImageEffect>("size", "Size", 1, 40, 10, (e, v) => e.Size = v),
        EffectParameters.FloatSlider<BevelImageEffect>("strength", "Strength", 0, 100, 70, (e, v) => e.Strength = v),
        EffectParameters.FloatSlider<BevelImageEffect>("light_angle", "Light angle", 0, 360, 225, (e, v) => e.LightAngle = v),
        EffectParameters.Color<BevelImageEffect>("highlight_color", "Highlight color", new SKColor(255, 255, 255, 217), (e, v) => e.HighlightColor = v),
        EffectParameters.Color<BevelImageEffect>("shadow_color", "Shadow color", new SKColor(0, 0, 0, 176), (e, v) => e.ShadowColor = v)
    ];

    public int Size { get; set; } = 10;
    public float Strength { get; set; } = 70f; // 0..100
    public float LightAngle { get; set; } = 225f; // degrees
    public SKColor HighlightColor { get; set; } = new SKColor(255, 255, 255, 217);
    public SKColor ShadowColor { get; set; } = new SKColor(0, 0, 0, 176);

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int size = Math.Clamp(Size, 1, 40);
        float strength = Math.Clamp(Strength, 0f, 100f) / 100f;

        if (strength <= 0f || size <= 0 || (HighlightColor.Alpha == 0 && ShadowColor.Alpha == 0))
        {
            return source.Copy();
        }

        int width = source.Width;
        int height = source.Height;
        SKColor[] srcPixels = source.Pixels;
        SKColor[] dstPixels = new SKColor[srcPixels.Length];
        bool hasTransparentPixels = HasTransparentPixels(srcPixels);

        float angleRad = LightAngle * MathF.PI / 180f;
        float dirX = MathF.Cos(angleRad);
        float dirY = MathF.Sin(angleRad);
        int sampleCount = Math.Clamp(size, 1, 12);

        float weightSum = 0f;
        float[] offsets = new float[sampleCount];
        float[] weights = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (i + 1f) / sampleCount;
            offsets[i] = t * size;
            weights[i] = 1f - (t * 0.72f);
            weightSum += weights[i];
        }

        float highlightAlpha = HighlightColor.Alpha / 255f;
        float shadowAlpha = ShadowColor.Alpha / 255f;
        float[] leftWeights = BuildEdgeWeights(width, size, fromStart: true);
        float[] rightWeights = BuildEdgeWeights(width, size, fromStart: false);
        float[] topWeights = BuildEdgeWeights(height, size, fromStart: true);
        float[] bottomWeights = BuildEdgeWeights(height, size, fromStart: false);

        int[] sampleOffsetX = new int[sampleCount];
        int[] sampleOffsetY = new int[sampleCount];
        if (hasTransparentPixels)
        {
            for (int i = 0; i < sampleCount; i++)
            {
                sampleOffsetX[i] = (int)MathF.Round(dirX * offsets[i]);
                sampleOffsetY[i] = (int)MathF.Round(dirY * offsets[i]);
            }
        }

        Parallel.For(0, height, y =>
        {
            int row = y * width;
            float topWeight = topWeights[y];
            float bottomWeight = bottomWeights[y];

            for (int x = 0; x < width; x++)
            {
                SKColor original = srcPixels[row + x];
                float originalAlpha = original.Alpha / 255f;

                if (original.Alpha == 0)
                {
                    dstPixels[row + x] = original;
                    continue;
                }

                float leftWeight = leftWeights[x];
                float rightWeight = rightWeights[x];

                float boundResponse =
                    (leftWeight * -dirX) +
                    (rightWeight * dirX) +
                    (topWeight * -dirY) +
                    (bottomWeight * dirY);

                float boundPresence = ProceduralEffectHelper.Clamp01(
                    MathF.Max(MathF.Max(leftWeight, rightWeight), MathF.Max(topWeight, bottomWeight)) +
                    (MathF.Min(leftWeight + rightWeight + topWeight + bottomWeight, 1f) * 0.35f));

                float response = boundResponse * 0.95f;
                float edgePresence = boundPresence;

                if (hasTransparentPixels)
                {
                    float alphaResponse = 0f;
                    float alphaEdgePresence = 0f;

                    for (int i = 0; i < sampleCount; i++)
                    {
                        int dx = sampleOffsetX[i];
                        int dy = sampleOffsetY[i];
                        float weight = weights[i];

                        float towardLight = SampleAlpha(srcPixels, width, height, x + dx, y + dy);
                        float awayFromLight = SampleAlpha(srcPixels, width, height, x - dx, y - dy);
                        float delta = awayFromLight - towardLight;

                        alphaResponse += delta * weight;
                        alphaEdgePresence += MathF.Abs(delta) * weight;
                    }

                    alphaResponse /= weightSum;
                    alphaEdgePresence = ProceduralEffectHelper.Clamp01((alphaEdgePresence / weightSum) * 1.45f);
                    response += alphaResponse * 1.25f;
                    edgePresence = ProceduralEffectHelper.Clamp01(MathF.Max(edgePresence, alphaEdgePresence));
                }

                float highlightMix = ProceduralEffectHelper.Clamp01(MathF.Max(0f, response) * 1.55f * strength) * edgePresence * highlightAlpha * originalAlpha;
                float shadowMix = ProceduralEffectHelper.Clamp01(MathF.Max(0f, -response) * 1.55f * strength) * edgePresence * shadowAlpha * originalAlpha;

                float red = original.Red;
                float green = original.Green;
                float blue = original.Blue;

                if (shadowMix > 0f)
                {
                    red = ProceduralEffectHelper.Lerp(red, ShadowColor.Red, shadowMix);
                    green = ProceduralEffectHelper.Lerp(green, ShadowColor.Green, shadowMix);
                    blue = ProceduralEffectHelper.Lerp(blue, ShadowColor.Blue, shadowMix);
                }

                if (highlightMix > 0f)
                {
                    red = ProceduralEffectHelper.Lerp(red, HighlightColor.Red, highlightMix);
                    green = ProceduralEffectHelper.Lerp(green, HighlightColor.Green, highlightMix);
                    blue = ProceduralEffectHelper.Lerp(blue, HighlightColor.Blue, highlightMix);
                }

                float lift = (highlightMix - shadowMix) * 26f * strength;
                dstPixels[row + x] = new SKColor(
                    ProceduralEffectHelper.ClampToByte(red + lift),
                    ProceduralEffectHelper.ClampToByte(green + lift),
                    ProceduralEffectHelper.ClampToByte(blue + lift),
                    original.Alpha);
            }
        });

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = dstPixels
        };
    }

    private static float SampleAlpha(SKColor[] pixels, int width, int height, float x, float y)
    {
        int right = width - 1;
        int bottom = height - 1;

        x = Math.Clamp(x, 0f, right);
        y = Math.Clamp(y, 0f, bottom);

        int x0 = (int)MathF.Floor(x);
        int y0 = (int)MathF.Floor(y);
        int x1 = x0 < right ? x0 + 1 : right;
        int y1 = y0 < bottom ? y0 + 1 : bottom;

        float tx = x - x0;
        float ty = y - y0;

        float a00 = pixels[(y0 * width) + x0].Alpha;
        float a10 = pixels[(y0 * width) + x1].Alpha;
        float a01 = pixels[(y1 * width) + x0].Alpha;
        float a11 = pixels[(y1 * width) + x1].Alpha;

        float a0 = ProceduralEffectHelper.Lerp(a00, a10, tx);
        float a1 = ProceduralEffectHelper.Lerp(a01, a11, tx);
        return ProceduralEffectHelper.Lerp(a0, a1, ty) / 255f;
    }

    private static float[] BuildEdgeWeights(int length, int size, bool fromStart)
    {
        float[] weights = new float[length];

        for (int i = 0; i < length; i++)
        {
            int distanceToEdge = fromStart ? i : (length - 1) - i;
            if (distanceToEdge < 0 || distanceToEdge >= size)
            {
                weights[i] = 0f;
                continue;
            }

            weights[i] = 1f - ProceduralEffectHelper.SmoothStep(0f, size, distanceToEdge);
        }

        return weights;
    }

    private static bool HasTransparentPixels(SKColor[] pixels)
    {
        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].Alpha != 255)
            {
                return true;
            }
        }

        return false;
    }
}