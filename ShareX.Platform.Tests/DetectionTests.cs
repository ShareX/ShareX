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

using System.Collections.Generic;
using Xunit;

namespace ShareX.Platform.Tests;

public class PlatformDetectorTests
{
    private static PlatformInfo DetectLinux(Dictionary<string, string> environment, params string[] files) =>
        PlatformDetector.Detect(OperatingSystemKind.Linux, name => environment.GetValueOrDefault(name), path => System.Array.IndexOf(files, path) >= 0);

    [Fact]
    public void Windows_UsesWin32()
    {
        PlatformInfo info = PlatformDetector.Detect(OperatingSystemKind.Windows, _ => null, _ => false);

        Assert.Equal(DisplayServer.Win32, info.DisplayServer);
        Assert.Null(info.Distribution);
    }

    [Fact]
    public void MacOS_DetectsAppSandbox()
    {
        PlatformInfo info = PlatformDetector.Detect(OperatingSystemKind.MacOS, name => name == "APP_SANDBOX_CONTAINER_ID" ? "x" : null, _ => false);

        Assert.Equal(DisplayServer.Quartz, info.DisplayServer);
        Assert.True(info.IsSandboxed);
    }

    [Theory]
    [InlineData("wayland", null, null, DisplayServer.Wayland)]
    [InlineData(null, "wayland-0", ":0", DisplayServer.Wayland)]
    [InlineData("x11", null, ":0", DisplayServer.X11)]
    [InlineData(null, null, ":1", DisplayServer.X11)]
    [InlineData("tty", null, null, DisplayServer.None)]
    public void Linux_DetectsDisplayServer(string? sessionType, string? waylandDisplay, string? display, DisplayServer expected)
    {
        Dictionary<string, string> environment = new Dictionary<string, string>();
        if (sessionType != null) environment["XDG_SESSION_TYPE"] = sessionType;
        if (waylandDisplay != null) environment["WAYLAND_DISPLAY"] = waylandDisplay;
        if (display != null) environment["DISPLAY"] = display;

        Assert.Equal(expected, DetectLinux(environment).DisplayServer);
    }

    [Theory]
    [InlineData("ubuntu:GNOME", DesktopEnvironment.Gnome, "GNOME")]
    [InlineData("KDE", DesktopEnvironment.Kde, "KDE")]
    [InlineData("X-Cinnamon", DesktopEnvironment.Cinnamon, "X-Cinnamon")]
    [InlineData("XFCE", DesktopEnvironment.Xfce, "XFCE")]
    [InlineData("pop:GNOME", DesktopEnvironment.Gnome, "pop")]
    [InlineData("Hyprland", DesktopEnvironment.Hyprland, "Hyprland")]
    [InlineData("niri", DesktopEnvironment.Other, "niri")]
    public void Linux_DetectsDesktopEnvironment(string currentDesktop, DesktopEnvironment expected, string expectedName)
    {
        PlatformInfo info = DetectLinux(new Dictionary<string, string> { ["XDG_CURRENT_DESKTOP"] = currentDesktop });

        Assert.Equal(expected, info.DesktopEnvironment);
        Assert.Equal(expectedName, info.DesktopEnvironmentName);
    }

    [Fact]
    public void Linux_CompositorSocketsWinOverCurrentDesktop()
    {
        PlatformInfo info = DetectLinux(new Dictionary<string, string>
        {
            ["XDG_SESSION_TYPE"] = "wayland",
            ["XDG_CURRENT_DESKTOP"] = "GNOME",
            ["SWAYSOCK"] = "/run/user/1000/sway-ipc.sock"
        });

        Assert.Equal(DesktopEnvironment.Sway, info.DesktopEnvironment);
        Assert.True(info.IsWlrootsCompositor);
    }

    [Fact]
    public void Linux_DetectsFlatpakAndSnap()
    {
        Assert.True(DetectLinux(new Dictionary<string, string>(), "/.flatpak-info").IsSandboxed);
        Assert.True(DetectLinux(new Dictionary<string, string> { ["SNAP"] = "/snap/sharex/1" }).IsSandboxed);
        Assert.False(DetectLinux(new Dictionary<string, string>()).IsSandboxed);
    }

