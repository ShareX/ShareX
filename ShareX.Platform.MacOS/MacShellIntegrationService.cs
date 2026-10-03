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
using System.IO;
using System.Linq;
using System.Text;

namespace ShareX.Platform.MacOS;

/// <summary>
/// Browser extension hosts on macOS. Context menu entries need a Finder extension and file types are declared in the app
/// bundle's Info.plist, so those are not available to ShareX at run time.
/// </summary>
public sealed class MacShellIntegrationService : IShellIntegrationService
{
    private static readonly string[] ChromiumFolders = ["Google/Chrome", "Google/Chrome Beta", "Chromium", "BraveSoftware/Brave-Browser",
        "Microsoft Edge", "Vivaldi"];

    private readonly string applicationSupport;

    public MacShellIntegrationService()
        : this(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support"))
    {
    }

    public MacShellIntegrationService(string applicationSupport)
    {
        this.applicationSupport = applicationSupport;
    }

    public FeatureSupport Support { get; } =
        FeatureSupport.NotSupported("Finder context menu entries need a Finder extension, which ShareX does not have yet.");

    public bool IsRegistered(ShellMenuEntry entry) => false;

    public void Register(ShellMenuEntry entry) => throw new PlatformNotSupportedException(Support.Reason);

    public void Unregister(ShellMenuEntry entry)
    {
    }

    public FeatureSupport FileAssociationSupport { get; } =
        FeatureSupport.NotSupported("On macOS the file types an application opens are declared in its app bundle.");

    public bool IsAssociated(FileAssociation association) => false;

    public void Associate(FileAssociation association) => throw new PlatformNotSupportedException(FileAssociationSupport.Reason);

    public void RemoveAssociation(FileAssociation association)
    {
    }

    public FeatureSupport BrowserHostSupport => FeatureSupport.Supported;

    internal IReadOnlyList<string> GetBrowserHostManifestPaths(BrowserHost host)
    {
        IEnumerable<string> folders = host.Browser == BrowserFamily.Firefox
            ? new[] { Path.Combine(applicationSupport, "Mozilla") }
            : ChromiumFolders.Select(folder => Path.Combine(applicationSupport, folder)).Where(Directory.Exists).DefaultIfEmpty(Path.Combine(applicationSupport, "Google", "Chrome"));

        return folders.Select(folder => Path.Combine(folder, "NativeMessagingHosts", host.Name + ".json")).ToList();
    }

    public bool IsBrowserHostRegistered(BrowserHost host) =>
        GetBrowserHostManifestPaths(host).Any(path => NativeMessagingManifest.PointsAt(path, host));

    public void RegisterBrowserHost(BrowserHost host)
    {
        string manifest = NativeMessagingManifest.Create(host);

        foreach (string path in GetBrowserHostManifestPaths(host))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, manifest, new UTF8Encoding(false));
        }
    }

    public void UnregisterBrowserHost(BrowserHost host)
    {
        foreach (string path in GetBrowserHostManifestPaths(host).Where(File.Exists))
        {
            File.Delete(path);
        }
    }

    public FeatureSupport SendToSupport { get; } = FeatureSupport.NotSupported("The Send to menu is part of Windows Explorer.");

    public bool IsInSendTo(string name, string executablePath) => false;

    public void SetInSendTo(string name, string executablePath, bool enabled)
    {
    }
}
