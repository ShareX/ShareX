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

using ShareX.Platform.Imaging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Platform;

public sealed class UnsupportedApplicationLaunchService(string reason) : IApplicationLaunchService
{
    public FeatureSupport Support { get; } = FeatureSupport.NotSupported(reason);

    public string GetExecutablePath(string directory, string applicationName) => System.IO.Path.Combine(directory, applicationName);

    public int LaunchDetached(string executablePath, IReadOnlyList<string> arguments) =>
        throw new PlatformNotSupportedException(Support.Reason);
}

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

    public long GetActiveWindowHandle() => 0;

    public PlatformRectangle? GetWindowBounds(long windowHandle) => null;

    public PlatformRectangle? GetClientBounds(long windowHandle) => null;

    public double GetOwnWindowPixelScale(PlatformPoint point) => 1;

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

    public bool RestoreWindow(long windowHandle) => false;

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

    public FeatureSupport FileAssociationSupport => Support;

    public bool IsAssociated(FileAssociation association) => false;

    public void Associate(FileAssociation association) => throw new PlatformNotSupportedException(Support.Reason);

    public void RemoveAssociation(FileAssociation association)
    {
    }

    public FeatureSupport BrowserHostSupport => Support;

    public bool IsBrowserHostRegistered(BrowserHost host) => false;

    public void RegisterBrowserHost(BrowserHost host) => throw new PlatformNotSupportedException(Support.Reason);

    public void UnregisterBrowserHost(BrowserHost host)
    {
    }

    public FeatureSupport SendToSupport => Support;

    public bool IsInSendTo(string name, string executablePath) => false;

    public void SetInSendTo(string name, string executablePath, bool enabled)
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

    public string GetDefaultFFmpegPath(string applicationDirectory) => System.IO.Path.Combine(applicationDirectory, "ffmpeg");
}

public sealed class UnsupportedOcrService(string reason) : IOcrService
{
    public FeatureSupport Support { get; } = FeatureSupport.NotSupported(reason);

    public IReadOnlyList<OcrLanguage> GetLanguages() => Array.Empty<OcrLanguage>();

    public Task<string> RecognizeAsync(byte[] png, string languageTag, bool singleLine, CancellationToken cancellationToken = default) =>
        throw new PlatformNotSupportedException(Support.Reason);
}

public sealed class UnsupportedCodeSignatureService(string reason) : ICodeSignatureService
{
    public FeatureSupport Support { get; } = FeatureSupport.NotSupported(reason);

    public bool IsTrusted(string filePath) => false;
}

public sealed class UnsupportedThumbnailService(string reason) : IThumbnailService
{
    public FeatureSupport Support { get; } = FeatureSupport.NotSupported(reason);

    public byte[]? GetThumbnail(string path, int maxWidth, int maxHeight) => null;
}

public sealed class UnsupportedTaskbarService(string reason) : ITaskbarService
{
    public FeatureSupport Support { get; } = FeatureSupport.NotSupported(reason);

    public void SetProgressValue(int value, int maximum)
    {
    }

    public void SetProgressState(TaskbarProgressState state)
    {
    }
}

public sealed class UnsupportedSystemGraphicsService(string reason) : ISystemGraphicsService
{
    public FeatureSupport EmojiSupport { get; } = FeatureSupport.NotSupported(reason);

    public PixelBuffer? RenderEmoji(string text, int canvasSize, float fontSize) => null;

    public FeatureSupport CursorSupport { get; } = FeatureSupport.NotSupported(reason);

    public SystemCursorImage? GetSystemCursor(SystemCursor cursor, int? size = null) => null;
}

public sealed class UnsupportedPrintService(string reason) : IPrintService
{
    public FeatureSupport Support { get; } = FeatureSupport.NotSupported(reason);

    public FeatureSupport DialogSupport => Support;

    public bool IsPrinterInstalled(string printerName) => false;

    public PrintPageSetup GetPageSetup(PrintOptions options) => PrintPageSetup.Letter;

    public bool ShowPrintDialog(PrintOptions options) => false;

    public void Print(PrintOptions options, string documentName, Func<PrintPageSetup, int, PrintedPage?> renderPage) =>
        throw new PlatformNotSupportedException(Support.Reason);
}

public sealed class UnsupportedSoundService(string reason) : ISoundService
{
    public FeatureSupport Support { get; } = FeatureSupport.NotSupported(reason);

    public void Play(byte[] wav)
    {
    }

    public void PlayFile(string filePath)
    {
    }
}

public sealed class UnsupportedDesktopWallpaperService(string reason) : IDesktopWallpaperService
{
    public FeatureSupport Support { get; } = FeatureSupport.NotSupported(reason);

    public bool RequiresPrewarm => false;

    public DesktopWallpaper? GetWallpaper() => null;

    public void Prewarm()
    {
    }
}
