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
    WfRecorder
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
        [LinuxTool.WfRecorder] = "wf-recorder"
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
