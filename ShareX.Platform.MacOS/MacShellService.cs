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

    // macOS uniform type identifiers have no simple extension to MIME table; callers fall back to their own.
    public string? GetMimeType(string extension) => null;

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
