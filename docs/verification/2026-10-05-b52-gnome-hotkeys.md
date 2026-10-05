# B52: Ubuntu GNOME hotkey portal identity

Jaex reported that every hotkey failed with a host Registry rejection of `io.snapcraft` application identity. The registry is for unsandboxed applications; the portal supplies identity for Snap and Flatpak processes. ShareX attempted host registration unconditionally and then disabled its hotkey service when that registration was rejected.

The fix checks the actual Flatpak marker and the Snap cgroup controllers used by the portal before registering. It does not infer sandbox identity from inherited environment variables. If process metadata was unavailable and the portal replies with its specific sandbox-registration rejection, ShareX closes that D-Bus peer and opens a fresh one without host registration. This also avoids newer portals retaining the rejected registration on the original peer. Missing desktop-entry and unrelated failures remain visible; the normal `sharex` host registration and GNOME shortcut permission flow are retained.

Implementation is limited to `ShareX.Platform.Linux/DBus/DBusSession.cs` and the new internal registration helper. Windows, platform contracts, shared application presentation, dependencies and macOS are unchanged. No desktop entry, key binding, compositor setting or permission was installed or changed.

## Verification

- Linux Release solution build: **0 warnings, 0 errors**.
- All five proper test projects: **687 passed, 70 platform/optional skips, 0 failures**, with the existing real-desktop lifetime fixtures enabled. [Gate results](assets/2026-10-05-b52-gnome-hotkeys/gate.json).
- Thirty portable regression cases cover cgroup classification, omitted sandbox registration, awaited host registration, older portals, sandbox rejection recovery, missing desktop entries, unrelated denial and transport failures.
- Four additional isolated D-Bus tests exercise production connection creation: host registration precedes portal calls; detected sandboxes omit it; an undetected Snap gets a new usable peer after rejection, and the rejected peer no longer owns its bus name; missing `sharex.desktop` remains an error. The fixture uses the existing D-Bus daemon on its own private session bus and does not contact the user's portal or alter permissions.
- The current Ubuntu GNOME desktop exposes GlobalShortcuts v1. A separate native host probe reports that `sharex.desktop` is absent on this development checkout. This is distinct from the attached Snap-registration failure. Actual GNOME permission granting and physical shortcut firing are **not verified** by the isolated transport tests. Windows desktop regression sign-off is also unverified here.

The task claim was recorded locally after the baseline green gate. The initial publication attempt failed because this shell did not inherit VS Code's Git integration or a Git author. Jaex subsequently authorized commit/push and identified VS Code's existing repository access. Publication uses that existing askpass integration and Jaex's established repository author identity; no credentials are copied into the repository or logs.

Portal references: [host Registry contract](https://flatpak.github.io/xdg-desktop-portal/docs/doc-org.freedesktop.host.portal.Registry.html), [Snap cgroup detection](https://github.com/flatpak/xdg-desktop-portal/blob/main/shared/xdp-app-info-snap.c), [current rejected registration handling](https://github.com/flatpak/xdg-desktop-portal/blob/main/desktop-portal/registry.c).

## Conditional publication follow-up

Jaex authorized commit/push only after the keyboard implementation is shown working on Ubuntu. Inspection confirms the running Debug ShareX process belongs to VS Code's Snap cgroup, matching the attached failure. Debug was rebuilt successfully with 0 warnings/errors; an already running process still needs a restart to load rebuilt assemblies.

A standalone probe using the production Linux hotkey service and a harmless Ctrl+Alt+Shift+F8 trigger is prepared outside the repository. Its build passes with 0 warnings/errors. The native GNOME run is pending approval for the temporary ShareX desktop entry and shortcut permission dialog, as required by AGENTS.md. No key firing or approval is inferred from its compilation or the isolated regression tests. The initial GitHub authentication obstacle was traced to this shell missing VS Code's askpass environment. Jaex subsequently authorized commit/push using the existing VS Code access. Actual GNOME key delivery remains unverified; publication does not tick that desktop gate.

The publication gate was repeated on the final source after the latest pull: Linux Release build 0 warnings/errors and all five test projects 687 passed / 70 platform or optional skips / 0 failures, with desktop lifetime fixtures enabled.
