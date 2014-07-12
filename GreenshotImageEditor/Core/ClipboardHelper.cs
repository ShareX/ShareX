/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2013  Thomas Braun, Jens Klingen, Robin Krom
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

using Greenshot.IniFile;
using Greenshot.Plugin;
using GreenshotPlugin.UnmanagedHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GreenshotPlugin.Core
{
    /// <summary>
    /// Description of ClipboardHelper.
    /// </summary>
    public static class ClipboardHelper
    {
        private static readonly Object clipboardLockObject = new Object();
        private static readonly CoreConfiguration config = IniConfig.GetIniSection<CoreConfiguration>();
        private static readonly string FORMAT_FILECONTENTS = "FileContents";
        private static readonly string FORMAT_PNG = "PNG";
        private static readonly string FORMAT_PNG_OFFICEART = "PNG+Office Art";
        private static readonly string FORMAT_JPG = "JPG";
        private static readonly string FORMAT_JFIF = "JFIF";
        private static readonly string FORMAT_JFIF_OFFICEART = "JFIF+Office Art";
        private static readonly string FORMAT_GIF = "GIF";
        private static readonly string FORMAT_BITMAP_PLACEHOLDER = "_BITMAP_";

        private static IntPtr nextClipboardViewer = IntPtr.Zero;
        // Template for the HTML Text on the clipboard
        // see: http://msdn.microsoft.com/en-us/library/ms649015%28v=vs.85%29.aspx
        // or:  http://msdn.microsoft.com/en-us/library/Aa767917.aspx
        private const string HTML_CLIPBOARD_STRING = @"Version:0.9
StartHTML:<<<<<<<1
EndHTML:<<<<<<<2
StartFragment:<<<<<<<3
EndFragment:<<<<<<<4
StartSelection:<<<<<<<3
EndSelection:<<<<<<<4
<!DOCTYPE>
<HTML>
<HEAD>
<TITLE>Greenshot capture</TITLE>
</HEAD>
<BODY>
<!--StartFragment -->
<img border='0' src='file:///${file}' width='${width}' height='${height}'>
<!--EndFragment -->
</BODY>
</HTML>";
        private const string HTML_CLIPBOARD_BASE64_STRING = @"Version:0.9
StartHTML:<<<<<<<1
EndHTML:<<<<<<<2
StartFragment:<<<<<<<3
EndFragment:<<<<<<<4
StartSelection:<<<<<<<3
EndSelection:<<<<<<<4
<!DOCTYPE>
<HTML>
<HEAD>
<TITLE>Greenshot capture</TITLE>
</HEAD>
<BODY>
<!--StartFragment -->
<img border='0' src='data:image/${format};base64,${data}' width='${width}' height='${height}'>
<!--EndFragment -->
</BODY>
</HTML>";

        /// <summary>
        /// Get the current "ClipboardOwner" but only if it isn't us!
        /// </summary>
        /// <returns>current clipboard owner</returns>
        private static string GetClipboardOwner()
        {
            string owner = null;
            try
            {
                IntPtr hWnd = User32.GetClipboardOwner();
                if (hWnd != IntPtr.Zero)
                {
                    IntPtr pid = IntPtr.Zero;
                    IntPtr tid = User32.GetWindowThreadProcessId(hWnd, out pid);
                    Process me = Process.GetCurrentProcess();
                    Process ownerProcess = Process.GetProcessById(pid.ToInt32());
                    // Exclude myself
                    if (ownerProcess != null && me.Id != ownerProcess.Id)
                    {
                        // Get Process Name
                        owner = ownerProcess.ProcessName;
                        // Try to get the starting Process Filename, this might fail.
                        try
                        {
                            owner = ownerProcess.Modules[0].FileName;
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LOG.Warn("Non critical error: Couldn't get clipboard owner.", e);
            }
            return owner;
        }

        /// <summary>
        /// The SetDataObject will lock/try/catch clipboard operations making it save and not show exceptions.
        /// The bool "copy" is used to decided if the information stays on the clipboard after exit.
        /// </summary>
        /// <param name="ido"></param>
        /// <param name="copy"></param>
        private static void SetDataObject(IDataObject ido, bool copy)
        {
            lock (clipboardLockObject)
            {
                int retryCount = 2;
                while (retryCount >= 0)
                {
                    try
                    {
                        Clipboard.SetDataObject(ido, copy);
                        break;
                    }
                    catch (Exception ee)
                    {
                        if (retryCount == 0)
                        {
                            string messageText = null;
                            string clipboardOwner = GetClipboardOwner();
                            if (clipboardOwner != null)
                            {
                                messageText = Language.GetFormattedString("clipboard_inuse", clipboardOwner);
                            }
                            else
                            {
                                messageText = Language.GetString("clipboard_error");
                            }
                            LOG.Error(messageText, ee);
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }
                    }
                    finally
                    {
                        --retryCount;
                    }
                }
            }
        }

        /// <summary>
        /// The GetDataObject will lock/try/catch clipboard operations making it save and not show exceptions.
        /// </summary>
        public static IDataObject GetDataObject()
        {
            lock (clipboardLockObject)
            {
                int retryCount = 2;
                while (retryCount >= 0)
                {
                    try
                    {
                        return Clipboard.GetDataObject();
                    }
                    catch (Exception ee)
                    {
                        if (retryCount == 0)
                        {
                            string messageText = null;
                            string clipboardOwner = GetClipboardOwner();
                            if (clipboardOwner != null)
                            {
                                messageText = Language.GetFormattedString("clipboard_inuse", clipboardOwner);
                            }
                            else
                            {
                                messageText = Language.GetString("clipboard_error");
                            }
                            LOG.Error(messageText, ee);
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }
                    }
                    finally
                    {
                        --retryCount;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Wrapper for Clipboard.ContainsText, Created for Bug #3432313
        /// </summary>
        /// <returns>boolean if there is text on the clipboard</returns>
        public static bool ContainsText()
        {
            IDataObject clipboardData = GetDataObject();
            return ContainsText(clipboardData);
        }

        /// <summary>
        /// Test if the IDataObject contains Text
        /// </summary>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        public static bool ContainsText(IDataObject dataObject)
        {
            if (dataObject != null)
            {
                if (dataObject.GetDataPresent(DataFormats.Text) || dataObject.GetDataPresent(DataFormats.UnicodeText))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Wrapper for Clipboard.ContainsImage, specialized for Greenshot, Created for Bug #3432313
        /// </summary>
        /// <returns>boolean if there is an image on the clipboard</returns>
        public static bool ContainsImage()
        {
            IDataObject clipboardData = GetDataObject();
            return ContainsImage(clipboardData);
        }

        /// <summary>
        /// Check if the IDataObject has an image
        /// </summary>
        /// <param name="dataObject"></param>
        /// <returns>true if an image is there</returns>
        public static bool ContainsImage(IDataObject dataObject)
        {
            if (dataObject != null)
            {
                if (dataObject.GetDataPresent(DataFormats.Bitmap)
                    || dataObject.GetDataPresent(DataFormats.Dib)
                    || dataObject.GetDataPresent(DataFormats.Tiff)
                    || dataObject.GetDataPresent(DataFormats.EnhancedMetafile)
                    || dataObject.GetDataPresent(FORMAT_PNG)
                    || dataObject.GetDataPresent(FORMAT_JPG)
                    || dataObject.GetDataPresent(FORMAT_GIF))
                {
                    return true;
                }
                List<string> imageFiles = GetImageFilenames(dataObject);
                if (imageFiles != null && imageFiles.Count > 0)
                {
                    return true;
                }
                if (dataObject.GetDataPresent(FORMAT_FILECONTENTS))
                {
                    try
                    {
                        MemoryStream imageStream = dataObject.GetData(FORMAT_FILECONTENTS) as MemoryStream;
                        if (isValidStream(imageStream))
                        {
                            using (Image tmpImage = Image.FromStream(imageStream))
                            {
                                // If we get here, there is an image
                                return true;
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Simple helper to check the stream
        /// </summary>
        /// <param name="memoryStream"></param>
        /// <returns></returns>
        private static bool isValidStream(MemoryStream memoryStream)
        {
            return memoryStream != null && memoryStream.Length > 0;
        }

        /// <summary>
        /// Wrapper for Clipboard.GetImage, Created for Bug #3432313
        /// </summary>
        /// <returns>Image if there is an image on the clipboard</returns>
        public static Image GetImage()
        {
            IDataObject clipboardData = GetDataObject();
            // Return the first image
            foreach (Image clipboardImage in GetImages(clipboardData))
            {
                return clipboardImage;
            }
            return null;
        }

        /// <summary>
        /// Get all images (multiple if filenames are available) from the dataObject
        /// Returned images must be disposed by the calling code!
        /// </summary>
        /// <param name="dataObject"></param>
        /// <returns>IEnumerable<Image></returns>
        public static IEnumerable<Image> GetImages(IDataObject dataObject)
        {
            // Get single image, this takes the "best" match
            Image singleImage = GetImage(dataObject);
            if (singleImage != null)
            {
                LOG.InfoFormat("Got image from clipboard with size {0} and format {1}", singleImage.Size, singleImage.PixelFormat);
                yield return singleImage;
            }
            else
            {
                // check if files are supplied
                List<string> imageFiles = GetImageFilenames(dataObject);
                if (imageFiles != null)
                {
                    foreach (string imageFile in imageFiles)
                    {
                        Image returnImage = null;
                        try
                        {
                            returnImage = ImageHelper.LoadImage(imageFile);
                        }
                        catch (Exception streamImageEx)
                        {
                            LOG.Error("Problem retrieving Image from clipboard.", streamImageEx);
                        }
                        if (returnImage != null)
                        {
                            LOG.InfoFormat("Got image from clipboard with size {0} and format {1}", returnImage.Size, returnImage.PixelFormat);
                            yield return returnImage;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get an Image from the IDataObject, don't check for FileDrop
        /// </summary>
        /// <param name="dataObject"></param>
        /// <returns>Image or null</returns>
        private static Image GetImage(IDataObject dataObject)
        {
            Image returnImage = null;
            if (dataObject != null)
            {
                IList<string> formats = GetFormats(dataObject);
                string[] retrieveFormats;

                // Found a weird bug, where PNG's from Outlook 2010 are clipped
                // So I build some special logik to get the best format:
                if (formats != null && formats.Contains(FORMAT_PNG_OFFICEART) && formats.Contains(DataFormats.Dib))
                {
                    // Outlook ??
                    LOG.Info("Most likely the current clipboard contents come from Outlook, as this has a problem with PNG and others we place the DIB format to the front...");
                    retrieveFormats = new string[] { DataFormats.Dib, FORMAT_BITMAP_PLACEHOLDER, FORMAT_FILECONTENTS, FORMAT_PNG_OFFICEART, FORMAT_PNG, FORMAT_JFIF_OFFICEART, FORMAT_JPG, FORMAT_JFIF, DataFormats.Tiff, FORMAT_GIF };
                }
                else
                {
                    retrieveFormats = new string[] { FORMAT_PNG_OFFICEART, FORMAT_PNG, FORMAT_JFIF_OFFICEART, FORMAT_JPG, FORMAT_JFIF, DataFormats.Tiff, DataFormats.Dib, FORMAT_BITMAP_PLACEHOLDER, FORMAT_FILECONTENTS, FORMAT_GIF };
                }
                foreach (string currentFormat in retrieveFormats)
                {
                    if (FORMAT_BITMAP_PLACEHOLDER.Equals(currentFormat))
                    {
                        LOG.Info("Using default .NET Clipboard.GetImage()");
                        try
                        {
                            returnImage = Clipboard.GetImage();
                            if (returnImage != null)
                            {
                                return returnImage;
                            }
                            else
                            {
                                LOG.Info("Clipboard.GetImage() didn't return an image.");
                            }
                        }
                        catch (Exception ex)
                        {
                            LOG.Error("Problem retrieving Image via Clipboard.GetImage(): ", ex);
                        }
                    }
                    else if (formats.Contains(currentFormat))
                    {
                        LOG.InfoFormat("Found {0}, trying to retrieve.", currentFormat);
                        if (currentFormat == DataFormats.Dib)
                        {
                            returnImage = GetDIBImage(dataObject);
                        }
                        else
                        {
                            returnImage = GetImageFormat(currentFormat, dataObject);
                        }
                        if (returnImage != null)
                        {
                            return returnImage;
                        }
                    }
                    else
                    {
                        LOG.DebugFormat("Couldn't find format {0}.", currentFormat);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// the DIB readed should solve the issue reported here: https://sourceforge.net/projects/greenshot/forums/forum/676083/topic/6354353/index/page/1
        /// </summary>
        /// <returns>Image</returns>
        private static Image GetDIBImage(IDataObject dataObejct)
        {
            try
            {
                // If the EnableSpecialDIBClipboardReader flag in the config is set, use the code from:
                // http://www.thomaslevesque.com/2009/02/05/wpf-paste-an-image-from-the-clipboard/
                // to read the DeviceIndependentBitmap from the clipboard, this might fix bug 3576125
                if (config.EnableSpecialDIBClipboardReader)
                {
                    MemoryStream dibStream = GetFromDataObject(dataObejct, DataFormats.Dib) as MemoryStream;
                    if (isValidStream(dibStream))
                    {
                        LOG.Info("Found valid DIB stream, trying to process it.");
                        byte[] dibBuffer = new byte[dibStream.Length];
                        dibStream.Read(dibBuffer, 0, dibBuffer.Length);
                        BitmapInfoHeader infoHeader = BinaryStructHelper.FromByteArray<BitmapInfoHeader>(dibBuffer);
                        // Only use this code, when the biCommpression != 0 (BI_RGB)
                        if (infoHeader.biCompression != 0)
                        {
                            LOG.InfoFormat("Using special DIB format reader for biCompression {0}", infoHeader.biCompression);
                            int fileHeaderSize = Marshal.SizeOf(typeof(BitmapFileHeader));
                            uint infoHeaderSize = infoHeader.biSize;
                            int fileSize = (int)(fileHeaderSize + infoHeader.biSize + infoHeader.biSizeImage);

                            BitmapFileHeader fileHeader = new BitmapFileHeader();
                            fileHeader.bfType = BitmapFileHeader.BM;
                            fileHeader.bfSize = fileSize;
                            fileHeader.bfReserved1 = 0;
                            fileHeader.bfReserved2 = 0;
                            fileHeader.bfOffBits = (int)(fileHeaderSize + infoHeaderSize + infoHeader.biClrUsed * 4);

                            byte[] fileHeaderBytes = BinaryStructHelper.ToByteArray<BitmapFileHeader>(fileHeader);

                            using (MemoryStream bitmapStream = new MemoryStream())
                            {
                                bitmapStream.Write(fileHeaderBytes, 0, fileHeaderSize);
                                bitmapStream.Write(dibBuffer, 0, dibBuffer.Length);
                                bitmapStream.Seek(0, SeekOrigin.Begin);
                                using (Image tmpImage = Image.FromStream(bitmapStream))
                                {
                                    if (tmpImage != null)
                                    {
                                        return ImageHelper.Clone(tmpImage);
                                    }
                                }
                            }
                        }
                        else
                        {
                            LOG.InfoFormat("Skipping special DIB format reader for biCompression {0}", infoHeader.biCompression);
                        }
                    }
                }
                else
                {
                    LOG.Info("Skipping special DIB format reader as it's disabled in the configuration.");
                }
            }
            catch (Exception dibEx)
            {
                LOG.Error("Problem retrieving DIB from clipboard.", dibEx);
            }
            return null;
        }

        /// <summary>
        /// Helper method to try to get an image in the specified format from the dataObject
        /// </summary>
        /// <param name="format">string with the format</param>
        /// <param name="dataObject">IDataObject</param>
        /// <returns>Image or null</returns>
        private static Image GetImageFormat(string format, IDataObject dataObject)
        {
            MemoryStream imageStream = GetFromDataObject(dataObject, format) as MemoryStream;
            if (isValidStream(imageStream))
            {
                try
                {
                    using (FileStream fs = new FileStream(@"C:\Localdata\test.png", FileMode.OpenOrCreate))
                    {
                        imageStream.WriteTo(fs);
                    }
                    imageStream.Seek(0, SeekOrigin.Begin);
                    using (Image tmpImage = Image.FromStream(imageStream, true, true))
                    {
                        if (tmpImage != null)
                        {
                            LOG.InfoFormat("Got image with clipboard format {0} from the clipboard.", format);
                            return ImageHelper.Clone(tmpImage);
                        }
                    }
                }
                catch (Exception streamImageEx)
                {
                    LOG.Error(string.Format("Problem retrieving {0} from clipboard.", format), streamImageEx);
                }
            }
            return null;
        }

        /// <summary>
        /// Wrapper for Clipboard.GetText created for Bug #3432313
        /// </summary>
        /// <returns>string if there is text on the clipboard</returns>
        public static string GetText()
        {
            return GetText(GetDataObject());
        }

        /// <summary>
        /// Get Text from the DataObject
        /// </summary>
        /// <returns>string if there is text on the clipboard</returns>
        public static string GetText(IDataObject dataObject)
        {
            if (ContainsText(dataObject))
            {
                return (String)dataObject.GetData(DataFormats.Text);
            }
            return null;
        }

        /// <summary>
        /// Set text to the clipboard
        /// </summary>
        /// <param name="text"></param>
        public static void SetClipboardData(string text)
        {
            IDataObject ido = new DataObject();
            ido.SetData(DataFormats.Text, true, text);
            SetDataObject(ido, true);
        }

        private static string getHTMLString(ISurface surface, string filename)
        {
            string utf8EncodedHTMLString = Encoding.GetEncoding(0).GetString(Encoding.UTF8.GetBytes(HTML_CLIPBOARD_STRING));
            utf8EncodedHTMLString = utf8EncodedHTMLString.Replace("${width}", surface.Image.Width.ToString());
            utf8EncodedHTMLString = utf8EncodedHTMLString.Replace("${height}", surface.Image.Height.ToString());
            utf8EncodedHTMLString = utf8EncodedHTMLString.Replace("${file}", filename);
            StringBuilder sb = new StringBuilder();
            sb.Append(utf8EncodedHTMLString);
            sb.Replace("<<<<<<<1", (utf8EncodedHTMLString.IndexOf("<HTML>") + "<HTML>".Length).ToString("D8"));
            sb.Replace("<<<<<<<2", (utf8EncodedHTMLString.IndexOf("</HTML>")).ToString("D8"));
            sb.Replace("<<<<<<<3", (utf8EncodedHTMLString.IndexOf("<!--StartFragment -->") + "<!--StartFragment -->".Length).ToString("D8"));
            sb.Replace("<<<<<<<4", (utf8EncodedHTMLString.IndexOf("<!--EndFragment -->")).ToString("D8"));
            return sb.ToString();
        }

        private static string getHTMLDataURLString(ISurface surface, MemoryStream pngStream)
        {
            string utf8EncodedHTMLString = Encoding.GetEncoding(0).GetString(Encoding.UTF8.GetBytes(HTML_CLIPBOARD_BASE64_STRING));
            utf8EncodedHTMLString = utf8EncodedHTMLString.Replace("${width}", surface.Image.Width.ToString());
            utf8EncodedHTMLString = utf8EncodedHTMLString.Replace("${height}", surface.Image.Height.ToString());
            utf8EncodedHTMLString = utf8EncodedHTMLString.Replace("${format}", "png");
            utf8EncodedHTMLString = utf8EncodedHTMLString.Replace("${data}", Convert.ToBase64String(pngStream.GetBuffer(), 0, (int)pngStream.Length));
            StringBuilder sb = new StringBuilder();
            sb.Append(utf8EncodedHTMLString);
            sb.Replace("<<<<<<<1", (utf8EncodedHTMLString.IndexOf("<HTML>") + "<HTML>".Length).ToString("D8"));
            sb.Replace("<<<<<<<2", (utf8EncodedHTMLString.IndexOf("</HTML>")).ToString("D8"));
            sb.Replace("<<<<<<<3", (utf8EncodedHTMLString.IndexOf("<!--StartFragment -->") + "<!--StartFragment -->".Length).ToString("D8"));
            sb.Replace("<<<<<<<4", (utf8EncodedHTMLString.IndexOf("<!--EndFragment -->")).ToString("D8"));
            return sb.ToString();
        }

        /// <summary>
        /// Set Object with type Type to the clipboard
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="obj">object</param>
        public static void SetClipboardData(Type type, Object obj)
        {
            DataFormats.Format format = DataFormats.GetFormat(type.FullName);

            //now copy to clipboard
            IDataObject dataObj = new DataObject();
            dataObj.SetData(format.Name, false, obj);
            // Use false to make the object dissapear when the application stops.
            SetDataObject(dataObj, true);
        }

        /// <summary>
        /// Retrieve a list of all formats currently on the clipboard
        /// </summary>
        /// <returns>List<string> with the current formats</returns>
        public static List<string> GetFormats()
        {
            return GetFormats(GetDataObject());
        }

        /// <summary>
        /// Retrieve a list of all formats currently in the IDataObject
        /// </summary>
        /// <returns>List<string> with the current formats</returns>
        public static List<string> GetFormats(IDataObject dataObj)
        {
            string[] formats = null;

            if (dataObj != null)
            {
                formats = dataObj.GetFormats();
            }
            if (formats != null)
            {
                LOG.DebugFormat("Got clipboard formats: {0}", String.Join(",", formats));
                return new List<string>(formats);
            }
            return new List<string>();
        }

        /// <summary>
        /// Check if there is currently something in the dataObject which has the supplied format
        /// </summary>
        /// <param name="dataObject">IDataObject</param>
        /// <param name="format">string with format</param>
        /// <returns>true if one the format is found</returns>
        public static bool ContainsFormat(string format)
        {
            return ContainsFormat(GetDataObject(), new string[] { format });
        }

        /// <summary>
        /// Check if there is currently something on the clipboard which has the supplied format
        /// </summary>
        /// <param name="format">string with format</param>
        /// <returns>true if one the format is found</returns>
        public static bool ContainsFormat(IDataObject dataObject, string format)
        {
            return ContainsFormat(dataObject, new string[] { format });
        }

        /// <summary>
        /// Check if there is currently something on the clipboard which has one of the supplied formats
        /// </summary>
        /// <param name="formats">string[] with formats</param>
        /// <returns>true if one of the formats was found</returns>
        public static bool ContainsFormat(string[] formats)
        {
            return ContainsFormat(GetDataObject(), formats);
        }

        /// <summary>
        /// Check if there is currently something on the clipboard which has one of the supplied formats
        /// </summary>
        /// <param name="dataObject">IDataObject</param>
        /// <param name="formats">string[] with formats</param>
        /// <returns>true if one of the formats was found</returns>
        public static bool ContainsFormat(IDataObject dataObject, string[] formats)
        {
            bool formatFound = false;
            List<string> currentFormats = GetFormats(dataObject);
            if (currentFormats == null || currentFormats.Count == 0 || formats == null || formats.Length == 0)
            {
                return false;
            }
            foreach (string format in formats)
            {
                if (currentFormats.Contains(format))
                {
                    formatFound = true;
                    break;
                }
            }
            return formatFound;
        }

        /// <summary>
        /// Get Object of type Type from the clipboard
        /// </summary>
        /// <param name="type">Type to get</param>
        /// <returns>object from clipboard</returns>
        public static Object GetClipboardData(Type type)
        {
            string format = type.FullName;
            return GetClipboardData(format);
        }

        /// <summary>
        /// Get Object for format from IDataObject
        /// </summary>
        /// <param name="dataObj">IDataObject</param>
        /// <param name="type">Type to get</param>
        /// <returns>object from IDataObject</returns>
        public static Object GetFromDataObject(IDataObject dataObj, Type type)
        {
            if (type != null)
            {
                return GetFromDataObject(dataObj, type.FullName);
            }
            return null;
        }

        /// <summary>
        /// Get ImageFilenames from the IDataObject
        /// </summary>
        /// <param name="dataObject">IDataObject</param>
        /// <returns></returns>
        public static List<string> GetImageFilenames(IDataObject dataObject)
        {
            List<string> filenames = new List<string>();
            string[] dropFileNames = (string[])dataObject.GetData(DataFormats.FileDrop);
            try
            {
                if (dropFileNames != null && dropFileNames.Length > 0)
                {
                    foreach (string filename in dropFileNames)
                    {
                        string ext = Path.GetExtension(filename).ToLower();
                        if ((ext == ".jpg") || (ext == ".jpeg") || (ext == ".tiff") || (ext == ".gif") || (ext == ".png") || (ext == ".bmp") || (ext == ".ico") || (ext == ".wmf"))
                        {
                            filenames.Add(filename);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LOG.Warn("Ignoring an issue with getting the dropFilenames from the clipboard: ", ex);
            }
            return filenames;
        }

        /// <summary>
        /// Get Object for format from IDataObject
        /// </summary>
        /// <param name="dataObj">IDataObject</param>
        /// <param name="format">format to get</param>
        /// <returns>object from IDataObject</returns>
        public static Object GetFromDataObject(IDataObject dataObj, string format)
        {
            if (dataObj != null)
            {
                try
                {
                    return dataObj.GetData(format);
                }
                catch (Exception e)
                {
                    LOG.Error("Error in GetClipboardData.", e);
                }
            }
            return null;
        }

        /// <summary>
        /// Get Object for format from the clipboard
        /// </summary>
        /// <param name="format">format to get</param>
        /// <returns>object from clipboard</returns>
        public static Object GetClipboardData(string format)
        {
            return GetFromDataObject(GetDataObject(), format);
        }
    }
}