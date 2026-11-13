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

#nullable enable

using ShareX.HelpersLib;
using System;
using System.IO;

namespace ShareX;

internal static class AppPaths
{
    internal const string HistoryFileName = "History.db";
    internal const string HistoryFileNameOld = "History.json";
    internal const string LogsFolderName = "Logs";

    internal static readonly string DefaultPersonalFolder = GetDefaultPersonalFolder();
    internal static readonly string PortablePersonalFolder = FileHelpers.GetAbsolutePath(ApplicationInfo.Name);
    internal static readonly string PortableCheckFilePath = FileHelpers.GetAbsolutePath("Portable");
    internal static readonly string SteamInAppFilePath = FileHelpers.GetAbsolutePath("Steam");

    private static string GetDefaultPersonalFolder()
    {
        if (OperatingSystem.IsLinux())
        {
            string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string legacyDir = Path.Combine(homeDir, ApplicationInfo.Name);

            string configBase = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (string.IsNullOrEmpty(configBase))
            {
                configBase = Path.Combine(homeDir, ".config");
            }

            string xdgDir = Path.Combine(configBase, "sharex");
            string xdgDirCap = Path.Combine(configBase, ApplicationInfo.Name);

            if (Directory.Exists(xdgDir))
            {
                return xdgDir;
            }

            if (Directory.Exists(xdgDirCap))
            {
                return xdgDirCap;
            }

            if (Directory.Exists(legacyDir))
            {
                return legacyDir;
            }

            return xdgDir;
        }

        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), ApplicationInfo.Name);
    }

    internal static string PersonalFolder => string.IsNullOrEmpty(PersonalPathManager.CustomPersonalPath)
        ? DefaultPersonalFolder
        : FileHelpers.ExpandFolderVariables(PersonalPathManager.CustomPersonalPath);

    internal static string? HistoryFilePath => StartupOptions.Sandbox ? null : Path.Combine(PersonalFolder, HistoryFileName);
    internal static string? HistoryFilePathOld => StartupOptions.Sandbox ? null : Path.Combine(PersonalFolder, HistoryFileNameOld);
    internal static string LogsFolder => Path.Combine(PersonalFolder, LogsFolderName);

    internal static string? LogsFilePath
    {
        get
        {
            if (SystemOptions.DisableLogging)
            {
                return null;
            }

            string fileName = $"ShareX-Log-{DateTime.Now:yyyy-MM}.txt";
            return Path.Combine(LogsFolder, fileName);
        }
    }

    internal static string ScreenshotsParentFolder
    {
        get
        {
            if (ApplicationState.SettingsOrNull is { UseCustomScreenshotsPath: true } settings)
            {
                string primaryPath = settings.CustomScreenshotsPath;
                string fallbackPath = settings.CustomScreenshotsPath2;

                if (!string.IsNullOrEmpty(primaryPath))
                {
                    primaryPath = FileHelpers.ExpandFolderVariables(primaryPath);
                    if (string.IsNullOrEmpty(fallbackPath) || Directory.Exists(primaryPath))
                    {
                        return primaryPath;
                    }
                }

                if (!string.IsNullOrEmpty(fallbackPath))
                {
                    fallbackPath = FileHelpers.ExpandFolderVariables(fallbackPath);
                    if (Directory.Exists(fallbackPath))
                    {
                        return fallbackPath;
                    }
                }
            }

            return GetDefaultScreenshotsParentFolder();
        }
    }

    private static string GetDefaultScreenshotsParentFolder()
    {
        if (OperatingSystem.IsLinux())
        {
            string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string legacyDir = Path.Combine(homeDir, ApplicationInfo.Name, "Screenshots");
            if (Directory.Exists(legacyDir))
            {
                return legacyDir;
            }

            string picturesDir = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (string.IsNullOrEmpty(picturesDir))
            {
                picturesDir = Path.Combine(homeDir, "Pictures");
            }

            string sharexPictures = Path.Combine(picturesDir, ApplicationInfo.Name);
            string screenshotsPictures = Path.Combine(picturesDir, "Screenshots");

            if (Directory.Exists(sharexPictures))
            {
                return sharexPictures;
            }

            if (Directory.Exists(screenshotsPictures))
            {
                return screenshotsPictures;
            }

            return sharexPictures;
        }

        return Path.Combine(PersonalFolder, "Screenshots");
    }

    internal static string ImageEffectsFolder => Path.Combine(PersonalFolder, "ImageEffects");
    internal static string ModelsFolder => Path.Combine(PersonalFolder, "Models");
}
