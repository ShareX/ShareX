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

using ShareX.Platform.Linux;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace ShareX.Platform.Tests;

public class DesktopEntryTests
{
    [Theory]
    [InlineData("/usr/bin/sharex", "/usr/bin/sharex")]
    [InlineData("/opt/Share X/sharex", "\"/opt/Share X/sharex\"")]
    [InlineData("100%", "100%%")]
    [InlineData("a\"b$c", "\"a\\\"b\\$c\"")]
    [InlineData("", "\"\"")]
    public void QuoteArgument_FollowsSpecification(string argument, string expected)
    {
        Assert.Equal(expected, DesktopEntry.QuoteArgument(argument));
    }

    [Theory]
    [InlineData("/usr/bin/sharex")]
    [InlineData("/opt/Share X/sharex", "-silent")]
    [InlineData("/usr/bin/sharex", "-ImageEditor", "with \"quotes\" and `ticks`", "50%", "back\\slash")]
    [InlineData("/usr/bin/sharex", "")]
    public void BuildExec_RoundTripsThroughParseExec(string executable, params string[] arguments)
    {
        string exec = DesktopEntry.BuildExec(executable, arguments, appendFileCode: true);

        Assert.EndsWith(" %F", exec);
        Assert.Equal(new[] { executable }.Concat(arguments), DesktopEntry.ParseExec(exec));
    }

    [Fact]
    public void Parse_ReadsOnlyRequestedGroup()
    {
        const string content = "# comment\r\n[Desktop Entry]\r\nName=ShareX\r\nName[de]=ShareX DE\r\nExec=sharex\r\n\r\n[Desktop Action edit]\r\nName=Edit\r\n";

        Dictionary<string, string> entry = DesktopEntry.Parse(content);

        Assert.Equal("ShareX", entry["Name"]);
        Assert.Equal("ShareX DE", entry["Name[de]"]);
        Assert.Equal(3, entry.Count);
        Assert.Equal("Edit", DesktopEntry.Parse(content, "Desktop Action edit")["Name"]);
    }

    [Fact]
    public void EscapeValue_EscapesControlCharacters()
    {
        Assert.Equal("a\\\\b\\nc", DesktopEntry.EscapeValue("a\\b\nc"));
    }
}

public class XdgPathServiceTests
{
    private const string Home = "/home/user";

    private static XdgPathService Create(Dictionary<string, string>? environment = null, string? userDirs = null) =>
        new XdgPathService(name => environment?.GetValueOrDefault(name), path => path == "/home/user/.config/user-dirs.dirs" ? userDirs : null, Home);

    [UnixFact]
    public void BaseDirectories_FallBackToHome()
    {
        XdgPathService paths = Create();

        Assert.Equal("/home/user/.config", paths.ConfigHome);
        Assert.Equal("/home/user/.local/share", paths.DataHome);
        Assert.Equal("/home/user/.cache/ShareX", paths.GetCacheDirectory("ShareX"));
        Assert.Equal("/home/user/.config/ShareX", paths.GetDefaultPersonalFolder("ShareX"));
    }

    [UnixFact]
    public void BaseDirectories_IgnoreRelativeValues()
    {
        XdgPathService paths = Create(new Dictionary<string, string> { ["XDG_CONFIG_HOME"] = "relative/config", ["XDG_DATA_HOME"] = "/data" });

        Assert.Equal("/home/user/.config", paths.ConfigHome);
        Assert.Equal("/data", paths.DataHome);
    }

    [UnixFact]
    public void UserDirectories_ReadLocalisedUserDirs()
    {
        const string userDirs = "# written by xdg-user-dirs-update\nXDG_DESKTOP_DIR=\"$HOME/Schreibtisch\"\nXDG_PICTURES_DIR=\"$HOME/Bilder\"\nXDG_VIDEOS_DIR=\"$HOME/\"\nXDG_DOCUMENTS_DIR=\"/mnt/docs\"\n";
        XdgPathService paths = Create(userDirs: userDirs);

        Assert.Equal("/home/user/Bilder", paths.GetPicturesDirectory());
        Assert.Equal("/home/user/Schreibtisch", paths.GetDesktopDirectory());
        Assert.Equal("/mnt/docs", paths.GetDocumentsDirectory());
        // A folder pointing at $HOME is disabled, so the default name is used.
        Assert.Equal("/home/user/Videos", paths.GetVideosDirectory());
    }

