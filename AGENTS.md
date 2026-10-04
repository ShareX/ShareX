# AGENTS.md — ShareX

Rules for people and coding agents working in this repository.

The `cross-platform-v2` branch ports ShareX to Linux and macOS while preserving Windows support. Windows and Linux are complete (owner decision, 2026-10-04); the macOS phase is open. Before changing anything, read:

- [docs/cross-platform-learnings.md](docs/cross-platform-learnings.md): what the first attempt taught, and the rules that follow from it.
- [docs/cross-platform-delegation.md](docs/cross-platform-delegation.md): which agent owns which project, the task tracker and the request log.
- [docs/cross-platform.md](docs/cross-platform.md): the target architecture and how to build.
- [docs/cross-platform-roadmap.md](docs/cross-platform-roadmap.md): progress per operating system, the roadmap to 100%, the gates between phases and the commitments McoreD, Jaex and their agents have made.

## Platform priorities: macOS phase

Windows and Linux are complete (McoreD, 2026-10-04). **The macOS phase is open**: port the full `ShareX` application to macOS (Apple silicon and Intel, macOS 13 or later), including its main window, menu bar icon, hotkeys, capture, recording, upload, history, editor and tools.

- **Keep Windows and Linux working.** Every change must keep their behaviour; Windows stays identical to v22. Field testing on GNOME, KDE, sway, X11 and Windows 10/11 continues, and its bug rows come before new macOS work.
- **Ownership.** M (McoreD's agent) owns `ShareX.Platform.MacOS`, macOS packaging and CI, and shared contracts; J (Jaex's agent) owns the frontend, including macOS UI adaptation (menu bar, keyboard shortcuts with Command, window chrome). No one here has a Mac: macOS code is built and unit tested on the `macos-latest` CI runner, and McoreD, Jaex and users verify it on real Macs and file bug rows.
- **macOS dependencies** follow the same principle as Linux: system frameworks and tools (AppKit, CoreGraphics, ScreenCaptureKit, Vision, QuickLook, `screencapture`, `afplay`) are used directly; optional programs such as FFmpeg are detected at run time; nothing is bundled or installed for parity. Permissions macOS asks for (Screen Recording, Accessibility, Input Monitoring) are requested at the moment a feature needs them, and the feature reports a reason until they are granted.
- Linux HDR support stays excluded.

## Linux dependencies and OCR: owner decision (final)

**Decided by McoreD (project owner), 2026-10-04. Final.** Agents follow this section as written and do not edit, weaken or revert it, or file requests against it; only McoreD changes it. Concerns go in the status log for McoreD.

1. **Core guarantee.** With only Microsoft .NET on a standard desktop session, ShareX starts and its core flows work: main window, tray, hotkeys where the desktop allows them, screenshot capture, clipboard, editor and tools, upload and history. Core flows use .NET, libraries ShareX already ships (Avalonia, SkiaSharp, ONNX Runtime, …) and the desktop's own APIs (X11, xdg-desktop-portal, PipeWire, D-Bus, compositor IPC, freedesktop.org specifications). A program the desktop's own portal runs counts as part of that desktop: Hyprland's and sway's screenshot portals run grim.
2. **Optional features use the operating system's own support.** A feature with no desktop-API path may use a program or library the distribution provides, detected at run time: FFmpeg for recording and the video tools, Tesseract and its installed language data for OCR, CUPS for printing, sound players. This mirrors Windows, where recording uses an FFmpeg ShareX does not ship.
3. **OCR is implemented on Linux** through the distribution's Tesseract (`TesseractOcrService`). Windows keeps Windows.Media.Ocr in `ShareX.Platform.Windows`; macOS uses Apple's Vision framework (McoreD, 2026-10-04).
4. **Never bundle, never install, never nag.** Do not bundle third-party engines, models, language data or tools to reach parity, never install anything or run package managers, and put no install commands in messages. When a part is missing, the feature reports `FeatureSupport.NotSupported` with a short reason that may name the missing package, and shared UI, hotkeys and CLI/task execution respect it.

These rules do not change Windows behaviour or dependencies.

## Start of every session

The other agent pushes to the same branch, and a running session does not see its work. At the start of every session (and again before starting a new task):

1. `git pull` on `cross-platform-v2`.
2. Read the newest entries of the **Status log** at the bottom of [docs/cross-platform-delegation.md](docs/cross-platform-delegation.md), and the **Handoffs**, **Requests** and **Bugs** rows addressed to you. Deal with those before starting new work.
3. Read [docs/cross-platform-roadmap.md](docs/cross-platform-roadmap.md): the gates, the open commitments addressed to you, and the progress estimates.
4. Check the task tables for what is `in progress` (yours to finish) and what you can claim next.
5. Claim a task (set it to `in progress` and push) before writing code.
6. When your work changes a percentage, ticks a gate or verification item, or makes or keeps a commitment, update the roadmap in the same push and add a line to its change log.

