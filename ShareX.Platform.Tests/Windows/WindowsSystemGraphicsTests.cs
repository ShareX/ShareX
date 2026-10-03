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
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace ShareX.Platform.Tests;

[Collection("Windows platform services")]
public sealed class WindowsSystemGraphicsTests
{
    [Fact]
    public void PremultipliedNativePixelsBecomeStraightBgra()
    {
        PixelBuffer result = WindowsSystemGraphicsService.FromPremultipliedBgra(3, 1,
            [128, 64, 32, 128, 100, 200, 255, 0, 10, 20, 30, 255]);
        Assert.Equal(new byte[] { 255, 128, 64, 128, 0, 0, 0, 0, 10, 20, 30, 255 }, result.Pixels);
    }

    [WindowsFact]
    public void EveryStandardCursorRendersAtItsNativeAndRequestedSizes()
    {
        using WindowsSystemGraphicsService graphics = new();
        Assert.True(graphics.CursorSupport.IsSupported);
        foreach (SystemCursor cursor in Enum.GetValues<SystemCursor>())
        {
            foreach (int? size in new int?[] { null, 24, 48 })
            {
                SystemCursorImage? result = graphics.GetSystemCursor(cursor, size);
                Assert.NotNull(result);
                if (size.HasValue) Assert.Equal(size.Value, result.Image.Width);
                Assert.InRange(result.Hotspot.X, 0, result.Image.Width - 1);
                Assert.InRange(result.Hotspot.Y, 0, result.Image.Height - 1);
                Assert.True(result.Image.Pixels.Where((_, index) => index % 4 == 3).Any(alpha => alpha > 0),
                    $"{cursor} at size {size?.ToString() ?? "native"} should contain visible pixels.");
            }
        }
        Assert.Null(graphics.GetSystemCursor((SystemCursor)int.MaxValue));
        Assert.Null(graphics.GetSystemCursor(SystemCursor.Arrow, 0));
        Assert.Null(graphics.GetSystemCursor(SystemCursor.Arrow, -1));
    }

    [WindowsFact]
    public void CursorResizingPreservesThinStrokesAndStraightAlpha()
    {
        PixelBuffer source = new(2, 1, [0, 0, 255, 255, 255, 0, 0, 0]);
        byte[] expectedSource = (byte[])source.Pixels.Clone();
        PixelBuffer resized = WindowsSystemGraphicsService.ResizeCursor(source, 1, 1);
        Assert.Equal(expectedSource, source.Pixels);
        Assert.Equal(1, resized.Width);
        Assert.Equal(1, resized.Height);
        Assert.InRange(resized.Pixels[3], (byte)127, (byte)128);
        Assert.Equal(new byte[] { 0, 0, 255 }, resized.Pixels[..3]);
    }

    [WindowsFact]
    public void CursorReadsOwnTheirPixelsAndReleaseTheirGdiObjects()
    {
        using WindowsSystemGraphicsService graphics = new();
        SystemCursorImage original = graphics.GetSystemCursor(SystemCursor.Arrow)!;
        byte[] expected = (byte[])original.Image.Pixels.Clone();
        Array.Clear(original.Image.Pixels);
        Assert.Equal(expected, graphics.GetSystemCursor(SystemCursor.Arrow)!.Image.Pixels);

        // Warm every native cursor resource before measuring per-read GDI objects.
        foreach (SystemCursor cursor in Enum.GetValues<SystemCursor>()) Assert.NotNull(graphics.GetSystemCursor(cursor));
        using Process process = Process.GetCurrentProcess();
        uint before = GetGuiResources(process.Handle, 0);
        for (int i = 0; i < 10; i++)
            foreach (SystemCursor cursor in Enum.GetValues<SystemCursor>()) Assert.NotNull(graphics.GetSystemCursor(cursor, 48));
        Assert.Equal(before, GetGuiResources(process.Handle, 0));
    }

    [WindowsFact]
    public async Task EmojiRendersInColorAcrossBackgroundCallsAndDisposesFactories()
    {
        WindowsSystemGraphicsService graphics = new();
        Assert.True(graphics.EmojiSupport.IsSupported);
        PixelBuffer?[] results = await Task.WhenAll(Enumerable.Range(0, 8).Select(_ =>
            Task.Run(() => graphics.RenderEmoji("😀", 128, 64))));
        foreach (PixelBuffer? pixels in results)
        {
            Assert.NotNull(pixels);
            Assert.Equal(128, pixels.Width);
            bool colored = false;
            for (int i = 0; i < pixels.Pixels.Length; i += 4)
                colored |= pixels.Pixels[i + 3] > 0 && pixels.Pixels[i] != pixels.Pixels[i + 2];
            Assert.True(colored, "DirectWrite should draw Segoe UI Emoji in color.");
            Assert.Equal(results[0]!.Pixels, pixels.Pixels);
        }
        Assert.Null(graphics.RenderEmoji(" ", 128, 64));
        Assert.Null(graphics.RenderEmoji("😀", 0, 64));
        Assert.Null(graphics.RenderEmoji("😀", 128, float.NaN));
        graphics.Dispose();
        graphics.Dispose();
        Assert.False(graphics.EmojiSupport.IsSupported);
        Assert.False(graphics.CursorSupport.IsSupported);
        Assert.Null(graphics.RenderEmoji("😀", 128, 64));
        Assert.Null(graphics.GetSystemCursor(SystemCursor.Arrow));
    }

    [DllImport("user32.dll")]
    private static extern uint GetGuiResources(IntPtr process, uint flag);
}
