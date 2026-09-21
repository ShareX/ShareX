#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using System;
using System.Text;
using System.Windows.Forms;

namespace ShareX;

internal static class ApplicationInfo
{
    internal const string Name = "ShareX";
    internal const string MutexName = "82E6AC09-0FEF-4390-AD9F-0DD3F5561EFC";
    internal static readonly string PipeName = $"{Environment.MachineName}-{Environment.UserName}-{Name}";

    internal const ShareXBuild Build =
#if RELEASE
        ShareXBuild.Release;
#elif STEAM
        ShareXBuild.Steam;
#elif MicrosoftStore
        ShareXBuild.MicrosoftStore;
#elif DEBUG
        ShareXBuild.Debug;
#else
        ShareXBuild.Unknown;
#endif

    internal const bool Dev = true;

    internal static string VersionText
    {
        get
        {
            StringBuilder versionText = new();
            Version version = Version.Parse(Application.ProductVersion);
            versionText.Append(version.Major).Append('.').Append(version.Minor);
            if (version.Build > 0 || version.Revision > 0) versionText.Append('.').Append(version.Build);
            if (version.Revision > 0) versionText.Append('.').Append(version.Revision);
            if (Dev) versionText.Append(" Dev");
            if (StartupOptions.Portable) versionText.Append(" Portable");
            return versionText.ToString();
        }
    }

    internal static string Title
    {
        get
        {
            string title = $"{Name} {VersionText}";
            if (ApplicationState.SettingsOrNull is { DevMode: true })
            {
                string info = Build.ToString();
                if (StartupOptions.IsAdmin)
                {
                    info += ", Admin";
                }

                title += $" ({info})";
            }

            return title;
        }
    }

    internal static string TitleShort => ApplicationState.SettingsOrNull is { DevMode: true } ? Title : Name;
}
