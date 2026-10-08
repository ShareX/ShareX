# macOS retest procedure (round 2)

A second pass on a Mac after the fixes for the round 1 report, `macos-test-report-2026-10-05.md` (findings F1 to F13). It checks each fix, runs the steps that could not be run last time, and tries the new release packaging and Vision OCR. The full procedure stays in [20261004-macos-test-procedure.md](20261004-macos-test-procedure.md); this file refers to its steps by number.

Use the same report format as last time: a stable ID per finding, severity, step, evidence and suspected cause. Results are **pass**, **fail**, **partial** or **skipped**. New findings continue the numbering from **F14**. Keep the log when something fails: `~/Documents/ShareX/Logs/ShareX-Log-<date>.txt`.

Uploads go only to the local test server from step 5 of the full procedure. Never upload to a third-party host.

**Physical keys and clicks.** Several fixes are about keyboard focus and system shortcuts, which synthetic events can bypass. Where a step says *by hand*, the person at the Mac must press the keys or drag with the real mouse, and the report should say it was done that way. Last time, Cmd+Q and Cmd+Shift+3 could not be sent by the test tool.

## 0. Before you start

1. Quit ShareX. Delete `/Applications/ShareX.app` and the five empty `NativeMessagingHosts` folders left from round 1 (`~/Library/Application Support/<browser>/NativeMessagingHosts/`, if they are still empty).
2. Keep `~/Documents/ShareX` (settings, the "Local test" uploader). Restore the default hotkeys if they were changed.
3. Note the machine (CPU, macOS version, displays) in the report header. If an **Intel Mac** or a **second monitor** is available, use it this time; both were skipped in round 1.

## 1. Install from the disk image

Build at commit `1dc43905d` or later on `cross-platform-v2`.

1. Either download the `sharex-macos-arm64` (or `sharex-macos-x64`) artifact from the latest successful **Cross-platform** workflow run on GitHub, or build locally:

   ```
   git pull
   Scripts/package-macos.sh arm64
   ```

   A local build writes both `artifacts/ShareX-<version>-macos-arm64.zip` and `.dmg`.
2. Open the `.dmg`. Check: the window shows **ShareX.app** and an **Applications** shortcut. Drag ShareX.app onto Applications, then eject the disk image.
3. If the build came from GitHub it is quarantined and signed ad hoc, so macOS says it is damaged. Run `xattr -cr /Applications/ShareX.app` as in step 1.3 of the full procedure, and record that this was needed. (A Developer ID signed build will not need it; none exists yet.)
4. Check the browser host is inside the bundle (F3):

   ```
   ls /Applications/ShareX.app/Contents/MacOS/ShareX_NativeMessagingHost*
   ```

   Expected: `ShareX_NativeMessagingHost`, `ShareX_NativeMessagingHost.dll` and its `.deps.json` / `.runtimeconfig.json`.
5. Start ShareX. If the Screen Recording permission was reset by the reinstall, grant it again and restart (full procedure, step 2).

## 2. Fix checks

### F1. Esc cancels the region capture (was High)

1. *By hand:* click another application (Finder) so it is in front. Start Region capture from the ShareX menu bar icon. Press **Esc** without clicking the overlay first. The overlay closes and no capture is taken.
2. Repeat, starting with the default hotkey (**Option+Shift+4**) while Finder is in front.
3. Repeat once more after dragging part of a region, then pressing Esc.
4. Open a capture in the image viewer and press Esc: the viewer closes. Open a context menu in ShareX and press Esc: the menu closes.

### F2. Region recording on Retina records the right area (was High)

1. Record a region to MP4 (full procedure 4.2) around a window with clear edges. Play it: the video shows exactly the selected area, at full Retina resolution (about twice the size in points).
2. Record a region as GIF (4.3) and check the same.
3. If a second monitor is available, especially a non-Retina one, record a region on it (4.4).
4. Record the full screen once and check nothing is cut off.

### F3. Browser extension (was High)

1. Application settings > Integration: turn on browser extension support.
2. Install the ShareX extension in a Chromium browser (Chrome, Brave, Edge or Vivaldi).
3. With the local test server running and "Local test" as the destination, right-click an image on a web page > upload it with ShareX. ShareX uploads it and copies the URL.
4. Repeat while ShareX is **not** running: the extension must start it.

### F4. Conflicts with macOS shortcuts (was Medium)

1. Hotkey settings: the default hotkeys show **⌥⇧3**, **⌥⇧4** and so on, with symbols, not "Alt" or "Win".
2. Assign **⌘⇧3** to a ShareX task. ShareX must show it as **in use** (a warning state on the hotkey) and must not register it.
3. *By hand:* press ⌘⇧3. Only macOS takes its screenshot; ShareX does nothing.
4. In System Settings > Keyboard > Keyboard Shortcuts > Screenshots, turn off "Save picture of screen as a file" (⌘⇧3). Wait about 5 seconds, then assign ⌘⇧3 in ShareX again: it registers now and works *by hand*. Turn the macOS shortcut back on afterwards.
5. Assign a free combination with Command (for example **⌃⌘1**) and check it fires from another application, *by hand*.

### F5. Application menu (was Medium)

1. The menu next to the Apple menu is **ShareX** (not "Avalonia") and contains **About ShareX**, **Settings…** with **⌘,** and **Quit ShareX** with **⌘Q**.
2. About opens ShareX's About window. Settings opens Application settings. *By hand:* ⌘, does the same.
3. The labels follow the UI language (last time it was French).

