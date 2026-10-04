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

using Microsoft.Win32;
using ShareX.Platform.Windows.Native;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace ShareX.Platform.Windows;

/// <summary>Shell execution and Explorer selection.</summary>
public sealed class WindowsShellService : IShellService
{
    public bool OpenUrl(string url) => Start(url);

    public bool OpenPath(string path) => Start(path);

    // Explorer's desktop list view, as in v22 (W13: J to confirm or report a reason when Explorer is not the shell).
    public FeatureSupport DesktopIconsSupport => FeatureSupport.Supported;

    public bool? AreDesktopIconsVisible()
    {
        IntPtr icons = GetDesktopListView();
        return icons != IntPtr.Zero && Win32.IsWindowVisible(icons);
    }

    public bool SetDesktopIconsVisible(bool visible)
    {
        IntPtr icons = GetDesktopListView();

        if (icons == IntPtr.Zero)
        {
            return false;
        }

        // ShowWindow returns the previous visibility, not success.
        Win32.ShowWindow(icons, visible ? Win32.SW_SHOW : Win32.SW_HIDE);
        return true;
    }

    /// <summary>The desktop's icon list view: under Progman, or under a WorkerW window once a wallpaper slideshow has run.</summary>
    private static IntPtr GetDesktopListView()
    {
        IntPtr progman = Win32.FindWindow("Progman", null);
        IntPtr defView = Win32.FindWindowEx(progman, IntPtr.Zero, "SHELLDLL_DefView", null);

        if (defView == IntPtr.Zero)
        {
            IntPtr worker = IntPtr.Zero;

            while ((worker = Win32.FindWindowEx(IntPtr.Zero, worker, "WorkerW", null)) != IntPtr.Zero)
            {
                defView = Win32.FindWindowEx(worker, IntPtr.Zero, "SHELLDLL_DefView", null);

                if (defView != IntPtr.Zero)
                {
                    break;
                }
            }
        }

        return defView != IntPtr.Zero ? Win32.FindWindowEx(defView, IntPtr.Zero, "SysListView32", "FolderView") : IntPtr.Zero;
    }

    /// <summary>
    /// Finds an installed program the way ShareX always has: the shell's application registrations
    /// (HKCR\Applications\{name}\shell\open|edit\command), then the programs the user has run (the MuiCache).
    /// </summary>
    public string? FindProgram(string executableName)
    {
        foreach (string command in new[] { "open", "edit" })
        {
            if (Registry.GetValue($@"HKEY_CLASSES_ROOT\Applications\{executableName}\shell\{command}\command", null, null) is string value &&
                ParseQuoted(value) is string path && System.IO.File.Exists(path))
            {
                return path;
            }
        }

        using RegistryKey? programs = Registry.CurrentUser.OpenSubKey(@"Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\MuiCache");

        foreach (string name in programs?.GetValueNames() ?? Array.Empty<string>())
        {
            string programPath = name;

            foreach (string suffix in new[] { ".ApplicationCompany", ".FriendlyAppName" })
            {
                if (programPath.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                {
                    programPath = programPath[..^suffix.Length];
                }
            }

            if (programPath.EndsWith(executableName, StringComparison.OrdinalIgnoreCase) && System.IO.File.Exists(programPath))
            {
                return programPath;
            }
        }

        return null;
    }

    /// <summary>The text between the first pair of quotes, or the whole trimmed text when it is not quoted.</summary>
    internal static string ParseQuoted(string text)
    {
        text = text.Trim();
        int first = text.IndexOf('"');

        if (first >= 0)
        {
            text = text[(first + 1)..];
            int second = text.IndexOf('"');

            if (second >= 0)
            {
                text = text[..second];
            }
        }

        return text;
    }

    public string? GetMimeType(string extension)
    {
        using RegistryKey? key = Registry.ClassesRoot.OpenSubKey(extension);
        return key?.GetValue("Content Type") as string;
    }

    public bool RevealInFileManager(string path)
    {
        // The same call ShareX.HelpersLib uses. It needs COM on the calling thread.
        Win32.CoInitializeEx(IntPtr.Zero, Win32.COINIT_APARTMENTTHREADED);
        IntPtr pidl = Win32.ILCreateFromPathW(path);

        if (pidl == IntPtr.Zero)
        {
            return false;
        }

        try
        {
            return Win32.SHOpenFolderAndSelectItems(pidl, 0, IntPtr.Zero, 0) == 0;
        }
        finally
        {
            Win32.ILFree(pidl);
        }
    }

    private static bool Start(string target)
    {
        try
        {
            using Process? process = Process.Start(new ProcessStartInfo(target) { UseShellExecute = true });
            return true;
        }
        catch (Win32Exception)
        {
            return false;
        }
    }
}

/// <summary>Explorer context menu entries under HKCU\Software\Classes, identical to the keys ShareX has always written.</summary>
public sealed class WindowsShellIntegrationService : IShellIntegrationService
{
    private readonly RegistryKey registryRoot;
    private readonly string sendToFolder;

