Run the graphics checks with the standard test runner:

```powershell
dotnet test ShareX.ImageEditor.Tests -c Release -p:Platform=x64
```

Font resolution and bundled cursor checks run on every operating system. System color emoji checks skip when no supported color font is installed. Tests use generated text and bundled cursor images.

Interactive cursor checks load the three embedded Avalonia cursor assets without a desktop session, compare their pixels with Skia's ICO decoder, and verify scaled hotspots from 125% to 250% DPI. Synthetic CUR fixtures cover PNG, monochrome masks and 32-bit alpha. Optional artifacts include `interactive-cursors-dpi.png`; magenta circles mark the cursor hotspots.

Optional font fixtures and contact sheets use environment variables:

```powershell
$env:SHAREX_TEST_EMOJI_FONT = 'C:\path\to\NotoColorEmoji.ttf'
$env:SHAREX_TEST_GRAPHICS_OUTPUT = 'C:\path\to\verification-output'
dotnet test ShareX.ImageEditor.Tests -c Release -p:Platform=x64
```

The font fixture test skips unless `SHAREX_TEST_EMOJI_FONT` is set. The contact-sheet test skips unless `SHAREX_TEST_GRAPHICS_OUTPUT` is set. No fonts are downloaded or installed by the tests.

Windows application checks use the real Release build output:

```powershell
dotnet build ShareX.sln -c Release -p:Platform=x64
$env:SHAREX_TEST_WINDOWS_APPLICATION_DIRECTORY = Join-Path (Get-Location) 'ShareX/bin/Release/win-x64'
$env:SHAREX_TEST_WINDOWS_SDK_PLATFORM_ASSEMBLY = Join-Path (Get-Location) 'ShareX.Platform.Windows/bin/Release/net10.0-windows10.0.22621.0/win-x64/ShareX.Platform.Windows.dll'
dotnet test ShareX.ImageEditor.Tests -c Release -p:Platform=x64
```

The artifact checks verify the application gets the Windows SDK platform assembly and use its native OCR service on generated text. The startup smoke test launches a temporary copy with synthetic portable settings, updates/uploads/hotkeys disabled, and `-silent -multi -portable -ExitShareX`. It verifies real application initialization, clean CLI shutdown, retained settings and closure of its empty SQLite history. The copy uses its own personal folder, skips file-type registration and sends no commands to a running ShareX instance. The test cleans up its process and files; no user images or settings are used. With `SHAREX_TEST_GRAPHICS_OUTPUT` set, it saves its own startup log as `windows-application-startup.log`.

These checks skip off Windows or when the application fixture variables are unset. They do not replace Windows 10/11 desktop, capture, upload, tray interaction or editor workflow verification.

Native Windows updater signature checks reject missing, malformed and unsigned fixtures. The trusted/tampered comparison uses an optional, already signed binary:

```powershell
$env:SHAREX_TEST_SIGNED_WINDOWS_BINARY = (Get-Command dotnet).Source
dotnet test ShareX.ImageEditor.Tests -c Release -p:Platform=x64
```

The test copies the supplied binary into its own temporary directory, changes one DOS-header byte in a second copy, and verifies trust through `WindowsCodeSignatureService` with the updater's standard Windows revocation policy. Repeated checks confirm that the signed copy is trusted, the modified copy is rejected, source bytes stay unchanged and fixture files can be opened exclusively afterwards. No fixture is executed and no certificate or trust setting is installed or changed. These tests skip off Windows; the signed comparison also skips when its fixture variable is unset.
