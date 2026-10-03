# Lessons from the first cross-platform branch

The first attempt lived on the `cross-platform` branch (frozen at `55e0c7d90`, kept for reference). It produced working code for Linux and macOS, but it was written in parallel with Jaex's own migration on `develop`, and merging the two cost a 123-file conflict. `cross-platform-v2` starts again from `develop` (`fd61635f2`, after Jaex finished moving everything to SkiaSharp and Avalonia) and applies what that attempt taught. Read this before changing anything on the branch.

## How we work

1. **One branch, two agents, from day one.** Both agents commit to `cross-platform-v2`. Parallel branches that touch the same files cost more to merge than the work itself. `develop` only takes v22 hotfixes from now on, and every hotfix is merged into `cross-platform-v2` the same day.
2. **Never port the same code twice.** In the first attempt both sides ported `ImageEffectsLib`, the capture pipeline and `HelpersLib` to SkiaSharp at the same time; one version had to be thrown away. Claim a task in [the tracker](cross-platform-delegation.md) before starting, and work only in the projects you own.
3. **Moving files is the most expensive change.** Git's rename and directory-rename detection followed a folder move and put the other side's new files into the wrong project. Moving or renaming a file across projects is its own commit, announced in the tracker's status log, and the other agent pulls before continuing.
4. **No intermediate "Windows helpers" project.** `ShareX.HelpersLib.Windows` was meant as a temporary home for Windows code, and every file that passed through it was moved twice and conflicted twice. Windows-only code goes straight into `ShareX.Platform.Windows` behind a service, or stays where it is until it can.
5. **The Windows side needs a Windows machine.** Agent M (Linux) wrote the Windows implementations of HDR capture, transparent window capture, window snapping, the mouse hook and OCR, and none of it has run on Windows yet. Whatever moves into `ShareX.Platform.Windows` is not done until Agent J has run it (task J1).
6. **Commit and push small, often, and only green.** Build the whole solution and run every test project before each push; fetch first, because the other agent may have pushed.

## Architecture decisions that held up

7. **Platform abstraction, not conditional compilation.** `#if WINDOWS` and `OperatingSystem.IsWindows()` branches in shared code were tried and rejected: they spread OS logic through the codebase and cannot be tested on other systems. Interfaces live in `ShareX.Platform`; each OS implements every interface (or reports `FeatureSupport.NotSupported` with a user-facing reason). Shared code calls `PlatformServices.Current`. The only `OperatingSystem.Is…()` check is the one that picks the implementation at start up.
8. **Win32 interop does not belong in a shared library,** not even marked `[SupportedOSPlatform("windows")]`. An attempt to keep `NativeMethods` in `HelpersLib` that way was reverted; it kept the shared library Windows-shaped and blocked retargeting.
9. **CA1416 as an error in every shared project** turns a Linux run time crash into a build error. Do not suppress it; move the code behind a service.
10. **Model results with portable types.** `PlatformWindow`, `SnapTarget`, `ScreenInfo`, `PlatformRectangle`, `PixelBuffer` (straight-alpha BGRA). Handles cross the interface as `long` and mean nothing outside their platform.
11. **Keep pixels, not PNGs,** when a capture backend already has pixels (`ScreenCaptureResult.Pixels`). Encoding 4K captures to PNG just to decode them again was measurable.
12. **WinRT needs the Windows SDK target framework.** OCR (`Windows.Media.Ocr`) only compiles for `net10.0-windows10.0.22621.0`. `ShareX.Platform.Windows` builds both `net10.0` (for hosts that run everywhere) and the Windows SDK target; the Windows application builds the Windows SDK target.
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

## Reusable work on the first branch

These pieces exist on `cross-platform` and can be brought over by path (`git checkout cross-platform -- <path>`), then adapted, instead of being written again. Bringing one over is a claimed task like any other.

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
