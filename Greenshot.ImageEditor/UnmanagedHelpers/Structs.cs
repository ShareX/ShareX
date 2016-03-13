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
using System.Drawing;
using System.Runtime.InteropServices;

namespace GreenshotPlugin.UnmanagedHelpers
{
    [StructLayout(LayoutKind.Sequential), Serializable()]
    public struct SIZE
    {
        public int width;
        public int height;

        public SIZE(Size size) : this(size.Width, size.Height)
        {
        }

        public SIZE(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public Size ToSize()
        {
            return new Size(width, height);
        }
    }

    [StructLayout(LayoutKind.Sequential), Serializable()]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }

        public POINT(Point point)
        {
            X = point.X;
            Y = point.Y;
        }

        public static implicit operator Point(POINT p)
        {
            return new Point(p.X, p.Y);
        }

        public static implicit operator POINT(Point p)
        {
            return new POINT(p.X, p.Y);
        }

        public Point ToPoint()
        {
            return new Point(X, Y);
        }

        override public string ToString()
        {
            return X + "," + Y;
        }
    }

    [StructLayout(LayoutKind.Sequential), Serializable()]
    public struct RECT
    {
        private int _Left;
        private int _Top;
        private int _Right;
        private int _Bottom;

        public RECT(RECT rectangle)
            : this(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom)
        {
        }

        public RECT(Rectangle rectangle)
            : this(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom)
        {
        }

        public RECT(int Left, int Top, int Right, int Bottom)
        {
            _Left = Left;
            _Top = Top;
            _Right = Right;
            _Bottom = Bottom;
        }

        public int X
        {
            get
            {
                return _Left;
            }
            set
            {
                _Left = value;
            }
        }
        public int Y
        {
            get
            {
                return _Top;
            }
            set
            {
                _Top = value;
            }
        }
        public int Left
        {
            get
            {
                return _Left;
            }
            set
            {
                _Left = value;
            }
        }
        public int Top
        {
            get
            {
                return _Top;
            }
            set
            {
                _Top = value;
            }
        }
        public int Right
        {
            get
            {
                return _Right;
            }
            set
            {
                _Right = value;
            }
        }
        public int Bottom
        {
            get
            {
                return _Bottom;
            }
            set
            {
                _Bottom = value;
            }
        }
        public int Height
        {
            get
            {
                return _Bottom - _Top;
            }
            set
            {
                _Bottom = value - _Top;
            }
        }
        public int Width
        {
            get
            {
                return _Right - _Left;
            }
            set
            {
                _Right = value + _Left;
            }
        }
        public Point Location
        {
            get
            {
                return new Point(Left, Top);
            }
            set
            {
                _Left = value.X;
                _Top = value.Y;
            }
        }
        public Size Size
        {
            get
            {
                return new Size(Width, Height);
            }
            set
            {
                _Right = value.Width + _Left;
                _Bottom = value.Height + _Top;
            }
        }

        public static implicit operator Rectangle(RECT Rectangle)
        {
            return new Rectangle(Rectangle.Left, Rectangle.Top, Rectangle.Width, Rectangle.Height);
        }

        public static implicit operator RECT(Rectangle Rectangle)
        {
            return new RECT(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom);
        }

        public static bool operator ==(RECT Rectangle1, RECT Rectangle2)
        {
            return Rectangle1.Equals(Rectangle2);
        }

        public static bool operator !=(RECT Rectangle1, RECT Rectangle2)
        {
            return !Rectangle1.Equals(Rectangle2);
        }

        public override string ToString()
        {
            return "{Left: " + _Left + "; " + "Top: " + _Top + "; Right: " + _Right + "; Bottom: " + _Bottom + "}";
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public bool Equals(RECT Rectangle)
        {
            return Rectangle.Left == _Left && Rectangle.Top == _Top && Rectangle.Right == _Right && Rectangle.Bottom == _Bottom;
        }

        public Rectangle ToRectangle()
        {
            return new Rectangle(Left, Top, Width, Height);
        }

        public override bool Equals(object Object)
        {
            if (Object is RECT)
            {
                return Equals((RECT)Object);
            }
            else if (Object is Rectangle)
            {
                return Equals(new RECT((Rectangle)Object));
            }

            return false;
        }
    }

    /// <summary>
    /// A floating point GDI Plus width/hight based rectangle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RECTF
    {
        /// <summary>
        /// The X corner location of the rectangle.
        /// </summary>
        public float X;

        /// <summary>
        /// The Y corner location of the rectangle.
        /// </summary>
        public float Y;

        /// <summary>
        /// The width of the rectangle.
        /// </summary>
        public float Width;

        /// <summary>
        /// The height of the rectangle.
        /// </summary>
        public float Height;

        /// <summary>
        /// Creates a new GDI Plus rectangle.
        /// </summary>
        /// <param name="x">The X corner location of the rectangle.</param>
        /// <param name="y">The Y corner location of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        public RECTF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Creates a new GDI Plus rectangle from a System.Drawing.RectangleF.
        /// </summary>
        /// <param name="rect">The rectangle to base this GDI Plus rectangle on.</param>
        public RECTF(RectangleF rect)
        {
            X = rect.X;
            Y = rect.Y;
            Width = rect.Width;
            Height = rect.Height;
        }

        /// <summary>
        /// Creates a new GDI Plus rectangle from a System.Drawing.Rectangle.
        /// </summary>
        /// <param name="rect">The rectangle to base this GDI Plus rectangle on.</param>
        public RECTF(Rectangle rect)
        {
            X = rect.X;
            Y = rect.Y;
            Width = rect.Width;
            Height = rect.Height;
        }

        /// <summary>
        /// Returns a RectangleF for this GDI Plus rectangle.
        /// </summary>
        /// <returns>A System.Drawing.RectangleF structure.</returns>
        public RectangleF ToRectangle()
        {
            return new RectangleF(X, Y, Width, Height);
        }

        /// <summary>
        /// Returns a RectangleF for a GDI Plus rectangle.
        /// </summary>
        /// <param name="rect">The GDI Plus rectangle to get the RectangleF for.</param>
        /// <returns>A System.Drawing.RectangleF structure.</returns>
        public static RectangleF ToRectangle(RECTF rect)
        {
            return rect.ToRectangle();
        }

        /// <summary>
        /// Returns a GDI Plus rectangle for a RectangleF structure.
        /// </summary>
        /// <param name="rect">The RectangleF to get the GDI Plus rectangle for.</param>
        /// <returns>A GDI Plus rectangle structure.</returns>
        public static RECTF FromRectangle(RectangleF rect)
        {
            return new RECTF(rect);
        }

        /// <summary>
        /// Returns a GDI Plus rectangle for a Rectangle structure.
        /// </summary>
        /// <param name="rect">The Rectangle to get the GDI Plus rectangle for.</param>
        /// <returns>A GDI Plus rectangle structure.</returns>
        public static RECTF FromRectangle(Rectangle rect)
        {
            return new RECTF(rect);
        }
    }

    /// <summary>
    /// The structure for the WindowInfo
    /// See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms632610%28v=vs.85%29.aspx
    /// </summary>
    [StructLayout(LayoutKind.Sequential), Serializable()]
    public struct WindowInfo
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

        // Allows automatic initialization of "cbSize" with "new WINDOWINFO(null/true/false)".
        public WindowInfo(Boolean? filler) : this()
        {
            cbSize = (UInt32)(Marshal.SizeOf(typeof(WindowInfo)));
        }
    }

    /// <summary>
    /// Contains information about the placement of a window on the screen.
    /// </summary>
    [StructLayout(LayoutKind.Sequential), Serializable()]
    public struct WindowPlacement
    {
        /// <summary>
        /// The length of the structure, in bytes. Before calling the GetWindowPlacement or SetWindowPlacement functions, set this member to sizeof(WINDOWPLACEMENT).
        /// <para>
        /// GetWindowPlacement and SetWindowPlacement fail if this member is not set correctly.
        /// </para>
        /// </summary>
        public int Length;

        /// <summary>
        /// Specifies flags that control the position of the minimized window and the method by which the window is restored.
        /// </summary>
        public WindowPlacementFlags Flags;

        /// <summary>
        /// The current show state of the window.
        /// </summary>
        public ShowWindowCommand ShowCmd;

        /// <summary>
        /// The coordinates of the window's upper-left corner when the window is minimized.
        /// </summary>
        public POINT MinPosition;

        /// <summary>
        /// The coordinates of the window's upper-left corner when the window is maximized.
        /// </summary>
        public POINT MaxPosition;

        /// <summary>
        /// The window's coordinates when the window is in the restored position.
        /// </summary>
        public RECT NormalPosition;

        /// <summary>
        /// Gets the default (empty) value.
        /// </summary>
        public static WindowPlacement Default
        {
            get
            {
                WindowPlacement result = new WindowPlacement();
                result.Length = Marshal.SizeOf(result);
                return result;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CursorInfo
    {
        public Int32 cbSize;
        public Int32 flags;
        public IntPtr hCursor;
        public POINT ptScreenPos;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct IconInfo
    {
        public bool fIcon;
        public Int32 xHotspot;
        public Int32 yHotspot;
        public IntPtr hbmMask;
        public IntPtr hbmColor;
    }

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct SCROLLINFO
    {
        public int cbSize;
        public int fMask;
        public int nMin;
        public int nMax;
        public int nPage;
        public int nPos;
        public int nTrackPos;
    }
}