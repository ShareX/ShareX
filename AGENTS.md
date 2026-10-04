# AGENTS.md — ShareX

Rules for people and coding agents working in this repository.

The `cross-platform-v2` branch ports ShareX to Linux while preserving Windows support. macOS is a possible later phase. Before changing anything, read:

- [docs/cross-platform-learnings.md](docs/cross-platform-learnings.md): what the first attempt taught, and the rules that follow from it.
- [docs/cross-platform-delegation.md](docs/cross-platform-delegation.md): which agent owns which project, the task tracker and the request log.
- [docs/cross-platform.md](docs/cross-platform.md): the target architecture and how to build.
- [docs/cross-platform-roadmap.md](docs/cross-platform-roadmap.md): progress per operating system, the roadmap to 100%, the gates between phases and the commitments McoreD, Jaex and their agents have made.

## Platform priorities: Linux first

All agents must focus on **Windows and Linux only** during the current port. Prioritize porting the full **`ShareX` application project** to Linux, including its main window, tray, hotkeys, capture, upload, history, editor and tools. Keep Windows behaviour identical to v22 throughout the migration.

**Do not work on the macOS port now.** macOS implementation, UI adaptation, desktop verification, packaging and CI work are deferred. Only after the Linux port is **100% complete for the agreed scope and stable enough for normal use**, with real application flows verified on the supported Linux desktops and Windows regressions checked, may a macOS port be considered. A successful build or a working standalone editor alone does not meet this milestone. Linux HDR and OCR support are excluded from the agreed scope.

**OCR remains Windows-only.** Preserve the Windows.Media.Ocr implementation behind `IOcrService`. Do not implement or expose OCR on Linux or macOS, or add OCR package requirements there. Non-Windows OCR services must report `FeatureSupport.NotSupported` with a user-facing reason, and shared UI and task execution must respect that support value. This also applies to OCR backends carried over from the first cross-platform attempt.

**Exception (McoreD, 2026-10-04):** McoreD directed M to complete the macOS implementation (M10) on `cross-platform-v2` ahead of G1. Jaex has not yet approved the macOS phase. It does not change the Linux-first priority for other work, the OCR rule below or the Windows ownership rule.

Existing macOS code may remain. When a shared contract changes, add only the minimum unsupported stubs or mechanical updates needed to keep the solution compiling; this does not authorize macOS feature work. This priority rule takes precedence over older three-platform task wording and target architecture descriptions.

## Linux dependencies: no extra installations

**Microsoft .NET is the only additional runtime users may be required to install for the Linux port.** Libraries already included with ShareX, such as Avalonia and SkiaSharp, are allowed. Use those libraries and APIs provided by the existing Linux desktop; do not require users to install additional packages, command-line tools, native libraries, engines, fonts or services to enable a ShareX feature.

**Do not port a feature to Linux if it requires such an external dependency.** OCR is an explicit example and remains Windows-only. These features are excluded from the agreed Linux scope. Report `FeatureSupport.NotSupported` with a clear user-facing reason, and respect it in both shared UI and task execution. Do not offer package-install commands, install dependencies automatically, or add a new bundled dependency to work around this rule.

These rules apply to optional features too. Detecting a separately installed program at run time does not authorize an excluded Linux feature; OCR remains Windows-only even if Tesseract is present.

