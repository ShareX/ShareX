#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

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
using ShareX.HelpersLib.Properties;
using Shell32;
using System;
using System.IO;
using System.Windows.Forms;
using File = System.IO.File;
using Folder = Shell32.Folder;

namespace ShareX.HelpersLib
{
    public static class ShortcutHelpers
    {
        public static bool SetShortcut(bool create, Environment.SpecialFolder specialFolder, string targetPath = "", string arguments = "")
        {
            string shortcutPath = GetShortcutPath(specialFolder);
            return SetShortcut(create, shortcutPath, targetPath, arguments);
        }

        public static bool SetShortcut(bool create, string shortcutPath, string targetPath = "", string arguments = "")
        {
            try
            { 
                if (create)
                {
                    return Create(shortcutPath, targetPath, arguments);
                }

                return Delete(shortcutPath);
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
                e.ShowError();
            }

            return false;
        }

        public static bool CheckShortcut(Environment.SpecialFolder specialFolder, string checkPath)
        {
            string shortcutPath = GetShortcutPath(specialFolder);
            return CheckShortcut(shortcutPath, checkPath);
        }

        public static bool CheckShortcut(string shortcutPath, string targetPath)
        {
            if (!string.IsNullOrEmpty(shortcutPath) && !string.IsNullOrEmpty(targetPath) && File.Exists(shortcutPath))
            {
                string checkPath = GetShortcutTargetPath(shortcutPath);
                return !string.IsNullOrEmpty(checkPath) && checkPath.Equals(targetPath, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }

        private static bool Create(string shortcutPath, string targetPath, string arguments = "")
        {
            if (!string.IsNullOrEmpty(shortcutPath) && !string.IsNullOrEmpty(targetPath) && File.Exists(targetPath))
            {
                Delete(shortcutPath);

                IWshShell wsh = new WshShellClass();
                IWshShortcut shortcut = (IWshShortcut)wsh.CreateShortcut(shortcutPath);
                shortcut.TargetPath = targetPath;
                shortcut.Arguments = arguments;
                shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);
                shortcut.Save();

                return true;
            }

            return false;
        }

        private static bool Delete(string shortcutPath)
        {
            if (!string.IsNullOrEmpty(shortcutPath) && File.Exists(shortcutPath))
            {
                File.Delete(shortcutPath);
                return true;
            }

            return false;
        }

        private static string GetShortcutTargetPath(string shortcutPath)
        {
            string directory = Path.GetDirectoryName(shortcutPath);
            string filename = Path.GetFileName(shortcutPath);

            try
            {
                Shell shell = new ShellClass();
                Folder folder = shell.NameSpace(directory);
                FolderItem folderItem = folder.ParseName(filename);

                if (folderItem != null)
                {
                    ShellLinkObject link = (ShellLinkObject)folderItem.GetLink;
                    return link.Path;
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return null;
        }

        private static string GetShortcutPath(Environment.SpecialFolder specialFolder)
        {
            string folderPath = Environment.GetFolderPath(specialFolder);
            return Path.Combine(folderPath, "ShareX.lnk");
        }

        public static void PinUnpinTaskBar(string filePath, bool pin)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                string directory = Path.GetDirectoryName(filePath);
                string filename = Path.GetFileName(filePath);

                Shell shell = new ShellClass();
                Folder folder = shell.NameSpace(directory);
                FolderItem folderItem = folder.ParseName(filename);

                FolderItemVerbs verbs = folderItem.Verbs();

                for (int i = 0; i < verbs.Count; i++)
                {
                    FolderItemVerb verb = verbs.Item(i);
                    string verbName = verb.Name.Replace(@"&", "");

                    if ((pin && verbName.Equals("pin to taskbar", StringComparison.InvariantCultureIgnoreCase)) ||
                        (!pin && verbName.Equals("unpin from taskbar", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        verb.DoIt();
                        return;
                    }
                }
            }
        }
    }
}