#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.
*/

#endregion License Information (GPL v3)

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ShareX.Tools;

// Builds a frame-local adaptive palette from a bounded 5-bit RGB histogram. Palette index 255 is
// reserved for GIF transparency, leaving the remaining 255 entries available for opaque colors.
internal static class GifFrameQuantizer
{
    private const int ColorBits = 5;
    private const int ColorSideLength = 1 << ColorBits;
    private const int HistogramSideLength = ColorSideLength + 1;
    private const int HistogramLength = HistogramSideLength * HistogramSideLength * HistogramSideLength;
    private const int MaximumOpaqueColors = 255;
    private const byte TransparentPaletteIndex = 255;
    private const byte AlphaThreshold = 128;

    public static Bitmap Quantize(Bitmap source)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (source.PixelFormat != PixelFormat.Format32bppArgb)
        {
            using Bitmap copy = CreateArgbCopy(source);
            return Quantize(copy);
        }

        Rectangle bounds = new(Point.Empty, source.Size);
        ColorMoments moments = new();
        BitmapData sourceData = source.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        try
        {
            moments.AddPixels(sourceData, source.Width, source.Height);
        }
        finally
        {
            source.UnlockBits(sourceData);
        }

        Color[] paletteColors = moments.CreatePalette(MaximumOpaqueColors);
        byte[] colorLookup = CreateColorLookup(paletteColors);

        Bitmap output = new(source.Width, source.Height, PixelFormat.Format8bppIndexed);

