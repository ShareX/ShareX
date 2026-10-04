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
using System.IO;
using System.Linq;
using System.Text;

namespace ShareX.Platform.Windows;

/// <summary>Launch at sign in with a shortcut in the user's Startup folder, exactly as ShareX has always done.</summary>
/// <remarks>The Microsoft Store build uses the package's StartupTask instead, which stays in the ShareX project.</remarks>
public sealed class WindowsStartupService : IStartupService
{
    private const string StartupApprovedKey = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\StartupFolder";

    private readonly string startupFolder;

    public WindowsStartupService()
        : this(Environment.GetFolderPath(Environment.SpecialFolder.Startup))
    {
    }

    public WindowsStartupService(string startupFolder)
    {
        this.startupFolder = startupFolder;
    }

    public FeatureSupport Support => FeatureSupport.Supported;

    // Startup folder shortcuts pass -silent. The packaged StartupTask activation check is W12 (J); until then the Store build
    // keeps its own check in Program.
    public bool WasStartedBySignIn => false;

    public string GetShortcutPath(StartupRegistration registration)
    {
        string name = registration.Name.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase) ? registration.Name : registration.Name + ".lnk";
        return Path.Combine(startupFolder, name);
    }

    public StartupRegistrationState GetState(StartupRegistration registration)
    {
        string shortcutPath = GetShortcutPath(registration);

        if (!IsShortcutTo(shortcutPath, registration.ExecutablePath))
        {
            return StartupRegistrationState.Disabled;
        }

        // Task Manager's Startup tab records its disabled state here without deleting the shortcut.
        if (Registry.GetValue(StartupApprovedKey, Path.GetFileName(shortcutPath), null) is byte[] status && status.Length > 0 && status[0] == 3)
        {
            return StartupRegistrationState.DisabledByUser;
        }

        return StartupRegistrationState.Enabled;
    }

    public void SetEnabled(StartupRegistration registration, bool enabled)
    {
        string shortcutPath = GetShortcutPath(registration);

        // Leave an existing shortcut alone when its requested target is missing, as in v22.
        if (enabled && !File.Exists(registration.ExecutablePath))
        {
            return;
        }

        if (File.Exists(shortcutPath))
        {
            File.Delete(shortcutPath);
        }

        if (enabled)
        {
            IWshShortcut shortcut = ((IWshShell)new WshShell()).CreateShortcut(shortcutPath);
            shortcut.TargetPath = registration.ExecutablePath;
            shortcut.Arguments = string.Join(" ", registration.Arguments.Select(QuoteArgument));
            shortcut.WorkingDirectory = Path.GetDirectoryName(registration.ExecutablePath) ?? "";
            shortcut.Save();
        }
    }

    internal static bool IsShortcutTo(string shortcutPath, string targetPath)
    {
        if (string.IsNullOrEmpty(targetPath) || !File.Exists(shortcutPath))
        {
            return false;
        }

        try
        {
            string shortcutTarget = ((IWshShell)new WshShell()).CreateShortcut(shortcutPath).TargetPath;
            return !string.IsNullOrEmpty(shortcutTarget) && shortcutTarget.Equals(targetPath, StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception e) when (e is System.Runtime.InteropServices.COMException or UnauthorizedAccessException or IOException)
        {
            return false;
        }
    }

    internal static string QuoteArgument(string argument)
    {
        if (argument.Length > 0 && argument.IndexOfAny([' ', '\t', '"']) < 0)
        {
            return argument;
        }

        StringBuilder quoted = new StringBuilder("\"");
        int backslashes = 0;

        foreach (char character in argument)
        {
            if (character == '\\')
            {
                backslashes++;
                continue;
            }

            quoted.Append('\\', character == '"' ? backslashes * 2 + 1 : backslashes);
            quoted.Append(character);
            backslashes = 0;
        }

        // Backslashes before the closing quote must be doubled so they do not escape it.
        quoted.Append('\\', backslashes * 2);
        quoted.Append('"');
        return quoted.ToString();
    }
}
