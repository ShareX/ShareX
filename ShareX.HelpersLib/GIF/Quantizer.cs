#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2024 ShareX Team

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
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ShareX.HelpersLib.GIF;

/// <summary>
/// Summary description for Class1.
/// </summary>
public abstract class Quantizer
{
    /// <summary>
    /// Construct the quantizer
    /// </summary>
    /// <param name="singlePass">If true, the quantization only needs to loop through the source pixels once</param>
    /// <remarks>
    /// If you construct this class with a true value for singlePass, then the code will, when quantizing your image,
    /// only call the 'QuantizeImage' function. If two passes are required, the code will call 'InitialQuantizeImage'
    /// and then 'QuantizeImage'.
    /// </remarks>
    protected Quantizer(bool singlePass)
    {
        _singlePass = singlePass;
        _pixelSize = Marshal.SizeOf<Color32>();
    }

    /// <summary>
    /// Quantize an image and return the resulting output bitmap
    /// </summary>
    /// <param name="source">The image to quantize</param>
    /// <returns>A quantized version of the image</returns>
    public Bitmap Quantize(Image source)
    {
        // Get the size of the source image
        int height = source.Height;
        int width = source.Width;

        // And construct a rectangle from these dimensions
        Rectangle bounds = new(0, 0, width, height);

        // First off take a 32bpp copy of the image
        Bitmap copy = new(width, height, PixelFormat.Format32bppArgb);

        // And construct an 8bpp version
        Bitmap output = new(width, height, PixelFormat.Format8bppIndexed);

        // Now lock the bitmap into memory
        using (Graphics g = Graphics.FromImage(copy))
        {
            g.PageUnit = GraphicsUnit.Pixel;

            // Draw the source image onto the copy bitmap,
            // which will effect a widening as appropriate.
            g.DrawImage(source, bounds);
        }

        // Define a pointer to the bitmap data
        BitmapData sourceData = null;

        try
        {
            // Get the source image bits and lock into memory
            sourceData = copy.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            // Call the FirstPass function if not a single pass algorithm.
            // For something like an octree quantizer, this will run through
            // all image pixels, build a data structure, and create a palette.
            if (!_singlePass)
                FirstPass(sourceData, width, height);

            // Then set the color palette on the output bitmap. I'm passing in the current palette
            // as there's no way to construct a new, empty palette.
            output.Palette = GetPalette(output.Palette);

            // Then call the second pass which actually does the conversion
            SecondPass(sourceData, output, width, height, bounds);
        } finally
        {
            // Ensure that the bits are unlocked
            copy.UnlockBits(sourceData);
        }

        // Last but not least, return the output bitmap
        return output;
    }

    /// <summary>
    /// Execute the first pass through the pixels in the image
    /// </summary>
    /// <param name="sourceData">The source data</param>
    /// <param name="width">The width in pixels of the image</param>
    /// <param name="height">The height in pixels of the image</param>
    protected virtual void FirstPass(BitmapData sourceData, int width, int height)
    {
        IntPtr pSourceRow = sourceData.Scan0;

        for (int row = 0; row < height; row++)
        {
            IntPtr pSourcePixel = pSourceRow;

            for (int col = 0; col < width; col++)
            {
                InitialQuantizePixel(new Color32(pSourcePixel));
                pSourcePixel = checked((IntPtr)((long)pSourcePixel + _pixelSize));
            }

            pSourceRow = checked((IntPtr)((long)pSourceRow + sourceData.Stride));
        }
    }

    /// <summary>
    /// Execute a second pass through the bitmap
    /// </summary>
    /// <param name="sourceData">The source bitmap, locked into memory</param>
    /// <param name="output">The output bitmap</param>
    /// <param name="width">The width in pixels of the image</param>
    /// <param name="height">The height in pixels of the image</param>
    /// <param name="bounds">The bounding rectangle</param>
    protected virtual void SecondPass(BitmapData sourceData, Bitmap output, int width, int height, Rectangle bounds)
    {
        BitmapData outputData = null;

        try
        {
            outputData = output.LockBits(bounds, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            IntPtr pSourceRow = sourceData.Scan0;
            IntPtr pSourcePixel = pSourceRow;
            IntPtr pPreviousPixel = pSourcePixel;

            IntPtr pDestinationRow = outputData.Scan0;
            IntPtr pDestinationPixel = pDestinationRow;

            byte pixelValue = QuantizePixel(new Color32(pSourcePixel));
            Marshal.WriteByte(pDestinationPixel, pixelValue);

            for (int row = 0; row < height; row++)
            {
                pSourcePixel = pSourceRow;
                pDestinationPixel = pDestinationRow;

                for (int col = 0; col < width; col++)
                {
                    if (Marshal.ReadInt32(pPreviousPixel) != Marshal.ReadInt32(pSourcePixel))
                    {
                        pixelValue = QuantizePixel(new Color32(pSourcePixel));
                        pPreviousPixel = pSourcePixel;
                    }

                    Marshal.WriteByte(pDestinationPixel, pixelValue);

                    pSourcePixel = checked((IntPtr)((long)pSourcePixel + _pixelSize));
                    pDestinationPixel = checked((IntPtr)((long)pDestinationPixel + 1));
                }

                pSourceRow = checked((IntPtr)((long)pSourceRow + sourceData.Stride));
                pDestinationRow = checked((IntPtr)((long)pDestinationRow + outputData.Stride));
            }
        } finally
        {
            output.UnlockBits(outputData);
        }
    }

    /// <summary>
    /// Override this to process the pixel in the first pass of the algorithm
    /// </summary>
    /// <param name="pixel">The pixel to quantize</param>
    /// <remarks>
    /// This function need only be overridden if your quantize algorithm needs two passes,
    /// such as an Octree quantizer.
    /// </remarks>
    protected virtual void InitialQuantizePixel(Color32 pixel)
    {
    }

    /// <summary>
    /// Override this to process the pixel in the second pass of the algorithm
    /// </summary>
    /// <param name="pixel">The pixel to quantize</param>
    /// <returns>The quantized value</returns>
    protected abstract byte QuantizePixel(Color32 pixel);

    /// <summary>
    /// Retrieve the palette for the quantized image
    /// </summary>
    /// <param name="original">Any old palette, this is overrwritten</param>
    /// <returns>The new color palette</returns>
    protected abstract ColorPalette GetPalette(ColorPalette original);

    /// <summary>
    /// Flag used to indicate whether a single pass or two passes are needed for quantization.
    /// </summary>
    private bool _singlePass;
    private int _pixelSize;

    /// <summary>
    /// Struct that defines a 32 bpp colour
    /// </summary>
    /// <remarks>
    /// This struct is used to read data from a 32 bits per pixel image
    /// in memory, and is ordered in this manner as this is the way that
    /// the data is layed out in memory
    /// </remarks>
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
        public Color Color => Color.FromArgb(Alpha, Red, Green, Blue);
    }
}