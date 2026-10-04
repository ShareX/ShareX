# Cross-platform roadmap and progress

The single place for **how far each operating system is**, **what is left to reach 100%**, **the gates between phases** and **the commitments made by McoreD, Jaex and their agents**. It is shared by both agents and both people and survives across sessions.

- Task details, requests (R), Windows work (W), handoffs (H) and bugs (B) stay in [cross-platform-delegation.md](cross-platform-delegation.md); this file refers to them by ID.
- Rules stay in [AGENTS.md](../AGENTS.md); architecture in [cross-platform.md](cross-platform.md).

Jaex's current scope is authoritative: Windows and Linux only; Linux HDR and OCR are excluded, and OCR remains Windows-only. Microsoft .NET is the only additional installation Linux users may be required to make. Already included libraries and existing desktop APIs are allowed; features requiring other installations must remain unsupported. R20/R22 must align carried-over backends, capability reasons and packaging with these rules before G1 can pass. macOS feature work remains deferred.

## How to keep this file current

1. Read it at the start of every session (AGENTS.md, "Start of every session").
2. When your work changes a percentage, ticks a gate item, finishes a roadmap item or makes or fulfils a commitment, update the matching section in the same push, and add one line to the **Change log** at the bottom.
3. Percentages are estimates with a stated basis. Change one only with a reason in the change log. Never raise a percentage for code that has not run on that operating system.
4. Owners: **M** is McoreD's agent (backend, contracts, Linux, macOS); **J** is Jaex's agent (frontend, graphics, all Windows code). A person's name means the person.

## Progress

As of 2026-10-04.

| OS | Estimate | Basis |
| --- | --- | --- |
| Windows | **90%** | Estimate retained from M's initial roadmap. W5, W9 and the R24-R27 native migrations are implemented and covered by Windows fixtures; real isolated startup and loopback upload/history runs also pass. J1 Windows 10/11 desktop sign-off and full J8 flows remain unverified; R29 fixes the B24 connection collision in code; R38 history process-exit drain guarantees and native fixture cleanup issue B27 remain open. |
| Linux | **80%** | Estimate retained from M's initial roadmap, pending the agreed-scope audit. M reports capture, overlays, recording, hotkeys, tray, clipboard, uploads, history, editor and tools on Hyprland; other desktop flows are unverified. Linux OCR/HDR are excluded. R20/R22 policy compliance, GNOME/KDE/sway/X11 verification, scrolling capture, mixed scaling, mouse highlighter/overlays, R38 history process-exit drain guarantees and packaging remain outstanding. R28 browser launch is implemented and M verified it on Hyprland; other desktop flows remain unverified. |
| macOS | **20%** | Shared contracts and the first attempt's macOS services exist (capture through `screencapture`, clipboard, hotkeys, login item, CUPS printing, sounds); many services are unsupported stubs and the application has never run on a Mac on this branch. Deferred by AGENTS.md until gate G1. |

### What 100% means

An operating system is at 100% when:

1. Every feature in the agreed scope works through already included libraries or existing desktop APIs, or reports `FeatureSupport.NotSupported` with a user-facing reason for platform limits or excluded dependencies. Shared UI and task execution respect support. Linux HDR/OCR are excluded; no extra Linux feature installation beyond Microsoft .NET is required or recommended.
2. The real application flows (capture, record, upload, history, editor, tools, hotkeys, tray, settings, start at login, integration) have been run on that operating system, on each supported desktop for Linux.
3. It installs the normal way for that operating system, and CI builds and tests it.
4. No open bug for it is rated as blocking.

Supported Linux desktops: Hyprland, sway, GNOME (Wayland), KDE Plasma (Wayland) and X11 sessions (Xfce, Cinnamon, MATE and other EWMH window managers).

## Gates

### G1: Linux complete (opens the macOS phase)

When every item is ticked, M proposes the AGENTS.md rewrite that starts macOS work, and McoreD and Jaex approve it.

