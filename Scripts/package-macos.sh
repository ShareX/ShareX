#!/usr/bin/env bash
# Builds ShareX.app for macOS and zips it.
#
#   Scripts/package-macos.sh [arm64|x64] [OUTPUT_DIR]     default: arm64 into ./artifacts
#
# Needs the .NET 10 SDK. Runs on macOS or Linux; on macOS the bundle is signed ad hoc with codesign, which Apple silicon requires
# before it runs any arm64 program. A bundle built elsewhere must be signed on a Mac first:
#   codesign --force --deep --sign - ShareX.app
# Users install by moving ShareX.app to /Applications. ShareX asks for Screen Recording permission the first time it captures.
set -euo pipefail

arch="${1:-arm64}"
out="${2:-artifacts}"
case "$arch" in
  arm64) rid=osx-arm64; platform=ARM64 ;;
  x64) rid=osx-x64; platform=x64 ;;
  *) echo "Unknown architecture '$arch' (arm64 or x64)" >&2; exit 2 ;;
esac

repo="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
version="$(sed -n 's:.*<Version>\(.*\)</Version>.*:\1:p' "$repo/Directory.Build.props" | head -1)"
staging="$(mktemp -d)"
trap 'rm -rf "$staging"' EXIT

echo "Building ShareX $version for $rid..."
dotnet publish "$repo/ShareX/ShareX.csproj" -c Release -r "$rid" --self-contained true \
  -p:Platform="$platform" -p:PublishTrimmed=false -p:DebugType=none -p:DebugSymbols=false -o "$staging/publish" >"$staging/build.log" 2>&1 \
  || { cat "$staging/build.log" >&2; exit 1; }

app="$staging/ShareX.app"
mkdir -p "$app/Contents/MacOS" "$app/Contents/Resources"
cp -R "$staging/publish/." "$app/Contents/MacOS/"
chmod +x "$app/Contents/MacOS/ShareX" "$app/Contents/MacOS/ShareX_NativeMessagingHost"

# A one-image .icns (256x256 PNG, type ic08) written directly, so no Apple tool is needed to build it.
python3 - "$repo/ShareX.HelpersLib/Resources/ShareX_Logo.png" "$app/Contents/Resources/ShareX.icns" <<'PY'
import struct, sys
png = open(sys.argv[1], 'rb').read()
chunk = b'ic08' + struct.pack('>I', len(png) + 8) + png
open(sys.argv[2], 'wb').write(b'icns' + struct.pack('>I', len(chunk) + 8) + chunk)
PY

cat >"$app/Contents/Info.plist" <<PLIST
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
  <key>CFBundleIdentifier</key><string>com.getsharex.ShareX</string>
  <key>CFBundleName</key><string>ShareX</string>
  <key>CFBundleDisplayName</key><string>ShareX</string>
  <key>CFBundleExecutable</key><string>ShareX</string>
  <key>CFBundleIconFile</key><string>ShareX</string>
  <key>CFBundlePackageType</key><string>APPL</string>
  <key>CFBundleShortVersionString</key><string>$version</string>
  <key>CFBundleVersion</key><string>$version</string>
  <key>CFBundleInfoDictionaryVersion</key><string>6.0</string>
  <key>LSMinimumSystemVersion</key><string>13.0</string>
  <key>LSApplicationCategoryType</key><string>public.app-category.productivity</string>
  <key>NSHighResolutionCapable</key><true/>
  <key>NSPrincipalClass</key><string>NSApplication</string>
  <key>NSMicrophoneUsageDescription</key><string>ShareX records audio with screen recordings when you choose a microphone.</string>
  <key>NSAppleEventsUsageDescription</key><string>ShareX reads the active window's title for file names and window capture.</string>
  <key>CFBundleDocumentTypes</key>
  <array>
    <dict>
      <key>CFBundleTypeName</key><string>ShareX custom uploader</string>
      <key>CFBundleTypeRole</key><string>Viewer</string>
      <key>LSHandlerRank</key><string>Owner</string>
      <key>CFBundleTypeExtensions</key><array><string>sxcu</string></array>
    </dict>
    <dict>
      <key>CFBundleTypeName</key><string>ShareX image effect</string>
      <key>CFBundleTypeRole</key><string>Viewer</string>
      <key>LSHandlerRank</key><string>Owner</string>
      <key>CFBundleTypeExtensions</key><array><string>sxie</string></array>
    </dict>
  </array>
</dict>
</plist>
PLIST

if command -v codesign >/dev/null; then
  codesign --force --deep --sign - "$app"
  signed="signed ad hoc"
else
  signed="NOT signed: run 'codesign --force --deep --sign - ShareX.app' on a Mac before it will start on Apple silicon"
fi

mkdir -p "$out"
zip_path="$(cd "$out" && pwd)/ShareX-$version-macos-$arch.zip"
rm -f "$zip_path"
if command -v ditto >/dev/null; then
  ditto -c -k --keepParent "$app" "$zip_path"
else
  (cd "$staging" && zip -qry "$zip_path" ShareX.app)
fi

echo "Package: $zip_path ($signed)"
