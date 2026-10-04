# Agent J: real KDE Wayland verification, 2026-10-04

Host: Fedora Linux 44 (KDE Plasma Desktop Edition), KDE Wayland (`XDG_CURRENT_DESKTOP=KDE`, `XDG_SESSION_TYPE=wayland`, `WAYLAND_DISPLAY=wayland-0`), Avalonia on the existing XWayland server (`DISPLAY=:0`). .NET SDK 10.0.112, runtime 10.0.12, x64. KDE screenshot portal version 2 and StatusNotifierWatcher were present. No packages, fonts or helpers were installed, and no desktop/compositor configuration was changed.

Source started at `c7b83801c`, including B40 `f67f3f338` and B41 `c7b83801c`. Both were pulled; B41 was not duplicated. Automatic cross-platform-v2 workflows remain excluded; the Linux workflow is manual and the macOS app job disabled.

## Method and evidence

Ran the full Release application on the actual desktop with a private copied application and settings under `/tmp/sharex-j-kde-20261004`. The copy's apphost was renamed `sharex-test` because its normal `ShareX` filename collides with the portable settings directory (B44). Disabled update checks, used generated images/text, and configured only an ephemeral IPv4 loopback custom uploader. Window rendering was read from the application's real XWayland windows; tray actions used KDE's real StatusNotifierItem/dbusmenu. Editor paste used keyboard events routed to its actual window.

- Main window: visible, correctly themed; startup logged approximately 1.1–1.8 seconds.
- Settings: application and task settings windows opened and rendered.
- Tray: registered active StatusNotifierItem, 128×128 ARGB icon and populated menu; settings/history/tool actions executed through it. Host loss/recovery and physical panel button interactions remain unverified (R45).
- Hotkeys: missing `sharex.desktop` prevented portal registration; the reason was logged without a blocking warning. No desktop entry was added. Shortcut firing remains unverified.
- Capture: fullscreen from the tray hung the UI and prevented normal SIGTERM completion. Temporary tracing in J-owned DesktopServices confirmed UI-thread execution, completed screen queries and the synchronous wait in CaptureFullscreen.Execute. No portal Request object appeared. Diagnostics were removed. Backend request R46/B47 is open for M; capture, region and capture-dependent tools are not signed off.
- Upload/history: generated 640×400 PNG uploaded through the real application to `http://127.0.0.1:36915/upload`; all 4930 bytes matched. Source and received SHA-256: `592a0213ae1e274fd5420fa25a5981b288d5a544d42bfd168a149e133b5d4503`. Returned loopback URL was persisted with the correct source path and Image type. SQLite `PRAGMA integrity_check` returned `ok`. The history window displayed the row and its image preview after restart. A second full-app run uploaded generated text (61 bytes, SHA-256 `7933672f71a5d66b7b3506e4266e9ca787ebf0632bca028935b5128ea44b390d`) and binary data (4096 bytes, SHA-256 `c8f5d0341d54d951a71b136e6e2afcb14d11ed8489a7ae126a8fee0df6ecf193`); both matched byte-for-byte at the server, persisted the correct Text/File rows and loopback URLs, and AutoClose exited 0. No third-party destination was used.
- Editor/clipboard: the same synthetic image opened in the full app's editor. Its copied image was received by an external X11 clipboard consumer as 640×400 RGBA, pixel-identical to the source. Paste opened the placement dialog; choosing insertion below created a selected image annotation and expanded the document to 640×800. B40 paste/expansion verified. Native Wayland clipboard reception, other import routes, and B41 cut/undo rendering remain unverified; a consumer without a focused native Wayland surface returned no image and is not evidence of a clipboard failure. Cut/undo attempts could not be visually verified after the editor window became hidden.
- Shutdown: local upload AutoClose and normal SIGTERM after the editor run both saved ApplicationConfig, UploadersConfig and HotkeysConfig, logged ShareX closed and exited with code 0. Tray-menu Exit also saved all three settings files and exited 0. Only hung fixture-owned capture processes were forcibly terminated after normal shutdown failed.

## B45 startup fix and validation

Early startup dialogs previously initialized Avalonia without a lifetime; later application startup threw `Avalonia is already initialized`. EnsureInitialized now prepares an explicit desktop lifetime, and Initialize attaches host callbacks to that same owned lifetime. Cold startup behavior remains unchanged; duplicate initialization and unrelated lifetimes are rejected.

