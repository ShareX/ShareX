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
using ShareX.Platform.Windows;
using System;
using System.Buffers.Binary;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Xunit;
using Xunit.Abstractions;

namespace ShareX.Platform.Tests;

// Synthetic textures only: no display duplication, desktop capture or HDR setting changes.
public sealed class HdrCaptureTests(ITestOutputHelper output)
{
    [Theory]
    [InlineData(0, 1, 0)]
    [InlineData(0.125, 1, 99)]
    [InlineData(0.25, 1, 137)]
    [InlineData(0.5, 1, 188)]
    [InlineData(1, 1, 255)]
    [InlineData(2, 2, 255)]
    public void ScRgbSdrPixelsRetainTheirSrgbEncoding(double linear, double whiteScale, byte encoded)
    {
        using Texture texture = HalfTexture(1, 1, [(float)linear, (float)linear, (float)linear, 0.25f]);
        PixelBuffer reference = HdrScreenCapture.ConvertToSdrReference(texture.Mapped, texture.Description, (float)whiteScale);
        PixelBuffer fallback = HdrScreenCapture.ConvertToSrgb(texture.Mapped, texture.Description, (float)whiteScale, 1000);
        Assert.Equal(new byte[] { encoded, encoded, encoded, 0 }, reference.Pixels);
        Assert.Equal(new byte[] { encoded, encoded, encoded, 255 }, fallback.Pixels);
    }

    [Fact]
    public void HalfFloatRowsRespectPaddingAndBgraChannelOrder()
    {
        using Texture texture = HalfTexture(2, 2,
        [
            0.5f, 0.25f, 0.125f, 0,
            0, 0, 0, 1,
            0, 1, 0, 0.5f,
            1, 0, 0, 0.75f
        ]);
        PixelBuffer image = HdrScreenCapture.ConvertToSrgb(texture.Mapped, texture.Description, 1, 1000);
        Assert.Equal(new byte[] { 99, 137, 188, 255, 0, 0, 0, 255, 0, 255, 0, 255, 0, 0, 255, 255 }, image.Pixels);
        Assert.Equal(8, image.Stride);
        texture.AssertPaddingUnchanged();
    }

    [Fact]
    public void SdrReferenceMasksExtendedRangeWithoutUsingSourceAlpha()
    {
        using Texture texture = HalfTexture(3, 1,
        [
            0.5f, 0.25f, 0.125f, 0,
            2, 0.5f, 0, 0,
            0, -0.25f, 0, 1
        ]);
        PixelBuffer image = HdrScreenCapture.ConvertToSdrReference(texture.Mapped, texture.Description, 1);
        Assert.Equal(new byte[] { 99, 137, 188, 0, 0, 188, 255, 255, 0, 0, 0, 255 }, image.Pixels);
    }

    [Fact]
    public void Hdr10WhiteMatchesScRgbAtTheWindowsSdrWhiteLevel()
    {
        // Microsoft specifies scRGB (1,1,1) and HDR10 (497,497,497) as 80-nit white:
        // https://learn.microsoft.com/windows/win32/direct3darticles/high-dynamic-range
        // A raised SDR white level makes this an in-range midtone, away from quantized clipping.
        using Texture half = HalfTexture(1, 1, [1, 1, 1, 0]);
        using Texture pq = PackedTexture(1, 1, [PackHdr10(497, 497, 497, 3)]);
        PixelBuffer scRgb = HdrScreenCapture.ConvertToSdrReference(half.Mapped, half.Description, 2);
        PixelBuffer hdr10 = HdrScreenCapture.ConvertToSdrReference(pq.Mapped, pq.Description, 2);
        for (int channel = 0; channel < 3; channel++)
        {
            Assert.InRange(Math.Abs(hdr10.Pixels[channel] - scRgb.Pixels[channel]), 0, 1);
        }
        Assert.Equal(0, hdr10.Pixels[3]);
    }

    [Fact]
    public void PackedHdr10RowsRespectPaddingAndPreserveBlackAndWhite()
    {
        using Texture texture = PackedTexture(2, 2,
            [PackHdr10(0, 0, 0, 3), PackHdr10(497, 497, 497, 0), PackHdr10(497, 497, 497, 3), PackHdr10(1023, 1023, 1023, 0)]);
        PixelBuffer image = HdrScreenCapture.ConvertToSdrReference(texture.Mapped, texture.Description, 2);
        Assert.Equal(new byte[] { 0, 0, 0, 0 }, image.Pixels[..4]);
        Assert.Equal(image.Pixels[4..8], image.Pixels[8..12]);
        Assert.Equal(new byte[] { 255, 255, 255, 255 }, image.Pixels[12..16]);
        texture.AssertPaddingUnchanged();
    }

