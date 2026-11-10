#!/usr/bin/env bash
# Starts an installed ShareX in a clean home on a virtual X server with its own D-Bus session, takes a full screen screenshot
# through the command line (default settings save it and copy it; nothing is uploaded) and closes it with SIGTERM, which must
# save the settings. It then sends an image to ShareX the way the browser extension does, through ShareX_NativeMessagingHost (a data
# URL, so nothing is downloaded), and checks that it was saved too. The settings skip the welcome screen, as for a returning user: on a first run it waits for the user
# before command line work.
#
#   Scripts/smoke-test-linux.sh COMMAND [ARGUMENTS...]     for example /usr/bin/sharex, or ShareX.AppImage
#
# Needs xvfb-run, dbus-run-session and python3 (Debian and Ubuntu: xvfb dbus-x11).
set -euo pipefail

export HOME="$(mktemp -d)"
mkdir -p "$HOME/Documents/ShareX"
echo '{ "ShowStartScreen": false }' > "$HOME/Documents/ShareX/ApplicationConfig.json"

# The host sits next to ShareX: /usr/bin/sharex and bin/sharex link to it, an extracted AppImage keeps it under usr/lib.
app="$(dirname "$(readlink -f "$(command -v "$1")")")"
[ -x "$app/ShareX_NativeMessagingHost" ] || app="$app/usr/lib/sharex/app"
export BROWSER_HOST="$app/ShareX_NativeMessagingHost"
[ -x "$BROWSER_HOST" ] || { echo "No ShareX_NativeMessagingHost next to $1." >&2; exit 1; }

xvfb-run -a -s "-screen 0 1280x800x24" dbus-run-session -- bash -euo pipefail -c '
  "$@" -silent & pid=$!
  logs="$HOME/Documents/ShareX/Logs"
  for i in $(seq 1 60); do
    grep -qs "Startup time" "$logs"/ShareX-Log-*.txt && break
    sleep 1
  done
  "$@" -PrintScreen
  for i in $(seq 1 30); do
    compgen -G "$HOME/Documents/ShareX/Screenshots/*/*" >/dev/null && break
    sleep 1
  done
  screenshots() { { compgen -G "$HOME/Documents/ShareX/Screenshots/*/*" || true; } | wc -l; }
  before=$(screenshots)
  # A 1x1 PNG, framed as Chrome frames native messages: a 4 byte length in native byte order, then the JSON. The host shows a
  # dialog when it fails, so it gets a time limit.
  message="{\"Action\":1,\"URL\":\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8DwHwAFBQIAX8jx0gAAAABJRU5ErkJggg==\"}"
  python3 -I -c "import struct, sys; m = sys.argv[1].encode(); sys.stdout.buffer.write(struct.pack(\"=I\", len(m)) + m)" "$message" |
    timeout 60 "$BROWSER_HOST" chrome-extension://nlkoigbdolhchiicbonbihbphgamnaoc/ >/dev/null || echo "ShareX_NativeMessagingHost failed." >&2
  for i in $(seq 1 30); do
    [ "$(screenshots)" -gt "$before" ] && break
    sleep 1
  done
  kill -TERM "$pid"
  wait "$pid" || true
  cat "$logs"/ShareX-Log-*.txt
  compgen -G "$HOME/Documents/ShareX/Screenshots/*/*" >/dev/null || { echo "No screenshot was saved." >&2; exit 1; }
  file "$HOME"/Documents/ShareX/Screenshots/*/*
  [ "$(screenshots)" -gt "$before" ] || { echo "The image sent through the browser extension host was not saved." >&2; exit 1; }
  test -s "$HOME/Documents/ShareX/ApplicationConfig.json"
  ! grep -q "Unhandled exception" "$logs"/ShareX-Log-*.txt
' smoke "$@"