Release solution build: 0 warnings/errors. All five proper test projects, with `SHAREX_TEST_DESKTOP=1`: 629 passed / 62 platform or optional-fixture skips. The three new child-process fixtures ran on this KDE desktop, including a real synthetic early dialog/message loop followed by desktop startup and clean exit. All eight history storage/exit tests ran on Linux. No fresh Windows execution is claimed from this Linux host; Windows 10/11 parity and native desktop checks remain outstanding.

The Linux/Windows estimates remain 90%; G1/G2 are not complete. Recording, hotkey firing, integration, mixed-scale/multi-monitor, capture-dependent tools, full clipboard/editor routes, and R20/R22 scope alignment remain open. Linux OCR/HDR and current macOS work are excluded by Jaex's instructions.

## B46 after-capture capability UI

The running KDE tray initially disabled Tools/OCR but offered After capture tasks/Recognize text (OCR). The shared UI now reads OCR/printing service support for after-capture choices in main/tray menus, task settings, the after-capture dialog and quick-task presets. Saved flags are retained; unsupported edits are rejected and reasons shown. Actual KDE dbusmenu verification confirms OCR is now disabled with the service reason. The other changed dialog interactions were not visually verified in this pass: the later fixture windows were unmapped when sampled. This is not a claim that R20/R22 backend policy is complete. Linux OCR remains excluded.

The final Release solution gate again passed with 0 warnings/errors, 629 tests passed and 62 skipped across all five proper projects, with actual-desktop startup fixtures enabled. No fresh Windows execution is claimed.

## R46 capture handoff recheck

Pulled M's `512e8f33d` async application capture fix. Actual KDE tray fullscreen capture returned, created a real portal Request and completed twice; monitor capture completed once. Each saved a 1918×972 PNG inside the private settings directory, with no upload. The tray remained responsive. Region capture created its real window; Escape cancelled it without another saved image. With a second region window pending, SIGTERM exited 0 in 0.202 seconds and saved all three settings files. The original B47 UI deadlock symptom is fixed.

A generated native Wayland fullscreen fixture was shown for capture. The resulting portal images also contain stale, overlapping editor/settings/tool windows from processes already closed. XWayland application windows were unmapped when sampled. The KWin service logs 54 “Failed referencing shared surface” / “Error -22 (Invalid argument)” entries around 17:13–17:15. This limits pixel correctness, current rendering and region interaction verification; those checks are not signed off. No desktop/compositor service or configuration change was made.

The combined source build passes. Publication is pending a narrow user ownership exception for the newly added M-owned policy-only RepositoryRulesTests file: two assertions require macOS/OCR scope and enabled automatic workflows, contrary to this session's direct user instructions. The combined run reports 630 passed, 62 skipped and those 2 policy failures; none were hidden or skipped. The exact removal patch is `/tmp/sharex-j-kde-20261004/remove-conflicting-policy-tests.patch`; runtime/backend source remains untouched by it.

## B44 normal portable apphost and latest gate

Pulled M's `caca2a0ec` path fix and launched the unrenamed Release apphost `./ShareX` from a fresh private copy. It used `/tmp/sharex-j-kde-20261004/normal-app/ShareX-Personal`, with no fallback or early path error. The real application uploaded the generated PNG byte-identically to the loopback endpoint, persisted its Image history row (`PRAGMA integrity_check` = `ok`), saved all three settings files and AutoClose exited 0. The renamed-apphost workaround is no longer needed.

After all incoming source through `32d1f97a1`, Release solution build: 0 warnings/errors. All five proper test projects ran with `SHAREX_TEST_DESKTOP=1`: **634 passed, 67 skipped, 2 failed**. Both failures are the new policy assertions described above (AgentsFile_KeepsTheAgreedRules and CrossPlatformWorkflow_RunsOnThisBranch). No failure was hidden or skipped. The application tests, all eight history tests and three actual-desktop startup fixtures passed. B46 and the scope-preserving merge/evidence commits remain local pending the narrow requested test-file ownership exception. Workflow exclusions are restored in the local combined tree but are not yet published; the other agent's remote workflow remains re-enabled.

H15 hidden-window recovery is an available J handoff but has not been implemented without a published claim. Full KDE visual/physical interaction, native Wayland clipboard/cut/import flows, hotkey firing and Windows desktop sign-off remain open. All fixture app/synthetic-window processes are closed.
