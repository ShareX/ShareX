# Lessons from the first cross-platform branch

The first attempt lived on a branch also named `cross-platform` (frozen at `55e0c7d90`, kept for reference). It produced working code for Linux and macOS, but it was written in parallel with Jaex's own migration on `develop`, and merging the two cost a 123-file conflict. `cross-platform-v2` starts again from `develop` (`fd61635f2`, after Jaex finished moving everything to SkiaSharp and Avalonia) and applies what that attempt taught. Read this before changing anything on the branch. The second attempt, `cross-platform-v2`, took over the name `cross-platform` on 2026-11-04.

## How we work

1. **One branch, two agents, from day one.** Both agents commit to `cross-platform-v2`. Parallel branches that touch the same files cost more to merge than the work itself. `develop` only takes v22 hotfixes from now on, and every hotfix is merged into `cross-platform-v2` the same day.
2. **Never port the same code twice.** In the first attempt both sides ported `ImageEffectsLib`, the capture pipeline and `HelpersLib` to SkiaSharp at the same time; one version had to be thrown away. Agree who ports what before starting, and work only in the projects you own.
3. **Moving files is the most expensive change.** Git's rename and directory-rename detection followed a folder move and put the other side's new files into the wrong project. Moving or renaming a file across projects is its own commit, announced to everyone working on the branch, who pull before continuing.
4. **No intermediate "Windows helpers" project.** `ShareX.HelpersLib.Windows` was meant as a temporary home for Windows code, and every file that passed through it was moved twice and conflicted twice. Windows-only code goes straight into `ShareX.Platform.Windows` behind a service, or stays where it is until it can.
5. **The Windows side needs a Windows machine.** Agent M (Linux) wrote the Windows implementations of HDR capture, transparent window capture, window snapping, the mouse hook and OCR, and none of it has run on Windows yet. Whatever moves into `ShareX.Platform.Windows` is not done until Agent J has run it (task J1).
6. **Commit and push small, often, and only green.** Build the whole solution and run every test project before each push; fetch first, because the other agent may have pushed.

## Architecture decisions that held up

7. **Platform abstraction, not conditional compilation.** `#if WINDOWS` and `OperatingSystem.IsWindows()` branches in shared code were tried and rejected: they spread OS logic through the codebase and cannot be tested on other systems. Interfaces live in `ShareX.Platform`; each OS implements every interface (or reports `FeatureSupport.NotSupported` with a user-facing reason). Shared code calls `PlatformServices.Current`. The only `OperatingSystem.Is…()` check is the one that picks the implementation at start up.
8. **Win32 interop does not belong in a shared library,** not even marked `[SupportedOSPlatform("windows")]`. An attempt to keep `NativeMethods` in `HelpersLib` that way was reverted; it kept the shared library Windows-shaped and blocked retargeting.
9. **CA1416 as an error in every shared project** turns a Linux run time crash into a build error. Do not suppress it; move the code behind a service.
10. **Model results with portable types.** `PlatformWindow`, `SnapTarget`, `ScreenInfo`, `PlatformRectangle`, `PixelBuffer` (straight-alpha BGRA). Handles cross the interface as `long` and mean nothing outside their platform.
11. **Keep pixels, not PNGs,** when a capture backend already has pixels (`ScreenCaptureResult.Pixels`). Encoding 4K captures to PNG just to decode them again was measurable.
12. **WinRT needs the Windows SDK target framework.** WinRT code (the native screen recorder; `Windows.Media.Ocr` until PaddleOCR replaced it) only compiles for `net10.0-windows10.0.22621.0`. `ShareX.Platform.Windows` builds both `net10.0` (for hosts that run everywhere) and the Windows SDK target; the Windows application builds the Windows SDK target.
13. **The application chooses its platform at start up** (`PlatformServices.Initialize`) and nothing else does.

## Build and tooling

14. **`Directory.Build.props` is case sensitive on Linux and macOS.** `develop` names it `Directory.build.props`, which MSBuild ignores on case-sensitive file systems. Rename it (and `Directory.Build.targets`).
15. **Set `EnableWindowsTargeting` on non-Windows hosts** so `net10.0-windows` projects still compile on Linux while they are being migrated.
16. **Build with `-p:Platform=x64`.** A plain `dotnet build ShareX.sln` uses `Any CPU`, which the projects do not configure.
17. **Windows-only projects:** `ShareX.Setup` (installer) and `ShareX.Steam` (`net48` launcher) stay Windows-only by design.

## Linux and macOS realities

