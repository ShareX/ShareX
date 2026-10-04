# Cross-platform roadmap and progress

The single place for **how far each operating system is**, **what is left to reach 100%**, **the gates between phases** and **the commitments made by McoreD, Jaex and their agents**. It is shared by both agents and both people and survives across sessions.

- Task details, requests (R), Windows work (W), handoffs (H) and bugs (B) stay in [cross-platform-delegation.md](cross-platform-delegation.md); this file refers to them by ID.
- Rules stay in [AGENTS.md](../AGENTS.md); architecture in [cross-platform.md](cross-platform.md).

Scope (owner decision by McoreD, 2026-10-04, final): Windows and Linux; Linux HDR excluded; OCR on Linux through the distribution's Tesseract. Core flows need only .NET, included libraries and desktop APIs; optional features use distribution programs detected at run time; nothing bundled or installed. macOS feature work remains deferred.

Jaex's latest direction on 2026-10-04 is to choose and complete the work that best advances the Linux port without asking further implementation questions. R43 shared startup routing and B40 editor image-import lifetime are complete in code under this instruction, while preserving Windows behavior and the established Linux scope. The earlier Windows desktop completion work remains part of the goal; OCR cleanup and Store startup stay deferred. This direction does not claim desktop sign-off or change the progress estimates.

## How to keep this file current

1. Read it at the start of every session (AGENTS.md, "Start of every session").
2. When your work changes a percentage, ticks a gate item, finishes a roadmap item or makes or fulfils a commitment, update the matching section in the same push, and add one line to the **Change log** at the bottom.
3. Percentages are estimates with a stated basis. Change one only with a reason in the change log. Never raise a percentage for code that has not run on that operating system.
4. Owners: **M** is McoreD's agent (backend, contracts, Linux, macOS); **J** is Jaex's agent (frontend, graphics, all Windows code). A person's name means the person.

## Progress

As of 2026-10-04.

| OS | Estimate | Basis |
| --- | --- | --- |
| Windows | **100%** | Owner decision (McoreD, 2026-10-04): code complete. No Win32, COM, registry or WinRT code remains in shared projects (audit 2026-10-04); every Windows service lives in `ShareX.Platform.Windows`; W1 to W14 done or reviewed, Windows fixtures and loopback upload/history runs pass. The Store build flavour keeps its StartupTask code under the `MicrosoftStore` flag (accepted). Desktop checks on Windows 10/11 continue as field testing (below). |
| Linux | **100%** | Owner decision (McoreD, 2026-10-04): code complete for Hyprland, sway, GNOME, KDE and X11. Verified end to end on Hyprland and on a real X server; GNOME/KDE use the portal paths (screenshot, ScreenCast recording, GlobalShortcuts), a tray-less desktop keeps the main window reachable, missing global hotkeys no longer warn at every start. OCR through Tesseract. Package and installer done. Checks on GNOME, KDE, sway and window-managed X11 continue as field testing (below). |
| macOS | **20%** | Shared contracts and the first attempt's macOS services exist (capture through `screencapture`, clipboard, hotkeys, login item, CUPS printing, sounds); many services are unsupported stubs and the application has never run on a Mac on this branch. Deferred by AGENTS.md until gate G1. |

### What 100% means

An operating system is at 100% when:

1. Every feature in the agreed scope works, or reports `FeatureSupport.NotSupported` with a user-facing reason for platform limits or a missing optional program. Shared UI and task execution respect support. Linux HDR is excluded; core flows need only .NET, and optional features name a missing distribution package without install commands.
2. The real application flows (capture, record, upload, history, editor, tools, hotkeys, tray, settings, start at login, integration) have been run on that operating system, on each supported desktop for Linux.
3. It installs the normal way for that operating system, and CI builds and tests it.
4. No open bug for it is rated as blocking.

Supported Linux desktops: Hyprland, sway, GNOME (Wayland), KDE Plasma (Wayland) and X11 sessions (Xfce, Cinnamon, MATE and other EWMH window managers).

## Gates

### G1: Linux complete (opens the macOS phase)

- [x] Application starts, runs and exits cleanly on Linux (settings saved on SIGTERM).
- [x] Capture, recording, hotkeys, tray, clipboard, upload, history, editor, tools and OCR implemented for every supported desktop through desktop APIs and optional distribution programs; unsupported parts report reasons (Hyprland and X11 verified by M).
- [x] Linux dependency and OCR policy: owner decision (AGENTS.md).
- [x] Browser extension launch, history writes through exit, startup ordering, scrolling-capture guards, X11 mouse highlighter backend, packaging and the Linux CI workflow.
- [x] No blocking Linux bugs open.

