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

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace ShareX.ImageEditor.Integration;

/// <summary>
/// Default Linux wallpaper resolver used by Avalonia hosts that do not provide a custom implementation.
/// </summary>
internal sealed class LinuxDesktopWallpaperService : IDesktopWallpaperService
{
    private static readonly string[] SandboxHostRootPrefixes =
    [
        "/run/host",
        "/var/run/host"
    ];

    private static readonly string[] CommonExecutableDirectories =
    [
        "/usr/bin",
        "/bin",
        "/usr/local/bin"
    ];

    private const string WallpaperConversionCacheDirectoryName = "sharex-imageeditor-wallpaper-cache";
    private const int WallpaperConverterTimeoutMilliseconds = 15000;
    private const int GdkPixbufWallpaperSize = 4096;
    private static readonly ConcurrentDictionary<string, object> WallpaperConversionLocks = new(StringComparer.Ordinal);

    private enum Provider
    {
        Gnome,
        Cinnamon,
        Mate,
        Xfce,
        Kde,
        Lxqt,
        Lxde
    }

    public bool IsSupported => OperatingSystem.IsLinux() && GetPreferredProviders().Any(IsAvailable);
    public bool RequiresDesktopWallpaperPrewarm => OperatingSystem.IsLinux();

    public bool TryGetDesktopWallpaper(out DesktopWallpaperInfo? wallpaper)
    {
        wallpaper = null;

        if (!OperatingSystem.IsLinux())
        {
            return false;
        }

        foreach (Provider provider in GetPreferredProviders())
        {
            if (!IsAvailable(provider))
            {
                continue;
            }

            if (TryGetDesktopWallpaper(provider, out wallpaper))
            {
                return true;
            }
        }

        return false;
    }

    public void PrewarmDesktopWallpaper()
    {
        _ = TryGetDesktopWallpaper(out _);
    }

    private static IEnumerable<Provider> GetPreferredProviders()
    {
        List<Provider> providers = new();
        AddDetectedProvider(providers, DetectDesktopEnvironment());

        Provider[] fallbackOrder =
        [
            Provider.Gnome,
            Provider.Cinnamon,
            Provider.Mate,
            Provider.Xfce,
            Provider.Kde,
            Provider.Lxqt,
            Provider.Lxde
        ];

        foreach (Provider provider in fallbackOrder)
        {
            AddProvider(providers, provider);
        }

        return providers;
    }

    private static string? DetectDesktopEnvironment()
    {
        string? currentDesktop = Environment.GetEnvironmentVariable("XDG_CURRENT_DESKTOP");
        if (ContainsDesktopToken(currentDesktop, "GNOME"))
        {
            return "GNOME";
        }

        if (ContainsDesktopToken(currentDesktop, "CINNAMON"))
        {
            return "CINNAMON";
        }

        if (ContainsDesktopToken(currentDesktop, "MATE"))
        {
            return "MATE";
        }

        if (ContainsDesktopToken(currentDesktop, "XFCE"))
        {
            return "XFCE";
        }

        if (ContainsDesktopToken(currentDesktop, "KDE"))
        {
            return "KDE";
        }

        if (ContainsDesktopToken(currentDesktop, "LXQT"))
        {
            return "LXQT";
        }

        if (ContainsDesktopToken(currentDesktop, "LXDE"))
        {
            return "LXDE";
        }

        if (string.Equals(Environment.GetEnvironmentVariable("KDE_FULL_SESSION"), "true", StringComparison.OrdinalIgnoreCase))
        {
            return "KDE";
        }

        string? desktopSession = Environment.GetEnvironmentVariable("DESKTOP_SESSION");
        if (ContainsDesktopToken(desktopSession, "GNOME"))
        {
            return "GNOME";
        }

        if (ContainsDesktopToken(desktopSession, "CINNAMON"))
        {
            return "CINNAMON";
        }

        if (ContainsDesktopToken(desktopSession, "MATE"))
        {
            return "MATE";
        }

        if (ContainsDesktopToken(desktopSession, "XFCE"))
        {
            return "XFCE";
        }

        if (ContainsDesktopToken(desktopSession, "KDE"))
        {
            return "KDE";
        }

        if (ContainsDesktopToken(desktopSession, "LXQT"))
        {
            return "LXQT";
        }

        if (ContainsDesktopToken(desktopSession, "LXDE"))
        {
            return "LXDE";
        }

        return null;
    }

