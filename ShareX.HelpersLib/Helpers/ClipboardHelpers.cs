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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public static class ClipboardHelpers
    {
        public const string FORMAT_PNG = "PNG";
        public const string FORMAT_17 = "Format17";

        private const int RetryTimes = 20;
        private const int RetryDelay = 100;

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

        public static bool CopyImage(Image img, string fileName = null)
        {
            if (img != null)
            {
                try
                {
                    if (HelpersOptions.UseAlternativeClipboardCopyImage)
                    {
                        return CopyImageAlternative2(img, fileName);
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
                dataObject.SetData(FORMAT_PNG, false, msPNG);

                img.Save(msBMP, ImageFormat.Bmp);
                msBMP.CopyStreamTo(msDIB, 14, (int)msBMP.Length - 14);
                dataObject.SetData(DataFormats.Dib, true, msDIB);

                return CopyData(dataObject);
            }
        }

        private static bool CopyImageAlternative2(Image img, string fileName = null)
        {
            using (Bitmap bmpNonTransparent = img.CreateEmptyBitmap(PixelFormat.Format24bppRgb))
            using (MemoryStream msPNG = new MemoryStream())
            using (MemoryStream msDIB = new MemoryStream())
            {
                IDataObject dataObject = new DataObject();

                using (Graphics g = Graphics.FromImage(bmpNonTransparent))
                {
                    g.Clear(Color.White);
                    g.DrawImage(img, 0, 0, img.Width, img.Height);
                }

                dataObject.SetData(DataFormats.Bitmap, true, bmpNonTransparent);

                img.Save(msPNG, ImageFormat.Png);
                dataObject.SetData(FORMAT_PNG, false, msPNG);

                byte[] dibData = ClipboardHelpersEx.ConvertToDib(img);
                msDIB.Write(dibData, 0, dibData.Length);
                dataObject.SetData(DataFormats.Dib, false, msDIB);

                if (!string.IsNullOrEmpty(fileName))
                {
                    string htmlFragment = GenerateHTMLFragment($"<img src=\"{fileName}\"/>");
                    dataObject.SetData(DataFormats.Html, htmlFragment);
                }

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

        public static bool CopyImageFromFile(string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                try
                {
                    using (Bitmap bmp = ImageHelpers.LoadImage(path))
                    {
                        string fileName = Path.GetFileName(path);
                        return CopyImage(bmp, fileName);
                    }
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e, "Clipboard copy image from file failed.");
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

        public static Bitmap GetImage(bool checkContainsImage = false)
        {
            try
            {
                lock (ClipboardLock)
                {
                    if (!checkContainsImage || Clipboard.ContainsImage())
                    {
                        if (HelpersOptions.UseAlternativeClipboardGetImage)
                        {
                            return GetImageAlternative2();
                        }

                        return (Bitmap)Clipboard.GetImage();
                    }
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

                if (dataFormats.Contains(FORMAT_PNG))
                {
                    using (MemoryStream ms = dataObject.GetData(FORMAT_PNG) as MemoryStream)
                    {
                        if (ms != null)
                        {
                            using (Image img = Image.FromStream(ms))
                            {
                                return (Image)img.Clone();
                            }
                        }
                    }
                }
                else
                {
                    foreach (string format in new[] { DataFormats.Dib, FORMAT_17 })
                    {
                        if (dataFormats.Contains(format))
                        {
                            using (MemoryStream ms = dataObject.GetData(format) as MemoryStream)
                            {
                                if (ms != null)
                                {
                                    try
                                    {
                                        return GetDIBImage(ms);
                                    }
                                    catch (Exception e)
                                    {
                                        DebugHelper.WriteException(e);
                                    }
                                }
                            }
                        }
                    }
                }

                if (dataObject.GetDataPresent(DataFormats.Bitmap, true))
                {
                    return dataObject.GetData(DataFormats.Bitmap, true) as Image;
                }
            }

            return null;
        }

        public static Bitmap GetImageAlternative2()
        {
            IDataObject dataObject = Clipboard.GetDataObject();

            if (dataObject != null)
            {
                string[] dataFormats = dataObject.GetFormats(false);

                if (dataFormats.Contains(FORMAT_PNG))
                {
                    using (MemoryStream ms = dataObject.GetData(FORMAT_PNG) as MemoryStream)
                    {
                        if (ms != null)
                        {
                            using (Bitmap bmp = new Bitmap(ms))
                            {
                                return ClipboardHelpersEx.CloneImage(bmp);
                            }
                        }
                    }
                }
                else if (dataFormats.Contains(DataFormats.Dib))
                {
                    using (MemoryStream ms = dataObject.GetData(DataFormats.Dib) as MemoryStream)
                    {
                        if (ms != null)
                        {
                            return ClipboardHelpersEx.ImageFromClipboardDib(ms.ToArray());
                        }
                    }
                }
                else if (dataFormats.Contains(FORMAT_17))
                {
                    using (MemoryStream ms = dataObject.GetData(FORMAT_17) as MemoryStream)
                    {
                        if (ms != null)
                        {
                            return ClipboardHelpersEx.DIBV5ToBitmap(ms.ToArray());
                        }
                    }
                }
                else if (dataFormats.Contains(DataFormats.Bitmap))
                {
                    return dataObject.GetData(DataFormats.Bitmap, true) as Bitmap;
                }
            }

            return null;
        }

        private static Image GetDIBImage(MemoryStream ms)
        {
            byte[] dib = ms.ToArray();

            BITMAPINFOHEADER infoHeader = Helpers.ByteArrayToStructure<BITMAPINFOHEADER>(dib);

            IntPtr gcHandle = IntPtr.Zero;

            try
            {
                GCHandle handle = GCHandle.Alloc(dib, GCHandleType.Pinned);
                gcHandle = GCHandle.ToIntPtr(handle);

                if (infoHeader.biSizeImage == 0)
                {
                    infoHeader.biSizeImage = (uint)(infoHeader.biWidth * infoHeader.biHeight * (infoHeader.biBitCount >> 3));
                }

                using (Bitmap bmp = new Bitmap(infoHeader.biWidth, infoHeader.biHeight, -(int)(infoHeader.biSizeImage / infoHeader.biHeight),
                    infoHeader.biBitCount == 32 ? PixelFormat.Format32bppArgb : PixelFormat.Format24bppRgb,
                    new IntPtr((long)handle.AddrOfPinnedObject() + infoHeader.OffsetToPixels + ((infoHeader.biHeight - 1) * (int)(infoHeader.biSizeImage / infoHeader.biHeight)))))
                {
                    return new Bitmap(bmp);
                }
            }
            finally
            {
                if (gcHandle != IntPtr.Zero)
                {
                    GCHandle.FromIntPtr(gcHandle).Free();
                }
            }
        }

        public static string GetText(bool checkContainsText = false)
        {
            try
            {
                lock (ClipboardLock)
                {
                    if (!checkContainsText || Clipboard.ContainsText())
                    {
                        return Clipboard.GetText();
                    }
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e, "Clipboard get text failed.");
            }

            return null;
        }

        public static string[] GetFileDropList(bool checkContainsFileDropList = false)
        {
            try
            {
                lock (ClipboardLock)
                {
                    if (!checkContainsFileDropList || Clipboard.ContainsFileDropList())
                    {
                        return Clipboard.GetFileDropList().Cast<string>().ToArray();
                    }
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e, "Clipboard get file drop list failed.");
            }

            return null;
        }

        public static Bitmap TryGetImage()
        {
            if (ContainsImage())
            {
                return GetImage();
            }
            else if (ContainsFileDropList())
            {
                string[] files = GetFileDropList();

                if (files != null)
                {
                    string imageFilePath = files.FirstOrDefault(x => FileHelpers.IsImageFile(x));

                    if (!string.IsNullOrEmpty(imageFilePath))
                    {
                        return ImageHelpers.LoadImage(imageFilePath);
                    }
                }
            }

            return null;
        }

        private static string GenerateHTMLFragment(string html)
        {
            StringBuilder sb = new StringBuilder();

            string header = "Version:0.9\r\nStartHTML:<<<<<<<<<1\r\nEndHTML:<<<<<<<<<2\r\nStartFragment:<<<<<<<<<3\r\nEndFragment:<<<<<<<<<4\r\n";
            string startHTML = "<html>\r\n<body>\r\n";
            string startFragment = "<!--StartFragment-->";
            string endFragment = "<!--EndFragment-->";
            string endHTML = "\r\n</body>\r\n</html>";

            sb.Append(header);

            int startHTMLLength = header.Length;
            int startFragmentLength = startHTMLLength + startHTML.Length + startFragment.Length;
            int endFragmentLength = startFragmentLength + Encoding.UTF8.GetByteCount(html);
            int endHTMLLength = endFragmentLength + endFragment.Length + endHTML.Length;

            sb.Replace("<<<<<<<<<1", startHTMLLength.ToString("D10"));
            sb.Replace("<<<<<<<<<2", endHTMLLength.ToString("D10"));
            sb.Replace("<<<<<<<<<3", startFragmentLength.ToString("D10"));
            sb.Replace("<<<<<<<<<4", endFragmentLength.ToString("D10"));

            sb.Append(startHTML);
            sb.Append(startFragment);
            sb.Append(html);
            sb.Append(endFragment);
            sb.Append(endHTML);

            return sb.ToString();
        }

        public static bool ContainsImage()
        {
            try
            {
                return Clipboard.ContainsImage();
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return false;
        }

        public static bool ContainsText()
        {
            try
            {
                return Clipboard.ContainsText();
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return false;
        }

        public static bool ContainsFileDropList()
        {
            try
            {
                return Clipboard.ContainsFileDropList();
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return false;
        }
    }
}