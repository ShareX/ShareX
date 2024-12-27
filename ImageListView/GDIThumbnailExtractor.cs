// ImageListView - A listview control for image files
// Copyright (C) 2009 Ozgur Ozcitak
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Ozgur Ozcitak (ozcitak@yahoo.com)
//
// WIC support coded by Jens

using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;

namespace ShareX.ImageListView;

/// <summary>
/// Extracts thumbnails from images.
/// </summary>
public partial class GDIExtractor : IExtractor
{
    #region Exif Tag IDs
    private const int TagThumbnailData = 0x501B;
    private const int TagOrientation = 0x0112;
    #endregion

    #region Public Methods
    /// <summary>
    /// Creates a thumbnail from the given image.
    /// </summary>
    /// <param name="image">The source image.</param>
    /// <param name="size">Requested image size.</param>
    /// <param name="useEmbeddedThumbnails">Embedded thumbnail usage.</param>
    /// <param name="useExifOrientation">true to automatically rotate images based on Exif orientation; otherwise false.</param>
    /// <returns>The thumbnail image from the given image or null if an error occurs.</returns>
    public virtual Image GetThumbnail(Image image, Size size, UseEmbeddedThumbnails useEmbeddedThumbnails, bool useExifOrientation)
    {
        return size.Width <= 0 || size.Height <= 0
            ? throw new ArgumentException()
            : GetThumbnailBmp(image, size,
            useExifOrientation ? GetRotation(image) : 0);
    }
    /// <summary>
    /// Creates a thumbnail from the given image file.
    /// </summary>
    /// <param name="filename">The filename pointing to an image.</param>
    /// <param name="size">Requested image size.</param>
    /// <param name="useEmbeddedThumbnails">Embedded thumbnail usage.</param>
    /// <param name="useExifOrientation">true to automatically rotate images based on Exif orientation; otherwise false.</param>
    /// <returns>The thumbnail image from the given file or null if an error occurs.</returns>
    public virtual Image GetThumbnail(string filename, Size size, UseEmbeddedThumbnails useEmbeddedThumbnails, bool useExifOrientation)
    {
        return string.IsNullOrEmpty(filename)
            ? throw new ArgumentException("Filename cannot be empty", "filename")
            : size.Width <= 0 || size.Height <= 0
            ? throw new ArgumentException("Thumbnail size cannot be empty.", "size")
            : GetThumbnailBmp(filename, size, useEmbeddedThumbnails,
            useExifOrientation ? GetRotation(filename) : 0);
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// Creates a thumbnail from the given image.
    /// </summary>
    /// <param name="image">The source image.</param>
    /// <param name="size">Requested image size.</param>
    /// <param name="rotate">Rotation angle.</param>
    /// <returns>The image from the given file or null if an error occurs.</returns>
    internal static Image GetThumbnailBmp(Image image, Size size, int rotate)
    {
        if (size.Width <= 0 || size.Height <= 0)
            throw new ArgumentException();

        Image? thumb = null;
        try
        {
            double scale = rotate % 180 != 0
                ? Math.Min(size.Height / (double)image.Width,
                    size.Width / (double)image.Height)
                : Math.Min(size.Width / (double)image.Width,
                    size.Height / (double)image.Height);
            thumb = ScaleDownRotateBitmap(image, scale, rotate);
        } catch
        {
            thumb?.Dispose();
            thumb = null;
        }

        return thumb;
    }
    /// <summary>
    /// Creates a thumbnail from the given image file.
    /// </summary>
    /// <param name="filename">The filename pointing to an image.</param>
    /// <param name="size">Requested image size.</param>
    /// <param name="useEmbeddedThumbnails">Embedded thumbnail usage.</param>
    /// <param name="rotate">Rotation angle.</param>
    /// <returns>The image from the given file or null if an error occurs.</returns>
    internal static Image GetThumbnailBmp(string filename, Size size, UseEmbeddedThumbnails useEmbeddedThumbnails, int rotate)
    {
        if (size.Width <= 0 || size.Height <= 0)
            throw new ArgumentException();

        // Check if this is an image file
        try
        {
            using FileStream stream = new(filename, FileMode.Open, FileAccess.Read);
            if (!IsImage(stream))
                return null;
        } catch
        {
            return null;
        }

        Image source = null;
        Image thumb = null;

        // Try to read the exif thumbnail
        if (useEmbeddedThumbnails != UseEmbeddedThumbnails.Never)
        {
            try
            {
                using FileStream stream = new(filename, FileMode.Open, FileAccess.Read);
                using Image img = Image.FromStream(stream, false, false);
                foreach (int index in img.PropertyIdList)
                {
                    if (index == TagThumbnailData)
                    {
                        // Fetch the embedded thumbnail
                        byte[] rawImage = img.GetPropertyItem(TagThumbnailData).Value;
                        using (MemoryStream memStream = new(rawImage))
                        {
                            source = Image.FromStream(memStream);
                        }
                        if (useEmbeddedThumbnails == UseEmbeddedThumbnails.Auto)
                        {
                            // Check that the embedded thumbnail is large enough.
                            if (Math.Max((float)source.Width / (float)size.Width,
                                (float)source.Height / (float)size.Height) < 1.0f)
                            {
                                source.Dispose();
                                source = null;
                            }
                        }
                        break;
                    }
                }
            } catch
            {
                if (source != null)
                    source.Dispose();
                source = null;
            }
        }

        // Fix for the missing semicolon in GIF files
        MemoryStream streamCopy = null;
        try
        {
            if (source == null)
            {
                using FileStream stream = new(filename, FileMode.Open, FileAccess.Read);
                byte[] gifSignature = new byte[4];
                stream.ReadExactly(gifSignature, 0, 4);
                if (Encoding.ASCII.GetString(gifSignature) == "GIF8")
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    streamCopy = new MemoryStream();
                    byte[] buffer = new byte[32768];
                    int read = 0;
                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        streamCopy.Write(buffer, 0, read);
                    }
                    // Append the missing semicolon
                    streamCopy.Seek(-1, SeekOrigin.End);
                    if (streamCopy.ReadByte() != 0x3b)
                        streamCopy.WriteByte(0x3b);
                    source = Image.FromStream(streamCopy);
                }
            }
        } catch
        {
            if (source != null)
                source.Dispose();
            source = null;
            if (streamCopy != null)
                streamCopy.Dispose();
            streamCopy = null;
        }

