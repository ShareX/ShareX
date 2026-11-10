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

using ShareX.HelpersLib;
using ShareX.Platform.Imaging;
using SkiaSharp;
using Xunit;

namespace ShareX.Tools.Tests;

public sealed class PlatformImageConverterTests
{
    [Fact]
    public void OpaqueCapturesAreCopiedWithTheSamePixelsAsTheConversion()
    {
        // 13 pixels wide so the rows are not a whole number of vectors.
        PixelBuffer buffer = new(13, 5);
        new Random(9131).NextBytes(buffer.Pixels);
        PixelBuffer converted = new(13, 5);
        buffer.Pixels.CopyTo(converted.Pixels, 0);
        PixelBuffer.MakeOpaque(converted.Pixels);
        buffer.MakeOpaque();

        using SKBitmap fast = PlatformImageConverter.ToSKBitmap(buffer);
        using SKBitmap slow = PlatformImageConverter.ToSKBitmap(converted);

        Assert.True(buffer.IsOpaque);
        Assert.False(converted.IsOpaque);
        Assert.Equal(slow.Bytes, fast.Bytes);
    }
}
