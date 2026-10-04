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

The application upload checks use the same isolated copy with a generated binary file or PNG. Their synthetic settings enable only a custom uploader pointing at the fixture's .NET HTTP server, which binds to IPv4 loopback on an ephemeral port. Updates, hotkeys, proxy, clipboard actions, notifications and sounds are disabled. The source file comes first in the CLI arguments so it is parsed as an upload rather than a flag parameter; `-AutoClose` exits after the task completes. Checks compare every received/source byte, validate the returned local URL and exactly one persisted SQLite history row through a read-only connection, and confirm that source/history files are released after exit. No user data or third-party upload destination is used. Optional logs are `windows-application-binary-upload.log` and `windows-application-image-upload.log`.

These checks skip off Windows or when the application fixture variables are unset. They do not replace Windows 10/11 desktop, capture, tray interaction or editor workflow verification.

Native Windows session checks create only their own hidden top-level windows on an STA thread. Direct messages to those windows verify synchronous restart-query registration, cancelled queries without save/close, confirmed session-end timing, exception routing, thread ownership and disposal without changing foreground focus. A native restart-settings round trip verifies `-silent`, Unicode arguments and zero flags, restoring the test process's previous restart registration afterwards. Nothing is broadcast and no user session is ended. These checks skip off Windows and do not replace interactive Windows 10/11 shutdown/update verification.

Native Windows tray checks use generated transparent PNGs, owned hidden receiver windows and a fake shell notification callback. They verify all three buttons' press/release/double-click messages, native close requests and exception routing, system double-click timing, tooltip truncation, visibility/add/modify/delete ordering and recovery from a synthetic `TaskbarCreated` message or failed update. Native icon/window lifetime checks cover failed replacement, wrong-thread access, repeated cleanup and unchanged USER/GDI resource counts. They never add an icon to the actual notification area or restart Explorer. The Avalonia fallback capability check runs on every OS; the native checks skip off Windows. Real tray interaction, theme and display scaling still need desktop verification.

`WindowsApplicationLaunchTests` execute private copies of this test project's generated apphost. Only the copied PE subsystem is changed to keep the fixture hidden. Generated argument data covers empty values, Unicode, spaces, quotes, backslashes and shell metacharacters. A fixture parent joins its own `BREAKAWAY_OK` / `KILL_ON_JOB_CLOSE` job; the child must escape that specific job and finish after job closure terminates the parent. An isolated fixture checks process/thread handle counts across repeated successful and failed launches, after runtime warmup. No browser, installed ShareX, user data, desktop configuration or third-party service is involved. Native checks skip off Windows; the unsupported capability check runs everywhere. The custom test entry point is inert without fixture arguments and does not affect xUnit discovery.

With `SHAREX_TEST_WINDOWS_APPLICATION_DIRECTORY` set, two additional checks run the built native-messaging host in that private directory, with the generated fixture apphost named `ShareX.exe`. A length-prefixed UTF-8 synthetic payload must be echoed byte for byte and delivered unchanged through the temporary JSON and separate `-NativeMessagingInput` arguments. The fixture child removes its own input file. A zero-length message must exit without echoing or starting a child. These tests do not launch the real ShareX application or a browser.

Native Windows updater signature checks reject missing, malformed and unsigned fixtures. The trusted/tampered comparison uses an optional, already signed binary:

```powershell
$env:SHAREX_TEST_SIGNED_WINDOWS_BINARY = (Get-Command dotnet).Source
dotnet test ShareX.ImageEditor.Tests -c Release -p:Platform=x64
```

The test copies the supplied binary into its own temporary directory, changes one DOS-header byte in a second copy, and verifies trust through `WindowsCodeSignatureService` with the updater's standard Windows revocation policy. Repeated checks confirm that the signed copy is trusted, the modified copy is rejected, source bytes stay unchanged and fixture files can be opened exclusively afterwards. No fixture is executed and no certificate or trust setting is installed or changed. These tests skip off Windows; the signed comparison also skips when its fixture variable is unset.

Auto-capture, FFmpeg device-action and scrolling UI support regressions run in this project on every host. They inject capabilities and controlled asynchronous callbacks; auto-capture frames are generated one-pixel Skia bitmaps. They do not open selectors, capture the desktop, discover devices, download packages or upload data. Native Windows device-capability parity is separately gated to Windows.

Recording-window lifetime tests also run on every host using injected dispatch queues and fixture-owned .NET events. They cover callbacks queued before closure, a synchronous invoke awaiting UI execution, reentrant signals and repeated cleanup ownership. They do not construct a recording window or run a recording backend; recording-worker completion and real desktop shutdown need separate verification.

Capture-frame scaling tests exercise the shared presentation geometry at fractional render DPI, across toolbar/scale changes and at negative origins. They verify unchanged physical recording bounds and native frame-mask dimensions without constructing windows or changing display settings. Actual mixed-monitor rendering and compositor coordinate mapping remain separate desktop checks.

Editor image-import lifetime tests (B40) run on every host through the same portable operation, owned-image and stream-reader helpers used by the editor. Controlled providers and generated Skia images exercise late results after close, unload, document/owner changes and removed replacement targets; blocked stream and placement waits; download timeout reporting; accepted Core/annotation ownership; rejected imports and failures before/after transfer. They do not open file pickers, read the real clipboard, construct windows or contact a network service. Actual picker, clipboard, drop, URL and canvas-expansion interaction still requires desktop verification.

Editor cut tests (B41) exercise the actual portable cut controller and Core history with generated annotations and controlled clipboard-clear tasks. Cut copies and removes its original target before any clipboard await; later selections/documents survive, leaving the editor releases the wait, clipboard failures keep the accepted edit, and undo/redo and step renumbering retain the existing Core behavior. Image clipboard clones and undo snapshots have independent ownership. These tests use no real clipboard, window or input; actual cut/paste interaction remains a desktop check.