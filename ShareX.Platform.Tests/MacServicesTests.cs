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
using System.Collections.Concurrent;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using Xunit;

namespace ShareX.Platform.Tests;

public sealed class MacOSFactAttribute : FactAttribute
{
    public MacOSFactAttribute()
    {
        if (!OperatingSystem.IsMacOS())
        {
            Skip = "Uses macOS frameworks; runs on the macos-latest CI runner.";
        }
    }
}

/// <summary>macOS services checked on a real Mac: the Cross-platform workflow's macos-latest runner.</summary>
public class MacServicesTests
{
    /// <summary>"ShareX reads macOS" in black on white, 520x80, rendered with SkiaSharp.</summary>
    private const string TextImage = "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";

    [Fact]
    public void VisionLanguages_MatchExactThenByLanguage()
    {
        string[] supported = ["en-US", "fr-FR", "zh-Hans"];

        Assert.Equal("en-US", VisionOcrService.MatchLanguage("en-US", supported));
        Assert.Equal("en-US", VisionOcrService.MatchLanguage("en", supported));
        Assert.Equal("fr-FR", VisionOcrService.MatchLanguage("fr-CA", supported));
        Assert.Equal("zh-Hans", VisionOcrService.MatchLanguage("zh", supported));
        Assert.Null(VisionOcrService.MatchLanguage("eng", supported));
        Assert.Null(VisionOcrService.MatchLanguage("", supported));
    }

    [Fact]
    public void PipePath_IsShortStableAndPerUser()
    {
        string name = "a-very-long-machine-name.local-someone-ShareX";
        string path = MacSystemInfoService.CreateShortPipePath(name, 501);

        Assert.StartsWith("/tmp/sharex-501-", path);
        Assert.True(path.Length < 104);
        Assert.Equal(path, MacSystemInfoService.CreateShortPipePath(name, 501));
        Assert.NotEqual(path, MacSystemInfoService.CreateShortPipePath(name, 502));
    }

    [MacOSFact]
    public void VisionOcr_ReadsRenderedText()
    {
        if (!OperatingSystem.IsMacOS()) return;
        VisionOcrService ocr = new VisionOcrService();

        Assert.True(ocr.Support.IsSupported, ocr.Support.Reason);
        Assert.Contains(ocr.GetLanguages(), language => language.Tag.StartsWith("en", StringComparison.OrdinalIgnoreCase));
        string text = ocr.RecognizeAsync(Convert.FromBase64String(TextImage), "en-US", false).GetAwaiter().GetResult();
        Assert.Equal("ShareX reads macOS", text.Trim());
    }

    [MacOSFact]
    public void QuickLook_ThumbnailsAPng()
    {
        if (!OperatingSystem.IsMacOS()) return;
        string file = Path.Combine(Path.GetTempPath(), "sharex-ql-" + Guid.NewGuid().ToString("N") + ".png");
        File.WriteAllBytes(file, Convert.FromBase64String(TextImage));

        try
        {
            byte[]? thumbnail = new QuickLookThumbnailService(CommandRunner.Default).GetThumbnail(file, 128, 128);
            Assert.NotNull(thumbnail);
            Assert.Equal(new byte[] { 0x89, 0x50, 0x4E, 0x47 }, thumbnail![..4]);
        }
        finally
        {
            File.Delete(file);
        }
    }

    [MacOSFact]
    public void LaunchDetached_StartsInOwnSessionWithArguments()
    {
        if (!OperatingSystem.IsMacOS()) return;
        string output = Path.Combine(Path.GetTempPath(), "sharex-maclaunch-" + Guid.NewGuid().ToString("N"));
        string script = "out=\"$1\"; shift; { echo $$; ps -o sess= -p $$ >/dev/null 2>&1; ps -o pgid= -p $$ | tr -d ' '; for a in \"$@\"; do printf '[%s]\n' \"$a\"; done; } > \"$out.tmp\" && mv \"$out.tmp\" \"$out\"";

        try
        {
            int pid = new MacApplicationLaunchService().LaunchDetached("/bin/sh", ["-c", script, "/bin/sh", output, "two words", ""]);
            SpinWait.SpinUntil(() => File.Exists(output), TimeSpan.FromSeconds(10));
            string[] lines = File.ReadAllLines(output);

            Assert.Equal(pid.ToString(), lines[0]);
            // A new session also starts a new process group led by the child.
            Assert.Equal(pid.ToString(), lines[1]);
            Assert.Equal(["[two words]", "[]"], lines.Skip(2));
        }
        finally
        {
            File.Delete(output);
        }
    }

    [Fact]
    public void Overlay_ConvertsTopLeftCoordinatesToCocoa()
    {
        CoreGraphics.CGRect rect = MacScreenOverlay.ToCocoa(new PlatformRectangle(100, 50, 200, 80), 900);

        Assert.Equal(100, rect.X);
        Assert.Equal(900 - 50 - 80, rect.Y);
        Assert.Equal(200, rect.Width);
        Assert.Equal(80, rect.Height);
    }

    [Fact]
    public void MouseHook_ReportsButtonTransitions()
    {
        PlatformPoint at = new PlatformPoint(3, 4);
        GlobalMouseButtonEvent[] changes = MacMouseHook.GetButtonChanges([false, false, true], [true, false, false], at, 9).ToArray();

        Assert.Equal([new GlobalMouseButtonEvent(GlobalMouseButton.Primary, true, at, 9), new GlobalMouseButtonEvent(GlobalMouseButton.Secondary, false, at, 9)], changes);
    }

    private sealed class Listener : IGlobalMouseListener
    {
        public ConcurrentQueue<PlatformPoint> Moves { get; } = new();
        public void OnMove(PlatformPoint position) => Moves.Enqueue(position);
        public void OnButton(GlobalMouseButtonEvent buttonEvent) { }
    }

    [MacOSFact]
    public void MouseHook_FollowsThePointer()
    {
        if (!OperatingSystem.IsMacOS()) return;
        Listener listener = new Listener();

        using (new MacMouseHook(listener))
        {
            Thread.Sleep(100);
            CoreGraphics.CGWarpMouseCursorPosition(new CoreGraphics.CGPoint { X = 123, Y = 77 });
            SpinWait.SpinUntil(() => listener.Moves.Contains(new PlatformPoint(123, 77)), TimeSpan.FromSeconds(3));
        }

        Assert.Contains(new PlatformPoint(123, 77), listener.Moves);
    }
}
