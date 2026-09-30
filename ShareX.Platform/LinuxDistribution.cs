using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ShareX.Platform;

/// <summary>Linux distribution families that share a package manager and package names.</summary>
public enum LinuxDistributionFamily
{
    Unknown,
    /// <summary>Debian, Ubuntu, Linux Mint, Pop!_OS, elementary OS, Zorin OS, KDE neon.</summary>
    Debian,
    /// <summary>Fedora, RHEL, CentOS Stream, Rocky Linux, AlmaLinux, Nobara.</summary>
    Fedora,
    /// <summary>Arch Linux, Omarchy, EndeavourOS, Manjaro, Garuda, CachyOS, SteamOS.</summary>
    Arch,
    /// <summary>openSUSE Tumbleweed, Leap, Slowroll and SUSE Linux Enterprise.</summary>
    OpenSuse,
    NixOS,
    Alpine,
    Gentoo,
    Void,
    Solus
}

public enum LinuxPackageManager
{
    Unknown,
    Apt,
    Dnf,
    /// <summary>Image based Fedora variants (Silverblue, Kinoite, Bazzite) layer packages with rpm-ostree.</summary>
    RpmOstree,
    Pacman,
    Zypper,
    Nix,
    Apk,
    Emerge,
    Xbps,
    Eopkg
}

/// <summary>A Linux distribution as described by /etc/os-release.</summary>
/// <param name="Id">The os-release ID, for example "ubuntu", "fedora", "arch", "endeavouros", "manjaro" or "opensuse-tumbleweed".</param>
/// <param name="IdLike">The os-release ID_LIKE list, most specific first.</param>
/// <param name="VariantId">The os-release VARIANT_ID, for example "silverblue" or "kinoite".</param>
public sealed record LinuxDistribution(
    string Id,
    IReadOnlyList<string> IdLike,
    string Name,
    string PrettyName,
    string? VersionId,
    string? VariantId,
    LinuxDistributionFamily Family,
    LinuxPackageManager PackageManager,
    bool IsImmutable)
{
    public static LinuxDistribution Unknown { get; } = new LinuxDistribution("linux", Array.Empty<string>(), "Linux", "Linux", null, null,
        LinuxDistributionFamily.Unknown, LinuxPackageManager.Unknown, false);

    /// <summary>True when the distribution is, or declares itself like, the given os-release ID.</summary>
    public bool IsOrIsLike(string id) => string.Equals(Id, id, StringComparison.OrdinalIgnoreCase) ||
        IdLike.Any(like => string.Equals(like, id, StringComparison.OrdinalIgnoreCase));

    public override string ToString() => PrettyName;

    /// <summary>Reads /etc/os-release, falling back to /usr/lib/os-release as the specification requires.</summary>
    public static LinuxDistribution Detect()
    {
        foreach (string path in new[] { "/etc/os-release", "/usr/lib/os-release" })
        {
            try
            {
                if (File.Exists(path))
                {
                    return Parse(File.ReadAllText(path), File.Exists("/run/ostree-booted"));
                }
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException)
            {
            }
        }

        return Unknown;
    }

    public static LinuxDistribution Parse(string osRelease, bool ostreeBooted = false)
    {
        Dictionary<string, string> values = ParseOsRelease(osRelease);

        string id = values.GetValueOrDefault("ID", "linux").ToLowerInvariant();
        string[] idLike = values.GetValueOrDefault("ID_LIKE", "").ToLowerInvariant()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        string name = values.GetValueOrDefault("NAME", "Linux");
        string prettyName = values.GetValueOrDefault("PRETTY_NAME", name);
        string? versionId = values.GetValueOrDefault("VERSION_ID");
        string? variantId = values.GetValueOrDefault("VARIANT_ID")?.ToLowerInvariant();

        LinuxDistributionFamily family = GetFamily(id);

        foreach (string like in idLike)
        {
            if (family != LinuxDistributionFamily.Unknown) break;
            family = GetFamily(like);
        }

        bool immutable = ostreeBooted || variantId is "silverblue" or "kinoite" or "sericea" or "onyx" or "coreos" or "iot" ||
            id is "bazzite" or "bluefin" or "aurora" or "steamos" or "opensuse-microos" or "opensuse-aeon" or "opensuse-kalpa";

        LinuxPackageManager packageManager = family switch
        {
            LinuxDistributionFamily.Debian => LinuxPackageManager.Apt,
            LinuxDistributionFamily.Fedora => immutable ? LinuxPackageManager.RpmOstree : LinuxPackageManager.Dnf,
            LinuxDistributionFamily.Arch => LinuxPackageManager.Pacman,
            LinuxDistributionFamily.OpenSuse => LinuxPackageManager.Zypper,
            LinuxDistributionFamily.NixOS => LinuxPackageManager.Nix,
            LinuxDistributionFamily.Alpine => LinuxPackageManager.Apk,
            LinuxDistributionFamily.Gentoo => LinuxPackageManager.Emerge,
            LinuxDistributionFamily.Void => LinuxPackageManager.Xbps,
            LinuxDistributionFamily.Solus => LinuxPackageManager.Eopkg,
            _ => LinuxPackageManager.Unknown
        };

        // NixOS and SteamOS keep their root file system read only as well.
        immutable |= family == LinuxDistributionFamily.NixOS;

        return new LinuxDistribution(id, idLike, name, prettyName, versionId, variantId, family, packageManager, immutable);
    }

    private static LinuxDistributionFamily GetFamily(string id)
    {
        if (id.StartsWith("opensuse", StringComparison.Ordinal) || id is "suse" or "sles" or "sled")
        {
            return LinuxDistributionFamily.OpenSuse;
        }

        return id switch
        {
            "debian" or "ubuntu" or "linuxmint" or "pop" or "elementary" or "zorin" or "neon" or "kali" or "raspbian" or "pureos" or "deepin" =>
                LinuxDistributionFamily.Debian,
            "fedora" or "rhel" or "centos" or "rocky" or "almalinux" or "ol" or "nobara" or "bazzite" or "bluefin" or "aurora" or "ultramarine" =>
                LinuxDistributionFamily.Fedora,
            "arch" or "omarchy" or "endeavouros" or "manjaro" or "garuda" or "cachyos" or "artix" or "arcolinux" or "steamos" or "archcraft" =>
                LinuxDistributionFamily.Arch,
            "nixos" => LinuxDistributionFamily.NixOS,
            "alpine" or "postmarketos" => LinuxDistributionFamily.Alpine,
            "gentoo" or "funtoo" => LinuxDistributionFamily.Gentoo,
            "void" => LinuxDistributionFamily.Void,
            "solus" => LinuxDistributionFamily.Solus,
            _ => LinuxDistributionFamily.Unknown
        };
    }

    internal static Dictionary<string, string> ParseOsRelease(string content)
    {
        Dictionary<string, string> values = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (string rawLine in content.Split('\n'))
        {
            string line = rawLine.Trim();

            if (line.Length == 0 || line[0] == '#')
            {
                continue;
            }

            int equals = line.IndexOf('=');

            if (equals <= 0)
            {
                continue;
            }

            string value = line[(equals + 1)..].Trim();

            if (value.Length >= 2 && (value[0] == '"' || value[0] == '\'') && value[^1] == value[0])
            {
                value = value[1..^1];
            }

            values[line[..equals].Trim()] = value.Replace("\\\"", "\"").Replace("\\$", "$").Replace("\\`", "`").Replace("\\\\", "\\");
        }

        return values;
    }
}
