# Cross-platform delegation and tracker (v2)

Two developers, each with a coding agent, make ShareX cross-platform together on one branch, `cross-platform-v2`. This file says who owns what, how the two avoid getting in each other's way, and where every task stands. Both agents read it, together with [AGENTS.md](../AGENTS.md) and [the learnings](cross-platform-learnings.md), before starting work, and update it in the same commit as the work.

| | Person | Agent | Machines | Leads |
| --- | --- | --- | --- | --- |
| **J** | Jaex | Jaex's agent | Windows and Linux | The platform contracts (`ShareX.Platform`), Windows (`ShareX.Platform.Windows`, Windows verification), the application and its features |
| **M** | McoreD | McoreD's agent | Linux (Omarchy, Hyprland) | Linux and macOS (`ShareX.Platform.Linux`, `.MacOS`), build, CI and packaging |

Both develop and test on Linux. Only J can verify Windows behaviour.

**Start here:** the [first steps](#first-steps) section says what each of you does first.

## Goal

ShareX is cross-platform when all of these hold:

1. The `ShareX` application (main window, tray, hotkeys, capture, upload, history, editor and tools) builds and runs on Windows, Linux and macOS. Shared projects target plain `net10.0`; Windows-only features come from `ShareX.Platform.Windows`.
2. No shared project contains P/Invoke, registry access, WinRT calls or `OperatingSystem.Is…()` branches (AGENTS.md, "Forbidden in shared code").
3. Every Windows feature still works on Windows exactly as in v22, verified on Windows.
4. On Linux (X11, and Wayland on Hyprland, sway, GNOME and KDE) and macOS, every feature works or is disabled with a reason that tells the user what to install or why it is not possible.
5. CI builds and tests on all three operating systems. Linux has an install path (script, then a package).

## What the work is

Most of it is refactoring: `develop` is complete and correct on Windows, and every piece of Windows-only code in it (P/Invoke, registry, WinRT, `OperatingSystem.Is…` branches) moves behind an interface in `ShareX.Platform`, into `ShareX.Platform.Windows`, with no change in behaviour. Callers switch to `PlatformServices.Current`, and each project then retargets to `net10.0`. That is J's lane (J2 to J8) and M's caller lane (M3 to M5).

The refactoring alone does not make ShareX run elsewhere. The rest is:

1. **Linux and macOS implementations of every contract.** Most exist from the first branch and arrive in M1; new contracts get them as J adds them (M11).
2. **Features with no equivalent.** Wayland does not let applications list windows, move the pointer or watch global input, so those features report `FeatureSupport.NotSupported` with a reason, and the UI hides or disables them instead of failing (AGENTS.md, step 4).
3. **Gaps that need new code,** not a port: Wayland screen recording, audio sources, global shortcuts through the portal, macOS OCR and window management (M6, M7).
4. **Running the real application** on Linux and macOS: start up choosing the platform (J7), Avalonia behaviour on each desktop (full-screen overlays, tray, multi-monitor scaling), packaging and CI (M8, M9).

## Branches

- **`cross-platform-v2`**, created from `develop` at `fd61635f2` on 2026-10-03, after Jaex finished the SkiaSharp and Avalonia migration. Both agents commit here. There is no second development branch to merge, which is the main lesson of the first attempt.
- **`develop`**: v22 hotfixes only. Whoever makes a hotfix merges it into `cross-platform-v2` the same day.
- **`cross-platform`**: the first attempt, frozen at `55e0c7d90`. Never merged. Pieces are brought over by path; [the learnings](cross-platform-learnings.md#reusable-work-on-the-first-branch) list them.
- When the goal is met, `cross-platform-v2` becomes the main line (merged into `develop` for v23, or replacing it). Jaex and McoreD decide; record it here.

## Ownership

The platform abstraction has three layers: the **contracts** (`ShareX.Platform`), the **implementations** (`ShareX.Platform.Windows`, `.Linux`, `.MacOS`) and the **callers** (every shared project and the application). Ownership follows the layers, and within the callers it is split by project.

| Layer | Project or files | Owner | Notes |
| --- | --- | --- | --- |
| Contracts | `ShareX.Platform` (interfaces, models, `PlatformServices`, detection) | **J** | One owner, because every contract change touches all three implementations. M asks for changes through the [request log](#requests); see the [contract protocol](#changing-a-contract). Exception: `LinuxPackages.cs` and `LinuxDistribution.cs` (install hints and distribution detection) belong to M. |
| Windows | `ShareX.Platform.Windows` | **J** | J leads all Windows work. |
| Linux | `ShareX.Platform.Linux` | **M** | J tests on Linux too and files bug rows; J may take a Linux task by claiming it here. |
| macOS | `ShareX.Platform.MacOS` | **M** | Needs a Mac tester (M7). |
| Tests | `ShareX.Platform.Tests` | Both | Each adds tests for what they own, in separate files: J `CoreTests.cs` and `WindowsTests.cs`, M `LinuxTests.cs`, `MacOSTests.cs` and `DetectionTests.cs`. |
| Callers: Windows code that moves into `ShareX.Platform.Windows` | `ShareX.HelpersLib` (all of it, including `Native/`), the `ShareX` application, `ShareX.NativeMessagingHost` | **J** | This code moves into J's Windows project, so J moves it, switches the call sites and retargets these projects to `net10.0`. |
| Callers: code whose Windows implementation already exists from the first branch | The operating system files of `ShareX.ScreenCaptureLib`, `ShareX.Tools`, `ShareX.ImageEditor`, `ShareX.Avalonia` and `ShareX.HistoryLib`, listed [below](#operating-system-files-owned-by-m), and the target framework of `ScreenCaptureLib` and `Tools` | **M** | The Windows side (GDI, HDR, transparent capture, OCR, mouse hook, overlays, window management) was written on the first branch and arrives with `ShareX.Platform.Windows` in M1. M changes only the shared side. A Windows fix goes to J as a bug row. |
| Callers: everything else | All other files in every shared project, `UploadersLib`, `ImageEffectsLib`, `HistoryLib` (except the file below), feature work everywhere | **J** | |
| Build | `Directory.Build.*`, `.github/workflows/platform.yml`, `docs/cross-platform.md` | M | |
| Build | `.github/workflows/build.yml`, `ShareX.Setup`, `ShareX.Steam`, Windows installer and release scripts | J | Setup and Steam stay Windows-only. |
| Packaging | `Scripts/install-linux.sh`, AppImage, Flatpak, macOS bundle | M | |
| Shared docs | `AGENTS.md`, `docs/cross-platform-learnings.md`, this file | Both | Edit only your own rows; keep edits small. |

### Operating system files owned by M

State at `fd61635f2`. J owns every other file in these projects.

| Project | Files |
| --- | --- |
| `ShareX.ScreenCaptureLib` | `HDRScreenCapture.cs`, `Screenshot.cs`, `Screenshot_Transparent.cs`, `ScrollingCaptureManager.cs`, `Helpers/SimpleWindowInfo.cs`, `Helpers/WindowsList.cs`, `Helpers/WindowsRectangleList.cs`, `Presentation/ScrollingCapture/ScrollingCaptureRegionWindow.axaml.cs`, and the tray and window-region parts of `Presentation/ScreenRecording/ScreenRecordWindow.axaml.cs` (claim the file before editing) |
| `ShareX.Tools` | `Tools/OCR/OCRHelper.cs`, `Tools/MouseHighlighter/MouseHighlighterMouseHook.cs`, `Tools/MouseHighlighter/MouseHighlighterOverlayWindow.cs`, `Tools/InspectWindow/InspectWindowService.cs`, `Tools/InspectWindow/InspectWindowViewModel.cs`, `Tools/BorderlessWindow/BorderlessWindowInfo.cs`, `Tools/BorderlessWindow/BorderlessWindowManager.cs`, `Tools/MonitorTest/MonitorTestWindow.axaml.cs` |
| `ShareX.ImageEditor` | `Integration/EditorServices.cs`, `Integration/*DesktopWallpaperService.cs`, `Presentation/Emoji/WindowsEmojiBitmapRenderer.cs`, `Presentation/Rendering/WindowsCursorBitmapRenderer.cs`, `Presentation/Views/EditorView.ImageInsert.cs` |
| `ShareX.Avalonia` | `Windows/ScreenColorPickerWindow.axaml.cs` |
| `ShareX.HistoryLib` | `Models/ImageHistoryModels.cs` (operating system thumbnail part; claim before editing) |

If a file not listed anywhere turns out to contain operating system code, the agent who finds it adds it to the right table in its own commit before touching it. By default it belongs to J if its project is J's, and to M if its Windows implementation already exists on the first branch.

## How the agents work together

1. **Pull, claim, push.** Pull before starting. Set the task to `in progress (date)` and push that one-line change before writing code. Push small commits often, and only when `dotnet build ShareX.sln -c Release -p:Platform=x64` has 0 errors and every test project passes.
2. **Commit subjects start with the task id,** for example `J2: Move DPAPI behind ISecretProtectionService`. Finish a task by setting it to `done (commit)`.
3. **Requests (M → J).** When M needs a new contract or a change to one (usually for Linux or macOS), M adds a request row. J follows the contract protocol and marks it `ready (commit)`.
4. **Bugs cross layers by row, not by edit.** A problem in someone else's files is a bug row with the owner set; the owner fixes it. Exception: a one-line fix that blocks the build may be made by either agent, recorded in the status log.
5. **Handoffs (M → J).** When M changes a shared API in M's files that J's files still call, M keeps the old API as a thin wrapper over the new one, lists the call sites in the handoff log, and deletes the wrapper once J has switched them. The build stays green and nobody edits the other's files.
6. **No file moves across projects without an announcement.** A move is its own commit, written in the status log first; the other agent pulls before continuing (lesson 3).
7. **Both test on Linux; only J signs off Windows.** Anything in `ShareX.Platform.Windows` that has not run on Windows is listed in the J1 checklist.
8. **Blocked?** Set `blocked: reason` and pick another task.
9. **Status log.** One line per working session at the bottom: date, agent, what changed, files claimed or released.

Status values: `todo`, `in progress (YYYY-MM-DD)`, `blocked: reason`, `review` (needs the other agent or a human), `done (commit)`.

### Changing a contract

1. J adds the interface or member to `ShareX.Platform`, and in the same commit adds a "not supported" implementation to `ShareX.Platform.Windows`, `.Linux` and `.MacOS` (using `UnsupportedServices.cs` where possible) and to test fakes, so the build stays green. Adding these stubs is the only edit J makes in M's Linux and macOS projects.
2. The commit is recorded in the request row and the status log.
3. Each platform owner replaces the stub with a real implementation in their own project.
4. Changing or removing an existing member needs the other agent's `review` first, because it breaks the other side's code.

## Order of work

1. **Foundation (M0, M1, then J0).** Build plumbing, then the platform projects (brought over by M, then handed to J), then platform start up in the application. This takes about a day; J verifies Windows on the first branch (J1) meanwhile.
2. **Callers, in parallel.** J moves `HelpersLib` and the application's operating system code into `ShareX.Platform.Windows` and switches its call sites (J2 to J5). M switches `ScreenCaptureLib`, `Tools`, `ImageEditor` and `Avalonia` to the services (M3 to M5). The two lanes touch different files.
3. **Retarget.** `HelpersLib` moves to `net10.0` first (J2), then the libraries that depend on it (J6, M3, M4), then the application (J7).
4. **Run everywhere.** Linux and macOS gaps, packaging, and an end-to-end run of the real application on Linux by both of you and on Windows by J (M6 to M10, J1).

## First steps

### McoreD (Agent M)

1. **M0, today.** Rename `Directory.build.props` and `Directory.build.targets` to `Directory.Build.*`, set `EnableWindowsTargeting` off Windows, bring `.github/workflows/platform.yml` over, and get the solution building on Linux. Announce the rename in the status log; it touches the repository root.
2. **M1, right after.** Bring the four platform projects and `ShareX.Platform.Tests` (168 tests) and `ShareX.ImageEffectsLib.Tests` (65 legacy preset tests) from the first branch, add them to the solution and make the tests pass. Then hand `ShareX.Platform` and `ShareX.Platform.Windows` to J in the status log.
3. **Then M3:** start the shared side of `ScreenCaptureLib` (snap targets, `Screenshot` facade, scrolling capture), which does not wait for `HelpersLib`'s retarget until the very end.

### Jaex (Agent J)

1. **J1, now, in parallel with M0 and M1.** On Windows, build the first branch (`git checkout cross-platform`, `dotnet build ShareX.sln -c Release -p:Platform=x64`) and work through the [Windows checklist](#j1-windows-verification-checklist). That code is what M1 brings over, so every bug found now is fixed before it lands. File bug rows on `cross-platform-v2`.
2. **Linux set-up, now.** Install the .NET 10 SDK and the Linux tools on your Linux machine (on Arch: `sudo pacman -S --needed dotnet-sdk wl-clipboard grim slurp libnotify libsecret ffmpeg tesseract tesseract-data-eng`), and build `cross-platform-v2` there once M0 lands.
3. **J0, once M1 lands.** Call `PlatformServices.Initialize(new WindowsPlatformServices())` at start up and `PlatformServices.Shutdown()` on exit.
4. **Then J2:** start moving `HelpersLib`'s Windows code into `ShareX.Platform.Windows`. Take the areas in this order, because later areas and M's lane depend on the early ones: secrets (DPAPI), clipboard, shell (open, reveal), system information, screens and cursor, MIME types, thumbnails, then hotkeys, tray, registry, DWM, taskbar, printing and the rest. Most of these services and their Windows implementations already exist in M1's projects; the work is switching `HelpersLib` to them and deleting the old interop.

## Tasks: Agent M

| ID | Task | Depends on | Status |
| --- | --- | --- | --- |
| M0 | Build plumbing: rename `Directory.build.props` and `Directory.build.targets` to `Directory.Build.*` (one commit, announced, lesson 14), set `EnableWindowsTargeting` off Windows, bring `.github/workflows/platform.yml`, and make `dotnet build ShareX.sln -c Release -p:Platform=x64` pass on Linux. | none | todo |
| M1 | Bring `ShareX.Platform`, `.Windows`, `.Linux`, `.MacOS`, `ShareX.Platform.Tests` and `ShareX.ImageEffectsLib.Tests` from the first branch; add them to `ShareX.sln`; tests pass; hand `ShareX.Platform` and `ShareX.Platform.Windows` to J. | M0 | todo |
| M3 | `ScreenCaptureLib` → `net10.0`: re-apply the design of first-branch commit `4a25a9e04` to the current code (`Screenshot` facade over `IScreenCaptureService`, snap targets, scrolling capture through `IInputService`, frame windows through `SetWindowShape`/`SetOverlayStyle`, recording devices from the platform). Retarget once `HelpersLib` is `net10.0`. | M1 (retarget: J2) | todo |
| M4 | `Tools` operating system files → services: OCR (`IOcrService`), mouse highlighter (`HookMouse`, `CreateOverlay`), inspect and borderless window (`IWindowManagementService`), monitor test. Reuse the first branch's stash. Retarget once `HelpersLib` is `net10.0`. | M1 (retarget: J2) | todo |
| M5 | `ImageEditor` and `ShareX.Avalonia` operating system files: `IDesktopWallpaperService` (request R6 to J; M implements Linux and macOS), emoji and cursor renderers, image insert, screen colour picker. `HistoryLib` thumbnails through `IThumbnailService`. | M1 | todo |
| M6 | Linux gaps: Wayland screen recording (portal ScreenCast and PipeWire, or wf-recorder/wl-screenrec on wlroots), PulseAudio/PipeWire audio for FFmpeg, X11 mouse hook and overlay, window icons on Wayland, region capture overlay behaviour on Hyprland, sway, GNOME and KDE. | M3, M4 | todo |
| M7 | macOS: OCR with Vision, window management through the Accessibility API, mouse hook (CGEventTap) and overlay, recording on Apple silicon. Needs a Mac tester. | M3, M4 | todo |
| M8 | Packaging and CI: build and test the real application on Linux and macOS in CI once J7 lands; Linux installer for the real application; then AppImage or Flatpak; keep `docs/cross-platform.md` current. | J7 | todo |
| M9 | Linux end-to-end run of the real application on Omarchy (Hyprland), GNOME and KDE: main window, tray, hotkeys, every capture mode, upload, history, editor, every tool. Both of you run it; file bug rows. | J7 | todo |
| M10 | Decide with Jaex whether `ShareX.Desktop` (the first branch's host and `sharex` command line) and `ShareX.Destinations` come over, are folded into the application, or are dropped. | J7 | todo |
| M11 | Implement the Linux and macOS side of every contract J adds, replacing the stubs. | contracts | ongoing |

## Tasks: Agent J

| ID | Task | Depends on | Status |
| --- | --- | --- | --- |
| J0 | Call `PlatformServices.Initialize(new WindowsPlatformServices())` at application start up and `Shutdown()` on exit. | M1 | todo |
| J1 | **Verify on Windows** everything in `ShareX.Platform.Windows`, using the checklist below. Start now on the first branch; continue on `cross-platform-v2` after M1. File bug rows. | none | todo |
| J2 | `HelpersLib` → `net10.0` with CA1416 as an error. Move its Windows code (`Native/`, `CursorData`, `DWMManager`, `DesktopIconManager`, `TimerResolutionManager`, `RegistryHelpers`, `WindowsImageInterop`, `AuthenticodeSignatureVerifier`, `AvaloniaClipboard` and `DesktopScreen` branches, `LucideTrayIcon`, `InputManager`, `WindowsHotkeyHost`, printing, and the Windows members of `Helpers`, `FileHelpers`, `CaptureHelpers`, `ClipboardHelpers`, `Extensions`, `MimeTypes`) into `ShareX.Platform.Windows` behind services, in the order given under [first steps](#jaex-agent-j). | M1, J0 | todo |
| J3 | Application hotkeys and tray: switch `HotkeyManager` to `IPlatformServices.Hotkeys`, `TrayIconService` to Avalonia `TrayIcon` with `DesktopServices.RegisterTrayIcon`, and retire whatever is left of `Forms/MainForm.cs`. | J2 (hotkey service) | todo |
| J4 | Application Win32 calls: `CaptureHelpers/CaptureBase.cs`, `CaptureHelpers/CaptureCustomWindow.cs`, `CaptureHelpers/CaptureWindow.cs`, `Presentation/MainWindow/ThumbnailItemViewModel.cs`, `ScreenRecordManager.cs`, `TaskHelpers.cs` → `IWindowService` and `IWindowManagementService`. | M1, J0 | todo |
| J5 | Application registry: `StartupManager.cs`, `SystemOptions.cs`, `IntegrationHelpers.cs` → `IStartupService`, `IShellIntegrationService` and the settings files (requests R3, R4). | M1, requests | todo |
| J6 | Retarget `HistoryLib`, `UploadersLib` and `ImageEffectsLib` to `net10.0` with CA1416 as an error. | J2 | todo |
| J7 | Retarget the `ShareX` application to `net10.0` plus `net10.0-windows10.0.22621.0` (Windows release, for WinRT features). Choose `WindowsPlatformServices`, `LinuxPlatformServices` or `MacPlatformServices` at start up. Run it on Linux. | J2 to J6, M3, M4 | todo |
| J8 | `NativeMessagingHost` → `net10.0`: `CreateProcess` with job breakaway and browser manifest registration (R3) behind services. | M1 | todo |
| J9 | Serve M's requests (contract protocol), implement the Windows side of every contract, and switch call sites listed in the handoff log. | requests, handoffs | ongoing |

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

## Requests

Either agent asks for contract changes here (most come from M for Linux and macOS needs, or from J's own migration in J2); J adds the contract and fills in the interface and status.

| ID | Need | Call sites | Interface | Status |
| --- | --- | --- | --- | --- |
| R1 | Printing: print dialog, preview, print an image or text | `HelpersLib/Printer`, print windows | | todo |
| R2 | Taskbar progress and overlay | application upload progress | | todo |
| R3 | Browser native-messaging host registration (Chrome, Firefox) | `IntegrationHelpers`, `NativeMessagingHost` | | todo |
| R4 | Shortcut (`.lnk`) creation | application integration settings | probably `IStartupService`/`IShellIntegrationService` | todo |
| R5 | Synthetic input beyond scrolling: typing text, key combinations | application actions | extend `IInputService` | todo |
| R6 | Desktop wallpaper (set an image as wallpaper) | `ImageEditor/Integration` | `IDesktopWallpaperService` (M5) | todo |

## Handoffs

M adds a row when a shared API changed and J's files still call the old wrapper.

| ID | Old API (wrapper kept) | New API | Call sites in J's files | Status |
| --- | --- | --- | --- | --- |

## Bugs

| ID | Found by | Platform | Description | Owner | Status |
| --- | --- | --- | --- | --- | --- |

## Status log

One line per working session, newest at the bottom.

- 2026-10-03, M: Created `cross-platform-v2` from `develop` `fd61635f2`. Added AGENTS.md, this tracker, `docs/cross-platform-learnings.md` and `docs/cross-platform.md`. No code changes yet.
- 2026-10-03, M: Ownership by layer: J leads the contracts (`ShareX.Platform`) and Windows (`ShareX.Platform.Windows`, `HelpersLib`, application); M leads Linux and macOS. Both test on Linux. Added first steps for each of you.
