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

using System.Text;

namespace ShareX.ImageListView;

#region FileSystemAdaptor
/// <summary>
/// Represents a file system adaptor.
/// </summary>
public class FileSystemAdaptor : ImageListView.ImageListViewItemAdaptor
{
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemAdaptor"/> class.
    /// </summary>
    public FileSystemAdaptor()
    {
        disposed = false;
    }

    /// <summary>
    /// Returns the thumbnail image for the given item.
    /// </summary>
    /// <param name="key">Item key.</param>
    /// <param name="size">Requested image size.</param>
    /// <param name="useEmbeddedThumbnails">Embedded thumbnail usage.</param>
    /// <param name="useExifOrientation">true to automatically rotate images based on Exif orientation; otherwise false.</param>
    /// <returns>The thumbnail image from the given item or null if an error occurs.</returns>
    public override Image GetThumbnail(object key, Size size, UseEmbeddedThumbnails useEmbeddedThumbnails, bool useExifOrientation)
    {
        if (disposed)
            return null;

        string filename = (string)key;
        return File.Exists(filename) ? Extractor.Instance.GetThumbnail(filename, size, useEmbeddedThumbnails, useExifOrientation) : null;
    }
    /// <summary>
    /// Returns a unique identifier for this thumbnail to be used in persistent
    /// caching.
    /// </summary>
    /// <param name="key">Item key.</param>
    /// <param name="size">Requested image size.</param>
    /// <param name="useEmbeddedThumbnails">Embedded thumbnail usage.</param>
    /// <param name="useExifOrientation">true to automatically rotate images based on Exif orientation; otherwise false.</param>
    /// <returns>A unique identifier string for the thumnail.</returns>
    public override string GetUniqueIdentifier(object key, Size size, UseEmbeddedThumbnails useEmbeddedThumbnails, bool useExifOrientation)
    {
        StringBuilder sb = new();
        sb.Append((string)key);// Filename
        sb.Append(':');
        sb.Append(size.Width); // Thumbnail size
        sb.Append(',');
        sb.Append(size.Height);
        sb.Append(':');
        sb.Append(useEmbeddedThumbnails);
        sb.Append(':');
        sb.Append(useExifOrientation);
        return sb.ToString();
    }
    /// <summary>
    /// Returns the path to the source image for use in drag operations.
    /// </summary>
    /// <param name="key">Item key.</param>
    /// <returns>The path to the source image.</returns>
    public override string GetSourceImage(object key)
    {
        if (disposed)
            return null;

        string filename = (string)key;
        return filename;
    }
    /// <summary>
    /// Returns the details for the given item.
    /// </summary>
    /// <param name="key">Item key.</param>
    /// <returns>An array of tuples containing item details or null if an error occurs.</returns>
    public override Utility.Tuple<ColumnType, string, object>[] GetDetails(object key)
    {
        if (disposed)
            return null;

        string filename = (string)key;
        List<Utility.Tuple<ColumnType, string, object>> details = new();

        // Get file info
        if (File.Exists(filename))
        {
            FileInfo info = new(filename);
            details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.DateCreated, string.Empty, info.CreationTime));
            details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.DateAccessed, string.Empty, info.LastAccessTime));
            details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.DateModified, string.Empty, info.LastWriteTime));
            details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.FileSize, string.Empty, info.Length));
            details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.FilePath, string.Empty, info.DirectoryName ?? ""));
            details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.FolderName, string.Empty, info.Directory.Name ?? ""));

            // Get metadata
            Metadata metadata = Extractor.Instance.GetMetadata(filename);
            details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.Dimensions, string.Empty, new Size(metadata.Width, metadata.Height)));
            details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.Resolution, string.Empty, new SizeF((float)metadata.DPIX, (float)metadata.DPIY)));
            details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.ImageDescription, string.Empty, metadata.ImageDescription ?? ""));
            details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.EquipmentModel, string.Empty, metadata.EquipmentModel ?? ""));
            details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.DateTaken, string.Empty, metadata.DateTaken));
            details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.Artist, string.Empty, metadata.Artist ?? ""));
            details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.Copyright, string.Empty, metadata.Copyright ?? ""));
            details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.ExposureTime, string.Empty, (float)metadata.ExposureTime));
            details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.FNumber, string.Empty, (float)metadata.FNumber));
            details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.ISOSpeed, string.Empty, (ushort)metadata.ISOSpeed));
            details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.UserComment, string.Empty, metadata.Comment ?? ""));
            details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.Rating, string.Empty, (ushort)metadata.Rating));
            details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.Software, string.Empty, metadata.Software ?? ""));
            details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.FocalLength, string.Empty, (float)metadata.FocalLength));
        }

        return details.ToArray();
    }
    /// <summary>
    /// Performs application-defined tasks associated with freeing,
    /// releasing, or resetting unmanaged resources.
    /// </summary>
    public override void Dispose()
    {
        disposed = true;
    }
}
#endregion

