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

namespace ShareX;

internal static class FileTypeRegistration
{
    internal static void RegisterMissingExtensions()
    {
#if !MicrosoftStore
        // On macOS the app bundle declares its file types; there is nothing to register at run time.
        if (StartupOptions.Portable || !Platform.PlatformServices.Current.ShellIntegration.FileAssociationSupport.IsSupported)
        {
            return;
        }

        if (!IntegrationHelpers.CheckCustomUploaderExtension())
        {
            IntegrationHelpers.CreateCustomUploaderExtension(true);
        }

        if (!IntegrationHelpers.CheckImageEffectExtension())
        {
            IntegrationHelpers.CreateImageEffectExtension(true);
        }
#endif
    }

    /// <summary>
    /// Where no installer registers the browser extension host (Linux, macOS), ShareX does it once, as the Windows setup does by
    /// default. After that the "Enable Chrome extension support" and "Enable Firefox add-on support" settings decide. While it
    /// is on, the registration is refreshed at every start: an AppImage's launcher follows the file when it is moved.
    /// </summary>
    internal static void RegisterBrowserHost()
    {
        Platform.IShellIntegrationService integration = Platform.PlatformServices.Current.ShellIntegration;

        if (StartupOptions.Portable || !integration.RegistersBrowserHostOnFirstRun || !integration.BrowserHostSupport.IsSupported)
        {
            return;
        }

        bool firstRun = !ApplicationState.Settings.BrowserExtensionSupportOffered;
        ApplicationState.Settings.BrowserExtensionSupportOffered = true;

        if (firstRun || IntegrationHelpers.CheckChromeExtensionSupport())
        {
            IntegrationHelpers.CreateChromeExtensionSupport(true);
        }

        if (firstRun || IntegrationHelpers.CheckFirefoxAddonSupport())
        {
            IntegrationHelpers.CreateFirefoxAddonSupport(true);
        }
    }
}
