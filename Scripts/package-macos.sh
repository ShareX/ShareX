#!/usr/bin/env bash
# Builds ShareX.app for macOS and packages it as a zip and, on a Mac, a disk image.
#
#   Scripts/package-macos.sh [arm64|x64] [OUTPUT_DIR]     default: arm64 into ./artifacts
#
# Needs the .NET 10 SDK. Runs on macOS or Linux. On a Mac the bundle is signed with codesign: ad hoc by default, which Apple
# silicon requires before it runs any arm64 program, or for release with a Developer ID and the hardened runtime when
# MACOS_SIGN_IDENTITY is set (for example "Developer ID Application: ShareX Team (TEAMID)", already in the keychain).
# A signed build is notarized and stapled when one of these is set as well:
#   MACOS_NOTARY_PROFILE                                   a notarytool keychain profile (xcrun notarytool store-credentials)
#   MACOS_NOTARY_KEY, MACOS_NOTARY_KEY_ID, MACOS_NOTARY_ISSUER   an App Store Connect API key (.p8 path), its id and issuer
# A bundle built off a Mac must be signed on one first:  codesign --force --deep --sign - ShareX.app
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
      <key>LSItemContentTypes</key><array><string>com.getsharex.sxcu</string></array>
    </dict>
    <dict>
      <key>CFBundleTypeName</key><string>ShareX image effect</string>
      <key>CFBundleTypeRole</key><string>Viewer</string>
      <key>LSHandlerRank</key><string>Owner</string>
      <key>LSItemContentTypes</key><array><string>com.getsharex.sxie</string></array>
    </dict>
    <!-- Any file can be opened with ShareX (Open With, or dropped on the Dock icon) to upload it. -->
    <dict>
      <key>CFBundleTypeName</key><string>File to upload</string>
      <key>CFBundleTypeRole</key><string>Viewer</string>
      <key>LSHandlerRank</key><string>Alternate</string>
      <key>LSItemContentTypes</key><array><string>public.item</string></array>
    </dict>
  </array>
  <key>UTExportedTypeDeclarations</key>
  <array>
    <dict>
      <key>UTTypeIdentifier</key><string>com.getsharex.sxcu</string>
      <key>UTTypeDescription</key><string>ShareX custom uploader</string>
      <key>UTTypeConformsTo</key><array><string>public.json</string></array>
      <key>UTTypeTagSpecification</key>
      <dict>
        <key>public.filename-extension</key><array><string>sxcu</string></array>
        <key>public.mime-type</key><array><string>application/x-sharex-custom-uploader</string></array>
      </dict>
    </dict>
    <dict>
      <key>UTTypeIdentifier</key><string>com.getsharex.sxie</string>
      <key>UTTypeDescription</key><string>ShareX image effect</string>
      <key>UTTypeConformsTo</key><array><string>public.data</string></array>
      <key>UTTypeTagSpecification</key>
      <dict>
        <key>public.filename-extension</key><array><string>sxie</string></array>
        <key>public.mime-type</key><array><string>application/x-sharex-image-effect</string></array>
      </dict>
    </dict>
  </array>
</dict>
</plist>
PLIST

notarytool() {
  if [ -n "${MACOS_NOTARY_PROFILE:-}" ]; then
    xcrun notarytool "$@" --keychain-profile "$MACOS_NOTARY_PROFILE"
  else
    xcrun notarytool "$@" --key "$MACOS_NOTARY_KEY" --key-id "$MACOS_NOTARY_KEY_ID" --issuer "$MACOS_NOTARY_ISSUER"
  fi
}