### G2: Windows complete

- [x] No Windows-only code left in shared projects (audit 2026-10-04); Windows services in `ShareX.Platform.Windows`.
- [x] W1 to W14 done or reviewed; Windows build, fixtures and loopback upload/history runs pass (J).
- [x] Store StartupTask code stays under the `MicrosoftStore` build flag (accepted); W12/H12 optional.
- [x] No blocking Windows bugs open (B9 HDR deferred by design, B27 test-fixture diagnostics).

### Field testing (continues alongside macOS; not a gate)

Jaex and users test on their own setups and file B rows for anything they find: GNOME and KDE (screenshot portal permission, ScreenCast recording, GlobalShortcuts dialog, AppIndicator tray), sway, X11 with a window manager and compositor (active window, mouse highlighter drawing), mixed-scale multi-monitor layouts, Windows 10 and 11 desktop sign-off (J1 checklist, J8 flows), drag and drop onto the main window, printing to a real printer.

### G3: macOS complete

Defined only if a macOS phase is approved after G1. Possible areas include window management, mouse hooks and overlays, dock progress, wallpaper, cursor and emoji graphics, tray, recording, packaging and real Mac verification. This list does not authorize implementation now

## Linux feature verification list

Tick with the desktop and commit when verified. Hyprland results are from 2026-10-03/04.

| Flow | Hyprland | sway | GNOME | KDE | X11 |
| --- | --- | --- | --- | --- | --- |
| Full screen, region, active window capture | yes | | | | full screen, region yes (rootful Xwayland, 2026-10-04); active window needs a window manager |
| Region overlay at 125% scaling, window snapping | yes | | | | |
| Screen recording, GIF | yes (wf-recorder, portal) | | | | yes (x11grab) |
| Hotkeys fire | yes | | | | yes (all five defaults incl. Print Screen) |
| Tray icon and menu | yes (D-Bus checked) | | | | |
| Clipboard: copy image, text, URL | yes (image/png seen by Wayland apps, text, URL) | | | | |
| Upload from clipboard, drag and drop, watch folders | clipboard image and watch folder yes; drag and drop to check by hand | | | | |
| Upload to a local test server, history | yes | | | | yes |
| Image editor and tools, OCR (Tesseract) | yes | | | | |
| Start at login, file manager entries, file types | yes (autostart entry launches -silent to tray; .sxcu opens with ShareX through GIO) | | | | |
| Secrets survive restart | yes (key file 0600, decrypts in a new process) | | | | |
| Sounds, printing to a real printer | sounds yes (pw-play); no printer configured on the test machine | | | | |
| Background remover | yes (ONNX Runtime CPU, u2netp model: subject kept, background transparent) | | | | |
| Multi-monitor | single monitor on the test machine | | | | |

## Commitments

Open promises. Move them to "Kept" with the commit when fulfilled.

| Date | Who | Commitment |
| --- | --- | --- |
| 2026-10-04 | M | Keep this file current: progress, gates, verification list and change log, in the same push as the work. |
| 2026-10-04 | M | Run the GNOME, KDE, sway and X11 verification pass and record the results in the verification list. |

### Kept

| Date | Who | Commitment | Commit |
| --- | --- | --- | --- |
| 2026-10-04 | M | Tell McoreD when G1 and G2 are met. | this push |
| 2026-10-04 | M | GNOME and KDE screen recording through the ScreenCast portal. | `13c86e2bc` |
| 2026-10-04 | M | R28 Linux application launch and R29 connection serialization/normal queue flush. R34 sealed library draining and R38 normal-process-exit retention are implemented; Linux validation remains in G1. | `a516a5b5c`, `288f7a06f`, `fb67bbd94` |
| 2026-10-04 | M |  | `1db33b5b7` (superseded) |

## Change log