    private static bool ContainsDesktopToken(string? value, string token)
    {
        return !string.IsNullOrWhiteSpace(value) &&
               value.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static void AddDetectedProvider(List<Provider> providers, string? desktop)
    {
        switch (desktop)
        {
            case "GNOME":
                AddProvider(providers, Provider.Gnome);
                break;
            case "CINNAMON":
                AddProvider(providers, Provider.Cinnamon);
                AddProvider(providers, Provider.Gnome);
                break;
            case "MATE":
                AddProvider(providers, Provider.Mate);
                break;
            case "XFCE":
                AddProvider(providers, Provider.Xfce);
                break;
            case "KDE":
                AddProvider(providers, Provider.Kde);
                break;
            case "LXQT":
                AddProvider(providers, Provider.Lxqt);
                break;
            case "LXDE":
                AddProvider(providers, Provider.Lxde);
                break;
        }
    }

    private static void AddProvider(List<Provider> providers, Provider provider)
    {
        if (!providers.Contains(provider))
        {
            providers.Add(provider);
        }
    }

    private static bool IsAvailable(Provider provider)
    {
        return provider switch
        {
            Provider.Gnome => CommandExists("gsettings") && GSettingsSchemaExists("org.gnome.desktop.background"),
            Provider.Cinnamon => CommandExists("gsettings") && GSettingsSchemaExists("org.cinnamon.desktop.background"),
            Provider.Mate => CommandExists("gsettings") && GSettingsSchemaExists("org.mate.background"),
            Provider.Xfce => CommandExists("xfconf-query"),
            Provider.Kde => File.Exists(GetKdeWallpaperConfigPath()),
            Provider.Lxqt => HasPcmanfmConfigDirectory("pcmanfm-qt"),
            Provider.Lxde => HasPcmanfmConfigDirectory("pcmanfm"),
            _ => false
        };
    }

    private static bool TryGetDesktopWallpaper(Provider provider, out DesktopWallpaperInfo? wallpaper)
    {
        return provider switch
        {
            Provider.Gnome => TryGetGSettingsWallpaper("org.gnome.desktop.background", true, out wallpaper),
            Provider.Cinnamon => TryGetGSettingsWallpaper("org.cinnamon.desktop.background", true, out wallpaper),
            Provider.Mate => TryGetMateWallpaper(out wallpaper),
            Provider.Xfce => TryGetXfceWallpaper(out wallpaper),
            Provider.Kde => TryGetKdeWallpaper(out wallpaper),
            Provider.Lxqt => TryGetPcmanfmWallpaper("pcmanfm-qt", ["lxqt", "default"], ["settings.conf"], out wallpaper),
            Provider.Lxde => TryGetPcmanfmWallpaper("pcmanfm", ["LXDE", "default", "lxde"], ["pcmanfm.conf", "desktop-items-0.conf"], out wallpaper),
            _ => ReturnFalse(out wallpaper)
        };
    }

    private static bool TryGetGSettingsWallpaper(string schema, bool allowDarkVariant, out DesktopWallpaperInfo? wallpaper)
    {
        wallpaper = null;

        string? pictureValue = null;
        if (allowDarkVariant && IsDarkWallpaperPreferred() &&
            TryReadGSettingsValue(schema, "picture-uri-dark", out string? darkPictureValue))
        {
            pictureValue = darkPictureValue;
        }

        if (string.IsNullOrWhiteSpace(pictureValue) &&
            !TryReadGSettingsValue(schema, "picture-uri", out pictureValue))
        {
            return false;
        }

        string? path = ResolveAccessibleWallpaperPath(ParseWallpaperPath(pictureValue));
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        DesktopWallpaperLayout layout = DesktopWallpaperLayout.Fill;
        if (TryReadGSettingsValue(schema, "picture-options", out string? pictureOptions))
        {
            layout = MapGSettingsPictureOptions(pictureOptions);
        }

        wallpaper = new DesktopWallpaperInfo
        {
            Path = path,
            Layout = layout
        };

        return true;
    }

    private static bool TryGetMateWallpaper(out DesktopWallpaperInfo? wallpaper)
    {
        wallpaper = null;

        if (!TryReadGSettingsValue("org.mate.background", "picture-filename", out string? pictureValue))
        {
            return false;
        }

        string? path = ResolveAccessibleWallpaperPath(ParseWallpaperPath(pictureValue));
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        DesktopWallpaperLayout layout = DesktopWallpaperLayout.Fill;
        if (TryReadGSettingsValue("org.mate.background", "picture-options", out string? pictureOptions))
        {
            layout = MapGSettingsPictureOptions(pictureOptions);
        }

        wallpaper = new DesktopWallpaperInfo
        {
            Path = path,
            Layout = layout
        };

        return true;
    }

    private static bool TryGetXfceWallpaper(out DesktopWallpaperInfo? wallpaper)
    {
        wallpaper = null;

        if (!TryReadCommandOutput("xfconf-query", "-c xfce4-desktop -l", out string propertyList))
        {
            return false;
        }

        IEnumerable<string> candidateProperties = propertyList
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(line => line.EndsWith("/last-image", StringComparison.Ordinal) ||
                           line.EndsWith("/image-path", StringComparison.Ordinal))
            .OrderBy(line => line.Contains("workspace0", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
            .ThenBy(line => line, StringComparer.Ordinal);

        foreach (string property in candidateProperties)
        {
            if (!TryReadCommandOutput("xfconf-query", $"-c xfce4-desktop -p {QuoteArgument(property)}", out string pictureValue))
            {
                continue;
            }

            string? path = ResolveAccessibleWallpaperPath(ParseWallpaperPath(pictureValue));
            if (string.IsNullOrWhiteSpace(path))
            {
                continue;
            }

            DesktopWallpaperLayout layout = DesktopWallpaperLayout.Fill;
            string styleProperty = property.EndsWith("/last-image", StringComparison.Ordinal)
                ? property.Substring(0, property.Length - "/last-image".Length) + "/image-style"
                : property.Substring(0, property.Length - "/image-path".Length) + "/image-style";

            if (TryReadCommandOutput("xfconf-query", $"-c xfce4-desktop -p {QuoteArgument(styleProperty)}", out string styleValue))
            {
                layout = MapXfceStyle(styleValue);
            }

            wallpaper = new DesktopWallpaperInfo
            {
                Path = path!,
                Layout = layout
            };

            return true;
        }

        return false;
    }

    private static bool TryGetKdeWallpaper(out DesktopWallpaperInfo? wallpaper)
    {
        wallpaper = null;

        string configPath = GetKdeWallpaperConfigPath();
        if (!File.Exists(configPath))
        {
            return false;
        }

        string? path = null;
        string? fillMode = null;
        bool inWallpaperSection = false;

        foreach (string rawLine in File.ReadLines(configPath))
        {
            string line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
            {
                continue;
            }

            if (line.StartsWith('[') && line.EndsWith(']'))
            {
                if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                {
                    break;
                }

                inWallpaperSection = line.Contains("Wallpaper][org.kde.image][General]", StringComparison.Ordinal);
                continue;
            }

            if (!inWallpaperSection)
            {
                continue;
            }

            int separatorIndex = line.IndexOf('=');
            if (separatorIndex <= 0)
            {
                continue;
            }

            string key = line.Substring(0, separatorIndex).Trim();
            string value = line.Substring(separatorIndex + 1).Trim();

            if (key.Equals("Image", StringComparison.Ordinal))
            {
                string? candidatePath = ResolveAccessibleWallpaperPath(ParseWallpaperPath(value));
                if (!string.IsNullOrWhiteSpace(candidatePath))
                {
                    path = candidatePath;
                }
            }
            else if (key.Equals("FillMode", StringComparison.Ordinal))
            {
                fillMode = value;
            }
        }

        path = ResolveAccessibleWallpaperPath(path);
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        wallpaper = new DesktopWallpaperInfo
        {
            Path = path,
            Layout = MapKdeFillMode(fillMode)
        };

        return true;
    }

    private static bool TryGetPcmanfmWallpaper(
        string appDirectoryName,
        IReadOnlyList<string> preferredProfiles,
        IReadOnlyList<string> configFileNames,
        out DesktopWallpaperInfo? wallpaper)
    {
        wallpaper = null;

        foreach (string configFile in EnumeratePcmanfmConfigFiles(appDirectoryName, preferredProfiles, configFileNames))
        {
            if (!TryReadPcmanfmWallpaper(configFile, out string? path, out DesktopWallpaperLayout layout))
            {
                continue;
            }

            wallpaper = new DesktopWallpaperInfo
            {
                Path = path!,
                Layout = layout
            };

            return true;
        }

        return false;
    }

    private static bool IsDarkWallpaperPreferred()
    {
        return TryReadGSettingsValue("org.gnome.desktop.interface", "color-scheme", out string? colorScheme) &&
               string.Equals(colorScheme, "prefer-dark", StringComparison.OrdinalIgnoreCase);
    }

    private static bool TryReadGSettingsValue(string schema, string key, out string? value)
    {
        value = null;

        if (!TryReadCommandOutput("gsettings", $"get {schema} {key}", out string output))
        {
            return false;
        }

        value = output.Trim().Trim('\'', '"');
        return !string.IsNullOrWhiteSpace(value) && !string.Equals(value, "nothing", StringComparison.OrdinalIgnoreCase);
    }

    private static DesktopWallpaperLayout MapGSettingsPictureOptions(string? option)
    {
        return option?.Trim().Trim('\'', '"').ToLowerInvariant() switch
        {
            "wallpaper" => DesktopWallpaperLayout.Tile,
            "centered" => DesktopWallpaperLayout.Center,
            "scaled" => DesktopWallpaperLayout.Fit,
            "stretched" => DesktopWallpaperLayout.Stretch,
            "zoom" => DesktopWallpaperLayout.Fill,
            "spanned" => DesktopWallpaperLayout.Span,
            _ => DesktopWallpaperLayout.Fill
        };
    }

    private static DesktopWallpaperLayout MapXfceStyle(string? styleValue)
    {
        if (!int.TryParse(styleValue?.Trim(), out int style))
        {
            return DesktopWallpaperLayout.Fill;
        }

        return style switch
        {
            1 => DesktopWallpaperLayout.Center,
            2 => DesktopWallpaperLayout.Tile,
            3 => DesktopWallpaperLayout.Stretch,
            4 => DesktopWallpaperLayout.Fit,
            5 => DesktopWallpaperLayout.Fill,
            6 => DesktopWallpaperLayout.Span,
            _ => DesktopWallpaperLayout.Fill
        };
    }

    private static DesktopWallpaperLayout MapKdeFillMode(string? fillModeValue)
    {
        if (int.TryParse(fillModeValue?.Trim(), out int fillMode))
        {
            return fillMode switch
            {
                0 => DesktopWallpaperLayout.Stretch,
                1 => DesktopWallpaperLayout.Fit,
                2 => DesktopWallpaperLayout.Fill,
                3 => DesktopWallpaperLayout.Tile,
                4 => DesktopWallpaperLayout.Tile,
                5 => DesktopWallpaperLayout.Tile,
                6 => DesktopWallpaperLayout.Center,
                _ => DesktopWallpaperLayout.Fill
            };
        }

        return fillModeValue?.Trim().ToLowerInvariant() switch
        {
            "stretch" => DesktopWallpaperLayout.Stretch,
            "preserveaspectfit" => DesktopWallpaperLayout.Fit,
            "preserveaspectcrop" => DesktopWallpaperLayout.Fill,
            "tile" => DesktopWallpaperLayout.Tile,
            "pad" => DesktopWallpaperLayout.Center,
            _ => DesktopWallpaperLayout.Fill
        };
    }

    private static string? ParseWallpaperPath(string? rawValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return null;
        }

        string value = rawValue.Trim().Trim('\'', '"');
        if (string.IsNullOrWhiteSpace(value) ||
            value.Equals("none", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (value.StartsWith("file://", StringComparison.OrdinalIgnoreCase) &&
            Uri.TryCreate(value, UriKind.Absolute, out Uri? fileUri))
        {
            return fileUri.LocalPath;
        }

        return value;
    }

    private static bool HasPcmanfmConfigDirectory(string appDirectoryName)
    {
        foreach (string rootDirectory in EnumeratePcmanfmRoots(appDirectoryName))
        {
            if (Directory.Exists(rootDirectory))
            {
                return true;
            }
        }

        return false;
    }

    private static string? ResolveAccessibleWallpaperPath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        foreach (string candidatePath in GetAccessiblePathCandidates(path))
        {
            if (File.Exists(candidatePath))
            {
                return ResolveCompatibleWallpaperPath(candidatePath);
            }
        }

        return null;
    }

    private static IEnumerable<string> GetAccessiblePathCandidates(string path)
    {
        yield return path;

        if (string.IsNullOrWhiteSpace(path) ||
            !Path.IsPathRooted(path) ||
            IsSandboxHostMirrorPath(path))
        {
            yield break;
        }

        string relativePath = path.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        foreach (string hostRootPrefix in SandboxHostRootPrefixes)
        {
            yield return Path.Combine(hostRootPrefix, relativePath);
        }
    }

    private static string? ResolveCompatibleWallpaperPath(string candidatePath)
    {
        if (!RequiresWallpaperConversion(candidatePath))
        {
            return candidatePath;
        }

        return TryConvertWallpaper(candidatePath, out string? convertedPath) ? convertedPath : null;
    }

    private static bool RequiresWallpaperConversion(string path)
    {
        return !string.IsNullOrWhiteSpace(path) &&
               Path.GetExtension(path).Equals(".jxl", StringComparison.OrdinalIgnoreCase);
    }

    private static string GetWallpaperConversionCachePath(string sourcePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourcePath);

        string cacheDirectory = Path.Combine(Path.GetTempPath(), WallpaperConversionCacheDirectoryName);
        Directory.CreateDirectory(cacheDirectory);

        FileInfo sourceInfo = new(sourcePath);
        string cacheKey = string.Concat(sourcePath, "|", sourceInfo.Length, "|", sourceInfo.LastWriteTimeUtc.Ticks);
        string cacheHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(cacheKey))).ToLowerInvariant();
        string cacheFileNameBase = Path.GetFileNameWithoutExtension(sourcePath);
        if (string.IsNullOrWhiteSpace(cacheFileNameBase))
        {
            cacheFileNameBase = "wallpaper";
        }

        return Path.Combine(cacheDirectory, $"{cacheFileNameBase}-{cacheHash}.png");
    }