#region URIAdaptor
/// <summary>
/// Represents a URI adaptor.
/// </summary>
public class URIAdaptor : ImageListView.ImageListViewItemAdaptor
{
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="URIAdaptor"/> class.
    /// </summary>
    public URIAdaptor()
    {
        disposed = false;
    }

    /// <summary>
    /// Returns the thumbnail image for the given item.
    /// </summary>
    /// <param name="key">Item key.</param>
    /// <param name="size">Requested image size.</param>
    /// <param name="useEmbeddedThumbnails">Embedded thumbnail usage.</param>
    /// <param name="useExifOrientation">true to automatically rotate images based on Exif orientation; otherwise false.</param>
    /// <returns>The thumbnail image from the given item or null if an error occurs.</returns>
    public override Image? GetThumbnail(object key, Size size, UseEmbeddedThumbnails useEmbeddedThumbnails, bool useExifOrientation)
    {
        if (disposed)
            return null;

        string uri = (string)key;
        try
        {
            using HttpClient client = new();
            byte[] imageData = client.GetByteArrayAsync(uri).Result;
            using MemoryStream stream = new(imageData);
            using Image sourceImage = Image.FromStream(stream);
            return Extractor.Instance.GetThumbnail(sourceImage, size, useEmbeddedThumbnails, useExifOrientation);
        } catch
        {
            return null;
        }
    }
    /// <summary>
    /// Returns a unique identifier for this thumbnail to be used in persistent
    /// caching.
    /// </summary>
    /// <param name="key">Item key.</param>
    /// <param name="size">Requested image size.</param>
    /// <param name="useEmbeddedThumbnails">Embedded thumbnail usage.</param>
    /// <param name="useExifOrientation">true to automatically rotate images based on Exif orientation; otherwise false.</param>
    /// <returns>A unique identifier string for the thumbnail.</returns>
    public override string GetUniqueIdentifier(object key, Size size, UseEmbeddedThumbnails useEmbeddedThumbnails, bool useExifOrientation)
    {
        StringBuilder sb = new();
        sb.Append((string)key);// Uri
        sb.Append(':');
        sb.Append(size.Width); // Thumbnail size
        sb.Append(',');
        sb.Append(size.Height);
        sb.Append(':');
        sb.Append(useEmbeddedThumbnails);
        sb.Append(':');
        sb.Append(useExifOrientation);
        sb.Append(':');
        return sb.ToString();
    }
    /// <summary>
    /// Returns the path to the source image for use in drag operations.
    /// </summary>
    /// <param name="key">Item key.</param>
    /// <returns>The path to the source image.</returns>
    public override string? GetSourceImage(object key)
    {
        if (disposed)
            return null;

        string uri = (string)key;
        try
        {
            string filename = Path.GetTempFileName();
            using HttpClient client = new();
            byte[] imageData = client.GetByteArrayAsync(uri).Result;
            File.WriteAllBytes(filename, imageData);
            return filename;
        } catch
        {
            return null;
        }
    }
    /// <summary>
    /// Returns the details for the given item.
    /// </summary>
    /// <param name="key">Item key.</param>
    /// <returns>An array of 2-tuples containing item details or null if an error occurs.</returns>
    public override Utility.Tuple<ColumnType, string, object>[] GetDetails(object key)
    {
        if (disposed)
            return null;

        string uri = (string)key;
        List<Utility.Tuple<ColumnType, string, object>> details = [new Utility.Tuple<ColumnType, string, object>(ColumnType.Custom, "URL", uri)];

        return [.. details];
    }
    /// <summary>
    /// Performs application-defined tasks associated with freeing,
    /// releasing, or resetting unmanaged resources.
    /// </summary>
    public override void Dispose()
    {
        disposed = true;
    }
}
#endregion
