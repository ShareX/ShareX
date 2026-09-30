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