- [x] Application starts, runs and exits cleanly on Linux (settings saved on SIGTERM).
- [ ] Recording verified on every supported Wayland desktop. Implementations exist (`13c86e2bc`); M reports Hyprland verification, while GNOME/KDE/sway desktop verification and R22 dependency compliance remain outstanding.
- [ ] R20 Windows-only OCR and R22 no-extra-installations policy implemented in backends, capability reasons, execution guards and packaging.
- [x] Individual DirectShow discovery and Windows recorder-device download are guarded by capabilities: R32 contract (M), W11 Windows parity review and H11 guarded UI (J, 2026-10-04), with eight new proper tests.
- [ ] Verification pass on GNOME, KDE, sway and an X11 session, at least in virtual machines (M runs flows, J checks UI). Hyprland done.
- [x] Browser extension launches ShareX on Linux: R28 (M), after J's R26 contract work; M verified cold/warm framed payloads on Hyprland (a516a5b5c), other desktop flows remain in the verification pass.
- [ ] Upload history writes are serialized and finished before exit: R29 serialization and R34 sealed queue/deferred connection close are implemented; R38 still requires the application lifetime to await accepted writes through process exit.
- [x] Unsupported mouse-highlighter automatic startup is skipped while retaining saved flags: R31 (M); controlled coverage added, actual desktop startup remains in the verification pass.
- [ ] Expected desktop shortcut conflicts do not block startup browser/CLI actions while preserving interactive Windows warnings: R36 hotkey warnings and R39 first-run welcome ordering (M startup/policy, J presentation).
- [ ] Scrolling capture support and failure reasons verified per desktop: B31 frontend and R33 backend guards are implemented; B33 frontend alignment with FailureReason and supported auto-top inputs is complete, with four portable input-combination cases. The per-desktop support table is in docs/cross-platform.md; actual desktop verification remains open.
- [ ] Mixed-scale multi-monitor capture and overlays (rest of R12; M backend, J overlays). B35 recording/scrolling frames and B38 ruler sizing now use actual render DPI and preserve returned/native bounds; B35 has 15 portable geometry cases and existing ruler capture regressions pass. Actual desktop verification and backend coordinate mapping remain open.
- [ ] Mouse highlighter and overlays on X11; reported unsupported on Wayland (M hook and overlay surface, J drawing). M: hook and overlay done and tested on a real X server. B39 synthetic Skia/buffer tests and a hidden Windows DIB fixture pass; actual drawing verification on a composited X11 desktop remains (VM).
- [ ] Linux feature verification list below is all ticked.
- [x] Packaging decided and working: self-contained tar.gz with installer (Flatpak deferred, sandbox blocks capture paths); CI workflow builds, tests, packages and runs the app headless, started by hand while branch workflows are paused (M11).
- [ ] No blocking Linux bugs open. B18 print guards are fixed in code (`J5`, 2026-10-04), with two portable regression tests and a reason translated in all 28 cultures; backend policy and desktop printing verification remain open.

### G2: Windows sign-off

- [ ] J1 checklist ticked on Windows 10 and 11 (J).
- [ ] J8 end-to-end runs of the real application (J, Jaex).
- [ ] W5, W9 and the rest of J9 done; no Windows-only code left in shared projects (J). Microsoft Store startup WinRT/type aliases remain in shared code and require R37 (M contract/routing, J Windows implementation/UI).

### G3: macOS complete

Defined only if a macOS phase is approved after G1. Possible areas include window management, mouse hooks and overlays, dock progress, wallpaper, cursor and emoji graphics, tray, recording, packaging and real Mac verification. This list does not authorize implementation now; OCR remains Windows-only under the current instructions.

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
| Image editor and tools (OCR excluded) | yes | | | | |
| Start at login, file manager entries, file types | yes (autostart entry launches -silent to tray; .sxcu opens with ShareX through GIO) | | | | |
| Secrets survive restart | yes (key file 0600, decrypts in a new process) | | | | |
| Sounds, printing to a real printer | sounds yes (pw-play); no printer configured on the test machine | | | | |
| Background remover | yes (ONNX Runtime CPU, u2netp model: subject kept, background transparent) | | | | |
| Multi-monitor | single monitor on the test machine | | | | |

## Commitments

Open promises. Move them to "Kept" with the commit when fulfilled.

| Date | Who | Commitment |
| --- | --- | --- |
| 2026-10-04 | M | Tell McoreD when G1 and G2 are met and propose the AGENTS.md rewrite that starts the macOS phase, with the G3 list as its tasks. |
| 2026-10-04 | M | Keep this file current: progress, gates, verification list and change log, in the same push as the work. |
| 2026-10-04 | M | Run the GNOME, KDE, sway and X11 verification pass and record the results in the verification list. |

### Kept

| Date | Who | Commitment | Commit |
| --- | --- | --- | --- |
| 2026-10-04 | M | GNOME and KDE screen recording through the ScreenCast portal. | `13c86e2bc` |
| 2026-10-04 | M | R28 Linux application launch and R29 connection serialization/normal queue flush. R34 sealed library draining is implemented; R38 application process-exit draining remains open. | `a516a5b5c`, `288f7a06f` |
| 2026-10-04 | M | Historical policy edit; superseded by Jaex's explicit Windows-only OCR/no-extra-installations instructions and subsequent AGENTS.md corrections. | `1db33b5b7` (superseded) |

