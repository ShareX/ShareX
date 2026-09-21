#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/

#endregion License Information (GPL v3)

#nullable enable

using System;
using System.Threading.Tasks;

namespace ShareX;

internal static class ApplicationCommandLine
{
    private static ShareXCLIManager? _initial;

    private static ShareXCLIManager Initial => _initial ??
        throw new InvalidOperationException("Command-line processing has not been initialized.");

    internal static void Initialize(string[] args)
    {
        _initial = new ShareXCLIManager(args);
        _initial.ParseCommands();
        StartupOptions.AutoClose = _initial.IsCommandExist("AutoClose");
    }

    internal static bool IsCommandPresent(params string[] commands) => Initial.IsCommandExist(commands);

    internal static bool TryHandleUninstall()
    {
        if (!IsCommandPresent("uninstall"))
        {
            return false;
        }

        try
        {
            IntegrationHelpers.Uninstall();
        }
        catch
        {
        }

        return true;
    }

    internal static Task ExecuteInitialAsync() => Initial.UseCommandLineArgs();

    internal static async Task ExecuteReceivedAsync(string[]? args)
    {
        args ??= [];

        if (args.Length == 0)
        {
            if (ApplicationState.Settings.ShowTray)
            {
                // Workaround for Windows startup tray icon bug.
                MainWindowIntegration.SetTrayVisible(false);
                MainWindowIntegration.SetTrayVisible(true);
            }

            MainWindowIntegration.Activate();
        }
        else if (MainWindowIntegration.IsVisible)
        {
            MainWindowIntegration.Activate();
        }

        ShareXCLIManager commandLine = new(args);
        commandLine.ParseCommands();
        await commandLine.UseCommandLineArgs();
    }
}