Audit implementations carried over from the first cross-platform attempt against this rule. Use an implementation based on already included libraries or existing desktop APIs where possible; otherwise retire the dependency-based Linux implementation and report the feature as unsupported. An optional helper must not become an installation requirement. This rule supersedes earlier Linux package recommendations and does not change Windows behaviour or dependencies.

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
| `ShareX.Platform.Linux` | Existing Linux desktop APIs: X11, Wayland portals and compositor IPC, D-Bus, freedesktop.org specifications (XDG directories, autostart, desktop entries, thumbnails, Secret Service), subject to the no-extra-installations rule above. Per-desktop implementations live under `Desktop/`, chosen by `LinuxDesktop`. | Code that other platforms need; feature implementations requiring additional external dependencies. |
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
3. Implement it on **Windows and Linux** during the current phase, except features excluded by the scope or dependency rules above; keep macOS compatible with shared contracts using unsupported stubs only. Where a platform cannot do it, return `FeatureSupport.NotSupported(reason)` with a user-facing reason. Linux implementations must use already included libraries or existing desktop APIs without requiring extra installations; do not use `LinuxPackages` to recommend installing feature dependencies. `UnsupportedServices.cs` holds reusable "not available" implementations.
4. In shared code, read `Support` before offering the feature, and hide or disable the UI with the reason as its tooltip rather than failing at run time.
5. Keep Windows behaviour identical to what it was. The Windows implementation is usually the code that used to live in the shared project, moved behind the interface by Jaex's agent, and it is not done until it has run on Windows.
6. Add tests in `ShareX.Platform.Tests` for parsing and argument building so they run on every OS.

### Enforcement

- Shared projects set `<WarningsAsErrors>$(WarningsAsErrors);CA1416</WarningsAsErrors>` once they target `net10.0`. Never suppress CA1416 with `#pragma`, `[SuppressMessage]` or `NoWarn` in shared code; move the code behind a platform service instead.
- `dotnet build ShareX.sln -c Release -p:Platform=x64` must pass on Windows and Linux, and every test project must pass, before each push. macOS build and desktop verification are deferred with the macOS port.
- The Windows application must keep opening and running from Visual Studio on Windows without extra steps.

### Migration debt on `cross-platform-v2` (remove, do not add to)

State at the branch point (`develop` `fd61635f2`). WinForms and GDI+ are already gone. Update this list as items are done.

- [x] Build on Linux and macOS: rename `Directory.build.props` and `Directory.build.targets` to `Directory.Build.*`, set `EnableWindowsTargeting` on non-Windows hosts, add the cross-OS CI workflow.
- [x] `ShareX.Platform*` projects and tests on this branch (brought from `cross-platform`).
- [ ] `PlatformServices.Initialize` at application start up.
- [ ] `net10.0-windows` targets: `HelpersLib`, `HistoryLib`, `UploadersLib`, `ImageEffectsLib`, `ScreenCaptureLib`, `NativeMessagingHost`, `Tools` and `ShareX` (`net10.0-windows10.0.22621.0`).
- [x] Remaining browser process-launch declarations in `HelpersLib` → `ShareX.Platform.Windows` behind services. Tray/session and browser launch now use portable services; obsolete shared native declarations and duplicate shortcut COM code are removed.
- [ ] Win32 interop in `ScreenCaptureLib` (GDI and HDR capture, transparent window capture, window lists, scrolling input, frame window regions) → services.
- [ ] Win32 interop and WinRT in `Tools` (OCR, mouse highlighter, inspect and borderless window, ruler, clipboard viewer) → services.
- [ ] Win32 calls and registry in the `ShareX` application (capture helpers, window menu, task metadata, notification and upload windows, startup and shell integration).
- [x] `OperatingSystem.Is…()` and P/Invoke in `ImageEditor` and `ShareX.Avalonia`: wallpaper, native emoji/cursors and colour sampling use platform services; image insert and cursor assets use portable imaging.
- [x] `NativeMessagingHost`: Windows `CreateProcess` and browser manifest registration → services. The host initializes its platform and passes separate arguments; Linux launches through posix_spawn in a new session (R28).

## Other rules

- Never send real user data (screenshots, files, credentials) to third-party upload services in tests or manual checks. Test uploads against a local HTTP server.
- Uploading is opt-in. Defaults must not name or contact a third-party host.
- Secrets in settings are encrypted through `ISecretProtectionService`. Never fall back to writing them in plain text.
- ShareX and XerahS are different applications; never install one over the other. Do not change a user's desktop or compositor configuration without asking.
- Check that code is used before porting it.