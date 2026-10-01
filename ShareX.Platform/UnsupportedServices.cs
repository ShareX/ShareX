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

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform;

/// <summary>Placeholder implementations used when a platform or session cannot provide a feature.</summary>
public sealed class UnsupportedNotificationService(string reason) : INotificationService
{
    public FeatureSupport Support { get; } = FeatureSupport.NotSupported(reason);

    public Task<bool> ShowAsync(PlatformNotification notification, CancellationToken cancellationToken = default) => Task.FromResult(false);
}

public sealed class UnsupportedHotkeyService(string reason) : IHotkeyService
{
    public FeatureSupport Support { get; } = FeatureSupport.NotSupported(reason);

    public event EventHandler<HotkeyPressedEventArgs>? HotkeyPressed { add { } remove { } }

    public HotkeyRegistrationStatus Register(int id, PlatformHotkey hotkey) => HotkeyRegistrationStatus.NotSupported;

    public bool Unregister(int id) => false;

    public void UnregisterAll()
    {
    }

    public void Dispose()
    {
    }
}

public sealed class UnsupportedWindowService(string reason) : IWindowService
{
    public FeatureSupport Support { get; } = FeatureSupport.NotSupported(reason);

    public IReadOnlyList<PlatformWindow> GetWindows() => Array.Empty<PlatformWindow>();

    public PlatformWindow? GetActiveWindow() => null;

    public PlatformPoint? GetCursorPosition() => null;

    public void ReleaseMouseCapture()
    {
    }

    public bool ConfineCursor(long windowHandle) => false;

    public void ReleaseCursorConfinement()
    {
    }

    public IReadOnlyList<SnapTarget> GetSnapTargets(bool includeControls, long ignoredHandle, System.Threading.CancellationToken cancellationToken = default) =>
        Array.Empty<SnapTarget>();

    public bool SetCursorPosition(PlatformPoint position) => false;

    public bool ActivateWindow(long windowHandle) => false;

    public bool SetOverlayStyle(long windowHandle, bool clickThrough) => false;

    public bool SetWindowShape(long windowHandle, IReadOnlyList<PlatformRectangle> visibleAreas) => false;

    public FeatureSupport OverlaySupport => Support;

    public IScreenOverlay CreateOverlay(PlatformRectangle screenBounds) => throw new PlatformNotSupportedException(Support.Reason);
}

public sealed class UnsupportedInputService(string reason) : IInputService
{
    public FeatureSupport KeyboardSupport { get; } = FeatureSupport.NotSupported(reason);

    public FeatureSupport MouseWheelSupport { get; } = FeatureSupport.NotSupported(reason);

    public FeatureSupport WindowScrollSupport { get; } = FeatureSupport.NotSupported(reason);

    public bool SendKeyPress(int virtualKey) => false;

    public bool SendMouseWheel(int detents) => false;

    public bool ScrollWindow(long windowHandle, WindowScrollCommand command) => false;

    public FeatureSupport MouseHookSupport { get; } = FeatureSupport.NotSupported(reason);

    public IDisposable HookMouse(IGlobalMouseListener listener) => throw new PlatformNotSupportedException(MouseHookSupport.Reason);
}

public sealed class UnsupportedShellIntegrationService(string reason) : IShellIntegrationService
{
    public FeatureSupport Support { get; } = FeatureSupport.NotSupported(reason);

    public bool IsRegistered(ShellMenuEntry entry) => false;

    public void Register(ShellMenuEntry entry) => throw new PlatformNotSupportedException(Support.Reason);

    public void Unregister(ShellMenuEntry entry)
    {
    }
}

public sealed class UnsupportedScreenRecordingService(string reason) : IScreenRecordingService
{
    public FeatureSupport Support { get; } = FeatureSupport.NotSupported(reason);

    public IReadOnlyList<string> GetSupportedDevices() => Array.Empty<string>();

    public FFmpegVideoInput CreateVideoInput(ScreenRecordingRequest request) => throw new PlatformNotSupportedException(Support.Reason);

    public void PrepareDevice(string device, ScreenRecordingRequest request)
    {
    }
}

public sealed class UnsupportedOcrService(string reason) : IOcrService
{
    public FeatureSupport Support { get; } = FeatureSupport.NotSupported(reason);

    public IReadOnlyList<OcrLanguage> GetLanguages() => Array.Empty<OcrLanguage>();

    public Task<string> RecognizeAsync(byte[] png, string languageTag, bool singleLine, CancellationToken cancellationToken = default) =>
        throw new PlatformNotSupportedException(Support.Reason);
}

public sealed class UnsupportedThumbnailService(string reason) : IThumbnailService
{
    public FeatureSupport Support { get; } = FeatureSupport.NotSupported(reason);

    public byte[]? GetThumbnail(string path, int maxWidth, int maxHeight) => null;
}