    [UnixFact]
    public void UserDirectories_PreferEnvironment()
    {
        XdgPathService paths = Create(new Dictionary<string, string> { ["XDG_PICTURES_DIR"] = "~/Shots" }, "XDG_PICTURES_DIR=\"$HOME/Bilder\"");

        Assert.Equal("/home/user/Shots", paths.GetPicturesDirectory());
    }
}

public sealed class XdgAutostartServiceTests : IDisposable
{
    private readonly string directory = Path.Combine(Path.GetTempPath(), "sharex-autostart-" + Guid.NewGuid().ToString("N"));

    private static readonly StartupRegistration Registration = new StartupRegistration("ShareX", "Share X", "/opt/Share X/ShareX", ["-silent"]) { IconName = "sharex" };

    [Fact]
    public void CreateEntry_WritesQuotedExec()
    {
        string entry = XdgAutostartService.CreateEntry(Registration);

        Assert.Contains("Exec=\"/opt/Share X/ShareX\" -silent\n", entry);
        Assert.Contains("Name=Share X\n", entry);
        Assert.Contains("Icon=sharex\n", entry);
    }

    [Fact]
    public void SetEnabled_RoundTripsState()
    {
        XdgAutostartService service = new XdgAutostartService(directory);

        Assert.Equal(StartupRegistrationState.Disabled, service.GetState(Registration));

        service.SetEnabled(Registration, true);
        Assert.Equal(StartupRegistrationState.Enabled, service.GetState(Registration));
        Assert.Equal(StartupRegistrationState.Disabled, service.GetState(Registration with { ExecutablePath = "/usr/bin/other" }));

        File.AppendAllText(service.GetEntryPath(Registration), "Hidden=true\n");
        Assert.Equal(StartupRegistrationState.DisabledByUser, service.GetState(Registration));

        service.SetEnabled(Registration, false);
        Assert.False(File.Exists(service.GetEntryPath(Registration)));
    }

    public void Dispose()
    {
        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, true);
        }
    }
}

public class LinuxShellIntegrationTests
{
    private static readonly ShellMenuEntry Upload = new ShellMenuEntry("ShareX", "Upload with ShareX", "/opt/Share X/ShareX", [], ShellMenuTarget.FilesAndFolders) { Icon = "sharex" };

    private static readonly ShellMenuEntry Edit = new ShellMenuEntry("ShareXImageEditor", "Edit with ShareX", "/usr/bin/sharex", ["-ImageEditor"], ShellMenuTarget.Images);

    [Fact]
    public void CreateScript_QuotesForShell()
    {
        string script = LinuxShellIntegrationService.CreateScript(Upload);

        Assert.StartsWith("#!/bin/sh\n", script);
        Assert.EndsWith("exec '/opt/Share X/ShareX' \"$@\"\n", script);
        Assert.DoesNotContain("case", script);
    }

    [Fact]
    public void CreateScript_FiltersImages()
    {
        string script = LinuxShellIntegrationService.CreateScript(Edit);

        Assert.Contains("*.png|*.jpg", script);
        Assert.EndsWith("exec '/usr/bin/sharex' '-ImageEditor' \"$@\"\n", script);
    }

    [Fact]
    public void ShellQuote_EscapesSingleQuotes()
    {
        Assert.Equal("'it'\\''s'", LinuxShellIntegrationService.ShellQuote("it's"));
    }

    [Fact]
    public void CreateDolphinServiceMenu_UsesMimeTypes()
    {
        string upload = LinuxShellIntegrationService.CreateDolphinServiceMenu(Upload);
        string edit = LinuxShellIntegrationService.CreateDolphinServiceMenu(Edit);

        Assert.Contains("MimeType=all/allfiles;inode/directory;\n", upload);
        Assert.Contains("Exec=\"/opt/Share X/ShareX\" %F\n", upload);
        Assert.Contains("Icon=sharex\n", upload);
        Assert.Contains("MimeType=image/png;", edit);
        Assert.Contains("Exec=/usr/bin/sharex -ImageEditor %F\n", edit);
    }

