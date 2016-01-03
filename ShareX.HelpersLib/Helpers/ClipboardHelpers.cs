#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public static class ClipboardHelpers
    {
        private const int RetryTimes = 20, RetryDelay = 100;

        private static readonly object ClipboardLock = new object();

        private static bool CopyData(IDataObject data, bool copy = true)
        {
            if (data != null)
            {
                lock (ClipboardLock)
                {
                    Clipboard.SetDataObject(data, copy, RetryTimes, RetryDelay);
                }

                return true;
            }

            return false;
        }

        public static bool Clear()
        {
            try
            {
                IDataObject data = new DataObject();
                CopyData(data, false);
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e, "Clipboard clear failed.");
            }

            return false;
        }

        public static bool CopyText(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                try
                {
                    IDataObject data = new DataObject();
                    string dataFormat;

                    if (Environment.OSVersion.Platform != PlatformID.Win32NT || Environment.OSVersion.Version.Major < 5)
                    {
                        dataFormat = DataFormats.Text;
                    }
                    else
                    {
                        dataFormat = DataFormats.UnicodeText;
                    }

                    data.SetData(dataFormat, false, text);
                    return CopyData(data);
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e, "Clipboard copy text failed.");
                }
            }

            return false;
        }

        public static bool CopyImage(Image img)
        {
            if (img != null)
            {
                try
                {
                    if (HelpersOptions.UseAlternativeCopyImage)
                    {
                        return CopyImageAlternative(img);
                    }

                    if (HelpersOptions.DefaultCopyImageFillBackground)
                    {
                        return CopyImageDefaultFillBackground(img, Color.White);
                    }

                    return CopyImageDefault(img);
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e, "Clipboard copy image failed.");
                }
            }

            return false;
        }

        private static bool CopyImageDefault(Image img)
        {
            IDataObject dataObject = new DataObject();
            dataObject.SetData(DataFormats.Bitmap, true, img);

            return CopyData(dataObject);
        }

        private static bool CopyImageDefaultFillBackground(Image img, Color background)
        {
            using (Bitmap bmp = img.CreateEmptyBitmap(PixelFormat.Format24bppRgb))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(background);
                g.DrawImage(img, 0, 0, img.Width, img.Height);

                IDataObject dataObject = new DataObject();
                dataObject.SetData(DataFormats.Bitmap, true, bmp);

                return CopyData(dataObject);
            }
        }

        private static bool CopyImageAlternative(Image img)
        {
            using (MemoryStream msPNG = new MemoryStream())
            using (MemoryStream msBMP = new MemoryStream())
            using (MemoryStream msDIB = new MemoryStream())
            {
                IDataObject dataObject = new DataObject();

                img.Save(msPNG, ImageFormat.Png);
                dataObject.SetData("PNG", false, msPNG);

                img.Save(msBMP, ImageFormat.Bmp);
                msBMP.CopyStreamTo(msDIB, 14, (int)msBMP.Length - 14);
                dataObject.SetData(DataFormats.Dib, true, msDIB);

                return CopyData(dataObject);
            }
        }

        public static bool CopyFile(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                return CopyFile(new string[] { path });
            }

            return false;
        }

        public static bool CopyFile(string[] paths)
        {
            if (paths != null && paths.Length > 0)
            {
                try
                {
                    IDataObject dataObject = new DataObject();
                    dataObject.SetData(DataFormats.FileDrop, true, paths);

                    return CopyData(dataObject);
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e, "Clipboard copy file failed.");
                }
            }

            return false;
        }

        public static bool CopyTextFromFile(string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                try
                {
                    string text = File.ReadAllText(path, Encoding.UTF8);
                    return CopyText(text);
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e, "Clipboard copy text from file failed.");
                }
            }

            return false;
        }

        public static bool CopyImageFromFile(string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                try
                {
                    using (Image img = ImageHelpers.LoadImage(path))
                    {
                        return CopyImage(img);
                    }
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e, "Clipboard copy image from file failed.");
                }
            }

            return false;
        }

        public static Image GetImage()
        {
            try
            {
                lock (ClipboardLock)
                {
                    if (HelpersOptions.UseAlternativeGetImage)
                    {
                        return GetImageAlternative();
                    }

                    return Clipboard.GetImage();
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e, "Clipboard get image failed.");
            }

            return null;
        }

        private static Image GetImageAlternative()
        {
            IDataObject dataObject = Clipboard.GetDataObject();

            if (dataObject != null)
            {
                string[] dataFormats = dataObject.GetFormats(false);

                if (dataFormats.Contains("PNG"))
                {
                    using (MemoryStream ms = (MemoryStream)dataObject.GetData("PNG"))
                    {
                        return (Image)Image.FromStream(ms).Clone();
                    }
                }

                if (dataFormats.Contains(DataFormats.Dib))
                {
                    byte[] dib;

                    using (MemoryStream ms = (MemoryStream)dataObject.GetData(DataFormats.Dib))
                    {
                        dib = ms.ToArray();
                    }

                    short bpp = BitConverter.ToInt16(dib, 14);

                    if (bpp == 32)
                    {
                        GCHandle gch = GCHandle.Alloc(dib, GCHandleType.Pinned);

                        try
                        {
                            int width = BitConverter.ToInt32(dib, 4);
                            int height = BitConverter.ToInt32(dib, 8);
                            int stride = width * 4;
                            IntPtr ptr = new IntPtr((long)gch.AddrOfPinnedObject() + 40);

                            using (Bitmap bmp = new Bitmap(width, height, stride, PixelFormat.Format32bppArgb, ptr))
                            {
                                Image img = (Image)bmp.Clone();
                                img.RotateFlip(RotateFlipType.Rotate180FlipX);
                                return img;
                            }
                        }
                        finally
                        {
                            gch.Free();
                        }
                    }
                }

                if (dataFormats.Contains(DataFormats.Bitmap) || dataFormats.Contains(typeof(Bitmap).FullName))
                {
                    return dataObject.GetData(DataFormats.Bitmap, true) as Image;
                }
            }

            return null;
        }
    }
}