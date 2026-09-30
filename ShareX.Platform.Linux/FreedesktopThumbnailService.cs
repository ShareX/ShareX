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
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ShareX.Platform.Linux;

/// <summary>
/// Reads thumbnails that file managers already created in the freedesktop.org thumbnail cache
/// (https://specifications.freedesktop.org/thumbnail-spec/latest/). Nothing is generated here.
/// </summary>
public sealed class FreedesktopThumbnailService : IThumbnailService
{
    private readonly string cacheRoot;

    public FreedesktopThumbnailService(string cacheRoot)
    {
        this.cacheRoot = cacheRoot;
    }

    public FeatureSupport Support => FeatureSupport.Supported;

    public byte[]? GetThumbnail(string path, int maxWidth, int maxHeight)
    {
        try
        {
            string fullPath = Path.GetFullPath(path);
            string name = GetThumbnailName(fullPath);

            // The spec defines 128, 256 and 512 pixel sizes. Take the smallest that is big enough, then any that exists.
            int wanted = Math.Max(maxWidth, maxHeight);
            string[] folders = wanted <= 128 ? new[] { "normal", "large", "x-large", "xx-large" } : wanted <= 256 ? new[] { "large", "x-large", "xx-large", "normal" } : new[] { "x-large", "xx-large", "large", "normal" };

            foreach (string folder in folders)
            {
                string candidate = Path.Combine(cacheRoot, folder, name);

                if (File.Exists(candidate))
                {
                    byte[] png = File.ReadAllBytes(candidate);

                    // The spec requires checking Thumb::MTime. A file edited after the thumbnail was made needs a new one.
                    if (IsCurrent(png, fullPath))
                    {
                        return png;
                    }
                }
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
        }

        return null;
    }

    internal static bool IsCurrent(byte[] png, string fullPath)
    {
        string? mtime = ReadTextChunk(png, "Thumb::MTime");

        if (mtime == null || !long.TryParse(mtime, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out long seconds))
        {
            return false;
        }

        return seconds == new DateTimeOffset(File.GetLastWriteTimeUtc(fullPath)).ToUnixTimeSeconds();
    }

    /// <summary>Returns the value of a PNG tEXt chunk, or null. Thumbnailers write Thumb::URI and Thumb::MTime this way.</summary>
    internal static string? ReadTextChunk(byte[] png, string keyword)
    {
        int offset = 8;

        while (offset + 8 <= png.Length)
        {
            int length = (png[offset] << 24) | (png[offset + 1] << 16) | (png[offset + 2] << 8) | png[offset + 3];
            string type = Encoding.ASCII.GetString(png, offset + 4, 4);
            int data = offset + 8;

            if (length < 0 || data + length > png.Length)
            {
                return null;
            }

            if (type == "tEXt")
            {
                int separator = Array.IndexOf(png, (byte)0, data, length);

                if (separator > data && Encoding.Latin1.GetString(png, data, separator - data) == keyword)
                {
                    return Encoding.Latin1.GetString(png, separator + 1, data + length - separator - 1);
                }
            }
            else if (type == "IDAT" || type == "IEND")
            {
                // Text chunks the spec cares about come before the image data.
                return null;
            }

            offset = data + length + 4;
        }

        return null;
    }

    /// <summary>The cache file name is the MD5 of the file's canonical URI.</summary>
    internal static string GetThumbnailName(string fullPath)
    {
        string uri = new Uri(fullPath).AbsoluteUri;
        byte[] hash = MD5.HashData(Encoding.UTF8.GetBytes(uri));
        return Convert.ToHexString(hash).ToLowerInvariant() + ".png";
    }
}
