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

using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using System;

namespace ShareX.HelpersLib
{
    // Windows-only members of Helpers, kept in the same namespace so existing call sites keep compiling.
    public static class HelpersWindows
    {
        private static Cursor[] cursorList;

        extension(Helpers)
        {
            public static Cursor[] CursorList
            {
                get
                {
                    if (cursorList == null)
                    {
                        cursorList = new Cursor[] {
                            Cursors.AppStarting, Cursors.Arrow, Cursors.Cross, Cursors.Default, Cursors.Hand, Cursors.Help,
                            Cursors.HSplit, Cursors.IBeam, Cursors.No, Cursors.NoMove2D, Cursors.NoMoveHoriz, Cursors.NoMoveVert,
                            Cursors.PanEast, Cursors.PanNE, Cursors.PanNorth, Cursors.PanNW, Cursors.PanSE, Cursors.PanSouth,
                            Cursors.PanSW, Cursors.PanWest, Cursors.SizeAll, Cursors.SizeNESW, Cursors.SizeNS, Cursors.SizeNWSE,
                            Cursors.SizeWE, Cursors.UpArrow, Cursors.VSplit, Cursors.WaitCursor
                        };
                    }

                    return cursorList;
                }
            }

            public static Cursor CreateCursor(byte[] data)
            {
                using (MemoryStream ms = new MemoryStream(data))
                {
                    return new Cursor(ms);
                }
            }

            public static void LockCursorToWindow(Form form)
            {
                form.Activated += (sender, e) => Cursor.Clip = form.Bounds;
                form.Deactivate += (sender, e) => Cursor.Clip = Rectangle.Empty;
            }



            public static void PlaySound(Stream stream)
            {
                if (stream != null)
                {
                    Task.Run(() =>
                    {
                        using (stream)
                        using (SoundPlayer soundPlayer = new SoundPlayer(stream))
                        {
                            soundPlayer.Play();
                        }
                    });
                }
            }

            public static void PlaySoundSync(Stream stream)
            {
                if (stream != null)
                {
                    Task.Run(() =>
                    {
                        using (stream)
                        using (SoundPlayer soundPlayer = new SoundPlayer(stream))
                        {
                            soundPlayer.PlaySync();
                        }
                    });
                }
            }

            public static void PlaySoundAsync(string filePath)
            {
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    Task.Run(() =>
                    {
                        using (SoundPlayer soundPlayer = new SoundPlayer(filePath))
                        {
                            soundPlayer.PlaySync();
                        }
                    });
                }
            }

            public static Icon GetProgressIcon(int percentage, Color color)
            {
                percentage = percentage.Clamp(0, 100);

                Size size = SystemInformation.SmallIconSize;

                using (Bitmap bmp = new Bitmap(size.Width, size.Height))
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    using (Brush brush = new SolidBrush(Color.FromArgb(39, 39, 39)))
                    {
                        g.FillRectangle(brush, 0, 0, size.Width, size.Height);
                    }

                    int y = (int)(size.Height * (percentage / 100f));

                    if (y > 0)
                    {
                        using (Brush brush = new SolidBrush(color))
                        {
                            g.FillRectangle(brush, 0, size.Height - y, size.Width, y);
                        }

                        if (y < size.Height)
                        {
                            using (Pen pen = new Pen(ColorHelpers.LighterColor(color, 0.3f)))
                            {
                                g.DrawLine(pen, 0, size.Height - y, size.Width - 1, size.Height - y);
                            }
                        }
                    }

                    using (Font font = new Font("Arial", 10))
                    using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        percentage = percentage.Clamp(0, 99);

                        g.DrawString(percentage.ToString(), font, Brushes.White, size.Width / 2f, size.Height / 2f, sf);
                    }

                    bmp.SetPixel(0, 0, Color.Transparent);
                    bmp.SetPixel(bmp.Width - 1, 0, Color.Transparent);
                    bmp.SetPixel(0, bmp.Height - 1, Color.Transparent);
                    bmp.SetPixel(bmp.Width - 1, bmp.Height - 1, Color.Transparent);

                    return Icon.FromHandle(bmp.GetHicon());
                }
            }

            public static void LockCursorToWindow(Avalonia.Controls.Window window)
            {
                window.Activated += (sender, e) =>
                {
                    IntPtr handle = window.TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
                    if (handle != IntPtr.Zero)
                    {
                        Rectangle bounds = NativeMethods.GetWindowRect(handle);
                        if (bounds.Width > 0 && bounds.Height > 0)
                        {
                            Cursor.Clip = bounds;
                        }
                    }
                };
                window.Deactivated += (sender, e) => Cursor.Clip = Rectangle.Empty;
                window.Closed += (sender, e) => Cursor.Clip = Rectangle.Empty;
            }


            public static Icon GetProgressIcon(int percentage)
            {
                return GetProgressIcon(percentage, Color.FromArgb(16, 116, 193));
            }

        }
    }
}
