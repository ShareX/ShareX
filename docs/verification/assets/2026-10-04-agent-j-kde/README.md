# Retained synthetic KDE verification assets

These files come from the actual full ShareX application on Fedora 44 KDE Wayland, with Avalonia on XWayland, at source `c00063b1a`. Upload was disabled for this run. The [verification report](../../2026-10-04-agent-j-kde.md) describes the checks and their limits.

- `original-synthetic.png`: generated 640×400 editor input.
- `synthetic.png`: the saved 640×800 result after paste below, cut, undo, redo and internal paste.
- `clipboard-x11.*` and `clipboard-wayland.*`: images received by active external GTK clipboard consumers and their display/surface receipts.
- `editor-save-verification.json`: decoded-pixel checks and saved-file hash.
- `editor-*.png`: captures of the app's own editor window during the operations.
- `qr-open.png` and `color-open.png`: app tool windows displaying synthetic QR content and the initial red color respectively; they do not prove copy/save/scan or color editing.
- `manifest.json`: exact byte sizes and SHA-256 hashes of the retained files.

All content is synthetic. No whole-desktop screenshot, personal file, user clipboard content or credential is retained here. The test source remains in the proper test projects; these data files preserve manual application evidence that was previously left in volatile `/tmp`.
