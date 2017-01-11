#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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
using System.Globalization;
using System.Runtime.InteropServices;

namespace ShareX.HelpersLib
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left, Top, Right, Bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public RECT(Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom)
        {
        }

        public int X
        {
            get { return Left; }
            set { Right -= Left - value; Left = value; }
        }

        public int Y
        {
            get { return Top; }
            set { Bottom -= Top - value; Top = value; }
        }

        public int Width
        {
            get { return Right - Left; }
            set { Right = value + Left; }
        }

        public int Height
        {
            get { return Bottom - Top; }
            set { Bottom = value + Top; }
        }

        public Point Location
        {
            get { return new Point(Left, Top); }
            set { X = value.X; Y = value.Y; }
        }

        public Size Size
        {
            get { return new Size(Width, Height); }
            set { Width = value.Width; Height = value.Height; }
        }

        public static implicit operator Rectangle(RECT r)
        {
            return new Rectangle(r.Left, r.Top, r.Width, r.Height);
        }

        public static implicit operator RECT(Rectangle r)
        {
            return new RECT(r);
        }

        public static bool operator ==(RECT r1, RECT r2)
        {
            return r1.Equals(r2);
        }

        public static bool operator !=(RECT r1, RECT r2)
        {
            return !r1.Equals(r2);
        }

        public bool Equals(RECT r)
        {
            return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
        }

        public override bool Equals(object obj)
        {
            if (obj is RECT)
            {
                return Equals((RECT)obj);
            }

            if (obj is Rectangle)
            {
                return Equals(new RECT((Rectangle)obj));
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ((Rectangle)this).GetHashCode();
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SIZE
    {
        public int Width;
        public int Height;

        public SIZE(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public static explicit operator Size(SIZE s)
        {
            return new Size(s.Width, s.Height);
        }

        public static explicit operator SIZE(Size s)
        {
            return new SIZE(s.Width, s.Height);
        }

        public override string ToString()
        {
            return string.Format("{0}x{1}", Width, Height);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static explicit operator Point(POINT p)
        {
            return new Point(p.X, p.Y);
        }

        public static explicit operator POINT(Point p)
        {
            return new POINT(p.X, p.Y);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWINFO
    {
        public uint cbSize;
        public RECT rcWindow;
        public RECT rcClient;
        public uint dwStyle;
        public uint dwExStyle;
        public uint dwWindowStatus;
        public uint cxWindowBorders;
        public uint cyWindowBorders;
        public ushort atomWindowType;
        public ushort wCreatorVersion;

        public WINDOWINFO(bool? filler) : this() // Allows automatic initialization of "cbSize" with "new WINDOWINFO(null/true/false)".
        {
            cbSize = (uint)Marshal.SizeOf(typeof(WINDOWINFO));
        }
    }

    public struct WINDOWPLACEMENT
    {
        public int length;
        public int flags;
        public WindowShowStyle showCmd;
        public POINT ptMinPosition;
        public POINT ptMaxPosition;
        public RECT rcNormalPosition;
    }

    public struct BLENDFUNCTION
    {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct APPBARDATA
    {
        public int cbSize;
        public IntPtr hWnd;
        public int uCallbackMessage;
        public int uEdge;
        public RECT rc;
        public IntPtr lParam;

        public static APPBARDATA NewAPPBARDATA()
        {
            APPBARDATA abd = new APPBARDATA();
            abd.cbSize = Marshal.SizeOf(typeof(APPBARDATA));
            return abd;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DWM_BLURBEHIND
    {
        public DWM_BB dwFlags;
        public bool fEnable;
        public IntPtr hRgnBlur;
        public bool fTransitionOnMaximized;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MARGINS
    {
        public int leftWidth;
        public int rightWidth;
        public int topHeight;
        public int bottomHeight;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DWM_THUMBNAIL_PROPERTIES
    {
        public int dwFlags;
        public RECT rcDestination;
        public RECT rcSource;
        public byte opacity;
        public bool fVisible;
        public bool fSourceClientAreaOnly;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CursorInfo
    {
        public Int32 cbSize; // Specifies the size, in bytes, of the structure.
        public Int32 flags; // Specifies the cursor state. This parameter can be one of the following values:
        public IntPtr hCursor; // Handle to the cursor.
        public Point ptScreenPos; // A POINT structure that receives the screen coordinates of the cursor.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct IconInfo
    {
        public bool fIcon; // Specifies whether this structure defines an icon or a cursor. A value of TRUE specifies
        public Int32 xHotspot; // Specifies the x-coordinate of a cursor's hot spot. If this structure defines an icon, the hot
        public Int32 yHotspot; // Specifies the y-coordinate of the cursor's hot spot. If this structure defines an icon, the hot
        public IntPtr hbmMask; // (HBITMAP) Specifies the icon bitmask bitmap. If this structure defines a black and white icon,
        public IntPtr hbmColor; // (HBITMAP) Handle to the icon color bitmap. This member can be optional if this
    }

    /// <summary>
    /// Structure, which contains information for a single stream .
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public struct AVISTREAMINFO
    {
        /// <summary>
        /// Four-character code indicating the stream type.
        /// </summary>
        ///
        [MarshalAs(UnmanagedType.I4)]
        public int type;

        /// <summary>
        /// Four-character code of the compressor handler that will compress this video stream when it is saved.
        /// </summary>
        ///
        [MarshalAs(UnmanagedType.I4)]
        public int handler;

        /// <summary>
        /// Applicable flags for the stream.
        /// </summary>
        ///
        [MarshalAs(UnmanagedType.I4)]
        public int flags;

        /// <summary>
        /// Capability flags; currently unused.
        /// </summary>
        ///
        [MarshalAs(UnmanagedType.I4)]
        public int Capabilities;

        /// <summary>
        /// Priority of the stream.
        /// </summary>
        ///
        [MarshalAs(UnmanagedType.I2)]
        public short priority;

        /// <summary>
        /// Language of the stream.
        /// </summary>
        ///
        [MarshalAs(UnmanagedType.I2)]
        public short language;

        /// <summary>
        /// Time scale applicable for the stream.
        /// </summary>
        ///
        /// <remarks>Dividing <b>rate</b> by <b>scale</b> gives the playback rate in number of samples per second.</remarks>
        ///
        [MarshalAs(UnmanagedType.I4)]
        public int scale;

        /// <summary>
        /// Rate in an integer format.
        /// </summary>
        ///
        [MarshalAs(UnmanagedType.I4)]
        public int rate;

        /// <summary>
        /// Sample number of the first frame of the AVI file.
        /// </summary>
        ///
        [MarshalAs(UnmanagedType.I4)]
        public int start;

        /// <summary>
        /// Length of this stream.
        /// </summary>
        ///
        /// <remarks>The units are defined by <b>rate</b> and <b>scale</b>.</remarks>
        ///
        [MarshalAs(UnmanagedType.I4)]
        public int length;

        /// <summary>
        /// Audio skew. This member specifies how much to skew the audio data ahead of the video frames in interleaved files.
        /// </summary>
        ///
        [MarshalAs(UnmanagedType.I4)]
        public int initialFrames;

        /// <summary>
        /// Recommended buffer size, in bytes, for the stream.
        /// </summary>
        ///
        [MarshalAs(UnmanagedType.I4)]
        public int suggestedBufferSize;

        /// <summary>
        /// Quality indicator of the video data in the stream.
        /// </summary>
        ///
        /// <remarks>Quality is represented as a number between 0 and 10,000.</remarks>
        ///
        [MarshalAs(UnmanagedType.I4)]
        public int quality;

        /// <summary>
        /// Size, in bytes, of a single data sample.
        /// </summary>
        ///
        [MarshalAs(UnmanagedType.I4)]
        public int sampleSize;

        /// <summary>
        /// Dimensions of the video destination rectangle.
        /// </summary>
        ///
        [MarshalAs(UnmanagedType.Struct, SizeConst = 16)]
        public RECT rectFrame;

        /// <summary>
        /// Number of times the stream has been edited.
        /// </summary>
        ///
        [MarshalAs(UnmanagedType.I4)]
        public int editCount;

        /// <summary>
        /// Number of times the stream format has changed.
        /// </summary>
        ///
        [MarshalAs(UnmanagedType.I4)]
        public int formatChangeCount;

        /// <summary>
        /// Description of the stream.
        /// </summary>
        ///
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string name;
    }

    /// <summary>
    /// Structure, which contains information about a stream and how it is compressed and saved.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct AVICOMPRESSOPTIONS
    {
        /// <summary>
        /// Four-character code indicating the stream type.
        /// </summary>
        ///
        [MarshalAs(UnmanagedType.I4)]
        public int type;

        /// <summary>
        /// Four-character code for the compressor handler that will compress this video stream when it is saved.
        /// </summary>
        ///
        [MarshalAs(UnmanagedType.I4)]
        public int handler;

        /// <summary>
        /// Maximum period between video key frames.
        /// </summary>
        ///
        [MarshalAs(UnmanagedType.I4)]
        public int keyFrameEvery;

        /// <summary>
        /// Quality value passed to a video compressor.
        /// </summary>
        ///
        [MarshalAs(UnmanagedType.I4)]
        public int quality;

        /// <summary>
        /// Video compressor data rate.
        /// </summary>
        ///
        [MarshalAs(UnmanagedType.I4)]
        public int bytesPerSecond;

        /// <summary>
        /// Flags used for compression.
        /// </summary>
        ///
        [MarshalAs(UnmanagedType.I4)]
        public int flags;

        /// <summary>
        /// Pointer to a structure defining the data format.
        /// </summary>
        ///
        [MarshalAs(UnmanagedType.I4)]
        public int format;

        /// <summary>
        /// Size, in bytes, of the data referenced by <b>format</b>.
        /// </summary>
        ///
        [MarshalAs(UnmanagedType.I4)]
        public int formatSize;

        /// <summary>
        /// Video-compressor-specific data; used internally.
        /// </summary>
        ///
        [MarshalAs(UnmanagedType.I4)]
        public int parameters;

        /// <summary>
        /// Size, in bytes, of the data referenced by <b>parameters</b>.
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public int parametersSize;

        /// <summary>
        /// Interleave factor for interspersing stream data with data from the first stream.
        /// </summary>
        ///
        [MarshalAs(UnmanagedType.I4)]
        public int interleaveEvery;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct BITMAPFILEHEADER
    {
        public static readonly short BM = 0x4d42;
        public short bfType;
        public int bfSize;
        public short bfReserved1;
        public short bfReserved2;
        public int bfOffBits;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct BITMAPINFOHEADER
    {
        [FieldOffset(0)]
        public uint biSize;
        [FieldOffset(4)]
        public int biWidth;
        [FieldOffset(8)]
        public int biHeight;
        [FieldOffset(12)]
        public ushort biPlanes;
        [FieldOffset(14)]
        public ushort biBitCount;
        [FieldOffset(16)]
        public BI_COMPRESSION biCompression;
        [FieldOffset(20)]
        public uint biSizeImage;
        [FieldOffset(24)]
        public int biXPelsPerMeter;
        [FieldOffset(28)]
        public int biYPelsPerMeter;
        [FieldOffset(32)]
        public uint biClrUsed;
        [FieldOffset(36)]
        public uint biClrImportant;
        [FieldOffset(40)]
        public uint bV5RedMask;
        [FieldOffset(44)]
        public uint bV5GreenMask;
        [FieldOffset(48)]
        public uint bV5BlueMask;
        [FieldOffset(52)]
        public uint bV5AlphaMask;
        [FieldOffset(56)]
        public uint bV5CSType;
        [FieldOffset(60)]
        public CIEXYZTRIPLE bV5Endpoints;
        [FieldOffset(96)]
        public uint bV5GammaRed;
        [FieldOffset(100)]
        public uint bV5GammaGreen;
        [FieldOffset(104)]
        public uint bV5GammaBlue;
        [FieldOffset(108)]
        public uint bV5Intent;
        [FieldOffset(112)]
        public uint bV5ProfileData;
        [FieldOffset(116)]
        public uint bV5ProfileSize;
        [FieldOffset(120)]
        public uint bV5Reserved;

        public const int DIB_RGB_COLORS = 0;

        public BITMAPINFOHEADER(int width, int height, ushort bpp)
        {
            biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER));
            biPlanes = 1;
            biCompression = BI_COMPRESSION.BI_RGB;
            biWidth = width;
            biHeight = height;
            biBitCount = bpp;
            biSizeImage = (uint)(width * height * (bpp >> 3));
            biXPelsPerMeter = 0;
            biYPelsPerMeter = 0;
            biClrUsed = 0;
            biClrImportant = 0;
            bV5RedMask = (uint)255 << 16;
            bV5GreenMask = (uint)255 << 8;
            bV5BlueMask = (uint)255;
            bV5AlphaMask = (uint)255 << 24;
            bV5CSType = 1934772034;
            bV5Endpoints = new CIEXYZTRIPLE();
            bV5Endpoints.ciexyzBlue = new CIEXYZ(0);
            bV5Endpoints.ciexyzGreen = new CIEXYZ(0);
            bV5Endpoints.ciexyzRed = new CIEXYZ(0);
            bV5GammaRed = 0;
            bV5GammaGreen = 0;
            bV5GammaBlue = 0;
            bV5Intent = 4;
            bV5ProfileData = 0;
            bV5ProfileSize = 0;
            bV5Reserved = 0;
        }

        public uint OffsetToPixels
        {
            get
            {
                if (biCompression == BI_COMPRESSION.BI_BITFIELDS)
                {
                    return biSize + 3 * 4;
                }

                return biSize;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CIEXYZ
    {
        public uint ciexyzX;
        public uint ciexyzY;
        public uint ciexyzZ;

        public CIEXYZ(uint FXPT2DOT30)
        {
            ciexyzX = FXPT2DOT30;
            ciexyzY = FXPT2DOT30;
            ciexyzZ = FXPT2DOT30;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CIEXYZTRIPLE
    {
        public CIEXYZ ciexyzRed;
        public CIEXYZ ciexyzGreen;
        public CIEXYZ ciexyzBlue;
    }

    public struct INPUT
    {
        public InputType Type;
        public InputUnion Data;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct InputUnion
    {
        [FieldOffset(0)]
        public MOUSEINPUT Mouse;

        [FieldOffset(0)]
        public KEYBDINPUT Keyboard;

        [FieldOffset(0)]
        public HARDWAREINPUT Hardware;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public MouseEventFlags dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KEYBDINPUT
    {
        public VirtualKeyCode wVk;
        public ushort wScan;
        public KeyboardEventFlags dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HARDWAREINPUT
    {
        public int uMsg;
        public short wParamL;
        public short wParamH;
    }

    [ComImport]
    [Guid("0000010D-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IViewObject
    {
        void Draw([MarshalAs(UnmanagedType.U4)] uint dwAspect, int lindex, IntPtr pvAspect, [In] IntPtr ptd, IntPtr hdcTargetDev, IntPtr hdcDraw, [MarshalAs(UnmanagedType.Struct)] ref RECT lprcBounds, [In] IntPtr lprcWBounds, IntPtr pfnContinue, [MarshalAs(UnmanagedType.U4)] uint dwContinue);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FLASHWINFO
    {
        public uint cbSize;
        public IntPtr hwnd;
        public uint dwFlags;
        public uint uCount;
        public uint dwTimeout;
    }

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct SCROLLINFO
    {
        public uint cbSize;
        public uint fMask;
        public int nMin;
        public int nMax;
        public uint nPage;
        public int nPos;
        public int nTrackPos;
    }
}