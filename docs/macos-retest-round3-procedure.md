# macOS retest procedure (round 3)

A short, focused pass after the fixes for the round 2 report (`macos-retest-report-2026-10-07.md`, commit `30d18dbd0`). Only the areas that changed are tested. Same report format, IDs and result words as round 2 (**fixed**, **still fails**, **partial**, **not checked**); new findings continue from **F25**. Tag every check `[by hand]` or `[injected]`. Most checks here are about where things appear on screen, so do them **by hand** where possible.

Uploads only to the local test server (`127.0.0.1:8000`, step 5 of [macos-test-procedure.md](macos-test-procedure.md)).

## 0. Install

1. Build at commit `30d18dbd0` or later: `git pull && Scripts/package-macos.sh arm64`, then install from the `.dmg`.
2. The new build has a new ad hoc signature. In System Settings > Privacy & Security > Screen & System Audio Recording, remove ShareX with the minus button, add it again, then restart ShareX. Do the same under Accessibility if it was granted before.
3. **Start ShareX from the Dock or Finder, not from Terminal**, for the whole round. Terminal gives it a different PATH and different permissions, which hid F15 last time.

## 1. Region overlay position (F18, was High)

1. *By hand:* start Region capture (⌥⇧4). Look at the top of the screen: there must be **one** menu bar (the frozen one), not the live menu bar with a dimmed copy below it. The frozen screenshot must not be shifted down, and its bottom edge must not be cut off.
2. Select a rectangle tightly around a recognisable control (for example a toolbar button). Open the saved image: it starts exactly at that control's top-left corner, with no extra strip above or missing strip below.
3. Select a region touching the **bottom edge** of the screen. With the Dock set to hide automatically, the Dock must not appear while selecting (F22), and the image includes the bottom rows.
4. Esc still cancels the overlay (F1 regression check).
5. If a Mac without a notch or an external display is available, repeat 1 and 2 there.

## 2. Cursor and pointer (F8, F22)

1. Turn on "Show cursor" in task settings. *By hand:* put the pointer over a recognisable spot, press ⌥⇧4, drag a region containing that spot starting elsewhere. The cursor in the image is at the spot where it was, at the right size.
2. During the selection the pointer is a sharp crosshair of normal size (it is now macOS's own crosshair).

## 3. Screen recording (F15, F16, F2)

1. With FFmpeg installed by Homebrew and ShareX started from the Dock, ⌥⇧5 (or Capture > Screen recording) starts a recording: no "Install FFmpeg" message.
2. While recording: the red dashed border sits **around** the selected region, the same size as the selection, and the control bar (Pause / Restart / Abort) is **outside** it.
3. Stop and play the MP4. Extract a frame: `ffmpeg -ss 2 -i <file> -frames:v 1 f.png`. It shows exactly the selected area, with no red line and no control bar, at full Retina resolution.
4. Repeat 2 and 3 for a GIF.
5. Record the full screen once (not run in round 2).

## 4. Scrolling capture (F17, F23)

1. Remove ShareX from Accessibility. Start Scrolling capture. Expected: macOS's Accessibility prompt appears **before** any region selection, and the ShareX window shows (with visible content, not blank) a message saying the Accessibility permission is needed and how to grant it.
2. Allow ShareX under Accessibility (restart ShareX if macOS asks).
3. Start Scrolling capture on a long web page or Finder list. The green frame matches the selected region in size and position. The window is out of the way during the capture.
4. At the end the "ShareX - Scrolling capture" window comes back **with content**: the stitched image preview, its size, and the Upload / Copy buttons. Copy the result and paste it somewhere: no repeated or missing bands.

## 5. Image viewer (F20)

1. Open History, select an image and open it in the viewer. It covers the screen (not a separate full-screen space with its own animation).
2. Press Esc: the viewer closes and ShareX keeps working. Repeat with Enter, a click, and the arrow keys to change image.

## 6. Menu bar icon (F24)

1. **Left click** the ShareX menu bar icon: it runs the left click action from Application settings > General (by default it opens ShareX), and no menu opens.
2. **Right click**, and **Control-click**: ShareX's menu opens, below the icon, and its items work (try a capture and Exit).
3. If a mouse with a middle button is available, middle click runs the middle click action.
4. The icon is visible and sharp, at normal menu bar size. Its tooltip says ShareX.
5. Quit with ⌘Q: the icon disappears and the process ends.

## 7. Small items

| ID | Check |
| --- | --- |
| F14 | About > check for updates: no "Update check failed". Expected "up to date" (or a link to the release page). |
| F19 | Tools > OCR on the French system: the language is French by default. Recognition still works. |
| F11 | Settings > Integration: hovering the disabled "Send to" option shows a reason. |
| F4 | Hotkey settings show modifiers in macOS order: `⌥ + ⇧ + 4`, and `⌃ + ⌥ + ⇧ + ⌘` when combined. |
| F21 | By design: in the editor ⌘C copies the image, **⌘⇧C** copies the selected shape and **⌘D** duplicates it. Check ⌘⇧C then ⌘V pastes only the shape. |
| Ruler, pin | Tools > Ruler on a region: the ruler window matches the region. Pin a region: the pinned window has the region's size. |

## 8. Report

`macos-retest-report-<date>.md`, as in round 2: front matter (commit, how ShareX was started, machine, displays), a summary table with one line per item above, new findings from F25, and the state left on the Mac. If everything in sections 1 to 6 passes, say so in the verdict; that closes the Mac rerun in the roadmap's release checks.
