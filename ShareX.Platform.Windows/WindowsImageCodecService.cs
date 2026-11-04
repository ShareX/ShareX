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
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace ShareX.Platform.Windows;

/// <summary>GDI+ (System.Drawing) for TIFF and the other formats Windows Imaging Component reads.</summary>
[SupportedOSPlatform("windows")]
public sealed class WindowsImageCodecService : IImageCodecService
{
    public FeatureSupport TiffSupport => FeatureSupport.Supported;

    public byte[]? DecodeToPng(string path)
    {
        try
        {
            using Image image = Image.FromFile(path);
            using MemoryStream png = new MemoryStream();
            image.Save(png, ImageFormat.Png);
            return png.ToArray();
        }
        catch (Exception e) when (e is OutOfMemoryException or ArgumentException or IOException or ExternalException)
        {
            // GDI+ reports an unknown format as OutOfMemoryException.
            return null;
        }
    }

    public byte[]? EncodeTiff(byte[] png)
    {
        using MemoryStream input = new MemoryStream(png, writable: false);
        using Image image = Image.FromStream(input);
        using MemoryStream tiff = new MemoryStream();
        image.Save(tiff, ImageFormat.Tiff);
        return tiff.ToArray();
    }
}
