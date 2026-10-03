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

using System.Collections.Generic;

namespace ShareX.Platform;

/// <summary>Values match Windows.ApplicationModel.StartupTaskState so they can be cast directly.</summary>
public enum StartupRegistrationState
{
    Disabled = 0,
    DisabledByUser = 1,
    Enabled = 2,
    DisabledByPolicy = 3,
    EnabledByPolicy = 4
}

/// <summary>Describes how the application should be launched when the user signs in.</summary>
/// <param name="Name">Short identifier, for example "ShareX". Used for the shortcut, plist label or .desktop file name.</param>
/// <param name="DisplayName">Human readable name shown by the OS.</param>
/// <param name="ExecutablePath">Absolute path of the executable to launch.</param>
/// <param name="Arguments">Command line arguments, for example "-silent".</param>
public sealed record StartupRegistration(string Name, string DisplayName, string ExecutablePath, IReadOnlyList<string> Arguments)
{
    /// <summary>Reverse DNS identifier used by macOS launch agents.</summary>
    public string? BundleIdentifier { get; init; }

    /// <summary>Optional icon name or path for XDG autostart entries.</summary>
    public string? IconName { get; init; }
}

/// <summary>Launch at sign in: Startup folder shortcut on Windows, LaunchAgents on macOS, XDG autostart on Linux.</summary>
public interface IStartupService
{
    FeatureSupport Support { get; }

    StartupRegistrationState GetState(StartupRegistration registration);

    /// <summary>Enables or disables launch at sign in. Throws when the platform refuses the change.</summary>
    void SetEnabled(StartupRegistration registration, bool enabled);
}