# Submits a file and polls for Apple's verdict, retrying through network drops (notarytool --wait gives up on the first one).
# Returns 0 when accepted, 1 when rejected (the log is printed), and 2 when Apple is still reviewing after
# MACOS_NOTARY_TIMEOUT seconds: a new Developer ID team's first submissions can take hours. The file is signed either way, and
# Gatekeeper asks Apple online, so it opens without a warning as soon as the review passes; only the stapled ticket is missing.
notarize() {
  local id status deadline=$((SECONDS + ${MACOS_NOTARY_TIMEOUT:-1200}))
  id="$(notarytool submit "$1" --output-format plist | plutil -extract id raw -)"
  echo "Notarization submission $id for $(basename "$1")"
  while [ "$SECONDS" -lt "$deadline" ]; do
    sleep 30
    status="$(notarytool info "$id" --output-format plist 2>/dev/null | plutil -extract status raw - 2>/dev/null)" || continue
    case "$status" in
      Accepted) echo "Notarization accepted"; return 0 ;;
      Invalid|Rejected) echo "Notarization $status" >&2; notarytool log "$id" >&2 || true; return 1 ;;
    esac
  done
  echo "::warning::Apple is still reviewing notarization submission $id; the build is signed but not stapled"
  return 2
}
can_notarize() { [ -n "${MACOS_NOTARY_PROFILE:-}" ] || [ -n "${MACOS_NOTARY_KEY:-}" ]; }

if ! command -v codesign >/dev/null; then
  signed="NOT signed: run 'codesign --force --deep --sign - ShareX.app' on a Mac before it will start on Apple silicon"
elif [ -n "${MACOS_SIGN_IDENTITY:-}" ]; then
  # Inside out, as Apple asks (--deep is not used for release signing): every file in Contents/MacOS, then the bundle, whose main
  # executable gets the entitlements. Mach-O files (native libraries, the browser host, .NET's createdump) get the hardened
  # runtime. .NET keeps its managed assemblies there too, and codesign counts every file in Contents/MacOS as code, so the
  # .dll and .json files are signed as well (the signature goes into extended attributes, which ditto and the DMG keep);
  # otherwise `codesign --verify --strict` rejects the bundle ("code object is not signed at all").
  entitlements="$repo/Scripts/macos/ShareX.entitlements"
  find "$app/Contents/MacOS" -type f ! -path "$app/Contents/MacOS/ShareX" -print0 | while IFS= read -r -d '' file; do
    if file -b "$file" | grep -q '^Mach-O'; then
      codesign --force --timestamp --options runtime --entitlements "$entitlements" --sign "$MACOS_SIGN_IDENTITY" "$file"
    else
      codesign --force --timestamp --sign "$MACOS_SIGN_IDENTITY" "$file"
    fi
  done
  codesign --force --timestamp --options runtime --entitlements "$entitlements" --sign "$MACOS_SIGN_IDENTITY" "$app"
  codesign --verify --strict --deep "$app"
  signed="signed by $MACOS_SIGN_IDENTITY"
else
  codesign --force --deep --sign - "$app"
  signed="signed ad hoc"
fi

mkdir -p "$out"
out="$(cd "$out" && pwd)"

# The disk image opens to ShareX.app beside a link to /Applications to drag it onto.
if command -v hdiutil >/dev/null; then
  dmg_path="$out/ShareX-$version-macos-$arch.dmg"
  mkdir "$staging/dmg"
  cp -R "$app" "$staging/dmg/"
  ln -s /Applications "$staging/dmg/Applications"
  rm -f "$dmg_path"
  hdiutil create -quiet -volname "ShareX $version" -srcfolder "$staging/dmg" -fs HFS+ -format UDZO "$dmg_path"
  if [ -n "${MACOS_SIGN_IDENTITY:-}" ]; then
    codesign --force --timestamp --sign "$MACOS_SIGN_IDENTITY" "$dmg_path"
    # One submission covers the disk image and the app inside it, so the app is stapled from the same ticket before it is zipped.
    if can_notarize; then
      rc=0; notarize "$dmg_path" || rc=$?
      case $rc in
        0) xcrun stapler staple "$dmg_path"; xcrun stapler staple "$app"; signed="$signed, notarized" ;;
        2) signed="$signed, notarization pending" ;;
        *) exit 1 ;;
      esac
    fi
  fi
  echo "Disk image: $dmg_path ($signed)"
fi

zip_path="$out/ShareX-$version-macos-$arch.zip"
rm -f "$zip_path"
if command -v ditto >/dev/null; then
  ditto -c -k --keepParent "$app" "$zip_path"
else
  (cd "$staging" && zip -qry "$zip_path" ShareX.app)
fi
echo "Package: $zip_path ($signed)"

