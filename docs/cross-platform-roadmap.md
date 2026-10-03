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
| Windows | **90%** | Estimate retained from M's initial roadmap. W5, W9 and the R24-R27 native migrations are implemented and covered by Windows fixtures; real isolated startup and loopback upload/history runs also pass. J1 Windows 10/11 desktop sign-off and full J8 flows remain unverified; intermittent history shutdown bug B24/R29 and native fixture cleanup issue B27 remain open. |
| Linux | **80%** | Estimate retained from M's initial roadmap, pending the agreed-scope audit. M reports capture, overlays, recording, hotkeys, tray, clipboard, uploads, history, editor and tools on Hyprland; other desktop flows are unverified. Linux OCR/HDR are excluded. R20/R22 policy compliance, GNOME/KDE/sway/X11 verification, scrolling capture, mixed scaling, mouse highlighter/overlays, browser launch (R28), history shutdown (R29) and packaging remain outstanding. |
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
- [ ] Verification pass on GNOME, KDE, sway and an X11 session, at least in virtual machines (M runs flows, J checks UI). Hyprland done.
- [ ] Browser extension launches ShareX on Linux: R28 (M), after J's R26 contract work.
- [ ] Upload history writes are serialized and finished before exit: R29 (M).
- [ ] Unsupported mouse-highlighter automatic startup is skipped while retaining saved flags: R31 (M).
- [ ] Scrolling capture on Wayland through the RemoteDesktop portal, or reported unsupported with a reason per desktop (M).
- [ ] Mixed-scale multi-monitor capture and overlays (rest of R12; M backend, J overlays).
- [ ] Mouse highlighter and overlays on X11; reported unsupported on Wayland (M hook and overlay surface, J drawing).
- [ ] Linux feature verification list below is all ticked.
- [ ] Packaging decided and working (installer script done; AppImage or Flatpak), CI builds and runs the application on Linux (M11).
- [ ] No blocking Linux bugs open. B18 print guards are fixed in code (`J5`, 2026-10-04), with two portable regression tests and a reason translated in all 28 cultures; backend policy and desktop printing verification remain open.

### G2: Windows sign-off

- [ ] J1 checklist ticked on Windows 10 and 11 (J).
- [ ] J8 end-to-end runs of the real application (J, Jaex).
- [ ] W5, W9 and the rest of J9 done; no Windows-only code left in shared projects (J).

### G3: macOS complete

Defined only if a macOS phase is approved after G1. Possible areas include window management, mouse hooks and overlays, dock progress, wallpaper, cursor and emoji graphics, tray, recording, packaging and real Mac verification. This list does not authorize implementation now; OCR remains Windows-only under the current instructions.

## Linux feature verification list

Tick with the desktop and commit when verified. Hyprland results are from 2026-10-03/04.

| Flow | Hyprland | sway | GNOME | KDE | X11 |
| --- | --- | --- | --- | --- | --- |
| Full screen, region, active window capture | yes | | | | |
| Region overlay at 125% scaling, window snapping | yes | | | | |
| Screen recording, GIF | yes (wf-recorder, portal) | | | | |
| Hotkeys fire | yes | | | | |
| Tray icon and menu | yes (D-Bus checked) | | | | |
| Clipboard: copy image, text, URL | yes (text, URL) | | | | |
| Upload from clipboard, drag and drop, watch folders | | | | | |
| Upload to a local test server, history | yes | | | | |
| Image editor and tools (OCR excluded) | yes | | | | |
| Start at login, file manager entries, file types | | | | | |
| Secrets survive restart | | | | | |
| Sounds, printing to a real printer | | | | | |
| Background remover | | | | | |
| Multi-monitor | | | | | |

## Commitments

Open promises. Move them to "Kept" with the commit when fulfilled.

| Date | Who | Commitment |
| --- | --- | --- |
| 2026-10-04 | M | Tell McoreD when G1 and G2 are met and propose the AGENTS.md rewrite that starts the macOS phase, with the G3 list as its tasks. |
| 2026-10-04 | M | Keep this file current: progress, gates, verification list and change log, in the same push as the work. |
| 2026-10-04 | M | Do R28 (Linux application launch for the browser extension) and R29 (history write serialization). |
| 2026-10-04 | M | Run the GNOME, KDE, sway and X11 verification pass and record the results in the verification list. |

### Kept

| Date | Who | Commitment | Commit |
| --- | --- | --- | --- |
| 2026-10-04 | M | GNOME and KDE screen recording through the ScreenCast portal. | `13c86e2bc` |
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
