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

using System;
using System.Collections.Generic;
using System.Linq;

namespace ShareX.Platform;

/// <summary>Helper programs ShareX can use on Linux.</summary>
public enum LinuxTool
{
    /// <summary>wl-copy and wl-paste.</summary>
    WlClipboard,
    Xclip,
    Xsel,
    /// <summary>notify-send.</summary>
    Libnotify,
    /// <summary>secret-tool.</summary>
    SecretTool,
    Grim,
    Slurp,
    FFmpeg,
    /// <summary>xdg-open.</summary>
    XdgUtils,
    /// <summary>xdg-desktop-portal plus a backend for the current desktop.</summary>
    XdgDesktopPortal,
    WfRecorder,
    /// <summary>Tesseract OCR with at least one language's data.</summary>
    Tesseract,
    /// <summary>The CUPS lp, lpstat and lpoptions commands.</summary>
    Cups,
    /// <summary>pw-play, PipeWire's sound player.</summary>
    PipeWire,
    /// <summary>GStreamer's PipeWire plugin (pipewiresrc), for recording through the ScreenCast portal.</summary>
    GStreamerPipeWire
}

/// <summary>Maps helper programs to package names and install commands for each distribution family.</summary>
public static class LinuxPackages
{
    private static readonly Dictionary<LinuxTool, string> Commands = new Dictionary<LinuxTool, string>
    {
        [LinuxTool.WlClipboard] = "wl-copy",
        [LinuxTool.Xclip] = "xclip",
        [LinuxTool.Xsel] = "xsel",
        [LinuxTool.Libnotify] = "notify-send",
        [LinuxTool.SecretTool] = "secret-tool",
        [LinuxTool.Grim] = "grim",
        [LinuxTool.Slurp] = "slurp",
        [LinuxTool.FFmpeg] = "ffmpeg",
        [LinuxTool.XdgUtils] = "xdg-open",
        [LinuxTool.XdgDesktopPortal] = "xdg-desktop-portal",
        [LinuxTool.WfRecorder] = "wf-recorder",
        [LinuxTool.Tesseract] = "tesseract",
        [LinuxTool.Cups] = "lp",
        [LinuxTool.PipeWire] = "pw-play",
        [LinuxTool.GStreamerPipeWire] = "gst-launch-1.0"
    };

    /// <summary>The executable name used to check whether the tool is installed.</summary>
    public static string GetCommand(LinuxTool tool) => Commands[tool];

    public static string GetPackageName(LinuxTool tool, LinuxDistributionFamily family)
    {
        return (tool, family) switch
        {
            (LinuxTool.WlClipboard, _) => "wl-clipboard",
            (LinuxTool.Libnotify, LinuxDistributionFamily.Debian) => "libnotify-bin",
            (LinuxTool.Libnotify, LinuxDistributionFamily.Arch) => "libnotify",
            (LinuxTool.Libnotify, LinuxDistributionFamily.OpenSuse) => "libnotify-tools",
            (LinuxTool.Libnotify, LinuxDistributionFamily.NixOS) => "libnotify",
            (LinuxTool.Libnotify, _) => "libnotify",
            (LinuxTool.SecretTool, LinuxDistributionFamily.Debian) => "libsecret-tools",
            (LinuxTool.SecretTool, LinuxDistributionFamily.Arch) => "libsecret",
            (LinuxTool.SecretTool, LinuxDistributionFamily.OpenSuse) => "libsecret-tools",
            (LinuxTool.SecretTool, _) => "libsecret",
            (LinuxTool.FFmpeg, LinuxDistributionFamily.Fedora) => "ffmpeg-free",
            (LinuxTool.XdgUtils, _) => "xdg-utils",
            (LinuxTool.XdgDesktopPortal, _) => "xdg-desktop-portal",
            (LinuxTool.WfRecorder, _) => "wf-recorder",
            (LinuxTool.Tesseract, LinuxDistributionFamily.Debian) => "tesseract-ocr",
            (LinuxTool.Tesseract, LinuxDistributionFamily.OpenSuse) => "tesseract-ocr",
            (LinuxTool.Tesseract, LinuxDistributionFamily.Arch) => "tesseract tesseract-data-eng",
            (LinuxTool.Cups, LinuxDistributionFamily.Debian) => "cups-client",
            (LinuxTool.Cups, LinuxDistributionFamily.Fedora) => "cups-client",
            (LinuxTool.Cups, LinuxDistributionFamily.OpenSuse) => "cups-client",
            (LinuxTool.Cups, _) => "cups",
            (LinuxTool.PipeWire, LinuxDistributionFamily.Debian) => "pipewire-bin",
            (LinuxTool.PipeWire, LinuxDistributionFamily.Fedora) => "pipewire-utils",
            (LinuxTool.PipeWire, _) => "pipewire",
            (LinuxTool.GStreamerPipeWire, LinuxDistributionFamily.Debian) => "gstreamer1.0-pipewire",
            (LinuxTool.GStreamerPipeWire, LinuxDistributionFamily.Fedora) => "pipewire-gstreamer",
            (LinuxTool.GStreamerPipeWire, LinuxDistributionFamily.OpenSuse) => "gstreamer-plugin-pipewire",
            (LinuxTool.GStreamerPipeWire, _) => "gst-plugin-pipewire",
            _ => Commands[tool]
        };
    }