    [Fact]
    public void Linux_UsesUnknownDistributionWhenNoneSupplied()
    {
        Assert.Same(LinuxDistribution.Unknown, DetectLinux(new Dictionary<string, string>()).Distribution);
    }
}

public class LinuxDistributionTests
{
    [Theory]
    [InlineData("ID=ubuntu\nID_LIKE=debian\nVERSION_ID=\"24.04\"", LinuxDistributionFamily.Debian, LinuxPackageManager.Apt)]
    [InlineData("ID=linuxmint\nID_LIKE=\"ubuntu debian\"", LinuxDistributionFamily.Debian, LinuxPackageManager.Apt)]
    [InlineData("ID=fedora\nVERSION_ID=42", LinuxDistributionFamily.Fedora, LinuxPackageManager.Dnf)]
    [InlineData("ID=fedora\nVARIANT_ID=silverblue", LinuxDistributionFamily.Fedora, LinuxPackageManager.RpmOstree)]
    [InlineData("ID=bazzite\nID_LIKE=\"fedora\"", LinuxDistributionFamily.Fedora, LinuxPackageManager.RpmOstree)]
    [InlineData("ID=arch", LinuxDistributionFamily.Arch, LinuxPackageManager.Pacman)]
    [InlineData("ID=omarchy\nID_LIKE=arch", LinuxDistributionFamily.Arch, LinuxPackageManager.Pacman)]
    [InlineData("ID=endeavouros\nID_LIKE=arch", LinuxDistributionFamily.Arch, LinuxPackageManager.Pacman)]
    [InlineData("ID=manjaro\nID_LIKE=arch", LinuxDistributionFamily.Arch, LinuxPackageManager.Pacman)]
    [InlineData("ID=cachyos", LinuxDistributionFamily.Arch, LinuxPackageManager.Pacman)]
    [InlineData("ID=\"opensuse-tumbleweed\"\nID_LIKE=\"opensuse suse\"", LinuxDistributionFamily.OpenSuse, LinuxPackageManager.Zypper)]
    [InlineData("ID=nixos", LinuxDistributionFamily.NixOS, LinuxPackageManager.Nix)]
    [InlineData("ID=alpine", LinuxDistributionFamily.Alpine, LinuxPackageManager.Apk)]
    [InlineData("ID=void", LinuxDistributionFamily.Void, LinuxPackageManager.Xbps)]
    [InlineData("ID=someos\nID_LIKE=\"ubuntu\"", LinuxDistributionFamily.Debian, LinuxPackageManager.Apt)]
    [InlineData("ID=someos", LinuxDistributionFamily.Unknown, LinuxPackageManager.Unknown)]
    public void Parse_MapsFamilyAndPackageManager(string osRelease, LinuxDistributionFamily family, LinuxPackageManager packageManager)
    {
        LinuxDistribution distribution = LinuxDistribution.Parse(osRelease);

        Assert.Equal(family, distribution.Family);
        Assert.Equal(packageManager, distribution.PackageManager);
    }

    [Fact]
    public void Parse_ReadsQuotedAndEscapedValues()
    {
        const string osRelease = "# comment\nNAME=\"Pop!_OS\"\nPRETTY_NAME='Pop!_OS 24.04 LTS'\nID=pop\nID_LIKE=\"ubuntu debian\"\nVERSION_ID=\"24.04\"\nHOME_URL=\"https://pop.system76.com/\"\nX=\"a \\\"quoted\\\" \\$value\"\n";

        LinuxDistribution distribution = LinuxDistribution.Parse(osRelease);

        Assert.Equal("pop", distribution.Id);
        Assert.Equal(new[] { "ubuntu", "debian" }, distribution.IdLike);
        Assert.Equal("Pop!_OS", distribution.Name);
        Assert.Equal("Pop!_OS 24.04 LTS", distribution.PrettyName);
        Assert.Equal("24.04", distribution.VersionId);
        Assert.True(distribution.IsOrIsLike("debian"));
        Assert.Equal("a \"quoted\" $value", LinuxDistribution.ParseOsRelease(osRelease)["X"]);
    }

