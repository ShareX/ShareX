# Cross-platform delegation and tracker

Two coding agents finish the cross-platform work in parallel. This file says who owns what, how they coordinate, and where every task stands. Both agents read it before starting work and update it in the same commit as the work.

- **Agent J**: Jaex's agent. Windows application shell, Windows verification.
- **Agent M**: McoreD's agent. Platform layer, Linux and macOS, ShareX.Tools, packaging and CI.

Rules for the code itself are in [AGENTS.md](../AGENTS.md) (platform abstraction rules). Architecture and status are in [cross-platform.md](cross-platform.md).

## Goal

ShareX is cross-platform when all of these hold:

1. `ShareX.csproj` (the real application, with its main window, tray, hotkeys, capture, upload, history and tools) targets plain `net10.0`, with Windows-only features coming from `ShareX.Platform.Windows`. It builds and runs on Windows, Linux and macOS.
2. `ShareX.HelpersLib.Windows` no longer exists.
3. No shared project contains P/Invoke, registry access, WinForms, GDI+ or `OperatingSystem.Is…()` branches (AGENTS.md "Forbidden in shared code").
4. Every Windows feature works on Windows exactly as before, verified by running the app on Windows.
5. On Linux (X11 and Wayland: Hyprland, sway, GNOME, KDE) and macOS, every feature either works or is disabled with a reason that tells the user what to install or why it is not possible.
6. CI builds and tests on all three operating systems, and Linux has an install path (script, then a package).

## Step 0: one branch (decision needed)

`develop` has no `ShareX.Platform*` projects, so platform work cannot be done there. Before the split starts, pick one:

- **A (recommended).** Jaex merges `cross-platform` into `develop` once, and both agents then work on `develop`. There is a single line of history and no more two-way merges.
- **B.** Both agents work on `cross-platform` and Jaex keeps `develop` for releases. `develop` is merged into `cross-platform` after every Jaex push, as now.

Until this is decided, Agent J keeps working on `develop` and Agent M merges `develop` into `cross-platform` after each Jaex push. When both changed the same code, `develop`'s version wins and the platform abstraction is re-applied on top.

Decision: _pending_ (record the choice and date here).

## Ownership

An agent edits only the projects it owns. To change something in the other agent's project, add a row to the [request log](#requests-between-agents) instead.

