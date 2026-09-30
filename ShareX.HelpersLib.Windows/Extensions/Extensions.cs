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

using MessageBox = ShareX.AvaloniaUI.MessageBox;
using MessageBoxButtons = ShareX.AvaloniaUI.MessageBoxButtons;
using MessageBoxIcon = ShareX.AvaloniaUI.MessageBoxIcon;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;

namespace ShareX.HelpersLib
{
    // Windows-only members of Extensions, kept in the same namespace so existing call sites keep compiling.
    public static class ExtensionsWindows
    {
        public static void DisposeHandle(this Icon icon)
        {
            if (icon.Handle != IntPtr.Zero)
            {
                NativeMethods.DestroyIcon(icon.Handle);
            }
        }


        public static byte[] GetBytes(this Image img)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, img.RawFormat);
                return ms.ToArray();
            }
        }

        public static Stream GetStream(this Image img)
        {
            MemoryStream ms = new MemoryStream();
            img.Save(ms, img.RawFormat);
            return ms;
        }

        public static ImageCodecInfo GetCodecInfo(this ImageFormat format)
        {
            return ImageCodecInfo.GetImageEncoders().FirstOrDefault(info => info.FormatID.Equals(format.Guid));
        }

        public static string GetMimeType(this ImageFormat format)
        {
            ImageCodecInfo codec = format.GetCodecInfo();

            if (codec != null) return codec.MimeType;

            return "image/unknown";
        }

        public static Icon ToIcon(this Bitmap bmp)
        {
            IntPtr handle = bmp.GetHicon();
            return Icon.FromHandle(handle);
        }

        public static Bitmap CreateEmptyBitmap(this Image img, int widthOffset = 0, int heightOffset = 0, PixelFormat pixelFormat = PixelFormat.Format32bppArgb)
        {
            Bitmap bmp = new Bitmap(img.Width + widthOffset, img.Height + heightOffset, pixelFormat);
            bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution);
            return bmp;
        }

        public static Bitmap CreateEmptyBitmap(this Image img, PixelFormat pixelFormat)
        {
            return img.CreateEmptyBitmap(0, 0, pixelFormat);
        }
    }
}