### F6. Quit really quits (was High)

1. *By hand:* press **⌘Q** with the main window in front. The process ends within a few seconds (check with `pgrep -x ShareX`), and the log ends with `ShareX closed.` and no `Unhandled exception`.
2. Start ShareX, then choose **Quit ShareX** from the application menu. Same result.
3. Start ShareX, right-click its Dock icon > **Quit**. Same result.
4. Close the main window with the red button: ShareX keeps running in the menu bar (closing the window is not quitting).
5. Settings changed just before quitting are kept after the restart.

### F7. Editor uses Command (was Medium)

*By hand*, in the image editor:

1. **⌘Z** undoes, **⌘⇧Z** (or ⌘Y) redoes, **⌘S** saves, **⌘C** / **⌘V** copy and paste a shape.
2. The editor menus show ⌘ shortcuts, not ^ or Ctrl.
3. Ctrl-based shortcuts no longer act as Command (Ctrl+Z does nothing or does what macOS normally does).
4. Tool keys without modifiers (the single letters shown in the toolbar tooltips) still work.

### F8. Cursor position in region captures (was Medium)

1. Turn on "Show cursor" in task settings.
2. *By hand:* place the pointer over a recognisable spot, press the Region capture hotkey, then drag a region that contains that spot **starting elsewhere**.
3. The captured image shows the pointer where it was when the hotkey was pressed, at the right size, not at the drag start.

### F9. Window shadow (Low, unchanged by design)

No change: as in ShareX on Windows, the shadow option only applies when "Capture window with transparency" is on. Check it there: Task settings > Capture, turn on **Capture window with transparency**, then capture a window with **Capture window with shadow** on, then off. With it on, the image has transparent corners and the shadow; with it off, no shadow.

### F10. Scrolling capture without Accessibility (was Low)

1. In System Settings > Privacy & Security > Accessibility, remove ShareX (or turn it off).
2. Start Scrolling capture and select a region. ShareX must come to the front and show a message saying to allow ShareX under Accessibility. The status line in the scrolling capture window says the same. There is no blank window without explanation.
3. Allow ShareX, restart it, and run a real scrolling capture on a long web page or Finder list. Check the stitched image has no repeated or missing bands. (Not run in round 1.)

### F11. Windows-only entries (was Low)

1. The menu bar icon menu has no "Restart as administrator".
2. Send to and Explorer-specific options are disabled with a short reason as their tooltip.
3. Note any other Windows-only wording still visible, with screenshots, as a new finding.

### F12. Browser host registration (was Low)

1. With browser extension support on, list `~/Library/Application Support/*/NativeMessagingHosts/com.getsharex.sharex.json`.
2. Turn it off. The manifests are removed **and** their now empty `NativeMessagingHosts` folders are removed too. Folders that hold other applications' manifests stay.
3. Note: manifests are still written for each supported Chromium browser even if it is not installed; this is harmless and not a failure.

### F13. History preview (was Low)

1. Open History with the uploads made against the local test server (which does not serve the image back).
2. The log gets at most one short line per item, without a stack trace.

## 3. New since round 1

### OCR with Vision

OCR is now available on macOS through Apple's Vision framework. It is no longer disabled with a "Windows only" reason.

1. Tools > OCR, select a region containing text, for example this document in a browser. The recognised text appears and can be copied.
2. Try text in another language (for example French) and a mixed light/dark background.
3. Turn on the after-capture task "Recognize text (OCR)" and take a region capture. The text is recognised.

### Updates

1. Settings > Updates (or About): checking for updates must not say "update check failed" because of missing Windows files. No macOS release exists yet, so "no update found" or a release-page link is expected. Record the exact message.

## 4. Steps not run in round 1

Run these from the full procedure:

| Step | What |
| --- | --- |
| 3.7, 4.4 | Second monitor capture and recording, if available. |
| 8.2 | Close the main window, click the Dock icon *by hand*: the window comes back. |
| 8.4 | *By hand:* drag an image onto the ShareX Dock icon. It is uploaded. |
| 8.5 | *By hand:* Finder > right-click a file > Open With > ShareX. It is uploaded. |
| 9.1 | Change the theme and the language, quit with ⌘Q, restart: both are kept. |
| 9.2, 9.3 | Start at login on: log out and back in, ShareX starts in the menu bar. Off: it does not. |
| 10.2 | Finder right-click > **Quick Actions** > Upload with ShareX (last time only the Services menu was used). |
| 12.1 | Final quit with ⌘Q; the log has no `Unhandled exception`. |

If an Intel Mac is available, repeat sections 1 and 2 with the `x64` build there; at least F1, F2, F5 and F6.

## 5. Report

1. Write the report like `macos-test-report-2026-10-05.md`, with a front matter block (commit, build source: CI artifact or local, machine, displays, UI language, tester), a summary table and results by step. Name it `macos-test-report-<date>.md`.
2. For each F1 to F13, give the round 2 result in one line: **fixed**, **still fails** (with evidence) or **not checked**.
3. List what was done by hand and what was done with synthetic events.
4. Describe the state left on the Mac: settings changed, Accessibility and Screen Recording grants, login items, Finder workflows, browser manifests.
5. Uploads only to `127.0.0.1`; say so in the report.
