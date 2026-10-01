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
using System.Drawing;
using SkiaSharp;
using System.Runtime.InteropServices;

namespace ShareX.HelpersLib
{
    public class CursorData
    {
        public IntPtr Handle { get; private set; }
        public Point Position { get; private set; }
        public Size Size { get; private set; }
        public float SizeMultiplier { get; private set; }
        public bool IsDefaultSize => SizeMultiplier == 1f;
        public Point Hotspot { get; private set; }
        public Point DrawPosition => new Point(Position.X - Hotspot.X, Position.Y - Hotspot.Y);
        public bool IsVisible { get; private set; }

        public CursorData()
        {
            UpdateCursorData();
        }

        public void UpdateCursorData()
        {
            Handle = IntPtr.Zero;
            Position = Point.Empty;
            IsVisible = false;

            CursorInfo cursorInfo = new CursorInfo();
            cursorInfo.cbSize = Marshal.SizeOf(cursorInfo);

            if (NativeMethods.GetCursorInfo(out cursorInfo))
            {
                Handle = cursorInfo.hCursor;
                Position = cursorInfo.ptScreenPos;
                Size = Size.Empty;
                SizeMultiplier = GetCursorSizeMultiplier();
                IsVisible = cursorInfo.flags == NativeConstants.CURSOR_SHOWING;

                if (IsVisible)
                {
                    IntPtr iconHandle = NativeMethods.CopyIcon(Handle);

                    if (iconHandle != IntPtr.Zero)
                    {
                        if (NativeMethods.GetIconInfo(iconHandle, out IconInfo iconInfo))
                        {
                            if (IsDefaultSize)
                            {
                                Hotspot = new Point(iconInfo.xHotspot, iconInfo.yHotspot);
                            }
                            else
                            {
                                Hotspot = new Point((int)Math.Round(iconInfo.xHotspot * SizeMultiplier), (int)Math.Round(iconInfo.yHotspot * SizeMultiplier));
                            }

                            if (iconInfo.hbmColor != IntPtr.Zero)
                            {
                                NativeMethods.DeleteObject(iconInfo.hbmColor);
                            }

                            if (iconInfo.hbmMask != IntPtr.Zero)
                            {
                                if (!IsDefaultSize)
                                {
                                    Size maskSize = WindowsImageInterop.GetBitmapSize(iconInfo.hbmMask);
                                    int cursorWidth = maskSize.Width;
                                    int cursorHeight = iconInfo.hbmColor != IntPtr.Zero ? maskSize.Height : maskSize.Height / 2;
                                    Size = new Size((int)Math.Round(cursorWidth * SizeMultiplier), (int)Math.Round(cursorHeight * SizeMultiplier));
                                }

                                NativeMethods.DeleteObject(iconInfo.hbmMask);
                            }
                        }

                        NativeMethods.DestroyIcon(iconHandle);
                    }
                }
            }
        }

        public static float GetCursorSizeMultiplier()
        {
            float sizeMultiplier = 1f;

            int? cursorSize = RegistryHelpers.GetValueDWord(@"SOFTWARE\Microsoft\Accessibility", "CursorSize");

            if (cursorSize != null && cursorSize > 1)
            {
                sizeMultiplier = 1f + ((cursorSize.Value - 1) * 0.5f);
            }

            return sizeMultiplier;
        }

        public void DrawCursor(IntPtr hdcDest)
        {
            DrawCursor(hdcDest, Point.Empty);
        }

        public void DrawCursor(IntPtr hdcDest, Point offset)
        {
            if (IsVisible)
            {
                Point drawPosition = new Point(DrawPosition.X - offset.X, DrawPosition.Y - offset.Y);

                NativeMethods.DrawIconEx(hdcDest, drawPosition.X, drawPosition.Y, Handle, Size.Width, Size.Height, 0, IntPtr.Zero, NativeConstants.DI_NORMAL);
            }
        }

        public void DrawCursor(SKBitmap image) => DrawCursor(image, Point.Empty);

        public void DrawCursor(SKBitmap image, Point offset)
        {
            if (!IsVisible) return;
            using SKBitmap cursor = ToBitmap();
            using SKCanvas canvas = new(image);
            Point position = new(DrawPosition.X - offset.X, DrawPosition.Y - offset.Y);
            canvas.DrawBitmap(cursor, position.X, position.Y);
        }

        public SKBitmap ToBitmap() => WindowsImageInterop.FromIcon(Handle, Size);
    }
}
