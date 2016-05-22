/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2015 Thomas Braun, Jens Klingen, Robin Krom
 *
 * For more information see: http://getgreenshot.org/
 * The Greenshot project is hosted on Sourceforge: http://sourceforge.net/projects/greenshot/
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 1 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace GreenshotPlugin.Core
{
    /// <summary>
    /// The interface for the FastBitmap
    /// </summary>
    public interface IFastBitmap : IDisposable
    {
        /// <summary>
        /// Get the color at x,y
        /// The returned Color object depends on the underlying pixel format
        /// </summary>
        /// <param name="x">int x</param>
        /// <param name="y">int y</param>
        /// <returns>Color</returns>
        Color GetColorAt(int x, int y);

        /// <summary>
        /// Set the color at the specified location
        /// </summary>
        /// <param name="x">int x</param>
        /// <param name="y">int y</param>
        /// <param name="color">Color</param>
        void SetColorAt(int x, int y, Color color);

        /// <summary>
        /// Get the color at x,y
        /// The returned byte[] color depends on the underlying pixel format
        /// </summary>
        /// <param name="x">int x</param>
        /// <param name="y">int y</par
        void GetColorAt(int x, int y, byte[] color);

        /// <summary>
        /// Set the color at the specified location
        /// </summary>
        /// <param name="x">int x</param>
        /// <param name="y">int y</param>
        /// <param name="color">byte[] color</param>
        void SetColorAt(int x, int y, byte[] color);

        /// <summary>
        /// Lock the bitmap
        /// </summary>
        void Lock();

        /// <summary>
        /// Unlock the bitmap
        /// </summary>
        void Unlock();

        /// <summary>
        /// Unlock the bitmap and get the underlying bitmap in one call
        /// </summary>
        /// <returns></returns>
        Bitmap UnlockAndReturnBitmap();

        /// <summary>
        /// Size of the underlying image
        /// </summary>
        Size Size
        {
            get;
        }

        /// <summary>
        /// Height of the image area that this fastbitmap covers
        /// </summary>
        int Height
        {
            get;
        }

        /// <summary>
        /// Width of the image area that this fastbitmap covers
        /// </summary>
        int Width
        {
            get;
        }

        /// <summary>
        /// Top of the image area that this fastbitmap covers
        /// </summary>
        int Top
        {
            get;
        }

        /// <summary>
        /// Left of the image area that this fastbitmap covers
        /// </summary>
        int Left
        {
            get;
        }

        /// <summary>
        /// Right of the image area that this fastbitmap covers
        /// </summary>
        int Right
        {
            get;
        }

        /// <summary>
        /// Bottom of the image area that this fastbitmap covers
        /// </summary>
        int Bottom
        {
            get;
        }

        /// <summary>
        /// Does the underlying image need to be disposed
        /// </summary>
        bool NeedsDispose
        {
            get;
            set;
        }

        /// <summary>
        /// Returns if this FastBitmap has an alpha channel
        /// </summary>
        bool hasAlphaChannel
        {
            get;
        }

        /// <summary>
        /// Draw the stored bitmap to the destionation bitmap at the supplied point
        /// </summary>
        /// <param name="graphics">Graphics</param>
        /// <param name="destination">Point with location</param>
        void DrawTo(Graphics graphics, Point destination);

        /// <summary>
        /// Draw the stored Bitmap on the Destination bitmap with the specified rectangle
        /// Be aware that the stored bitmap will be resized to the specified rectangle!!
        /// </summary>
        /// <param name="graphics">Graphics</param>
        /// <param name="destinationRect">Rectangle with destination</param>
        void DrawTo(Graphics graphics, Rectangle destinationRect);

        /// <summary>
        /// Return true if the coordinates are inside the FastBitmap
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        bool Contains(int x, int y);

        /// <summary>
        /// Set the bitmap resolution
        /// </summary>
        /// <param name="horizontal"></param>
        /// <param name="vertical"></param>
        void SetResolution(float horizontal, float vertical);
    }

    /// <summary>
    /// This interface can be used for when offsetting is needed
    /// </summary>
    public interface IFastBitmapWithOffset : IFastBitmap
    {
        /// <summary>
        /// Return true if the coordinates are inside the FastBitmap
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        new bool Contains(int x, int y);

        /// <summary>
        /// Set the color at the specified location, using offsetting so the original coordinates can be used
        /// </summary>
        /// <param name="x">int x</param>
        /// <param name="y">int y</param>
        /// <param name="color">Color color</param>
        new void SetColorAt(int x, int y, Color color);

        /// <summary>
        /// Set the color at the specified location, using offsetting so the original coordinates can be used
        /// </summary>
        /// <param name="x">int x</param>
        /// <param name="y">int y</param>
        /// <param name="color">byte[] color</param>
        new void SetColorAt(int x, int y, byte[] color);

        /// <summary>
        /// Get the color at x,y
        /// The returned Color object depends on the underlying pixel format
        /// </summary>
        /// <param name="x">int x</param>
        /// <param name="y">int y</param>
        /// <returns>Color</returns>
        new Color GetColorAt(int x, int y);

        /// <summary>
        /// Get the color at x,y, using offsetting so the original coordinates can be used
        /// The returned byte[] color depends on the underlying pixel format
        /// </summary>
        /// <param name="x">int x</param>
        /// <param name="y">int y</par
        new void GetColorAt(int x, int y, byte[] color);

        new int Left
        {
            get;
            set;
        }

        new int Top
        {
            get;
            set;
        }
    }

    /// <summary>
    /// This interface can be used for when clipping is needed
    /// </summary>
    public interface IFastBitmapWithClip : IFastBitmap
    {
        Rectangle Clip
        {
            get;
            set;
        }

        bool InvertClip
        {
            get;
            set;
        }

        /// <summary>
        /// Set the color at the specified location, this doesn't do anything if the location is excluded due to clipping
        /// </summary>
        /// <param name="x">int x</param>
        /// <param name="y">int y</param>
        /// <param name="color">Color color</param>
        new void SetColorAt(int x, int y, Color color);

        /// <summary>
        /// Set the color at the specified location, this doesn't do anything if the location is excluded due to clipping
        /// </summary>
        /// <param name="x">int x</param>
        /// <param name="y">int y</param>
        /// <param name="color">byte[] color</param>
        new void SetColorAt(int x, int y, byte[] color);

        /// <summary>
        /// Return true if the coordinates are inside the FastBitmap and not clipped
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        new bool Contains(int x, int y);
    }

    /// <summary>
    /// This interface is implemented when there is a alpha-blending possibility
    /// </summary>
    public interface IFastBitmapWithBlend : IFastBitmap
    {
        Color BackgroundBlendColor
        {
            get;
            set;
        }

        Color GetBlendedColorAt(int x, int y);
    }

    /// <summary>
    /// The base class for the fast bitmap implementation
    /// </summary>
    public unsafe abstract class FastBitmap : IFastBitmap, IFastBitmapWithClip, IFastBitmapWithOffset
    {
        protected const int PIXELFORMAT_INDEX_A = 3;
        protected const int PIXELFORMAT_INDEX_R = 2;
        protected const int PIXELFORMAT_INDEX_G = 1;
        protected const int PIXELFORMAT_INDEX_B = 0;

        public const int COLOR_INDEX_R = 0;
        public const int COLOR_INDEX_G = 1;
        public const int COLOR_INDEX_B = 2;
        public const int COLOR_INDEX_A = 3;

        protected Rectangle area = Rectangle.Empty;
        /// <summary>
        /// If this is set to true, the bitmap will be disposed when disposing the IFastBitmap
        /// </summary>
        public bool NeedsDispose
        {
            get;
            set;
        }

        public Rectangle Clip
        {
            get;
            set;
        }

        public bool InvertClip
        {
            get;
            set;
        }

        /// <summary>
        /// The bitmap for which the FastBitmap is creating access
        /// </summary>
        protected Bitmap bitmap;

        protected BitmapData bmData;
        protected int stride; /* bytes per pixel row */
        protected bool bitsLocked;
        protected byte* pointer;

        public static IFastBitmap Create(Bitmap source)
        {
            return Create(source, Rectangle.Empty);
        }

        public void SetResolution(float horizontal, float vertical)
        {
            bitmap.SetResolution(horizontal, vertical);
        }

        /// <summary>
        /// Factory for creating a FastBitmap depending on the pixelformat of the source
        /// The supplied rectangle specifies the area for which the FastBitmap does its thing
        /// </summary>
        /// <param name="source">Bitmap to access</param>
        /// <param name="area">Rectangle which specifies the area to have access to, can be Rectangle.Empty for the whole image</param>
        /// <returns>IFastBitmap</returns>
        public static IFastBitmap Create(Bitmap source, Rectangle area)
        {
            switch (source.PixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    return new FastChunkyBitmap(source, area);
                case PixelFormat.Format24bppRgb:
                    return new Fast24RGBBitmap(source, area);
                case PixelFormat.Format32bppRgb:
                    return new Fast32RGBBitmap(source, area);
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                    return new Fast32ARGBBitmap(source, area);
                default:
                    throw new NotSupportedException(string.Format("Not supported Pixelformat {0}", source.PixelFormat));
            }
        }

        /// <summary>
        /// Factory for creating a FastBitmap as a destination for the source
        /// </summary>
        /// <param name="source">Bitmap to clone</param>
        /// <returns>IFastBitmap</returns>
        public static IFastBitmap CreateCloneOf(Image source)
        {
            return CreateCloneOf(source, source.PixelFormat, Rectangle.Empty);
        }

        /// <summary>
        /// Factory for creating a FastBitmap as a destination for the source
        /// </summary>
        /// <param name="source">Bitmap to clone</param>
        /// <param name="pixelFormat">new Pixelformat</param>
        /// <returns>IFastBitmap</returns>
        public static IFastBitmap CreateCloneOf(Image source, PixelFormat pixelFormat)
        {
            return CreateCloneOf(source, pixelFormat, Rectangle.Empty);
        }

        /// <summary>
        /// Factory for creating a FastBitmap as a destination for the source
        /// </summary>
        /// <param name="source">Bitmap to clone</param>
        /// <param name="area">Area of the bitmap to access, can be Rectangle.Empty for the whole</param>
        /// <returns>IFastBitmap</returns>
        public static IFastBitmap CreateCloneOf(Image source, Rectangle area)
        {
            return CreateCloneOf(source, PixelFormat.DontCare, area);
        }

        /// <summary>
        /// Factory for creating a FastBitmap as a destination for the source
        /// </summary>
        /// <param name="source">Bitmap to clone</param>
        /// <param name="pixelFormat">Pixelformat of the cloned bitmap</param>
        /// <param name="area">Area of the bitmap to access, can be Rectangle.Empty for the whole</param>
        /// <returns>IFastBitmap</returns>
        public static IFastBitmap CreateCloneOf(Image source, PixelFormat pixelFormat, Rectangle area)
        {
            Bitmap destination = ImageHelper.CloneArea(source, area, pixelFormat);
            FastBitmap fastBitmap = Create(destination) as FastBitmap;
            fastBitmap.NeedsDispose = true;
            fastBitmap.Left = area.Left;
            fastBitmap.Top = area.Top;
            return fastBitmap;
        }

        /// <summary>
        /// Factory for creating a FastBitmap as a destination
        /// </summary>
        /// <param name="newSize"></param>
        /// <param name="pixelFormat"></param>
        /// <param name="backgroundColor"></param>
        /// <returns>IFastBitmap</returns>
        public static IFastBitmap CreateEmpty(Size newSize, PixelFormat pixelFormat, Color backgroundColor)
        {
            Bitmap destination = ImageHelper.CreateEmpty(newSize.Width, newSize.Height, pixelFormat, backgroundColor, 96f, 96f);
            IFastBitmap fastBitmap = Create(destination);
            fastBitmap.NeedsDispose = true;
            return fastBitmap;
        }

        /// <summary>
        /// Constructor which stores the image and locks it when called
        /// </summary>
        /// <param name="bitmap"></param>
        protected FastBitmap(Bitmap bitmap, Rectangle area)
        {
            this.bitmap = bitmap;
            Rectangle bitmapArea = new Rectangle(Point.Empty, bitmap.Size);
            if (area != Rectangle.Empty)
            {
                area.Intersect(bitmapArea);
                this.area = area;
            }
            else
            {
                this.area = bitmapArea;
            }
            // As the lock takes care that only the specified area is made available we need to calculate the offset
            Left = area.Left;
            Top = area.Top;
            // Default cliping is done to the area without invert
            Clip = this.area;
            InvertClip = false;
            // Always lock, so we don't need to do this ourselves
            Lock();
        }

        /// <summary>
        /// Return the size of the image
        /// </summary>
        public Size Size
        {
            get
            {
                if (area == Rectangle.Empty)
                {
                    return bitmap.Size;
                }
                return area.Size;
            }
        }

        /// <summary>
        /// Return the width of the image
        /// </summary>
        public int Width
        {
            get
            {
                if (area == Rectangle.Empty)
                {
                    return bitmap.Width;
                }
                return area.Width;
            }
        }

        /// <summary>
        /// Return the height of the image
        /// </summary>
        public int Height
        {
            get
            {
                if (area == Rectangle.Empty)
                {
                    return bitmap.Height;
                }
                return area.Height;
            }
        }

        private int left;
        /// <summary>
        /// Return the left of the fastbitmap, this is also used as an offset
        /// </summary>
        public int Left
        {
            get
            {
                return 0;
            }
            set
            {
                left = value;
            }
        }

        /// <summary>
        /// Return the left of the fastbitmap, this is also used as an offset
        /// </summary>
        int IFastBitmapWithOffset.Left
        {
            get
            {
                return left;
            }
            set
            {
                left = value;
            }
        }

        private int top;
        /// <summary>
        /// Return the top of the fastbitmap, this is also used as an offset
        /// </summary>
        public int Top
        {
            get
            {
                return 0;
            }
            set
            {
                top = value;
            }
        }

        /// <summary>
        /// Return the top of the fastbitmap, this is also used as an offset
        /// </summary>
        int IFastBitmapWithOffset.Top
        {
            get
            {
                return top;
            }
            set
            {
                top = value;
            }
        }

        /// <summary>
        /// Return the right of the fastbitmap
        /// </summary>
        public int Right
        {
            get
            {
                return Left + Width;
            }
        }

        /// <summary>
        /// Return the bottom of the fastbitmap
        /// </summary>
        public int Bottom
        {
            get
            {
                return Top + Height;
            }
        }

        /// <summary>
        /// Returns the underlying bitmap, unlocks it and prevents that it will be disposed
        /// </summary>
        public Bitmap UnlockAndReturnBitmap()
        {
            if (bitsLocked)
            {
                Unlock();
            }
            NeedsDispose = false;
            return bitmap;
        }

        public virtual bool hasAlphaChannel
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~FastBitmap()
        {
            Dispose(false);
        }

        /// <summary>
        /// The public accessible Dispose
        /// Will call the GarbageCollector to SuppressFinalize, preventing being cleaned twice
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)

        /// <summary>
        /// This Dispose is called from the Dispose and the Destructor.
        /// When disposing==true all non-managed resources should be freed too!
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            Unlock();
            if (disposing)
            {
                if (bitmap != null && NeedsDispose)
                {
                    bitmap.Dispose();
                }
            }
            bitmap = null;
            bmData = null;
            pointer = null;
        }

        /// <summary>
        /// Lock the bitmap so we have direct access to the memory
        /// </summary>
        public void Lock()
        {
            if (Width > 0 && Height > 0 && !bitsLocked)
            {
                bmData = bitmap.LockBits(area, ImageLockMode.ReadWrite, bitmap.PixelFormat);
                bitsLocked = true;

                IntPtr Scan0 = bmData.Scan0;
                pointer = (byte*)(void*)Scan0;
                stride = bmData.Stride;
            }
        }

        /// <summary>
        /// Unlock the System Memory
        /// </summary>
        public void Unlock()
        {
            if (bitsLocked)
            {
                bitmap.UnlockBits(bmData);
                bitsLocked = false;
            }
        }

        /// <summary>
        /// Draw the stored bitmap to the destionation bitmap at the supplied point
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="destination"></param>
        public void DrawTo(Graphics graphics, Point destination)
        {
            DrawTo(graphics, new Rectangle(destination, area.Size));
        }

        /// <summary>
        /// Draw the stored Bitmap on the Destination bitmap with the specified rectangle
        /// Be aware that the stored bitmap will be resized to the specified rectangle!!
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="destinationRect"></param>
        /// <param name="destination"></param>
        public void DrawTo(Graphics graphics, Rectangle destinationRect)
        {
            // Make sure this.bitmap is unlocked, if it was locked
            bool isLocked = bitsLocked;
            if (isLocked)
            {
                Unlock();
            }

            graphics.DrawImage(bitmap, destinationRect, area, GraphicsUnit.Pixel);
        }

        /// <summary>
        /// returns true if x & y are inside the FastBitmap
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>true if x & y are inside the FastBitmap</returns>
        public bool Contains(int x, int y)
        {
            return area.Contains(x - Left, y - Top);
        }

        public abstract Color GetColorAt(int x, int y);

        public abstract void SetColorAt(int x, int y, Color color);

        public abstract void GetColorAt(int x, int y, byte[] color);

        public abstract void SetColorAt(int x, int y, byte[] color);

        #region IFastBitmapWithClip

        bool IFastBitmapWithClip.Contains(int x, int y)
        {
            bool contains = Clip.Contains(x, y);
            if (InvertClip)
            {
                return !contains;
            }
            else
            {
                return contains;
            }
        }

        void IFastBitmapWithClip.SetColorAt(int x, int y, byte[] color)
        {
            bool contains = Clip.Contains(x, y);
            if ((InvertClip && contains) || (!InvertClip && !contains))
            {
                return;
            }
            SetColorAt(x, y, color);
        }

        void IFastBitmapWithClip.SetColorAt(int x, int y, Color color)
        {
            bool contains = Clip.Contains(x, y);
            if ((InvertClip && contains) || (!InvertClip && !contains))
            {
                return;
            }
            SetColorAt(x, y, color);
        }

        #endregion IFastBitmapWithClip

        #region IFastBitmapWithOffset

        /// <summary>
        /// returns true if x & y are inside the FastBitmap
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>true if x & y are inside the FastBitmap</returns>
        bool IFastBitmapWithOffset.Contains(int x, int y)
        {
            return area.Contains(x - Left, y - Top);
        }

        Color IFastBitmapWithOffset.GetColorAt(int x, int y)
        {
            x -= left;
            y -= top;
            return GetColorAt(x, y);
        }

        void IFastBitmapWithOffset.GetColorAt(int x, int y, byte[] color)
        {
            x -= left;
            y -= top;
            GetColorAt(x, y, color);
        }

        void IFastBitmapWithOffset.SetColorAt(int x, int y, byte[] color)
        {
            x -= left;
            y -= top;
            SetColorAt(x, y, color);
        }

        void IFastBitmapWithOffset.SetColorAt(int x, int y, Color color)
        {
            x -= left;
            y -= top;
            SetColorAt(x, y, color);
        }

        #endregion IFastBitmapWithOffset
    }

    /// <summary>
    /// This is the implementation of the FastBitmat for the 8BPP pixelformat
    /// </summary>
    public unsafe class FastChunkyBitmap : FastBitmap
    {
        // Used for indexed images
        private readonly Color[] colorEntries;
        private readonly Dictionary<Color, byte> colorCache = new Dictionary<Color, byte>();

        public FastChunkyBitmap(Bitmap source, Rectangle area) : base(source, area)
        {
            colorEntries = bitmap.Palette.Entries;
        }

        /// <summary>
        /// Get the color from the specified location
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>Color</returns>
        public override Color GetColorAt(int x, int y)
        {
            int offset = x + (y * stride);
            byte colorIndex = pointer[offset];
            return colorEntries[colorIndex];
        }

        /// <summary>
        /// Get the color from the specified location into the specified array
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color">byte[4] as reference</param>
        public override void GetColorAt(int x, int y, byte[] color)
        {
            throw new NotImplementedException("No performance gain!");
        }

        /// <summary>
        /// Set the color at the specified location from the specified array
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color">byte[4] as reference</param>
        public override void SetColorAt(int x, int y, byte[] color)
        {
            throw new NotImplementedException("No performance gain!");
        }

        /// <summary>
        /// Get the color-index from the specified location
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>byte with index</returns>
        public byte GetColorIndexAt(int x, int y)
        {
            int offset = x + (y * stride);
            return pointer[offset];
        }

        /// <summary>
        /// Set the color-index at the specified location
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color-index"></param>
        public void SetColorIndexAt(int x, int y, byte colorIndex)
        {
            int offset = x + (y * stride);
            pointer[offset] = colorIndex;
        }

        /// <summary>
        /// Set the supplied color at the specified location.
        /// Throws an ArgumentException if the color is not in the palette
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color">Color to set</param>
        public override void SetColorAt(int x, int y, Color color)
        {
            int offset = x + (y * stride);
            byte colorIndex;
            if (!colorCache.TryGetValue(color, out colorIndex))
            {
                bool foundColor = false;
                for (colorIndex = 0; colorIndex < colorEntries.Length; colorIndex++)
                {
                    if (color == colorEntries[colorIndex])
                    {
                        colorCache.Add(color, colorIndex);
                        foundColor = true;
                        break;
                    }
                }
                if (!foundColor)
                {
                    throw new ArgumentException("No such color!");
                }
            }
            pointer[offset] = colorIndex;
        }
    }

    /// <summary>
    /// This is the implementation of the IFastBitmap for 24 bit images (no Alpha)
    /// </summary>
    public unsafe class Fast24RGBBitmap : FastBitmap
    {
        public Fast24RGBBitmap(Bitmap source, Rectangle area) : base(source, area)
        {
        }

        /// <summary>
        /// Retrieve the color at location x,y
        /// Before the first time this is called the Lock() should be called once!
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y Coordinate</param>
        /// <returns>Color</returns>
        public override Color GetColorAt(int x, int y)
        {
            int offset = (x * 3) + (y * stride);
            return Color.FromArgb(255, pointer[PIXELFORMAT_INDEX_R + offset], pointer[PIXELFORMAT_INDEX_G + offset], pointer[PIXELFORMAT_INDEX_B + offset]);
        }

        /// <summary>
        /// Set the color at location x,y
        /// Before the first time this is called the Lock() should be called once!
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public override void SetColorAt(int x, int y, Color color)
        {
            int offset = (x * 3) + (y * stride);
            pointer[PIXELFORMAT_INDEX_R + offset] = color.R;
            pointer[PIXELFORMAT_INDEX_G + offset] = color.G;
            pointer[PIXELFORMAT_INDEX_B + offset] = color.B;
        }

        /// <summary>
        /// Get the color from the specified location into the specified array
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color">byte[4] as reference (r,g,b)</param>
        public override void GetColorAt(int x, int y, byte[] color)
        {
            int offset = (x * 3) + (y * stride);
            color[PIXELFORMAT_INDEX_R] = pointer[PIXELFORMAT_INDEX_R + offset];
            color[PIXELFORMAT_INDEX_G] = pointer[PIXELFORMAT_INDEX_G + offset];
            color[PIXELFORMAT_INDEX_B] = pointer[PIXELFORMAT_INDEX_B + offset];
        }

        /// <summary>
        /// Set the color at the specified location from the specified array
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color">byte[4] as reference (r,g,b)</param>
        public override void SetColorAt(int x, int y, byte[] color)
        {
            int offset = (x * 3) + (y * stride);
            pointer[PIXELFORMAT_INDEX_R + offset] = color[PIXELFORMAT_INDEX_R];
            pointer[PIXELFORMAT_INDEX_G + offset] = color[PIXELFORMAT_INDEX_G];
            pointer[PIXELFORMAT_INDEX_B + offset] = color[PIXELFORMAT_INDEX_B];
        }
    }

    /// <summary>
    /// This is the implementation of the IFastBitmap for 32 bit images (no Alpha)
    /// </summary>
    public unsafe class Fast32RGBBitmap : FastBitmap
    {
        public Fast32RGBBitmap(Bitmap source, Rectangle area) : base(source, area)
        {
        }

        /// <summary>
        /// Retrieve the color at location x,y
        /// Before the first time this is called the Lock() should be called once!
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y Coordinate</param>
        /// <returns>Color</returns>
        public override Color GetColorAt(int x, int y)
        {
            int offset = (x * 4) + (y * stride);
            return Color.FromArgb(255, pointer[PIXELFORMAT_INDEX_R + offset], pointer[PIXELFORMAT_INDEX_G + offset], pointer[PIXELFORMAT_INDEX_B + offset]);
        }

        /// <summary>
        /// Set the color at location x,y
        /// Before the first time this is called the Lock() should be called once!
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public override void SetColorAt(int x, int y, Color color)
        {
            int offset = (x * 4) + (y * stride);
            pointer[PIXELFORMAT_INDEX_R + offset] = color.R;
            pointer[PIXELFORMAT_INDEX_G + offset] = color.G;
            pointer[PIXELFORMAT_INDEX_B + offset] = color.B;
        }

        /// <summary>
        /// Get the color from the specified location into the specified array
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color">byte[4] as reference (a,r,g,b)</param>
        public override void GetColorAt(int x, int y, byte[] color)
        {
            int offset = (x * 4) + (y * stride);
            color[COLOR_INDEX_R] = pointer[PIXELFORMAT_INDEX_R + offset];
            color[COLOR_INDEX_G] = pointer[PIXELFORMAT_INDEX_G + offset];
            color[COLOR_INDEX_B] = pointer[PIXELFORMAT_INDEX_B + offset];
        }

        /// <summary>
        /// Set the color at the specified location from the specified array
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color">byte[4] as reference (r,g,b)</param>
        public override void SetColorAt(int x, int y, byte[] color)
        {
            int offset = (x * 4) + (y * stride);
            pointer[PIXELFORMAT_INDEX_R + offset] = color[COLOR_INDEX_R];   // R
            pointer[PIXELFORMAT_INDEX_G + offset] = color[COLOR_INDEX_G];
            pointer[PIXELFORMAT_INDEX_B + offset] = color[COLOR_INDEX_B];
        }
    }

    /// <summary>
    /// This is the implementation of the IFastBitmap for 32 bit images with Alpha
    /// </summary>
    public unsafe class Fast32ARGBBitmap : FastBitmap, IFastBitmapWithBlend
    {
        public override bool hasAlphaChannel
        {
            get
            {
                return true;
            }
        }

        public Color BackgroundBlendColor
        {
            get;
            set;
        }

        public Fast32ARGBBitmap(Bitmap source, Rectangle area) : base(source, area)
        {
            BackgroundBlendColor = Color.White;
        }

        /// <summary>
        /// Retrieve the color at location x,y
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y Coordinate</param>
        /// <returns>Color</returns>
        public override Color GetColorAt(int x, int y)
        {
            int offset = (x * 4) + (y * stride);
            return Color.FromArgb(pointer[PIXELFORMAT_INDEX_A + offset], pointer[PIXELFORMAT_INDEX_R + offset], pointer[PIXELFORMAT_INDEX_G + offset], pointer[PIXELFORMAT_INDEX_B + offset]);
        }

        /// <summary>
        /// Set the color at location x,y
        /// Before the first time this is called the Lock() should be called once!
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public override void SetColorAt(int x, int y, Color color)
        {
            int offset = (x * 4) + (y * stride);
            pointer[PIXELFORMAT_INDEX_A + offset] = color.A;
            pointer[PIXELFORMAT_INDEX_R + offset] = color.R;
            pointer[PIXELFORMAT_INDEX_G + offset] = color.G;
            pointer[PIXELFORMAT_INDEX_B + offset] = color.B;
        }

        /// <summary>
        /// Get the color from the specified location into the specified array
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color">byte[4] as reference (r,g,b,a)</param>
        public override void GetColorAt(int x, int y, byte[] color)
        {
            int offset = (x * 4) + (y * stride);
            color[COLOR_INDEX_R] = pointer[PIXELFORMAT_INDEX_R + offset];
            color[COLOR_INDEX_G] = pointer[PIXELFORMAT_INDEX_G + offset];
            color[COLOR_INDEX_B] = pointer[PIXELFORMAT_INDEX_B + offset];
            color[COLOR_INDEX_A] = pointer[PIXELFORMAT_INDEX_A + offset];
        }

        /// <summary>
        /// Set the color at the specified location from the specified array
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color">byte[4] as reference (r,g,b,a)</param>
        public override void SetColorAt(int x, int y, byte[] color)
        {
            int offset = (x * 4) + (y * stride);
            pointer[PIXELFORMAT_INDEX_R + offset] = color[COLOR_INDEX_R];   // R
            pointer[PIXELFORMAT_INDEX_G + offset] = color[COLOR_INDEX_G];
            pointer[PIXELFORMAT_INDEX_B + offset] = color[COLOR_INDEX_B];
            pointer[PIXELFORMAT_INDEX_A + offset] = color[COLOR_INDEX_A];
        }

        /// <summary>
        /// Retrieve the color, without alpha (is blended), at location x,y
        /// Before the first time this is called the Lock() should be called once!
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y Coordinate</param>
        /// <returns>Color</returns>
        public Color GetBlendedColorAt(int x, int y)
        {
            int offset = (x * 4) + (y * stride);
            int a = pointer[PIXELFORMAT_INDEX_A + offset];
            int red = pointer[PIXELFORMAT_INDEX_R + offset];
            int green = pointer[PIXELFORMAT_INDEX_G + offset];
            int blue = pointer[PIXELFORMAT_INDEX_B + offset];

            if (a < 255)
            {
                // As the request is to get without alpha, we blend.
                int rem = 255 - a;
                red = (red * a + BackgroundBlendColor.R * rem) / 255;
                green = (green * a + BackgroundBlendColor.G * rem) / 255;
                blue = (blue * a + BackgroundBlendColor.B * rem) / 255;
            }
            return Color.FromArgb(255, red, green, blue);
        }
    }
}