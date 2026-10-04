# H15: actual application tray-host transition evidence

Ran the full Release ShareX app on the existing KDE Wayland desktop, with Avalonia on XWayland. Each run used a private D-Bus daemon and synthetic StatusNotifierWatcher; the KDE tray host and compositor were untouched. Updates, uploads and hotkeys were disabled in private portable settings. No packages were installed.

- `baseline-receipt.json`: source before the H15 UI patch. The app registered its tray item and started hidden; after the host released its bus name, it stayed alive with no mapped main window for 12 seconds.
- `fixed-receipt.json`: with the H15 patch, the main window mapped after 8.699 seconds. Host return kept it visible; close-to-tray worked again after availability refreshed; a second host loss restored the window again. SIGTERM exited 0 and saved all settings.
- `no-host-receipt.json`: with the host absent before startup, `-silent` still opened the main window in 1.408 seconds. Closing that window exited 0 and saved all settings.
- `main-recovered.png`, `main-no-host.png`: app-only captures of the empty private main window. No personal screen or clipboard data is included.
- `gate-results.json`: final full solution/all-five-project gate, 0 warnings/errors, 633 passed / 67 platform or optional skips / 0 failures. All three actual-desktop startup fixtures passed.
- `manifest.json`: hashes and byte sizes for these assets.

The two five-second checks (backend availability cache and UI timer) allow recovery to take about ten seconds. These runs verify the application's availability/visibility path on a real display with a synthetic host. They do not claim a physical KDE panel restart test, other Linux desktop sign-off or fresh Windows execution. The native Windows tray path was not changed.
