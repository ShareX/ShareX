# macOS test procedure

A manual pass of the real ShareX application on a Mac, in the order the steps depend on each other. It is a release check in [cross-platform-roadmap.md](cross-platform-roadmap.md). After a first pass, use [macos-retest-procedure.md](macos-retest-procedure.md) to check fixes.

Record each result as **pass**, **fail** (with what happened) or **skipped**. When something fails, keep the log: `~/Documents/ShareX/Logs/ShareX-Log-<date>.txt`.

Uploads go only to a local server (AGENTS.md: never send real data to third-party hosts).

## 1. Build and install

1. On the Mac, in the repository on `cross-platform-v2`, run:

   ```
   Scripts/package-macos.sh arm64
   ```

   Use `x64` on an Intel Mac. The zip and the disk image are written to `artifacts/ShareX-<version>-macos-<arch>.zip` and `.dmg`. A build from the Cross-platform workflow's artifacts (`sharex-macos-arm64` or `-x64`) can be used instead.
2. Open the disk image (or unzip the zip) and drag **ShareX.app** to **Applications**.
3. A Developer ID signed and notarized build opens directly. If an unsigned build was downloaded or copied from another machine, macOS may say *"ShareX is damaged and can't be opened"*. Gatekeeper says this about quarantined downloads that are not signed with a Developer ID. To fix it:
   1. Open Terminal.
   2. Type `xattr -cr ` (with a space at the end) but do not press Enter yet.
   3. Drag **ShareX.app** from Finder into the Terminal window. This pastes its full path.
   4. Now press Enter.

## 2. First start and permissions

1. Open ShareX from Applications. The welcome screen should say "Built for macOS".
2. Take a full screen capture (menu bar icon > Capture > Full screen). macOS asks for the **Screen Recording** permission.
3. Allow ShareX in System Settings > Privacy & Security > Screen & System Audio Recording.
4. **Quit and restart ShareX.** macOS only applies the permission on the next launch.
5. Repeat the full screen capture. It must show other applications' windows, not just the wallpaper.

## 3. Capture

Check each image in the editor or the screenshots folder.

1. Full screen.
2. Region: drag a region; also cancel with Esc. Check the overlay lines up with the screen.
3. Window: pick another application's window; try with and without the shadow option.
4. Active window.
5. Cursor: turn on "Show cursor" in task settings, capture a region, and check the pointer appears in the right place and at the right size.
6. **Retina**: repeat region and cursor on the built-in Retina display. The image must be full resolution and the region exact.
7. **Second monitor** (if available, ideally a non-Retina one): full screen, region across both screens, and a region on the second screen only.

## 4. Recording and GIF

1. Install FFmpeg (`brew install ffmpeg`) or set its path in Task settings > Screen recorder > FFmpeg options.
2. Record a region to MP4, stop it, and play the file.
3. Record a region as GIF and open it.
4. Repeat one recording on the second monitor, if available.

## 5. Upload and history

1. Start a local test server in Terminal:

   ```
   python3 -c "import http.server as h
   class S(h.BaseHTTPRequestHandler):
       def do_POST(s):
           s.rfile.read(int(s.headers['Content-Length'])); s.send_response(200); s.end_headers(); s.wfile.write(b'http://127.0.0.1:8000/ok.png')
   h.HTTPServer(('127.0.0.1', 8000), S).serve_forever()"
   ```
2. Save this as `local.sxcu`, then double-click it in Finder. ShareX must open its custom uploader import (this also tests **opening .sxcu files**):

   ```
   {
     "Version": "17.0.0",
     "Name": "Local test",
     "DestinationType": "ImageUploader, TextUploader, FileUploader",
     "RequestMethod": "POST",
     "RequestURL": "http://127.0.0.1:8000/",
     "Body": "MultipartFormData",
     "FileFormName": "file",
     "URL": "{response}"
   }
   ```
3. Choose "Local test" as the image, text and file destination.
4. Capture a region and upload it. The URL `http://127.0.0.1:8000/ok.png` must be copied to the clipboard.
5. Upload some text from the clipboard and a file.
6. Open History: all three uploads are listed, and the image shows a thumbnail.

## 6. Editor and tools

1. Open a capture in the image editor. Add a rectangle, arrow, text, an emoji sticker and a cursor annotation. Undo, redo and save.
2. Tools: colour picker (screen colour sampling), ruler, pin to screen, QR code, image combiner, video converter (needs FFmpeg) and scrolling capture. Scrolling capture asks for the **Accessibility** permission the first time; allow it, restart ShareX and try again.
3. OCR (Tools > OCR) recognises the text in a selected region through Apple's Vision framework.

## 7. Hotkeys

1. Assign a hotkey to Region capture in Hotkey settings, for example Control+Shift+4 (Macs have no Print Screen key).
2. Press it from another application. The capture must start.
3. Assign a hotkey that macOS already uses (Command+Shift+3). ShareX must report the conflict instead of failing silently.

## 8. Menu bar icon and Dock

1. The ShareX icon appears in the menu bar. Its menu opens and its captures work.
2. Close the main window, then click the ShareX icon in the Dock. The main window comes back.
3. The ShareX application menu (next to the Apple menu) shows About, Settings (Command+,) and Quit (Command+Q).
4. Drop an image file on the ShareX Dock icon. It is uploaded.
5. Right-click any file in Finder > Open With > ShareX. It is uploaded.

## 9. Settings and start at login

1. Change a few settings (theme, language, after-capture tasks), quit with Command+Q and restart. They are kept.
2. Turn on "Run ShareX when I sign in". Log out and back in. ShareX starts.
3. Turn it off and log out and in again. ShareX does not start.

## 10. Finder actions

1. In Application settings > Integration, turn on "Upload with ShareX" and "Edit with ShareX".
2. Right-click a file in Finder > Quick Actions (or Services) > **Upload with ShareX**. It is uploaded.
3. Right-click an image > **Edit with ShareX**. It opens in the editor.
4. Turn both off. The entries disappear (Finder may need a moment).

## 11. Browser extension

1. In Application settings > Integration, register the browser extension host for your browser.
2. Install the ShareX browser extension, right-click an image on a web page and choose to upload it with ShareX.
3. ShareX uploads it to the local test server (step 5).

## 12. Finish

1. Quit ShareX with Command+Q. The log ends without "Unhandled exception".
2. Report results and logs in the project thread. When everything passes, tick the Mac rerun in the roadmap's release checks.
