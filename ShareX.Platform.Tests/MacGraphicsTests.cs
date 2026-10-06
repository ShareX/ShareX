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

using ShareX.Platform.Diagnostics;
using ShareX.Platform.MacOS;
using ShareX.Platform.MacOS.Native;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace ShareX.Platform.Tests;

public class MacGraphicsTests
{
    [Fact]
    public void CursorSelectors_CoverEveryCursorAndEndWithPublicNames()
    {
        foreach (SystemCursor cursor in Enum.GetValues<SystemCursor>())
        {
            Assert.True(AppKitGraphicsService.CursorSelectors.TryGetValue(cursor, out string[]? selectors), cursor.ToString());

            // A private name is never the last resort when a public one exists, and every name is a class method name.
            Assert.All(selectors!, selector => Assert.DoesNotContain(":", selector));
            int firstPublic = Array.FindIndex(selectors!, selector => !selector.StartsWith('_'));
            Assert.True(firstPublic < 0 || selectors!.Skip(firstPublic).All(selector => !selector.StartsWith('_')), cursor.ToString());
        }

        Assert.Equal(["arrowCursor"], AppKitGraphicsService.CursorSelectors[SystemCursor.Arrow]);
        Assert.Empty(AppKitGraphicsService.CursorSelectors[SystemCursor.Wait]);
    }

    [Theory]
    [InlineData(new[] { 24, 48 }, null, 1)]
    [InlineData(new[] { 48, 24 }, null, 0)]
    [InlineData(new[] { 24, 48 }, 24, 0)]
    [InlineData(new[] { 24, 48 }, 40, 1)]
    [InlineData(new[] { 24, 48 }, 36, 1)]
    [InlineData(new[] { 32 }, 128, 0)]
    public void SelectRepresentation_NearestHeightElseLargest(int[] heights, int? wanted, int expected)
    {
        Assert.Equal(expected, AppKitImages.SelectRepresentation(heights, wanted));
    }

    [Fact]
    public void FileMediaSupport_UsesConfiguredOrPathFFmpeg()
    {
        string ffmpeg = Path.GetTempFileName();

        try
        {
            MacScreenRecordingService none = new MacScreenRecordingService(new RecordingRunner(), () => []);
            MacScreenRecordingService onPath = new MacScreenRecordingService(new RecordingRunner("ffmpeg"), () => []);

            Assert.True(none.GetFileMediaSupport(ffmpeg).IsSupported);
            Assert.True(onPath.GetFileMediaSupport("").IsSupported);
            FeatureSupport missing = none.GetFileMediaSupport(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));
            Assert.False(missing.IsSupported);
            Assert.Contains("FFmpeg", missing.Reason);
        }
        finally
        {
            File.Delete(ffmpeg);
        }
    }

    [MacOSFact]
    public void Cursors_HaveImagesAndHotspotsInside()
    {
        if (!OperatingSystem.IsMacOS()) return;
        AppKitGraphicsService graphics = new AppKitGraphicsService();

        Assert.True(graphics.CursorSupport.IsSupported);

        // A CI runner without a window server session renders no cursor images; a desktop Mac must.
        if (graphics.GetSystemCursor(SystemCursor.Arrow) == null && Environment.GetEnvironmentVariable("CI") == "true")
        {
            Console.WriteLine("No cursor images on this headless runner.");
            return;
        }

        foreach (SystemCursor cursor in new[] { SystemCursor.Arrow, SystemCursor.IBeam, SystemCursor.Cross, SystemCursor.Hand })
        {
            SystemCursorImage? image = graphics.GetSystemCursor(cursor);
            Assert.NotNull(image);
            Assert.InRange(image!.Hotspot.X, 0, image.Image.Width - 1);
            Assert.InRange(image.Hotspot.Y, 0, image.Image.Height - 1);
        }

        SystemCursorImage? small = graphics.GetSystemCursor(SystemCursor.Arrow, 16);
        SystemCursorImage? large = graphics.GetSystemCursor(SystemCursor.Arrow);
        Assert.True(small!.Image.Height <= large!.Image.Height);
        Assert.Null(graphics.GetSystemCursor(SystemCursor.Wait));
    }

    [MacOSFact]
    public void PlatformServices_OfferVisionOcrAndCursors()
    {
        if (!OperatingSystem.IsMacOS()) return;
        using MacPlatformServices services = new MacPlatformServices(PlatformDetector.Detect(), CommandRunner.Default);

        Assert.IsType<VisionOcrService>(services.Ocr);
        Assert.True(services.Ocr.Support.IsSupported, services.Ocr.Support.Reason);
        Assert.True(services.Graphics.CursorSupport.IsSupported);
    }
}