## Branches

- `cross-platform-v2`: all cross-platform work. Both agents commit here.
- `develop`: v22 release line, hotfixes only. Every hotfix is merged into `cross-platform-v2` the same day.
- `cross-platform`: the first attempt, frozen for reference. Do not merge it; bring pieces over by path as described in the learnings.

**Never force push** (`git push --force`, `--force-with-lease`, `+branch`) to any shared branch, and never rewrite pushed history (no amending, rebasing or resetting commits that are already on GitHub). The other agent builds on those commits. If a push is rejected, `git pull --rebase`, rebuild, and push again. To undo a pushed commit, add a `git revert` commit.

## Platform abstraction rules

### Where code lives

| Project | Holds | Must not hold |
| --- | --- | --- |
| `ShareX.Platform` | Interfaces (`I…Service`), shared models (`PlatformWindow`, `ScreenInfo`, `PlatformRectangle`, `PixelBuffer`, …), `PlatformServices`, platform detection, `FeatureSupport`, pure helpers such as `PngCodec`. | Any call into the operating system: P/Invoke, registry, D-Bus, process launches of OS tools, OS-specific types. |
| `ShareX.Platform.Windows` | Every piece of Windows-only code: Win32 and COM P/Invoke, registry, DPAPI, GDI capture, DWM, shell and Explorer integration, WinRT (OCR), Windows-only features such as HDR and transparent window capture. | Public types other than the service implementations and the models they need. Keep the Win32 declarations `internal`. |
| `ShareX.Platform.Linux` | Existing Linux desktop APIs: X11, Wayland portals and compositor IPC, D-Bus, freedesktop.org specifications (XDG directories, autostart, desktop entries, thumbnails, Secret Service), and optional distribution programs detected at run time (FFmpeg, Tesseract, CUPS), under the owner decision above. Per-desktop implementations live under `Desktop/`, chosen by `LinuxDesktop`. | Code that other platforms need; bundled third-party engines, tools or data; a core flow that only works with an optional program. |
| `ShareX.Platform.MacOS` | AppKit, CoreGraphics, Carbon, Vision, Keychain, LaunchAgents. | Same as above. |
| Everything else (`ShareX`, `HelpersLib`, `UploadersLib`, `HistoryLib`, `ScreenCaptureLib`, `ImageEffectsLib`, `Tools`, `ImageEditor`, `ShareX.Avalonia`, `NativeMessagingHost`) | Shared application code that targets plain `net10.0` and talks to the operating system only through `PlatformServices.Current`. | Anything in the "forbidden in shared code" list below. |

Exceptions: `ShareX.Setup` (Windows installer) and `ShareX.Steam` (`net48` Steam launcher) are Windows-only by design. The `ShareX` application may also build a `net10.0-windows10.0.22621.0` target, so the Windows release gets WinRT features from `ShareX.Platform.Windows`; it contains no Windows code itself.

There is no `ShareX.HelpersLib.Windows`. Windows-only code goes into `ShareX.Platform.Windows` behind a service.

### Forbidden in shared code

- `DllImport` / `LibraryImport`, COM interop, `Microsoft.Win32.Registry`, Win32 handles used as anything other than an opaque id.
- `System.Windows.Forms` and WPF. The UI is Avalonia.
- GDI+ at run time (`System.Drawing.Bitmap`, `Graphics`, `Font`, `Icon`, `Image.Save`). Images are SkiaSharp (`SKBitmap`) or encoded bytes (PNG). `System.Drawing` value types (`Point`, `Size`, `Rectangle`, `Color`) are fine; they are portable.
- `#if WINDOWS` (or any OS conditional compilation) and per-OS target frameworks such as `net10.0-windows`.
- `OperatingSystem.IsWindows()` / `IsLinux()` / `IsMacOS()` branches that contain operating system logic. Ask the platform service instead. The only accepted use is choosing which `IPlatformServices` implementation to create at start up.
- Running OS tools (`hyprctl`, `xdg-open`, `reg`, `powershell`, …) directly. Wrap them in a platform service, using `ICommandRunner` so they can be tested.
- Hard-coded paths, path separators or file name rules. Use `IPathService` and `Path`.

### Adding behaviour that differs by operating system

