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

using Microsoft.Win32;
using System;
using System.IO;

namespace ShareX.HelpersLib
{
    public static class RegistryHelpers
    {
        public static void CreateRegistry(string path, string value, RegistryHive root = RegistryHive.CurrentUser)
        {
            CreateRegistry(path, null, value, root);
        }

        public static void CreateRegistry(string path, string name, string value, RegistryHive root = RegistryHive.CurrentUser)
        {
            using (RegistryKey rk = RegistryKey.OpenBaseKey(root, RegistryView.Default).CreateSubKey(path))
            {
                if (rk != null)
                {
                    rk.SetValue(name, value, RegistryValueKind.String);
                }
            }
        }

        public static void CreateRegistry(string path, int value, RegistryHive root = RegistryHive.CurrentUser)
        {
            CreateRegistry(path, null, value, root);
        }

        public static void CreateRegistry(string path, string name, int value, RegistryHive root = RegistryHive.CurrentUser)
        {
            using (RegistryKey rk = RegistryKey.OpenBaseKey(root, RegistryView.Default).CreateSubKey(path))
            {
                if (rk != null)
                {
                    rk.SetValue(name, value, RegistryValueKind.DWord);
                }
            }
        }

        public static void RemoveRegistry(string path, RegistryHive root = RegistryHive.CurrentUser)
        {
            if (!string.IsNullOrEmpty(path))
            {
                using (RegistryKey rk = RegistryKey.OpenBaseKey(root, RegistryView.Default))
                {
                    rk.DeleteSubKeyTree(path, false);
                }
            }
        }

        public static object GetValue(string path, string name = null, RegistryHive root = RegistryHive.CurrentUser, RegistryView view = RegistryView.Default)
        {
            try
            {
                using (RegistryKey baseKey = RegistryKey.OpenBaseKey(root, view))
                using (RegistryKey rk = baseKey.OpenSubKey(path))
                {
                    if (rk != null)
                    {
                        return rk.GetValue(name);
                    }
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return null;
        }

        public static string GetValueString(string path, string name = null, RegistryHive root = RegistryHive.CurrentUser, RegistryView view = RegistryView.Default)
        {
            return GetValue(path, name, root, view) as string;
        }

        public static int? GetValueDWord(string path, string name = null, RegistryHive root = RegistryHive.CurrentUser, RegistryView view = RegistryView.Default)
        {
            return (int?)GetValue(path, name, root, view);
        }

        public static bool CheckStringValue(string path, string name = null, string value = null, RegistryHive root = RegistryHive.CurrentUser, RegistryView view = RegistryView.Default)
        {
            string registryValue = GetValueString(path, name, root, view);

            return registryValue != null && (value == null || registryValue.Equals(value, StringComparison.OrdinalIgnoreCase));
        }

        public static string SearchProgramPath(string fileName)
        {
            // First method: HKEY_CLASSES_ROOT\Applications\{fileName}\shell\{command}\command

            string[] commands = new string[] { "open", "edit" };

            foreach (string command in commands)
            {
                string path = $@"HKEY_CLASSES_ROOT\Applications\{fileName}\shell\{command}\command";
                string value = Registry.GetValue(path, null, null) as string;

                if (!string.IsNullOrEmpty(value))
                {
                    string filePath = value.ParseQuoteString();

                    if (File.Exists(filePath))
                    {
                        DebugHelper.WriteLine("Found program with first method: " + filePath);
                        return filePath;
                    }
                }
            }

            // Second method: HKEY_CURRENT_USER\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\MuiCache

            using (RegistryKey programs = Registry.CurrentUser.OpenSubKey(@"Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\MuiCache"))
            {
                if (programs != null)
                {
                    foreach (string filePath in programs.GetValueNames())
                    {
                        string programPath = filePath;

                        if (!string.IsNullOrEmpty(programPath))
                        {
                            foreach (string trim in new string[] { ".ApplicationCompany", ".FriendlyAppName" })
                            {
                                if (programPath.EndsWith(trim, StringComparison.OrdinalIgnoreCase))
                                {
                                    programPath = programPath.Remove(programPath.Length - trim.Length);
                                }
                            }

                            if (programPath.EndsWith(fileName, StringComparison.OrdinalIgnoreCase) && File.Exists(programPath))
                            {
                                DebugHelper.WriteLine("Found program with second method: " + programPath);
                                return programPath;
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}