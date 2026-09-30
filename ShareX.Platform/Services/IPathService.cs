namespace ShareX.Platform;

/// <summary>Resolves per user folders following each platform's conventions.</summary>
/// <remarks>
/// Windows: %APPDATA%, %LOCALAPPDATA%, Documents.
/// macOS: ~/Library/Application Support, ~/Library/Caches.
/// Linux: $XDG_CONFIG_HOME, $XDG_DATA_HOME, $XDG_CACHE_HOME and xdg-user-dirs.
/// </remarks>
public interface IPathService
{
    /// <summary>Folder for settings files, for example %APPDATA%\ShareX or $XDG_CONFIG_HOME/ShareX.</summary>
    string GetConfigDirectory(string applicationName);

    /// <summary>Folder for application data such as history databases.</summary>
    string GetDataDirectory(string applicationName);

    /// <summary>Folder for disposable caches.</summary>
    string GetCacheDirectory(string applicationName);

    /// <summary>The folder ShareX uses for its settings, history and logs when the user has not chosen a custom one.</summary>
    string GetDefaultPersonalFolder(string applicationName);

    /// <summary>The user's pictures folder, used as the parent of the screenshots folder.</summary>
    string GetPicturesDirectory();

    /// <summary>The user's videos folder.</summary>
    string GetVideosDirectory();

    /// <summary>The user's documents folder.</summary>
    string GetDocumentsDirectory();

    /// <summary>The user's desktop folder.</summary>
    string GetDesktopDirectory();
}
