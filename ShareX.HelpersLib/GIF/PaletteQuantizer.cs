#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;

namespace ShareX.HelpersLib
{
    /// <summary>
    /// Summary description for PaletteQuantizer.
    /// </summary>
    public class PaletteQuantizer : Quantizer
    {
        /// <summary>
        /// Construct the palette quantizer
        /// </summary>
        /// <param name="palette">The color palette to quantize to</param>
        /// <remarks>
        /// Palette quantization only requires a single quantization step
        /// </remarks>
        public PaletteQuantizer(ArrayList palette)
            : base(true)
        {
            _colorMap = new System.Collections.Hashtable();

            _colors = new Color[palette.Count];
            palette.CopyTo(_colors);

            // Precompute palette vectors for SIMD distance
            _paletteR = new int[_colors.Length];
            _paletteG = new int[_colors.Length];
            _paletteB = new int[_colors.Length];
            for (int i = 0; i < _colors.Length; i++)
            {
                var c = _colors[i];
                _paletteR[i] = c.R;
                _paletteG[i] = c.G;
                _paletteB[i] = c.B;
            }
        }

        /// <summary>
        /// Override this to process the pixel in the second pass of the algorithm
        /// </summary>
        /// <param name="pixel">The pixel to quantize</param>
        /// <returns>The quantized value</returns>
        protected override byte QuantizePixel(Color32 pixel)
        {
            byte colorIndex = 0;
            int colorHash = pixel.ARGB;

            // Check if the color is in the lookup table
            if (_colorMap.ContainsKey(colorHash))
            {
                colorIndex = (byte)_colorMap[colorHash];
            }
            else
            {
                // Transparent handling
                if (pixel.Alpha == 0)
                {
                    for (int index = 0; index < _colors.Length; index++)
                    {
                        if (_colors[index].A == 0)
                        {
                            colorIndex = (byte)index;
                            break;
                        }
                    }
                }
                else
                {
                    int leastDistance = int.MaxValue;
                    int bestIndex = 0;

                    int r = pixel.Red;
                    int g = pixel.Green;
                    int b = pixel.Blue;

                    if (Vector.IsHardwareAccelerated && _colors.Length >= Vector<int>.Count)
                    {
                        var vr = new Vector<int>(r);
                        var vg = new Vector<int>(g);
                        var vb = new Vector<int>(b);
                        int i = 0;
                        for (; i <= _colors.Length - Vector<int>.Count; i += Vector<int>.Count)
                        {
                            var pr = new Vector<int>(_paletteR, i) - vr;
                            var pg = new Vector<int>(_paletteG, i) - vg;
                            var pb = new Vector<int>(_paletteB, i) - vb;
                            var dist = (pr * pr) + (pg * pg) + (pb * pb);

                            // Find min element in dist
                            int localBest = 0;
                            int localMin = int.MaxValue;
                            for (int k = 0; k < Vector<int>.Count; k++)
                            {
                                int d = dist[k];
                                if (d < localMin)
                                {
                                    localMin = d;
                                    localBest = i + k;
                                }
                            }

                            if (localMin < leastDistance)
                            {
                                leastDistance = localMin;
                                bestIndex = localBest;
                                if (leastDistance == 0) break;
                            }
                        }

                        // Tail
                        for (; i < _colors.Length; i++)
                        {
                            int dr = _paletteR[i] - r;
                            int dg = _paletteG[i] - g;
                            int db = _paletteB[i] - b;
                            int d = (dr * dr) + (dg * dg) + (db * db);
                            if (d < leastDistance)
                            {
                                leastDistance = d;
                                bestIndex = i;
                                if (d == 0) break;
                            }
                        }

                        colorIndex = (byte)bestIndex;
                    }
                    else
                    {
                        // Scalar fallback
                        for (int index = 0; index < _colors.Length; index++)
                        {
                            var paletteColor = _colors[index];
                            int dr = paletteColor.R - r;
                            int dg = paletteColor.G - g;
                            int db = paletteColor.B - b;
                            int distance = (dr * dr) + (dg * dg) + (db * db);
                            if (distance < leastDistance)
                            {
                                colorIndex = (byte)index;
                                leastDistance = distance;
                                if (distance == 0) break;
                            }
                        }
                    }
                }

                // Cache result
                _colorMap.Add(colorHash, colorIndex);
            }

            return colorIndex;
        }

        /// <summary>
        /// Retrieve the palette for the quantized image
        /// </summary>
        /// <param name="palette">Any old palette, this is overrwritten</param>
        /// <returns>The new color palette</returns>
        protected override ColorPalette GetPalette(ColorPalette palette)
        {
            for (int index = 0; index < _colors.Length; index++)
            {
                palette.Entries[index] = _colors[index];
            }

            return palette;
        }

        private System.Collections.Hashtable _colorMap;
        protected Color[] _colors;

        // Precomputed palette channels for SIMD distance
        private int[] _paletteR;
        private int[] _paletteG;
        private int[] _paletteB;
    }
}