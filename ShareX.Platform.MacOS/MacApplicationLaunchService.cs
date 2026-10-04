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

using ShareX.Platform.MacOS.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;

namespace ShareX.Platform.MacOS;

/// <summary>
/// Starts ShareX for the browser extension's native messaging host in a session of its own (posix_spawn with POSIX_SPAWN_SETSID),
/// so it outlives the host, with stdio on /dev/null so it does not hold the browser's message pipe.
/// </summary>
[SupportedOSPlatform("macos")]
public sealed class MacApplicationLaunchService : IApplicationLaunchService
{
    public FeatureSupport Support => FeatureSupport.Supported;

    /// <summary>Inside ShareX.app both executables sit in Contents/MacOS without an extension.</summary>
    public string GetExecutablePath(string directory, string applicationName)
    {
        ArgumentException.ThrowIfNullOrEmpty(directory);
        ArgumentException.ThrowIfNullOrEmpty(applicationName);
        if (!Path.IsPathFullyQualified(directory))
        {
            throw new ArgumentException("The installation directory must be absolute.", nameof(directory));
        }
        if (applicationName.Contains('/') || applicationName.Contains('\0') || applicationName is "." or "..")
        {
            throw new ArgumentException("The application name must be a file name without a directory.", nameof(applicationName));
        }
        return Path.Combine(directory, applicationName);
    }

    public int LaunchDetached(string executablePath, IReadOnlyList<string> arguments)
    {
        ArgumentException.ThrowIfNullOrEmpty(executablePath);
        ArgumentNullException.ThrowIfNull(arguments);
        if (!Path.IsPathFullyQualified(executablePath) || executablePath.Contains('\0'))
        {
            throw new ArgumentException("The executable path must be absolute and contain no null characters.", nameof(executablePath));
        }
        if (arguments.Any(argument => argument is null || argument.Contains('\0')))
        {
            throw new ArgumentException("Arguments must contain no null values or characters.", nameof(arguments));
        }

        string[] environment = Environment.GetEnvironmentVariables().Cast<DictionaryEntry>()
            .Select(entry => $"{entry.Key}={entry.Value}").Where(variable => !variable.Contains('\0')).ToArray();
        return MacSpawn.SpawnDetached(executablePath, [executablePath, .. arguments], environment);
    }
}
