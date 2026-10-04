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
| `IClipboardService` | Win32 clipboard | NSPasteboard | xclip or xsel | wl-clipboard (also used for copying, since Wayland apps cannot read the XWayland clipboard) |
| `IScreenCaptureService` | GDI, HDR tone mapping, window and transparent window capture | `screencapture` (Screen Recording permission); cursor from NSCursor | Xlib | grim on wlroots, otherwise the xdg-desktop-portal Screenshot interface |
| `IScreenRecordingService` | FFmpeg gdigrab, ddagrab, DirectShow | FFmpeg avfoundation | FFmpeg x11grab | wf-recorder into FFmpeg on Hyprland and sway; ScreenCast portal with GStreamer's PipeWire plugin on GNOME, KDE and others |
| `IHotkeyService` | RegisterHotKey | Carbon hot keys | XGrabKey | Hyprland: portal GlobalShortcuts with keys bound by ShareX; sway: IPC binds; GNOME, KDE: portal GlobalShortcuts (needs the `sharex.desktop` entry) |
| `IWindowService` | Win32 windows and child controls, cursor, overlays | CGWindowList | EWMH, XShape | Hyprland and sway IPC only |
| `IWindowManagementService` | Inspect, top most, opacity, borderless | Inspect (CGWindowList, NSRunningApplication); changing other windows not allowed by macOS | EWMH | Hyprland and sway IPC only |
| `IInputService` | SendInput, WM_VSCROLL, low-level mouse hook | Quartz events (Accessibility permission) | XTEST | Hyprland key shortcuts only |
| `IOcrService` | Windows.Media.Ocr | Not supported (Windows only) | Tesseract | Tesseract |
| `INotificationService` | ShareX notification window | Notification Center (`osascript`) | org.freedesktop.Notifications or notify-send | Same as X11 |
| `IShellService` | ShellExecute and Explorer select | `open` and `open -R`; MIME types from uniform type identifiers | xdg-open and FileManager1 D-Bus | Same as X11 |
| `IShellIntegrationService` | Explorer context menu keys under HKCU | Finder Quick Actions in `~/Library/Services`; Launch Services default application for file types | File manager `.desktop` actions | Same as X11 |
| `ICredentialService` | Credential Manager | Keychain | Secret Service (secret-tool) | Same as X11 |
| `ISecretProtectionService` | DPAPI | AES-GCM with an owner-only key file | AES-GCM with an owner-only key file | Same as X11 |
| `IThumbnailService` | Explorer shell thumbnails | QuickLook (`qlmanage`) | freedesktop.org thumbnail cache | Same as X11 |
| `IPrintService` | System.Drawing.Printing and the print dialog | CUPS `lp` | CUPS `lp` (no printer dialog) | Same as X11 |
| `ISoundService` | PlaySound | afplay | pw-play, paplay or aplay | Same as X11 |
| `ITaskbarService` | ITaskbarList3 progress | Dock badge | Unity launcher API (KDE, Dash to Dock, Plank) | Same as X11 |
| `ISystemGraphicsService` | Not yet (W4: DirectWrite emoji, Win32 cursors) | NSCursor images; emoji drawn by ShareX | Cursor theme via libXcursor | Same as X11 |
| `ISystemInfoService`, `ISystemPreferencesService` | OS name, elevation, wheel lines, small icon size, task bar theme | `sw_vers`, defaults | os-release, defaults | Same as X11 |

## Linux desktops

Linux differs between compositors as much as between operating systems, so `ShareX.Platform.Linux` is desktop aware as well. `Desktop/LinuxDesktop` classifies the session (X11, Hyprland, sway, GNOME, KDE, other Wayland) and decides which implementation a service uses there; services that differ between compositors ask it instead of checking the desktop themselves. Desktop-specific pieces live under `Desktop/`, for example `HyprlandShortcutKeyBinder` and `SwayHotkeyService`. Per desktop:

| Area | X11 | Hyprland | sway | GNOME, KDE (Wayland) |
| --- | --- | --- | --- | --- |
| Windows, pointer, monitor layout | `X11WindowBackend` (EWMH, Xlib, RandR) | `HyprlandWindowBackend` (hyprctl; `HyprlandDispatcher` speaks Lua or classic dispatchers) | `SwayWindowBackend` (swaymsg) | not exposed (`UnsupportedWindowBackend`) |
| Screen capture | `X11CaptureBackend` | `GrimCaptureBackend` | `GrimCaptureBackend` | `PortalCaptureBackend` |
| Screen recording | `X11GrabRecordingBackend` | `WfRecorderRecordingBackend` | `WfRecorderRecordingBackend` | `PortalRecordingBackend`: ScreenCast portal, GStreamer `pipewiresrc`, frames cropped and passed to FFmpeg as YUV4MPEG |
| Hotkeys | `X11HotkeyService` | portal + `HyprlandShortcutKeyBinder` | `SwayHotkeyService` | portal |

