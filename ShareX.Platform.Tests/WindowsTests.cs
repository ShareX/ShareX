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
using Xunit;

namespace ShareX.Platform.Tests;

public class WindowsFormatTests
{
    private static readonly ShellMenuEntry Upload = new ShellMenuEntry("ShareX", "Upload with ShareX", @"C:\Program Files\ShareX\ShareX.exe", [], ShellMenuTarget.FilesAndFolders);

    private static readonly ShellMenuEntry Edit = new ShellMenuEntry("ShareXImageEditor", "Edit with ShareX", @"C:\Program Files\ShareX\ShareX.exe", ["-ImageEditor"], ShellMenuTarget.Images);

    [Fact]
    public void ShellMenu_KeepsExistingRegistryLayout()
    {
        Assert.Equal(new[] { @"Software\Classes\*\shell\ShareX", @"Software\Classes\Directory\shell\ShareX" }, WindowsShellIntegrationService.GetMenuKeys(Upload));
        Assert.Equal(new[] { @"Software\Classes\SystemFileAssociations\image\shell\ShareXImageEditor" }, WindowsShellIntegrationService.GetMenuKeys(Edit));
        Assert.Equal("\"C:\\Program Files\\ShareX\\ShareX.exe\" \"%1\"", WindowsShellIntegrationService.GetCommand(Upload));
        Assert.Equal("\"C:\\Program Files\\ShareX\\ShareX.exe\" -ImageEditor \"%1\"", WindowsShellIntegrationService.GetCommand(Edit));
        Assert.Equal("\"C:\\Program Files\\ShareX\\ShareX.exe\",0", WindowsShellIntegrationService.GetIcon(Upload));
    }

    [Theory]
    [InlineData("-silent", "-silent")]
    [InlineData(@"C:\My Files", "\"C:\\My Files\"")]
    [InlineData("a\"b", "\"a\\\"b\"")]
    [InlineData("", "\"\"")]
    public void StartupArguments_AreQuoted(string argument, string expected)
    {
        Assert.Equal(expected, WindowsStartupService.QuoteArgument(argument));
    }

    [Fact]
    public void CredentialTarget_CombinesServiceAndAccount()
    {
        Assert.Equal("ShareX:imgur", WindowsCredentialService.GetTargetName("ShareX", "imgur"));
    }

    [Fact]
    public void Dib_RoundTripsTopDownPixels()
    {
        PixelBuffer image = new PixelBuffer(2, 2, [1, 2, 3, 255, 4, 5, 6, 128, 7, 8, 9, 255, 10, 11, 12, 0]);

        byte[] dib = WindowsClipboardService.CreateDib(image);
        PixelBuffer? read = WindowsClipboardService.ReadDib(dib);

        Assert.NotNull(read);
        Assert.Equal(image.Pixels, read.Pixels);
        // Bottom up: the last row is stored first.
        Assert.Equal(7, dib[40]);
    }

    [Fact]
    public void Dib_TreatsZeroAlphaAsOpaque()
    {
        PixelBuffer image = new PixelBuffer(1, 1, [1, 2, 3, 0]);

        PixelBuffer? read = WindowsClipboardService.ReadDib(WindowsClipboardService.CreateDib(image));

        Assert.Equal(new byte[] { 1, 2, 3, 255 }, read!.Pixels);
    }

    [Fact]
    public void Dib_RejectsTruncatedData()
    {
        Assert.Null(WindowsClipboardService.ReadDib(new byte[10]));
    }

    [Fact]
    public void GdiGrab_UsesVirtualScreenOffset()
    {
        FFmpegVideoInput input = WindowsScreenRecordingService.CreateGdiGrabInput(new ScreenRecordingRequest(), new PlatformRectangle(-1920, 0, 3841, 1080));

        Assert.Equal("gdigrab", input.Device);
        Assert.Equal("-f gdigrab -thread_queue_size 1024 -rtbufsize 256M -framerate 30 -offset_x -1920 -offset_y 0 -video_size 3840x1080 -draw_mouse 1 -i desktop", input.InputArguments);
    }

    [Fact]
    public void DdaGrab_PicksOutputWithLargestOverlap()
    {
        ScreenInfo primary = new ScreenInfo(@"\\.\DISPLAY1", "Primary", new PlatformRectangle(0, 0, 1920, 1080), new PlatformRectangle(0, 0, 1920, 1040), true, 1);
        ScreenInfo secondary = new ScreenInfo(@"\\.\DISPLAY2", "Secondary", new PlatformRectangle(1920, 0, 2560, 1440), new PlatformRectangle(1920, 0, 2560, 1440), false, 1);
        ScreenRecordingRequest request = new ScreenRecordingRequest { Region = new PlatformRectangle(2000, 100, 800, 600), FrameRate = 60 };

        FFmpegVideoInput input = WindowsScreenRecordingService.CreateDdaGrabInput(request, [secondary, primary]);

        Assert.Equal("-f lavfi -i ddagrab=output_idx=1:draw_mouse=true:framerate=60:offset_x=80:offset_y=100:video_size=800x600:output_fmt=bgra", input.InputArguments);
        Assert.Equal(new[] { "hwdownload", "format=bgra" }, input.VideoFilters);
    }

    [Fact]
    public void DdaGrab_RequiresAScreen()
    {
        Assert.Throws<InvalidOperationException>(() => WindowsScreenRecordingService.CreateDdaGrabInput(new ScreenRecordingRequest(), []));
    }
}

public class TransparentWindowCaptureTests
{
    private static PixelBuffer Solid(int width, int height, byte b, byte g, byte r)
    {
        PixelBuffer buffer = new PixelBuffer(width, height);

        for (int i = 0; i < buffer.Pixels.Length; i += 4)
        {
            buffer.Pixels[i] = b;
            buffer.Pixels[i + 1] = g;
            buffer.Pixels[i + 2] = r;
            buffer.Pixels[i + 3] = 255;
        }

        return buffer;
    }

    [Fact]
    public void CombineBackgrounds_RecoversAlphaAndColour()
    {
        // A pure red pixel at 50% alpha: over white (255, 128, 128) in RGB, over black (128, 0, 0).
        PixelBuffer white = Solid(1, 1, 128, 128, 255);
        PixelBuffer black = Solid(1, 1, 0, 0, 128);

        PixelBuffer result = TransparentWindowCapture.CombineBackgrounds(white, black);

        Assert.Equal(128, result.Pixels[3]);
        Assert.Equal(255, result.Pixels[2]);
        Assert.Equal(0, result.Pixels[1]);
        Assert.Equal(0, result.Pixels[0]);
    }

    [Fact]
    public void CombineBackgrounds_OpaqueAndFullyTransparent()
    {
        PixelBuffer white = new PixelBuffer(2, 1, [10, 20, 30, 255, 255, 255, 255, 255]);
        PixelBuffer black = new PixelBuffer(2, 1, [10, 20, 30, 255, 0, 0, 0, 255]);

        PixelBuffer result = TransparentWindowCapture.CombineBackgrounds(white, black);

        Assert.Equal(new byte[] { 10, 20, 30, 255, 0, 0, 0, 0 }, result.Pixels);
    }

    [Fact]
    public void FindAutoCropRectangle_CropsTransparentBorder()
    {
        PixelBuffer image = new PixelBuffer(5, 4);
        image.Pixels[((2 * 5) + 1) * 4 + 3] = 255;
        image.Pixels[((1 * 5) + 3) * 4 + 3] = 40;

        Assert.Equal(new PlatformRectangle(1, 1, 3, 2), TransparentWindowCapture.FindAutoCropRectangle(image));
    }
}
