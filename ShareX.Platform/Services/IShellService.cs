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

namespace ShareX.Platform;

/// <summary>Opening URLs and files, and showing files in Explorer, Finder or the Linux file manager.</summary>
public interface IShellService
{
    bool OpenUrl(string url);

    /// <summary>Opens a file or folder with its default application.</summary>
    bool OpenPath(string path);

    /// <summary>Opens the containing folder and selects the file where the file manager supports it.</summary>
    bool RevealInFileManager(string path);

    /// <summary>
    /// The MIME type the system associates with a file extension such as ".png", or null. Windows reads the registry,
    /// Linux the shared-mime-info and mime.types databases. Callers keep their own table for the common types.
    /// </summary>
    string? GetMimeType(string extension);

    /// <summary>Whether the desktop shows its icons, or null where the platform does not say (only Windows does).</summary>
    bool? AreDesktopIconsVisible();

    /// <summary>Shows or hides the desktop icons, for clean full screen captures. Returns false where the platform cannot.</summary>
    bool SetDesktopIconsVisible(bool visible);

    /// <summary>
    /// The full path of an installed program by its executable name (for example "mspaint.exe" on Windows or "gimp" on Linux), or
    /// null. Windows asks the registry's application lists; Linux and macOS search PATH.
    /// </summary>
    string? FindProgram(string executableName);
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

/// <summary>A file type ShareX opens, such as custom uploaders (.sxcu) and image effect presets (.sxie).</summary>
/// <param name="Extension">With the dot, for example ".sxcu".</param>
/// <param name="TypeId">Stable identifier, for example "ShareX.sxcu" (the Windows ProgID).</param>
/// <param name="Description">Shown by the file manager, for example "ShareX custom uploader".</param>
/// <param name="MimeType">MIME type for Linux and macOS, for example "application/x-sharex-custom-uploader".</param>
/// <param name="Arguments">Arguments placed before the file path.</param>
public sealed record FileAssociation(string Extension, string TypeId, string Description, string MimeType, string ExecutablePath, IReadOnlyList<string> Arguments)
{
    /// <summary>Icon file. Windows uses it as DefaultIcon.</summary>
    public string? Icon { get; init; }
}

public enum BrowserFamily
{
    /// <summary>Chrome, Chromium, Edge, Brave, Vivaldi.</summary>
    Chromium,
    Firefox
}

/// <summary>A native messaging host that lets a browser extension talk to ShareX.</summary>
/// <param name="Name">The host name in the manifest, for example "com.getsharex.sharex".</param>
/// <param name="ManifestPath">The manifest shipped with ShareX. Windows registers it where it is; other platforms write a copy for each
/// browser with <paramref name="HostExecutablePath"/> as an absolute path.</param>
public sealed record BrowserHost(BrowserFamily Browser, string Name, string ManifestPath, string HostExecutablePath);

/// <summary>File manager integration: Explorer on Windows, Finder Services on macOS, Nautilus, Dolphin, Nemo and Thunar on Linux.</summary>
public interface IShellIntegrationService
{
    /// <summary>Context menu entries.</summary>
    FeatureSupport Support { get; }

    bool IsRegistered(ShellMenuEntry entry);

    void Register(ShellMenuEntry entry);

    void Unregister(ShellMenuEntry entry);

    FeatureSupport FileAssociationSupport { get; }

    bool IsAssociated(FileAssociation association);

    void Associate(FileAssociation association);

    void RemoveAssociation(FileAssociation association);

    FeatureSupport BrowserHostSupport { get; }

    bool IsBrowserHostRegistered(BrowserHost host);

    void RegisterBrowserHost(BrowserHost host);

    void UnregisterBrowserHost(BrowserHost host);

    /// <summary>The Explorer "Send to" menu. Windows only.</summary>
    FeatureSupport SendToSupport { get; }

    bool IsInSendTo(string name, string executablePath);

    void SetInSendTo(string name, string executablePath, bool enabled);
}