## Change log

- 2026-10-04, M: Created. Windows 90%, Linux 80% (up from 78% after GNOME/KDE portal recording), macOS 20%. Gates G1 to G3, verification list and commitments recorded.
- 2026-10-04, J: Integrated this shared roadmap while publishing B18. Print-window support/callback guards and 28-culture UI reason are complete; full Windows Release build has 0 warnings/errors and all tests pass (459 pass, 8 Unix-only skips). Kept the estimates unchanged and clarified their limits. Removed Linux OCR from the supported scope, restored the authoritative no-extra-installations/Windows-only OCR requirements, and added R20/R22 as explicit G1 gates. Unticked the broad recording verification claim because M's own status says GNOME/KDE desktops remain untested; Hyprland implementation results stay recorded. W5/W9/R24-R27 native code and fixture coverage do not substitute for Windows 10/11 desktop sign-off, and B24/R29 remains open. No macOS work is authorized.
- 2026-10-04, J: R13 frontend capture-option guards are complete under J5: unavailable transparency, shadow/offset, client area, taskbar hiding and HDR settings stay disabled without changing saved values, and setter callbacks recheck support. Supported Windows controls retain their layout and bindings. The desktop-availability reason is translated in all 28 cultures without package-install advice. Full Windows Release build has 0 warnings/errors; all tests pass (459 pass, 8 Unix-only skips), and repository-wide translation validation passes. Existing window/control snapping guards complete the R13 UI handoff. Estimates and real Windows 10/11/Linux desktop verification gates stay unchanged; R20/R21/R22 and R28/R29 remain outstanding, and macOS work remains deferred.
- 2026-10-04, J: B25/J5 sound-setting guards are complete in code: unavailable playback/custom-WAV cards stay disabled, saved flags/paths remain intact, and setters/file-picker callbacks recheck capability. The UI reason is translated in all 28 cultures. Full Windows Release build: 0 warnings/errors; all tests pass (459 pass, 8 Unix-only skips), and full translation validation passes. Filed R30 for a desktop-icon visibility capability so its unsupported Linux setting can be guarded without OS checks or side-effect probes. Estimates and actual desktop gates remain unchanged; backend policy and other open requests are unresolved.
- 2026-10-04, J: B26/J5 pin chooser capture guards and busy lifetime are fixed in code, with five synthetic source-selection cases and a 28-culture reason. Clipboard/file choices remain available through the toolkit; the separate platform-helper clipboard capability must not disable them. Final full Windows Release gate passes (0 warnings/errors, 464 pass, 8 Unix-only skips), and full translation validation passes. Earlier gates reproduced B24's SQLite shutdown race and exposed B27's generated-executable cleanup denial after the launch assertions passed; five isolated launch checks and the final gate pass without weakening assertions. Both issues remain open. Estimates and real Windows 10/11/Linux desktop gates are unchanged; backend policy/handoffs and macOS deferral remain in force.
- 2026-10-04, J: Hardened B27 native fixture cleanup with a bounded five-second budget, exact generated-temp-root validation and diagnostics that preserve native errors/records after partial deletion. Four deterministic cleanup regressions pass, including a lock beyond the former two-second budget and a persistent lock that still fails. The old generated executable was removable later, but its original denial cause remains unproven and B27 stays open. Native launch/job/handle assertions are unchanged. Full Windows Release gate passes (0 warnings/errors, 468 pass, 8 Unix-only skips). B24/R29 and real Windows 10/11/Linux verification remain open; estimates and gates are unchanged.
- 2026-10-04, J: B28/J5 screen-colour-picker task/menu, settings and default colour-dialog/editor pipette guards are complete, with one shared reason translated in all 28 cultures. Callbacks and setters recheck capture availability; saved formats, manual/clipboard colour selection and explicit host picker callbacks remain available. Full Windows Release build passes with 0 warnings/errors; all tests pass (468 pass, 8 Unix-only skips), and full translation validation passes. No desktop gate or estimate changed. B24/R29, B27 original denial cause and backend policy/desktop verification remain open; macOS stays deferred.
- 2026-10-04, J: B29/J5 mouse-highlighter settings, recording flag and start guards are complete with a reason in all 28 cultures. Manual Stop/stop-after-recording remain available after capability loss, saved unsupported options remain intact and three proper synthetic action regressions pass. Full final Windows Release gate: 0 warnings/errors, 471 pass, 8 Unix-only skips; full translation validation passes. Added R31 to G1 because automatic startup bypasses the task capability guard and still needs an M-owned backend fix. Estimates and actual Windows 10/11/Linux desktop verification gates remain unchanged; B24/R29, B27 original cleanup cause and other backend requests stay open, and macOS stays deferred.
- 2026-10-04, J: B30/J5 recording task-settings and FFmpeg recording-mode availability/action guards are complete. Saved options remain intact when unsupported, async discovery/file-picker callbacks recheck support, and general conversion mode remains available independently of capture. Four proper synthetic regressions and a 28-culture reason pass. Full Windows Release gate: 0 warnings/errors, 475 pass, 8 Unix-only skips; full translation validation passes. Added R32 to G1 for per-device DirectShow/download capabilities so supported Linux recording cannot expose Windows dependency setup; overall guards do not close that contract/policy gap. Estimates and actual Windows 10/11/Linux desktop verification gates remain unchanged; other requests and B24/B27 causes stay open, and macOS stays deferred.
- 2026-10-04, M: R29/B24 fixed: history writes are serialized and flushed before the database closes, with a new ShareX.HistoryLib.Tests project. G1 history item ticked.
- 2026-10-04, M: R28 done: the browser extension host launches ShareX on Linux (cold and warm start verified with framed payloads on Hyprland). G1 browser item ticked; R28/R29 commitment kept.
- 2026-10-04, M: Linux dependency policy reconciled in AGENTS.md on McoreD's instruction. R20 cancelled; R22 reframed (core flows need only .NET and desktop APIs; optional features may use distribution programs). G1 policy item reworded.
- 2026-10-04, M: R22 first slice: no install commands in reasons, Secret Service over D-Bus (no secret-tool), scale-correct portal crop and window capture, portal screenshot timeout. grim recognised as the wlroots desktops' own screenshot component.
- 2026-10-04, J: B31/J5/J4 scrolling-capture frontend action, settings and selection lifetime guards are complete, with seven controlled async regressions and one reason translated in all 28 cultures. Support is rechecked after hiding/selection, close prevents later capture/restoration, busy state prevents competing actions, saved invalid/unavailable options stay intact, Stop remains available after support loss and completed-image actions remain usable. Auto-top checks its actual keyboard/window-scroll prerequisites. R33 still requires backend checks after its own delays and safe selector cancellation; no actual desktop scrolling verification is claimed. Final full Windows Release gate: 0 warnings/errors, 482 pass, 8 Unix-only skips; full translation validation passes. B24/B27 did not recur, and their original causes remain open. Estimates and Windows 10/11/Linux desktop gates remain unchanged; backend policy and other open requests remain outstanding, and macOS stays deferred.
- 2026-10-04, J: Integrated M's R28/R29 code and Hyprland browser-launch report while rebasing B31, retaining both agents' history. Restored Jaex's direct Windows-only OCR and no-extra-installations scope after d60f56b5d reintroduced optional external programs/Linux OCR; R20 remains required and R22 retains M's claim with the authoritative scope. R29 connection serialization is retained, but source inspection found ignored incomplete flush results and an open queue admission boundary; R34 requests slow/late-write regression coverage and a complete drain before disposal, so the history G1 gate remains unchecked. Other desktop verification and estimates remain unchanged, and macOS stays deferred. Combined-source full Release verification follows before publishing.
- 2026-10-04, J: Final combined-source full Windows Release build/all five test projects pass (0 errors, 492 pass, 9 Unix/Linux-only skips), including strict native/application/loopback-history checks. Fixed only the incoming failing absolute-path fixture under the documented one-line exception; R35 records related validation cases that reject the wrong field on Windows. The first combined build's M-owned xUnit1031 async warning is recorded with R34; the final incremental build reports 0 warnings without a suppression. Translation validation remains green with unchanged catalogs. R29 is reviewed with its slow/late draining gap explicit, while R28 code and M's Hyprland verification stay recorded. Windows-only OCR/no-extra-installations scope is restored; actual desktop gates, estimates and macOS deferral remain unchanged.
- 2026-10-04, J: Integrated R22 first-slice source improvements (8cac48df6) during the final B31 rebase, retaining both agents' history and Jaex's authoritative Windows-only OCR/no-extra-installations rules. Final full Windows Release gate passes: 0 errors, one existing test analyzer warning tracked in R34, 485 pass / 10 Unix/Linux-only skips across all five test projects, including strict application/history/native fixtures. Test totals changed with M's retired install-command cases and new credential tests. Catalogs stay unchanged from full translation validation. Scope alignment, R34 history draining, other backend requests and actual desktop verification stay open; no estimates or desktop gate changed and macOS remains deferred.
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

