# B51: clipboard upload on the actual KDE desktop

Agent J, 2026-10-04. Fedora 44, KDE Plasma on Wayland, Avalonia through XWayland, .NET SDK 10.0.112 / runtime 10.0.12, x64. Claim `ae994e3a6` was pushed before source edits. Inputs and evidence are in [assets/2026-10-04-agent-j-kde/b51](assets/2026-10-04-agent-j-kde/b51/manifest.json).

## Observed failure and fix

The full ShareX application used a private portable profile. A native Wayland GTK publisher advertised the known 51-character synthetic string, confirmed its own readback and remained alive. The real tray **Upload clipboard** action opened an empty preview with Upload disabled. Native paste into ShareX's **Upload text** window read the exact string; its Upload action sent all 51 bytes to the loopback server. An X11 image clipboard upload also succeeded. These distinguish the preview failure from an unavailable clipboard or uploader.

The baseline preview read `ClipboardHelpers` synchronously in its constructor, before showing or activating its window. A temporary diagnostic in the actual app found no clipboard formats immediately after opening, followed by the expected text format/string once native activation had completed. Another baseline transition from X11 image to Wayland text first previewed the previous synthetic image, then returned empty. The diagnostic was removed.

The dialog now waits for activation, posts its read after native focus events, and acquires one asynchronous data transfer from its own clipboard. Upload stays disabled until a valid snapshot exists. Image, text and file priority is retained. Original PNG data uses the existing Skia decoder before native bitmap conversion, preserving the established Windows decode behavior, including its premultiplied representation. Closing during a pending read discards late content/errors and disposes the received transfer/native bitmap. No platform/helper/backend, contract, project, dependency or catalog changed.

## Actual application results

All uploads were explicit actions against `http://127.0.0.1:37929/upload`. The server accepted only the exact known text or a PNG whose decoded RGBA pixels matched the generated image. Unknown payloads were rejected. No personal data or third-party endpoint was used.

| Flow | Result |
| --- | --- |
| Native Wayland text preview and Upload | Correct 51-character preview; exact 51-byte request, SHA256 `846e3d00a356e01026615e89884ec17f6fb331ae8308544aab9fe67e419b1968`. Repeated after restart. |
| Three fresh native Wayland text publishers | Each activated, published/read back the known text, produced the correct ShareX preview and cancelled cleanly. |
| Native Wayland image preview and Upload | Correct 640 × 400 preview; server decoded every pixel equal to the synthetic fixture. Repeated after restart. PNG encodings may differ; the image comparison is of pixels. |
| X11 `text/uri-list` file clipboard | Preview listed the one synthetic file; Upload sent its known 51-byte contents correctly. Its `.txt` extension uses the existing text uploader route. Native Wayland file offers remain unverified. |
| Custom uploader import | A valid `.sxcu` with Version 22.0.0 opened the actual confirmation and settings window. Name, destinations and local endpoint persisted through normal exit/restart. An earlier fixture omitted Version and correctly failed validation; that was a fixture error. |
| History | Seven loopback uploads persisted with the expected URLs/types; SQLite integrity returned `ok`. Actual History rendered the rows and filename search visibly filtered to the one synthetic file. AT-SPI retained virtual containers, so their count was not used as the visible row count. |
| Shutdown | Six relevant full-app runs exited 0 through tray Exit and saved the private profile, including repeated starts after import. |

`requests.json`, `receipt.json`, the preview/history images, accessibility trees, publisher receipts and manifest contain the concrete results. The final manual app and full-gate binary hashes are recorded separately; their dialog behavior is the same, with the final build also containing a comment change. The proper dialog cases below run against the final build.

## Proper test and build gate

`dotnet build ShareX.sln -c Release -p:Platform=x64`: **0 warnings, 0 errors**. All five proper projects: **641 passed, 67 platform/optional skips, 0 failures**, with `SHAREX_TEST_DESKTOP=1` and `SHAREX_TEST_APPLICATION_DIRECTORY` pointing to the production build. [gate.json](assets/2026-10-04-agent-j-kde/b51/gate.json) records each project and outcome.

Six new `ShareX.ImageEditor.Tests` child-process cases load the actual application dialog and use controlled providers/generated images, without showing the fixture or reading the user's clipboard. They cover pending text, image priority, PNG priority/alpha against the established decoder, close before transfer, close during native bitmap acquisition and a late provider error. They verify disabled pending upload, explicit snapshot ownership, transfer/native bitmap disposal and absence of late UI updates. The five existing actual desktop startup/session lifetime cases also passed. These supplement the full-app flows; they do not constitute Windows desktop verification.

## Remaining limits

R51 asks M to audit direct clipboard dispatch and its helper context; the trace identifies activation timing, not a proven format parser bug. R49 hotkey backend guard, R50 cursor capture control and scope-alignment requests remain open. The proposed desktop entry/shortcut test still needs the pending user permission; it has not been installed. Region pointer automation reached the focused ShareX window but the desktop ignored the requested pointer movement, as recorded in `region-pointer-receipt.json`; physical region selection remains unverified. Other supported desktops and Windows 10/11 sign-off remain open. Broad gates and the 90% estimate are unchanged, macOS is deferred, and branch automations remain paused.
