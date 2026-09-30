using Microsoft.Win32;
using ShareX.Platform.Windows.Native;
using System;
using System.IO;
using System.Linq;

namespace ShareX.Platform.Windows;

/// <summary>Launch at sign in with a shortcut in the user's Startup folder, exactly as ShareX has always done.</summary>
/// <remarks>The Microsoft Store build uses the package's StartupTask instead, which stays in the ShareX project.</remarks>
public sealed class WindowsStartupService : IStartupService
{
    private const string StartupApprovedKey = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\StartupFolder";

    private readonly string startupFolder;

    public WindowsStartupService()
        : this(Environment.GetFolderPath(Environment.SpecialFolder.Startup))
    {
    }

    public WindowsStartupService(string startupFolder)
    {
        this.startupFolder = startupFolder;
    }

    public FeatureSupport Support => FeatureSupport.Supported;

    public string GetShortcutPath(StartupRegistration registration)
    {
        string name = registration.Name.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase) ? registration.Name : registration.Name + ".lnk";
        return Path.Combine(startupFolder, name);
    }

    public StartupRegistrationState GetState(StartupRegistration registration)
    {
        string shortcutPath = GetShortcutPath(registration);

        if (!IsShortcutTo(shortcutPath, registration.ExecutablePath))
        {
            return StartupRegistrationState.Disabled;
        }

        // Task Manager's Startup tab records its disabled state here without deleting the shortcut.
        if (Registry.GetValue(StartupApprovedKey, Path.GetFileName(shortcutPath), null) is byte[] status && status.Length > 0 && status[0] == 3)
        {
            return StartupRegistrationState.DisabledByUser;
        }

        return StartupRegistrationState.Enabled;
    }

    public void SetEnabled(StartupRegistration registration, bool enabled)
    {
        string shortcutPath = GetShortcutPath(registration);

        // Like ShortcutHelpers, leave an existing shortcut alone when the target to point at is missing.
        if (enabled && !File.Exists(registration.ExecutablePath))
        {
            return;
        }

        if (File.Exists(shortcutPath))
        {
            File.Delete(shortcutPath);
        }

        if (enabled)
        {
            IWshShortcut shortcut = ((IWshShell)new WshShell()).CreateShortcut(shortcutPath);
            shortcut.TargetPath = registration.ExecutablePath;
            shortcut.Arguments = string.Join(" ", registration.Arguments.Select(QuoteArgument));
            shortcut.WorkingDirectory = Path.GetDirectoryName(registration.ExecutablePath) ?? "";
            shortcut.Save();
        }
    }

    private static bool IsShortcutTo(string shortcutPath, string targetPath)
    {
        if (string.IsNullOrEmpty(targetPath) || !File.Exists(shortcutPath))
        {
            return false;
        }

        try
        {
            string shortcutTarget = ((IWshShell)new WshShell()).CreateShortcut(shortcutPath).TargetPath;
            return !string.IsNullOrEmpty(shortcutTarget) && shortcutTarget.Equals(targetPath, StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception e) when (e is System.Runtime.InteropServices.COMException or UnauthorizedAccessException or IOException)
        {
            return false;
        }
    }

    internal static string QuoteArgument(string argument) =>
        argument.Length > 0 && argument.IndexOfAny([' ', '\t', '"']) < 0 ? argument : "\"" + argument.Replace("\"", "\\\"") + "\"";
}
