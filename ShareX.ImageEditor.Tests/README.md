Run the graphics checks with the standard test runner:

```powershell
dotnet test ShareX.ImageEditor.Tests -c Release -p:Platform=x64
```

Font resolution and bundled cursor checks run on every operating system. System color emoji checks skip when no supported color font is installed. Tests use generated text and bundled cursor images.

Optional font fixtures and contact sheets use environment variables:

```powershell
$env:SHAREX_TEST_EMOJI_FONT = 'C:\path\to\NotoColorEmoji.ttf'
$env:SHAREX_TEST_GRAPHICS_OUTPUT = 'C:\path\to\verification-output'
dotnet test ShareX.ImageEditor.Tests -c Release -p:Platform=x64
```

The font fixture test skips unless `SHAREX_TEST_EMOJI_FONT` is set. The contact-sheet test skips unless `SHAREX_TEST_GRAPHICS_OUTPUT` is set. No fonts are downloaded or installed by the tests.