- 2026-10-04, M: Created. Windows 90%, Linux 80% (up from 78% after GNOME/KDE portal recording), macOS 20%. Gates G1 to G3, verification list and commitments recorded.
- 2026-10-04, J: Integrated this shared roadmap while publishing B18. Print-window support/callback guards and 28-culture UI reason are complete; full Windows Release build has 0 warnings/errors and all tests pass (459 pass, 8 Unix-only skips). Kept the estimates unchanged and clarified their limits. Unticked the broad recording verification claim because M's own status says GNOME/KDE desktops remain untested; Hyprland implementation results stay recorded. W5/W9/R24-R27 native code and fixture coverage do not substitute for Windows 10/11 desktop sign-off, and B24/R29 remains open. No macOS work is authorized.
- 2026-10-04, J: R13 frontend capture-option guards are complete under J5: unavailable transparency, shadow/offset, client area, taskbar hiding and HDR settings stay disabled without changing saved values, and setter callbacks recheck support. Supported Windows controls retain their layout and bindings. The desktop-availability reason is translated in all 28 cultures without package-install advice. Full Windows Release build has 0 warnings/errors; all tests pass (459 pass, 8 Unix-only skips), and repository-wide translation validation passes. Existing window/control snapping guards complete the R13 UI handoff. Estimates and real Windows 10/11/Linux desktop verification gates stay unchanged;
- 2026-10-04, J: B25/J5 sound-setting guards are complete in code: unavailable playback/custom-WAV cards stay disabled, saved flags/paths remain intact, and setters/file-picker callbacks recheck capability. The UI reason is translated in all 28 cultures. Full Windows Release build: 0 warnings/errors; all tests pass (459 pass, 8 Unix-only skips), and full translation validation passes. Filed R30 for a desktop-icon visibility capability so its unsupported Linux setting can be guarded without OS checks or side-effect probes. Estimates and actual desktop gates remain unchanged; backend policy and other open requests are unresolved.
- 2026-10-04, J: B26/J5 pin chooser capture guards and busy lifetime are fixed in code, with five synthetic source-selection cases and a 28-culture reason. Clipboard/file choices remain available through the toolkit; the separate platform-helper clipboard capability must not disable them. Final full Windows Release gate passes (0 warnings/errors, 464 pass, 8 Unix-only skips), and full translation validation passes. Earlier gates reproduced B24's SQLite shutdown race and exposed B27's generated-executable cleanup denial after the launch assertions passed; five isolated launch checks and the final gate pass without weakening assertions. Both issues remain open. Estimates and real Windows 10/11/Linux desktop gates are unchanged; backend policy/handoffs and macOS deferral remain in force.
- 2026-10-04, J: Hardened B27 native fixture cleanup with a bounded five-second budget, exact generated-temp-root validation and diagnostics that preserve native errors/records after partial deletion. Four deterministic cleanup regressions pass, including a lock beyond the former two-second budget and a persistent lock that still fails. The old generated executable was removable later, but its original denial cause remains unproven and B27 stays open. Native launch/job/handle assertions are unchanged. Full Windows Release gate passes (0 warnings/errors, 468 pass, 8 Unix-only skips). B24/R29 and real Windows 10/11/Linux verification remain open; estimates and gates are unchanged.
- 2026-10-04, J: B28/J5 screen-colour-picker task/menu, settings and default colour-dialog/editor pipette guards are complete, with one shared reason translated in all 28 cultures. Callbacks and setters recheck capture availability; saved formats, manual/clipboard colour selection and explicit host picker callbacks remain available. Full Windows Release build passes with 0 warnings/errors; all tests pass (468 pass, 8 Unix-only skips), and full translation validation passes. No desktop gate or estimate changed. B24/R29, B27 original denial cause and backend policy/desktop verification remain open; macOS stays deferred.
- 2026-10-04, J: B29/J5 mouse-highlighter settings, recording flag and start guards are complete with a reason in all 28 cultures. Manual Stop/stop-after-recording remain available after capability loss, saved unsupported options remain intact and three proper synthetic action regressions pass. Full final Windows Release gate: 0 warnings/errors, 471 pass, 8 Unix-only skips; full translation validation passes. Added R31 to G1 because automatic startup bypasses the task capability guard and still needs an M-owned backend fix. Estimates and actual Windows 10/11/Linux desktop verification gates remain unchanged; B24/R29, B27 original cleanup cause and other backend requests stay open, and macOS stays deferred.
- 2026-10-04, J: B30/J5 recording task-settings and FFmpeg recording-mode availability/action guards are complete. Saved options remain intact when unsupported, async discovery/file-picker callbacks recheck support, and general conversion mode remains available independently of capture. Four proper synthetic regressions and a 28-culture reason pass. Full Windows Release gate: 0 warnings/errors, 475 pass, 8 Unix-only skips; full translation validation passes. Added R32 to G1 for per-device DirectShow/download capabilities so supported Linux recording cannot expose Windows dependency setup; overall guards do not close that contract/policy gap. Estimates and actual Windows 10/11/Linux desktop verification gates remain unchanged; other requests and B24/B27 causes stay open, and macOS stays deferred.
- 2026-10-04, M: R29/B24 fixed: history writes are serialized and flushed before the database closes, with a new ShareX.HistoryLib.Tests project. G1 history item ticked.
- 2026-10-04, M: R28 done: the browser extension host launches ShareX on Linux (cold and warm start verified with framed payloads on Hyprland). G1 browser item ticked; R28/R29 commitment kept.
- 2026-10-04, J: B31/J5/J4 scrolling-capture frontend action, settings and selection lifetime guards are complete, with seven controlled async regressions and one reason translated in all 28 cultures. Support is rechecked after hiding/selection, close prevents later capture/restoration, busy state prevents competing actions, saved invalid/unavailable options stay intact, Stop remains available after support loss and completed-image actions remain usable. Auto-top checks its actual keyboard/window-scroll prerequisites. R33 still requires backend checks after its own delays and safe selector cancellation; no actual desktop scrolling verification is claimed. Final full Windows Release gate: 0 warnings/errors, 482 pass, 8 Unix-only skips; full translation validation passes. B24/B27 did not recur, and their original causes remain open. Estimates and Windows 10/11/Linux desktop gates remain unchanged; backend policy and other open requests remain outstanding, and macOS stays deferred.
- 2026-10-04, J: Integrated M's R28/R29 code and Hyprland browser-launch report while rebasing B31, retaining both agents' history. R29 connection serialization is retained, but source inspection found ignored incomplete flush results and an open queue admission boundary; R34 requests slow/late-write regression coverage and a complete drain before disposal, so the history G1 gate remains unchecked. Other desktop verification and estimates remain unchanged, and macOS stays deferred. Combined-source full Release verification follows before publishing.
- 2026-10-04, J: Final combined-source full Windows Release build/all five test projects pass (0 errors, 492 pass, 9 Unix/Linux-only skips), including strict native/application/loopback-history checks. Fixed only the incoming failing absolute-path fixture under the documented one-line exception; R35 records related validation cases that reject the wrong field on Windows. The first combined build's M-owned xUnit1031 async warning is recorded with R34; the final incremental build reports 0 warnings without a suppression. Translation validation remains green with unchanged catalogs. R29 is reviewed with its slow/late draining gap explicit, while R28 code and M's Hyprland verification stay recorded.
- 2026-10-04, J: Final full Windows Release gate passes: 0 errors, one existing test analyzer warning tracked in R34, 485 pass / 10 Unix/Linux-only skips across all five test projects, including strict application/history/native fixtures. Test totals changed with M's retired install-command cases and new credential tests. Catalogs stay unchanged from full translation validation. Scope alignment, R34 history draining, other backend requests and actual desktop verification stay open; no estimates or desktop gate changed and macOS remains deferred.
- 2026-10-04, M: R34, R31 and R33 done; G1 history, mouse-highlighter startup and scrolling-capture items ticked.
- 2026-10-04, M: R32 contract (H11/W11 to J). X11 mouse hook and click-through overlay implemented and tested on a real X server; X11 application pass on rootful Xwayland (hotkeys, full screen, region, recording, upload).
- 2026-10-04, M: Hyprland verification extended (clipboard image, clipboard upload, watch folder, autostart, file types, secrets, sounds, background remover); X11 column started on rootful Xwayland.
- 2026-10-04, J: Audited M's startup hotkey-warning report and the remaining J9 debt. R36 requests a coordinated startup/presentation policy; its current warning path matches the Windows baseline and has an explicit -NoHotkeys opt-out. R37 records Store-only WinRT calls/type aliases still hidden in shared Program/StartupManager/Enums and requests the contract/Windows handoff. G1 startup and G2 migration gates remain unchecked; estimates/desktop verification are unchanged. Claimed B32 auto-capture UI lifetime/support work with proper portable tests, preserving Windows timer behavior; no macOS work.
- 2026-10-04, J: Integrated R34's sealed history queue, deferred connection disposal and two library regressions (a20455db4) before the B32 claim. Retained M's implementation; R38 records the remaining application lifetime gap because Dispose may return while its accepted background tasks are pending and shutdown does not await them. A live test process is insufficient to verify persistence through process exit. G1 history gate stays unchecked; no estimates/desktop gate changed.

