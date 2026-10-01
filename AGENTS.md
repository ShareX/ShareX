# AGENTS.md — ShareX

Rules for people and coding agents working in this repository. The `cross-platform` branch is moving ShareX from a Windows (WinForms) application to one codebase that runs on Windows, macOS and Linux. Read [docs/cross-platform.md](docs/cross-platform.md) for the architecture and the current status.

## Platform abstraction rules

### Where code lives

| Project | Holds | Must not hold |
| --- | --- | --- |
| `ShareX.Platform` | Interfaces (`I…Service`), shared models (`PlatformWindow`, `ScreenInfo`, `PlatformRectangle`, …), `PlatformServices`, platform detection, `FeatureSupport`, Linux package install hints, pure helpers such as `PngCodec`. | Any call into the operating system: P/Invoke, registry, D-Bus, process launches of OS tools, OS-specific types. |
| `ShareX.Platform.Windows` | Every piece of Windows-only code: Win32 and COM P/Invoke, registry, DPAPI, GDI capture, DWM, shell and Explorer integration, Windows-only features such as HDR and transparent window capture. | Public types other than the service implementations and the models they need. Keep the Win32 declarations `internal`. |
| `ShareX.Platform.Linux` | X11, Wayland (xdg-desktop-portal, wlroots tools such as grim and slurp, Hyprland and sway IPC), D-Bus, freedesktop.org specifications (XDG directories, autostart, desktop entries, thumbnails, Secret Service). | Code that other platforms need. |
| `ShareX.Platform.MacOS` | AppKit, CoreGraphics, Carbon, Keychain, LaunchAgents. | Same as above. |
| Everything else (`ShareX`, `ShareX.Desktop`, `HelpersLib`, `UploadersLib`, `HistoryLib`, `Destinations`, `ScreenCaptureLib`, `ImageEffectsLib`, `Tools`, `ImageEditor`, `ShareX.Avalonia`) | Shared application code that targets plain `net10.0` and talks to the operating system only through `PlatformServices.Current`. | Anything in the "forbidden in shared code" list below. |

### Forbidden in shared code

- `DllImport` / `LibraryImport`, COM interop, `Microsoft.Win32.Registry`, Win32 handles used as anything other than an opaque id.
- `System.Windows.Forms` and WPF. The UI is Avalonia.
- GDI+ at run time (`System.Drawing.Bitmap`, `Graphics`, `Font`, `Icon`, `Image.Save`). Images in shared code are SkiaSharp (`SKBitmap`) or encoded bytes (PNG). `System.Drawing` value types (`Point`, `Size`, `Rectangle`, `Color`) are fine; they are portable.
- `#if WINDOWS` (or any OS conditional compilation) and per-OS project targets such as `net10.0-windows`.
- `OperatingSystem.IsWindows()` / `IsLinux()` / `IsMacOS()` branches that contain operating system logic. Ask the platform service instead. The only accepted use is choosing which `IPlatformServices` implementation to create at start up.
- Running OS tools (`hyprctl`, `xdg-open`, `reg`, `powershell`, …) directly. Wrap them in a platform service, using `ICommandRunner` so they can be tested.
- Hard-coded paths, path separators or file name rules. Use `IPathService` and `Path`.

### Adding behaviour that differs by operating system

1. Find the service in `ShareX.Platform/Services` that owns the area, or add a new `I…Service` and expose it on `IPlatformServices`.
2. Model the result with portable types. Never leak `IntPtr` meaning, Win32 enums, X11 atoms or Cocoa objects through the interface.
3. Implement it in **every** platform project. Where a platform cannot do it, return `FeatureSupport.NotSupported(reason)` with a user-facing reason; on Linux use `LinuxPackages` so the reason names the package and the install command for the user's distribution. `UnsupportedServices.cs` holds reusable "not available" implementations.
4. In shared code, read `Support` before offering the feature, and hide or disable the UI with the reason as its tooltip rather than failing at run time.
5. Keep Windows behaviour identical to what it was. The Windows implementation is usually the code that used to live in the shared project, moved behind the interface.
6. Add tests in `ShareX.Platform.Tests` for parsing and argument building so they run on every OS (see the Linux and macOS parsers there).

### Enforcement

- Shared projects set `<WarningsAsErrors>$(WarningsAsErrors);CA1416</WarningsAsErrors>`. Never suppress CA1416 with `#pragma`, `[SuppressMessage]` or `NoWarn` in shared code; move the code behind a platform service instead.
- `dotnet build ShareX.sln -c Release -p:Platform=x64` must pass on Windows, Linux and macOS. CI (`.github/workflows/platform.yml`) builds and tests on all three.
- The Windows application must keep opening and running from Visual Studio on Windows without extra steps.

### Migration debt (remove, do not add to)

These break the rules above and are being moved behind platform services. Do not copy their patterns into new code. Update the list as items are done.

- [ ] `ShareX.HelpersLib.Windows` (WinForms helpers, update and print windows) and the `net10.0-windows` targets of `ShareX`, `ScreenCaptureLib`, `ImageEffectsLib`, `Tools`.
- [ ] Win32 interop in `ShareX.HelpersLib/Native` (`NativeMethods`, `WindowInfo`, `CursorData`, `DWMManager`) → `ShareX.Platform.Windows`.
- [ ] GDI+ image code (`ImageHelpers`, `ImageEffectsLib`, `ColorMatrixManager`, GIF encoding, the capture pipeline in `TaskHelpers`) → SkiaSharp.
- [ ] Remaining P/Invoke in `ScreenCaptureLib` (HDR, scrolling and transparent capture), `Tools` (mouse highlighter, borderless window, inspect window), `ImageEditor` and `ShareX.Avalonia` → platform services.
- [ ] WinForms tray icon and hotkey host (`MainForm`, `TrayIconService`) → Avalonia `TrayIcon` and `IHotkeyService`.
- [ ] `OperatingSystem.Is…()` branches in shared code: `HelpersLib` (`Helpers` admin checks, OS name, tablet mode and cursor clipping, `MimeTypes`, `ThreadWorker`, `Native/WindowsInput`, `ColorPickerWindowIntegration`), `ShareX.Avalonia` (`ScreenColorPickerWindow`), `ImageEditor` (desktop wallpaper services, emoji and cursor renderers, `EditorServices`).

## Other rules

- Never send real user data (screenshots, files, credentials) to third-party upload services in tests or manual checks. Test uploads against a local HTTP server, as `ShareX.Desktop.Tests` does.
- Uploading is opt-in. Defaults must not name or contact a third-party host.
- Secrets in settings are encrypted through `ISecretProtectionService`. Never fall back to writing them in plain text.
- Merge `develop` into `cross-platform` regularly so the branch does not drift.
