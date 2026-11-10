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

using ShareX.Platform.Imaging;
using Xunit;

namespace ShareX.Platform.Tests;

public sealed class PixelBufferTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(64)]
    [InlineData(67)]
    public void MakeOpaqueSetsEveryAlphaAndKeepsTheColors(int pixelCount)
    {
        // Lengths that are and are not whole vectors, so both the vector loop and the remainder run.
        byte[] pixels = new byte[pixelCount * 4];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = (byte)(i % 4 == 3 ? 0 : i);
        }

        PixelBuffer.MakeOpaque(pixels);

        for (int i = 0; i < pixels.Length; i++)
        {
            Assert.Equal(i % 4 == 3 ? 255 : (byte)i, pixels[i]);
        }
    }
}