- 2026-10-04, J: Claim B36 converter UI picker/job/settings lifetime with proper controlled tests before code. R41 requests independent conversion/trimming/thumbnail capabilities and R22 backend execution guards; desktop recording Support must not disable independent Windows file tools. No actual desktop gate/estimate changes; backend scope/startup/history-exit/Store and desktop requests remain open, macOS deferred.

- 2026-10-04, J: B36 converter UI picker/job/settings lifetime is fixed in code, with twelve proper controlled tests and live argument parity. Late storage/progress/results are ignored after close, settings stay intact during busy/closed callbacks, cancellation resources remain valid until actual completion, and stale progress cannot reach another job. Full Windows Release gate: 0 warnings/errors, 554 pass / 12 Unix/Linux-only skips across all five test projects, strict native/application/local-history fixtures enabled. R21 portable Tools test target and R41/R22 independent file-media backend capabilities/scope remain outstanding. Actual Windows 10/11/Linux desktop gates and estimates unchanged; macOS deferred.

- 2026-10-04, J: Claim B37 pin chooser closed-selection and post-selector capability guards, with pending-close completion coordination and proper controlled tests. Clipboard/file sources remain independent of capture support; backend selector cancellation is unavailable and not claimed. Real Windows 10/11/Linux pin/overlay verification, R12 and other backend scope requests remain open; estimates unchanged and macOS deferred.

