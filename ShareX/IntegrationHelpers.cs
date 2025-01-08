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

using ShareX.HelpersLib;
using ShareX.Properties;
using System;
using System.IO;
using System.Windows.Forms;

namespace ShareX
{
    public static class IntegrationHelpers
    {
        private static readonly string ApplicationPath = $"\"{Application.ExecutablePath}\"";

        private static readonly string ShellExtMenuName = "ShareX";
        private static readonly string ShellExtMenuFiles = $@"Software\Classes\*\shell\{ShellExtMenuName}";
        private static readonly string ShellExtMenuFilesCmd = $@"{ShellExtMenuFiles}\command";
        private static readonly string ShellExtMenuDirectory = $@"Software\Classes\Directory\shell\{ShellExtMenuName}";
        private static readonly string ShellExtMenuDirectoryCmd = $@"{ShellExtMenuDirectory}\command";
        private static readonly string ShellExtDesc = Resources.IntegrationHelpers_UploadWithShareX;
        private static readonly string ShellExtIcon = $"{ApplicationPath},0";
        private static readonly string ShellExtPath = $"{ApplicationPath} \"%1\"";

        private static readonly string ShellExtEditName = "ShareXImageEditor";
        private static readonly string ShellExtEditImage = $@"Software\Classes\SystemFileAssociations\image\shell\{ShellExtEditName}";
        private static readonly string ShellExtEditImageCmd = $@"{ShellExtEditImage}\command";
        private static readonly string ShellExtEditDesc = Resources.IntegrationHelpers_EditWithShareX;
        private static readonly string ShellExtEditIcon = $"{ApplicationPath},0";
        private static readonly string ShellExtEditPath = $"{ApplicationPath} -ImageEditor \"%1\"";

        private static readonly string ShellCustomUploaderExtensionPath = @"Software\Classes\.sxcu";
        private static readonly string ShellCustomUploaderExtensionValue = "ShareX.sxcu";
        private static readonly string ShellCustomUploaderAssociatePath = $@"Software\Classes\{ShellCustomUploaderExtensionValue}";
        private static readonly string ShellCustomUploaderAssociateValue = "ShareX custom uploader";
        private static readonly string ShellCustomUploaderIconPath = $@"{ShellCustomUploaderAssociatePath}\DefaultIcon";
        private static readonly string ShellCustomUploaderIconValue = $"{ApplicationPath},0";
        private static readonly string ShellCustomUploaderCommandPath = $@"{ShellCustomUploaderAssociatePath}\shell\open\command";
        private static readonly string ShellCustomUploaderCommandValue = $"{ApplicationPath} -CustomUploader \"%1\"";

        private static readonly string ShellImageEffectExtensionPath = @"Software\Classes\.sxie";
        private static readonly string ShellImageEffectExtensionValue = "ShareX.sxie";
        private static readonly string ShellImageEffectAssociatePath = $@"Software\Classes\{ShellImageEffectExtensionValue}";
        private static readonly string ShellImageEffectAssociateValue = "ShareX image effect";
        private static readonly string ShellImageEffectIconPath = $@"{ShellImageEffectAssociatePath}\DefaultIcon";
        private static readonly string ShellImageEffectIconValue = $"{ApplicationPath},0";
        private static readonly string ShellImageEffectCommandPath = $@"{ShellImageEffectAssociatePath}\shell\open\command";
        private static readonly string ShellImageEffectCommandValue = $"{ApplicationPath} -ImageEffect \"%1\"";

        private static readonly string ChromeNativeMessagingHosts = @"SOFTWARE\Google\Chrome\NativeMessagingHosts\com.getsharex.sharex";
        private static readonly string FirefoxNativeMessagingHosts = @"SOFTWARE\Mozilla\NativeMessagingHosts\ShareX";
        private static readonly string ChromeHostManifestFilePath = FileHelpers.GetAbsolutePath("host-manifest-chrome.json");
        private static readonly string FirefoxHostManifestFilePath = FileHelpers.GetAbsolutePath("host-manifest-firefox.json");

