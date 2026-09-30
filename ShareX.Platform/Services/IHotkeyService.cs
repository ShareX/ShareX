using System;

namespace ShareX.Platform;

/// <summary>
/// System wide hotkeys: RegisterHotKey on Windows, Carbon hot keys on macOS, XGrabKey on X11 and the
/// GlobalShortcuts portal on Wayland.
/// </summary>
/// <remarks><see cref="HotkeyPressed"/> may be raised on a background thread. Marshal to the UI thread before touching UI.</remarks>
public interface IHotkeyService : IDisposable
{
    FeatureSupport Support { get; }

    event EventHandler<HotkeyPressedEventArgs>? HotkeyPressed;

    /// <param name="id">Caller chosen identifier, unique per registration.</param>
    HotkeyRegistrationStatus Register(int id, PlatformHotkey hotkey);

    bool Unregister(int id);

    void UnregisterAll();
}
