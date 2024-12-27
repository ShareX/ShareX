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

namespace ShareX.ImageListView;

/// <summary>
/// Interface for thumbnail, metadata and shell info extractors.
/// </summary>
public interface IExtractor
{
    /// <summary>
    /// Gets the name of this extractor.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Returns image metadata.
    /// </summary>
    /// <param name="path">Filepath of image</param>
    Metadata GetMetadata(string path);

    /// <summary>
    /// Creates a thumbnail from the given image file.
    /// </summary>
    /// <param name="filename">The filename pointing to an image.</param>
    /// <param name="size">Requested image size.</param>
    /// <param name="useEmbeddedThumbnails">Embedded thumbnail usage.</param>
    /// <param name="useExifOrientation">true to automatically rotate images based on Exif orientation; otherwise false.</param>
    /// <returns>The thumbnail image from the given file or null if an error occurs.</returns>
    Image GetThumbnail(string filename, Size size, UseEmbeddedThumbnails useEmbeddedThumbnails, bool useExifOrientation);

    /// <summary>
    /// Creates a thumbnail from the given image.
    /// </summary>
    /// <param name="image">The source image.</param>
    /// <param name="size">Requested image size.</param>
    /// <param name="useEmbeddedThumbnails">Embedded thumbnail usage.</param>
    /// <param name="useExifOrientation">true to automatically rotate images based on Exif orientation; otherwise false.</param>
    /// <returns>The thumbnail image from the given image or null if an error occurs.</returns>
    Image GetThumbnail(Image image, Size size, UseEmbeddedThumbnails useEmbeddedThumbnails, bool useExifOrientation);

    /// <summary>
    /// Returns shell info.
    /// </summary>
    /// <param name="path">Filepath of image</param>
    ShellInfo GetShellInfo(string path);
}