        public static bool CheckShellContextMenuButton()
        {
            try
            {
                return RegistryHelpers.CheckStringValue(ShellExtMenuFilesCmd, null, ShellExtPath) &&
                    RegistryHelpers.CheckStringValue(ShellExtMenuDirectoryCmd, null, ShellExtPath);
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return false;
        }

        public static void CreateShellContextMenuButton(bool create)
        {
            try
            {
                if (create)
                {
                    UnregisterShellContextMenuButton();
                    RegisterShellContextMenuButton();
                }
                else
                {
                    UnregisterShellContextMenuButton();
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }
        }

        private static void RegisterShellContextMenuButton()
        {
            RegistryHelpers.CreateRegistry(ShellExtMenuFiles, ShellExtDesc);
            RegistryHelpers.CreateRegistry(ShellExtMenuFiles, "Icon", ShellExtIcon);
            RegistryHelpers.CreateRegistry(ShellExtMenuFilesCmd, ShellExtPath);

            RegistryHelpers.CreateRegistry(ShellExtMenuDirectory, ShellExtDesc);
            RegistryHelpers.CreateRegistry(ShellExtMenuDirectory, "Icon", ShellExtIcon);
            RegistryHelpers.CreateRegistry(ShellExtMenuDirectoryCmd, ShellExtPath);
        }

        private static void UnregisterShellContextMenuButton()
        {
            RegistryHelpers.RemoveRegistry(ShellExtMenuFiles);
            RegistryHelpers.RemoveRegistry(ShellExtMenuDirectory);
        }

        public static bool CheckEditShellContextMenuButton()
        {
            try
            {
                return RegistryHelpers.CheckStringValue(ShellExtEditImageCmd, null, ShellExtEditPath);
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return false;
        }

        public static void CreateEditShellContextMenuButton(bool create)
        {
            try
            {
                if (create)
                {
                    UnregisterEditShellContextMenuButton();
                    RegisterEditShellContextMenuButton();
                }
                else
                {
                    UnregisterEditShellContextMenuButton();
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }
        }

        private static void RegisterEditShellContextMenuButton()
        {
            RegistryHelpers.CreateRegistry(ShellExtEditImage, ShellExtEditDesc);
            RegistryHelpers.CreateRegistry(ShellExtEditImage, "Icon", ShellExtEditIcon);
            RegistryHelpers.CreateRegistry(ShellExtEditImageCmd, ShellExtEditPath);
        }

        private static void UnregisterEditShellContextMenuButton()
        {
            RegistryHelpers.RemoveRegistry(ShellExtEditImage);
        }

        public static bool CheckCustomUploaderExtension()
        {
            try
            {
                return RegistryHelpers.CheckStringValue(ShellCustomUploaderExtensionPath, null, ShellCustomUploaderExtensionValue) &&
                    RegistryHelpers.CheckStringValue(ShellCustomUploaderCommandPath, null, ShellCustomUploaderCommandValue);
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return false;
        }

        public static void CreateCustomUploaderExtension(bool create)
        {
            try
            {
                if (create)
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

        private static void RegisterCustomUploaderExtension()
        {
            RegistryHelpers.CreateRegistry(ShellCustomUploaderExtensionPath, ShellCustomUploaderExtensionValue);
            RegistryHelpers.CreateRegistry(ShellCustomUploaderAssociatePath, ShellCustomUploaderAssociateValue);
            RegistryHelpers.CreateRegistry(ShellCustomUploaderIconPath, ShellCustomUploaderIconValue);
            RegistryHelpers.CreateRegistry(ShellCustomUploaderCommandPath, ShellCustomUploaderCommandValue);

            NativeMethods.SHChangeNotify(HChangeNotifyEventID.SHCNE_ASSOCCHANGED, HChangeNotifyFlags.SHCNF_FLUSH, IntPtr.Zero, IntPtr.Zero);
        }

        private static void UnregisterCustomUploaderExtension()
        {
            RegistryHelpers.RemoveRegistry(ShellCustomUploaderExtensionPath);
            RegistryHelpers.RemoveRegistry(ShellCustomUploaderAssociatePath);
        }

        public static bool CheckImageEffectExtension()
        {
            try
            {
                return RegistryHelpers.CheckStringValue(ShellImageEffectExtensionPath, null, ShellImageEffectExtensionValue) &&
                    RegistryHelpers.CheckStringValue(ShellImageEffectCommandPath, null, ShellImageEffectCommandValue);
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return false;
        }

        public static void CreateImageEffectExtension(bool create)
        {
            try
            {
                if (create)
                {
                    UnregisterImageEffectExtension();
                    RegisterImageEffectExtension();
                }
                else
                {
                    UnregisterImageEffectExtension();
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }
        }

        private static void RegisterImageEffectExtension()
        {
            RegistryHelpers.CreateRegistry(ShellImageEffectExtensionPath, ShellImageEffectExtensionValue);
            RegistryHelpers.CreateRegistry(ShellImageEffectAssociatePath, ShellImageEffectAssociateValue);
            RegistryHelpers.CreateRegistry(ShellImageEffectIconPath, ShellImageEffectIconValue);
            RegistryHelpers.CreateRegistry(ShellImageEffectCommandPath, ShellImageEffectCommandValue);

            NativeMethods.SHChangeNotify(HChangeNotifyEventID.SHCNE_ASSOCCHANGED, HChangeNotifyFlags.SHCNF_FLUSH, IntPtr.Zero, IntPtr.Zero);
        }

        private static void UnregisterImageEffectExtension()
        {
            RegistryHelpers.RemoveRegistry(ShellImageEffectExtensionPath);
            RegistryHelpers.RemoveRegistry(ShellImageEffectAssociatePath);
        }

        public static bool CheckChromeExtensionSupport()
        {
            try
            {
                return RegistryHelpers.CheckStringValue(ChromeNativeMessagingHosts, null, ChromeHostManifestFilePath) && File.Exists(ChromeHostManifestFilePath);
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return false;
        }

        public static void CreateChromeExtensionSupport(bool create)
        {
            try
            {
                if (create)
                {
                    UnregisterChromeExtensionSupport();
                    RegisterChromeExtensionSupport();
                }
                else
                {
                    UnregisterChromeExtensionSupport();
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }
        }

        private static void RegisterChromeExtensionSupport()
        {
            RegistryHelpers.CreateRegistry(ChromeNativeMessagingHosts, ChromeHostManifestFilePath);
        }

        private static void UnregisterChromeExtensionSupport()
        {
            RegistryHelpers.RemoveRegistry(ChromeNativeMessagingHosts);
        }

        public static bool CheckFirefoxAddonSupport()
        {
            try
            {
                return RegistryHelpers.CheckStringValue(FirefoxNativeMessagingHosts, null, FirefoxHostManifestFilePath) && File.Exists(FirefoxHostManifestFilePath);
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return false;
        }

        public static void CreateFirefoxAddonSupport(bool create)
        {
            try
            {
                if (create)
                {
                    UnregisterFirefoxAddonSupport();
                    RegisterFirefoxAddonSupport();
                }
                else
                {
                    UnregisterFirefoxAddonSupport();
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }
        }

        private static void RegisterFirefoxAddonSupport()
        {
            RegistryHelpers.CreateRegistry(FirefoxNativeMessagingHosts, FirefoxHostManifestFilePath);
        }

        private static void UnregisterFirefoxAddonSupport()
        {
            RegistryHelpers.RemoveRegistry(FirefoxNativeMessagingHosts);
        }

        public static bool CheckSendToMenuButton()
        {
            return ShortcutHelpers.CheckShortcut(Environment.SpecialFolder.SendTo, "ShareX", Application.ExecutablePath);
        }

        public static bool CreateSendToMenuButton(bool create)
        {
            return ShortcutHelpers.SetShortcut(create, Environment.SpecialFolder.SendTo, "ShareX", Application.ExecutablePath);
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
                    FileHelpers.CreateEmptyFile(path);
                }
                else if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
                e.ShowError();
                return;
            }

            MessageBox.Show(Resources.ApplicationSettingsForm_cbSteamShowInApp_CheckedChanged_For_settings_to_take_effect_ShareX_needs_to_be_reopened_from_Steam_,
                "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void Uninstall()
        {
            StartupManager.State = StartupState.Disabled;
            CreateShellContextMenuButton(false);
            CreateEditShellContextMenuButton(false);
            CreateCustomUploaderExtension(false);
            CreateImageEffectExtension(false);
            CreateSendToMenuButton(false);
            UnregisterChromeExtensionSupport();
            UnregisterFirefoxAddonSupport();
        }
    }
}