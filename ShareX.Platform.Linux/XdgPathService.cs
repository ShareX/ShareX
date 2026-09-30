using System;
using System.Collections.Generic;
using System.IO;

namespace ShareX.Platform.Linux;

/// <summary>Resolves folders with the XDG Base Directory specification and xdg-user-dirs.</summary>
public sealed class XdgPathService : IPathService
{
    private readonly Func<string, string?> getEnvironmentVariable;
    private readonly Func<string, string?> readFile;
    private readonly string home;

    public XdgPathService()
        : this(Environment.GetEnvironmentVariable, ReadFileOrNull, Environment.GetFolderPath(Environment.SpecialFolder.UserProfile))
    {
    }

    public XdgPathService(Func<string, string?> getEnvironmentVariable, Func<string, string?> readFile, string home)
    {
        this.getEnvironmentVariable = getEnvironmentVariable;
        this.readFile = readFile;
        this.home = string.IsNullOrEmpty(home) ? getEnvironmentVariable("HOME") ?? "/tmp" : home;
    }

    public string ConfigHome => GetBaseDirectory("XDG_CONFIG_HOME", ".config");

    public string DataHome => GetBaseDirectory("XDG_DATA_HOME", Path.Combine(".local", "share"));

    public string CacheHome => GetBaseDirectory("XDG_CACHE_HOME", ".cache");

    public string GetConfigDirectory(string applicationName) => Path.Combine(ConfigHome, applicationName);

    public string GetDataDirectory(string applicationName) => Path.Combine(DataHome, applicationName);

    public string GetCacheDirectory(string applicationName) => Path.Combine(CacheHome, applicationName);

    // Settings, history and logs live together in ShareX's personal folder, so the config directory is the natural home.
    public string GetDefaultPersonalFolder(string applicationName) => GetConfigDirectory(applicationName);

    public string GetPicturesDirectory() => GetUserDirectory("XDG_PICTURES_DIR", "Pictures");

    public string GetVideosDirectory() => GetUserDirectory("XDG_VIDEOS_DIR", "Videos");

    public string GetDocumentsDirectory() => GetUserDirectory("XDG_DOCUMENTS_DIR", "Documents");

    public string GetDesktopDirectory() => GetUserDirectory("XDG_DESKTOP_DIR", "Desktop");

    private string GetBaseDirectory(string variable, string fallbackRelativeToHome)
    {
        string? value = getEnvironmentVariable(variable);

        // The spec says relative paths are invalid and must be ignored.
        return !string.IsNullOrEmpty(value) && Path.IsPathRooted(value) ? value : Path.Combine(home, fallbackRelativeToHome);
    }

    /// <summary>Reads a folder from $XDG_CONFIG_HOME/user-dirs.dirs, which xdg-user-dirs localises (for example ~/Bilder).</summary>
    private string GetUserDirectory(string key, string fallbackName)
    {
        string? value = getEnvironmentVariable(key);

        if (string.IsNullOrEmpty(value))
        {
            string? content = readFile(Path.Combine(ConfigHome, "user-dirs.dirs"));

            if (content != null)
            {
                ParseUserDirs(content).TryGetValue(key, out value);
            }
        }

        if (!string.IsNullOrEmpty(value))
        {
            value = ExpandHome(value);

            // xdg-user-dirs points a disabled folder at $HOME itself. Treat that as unset.
            if (Path.IsPathRooted(value) && !string.Equals(value.TrimEnd('/'), home.TrimEnd('/'), StringComparison.Ordinal))
            {
                return value;
            }
        }

        return Path.Combine(home, fallbackName);
    }

    internal static Dictionary<string, string> ParseUserDirs(string content)
    {
        Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.Ordinal);

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

            string key = line[..equals].Trim();
            string value = line[(equals + 1)..].Trim();

            if (value.Length >= 2 && value[0] == '"' && value[^1] == '"')
            {
                value = value[1..^1].Replace("\\\"", "\"").Replace("\\\\", "\\");
            }

            result[key] = value;
        }

        return result;
    }

    private string ExpandHome(string value)
    {
        if (value.StartsWith("$HOME", StringComparison.Ordinal))
        {
            return home + value[5..];
        }

        if (value.StartsWith("${HOME}", StringComparison.Ordinal))
        {
            return home + value[7..];
        }

        if (value == "~" || value.StartsWith("~/", StringComparison.Ordinal))
        {
            return home + value[1..];
        }

        return value;
    }

    private static string? ReadFileOrNull(string path)
    {
        try
        {
            return File.Exists(path) ? File.ReadAllText(path) : null;
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            return null;
        }
    }
}
