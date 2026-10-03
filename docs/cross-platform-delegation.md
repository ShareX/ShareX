# Cross-platform delegation and tracker (v2)

Two coding agents make ShareX cross-platform together on one branch, `cross-platform-v2`. This file says who owns what, how they avoid getting in each other's way, and where every task stands. Both agents read it, together with [AGENTS.md](../AGENTS.md) and [the learnings](cross-platform-learnings.md), before starting work, and update it in the same commit as the work.

- **Agent J**: Jaex's agent. The application, its features and its Windows behaviour. Has a Windows machine.
- **Agent M**: McoreD's agent. The platform layer (`ShareX.Platform*`), moving operating system code behind it, Linux and macOS, packaging and CI. Works on Linux (Omarchy, Hyprland).

## Goal

ShareX is cross-platform when all of these hold:

1. The `ShareX` application (main window, tray, hotkeys, capture, upload, history, editor and tools) builds and runs on Windows, Linux and macOS. Shared projects target plain `net10.0`; Windows-only features come from `ShareX.Platform.Windows`.
2. No shared project contains P/Invoke, registry access, WinRT calls or `OperatingSystem.Is…()` branches (AGENTS.md, "Forbidden in shared code").
3. Every Windows feature still works on Windows exactly as in v22, verified by running it on Windows.
4. On Linux (X11, and Wayland on Hyprland, sway, GNOME and KDE) and macOS, every feature works or is disabled with a reason that tells the user what to install or why it is not possible.
5. CI builds and tests on all three operating systems. Linux has an install path (script, then a package).

## Branches

