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
    public FeatureSupport Support => FeatureSupport.Supported;

    public static string[] GetMenuKeys(ShellMenuEntry entry) => entry.Target switch
    {
        ShellMenuTarget.Images => [$@"Software\Classes\SystemFileAssociations\image\shell\{entry.Id}"],
        _ => [$@"Software\Classes\*\shell\{entry.Id}", $@"Software\Classes\Directory\shell\{entry.Id}"]
    };

    /// <summary>For example "C:\Program Files\ShareX\ShareX.exe" -ImageEditor "%1".</summary>
    public static string GetCommand(ShellMenuEntry entry) =>
        string.Join(" ", new[] { $"\"{entry.ExecutablePath}\"" }.Concat(entry.Arguments).Append("\"%1\""));

    public static string GetIcon(ShellMenuEntry entry) => entry.Icon ?? $"\"{entry.ExecutablePath}\",0";

    public bool IsRegistered(ShellMenuEntry entry) => GetMenuKeys(entry).All(key =>
    {
        using RegistryKey? command = Registry.CurrentUser.OpenSubKey(key + @"\command");
        return command?.GetValue(null) is string value && value.Equals(GetCommand(entry), StringComparison.OrdinalIgnoreCase);
    });

    public void Register(ShellMenuEntry entry)
    {
        Unregister(entry);

        foreach (string key in GetMenuKeys(entry))
        {
            using (RegistryKey menu = Registry.CurrentUser.CreateSubKey(key))
            {
                menu.SetValue(null, entry.Label, RegistryValueKind.String);
                menu.SetValue("Icon", GetIcon(entry), RegistryValueKind.String);
            }

            using (RegistryKey command = Registry.CurrentUser.CreateSubKey(key + @"\command"))
            {
                command.SetValue(null, GetCommand(entry), RegistryValueKind.String);
            }
        }
    }

    public void Unregister(ShellMenuEntry entry)
    {
        foreach (string key in GetMenuKeys(entry))
        {
            Registry.CurrentUser.DeleteSubKeyTree(key, false);
        }
    }
}