    [Fact]
    public void Parse_TreatsOstreeAndNixOSAsImmutable()
    {
        Assert.True(LinuxDistribution.Parse("ID=fedora", ostreeBooted: true).IsImmutable);
        Assert.True(LinuxDistribution.Parse("ID=nixos").IsImmutable);
        Assert.False(LinuxDistribution.Parse("ID=debian").IsImmutable);
    }

    [Fact]
    public void Parse_DefaultsWhenEmpty()
    {
        LinuxDistribution distribution = LinuxDistribution.Parse("");

        Assert.Equal("linux", distribution.Id);
        Assert.Equal("Linux", distribution.PrettyName);
    }
}

public class LinuxPackagesTests
{
    [Theory]
    [InlineData("ID=ubuntu", "sudo apt install libnotify-bin wl-clipboard")]
    [InlineData("ID=fedora", "sudo dnf install libnotify wl-clipboard")]
    [InlineData("ID=fedora\nVARIANT_ID=kinoite", "rpm-ostree install libnotify wl-clipboard")]
    [InlineData("ID=arch", "sudo pacman -S --needed libnotify wl-clipboard")]
    [InlineData("ID=opensuse-tumbleweed", "sudo zypper install libnotify-tools wl-clipboard")]
    [InlineData("ID=nixos", "nix-env -iA nixos.libnotify nixos.wl-clipboard")]
    public void GetInstallCommand_UsesDistributionPackageManager(string osRelease, string expected)
    {
        LinuxDistribution distribution = LinuxDistribution.Parse(osRelease);

        Assert.Equal(expected, LinuxPackages.GetInstallCommand(distribution, LinuxTool.Libnotify, LinuxTool.WlClipboard));
    }

    [Fact]
    public void GetInstallCommand_ReturnsNullForUnknownDistribution()
    {
        Assert.Null(LinuxPackages.GetInstallCommand(LinuxDistribution.Unknown, LinuxTool.FFmpeg));
    }

    [Theory]
    [InlineData(LinuxTool.SecretTool, LinuxDistributionFamily.Debian, "libsecret-tools")]
    [InlineData(LinuxTool.SecretTool, LinuxDistributionFamily.Arch, "libsecret")]
    [InlineData(LinuxTool.FFmpeg, LinuxDistributionFamily.Fedora, "ffmpeg-free")]
    [InlineData(LinuxTool.FFmpeg, LinuxDistributionFamily.Debian, "ffmpeg")]
    [InlineData(LinuxTool.XdgUtils, LinuxDistributionFamily.Alpine, "xdg-utils")]
    public void GetPackageName_MapsPerFamily(LinuxTool tool, LinuxDistributionFamily family, string expected)
    {
        Assert.Equal(expected, LinuxPackages.GetPackageName(tool, family));
    }

    [Fact]
    public void Missing_NamesAlternativesAndSuggestsFirst()
    {
        FeatureSupport support = LinuxPackages.Missing(LinuxDistribution.Parse("ID=debian"), LinuxTool.WlClipboard, LinuxTool.Xclip);

        Assert.False(support.IsSupported);
        Assert.Equal("Install wl-clipboard or xclip (sudo apt install wl-clipboard).", support.Reason);
    }

    [Fact]
    public void RecommendedTools_DependOnSession()
    {
        PlatformInfo hyprland = new PlatformInfo(OperatingSystemKind.Linux, DisplayServer.Wayland, DesktopEnvironment.Hyprland, "Hyprland", false);
        PlatformInfo x11 = new PlatformInfo(OperatingSystemKind.Linux, DisplayServer.X11, DesktopEnvironment.Xfce, "XFCE", false);

        Assert.Contains(LinuxTool.Grim, LinuxPackages.RecommendedTools(hyprland));
        Assert.Contains(LinuxTool.WlClipboard, LinuxPackages.RecommendedTools(hyprland));
        Assert.Contains(LinuxTool.Xclip, LinuxPackages.RecommendedTools(x11));
        Assert.DoesNotContain(LinuxTool.Grim, LinuxPackages.RecommendedTools(x11));
    }
}
