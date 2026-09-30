using ShareX.Platform.Diagnostics;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform.MacOS;

/// <summary>open(1) for URLs and files, and Finder selection with open -R.</summary>
public sealed class MacShellService : IShellService
{
    private readonly ICommandRunner runner;

    public MacShellService(ICommandRunner runner)
    {
        this.runner = runner;
    }

    public bool OpenUrl(string url) => Run("open", [url]);

    public bool OpenPath(string path) => Run("open", [path]);

    public bool RevealInFileManager(string path) => Run("open", ["-R", path]);

    private bool Run(string command, IReadOnlyList<string> arguments)
    {
        try
        {
            return runner.RunAsync(command, arguments, timeout: TimeSpan.FromSeconds(10)).GetAwaiter().GetResult().Success;
        }
        catch (Exception e) when (e is TimeoutException or System.ComponentModel.Win32Exception)
        {
            return false;
        }
    }
}

/// <summary>Notification Center banners through AppleScript's display notification.</summary>
/// <remarks>A signed app bundle can switch to UNUserNotificationCenter later to show its own icon and handle clicks.</remarks>
public sealed class MacNotificationService : INotificationService
{
    private readonly ICommandRunner runner;

    public MacNotificationService(ICommandRunner runner)
    {
        this.runner = runner;
    }

    public FeatureSupport Support => FeatureSupport.Supported;

    public async Task<bool> ShowAsync(PlatformNotification notification, CancellationToken cancellationToken = default)
    {
        // Text is passed as script arguments so it never needs AppleScript escaping.
        string[] arguments =
        [
            "-e", "on run argv",
            "-e", "display notification (item 2 of argv) with title (item 1 of argv)",
            "-e", "end run",
            notification.Title,
            notification.Message
        ];

        try
        {
            return (await runner.RunAsync("osascript", arguments, cancellationToken: cancellationToken).ConfigureAwait(false)).Success;
        }
        catch (Exception e) when (e is TimeoutException or System.ComponentModel.Win32Exception)
        {
            return false;
        }
    }
}
