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

using ShareX.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.Desktop.Tests;

internal sealed class FakeCapture : IScreenCaptureService
{
    public FeatureSupport Support { get; set; } = FeatureSupport.Supported;

    public ScreenCaptureFeatures Features => ScreenCaptureFeatures.None;

    public Exception? Throw { get; set; }

    public ScreenCaptureRequest? LastRequest { get; private set; }

    public byte[] Png { get; set; } = [0x89, 0x50, 0x4E, 0x47, 1, 2, 3];

    public PermissionState GetPermissionState() => PermissionState.NotRequired;

    public bool RequestPermission() => true;

    public IReadOnlyList<ScreenInfo> GetScreens() =>
    [
        new ScreenInfo("a", "A", new PlatformRectangle(0, 0, 10, 10), new PlatformRectangle(0, 0, 10, 10), false, 1),
        new ScreenInfo("b", "B", new PlatformRectangle(10, 0, 10, 10), new PlatformRectangle(10, 0, 10, 10), true, 1)
    ];

    public Task<ScreenCaptureResult> CaptureAsync(ScreenCaptureRequest request, CancellationToken cancellationToken = default)
    {
        LastRequest = request;

        if (Throw != null)
        {
            throw Throw;
        }

        return Task.FromResult(new ScreenCaptureResult(Png, new PlatformRectangle(0, 0, 10, 10), "fake"));
    }
}

internal sealed class FakeClipboard : IClipboardService
{
    public FeatureSupport Support { get; } = FeatureSupport.Supported;

    public byte[]? Image { get; private set; }

    public string? Text { get; private set; }

    public Task<bool> SetTextAsync(string text, CancellationToken cancellationToken = default) { Text = text; return Task.FromResult(true); }

    public Task<string?> GetTextAsync(CancellationToken cancellationToken = default) => Task.FromResult(Text);

    public Task<bool> SetImageAsync(byte[] png, CancellationToken cancellationToken = default) { Image = png; return Task.FromResult(true); }

    public Task<byte[]?> GetImageAsync(CancellationToken cancellationToken = default) => Task.FromResult(Image);

    public Task<bool> SetFilesAsync(IReadOnlyList<string> paths, CancellationToken cancellationToken = default) => Task.FromResult(true);

    public Task<IReadOnlyList<string>> GetFilesAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<string>>([]);

    public Task<bool> ClearAsync(CancellationToken cancellationToken = default) { Text = null; Image = null; return Task.FromResult(true); }

    public Task<IReadOnlyList<string>> GetFormatsAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<string>>([]);

    public Task<byte[]?> GetDataAsync(string format, CancellationToken cancellationToken = default) => Task.FromResult<byte[]?>(null);
}

internal sealed class FakeNotifications : INotificationService
{
    public FeatureSupport Support { get; } = FeatureSupport.Supported;

    public List<PlatformNotification> Shown { get; } = new List<PlatformNotification>();

    public Task<bool> ShowAsync(PlatformNotification notification, CancellationToken cancellationToken = default) { Shown.Add(notification); return Task.FromResult(true); }
}

internal sealed class FakePaths(string root) : IPathService
{
    public string GetConfigDirectory(string applicationName) => Path.Combine(root, "config", applicationName);

    public string GetDataDirectory(string applicationName) => Path.Combine(root, "data", applicationName);

    public string GetCacheDirectory(string applicationName) => Path.Combine(root, "cache", applicationName);

    public string GetDefaultPersonalFolder(string applicationName) => GetConfigDirectory(applicationName);

    public string GetPicturesDirectory() => Path.Combine(root, "Pictures");

    public string GetVideosDirectory() => Path.Combine(root, "Videos");

    public string GetDocumentsDirectory() => Path.Combine(root, "Documents");

    public string GetDesktopDirectory() => Path.Combine(root, "Desktop");
}

internal sealed class NullStartup : IStartupService
{
    public FeatureSupport Support { get; } = FeatureSupport.Supported;

    public StartupRegistrationState GetState(StartupRegistration registration) => StartupRegistrationState.Disabled;

    public void SetEnabled(StartupRegistration registration, bool enabled)
    {
    }
}

