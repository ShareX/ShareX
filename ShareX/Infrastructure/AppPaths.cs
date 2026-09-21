#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
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

    internal static readonly string DefaultPersonalFolder =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), ApplicationInfo.Name);
    internal static readonly string PortablePersonalFolder = FileHelpers.GetAbsolutePath(ApplicationInfo.Name);
    internal static readonly string PortableCheckFilePath = FileHelpers.GetAbsolutePath("Portable");
    internal static readonly string SteamInAppFilePath = FileHelpers.GetAbsolutePath("Steam");

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

            return Path.Combine(PersonalFolder, "Screenshots");
        }
    }

    internal static string ImageEffectsFolder => Path.Combine(PersonalFolder, "ImageEffects");
    internal static string ModelsFolder => Path.Combine(PersonalFolder, "Models");
}
