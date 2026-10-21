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

using System;
using System.Collections.Generic;
using System.Drawing;

namespace ShareX.HelpersLib;

// Exact RGB nearest-color lookup with a bounded cache and a weighted k-d tree. Histogram
// precision is only used for palette construction; it never rounds the pixels being mapped.
internal sealed class GifPaletteMapper
{
    private readonly record struct ColorNode(int Red, int Green, int Blue, byte Index, int Axis, int Left, int Right);
    private readonly Color[] palette;
    private readonly int[] rgb;
    private readonly Dictionary<int, byte> exactColors;
    private readonly ColorNode[] nodes;
    private readonly int root, transparentIndex;
    private readonly int[] cachedColors = new int[1 << 20];
    private readonly byte[] cachedIndices = new byte[1 << 20];
    private int nodeCount;

    public GifPaletteMapper(Color[] colors, bool transparency)
    {
        transparentIndex = transparency ? colors.Length : -1;
        palette = new Color[colors.Length + (transparency ? 1 : 0)];
        colors.CopyTo(palette, 0);
        rgb = new int[colors.Length];
        exactColors = new(colors.Length);
        for (int index = 0; index < colors.Length; index++)
        {
            rgb[index] = colors[index].ToArgb();
            exactColors.TryAdd(rgb[index] & 0xFFFFFF, (byte)index);
        }
        if (transparency) palette[transparentIndex] = Color.Transparent;
        byte[] order = new byte[colors.Length];
        for (int index = 0; index < order.Length; index++) order[index] = (byte)index;
        nodes = new ColorNode[order.Length];
        root = BuildTree(order, 0, order.Length);
        Array.Fill(cachedColors, -1);
    }

    private int BuildTree(byte[] order, int start, int count)
    {
        if (count == 0) return -1;
        int minRed = 255, minGreen = 255, minBlue = 255, maxRed = 0, maxGreen = 0, maxBlue = 0;
        for (int index = start; index < start + count; index++)
        {
            Color color = palette[order[index]];
            minRed = Math.Min(minRed, color.R); maxRed = Math.Max(maxRed, color.R);
            minGreen = Math.Min(minGreen, color.G); maxGreen = Math.Max(maxGreen, color.G);
            minBlue = Math.Min(minBlue, color.B); maxBlue = Math.Max(maxBlue, color.B);
        }
        int redRange = maxRed - minRed, greenRange = maxGreen - minGreen, blueRange = maxBlue - minBlue;
        int redScore = 2 * redRange * redRange, greenScore = 4 * greenRange * greenRange, blueScore = 3 * blueRange * blueRange;
        int axis = greenScore > redScore ? 1 : 0;
        if (blueScore > Math.Max(redScore, greenScore)) axis = 2;
        Array.Sort(order, start, count, Comparer<byte>.Create((left, right) =>
        {
            Color first = palette[left], second = palette[right];
            return axis switch { 0 => first.R.CompareTo(second.R), 1 => first.G.CompareTo(second.G), _ => first.B.CompareTo(second.B) };
        }));
        int middle = start + count / 2;
        byte paletteIndex = order[middle];
        Color selected = palette[paletteIndex];
        int node = nodeCount++;
        int leftNode = BuildTree(order, start, middle - start);
        int rightNode = BuildTree(order, middle + 1, start + count - middle - 1);
        nodes[node] = new(selected.R, selected.G, selected.B, paletteIndex, axis, leftNode, rightNode);
        return node;
    }

    private byte Nearest(int red, int green, int blue)
    {
        int color = (red << 16) | (green << 8) | blue;
        int slot = (int)(((uint)color * 2654435761u) >> 12);
        if (cachedColors[slot] == color) return cachedIndices[slot];
        int distance = int.MaxValue;
        byte index = 0;
        Search(root, red, green, blue, ref distance, ref index);
        cachedColors[slot] = color;
        cachedIndices[slot] = index;
        return index;
    }

    private void Search(int position, int red, int green, int blue, ref int bestDistance, ref byte bestIndex)
    {
        if (position < 0 || bestDistance == 0) return;
        ref readonly ColorNode node = ref nodes[position];
        int dr = red - node.Red, dg = green - node.Green, db = blue - node.Blue;
        int distance = 2 * dr * dr + 4 * dg * dg + 3 * db * db;
        if (distance < bestDistance) { bestDistance = distance; bestIndex = node.Index; }
        int delta = node.Axis switch { 0 => dr, 1 => dg, _ => db };
        Search(delta <= 0 ? node.Left : node.Right, red, green, blue, ref bestDistance, ref bestIndex);
        int weight = node.Axis switch { 0 => 2, 1 => 4, _ => 3 };
        if (weight * delta * delta < bestDistance)
            Search(delta <= 0 ? node.Right : node.Left, red, green, blue, ref bestDistance, ref bestIndex);
    }

    public unsafe IndexedImage Quantize(SkiaPixelBuffer source)
    {
        int width = source.Width, height = source.Height;
        byte[] indices = new byte[source.PixelCount];
        int[] current = new int[(width + 2) * 3], next = new int[current.Length];
        fixed (int* first = current, second = next)
        fixed (byte* output = indices)
        {
            int* errors = first;
            int* below = second;
            for (int y = 0; y < height; y++)
            {
                int row = y * width;
                for (int x = 0; x < width; x++)
                {
                    ColorBgra pixel = source.Pointer[row + x];
                    if (pixel.Alpha < 128)
                    {
                        if (transparentIndex < 0) throw new InvalidOperationException("Include transparent frames before building the GIF palette.");
                        output[row + x] = (byte)transparentIndex;
                        continue;
                    }
                    // Never let error from a neighboring gradient dither a color that the
                    // palette can already reproduce exactly, such as a flat window background.
                    int originalColor = (pixel.Red << 16) | (pixel.Green << 8) | pixel.Blue;
                    if (exactColors.TryGetValue(originalColor, out byte exact))
                    {
                        output[row + x] = exact;
                        continue;
                    }
                    int error = (x + 1) * 3;
                    int red = Math.Clamp(pixel.Red + ((errors[error] + 2) >> 2), 0, 255);
                    int green = Math.Clamp(pixel.Green + ((errors[error + 1] + 2) >> 2), 0, 255);
                    int blue = Math.Clamp(pixel.Blue + ((errors[error + 2] + 2) >> 2), 0, 255);
                    byte index = Nearest(red, green, blue);
                    output[row + x] = index;
                    int color = rgb[index];
                    // Sierra Lite: 1/2 to the right, 1/4 below-left, 1/4 below.
                    Diffuse(errors, below, error, red - ((color >> 16) & 255));
                    Diffuse(errors, below, error + 1, green - ((color >> 8) & 255));
                    Diffuse(errors, below, error + 2, blue - (color & 255));
                }
                int* swap = errors; errors = below; below = swap;
                new Span<int>(below, current.Length).Clear();
            }
        }
        return new(width, height, indices, palette);
    }

    private static unsafe void Diffuse(int* current, int* next, int offset, int error)
    {
        current[offset + 3] += error * 2;
        next[offset - 3] += error;
        next[offset] += error;
    }
}