    public WindowsShellIntegrationService()
        : this(Registry.CurrentUser, Environment.GetFolderPath(Environment.SpecialFolder.SendTo))
    {
    }

    internal WindowsShellIntegrationService(RegistryKey registryRoot, string sendToFolder)
    {
        this.registryRoot = registryRoot;
        this.sendToFolder = sendToFolder;
    }

    public FeatureSupport Support => FeatureSupport.Supported;

    public static string[] GetMenuKeys(ShellMenuEntry entry) => entry.Target switch
    {
        ShellMenuTarget.Images => [$@"Software\Classes\SystemFileAssociations\image\shell\{entry.Id}"],
        _ => [$@"Software\Classes\*\shell\{entry.Id}", $@"Software\Classes\Directory\shell\{entry.Id}"]
    };

    /// <summary>For example "C:\Program Files\ShareX\ShareX.exe" -ImageEditor "%1".</summary>
    public static string GetCommand(ShellMenuEntry entry) =>
        string.Join(" ", new[] { $"\"{entry.ExecutablePath}\"" }.Concat(entry.Arguments.Select(WindowsStartupService.QuoteArgument)).Append("\"%1\""));

    public static string GetIcon(ShellMenuEntry entry) => entry.Icon ?? $"\"{entry.ExecutablePath}\",0";

    public bool IsRegistered(ShellMenuEntry entry) => GetMenuKeys(entry).All(key =>
    {
        using RegistryKey? command = registryRoot.OpenSubKey(key + @"\command");
        return command?.GetValue(null) is string value && value.Equals(GetCommand(entry), StringComparison.OrdinalIgnoreCase);
    });

    public void Register(ShellMenuEntry entry)
    {
        Unregister(entry);

        foreach (string key in GetMenuKeys(entry))
        {
            using (RegistryKey menu = registryRoot.CreateSubKey(key))
            {
                menu.SetValue(null, entry.Label, RegistryValueKind.String);
                menu.SetValue("Icon", GetIcon(entry), RegistryValueKind.String);
            }

            using (RegistryKey command = registryRoot.CreateSubKey(key + @"\command"))
            {
                command.SetValue(null, GetCommand(entry), RegistryValueKind.String);
            }
        }
    }

    public void Unregister(ShellMenuEntry entry)
    {
        foreach (string key in GetMenuKeys(entry))
        {
            registryRoot.DeleteSubKeyTree(key, false);
        }
    }

    public FeatureSupport FileAssociationSupport => FeatureSupport.Supported;

    /// <summary>For example "C:\Program Files\ShareX\ShareX.exe" -CustomUploader "%1".</summary>
    public static string GetCommand(FileAssociation association) =>
        string.Join(" ", new[] { $"\"{association.ExecutablePath}\"" }.Concat(association.Arguments.Select(WindowsStartupService.QuoteArgument)).Append("\"%1\""));

    private static string ExtensionKey(FileAssociation association) => $@"Software\Classes\{association.Extension}";

    private static string TypeKey(FileAssociation association) => $@"Software\Classes\{association.TypeId}";

    public bool IsAssociated(FileAssociation association)
    {
        using RegistryKey? extension = registryRoot.OpenSubKey(ExtensionKey(association));
        using RegistryKey? command = registryRoot.OpenSubKey(TypeKey(association) + @"\shell\open\command");
        return extension?.GetValue(null) is string typeId && typeId.Equals(association.TypeId, StringComparison.OrdinalIgnoreCase) &&
            command?.GetValue(null) is string value && value.Equals(GetCommand(association), StringComparison.OrdinalIgnoreCase);
    }