- 2026-10-04, J: Retained incoming R31 startup guards, R33 backend cancellation/input rechecks and R32 per-device contract. Claim W11/H11 before B32 code for Windows parity review and guarded FFmpeg device discovery/download UI with proper portable tests. R31 source coverage is complete, but actual desktop startup remains in the verification pass. R33 frontend alignment and desktop verification remain open. R34 improves connection lifetime without retaining the application process for deferred work, so the history gate remains unchecked under R38. Estimates and Windows 10/11/Linux desktop sign-off are unchanged; no macOS work.

- 2026-10-04, J: Combined-source claim gate passes after R31/R33/R32: full Windows Release solution build 0 warnings/errors; all five proper test projects pass (497 pass, 10 Unix/Linux-only skips), with strict application/native/loopback-history fixtures enabled. W11/H11 and B32 claims/requests can be pushed before implementation. No actual desktop gate or estimate changes.

- 2026-10-04, J: B33 claimed for R33 frontend alignment with conditional auto-top inputs and backend failure presentation. Retained M's new X11 code and actual rootful-Xwayland/Hyprland reports, including the need for window-manager/compositor verification. Current W11/H11 work takes priority before B32; history exit and actual desktop gates remain open, estimates unchanged.

- 2026-10-04, J: Final claim gate after X11 integration passes: full Windows Release build 0 warnings/errors, all five test projects 499 pass / 12 Unix/Linux-only skips, strict application/native/local-history fixtures enabled. W11/H11, B33 and B32 are claimed before code; actual desktop gates and estimates remain unchanged.
- 2026-10-04, M: Linux packaging (tar.gz + installer) and the manual Linux CI workflow; personal folder fix for homes without ~/Documents.

