# AGENTS.md — ShareX

Rules for people and coding agents working in this repository.

The `cross-platform-v2` branch turns ShareX from a Windows application into one codebase that runs on Windows, macOS and Linux. Before changing anything, read:

- [docs/cross-platform-learnings.md](docs/cross-platform-learnings.md): what the first attempt taught, and the rules that follow from it.
- [docs/cross-platform-delegation.md](docs/cross-platform-delegation.md): which agent owns which project, the task tracker and the request log.
- [docs/cross-platform.md](docs/cross-platform.md): the target architecture and how to build.

## Start of every session

The other agent pushes to the same branch, and a running session does not see its work. At the start of every session (and again before starting a new task):

1. `git pull` on `cross-platform-v2`.
2. Read the newest entries of the **Status log** at the bottom of [docs/cross-platform-delegation.md](docs/cross-platform-delegation.md), and the **Handoffs**, **Requests** and **Bugs** rows addressed to you. Deal with those before starting new work.
3. Check the task tables for what is `in progress` (yours to finish) and what you can claim next.
4. Claim a task (set it to `in progress` and push) before writing code.

## Branches

- `cross-platform-v2`: all cross-platform work. Both agents commit here.
- `develop`: v22 release line, hotfixes only. Every hotfix is merged into `cross-platform-v2` the same day.
- `cross-platform`: the first attempt, frozen for reference. Do not merge it; bring pieces over by path as described in the learnings.

**Never force push** (`git push --force`, `--force-with-lease`, `+branch`) to any shared branch, and never rewrite pushed history (no amending, rebasing or resetting commits that are already on GitHub). The other agent builds on those commits. If a push is rejected, `git pull --rebase`, rebuild, and push again. To undo a pushed commit, add a `git revert` commit.

## Platform abstraction rules

### Where code lives

| Project | Holds | Must not hold |
| --- | --- | --- |
| `ShareX.Platform` | Interfaces (`I…Service`), shared models (`PlatformWindow`, `ScreenInfo`, `PlatformRectangle`, `PixelBuffer`, …), `PlatformServices`, platform detection, `FeatureSupport`, Linux package install hints, pure helpers such as `PngCodec`. | Any call into the operating system: P/Invoke, registry, D-Bus, process launches of OS tools, OS-specific types. |
| `ShareX.Platform.Windows` | Every piece of Windows-only code: Win32 and COM P/Invoke, registry, DPAPI, GDI capture, DWM, shell and Explorer integration, WinRT (OCR), Windows-only features such as HDR and transparent window capture. | Public types other than the service implementations and the models they need. Keep the Win32 declarations `internal`. |
| `ShareX.Platform.Linux` | X11, Wayland (xdg-desktop-portal, wlroots tools such as grim and slurp, Hyprland and sway IPC), D-Bus, freedesktop.org specifications (XDG directories, autostart, desktop entries, thumbnails, Secret Service). | Code that other platforms need. |
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
3. Implement it in **every** platform project. Where a platform cannot do it, return `FeatureSupport.NotSupported(reason)` with a user-facing reason; on Linux use `LinuxPackages` so the reason names the package and the install command for the user's distribution. `UnsupportedServices.cs` holds reusable "not available" implementations.
4. In shared code, read `Support` before offering the feature, and hide or disable the UI with the reason as its tooltip rather than failing at run time.
5. Keep Windows behaviour identical to what it was. The Windows implementation is usually the code that used to live in the shared project, moved behind the interface by Jaex's agent, and it is not done until it has run on Windows.
6. Add tests in `ShareX.Platform.Tests` for parsing and argument building so they run on every OS.

### Enforcement

- Shared projects set `<WarningsAsErrors>$(WarningsAsErrors);CA1416</WarningsAsErrors>` once they target `net10.0`. Never suppress CA1416 with `#pragma`, `[SuppressMessage]` or `NoWarn` in shared code; move the code behind a platform service instead.
- `dotnet build ShareX.sln -c Release -p:Platform=x64` must pass on Windows, Linux and macOS, and every test project must pass, before each push.
- The Windows application must keep opening and running from Visual Studio on Windows without extra steps.

### Migration debt on `cross-platform-v2` (remove, do not add to)

State at the branch point (`develop` `fd61635f2`). WinForms and GDI+ are already gone. Update this list as items are done.

- [x] Build on Linux and macOS: rename `Directory.build.props` and `Directory.build.targets` to `Directory.Build.*`, set `EnableWindowsTargeting` on non-Windows hosts, add the cross-OS CI workflow.
- [x] `ShareX.Platform*` projects and tests on this branch (brought from `cross-platform`).
- [ ] `PlatformServices.Initialize` at application start up.
- [ ] `net10.0-windows` targets: `HelpersLib`, `HistoryLib`, `UploadersLib`, `ImageEffectsLib`, `ScreenCaptureLib`, `NativeMessagingHost`, `Tools` and `ShareX` (`net10.0-windows10.0.22621.0`).
- [ ] Win32 interop in `HelpersLib` (`Native/`, P/Invoke in 7 files, registry in 4, `OperatingSystem.Is…` in 8) → `ShareX.Platform.Windows` behind services.
- [ ] Win32 interop in `ScreenCaptureLib` (GDI and HDR capture, transparent window capture, window lists, scrolling input, frame window regions) → services.
- [ ] Win32 interop and WinRT in `Tools` (OCR, mouse highlighter, inspect and borderless window, ruler, clipboard viewer) → services.
- [ ] Win32 calls and registry in the `ShareX` application (capture helpers, window menu, task metadata, notification and upload windows, startup and shell integration).
- [ ] `OperatingSystem.Is…()` and P/Invoke in `ImageEditor` (desktop wallpaper, emoji and cursor renderers, `EditorServices`, image insert) and `ShareX.Avalonia` (screen colour picker, cursor assets).
- [ ] `NativeMessagingHost`: `CreateProcess` and browser manifest registration → services.

## Other rules

- Never send real user data (screenshots, files, credentials) to third-party upload services in tests or manual checks. Test uploads against a local HTTP server.
- Uploading is opt-in. Defaults must not name or contact a third-party host.
- Secrets in settings are encrypted through `ISecretProtectionService`. Never fall back to writing them in plain text.
- ShareX and XerahS are different applications; never install one over the other. Do not change a user's desktop or compositor configuration without asking.
- Check that code is used before porting it.