internal sealed class NullRecording : IScreenRecordingService
{
    public FeatureSupport Support { get; } = FeatureSupport.NotSupported("fake");

    public IReadOnlyList<string> GetSupportedDevices() => [];

    public FFmpegVideoInput CreateVideoInput(ScreenRecordingRequest request) => throw new NotSupportedException();

    public void PrepareDevice(string device, ScreenRecordingRequest request)
    {
    }
}

internal sealed class NullShell : IShellService
{
    public bool OpenUrl(string url) => true;

    public bool OpenPath(string path) => true;

    public bool RevealInFileManager(string path) => true;

    public string? GetMimeType(string extension) => null;
}

internal sealed class NullCredentials : ICredentialService
{
    public FeatureSupport Support { get; } = FeatureSupport.NotSupported("fake");

    public Task<bool> StoreAsync(string service, string account, string secret, CancellationToken cancellationToken = default) => Task.FromResult(false);

    public Task<string?> GetAsync(string service, string account, CancellationToken cancellationToken = default) => Task.FromResult<string?>(null);

    public Task<bool> DeleteAsync(string service, string account, CancellationToken cancellationToken = default) => Task.FromResult(false);
}

/// <summary>Real encryption that a test can make fail, to check that nothing falls back to plain text.</summary>
internal sealed class SwitchableSecrets(ISecretProtectionService inner) : ISecretProtectionService
{
    public bool Fail { get; set; }

    public byte[] Protect(byte[] data, byte[]? entropy = null) =>
        Fail ? throw new System.Security.Cryptography.CryptographicException("fake failure") : inner.Protect(data, entropy);

    public byte[] Unprotect(byte[] protectedData, byte[]? entropy = null) =>
        Fail ? throw new System.Security.Cryptography.CryptographicException("fake failure") : inner.Unprotect(protectedData, entropy);
}

internal sealed class FakeSystemInfo : ISystemInfoService
{
    public string OperatingSystemName => "Test OS";

    public bool IsElevated => false;

    public bool IsAdministratorGroupMember => false;

    public bool IsTabletMode => false;
}

internal sealed class FakePlatform : IPlatformServices
{
    public FakePlatform(string root)
    {
        Paths = new FakePaths(root);
        SecretsFake = new SwitchableSecrets(new KeyFileSecretProtectionService(Path.Combine(root, "secret.key")));
    }

    public FakeCapture Capture { get; } = new FakeCapture();

    public FakeClipboard ClipboardFake { get; } = new FakeClipboard();

    public FakeNotifications NotificationsFake { get; } = new FakeNotifications();

    public PlatformInfo Info { get; } = new PlatformInfo(OperatingSystemKind.Linux, DisplayServer.Wayland, DesktopEnvironment.Hyprland, "Hyprland", false);

    public IPathService Paths { get; }

    public IStartupService Startup { get; } = new NullStartup();

    public IClipboardService Clipboard => ClipboardFake;

    public IScreenCaptureService ScreenCapture => Capture;

    public IScreenRecordingService ScreenRecording { get; } = new NullRecording();

    public IHotkeyService Hotkeys { get; } = new UnsupportedHotkeyService("fake");

    public IWindowService Windows { get; } = new UnsupportedWindowService("fake");

    public IInputService Input { get; } = new UnsupportedInputService("fake");

    public IWindowManagementService WindowManagement { get; } = new UnsupportedWindowManagementService("fake");

    public INotificationService Notifications => NotificationsFake;

    public IShellService Shell { get; } = new NullShell();

    public IShellIntegrationService ShellIntegration { get; } = new UnsupportedShellIntegrationService("fake");

    public ICredentialService Credentials { get; } = new NullCredentials();

    public SwitchableSecrets SecretsFake { get; }

    public ISecretProtectionService Secrets => SecretsFake;

    public IThumbnailService Thumbnails { get; } = new UnsupportedThumbnailService("fake");

    public ISystemPreferencesService Preferences { get; } = new DefaultSystemPreferencesService();

    public ISystemInfoService SystemInfo { get; } = new FakeSystemInfo();

    public IOcrService Ocr { get; } = new UnsupportedOcrService("fake");

    public void Dispose()
    {
    }
}