    [Fact]
    public void CreateNemoAction_UsesExtensions()
    {
        Assert.Contains("Extensions=any;dir;\n", LinuxShellIntegrationService.CreateNemoAction(Upload));
        Assert.Contains("Extensions=png;jpg;", LinuxShellIntegrationService.CreateNemoAction(Edit));
    }
}

public class LinuxParsingTests
{
    [Fact]
    public void MimeDatabase_ReadsGlobs2AndMimeTypes_FirstDefinitionWins()
    {
        Dictionary<string, string> types = new Dictionary<string, string>();
        MimeDatabase.ParseGlobs2Line("# comment", types);
        MimeDatabase.ParseGlobs2Line("50:image/png:*.png", types);
        MimeDatabase.ParseGlobs2Line("50:text/x-readme:README", types);
        MimeDatabase.ParseGlobs2Line("50:application/x-tar:*.tar.*", types);
        MimeDatabase.ParseMimeTypesLine("image/x-png png", types);
        MimeDatabase.ParseMimeTypesLine("video/webm\twebm WEBMX", types);

        Assert.Equal("image/png", types["png"]);
        Assert.Equal("video/webm", types["webm"]);
        Assert.Equal("video/webm", types["webmx"]);
        Assert.False(types.ContainsKey("readme"));
        Assert.Equal(3, types.Count);
    }

    [Theory]
    [InlineData("mike wheel video", true)]
    [InlineData("mike sudo", true)]
    [InlineData("staff admin", true)]
    [InlineData("mike video audio", false)]
    [InlineData("", false)]
    public void UnixSystemInfo_RecognisesAdministratorGroups(string groups, bool expected)
    {
        Assert.Equal(expected, UnixSystemInfoService.IsAdministratorGroupList(groups));
    }

    [Theory]
    [InlineData("2874, 144", 2874, 144)]
    [InlineData("-1920, 30.6", -1920, 31)]
    [InlineData(" 10,20 \n", 10, 20)]
    public void ParseHyprlandCursorPosition_ReadsLayoutCoordinates(string output, int x, int y)
    {
        Assert.Equal(new PlatformPoint(x, y), LinuxWindowService.ParseHyprlandCursorPosition(output));
    }

    [Theory]
    [InlineData("")]
    [InlineData("error: no monitors")]
    [InlineData("1, 2, 3")]
    public void ParseHyprlandCursorPosition_RejectsAnythingElse(string output)
    {
        Assert.Null(LinuxWindowService.ParseHyprlandCursorPosition(output));
    }

    private sealed class FakeRunner(params string[] commands) : Diagnostics.ICommandRunner
    {
        public bool Exists(string command) => Array.IndexOf(commands, command) >= 0;

