# B48: Fedora session shutdown, 2026-10-04

Jaex reported Fedora KDE cancelling logout/shutdown while ShareX was running. Work was claimed and pushed as `b614349d8` before source changes.

The main window cancelled every ordinary close to hide in the tray, including Avalonia's `WindowCloseReason.OSShutdown`. Avalonia treats a window remaining open as cancellation of the desktop shutdown request. The editor, a message box without a Cancel button and busy scrolling/auto-capture/pin windows had similar cancellation paths. The six UI handlers now accept OSShutdown while retaining their existing ordinary-close behavior and cleanup. No platform contract, backend, dependency or Windows session implementation changed. The request propagation is described in [Avalonia's desktop lifetime source](https://github.com/AvaloniaUI/Avalonia/blob/12.1.3/src/Avalonia.Controls/ApplicationLifetimes/ClassicDesktopStyleApplicationLifetime.cs).

## Actual application session protocol

On the existing Fedora 44 KDE Wayland desktop, ran a copied full Release ShareX app with private portable settings, updates/uploads/hotkeys disabled and no external network requests. A private XSMP server used the desktop's existing libSM/libICE; only the fixture process received its SESSION_MANAGER address. It sent SaveYourself with shutdown=true, fast=false and interaction allowed, then recorded the application's InteractDone response. The real KDE session manager was never asked to log out or shut down. No packages or desktop configuration were changed.

- The retained pre-fix application replied cancelShutdown=true, reproducing the reported veto. SIGTERM then closed this fixture normally.
- The fixed application replied cancelShutdown=false and exited 0 for visible-window and silent startup runs.
- A further fixed run received ordinary WM_DELETE_WINDOW first and stayed running in the tray, then accepted the session request and exited 0.
- All four runs logged successful ApplicationConfig, UploadersConfig and HotkeysConfig saves and completed closure. The fixed visible run exited about 0.48 seconds after the request.

[Protocol receipts and binary hashes](assets/2026-10-04-agent-j-kde/b48/session-results.json) distinguish the retained baseline from the new build. The private probe source and scratch applications are retained under `/home/jaex/.codex/sharex-agent-j/kde-20261004/shutdown`.

## Regression and publication gate

Two new proper child-process cases run through the real Avalonia message loop: a dirty editor with exit confirmation enabled and a Yes/No message box without Cancel. They first verify that ordinary close keeps the window open, then inject only that child's OS shutdown event into the desktop lifetime and require no veto, no remaining windows and exactly one startup/exit callback. Both fail with “The window vetoed OS shutdown” against the retained baseline assemblies and pass with the fix. They are opt-in through the existing SHAREX_TEST_DESKTOP=1 fixture setting.

Full Linux Release solution build: 0 warnings, 0 errors. All five test projects with actual-desktop fixtures enabled: **635 passed, 67 platform/optional skips, 0 failures**. Platform 278/37, History 8/0, ImageEditor 180/28, ImageEffects 65/0, Tools 104/2 (pass/skip). All five Avalonia desktop-lifetime cases ran.

This verifies ShareX's real XSMP acknowledgement and normal process cleanup on KDE/XWayland without ending the desktop session. Interactive shutdown of the whole Fedora desktop, other desktops, busy tool interaction and fresh Windows desktop execution remain unverified. Estimates stay 90%; broad Linux/Windows gates and deferred macOS work are unchanged.
