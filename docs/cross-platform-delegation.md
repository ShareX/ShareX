# Cross-platform delegation and tracker (v2)

Two developers, each with a coding agent, make ShareX cross-platform together on one branch, `cross-platform-v2`. This file says who owns what, how the two avoid getting in each other's way, and where every task stands. Both agents read it, together with [AGENTS.md](../AGENTS.md) and [the learnings](cross-platform-learnings.md), before starting work, and update it in the same commit as the work.

Work is split by what each developer likes to do: Jaex takes frontend and graphics, McoreD takes backend.

| | Person | Agent | Machines | Works on |
| --- | --- | --- | --- | --- |
| **J** | Jaex | Jaex's agent | Windows and Linux | **Frontend and graphics**: every window, view, view model and control; the image editor; image processing and rendering; how capture overlays look and behave on each desktop; how the UI presents features a platform does not support. **Windows lead**: verifies and signs off everything Windows. |
| **M** | McoreD | McoreD's agent | Linux (Omarchy, Hyprland) | **Backend**: the platform contracts and all three implementations (Windows, Linux, macOS), moving operating system code behind them, application services and managers, uploaders, history storage, build, CI and packaging. |

Both develop and test on Linux. Only J can run Windows, so nothing on the Windows side is done until J has signed it off.

**Start here:** the [first steps](#first-steps) section says what each of you does first.

## Goal

ShareX is cross-platform when all of these hold:

1. The `ShareX` application (main window, tray, hotkeys, capture, upload, history, editor and tools) builds and runs on Windows, Linux and macOS. Shared projects target plain `net10.0`; Windows-only features come from `ShareX.Platform.Windows`.
2. No shared project contains P/Invoke, registry access, WinRT calls or `OperatingSystem.Is…()` branches (AGENTS.md, "Forbidden in shared code").
3. Every Windows feature still works on Windows exactly as in v22, verified on Windows.
4. On Linux (X11, and Wayland on Hyprland, sway, GNOME and KDE) and macOS, every feature works or is disabled with a reason that tells the user what to install or why it is not possible.
5. CI builds and tests on all three operating systems. Linux has an install path (script, then a package).

## What the work is

Most of it is refactoring: `develop` is complete and correct on Windows, and every piece of Windows-only code in it (P/Invoke, registry, WinRT, `OperatingSystem.Is…` branches) moves behind an interface in `ShareX.Platform`, into `ShareX.Platform.Windows`, with no change in behaviour. Callers switch to `PlatformServices.Current`, and each project then retargets to `net10.0`. Almost all of that code is backend code, so it is M's.

The refactoring alone does not make ShareX run elsewhere. The rest is:

1. **Linux and macOS implementations of every contract.** Most exist from the first branch and arrive in M1 (M).
2. **Features with no equivalent.** Wayland does not let applications list windows, move the pointer or watch global input. The backend reports `FeatureSupport.NotSupported` with a reason (M); the UI hides or disables the feature and shows the reason (J).
3. **Gaps that need new code,** not a port: Wayland screen recording, audio sources, portal global shortcuts, macOS OCR and window management (M).
4. **The UI on other desktops:** full-screen capture overlays, multi-monitor scaling, tray menus, fonts, emoji and cursors, theming and window behaviour on Hyprland, sway, GNOME, KDE and macOS (J).
5. **Running and shipping:** start up choosing the platform, retargeting, packaging and CI (M), with both of you running the real application on Linux.

## Branches

- **`cross-platform-v2`**, created from `develop` at `fd61635f2` on 2026-10-03, after Jaex finished the SkiaSharp and Avalonia migration. Both agents commit here. There is no second development branch to merge, which is the main lesson of the first attempt.
- **`develop`**: v22 hotfixes only. Whoever makes a hotfix merges it into `cross-platform-v2` the same day.
- **`cross-platform`**: the first attempt, frozen at `55e0c7d90`. Never merged. Pieces are brought over by path; [the learnings](cross-platform-learnings.md#reusable-work-on-the-first-branch) list them.
- When the goal is met, `cross-platform-v2` becomes the main line (merged into `develop` for v23, or replacing it). Jaex and McoreD decide; record it here.

## Ownership

The line is **frontend and graphics (J)** against **backend (M)**. The code already follows it: UI lives in `Presentation/` folders, `.axaml` files and `*Window`, `*ViewModel`, `*Control` classes, and each tool pairs a `*Service` or `*Helper` with a `*Window` and `*ViewModel`. So ownership is decided by where a file lives and what it is called, not by long file lists.

### Agent J owns (frontend and graphics)

- Every `.axaml` file and its code-behind, every `*Window`, `*ViewModel`, `*Control` and `*Dialog` class, and every file under a `Presentation/`, `Views/`, `ViewModels/`, `Controls/`, `Theming/` or `Rendering/` folder, in every project.
- `ShareX.Avalonia` and `ShareX.ImageEditor` (both whole), and `ShareX.ImageEditor.App`.
- Image processing and rendering: `ShareX.ImageEffectsLib` (whole), the SkiaSharp imaging layer in `ShareX.HelpersLib` (`SkiaImageHelpers*`, `SkiaDrawing`, `Image*` types, `GIF/`, colour and gradient types, `FontSafe`), and the image-processing services of tools (image resizer, watermark, combiner, splitter, converter, thumbnailer, GIF maker and trimmer, background remover, icon converter, QR code, image comparer).
- The drawing side of capture overlays: region capture, screen recording frame, scrolling capture region, colour picker, mouse highlighter drawing, pin to screen, ruler.
- `.github/workflows/build.yml`, `ShareX.Setup`, `ShareX.Steam`, Windows installer and release scripts.

### Agent M owns (backend)

- `ShareX.Platform`, `ShareX.Platform.Windows`, `ShareX.Platform.Linux`, `ShareX.Platform.MacOS` and `ShareX.Platform.Tests`: every contract and every implementation.
- Every other file: services, helpers, managers, settings, uploaders, history storage and native interop. In practice: the rest of `ShareX.HelpersLib` (including `Native/`), the non-UI files of the `ShareX` application (`Program.cs`, `TaskHelpers.cs`, `TaskManager.cs`, `UploadManager.cs`, `HotkeyManager.cs`, `StartupManager.cs`, `SystemOptions.cs`, `IntegrationHelpers.cs`, `ScreenRecordManager.cs`, `CaptureHelpers/`, `Infrastructure/`, settings and watch folders), the non-UI files of `ShareX.ScreenCaptureLib` and `ShareX.Tools` (capture, scrolling capture, recording options, OCR, mouse hook, inspect and borderless window services, clipboard viewer data, network, hashing, metadata, video tools), `ShareX.UploadersLib`, `ShareX.HistoryLib`'s data layer, `ShareX.NativeMessagingHost`.
- Every `.csproj` target framework change, `Directory.Build.*`, `.github/workflows/platform.yml`, `Scripts/install-linux.sh`, Linux and macOS packaging, `docs/cross-platform.md`.

### Shared

- `AGENTS.md`, `docs/cross-platform-learnings.md` and this file: edit only your own rows; keep edits small.
- `ShareX.Platform.Tests`: M owns it, and J adds tests there for graphics code that needs platform types.

### Windows lead

M writes the Windows implementations, but cannot run them. J is the Windows lead:

- Every change to `ShareX.Platform.Windows` is listed in the [J1 checklist](#j1-windows-verification-checklist) or as a bug row until J has run it on Windows.
- J decides how a feature should behave on Windows when the old behaviour is ambiguous, and reviews contract changes that affect what the UI can show.

### Borderline files

A file that mixes UI and backend (for example a window whose code-behind calls a Win32 API directly) belongs to J, because it is a UI file. M moves the backend call behind a service and leaves a thin wrapper; J switches the window to the service (a handoff, below). If a file's owner is unclear, whoever finds it adds a row to the status log saying who takes it, before touching it.

## How the agents work together

1. **Pull, claim, push.** Pull before starting. Set the task to `in progress (date)` and push that one-line change before writing code. Push small commits often, and only when `dotnet build ShareX.sln -c Release -p:Platform=x64` has 0 errors and every test project passes.
2. **Commit subjects start with the task id,** for example `M3: Move DPAPI behind ISecretProtectionService`. Finish a task by setting it to `done (commit)`.
3. **Requests (J → M).** When the UI needs something new from the backend or the platform (a capability, a `FeatureSupport` value to show, a new event), J adds a request row. M builds it and marks it `ready (commit)`.
4. **Handoffs (M → J).** When M moves a backend API that J's UI files still call, M keeps the old API as a thin wrapper over the new one, lists the call sites in the handoff log, and deletes the wrapper once J has switched them. The build stays green and nobody edits the other's files.
5. **Bugs cross the line by row, not by edit.** A problem in the other's files is a bug row with the owner set. Exception: a one-line fix that blocks the build may be made by either agent, recorded in the status log.
6. **No file moves across projects without an announcement.** A move is its own commit, written in the status log first; the other agent pulls before continuing (lesson 3).
7. **Blocked?** Set `blocked: reason` and pick another task.
8. **Status log.** One line per working session at the bottom: date, agent, what changed, files claimed or released.

Status values: `todo`, `in progress (YYYY-MM-DD)`, `blocked: reason`, `review` (needs the other agent or a human), `done (commit)`.

### Changing a contract

1. M adds the interface or member to `ShareX.Platform` and, in the same commit, a "not supported" implementation in every platform project (`UnsupportedServices.cs` where possible) and the test fakes, so the build stays green.
2. If the contract changes what the UI can offer or show, M marks it `review` and J looks at it before it is used.
3. M then implements it for real on Windows, Linux and macOS; the Windows part goes on the J1 checklist.
4. Changing or removing an existing member needs J's `review` first if UI code uses it.

## Order of work

1. **Foundation (M0, M1, M2).** Build plumbing, the platform projects, and platform start up. About a day. Meanwhile J verifies the first branch's Windows code (J1) and starts on the image editor on Linux (J2), which already runs there.
2. **Backend refactoring (M3 to M7)** and **UI on other desktops (J3 to J6), in parallel.** M moves operating system code behind services project by project, leaving wrappers and handoffs for UI call sites. J switches UI call sites, makes overlays and windows behave on each desktop, and shows unsupported features properly.
3. **Retarget.** Each library moves to `net10.0` as its operating system code is cleared, then the application (M8).
4. **Run everywhere.** Linux and macOS gaps, packaging, and end-to-end runs by both of you on Linux and by J on Windows (M9 to M12, J1, J8).

## First steps

### McoreD (Agent M)

1. **M0, today.** Rename `Directory.build.props` and `Directory.build.targets` to `Directory.Build.*`, set `EnableWindowsTargeting` off Windows, bring `.github/workflows/platform.yml` over, and get the solution building on Linux. Announce the rename in the status log; it touches the repository root.
2. **M1, right after.** Bring the four platform projects, `ShareX.Platform.Tests` (168 tests) and `ShareX.ImageEffectsLib.Tests` (65 legacy preset tests) from the first branch, add them to the solution and make the tests pass.
3. **M2, then M3.** Initialise the platform services in `Program.cs`, then move `HelpersLib`'s operating system code behind services in this order, because everything else builds on it: secrets (DPAPI), clipboard, shell (open, reveal), system information, screens and cursor, MIME types, thumbnails, then hotkeys, registry, startup, DWM, taskbar, printing back end and the rest. Most services and their Windows implementations already exist in M1's projects; the work is switching `HelpersLib` to them, deleting the old interop, and listing UI call sites as handoffs.

### Jaex (Agent J)

1. **J1, now.** On Windows, build the first branch (`git checkout cross-platform`, `dotnet build ShareX.sln -c Release -p:Platform=x64`) and work through the [Windows checklist](#j1-windows-verification-checklist). That is the Windows code M1 brings over, so bugs found now are fixed before it lands. File bug rows on `cross-platform-v2`.
2. **Linux set-up, now.** Install the .NET 10 SDK and the tools on your Linux machine (on Arch: `sudo pacman -S --needed dotnet-sdk wl-clipboard grim slurp libnotify libsecret ffmpeg tesseract tesseract-data-eng`), and build `cross-platform-v2` there once M0 lands.
3. **J2, now on Linux.** `ShareX.ImageEditor.App` already targets plain `net10.0`, so the image editor runs on Linux today: `dotnet run --project ShareX.ImageEditor.App -c Release -p:Platform=x64`. Make it right there: the emoji and cursor renderers (replace the Windows-only renderers with SkiaSharp ones), fonts that do not exist on Linux (Segoe UI, Arial: choose fallbacks), image insert, the screen colour picker, and anything that looks wrong on Hyprland, GNOME or KDE. That is all J's code and needs nothing from M.

## Tasks: Agent M (backend)

| ID | Task | Depends on | Status |
| --- | --- | --- | --- |
| M0 | Build plumbing: rename `Directory.build.props` and `Directory.build.targets` to `Directory.Build.*` (one commit, announced, lesson 14), set `EnableWindowsTargeting` off Windows, bring `.github/workflows/platform.yml`, and make `dotnet build ShareX.sln -c Release -p:Platform=x64` pass on Linux. | none | done (`423652e73`) |
| M1 | Bring `ShareX.Platform`, `.Windows`, `.Linux`, `.MacOS`, `ShareX.Platform.Tests` and `ShareX.ImageEffectsLib.Tests` from the first branch; add them to `ShareX.sln`; tests pass. | M0 | done (`021fe6d39`) |
| M2 | Platform start up in `Program.cs`: `PlatformServices.Initialize` with the services for the running operating system, `Shutdown` on exit. | M1 | in progress (2026-10-03) |
| M3 | `HelpersLib` backend → services, then `net10.0` with CA1416 as an error: `Native/`, `CursorData`, `DWMManager`, `DesktopIconManager`, `TimerResolutionManager`, `RegistryHelpers`, `WindowsImageInterop`, `AuthenticodeSignatureVerifier`, `AvaloniaClipboard` and `DesktopScreen` branches, `InputManager`, `WindowsHotkeyHost`, printing back end, and the Windows members of `Helpers`, `FileHelpers`, `CaptureHelpers`, `ClipboardHelpers`, `Extensions`, `MimeTypes`. Handoffs for UI call sites. | M1 | todo |
| M4 | `ScreenCaptureLib` backend → services and `net10.0`: re-apply first-branch commit `4a25a9e04` (`Screenshot` facade over `IScreenCaptureService`, snap targets, scrolling capture through `IInputService`, recording devices from the platform). The frame windows' `SetWindowShape`/`SetOverlayStyle` calls are handoffs to J. | M3 | todo |
| M5 | `Tools` backend → services and `net10.0`: OCR (`IOcrService`), mouse hook and overlay surface (`HookMouse`, `CreateOverlay`), inspect and borderless window services (`IWindowManagementService`), clipboard viewer data, ruler capture. Reuse the first branch's stash. | M3 | todo |
| M6 | Application backend: hotkeys (`HotkeyManager` → `IHotkeyService`), startup, `SystemOptions` and `IntegrationHelpers` registry → `IStartupService`/`IShellIntegrationService`, capture helpers and `ScreenRecordManager` window calls → `IWindowService`, `TaskHelpers` operating system calls. `NativeMessagingHost` → `net10.0`. | M3 | todo |
| M7 | Retarget `HistoryLib`, `UploadersLib`, `ImageEffectsLib` and the image editor's operating system services (desktop wallpaper → `IDesktopWallpaperService`) to `net10.0`. | M3 | todo |
| M8 | Retarget the `ShareX` application to `net10.0` plus `net10.0-windows10.0.22621.0` (Windows release, for WinRT features), choosing the platform at start up. | M3 to M7, J3 | todo |
| M9 | Linux gaps: Wayland screen recording (portal ScreenCast and PipeWire, or wf-recorder/wl-screenrec on wlroots), PulseAudio/PipeWire audio for FFmpeg, X11 mouse hook and overlay surface, window icons on Wayland, portal global shortcuts. | M4, M5 | todo |
| M10 | macOS: OCR with Vision, window management through the Accessibility API, mouse hook (CGEventTap) and overlay surface, recording on Apple silicon. Needs a Mac tester. | M4, M5 | todo |
| M11 | Packaging and CI: build and test the real application on Linux and macOS in CI; Linux installer for the real application; then AppImage or Flatpak; keep `docs/cross-platform.md` current. | M8 | todo |
| M12 | Decide with Jaex whether `ShareX.Desktop` (the first branch's host and `sharex` command line) and `ShareX.Destinations` come over, are folded into the application, or are dropped. | M8 | todo |
| M13 | Serve J's requests, and keep the handoff log current. | requests | ongoing |

## Tasks: Agent J (frontend and graphics)

| ID | Task | Depends on | Status |
| --- | --- | --- | --- |
| J1 | **Verify on Windows** everything in `ShareX.Platform.Windows`, using the checklist below. Start now on the first branch; continue on `cross-platform-v2` for every change M makes. File bug rows. | none | todo |
| J2 | Image editor on Linux (runs today through `ShareX.ImageEditor.App`): SkiaSharp emoji and cursor renderers instead of the Windows-only ones, font fallbacks for fonts Linux lacks, image insert, screen colour picker window, appearance on Hyprland, GNOME and KDE. | none | todo |
| J3 | Application shell UI: tray through Avalonia `TrayIcon` and `DesktopServices.RegisterTrayIcon` (retire `WindowsTrayIcon` use in `TrayIconService`), retire what is left of `Forms/MainForm.cs`, main window behaviour on Linux desktops. | M2 | todo |
| J4 | Capture overlays on every desktop: region capture, screen recording frame and tool bar, scrolling capture region window, mouse highlighter drawing, pin to screen, ruler. Full-screen placement, multi-monitor and mixed scaling on Hyprland, sway, GNOME, KDE and macOS. Switch their window-shape and overlay calls to the services (handoffs from M4, M5). | M4, M5 | todo |
| J5 | Unsupported features in the UI: wherever a service reports `NotSupported`, hide or disable the menu item, button or setting and show the reason as its tooltip (window capture and snapping on GNOME/KDE Wayland, scroll methods, mouse highlighter, inspect window, OCR without Tesseract, recording on Wayland until M9). | M3 to M6 | todo |
| J6 | Graphics on other platforms: tray and menu icons on light and dark Linux panels, theme and accent detection, DPI and fractional scaling, image formats and codecs, HDR tone mapping review. | M2 | todo |
| J7 | Switch UI call sites listed in the handoff log as they appear. | handoffs | ongoing |
| J8 | End-to-end runs of the real application once M8 lands: on Windows (sign-off) and on Linux (with McoreD). File bug rows. | M8 | todo |

### J1 Windows verification checklist

Tick each item once it behaves as in v22 on Windows 10 and 11. File a bug row for anything that does not.

- [ ] Full screen, monitor, region and last-region capture (GDI, `WindowsScreenCaptureService`)
- [ ] Capture with cursor; mixed-DPI multi-monitor bounds
- [ ] HDR colour correction on an HDR display (`HdrScreenCapture`)
- [ ] Window capture, client area only, and auto-hide task bar
- [ ] Transparent window capture, with and without shadow, Windows 10 and 11 corners (`TransparentWindowCapture`, `Win32BackdropWindow`)
- [ ] Region capture snapping to windows, client areas and child controls (`SnapTargetCollector`); arrow-key cursor nudge
- [ ] Scrolling capture with each scroll method: mouse wheel, down arrow, page down, scroll message
- [ ] Screen recording: frame window click-through region, tool bar, tray icon colours on light and dark task bars, encoding progress icon
- [ ] FFmpeg devices: gdigrab, ddagrab, screen-capture-recorder (registry written by `PrepareDevice`), DirectShow audio
- [ ] Hotkeys (`WindowsHotkeyService`), start with Windows (`WindowsStartupService`), Explorer context menu (`WindowsShellIntegrationService`)
- [ ] Clipboard text, image and files; clipboard viewer formats
- [ ] Secrets saved by v22 (DPAPI) still load (`WindowsSecretProtectionService`)
- [ ] Explorer thumbnails in history (`WindowsThumbnailService`)
- [ ] OCR with Windows.Media.Ocr (`WindowsOcrService`), including Chinese, Japanese and right-to-left languages
- [ ] Mouse highlighter (`WindowsMouseHook`, `WindowsScreenOverlay`)
- [ ] Inspect window and borderless window (`WindowsWindowManagementService`)
- [ ] Window menu lists the same windows as v22 (Progman excluded)
- [ ] ShareX starts, runs and exits normally with the platform services initialised at start up (M2)

## Requests

J asks for backend or platform capabilities here; M fills in the interface and status.

| ID | Need | Call sites | Interface | Status |
| --- | --- | --- | --- | --- |
| R1 | Printing: print dialog and printing an image or text (the preview window is J's UI) | print windows | | todo |
| R2 | Taskbar progress and overlay | upload progress UI | | todo |
| R3 | Synthetic input beyond scrolling: typing text, key combinations | application actions | extend `IInputService` | todo |

## Handoffs

M adds a row when a backend API moved and J's UI files still call the old wrapper.

| ID | Old API (wrapper kept) | New API | Call sites in J's files | Status |
| --- | --- | --- | --- | --- |

## Bugs

| ID | Found by | Platform | Description | Owner | Status |
| --- | --- | --- | --- | --- | --- |

## Status log

One line per working session, newest at the bottom.

- 2026-10-03, M: Created `cross-platform-v2` from `develop` `fd61635f2`. Added AGENTS.md, this tracker, `docs/cross-platform-learnings.md` and `docs/cross-platform.md`. No code changes yet.
- 2026-10-03, M: Work split by preference: frontend and graphics to J, backend to M. M writes all platform contracts and implementations including Windows; J is the Windows lead and signs off every Windows change. Added first steps for each of you.
- 2026-10-03, M: Claimed M0 and M1. M0 renames `Directory.build.props` and `Directory.build.targets` to `Directory.Build.*` at the repository root in its own commit; pull after it lands.
- 2026-10-03, M: M0 done (`2c700685a` rename, `423652e73`). The solution builds on Linux with 0 errors and 0 warnings. Pull before building: `Directory.build.*` are now `Directory.Build.*`.
- 2026-10-03, M: M1 done (`021fe6d39`). Platform projects and `ShareX.Platform.Tests` (168 passing on Linux) are in the solution; CI workflow `platform.yml` added. Edited J's `ShareX.ImageEffectsLib.csproj` (one line: `InternalsVisibleTo` for the preset tests, build-only). `ShareX.ImageEffectsLib.Tests` runs on Windows only until M7, because ImageEffectsLib still needs the Windows desktop runtime.