    /// <summary>A copy and paste install command, for example "sudo pacman -S --needed wl-clipboard".</summary>
    public static string? GetInstallCommand(LinuxDistribution distribution, params LinuxTool[] tools)
    {
        if (tools.Length == 0)
        {
            return null;
        }

        string packages = string.Join(" ", tools.Select(tool => GetPackageName(tool, distribution.Family)).Distinct());

        return distribution.PackageManager switch
        {
            LinuxPackageManager.Apt => $"sudo apt install {packages}",
            LinuxPackageManager.Dnf => $"sudo dnf install {packages}",
            LinuxPackageManager.RpmOstree => $"rpm-ostree install {packages}",
            LinuxPackageManager.Pacman => $"sudo pacman -S --needed {packages}",
            LinuxPackageManager.Zypper => $"sudo zypper install {packages}",
            LinuxPackageManager.Nix => $"nix-env -iA {string.Join(" ", tools.Select(tool => "nixos." + GetPackageName(tool, distribution.Family)).Distinct())}",
            LinuxPackageManager.Apk => $"sudo apk add {packages}",
            LinuxPackageManager.Emerge => $"sudo emerge --ask {packages}",
            LinuxPackageManager.Xbps => $"sudo xbps-install {packages}",
            LinuxPackageManager.Eopkg => $"sudo eopkg install {packages}",
            _ => null
        };
    }

    /// <summary>A user facing reason for <see cref="FeatureSupport"/> that names the package and the command for this distribution.</summary>
    public static FeatureSupport Missing(LinuxDistribution distribution, params LinuxTool[] tools)
    {
        string names = string.Join(" or ", tools.Select(tool => GetPackageName(tool, distribution.Family)).Distinct());
        // Suggest the first alternative only, the others are listed by name.
        string? command = GetInstallCommand(distribution, tools[0]);
        string reason = command != null ? $"Install {names} ({command})." : $"Install {names} with your package manager.";
        return FeatureSupport.NotSupported(reason);
    }

    /// <summary>Distributions are asked to list their own ShareX package so users do not have to hunt for dependencies.</summary>
    public static IReadOnlyList<LinuxTool> RecommendedTools(PlatformInfo info)
    {
        List<LinuxTool> tools = new List<LinuxTool> { LinuxTool.XdgUtils, LinuxTool.Libnotify, LinuxTool.SecretTool, LinuxTool.FFmpeg };

        if (info.IsWayland)
        {
            tools.Add(LinuxTool.WlClipboard);
            tools.Add(LinuxTool.XdgDesktopPortal);

            if (info.IsWlrootsCompositor)
            {
                tools.Add(LinuxTool.Grim);
                tools.Add(LinuxTool.Slurp);
            }
        }
        else
        {
            tools.Add(LinuxTool.Xclip);
        }

        return tools;
    }
}
