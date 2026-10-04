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

using ShareX.Platform.Diagnostics;
using ShareX.Platform.MacOS.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;

namespace ShareX.Platform.MacOS;

/// <summary>
/// Finder integration on macOS: Quick Actions (Automator services in ~/Library/Services) for the context menu entries, Launch
/// Services for the default application of ShareX's file types, and browser extension hosts.
/// </summary>
public sealed class MacShellIntegrationService : IShellIntegrationService
{
    private static readonly string[] ChromiumFolders = ["Google/Chrome", "Google/Chrome Beta", "Chromium", "BraveSoftware/Brave-Browser",
        "Microsoft Edge", "Vivaldi"];

    private readonly string applicationSupport;
    private readonly string servicesFolder;
    private readonly ICommandRunner? runner;

    public MacShellIntegrationService(ICommandRunner? runner = null)
        : this(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library"), runner)
    {
    }

    /// <param name="library">The user's Library folder; tests pass a temporary one.</param>
    public MacShellIntegrationService(string library, ICommandRunner? runner = null)
    {
        applicationSupport = Path.Combine(library, "Application Support");
        servicesFolder = Path.Combine(library, "Services");
        this.runner = runner;
    }

    public FeatureSupport Support => FeatureSupport.Supported;

    internal string GetQuickActionPath(ShellMenuEntry entry) => Path.Combine(servicesFolder, FinderQuickAction.GetBundleName(entry));

    public bool IsRegistered(ShellMenuEntry entry)
    {
        string document = Path.Combine(GetQuickActionPath(entry), "Contents", "document.wflow");
        return File.Exists(document) && File.ReadAllText(document).Contains(SecurityElement.Escape(FinderQuickAction.CreateCommand(entry)), StringComparison.Ordinal);
    }

    public void Register(ShellMenuEntry entry)
    {
        string contents = Path.Combine(GetQuickActionPath(entry), "Contents");
        Directory.CreateDirectory(contents);
        File.WriteAllText(Path.Combine(contents, "Info.plist"), FinderQuickAction.CreateInfoPlist(entry), new UTF8Encoding(false));
        File.WriteAllText(Path.Combine(contents, "document.wflow"), FinderQuickAction.CreateDocument(entry), new UTF8Encoding(false));
        RefreshServices();
    }

    public void Unregister(ShellMenuEntry entry)
    {
        string path = GetQuickActionPath(entry);

        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
            RefreshServices();
        }
    }

    /// <summary>Asks the pasteboard server to reread the services, so Finder shows the change without logging out.</summary>
    private void RefreshServices()
    {
        try
        {
            runner?.RunAsync("/System/Library/CoreServices/pbs", ["-update"], timeout: TimeSpan.FromSeconds(10)).GetAwaiter().GetResult();
        }
        catch (Exception e) when (e is TimeoutException or System.ComponentModel.Win32Exception)
        {
        }
    }

    /// <summary>ShareX.app declares its file types; Launch Services can then make it their default application.</summary>
    public FeatureSupport FileAssociationSupport => LaunchServices.MainBundleIdentifier != null
        ? FeatureSupport.Supported
        : FeatureSupport.NotSupported("File types can only be associated when ShareX runs from ShareX.app.");

    public bool IsAssociated(FileAssociation association)
    {
        string? bundleId = LaunchServices.MainBundleIdentifier;
        string? type = LaunchServices.GetTypeForExtension(association.Extension.TrimStart('.'));
        return bundleId != null && type != null &&
            string.Equals(LaunchServices.GetDefaultHandler(type), bundleId, StringComparison.OrdinalIgnoreCase);
    }

    public void Associate(FileAssociation association)
    {
        string bundleId = LaunchServices.MainBundleIdentifier ?? throw new PlatformNotSupportedException(FileAssociationSupport.Reason);
        LaunchServices.RegisterMainBundle();
        string type = LaunchServices.GetTypeForExtension(association.Extension.TrimStart('.')) ??
            throw new InvalidOperationException($"macOS has no type for {association.Extension}.");

        if (!LaunchServices.SetDefaultHandler(type, bundleId))
        {
            throw new InvalidOperationException($"macOS did not make ShareX the default application for {association.Extension}.");
        }
    }

    // Launch Services keeps a default application per type and has no way to clear it; ShareX stays one of the type's applications
    // as long as ShareX.app declares it, and the user picks another with Get Info > Open with.
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
