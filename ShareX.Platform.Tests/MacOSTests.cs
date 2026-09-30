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

using ShareX.Platform.MacOS;
using ShareX.Platform.MacOS.Native;
using System;
using System.Collections.Generic;
using Xunit;

namespace ShareX.Platform.Tests;

public class LaunchAgentTests
{
    private static readonly StartupRegistration Registration =
        new StartupRegistration("ShareX", "ShareX", "/Applications/ShareX.app/Contents/MacOS/ShareX", ["-silent", "a&b"]) { BundleIdentifier = "com.getsharex.ShareX" };

    [Fact]
    public void CreatePlist_IsValidAndReadable()
    {
        string plist = LaunchAgentStartupService.CreatePlist(Registration);

        Assert.Contains("<string>com.getsharex.ShareX</string>", plist);
        Assert.Contains("<string>a&amp;b</string>", plist);
        Assert.Contains("<key>RunAtLoad</key>\n  <true/>", plist);
        Assert.Equal(Registration.ExecutablePath, LaunchAgentStartupService.ReadProgram(plist));
    }

    [Fact]
    public void ReadProgram_ReturnsNullForInvalidXml()
    {
        Assert.Null(LaunchAgentStartupService.ReadProgram("not xml"));
    }

    [Fact]
    public void GetLabel_FallsBackToDefault()
    {
        Assert.Equal("com.getsharex.ShareX", LaunchAgentStartupService.GetLabel(Registration));
        Assert.False(string.IsNullOrEmpty(LaunchAgentStartupService.GetLabel(Registration with { BundleIdentifier = null })));
    }

    [Theory]
    [InlineData("\t\"com.getsharex.ShareX\" => disabled", true)]
    [InlineData("\t\"com.getsharex.ShareX\" => true", true)]
    [InlineData("\t\"com.getsharex.ShareX\" => enabled", false)]
    [InlineData("\t\"com.getsharex.ShareX.helper\" => disabled", false)]
    public void IsDisabledLine_ParsesLaunchctlOutput(string line, bool expected)
    {
        Assert.Equal(expected, LaunchAgentStartupService.IsDisabledLine(line, "com.getsharex.ShareX"));
    }
}

public class MacCaptureTests
{
    private static readonly ScreenInfo Retina = new ScreenInfo("1", "Built-in", new PlatformRectangle(0, 0, 1512, 982), new PlatformRectangle(0, 25, 1512, 957), true, 2);
    private static readonly ScreenInfo External = new ScreenInfo("2", "Studio Display", new PlatformRectangle(1512, 0, 2560, 1440), new PlatformRectangle(1512, 0, 2560, 1440), false, 1);

    [Fact]
    public void CreateArguments_Region()
    {
        List<string> arguments = MacScreenCaptureService.CreateArguments(ScreenCaptureRequest.ForRegion(new PlatformRectangle(10, 20, 300, 200), includeCursor: true), "/tmp/a.png");

        Assert.Equal(new[] { "-x", "-t", "png", "-C", "-R10,20,300,200", "/tmp/a.png" }, arguments);
    }

    [Fact]
    public void AVFoundation_RecordsWholePrimaryScreen()
    {
        FFmpegVideoInput input = MacScreenRecordingService.CreateAVFoundationInput(new ScreenRecordingRequest(), [Retina, External]);

        Assert.Equal("avfoundation", input.Device);
        Assert.Contains("-i \"Capture screen 0:none\"", input.InputArguments);
        Assert.Empty(input.VideoFilters);
    }

    [Fact]
    public void AVFoundation_CropsRegionInPixels()
    {
        ScreenRecordingRequest request = new ScreenRecordingRequest { Region = new PlatformRectangle(1612, 100, 401, 300), DrawCursor = false };

        FFmpegVideoInput input = MacScreenRecordingService.CreateAVFoundationInput(request, [Retina, External]);

        Assert.Contains("-capture_cursor 0", input.InputArguments);
        Assert.Contains("\"Capture screen 1:none\"", input.InputArguments);
        Assert.Equal(new[] { "crop=400:300:100:100" }, input.VideoFilters);
    }

    [Fact]
    public void AVFoundation_ScalesRetinaRegion()
    {
        ScreenRecordingRequest request = new ScreenRecordingRequest { Region = new PlatformRectangle(10, 10, 100, 50) };

        FFmpegVideoInput input = MacScreenRecordingService.CreateAVFoundationInput(request, [Retina]);

        Assert.Equal(new[] { "crop=200:100:20:20" }, input.VideoFilters);
    }

    [Fact]
    public void AVFoundation_RequiresAScreen()
    {
        Assert.Throws<InvalidOperationException>(() => MacScreenRecordingService.CreateAVFoundationInput(new ScreenRecordingRequest(), []));
    }

    [Fact]
    public void KeyMap_UsesAnsiPositions()
    {
        Assert.True(MacKeyMap.TryGetKeyCode('A', out uint a));
        Assert.Equal(0x00u, a);
        Assert.True(MacKeyMap.TryGetKeyCode(VirtualKeys.PrintScreen, out uint print));
        Assert.Equal(0x69u, print);
        Assert.Equal(Carbon.cmdKey | Carbon.shiftKey, MacKeyMap.ToCarbonModifiers(HotkeyModifiers.Super | HotkeyModifiers.Shift));
    }
}
