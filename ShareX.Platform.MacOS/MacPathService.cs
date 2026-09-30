using System;
using System.IO;

namespace ShareX.Platform.MacOS;

/// <summary>Folders following Apple's File System Programming Guide.</summary>
public sealed class MacPathService : IPathService
{
    private readonly string home;

    public MacPathService()
        : this(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile))
    {
    }

    public MacPathService(string home)
    {
        this.home = home;
    }

    public string LibraryDirectory => Path.Combine(home, "Library");

    public string GetConfigDirectory(string applicationName) => Path.Combine(LibraryDirectory, "Application Support", applicationName);

    public string GetDataDirectory(string applicationName) => GetConfigDirectory(applicationName);

    public string GetCacheDirectory(string applicationName) => Path.Combine(LibraryDirectory, "Caches", applicationName);

    public string GetDefaultPersonalFolder(string applicationName) => GetConfigDirectory(applicationName);

    public string GetPicturesDirectory() => Path.Combine(home, "Pictures");

    public string GetVideosDirectory() => Path.Combine(home, "Movies");

    public string GetDocumentsDirectory() => Path.Combine(home, "Documents");

    public string GetDesktopDirectory() => Path.Combine(home, "Desktop");
}
