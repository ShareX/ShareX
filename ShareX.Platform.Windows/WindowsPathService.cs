using System;
using System.IO;

namespace ShareX.Platform.Windows;

/// <summary>Known folders on Windows. The personal folder stays in Documents\ShareX so existing installs keep their settings.</summary>
public sealed class WindowsPathService : IPathService
{
    public string GetConfigDirectory(string applicationName) => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), applicationName);

    public string GetDataDirectory(string applicationName) => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), applicationName);

    public string GetCacheDirectory(string applicationName) => Path.Combine(GetDataDirectory(applicationName), "Cache");

    public string GetDefaultPersonalFolder(string applicationName) => Path.Combine(GetDocumentsDirectory(), applicationName);

    public string GetPicturesDirectory() => Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

    public string GetVideosDirectory() => Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);

    public string GetDocumentsDirectory() => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    public string GetDesktopDirectory() => Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
}
