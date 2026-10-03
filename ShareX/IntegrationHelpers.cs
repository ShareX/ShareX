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

using ShareX.HelpersLib;
using ShareX.Localization;
using ShareX.Platform;
using System;
using System.IO;
using MessageBox = ShareX.AvaloniaUI.MessageBox;
using MessageBoxButtons = ShareX.AvaloniaUI.MessageBoxButtons;
using MessageBoxIcon = ShareX.AvaloniaUI.MessageBoxIcon;

namespace ShareX
{
    public static class IntegrationHelpers
    {
        private static readonly string ApplicationPath = Environment.ProcessPath;
        private static readonly string FileIconPath = FileHelpers.GetAbsolutePath("ShareX_File_Icon.ico");

        // The browser extension host sits next to ShareX: ShareX_NativeMessagingHost.exe on Windows, without an extension elsewhere.
        private static readonly string NativeMessagingHostPath =
            FileHelpers.GetAbsolutePath("ShareX_NativeMessagingHost" + Path.GetExtension(Environment.ProcessPath));

        private static readonly string ShellExtMenuName = "ShareX";
        private static readonly string ShellExtDesc = Strings.IntegrationHelpers_UploadWithShareX;

        private static readonly string ShellExtEditName = "ShareXImageEditor";
        private static readonly string ShellExtEditDesc = Strings.IntegrationHelpers_EditWithShareX;

        // Explorer on Windows, Nautilus, Dolphin, Nemo, Caja and Thunar on Linux.
        private static ShellMenuEntry UploadMenuEntry => new ShellMenuEntry(ShellExtMenuName, ShellExtDesc, ApplicationPath,
            Array.Empty<string>(), ShellMenuTarget.FilesAndFolders);

        private static ShellMenuEntry EditMenuEntry => new ShellMenuEntry(ShellExtEditName, ShellExtEditDesc, ApplicationPath,
            ["-ImageEditor"], ShellMenuTarget.Images);

        private static FileAssociation CustomUploaderAssociation => new FileAssociation(".sxcu", "ShareX.sxcu", "ShareX custom uploader",
            "application/x-sharex-custom-uploader", ApplicationPath, ["-CustomUploader"])
        {
            Icon = FileIconPath
        };

        private static FileAssociation ImageEffectAssociation => new FileAssociation(".sxie", "ShareX.sxie", "ShareX image effect",
            "application/x-sharex-image-effect", ApplicationPath, ["-ImageEffect"])
        {
            Icon = FileIconPath
        };

        private static BrowserHost ChromeHost => new BrowserHost(BrowserFamily.Chromium, "com.getsharex.sharex",
            FileHelpers.GetAbsolutePath("host-manifest-chrome.json"), NativeMessagingHostPath);

        private static BrowserHost FirefoxHost => new BrowserHost(BrowserFamily.Firefox, "ShareX",
            FileHelpers.GetAbsolutePath("host-manifest-firefox.json"), NativeMessagingHostPath);

        private static IShellIntegrationService ShellIntegration => PlatformServices.Current.ShellIntegration;

        public static bool CheckShellContextMenuButton()
        {
            try
            {
                return PlatformServices.Current.ShellIntegration.IsRegistered(UploadMenuEntry);
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
            PlatformServices.Current.ShellIntegration.Register(UploadMenuEntry);
        }

        private static void UnregisterShellContextMenuButton()
        {
            PlatformServices.Current.ShellIntegration.Unregister(UploadMenuEntry);
        }

        public static bool CheckEditShellContextMenuButton()
        {
            try
            {
                return PlatformServices.Current.ShellIntegration.IsRegistered(EditMenuEntry);
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
            PlatformServices.Current.ShellIntegration.Register(EditMenuEntry);
        }

        private static void UnregisterEditShellContextMenuButton()
        {
            PlatformServices.Current.ShellIntegration.Unregister(EditMenuEntry);
        }

        public static bool CheckCustomUploaderExtension() => Check(() => ShellIntegration.IsAssociated(CustomUploaderAssociation));

        public static void CreateCustomUploaderExtension(bool create) => Set(create,
            () => ShellIntegration.Associate(CustomUploaderAssociation), () => ShellIntegration.RemoveAssociation(CustomUploaderAssociation));

        public static bool CheckImageEffectExtension() => Check(() => ShellIntegration.IsAssociated(ImageEffectAssociation));

        public static void CreateImageEffectExtension(bool create) => Set(create,
            () => ShellIntegration.Associate(ImageEffectAssociation), () => ShellIntegration.RemoveAssociation(ImageEffectAssociation));

        public static bool CheckChromeExtensionSupport() => Check(() => ShellIntegration.IsBrowserHostRegistered(ChromeHost));

        public static void CreateChromeExtensionSupport(bool create) => Set(create,
            () => ShellIntegration.RegisterBrowserHost(ChromeHost), UnregisterChromeExtensionSupport);

        private static void UnregisterChromeExtensionSupport() => ShellIntegration.UnregisterBrowserHost(ChromeHost);

        public static bool CheckFirefoxAddonSupport() => Check(() => ShellIntegration.IsBrowserHostRegistered(FirefoxHost));

        public static void CreateFirefoxAddonSupport(bool create) => Set(create,
            () => ShellIntegration.RegisterBrowserHost(FirefoxHost), UnregisterFirefoxAddonSupport);

        private static void UnregisterFirefoxAddonSupport() => ShellIntegration.UnregisterBrowserHost(FirefoxHost);

        public static bool CheckSendToMenuButton() => Check(() => ShellIntegration.IsInSendTo("ShareX", ApplicationPath));

        public static bool CreateSendToMenuButton(bool create)
        {
            try
            {
                ShellIntegration.SetInSendTo("ShareX", ApplicationPath, create);
                return true;
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
                e.ShowError();
                return false;
            }
        }

        private static bool Check(Func<bool> check)
        {
            try
            {
                return check();
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
                return false;
            }
        }

        /// <summary>Registers after removing what was there, or only removes; failures are logged as they always were.</summary>
        private static void Set(bool create, Action register, Action unregister)
        {
            try
            {
                unregister();

                if (create)
                {
                    register();
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }
        }

        public static bool CheckSteamShowInApp()
        {
            return File.Exists(AppPaths.SteamInAppFilePath);
        }

        public static void SteamShowInApp(bool showInApp)
        {
            string path = AppPaths.SteamInAppFilePath;

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

            MessageBox.Show(Strings.ApplicationSettingsForm_cbSteamShowInApp_CheckedChanged_For_settings_to_take_effect_ShareX_needs_to_be_reopened_from_Steam_,
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
