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

using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ShareX.HelpersLib;

/// <summary>Builds an adaptive palette from one or more frames, then applies Sierra Lite dithering.</summary>
public sealed class AdaptiveGifQuantizer
{
    private const int ColorBits = 6;
    private const int ColorSideLength = 1 << ColorBits;
    private const int HistogramSideLength = ColorSideLength + 1;
    private const int HistogramLength = HistogramSideLength * HistogramSideLength * HistogramSideLength;
    private const byte AlphaThreshold = 128;
    private ColorMoments moments = new();
    private GifPaletteMapper mapper;
    private bool hasFrames;

    /// <summary>Add every source frame before quantizing to share a stable palette across an animation.</summary>
    public void AddFrame(SKBitmap source)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (mapper != null) throw new InvalidOperationException("Add frames before quantizing the palette.");
        using SkiaPixelBuffer pixels = new(source, true, PixelAccess.ReadOnly);
        moments.AddPixels(pixels, source.Width, source.Height);
        hasFrames = true;
    }

    /// <summary>Quantizes using the accumulated palette, or builds a palette from this image on the first call.</summary>
    public IndexedImage Quantize(SKBitmap source)
    {
        ArgumentNullException.ThrowIfNull(source);
        using SkiaPixelBuffer pixels = new(source, true, PixelAccess.ReadOnly);
        if (mapper == null)
        {
            if (!hasFrames) moments.AddPixels(pixels, source.Width, source.Height);
            Color[] colors = moments.CreatePalette(moments.HasTransparency ? 255 : 256);
            mapper = new(colors, moments.HasTransparency);
            moments = null;
        }
        return mapper.Quantize(pixels);
    }

    private static int GetHistogramIndex(int red, int green, int blue) =>
        (red * HistogramSideLength + green) * HistogramSideLength + blue;

    private sealed class ColorMoments
    {
        private const int MinimumFlatRun = 8;
        private const int MaximumFlatCandidates = 64;
        private const int MaximumPreservedColors = 32;
        private sealed class ColorFrequency(int color, long count, long error)
        {
            public int Color = color;
            public long Count = count;
            public long Error = error;
        }
        private readonly long[] _weights = new long[HistogramLength];
        private readonly long[] _redMoments = new long[HistogramLength];
        private readonly long[] _greenMoments = new long[HistogramLength];
        private readonly long[] _blueMoments = new long[HistogramLength];
        private HashSet<int> _exactColors = new();
        private readonly Dictionary<int, ColorFrequency> _flatColors = new();
        private readonly PriorityQueue<ColorFrequency, long> _flatCandidates = new();
        private long _opaquePixels;

        public int DistinctColorCount { get; private set; }
        public bool HasTransparency { get; private set; }

        public unsafe void AddPixels(SkiaPixelBuffer bitmapData, int width, int height)
        {
            int previousColor = -1;
            long opaquePixels = 0;
            for (int y = 0; y < height; y++)
            {
                byte* row = (byte*)bitmapData.Pointer + y * width * 4;
                int runColor = -1, runLength = 0;

                for (int x = 0; x < width; x++)
                {
                    byte* pixel = row + x * 4;

                    if (pixel[3] < AlphaThreshold)
                    {
                        HasTransparency = true;
                        if (runLength >= MinimumFlatRun) AddFlatRun(runColor, runLength);
                        runColor = -1;
                        runLength = 0;
                        continue;
                    }

                    int blue = pixel[0];
                    int green = pixel[1];
                    int red = pixel[2];
                    int color = (red << 16) | (green << 8) | blue;
                    opaquePixels++;
                    if (color == runColor) runLength++;
                    else
                    {
                        if (runLength >= MinimumFlatRun) AddFlatRun(runColor, runLength);
                        runColor = color;
                        runLength = 1;
                    }
                    if (_exactColors != null && color != previousColor)
                    {
                        _exactColors.Add(color);
                        if (_exactColors.Count > 256) _exactColors = null;
                        previousColor = color;
                    }
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
                if (runLength >= MinimumFlatRun) AddFlatRun(runColor, runLength);
            }
            _opaquePixels += opaquePixels;
        }

        private void AddFlatRun(int color, int length)
        {
            if (_flatColors.TryGetValue(color, out ColorFrequency frequency))
            {
                frequency.Count += length;
                return;
            }
            if (_flatColors.Count < MaximumFlatCandidates)
            {
                frequency = new(color, length, 0);
            }
            else
            {
                // Counts only increase, so refresh stale priorities when they reach the front.
                // There is one queue entry per candidate, keeping both memory and lookup bounded.
                while (_flatCandidates.TryPeek(out ColorFrequency candidate, out long count) && candidate.Count != count)
                {
                    _flatCandidates.Dequeue();
                    _flatCandidates.Enqueue(candidate, candidate.Count);
                }
                frequency = _flatCandidates.Dequeue();
                _flatColors.Remove(frequency.Color);
                long minimum = frequency.Count;
                frequency.Color = color;
                frequency.Count = minimum + length;
                frequency.Error = minimum;
            }
            // A bounded weighted frequency sketch tracks large solid areas without storing a
            // full RGB histogram. Count - Error is the number of pixels definitely observed.
            _flatColors.Add(color, frequency);
            _flatCandidates.Enqueue(frequency, frequency.Count);
        }

        public Color[] CreatePalette(int maximumColorCount)
        {
            if (DistinctColorCount == 0)
            {
                return [Color.Black];
            }

            // Preserve every RGB value when the image already fits in a GIF palette. Histogram
            // buckets must not collapse nearby UI colors or a complete 256-shade grayscale ramp.
            if (_exactColors != null && _exactColors.Count <= maximumColorCount)
                return _exactColors.Order().Select(color => Color.FromArgb(unchecked((int)0xFF000000) | color)).ToArray();

            var preserved = _flatColors
                .Where(entry => entry.Value.Count - entry.Value.Error >= Math.Max(MinimumFlatRun, _opaquePixels / 256))
                .OrderByDescending(entry => entry.Value.Count - entry.Value.Error).ThenBy(entry => entry.Key)
                .Take(MaximumPreservedColors).ToArray();
            foreach (var entry in preserved)
            {
                int red = (entry.Key >> 16) & 255, green = (entry.Key >> 8) & 255, blue = entry.Key & 255;
                int index = GetHistogramIndex((red >> (8 - ColorBits)) + 1,
                    (green >> (8 - ColorBits)) + 1, (blue >> (8 - ColorBits)) + 1);
                long count = entry.Value.Count - entry.Value.Error;
                // Reserve the exact UI color before splitting the remaining histogram. Otherwise
                // nearby grays in the same bucket are averaged and require visible dithering.
                _weights[index] -= count;
                _redMoments[index] -= red * count;
                _greenMoments[index] -= green * count;
                _blueMoments[index] -= blue * count;
                if (_weights[index] == 0) DistinctColorCount--;
            }
            Color[] flatPalette = preserved.Select(entry => Color.FromArgb(unchecked((int)0xFF000000) | entry.Key)).ToArray();
            if (DistinctColorCount == 0) return flatPalette;
            CalculateCumulativeMoments();
            int targetColorCount = Math.Min(maximumColorCount - flatPalette.Length, DistinctColorCount);
            List<ColorCube> cubes =
            [
                new ColorCube
                {
                    RedMaximum = ColorSideLength,
                    GreenMaximum = ColorSideLength,
                    BlueMaximum = ColorSideLength
                }
            ];
            PriorityQueue<(int Index, ColorCube First, ColorCube Second), (double Gain, int Index)> splits = new();
            void QueueSplit(int index)
            {
                if (TrySplit(cubes[index], out ColorCube first, out ColorCube second, out double gain))
                    splits.Enqueue((index, first, second), (-gain, index));
            }
            // A cube's optimal split cannot change until that cube is split. Cache each result
            // instead of rescanning all the other cubes after every palette entry is added.
            QueueSplit(0);
            while (cubes.Count < targetColorCount && splits.TryDequeue(out var split, out _))
            {
                cubes[split.Index] = split.First;
                int secondIndex = cubes.Count;
                cubes.Add(split.Second);
                QueueSplit(split.Index);
                QueueSplit(secondIndex);
            }

            Color[] palette = new Color[cubes.Count + flatPalette.Length];
            flatPalette.CopyTo(palette, 0);

            for (int index = 0; index < cubes.Count; index++)
            {
                ColorCube cube = cubes[index];
                long weight = GetVolume(cube, _weights);
                int red = (int)Math.Clamp((GetVolume(cube, _redMoments) + weight / 2) / weight, 0, 255);
                int green = (int)Math.Clamp((GetVolume(cube, _greenMoments) + weight / 2) / weight, 0, 255);
                int blue = (int)Math.Clamp((GetVolume(cube, _blueMoments) + weight / 2) / weight, 0, 255);
                palette[index + flatPalette.Length] = Color.FromArgb(red, green, blue);
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
            (2 * (double)red * red + 4 * (double)green * green + 3 * (double)blue * blue) / weight;

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
