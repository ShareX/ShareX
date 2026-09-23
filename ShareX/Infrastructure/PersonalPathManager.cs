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

using ShareX.AvaloniaUI;
using ShareX.HelpersLib;
using ShareX.Localization;
using System;
using System.IO;
using System.Text;

namespace ShareX;

internal static class PersonalPathManager
{
    private const string ConfigFileName = "PersonalPath.cfg";
    private static readonly string CurrentConfigFilePath = Path.Combine(AppPaths.DefaultPersonalFolder, ConfigFileName);
    private static readonly string PreviousConfigFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ApplicationInfo.Name, ConfigFileName);

    internal static string CustomPersonalPath { get; private set; } = string.Empty;
    internal static string? DetectionMethod { get; private set; }

    private static string ConfigFilePath
    {
        get
        {
            string relativePath = FileHelpers.GetAbsolutePath(ConfigFileName);
            return File.Exists(relativePath) ? relativePath : CurrentConfigFilePath;
        }
    }

    internal static void Initialize()
    {
        StartupOptions.Sandbox = ApplicationCommandLine.IsCommandPresent("sandbox");
        if (StartupOptions.Sandbox)
        {
            return;
        }

        if (ApplicationCommandLine.IsCommandPresent("portable", "p"))
        {
            StartupOptions.Portable = true;
            CustomPersonalPath = AppPaths.PortablePersonalFolder;
            DetectionMethod = "Portable CLI flag";
        }
        else if (File.Exists(AppPaths.PortableCheckFilePath))
        {
            StartupOptions.Portable = true;
            CustomPersonalPath = AppPaths.PortablePersonalFolder;
            DetectionMethod = $"Portable file ({AppPaths.PortableCheckFilePath})";
        }
        else if (!string.IsNullOrEmpty(SystemOptions.PersonalPath))
        {
            CustomPersonalPath = SystemOptions.PersonalPath;
            DetectionMethod = "Registry";
        }
        else
        {
#if !MicrosoftStore
            MigrateConfig();
#endif
            string configuredPath = ReadConfig();
            if (!string.IsNullOrEmpty(configuredPath))
            {
                CustomPersonalPath = FileHelpers.GetAbsolutePath(configuredPath);
                DetectionMethod = $"PersonalPath.cfg file ({ConfigFilePath})";
            }
        }

        EnsurePersonalFolder();
    }

    internal static void CreateApplicationFolders()
    {
        if (!StartupOptions.Sandbox && Directory.Exists(AppPaths.PersonalFolder))
        {
            FileHelpers.CreateDirectory(SettingManager.BackupFolder);
            FileHelpers.CreateDirectory(AppPaths.ImageEffectsFolder);
            FileHelpers.CreateDirectory(AppPaths.ScreenshotsParentFolder);
        }
    }

    internal static string ReadConfig()
    {
        return File.Exists(ConfigFilePath)
            ? File.ReadAllText(ConfigFilePath, Encoding.UTF8).Trim()
            : string.Empty;
    }

    internal static bool WriteConfig(string? path)
    {
        path = path?.Trim() ?? string.Empty;
        bool isDefaultPath = string.IsNullOrEmpty(path) && !File.Exists(ConfigFilePath);
        if (isDefaultPath || path.Equals(ReadConfig(), StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        try
        {
            FileHelpers.CreateDirectoryFromFilePath(ConfigFilePath);
            File.WriteAllText(ConfigFilePath, path, Encoding.UTF8);
            return true;
        }
        catch (UnauthorizedAccessException exception)
        {
            DebugHelper.WriteException(exception);
            MessageBox.Show(string.Format(Strings.Program_WritePersonalPathConfig_Cant_access_to_file, ConfigFilePath),
                ApplicationInfo.Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception exception)
        {
            DebugHelper.WriteException(exception);
            exception.ShowError();
        }

        return false;
    }

    private static void EnsurePersonalFolder()
    {
        if (Directory.Exists(AppPaths.PersonalFolder))
        {
            return;
        }

        try
        {
            Directory.CreateDirectory(AppPaths.PersonalFolder);
        }
        catch (Exception exception)
        {
            StringBuilder message = new();
            message.AppendFormat("{0} \"{1}\"", Strings.Program_Run_Unable_to_create_folder_, AppPaths.PersonalFolder);
            message.AppendLine();
            if (!string.IsNullOrEmpty(DetectionMethod))
            {
                message.AppendLine("Personal path detection method: " + DetectionMethod);
            }

            message.AppendLine();
            message.Append(exception);
            MessageBox.Show(message.ToString(), $"{ApplicationInfo.Name} - {Strings.Error}",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            CustomPersonalPath = string.Empty;
        }
    }

    private static void MigrateConfig()
    {
        if (!File.Exists(PreviousConfigFilePath))
        {
            return;
        }

        try
        {
            if (!File.Exists(CurrentConfigFilePath))
            {
                FileHelpers.CreateDirectoryFromFilePath(CurrentConfigFilePath);
                File.Move(PreviousConfigFilePath, CurrentConfigFilePath);
            }

            File.Delete(PreviousConfigFilePath);
            Directory.Delete(Path.GetDirectoryName(PreviousConfigFilePath)!);
        }
        catch (Exception exception)
        {
            exception.ShowError();
        }
    }
}