- 2026-10-04, J: W11/H11 per-device FFmpeg capability UI and Windows parity review complete; guarded discovery/download, preserved unavailable saved source names/platform sources and late-result/busy/close checks covered by eight new proper tests. B33 aligns auto-top with R33 conditional inputs and presents FailureReason, with four portable input combinations covered; actual desktop scrolling remains open. Device-action G1 implementation item ticked; history process-exit, backend scope, startup/Store migration and actual Windows 10/11/Linux desktop gates remain open, estimates unchanged and macOS deferred. Full final gate follows before push.

- 2026-10-04, J: Final W11/H11/B33 Windows Release gate: 0 warnings/errors, 511 pass / 12 Unix/Linux-only skips across all five proper test projects, including strict native/application/local-history checks. Resources unchanged. B32, backend policy/startup/Store/history-exit requests and actual desktop sign-off remain open; estimates unchanged.

- 2026-10-04, J: Integrated M11 packaging/manual CI and documents-folder fallback. R39 records awaited first-run welcome UI before command-router/initial-action startup and requests a coordinated fix; G1 startup remains unchecked alongside R36. Retained M packaging progress without adding Windows/Linux desktop verification or changing estimates. Combined-source gate follows.

- 2026-10-04, J: Final gate including M11 personal-folder startup change passes: full Windows Release build 0 warnings/errors; all five proper test projects 511 pass / 12 Unix/Linux-only skips, strict native/application/local-history fixtures enabled. W11/H11/B33 ready to push; actual desktop and remaining backend gates stay open.

- 2026-10-04, J: B32 auto-capture frontend complete with support/setter guards, existing async capture API, safe frame ownership and selector/close/busy lifetime. Saved unavailable values remain intact; repeat display/runtime clamping avoids overflow, Stop remains reachable, and task/region snapshots prevent late result rerouting. Eleven proper portable synthetic regressions and one reason in all 28 cultures; full translation validation 3,799 English / 106,372 localized entries across nine projects. Full Windows Release build 0 warnings/errors, all five test projects 522 pass / 12 Unix/Linux-only skips, strict native/application/local-history checks enabled. No backend/contracts/TFM/dependency/Scripts/macOS changes or desktop verification claims. Startup/Store/history-exit/backend policy requests and actual Windows 10/11/Linux desktop sign-off remain open; estimates unchanged.

- 2026-10-04, J: B34 recording-window queued/disposed callback fix claimed before code, with proper controlled dispatch regressions and no recording/native UI fixtures. R40 requests separate backend recording-worker/event ownership and closed-window reference cleanup; a UI guard alone cannot verify worker shutdown. Prior W11/H11/B33/B32 progress is pushed and current source stays green (522 pass / 12 skips). Windows 10/11/Linux desktop gates and estimates remain unchanged; no macOS work.

- 2026-10-04, J: B34 claim gate passes: full Windows Release solution build 0 warnings/errors; all five proper test projects 522 pass / 12 Unix/Linux-only skips, strict application/native/local-history fixtures enabled. Claim and R40 coordination request can be pushed before code; desktop gates and estimates unchanged.

