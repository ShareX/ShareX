#!/usr/bin/env bash
# Turns the self-contained Linux package that install-linux.sh --package makes into a .deb, an .rpm and an AppImage.
#
#   Scripts/package-linux.sh OUTDIR/ShareX-<version>-linux-<arch>.tar.gz OUTDIR
#
# Writes ShareX-<version>-linux-<arch>.deb, .rpm and .AppImage next to it in OUTDIR, named like the other release files.
# Needs nfpm (https://nfpm.goreleaser.com) for the .deb and .rpm, and appimagetool with a type 2 runtime for the AppImage:
#   NFPM=nfpm  APPIMAGETOOL=appimagetool  APPIMAGE_RUNTIME=path/to/runtime-<cpu>
# SHAREX_PRERELEASE (for example dev42) marks preview builds as older than the release of the same version: 22.4.0~dev42.
set -euo pipefail

tarball="${1:?usage: package-linux.sh ShareX-<version>-linux-<arch>.tar.gz OUTDIR}"
out="${2:?usage: package-linux.sh ShareX-<version>-linux-<arch>.tar.gz OUTDIR}"
nfpm="${NFPM:-nfpm}"
appimagetool="${APPIMAGETOOL:-appimagetool}"

name="$(basename "$tarball" .tar.gz)"
[[ "$name" =~ ^ShareX-([0-9.]+)-linux-(x64|arm64)$ ]] || { echo "Unexpected package name: $name" >&2; exit 2; }
version="${BASH_REMATCH[1]}"
case "${BASH_REMATCH[2]}" in
  x64) nfpm_arch=amd64; appimage_arch=x86_64 ;;
  arm64) nfpm_arch=arm64; appimage_arch=aarch64 ;;
esac

staging="$(mktemp -d)"
trap 'rm -rf "$staging"' EXIT
mkdir -p "$out"
tar -C "$staging" -xzf "$tarball"

# The .deb and .rpm install to /usr through the package's own install.sh, so they get the same files and menu entry.
DESTDIR="$staging/root" PREFIX=/usr "$staging/$name/install.sh" >/dev/null

cat >"$staging/nfpm.yaml" <<NFPM
name: sharex
arch: $nfpm_arch
platform: linux
version: $version
prerelease: ${SHAREX_PRERELEASE:-}
section: graphics
priority: optional
maintainer: ShareX Team
vendor: ShareX Team
homepage: https://getsharex.com
license: GPL-3.0-or-later
description: |-
  Screenshot, screen recording and file sharing tool
  Captures regions, windows and the full screen, annotates them and uploads them to many destinations.
contents:
  - src: $staging/root/usr/lib/sharex
    dst: /usr/lib/sharex
    type: tree
  - src: /usr/lib/sharex/app/ShareX
    dst: /usr/bin/sharex
    type: symlink
  - src: $staging/root/usr/share/applications/sharex.desktop
    dst: /usr/share/applications/sharex.desktop
  - src: $staging/root/usr/share/icons/hicolor/256x256/apps/sharex.png
    dst: /usr/share/icons/hicolor/256x256/apps/sharex.png
# The browser extension's host, registered for Chrome, Chromium, Edge and Firefox.
  - src: $staging/root/etc/opt/chrome/native-messaging-hosts/com.getsharex.sharex.json
    dst: /etc/opt/chrome/native-messaging-hosts/com.getsharex.sharex.json
  - src: $staging/root/etc/chromium/native-messaging-hosts/com.getsharex.sharex.json
    dst: /etc/chromium/native-messaging-hosts/com.getsharex.sharex.json
  - src: $staging/root/etc/opt/edge/native-messaging-hosts/com.getsharex.sharex.json
    dst: /etc/opt/edge/native-messaging-hosts/com.getsharex.sharex.json
  - src: $staging/root/usr/lib/mozilla/native-messaging-hosts/ShareX.json
    dst: /usr/lib/mozilla/native-messaging-hosts/ShareX.json
# The runtime is bundled. These are the command line tools ShareX uses for the clipboard, recording and notifications; it
# says which one is missing when a feature needs it.
overrides:
  deb:
    recommends: [wl-clipboard, xclip, ffmpeg, xdg-utils, libnotify-bin]
    suggests: [gstreamer1.0-pipewire]
  rpm:
    recommends: [wl-clipboard, xclip, ffmpeg-free, xdg-utils, libnotify]
    suggests: [pipewire-gstreamer]
rpm:
  summary: Screenshot, screen recording and file sharing tool
NFPM

"$nfpm" package --config "$staging/nfpm.yaml" --packager deb --target "$out/$name.deb"
"$nfpm" package --config "$staging/nfpm.yaml" --packager rpm --target "$out/$name.rpm"

# The AppImage runs ShareX from its mounted image. ShareX writes its own hidden sharex.desktop at start, which the portal
# needs for screenshots and hotkeys, so the menu entry here is only for AppImage launchers that integrate it.
appdir="$staging/ShareX.AppDir"
mkdir -p "$appdir"
cp -a "$staging/root/usr" "$appdir/usr"
rm "$appdir/usr/bin/sharex"
# The system-wide browser manifests are for the .deb and .rpm; the AppImage registers itself when it runs.
rm -r "$appdir/usr/lib/mozilla"
ln -s ../lib/sharex/app/ShareX "$appdir/usr/bin/sharex"
sed 's|/usr/bin/sharex|sharex|' "$staging/root/usr/share/applications/sharex.desktop" >"$appdir/sharex.desktop"
cp "$staging/root/usr/share/icons/hicolor/256x256/apps/sharex.png" "$appdir/sharex.png"
ln -s sharex.png "$appdir/.DirIcon"
cat >"$appdir/AppRun" <<'APPRUN'
#!/bin/sh
here="$(dirname "$(readlink -f "$0")")"
# Browsers start the extension host through the launcher ShareX writes to ~/.local/share/sharex/browser-host, because the
# image is mounted at a new path on every start.
if [ "${1:-}" = "--browser-host" ]; then
  shift
  exec "$here/usr/lib/sharex/app/ShareX_NativeMessagingHost" "$@"
fi
exec "$here/usr/lib/sharex/app/ShareX" "$@"
APPRUN
chmod +x "$appdir/AppRun"

runtime=()
[ -n "${APPIMAGE_RUNTIME:-}" ] && runtime=(--runtime-file "$APPIMAGE_RUNTIME")
ARCH="$appimage_arch" "$appimagetool" --no-appstream "${runtime[@]}" "$appdir" "$out/$name.AppImage" >/dev/null
chmod +x "$out/$name.AppImage"

ls -l "$out/$name".{deb,rpm,AppImage}