        public System.Threading.Tasks.Task<Diagnostics.CommandResult> RunAsync(string command, IReadOnlyList<string> arguments, byte[]? standardInput = null,
            TimeSpan? timeout = null, System.Threading.CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public System.Threading.Tasks.Task<int> RunForkingAsync(string command, IReadOnlyList<string> arguments, byte[]? standardInput = null,
            TimeSpan? timeout = null, System.Threading.CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }

    private static readonly PlatformInfo Wayland = new PlatformInfo(OperatingSystemKind.Linux, DisplayServer.Wayland, DesktopEnvironment.Gnome, "GNOME", false);
    private static readonly PlatformInfo Sway = new PlatformInfo(OperatingSystemKind.Linux, DisplayServer.Wayland, DesktopEnvironment.Sway, "sway", false);
    private static readonly PlatformInfo X11Session = new PlatformInfo(OperatingSystemKind.Linux, DisplayServer.X11, DesktopEnvironment.Xfce, "XFCE", false);

    [UnixFact]
    public void ParseUriList_ReadsFilesAndSkipsComments()
    {
        IReadOnlyList<string> paths = LinuxClipboardService.ParseUriList("copy\n# comment\nfile:///home/user/My%20Pictures/a.png\r\nhttps://example.com/b.png\nfile:///tmp/c.txt\n");

        Assert.Equal(new[] { "/home/user/My Pictures/a.png", "/tmp/c.txt" }, paths);
    }

    [Fact]
    public void ClipboardBackend_PrefersWlClipboardOnWayland()
    {
        Assert.Equal(LinuxClipboardService.Backend.WlClipboard, LinuxClipboardService.SelectBackend(Wayland, new FakeRunner("wl-copy", "wl-paste", "xclip")));
        Assert.Equal(LinuxClipboardService.Backend.Xclip, LinuxClipboardService.SelectBackend(Wayland, new FakeRunner("xclip")));
        Assert.Equal(LinuxClipboardService.Backend.Xclip, LinuxClipboardService.SelectBackend(X11Session, new FakeRunner("wl-copy", "wl-paste", "xclip")));
        Assert.Equal(LinuxClipboardService.Backend.Xsel, LinuxClipboardService.SelectBackend(X11Session, new FakeRunner("xsel")));
        Assert.Equal(LinuxClipboardService.Backend.None, LinuxClipboardService.SelectBackend(X11Session, new FakeRunner()));
    }

    [Fact]
    public void CaptureBackend_DependsOnSession()
    {
        Assert.Equal(LinuxScreenCaptureService.Backend.X11, LinuxScreenCaptureService.SelectBackend(X11Session, false, false));
        Assert.Equal(LinuxScreenCaptureService.Backend.Grim, LinuxScreenCaptureService.SelectBackend(Sway, true, true));
        Assert.Equal(LinuxScreenCaptureService.Backend.Portal, LinuxScreenCaptureService.SelectBackend(Sway, false, true));
        Assert.Equal(LinuxScreenCaptureService.Backend.Portal, LinuxScreenCaptureService.SelectBackend(Wayland, true, true));
        Assert.Equal(LinuxScreenCaptureService.Backend.None, LinuxScreenCaptureService.SelectBackend(Wayland, false, false));
    }

    [Fact]
    public void Geometry_RoundTrips()
    {
        PlatformRectangle area = new PlatformRectangle(-1920, 10, 800, 600);

        Assert.Equal("-1920,10 800x600", LinuxScreenCaptureService.FormatGeometry(area));
        Assert.Equal(area, LinuxScreenCaptureService.ParseGeometry("-1920,10 800x600\n"));
    }

    [Fact]
    public void ResolveArea_UnionsScreensForFullScreen()
    {
        ScreenInfo left = new ScreenInfo("DP-1", "Left", new PlatformRectangle(0, 0, 1920, 1080), new PlatformRectangle(0, 0, 1920, 1080), true, 1);
        ScreenInfo right = new ScreenInfo("DP-2", "Right", new PlatformRectangle(1920, 0, 2560, 1440), new PlatformRectangle(1920, 0, 2560, 1440), false, 1);
        ScreenInfo[] screens = [left, right];

        Assert.Equal(new PlatformRectangle(0, 0, 4480, 1440), LinuxScreenCaptureService.ResolveArea(ScreenCaptureRequest.FullScreen(), screens, PlatformRectangle.Empty));
        Assert.Equal(right.Bounds, LinuxScreenCaptureService.ResolveArea(ScreenCaptureRequest.ForScreen("DP-2"), screens, PlatformRectangle.Empty));
        Assert.Throws<ArgumentException>(() => LinuxScreenCaptureService.ResolveArea(ScreenCaptureRequest.ForScreen("HDMI-1"), screens, PlatformRectangle.Empty));
    }

    [Fact]
    public void ParseHyprlandMonitors_ConvertsToLogicalSize()
    {
        using JsonDocument json = JsonDocument.Parse("""
            [
              { "name": "eDP-1", "description": "BOE panel", "x": 0, "y": 0, "width": 2880, "height": 1800, "scale": 2.0, "focused": true },
              { "name": "HDMI-A-1", "x": 1440, "y": 0, "width": 1920, "height": 1080, "scale": 1.0, "focused": false, "disabled": false },
              { "name": "DP-3", "x": 0, "y": 0, "width": 1920, "height": 1080, "disabled": true }
            ]
            """);

        IReadOnlyList<ScreenInfo> screens = LinuxScreenCaptureService.ParseHyprlandMonitors(json.RootElement);

        Assert.Equal(2, screens.Count);
        Assert.Equal(new PlatformRectangle(0, 0, 1440, 900), screens[0].Bounds);
        Assert.True(screens[0].IsPrimary);
        Assert.Equal("BOE panel", screens[0].Name);
        Assert.Equal(new PlatformRectangle(1440, 0, 1920, 1080), screens[1].Bounds);
    }

    [Fact]
    public void ParseSwayOutputs_SkipsInactive()
    {
        using JsonDocument json = JsonDocument.Parse("""
            [
              { "name": "DP-1", "make": "Dell", "model": "U2720Q", "active": true, "focused": true, "scale": 1.5, "rect": { "x": 0, "y": 0, "width": 2560, "height": 1440 } },
              { "name": "HDMI-A-1", "active": false, "rect": { "x": 0, "y": 0, "width": 0, "height": 0 } }
            ]
            """);

        ScreenInfo screen = Assert.Single(LinuxScreenCaptureService.ParseSwayOutputs(json.RootElement));

        Assert.Equal("Dell U2720Q", screen.Name);
        Assert.Equal(1.5, screen.ScaleFactor);
        Assert.Equal(new PlatformRectangle(0, 0, 2560, 1440), screen.Bounds);
    }

    [Fact]
    public void ParseHyprlandClients_OrdersByFocusHistory()
    {
        using JsonDocument json = JsonDocument.Parse("""
            [
              { "address": "0x10", "mapped": true, "hidden": false, "at": [0, 0], "size": [800, 600], "class": "kitty", "title": "Terminal", "pid": -1, "focusHistoryID": 1 },
              { "address": "0x20", "mapped": true, "hidden": false, "at": [10, 20], "size": [640, 480], "class": "firefox", "title": "Browser", "pid": -1, "focusHistoryID": 0 },
              { "address": "0x30", "mapped": false, "at": [0, 0], "size": [1, 1], "class": "x", "title": "Unmapped", "pid": -1, "focusHistoryID": 2 }
            ]
            """);

        IReadOnlyList<PlatformWindow> windows = LinuxWindowService.ParseHyprlandClients(json.RootElement);

        Assert.Equal(new[] { "Browser", "Terminal" }, windows.Select(w => w.Title));
        Assert.Equal(0x20, windows[0].Handle);
        Assert.Equal("firefox", windows[0].ProcessName);
        Assert.Equal(new PlatformRectangle(10, 20, 640, 480), windows[0].Bounds);
    }

    [Fact]
    public void ParseHyprlandClients_KeepsOnlyVisibleWorkspaces()
    {
        using JsonDocument monitors = JsonDocument.Parse("""
            [
              { "name": "eDP-1", "activeWorkspace": { "id": 2, "name": "2" }, "specialWorkspace": { "id": 0, "name": "" } },
              { "name": "DP-1", "activeWorkspace": { "id": 5, "name": "5" }, "specialWorkspace": { "id": -98, "name": "special:magic" } }
            ]
            """);
        using JsonDocument clients = JsonDocument.Parse("""
            [
              { "address": "0x10", "mapped": true, "at": [0, 0], "size": [800, 600], "title": "Hidden workspace", "pid": -1, "workspace": { "id": 1 } },
              { "address": "0x20", "mapped": true, "at": [0, 0], "size": [800, 600], "title": "On screen", "pid": -1, "workspace": { "id": 2 } },
              { "address": "0x30", "mapped": true, "at": [0, 0], "size": [800, 600], "title": "Scratchpad", "pid": -1, "workspace": { "id": -98 } }
            ]
            """);

        IReadOnlyCollection<int> visible = LinuxWindowService.ParseHyprlandVisibleWorkspaces(monitors.RootElement);
        IReadOnlyList<PlatformWindow> windows = LinuxWindowService.ParseHyprlandClients(clients.RootElement, visible);

        Assert.Equal(new[] { 2, 5, -98 }.Order(), visible.Order());
        Assert.Equal(new[] { "On screen", "Scratchpad" }, windows.Select(w => w.Title).Order());
    }

    [Fact]
    public void FormatHyprlandAddress_IsHex()
    {
        Assert.Equal("address:0x55d0c2a6e3c0", LinuxWindowService.FormatHyprlandAddress(0x55d0c2a6e3c0));
    }

    [Fact]
    public void ParseSwayTree_FindsNestedAndFloatingViews()
    {
        using JsonDocument json = JsonDocument.Parse("""
            {
              "id": 1, "type": "root", "rect": { "x": 0, "y": 0, "width": 1920, "height": 1080 },
              "nodes": [
                { "id": 2, "type": "output", "nodes": [
                  { "id": 3, "type": "workspace", "nodes": [
                    { "id": 10, "pid": 999999, "app_id": "foot", "name": "foot", "visible": true, "rect": { "x": 0, "y": 0, "width": 960, "height": 1080 } }
                  ], "floating_nodes": [
                    { "id": 11, "pid": 999998, "app_id": null, "name": "Floating", "visible": false, "rect": { "x": 100, "y": 100, "width": 300, "height": 200 } }
                  ] }
                ] }
              ]
            }
            """);

        IReadOnlyList<PlatformWindow> windows = LinuxWindowService.ParseSwayTree(json.RootElement);

        Assert.Equal(2, windows.Count);
        Assert.Equal("foot", windows[0].ProcessName);
        Assert.False(windows[0].IsMinimized);
        Assert.True(windows[1].IsMinimized);
        Assert.Equal(11, windows[1].Handle);
    }

    [Fact]
    public void X11Grab_UsesRegionAndDisplay()
    {
        ScreenRecordingRequest request = new ScreenRecordingRequest { Region = new PlatformRectangle(100, 50, 801, 601), FrameRate = 60, DrawCursor = false };

        FFmpegVideoInput input = LinuxScreenRecordingService.CreateX11GrabInput(request, ":1", PlatformRectangle.Empty);

        Assert.Equal("x11grab", input.Device);
        Assert.Equal("-f x11grab -thread_queue_size 1024 -framerate 60 -draw_mouse 0 -video_size 800x600 -i :1+100,50", input.InputArguments);
    }

    [Fact]
    public void X11Grab_FallsBackToRootWindow()
    {
        FFmpegVideoInput input = LinuxScreenRecordingService.CreateX11GrabInput(new ScreenRecordingRequest(), ":0", new PlatformRectangle(0, 0, 3840, 2160));

        Assert.EndsWith("-video_size 3840x2160 -i :0+0,0", input.InputArguments);
    }

    [Theory]
    [InlineData(VirtualKeys.A, HotkeyModifiers.Control | HotkeyModifiers.Shift, "CTRL+SHIFT+a")]
    [InlineData(VirtualKeys.F1 + 11, HotkeyModifiers.Super, "LOGO+F12")]
    [InlineData(VirtualKeys.D0 + 5, HotkeyModifiers.Alt, "ALT+5")]
    [InlineData(VirtualKeys.NumPad0 + 3, HotkeyModifiers.None, "KP_3")]
    public void XKeyMap_FormatsPortalTriggers(int key, HotkeyModifiers modifiers, string expected)
    {
        Assert.Equal(expected, Linux.Native.XKeyMap.ToShortcutTrigger(new PlatformHotkey(key, modifiers)));
    }

    [Fact]
    public void XKeyMap_MapsPrintScreen()
    {
        Assert.True(Linux.Native.XKeyMap.TryGetKeysym(VirtualKeys.PrintScreen, out uint keysym, out string name));
        Assert.Equal(0xFF61u, keysym);
        Assert.Equal("Print", name);
        Assert.False(Linux.Native.XKeyMap.TryGetKeysym(0, out _, out _));
    }

    [Fact]
    public void EscapeBody_EscapesMarkup()
    {
        Assert.Equal("a &lt;b&gt; &amp; c", FreedesktopNotificationService.EscapeBody("a <b> & c"));
    }
}