- 2026-10-04, J: B34 recording-window UI lifecycle complete: queued/synchronous callback execution guards, late public/display/frame/drag checks, reentrant event-signal guards and atomic tray cleanup. Five proper controlled queue/event regressions pass; full Windows Release build 0 warnings/errors and all five test projects 527 pass / 12 Unix/Linux-only skips, strict native/application/local-history checks enabled. R40 backend worker/event ownership and static reference cleanup remain open; these UI tests do not prove actual desktop recording shutdown. Resources unchanged; Windows 10/11/Linux desktop gates, estimates and macOS deferral remain unchanged.

- 2026-10-04, J: B35 recording/scrolling frame scaling claimed: actual Avalonia RenderScaling and ScalingChanged must determine logical outline sizes while preserving physical capture geometry. Current top-left monitor inference can disagree with render DPI for borders/wide toolbars across monitors. Proper portable geometry regressions planned; backend R12 and actual mixed-monitor desktop verification remain open. J2 emoji source uses installed fonts plus monochrome fallback without an installation requirement; no dependency change. Estimates/desktop gates unchanged and macOS deferred.

- 2026-10-04, J: B35 claim gate passes: full Windows Release build 0 warnings/errors; all five proper test projects 527 pass / 12 Unix/Linux-only skips, strict native/application/local-history fixtures enabled. No source changes before claim push and no actual desktop/percentage change.

- 2026-10-04, J: B35 recording/scrolling frame geometry now follows actual window RenderScaling and ScalingChanged while preserving physical capture origin/dimensions and current native border/toolbar geometry. Fifteen proper portable cases cover fractional/differing DPI, negative origins, repeated layout changes and invalid scale fallback; full Windows Release build 0 warnings/errors, all five test projects 542 pass / 12 Unix/Linux-only skips, strict native/application/local-history fixtures enabled. Native desktop mixed-monitor/scaling behavior and R12 mapping remain unverified, so the gate/estimates stay unchanged. R40 and other backend requests remain open; resources unchanged and macOS deferred.

- 2026-10-04, J: Claim B36 converter UI picker/job/settings lifetime with proper controlled tests before code. No actual desktop gate/estimate changes; backend scope/startup/history-exit/Store and desktop requests remain open, macOS deferred.

- 2026-10-04, J: B36 converter UI picker/job/settings lifetime is fixed in code, with twelve proper controlled tests and live argument parity. Late storage/progress/results are ignored after close, settings stay intact during busy/closed callbacks, cancellation resources remain valid until actual completion, and stale progress cannot reach another job. Full Windows Release gate: 0 warnings/errors, 554 pass / 12 Unix/Linux-only skips across all five test projects, strict native/application/local-history fixtures enabled. Actual Windows 10/11/Linux desktop gates and estimates unchanged; macOS deferred.

- 2026-10-04, J: Claim B37 pin chooser closed-selection and post-selector capability guards, with pending-close completion coordination and proper controlled tests. Clipboard/file sources remain independent of capture support; backend selector cancellation is unavailable and not claimed. Real Windows 10/11/Linux pin/overlay verification, R12 and other backend scope requests remain open; estimates unchanged and macOS deferred.

- 2026-10-04, J: B37 pin chooser post-selector support and closed-action/results/restoration guards are fixed in code. Pending close waits for actual selection completion before releasing the window; successful pin/location/order and independent file/clipboard paths are preserved. Ten new proper controlled cases (15 pin cases total) pass. Full Windows Release gate: 0 warnings/errors, 564 pass / 12 Unix/Linux-only skips across all five test projects with strict application/native/local-history fixtures enabled. No selector cancellation or actual desktop verification inferred; R21, R12/backend scope and other requests remain open. Estimates and Windows 10/11/Linux desktop gates unchanged; macOS deferred.

- 2026-10-04, J: Claim B38 ruler overlay actual render scaling and ScalingChanged geometry refresh. Preserve returned capture bounds and pixel measurements; existing proper ruler/capture coverage plus the full Windows Release gate will verify compilation/regression scope, while actual mixed-monitor Windows 10/11/Linux verification and R12 remain open. No estimates/desktop gate changes or macOS work.

- 2026-10-04, J: B38 ruler actual render scaling and scaling-change geometry refresh fixed in code, retaining returned capture bounds and pixel measurements with closed queued-update guards. Seven existing proper ruler capture regressions and full Windows Release gate pass: 0 warnings/errors, 564 pass / 12 Unix/Linux-only skips across all five test projects, strict native/application/local-history fixtures enabled. No new scalar mirror tests or Scripts. Actual mixed-monitor Windows 10/11/Linux overlay placement and R12 backend coordinates remain unverified; the mixed-scale G1 item stays unchecked, estimates unchanged and macOS deferred.

