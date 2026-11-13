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
using ShareX.HelpersLib;
using System;

#if MicrosoftStore
using Windows.ApplicationModel;
#endif

namespace ShareX
{
    public static class StartupManager
    {
#if MicrosoftStore
        private const int StartupTargetIndex = 0;
        private static readonly StartupTask packageTask = StartupTask.GetForCurrentPackageAsync().GetAwaiter().GetResult()[StartupTargetIndex];
#endif

        public static string StartupTargetPath
        {
            get
            {
#if STEAM
                return FileHelpers.GetAbsolutePath("../ShareX_Launcher.exe");
#else
                if (!OperatingSystem.IsWindows())
                {
                    string dir = AppContext.BaseDirectory;
                    while (!string.IsNullOrEmpty(dir))
                    {
                        string runScript = System.IO.Path.Combine(dir, "run.sh");
                        if (System.IO.File.Exists(runScript))
                        {
                            return runScript;
                        }
                        System.IO.DirectoryInfo parent = System.IO.Directory.GetParent(dir);
                        dir = parent?.FullName;
                    }

                    string processPath = Environment.ProcessPath;
                    if (!string.IsNullOrEmpty(processPath) && System.IO.Path.GetFileNameWithoutExtension(processPath).Equals("dotnet", StringComparison.OrdinalIgnoreCase))
                    {
                        string entryDll = typeof(StartupManager).Assembly.Location;
                        if (string.IsNullOrEmpty(entryDll) || !System.IO.File.Exists(entryDll))
                        {
                            entryDll = System.IO.Path.Combine(AppContext.BaseDirectory, "ShareX.dll");
                        }
                        if (System.IO.File.Exists(entryDll))
                        {
                            return entryDll;
                        }
                    }
                }
                return Environment.ProcessPath;
#endif
            }
        }

        public static StartupState State
        {
            get
            {
#if MicrosoftStore
                return (StartupState)packageTask.State;
#else
                if (OperatingSystem.IsWindows())
                {
                    if (ShortcutHelpers.CheckShortcut(Environment.SpecialFolder.Startup, "ShareX", StartupTargetPath))
                    {
                        if (Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\StartupFolder",
                            "ShareX.lnk", null) is byte[] status && status.Length > 0 && status[0] == 3)
                        {
                            return StartupState.DisabledByUser;
                        }
                        else
                        {
                            return StartupState.Enabled;
                        }
                    }
                }
                else
                {
                    if (IsFlatpak)
                    {
                        return System.IO.File.Exists(FlatpakAutostartMarker) ? StartupState.Enabled : StartupState.Disabled;
                    }

                    string autostartDesktop = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "autostart", "ShareX.desktop");
                    if (System.IO.File.Exists(autostartDesktop) || ShortcutHelpers.CheckShortcut(Environment.SpecialFolder.Startup, "ShareX", StartupTargetPath))
                    {
                        return StartupState.Enabled;
                    }
                }

                return StartupState.Disabled;
#endif
            }
            set
            {
#if MicrosoftStore
                if (value == StartupState.Enabled)
                {
                    packageTask.RequestEnableAsync().GetAwaiter().GetResult();
                }
                else if (value == StartupState.Disabled)
                {
                    packageTask.Disable();
                }
                else
                {
                    throw new NotSupportedException();
                }
#else
                if (value == StartupState.Enabled || value == StartupState.Disabled)
                {
                    if (!OperatingSystem.IsWindows() && IsFlatpak)
                    {
                        SetFlatpakAutostart(value == StartupState.Enabled);
                    }
                    else
                    {
                        ShortcutHelpers.SetShortcut(value == StartupState.Enabled, Environment.SpecialFolder.Startup, "ShareX", StartupTargetPath, "-silent");
                    }
                }
                else
                {
                    throw new NotSupportedException();
                }
#endif
            }
        }

        private static bool IsFlatpak => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLATPAK_ID")) || System.IO.File.Exists("/.flatpak-info");

        private static string FlatpakAutostartMarker => System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "autostart", "ShareX.desktop");

        private static void SetFlatpakAutostart(bool enabled)
        {
            try
            {
                string marker = FlatpakAutostartMarker;
                string dir = System.IO.Path.GetDirectoryName(marker);
                if (enabled)
                {
                    if (!string.IsNullOrEmpty(dir) && !System.IO.Directory.Exists(dir))
                    {
                        System.IO.Directory.CreateDirectory(dir);
                    }
                    System.IO.File.WriteAllText(marker, "[Desktop Entry]\nType=Application\nName=ShareX\nExec=ShareX -silent\n");

                    using System.Diagnostics.Process proc = new();
                    proc.StartInfo.FileName = "gdbus";
                    proc.StartInfo.Arguments = "call --session --dest org.freedesktop.portal.Desktop --object-path /org/freedesktop/portal/desktop --method org.freedesktop.portal.Background.RequestBackground \"\" \"{'reason': <'Start ShareX with system'>, 'autostart': <true>, 'commandline': <['ShareX', '-silent']>}\"";
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.Start();
                    proc.WaitForExit(3000);
                }
                else
                {
                    if (System.IO.File.Exists(marker))
                    {
                        System.IO.File.Delete(marker);
                    }

                    using System.Diagnostics.Process proc = new();
                    proc.StartInfo.FileName = "gdbus";
                    proc.StartInfo.Arguments = "call --session --dest org.freedesktop.portal.Desktop --object-path /org/freedesktop/portal/desktop --method org.freedesktop.portal.Background.RequestBackground \"\" \"{'reason': <'Disable ShareX autostart'>, 'autostart': <false>, 'commandline': <['ShareX', '-silent']>}\"";
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.Start();
                    proc.WaitForExit(3000);
                }
            }
            catch (Exception ex)
            {
                DebugHelper.WriteException(ex);
            }
        }
    }
}