| Project | Owner | Notes |
| --- | --- | --- |
| `ShareX.Platform`, `ShareX.Platform.Windows`, `ShareX.Platform.Linux`, `ShareX.Platform.MacOS`, `ShareX.Platform.Tests` | M | All interfaces and every OS implementation. J requests new services (see below). |
| `ShareX.Tools` | M | Includes the stashed port (`stash@{0}` on McoreD's machine). |
| `ShareX.ScreenCaptureLib` | M | Already portable; M keeps it that way. |
| `ShareX.ImageEditor`: `Integration/*DesktopWallpaperService.cs`, `Integration/EditorServices.cs`, `Presentation/Emoji/WindowsEmojiBitmapRenderer.cs`, `Presentation/Rendering/WindowsCursorBitmapRenderer.cs`, `Presentation/Views/EditorView.ImageInsert.cs` | M | Only these OS-specific files. |
| `ShareX.ImageEditor` (everything else), `ShareX.ImageEditor.App` | J | Editor features continue as normal. |
| `ShareX.Avalonia`: `Windows/ScreenColorPickerWindow.axaml.cs`, `Input/CursorAssetLoader.cs` | M | Only these two files. |
| `ShareX.Avalonia` (everything else) | J | |
| `ShareX.HelpersLib`: `Helpers/AuthenticodeSignatureVerifier.cs`, `Helpers/AvaloniaClipboard.cs`, `Helpers/PortableShell.cs`, `Helpers/CaptureHelpers.cs` | M | Platform-facing helpers. |
| `ShareX.HelpersLib` (everything else) | J | |
| `ShareX` (the app) | J | |
| `ShareX.HelpersLib.Windows` | J | J dissolves it. Windows interop that must survive moves into `ShareX.Platform.Windows` through a request to M. |
| `ShareX.HistoryLib`, `ShareX.UploadersLib`, `ShareX.Destinations`, `ShareX.ImageEffectsLib` | J | Already portable. |
| `ShareX.NativeMessagingHost`, `ShareX.Setup`, `ShareX.Steam` | J | Setup and Steam may stay Windows-only (installer and Steam launcher). |
| `ShareX.Desktop`, `ShareX.Desktop.Tests`, `ShareX.Destinations.Tests`, `ShareX.ImageEffectsLib.Tests` | M | |
| `Scripts/install-linux.sh`, Linux and macOS packaging, `.github/workflows/platform.yml`, `docs/cross-platform.md` | M | |
| `.github/workflows/build.yml`, Windows installer and release scripts | J | |
| `AGENTS.md`, this file | Both | Edit only your own rows; keep edits small. |

## How the agents work together

1. **Claim before starting.** Set the task to `in progress` with the date and push that change before writing code, so the other agent sees it.
2. **One task per commit series.** Start commit subjects with the task id, for example `M1: Port OCR window to IOcrService`. End the task by setting it to `done` with the commit hash.
3. **Requests between agents.** When J needs a platform capability (for example "move printing behind a service"), J adds a row to the request log with the call sites that need it. M designs the interface, implements Windows (same behaviour as today), Linux and macOS, and marks the request `ready` with the commit. J then switches the call sites. M never edits J's call sites, and J never edits `ShareX.Platform*`.
4. **Windows behaviour is J's to verify.** M has no Windows machine. Anything M moves into `ShareX.Platform.Windows` is listed under J1 until J has run it.
5. **Before every push:** `dotnet build ShareX.sln -c Release -p:Platform=x64` with 0 errors, and all test projects pass. Fetch and rebase or merge first; another agent may have pushed.
6. **Blocked?** Set the status to `blocked`, say on what, and pick another task.
7. **Status log.** Append one line per working session to the [log](#status-log) at the bottom: date, agent, what changed.

Status values: `todo`, `in progress (YYYY-MM-DD)`, `blocked: reason`, `review` (needs the other agent or a human), `done (commit)`.

## Tasks: Agent J (Windows and application shell)

| ID | Task | Projects | Depends on | Status |
| --- | --- | --- | --- | --- |
| J1 | **Verify on Windows** everything M moved into `ShareX.Platform.Windows`, and file a bug row for anything that differs from before. Checklist below. | run the app | none | todo |
| J2 | Replace the WinForms hotkey host (`HotkeyForm`, `IHotkeyHost`, `HotkeyManager`) with `IPlatformServices.Hotkeys` (`IHotkeyService`; `WindowsHotkeyService` already exists). | `ShareX`, `HelpersLib.Windows/Input` | none | todo |
| J3 | Replace `MainForm` and the WinForms message loop in `Program.cs` with the Avalonia lifetime; tray via Avalonia `TrayIcon` and `DesktopServices.RegisterTrayIcon` (retire `TrayIconService`'s WinForms parts and `WindowsTrayIcon`). | `ShareX` | J2 | todo |
| J4 | Replace the Win32 calls left in the app: `CaptureBase`, `CaptureWindow`, `CaptureCustomWindow`, `TaskMetadata`, `MainMenuBuilder` (window icons via `IWindowManagementService.GetIcon`), `ScreenRecordManager` (active window via `IWindowService.GetActiveWindow`), `NotificationWindow`, `AfterCaptureWindow`, `AfterUploadWindow`, `BeforeUploadWindow`, `ClipboardUploadWindow`, `ThumbnailItemViewModel`. File a request for anything the services do not cover yet. | `ShareX` | none | todo |
| J5 | Replace the registry use in `SystemOptions` and `IntegrationHelpers` with `IStartupService`, `IShellIntegrationService` and the settings files (request a service if one is missing, for example browser native-messaging registration). | `ShareX` | R-rows | todo |
| J6 | Dissolve `ShareX.HelpersLib.Windows`. Move portable code (update checkers, FFmpeg downloader, `WindowState`, `XmlFont`, print settings, update windows) into `HelpersLib`. Request services for Windows-only behaviour (printing, DWM, taskbar progress, desktop icons, timer resolution, shortcuts, clipboard extras, input simulation). Delete the rest, then remove the project from the solution. | `HelpersLib.Windows`, `HelpersLib` | J2 to J5, R-rows | todo |
| J7 | Retarget `ShareX.csproj` to `net10.0;net10.0-windows10.0.22621.0`. The plain target is what Linux and macOS build. The Windows release builds the Windows SDK target, so `ShareX.Platform.Windows` supplies its WinRT features (OCR) there. Choose `IPlatformServices` by OS at start up. Add the host-project exception to AGENTS.md. | `ShareX` | J2 to J6, M1 | todo |
| J8 | `ShareX.NativeMessagingHost` → `net10.0`: replace `CreateProcess` with `Process.Start` (job breakaway behind a service if still needed) and move manifest registration to a service. | `NativeMessagingHost` | R-row | todo |
| J9 | Keep `ShareX.Setup` and `ShareX.Steam` Windows-only, and document them as the allowed exceptions in AGENTS.md. | `Setup`, `Steam` | none | todo |

### J1 Windows verification checklist

Tick each item once it behaves as in the last release on Windows 10 and 11. File a bug row for anything that does not.

- [ ] Full screen, monitor, region and last-region capture (GDI, `WindowsScreenCaptureService`)
- [ ] Capture with cursor; mixed-DPI multi-monitor bounds
- [ ] HDR colour correction on an HDR display (`HdrScreenCapture`, now on raw buffers)
- [ ] Window capture, client area only, and auto-hide task bar
- [ ] Transparent window capture, with and without shadow, Windows 10 and 11 corners (`TransparentWindowCapture`, `Win32BackdropWindow`)
- [ ] Region capture snapping to windows, client areas and child controls (`SnapTargetCollector`); arrow-key cursor nudge
- [ ] Scrolling capture with each scroll method: mouse wheel, down arrow, page down, scroll message
- [ ] Screen recording: frame window click-through region, tool bar, tray icon colours on light and dark task bars, encoding progress icon
- [ ] FFmpeg devices: gdigrab, ddagrab, screen-capture-recorder (registry written by `PrepareDevice`), DirectShow audio
- [ ] OCR with Windows.Media.Ocr (`WindowsOcrService`), including Chinese, Japanese and right-to-left languages
- [ ] Clipboard viewer formats (`GetFormatsAsync`/`GetDataAsync`)
- [ ] Mouse highlighter (`WindowsMouseHook`, `WindowsScreenOverlay`), after M1
- [ ] Inspect window and borderless window (`WindowsWindowManagementService`), after M1
- [ ] Window menu lists the same windows as before (Progman excluded)

## Tasks: Agent M (platform layer, Linux, macOS, Tools)

| ID | Task | Projects | Depends on | Status |
| --- | --- | --- | --- | --- |
| M1 | Finish `ShareX.Tools` → `net10.0` and drop its `HelpersLib.Windows` reference: OCR, clipboard viewer, mouse highlighter (stashed work), inspect and borderless window via `IWindowManagementService`, ruler capture via `IScreenCaptureService`, and the leftovers in pin to screen, monitor test, image combiner and GIF trimmer. Reconcile with develop's new `AvaloniaClipboard`. | `Tools` | none | todo |
| M2 | ImageEditor OS integrations → services: `IDesktopWallpaperService` (move the Windows, Linux and macOS implementations into `ShareX.Platform.*`), the emoji and cursor renderers, and the P/Invoke in `EditorView.ImageInsert`. | `ImageEditor` (owned files), `Platform.*` | none | todo |
| M3 | `ShareX.Avalonia`: screen colour picker P/Invoke and OS branch → `IWindowService`/`IScreenCaptureService`; cursor asset loader. | `Avalonia` (owned files) | none | todo |
| M4 | `HelpersLib` platform helpers: Authenticode verification → `ICodeSignatureService` (WinVerifyTrust on Windows, not applicable elsewhere); `AvaloniaClipboard` format-name branches → `IClipboardService`. | `HelpersLib` (owned files), `Platform.*` | none | todo |
| M5 | Serve J's requests (request log): design the interface, implement it on all three platforms, add tests. | `Platform.*` | requests | ongoing |
| M6 | Linux gaps: Wayland screen recording (xdg-desktop-portal ScreenCast with PipeWire, or wf-recorder/wl-screenrec on wlroots); PulseAudio/PipeWire audio sources for FFmpeg; X11 mouse hook and overlay for the highlighter; window icons on Wayland from desktop entries; borderless/top-most on GNOME and KDE where possible. | `Platform.Linux` | none | todo |
| M7 | macOS gaps: OCR with the Vision framework, window management (Accessibility API), mouse hook (CGEventTap) and overlay, recording check on Apple silicon. Needs a Mac tester. | `Platform.MacOS` | tester | todo |
| M8 | Packaging and CI: build the real app on Linux and macOS in `platform.yml` once J7 lands; Linux install script for the real app; then AppImage or Flatpak; keep `docs/cross-platform.md` current. | scripts, CI, docs | J7 for the app build | todo |
| M9 | Linux end-to-end run of the real app on Omarchy (Hyprland), GNOME and KDE: every main-window and tray action, every tool, and every capture, upload and history path. File bug rows. | run the app | J7 | todo |
| M10 | Decide `ShareX.Desktop`'s future once the real app runs everywhere: fold its CLI (`sharex capture …`) into the app and retire the separate host. Needs Jaex and McoreD to agree. | `Desktop` | J7, M9 | todo |

## Requests between agents

J adds rows. M fills in "Interface" and "Status".

| ID | Requested by | Need | Call sites | Interface | Status |
| --- | --- | --- | --- | --- | --- |
| R1 | J (pre-filled) | Printing (print dialog, preview, print image or text) | `HelpersLib.Windows/Printer`, `Presentation/Print` | | todo |
| R2 | J (pre-filled) | Taskbar progress and overlay (`TaskbarManager`) | app upload progress | | todo |
| R3 | J (pre-filled) | Browser native-messaging host registration (Chrome, Firefox) | `IntegrationHelpers`, `NativeMessagingHost` | | todo |
| R4 | J (pre-filled) | Shortcut (.lnk) creation (`ShortcutHelpers`, `WshShell`) | app integration settings | probably `IStartupService`/`IShellIntegrationService` | todo |
| R5 | J (pre-filled) | Synthetic input beyond scrolling (`InputHelpers`, `InputManager`: text typing, key combinations) | app actions | extend `IInputService` | todo |

## Bugs found during verification

| ID | Found by | Platform | Description | Owner | Status |
| --- | --- | --- | --- | --- | --- |

## Status log

Newest at the bottom, one line per working session.

- 2026-10-02, M: Document created after merging `develop` (Jaex, 12 commits) into `cross-platform` at `6c66e109c`. Solution builds on Linux; 326 tests pass. Tools port stashed pending Jaex finishing.
