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

public class LinuxFileAssociationTests : IDisposable
{
    private readonly string root = Path.Combine(Path.GetTempPath(), "sharex-tests-" + Guid.NewGuid().ToString("N"));

    private static readonly FileAssociation CustomUploader = new FileAssociation(".sxcu", "ShareX.sxcu", "ShareX custom uploader & more",
        "application/x-sharex-custom-uploader", "/opt/Share X/ShareX", ["-CustomUploader"]);

    public void Dispose()
    {
        if (Directory.Exists(root)) Directory.Delete(root, true);
    }

    private LinuxShellIntegrationService Create() =>
        new LinuxShellIntegrationService(Path.Combine(root, "data"), Path.Combine(root, "config"), Path.Combine(root, "home"), new MissingToolsRunner());

    [Fact]
    public void MimePackage_DeclaresTheGlobAndEscapes()
    {
        string xml = LinuxShellIntegrationService.CreateMimePackage(CustomUploader);

        Assert.Contains("<mime-type type=\"application/x-sharex-custom-uploader\">", xml);
        Assert.Contains("<glob pattern=\"*.sxcu\"/>", xml);
        Assert.Contains("ShareX custom uploader &amp; more", xml);
        System.Xml.Linq.XDocument.Parse(xml);
    }

    [Fact]
    public void DesktopEntry_OpensOneFileAndStaysOutOfTheLauncher()
    {
        string entry = LinuxShellIntegrationService.CreateAssociationDesktopEntry(CustomUploader);

        Assert.Contains("Exec=\"/opt/Share X/ShareX\" -CustomUploader %f\n", entry);
        Assert.Contains("MimeType=application/x-sharex-custom-uploader;\n", entry);
        Assert.Contains("NoDisplay=true\n", entry);
    }

    [Fact]
    public void Associate_ThenRemove()
    {
        LinuxShellIntegrationService service = Create();

        service.Associate(CustomUploader);
        Assert.True(service.IsAssociated(CustomUploader));
        Assert.True(File.Exists(Path.Combine(root, "data", "mime", "packages", "sharex-sharex_sxcu.xml")));

        service.RemoveAssociation(CustomUploader);
        Assert.False(service.IsAssociated(CustomUploader));
    }

    [Fact]
    public void BrowserHosts_GoToInstalledBrowsersWithAnAbsolutePath()
    {
        string manifest = Path.Combine(root, "host-manifest-chrome.json");
        Directory.CreateDirectory(root);
        File.WriteAllText(manifest, """{ "name": "x", "path": "ShareX_NativeMessagingHost.exe", "type": "stdio", "allowed_origins": ["chrome-extension://abc/"] }""");
        Directory.CreateDirectory(Path.Combine(root, "config", "chromium"));
        Directory.CreateDirectory(Path.Combine(root, "config", "BraveSoftware", "Brave-Browser"));
        BrowserHost host = new BrowserHost(BrowserFamily.Chromium, "com.getsharex.sharex", manifest, Path.Combine(root, "ShareX_NativeMessagingHost"));
        LinuxShellIntegrationService service = Create();

        Assert.Equal(2, service.GetBrowserHostManifestPaths(host).Count);
        Assert.False(service.IsBrowserHostRegistered(host));

        service.RegisterBrowserHost(host);
        string written = File.ReadAllText(Path.Combine(root, "config", "chromium", "NativeMessagingHosts", "com.getsharex.sharex.json"));

        Assert.True(service.IsBrowserHostRegistered(host));
        using JsonDocument document = JsonDocument.Parse(written);
        string hostPath = document.RootElement.GetProperty("path").GetString()!;
        Assert.True(Path.IsPathFullyQualified(hostPath));
        Assert.Equal(host.HostExecutablePath, hostPath);
        Assert.Contains("\"name\": \"com.getsharex.sharex\"", written);
        Assert.Contains("chrome-extension://abc/", written);

        service.UnregisterBrowserHost(host);
        Assert.False(service.IsBrowserHostRegistered(host));
    }

