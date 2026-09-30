#!/usr/bin/env bash
# Builds ShareX.Desktop and installs it for the current user (no root needed).
#
#   Scripts/install-linux.sh              build and install to ~/.local
#   Scripts/install-linux.sh --autostart  also start ShareX when you log in
#   Scripts/install-linux.sh --uninstall  remove it again (settings and screenshots are kept)
#
# Needs the .NET 10 SDK. The installed copy is self-contained, so it does not need .NET at run time.
set -euo pipefail

prefix="${PREFIX:-$HOME/.local}"
app_dir="$prefix/lib/sharex/app"
bin_link="$prefix/bin/sharex"
desktop_file="$prefix/share/applications/sharex.desktop"
icon_file="$prefix/share/icons/hicolor/256x256/apps/sharex.png"
autostart_file="${XDG_CONFIG_HOME:-$HOME/.config}/autostart/sharex.desktop"
repo="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

stop_running() {
  if [ -x "$bin_link" ]; then
    "$bin_link" quit >/dev/null 2>&1 || true
  fi
}

case "${1:-}" in
  --uninstall)
    stop_running
    rm -rf "$prefix/lib/sharex/app"
    rm -f "$bin_link" "$desktop_file" "$icon_file" "$autostart_file"
    rmdir "$prefix/lib/sharex" 2>/dev/null || true
    echo "ShareX removed. Settings in ~/.config/ShareX and screenshots in ~/Pictures/ShareX were kept."
    exit 0
    ;;
  ""|--autostart) ;;
  *) echo "Unknown option '$1'" >&2; exit 2 ;;
esac

case "$(uname -m)" in
  x86_64) rid=linux-x64 ;;
  aarch64|arm64) rid=linux-arm64 ;;
  *) echo "Unsupported CPU: $(uname -m)" >&2; exit 1 ;;
esac

command -v dotnet >/dev/null || { echo "The .NET 10 SDK is required (https://dot.net)." >&2; exit 1; }

staging="$(mktemp -d)"
trap 'rm -rf "$staging"' EXIT

echo "Building ShareX.Desktop for $rid..."
dotnet publish "$repo/ShareX.Desktop/ShareX.Desktop.csproj" -c Release -r "$rid" --self-contained true \
  -p:Platform=x64 -p:PublishTrimmed=false -p:DebugType=none -p:DebugSymbols=false -o "$staging/app" >"$staging/build.log" 2>&1 \
  || { cat "$staging/build.log" >&2; exit 1; }

stop_running
mkdir -p "$app_dir" "$prefix/bin" "$prefix/share/applications" "$(dirname "$icon_file")"
rm -rf "$app_dir"
cp -r "$staging/app" "$app_dir"
chmod +x "$app_dir/sharex"
ln -sf "$app_dir/sharex" "$bin_link"
cp "$repo/ShareX.Desktop/Assets/ShareX.png" "$icon_file"

cat >"$desktop_file" <<DESKTOP
[Desktop Entry]
Type=Application
Name=ShareX
GenericName=Screenshot and upload tool
Comment=Capture, annotate and share screenshots
Exec=$bin_link %F
Icon=sharex
Terminal=false
Categories=Graphics;Utility;
MimeType=image/png;image/jpeg;image/bmp;image/gif;image/webp;
Actions=Region;FullScreen;

[Desktop Action Region]
Name=Capture region
Exec=$bin_link capture region

[Desktop Action FullScreen]
Name=Capture full screen
Exec=$bin_link capture fullscreen
DESKTOP

if [ "${1:-}" = "--autostart" ]; then
  mkdir -p "$(dirname "$autostart_file")"
  cat >"$autostart_file" <<DESKTOP
[Desktop Entry]
Type=Application
Name=ShareX
Exec=$bin_link
Icon=sharex
Terminal=false
X-GNOME-Autostart-enabled=true
DESKTOP
  echo "ShareX will start when you log in."
fi

echo "Installed to $app_dir"
echo "Run 'sharex doctor' to check this system and 'sharex hotkeys' for key bindings."
