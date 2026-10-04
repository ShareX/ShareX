# B49: unsupported hotkey enabling on actual KDE

Fedora 44 KDE Plasma Wayland, Avalonia on the existing XWayland display. .NET SDK 10.0.112/runtime 10.0.12, x64. Full Release ShareX app with private portable settings, synthetic workflow/content and upload disabled. No package, desktop entry, compositor configuration or real hotkey was installed/changed.

## Observed failure

The actual GlobalShortcuts portal reports version 2, but desktop information for sharex.desktop is absent and Registry.Register rejects sharex with “App info not found”. With this unsupported capability, the real Hotkey settings Enable button remained enabled. Its native AT-SPI click cleared DisableHotkeys; normal tray Exit saved false. Baseline app source bf89c5ca5 has the same hotkey UI through d89561e00. Claim 0e586dd61 was published before edits.

## Change and actual verification

The presentation service exposes the existing Hotkeys.Support value. Settings show the unavailable reason, disable the Enable button, and recheck support in both the ViewModel and dispatched adapter action. Existing custom service providers remain compatible through the supported default. The shared toggle task mapping and the tray's separate toggle item use that capability only when enabling; the tray callback checks again at execution. Disabling and saved workflow/gesture definitions remain available.

- With DisableHotkeys=true, the actual settings button is disabled and its accessibility action is rejected as “Element not enabled”. The real KDE dbusmenu Enable item is also disabled and names the reason. A directly delivered clicked event leaves settings unchanged. Normal Exit returns 0 and persists true.
- With an existing DisableHotkeys=false, the unavailable reason remains visible, the Enable button is absent and the real tray Disable item is enabled. Normal Exit returns 0 and retains false, so viewing unsupported settings does not rewrite the user's choice.
- A third run clicks the enabled Disable item; normal Exit returns 0 and saves true. Upload remains disabled in every run.

[Retained app-only screenshots, accessibility states, action/persistence receipts, source hashes and manifest](assets/2026-10-04-agent-j-kde/b49/README.md) survive reboot. Native AT-SPI and KDE D-Bus exercised the actual app; no helper-only result substitutes for the flow. R49 requests the M-owned direct backend enabling/registration guard; backend source was untouched. Shortcut registration/firing still requires an approved normal product desktop entry and remains unverified. Windows supported behavior follows the existing path, but this Linux run is not fresh Windows desktop sign-off.

## Other application flow evidence

The real custom task after-capture menu now confirms B46: preserved OCR is selected and disabled while Copy image, Save image and Print image remain enabled according to their service support. No OCR or print job was executed.

The actual QR tool generated `ShareX synthetic QR fixture 2026-10-04`. Its native accessibility Copy image button placed a 418×418 RGBA image on the clipboard, received by an active external native Wayland GTK window. The retained QR copy is synthetic. Scan image opened the native KDE file picker, but that Qt picker was unavailable to the existing accessibility automation; scan/save are not yet verified. Normal app Exit closed that fixture. No desktop accessibility configuration was changed.

## Full publication gate

`dotnet build ShareX.sln -c Release -p:Platform=x64`: 0 warnings/errors. Every proper test project ran with SHAREX_TEST_DESKTOP=1: **635 passed, 67 platform or optional skips, 0 failures**. Platform 278/37, History 8/0, ImageEditor 180/28, ImageEffects 65/0, Tools 104/2 (pass/skip). All five actual-desktop startup/session-shutdown cases passed. Existing regression suites plus actual app flows validate this small presentation guard; no implementation-mirroring helper tests were added.

Linux/Windows estimates stay 90%, broad desktop gates remain open, R20/R22/R48/R49 backend work stays coordinated, macOS remains deferred and automatic cross-platform-v2 workflows remain paused.