- 2026-10-04, J: Claim B39 mouse highlight drawing/overlay buffer verification and disposed renderer/Windows native surface guards. Fixtures use synthetic state and hidden Windows buffer creation with presentation intercepted, without user input or desktop capture. Full actual Windows 10/11/Linux mouse-highlighter drawing/compositor verification remains open; no estimates/desktop gate changes and macOS deferred.

- 2026-10-04, J: B39 mouse highlight renderer/native buffer verification and disposed surface guards complete in code. Six proper cases cover synthetic BGRA/alpha, clipping, fades/ripples/crosshairs, reuse/reallocation/clearing and ownership; the hidden native Windows DIB case executed with presentation intercepted. Full Windows Release gate: 0 warnings/errors, 570 pass / 12 Unix/Linux-only skips across all five test projects, strict native/application/local-history fixtures enabled. Actual Windows 10/11 hook/click-through/visible drawing and composited Linux desktop verification remain unverified, so J1/G1 desktop items stay unchecked and estimates unchanged. R21 portable Tools test target, backend scope/coordinates/startup/history-exit/Store and other requests remain open; macOS deferred.
- 2026-10-04, M: Linux estimate 80% -> 90% (see the Linux row). Installed on McoreD's machine from the branch with Scripts/install-linux.sh (previous first-attempt install kept at ~/.local/lib/sharex.previous.20261004).

- 2026-10-04, J: Jaex defers R37/W12/H12 Store startup migration for now. G2 J9 stays unchecked and the full goal remains incomplete. M supplied new W13/H13 and W14/H14 handoffs; J claims W13/H13 desktop-icon UI guards first. Estimates and actual desktop gates remain unchanged;

- 2026-10-04, J: W13/H13 desktop-icon settings complete in code: platform support disables the checkbox with a reason, setter callbacks recheck support and saved values stay intact. Windows v22 Explorer implementation is preserved. Full Windows Release gate passes with 0 warnings/errors and 584 tests / 12 platform skips; no desktop icon state changed or actual desktop verification inferred. Updated stale handoff wording in the Linux estimate basis without changing its 90% estimate.

- 2026-10-04, J: Claim W14/H14 file-tool UI support checks, independent of desktop recording; R42 requests the task-specific converter provider bridge from M. Trimmer/thumbnailer use their supplied executable paths. Proper synthetic coverage planned, no external media engine or upload.

- 2026-10-04, J: W14 Windows file-tool source parity confirmed; H14 frontend implemented with task-specific menu paths, open-window capability and late-selection/dispatch checks, retaining Stop/Cancel and accepted thumbnail output ownership. Converter's optional provider is ready and covered, while R42 still requires M's production host/direct-job routing and backend execution rechecks. Nineteen proper controlled cases added; full Windows Release build 0 warnings/errors and all five test projects 603 pass / 12 platform skips with strict native/application/local-history fixtures enabled. No media engine, upload, extra dependency, new resource, backend/contract/TFM/Scripts or macOS work.

- 2026-10-04, J: Source/test audit retains R36/R38/R39/R40 implementation and recognizes the real R38 child-process successful-drain proof. Updated stale history wording to its implemented normal-exit wait, with the separate 2 min budget stated explicitly. R44 requests durable timeout diagnostics and bounded child coverage; R43 requests forwarded first-run actions and shutdown-aware startup routing/presentation coverage. No application source changed or desktop result inferred; all relevant gates remain unchecked, estimates unchanged, R37 still deferred and macOS deferred.

- 2026-10-04, J: Final audit gate passes: full Windows Release build 0 warnings/errors; all five proper test projects 603 pass / 12 platform skips with strict native/application/local-history fixtures enabled. R43/R44 requests and corrected evidence can be published; estimates and actual desktop gates unchanged.

- 2026-10-04, J: Jaex authorizes the exact two-file R42 exception; J claims the production converter provider bridge, actual task-settings capability routing and pre-launch support check under H14. Complete the reviewed patch with proper existing coverage and the full Windows Release gate. R37 remains deferred, estimates/gates unchanged and macOS deferred.

- 2026-10-04, J: R42 claim publication gate passes: full Windows Release build 0 warnings/errors, 603 tests pass / 12 platform skips across all five test projects with strict fixtures. No code applied before claim push; estimates/desktop gates unchanged.

