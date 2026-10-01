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
using Bitmap = SkiaSharp.SKBitmap;
using Image = SkiaSharp.SKBitmap;
using ImageFormat = SkiaSharp.SKEncodedImageFormat;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using SkiaSharp;

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

        public static Bitmap ConvertClipboardDibToBitmap(byte[] data)
        {
            return ClipboardHelpersEx.ImageFromClipboardDib(data);
        }

        public static Bitmap ConvertClipboardDibV5ToBitmap(byte[] data)
        {
            return ClipboardHelpersEx.DIBV5ToBitmap(data);
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
            if (img == null) return false;
            try
            {
                using MemoryStream png = new();
                using MemoryStream dib = new();
                DataObject data = new();
                img.Save(png, ImageFormat.Png);
                data.SetData(FORMAT_PNG, false, png);
                if (HelpersOptions.UseAlternativeClipboardCopyImage && !HelpersOptions.DefaultCopyImageFillBackground)
                {
                    byte[] bytes = ClipboardHelpersEx.ConvertToDib(img);
                    dib.Write(bytes);
                }
                else
                {
                    using Bitmap opaque = SkiaImageHelpers.FillBackground(img, Color.White);
                    using MemoryStream bmp = new();
                    opaque.Save(bmp, ImageFormat.Bmp);
                    bmp.CopyStreamTo(dib, 14, (int)bmp.Length - 14);
                }
                data.SetData(DataFormats.Dib, false, dib);
                if (!string.IsNullOrEmpty(fileName))
                    data.SetData(DataFormats.Html, GenerateHTMLFragment($"<img src=\"{fileName}\"/>"));
                return CopyData(data);
            }
            catch (Exception exception) { DebugHelper.WriteException(exception, "Clipboard copy image failed."); return false; }
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
                    using (Bitmap bmp = SkiaImageHelpers.LoadImage(path))
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
                    return GetImageAlternative2();
                }
            }
            catch (Exception exception) { DebugHelper.WriteException(exception, "Clipboard get image failed."); return null; }
        }

        public static Bitmap GetImageAlternative2()
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data == null) return null;
            foreach (string format in new[] { FORMAT_PNG, FORMAT_17, DataFormats.Dib })
            {
                if (data.GetData(format, true) is not MemoryStream stream) continue;
                byte[] bytes = stream.ToArray();
                Bitmap image = format == FORMAT_PNG ? SkiaImageHelpers.ByteArrayToBitmap(bytes) : ClipboardHelpersEx.ImageFromClipboardDib(bytes);
                if (image != null) return image;
            }
            return null;
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
                        return SkiaImageHelpers.LoadImage(imageFilePath);
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
                return Clipboard.ContainsImage() || Clipboard.ContainsData(FORMAT_PNG) || Clipboard.ContainsData(FORMAT_17) || Clipboard.ContainsData(DataFormats.Dib);
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