    [Fact]
    public void FirefoxHost_DefaultsToMozillaFolder()
    {
        BrowserHost host = new BrowserHost(BrowserFamily.Firefox, "ShareX", "unused.json", "/opt/sharex/host");

        Assert.Equal(Path.Combine(root, "home", ".mozilla", "native-messaging-hosts", "ShareX.json"), Assert.Single(Create().GetBrowserHostManifestPaths(host)));
    }

    private sealed class MissingToolsRunner : ShareX.Platform.Diagnostics.ICommandRunner
    {
        public bool Exists(string command) => false;

        public System.Threading.Tasks.Task<ShareX.Platform.Diagnostics.CommandResult> RunAsync(string command, IReadOnlyList<string> arguments, byte[]? standardInput = null,
            TimeSpan? timeout = null, System.Threading.CancellationToken cancellationToken = default) => throw new InvalidOperationException();

        public System.Threading.Tasks.Task<int> RunForkingAsync(string command, IReadOnlyList<string> arguments, byte[]? standardInput = null,
            TimeSpan? timeout = null, System.Threading.CancellationToken cancellationToken = default) => throw new InvalidOperationException();
    }
}

public class PolicyTests : IDisposable
{
    private readonly string root = Path.Combine(Path.GetTempPath(), "sharex-policy-" + Guid.NewGuid().ToString("N"));

    public void Dispose()
    {
        if (Directory.Exists(root)) Directory.Delete(root, true);
    }

    [Fact]
    public void SystemFileWinsOverUserFile()
    {
        Directory.CreateDirectory(root);
        string system = Path.Combine(root, "system.json"), user = Path.Combine(root, "user.json");
        File.WriteAllText(system, """{ "DisableUpload": true }""");
        File.WriteAllText(user, """{ "DisableUpload": false, "PersonalPath": "/data/sharex", "DisableLogging": 1 }""");
        DefaultSystemPreferencesService preferences = new DefaultSystemPreferencesService(system, user, Path.Combine(root, "missing.json"));

        Assert.Equal(true, preferences.GetPolicy("DisableUpload"));
        Assert.Equal("/data/sharex", preferences.GetPolicy("PersonalPath"));
        Assert.True(Convert.ToBoolean(preferences.GetPolicy("DisableLogging")));
        Assert.Null(preferences.GetPolicy("DisableUpdateCheck"));
    }