    private static bool TryConvertWallpaper(string sourcePath, out string? convertedPath)
    {
        convertedPath = null;

        string cachePath = GetWallpaperConversionCachePath(sourcePath);
        object conversionLock = WallpaperConversionLocks.GetOrAdd(cachePath, static _ => new object());

        lock (conversionLock)
        {
            if (File.Exists(cachePath))
            {
                convertedPath = cachePath;
                return true;
            }

            if (TryConvertWallpaperWithFfmpeg(sourcePath, cachePath) ||
                TryConvertWallpaperWithGlycin(sourcePath, cachePath) ||
                TryConvertWallpaperWithGdkPixbuf(sourcePath, cachePath))
            {
                convertedPath = cachePath;
                return true;
            }

            return false;
        }
    }

    private static bool TryConvertWallpaperWithFfmpeg(string sourcePath, string outputPath)
    {
        if (!TryResolveCommandPath("ffmpeg", out string? ffmpegPath))
        {
            return false;
        }

        return TryRunWallpaperConverter(
            ffmpegPath!,
            $"-hide_banner -loglevel error -y -i {QuoteArgument(sourcePath)} -frames:v 1 {QuoteArgument(GetTemporaryConvertedWallpaperPath(outputPath))}",
            outputPath);
    }

    private static bool TryConvertWallpaperWithGdkPixbuf(string sourcePath, string outputPath)
    {
        if (!TryResolveCommandPath("gdk-pixbuf-thumbnailer", out string? thumbnailerPath))
        {
            return false;
        }

        return TryRunWallpaperConverter(
            thumbnailerPath!,
            $"-s {GdkPixbufWallpaperSize} {QuoteArgument(sourcePath)} {QuoteArgument(GetTemporaryConvertedWallpaperPath(outputPath))}",
            outputPath);
    }

