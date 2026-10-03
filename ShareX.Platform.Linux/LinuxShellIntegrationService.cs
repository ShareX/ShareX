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
using System.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ShareX.Platform.Linux;

/// <summary>
/// File manager context menu entries for Nautilus (GNOME), Dolphin (KDE Plasma), Nemo (Cinnamon), Caja (MATE) and Thunar (Xfce).
/// </summary>
/// <remarks>Entries are written for every supported file manager so switching desktops keeps them working.</remarks>
public sealed class LinuxShellIntegrationService : IShellIntegrationService
{
    private static readonly string[] ImageMimeTypes = ["image/png", "image/jpeg", "image/gif", "image/bmp", "image/webp", "image/tiff", "image/avif"];
    private static readonly string[] ImageExtensions = ["png", "jpg", "jpeg", "gif", "bmp", "webp", "tif", "tiff", "avif"];

    // Chromium based browsers read native messaging manifests from <config>/<browser>/NativeMessagingHosts.
    private static readonly string[] ChromiumConfigFolders = ["google-chrome", "google-chrome-beta", "google-chrome-unstable", "chromium",
        "BraveSoftware/Brave-Browser", "microsoft-edge", "vivaldi"];

    // Firefox and its forks read them from ~/.<browser>/native-messaging-hosts.
    private static readonly string[] FirefoxHomeFolders = [".mozilla", ".librewolf", ".waterfox"];

    private readonly string dataHome;
    private readonly string configHome;
    private readonly string homeDirectory;
    private readonly ICommandRunner runner;