1. Find the service in `ShareX.Platform/Services` that owns the area, or add a new `I…Service` and expose it on `IPlatformServices`. Contracts in `ShareX.Platform` are changed only by their owner (McoreD's agent, see the delegation file), who adds "not supported" stubs to every platform project in the same commit; others file a request. `ShareX.Platform.Windows` is written by Jaex's agent (the Windows lead): McoreD's agent adds a W row to the delegation file instead of writing Windows code, and switches shared code only after the Windows implementation is done.
2. Model the result with portable types. Never leak `IntPtr` meaning, Win32 enums, X11 atoms or Cocoa objects through the interface.
3. Implement it on **Windows, Linux and macOS**, except features excluded by the scope rule above. Where a platform cannot do it, return `FeatureSupport.NotSupported(reason)` with a user-facing reason. Linux implementations follow the owner decision above: desktop APIs first, optional distribution programs detected at run time, nothing bundled or installed, no install commands in reasons. `UnsupportedServices.cs` holds reusable "not available" implementations.
4. In shared code, read `Support` before offering the feature, and hide or disable the UI with the reason as its tooltip rather than failing at run time.
5. Keep Windows behaviour identical to what it was. The Windows implementation is usually the code that used to live in the shared project, moved behind the interface by Jaex's agent, and it is not done until it has run on Windows.
6. Add tests in `ShareX.Platform.Tests` for parsing and argument building so they run on every OS.

### Enforcement

- Shared projects set `<WarningsAsErrors>$(WarningsAsErrors);CA1416</WarningsAsErrors>` once they target `net10.0`. Never suppress CA1416 with `#pragma`, `[SuppressMessage]` or `NoWarn` in shared code; move the code behind a platform service instead.
- `dotnet build ShareX.sln -c Release -p:Platform=x64` must pass on Windows and Linux, and every test project must pass, before each push. The `Cross-platform` workflow builds and tests on Windows, Linux and macOS for every push to `cross-platform-v2`; keep it green.
- The Windows application must keep opening and running from Visual Studio on Windows without extra steps.

### Migration debt on `cross-platform-v2` (remove, do not add to)

State at the branch point (`develop` `fd61635f2`). WinForms and GDI+ are already gone. Update this list as items are done.

- [x] Build on Linux and macOS: rename `Directory.build.props` and `Directory.build.targets` to `Directory.Build.*`, set `EnableWindowsTargeting` on non-Windows hosts, add the cross-OS CI workflow.
- [x] `ShareX.Platform*` projects and tests on this branch (brought from `cross-platform`).
- [x] `PlatformServices.Initialize` at application start up (`Program.Main`, `PlatformBootstrap.CreateForCurrentOS`).
- [x] `net10.0-windows` targets: every shared project targets plain `net10.0`; only the `ShareX` application adds `net10.0-windows10.0.22621.0` when built on Windows, as allowed above.
- [x] Remaining browser process-launch declarations in `HelpersLib` → `ShareX.Platform.Windows` behind services. Tray/session and browser launch now use portable services; obsolete shared native declarations and duplicate shortcut COM code are removed.
- [x] Win32 interop in `ScreenCaptureLib` (GDI and HDR capture, transparent window capture, window lists, scrolling input, frame window regions) → services. No P/Invoke, registry or COM remains in shared projects (audit 2026-10-04).
- [x] Win32 interop and WinRT in `Tools` (OCR, mouse highlighter, inspect and borderless window, ruler, clipboard viewer) → services.
- [x] Win32 calls and registry in the `ShareX` application (capture helpers, window menu, task metadata, notification and upload windows, startup and shell integration). The Microsoft Store build flavour keeps its packaged StartupTask and activation check under the `MicrosoftStore` build flag, which only Windows Store builds set; moving it into `WindowsStartupService` through `IStartupService.WasStartedBySignIn` (W12/H12) is optional clean-up, not a blocker.
- [x] `OperatingSystem.Is…()` and P/Invoke in `ImageEditor` and `ShareX.Avalonia`: wallpaper, native emoji/cursors and colour sampling use platform services; image insert and cursor assets use portable imaging.
- [x] `NativeMessagingHost`: Windows `CreateProcess` and browser manifest registration → services. The host initializes its platform and passes separate arguments; Linux launches through posix_spawn in a new session (R28).

## Other rules

- Never send real user data (screenshots, files, credentials) to third-party upload services in tests or manual checks. Test uploads against a local HTTP server.
- Uploading is opt-in. Defaults must not name or contact a third-party host.
- Secrets in settings are encrypted through `ISecretProtectionService`. Never fall back to writing them in plain text.
- ShareX and XerahS are different applications; never install one over the other. Do not change a user's desktop or compositor configuration without asking.
- Check that code is used before porting it.
