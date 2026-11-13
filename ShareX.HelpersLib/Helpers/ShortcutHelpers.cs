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
using System.IO;
using File = System.IO.File;

namespace ShareX.HelpersLib
{
    public static class ShortcutHelpers
    {
        public static bool SetShortcut(bool create, Environment.SpecialFolder specialFolder, string shortcutName, string targetPath, string arguments = "")
        {
            string shortcutPath = GetShortcutPath(specialFolder, shortcutName);
            return SetShortcut(create, shortcutPath, targetPath, arguments);
        }

        public static bool SetShortcut(bool create, string shortcutPath, string targetPath, string arguments = "")
        {
            try
            {
                if (create)
                {
                    return CreateShortcut(shortcutPath, targetPath, arguments);
                }
                else
                {
                    return DeleteShortcut(shortcutPath);
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
                e.ShowError();
            }

            return false;
        }

        public static bool CheckShortcut(Environment.SpecialFolder specialFolder, string shortcutName, string targetPath)
        {
            string shortcutPath = GetShortcutPath(specialFolder, shortcutName);
            return CheckShortcut(shortcutPath, targetPath);
        }

        public static bool CheckShortcut(string shortcutPath, string targetPath)
        {
            if (!string.IsNullOrEmpty(shortcutPath) && !string.IsNullOrEmpty(targetPath) && File.Exists(shortcutPath))
            {
                try
                {
                    string shortcutTargetPath = GetShortcutTargetPath(shortcutPath);
                    return !string.IsNullOrEmpty(shortcutTargetPath) && shortcutTargetPath.Equals(targetPath, StringComparison.OrdinalIgnoreCase);
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                }
            }

            return false;
        }

        private static string GetShortcutPath(Environment.SpecialFolder specialFolder, string shortcutName)
        {
            if (!OperatingSystem.IsWindows())
            {
                string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string folderPath;
                if (specialFolder == Environment.SpecialFolder.Startup)
                {
                    folderPath = Path.Combine(home, ".config", "autostart");
                }
                else
                {
                    folderPath = Path.Combine(home, ".local", "share", "applications");
                }

                if (!shortcutName.EndsWith(".desktop", StringComparison.OrdinalIgnoreCase))
                {
                    shortcutName = Path.GetFileNameWithoutExtension(shortcutName) + ".desktop";
                }

                return Path.Combine(folderPath, shortcutName);
            }

            string winFolderPath = Environment.GetFolderPath(specialFolder);

            if (!shortcutName.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase))
            {
                shortcutName += ".lnk";
            }

            return Path.Combine(winFolderPath, shortcutName);
        }

        private static bool CreateShortcut(string shortcutPath, string targetPath, string arguments = "")
        {
            if (!string.IsNullOrEmpty(shortcutPath) && !string.IsNullOrEmpty(targetPath) && File.Exists(targetPath))
            {
                DeleteShortcut(shortcutPath);

                if (!OperatingSystem.IsWindows())
                {
                    try
                    {
                        string dir = Path.GetDirectoryName(shortcutPath);
                        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }

                        string name = Path.GetFileNameWithoutExtension(shortcutPath);
                        string exec;
                        if (targetPath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                        {
                            exec = string.IsNullOrEmpty(arguments) ? $"dotnet \"{targetPath}\"" : $"dotnet \"{targetPath}\" {arguments}";
                        }
                        else if (Path.GetFileNameWithoutExtension(targetPath).Equals("dotnet", StringComparison.OrdinalIgnoreCase))
                        {
                            string runSh = null;
                            string searchDir = AppContext.BaseDirectory;
                            while (!string.IsNullOrEmpty(searchDir))
                            {
                                string candidate = Path.Combine(searchDir, "run.sh");
                                if (File.Exists(candidate))
                                {
                                    runSh = candidate;
                                    break;
                                }
                                searchDir = Directory.GetParent(searchDir)?.FullName;
                            }

                            if (!string.IsNullOrEmpty(runSh))
                            {
                                exec = string.IsNullOrEmpty(arguments) ? $"\"{runSh}\"" : $"\"{runSh}\" {arguments}";
                            }
                            else
                            {
                                string dllPath = Path.Combine(AppContext.BaseDirectory, "ShareX.dll");
                                exec = string.IsNullOrEmpty(arguments) ? $"dotnet \"{dllPath}\"" : $"dotnet \"{dllPath}\" {arguments}";
                            }
                        }
                        else
                        {
                            exec = string.IsNullOrEmpty(arguments) ? $"\"{targetPath}\"" : $"\"{targetPath}\" {arguments}";
                        }
                        string content = $"[Desktop Entry]\nType=Application\nName={name}\nExec={exec}\nTerminal=false\nIcon=sharex\nComment=ShareX\n";
                        File.WriteAllText(shortcutPath, content);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        DebugHelper.WriteException(ex);
                        return false;
                    }
                }

                WshShell shell = new WshShell();
                IWshShortcut shortcut = ((IWshShell)shell).CreateShortcut(shortcutPath);
                shortcut.TargetPath = targetPath;
                shortcut.Arguments = arguments;
                shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);
                shortcut.Save();

                return true;
            }

            return false;
        }

        private static string GetShortcutTargetPath(string shortcutPath)
        {
            if (!OperatingSystem.IsWindows())
            {
                if (!string.IsNullOrEmpty(shortcutPath) && File.Exists(shortcutPath))
                {
                    foreach (string line in File.ReadAllLines(shortcutPath))
                    {
                        if (line.StartsWith("Exec=", StringComparison.OrdinalIgnoreCase))
                        {
                            string exec = line.Substring(5).Trim();
                            if (exec.StartsWith("dotnet ", StringComparison.OrdinalIgnoreCase))
                            {
                                string rem = exec.Substring(7).Trim();
                                if (rem.StartsWith("\""))
                                {
                                    int nextQuote = rem.IndexOf('\"', 1);
                                    if (nextQuote > 1) return rem.Substring(1, nextQuote - 1);
                                }
                                int spaceIdx = rem.IndexOf(' ');
                                return spaceIdx > 0 ? rem.Substring(0, spaceIdx) : rem;
                            }
                            if (exec.StartsWith("\""))
                            {
                                int nextQuote = exec.IndexOf('\"', 1);
                                if (nextQuote > 1) return exec.Substring(1, nextQuote - 1);
                            }
                            int sIdx = exec.IndexOf(' ');
                            return sIdx > 0 ? exec.Substring(0, sIdx) : exec;
                        }
                    }
                }
                return string.Empty;
            }

            WshShell shell = new WshShell();
            IWshShortcut shortcut = ((IWshShell)shell).CreateShortcut(shortcutPath);
            return shortcut.TargetPath;
        }

        private static bool DeleteShortcut(string shortcutPath)
        {
            if (!string.IsNullOrEmpty(shortcutPath) && File.Exists(shortcutPath))
            {
                File.Delete(shortcutPath);
                return true;
            }

            return false;
        }
    }
}