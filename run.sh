#!/usr/bin/env bash
set -e
DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

if [ -z "$DISPLAY" ] && command -v systemctl >/dev/null 2>&1; then
    eval $(systemctl --user show-environment 2>/dev/null | grep -E '^(DISPLAY|WAYLAND_DISPLAY|XAUTHORITY|XDG_RUNTIME_DIR)=' | sed 's/^/export /')
fi

export TMPDIR="/tmp"

if [ "$1" = "--restart" ] || [ "$1" = "-r" ]; then
    pkill -f 'ShareX.dll' 2>/dev/null || true
    rm -f /tmp/CoreFxPipe_*-ShareX 2>/dev/null || true
    sleep 0.5
    shift
fi

TARGET_DLL=""
for candidate in \
    "$DIR/ShareX/bin/Debug/ShareX.dll" \
    "$DIR/source/ShareX/bin/Debug/ShareX.dll" \
    "$DIR/ShareX/bin/Release/ShareX.dll" \
    "$DIR/source/ShareX/bin/Release/ShareX.dll" \
    "$DIR/ShareX/bin/Debug/net10.0/ShareX.dll" \
    "$DIR/source/ShareX/bin/Debug/net10.0/ShareX.dll" \
    "$DIR/ShareX/bin/Release/net10.0/ShareX.dll" \
    "$DIR/source/ShareX/bin/Release/net10.0/ShareX.dll" \
    "$DIR/ShareX/bin/Debug/net10.0/linux-x64/ShareX.dll" \
    "$DIR/ShareX/bin/Release/net10.0/linux-x64/ShareX.dll"; do
    if [ -f "$candidate" ]; then
        TARGET_DLL="$candidate"
        break
    fi
done

if [ -z "$TARGET_DLL" ]; then
    echo "Erro: ShareX.dll não foi encontrado a partir de $DIR." >&2
    exit 1
fi

exec dotnet "$TARGET_DLL" "$@"