    [Fact]
    public void BrokenFileIsIgnored()
    {
        Directory.CreateDirectory(root);
        string broken = Path.Combine(root, "broken.json");
        File.WriteAllText(broken, "{ not json");

        Assert.Null(new DefaultSystemPreferencesService(broken).GetPolicy("DisableUpload"));
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
    public void ParseHyprlandDetails_FloatingPinnedWindowIsTopMost()
    {
        using JsonDocument json = JsonDocument.Parse("""
            { "address": "0x20", "mapped": true, "at": [10, 20], "size": [640, 480], "class": "firefox", "title": "Browser", "pid": -1,
              "floating": true, "pinned": true, "fullscreen": 0, "xwayland": false, "workspace": { "id": 2, "name": "2" } }
            """);

        WindowDetails details = LinuxWindowManagementService.ParseHyprlandDetails(json.RootElement);

        Assert.Equal("firefox", details.ClassName);
        Assert.True(details.IsTopMost);
        Assert.Null(details.Opacity);
        Assert.Equal(new[] { "floating", "pinned", "workspace 2" }, details.Styles);
    }

    [Fact]
    public void ParseHyprlandDetails_TiledWindowCannotBePinned()
    {
        using JsonDocument json = JsonDocument.Parse("""
            { "address": "0x20", "at": [0, 0], "size": [10, 10], "title": "T", "pid": -1, "floating": false, "pinned": false }
            """);

        Assert.Null(LinuxWindowManagementService.ParseHyprlandDetails(json.RootElement).IsTopMost);
    }

    [Fact]
    public void ParseNetWmIcon_PicksSmallestAtLeastPreferred()
    {
        nuint[] data =
        [
            1, 1, 0xFF000000,
            2, 1, 0x80FF0000, 0xFF00FF00,
            4, 1, 0, 0, 0, 0
        ];

        PixelBuffer? icon = LinuxWindowManagementService.ParseNetWmIcon(data, 2);

        Assert.NotNull(icon);
        Assert.Equal(2, icon.Width);
        // 0x80FF0000 is half transparent red: B, G, R, A.
        Assert.Equal(new byte[] { 0, 0, 255, 128, 0, 255, 0, 255 }, icon.Pixels);
    }

    [Fact]
    public void ParseNetWmIcon_RejectsTruncatedData()
    {
        Assert.Null(LinuxWindowManagementService.ParseNetWmIcon([16, 16, 0], 32));
    }

    [Fact]
    public void Tesseract_ParsesLanguageList()
    {
        IReadOnlyList<OcrLanguage> languages = TesseractOcrService.ParseLanguages("List of available languages in \"/usr/share/tessdata/\" (3):\neng\nosd\ndeu\n");

        Assert.Equal(new[] { "deu", "eng" }.Order(), languages.Select(l => l.Tag).Order());
    }

    [Theory]
    [InlineData("en", "eng")]
    [InlineData("en-US", "eng")]
    [InlineData("de-DE", "deu")]
    [InlineData("zh-Hans", "chi_sim")]
    [InlineData("zh-TW", "chi_tra")]
    [InlineData("eng", "eng")]
    public void Tesseract_MatchesWindowsLanguageTags(string tag, string expected)
    {
        OcrLanguage[] installed = [new("eng", "English"), new("deu", "German"), new("chi_sim", "Chinese"), new("chi_tra", "Chinese (Traditional)")];

        Assert.Equal(expected, TesseractOcrService.MatchLanguage(tag, installed));
    }

    [Fact]
    public void Tesseract_FormatsSingleLine()
    {
        Assert.Equal("first second", TesseractOcrService.FormatText("first\n\nsecond\n\f", singleLine: true));
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

public class UnityLauncherTaskbarTests
{
    [Fact]
    public void PropertiesCarryProgressAndVisibility()
    {
        var properties = UnityLauncherTaskbarService.CreateProperties(0.5, true);

        Assert.Equal(0.5, properties["progress"].GetDouble());
        Assert.True(properties["progress-visible"].GetBool());
    }

    [Fact]
    public void ZeroMaximumIsIgnored()
    {
        // Must not divide by zero or throw, with or without a session bus.
        new UnityLauncherTaskbarService().SetProgressValue(5, 0);
    }
}

public class WindowDetailsTests
{
    [Fact]
    public void ClientBoundsAreInClientCoordinates()
    {
        Assert.Equal(new PlatformRectangle(0, 0, 800, 600), LinuxWindowManagementService.ToClientCoordinates(new PlatformRectangle(-1920, 40, 800, 600)));
        Assert.Null(LinuxWindowManagementService.ToClientCoordinates(null));
    }
}

public class XcursorGraphicsTests
{
    [Fact]
    public void EveryCursorHasNames()
    {
        foreach (SystemCursor cursor in Enum.GetValues<SystemCursor>())
        {
            Assert.NotEmpty(XcursorGraphicsService.GetCursorNames(cursor));
        }
    }

    [Fact]
    public void PremultipliedArgbBecomesStraightBgra()
    {
        // Opaque red, half transparent white (premultiplied 0x80), fully transparent.
        PixelBuffer buffer = XcursorGraphicsService.FromPremultipliedArgb([0xFFFF0000, 0x80808080, 0x00000000], 3, 1);

        Assert.Equal(new byte[] { 0, 0, 255, 255, 255, 255, 255, 128, 0, 0, 0, 0 }, buffer.Pixels);
    }

    [Fact]
    public void LoadsArrowFromInstalledTheme()
    {
        XcursorGraphicsService service = new XcursorGraphicsService(null, "32");

        if (!OperatingSystem.IsLinux() || !service.CursorSupport.IsSupported)
        {
            return;
        }

        SystemCursorImage? arrow = service.GetSystemCursor(SystemCursor.Arrow);

        // A machine without any cursor theme has no arrow; when there is one, it has pixels and a hotspot inside it.
        if (arrow != null)
        {
            Assert.InRange(arrow.Hotspot.X, 0, arrow.Image.Width - 1);
            Assert.InRange(arrow.Hotspot.Y, 0, arrow.Image.Height - 1);
            Assert.Contains(arrow.Image.Pixels.Where((_, i) => i % 4 == 3), alpha => alpha != 0);

            string? dump = Environment.GetEnvironmentVariable("SHAREX_CURSOR_DUMP");
            if (!string.IsNullOrEmpty(dump))
            {
                foreach (SystemCursor cursor in Enum.GetValues<SystemCursor>())
                {
                    SystemCursorImage? image = service.GetSystemCursor(cursor);
                    if (image != null) File.WriteAllBytes(Path.Combine(dump, cursor + ".png"), PngCodec.Encode(image.Image));
                }
            }
        }
    }
}

public class GrimRegionTests
{
    private static readonly ScreenInfo[] Screens =
    [
        new ScreenInfo("DP-1", "DP-1", new PlatformRectangle(0, 0, 3072, 1728), new PlatformRectangle(0, 0, 3072, 1728), true, 1.25),
        new ScreenInfo("HDMI-A-1", "HDMI-A-1", new PlatformRectangle(3072, 0, 1920, 1080), new PlatformRectangle(3072, 0, 1920, 1080), false, 1)
    ];

    [Fact]
    public void PartlyVisibleWindowIsClipped() =>
        Assert.Equal(new PlatformRectangle(0, 100, 300, 200), LinuxScreenCaptureService.ClipToScreens(new PlatformRectangle(-100, 100, 400, 200), Screens));

    [Fact]
    public void WindowOffEveryScreenIsNothingToCapture() =>
        Assert.Throws<ArgumentException>(() => LinuxScreenCaptureService.ClipToScreens(new PlatformRectangle(-25600, -25600, 640, 640), Screens));

    [Fact]
    public void UnknownLayoutPassesThrough() =>
        Assert.Equal(new PlatformRectangle(-5, -5, 10, 10), LinuxScreenCaptureService.ClipToScreens(new PlatformRectangle(-5, -5, 10, 10), []));
}

public class WaylandRecordingTests
{
    [Fact]
    public void WfRecorderFeedsFFmpegThroughThePipe()
    {
        ScreenRecordingRequest request = new ScreenRecordingRequest { Region = new PlatformRectangle(10, 20, 301, 201), FrameRate = 30 };

        FFmpegVideoInput input = LinuxScreenRecordingService.CreateWfRecorderInput(request, "/run/user/1000/rec.mkv", new PlatformRectangle(0, 0, 3072, 1728));

        Assert.Equal("wf-recorder", input.Device);
        Assert.Equal("-thread_queue_size 1024 -f matroska -i \"/run/user/1000/rec.mkv\"", input.InputArguments);
        Assert.Equal(["crop=trunc(iw/2)*2:trunc(ih/2)*2"], input.VideoFilters);
        Assert.IsType<WfRecorderSource>(input.Source);
    }

    [Fact]
    public void WfRecorderRecordsTheEvenRegionLosslessly()
    {
        IReadOnlyList<string> arguments = WfRecorderSource.CreateArguments("/tmp/p.mkv", new PlatformRectangle(10, 20, 300, 200), 30);

        Assert.Equal(["-g", "10,20 300x200", "-r", "30", "-c", "libx264", "-p", "preset=ultrafast", "-p", "qp=0", "-x", "yuv444p",
            "-m", "matroska", "-y", "-f", "/tmp/p.mkv"], arguments);
    }

    [Fact]
    public void WholeDesktopWhenNoRegion()
    {
        FFmpegVideoInput input = LinuxScreenRecordingService.CreateWfRecorderInput(new ScreenRecordingRequest(), "/tmp/p.mkv", new PlatformRectangle(0, 0, 3072, 1728));

        Assert.Contains("0,0 3072x1728", ((WfRecorderSource)input.Source!).ToString() + string.Join(" ", typeof(WfRecorderSource)
            .GetField("arguments", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(input.Source) as IReadOnlyList<string> ?? []));
    }
}

public class HyprlandOptionTests
{
    [Theory]
    [InlineData("{\"option\": \"xwayland:force_zero_scaling\", \"bool\": true, \"set\": true}", true)]
    [InlineData("{\"option\": \"xwayland:force_zero_scaling\", \"int\": 0, \"set\": false}", false)]
    [InlineData("{\"option\": \"xwayland:force_zero_scaling\", \"int\": 1}", true)]
    [InlineData("{\"option\": \"x\"}", null)]
    public void ReadsBooleanOption(string json, bool? expected)
    {
        using JsonDocument document = JsonDocument.Parse(json);
        Assert.Equal(expected, LinuxWindowService.ParseHyprlandBoolOption(document.RootElement));
    }
}

public class LinuxDesktopWallpaperTests
{
    [UnixFact]
    public void LookupOnThisDesktopReturnsAnExistingFileOrNothing()
    {
        LinuxDesktopWallpaperService service = new LinuxDesktopWallpaperService(ShareX.Platform.Diagnostics.CommandRunner.Default);

        DesktopWallpaper? wallpaper = service.Support.IsSupported ? service.GetWallpaper() : null;

        if (wallpaper != null)
        {
            Assert.True(File.Exists(wallpaper.Path), wallpaper.Path);
        }
    }
}

public class FeatureReasonTests
{
    private static readonly PlatformInfo Hyprland = new PlatformInfo(OperatingSystemKind.Linux, DisplayServer.Wayland, DesktopEnvironment.Hyprland, "Hyprland", false);

    [Fact]
    public void WindowsOnlyCaptureOptionsExplainWhy()
    {
        LinuxScreenCaptureService capture = new LinuxScreenCaptureService(Hyprland, new RecordingRunner("grim", "slurp"));

        Assert.True(capture.Support.IsSupported);
        Assert.Contains("only", capture.GetFeatureSupport(ScreenCaptureFeatures.TransparentWindow).Reason);
        Assert.Contains("HDR", capture.GetFeatureSupport(ScreenCaptureFeatures.HdrToneMapping).Reason);
        Assert.Contains("single window", capture.GetFeatureSupport(ScreenCaptureFeatures.Window).Reason);
    }

    [Fact]
    public void HyprlandWindowOperationsExplainWhatWaylandAllows()
    {
        LinuxWindowManagementService management = new LinuxWindowManagementService(new LinuxWindowService(Hyprland, new RecordingRunner("hyprctl")), new RecordingRunner("hyprctl"));

        Assert.True(management.GetSupport(WindowManagementFeature.Inspect).IsSupported);
        Assert.True(management.GetSupport(WindowManagementFeature.TopMost).IsSupported);
        Assert.Contains("opacity", management.GetSupport(WindowManagementFeature.Opacity).Reason);
        Assert.Contains("controls", management.GetSupport(WindowManagementFeature.ChildControls).Reason);
    }
}

public class HyprlandShortcutKeyBinderTests
{
    [Fact]
    public void HotkeysMapToHyprlandModifiersAndKeys()
    {
        Assert.True(ShareX.Platform.Linux.Desktop.HyprlandShortcutKeyBinder.TryGetKey(new PlatformHotkey(0x2C, HotkeyModifiers.Control | HotkeyModifiers.Shift), out int mask, out string key, out string combination));
        Assert.Equal(5, mask);
        Assert.Equal("Print", key);
        Assert.Equal("CTRL + SHIFT + Print", combination);
        Assert.Equal("CTRL SHIFT", ShareX.Platform.Linux.Desktop.HyprlandShortcutKeyBinder.ClassicModifiers(mask));
    }

    [Fact]
    public void ParsesMainSubmapKeyBinds()
    {
        string json = "[{\"modmask\":0,\"key\":\"PRINT\",\"submap\":\"\",\"mouse\":false},{\"modmask\":64,\"key\":\"mouse:272\",\"submap\":\"\",\"mouse\":true},{\"modmask\":4,\"key\":\"RETURN\",\"submap\":\"capture\",\"mouse\":false}]";

        Assert.Equal([(0, "PRINT")], ShareX.Platform.Linux.Desktop.HyprlandShortcutKeyBinder.ParseBinds(json));
    }

    [Fact]
    public void KeysTheConfigurationUsesAreInUse()
    {
        BindsRunner runner = new BindsRunner("[{\"modmask\":0,\"key\":\"PRINT\",\"submap\":\"\"}]");
        using ShareX.Platform.Linux.Desktop.HyprlandShortcutKeyBinder binder = new ShareX.Platform.Linux.Desktop.HyprlandShortcutKeyBinder(runner, "sharex");

        Assert.True(binder.IsInUse(new PlatformHotkey(0x2C, HotkeyModifiers.None)));
        Assert.False(binder.IsInUse(new PlatformHotkey(0x2C, HotkeyModifiers.Control)));
    }

    [Fact]
    public void LuaStringsAreEscaped() =>
        Assert.Equal("\"ShareX: \\\"x\\\" \\\\ y\"", ShareX.Platform.Linux.Desktop.HyprlandShortcutKeyBinder.Lua("ShareX: \"x\" \\ y"));

    private sealed class BindsRunner(string binds) : Diagnostics.ICommandRunner
    {
        public bool Exists(string command) => true;

        public System.Threading.Tasks.Task<Diagnostics.CommandResult> RunAsync(string command, IReadOnlyList<string> arguments, byte[]? standardInput = null,
            TimeSpan? timeout = null, System.Threading.CancellationToken cancellationToken = default) =>
            System.Threading.Tasks.Task.FromResult(new Diagnostics.CommandResult(0, System.Text.Encoding.UTF8.GetBytes(arguments.Count > 0 && arguments[0] == "binds" ? binds : "ok"), ""));

        public System.Threading.Tasks.Task<int> RunForkingAsync(string command, IReadOnlyList<string> arguments, byte[]? standardInput = null,
            TimeSpan? timeout = null, System.Threading.CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}

public class SwayHotkeyTests
{
    [Fact]
    public void HotkeysBecomeSwayCombinations() =>
        Assert.Equal("Ctrl+Shift+Print", ShareX.Platform.Linux.Desktop.SwayHotkeyService.ToCombination(new PlatformHotkey(0x2C, HotkeyModifiers.Control | HotkeyModifiers.Shift)));

    [Fact]
    public void ConfiguredBindsResolveVariablesAndAliases()
    {
        string config = System.Text.Json.JsonSerializer.Serialize(new { config = "set $mod Mod4\nbindsym $mod+Return exec foot\nbindsym --release Print exec grim\nbindsym Control+Alt+Delete exit\n" });

        HashSet<string> combinations = ShareX.Platform.Linux.Desktop.SwayHotkeyService.GetConfiguredCombinations(config);

        Assert.Contains("mod4+return", combinations);
        Assert.Contains("print", combinations);
        Assert.Contains(ShareX.Platform.Linux.Desktop.SwayHotkeyService.Normalize("Ctrl+Mod1+Delete"), combinations);
        Assert.DoesNotContain(ShareX.Platform.Linux.Desktop.SwayHotkeyService.Normalize("Ctrl+Print"), combinations);
    }

    [Theory]
    [InlineData("{\"change\":\"run\",\"binding\":{\"command\":\"nop sharex-3\"}}", 3)]
    [InlineData("{\"change\":\"run\",\"binding\":{\"command\":\"exec foot\"}}", null)]
    [InlineData("not json", null)]
    public void BindingEventsNameTheHotkey(string line, int? id) =>
        Assert.Equal(id, ShareX.Platform.Linux.Desktop.SwayHotkeyService.ParseBindingEvent(line));

    [Theory]
    [InlineData("[{\"success\": true}]", true)]
    [InlineData("[{\"success\": false, \"error\": \"x\"}]", false)]
    [InlineData(null, false)]
    public void CommandResults(string? output, bool success) =>
        Assert.Equal(success, ShareX.Platform.Linux.Desktop.SwayHotkeyService.IsSuccess(output));
}

public class LinuxDesktopKindTests
{
    [Theory]
    [InlineData(DisplayServer.X11, DesktopEnvironment.Gnome, ShareX.Platform.Linux.Desktop.LinuxDesktopKind.X11)]
    [InlineData(DisplayServer.Wayland, DesktopEnvironment.Hyprland, ShareX.Platform.Linux.Desktop.LinuxDesktopKind.Hyprland)]
    [InlineData(DisplayServer.Wayland, DesktopEnvironment.Sway, ShareX.Platform.Linux.Desktop.LinuxDesktopKind.Sway)]
    [InlineData(DisplayServer.Wayland, DesktopEnvironment.Kde, ShareX.Platform.Linux.Desktop.LinuxDesktopKind.Kde)]
    [InlineData(DisplayServer.Wayland, DesktopEnvironment.Xfce, ShareX.Platform.Linux.Desktop.LinuxDesktopKind.OtherWayland)]
    public void DesktopKindFollowsSession(DisplayServer server, DesktopEnvironment desktop, ShareX.Platform.Linux.Desktop.LinuxDesktopKind kind) =>
        Assert.Equal(kind, ShareX.Platform.Linux.Desktop.LinuxDesktop.GetKind(new PlatformInfo(OperatingSystemKind.Linux, server, desktop, desktop.ToString(), false)));
}

public class PortalRecordingTests
{
    private static readonly ShareX.Platform.Linux.DBus.ScreenCastStream Monitor =
        new(57, new PlatformPoint(0, 0), new PlatformSize(3072, 1728));

    [Fact]
    public void FullScreenUsesTheWholeEvenFrame() =>
        Assert.Equal(new PlatformRectangle(0, 0, 3840, 2160),
            ShareX.Platform.Linux.Desktop.PipeWireRecordingSource.GetCrop(new ScreenRecordingRequest(), Monitor, new PlatformSize(3840, 2161)));

    [Fact]
    public void RegionIsScaledToStreamPixels()
    {
        // 125%: a 960x560 region at (320, 320) in compositor coordinates is 1200x700 stream pixels at (400, 400).
        ScreenRecordingRequest request = new ScreenRecordingRequest { Region = new PlatformRectangle(320, 320, 960, 560) };

        Assert.Equal(new PlatformRectangle(400, 400, 1200, 700),
            ShareX.Platform.Linux.Desktop.PipeWireRecordingSource.GetCrop(request, Monitor, new PlatformSize(3840, 2160)));
    }

    [Fact]
    public void CropsAllThreePlanes()
    {
        // 4x4 I420: Y values 0..15, U 100..103, V 200..203.
        byte[] input = new byte[24];
        for (int i = 0; i < 16; i++) input[i] = (byte)i;
        for (int i = 0; i < 4; i++) { input[16 + i] = (byte)(100 + i); input[20 + i] = (byte)(200 + i); }
        byte[] output = new byte[6];

        ShareX.Platform.Linux.Desktop.PipeWireRecordingSource.CropI420(input, new PlatformSize(4, 4), new PlatformRectangle(2, 2, 2, 2), output);

        Assert.Equal(new byte[] { 10, 11, 14, 15, 103, 203 }, output);
    }

    [Fact]
    public void ReadsNegotiatedFrameSize() =>
        Assert.Equal(new PlatformSize(3840, 2160), ShareX.Platform.Linux.Desktop.PipeWireRecordingSource.ParseFrameSize(
            "/GstPipeline:pipeline0/GstPipeWireSrc:pipewiresrc0.GstPad:src: caps = video/x-raw, format=(string)BGRx, width=(int)3840, height=(int)2160, framerate=(fraction)0/1"));

    [Fact]
    public void PipelineWritesRawFramesAtTheFrameRate()
    {
        IReadOnlyList<string> arguments = ShareX.Platform.Linux.Desktop.PipeWireRecordingSource.CreatePipelineArguments(57, new PlatformSize(1920, 1080), 30);

        Assert.Contains("path=57", arguments);
        Assert.Contains("video/x-raw,format=I420,width=1920,height=1080,framerate=30/1", arguments);
        Assert.Equal("fd=1", arguments[^1]);
    }
}

public class PortalCaptureCropTests
{
    [Theory]
    // 125%: a 3840x2160 screenshot of a 3072x1728 layout.
    [InlineData(0, 0, 3072, 1728, 100, 200, 400, 300, 3840, 2160, 125, 250, 500, 375)]
    // Unscaled two-monitor layout starting left of zero.
    [InlineData(-1920, 0, 3840, 1080, -1920, 0, 1920, 1080, 3840, 1080, 0, 0, 1920, 1080)]
    // Partly outside the image is clipped.
    [InlineData(0, 0, 1000, 1000, 900, 900, 200, 200, 2000, 2000, 1800, 1800, 200, 200)]
    public void MapToPixels_ScalesFromLayoutToScreenshot(int lx, int ly, int lw, int lh, int ax, int ay, int aw, int ah, int pw, int ph,
        int ex, int ey, int ew, int eh)
    {
        PlatformRectangle mapped = ShareX.Platform.Linux.Desktop.PortalCaptureBackend.MapToPixels(new PlatformRectangle(ax, ay, aw, ah),
            new PlatformRectangle(lx, ly, lw, lh), new PlatformSize(pw, ph));

        Assert.Equal(new PlatformRectangle(ex, ey, ew, eh), mapped);
    }
}

public class RecordingDeviceActionTests
{
    [Theory]
    [InlineData(RecordingDeviceAction.ListDirectShowDevices)]
    [InlineData(RecordingDeviceAction.InstallRecorderDevices)]
    public void Linux_ReportsDirectShowActionsUnsupportedEvenWhenRecordingWorks(RecordingDeviceAction action)
    {
        PlatformInfo x11 = new PlatformInfo(OperatingSystemKind.Linux, DisplayServer.X11, DesktopEnvironment.Xfce, "XFCE", false);
        LinuxScreenRecordingService service = new LinuxScreenRecordingService(x11, new RecordingRunner("ffmpeg"), () => []);

        Assert.True(service.Support.IsSupported);
        FeatureSupport support = service.GetDeviceActionSupport(action);
        Assert.False(support.IsSupported);
        Assert.False(string.IsNullOrWhiteSpace(support.Reason));
    }
}

public class X11MouseHookTests
{
    [Fact]
    public void GetButtonChanges_ReportsPressesAndReleasesPerButton()
    {
        const uint left = 1 << 8, middle = 1 << 9, right = 1 << 10;
        PlatformPoint at = new PlatformPoint(5, 6);

        GlobalMouseButtonEvent[] pressed = X11MouseHook.GetButtonChanges(0, left | right, at, 42).ToArray();
        GlobalMouseButtonEvent[] released = X11MouseHook.GetButtonChanges(left | right, right | middle, at, 43).ToArray();

        Assert.Equal([new GlobalMouseButtonEvent(GlobalMouseButton.Primary, true, at, 42), new GlobalMouseButtonEvent(GlobalMouseButton.Secondary, true, at, 42)], pressed);
        Assert.Equal([new GlobalMouseButtonEvent(GlobalMouseButton.Primary, false, at, 43), new GlobalMouseButtonEvent(GlobalMouseButton.Middle, true, at, 43)], released);
        Assert.Empty(X11MouseHook.GetButtonChanges(right, right | (1 << 0), at, 44));
    }

    [Fact]
    public void Wayland_ReportsHookAndOverlayUnsupported()
    {
        PlatformInfo hyprland = new PlatformInfo(OperatingSystemKind.Linux, DisplayServer.Wayland, DesktopEnvironment.Hyprland, "Hyprland", false);

        Assert.False(new LinuxInputService(hyprland, new RecordingRunner()).MouseHookSupport.IsSupported);
        Assert.False(new LinuxWindowService(hyprland, new RecordingRunner()).OverlaySupport.IsSupported);
    }
}