    public void Associate(FileAssociation association)
    {
        RemoveAssociation(association, notify: false);

        using (RegistryKey key = registryRoot.CreateSubKey(ExtensionKey(association)))
        {
            key.SetValue(null, association.TypeId, RegistryValueKind.String);
        }

        using (RegistryKey key = registryRoot.CreateSubKey(TypeKey(association)))
        {
            key.SetValue(null, association.Description, RegistryValueKind.String);
        }

        if (association.Icon != null)
        {
            using RegistryKey key = registryRoot.CreateSubKey(TypeKey(association) + @"\DefaultIcon");
            key.SetValue(null, $"\"{association.Icon}\"", RegistryValueKind.String);
        }

        using (RegistryKey key = registryRoot.CreateSubKey(TypeKey(association) + @"\shell\open\command"))
        {
            key.SetValue(null, GetCommand(association), RegistryValueKind.String);
        }

        NotifyAssociationsChanged();
    }

    public void RemoveAssociation(FileAssociation association) => RemoveAssociation(association, notify: true);

    private void RemoveAssociation(FileAssociation association, bool notify)
    {
        registryRoot.DeleteSubKeyTree(ExtensionKey(association), false);
        registryRoot.DeleteSubKeyTree(TypeKey(association), false);

        if (notify)
        {
            NotifyAssociationsChanged();
        }
    }

    /// <summary>Tells Explorer to refresh file type icons and verbs (SHCNE_ASSOCCHANGED, SHCNF_FLUSH).</summary>
    private static void NotifyAssociationsChanged() => Win32.SHChangeNotify(0x08000000, 0x1000, IntPtr.Zero, IntPtr.Zero);

    public FeatureSupport BrowserHostSupport => FeatureSupport.Supported;

    private static string BrowserHostKey(BrowserHost host) => host.Browser == BrowserFamily.Firefox
        ? $@"SOFTWARE\Mozilla\NativeMessagingHosts\{host.Name}"
        : $@"SOFTWARE\Google\Chrome\NativeMessagingHosts\{host.Name}";

    /// <summary>Windows browsers read the manifest path from the registry; the manifest stays where ShareX installed it.</summary>
    public bool IsBrowserHostRegistered(BrowserHost host)
    {
        using RegistryKey? key = registryRoot.OpenSubKey(BrowserHostKey(host));
        return key?.GetValue(null) is string path && path.Equals(host.ManifestPath, StringComparison.OrdinalIgnoreCase) && System.IO.File.Exists(host.ManifestPath);
    }

    public void RegisterBrowserHost(BrowserHost host)
    {
        UnregisterBrowserHost(host);
        using RegistryKey key = registryRoot.CreateSubKey(BrowserHostKey(host));
        key.SetValue(null, host.ManifestPath, RegistryValueKind.String);
    }

    public void UnregisterBrowserHost(BrowserHost host) => registryRoot.DeleteSubKeyTree(BrowserHostKey(host), false);

    public FeatureSupport SendToSupport => FeatureSupport.Supported;

    private string SendToShortcutPath(string name) => System.IO.Path.Combine(sendToFolder,
        name.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase) ? name : name + ".lnk");

    public bool IsInSendTo(string name, string executablePath) => WindowsStartupService.IsShortcutTo(SendToShortcutPath(name), executablePath);

    public void SetInSendTo(string name, string executablePath, bool enabled)
    {
        string shortcutPath = SendToShortcutPath(name);

        // As before: an existing shortcut stays when there is nothing to point it at.
        if (enabled && !System.IO.File.Exists(executablePath))
        {
            return;
        }

        if (System.IO.File.Exists(shortcutPath))
        {
            System.IO.File.Delete(shortcutPath);
        }

        if (enabled)
        {
            IWshShortcut shortcut = ((IWshShell)new WshShell()).CreateShortcut(shortcutPath);
            shortcut.TargetPath = executablePath;
            shortcut.Arguments = "";
            shortcut.WorkingDirectory = System.IO.Path.GetDirectoryName(executablePath) ?? "";
            shortcut.Save();
        }
    }
}