    public LinuxShellIntegrationService(XdgPathService paths, ICommandRunner runner)
        : this(paths.DataHome, paths.ConfigHome, Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), runner)
    {
    }

    public LinuxShellIntegrationService(string dataHome, string configHome)
        : this(dataHome, configHome, Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), CommandRunner.Default)
    {
    }

    public LinuxShellIntegrationService(string dataHome, string configHome, string homeDirectory, ICommandRunner runner)
    {
        this.dataHome = dataHome;
        this.configHome = configHome;
        this.homeDirectory = homeDirectory;
        this.runner = runner;
    }

    public FeatureSupport Support => FeatureSupport.Supported;

    internal string NautilusScriptPath(ShellMenuEntry entry) => Path.Combine(dataHome, "nautilus", "scripts", SafeFileName(entry.Label));

    internal string CajaScriptPath(ShellMenuEntry entry) => Path.Combine(configHome, "caja", "scripts", SafeFileName(entry.Label));

    internal string DolphinServiceMenuPath(ShellMenuEntry entry) => Path.Combine(dataHome, "kio", "servicemenus", entry.Id + ".desktop");

    internal string NemoActionPath(ShellMenuEntry entry) => Path.Combine(dataHome, "nemo", "actions", entry.Id + ".nemo_action");

    internal string ThunarActionsPath => Path.Combine(configHome, "Thunar", "uca.xml");

    public bool IsRegistered(ShellMenuEntry entry)
    {
        string expectedExec = DesktopEntry.BuildExec(entry.ExecutablePath, entry.Arguments, appendFileCode: true);
        string path = DolphinServiceMenuPath(entry);

        if (File.Exists(path) && File.ReadAllText(path).Contains("Exec=" + expectedExec, StringComparison.Ordinal))
        {
            return true;
        }

        path = NautilusScriptPath(entry);
        return File.Exists(path) && File.ReadAllText(path).Contains(ShellQuote(entry.ExecutablePath), StringComparison.Ordinal);
    }

    public void Register(ShellMenuEntry entry)
    {
        Unregister(entry);

        string script = CreateScript(entry);
        WriteExecutable(NautilusScriptPath(entry), script);
        WriteExecutable(CajaScriptPath(entry), script);
        // Plasma 5.24+ ignores service menus that are not executable.
        WriteExecutable(DolphinServiceMenuPath(entry), CreateDolphinServiceMenu(entry));
        WriteFile(NemoActionPath(entry), CreateNemoAction(entry));
        UpdateThunarActions(entry, register: true);
    }

    public void Unregister(ShellMenuEntry entry)
    {
        foreach (string path in new[] { NautilusScriptPath(entry), CajaScriptPath(entry), DolphinServiceMenuPath(entry), NemoActionPath(entry) })
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        UpdateThunarActions(entry, register: false);
    }

    internal static string CreateScript(ShellMenuEntry entry)
    {
        // Nautilus and Caja pass the selected files as arguments. Images only entries still show for every file, so filter here.
        StringBuilder builder = new StringBuilder();
        builder.Append("#!/bin/sh\n");
        builder.Append("# Generated by ShareX. ").Append(entry.Id).Append('\n');

        if (entry.Target == ShellMenuTarget.Images)
        {
            builder.Append("for file in \"$@\"; do\n");
            builder.Append("  case \"$(printf '%s' \"$file\" | tr '[:upper:]' '[:lower:]')\" in\n");
            builder.Append("    ").Append(string.Join("|", ImageExtensions.Select(e => "*." + e))).Append(") ;;\n");
            builder.Append("    *) exit 0 ;;\n");
            builder.Append("  esac\n");
            builder.Append("done\n");
        }

        builder.Append("exec ").Append(ShellQuote(entry.ExecutablePath));

        foreach (string argument in entry.Arguments)
        {
            builder.Append(' ').Append(ShellQuote(argument));
        }

        builder.Append(" \"$@\"\n");
        return builder.ToString();
    }

    internal static string CreateDolphinServiceMenu(ShellMenuEntry entry)
    {
        string mimeTypes = entry.Target == ShellMenuTarget.Images ? string.Join(";", ImageMimeTypes) + ";" : "all/allfiles;inode/directory;";
        string action = "sharex_" + SafeIdentifier(entry.Id);

        StringBuilder builder = new StringBuilder();
        builder.Append("[Desktop Entry]\n");
        builder.Append("Type=Service\n");
        builder.Append("X-KDE-ServiceTypes=KonqPopupMenu/Plugin\n");
        builder.Append("MimeType=").Append(mimeTypes).Append('\n');
        builder.Append("Actions=").Append(action).Append('\n');
        builder.Append("X-KDE-Priority=TopLevel\n\n");
        builder.Append("[Desktop Action ").Append(action).Append("]\n");
        builder.Append("Name=").Append(DesktopEntry.EscapeValue(entry.Label)).Append('\n');

        if (!string.IsNullOrEmpty(entry.Icon))
        {
            builder.Append("Icon=").Append(DesktopEntry.EscapeValue(entry.Icon)).Append('\n');
        }

        builder.Append("Exec=").Append(DesktopEntry.BuildExec(entry.ExecutablePath, entry.Arguments, appendFileCode: true)).Append('\n');
        return builder.ToString();
    }

    internal static string CreateNemoAction(ShellMenuEntry entry)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("[Nemo Action]\n");
        builder.Append("Active=true\n");
        builder.Append("Name=").Append(entry.Label.Replace("_", "__")).Append('\n');
        builder.Append("Comment=").Append(entry.Label).Append('\n');
        builder.Append("Exec=").Append(DesktopEntry.BuildExec(entry.ExecutablePath, entry.Arguments, appendFileCode: true)).Append('\n');

        if (!string.IsNullOrEmpty(entry.Icon))
        {
            builder.Append("Icon-Name=").Append(entry.Icon).Append('\n');
        }

        builder.Append("Selection=NotNone\n");
        builder.Append(entry.Target == ShellMenuTarget.Images
            ? "Extensions=" + string.Join(";", ImageExtensions) + ";\n"
            : "Extensions=any;dir;\n");
        return builder.ToString();
    }

    private void UpdateThunarActions(ShellMenuEntry entry, bool register)
    {
        string path = ThunarActionsPath;

        if (!register && !File.Exists(path))
        {
            return;
        }

        XDocument document;

        try
        {
            document = File.Exists(path) ? XDocument.Load(path) : new XDocument(new XElement("actions"));
        }
        catch (System.Xml.XmlException)
        {
            // Leave a file we cannot parse alone rather than overwrite the user's actions.
            return;
        }

        XElement root = document.Root ?? new XElement("actions");
        string uniqueId = "sharex-" + entry.Id;

        foreach (XElement existing in root.Elements("action").Where(a => (string?)a.Element("unique-id") == uniqueId).ToList())
        {
            existing.Remove();
        }

        if (register)
        {
            string command = string.Join(" ", new[] { entry.ExecutablePath }.Concat(entry.Arguments).Select(ShellQuote)) + " %F";
            XElement action = new XElement("action",
                new XElement("icon", entry.Icon ?? ""),
                new XElement("name", entry.Label),
                new XElement("submenu", ""),
                new XElement("unique-id", uniqueId),
                new XElement("command", command),
                new XElement("description", entry.Label),
                new XElement("range", ""),
                new XElement("patterns", entry.Target == ShellMenuTarget.Images ? string.Join(";", ImageExtensions.Select(e => "*." + e)) : "*"));

            if (entry.Target == ShellMenuTarget.Images)
            {
                action.Add(new XElement("image-files"));
            }
            else
            {
                action.Add(new XElement("directories"), new XElement("audio-files"), new XElement("image-files"),
                    new XElement("other-files"), new XElement("text-files"), new XElement("video-files"));
            }

            root.Add(action);
        }

        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        document.Save(path);
    }

    /// <summary>Quotes an argument for POSIX sh.</summary>
    internal static string ShellQuote(string value) => "'" + value.Replace("'", "'\\''") + "'";

    public FeatureSupport FileAssociationSupport => FeatureSupport.Supported;

    internal string MimePackagePath(FileAssociation association) => Path.Combine(dataHome, "mime", "packages", AssociationId(association) + ".xml");

    internal string AssociationDesktopPath(FileAssociation association) => Path.Combine(dataHome, "applications", AssociationId(association) + ".desktop");

    private static string AssociationId(FileAssociation association) => "sharex-" + SafeIdentifier(association.TypeId);

    /// <summary>Exec for one file: the program, its arguments and %f.</summary>
    internal static string AssociationExec(FileAssociation association) =>
        DesktopEntry.BuildExec(association.ExecutablePath, association.Arguments) + " %f";

    public bool IsAssociated(FileAssociation association)
    {
        string desktopPath = AssociationDesktopPath(association);
        return File.Exists(MimePackagePath(association)) && File.Exists(desktopPath) &&
            File.ReadAllText(desktopPath).Contains("Exec=" + AssociationExec(association), StringComparison.Ordinal);
    }

    /// <summary>
    /// Declares the MIME type for the extension (shared-mime-info package), adds a hidden desktop entry that opens it, and makes
    /// that entry the default application for the type.
    /// </summary>
    public void Associate(FileAssociation association)
    {
        WriteFile(MimePackagePath(association), CreateMimePackage(association));
        WriteFile(AssociationDesktopPath(association), CreateAssociationDesktopEntry(association));
        RefreshAssociations(association, setDefault: true);
    }

    public void RemoveAssociation(FileAssociation association)
    {
        foreach (string path in new[] { MimePackagePath(association), AssociationDesktopPath(association) })
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        RefreshAssociations(association, setDefault: false);
    }

    internal static string CreateMimePackage(FileAssociation association)
    {
        string glob = "*" + association.Extension;
        return $"""
            <?xml version="1.0" encoding="UTF-8"?>
            <!-- Generated by ShareX. -->
            <mime-info xmlns="http://www.freedesktop.org/standards/shared-mime-info">
              <mime-type type="{SecurityElement.Escape(association.MimeType)}">
                <comment>{SecurityElement.Escape(association.Description)}</comment>
                <glob pattern="{SecurityElement.Escape(glob)}"/>
              </mime-type>
            </mime-info>

            """;
    }

    internal static string CreateAssociationDesktopEntry(FileAssociation association)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("[Desktop Entry]\n");
        builder.Append("Type=Application\n");
        builder.Append("Name=ShareX\n");
        builder.Append("Comment=").Append(DesktopEntry.EscapeValue(association.Description)).Append('\n');
        builder.Append("Exec=").Append(AssociationExec(association)).Append('\n');
        builder.Append("MimeType=").Append(association.MimeType).Append(";\n");
        builder.Append("Icon=").Append(association.Icon != null && File.Exists(association.Icon) ? association.Icon : "sharex").Append('\n');
        // Only for opening files of this type; the application launcher has its own entry.
        builder.Append("NoDisplay=true\n");
        return builder.ToString();
    }

    private void RefreshAssociations(FileAssociation association, bool setDefault)
    {
        // Each tool is optional: without it the files still exist and take effect the next time the desktop rebuilds its caches.
        Run("update-mime-database", [Path.Combine(dataHome, "mime")]);
        Run("update-desktop-database", [Path.Combine(dataHome, "applications")]);

        if (setDefault)
        {
            Run("xdg-mime", ["default", AssociationId(association) + ".desktop", association.MimeType]);
        }
    }

    private void Run(string command, IReadOnlyList<string> arguments)
    {
        if (!runner.Exists(command))
        {
            return;
        }

        try
        {
            runner.RunAsync(command, arguments, timeout: TimeSpan.FromSeconds(15)).GetAwaiter().GetResult();
        }
        catch (Exception e) when (e is TimeoutException or System.ComponentModel.Win32Exception or InvalidOperationException)
        {
        }
    }

    public FeatureSupport BrowserHostSupport => FeatureSupport.Supported;

    /// <summary>Manifest locations for the browsers installed for this user, or the most common ones when none is installed yet.</summary>
    internal IReadOnlyList<string> GetBrowserHostManifestPaths(BrowserHost host)
    {
        string fileName = host.Name + ".json";
        IEnumerable<string> folders = host.Browser == BrowserFamily.Firefox
            ? FirefoxHomeFolders.Select(folder => Path.Combine(homeDirectory, folder))
            : ChromiumConfigFolders.Select(folder => Path.Combine(configHome, folder));
        List<string> installed = folders.Where(Directory.Exists).ToList();

        if (installed.Count == 0)
        {
            installed.Add(host.Browser == BrowserFamily.Firefox ? Path.Combine(homeDirectory, ".mozilla") : Path.Combine(configHome, "google-chrome"));
        }

        string hostsFolder = host.Browser == BrowserFamily.Firefox ? "native-messaging-hosts" : "NativeMessagingHosts";
        return installed.Select(folder => Path.Combine(folder, hostsFolder, fileName)).ToList();
    }

    public bool IsBrowserHostRegistered(BrowserHost host) =>
        GetBrowserHostManifestPaths(host).Any(path => NativeMessagingManifest.PointsAt(path, host));

    public void RegisterBrowserHost(BrowserHost host)
    {
        string manifest = NativeMessagingManifest.Create(host);

        foreach (string path in GetBrowserHostManifestPaths(host))
        {
            WriteFile(path, manifest);
        }
    }

    public void UnregisterBrowserHost(BrowserHost host)
    {
        foreach (string path in GetBrowserHostManifestPaths(host))
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    public FeatureSupport SendToSupport { get; } =
        FeatureSupport.NotSupported("The Send to menu is part of Windows Explorer. Use the file manager's context menu entries instead.");

    public bool IsInSendTo(string name, string executablePath) => false;

    public void SetInSendTo(string name, string executablePath, bool enabled)
    {
    }

    private static string SafeFileName(string label)
    {
        char[] invalid = ['/', '\0'];
        return new string(label.Select(c => invalid.Contains(c) ? '_' : c).ToArray());
    }

    private static string SafeIdentifier(string id) => new string(id.Select(c => char.IsLetterOrDigit(c) ? char.ToLowerInvariant(c) : '_').ToArray());

    private static void WriteFile(string path, string content)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content, new UTF8Encoding(false));
    }

    private static void WriteExecutable(string path, string content)
    {
        WriteFile(path, content);

        if (!OperatingSystem.IsWindows())
        {
            File.SetUnixFileMode(path, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute |
                UnixFileMode.GroupRead | UnixFileMode.GroupExecute | UnixFileMode.OtherRead | UnixFileMode.OtherExecute);
        }
    }
}
