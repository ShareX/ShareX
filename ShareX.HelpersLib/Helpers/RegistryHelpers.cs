#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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
using ShareX.HelpersLib.Properties;
using System;
using System.IO;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public static class RegistryHelpers
    {
        private static readonly string WindowsStartupRun = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private static readonly string ApplicationName = "ShareX";
        private static readonly string ApplicationPath = string.Format("\"{0}\"", Application.ExecutablePath);
        private static readonly string StartupPath = ApplicationPath + " -silent";

        private static readonly string ShellExtMenuFiles = @"Software\Classes\*\shell\" + ApplicationName;
        private static readonly string ShellExtMenuFilesCmd = ShellExtMenuFiles + @"\command";

        private static readonly string ShellExtMenuDirectory = @"Software\Classes\Directory\shell\" + ApplicationName;
        private static readonly string ShellExtMenuDirectoryCmd = ShellExtMenuDirectory + @"\command";

        private static readonly string ShellExtMenuFolders = @"Software\Classes\Folder\shell\" + ApplicationName;
        private static readonly string ShellExtMenuFoldersCmd = ShellExtMenuFolders + @"\command";

        private static readonly string ShellExtDesc = string.Format(Resources.RegistryHelpers_ShellExtDesc_Upload_with__0_, ApplicationName);
        private static readonly string ShellExtIcon = ApplicationPath + ",0";
        private static readonly string ShellExtPath = ApplicationPath + " \"%1\"";

        private static readonly string ShellCustomUploaderExtensionPath = @"Software\Classes\.sxcu";
        private static readonly string ShellCustomUploaderExtensionValue = "ShareX.sxcu";
        private static readonly string ShellCustomUploaderAssociatePath = @"Software\Classes\" + ShellCustomUploaderExtensionValue;
        private static readonly string ShellCustomUploaderAssociateValue = "ShareX custom uploader";
        private static readonly string ShellCustomUploaderIconPath = ShellCustomUploaderAssociatePath + @"\DefaultIcon";
        private static readonly string ShellCustomUploaderIconValue = ApplicationPath + ",0";
        private static readonly string ShellCustomUploaderCommandPath = ShellCustomUploaderAssociatePath + @"\shell\open\command";
        private static readonly string ShellCustomUploaderCommandValue = ApplicationPath + " \"%1\"";

        private static readonly string ChromeNativeMessagingHosts = @"SOFTWARE\Google\Chrome\NativeMessagingHosts\com.getsharex.sharex";

        public static bool CheckStartWithWindows()
        {
            try
            {
                return CheckRegistry(WindowsStartupRun, ApplicationName, StartupPath);
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return false;
        }

        public static void SetStartWithWindows(bool startWithWindows)
        {
            try
            {
                using (RegistryKey regkey = Registry.CurrentUser.OpenSubKey(WindowsStartupRun, true))
                {
                    if (regkey != null)
                    {
                        if (startWithWindows)
                        {
                            regkey.SetValue(ApplicationName, StartupPath, RegistryValueKind.String);
                        }
                        else
                        {
                            regkey.DeleteValue(ApplicationName, false);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }
        }

        public static bool CheckShellContextMenu()
        {
            try
            {
                return CheckRegistry(ShellExtMenuFilesCmd, null, ShellExtPath) && CheckRegistry(ShellExtMenuDirectoryCmd, null, ShellExtPath);
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return false;
        }

        public static void SetShellContextMenu(bool register)
        {
            try
            {
                if (register)
                {
                    UnregisterShellContextMenu();
                    RegisterShellContextMenu();
                }
                else
                {
                    UnregisterShellContextMenu();
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }
        }

        public static void RegisterShellContextMenu()
        {
            CreateRegistry(ShellExtMenuFiles, ShellExtDesc);
            CreateRegistry(ShellExtMenuFiles, "Icon", ShellExtIcon);
            CreateRegistry(ShellExtMenuFilesCmd, ShellExtPath);

            CreateRegistry(ShellExtMenuDirectory, ShellExtDesc);
            CreateRegistry(ShellExtMenuDirectory, "Icon", ShellExtIcon);
            CreateRegistry(ShellExtMenuDirectoryCmd, ShellExtPath);
        }

        public static void UnregisterShellContextMenu()
        {
            RemoveRegistry(ShellExtMenuFiles, true);
            RemoveRegistry(ShellExtMenuDirectory, true);
            RemoveRegistry(ShellExtMenuFolders, true);
        }

        public static bool CheckCustomUploaderExtension()
        {
            try
            {
                return CheckRegistry(ShellCustomUploaderExtensionPath, null, ShellCustomUploaderExtensionValue) && CheckRegistry(ShellCustomUploaderCommandPath, null, ShellCustomUploaderCommandValue);
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return false;
        }

        public static void SetCustomUploaderExtension(bool register)
        {
            try
            {
                if (register)
                {
                    UnregisterCustomUploaderExtension();
                    RegisterCustomUploaderExtension();
                }
                else
                {
                    UnregisterCustomUploaderExtension();
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }
        }

        public static void RegisterCustomUploaderExtension()
        {
            CreateRegistry(ShellCustomUploaderExtensionPath, ShellCustomUploaderExtensionValue);
            CreateRegistry(ShellCustomUploaderAssociatePath, ShellCustomUploaderAssociateValue);
            CreateRegistry(ShellCustomUploaderIconPath, ShellCustomUploaderIconValue);
            CreateRegistry(ShellCustomUploaderCommandPath, ShellCustomUploaderCommandValue);

            NativeMethods.SHChangeNotify(HChangeNotifyEventID.SHCNE_ASSOCCHANGED, HChangeNotifyFlags.SHCNF_FLUSH, IntPtr.Zero, IntPtr.Zero);
        }

        public static void UnregisterCustomUploaderExtension()
        {
            RemoveRegistry(ShellCustomUploaderExtensionPath);
            RemoveRegistry(ShellCustomUploaderAssociatePath, true);
        }

        public static void RegisterChromeSupport(string filepath)
        {
            CreateRegistry(ChromeNativeMessagingHosts, filepath);
        }

        public static void UnregisterChromeSupport()
        {
            RemoveRegistry(ChromeNativeMessagingHosts);
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
    }
}