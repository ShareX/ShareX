# Cross-platform architecture

ShareX moves operating system specific code behind a platform abstraction layer so the same application runs on Windows, macOS and Linux. Requirements are tracked in [#8867](https://github.com/ShareX/ShareX/issues/8867). Work happens on `cross-platform-v2`; who does what is in [cross-platform-delegation.md](cross-platform-delegation.md), and why things are done this way is in [cross-platform-learnings.md](cross-platform-learnings.md).

This document describes the **target** architecture. At the branch point (`develop` `fd61635f2`) WinForms and GDI+ are already gone, the platform projects are not on this branch yet (task M1), and the libraries still target `net10.0-windows`. The migration debt list in [AGENTS.md](../AGENTS.md) tracks the gap.

## Projects

| Project | Target | Purpose |
| --- | --- | --- |
| `ShareX.Platform` | `net10.0` | Interfaces, `PlatformServices`, `PlatformDetector`, `PlatformInfo`, `LinuxDistribution`, `LinuxPackages`, portable models and helpers. No operating system calls. |
| `ShareX.Platform.Windows` | `net10.0` and `net10.0-windows10.0.22621.0` | Win32, COM and WinRT implementations. Behaviour is identical to the code they replace. The Windows SDK target adds WinRT features (OCR). |
| `ShareX.Platform.MacOS` | `net10.0` | AppKit, CoreGraphics, Carbon, Keychain and LaunchAgent implementations. |
| `ShareX.Platform.Linux` | `net10.0` | X11, Wayland (portals, wlroots tools, Hyprland and sway IPC), freedesktop.org (XDG, D-Bus, Secret Service, thumbnails), Tesseract. |
| `ShareX.Platform.Tests` | `net10.0` | xUnit tests for parsers and argument building; run on all three operating systems. |
| `ShareX.HelpersLib`, `ShareX.UploadersLib`, `ShareX.HistoryLib`, `ShareX.ImageEffectsLib`, `ShareX.ScreenCaptureLib`, `ShareX.Tools`, `ShareX.ImageEditor`, `ShareX.Avalonia`, `ShareX.NativeMessagingHost` | `net10.0` | Portable code. Anything that needs the operating system goes through `PlatformServices.Current`. |
| `ShareX` | `net10.0` and `net10.0-windows10.0.22621.0` | The application. Chooses the platform services at start up; contains no operating system code itself. |
| `ShareX.Setup`, `ShareX.Steam` | Windows only | Installer and Steam launcher. |

## Services

`IPlatformServices` exposes one service per area. Every service reports a `FeatureSupport` value; when a feature is unavailable the reason is user facing and, on Linux, names the package and the install command for the running distribution.

| Service | Windows | macOS | Linux X11 | Linux Wayland |
| --- | --- | --- | --- | --- |
| `IPathService` | Known folders | `~/Library` | XDG base directories | XDG base directories |
| `IStartupService` | Run registry key | LaunchAgent plist | XDG autostart `.desktop` | XDG autostart `.desktop` |
| `IClipboardService` | Win32 clipboard | NSPasteboard | xclip or xsel | wl-clipboard |
| `IScreenCaptureService` | GDI, HDR tone mapping, window and transparent window capture | `screencapture` (Screen Recording permission) | Xlib | grim on wlroots, otherwise the xdg-desktop-portal Screenshot interface |
| `IScreenRecordingService` | FFmpeg gdigrab, ddagrab, DirectShow | FFmpeg avfoundation | FFmpeg x11grab | Not yet (portal ScreenCast) |
| `IHotkeyService` | RegisterHotKey | Carbon hot keys | XGrabKey | Portal GlobalShortcuts |
| `IWindowService` | Win32 windows and child controls, cursor, overlays | CGWindowList | EWMH, XShape | Hyprland and sway IPC only |
| `IWindowManagementService` | Inspect, top most, opacity, borderless | Not yet | EWMH | Hyprland and sway IPC only |
| `IInputService` | SendInput, WM_VSCROLL, low-level mouse hook | Quartz events (Accessibility permission) | XTEST | Hyprland key shortcuts only |
| `IOcrService` | Windows.Media.Ocr | Not yet (Vision) | Tesseract | Tesseract |
| `INotificationService` | ShareX notification window | ShareX notification window | org.freedesktop.Notifications or notify-send | Same as X11 |
| `IShellService` | ShellExecute and Explorer select | `open` and `open -R` | xdg-open and FileManager1 D-Bus | Same as X11 |
| `IShellIntegrationService` | Explorer context menu keys under HKCU | Not supported | File manager `.desktop` actions | Same as X11 |
| `ICredentialService` | Credential Manager | Keychain | Secret Service (secret-tool) | Same as X11 |
| `ISecretProtectionService` | DPAPI | AES-GCM with an owner-only key file | AES-GCM with an owner-only key file | Same as X11 |
| `IThumbnailService` | Explorer shell thumbnails | Not yet | freedesktop.org thumbnail cache | Same as X11 |
| `ISystemInfoService`, `ISystemPreferencesService` | OS name, elevation, wheel lines, small icon size, task bar theme | `sw_vers`, defaults | os-release, defaults | Same as X11 |

## Start up

`Program` calls `PlatformServices.Initialize` once with `WindowsPlatformServices`, `MacPlatformServices` or `LinuxPlatformServices`, chosen by operating system. That is the only operating system check in shared code. After that everything uses `PlatformServices.Current`, and `PlatformServices.Shutdown` disposes it on exit.

## Linux distribution awareness

`PlatformDetector` reads `/etc/os-release` into `LinuxDistribution` (family, immutable variants, package manager) and detects the display server and desktop environment. `LinuxPackages.GetInstallCommand` turns missing tools into a copy and paste command for apt, dnf, rpm-ostree, pacman, zypper, nix, apk, emerge, xbps and eopkg.

## Building

.NET 10 SDK on every operating system.

```sh
dotnet build ShareX.sln -c Release -p:Platform=x64
dotnet test ShareX.Platform.Tests
```

Pass `-p:Platform=x64` (or `ARM64`); a plain `dotnet build ShareX.sln` uses `Any CPU`, which the projects do not configure. Building on Linux and macOS needs `Directory.Build.props` (capital B) with `EnableWindowsTargeting` set off Windows; that is task M0.
