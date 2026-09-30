# Cross platform architecture

ShareX 23 moves operating system specific code behind a platform abstraction layer so the same application can run on Windows, macOS and Linux. Requirements are tracked in [#8867](https://github.com/ShareX/ShareX/issues/8867).

## Projects

| Project | Target | Purpose |
| --- | --- | --- |
| `ShareX.Platform` | `net10.0` | Interfaces, `PlatformDetector`, `PlatformInfo`, `LinuxDistribution`, `LinuxPackages`, shared helpers. No OS calls. |
| `ShareX.Platform.Windows` | `net10.0` (Windows only) | Win32 implementations. Behaviour is identical to the code it replaces. |
| `ShareX.Platform.MacOS` | `net10.0` | AppKit, Carbon, Keychain and LaunchAgent implementations. |
| `ShareX.Platform.Linux` | `net10.0` | X11, Wayland, freedesktop.org (XDG, D-Bus, Secret Service) implementations. |
| `ShareX.Platform.Tests` | `net10.0` | xUnit tests that run on all three operating systems. |

`IPlatformServices` exposes one service per area:

| Service | Windows | macOS | Linux X11 | Linux Wayland |
| --- | --- | --- | --- | --- |
| `IPathService` | Known folders | `~/Library` | XDG base directories | XDG base directories |
| `IStartupService` | Run registry key | LaunchAgent plist | XDG autostart `.desktop` | XDG autostart `.desktop` |
| `IClipboardService` | Win32 clipboard | NSPasteboard | xclip or xsel | wl-clipboard |
| `IScreenCaptureService` | GDI | CoreGraphics (needs Screen Recording permission) | Xlib | grim, or the xdg-desktop-portal Screenshot interface |
| `IScreenRecordingService` | FFmpeg gdigrab or ddagrab | FFmpeg avfoundation | FFmpeg x11grab | Not yet (portal ScreenCast pending) |
| `IHotkeyService` | RegisterHotKey | Carbon hot keys | XGrabKey | Not yet (portal GlobalShortcuts pending) |
| `IWindowService` | Win32 window enumeration | CGWindowList | EWMH | Limited by the compositor |
| `INotificationService` | ShareX notification window | ShareX notification window | org.freedesktop.Notifications or notify-send | Same as X11 |
| `IShellService` | ShellExecute and Explorer select | `open` and `open -R` | xdg-open and FileManager1 D-Bus | Same as X11 |
| `IShellIntegrationService` | Explorer context menu keys under HKCU | Not supported | File manager `.desktop` actions | Same as X11 |
| `ICredentialService` | Credential Manager | Keychain | Secret Service (secret-tool) | Same as X11 |

Every service reports a `FeatureSupport` value. When a feature is unavailable the reason is user facing and, on Linux, names the package and the install command for the running distribution.

## Start up

`Program` calls `PlatformServices.Initialize` once. The Windows application passes `WindowsPlatformServices`. macOS and Linux hosts pass `MacPlatformServices` or `LinuxPlatformServices`. After that the rest of the application uses `PlatformServices.Current`. `PlatformServices.Shutdown` disposes it on exit. Code that still needs Win32 directly is guarded with `OperatingSystem.IsWindows()`.

## Linux distribution awareness

`PlatformDetector` reads `/etc/os-release` (falling back to `/usr/lib/os-release`) into `LinuxDistribution`:

* `Id`, `IdLike`, `VariantId`, `PrettyName` and `VersionId` come straight from os-release.
* `Family` is resolved from `ID` and then `ID_LIKE`: Debian, Fedora, Arch, openSUSE, NixOS, Alpine, Gentoo, Void or Solus. Derivatives such as Ubuntu, Mint, Pop!_OS, Nobara, Bazzite, EndeavourOS, Manjaro, CachyOS and Omarchy map to their family.
* `IsImmutable` is set for rpm-ostree variants (Silverblue, Kinoite and similar), NixOS and systems booted with ostree.
* `PackageManager` is chosen from the family, with `rpm-ostree` on immutable Fedora.

It also detects the display server (`XDG_SESSION_TYPE`, `WAYLAND_DISPLAY`, `DISPLAY`), the desktop environment (`XDG_CURRENT_DESKTOP`).

`LinuxPackages.GetInstallCommand` turns missing helper tools into a copy and paste command:

| Family | Example |
| --- | --- |
| Debian and Ubuntu | `sudo apt install wl-clipboard libnotify-bin libsecret-tools ffmpeg` |
| Fedora | `sudo dnf install wl-clipboard libnotify libsecret ffmpeg-free` |
| Fedora Atomic | `rpm-ostree install wl-clipboard libnotify libsecret ffmpeg-free` |
| Arch and derivatives | `sudo pacman -S --needed wl-clipboard libnotify libsecret ffmpeg` |
| openSUSE | `sudo zypper install wl-clipboard libnotify-tools libsecret-tools ffmpeg` |
| NixOS | `nix-env -iA nixos.wl-clipboard nixos.libnotify nixos.libsecret nixos.ffmpeg` |
| Alpine | `sudo apk add wl-clipboard libnotify libsecret ffmpeg` |
| Gentoo | `sudo emerge --ask wl-clipboard libnotify libsecret ffmpeg` |
| Void | `sudo xbps-install wl-clipboard libnotify libsecret ffmpeg` |
| Solus | `sudo eopkg install wl-clipboard libnotify libsecret ffmpeg` |

Replace `wl-clipboard` with `xclip` on X11. Wayland capture also uses `grim`, `slurp` and `xdg-desktop-portal` with the backend for your desktop.

## Building

.NET 10 SDK is required on every OS.

```sh
# Platform libraries and tests, any OS
dotnet build ShareX.Platform.Linux
dotnet build ShareX.Platform.MacOS
dotnet build ShareX.Platform.Windows -p:EnableWindowsTargeting=true
dotnet test ShareX.Platform.Tests
dotnet test ShareX.Destinations.Tests

# The application
dotnet build ShareX/ShareX.csproj -c Release -p:Platform=x64
```

On Linux or macOS add `-p:EnableWindowsTargeting=true` to build projects that still target Windows. The `Portable libraries` workflow (`.github/workflows/platform.yml`) builds and tests the platform projects on Ubuntu, macOS and Windows.

## Status

Done: the abstraction, all three implementations, distribution detection and install hints, application wiring for paths, start up and shell integration, tests and CI.

Open work:

* Wayland screen recording through the portal ScreenCast interface and PipeWire.
* Wayland global hotkeys through the portal GlobalShortcuts interface.
* Moving the remaining WinForms and HelpersLib Win32 calls behind the services.
* Packaging: MSIX and installer on Windows, a signed `.app` on macOS, Flatpak, AppImage, `.deb` and `.rpm` on Linux.
* Runtime testing of the Avalonia UI on macOS and Linux desktops.