        try
        {
            SetPalette(output, paletteColors);
            WriteIndexedPixels(source, output, colorLookup);
            return output;
        }
        catch
        {
            output.Dispose();
            throw;
        }
    }

    private static Bitmap CreateArgbCopy(Bitmap source)
    {
        Bitmap copy = new(source.Width, source.Height, PixelFormat.Format32bppArgb);

        using Graphics graphics = Graphics.FromImage(copy);
        graphics.CompositingMode = CompositingMode.SourceCopy;
        graphics.DrawImageUnscaled(source, Point.Empty);
        return copy;
    }

    private static void SetPalette(Bitmap bitmap, IReadOnlyList<Color> colors)
    {
        ColorPalette palette = bitmap.Palette;

        for (int index = 0; index < colors.Count; index++)
        {
            palette.Entries[index] = colors[index];
        }

        for (int index = colors.Count; index < TransparentPaletteIndex; index++)
        {
            palette.Entries[index] = Color.Black;
        }

        palette.Entries[TransparentPaletteIndex] = Color.Transparent;
        bitmap.Palette = palette;
    }

    private static byte[] CreateColorLookup(IReadOnlyList<Color> palette)
    {
        byte[] lookup = new byte[ColorSideLength * ColorSideLength * ColorSideLength];

        for (int red = 0; red < ColorSideLength; red++)
        {
            int redValue = Math.Min(255, (red << (8 - ColorBits)) + 4);

            for (int green = 0; green < ColorSideLength; green++)
            {
                int greenValue = Math.Min(255, (green << (8 - ColorBits)) + 4);

                for (int blue = 0; blue < ColorSideLength; blue++)
                {
                    int blueValue = Math.Min(255, (blue << (8 - ColorBits)) + 4);
                    int bestDistance = int.MaxValue;
                    int bestIndex = 0;

                    for (int paletteIndex = 0; paletteIndex < palette.Count; paletteIndex++)
                    {
                        Color color = palette[paletteIndex];
                        int redDifference = redValue - color.R;
                        int greenDifference = greenValue - color.G;
                        int blueDifference = blueValue - color.B;
                        int redMean = (redValue + color.R) / 2;
                        int redWeight = 2 + redMean / 128;
                        int blueWeight = 2 + (255 - redMean) / 128;
                        int distance = redWeight * redDifference * redDifference +
                            4 * greenDifference * greenDifference + blueWeight * blueDifference * blueDifference;

                        if (distance < bestDistance)
                        {
                            bestDistance = distance;
                            bestIndex = paletteIndex;
                        }
                    }

                    lookup[GetLookupIndex(red, green, blue)] = (byte)bestIndex;
                }
            }
        }

        return lookup;
    }

    private static unsafe void WriteIndexedPixels(Bitmap source, Bitmap output, byte[] colorLookup)
    {
        Rectangle bounds = new(Point.Empty, source.Size);
        BitmapData sourceData = source.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        BitmapData outputData = output.LockBits(bounds, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

        try
        {
            for (int y = 0; y < source.Height; y++)
            {
                byte* sourceRow = (byte*)sourceData.Scan0 + y * sourceData.Stride;
                byte* outputRow = (byte*)outputData.Scan0 + y * outputData.Stride;

                for (int x = 0; x < source.Width; x++)
                {
                    byte* pixel = sourceRow + x * 4;

                    if (pixel[3] < AlphaThreshold)
                    {
                        outputRow[x] = TransparentPaletteIndex;
                        continue;
                    }

                    outputRow[x] = colorLookup[GetLookupIndex(pixel[2] >> (8 - ColorBits),
                        pixel[1] >> (8 - ColorBits), pixel[0] >> (8 - ColorBits))];
                }
            }
        }
        finally
        {
            source.UnlockBits(sourceData);
            output.UnlockBits(outputData);
        }
    }

    private static int GetLookupIndex(int red, int green, int blue) =>
        (red * ColorSideLength + green) * ColorSideLength + blue;

    private static int GetHistogramIndex(int red, int green, int blue) =>
        (red * HistogramSideLength + green) * HistogramSideLength + blue;

    private sealed class ColorMoments
    {
        private readonly long[] _weights = new long[HistogramLength];
        private readonly long[] _redMoments = new long[HistogramLength];
        private readonly long[] _greenMoments = new long[HistogramLength];
        private readonly long[] _blueMoments = new long[HistogramLength];

        public int DistinctColorCount { get; private set; }

        public unsafe void AddPixels(BitmapData bitmapData, int width, int height)
        {
            for (int y = 0; y < height; y++)
            {
                byte* row = (byte*)bitmapData.Scan0 + y * bitmapData.Stride;

                for (int x = 0; x < width; x++)
                {
                    byte* pixel = row + x * 4;

                    if (pixel[3] < AlphaThreshold)
                    {
                        continue;
                    }

                    int blue = pixel[0];
                    int green = pixel[1];
                    int red = pixel[2];
                    int index = GetHistogramIndex((red >> (8 - ColorBits)) + 1,
                        (green >> (8 - ColorBits)) + 1, (blue >> (8 - ColorBits)) + 1);

                    if (_weights[index]++ == 0)
                    {
                        DistinctColorCount++;
                    }

                    _redMoments[index] += red;
                    _greenMoments[index] += green;
                    _blueMoments[index] += blue;
                }
            }
        }

        public Color[] CreatePalette(int maximumColorCount)
        {
            if (DistinctColorCount == 0)
            {
                return [Color.Black];
            }

            CalculateCumulativeMoments();
            int targetColorCount = Math.Min(maximumColorCount, DistinctColorCount);
            List<ColorCube> cubes =
            [
                new ColorCube
                {
                    RedMaximum = ColorSideLength,
                    GreenMaximum = ColorSideLength,
                    BlueMaximum = ColorSideLength
                }
            ];

            while (cubes.Count < targetColorCount)
            {
                int bestCubeIndex = -1;
                double bestGain = 0;
                ColorCube bestFirst = default;
                ColorCube bestSecond = default;

                for (int index = 0; index < cubes.Count; index++)
                {
                    if (TrySplit(cubes[index], out ColorCube first, out ColorCube second, out double gain) &&
                        gain > bestGain)
                    {
                        bestCubeIndex = index;
                        bestGain = gain;
                        bestFirst = first;
                        bestSecond = second;
                    }
                }

                if (bestCubeIndex < 0)
                {
                    break;
                }

                cubes[bestCubeIndex] = bestFirst;
                cubes.Add(bestSecond);
            }

            Color[] palette = new Color[cubes.Count];

            for (int index = 0; index < cubes.Count; index++)
            {
                ColorCube cube = cubes[index];
                long weight = GetVolume(cube, _weights);
                int red = (int)Math.Clamp((GetVolume(cube, _redMoments) + weight / 2) / weight, 0, 255);
                int green = (int)Math.Clamp((GetVolume(cube, _greenMoments) + weight / 2) / weight, 0, 255);
                int blue = (int)Math.Clamp((GetVolume(cube, _blueMoments) + weight / 2) / weight, 0, 255);
                palette[index] = Color.FromArgb(red, green, blue);
            }

            return palette;
        }

        private void CalculateCumulativeMoments()
        {
            long[] areaWeights = new long[HistogramSideLength];
            long[] areaReds = new long[HistogramSideLength];
            long[] areaGreens = new long[HistogramSideLength];
            long[] areaBlues = new long[HistogramSideLength];

            for (int red = 1; red <= ColorSideLength; red++)
            {
                Array.Clear(areaWeights);
                Array.Clear(areaReds);
                Array.Clear(areaGreens);
                Array.Clear(areaBlues);

                for (int green = 1; green <= ColorSideLength; green++)
                {
                    long lineWeight = 0;
                    long lineRed = 0;
                    long lineGreen = 0;
                    long lineBlue = 0;

                    for (int blue = 1; blue <= ColorSideLength; blue++)
                    {
                        int index = GetHistogramIndex(red, green, blue);
                        lineWeight += _weights[index];
                        lineRed += _redMoments[index];
                        lineGreen += _greenMoments[index];
                        lineBlue += _blueMoments[index];
                        areaWeights[blue] += lineWeight;
                        areaReds[blue] += lineRed;
                        areaGreens[blue] += lineGreen;
                        areaBlues[blue] += lineBlue;
                        int previousPlaneIndex = GetHistogramIndex(red - 1, green, blue);
                        _weights[index] = _weights[previousPlaneIndex] + areaWeights[blue];
                        _redMoments[index] = _redMoments[previousPlaneIndex] + areaReds[blue];
                        _greenMoments[index] = _greenMoments[previousPlaneIndex] + areaGreens[blue];
                        _blueMoments[index] = _blueMoments[previousPlaneIndex] + areaBlues[blue];
                    }
                }
            }
        }

        private bool TrySplit(ColorCube cube, out ColorCube first, out ColorCube second, out double gain)
        {
            long totalWeight = GetVolume(cube, _weights);
            long totalRed = GetVolume(cube, _redMoments);
            long totalGreen = GetVolume(cube, _greenMoments);
            long totalBlue = GetVolume(cube, _blueMoments);
            double originalScore = GetColorScore(totalRed, totalGreen, totalBlue, totalWeight);
            double bestScore = double.NegativeInfinity;
            SplitAxis bestAxis = SplitAxis.None;
            int bestCut = 0;

            EvaluateCuts(cube, SplitAxis.Red, cube.RedMinimum + 1, cube.RedMaximum,
                totalWeight, totalRed, totalGreen, totalBlue, ref bestScore, ref bestAxis, ref bestCut);
            EvaluateCuts(cube, SplitAxis.Green, cube.GreenMinimum + 1, cube.GreenMaximum,
                totalWeight, totalRed, totalGreen, totalBlue, ref bestScore, ref bestAxis, ref bestCut);
            EvaluateCuts(cube, SplitAxis.Blue, cube.BlueMinimum + 1, cube.BlueMaximum,
                totalWeight, totalRed, totalGreen, totalBlue, ref bestScore, ref bestAxis, ref bestCut);

            first = cube;
            second = cube;

            switch (bestAxis)
            {
                case SplitAxis.Red:
                    first.RedMaximum = bestCut;
                    second.RedMinimum = bestCut;
                    break;
                case SplitAxis.Green:
                    first.GreenMaximum = bestCut;
                    second.GreenMinimum = bestCut;
                    break;
                case SplitAxis.Blue:
                    first.BlueMaximum = bestCut;
                    second.BlueMinimum = bestCut;
                    break;
                default:
                    gain = 0;
                    return false;
            }

            gain = bestScore - originalScore;
            return gain > 0;
        }

        private void EvaluateCuts(ColorCube cube, SplitAxis axis, int firstCut, int lastCut,
            long totalWeight, long totalRed, long totalGreen, long totalBlue,
            ref double bestScore, ref SplitAxis bestAxis, ref int bestCut)
        {
            for (int cut = firstCut; cut < lastCut; cut++)
            {
                ColorCube candidate = cube;

                switch (axis)
                {
                    case SplitAxis.Red:
                        candidate.RedMaximum = cut;
                        break;
                    case SplitAxis.Green:
                        candidate.GreenMaximum = cut;
                        break;
                    case SplitAxis.Blue:
                        candidate.BlueMaximum = cut;
                        break;
                }

                long firstWeight = GetVolume(candidate, _weights);
                long secondWeight = totalWeight - firstWeight;

                if (firstWeight == 0 || secondWeight == 0)
                {
                    continue;
                }

                long firstRed = GetVolume(candidate, _redMoments);
                long firstGreen = GetVolume(candidate, _greenMoments);
                long firstBlue = GetVolume(candidate, _blueMoments);
                double score = GetColorScore(firstRed, firstGreen, firstBlue, firstWeight) +
                    GetColorScore(totalRed - firstRed, totalGreen - firstGreen, totalBlue - firstBlue, secondWeight);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestAxis = axis;
                    bestCut = cut;
                }
            }
        }

        private static double GetColorScore(long red, long green, long blue, long weight) =>
            ((double)red * red + (double)green * green + (double)blue * blue) / weight;

        private static long GetVolume(ColorCube cube, long[] moments) =>
            moments[GetHistogramIndex(cube.RedMaximum, cube.GreenMaximum, cube.BlueMaximum)] -
            moments[GetHistogramIndex(cube.RedMaximum, cube.GreenMaximum, cube.BlueMinimum)] -
            moments[GetHistogramIndex(cube.RedMaximum, cube.GreenMinimum, cube.BlueMaximum)] +
            moments[GetHistogramIndex(cube.RedMaximum, cube.GreenMinimum, cube.BlueMinimum)] -
            moments[GetHistogramIndex(cube.RedMinimum, cube.GreenMaximum, cube.BlueMaximum)] +
            moments[GetHistogramIndex(cube.RedMinimum, cube.GreenMaximum, cube.BlueMinimum)] +
            moments[GetHistogramIndex(cube.RedMinimum, cube.GreenMinimum, cube.BlueMaximum)] -
            moments[GetHistogramIndex(cube.RedMinimum, cube.GreenMinimum, cube.BlueMinimum)];
    }

    private enum SplitAxis
    {
        None,
        Red,
        Green,
        Blue
    }

    private struct ColorCube
    {
        public int RedMinimum;
        public int RedMaximum;
        public int GreenMinimum;
        public int GreenMaximum;
        public int BlueMinimum;
        public int BlueMaximum;
    }
}
