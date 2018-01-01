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

using Microsoft.Win32;
using System;
using System.IO;

namespace ShareX.HelpersLib
{
    public static class RegistryHelpers
    {
        public static void CreateRegistry(string path, string value)
        {
            CreateRegistry(path, null, value);
        }

        public static void CreateRegistry(string path, string name, string value)
        {
            using (RegistryKey rk = Registry.CurrentUser.CreateSubKey(path))
            {
                if (rk != null)
                {
                    rk.SetValue(name, value, RegistryValueKind.String);
                }
            }
        }

        public static void CreateRegistry(string path, int value)
        {
            CreateRegistry(path, null, value);
        }

        public static void CreateRegistry(string path, string name, int value)
        {
            using (RegistryKey rk = Registry.CurrentUser.CreateSubKey(path))
            {
                if (rk != null)
                {
                    rk.SetValue(name, value, RegistryValueKind.DWord);
                }
            }
        }

        public static void RemoveRegistry(string path, bool recursive = false)
        {
            using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(path))
            {
                if (rk != null)
                {
                    if (recursive)
                    {
                        Registry.CurrentUser.DeleteSubKeyTree(path);
                    }
                    else
                    {
                        Registry.CurrentUser.DeleteSubKey(path);
                    }
                }
            }
        }

        public static bool CheckRegistry(string path, string name = null, string value = null)
        {
            string registryValue = GetRegistryValue(path, name);

            if (registryValue != null)
            {
                return value == null || registryValue.Equals(value, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }

        public static string GetRegistryValue(string path, string name = null)
        {
            using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(path))
            {
                if (rk != null)
                {
                    return rk.GetValue(name, null) as string;
                }
            }

            return null;
        }

        public static ExternalProgram FindProgram(string name, string filename)
        {
            // First method: HKEY_CLASSES_ROOT\Applications\{filename}\shell\{command}\command

            string[] commands = new string[] { "open", "edit" };

            foreach (string command in commands)
            {
                string path = string.Format(@"HKEY_CLASSES_ROOT\Applications\{0}\shell\{1}\command", filename, command);
                string value = Registry.GetValue(path, null, null) as string;

                if (!string.IsNullOrEmpty(value))
                {
                    string filePath = value.ParseQuoteString();

                    if (File.Exists(filePath))
                    {
                        DebugHelper.WriteLine("Found program with first method: " + filePath);
                        return new ExternalProgram(name, filePath);
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
                        if (!string.IsNullOrEmpty(filePath) && programs.GetValueKind(filePath) == RegistryValueKind.String)
                        {
                            string programName = programs.GetValue(filePath, null) as string;

                            if (!string.IsNullOrEmpty(programName) && programName.Equals(name, StringComparison.InvariantCultureIgnoreCase) && File.Exists(filePath))
                            {
                                DebugHelper.WriteLine("Found program with second method: " + filePath);
                                return new ExternalProgram(name, filePath);
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}