18. **Wayland is not X11.** Applications cannot list other windows, move the pointer, read input meant for other windows or place windows at exact coordinates, except where the compositor offers an IPC (Hyprland's `hyprctl`, sway's `swaymsg`). GNOME and KDE on Wayland expose almost none of this. Design every window feature with a "not supported, here is why" path.
19. **Hyprland reports clients on hidden workspaces** with their last position. Filter by the monitors' active and special workspaces, or region capture snaps to invisible windows.
20. **Avalonia uses X11 (through XWayland on Wayland sessions).** Its window handles are X11 ids, not compositor ids, so "ignore my own window" must also exclude windows of the current process.
21. **Coordinates:** compositors report logical (scaled) layout coordinates, and `grim` returns physical pixels (a 3048-pixel-wide window at 1.25 scale captures as 3810 pixels).
22. **Name the missing package.** On Linux, `LinuxPackages` turns a missing tool into the install command for the running distribution (pacman, apt, dnf, rpm-ostree, zypper, nix and more). Users fix the problem instead of filing a bug.
23. **Tray icons created at run time must be registered with the application** (`DesktopServices.RegisterTrayIcon`, #8875), or they never appear.

## Safety rules learned the hard way

24. **Never upload real user data to third-party hosts in tests.** A test once uploaded a real 4K desktop screenshot to Imgur because Imgur was the default destination. Uploading is opt-in, defaults name no third-party host, and upload tests run against a local `HttpListener`.
25. **Secrets are always encrypted** through `ISecretProtectionService` (DPAPI on Windows, AES-GCM with an owner-only key file elsewhere). Saving fails rather than falling back to plain text. Secrets saved by Windows (DPAPI) cannot be read on Linux; tell the user to sign in again.
26. **ShareX and XerahS are different applications.** Never install ShareX over `~/.local/lib/xerahs`, and never change a user's compositor configuration (for example Hyprland key bindings) without asking.
27. **Check whether code is used before porting it.** The first attempt ported a GIF frame cache that nothing called, but the `ScreenRecordOutput.GIF` value next to it was in use.

## Merging develop and native code

28. **Merge `develop` the day it moves.** One month without a merge (2 October to 4 November 2026) cost 233 commits and 31 conflicting files, and `develop` had meanwhile added Windows-only code to shared projects: a native recorder, a Media Foundation player, a Win32 clipboard writer and GDI+ TIFF. Each one became a `ShareX.Platform` service with the Windows code in `ShareX.Platform.Windows` (f8c03a890).
29. **A resolved conflict is not a merged behaviour.** Taking `develop`'s video trimmer dropped two support checks this branch had added; only the existing tests caught it. Run every test project before committing a merge, and read each side's diff since the merge base, not just the conflict hunks.
30. **Native views go through `NativeControlHost` and a platform session.** `VideoView` hosts whatever child view `IVideoPlaybackService` creates: an HWND on Windows, an X11 window on Linux. Avalonia's X11 host reparents and resizes the child itself.
31. **Avalonia on macOS places windows in points but renders at a scaling of 2.** Use `WindowScaling.GetPositionScaling` when turning screen coordinates into window sizes; dividing by `RenderScaling` halves every overlay on a Retina Mac.
32. **GStreamer: `gst_bus_pop_filtered` discards the messages it skips.** Polling errors first silently dropped `ASYNC_DONE`. Pop every message once and branch on its type.
33. **GStreamer GL follows `WAYLAND_DISPLAY`** and crashes drawing into an X11 window; set `GST_GL_WINDOW=x11` in the native environment (`setenv`, not `Environment.SetEnvironmentVariable`) before `gst_init`.
34. **Prove it on the real artifact.** Both GStreamer faults passed compilation and unit tests; a small probe against a real video and window, and `coredumpctl` for the crash, found them. Keep such probes as opt-in tests (`SHAREX_GSTREAMER_WINDOW_TEST=1`).
35. **Do not follow Windows' DPI changes on a window that spans monitors.** The region capture overlay covers monitors with different scaling, so Windows switches its DPI as soon as it is moved or resized. Re-applying the size on `ScalingChanged` (e651835) resized it again, which switched the DPI back: about 2,800 switches in 10 seconds, seen as flicker, with selections landing off the screenshot. The overlay is sized in device pixels, so it now drops `WM_DPICHANGED` (`IWindowService.KeepScaleFixed`) and keeps the scale it was created with. Log `ScalingChanged`, `PositionChanged` and `Resized` before theorising.
36. **A flag merged from develop carries develop's assumptions.** `ScreenshotIsOpaque = true` was true on Windows because its capture set every alpha to 255; other platforms' captures did not promise that. When a merge brings a fast path that depends on a property of the data, establish the property in shared code (here `Screenshot` makes every screen capture opaque) instead of trusting each platform.
37. **Do not warm up with a real capture.** A 1x1 screenshot at startup is silent on Windows but goes through the screenshot portal on GNOME and KDE and runs `screencapture` on macOS.

## Reusable work on the first branch

These pieces exist on the first attempt, kept only on McoreD's machine as the local branch `archive/cross-platform-v1` (55e0c7d90). Bring them over by path (`git checkout archive/cross-platform-v1 -- <path>`), then adapt, instead of writing them again.

| What | Path on `cross-platform` | Notes |
| --- | --- | --- |
| Platform interfaces, models, detection, Linux package hints | `ShareX.Platform/` | Includes `IOcrService`, `IInputService`, `IWindowManagementService`, overlays and snap targets. |
| Windows, Linux and macOS implementations | `ShareX.Platform.Windows/`, `ShareX.Platform.Linux/`, `ShareX.Platform.MacOS/` | Windows parts unverified on Windows (lesson 5). |
| Platform tests (168) | `ShareX.Platform.Tests/` | Run on every OS. |
| Legacy image effect preset tests (65) | `ShareX.ImageEffectsLib.Tests/` | Already pass against Jaex's ImageEffectsLib. |
| Destinations and upload routing | `ShareX.Destinations/`, `ShareX.Destinations.Tests/` | |
| Cross-platform host and CLI (`sharex`) | `ShareX.Desktop/`, `ShareX.Desktop.Tests/` | Its future is task M10. |
| Portable ScreenCaptureLib design | commit `4a25a9e04` | Screenshot facade, snap targets, scrolling capture on services, frame window shapes. Re-apply to develop's current code, do not copy files blindly. |
| Tools port in progress | `stash@{0}` on McoreD's machine | OCR, clipboard viewer, mouse highlighter. |
| CI for all three operating systems | `.github/workflows/platform.yml` | |
| Linux installer | `Scripts/install-linux.sh` | Installs to `~/.local/lib/sharex`. |
