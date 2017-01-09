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

using ShareX.HelpersLib;
using ShareX.Properties;
using System;
using System.IO;
using System.Windows.Forms;

namespace ShareX
{
    public static class IntegrationHelpers
    {
        private static string GetStartupTargetPath()
        {
#if STEAM
            return Helpers.GetAbsolutePath("../ShareX_Launcher.exe");
#else
            return Application.ExecutablePath;
#endif
        }

        public static bool CheckStartupShortcut()
        {
            return ShortcutHelpers.CheckShortcut(Environment.SpecialFolder.Startup, GetStartupTargetPath());
        }

        public static void CreateStartupShortcut(bool create)
        {
            ShortcutHelpers.SetShortcut(create, Environment.SpecialFolder.Startup, GetStartupTargetPath(), "-silent");
        }

        public static bool CheckShellContextMenuButton()
        {
            return RegistryHelpers.CheckShellContextMenu();
        }

        public static void CreateShellContextMenuButton(bool create)
        {
            RegistryHelpers.SetShellContextMenu(create);
        }

        public static bool CheckCustomUploaderExtension()
        {
            return RegistryHelpers.CheckCustomUploaderExtension();
        }

        public static void CreateCustomUploaderExtension(bool create)
        {
            RegistryHelpers.SetCustomUploaderExtension(create);
        }

        public static bool CheckSendToMenuButton()
        {
            return ShortcutHelpers.CheckShortcut(Environment.SpecialFolder.SendTo, Application.ExecutablePath);
        }

        public static void CreateSendToMenuButton(bool create)
        {
            ShortcutHelpers.SetShortcut(create, Environment.SpecialFolder.SendTo, Application.ExecutablePath);
        }

        public static bool CheckSteamShowInApp()
        {
            return File.Exists(Program.SteamInAppFilePath);
        }

        public static void SteamShowInApp(bool showInApp)
        {
            string path = Program.SteamInAppFilePath;

            try
            {
                if (showInApp)
                {
                    File.Create(path).Dispose();
                }
                else if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
                MessageBox.Show(e.ToString(), "ShareX - " + Resources.TaskManager_task_UploadCompleted_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show(Resources.ApplicationSettingsForm_cbSteamShowInApp_CheckedChanged_For_settings_to_take_effect_ShareX_needs_to_be_reopened_from_Steam_,
                "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void Uninstall()
        {
            CreateStartupShortcut(false);
            CreateShellContextMenuButton(false);
            CreateSendToMenuButton(false);
        }
    }
}