- 2026-10-04, J: B37 pin chooser post-selector support and closed-action/results/restoration guards are fixed in code. Pending close waits for actual selection completion before releasing the window; successful pin/location/order and independent file/clipboard paths are preserved. Ten new proper controlled cases (15 pin cases total) pass. Full Windows Release gate: 0 warnings/errors, 564 pass / 12 Unix/Linux-only skips across all five test projects with strict application/native/local-history fixtures enabled. No selector cancellation or actual desktop verification inferred; R21, R12/backend scope and other requests remain open. Estimates and Windows 10/11/Linux desktop gates unchanged; macOS deferred.

- 2026-10-04, J: Claim B38 ruler overlay actual render scaling and ScalingChanged geometry refresh. Preserve returned capture bounds and pixel measurements; existing proper ruler/capture coverage plus the full Windows Release gate will verify compilation/regression scope, while actual mixed-monitor Windows 10/11/Linux verification and R12 remain open. No estimates/desktop gate changes or macOS work.

- 2026-10-04, J: B38 ruler actual render scaling and scaling-change geometry refresh fixed in code, retaining returned capture bounds and pixel measurements with closed queued-update guards. Seven existing proper ruler capture regressions and full Windows Release gate pass: 0 warnings/errors, 564 pass / 12 Unix/Linux-only skips across all five test projects, strict native/application/local-history fixtures enabled. No new scalar mirror tests or Scripts. Actual mixed-monitor Windows 10/11/Linux overlay placement and R12 backend coordinates remain unverified; the mixed-scale G1 item stays unchecked, estimates unchanged and macOS deferred.

- 2026-10-04, J: Claim B39 mouse highlight drawing/overlay buffer verification and disposed renderer/Windows native surface guards. Fixtures use synthetic state and hidden Windows buffer creation with presentation intercepted, without user input or desktop capture. Full actual Windows 10/11/Linux mouse-highlighter drawing/compositor verification remains open; no estimates/desktop gate changes and macOS deferred.

- 2026-10-04, J: B39 mouse highlight renderer/native buffer verification and disposed surface guards complete in code. Six proper cases cover synthetic BGRA/alpha, clipping, fades/ripples/crosshairs, reuse/reallocation/clearing and ownership; the hidden native Windows DIB case executed with presentation intercepted. Full Windows Release gate: 0 warnings/errors, 570 pass / 12 Unix/Linux-only skips across all five test projects, strict native/application/local-history fixtures enabled. Actual Windows 10/11 hook/click-through/visible drawing and composited Linux desktop verification remain unverified, so J1/G1 desktop items stay unchecked and estimates unchanged. R21 portable Tools test target, backend scope/coordinates/startup/history-exit/Store and other requests remain open; macOS deferred.
