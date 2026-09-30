using System.Collections.Generic;

namespace ShareX.Platform;

/// <summary>Opening URLs and files, and showing files in Explorer, Finder or the Linux file manager.</summary>
public interface IShellService
{
    bool OpenUrl(string url);

    /// <summary>Opens a file or folder with its default application.</summary>
    bool OpenPath(string path);

    /// <summary>Opens the containing folder and selects the file where the file manager supports it.</summary>
    bool RevealInFileManager(string path);
}

public enum ShellMenuTarget
{
    /// <summary>Any file and any folder.</summary>
    FilesAndFolders,
    /// <summary>Image files only.</summary>
    Images
}

/// <summary>A context menu entry added to the file manager.</summary>
/// <param name="Id">Stable identifier, for example "ShareX" or "ShareXImageEditor".</param>
/// <param name="Label">Text shown in the menu, for example "Upload with ShareX".</param>
/// <param name="ExecutablePath">Absolute path of the program to run.</param>
/// <param name="Arguments">Arguments placed before the selected file path.</param>
public sealed record ShellMenuEntry(string Id, string Label, string ExecutablePath, IReadOnlyList<string> Arguments, ShellMenuTarget Target)
{
    /// <summary>Icon path or theme icon name. Windows uses "executable,index".</summary>
    public string? Icon { get; init; }
}

/// <summary>File manager integration: Explorer on Windows, Finder Services on macOS, Nautilus, Dolphin, Nemo and Thunar on Linux.</summary>
public interface IShellIntegrationService
{
    FeatureSupport Support { get; }

    bool IsRegistered(ShellMenuEntry entry);

    void Register(ShellMenuEntry entry);

    void Unregister(ShellMenuEntry entry);
}
