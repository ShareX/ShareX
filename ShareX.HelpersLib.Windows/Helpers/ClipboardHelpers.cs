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
using System.IO;
using System.Linq;
using System.Text;
using Bitmap = SkiaSharp.SKBitmap;
using Image = SkiaSharp.SKBitmap;
using ImageFormat = SkiaSharp.SKEncodedImageFormat;

namespace ShareX.HelpersLib
{
    public static class ClipboardHelpers
    {
        public const string FORMAT_PNG = "PNG";
        public const string FORMAT_17 = "Format17";

        public static bool Clear()
        {
            try { return AvaloniaClipboard.Clear(); }
            catch (Exception e) { DebugHelper.WriteException(e, "Clipboard clear failed."); return false; }
        }

        public static ClipboardData CaptureData() => AvaloniaClipboard.Capture();

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
            if (string.IsNullOrEmpty(text)) return false;
            try { return AvaloniaClipboard.SetText(text); }
            catch (Exception e) { DebugHelper.WriteException(e, "Clipboard copy text failed."); return false; }
        }

        public static bool CopyImage(Image img, string fileName = null)
        {
            if (img == null) return false;
            try
            {
                using MemoryStream png = new();
                img.Save(png, ImageFormat.Png);
                byte[] dib;
                if (HelpersOptions.UseAlternativeClipboardCopyImage && !HelpersOptions.DefaultCopyImageFillBackground)
                    dib = ClipboardHelpersEx.ConvertToDib(img);
                else
                {
                    using Bitmap opaque = SkiaImageHelpers.FillBackground(img, Color.White);
                    using MemoryStream bmp = new();
                    opaque.Save(bmp, ImageFormat.Bmp);
                    dib = bmp.ToArray()[14..];
                }
                return AvaloniaClipboard.SetImage(png.ToArray(), dib,
                    string.IsNullOrEmpty(fileName) ? null : OperatingSystem.IsWindows() ? GenerateHTMLFragment($"<img src=\"{fileName}\"/>") : $"<img src=\"{fileName}\"/>");
            }
            catch (Exception e) { DebugHelper.WriteException(e, "Clipboard copy image failed."); return false; }
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
            if (paths == null || paths.Length == 0) return false;
            try { return AvaloniaClipboard.SetFiles(paths); }
            catch (Exception e) { DebugHelper.WriteException(e, "Clipboard copy file failed."); return false; }
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
                byte[] png = AvaloniaClipboard.GetImage();
                if (png != null) return SkiaImageHelpers.ByteArrayToBitmap(png);
                ClipboardData data = CaptureData();
                foreach (string format in new[] { FORMAT_PNG, "image/png", FORMAT_17, ClipboardDataFormats.Dib })
                {
                    if (data.GetData(format) is not byte[] bytes) continue;
                    Bitmap image = format is FORMAT_PNG or "image/png" ? SkiaImageHelpers.ByteArrayToBitmap(bytes) :
                        ClipboardHelpersEx.ImageFromClipboardDib(bytes);
                    if (image != null) return image;
                }
            }
            catch (Exception e) { DebugHelper.WriteException(e, "Clipboard get image failed."); }
            return null;
        }

        public static Bitmap GetImageAlternative2() => GetImage();

        public static string GetText(bool checkContainsText = false)
        {
            try { return AvaloniaClipboard.GetText(); }
            catch (Exception e) { DebugHelper.WriteException(e, "Clipboard get text failed."); return null; }
        }

        public static string[] GetFileDropList(bool checkContainsFileDropList = false)
        {
            try { return AvaloniaClipboard.GetFiles(); }
            catch (Exception e) { DebugHelper.WriteException(e, "Clipboard get files failed."); return null; }
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

        public static bool ContainsImage() => Contains(ClipboardDataFormats.Bitmap);
        public static bool ContainsText() => Contains(ClipboardDataFormats.Text);
        public static bool ContainsFileDropList() => Contains(ClipboardDataFormats.FileDrop);

        private static bool Contains(string format)
        {
            try { return AvaloniaClipboard.Contains(format); }
            catch (Exception e) { DebugHelper.WriteException(e); return false; }
        }
    }
}