The services (`LinuxWindowService`, `LinuxScreenCaptureService`, `LinuxScreenRecordingService`, `IHotkeyService`) are façades over these.

## Start up

`Program` calls `PlatformServices.Initialize` once with `WindowsPlatformServices`, `MacPlatformServices` or `LinuxPlatformServices`, chosen by operating system. That is the only operating system check in shared code. After that everything uses `PlatformServices.Current`, and `PlatformServices.Shutdown` disposes it on exit.

## Linux distribution awareness

`PlatformDetector` reads `/etc/os-release` into `LinuxDistribution` (family, immutable variants, package manager) and detects the display server and desktop environment. `LinuxPackages.Missing` names the distribution's package for a missing optional program in a feature's support reason; ShareX never shows install commands or installs anything (AGENTS.md, "Linux dependencies").

## Scrolling capture per desktop

Scrolling capture needs a screenshot of the selected window and a way to scroll it. Each scroll method checks its input support right before every step (R33), so a desktop that stops allowing input ends the capture with a reason instead of sending input blindly.

| Desktop | Mouse wheel | Down arrow / Page Down | Scroll message |
| --- | --- | --- | --- |
| Windows | yes | yes | yes |
| X11 (Xfce, Cinnamon, MATE, …) | yes (XTEST) | yes (XTEST) | no: only Windows lets one application drive another's scroll bars |
| Hyprland | no | yes (compositor key shortcuts) | no |
| sway, GNOME, KDE (Wayland) | no | no | no |

On sway, GNOME and KDE scrolling capture is reported unsupported with the Wayland reason. The RemoteDesktop portal could provide input there later; it asks the user for permission each session.

## Linux package

`Scripts/install-linux.sh` builds and installs ShareX for the current user under `~/.local` (no root). `Scripts/install-linux.sh --package DIR` makes a self-contained `sharex-linux-x64.tar.gz` (or `-arm64`); users extract it and run `./install.sh`, which needs neither the .NET SDK nor root, and `./install.sh --uninstall` removes it while keeping settings and screenshots. Flatpak is deferred: its sandbox blocks the compositor IPC, X11 window access and grim that capture relies on. The manual `Linux` workflow builds, tests, packages and runs the installed application headless.

## Building

.NET 10 SDK on every operating system.

```sh
dotnet build ShareX.sln -c Release -p:Platform=x64
dotnet test ShareX.Platform.Tests
```

Pass `-p:Platform=x64` (or `ARM64`); a plain `dotnet build ShareX.sln` uses `Any CPU`, which the projects do not configure. Building on Linux and macOS needs `Directory.Build.props` (capital B) with `EnableWindowsTargeting` set off Windows; that is task M0.

## Installing on Linux

```sh
Scripts/install-linux.sh              # build a self-contained copy and install it to ~/.local
Scripts/install-linux.sh --uninstall  # remove it; settings and screenshots in ~/Documents/ShareX stay
```

The script installs `~/.local/lib/sharex/app`, the `sharex` command and the `sharex.desktop` entry with "Capture region", "Capture full screen" and "Capture active window" actions. The desktop entry is required: xdg-desktop-portal identifies ShareX by it (application id `sharex`) before it grants global hotkeys.

Recommended packages: FFmpeg (recording), wl-clipboard (Wayland) or xclip (X11), grim and wf-recorder (Hyprland, sway), libnotify, libsecret, Tesseract with language data (OCR), CUPS (printing). ShareX names the missing package and the install command for the distribution where a feature needs one.

### Hyprland

Hyprland's GlobalShortcuts portal registers ShareX's hotkeys but does not assign keys to them, so ShareX binds each hotkey's keys itself at run time (`hyprctl eval` with `hl.bind` on Lua configurations, `hyprctl keyword bind` on classic ones). Key combinations the configuration already uses, such as Omarchy's Print Screen, are left alone and reported as in use, like a hotkey another application holds on Windows; choose another key in ShareX's hotkey settings. The binds are not written to the configuration, are removed when ShareX exits and are added again after `hyprctl reload`. `hyprctl binds` lists them with "ShareX:" descriptions.

With `xwayland:force_zero_scaling`, ShareX's windows use device pixels; the platform layer converts between them and Hyprland's layout coordinates.
