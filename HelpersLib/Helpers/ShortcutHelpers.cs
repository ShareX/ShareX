#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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

#if !__MonoCS__

using IWshRuntimeLibrary;

#endif

using System;
using System.IO;
using System.Windows.Forms;
using File = System.IO.File;

namespace HelpersLib
{
    public static class ShortcutHelpers
    {
        public static bool Create(string shortcutPath, string targetPath, string arguments = "")
        {
#if !__MonoCS__
            if (!string.IsNullOrEmpty(shortcutPath) && !string.IsNullOrEmpty(targetPath) && File.Exists(targetPath))
            {
                try
                {
                    IWshShell wsh = new WshShellClass();
                    IWshShortcut shortcut = (IWshShortcut)wsh.CreateShortcut(shortcutPath);
                    shortcut.TargetPath = targetPath;
                    shortcut.Arguments = arguments;
                    shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);
                    shortcut.Save();

                    return true;
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                }
            }
#endif
            return false;
        }

        public static bool Delete(string shortcutPath)
        {
            if (!string.IsNullOrEmpty(shortcutPath) && File.Exists(shortcutPath))
            {
                File.Delete(shortcutPath);
                return true;
            }

            return false;
        }

        public static bool SetShortcut(bool create, Environment.SpecialFolder specialFolder, string arguments = "")
        {
            string shortcutPath = GetShortcutPath(specialFolder);

            if (create)
            {
                return Create(shortcutPath, Application.ExecutablePath, arguments);
            }

            return Delete(shortcutPath);
        }

        public static bool CheckShortcut(Environment.SpecialFolder specialFolder)
        {
            string shortcutPath = GetShortcutPath(specialFolder);
            return File.Exists(shortcutPath);
        }

        private static string GetShortcutPath(Environment.SpecialFolder specialFolder)
        {
            string folderPath = Environment.GetFolderPath(specialFolder);
            string shortcutPath = Path.Combine(folderPath, Application.ProductName);

            if (!Path.GetExtension(shortcutPath).Equals(".lnk", StringComparison.InvariantCultureIgnoreCase))
            {
                shortcutPath = Path.ChangeExtension(shortcutPath, "lnk");
            }

            return shortcutPath;
        }
    }
}