    [Theory]
    [InlineData(497, 0, 0, 2)]
    [InlineData(0, 497, 0, 1)]
    [InlineData(0, 0, 497, 0)]
    public void WideGamutHdr10PrimariesKeepTheirDominantBgraChannel(uint red, uint green, uint blue, int dominant)
    {
        using Texture texture = PackedTexture(1, 1, [PackHdr10(red, green, blue, 0)]);
        PixelBuffer reference = HdrScreenCapture.ConvertToSdrReference(texture.Mapped, texture.Description, 1);
        PixelBuffer mapped = HdrScreenCapture.ConvertToSrgb(texture.Mapped, texture.Description, 1, 1000);
        Assert.Equal(255, reference.Pixels[dominant]);
        Assert.Equal(255, reference.Pixels[3]);
        Assert.Equal(255, mapped.Pixels[dominant]);
        Assert.Equal(255, mapped.Pixels[3]);
        for (int channel = 0; channel < 3; channel++)
        {
            if (channel == dominant) continue;
            Assert.Equal(0, reference.Pixels[channel]);
            Assert.InRange(mapped.Pixels[channel], 1, 254);
        }
    }

    [Fact]
    public void ExtendedGrayRampRemainsNeutralAndReachesTheDisplayPeak()
    {
        using Texture texture = HalfTexture(5, 1,
            [1.125f, 1.125f, 1.125f, 1, 2, 2, 2, 1, 4, 4, 4, 1, 8, 8, 8, 1, 12.5f, 12.5f, 12.5f, 1]);
        PixelBuffer image = HdrScreenCapture.ConvertToSrgb(texture.Mapped, texture.Description, 1, 1000);
        output.WriteLine("Fallback extended-range gray at scRGB 1.125: {0}; SDR white is preserved at 255.", image.Pixels[0]);
        byte previous = 0;
        for (int pixel = 0; pixel < 5; pixel++)
        {
            int offset = pixel * 4;
            Assert.Equal(image.Pixels[offset], image.Pixels[offset + 1]);
            Assert.Equal(image.Pixels[offset], image.Pixels[offset + 2]);
            Assert.InRange(image.Pixels[offset], previous, byte.MaxValue);
            Assert.Equal(255, image.Pixels[offset + 3]);
            previous = image.Pixels[offset];
        }
        Assert.Equal(255, previous);
    }

    [Theory]
    [InlineData(ModeRotation.Rotate90, 2, 3, new byte[] { 4, 1, 5, 2, 6, 3 })]
    [InlineData(ModeRotation.Rotate180, 3, 2, new byte[] { 6, 5, 4, 3, 2, 1 })]
    [InlineData(ModeRotation.Rotate270, 2, 3, new byte[] { 3, 6, 2, 5, 1, 4 })]
    public void RotationKeepsTheDesktopOrientationAndReferenceMask(ModeRotation rotation, int width, int height, byte[] labels)
    {
        PixelBuffer source = new(3, 2, LabelPixels([1, 2, 3, 4, 5, 6]));
        PixelBuffer rotated = HdrScreenCapture.RotateOutput(source, rotation);
        Assert.Equal(width, rotated.Width);
        Assert.Equal(height, rotated.Height);
        Assert.Equal(LabelPixels(labels), rotated.Pixels);
        Assert.Equal(LabelPixels([1, 2, 3, 4, 5, 6]), source.Pixels);
    }

    [Fact]
    public void UnrotatedOutputKeepsItsBuffer()
    {
        PixelBuffer source = new(3, 2, LabelPixels([1, 2, 3, 4, 5, 6]));
        Assert.Same(source, HdrScreenCapture.RotateOutput(source, ModeRotation.Identity));
        Assert.Same(source, HdrScreenCapture.RotateOutput(source, ModeRotation.Unspecified));
    }