- **`cross-platform-v2`**, created from `develop` at `fd61635f2` on 2026-10-03, after Jaex finished the SkiaSharp and Avalonia migration. Both agents commit here from the start. There is no second development branch to merge, which is the main lesson of the first attempt.
- **`develop`**: v22 hotfixes only. Whoever makes a hotfix merges it into `cross-platform-v2` the same day.
- **`cross-platform`**: the first attempt, frozen at `55e0c7d90`. Never merged. Pieces are brought over by path; [the learnings](cross-platform-learnings.md#reusable-work-on-the-first-branch) list them.
- When the goal is met, `cross-platform-v2` becomes the main line (merged into `develop` for v23, or replacing it). Jaex and McoreD decide; record it here.

## Ownership

Ownership is by file, so the two agents never edit the same file at the same time.

**Agent M owns:**

- `ShareX.Platform`, `ShareX.Platform.Windows`, `ShareX.Platform.Linux`, `ShareX.Platform.MacOS`, `ShareX.Platform.Tests`, and any project brought from the first branch for its own work (`ShareX.Desktop`, `ShareX.Destinations` and their tests, if M10 keeps them).
- The operating system files listed below, which M moves behind services and then deletes or makes portable.
- Build and CI plumbing for other operating systems: `Directory.Build.*`, `.github/workflows/platform.yml`, `Scripts/install-linux.sh`, Linux and macOS packaging, `docs/cross-platform.md`.
- Target framework changes in the `.csproj` of the projects whose operating system files M migrates (`HelpersLib`, `ScreenCaptureLib`, `Tools`, `NativeMessagingHost`).
- `ShareX.NativeMessagingHost` as a whole (it is one file).

**Agent J owns everything else.** That is the `ShareX` application (including its own Win32 call sites, which J switches to services), all feature work in every project, `ShareX.Setup`, `ShareX.Steam`, `.github/workflows/build.yml`, and the Windows installer and release scripts.

**Operating system files owned by Agent M** (state at `fd61635f2`):

| Project | Files |
| --- | --- |
| `ShareX.HelpersLib` | `Native/*` (all), `CursorData.cs`, `DWMManager.cs`, `DesktopIconManager.cs`, `TimerResolutionManager.cs`, `Helpers/RegistryHelpers.cs`, `Helpers/WindowsImageInterop.cs`, `Helpers/AuthenticodeSignatureVerifier.cs`, `Helpers/AvaloniaClipboard.cs`, `Helpers/DesktopScreen.cs`, `Helpers/LucideTrayIcon.cs`, `Input/InputManager.cs`, `Input/WindowsHotkeyHost.cs`, `Printer/WindowsPrintDialog.cs` |
| `ShareX.ScreenCaptureLib` | `HDRScreenCapture.cs`, `Screenshot.cs`, `Screenshot_Transparent.cs`, `ScrollingCaptureManager.cs`, `Helpers/SimpleWindowInfo.cs`, `Helpers/WindowsList.cs`, `Helpers/WindowsRectangleList.cs`, `Presentation/ScreenRecording/ScreenRecordWindow.axaml.cs` (tray and window region parts), `Presentation/ScrollingCapture/ScrollingCaptureRegionWindow.axaml.cs` |
| `ShareX.Tools` | `Tools/OCR/OCRHelper.cs`, `Tools/MouseHighlighter/MouseHighlighterMouseHook.cs`, `Tools/MouseHighlighter/MouseHighlighterOverlayWindow.cs`, `Tools/InspectWindow/InspectWindowService.cs`, `Tools/InspectWindow/InspectWindowViewModel.cs`, `Tools/BorderlessWindow/BorderlessWindowInfo.cs`, `Tools/BorderlessWindow/BorderlessWindowManager.cs`, `Tools/MonitorTest/MonitorTestWindow.axaml.cs` |
| `ShareX.ImageEditor` | `Integration/EditorServices.cs`, `Integration/*DesktopWallpaperService.cs`, `Presentation/Emoji/WindowsEmojiBitmapRenderer.cs`, `Presentation/Rendering/WindowsCursorBitmapRenderer.cs`, `Presentation/Views/EditorView.ImageInsert.cs` |
| `ShareX.Avalonia` | `Windows/ScreenColorPickerWindow.axaml.cs` |
| `ShareX.HistoryLib` | `Models/ImageHistoryModels.cs` (OS thumbnail part) |

**Mixed files.** `HelpersLib`'s `Helpers/Helpers.cs`, `Helpers/FileHelpers.cs`, `Helpers/CaptureHelpers.cs`, `Helpers/ClipboardHelpers.cs`, `Extensions/Extensions.cs`, `MimeTypes.cs`, `Printer/PrintHelper.cs`, `Presentation/ColorPicker/ColorPickerWindowIntegration.cs` and `Presentation/UpdateChecker/UpdateMessageWindow.axaml.cs` hold both portable code and Windows code. Agent J owns them. Agent M changes only their Windows members, and only after claiming the file in the status log ("M claims Helpers.cs for M2") and releasing it when pushed.

If a file not listed here turns out to contain operating system code, the agent who finds it adds it to this table in its own commit before touching it.

## How the agents work together

1. **Pull, claim, push.** Pull before starting. Set the task to `in progress (date)` and push that one-line change before writing code. Push small commits often, and only when `dotnet build ShareX.sln -c Release -p:Platform=x64` has 0 errors and every test project passes.
2. **Commit subjects start with the task id,** for example `M2: Move DPAPI behind ISecretProtectionService`. Finish a task by setting it to `done (commit)`.
3. **New platform capabilities go through the request log.** When J needs something from the operating system, J adds a request row with the call sites. M designs the interface, implements Windows (same behaviour as v22), Linux and macOS, adds tests, and marks it `ready (commit)`. J then switches the call sites in J's files.
4. **Call-site handoffs go the other way.** When M moves an API behind a service and J's files still call the old API, M leaves the old API in place as a thin wrapper over the service, lists the call sites in the handoff log, and J switches them. M deletes the wrapper once the handoff row is `done`. This keeps the build green without either agent editing the other's files.
5. **No file moves across projects without an announcement.** A move is its own commit, written in the status log first; the other agent pulls before continuing (lesson 3).
6. **Windows verification belongs to J.** Anything M puts into `ShareX.Platform.Windows` is listed in the J1 checklist until J has run it on Windows.
7. **Blocked?** Set `blocked: reason` and pick another task.
8. **Status log.** One line per working session at the bottom: date, agent, what changed, files claimed or released.

Status values: `todo`, `in progress (YYYY-MM-DD)`, `blocked: reason`, `review` (needs the other agent or a human), `done (commit)`.

## Order of work

The tasks are ordered to keep the build green and conflicts at zero:

1. **Foundation (M0, M1, J0).** Build plumbing, then the platform projects, then platform start up in the application. Small and done first.
2. **Libraries, in parallel.** M migrates `HelpersLib`, `ScreenCaptureLib`, `Tools`, `ImageEditor` and `Avalonia` operating system files (M2 to M5). J switches the application's own call sites (J2 to J5) and verifies Windows behaviour (J1). Handoffs and requests connect the two.
3. **Retarget.** Libraries move to `net10.0` as their operating system files are cleared (M2 to M4, J6), then the application (J7).
4. **Other platforms.** Linux and macOS gaps, packaging, an end-to-end Linux run of the real application (M6 to M9).

## Tasks: Agent M

| ID | Task | Depends on | Status |
| --- | --- | --- | --- |
| M0 | Build plumbing: rename `Directory.build.props` and `Directory.build.targets` to `Directory.Build.*` (one commit, announced, lesson 14), set `EnableWindowsTargeting` off Windows, bring `.github/workflows/platform.yml` from the first branch, and make `dotnet build ShareX.sln -c Release -p:Platform=x64` pass on Linux. | none | todo |
| M1 | Bring `ShareX.Platform`, `ShareX.Platform.Windows`, `ShareX.Platform.Linux`, `ShareX.Platform.MacOS` and `ShareX.Platform.Tests` from the first branch, add them to `ShareX.sln`, and make the tests pass. Bring `ShareX.ImageEffectsLib.Tests` (legacy preset tests) too. | M0 | todo |
| M2 | `HelpersLib` → `net10.0` with CA1416 as an error. Move the operating system files behind services: system information, MIME types, cursor confinement, DPAPI secrets, clipboard, shell, thumbnails, screens, hotkey host, tray icon, DWM, taskbar, desktop icons, timer resolution, Authenticode, printing (R1), input (R5). Reuse the first branch's services. Use handoffs for J's call sites. | M1 | todo |
| M3 | `ScreenCaptureLib` → `net10.0`: re-apply the design of first-branch commit `4a25a9e04` to the current code (`Screenshot` facade over `IScreenCaptureService`, snap targets, scrolling capture through `IInputService`, frame windows through `SetWindowShape`/`SetOverlayStyle`, recording devices from the platform). GDI, HDR and transparent capture live in `ShareX.Platform.Windows`. | M2 | todo |
| M4 | `Tools` → `net10.0`: OCR (`IOcrService`: WinRT on Windows, Tesseract on Linux), mouse highlighter (`HookMouse`, `CreateOverlay`), inspect and borderless window (`IWindowManagementService`), ruler capture, monitor test. Reuse the first branch's stash. | M2 | todo |
| M5 | `ImageEditor` and `ShareX.Avalonia` operating system files: `IDesktopWallpaperService`, emoji and cursor renderers, image insert, screen colour picker. | M1 | todo |
| M6 | Linux gaps: Wayland screen recording (portal ScreenCast and PipeWire, or wf-recorder/wl-screenrec on wlroots), PulseAudio/PipeWire audio for FFmpeg, global hotkeys through the portal GlobalShortcuts interface, X11 mouse hook and overlay, window icons on Wayland, region selection on X11. | M3, M4 | todo |
| M7 | macOS gaps: OCR with Vision, window management through the Accessibility API, mouse hook (CGEventTap) and overlay, recording on Apple silicon. Needs a Mac tester. | M3, M4 | todo |
| M8 | Packaging and CI: build the real application on Linux and macOS in CI once J7 lands; Linux installer for the real application; then AppImage or Flatpak; keep `docs/cross-platform.md` current. | J7 | todo |
| M9 | Run the real application end to end on Omarchy (Hyprland), GNOME and KDE: main window, tray, hotkeys, every capture mode, upload, history, editor, every tool. File bug rows. | J7 | todo |
| M10 | Decide with Jaex and McoreD whether `ShareX.Desktop` (the first branch's cross-platform host and `sharex` command line) and `ShareX.Destinations` come over, are folded into the application, or are dropped. | J7 | todo |
| M11 | Serve requests from J (request log). | requests | ongoing |
| M12 | `NativeMessagingHost` → `net10.0`: `CreateProcess` with job breakaway and browser manifest registration (R3) behind services. | M1 | todo |

## Tasks: Agent J

| ID | Task | Depends on | Status |
| --- | --- | --- | --- |
| J0 | Call `PlatformServices.Initialize(new WindowsPlatformServices())` at application start up (and `Shutdown` on exit). On other operating systems the application will choose the Linux or macOS services once it can run there (J7). | M1 | todo |
| J1 | **Verify on Windows** everything in `ShareX.Platform.Windows`, using the checklist below. This can start before M1: build the first branch (`cross-platform`) on Windows and check its Windows implementations, so problems are fixed before they are brought over. File bug rows. | none | todo |
| J2 | Switch the application's hotkeys from `WindowsHotkeyHost` to `IPlatformServices.Hotkeys` (`WindowsHotkeyService`). | M1, J0 | todo |
| J3 | Tray: move `TrayIconService` off `WindowsTrayIcon` to Avalonia `TrayIcon` with `DesktopServices.RegisterTrayIcon`, if not already done. Retire `Forms/MainForm.cs` if anything is left in it. | J2 | todo |
| J4 | Replace the Win32 calls in the application's own files: `CaptureHelpers/CaptureBase.cs`, `CaptureHelpers/CaptureCustomWindow.cs`, `CaptureHelpers/CaptureWindow.cs`, `Presentation/MainWindow/ThumbnailItemViewModel.cs`, `ScreenRecordManager.cs`, `TaskHelpers.cs` (window info via `IWindowService` and `IWindowManagementService`). File requests for anything missing. | M1, J0 | todo |
| J5 | Replace the registry in `StartupManager.cs`, `SystemOptions.cs` and `IntegrationHelpers.cs` with `IStartupService`, `IShellIntegrationService` and the settings files (requests R3, R4). | M1, requests | todo |
| J6 | Retarget `HistoryLib`, `UploadersLib` and `ImageEffectsLib` to `net10.0` with CA1416 as an error once `HelpersLib` is `net10.0`. | M2 | todo |
| J7 | Retarget the `ShareX` application to `net10.0` plus `net10.0-windows10.0.22621.0` (Windows release, for WinRT features). Choose the platform services by operating system at start up. | J2 to J6, M2 to M4 | todo |
| J8 | Switch call sites listed in the handoff log as they appear. | handoffs | ongoing |

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

## Requests (J → M)

J adds rows; M fills in the interface and status.

| ID | Need | Call sites | Interface | Status |
| --- | --- | --- | --- | --- |
| R1 | Printing: print dialog, preview, print an image or text | `HelpersLib/Printer`, print windows | | todo |
| R2 | Taskbar progress and overlay | application upload progress | | todo |
| R3 | Browser native-messaging host registration (Chrome, Firefox) | `IntegrationHelpers`, `NativeMessagingHost` | | todo |
| R4 | Shortcut (`.lnk`) creation | application integration settings | probably `IStartupService`/`IShellIntegrationService` | todo |
| R5 | Synthetic input beyond scrolling: typing text, key combinations | application actions | extend `IInputService` | todo |

## Handoffs (M → J)

M adds a row when an API moved behind a service and J's files still call the old wrapper.

| ID | Old API (wrapper kept) | New API | Call sites in J's files | Status |
| --- | --- | --- | --- | --- |

## Bugs

| ID | Found by | Platform | Description | Owner | Status |
| --- | --- | --- | --- | --- | --- |

## Status log

One line per working session, newest at the bottom.

- 2026-10-03, M: Created `cross-platform-v2` from `develop` `fd61635f2`. Added AGENTS.md, this tracker, `docs/cross-platform-learnings.md` and `docs/cross-platform.md`. No code changes yet.
