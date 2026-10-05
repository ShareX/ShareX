# B53: Fedora KDE native portal identity

Jaex reported that global hotkeys still failed on Fedora KDE after B52. The screenshot reports `Could not register app ID: App info not found for 'sharex'`. B52 retains a portal-provided sandbox identity; this separate native-host failure requires ShareX's own desktop entry.

Native ShareX now prepares its portal identity before `Registry.Register`. If no user or system `sharex.desktop` exists in the XDG data search path, it atomically writes a minimal `NoDisplay=true` entry in the user application directory. It uses the actual apphost, or the .NET host plus application assembly for `dotnet ShareX.dll`. It adds no launcher menu item, file association, autostart or shortcut grant. Only the exact marked template it generates can be refreshed for a moved/rebuilt application; installed/customized entries and symlinks are preserved. Unavailable filesystem paths retain the portal error. Detected sandboxes omit preparation and host registration, retaining B52 behavior. Test hosts and other platform-library consumers do not own this application identity.

## Actual Fedora portal verification

Fedora 44 KDE Wayland, existing `/usr/libexec/xdg-desktop-portal` and KDE backend, isolated session bus and temporary XDG directories. No user's desktop file, compositor configuration, shortcut bindings or permissions were changed. The temporary probe loads the production Linux library and exercises production connection creation, portal version lookup and portal request handling. Its entry assembly is named `ShareX` to exercise the application's default identity preparation.

- Without the desktop entry, the real host Registry returns the exact reported `App info not found for 'sharex'` error.
- With the fix, native apphost registration succeeds, GlobalShortcuts reports v2 and the real KDE `CreateSession` response is 0. The session is closed normally.
- The generated entry passes the existing desktop-file validator.
- An entry with a missing executable reproduces the real registration failure; refreshing our generated entry restores successful registration and session creation.
- The `dotnet ShareX.dll` launch also registers successfully and creates/closes a real KDE shortcut session.

[Receipt](assets/2026-10-05-b53-kde-hotkeys/receipt.json), [probe source](assets/2026-10-05-b53-kde-hotkeys/Program.cs), [isolated verification driver](assets/2026-10-05-b53-kde-hotkeys/verify.py).

Physical shortcut firing and actual user permission interaction remain unverified. No fresh Windows desktop sign-off is inferred; the implementation changes are Linux-only.

## Regression gate

Eighteen new cases cover native and .NET-host desktop identities, argument/key-value escaping, XDG search order, preservation of existing/customized entries, idempotence, stale generated identity refresh, invalid/unwritable paths, sandbox omission and preparation before host registration. The transport case checks production connection creation against a private D-Bus portal that rejects registration until the entry exists. All prior B52 sandbox/error/peer-disposal regressions remain green.

Full Linux Release solution build: **0 warnings, 0 errors**. All five proper test projects: **705 passed, 70 platform/optional skips, 0 failures**, with the actual-desktop lifetime fixtures enabled. [Gate receipt](assets/2026-10-05-b53-kde-hotkeys/gate.json). The Debug solution was also rebuilt for the development launch.

The [portal Registry specification](https://flatpak.github.io/xdg-desktop-portal/docs/doc-org.freedesktop.host.portal.Registry.html) requires the registered id to match a desktop entry and registration to precede portal calls. The generated Exec value follows the [desktop-entry escaping rules](https://specifications.freedesktop.org/desktop-entry/latest/exec-variables.html).