    [WindowsFact]
    public void NativeWindowsToneMapperOrLegacyFallbackReadsSyntheticTexture()
    {
        Exception? failure = null;
        Thread thread = new(() =>
        {
            try
            {
                using Texture half = HalfTexture(2, 2, [0, 0, 0, 1, 1, 1, 1, 1, 4, 2, 1, 1, 0.5f, 0.25f, 0.125f, 1]);
                using Texture packed = PackedTexture(2, 2,
                    [PackHdr10(0, 0, 0, 3), PackHdr10(497, 497, 497, 3), PackHdr10(497, 0, 0, 3), PackHdr10(0, 497, 0, 3)]);
                foreach (Texture texture in new[] { half, packed })
                {
                    PixelBuffer converted;
                    try
                    {
                        converted = HdrScreenCapture.ConvertWithWindowsToneMapper(texture.Mapped, texture.Description);
                        output.WriteLine("Native IWICBitmapToneMapper converted the synthetic {0} texture.", texture.Description.Format);
                    }
                    catch (Exception exception) when (exception.HResult is unchecked((int)0x80004002) or unchecked((int)0x88982F80))
                    {
                        // The existing production path falls back when the factory or pixel format is unavailable.
                        converted = HdrScreenCapture.ConvertToSrgb(texture.Mapped, texture.Description, 1, 1000);
                        output.WriteLine("Native tone mapping returned {0:X8}; the production fallback converted the synthetic {1} texture.",
                            exception.HResult, texture.Description.Format);
                    }
                    Assert.Equal(2, converted.Width);
                    Assert.Equal(2, converted.Height);
                    Assert.All(Enumerable.Range(0, 4), pixel => Assert.Equal(255, converted.Pixels[pixel * 4 + 3]));
                    Assert.Equal(new byte[] { 0, 0, 0, 255 }, converted.Pixels[..4]);
                    texture.AssertPaddingUnchanged();
                }
            }
            catch (Exception exception) { failure = exception; }
        }) { IsBackground = true };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(30)), "Native tone-map verification did not complete.");
        if (failure != null) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(failure).Throw();
    }

    private static Texture HalfTexture(int width, int height, float[] rgba)
    {
        byte[] packed = new byte[rgba.Length * 2];
        for (int index = 0; index < rgba.Length; index++)
            BinaryPrimitives.WriteUInt16LittleEndian(packed.AsSpan(index * 2), BitConverter.HalfToUInt16Bits((Half)rgba[index]));
        return new Texture(width, height, 8, Format.R16G16B16A16_Float, packed);
    }

    private static Texture PackedTexture(int width, int height, uint[] pixels)
    {
        byte[] packed = new byte[pixels.Length * 4];
        for (int index = 0; index < pixels.Length; index++)
            BinaryPrimitives.WriteUInt32LittleEndian(packed.AsSpan(index * 4), pixels[index]);
        return new Texture(width, height, 4, Format.R10G10B10A2_UNorm, packed);
    }

    private static uint PackHdr10(uint red, uint green, uint blue, uint alpha) => red | green << 10 | blue << 20 | alpha << 30;

    private static byte[] LabelPixels(byte[] labels) => labels.SelectMany(label => new byte[] { label, (byte)(label + 10), (byte)(label + 20), (byte)(label * 17) }).ToArray();

    private sealed class Texture : IDisposable
    {
        private readonly byte[] source;
        private readonly GCHandle pinned;
        private readonly int rowBytes;
        private readonly int rowPitch;
        public MappedSubresource Mapped { get; }
        public Texture2DDescription Description { get; }

        public Texture(int width, int height, int bytesPerPixel, Format format, byte[] pixels)
        {
            rowBytes = width * bytesPerPixel;
            rowPitch = rowBytes + 16;
            source = new byte[rowPitch * height];
            Array.Fill(source, (byte)0xCD);
            Assert.Equal(rowBytes * height, pixels.Length);
            for (int y = 0; y < height; y++) pixels.AsSpan(y * rowBytes, rowBytes).CopyTo(source.AsSpan(y * rowPitch));
            pinned = GCHandle.Alloc(source, GCHandleType.Pinned);
            Mapped = new MappedSubresource(pinned.AddrOfPinnedObject(), (uint)rowPitch, (uint)source.Length);
            Description = new Texture2DDescription { Width = (uint)width, Height = (uint)height, Format = format };
        }

        public void AssertPaddingUnchanged()
        {
            for (int y = 0; y < Description.Height; y++)
                Assert.All(source.AsSpan(y * rowPitch + rowBytes, rowPitch - rowBytes).ToArray(), value => Assert.Equal(0xCD, value));
        }

        public void Dispose() => pinned.Free();
    }
}
