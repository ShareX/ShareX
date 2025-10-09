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
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;

namespace ShareX.HelpersLib
{
    public unsafe class UnsafeBitmap : IDisposable
    {
        public ColorBgra* Pointer { get; private set; }
        public bool IsLocked { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public int PixelCount => Width * Height;

        private Bitmap bitmap;
        private BitmapData bitmapData;

        public UnsafeBitmap(Bitmap bitmap, bool lockBitmap = false, ImageLockMode imageLockMode = ImageLockMode.ReadWrite)
        {
            this.bitmap = bitmap;
            Width = bitmap.Width;
            Height = bitmap.Height;

            if (lockBitmap)
            {
                Lock(imageLockMode);
            }
        }

        public void Lock(ImageLockMode imageLockMode = ImageLockMode.ReadWrite)
        {
            if (!IsLocked)
            {
                IsLocked = true;
                bitmapData = bitmap.LockBits(new Rectangle(0, 0, Width, Height), imageLockMode, PixelFormat.Format32bppArgb);
                Pointer = (ColorBgra*)bitmapData.Scan0.ToPointer();
            }
        }

        public void Unlock()
        {
            if (IsLocked)
            {
                bitmap.UnlockBits(bitmapData);
                bitmapData = null;
                Pointer = null;
                IsLocked = false;
            }
        }

        public static bool operator ==(UnsafeBitmap bmp1, UnsafeBitmap bmp2)
        {
            return ReferenceEquals(bmp1, bmp2) || bmp1.Equals(bmp2);
        }

        public static bool operator !=(UnsafeBitmap bmp1, UnsafeBitmap bmp2)
        {
            return !(bmp1 == bmp2);
        }

        public override bool Equals(object obj)
        {
            return obj is UnsafeBitmap unsafeBitmap && Compare(unsafeBitmap, this);
        }

        public override int GetHashCode()
        {
            return PixelCount;
        }

        public static bool Compare(UnsafeBitmap bmp1, UnsafeBitmap bmp2)
        {
            int pixelCount = bmp1.PixelCount;

            if (pixelCount != bmp2.PixelCount)
            {
                return false;
            }

            bmp1.Lock(ImageLockMode.ReadOnly);
            bmp2.Lock(ImageLockMode.ReadOnly);

            try
            {
                // Create spans over the raw pixel buffers (BGRA as 32-bit uint)
                uint* p1 = (uint*)bmp1.Pointer;
                uint* p2 = (uint*)bmp2.Pointer;
                ReadOnlySpan<uint> s1 = new ReadOnlySpan<uint>(p1, pixelCount);
                ReadOnlySpan<uint> s2 = new ReadOnlySpan<uint>(p2, pixelCount);

                if (Vector.IsHardwareAccelerated)
                {
                    int vecWidth = Vector<uint>.Count;
                    int i = 0;
                    for (; i <= pixelCount - vecWidth; i += vecWidth)
                    {
                        var v1 = new Vector<uint>(s1.Slice(i, vecWidth));
                        var v2 = new Vector<uint>(s2.Slice(i, vecWidth));
                        if (!Vector.EqualsAll(v1, v2))
                        {
                            return false;
                        }
                    }

                    // Tail
                    for (; i < pixelCount; i++)
                    {
                        if (s1[i] != s2[i]) return false;
                    }

                    return true;
                }
                else
                {
                    for (int i = 0; i < pixelCount; i++)
                    {
                        if (s1[i] != s2[i])
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
            finally
            {
                bmp1.Unlock();
                bmp2.Unlock();
            }
        }

        public bool IsTransparent()
        {
            int pixelCount = PixelCount;

            // Span over pixel buffer
            uint* p = (uint*)Pointer;
            ReadOnlySpan<uint> s = new ReadOnlySpan<uint>(p, pixelCount);

            if (Vector.IsHardwareAccelerated)
            {
                int vecWidth = Vector<uint>.Count;
                var alphaMask = new Vector<uint>(0xFF000000u);
                int i = 0;

                for (; i <= pixelCount - vecWidth; i += vecWidth)
                {
                    var v = new Vector<uint>(s.Slice(i, vecWidth));
                    var masked = v & alphaMask;
                    // If not all alphas are 0xFF, then transparency exists
                    if (!Vector.EqualsAll(masked, alphaMask))
                    {
                        return true;
                    }
                }

                for (; i < pixelCount; i++)
                {
                    if ((s[i] & 0xFF000000u) != 0xFF000000u)
                    {
                        return true;
                    }
                }

                return false;
            }
            else
            {
                for (int i = 0; i < pixelCount; i++)
                {
                    if ((s[i] & 0xFF000000u) != 0xFF000000u)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public ColorBgra GetPixel(int i)
        {
            return Pointer[i];
        }

        public ColorBgra GetPixel(int x, int y)
        {
            return Pointer[x + (y * Width)];
        }

        public void SetPixel(int i, ColorBgra color)
        {
            Pointer[i] = color;
        }

        public void SetPixel(int i, uint color)
        {
            Pointer[i] = color;
        }

        public void SetPixel(int x, int y, ColorBgra color)
        {
            Pointer[x + (y * Width)] = color;
        }

        public void SetPixel(int x, int y, uint color)
        {
            Pointer[x + (y * Width)] = color;
        }

        public void ClearPixel(int i)
        {
            Pointer[i] = 0;
        }

        public void ClearPixel(int x, int y)
        {
            Pointer[x + (y * Width)] = 0;
        }

        public void Dispose()
        {
            Unlock();
        }
    }
}