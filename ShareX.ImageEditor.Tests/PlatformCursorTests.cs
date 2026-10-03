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

using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Presentation.Rendering;
using ShareX.Platform;
using ShareX.Platform.Imaging;
using SkiaSharp;
using Xunit;

namespace ShareX.ImageEditor.Tests;

public sealed class PlatformCursorTests
{
    [Theory]
    [InlineData(CursorType.Arrow, SystemCursor.Arrow)]
    [InlineData(CursorType.Default, SystemCursor.Arrow)]
    [InlineData(CursorType.WaitCursor, SystemCursor.Wait)]
    [InlineData(CursorType.IBeam, SystemCursor.IBeam)]
    public void StandardAnnotationUsesThePlatformTheme(CursorType requested, SystemCursor expected)
    {
        ThemeGraphics graphics = new();
        using SKBitmap? image = CursorBitmapRenderer.RenderCursorBitmap(requested, graphics);
        Assert.NotNull(image);
        Assert.Equal(expected, graphics.RequestedCursor);
        Assert.Equal(new SKColor(30, 20, 10), image.GetPixel(0, 0));
        Assert.Equal(SKAlphaType.Premul, image.AlphaType);
    }

    [Fact]
    public void BundledCursorsRemainAvailableWithoutANativeTheme()
    {
        ThemeGraphics graphics = new() { CursorSupport = FeatureSupport.NotSupported("No cursor theme.") };
        using SKBitmap? bundled = CursorBitmapRenderer.RenderCursorBitmap(CursorType.HSplit, graphics);
        Assert.NotNull(bundled);
        Assert.Null(graphics.RequestedCursor);
        Assert.Null(CursorBitmapRenderer.RenderCursorBitmap(CursorType.Arrow, graphics));
        Assert.Null(CursorBitmapRenderer.RenderCursorBitmap((CursorType)int.MaxValue, graphics));
    }

    [Fact]
    public void PlatformStraightAlphaConvertsToSkiaWithoutChangingItsSource()
    {
        PixelBuffer source = new(3, 1, [255, 128, 64, 128, 10, 20, 30, 255, 30, 20, 10, 0]);
        byte[] expected = (byte[])source.Pixels.Clone();
        using SKBitmap image = SystemGraphicsBitmapConversion.ToSkBitmap(source);
        byte[] actual = new byte[image.ByteCount];
        System.Runtime.InteropServices.Marshal.Copy(image.GetPixels(), actual, 0, actual.Length);
        Assert.Equal(new byte[] { 128, 64, 32, 128, 10, 20, 30, 255, 0, 0, 0, 0 }, actual);
        Assert.Equal(expected, source.Pixels);
    }

    private sealed class ThemeGraphics : ISystemGraphicsService
    {
        public FeatureSupport EmojiSupport => FeatureSupport.NotSupported("Shared Skia emoji.");
        public FeatureSupport CursorSupport { get; init; } = FeatureSupport.Supported;
        public SystemCursor? RequestedCursor { get; private set; }
        public PixelBuffer? RenderEmoji(string text, int canvasSize, float fontSize) => null;
        public SystemCursorImage? GetSystemCursor(SystemCursor cursor, int? size = null)
        {
            RequestedCursor = cursor;
            return new SystemCursorImage(new PixelBuffer(1, 1, [10, 20, 30, 255]), new PlatformPoint(0, 0));
        }
    }
}
