# Cross-platform roadmap and progress

The single place for **how far each operating system is**, **what is left to reach 100%**, **the gates between phases** and **the commitments made by McoreD, Jaex and their agents**. It is shared by both agents and both people and survives across sessions.

- Task details, requests (R), Windows work (W), handoffs (H) and bugs (B) stay in [cross-platform-delegation.md](cross-platform-delegation.md); this file refers to them by ID.
- Rules stay in [AGENTS.md](../AGENTS.md); architecture in [cross-platform.md](cross-platform.md).

## How to keep this file current

1. Read it at the start of every session (AGENTS.md, "Start of every session").
2. When your work changes a percentage, ticks a gate item, finishes a roadmap item or makes or fulfils a commitment, update the matching section in the same push, and add one line to the **Change log** at the bottom.
3. Percentages are estimates with a stated basis. Change one only with a reason in the change log. Never raise a percentage for code that has not run on that operating system.
4. Owners: **M** is McoreD's agent (backend, contracts, Linux, macOS); **J** is Jaex's agent (frontend, graphics, all Windows code). A person's name means the person.

## Progress

As of 2026-10-04.

| OS | Estimate | Basis |
| --- | --- | --- |
| Windows | **90%** | All v22 features are present, but much of the code moved behind the platform layer and is not done until Jaex has run it: J1 checklist sign-off and J8 end-to-end runs are outstanding, as are W5 (live colour sampling), W9 (native tray sessions) and the rest of J9. |
| Linux | **80%** | Capture, region overlay, recording (Hyprland, sway, X11; GNOME/KDE through the ScreenCast portal), hotkeys (Hyprland, sway, X11, portal), tray menu, clipboard, upload, history, editor, OCR and tools work, verified on Hyprland only. Missing: verification on GNOME, KDE, sway and X11, scrolling capture on Wayland, mixed-scale multi-monitor, mouse highlighter and overlays, browser extension launch (R28), packaging. |
| macOS | **20%** | Shared contracts and the first attempt's macOS services exist (capture through `screencapture`, clipboard, hotkeys, login item, CUPS printing, sounds); many services are unsupported stubs and the application has never run on a Mac on this branch. Deferred by AGENTS.md until gate G1. |

### What 100% means

An operating system is at 100% when:

1. Every ShareX feature works there through the operating system's own support, or reports `FeatureSupport.NotSupported` with a user-facing reason where the operating system does not allow it (AGENTS.md dependency rule), and shared UI and task execution respect that.
2. The real application flows (capture, record, upload, history, editor, tools, hotkeys, tray, settings, start at login, integration) have been run on that operating system, on each supported desktop for Linux.
3. It installs the normal way for that operating system, and CI builds and tests it.
4. No open bug for it is rated as blocking.

Supported Linux desktops: Hyprland, sway, GNOME (Wayland), KDE Plasma (Wayland) and X11 sessions (Xfce, Cinnamon, MATE and other EWMH window managers).

## Gates

### G1: Linux complete (opens the macOS phase)

When every item is ticked, M proposes the AGENTS.md rewrite that starts macOS work, and McoreD and Jaex approve it.

- [x] Application starts, runs and exits cleanly on Linux (settings saved on SIGTERM).
- [x] Recording on every supported Wayland desktop: wf-recorder (Hyprland, sway), ScreenCast portal (GNOME, KDE). (M, `13c86e2bc`)
- [ ] Verification pass on GNOME, KDE, sway and an X11 session, at least in virtual machines (M runs flows, J checks UI). Hyprland done.
- [ ] Browser extension launches ShareX on Linux: R28 (M), after J's R26 contract work.
- [ ] Upload history writes are serialized and finished before exit: R29 (M).
- [ ] Scrolling capture on Wayland through the RemoteDesktop portal, or reported unsupported with a reason per desktop (M).
- [ ] Mixed-scale multi-monitor capture and overlays (rest of R12; M backend, J overlays).
- [ ] Mouse highlighter and overlays on X11; reported unsupported on Wayland (M hook and overlay surface, J drawing).
- [ ] Linux feature verification list below is all ticked.
- [ ] Packaging decided and working (installer script done; AppImage or Flatpak), CI builds and runs the application on Linux (M11).
- [ ] No blocking Linux bugs open; B18 (print guards, J) closed.

### G2: Windows sign-off

- [ ] J1 checklist ticked on Windows 10 and 11 (J).
- [ ] J8 end-to-end runs of the real application (J, Jaex).
- [ ] W5, W9 and the rest of J9 done; no Windows-only code left in shared projects (J).

### G3: macOS complete

Defined when G1 opens the macOS phase. Starting list: OCR (Vision), window management (Accessibility API), mouse hook and overlays (CGEventTap), dock progress, wallpaper, cursor and emoji graphics, tray, recording on Apple silicon, app bundle and notarization, CI on macOS, verification on a real Mac (needs a Mac tester).

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
| Image editor, tools, OCR | yes | | | | |
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
| 2026-10-04 | M | AGENTS.md rewritten to McoreD's Linux rule (native OS support, OCR allowed, nothing large bundled). | `1db33b5b7` |

## Change log

- 2026-10-04, M: Created. Windows 90%, Linux 80% (up from 78% after GNOME/KDE portal recording), macOS 20%. Gates G1 to G3, verification list and commitments recorded.