    private static bool TryConvertWallpaperWithGlycin(string sourcePath, string outputPath)
    {
        if (!TryResolveCommandPath("glycin-thumbnailer", out string? thumbnailerPath))
        {
            return false;
        }

        return TryRunWallpaperConverter(
            thumbnailerPath!,
            $"--input {QuoteArgument(PathToFileUri(sourcePath))} --output {QuoteArgument(GetTemporaryConvertedWallpaperPath(outputPath))} --size {GdkPixbufWallpaperSize}",
            outputPath);
    }

    private static bool TryRunWallpaperConverter(string fileName, string arguments, string outputPath)
    {
        string tempOutputPath = GetTemporaryConvertedWallpaperPath(outputPath);

        try
        {
            string? outputDirectory = Path.GetDirectoryName(outputPath);
            if (string.IsNullOrWhiteSpace(outputDirectory))
            {
                return false;
            }

            Directory.CreateDirectory(outputDirectory);

            if (File.Exists(tempOutputPath))
            {
                File.Delete(tempOutputPath);
            }

            using Process? process = Process.Start(new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });

            if (process == null)
            {
                return false;
            }

            process.WaitForExit(WallpaperConverterTimeoutMilliseconds);

            if (!process.HasExited)
            {
                try
                {
                    process.Kill(entireProcessTree: true);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                return false;
            }

            if (process.ExitCode != 0 || !File.Exists(tempOutputPath))
            {
                return false;
            }

            File.Move(tempOutputPath, outputPath, overwrite: true);
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return false;
        }
        finally
        {
            try
            {
                if (File.Exists(tempOutputPath))
                {
                    File.Delete(tempOutputPath);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }

    private static string GetTemporaryConvertedWallpaperPath(string outputPath)
    {
        string? directory = Path.GetDirectoryName(outputPath);
        string fileName = Path.GetFileNameWithoutExtension(outputPath);

        if (string.IsNullOrWhiteSpace(directory))
        {
            directory = Path.GetTempPath();
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            fileName = "wallpaper";
        }

        return Path.Combine(directory, fileName + ".tmp.png");
    }

    private static bool IsSandboxHostMirrorPath(string path)
    {
        foreach (string hostRootPrefix in SandboxHostRootPrefixes)
        {
            if (path.Equals(hostRootPrefix, StringComparison.Ordinal) ||
                path.StartsWith(hostRootPrefix + Path.DirectorySeparatorChar, StringComparison.Ordinal) ||
                path.StartsWith(hostRootPrefix + Path.AltDirectorySeparatorChar, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static IEnumerable<string> EnumeratePcmanfmConfigFiles(
        string appDirectoryName,
        IReadOnlyList<string> preferredProfiles,
        IReadOnlyList<string> configFileNames)
    {
        HashSet<string> yieldedFiles = new(StringComparer.Ordinal);

        foreach (string rootDirectory in EnumeratePcmanfmRoots(appDirectoryName))
        {
            if (!Directory.Exists(rootDirectory))
            {
                continue;
            }

            foreach (string preferredProfile in preferredProfiles)
            {
                foreach (string configFileName in configFileNames)
                {
                    string candidateFile = Path.Combine(rootDirectory, preferredProfile, configFileName);
                    if (File.Exists(candidateFile) && yieldedFiles.Add(candidateFile))
                    {
                        yield return candidateFile;
                    }
                }
            }

            foreach (string profileDirectory in Directory.EnumerateDirectories(rootDirectory).OrderBy(path => path, StringComparer.Ordinal))
            {
                foreach (string configFileName in configFileNames)
                {
                    string candidateFile = Path.Combine(profileDirectory, configFileName);
                    if (File.Exists(candidateFile) && yieldedFiles.Add(candidateFile))
                    {
                        yield return candidateFile;
                    }
                }
            }
        }
    }

    private static IEnumerable<string> EnumeratePcmanfmRoots(string appDirectoryName)
    {
        string userConfigRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appDirectoryName);
        yield return userConfigRoot;
        yield return Path.Combine("/etc/xdg", appDirectoryName);
        yield return Path.Combine("/run/host/etc/xdg", appDirectoryName);
        yield return Path.Combine("/var/run/host/etc/xdg", appDirectoryName);
    }

    private static bool TryReadPcmanfmWallpaper(string configFile, out string? path, out DesktopWallpaperLayout layout)
    {
        path = null;
        layout = DesktopWallpaperLayout.Fill;
        string? rawPath = null;
        string? rawLayout = null;

        foreach (string rawLine in File.ReadLines(configFile))
        {
            string line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#') || line.StartsWith(';') || line.StartsWith('['))
            {
                continue;
            }

            int separatorIndex = line.IndexOf('=');
            if (separatorIndex <= 0)
            {
                continue;
            }

            string key = line.Substring(0, separatorIndex).Trim();
            string value = line.Substring(separatorIndex + 1).Trim();

            if (key.Equals("wallpaper", StringComparison.OrdinalIgnoreCase))
            {
                rawPath = value;
            }
            else if (key.Equals("wallpaper_mode", StringComparison.OrdinalIgnoreCase) ||
                     key.Equals("wallpapermode", StringComparison.OrdinalIgnoreCase))
            {
                rawLayout = value;
            }
        }

        path = ResolveAccessibleWallpaperPath(ParseWallpaperPath(rawPath));
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        layout = MapPcmanfmWallpaperMode(rawLayout);
        return true;
    }

    private static DesktopWallpaperLayout MapPcmanfmWallpaperMode(string? mode)
    {
        return mode?.Trim().Trim('\'', '"').ToLowerInvariant() switch
        {
            "center" or "centered" => DesktopWallpaperLayout.Center,
            "tile" or "tiled" => DesktopWallpaperLayout.Tile,
            "stretch" or "stretched" => DesktopWallpaperLayout.Stretch,
            "fit" or "scaled" => DesktopWallpaperLayout.Fit,
            "crop" or "zoom" or "fill" => DesktopWallpaperLayout.Fill,
            _ => DesktopWallpaperLayout.Fill
        };
    }

    private static bool GSettingsSchemaExists(string schema)
    {
        return TryReadCommandOutput("gsettings", "list-schemas", out string schemas) &&
               schemas.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                   .Any(line => line.Equals(schema, StringComparison.Ordinal));
    }

    private static string GetKdeWallpaperConfigPath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".config",
            "plasma-org.kde.plasma.desktop-appletsrc");
    }

    private static bool CommandExists(string command)
    {
        return TryResolveCommandPath(command, out _);
    }

    private static bool TryResolveCommandPath(string command, out string? resolvedPath)
    {
        resolvedPath = null;

        if (string.IsNullOrWhiteSpace(command))
        {
            return false;
        }

        if (Path.IsPathRooted(command))
        {
            resolvedPath = File.Exists(command) ? command : null;
            return resolvedPath != null;
        }

        if (TryReadCommandOutput("sh", $"-lc {QuoteArgument($"command -v {command}")}", out string shellResolvedPath) &&
            !string.IsNullOrWhiteSpace(shellResolvedPath))
        {
            resolvedPath = shellResolvedPath
                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(resolvedPath))
            {
                return true;
            }
        }

        string? pathEnvironment = Environment.GetEnvironmentVariable("PATH");
        if (!string.IsNullOrWhiteSpace(pathEnvironment))
        {
            foreach (string directory in pathEnvironment.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                string candidatePath = Path.Combine(directory, command);
                if (File.Exists(candidatePath))
                {
                    resolvedPath = candidatePath;
                    return true;
                }
            }
        }

        foreach (string directory in CommonExecutableDirectories)
        {
            string candidatePath = Path.Combine(directory, command);
            if (File.Exists(candidatePath))
            {
                resolvedPath = candidatePath;
                return true;
            }
        }

        return false;
    }

    private static bool TryReadCommandOutput(string fileName, string arguments, out string output)
    {
        output = string.Empty;

        try
        {
            using Process? process = Process.Start(new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });

            if (process == null)
            {
                return false;
            }

            output = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit(2000);
            return process.ExitCode == 0;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return false;
        }
    }

    private static string QuoteArgument(string value)
    {
        return "\"" + value.Replace("\"", "\\\"") + "\"";
    }

    private static string PathToFileUri(string path)
    {
        return new Uri(path).AbsoluteUri;
    }

    private static bool ReturnFalse(out DesktopWallpaperInfo? wallpaper)
    {
        wallpaper = null;
        return false;
    }
}