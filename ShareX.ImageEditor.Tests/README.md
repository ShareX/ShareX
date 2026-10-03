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