- 2026-10-04, J: Fulfilled authorized R42/H14 source integration after claim 8437f0b00: production converter callback uses its captured engine, direct jobs use actual settings/default fallback and backend launch rechecks cancellation/support. Exact approved two-file patch verified; existing controlled file-media/converter coverage and full Windows Release gate pass, 0 warnings/errors and 603 tests / 12 platform skips with strict fixtures. Removed R42 integration from the remaining Linux estimate basis without changing 90% or ticking actual desktop gates. R37 and macOS remain deferred.

- 2026-10-04, J: He explicitly keeps Microsoft Store startup (R37/W12/H12) deferred. J1/J8 Windows desktop completion is the current focus; R43 startup routing and R44 durable exit diagnostics still require backend coordination. Full Windows Release solution gate passes (0 warnings/errors, 603 tests / 12 platform skips across all five proper projects with strict fixtures). G1/G2 actual desktop gates and estimates remain unchanged; macOS remains deferred.

- 2026-10-04, J: M's reported policy override and G1 policy sign-off do not supersede those instructions here; Preserved M's report in the delegation status log. Windows desktop implementation is the immediate priority, Store startup remains deferred, and no application source, percentage or actual desktop sign-off changed.

- 2026-10-04, J: Claim approved R44 three-file history exception under Windows desktop completion before code. Exact isolated proposal reproduces the baseline timeout diagnostic loss and passes all eight History tests with synchronous final logging and bounded child fixtures. Full production Release claim gate passes (0 warnings/errors, 603 tests / 12 platform skips across all five proper projects with strict fixtures);

- 2026-10-04, J: Completed authorized R44 history shutdown diagnostics after claim 71122d4a8. The logger is flushed after the process-exit wait with the two-minute budget and logging settings preserved. A controlled asynchronous logger barrier reproduces the baseline loss and verifies the timeout warning after real exit; successful slow-write persistence remains covered and child waits/output reads are bounded. Full Windows Release solution gate passes (0 warnings/errors, 604 tests / 12 platform skips across all five proper projects with strict fixtures). Removed R44 from the remaining Windows implementation gaps; R43 and actual Windows 10/11 desktop sign-off remain open. Linux verification of the new case remains in its unchecked history gate.

- 2026-10-04, J: Claim the reviewed R43 shared startup/router and welcome presentation fix before production code under that instruction. Exact isolated source builds with 0 warnings/errors; all five proper projects pass 626 tests / 12 platform skips, with 22 new controlled/real first-start-exit cases. Production source is unchanged for the claim gate. Linux/Windows actual desktop sign-off and estimates remain unchanged;
- 2026-10-04, J: R43 completed in code after green claim 332fefdf5 under Jaex's instruction to choose the work that best advances Linux without further questions. Portable startup/command routing preserves initial-action order, welcome language selection before main-window construction, accepted-work warning deferral and shutdown-safe waiting/presentation. All 11 applied sources match the reviewed proposal. Full production Windows Release build 0 warnings/errors; five proper test projects 626 pass / 12 platform skips, including 22 new router/presentation/real first-start ExitShareX cases. R43 implementation is removed from the remaining Windows gaps; its actual Linux/Windows welcome/tray/language/shortcut-conflict flows remain in G1/J1/J8. No Linux execution is inferred from the unavailable local WSL or portable tests. Estimates/desktop gates unchanged;
- 2026-10-04, J: Claim B40 editor asynchronous image acquisition/publication lifetime under Linux-first J2/J8: guard picker/clipboard/drop/URL callbacks and expansion/replacement against owner changes and shutdown, with explicit Skia resource ownership and proper controlled tests. Record B41 selection identity across asynchronous clipboard clearing as a separate open bug. Current source remains the full green R43 implementation; verification/desktop gates and estimates stay unchanged. No backend/contracts/TFM/dependency/catalog/Scripts/macOS work;

- 2026-10-04, J: B40 asynchronous editor image imports complete in code under J2/J8 after claim 9c96e1258: stale owner/document/annotation results rejected, reads and placement waits canceled, image ownership explicit and existing URL timeout/normal import ordering retained. Twenty-seven controlled portable regressions cover actual shared acquisition/stream/publication helpers and synthetic Core/annotation ownership. Full Windows Release solution/all five projects pass (0 warnings/errors, 653 pass / 12 platform skips). B41 and actual Linux/Windows import/desktop interaction verification remain open; estimates and gates stay unchanged. No new Linux installation/dependency or macOS work;
- 2026-10-04, M: Owner decision by McoreD: Windows and Linux at 100% (code complete); G1 and G2 ticked; desktop and OS-version checks move to a field-testing list that continues alongside macOS. Migration-debt list in AGENTS.md fully ticked.