        // Revert to source image if an embedded thumbnail of required size
        // was not found.
        FileStream sourceStream = null;
        if (source == null)
        {
            try
            {
                sourceStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                source = Image.FromStream(sourceStream);
            } catch
            {
                if (source != null)
                    source.Dispose();
                if (sourceStream != null)
                    sourceStream.Dispose();
                source = null;
                sourceStream = null;
            }
        }

        // If all failed, return null.
        if (source == null) return null;

        // Create the thumbnail
        try
        {
            double scale = rotate % 180 != 0
                ? Math.Min(size.Height / (double)source.Width,
                    size.Width / (double)source.Height)
                : Math.Min(size.Width / (double)source.Width,
                    size.Height / (double)source.Height);
            thumb = ScaleDownRotateBitmap(source, scale, rotate);
        } catch
        {
            if (thumb != null)
                thumb.Dispose();
            thumb = null;
        } finally
        {
            if (source != null)
                source.Dispose();
            source = null;
            if (sourceStream != null)
                sourceStream.Dispose();
            sourceStream = null;
            if (streamCopy != null)
                streamCopy.Dispose();
            streamCopy = null;
        }

        return thumb;
    }
    /// <summary>
    /// Returns Exif rotation in degrees. Returns 0 if the metadata 
    /// does not exist or could not be read. A negative value means
    /// the image needs to be mirrored about the vertical axis.
    /// </summary>
    /// <param name="img">Image.</param>
    private static int GetRotation(Image img)
    {
        try
        {
            foreach (PropertyItem prop in img.PropertyItems)
            {
                if (prop.Id == TagOrientation)
                {
                    ushort orientationFlag = BitConverter.ToUInt16(prop.Value, 0);
                    if (orientationFlag == 1)
                        return 0;
                    else if (orientationFlag == 2)
                        return -360;
                    else if (orientationFlag == 3)
                        return 180;
                    else if (orientationFlag == 4)
                        return -180;
                    else if (orientationFlag == 5)
                        return -90;
                    else if (orientationFlag == 6)
                        return 90;
                    else if (orientationFlag == 7)
                        return -270;
                    else if (orientationFlag == 8)
                        return 270;
                }
            }
        } catch
        {
            ;
        }

        return 0;
    }
    /// <summary>
    /// Returns Exif rotation in degrees. Returns 0 if the metadata 
    /// does not exist or could not be read. A negative value means
    /// the image needs to be mirrored about the vertical axis.
    /// </summary>
    /// <param name="filename">Image.</param>
    private static int GetRotation(string filename)
    {
        try
        {
            using FileStream stream = new(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            using Image img = Image.FromStream(stream, false, false);
            return GetRotation(img);
        } catch
        {
            ;
        }

        return 0;
    }

    /// <summary>
    /// Scales down and rotates an image.
    /// </summary>
    /// <param name="source">Original image</param>
    /// <param name="scale">Uniform scaling factor</param>
    /// <param name="angle">Rotation angle</param>
    /// <returns>Scaled and rotated image</returns>
    private static Image ScaleDownRotateBitmap(Image source, double scale, int angle)
    {
        if (angle % 90 != 0)
        {
            throw new ArgumentException("Rotation angle should be a multiple of 90 degrees.", "angle");
        }

        // Do not upscale and no rotation.
        if ((float)scale >= 1.0f && angle == 0)
        {
            return new Bitmap(source);
        }

        int sourceWidth = source.Width;
        int sourceHeight = source.Height;

        // Scale
        double xScale = Math.Min(1.0, Math.Max(1.0 / (double)sourceWidth, scale));
        double yScale = Math.Min(1.0, Math.Max(1.0 / (double)sourceHeight, scale));

        int width = (int)((double)sourceWidth * xScale);
        int height = (int)((double)sourceHeight * yScale);
        int thumbWidth = Math.Abs(angle) % 180 == 0 ? width : height;
        int thumbHeight = Math.Abs(angle) % 180 == 0 ? height : width;

        Bitmap thumb = new(thumbWidth, thumbHeight);
        thumb.SetResolution(source.HorizontalResolution, source.VerticalResolution);
        using (Graphics g = Graphics.FromImage(thumb))
        {
            g.PixelOffsetMode = PixelOffsetMode.None;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.Clear(System.Drawing.Color.Transparent);

            g.TranslateTransform(-sourceWidth / 2, -sourceHeight / 2, MatrixOrder.Append);
            if (Math.Abs(angle) % 360 != 0)
                g.RotateTransform(Math.Abs(angle), MatrixOrder.Append);
            if (angle < 0)
                xScale = -xScale;
            g.ScaleTransform((float)xScale, (float)yScale, MatrixOrder.Append);
            g.TranslateTransform(thumbWidth / 2, thumbHeight / 2, MatrixOrder.Append);

            g.DrawImage(source, 0, 0);
        }

        return thumb;
    }
    /// <summary>
    /// Checks the stream header if it matches with
    /// any of the supported image file types.
    /// </summary>
    /// <param name="stream">An open stream pointing to an image file.</param>
    /// <returns>true if the stream is an image file (BMP, TIFF, PNG, GIF, JPEG, WMF, EMF, ICO, CUR);
    /// false otherwise.</returns>
    public static bool IsImage(Stream stream)
    {
        // Sniff some bytes from the start of the stream
        // and check against magic numbers of supported 
        // image file formats
        byte[] header = new byte[8];
        stream.Seek(0, SeekOrigin.Begin);
        if (stream.Read(header, 0, header.Length) != header.Length)
            return false;

        // BMP
        string bmpHeader = Encoding.ASCII.GetString(header, 0, 2);
        if (bmpHeader == "BM") // BM - Windows bitmap
            return true;
        else if (bmpHeader == "BA") // BA - Bitmap array
            return true;
        else if (bmpHeader == "CI") // CI - Color Icon
            return true;
        else if (bmpHeader == "CP") // CP - Color Pointer
            return true;
        else if (bmpHeader == "IC") // IC - Icon
            return true;
        else if (bmpHeader == "PT") // PI - Pointer
            return true;

        // TIFF
        string tiffHeader = Encoding.ASCII.GetString(header, 0, 4);
        if (tiffHeader == "MM\x00\x2a") // Big-endian
            return true;
        else if (tiffHeader == "II\x2a\x00") // Little-endian
            return true;

        // PNG
        if (header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47 &&
            header[4] == 0x0D && header[5] == 0x0A && header[6] == 0x1A && header[7] == 0x0A)
            return true;

        // GIF
        string gifHeader = Encoding.ASCII.GetString(header, 0, 4);
        if (gifHeader == "GIF8")
            return true;

        // JPEG
        if (header[0] == 0xFF && header[1] == 0xD8)
            return true;

        // WMF
        if (header[0] == 0xD7 && header[1] == 0xCD && header[2] == 0xC6 && header[3] == 0x9A)
            return true;

        // EMF
        if (header[0] == 0x01 && header[1] == 0x00 && header[2] == 0x00 && header[3] == 0x00)
            return true;

        // Windows Icons
        if (header[0] == 0x00 && header[1] == 0x00 && header[2] == 0x01 && header[3] == 0x00) // ICO
            return true;
        else if (header[0] == 0x00 && header[1] == 0x00 && header[2] == 0x02 && header[3] == 0x00) // CUR
            return true;

        return false;
    }
    #endregion
}
