#!/usr/bin/env bash
# Builds ShareX and installs it for the current user (no root needed).
#
#   Scripts/install-linux.sh              build and install to ~/.local
#   Scripts/install-linux.sh --uninstall  remove it again (settings and screenshots are kept)
#
# Set PREFIX to install somewhere else than ~/.local. Needs the .NET 10 SDK to build; the installed copy is self-contained.
# "Start ShareX when I log in" is a setting inside ShareX.
set -euo pipefail

prefix="${PREFIX:-$HOME/.local}"
app_dir="$prefix/lib/sharex/app"
bin_link="$prefix/bin/sharex"
# The portal looks ShareX up by this name (application id "sharex"); global hotkeys need it.
desktop_file="$prefix/share/applications/sharex.desktop"
icon_file="$prefix/share/icons/hicolor/256x256/apps/sharex.png"
repo="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

# The installed ShareX processes, found by executable because the sharex link changes their command line.
running_pids() {
  for pid in $(pgrep -i -x sharex 2>/dev/null); do
    [ "$(readlink "/proc/$pid/exe" 2>/dev/null)" = "$app_dir/ShareX" ] && echo "$pid"
  done
  return 0
}

# ShareX saves its settings and closes when it receives SIGTERM.
stop_running() {
  local pids
  pids="$(running_pids)"
  [ -n "$pids" ] || return 0
  kill -TERM $pids 2>/dev/null || true
  for _ in $(seq 1 50); do
    [ -n "$(running_pids)" ] || return 0
    sleep 0.1
  done
  kill -KILL $(running_pids) 2>/dev/null || true
}

case "${1:-}" in
  --uninstall)
    stop_running
    rm -rf "$app_dir"
    rm -f "$bin_link" "$desktop_file" "$icon_file"
    rmdir "$prefix/lib/sharex" 2>/dev/null || true
    command -v update-desktop-database >/dev/null && update-desktop-database "$prefix/share/applications" 2>/dev/null || true
    echo "ShareX removed. Settings and screenshots in ~/Documents/ShareX were kept."
    exit 0
    ;;
  "") ;;
  *) echo "Unknown option '$1'" >&2; exit 2 ;;
esac

case "$(uname -m)" in
  x86_64) rid=linux-x64; platform=x64 ;;
  aarch64|arm64) rid=linux-arm64; platform=ARM64 ;;
  *) echo "Unsupported CPU: $(uname -m)" >&2; exit 1 ;;
esac

command -v dotnet >/dev/null || { echo "The .NET 10 SDK is required (https://dot.net)." >&2; exit 1; }

staging="$(mktemp -d)"
trap 'rm -rf "$staging"' EXIT

echo "Building ShareX for $rid..."
dotnet publish "$repo/ShareX/ShareX.csproj" -c Release -r "$rid" --self-contained true \
  -p:Platform="$platform" -p:PublishTrimmed=false -p:DebugType=none -p:DebugSymbols=false -o "$staging/app" >"$staging/build.log" 2>&1 \
  || { cat "$staging/build.log" >&2; exit 1; }

stop_running
mkdir -p "$prefix/lib/sharex" "$prefix/bin" "$(dirname "$desktop_file")" "$(dirname "$icon_file")"
rm -rf "$app_dir"
cp -r "$staging/app" "$app_dir"
chmod +x "$app_dir/ShareX"
ln -sf "$app_dir/ShareX" "$bin_link"
cp "$repo/ShareX.HelpersLib/Resources/ShareX_Logo.png" "$icon_file"

cat >"$desktop_file" <<DESKTOP
[Desktop Entry]
Type=Application
Name=ShareX
GenericName=Screenshot and upload tool
Comment=Capture, annotate and share screenshots
Exec=$bin_link %F
Icon=sharex
Terminal=false
StartupWMClass=ShareX
Categories=Graphics;Utility;
Actions=Region;FullScreen;Window;

[Desktop Action Region]
Name=Capture region
Exec=$bin_link -RectangleRegion

[Desktop Action FullScreen]
Name=Capture full screen
Exec=$bin_link -PrintScreen

[Desktop Action Window]
Name=Capture active window
Exec=$bin_link -ActiveWindow
DESKTOP

command -v update-desktop-database >/dev/null && update-desktop-database "$prefix/share/applications" 2>/dev/null || true

echo "Installed to $app_dir"
echo "Start it from your application menu or run: sharex"

if [ "${XDG_CURRENT_DESKTOP:-}" = "Hyprland" ]; then
  cat <<'HYPRLAND'

Hyprland does not assign keys to global shortcuts by itself. After starting ShareX, list its hotkeys with
  hyprctl globalshortcuts
and bind the ones you want in your Hyprland configuration, for example:
  bind = CTRL, Print, global, sharex:sharex-1
In a Lua configuration, bind the key to hl.dsp.global("sharex:sharex-1").
HYPRLAND
fi
