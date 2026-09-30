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
                    return File.ReadAllBytes(candidate);
                }
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
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
