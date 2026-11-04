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

namespace ShareX.Platform;

/// <summary>
/// Image formats SkiaSharp cannot read or write, through the system's codecs: TIFF with GDI+ on Windows. Images are exchanged
/// as PNG bytes.
/// </summary>
public interface IImageCodecService
{
    FeatureSupport TiffSupport { get; }

    /// <summary>Any image the system can read (TIFF and others) as PNG, or null when it cannot.</summary>
    byte[]? DecodeToPng(string path);

    /// <summary>Encodes a PNG image as TIFF, or returns null when the system cannot.</summary>
    byte[]? EncodeTiff(byte[] png);
}

public sealed class UnsupportedImageCodecService(string reason) : IImageCodecService
{
    public FeatureSupport TiffSupport { get; } = FeatureSupport.NotSupported(reason);

    public byte[]? DecodeToPng(string path) => null;

    public byte[]? EncodeTiff(byte[] png) => null;
}
