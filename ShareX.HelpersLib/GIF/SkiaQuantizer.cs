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
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
namespace ShareX.HelpersLib
{
    public abstract class SkiaQuantizer
    {
        private readonly bool singlePass;
        protected SkiaQuantizer(bool singlePass) => this.singlePass = singlePass;
        public IndexedImage Quantize(SKBitmap source)
        {
            using SkiaPixelBuffer pixels = new(source, true, PixelAccess.ReadOnly);
            if (!singlePass)
                for (int index = 0; index < pixels.PixelCount; index++) InitialQuantizePixel(ToColor32(pixels.GetPixel(index)));
            Color[] palette = GetPalette(Enumerable.Repeat(Color.Black, 256).ToArray());
            byte[] indices = new byte[pixels.PixelCount];
            for (int index = 0; index < indices.Length; index++) indices[index] = QuantizePixel(ToColor32(pixels.GetPixel(index)));
            return new IndexedImage(source.Width, source.Height, indices, palette);
        }
        private static Color32 ToColor32(ColorBgra pixel) => new() { Blue = pixel.Blue, Green = pixel.Green, Red = pixel.Red, Alpha = pixel.Alpha };
        protected virtual void InitialQuantizePixel(Color32 pixel) { }
        protected abstract byte QuantizePixel(Color32 pixel);
        protected abstract Color[] GetPalette(Color[] palette);
        [StructLayout(LayoutKind.Explicit)]
        public struct Color32
        {
            public Color32(IntPtr pSourcePixel)
            {
                this = (Color32)Marshal.PtrToStructure(pSourcePixel, typeof(Color32));
            }

            /// <summary>
            /// Holds the blue component of the colour
            /// </summary>
            [FieldOffset(0)]
            public byte Blue;
            /// <summary>
            /// Holds the green component of the colour
            /// </summary>
            [FieldOffset(1)]
            public byte Green;
            /// <summary>
            /// Holds the red component of the colour
            /// </summary>
            [FieldOffset(2)]
            public byte Red;
            /// <summary>
            /// Holds the alpha component of the colour
            /// </summary>
            [FieldOffset(3)]
            public byte Alpha;

            /// <summary>
            /// Permits the color32 to be treated as an int32
            /// </summary>
            [FieldOffset(0)]
            public int ARGB;

            /// <summary>
            /// Return the color for this Color32 object
            /// </summary>
            public Color Color
            {
                get
                {
                    return Color.FromArgb(Alpha, Red, Green, Blue);
                }
            }
        }

    }
}
