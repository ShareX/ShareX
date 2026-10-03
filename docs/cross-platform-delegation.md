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
- **`ShareX.Platform.Windows` (whole): every Windows implementation of every service.** J is the Windows lead (below).
- Image processing and rendering: `ShareX.ImageEffectsLib` (whole), the SkiaSharp imaging layer in `ShareX.HelpersLib` (`SkiaImageHelpers*`, `SkiaDrawing`, `Image*` types, `GIF/`, colour and gradient types, `FontSafe`), and the image-processing services of tools (image resizer, watermark, combiner, splitter, converter, thumbnailer, GIF maker and trimmer, background remover, icon converter, QR code, image comparer).
- The drawing side of capture overlays: region capture, screen recording frame, scrolling capture region, colour picker, mouse highlighter drawing, pin to screen, ruler.
- `.github/workflows/build.yml`, `ShareX.Setup`, `ShareX.Steam`, Windows installer and release scripts.

### Agent M owns (backend)

- `ShareX.Platform` (every contract, `UnsupportedServices.cs`, shared helpers), `ShareX.Platform.Linux`, `ShareX.Platform.MacOS` and `ShareX.Platform.Tests`.
- Every other file except Windows code: services, helpers, managers, settings, uploaders, history storage and native interop. In practice: the rest of `ShareX.HelpersLib` (its Win32 code in `Native/` and similar moves to `ShareX.Platform.Windows`, which is J's job: task J9), the non-UI files of the `ShareX` application (`Program.cs`, `TaskHelpers.cs`, `TaskManager.cs`, `UploadManager.cs`, `HotkeyManager.cs`, `StartupManager.cs`, `SystemOptions.cs`, `IntegrationHelpers.cs`, `ScreenRecordManager.cs`, `CaptureHelpers/`, `Infrastructure/`, settings and watch folders), the non-UI files of `ShareX.ScreenCaptureLib` and `ShareX.Tools` (capture, scrolling capture, recording options, OCR, mouse hook, inspect and borderless window services, clipboard viewer data, network, hashing, metadata, video tools), `ShareX.UploadersLib`, `ShareX.HistoryLib`'s data layer, `ShareX.NativeMessagingHost`.
- Every `.csproj` target framework change, `Directory.Build.*`, `.github/workflows/platform.yml`, `Scripts/install-linux.sh`, Linux and macOS packaging, `docs/cross-platform.md`.

### Shared

- `AGENTS.md`, `docs/cross-platform-learnings.md` and this file: edit only your own rows; keep edits small.
- `ShareX.Platform.Tests`: M owns it, and J adds tests there for graphics code that needs platform types.

### Windows lead

J leads Windows: J writes, runs and signs off all Windows code.

- J owns `ShareX.Platform.Windows`. All Windows implementations are written there by J, including moving develop's Win32, COM, registry and WinRT code out of `HelpersLib`, `ScreenCaptureLib`, `Tools` and the application (J9).
- M never edits `ShareX.Platform.Windows`, with one exception: when M adds a new service to `IPlatformServices`, M wires a "not supported" stub into `WindowsPlatformServices` in the same commit (one line, so the build stays green) and adds a W row (below).
- M does not switch a shared call site to a service until the Windows implementation behind it is done, so Windows never loses a feature in between. Until then the old code stays where it is.
- J decides how a feature behaves on Windows when the old behaviour is ambiguous, and reviews contract changes that affect what the UI can show.
- Windows code that M wrote before 2026-10-03 (everything brought from the first branch, and the M3 to M6 commits up to `1cf9734e9`) is J's from now on. It is all on the J1 checklist; fixes are J's.

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
3. M implements it on Linux and macOS and adds a W row for the Windows implementation, naming where the old Windows code lives. J implements it in `ShareX.Platform.Windows` and marks the row done; M then switches the shared call site and deletes the old code.
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
3. **J2, now on Linux.** `ShareX.ImageEditor.App` already targets plain `net10.0`, so the image editor runs on Linux today: `dotnet run --project ShareX.ImageEditor.App -c Release -p:Platform=x64`. Make it right there: emoji and cursors off Windows (keep the Direct2D emoji and Win32 cursor renderers on Windows; elsewhere draw emoji with SkiaSharp from the system colour emoji font, Noto Color Emoji on Linux and Apple Color Emoji on macOS, and use the bundled cursors that are already the non-Windows fallback), fonts that do not exist on Linux (Segoe UI, Arial: choose fallbacks), image insert, the screen colour picker, and anything that looks wrong on Hyprland, GNOME or KDE. That is all J's code and needs nothing from M.

## Tasks: Agent M (backend)

| ID | Task | Depends on | Status |
| --- | --- | --- | --- |
| M0 | Build plumbing: rename `Directory.build.props` and `Directory.build.targets` to `Directory.Build.*` (one commit, announced, lesson 14), set `EnableWindowsTargeting` off Windows, bring `.github/workflows/platform.yml`, and make `dotnet build ShareX.sln -c Release -p:Platform=x64` pass on Linux. | none | done (`423652e73`) |
| M1 | Bring `ShareX.Platform`, `.Windows`, `.Linux`, `.MacOS`, `ShareX.Platform.Tests` and `ShareX.ImageEffectsLib.Tests` from the first branch; add them to `ShareX.sln`; tests pass. | M0 | done (`021fe6d39`) |
| M2 | Platform start up in `Program.cs`: `PlatformServices.Initialize` with the services for the running operating system, `Shutdown` on exit. | M1 | in progress (2026-10-03) |
| M3 | `HelpersLib` backend → services, then `net10.0` with CA1416 as an error. M adds the contracts, the Linux and macOS implementations and the portable façades (`WindowInfo`, print helpers and others keep their API over the services), and switches shared callers once J's Windows side is done (W rows). The Win32 code itself (`Native/`, `CursorData`, `DWMManager`, `DesktopIconManager`, `TimerResolutionManager`, `RegistryHelpers`, `WindowsImageInterop`, `InputManager`, `WindowsHotkeyHost`) moves to `ShareX.Platform.Windows` under J9. Handoffs for UI call sites. | M1, J9 | in progress (2026-10-03) |
| M4 | `ScreenCaptureLib` backend → services and `net10.0`: re-apply first-branch commit `4a25a9e04` (`Screenshot` facade over `IScreenCaptureService`, snap targets, scrolling capture through `IInputService`, recording devices from the platform). The frame windows' `SetWindowShape`/`SetOverlayStyle` calls are handoffs to J. | M3 | in progress (2026-10-03) |
| M5 | `Tools` backend → services and `net10.0`: OCR (`IOcrService`), mouse hook and overlay surface (`HookMouse`, `CreateOverlay`), inspect and borderless window services (`IWindowManagementService`), clipboard viewer data, ruler capture. Reuse the first branch's stash. | M3 | in progress (2026-10-03): OCR, mouse highlighter, inspect and borderless services done; retarget waits for HelpersLib, and the background remover needs a CPU ONNX Runtime off Windows (DirectML is Windows only) |
| M6 | Application backend: hotkeys (`HotkeyManager` → `IHotkeyService`), startup, `SystemOptions` and `IntegrationHelpers` registry → `IStartupService`/`IShellIntegrationService`, capture helpers and `ScreenRecordManager` window calls → `IWindowService`, `TaskHelpers` operating system calls. `NativeMessagingHost` → `net10.0`. | M3 | in progress (2026-10-03) |
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
| J1 | **Verify on Windows** everything in `ShareX.Platform.Windows`, using the checklist below. Start now on the first branch; continue on `cross-platform-v2` for every change M makes. File bug rows. | none | in progress (2026-10-03) |
| J2 | Image editor on Linux (runs today through `ShareX.ImageEditor.App`): emoji and cursors off Windows (Windows keeps its Direct2D and Win32 renderers; Linux and macOS draw emoji with SkiaSharp from the system colour emoji font and use the bundled cursors), font fallbacks for fonts Linux lacks, image insert, screen colour picker window, appearance on Hyprland, GNOME and KDE. | none | in progress (2026-10-03) |
| J3 | Application shell UI: tray through Avalonia `TrayIcon` and `DesktopServices.RegisterTrayIcon` (retire `WindowsTrayIcon` use in `TrayIconService`), retire what is left of `Forms/MainForm.cs`, main window behaviour on Linux desktops. | M2 | todo |
| J4 | Capture overlays on every desktop: region capture, screen recording frame and tool bar, scrolling capture region window, mouse highlighter drawing, pin to screen, ruler. Full-screen placement, multi-monitor and mixed scaling on Hyprland, sway, GNOME, KDE and macOS. Switch their window-shape and overlay calls to the services (handoffs from M4, M5). | M4, M5 | todo |
| J5 | Unsupported features in the UI: wherever a service reports `NotSupported`, hide or disable the menu item, button or setting and show the reason as its tooltip (window capture and snapping on GNOME/KDE Wayland, scroll methods, mouse highlighter, inspect window, OCR without Tesseract, recording on Wayland until M9). | M3 to M6 | todo |
| J6 | Graphics on other platforms: tray and menu icons on light and dark Linux panels, theme and accent detection, DPI and fractional scaling, image formats and codecs, HDR tone mapping review. | M2 | todo |
| J7 | Switch UI call sites listed in the handoff log as they appear. | handoffs | ongoing: H1 to H5 done |
| J9 | **Windows platform.** Own `ShareX.Platform.Windows`: work through the W rows, move develop's remaining Win32 code from `HelpersLib` (`Native/`, `NativeMethods*`, `WindowInfo` internals, `DWMManager`, `DesktopIconManager`, `TimerResolutionManager`, `RegistryHelpers`, `WindowsImageInterop`, `InputManager`, `WindowsHotkeyHost`, `CursorData`), `ScreenCaptureLib`, `Tools` and the application into it behind the existing services, and fix Windows bugs found in J1. | none | in progress (2026-10-03) |
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
- [ ] Copy image: other applications paste PNG, the DIB fallback and the HTML `<img>` fragment (CF_HTML now built by `WindowsClipboardService.EncodeHtml`)
- [ ] "Run as administrator" detection, Windows product name in the about and debug info, tablet mode (`WindowsSystemInfoService`)
- [ ] Update installer signature check (`WindowsCodeSignatureService`, moved from `AuthenticodeSignatureVerifier`)
- [ ] Window rectangle, client rectangle and "active window" recording area, cursor position and pixel colour (`WindowsWindowService.GetWindowBounds`, `GetClientBounds`, `GetActiveWindowHandle`; pixel colour now a one pixel GDI capture); "disable hotkeys and notifications while a full screen application runs"
- [ ] Capture hides and restores desktop icons ("auto hide desktop icons", `WindowsShellService.SetDesktopIconsVisible`)
- [ ] Capture custom window by title; capturing a minimised or background window restores and activates it first (`RestoreWindow`, `ActivateWindow`)
- [ ] "Make active window top most" toggles; "make active window borderless" works on the focused window (`GetActiveWindowHandle`)
- [ ] File names and history get the active window's title and process (`WindowManagement.GetDetails`)
- [ ] First run finds Paint, Paint.NET, Photoshop, IrfanView, XnView as external programs (`WindowsShellService.FindProgram`)
- [ ] Region capture with "show cursor" draws the cursor in the right place (`WindowsScreenCaptureService.CaptureCursor`)
- [ ] Integration settings: Explorer "Upload with ShareX" and "Edit with ShareX" entries, .sxcu and .sxie file types (open, icon), Chrome and Firefox extension hosts, Send to menu, start with Windows (Task Manager's "disabled" respected). Registry keys and command lines must match v22 exactly (`WindowsShellIntegrationService`, `WindowsStartupService`)
- [ ] Administrator policies DisableUpdateCheck, DisableUpload, DisableLogging, PersonalPath from HKLM and HKCU\SOFTWARE\ShareX (`WindowsSystemPreferencesService.GetPolicy`)
- [ ] Upload progress on the taskbar button, cleared when uploads finish; the "Show progress in taskbar" setting turns it off (`WindowsTaskbarService`)
- [ ] MIME type of an unusual extension comes from the registry (`WindowsShellService.GetMimeType`); "Open folder" selects the file

## Requests

J asks for backend or platform capabilities here; M fills in the interface and status.

| ID | Need | Call sites | Interface | Status |
| --- | --- | --- | --- | --- |
| R1 | Printing: print dialog and printing an image or text (the preview window is J's UI) | print windows | | todo |
| R2 | Taskbar progress and overlay | upload progress UI | `ITaskbarService` (`PlatformServices.Current.Taskbar`); `TaskbarManager` keeps its API on top. Develop never used overlay icons, so none were added | done |
| R3 | Synthetic input beyond scrolling: typing text, key combinations | application actions | extend `IInputService` | todo |
| R4 | Unblock the Windows test baseline before the W1/J1 claim push: thumbnail URI fixture uses `/home/jens/...`, which `new Uri(path)` rejects on Windows; browser-host fixture assumes `Path.GetFullPath("/opt/...")` stays a Unix path. Use a native absolute path and assert the parsed JSON path. | `ShareX.Platform.Tests/SecretAndThumbnailTests.cs:110`, `LinuxTests.cs:294` | test fixtures only | done (`ef69ad7a7`); Jaex approved the ownership exception |
| R5 | Preserve v22's configurable global `HotkeyRepeatLimit` when switching to `IHotkeyService`: `WindowsHotkeyService` must emit native repeats, including when the limit is zero. Apply the existing shared stopwatch throttle in the portable hotkey host/manager, and dispatch callbacks to Avalonia's UI thread so exceptions reach the application's handler. | `Forms/MainForm.cs:58`, `HotkeyManager`, portable replacement for `WindowsHotkeyHost` | existing `IHotkeyService`; shared wrapper, no contract change needed | todo (M); required before switching the shared caller in M6 |
| R6 | Report support and user-facing reasons separately for inspecting windows, selecting child controls, setting topmost, and setting opacity. `WindowDetails` nullable fields let J disable settings, but do not explain why, and `WindowManagement.Support` alone does not distinguish Wayland operations. | Inspect Window buttons/settings, J5 menus | extend `IWindowManagementService` or portable feature support model | todo (M); J uses existing nullable details until reasons are available |
| R7 | Clarify `WindowDetails.ClientBounds` coordinates: v22's Inspect Window displays local `GetClientRect` coordinates (origin 0,0), while `IWindowService.GetClientBounds` is a screen-coordinate rectangle for capture. J restored the local inspector coordinates on Windows. Document this distinction and align the Linux/macOS details where applicable. | Inspect Window details (H4) | `WindowDetails.ClientBounds` documentation/other platform implementations | todo (M); Windows behavior verified |
| R8 | Add portable graphics contracts so J can move the retained Windows Direct2D emoji and Win32 cursor renderers into `ShareX.Platform.Windows` without changing Windows behavior. Return encoded PNG or portable pixels; accept Unicode text/size and portable cursor identifiers. J can provide the shared Skia color-emoji path and bundled cursor fallback. | `WindowsEmojiBitmapRenderer`, `WindowsCursorBitmapRenderer`, editor emoji/cursor previews and annotations | graphics rendering services on `IPlatformServices`, with unsupported stubs | todo (M); required for J2/J9 renderer moves |
| R9 | Add a color-picker capture session that supplies sample/magnifier pixels and a capability/reason. Preserve live GDI sampling on Windows; on portal-only Wayland use one approved capture before showing the overlay instead of launching a screenshot/prompt on every pointer move. Also initialise platform services in the standalone editor at start up. | `ShareX.Avalonia/Windows/ScreenColorPickerWindow`, `ShareX.ImageEditor.App/Program.cs` | color-picker session and standalone platform bootstrap (M) | todo (M); required for J2 picker and cursor-position service calls |
| R10 | `Directory.Build.props` still sets `RuntimeIdentifier` to `win-x64`/`win-arm64` on every host. Choose the host RID for portable app execution/publishing and keep Windows installer jobs explicit. J's graphics verification overrides the RID with the host runtime so Linux/macOS native Skia/HarfBuzz assets can load. | standalone editor run, full app packaging, portable graphics verification | build/runtime configuration (M) | todo (M); native Linux shaping dependency is aligned to 8.3.1.5 in J's editor project |
| R11 | Remove the now-unused legacy `DWMManager`, `TimerResolutionManager`, `DesktopIconManager` and `CursorData` classes during M3 cleanup; current branch has no consumers. Desktop icon callers already use `IShellService`; the used v22 cursor dimensions/accessibility behavior has been checked and corrected in `WindowsScreenCaptureService` under J9. Do not port unused DWM composition or timer calls. | HelpersLib cleanup / retargeting | existing services; no new contract required | todo (M); J leaves M-owned shared files for M to remove |

## Windows work (M → J)

M adds a row when a contract needs a Windows implementation. J implements it in `ShareX.Platform.Windows`, ticks the matching J1 item after running it, and marks the row `done (commit)`. M then switches the shared call site and deletes the old code.

| ID | Contract | Old Windows code to move | Linux/macOS | Status |
| --- | --- | --- | --- | --- |
| W1 | Review the Windows code M wrote: `WindowsShellIntegrationService` (file types, browser hosts, Send to), `WindowsSystemPreferencesService.GetPolicy`, `WindowsTaskbarService`, `WindowsStartupService` (`b94ebc639`, `1cf9734e9`). Take ownership; fix whatever differs from v22. | already moved | done | done (`3cc03f554`); Windows build 26300 smoke checks pass; J1 visual sign-off pending |
| W2 | `IHotkeyService` for `HotkeyManager` (`WindowsHotkeyService` exists from the first branch; check it against `WindowsHotkeyHost`) | `ShareX.HelpersLib` `WindowsHotkeyHost`, `HotkeyForm` | X11 done; Wayland portal in M9 | done (`233d37baf`); shared repeat-limit wrapper required in R5 |
| W3 | Printing (R1): contract to come from M | `ShareX.HelpersLib` print helpers, `WindowsPrintDialog` | to do (M) | waiting for contract |

## Handoffs

M adds a row when a backend API moved and J's UI files still call the old wrapper.

| ID | Old API (wrapper kept) | New API | Call sites in J's files | Status |
| --- | --- | --- | --- | --- |
| H1 | `AuthenticodeSignatureVerifier.IsTrusted` (HelpersLib) | `PlatformServices.Current.CodeSignature.IsTrusted` | `ShareX.HelpersLib/Presentation/UpdateChecker/DownloaderWindow.axaml.cs` | done (`c5c2e0a86`) |
| H2 | `WindowsList.GetVisibleWindowsList()` and `WindowInfo` (ScreenCaptureLib, HelpersLib) | `PlatformServices.Current.Windows.GetWindows()` (`PlatformWindow`); icons from `WindowManagement.GetIcon` once M5 brings it | `ShareX/Presentation/MainWindow/MainMenuBuilder.cs` (window menu) | done (`c5c2e0a86`) |
| H4 | `WindowInfo` (HelpersLib) for the inspected window: title, class, process, rectangles, styles, top most, opacity | `PlatformServices.Current.WindowManagement.GetDetails` (`WindowDetails`), `SetTopMost`, `SetOpacity` | `ShareX.Tools/Tools/InspectWindow/InspectWindowViewModel.cs` | done (`c5c2e0a86`) |
| H5 | `NativeMethods.GetCursorPos` in the monitor test window | `CaptureHelpers.GetCursorPosition()` (portable since `6bc0bd5b5`) | `ShareX.Tools/Tools/MonitorTest/MonitorTestWindow.axaml.cs` | done (`c5c2e0a86`) |
| H3 | `Screenshot.Capture…()` now return `SKBitmap?` (null when nothing could be captured) | handle null | `ShareX/Presentation/ApplicationSettings/ApplicationSettingsViewModel.cs` (print screen), `ShareX/Presentation/AutoCapture/AutoCaptureWindow.axaml.cs` (nullable warnings) | done (`c5c2e0a86`) |

## Bugs

| ID | Found by | Platform | Description | Owner | Status |
| --- | --- | --- | --- | --- | --- |
| B1 | J | Windows | Baseline platform tests fail in `FreedesktopThumbnailTests.GetThumbnailName_IsTheMd5OfTheFileUri` and `LinuxFileAssociationTests.BrowserHosts_GoToInstalledBrowsersWithAnAbsolutePath` because their fixtures assume Unix paths. Fixed with Jaex's approval (R4): 171 pass, 7 skip, 0 fail; image-effects tests: 65 pass. | M | done (`ef69ad7a7`) |
| B2 | J | Windows | `WindowsSystemPreferencesService.GetPolicy` selects any non-null HKLM value before validation. Unlike v22, an invalid machine boolean prevents a valid HKCU policy from applying; `PersonalPath` also accepts non-string registry values through `ToString`. Registry access failures now escape instead of falling back. | J | done (`3cc03f554`); verified with synthetic policy inputs |
| B3 | J | Windows | `WindowsWindowService.GetWindows` excluded `WS_EX_TOOLWINDOW` windows, whereas v22's `WindowsList.GetVisibleWindowsList` did not. This changed which windows the capture menu could list. | J | done (`c5c2e0a86`); verified with an off-screen tool window |
| B4 | J | Windows | `WindowManagement.GetDetails` returned the inspector's client rectangle in screen coordinates instead of v22's local `GetClientRect` coordinates. The capture service's screen-coordinate rectangle is separate. | J | done (`c5c2e0a86`); local origin verified (R7) |
| B5 | J | Windows | `CaptureCursorImage` allocated a fixed 64x64 image and omitted v22's accessibility cursor-size multiplier and scaled hotspot. Large cursor images could clip; standalone and direct GDI cursor placement differed from v22. | J | done (`907cdff4a`); synthetic 96x80 color/monochrome cursors, scaling, negative coordinates and GDI cleanup verified |

## Status log

One line per working session, newest at the bottom.

- 2026-10-03, M: Created `cross-platform-v2` from `develop` `fd61635f2`. Added AGENTS.md, this tracker, `docs/cross-platform-learnings.md` and `docs/cross-platform.md`. No code changes yet.
- 2026-10-03, M: Work split by preference: frontend and graphics to J, backend to M. M writes all platform contracts and implementations including Windows; J is the Windows lead and signs off every Windows change. Added first steps for each of you.
- 2026-10-03, M: Claimed M0 and M1. M0 renames `Directory.build.props` and `Directory.build.targets` to `Directory.Build.*` at the repository root in its own commit; pull after it lands.
- 2026-10-03, M: M0 done (`2c700685a` rename, `423652e73`). The solution builds on Linux with 0 errors and 0 warnings. Pull before building: `Directory.build.*` are now `Directory.Build.*`.
- 2026-10-03, M: M1 done (`021fe6d39`). Platform projects and `ShareX.Platform.Tests` (168 passing on Linux) are in the solution; CI workflow `platform.yml` added. Edited J's `ShareX.ImageEffectsLib.csproj` (one line: `InternalsVisibleTo` for the preset tests, build-only). `ShareX.ImageEffectsLib.Tests` runs on Windows only until M7, because ImageEffectsLib still needs the Windows desktop runtime.
- 2026-10-03, M: Claimed M4. Bringing the first branch's finished ScreenCaptureLib port (develop has not changed ScreenCaptureLib since). M claims these J files until M4's push: `ScreenCaptureLib/Presentation/RegionCapture/*`, `ScreenRecording/ScreenRecordWindow.axaml.cs`, `ScrollingCapture/*`, `FFmpegOptions/FFmpegOptionsWindow.axaml.cs`.
- 2026-10-03, M: M3 steps pushed: HelpersLib system helpers (`43baac451`) and CaptureHelpers (`6bc0bd5b5`) through the services; new contracts `ICodeSignatureService`, `IClipboardService.FormatNames`/`EncodeHtml`, `IWindowService.GetWindowBounds`/`GetClientBounds`/`GetActiveWindowHandle`. Handoff H1.
- 2026-10-03, M: M4 code pushed (`f4c55173b`); ScreenCaptureLib retargets to net10.0 with HelpersLib (end of M3). **Released** the claimed `ScreenCaptureLib/Presentation` files. New file for J: `ShareX.Avalonia/Theming/TrayIconRenderer.cs` (Skia tray glyphs, from the first branch). Handoffs H2 (window menu) and H3 (nullable screenshots).
- 2026-10-03, M: M5 Tools backend pushed: OCR through `IOcrService` (WinRT on Windows, Tesseract on Linux), mouse highlighter hook and overlay through the platform (from the first branch's stash), inspect and borderless window services through `IWindowManagementService`. Handoffs H4 (inspect view model) and H5 (monitor test).
- 2026-10-03, M: Note for J: M5 changed a J file without claiming it first: `ShareX.Tools/Tools/MouseHighlighter/MouseHighlighterOverlayWindow.cs` (drawing now targets the platform overlay surface, `IScreenOverlay`, instead of a WinForms layered window). The Skia drawing code is unchanged.
- 2026-10-03, M: Claimed M6 (application backend files). Plan for M3's end: Win32 types that J's files still use (`WindowInfo`, `TaskbarManager`, print helpers, `LucideTrayIcon`) become portable façades with the same API over the platform services, so J's files keep building and work on Linux; the handoffs become clean-ups.
- 2026-10-03, M: M6 steps pushed: startup, shell menus, file types, browser hosts, Send to and admin policies through the platform services (`b94ebc639`); taskbar progress through new `ITaskbarService` (R2 done; Linux uses the Unity launcher API, which KDE and Dash to Dock show). `TaskbarManager` is now a portable façade in `HelpersLib/TaskbarManager.cs`; J's settings view model needs no change. Two J1 checklist items added.
- 2026-10-03, M: **Ownership change (McoreD's decision):** `ShareX.Platform.Windows` is now J's. J writes, runs and signs off all Windows code; M does contracts, Linux, macOS and shared call sites, and hands Windows work over as W rows. New task J9 (Windows platform) and table "Windows work" (W1 to W3). M3 no longer moves Win32 code itself.
- 2026-10-03, J: Pulled `cross-platform-v2` and claimed W1/J1. Following Jaex's goal objective, Windows verification starts on this branch; the first-steps wording about checking out the frozen first branch is superseded by that instruction. Reviewing shell integration, startup, policy and taskbar behavior against `develop` before the remaining Windows checklist. No file moves planned in this claim.
- 2026-10-03, J: Jaex explicitly approved the two M-owned Windows test-fixture fixes in R4 to unblock the claim push. Only the thumbnail hashing fixture and browser-host manifest assertion are changed; no Linux implementation or platform contract changes.
- 2026-10-03, J: J owns the new Windows verification utility in `Scripts/WindowsVerification/` and `Scripts/Verify-WindowsPlatform.ps1`. It exercises J's Windows services using a temporary harness, isolated registry keys and temporary shortcut folders; it does not replace the Windows 10/11 visual checklist or edit M's test project.
- 2026-10-03, J: W1 reviewed against `develop`. Fixed policy validation/fallback and registry access handling, case-insensitive file/browser registration detection, and escaping of quoted/trailing-backslash arguments. `Scripts/Verify-WindowsPlatform.ps1` passes all five check groups on Windows build 26300, including a temporary Task Manager disabled-startup value and taskbar COM calls against a hidden harness window. Release solution build: 0 errors; platform tests: 171 pass/7 Unix-only skips; image-effects tests: 65 pass. Windows 10/11 visual, HDR and mixed-DPI checklist items remain unchecked; Linux/macOS execution is not available on this host. Claimed W2 next; no file moves in this task.
- 2026-10-03, J: W2 Windows service is ready: preserves native repeat events for the shared R5 throttle; rejects invalid keys/modifiers; prevents cancelled commands from registering later; cancels queued callers on shutdown; isolates subscriber exceptions and ignores stale key messages; disposal from a callback avoids joining its own thread. Windows verification now passes seven groups, including real native registration conflicts and a blocked-callback timeout. Release solution build and both test projects pass (171 platform tests, 7 Unix-only skips; 65 image-effects tests). Claimed J7/H1 to H5 next; no moves planned.
- 2026-10-03, J: H1 to H5 switched: updater signature service, platform window menu and PNG icons, nullable print/auto-capture results, portable inspector details/topmost/opacity, and monitor-test cursor position. Unsupported window capture is disabled with the service reason (tooltip in Avalonia; label in native tray menus). B3/B4 restore v22 tool-window enumeration and local inspector client coordinates. Windows verification passes eight groups, including the real inspector view model on a synthetic window. R6/R7 request granular feature reasons and client-coordinate documentation from M. Claimed J2 next; no file moves.

- 2026-10-03, J: J2 graphics increment: Skia/HarfBuzz shapes installed Noto Color Emoji, Apple Color Emoji or Segoe UI Emoji, with a supported monochrome fallback. Windows retains Direct2D first. Added shared font resolution for annotations, watermarks, filters and ImageFont; saved family names stay unchanged. Image insertion uses storage-provider local paths and the cursor platform service, falling back to the editor screen when global pointer coordinates are unavailable; removed its Win32 declarations. Scripts/Verify-EditorGraphics.ps1 passes system-color-font previews/stickers, ZWJ shaping, cache ownership and all 13 bundled cursors; Noto bitmap font fixture at googlefonts/noto-emoji e20cbc2bbec1926686be9f9bee7d1d2cfa1fea0e also passes and both contact sheets were inspected. Release solution build: 0 errors/0 warnings; tests: 171 platform pass/7 Unix-only skips, 65 image-effects pass. Linux/macOS desktop execution is unavailable (WSL is not installed). R8/R9/R10 request native renderer contracts, picker sessions/bootstrap and host RID selection from M; J2 remains in progress. No file moves in this increment.

- 2026-10-03, J: Claimed J9 after J2 graphics push d6f3d60c6. First audit found no consumers of DWMManager, TimerResolutionManager, DesktopIconManager or CursorData, so their unused code does not need porting. WindowsScreenCaptureService already contains cursor rendering, but its 64x64 standalone buffer can clip large cursors and omits v22's accessibility size multiplier/hotspot scaling. J will correct and verify this existing Windows service first; no project-to-project file moves in this increment. Shared caller removal remains coordinated with M.
- 2026-10-03, J: J9 cursor correction: allocation uses native color/mask dimensions (monochrome mask height is halved), accessibility size and hotspot scale match develop, and standalone/direct GDI capture share those metrics. Native rendering reports allocation/draw failures and frees resources. Windows verification passes nine groups, including synthetic 96x80 color and monochrome cursors at default/2x/2.5x sizes, bottom-right pixels beyond the former 64x64 limit, negative origins and 80-repeat GDI handle checks. Full release build: 0 errors/0 warnings; both test projects pass (171/7 skips and 65). R11 asks M to remove four unused shared helpers; no files moved and J1 visual sign-off remains pending.

- 2026-10-03, J: Jaex requested proper test projects instead of the verification scripts J added on cross-platform-v2. Announcing the moves before editing: Scripts/WindowsVerification/Program.cs -> Windows tests in ShareX.Platform.Tests (native checks) and a new ShareX.Tools.Tests (inspector view model); Scripts/EditorGraphicsVerification/Program.cs -> new ShareX.ImageEditor.Tests. Delete only J's Verify-WindowsPlatform.ps1 and Verify-EditorGraphics.ps1 launchers; all scripts present at branch creation remain. This explicit request authorizes the required test-project/source and solution changes, including the M-owned platform tests. Existing J9 thumbnail/color-dialog edits remain a separate increment.
- 2026-10-03, J: Jaex asked to pause build.yml and platform.yml for cross-platform-v2 until the port is complete. Both workflows now exclude pushes to that branch; platform.yml also excludes PRs targeting it and skips jobs for PRs opened from it. Other branch and release-tag triggers stay enabled. Re-enable these exclusions/guards after the port is complete; this pause is explicitly authorized, including platform.yml ownership.
