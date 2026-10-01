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

using SkiaSharp;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using Avalonia.Controls;

namespace ShareX.HelpersLib;

public static partial class SkiaImageHelpers
{
    public static string OpenImageFileDialog(Window form = null, string initialDirectory = null)
    {
        string[] images = OpenImageFileDialog(false, form, initialDirectory);

        if (images != null && images.Length > 0)
        {
            return images[0];
        }

        return null;
    }

    public static string[] OpenImageFileDialog(bool multiselect, Window form = null, string initialDirectory = null)
    {
        return FileDialogHelpers.OpenFiles(filter:
            $"{Localization.Strings.ImageHelpers_Image_files} (*.png, *.jpg, *.jpeg, *.jpe, *.jfif, *.gif, *.bmp)|*.png;*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.bmp|" +
            "PNG (*.png)|*.png|JPEG (*.jpg, *.jpeg, *.jpe, *.jfif)|*.jpg;*.jpeg;*.jpe;*.jfif|GIF (*.gif)|*.gif|BMP (*.bmp)|*.bmp",
            multiselect: multiselect, initialDirectory: initialDirectory, owner: form);
    }

    public static bool SaveImage(SKBitmap img, string filePath)
    {
        FileHelpers.CreateDirectoryFromFilePath(filePath);
        ImageFileFormat imageFormat = GetImageFormat(filePath);

        try
        {
            img.Save(filePath, imageFormat);
            return true;
        }
        catch (Exception e)
        {
            DebugHelper.WriteException(e);
            e.ShowError();
        }

        return false;
    }

    public static string SaveImageFileDialog(SKBitmap img, string filePath = "", bool useLastDirectory = true)
    {
        string initialDirectory = useLastDirectory && Directory.Exists(HelpersOptions.LastSaveDirectory)
            ? HelpersOptions.LastSaveDirectory : Path.GetDirectoryName(filePath);
        string extension = Path.GetExtension(filePath).TrimStart('.').ToLowerInvariant();
        int filterIndex = extension switch { "jpg" or "jpeg" or "jpe" or "jfif" => 2, "gif" => 3, "bmp" => 4, _ => 1 };
        string selectedPath = FileDialogHelpers.SaveFile(filter:
            "PNG (*.png)|*.png|JPEG (*.jpg, *.jpeg, *.jpe, *.jfif)|*.jpg;*.jpeg;*.jpe;*.jfif|GIF (*.gif)|*.gif|BMP (*.bmp)|*.bmp",
            fileName: Path.GetFileName(filePath), initialDirectory: initialDirectory, defaultExtension: "png", filterIndex: filterIndex);
        if (!string.IsNullOrEmpty(selectedPath) && SaveImage(img, selectedPath))
        {
            HelpersOptions.LastSaveDirectory = Path.GetDirectoryName(selectedPath);
            return selectedPath;
        }
        return null;
    }

    public static SKBitmap LoadImageWithFileDialog(Window form = null)
    {
        string filePath = OpenImageFileDialog(form);

        if (!string.IsNullOrEmpty(filePath))
        {
            return LoadImage(filePath);
        }

        return null;
    }

    public static MemoryStream PNGStripChunks(MemoryStream stream, params string[] chunks)
    {
        MemoryStream output = new MemoryStream();
        stream.Seek(0, SeekOrigin.Begin);

        byte[] signature = new byte[8];
        stream.Read(signature, 0, 8);
        output.Write(signature, 0, 8);

        while (true)
        {
            byte[] lenBytes = new byte[4];
            if (stream.Read(lenBytes, 0, 4) != 4)
            {
                break;
            }

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(lenBytes);
            }

            int len = BitConverter.ToInt32(lenBytes, 0);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(lenBytes);
            }

            byte[] type = new byte[4];
            stream.Read(type, 0, 4);

            byte[] data = new byte[len + 4];
            stream.Read(data, 0, data.Length);

            string strType = Encoding.ASCII.GetString(type);

            if (!chunks.Contains(strType))
            {
                output.Write(lenBytes, 0, lenBytes.Length);
                output.Write(type, 0, type.Length);
                output.Write(data, 0, data.Length);
            }
        }

        return output;
    }

    public static MemoryStream PNGStripColorSpaceInformation(MemoryStream stream)
    {
        // http://www.libpng.org/pub/png/spec/1.2/PNG-Chunks.html
        // 4.2.2.1. gAMA Image gamma
        // 4.2.2.2. cHRM Primary chromaticities
        // 4.2.2.3. sRGB Standard RGB color space
        // 4.2.2.4. iCCP Embedded ICC profile
        return PNGStripChunks(stream, "gAMA", "cHRM", "sRGB", "iCCP");
    }

    public static string ImageFileToBase64(string path)
    {
        byte[] imageBytes = File.ReadAllBytes(path);
        return Convert.ToBase64String(imageBytes);
    }

    public static Size GetImageFileDimensions(string path)
    {
        try
        {
            using FileStream stream = File.OpenRead(path);
            using SKManagedStream managed = new(stream, false);
            using SKCodec codec = SKCodec.Create(managed);
            if (codec == null) return Size.Empty;
            return HelpersOptions.RotateImageByExifOrientationData && (int)codec.EncodedOrigin >= 5
                ? new Size(codec.Info.Height, codec.Info.Width) : new Size(codec.Info.Width, codec.Info.Height);
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception);
            return Size.Empty;
        }
    }

    public static MemoryStream GetStream(this SKBitmap bitmap)
    {
        MemoryStream stream = new();
        bitmap.Save(stream, SKEncodedImageFormat.Png);
        stream.Position = 0;
        return stream;
    }

    public static byte[] GetIconBytes(this SKBitmap bitmap)
    {
        using MemoryStream png = new();
        bitmap.Save(png, SKEncodedImageFormat.Png);
        using MemoryStream stream = new();
        using (BinaryWriter writer = new(stream, Encoding.UTF8, true))
        {
            writer.Write((ushort)0); writer.Write((ushort)1); writer.Write((ushort)1);
            writer.Write((byte)(bitmap.Width < 256 ? bitmap.Width : 0));
            writer.Write((byte)(bitmap.Height < 256 ? bitmap.Height : 0));
            writer.Write((byte)0); writer.Write((byte)0); writer.Write((ushort)1); writer.Write((ushort)32);
            writer.Write((int)png.Length); writer.Write(22); writer.Write(png.ToArray());
        }
        return stream.ToArray();
    }
}
