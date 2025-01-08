#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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

using IWshRuntimeLibrary;
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
            string folderPath = Environment.GetFolderPath(specialFolder);

            if (!shortcutName.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase))
            {
                shortcutName += ".lnk";
            }

            return Path.Combine(folderPath, shortcutName);
        }

        private static bool CreateShortcut(string shortcutPath, string targetPath, string arguments = "")
        {
            if (!string.IsNullOrEmpty(shortcutPath) && !string.IsNullOrEmpty(targetPath) && File.Exists(targetPath))
            {
                DeleteShortcut(shortcutPath);

                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
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
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
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