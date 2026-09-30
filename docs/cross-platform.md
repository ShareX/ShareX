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
| `ShareX.HelpersLib`, `ShareX.UploadersLib`, `ShareX.HistoryLib`, `ShareX.Destinations` | `net10.0` | Portable core. No WinForms, WPF or Win32 code. Anything that needs the operating system goes through `ShareX.Platform`. |
| `ShareX.HelpersLib.Windows` | `net10.0-windows` | WinForms, Win32 and GDI+ helpers that only the Windows application uses. Same namespaces as `HelpersLib`, so Windows code is unchanged. |
| `ShareX.Desktop` | `net10.0` | The cross-platform application (`sharex`): tray icon, command line, capture, upload, history and the image editor. Runs on Windows, macOS and Linux. |
| `ShareX.Desktop.Tests` | `net10.0` | xUnit tests for the application, including the real uploaders against a local server. |

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
| `ISecretProtectionService` | DPAPI | AES-GCM with an owner-only key file | AES-GCM with an owner-only key file | Same as X11 |
| `IThumbnailService` | Explorer shell thumbnails | Not yet | freedesktop.org thumbnail cache | Same as X11 |

Every service reports a `FeatureSupport` value. When a feature is unavailable the reason is user facing and, on Linux, names the package and the install command for the running distribution.

## Code that is shared and code that is not

Shared libraries target plain `net10.0` and never reference WinForms, WPF or Win32. When shared code needs the operating system it asks `PlatformServices.Current`: settings secrets use `Secrets`, copy to clipboard uses `Clipboard`, "show in folder" uses `Shell`, file thumbnails use `Thumbnails`.

Code that can only work on Windows lives in `ShareX.HelpersLib.Windows` (or, for real operating system services, in `ShareX.Platform.Windows`). A class that mixes both keeps its portable members in `HelpersLib` and exposes the Windows members through C# 14 extension members in the same namespace, so `Helpers.CreateCursor(...)` still compiles in the Windows application. Call sites that use an alias for the type (`using Helpers = ShareX.HelpersLib.Helpers;`) also need `using ShareX.HelpersLib;`.

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

## Running ShareX on Linux

`ShareX.Desktop` is the application that runs on Linux today. It needs the tools listed by `sharex doctor`; on Wayland compositors based on wlroots (Hyprland, Sway) that is `grim`, `slurp` and `wl-clipboard`.

```sh
Scripts/install-linux.sh              # build and install to ~/.local
Scripts/install-linux.sh --autostart  # also start when you log in
Scripts/install-linux.sh --uninstall

sharex                     # start the tray application
sharex capture region      # select an area (also: fullscreen, screen)
sharex capture region --upload --edit
sharex upload FILE...
sharex editor [FILE]
sharex history
sharex doctor              # what works here, and what to install for the rest
sharex hotkeys             # key binding lines for Hyprland, Sway or your desktop
sharex config              # where settings, upload accounts and history are stored
```

The first `sharex` command starts the tray application. Later commands are handed to it, which is how a key binding that runs `sharex capture region` works without starting a second copy.

Screenshots go to `~/Pictures/ShareX/<yyyy-MM>/` and are copied to the clipboard. **Nothing is uploaded until you choose a destination**: set `ImageUploader` (for example `CustomImageUploader`) and `UploadImage` in `~/.config/ShareX/DesktopSettings.json`. Upload accounts live in `UploadersConfig.json`, the same format the Windows application uses. Settings without secrets (custom uploaders, FTP hosts, S3 buckets) can be copied across, but passwords and tokens the Windows application saved are encrypted with DPAPI for that Windows account, so they load as empty on Linux and macOS: sign in to those destinations again. ShareX saves secrets encrypted on every platform and refuses to save rather than write one in plain text.

## Building

.NET 10 SDK is required on every OS.

```sh
# Platform libraries and tests, any OS
dotnet build ShareX.Platform.Linux
dotnet build ShareX.Platform.MacOS
dotnet build ShareX.Platform.Windows -p:EnableWindowsTargeting=true
dotnet test ShareX.Platform.Tests
dotnet test ShareX.Destinations.Tests

# The whole solution, on any OS
dotnet build ShareX.sln -c Release -p:Platform=x64

# The cross-platform application and its tests
dotnet test ShareX.Desktop.Tests
dotnet run --project ShareX.Desktop -- doctor
```

`Directory.Build.props` sets `EnableWindowsTargeting` on non-Windows hosts, so every project, including the WinForms application, compiles on Linux and macOS. Projects that target `net10.0-windows` compile there but only run on Windows until their WinForms and Win32 code is migrated to Avalonia and the platform services. Pass `-p:Platform=x64` (or `ARM64`); a plain `dotnet build ShareX.sln` uses the `Any CPU` platform, which the Windows projects do not configure.

The `Portable libraries` workflow (`.github/workflows/platform.yml`) builds and tests the platform projects on Ubuntu, macOS and Windows.

## Status

Done: the abstraction, all three implementations, distribution detection and install hints, application wiring for paths, start up and shell integration, tests and CI. The core libraries (`HelpersLib`, `UploadersLib`, `HistoryLib`, `Destinations`) are portable, and `ShareX.Desktop` runs capture, upload, history and the editor on Linux.

Open work:

* Wayland screen recording through the portal ScreenCast interface and PipeWire.
* Wayland global hotkeys through the portal GlobalShortcuts interface.
* Region selection on X11 (Wayland uses `slurp` or the portal; X11 needs ShareX's own overlay).
* The main window, settings and workflow editor in `ShareX.Desktop`; today it is configured through JSON files and the tray menu.
* Porting `ScreenCaptureLib`, `ImageEffectsLib` and `Tools` off WinForms and GDI+ so the Windows application can move to `ShareX.Desktop` too.
* Packaging: MSIX and installer on Windows, a signed `.app` on macOS, Flatpak, AppImage, `.deb` and `.rpm` on Linux.
* Runtime testing of the Avalonia UI on macOS and Linux desktops.
