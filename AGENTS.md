# AGENTS.md — ShareX

Rules for people and coding agents working in this repository.

The `cross-platform` branch makes ShareX run on Windows, Linux and macOS. Windows, Linux and macOS are complete. McoreD and Jaex agree on the goal: all three systems at 100%, with features such as OCR implemented through each system's own support.

Useful background: [docs/cross-platform.md](docs/cross-platform.md) (architecture, building, per-desktop notes), [docs/cross-platform-learnings.md](docs/cross-platform-learnings.md), [docs/cross-platform-roadmap.md](docs/cross-platform-roadmap.md) (progress and open work), [docs/20261004-macos-test-procedure.md](docs/20261004-macos-test-procedure.md).

## Scope

- **Windows** behaves as in v22.
- **Linux** (X11, Hyprland, sway, GNOME, KDE) and **macOS** (13 or later, Apple silicon and Intel) get every feature their system can support. Linux HDR is not supported.
- **OCR** uses ShareX's own PaddleOCR engine (ShareX.OCR, ONNX Runtime) everywhere it runs. Intel Macs, which ONNX Runtime has no build for, use the Vision framework.
- **Dependencies.** ShareX runs on .NET with the libraries it ships (Avalonia, SkiaSharp, ONNX Runtime, …). Core flows (start, tray, hotkeys, capture, clipboard, editor, upload, history) use only those and the system's own APIs. Optional features may use programs the system provides, detected at run time (FFmpeg for recording and video tools, CUPS for printing, grim on wlroots desktops). Nothing extra is bundled or installed, and messages never contain install commands.
- Where a system cannot do something, the service returns `FeatureSupport.NotSupported` with a short user-facing reason, and shared UI and task execution respect it.

## Git

- Work on `cross-platform` (named `cross-platform-v2` until 2026-11-04; the first attempt that used this name is gone). `develop` is the release line (v22.x, the Photo Editor, the native Windows recorder); merge it into `cross-platform` whenever it moves, porting new Windows-only code behind `ShareX.Platform`.
- Pull before starting and rebase before pushing. **Never force push** and never rewrite pushed history; undo with `git revert`.
- When a merge or rebase conflicts in this file or the docs, combine both sides; do not throw away the other side's text.

## Where code lives

| Project | Holds | Must not hold |
| --- | --- | --- |
| `ShareX.Platform` | Interfaces (`I…Service`), shared models (`PlatformWindow`, `ScreenInfo`, `PlatformRectangle`, `PixelBuffer`, …), `PlatformServices`, platform detection, `FeatureSupport`, pure helpers such as `PngCodec`. | Any call into the operating system: P/Invoke, registry, D-Bus, process launches of OS tools, OS-specific types. |
| `ShareX.Platform.Windows` | Windows-only code: Win32 and COM P/Invoke, registry, DPAPI, GDI capture, DWM, shell and Explorer integration, WinRT (OCR), HDR and transparent window capture. | Public types other than the service implementations and the models they need. Keep Win32 declarations `internal`. |
| `ShareX.Platform.Linux` | X11, Wayland portals and compositor IPC, D-Bus, freedesktop.org specifications, and optional distribution programs detected at run time. Per-desktop implementations live under `Desktop/`, chosen by `LinuxDesktop`. | Code other platforms need. |
| `ShareX.Platform.MacOS` | AppKit, CoreGraphics, Carbon, Vision, ImageIO, Keychain, LaunchAgents, QuickLook. | Code other platforms need. |
| Everything else (`ShareX`, `HelpersLib`, `UploadersLib`, `HistoryLib`, `ScreenCaptureLib`, `ImageEffectsLib`, `Tools`, `ImageEditor`, `ShareX.Avalonia`, `NativeMessagingHost`) | Shared code that targets plain `net10.0` and reaches the operating system only through `PlatformServices.Current`. | Anything in the list below. |

Exceptions: `ShareX.Setup` (Windows installer) and `ShareX.Steam` (`net48` launcher) are Windows-only. The `ShareX` application also builds `net10.0-windows10.0.22621.0` on Windows so the release gets WinRT features from `ShareX.Platform.Windows`; it contains no Windows code itself. The `MicrosoftStore` build flavour keeps its StartupTask code under that build flag.

### Forbidden in shared code

- `DllImport` / `LibraryImport`, COM interop, `Microsoft.Win32.Registry`, native handles used as anything other than an opaque id.
- WinForms and WPF. The UI is Avalonia.
- GDI+ at run time (`System.Drawing.Bitmap`, `Graphics`, `Font`, `Icon`, `Image.Save`). Images are SkiaSharp or encoded bytes. `System.Drawing` value types are fine.
- OS conditional compilation (`#if WINDOWS`) and per-OS target frameworks.
- `OperatingSystem.Is…()` branches with operating system logic, except choosing the `IPlatformServices` implementation at start up. Ask a platform service instead.
- Running OS tools directly. Wrap them in a platform service using `ICommandRunner` so they can be tested.
- Hard-coded paths, separators or file name rules. Use `IPathService` and `Path`.

### Adding behaviour that differs by operating system

1. Use the service in `ShareX.Platform/Services` that owns the area, or add an `I…Service` on `IPlatformServices`, with implementations (or reasoned `NotSupported` stubs) on all three platforms in the same change.
2. Model results with portable types; never leak `IntPtr` meaning, Win32 enums, X11 atoms or Cocoa objects.
3. In shared code, read `Support` before offering a feature and disable the UI with the reason as its tooltip.
4. Keep Windows behaviour identical to v22.
5. Add tests in the test projects. OS-specific tests use the `WindowsFact`, `LinuxFact` and `MacOSFact` attributes so they skip elsewhere.

## How to work

Evidence first, adapted from Cursor's pstack principles (github.com/cursor/plugins, MIT):

- **Find the root cause before fixing.** Reproduce the defect, explain why it happens, then fix that. A guard that hides a symptom is not a fix.
- **Prove it on the real artifact.** Build, run the tests, and for native code exercise the real library or window (a probe or an opt-in test), not just the compiler. Say what was checked and what was not.
- **Regression test first.** Every bug fixed gets a test that fails without the fix.
- **Test behaviour, not implementation.** Assert what users and callers see; do not pin private structure.
- **Small verifiable steps.** Each commit builds and passes on its own; land a platform contract with its stubs, then each implementation.
- **Migrate callers, then delete.** When code moves behind `ShareX.Platform`, switch every caller and remove the old path in the same change; no parallel copies.
- **Encode lessons in structure.** A mistake found once becomes a test, an analyzer setting or a rule here, and a line in `docs/cross-platform-learnings.md`.

## Checks before pushing

- `dotnet build ShareX.sln -c Release -p:Platform=x64` passes and every test project passes.
- Shared projects treat CA1416 as an error. Never suppress it with `#pragma`, `[SuppressMessage]` or `NoWarn` in shared code.
- The `Cross-platform` workflow builds and tests on Windows, Linux and macOS for every push and packages and launches `ShareX.app`; keep it green.
- The Windows application keeps opening and running from Visual Studio without extra steps.

## Other rules

- Never send real user data (screenshots, files, credentials) to third-party upload services in tests or manual checks. Test uploads against a local HTTP server.
- Uploading is opt-in. Defaults must not name or contact a third-party host.
- Secrets in settings are encrypted through `ISecretProtectionService`; never fall back to plain text.
- ShareX and XerahS are different applications; never install one over the other. Do not change a user's desktop or compositor configuration without asking.
- Check that code is used before porting it.
