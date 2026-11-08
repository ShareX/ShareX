#!/usr/bin/env bash
# Starts an installed ShareX in a clean home on a virtual X server with its own D-Bus session, takes a full screen screenshot
# through the command line (default settings save it and copy it; nothing is uploaded) and closes it with SIGTERM, which must
# save the settings. The settings skip the welcome screen, as for a returning user: on a first run it waits for the user
# before command line work.
#
#   Scripts/smoke-test-linux.sh COMMAND [ARGUMENTS...]     for example /usr/bin/sharex, or ShareX.AppImage
#
# Needs xvfb-run and dbus-run-session (Debian and Ubuntu: xvfb dbus-x11).
set -euo pipefail

export HOME="$(mktemp -d)"
mkdir -p "$HOME/Documents/ShareX"
echo '{ "ShowStartScreen": false }' > "$HOME/Documents/ShareX/ApplicationConfig.json"

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
  kill -TERM "$pid"
  wait "$pid" || true
  cat "$logs"/ShareX-Log-*.txt
  compgen -G "$HOME/Documents/ShareX/Screenshots/*/*" >/dev/null || { echo "No screenshot was saved." >&2; exit 1; }
  file "$HOME"/Documents/ShareX/Screenshots/*/*
  test -s "$HOME/Documents/ShareX/ApplicationConfig.json"
  ! grep -q "Unhandled exception" "$logs"/ShareX-Log-*.txt
' smoke "$@"
