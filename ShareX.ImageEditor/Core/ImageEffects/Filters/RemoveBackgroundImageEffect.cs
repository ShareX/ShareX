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

public sealed class RemoveBackgroundImageEffect : ImageEffectBase
{
    private const byte TransparentAlphaThreshold = 20;
    private const int MaxProcessDimension = 256;

    public override string Id => "remove_background";
    public override string Name => "Remove background";
    public override ImageEffectCategory Category => ImageEffectCategory.Manipulations;
    public override string IconKey => LucideIcons.scissors;
    public override string Description => "Removes border-connected background colors and turns them transparent.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<RemoveBackgroundImageEffect>("sensitivity", "Sensitivity (%)", 0, 100, 60, (effect, value) => effect.Sensitivity = value),
        EffectParameters.FloatSlider<RemoveBackgroundImageEffect>("center_protection", "Center protection (%)", 0, 100, 65, (effect, value) => effect.CenterProtection = value),
        EffectParameters.FloatSlider<RemoveBackgroundImageEffect>("edge_feather", "Edge feather (px)", 0, 24, 4, (effect, value) => effect.EdgeFeather = value, tickFrequency: 0.5, isSnapToTickEnabled: false, valueStringFormat: "{}{0:0.#}")
    ];

    public float Sensitivity { get; set; } = 60f; // 0..100
    public float CenterProtection { get; set; } = 65f; // 0..100
    public float EdgeFeather { get; set; } = 4f; // 0..24

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int width = source.Width;
        int height = source.Height;
        if (width <= 0 || height <= 0)
        {
            return source.Copy();
        }

        float sensitivity = Math.Clamp(Sensitivity, 0f, 100f) / 100f;
        float centerProtection = Math.Clamp(CenterProtection, 0f, 100f) / 100f;
        float featherRadius = Math.Clamp(EdgeFeather, 0f, 24f);

        SKColor[] sourcePixels = source.Pixels;
        if (!sourcePixels.Any(pixel => pixel.Alpha > TransparentAlphaThreshold))
        {
            return source.Copy();
        }

        float processScale = MathF.Min(1f, MaxProcessDimension / (float)Math.Max(width, height));
        int processWidth = Math.Max(1, (int)MathF.Round(width * processScale));
        int processHeight = Math.Max(1, (int)MathF.Round(height * processScale));

        using SKBitmap workingBitmap = processScale < 0.999f
            ? ResizeBitmap(source, processWidth, processHeight)
            : source.Copy();

        SKColor[] workingPixels = workingBitmap.Pixels;
        int pixelCount = processWidth * processHeight;

        PixelFeature[] features = new PixelFeature[pixelCount];
        float[] luma = new float[pixelCount];
        byte[] alpha = new byte[pixelCount];
        List<PixelFeature> borderSamples = new(Math.Max(128, pixelCount / 10));
        List<PixelFeature> centerCandidates = new(Math.Max(64, pixelCount / 7));

        int borderBand = Math.Clamp((int)MathF.Round(Math.Min(processWidth, processHeight) * 0.08f), 1, 24);
        int centerInsetX = Math.Clamp((int)MathF.Round(processWidth * 0.22f), borderBand, Math.Max(borderBand, processWidth / 2));
        int centerInsetY = Math.Clamp((int)MathF.Round(processHeight * 0.18f), borderBand, Math.Max(borderBand, processHeight / 2));
        int centerLeft = centerInsetX;
        int centerTop = centerInsetY;
        int centerRight = Math.Max(centerLeft, processWidth - centerInsetX - 1);
        int centerBottom = Math.Max(centerTop, processHeight - centerInsetY - 1);

        for (int y = 0; y < processHeight; y++)
        {
            int row = y * processWidth;
            bool isBorderRow = y < borderBand || y >= processHeight - borderBand;
            bool isCenterRow = y >= centerTop && y <= centerBottom;

            for (int x = 0; x < processWidth; x++)
            {
                int index = row + x;
                SKColor color = workingPixels[index];
                alpha[index] = color.Alpha;

                PixelFeature feature = PixelFeature.FromColor(color);
                features[index] = feature;
                luma[index] = feature.Luma;

                if (color.Alpha <= TransparentAlphaThreshold)
                {
                    continue;
                }

                bool isBorderPixel = isBorderRow || x < borderBand || x >= processWidth - borderBand;
                if (isBorderPixel)
                {
                    borderSamples.Add(feature);
                }

                if (isCenterRow && x >= centerLeft && x <= centerRight)
                {
                    centerCandidates.Add(feature);
                }
            }
        }

        if (borderSamples.Count < 12)
        {
            return source.Copy();
        }

        PixelFeature[] borderSampleArray = borderSamples.ToArray();
        PixelFeature[] backgroundPrototypes = BuildPrototypes(borderSampleArray, 4);
        PixelFeature[] foregroundSamples = SelectForegroundSamples(centerCandidates.ToArray(), backgroundPrototypes);
        PixelFeature[] foregroundPrototypes = BuildPrototypes(foregroundSamples, 3);

        float backgroundScale = DetermineAffinityScale(borderSampleArray, backgroundPrototypes, 0.045f);
        float foregroundScale = foregroundPrototypes.Length > 0
            ? DetermineAffinityScale(foregroundSamples, foregroundPrototypes, 0.055f)
            : 0.08f;

        float[] edgeMagnitude = ComputeEdgeMagnitude(luma, processWidth, processHeight);
        float[] backgroundLikelihood = new float[pixelCount];
        float[] penalty = new float[pixelCount];

        for (int y = 0; y < processHeight; y++)
        {
            int row = y * processWidth;

            for (int x = 0; x < processWidth; x++)
            {
                int index = row + x;

                if (alpha[index] <= TransparentAlphaThreshold)
                {
                    backgroundLikelihood[index] = 1f;
                    penalty[index] = 0f;
                    continue;
                }

                PixelFeature feature = features[index];

                float backgroundDistance = FindNearestDistance(feature, backgroundPrototypes);
                float foregroundDistance = foregroundPrototypes.Length > 0
                    ? FindNearestDistance(feature, foregroundPrototypes)
                    : backgroundDistance + 0.16f;

                float backgroundAffinity = MathF.Exp(-backgroundDistance / backgroundScale);
                float foregroundAffinity = foregroundPrototypes.Length > 0
                    ? MathF.Exp(-foregroundDistance / foregroundScale)
                    : 0.16f;

                float likelihood = backgroundAffinity / (backgroundAffinity + foregroundAffinity + 0.0001f);
                backgroundLikelihood[index] = likelihood;

                float centerWeight = GetCenterWeight(x, y, processWidth, processHeight);
                float alphaLift = 1f - (alpha[index] / 255f);

                float localPenalty =
                    ((1f - likelihood) * 0.72f) +
                    (edgeMagnitude[index] * 0.18f) +
                    (centerWeight * centerProtection * 0.22f) -
                    (alphaLift * 0.12f);

                penalty[index] = MathF.Max(0f, localPenalty);
            }
        }

        bool[] backgroundMask = FloodBackground(alpha, backgroundLikelihood, edgeMagnitude, penalty, processWidth, processHeight, sensitivity);
        byte[] smallMaskAlpha = new byte[pixelCount];

        for (int i = 0; i < pixelCount; i++)
        {
            smallMaskAlpha[i] = backgroundMask[i] ? (byte)0 : (byte)255;
        }

        using SKBitmap smallMaskBitmap = CreateMaskBitmap(smallMaskAlpha, processWidth, processHeight);
        using SKBitmap scaledMaskBitmap = processWidth == width && processHeight == height
            ? smallMaskBitmap.Copy()
            : ResizeBitmap(smallMaskBitmap, width, height);
        using SKBitmap finalMaskBitmap = featherRadius > 0.05f
            ? ApplyTransparentBlur(scaledMaskBitmap, featherRadius)
            : scaledMaskBitmap.Copy();

        SKColor[] maskPixels = finalMaskBitmap.Pixels;
        SKColor[] outputPixels = new SKColor[sourcePixels.Length];

        for (int i = 0; i < sourcePixels.Length; i++)
        {
            SKColor sourceColor = sourcePixels[i];
            float maskAlpha = maskPixels[i].Alpha / 255f;
            byte outputAlpha = ProceduralEffectHelper.ClampToByte(sourceColor.Alpha * maskAlpha);

            outputPixels[i] = new SKColor(
                sourceColor.Red,
                sourceColor.Green,
                sourceColor.Blue,
                outputAlpha);
        }

        return new SKBitmap(width, height, source.ColorType, source.AlphaType)
        {
            Pixels = outputPixels
        };
    }

    private static PixelFeature[] SelectForegroundSamples(PixelFeature[] candidates, PixelFeature[] backgroundPrototypes)
    {
        if (candidates.Length == 0 || backgroundPrototypes.Length == 0)
        {
            return candidates;
        }

        float[] distances = new float[candidates.Length];
        float sum = 0f;
        float max = 0f;

        for (int i = 0; i < candidates.Length; i++)
        {
            float distance = FindNearestDistance(candidates[i], backgroundPrototypes);
            distances[i] = distance;
            sum += distance;

            if (distance > max)
            {
                max = distance;
            }
        }

        float average = sum / Math.Max(1, candidates.Length);
        float cutoff = average + ((max - average) * 0.18f);

        PixelFeature[] selected = SelectByCutoff(candidates, distances, cutoff);
        if (selected.Length >= Math.Max(16, candidates.Length / 8))
        {
            return selected;
        }

        selected = SelectByCutoff(candidates, distances, average);
        return selected.Length >= 8 ? selected : candidates;
    }

    private static PixelFeature[] SelectByCutoff(PixelFeature[] candidates, float[] distances, float cutoff)
    {
        List<PixelFeature> selected = new(candidates.Length / 2);

        for (int i = 0; i < candidates.Length; i++)
        {
            if (distances[i] >= cutoff)
            {
                selected.Add(candidates[i]);
            }
        }

        return selected.ToArray();
    }

    private static PixelFeature[] BuildPrototypes(PixelFeature[] samples, int maxPrototypeCount)
    {
        if (samples.Length == 0 || maxPrototypeCount <= 0)
        {
            return Array.Empty<PixelFeature>();
        }

        int prototypeCount = Math.Min(maxPrototypeCount, samples.Length);
        PixelFeature[] prototypes = new PixelFeature[prototypeCount];
        prototypes[0] = samples[0];

        for (int i = 1; i < prototypeCount; i++)
        {
            int farthestIndex = 0;
            float farthestDistance = -1f;

            for (int sampleIndex = 0; sampleIndex < samples.Length; sampleIndex++)
            {
                float distance = FindNearestDistance(samples[sampleIndex], prototypes.AsSpan(0, i));
                if (distance > farthestDistance)
                {
                    farthestDistance = distance;
                    farthestIndex = sampleIndex;
                }
            }

            prototypes[i] = samples[farthestIndex];
        }

        for (int iteration = 0; iteration < 5; iteration++)
        {
            float[] sumLuma = new float[prototypeCount];
            float[] sumBlueDifference = new float[prototypeCount];
            float[] sumRedDifference = new float[prototypeCount];
            float[] sumSaturation = new float[prototypeCount];
            int[] counts = new int[prototypeCount];

            for (int sampleIndex = 0; sampleIndex < samples.Length; sampleIndex++)
            {
                PixelFeature sample = samples[sampleIndex];
                int nearestIndex = FindNearestIndex(sample, prototypes);

                sumLuma[nearestIndex] += sample.Luma;
                sumBlueDifference[nearestIndex] += sample.BlueDifference;
                sumRedDifference[nearestIndex] += sample.RedDifference;
                sumSaturation[nearestIndex] += sample.Saturation;
                counts[nearestIndex]++;
            }

            for (int prototypeIndex = 0; prototypeIndex < prototypeCount; prototypeIndex++)
            {
                if (counts[prototypeIndex] == 0)
                {
                    prototypes[prototypeIndex] = samples[(prototypeIndex * samples.Length) / prototypeCount];
                    continue;
                }

                float inv = 1f / counts[prototypeIndex];
                prototypes[prototypeIndex] = new PixelFeature(
                    sumLuma[prototypeIndex] * inv,
                    sumBlueDifference[prototypeIndex] * inv,
                    sumRedDifference[prototypeIndex] * inv,
                    sumSaturation[prototypeIndex] * inv);
            }
        }

        return prototypes;
    }

    private static float DetermineAffinityScale(PixelFeature[] samples, PixelFeature[] prototypes, float fallback)
    {
        if (samples.Length == 0 || prototypes.Length == 0)
        {
            return fallback;
        }

        float sum = 0f;
        for (int i = 0; i < samples.Length; i++)
        {
            sum += FindNearestDistance(samples[i], prototypes);
        }

        float average = sum / samples.Length;
        return MathF.Max(fallback, (average * 2.8f) + (fallback * 0.4f));
    }

    private static float[] ComputeEdgeMagnitude(float[] luma, int width, int height)
    {
        float[] edges = new float[luma.Length];
        int right = width - 1;
        int bottom = height - 1;

        for (int y = 0; y < height; y++)
        {
            int y0 = y > 0 ? y - 1 : 0;
            int y1 = y;
            int y2 = y < bottom ? y + 1 : bottom;

            for (int x = 0; x < width; x++)
            {
                int x0 = x > 0 ? x - 1 : 0;
                int x1 = x;
                int x2 = x < right ? x + 1 : right;

                float topLeft = luma[(y0 * width) + x0];
                float topCenter = luma[(y0 * width) + x1];
                float topRight = luma[(y0 * width) + x2];
                float middleLeft = luma[(y1 * width) + x0];
                float middleRight = luma[(y1 * width) + x2];
                float bottomLeft = luma[(y2 * width) + x0];
                float bottomCenter = luma[(y2 * width) + x1];
                float bottomRight = luma[(y2 * width) + x2];

                float gradientX = (-topLeft + topRight) + (-2f * middleLeft + (2f * middleRight)) + (-bottomLeft + bottomRight);
                float gradientY = (-topLeft - (2f * topCenter) - topRight) + (bottomLeft + (2f * bottomCenter) + bottomRight);

                float magnitude = MathF.Sqrt((gradientX * gradientX) + (gradientY * gradientY));
                edges[(y * width) + x] = Math.Clamp(magnitude / 1.35f, 0f, 1f);
            }
        }

        return edges;
    }

    private static bool[] FloodBackground(
        byte[] alpha,
        float[] backgroundLikelihood,
        float[] edgeMagnitude,
        float[] penalty,
        int width,
        int height,
        float sensitivity)
    {
        bool[] visited = new bool[penalty.Length];
        Queue<int> queue = new(Math.Max(64, penalty.Length / 8));

        float baseThreshold = 0.19f + (sensitivity * 0.28f);
        float seedThreshold = baseThreshold + 0.04f;
        float certaintyBoost = 0.04f + (sensitivity * 0.06f);

        void TrySeed(int index)
        {
            if (visited[index])
            {
                return;
            }

            if (alpha[index] <= TransparentAlphaThreshold ||
                penalty[index] <= seedThreshold ||
                backgroundLikelihood[index] >= 0.82f)
            {
                visited[index] = true;
                queue.Enqueue(index);
            }
        }

        for (int x = 0; x < width; x++)
        {
            TrySeed(x);
            TrySeed(((height - 1) * width) + x);
        }

        for (int y = 1; y < height - 1; y++)
        {
            TrySeed(y * width);
            TrySeed((y * width) + (width - 1));
        }

        while (queue.Count > 0)
        {
            int index = queue.Dequeue();
            int x = index % width;
            int y = index / width;

            for (int offsetY = -1; offsetY <= 1; offsetY++)
            {
                int neighborY = y + offsetY;
                if (neighborY < 0 || neighborY >= height)
                {
                    continue;
                }

                for (int offsetX = -1; offsetX <= 1; offsetX++)
                {
                    if (offsetX == 0 && offsetY == 0)
                    {
                        continue;
                    }

                    int neighborX = x + offsetX;
                    if (neighborX < 0 || neighborX >= width)
                    {
                        continue;
                    }

                    int neighborIndex = (neighborY * width) + neighborX;
                    if (visited[neighborIndex])
                    {
                        continue;
                    }

                    if (alpha[neighborIndex] <= TransparentAlphaThreshold)
                    {
                        visited[neighborIndex] = true;
                        queue.Enqueue(neighborIndex);
                        continue;
                    }

                    float localThreshold =
                        baseThreshold +
                        ((1f - edgeMagnitude[neighborIndex]) * 0.05f) +
                        (backgroundLikelihood[neighborIndex] * certaintyBoost);

                    if (penalty[neighborIndex] <= localThreshold)
                    {
                        visited[neighborIndex] = true;
                        queue.Enqueue(neighborIndex);
                    }
                }
            }
        }

        return visited;
    }

    private static float GetCenterWeight(int x, int y, int width, int height)
    {
        float centerX = (width - 1) * 0.5f;
        float centerY = (height - 1) * 0.5f;
        float dx = (x - centerX) / MathF.Max(1f, width * 0.5f);
        float dy = (y - centerY) / MathF.Max(1f, height * 0.5f);
        float distance = MathF.Sqrt((dx * dx) + (dy * dy));

        return 1f - ProceduralEffectHelper.SmoothStep(0.22f, 1f, distance);
    }

    private static SKBitmap CreateMaskBitmap(byte[] alphaMask, int width, int height)
    {
        SKColor[] pixels = new SKColor[alphaMask.Length];

        for (int i = 0; i < alphaMask.Length; i++)
        {
            byte alpha = alphaMask[i];
            pixels[i] = new SKColor(alpha, alpha, alpha, alpha);
        }

        return new SKBitmap(width, height)
        {
            Pixels = pixels
        };
    }

    private static SKBitmap ResizeBitmap(SKBitmap source, int width, int height)
    {
        if (source.Width == width && source.Height == height)
        {
            return source.Copy();
        }

        SKBitmap resized = new SKBitmap(width, height, source.ColorType, source.AlphaType);

        using (SKCanvas canvas = new(resized))
        using (SKPaint paint = new() { IsAntialias = true })
        {
            canvas.Clear(SKColors.Transparent);
            using SKImage sourceImage = SKImage.FromBitmap(source);
            canvas.DrawImage(sourceImage, new SKRect(0, 0, width, height), new SKSamplingOptions(SKCubicResampler.CatmullRom), paint);
        }

        return resized;
    }

    private static SKBitmap ApplyTransparentBlur(SKBitmap source, float radius)
    {
        if (radius <= 0.01f)
        {
            return source.Copy();
        }

        int padding = Math.Max(2, (int)MathF.Ceiling(radius * 2f));
        int expandedWidth = source.Width + (padding * 2);
        int expandedHeight = source.Height + (padding * 2);

        using SKBitmap expanded = new(expandedWidth, expandedHeight, source.ColorType, source.AlphaType);
        using (SKCanvas expandCanvas = new(expanded))
        {
            expandCanvas.Clear(SKColors.Transparent);
            expandCanvas.DrawBitmap(source, padding, padding);
        }

        float sigma = Math.Max(0.001f, radius / 3f);

        using SKBitmap blurredExpanded = new(expandedWidth, expandedHeight, source.ColorType, source.AlphaType);
        using (SKCanvas blurCanvas = new(blurredExpanded))
        using (SKPaint blurPaint = new() { ImageFilter = SKImageFilter.CreateBlur(sigma, sigma) })
        {
            blurCanvas.DrawBitmap(expanded, 0, 0, blurPaint);
        }

        SKBitmap result = new(source.Width, source.Height, source.ColorType, source.AlphaType);
        using (SKCanvas resultCanvas = new(result))
        {
            resultCanvas.Clear(SKColors.Transparent);
            resultCanvas.DrawBitmap(
                blurredExpanded,
                new SKRect(padding, padding, padding + source.Width, padding + source.Height),
                new SKRect(0, 0, source.Width, source.Height));
        }

        return result;
    }

    private static int FindNearestIndex(PixelFeature feature, PixelFeature[] prototypes)
    {
        int nearestIndex = 0;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < prototypes.Length; i++)
        {
            float distance = PixelFeature.DistanceSquared(feature, prototypes[i]);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestIndex = i;
            }
        }

        return nearestIndex;
    }

    private static float FindNearestDistance(PixelFeature feature, PixelFeature[] prototypes)
    {
        return FindNearestDistance(feature, prototypes.AsSpan());
    }

    private static float FindNearestDistance(PixelFeature feature, ReadOnlySpan<PixelFeature> prototypes)
    {
        if (prototypes.Length == 0)
        {
            return 1f;
        }

        float nearestDistance = float.MaxValue;

        for (int i = 0; i < prototypes.Length; i++)
        {
            float distance = PixelFeature.DistanceSquared(feature, prototypes[i]);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
            }
        }

        return MathF.Sqrt(nearestDistance);
    }

    private readonly record struct PixelFeature(float Luma, float BlueDifference, float RedDifference, float Saturation)
    {
        public static PixelFeature FromColor(SKColor color)
        {
            float r = color.Red / 255f;
            float g = color.Green / 255f;
            float b = color.Blue / 255f;

            float luma = (0.2126f * r) + (0.7152f * g) + (0.0722f * b);
            float blueDifference = (b - luma) * 1.15f;
            float redDifference = (r - luma) * 1.15f;
            float saturation = MathF.Max(r, MathF.Max(g, b)) - MathF.Min(r, MathF.Min(g, b));

            return new PixelFeature(luma, blueDifference, redDifference, saturation);
        }

        public static float DistanceSquared(PixelFeature left, PixelFeature right)
        {
            float luma = left.Luma - right.Luma;
            float blueDifference = left.BlueDifference - right.BlueDifference;
            float redDifference = left.RedDifference - right.RedDifference;
            float saturation = left.Saturation - right.Saturation;

            return (luma * luma * 0.52f) +
                   (blueDifference * blueDifference * 0.94f) +
                   (redDifference * redDifference * 0.94f) +
                   (saturation * saturation * 0.35f);
        }
    }
}