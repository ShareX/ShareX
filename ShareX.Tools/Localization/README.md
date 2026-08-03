# ShareX.Tools localization

`Strings.resx` is the default English catalog for the `ShareX.Tools` project. Each supported application culture has a complete satellite catalog named `Strings.<culture>.resx`.

The catalog contains 553 scoped keys covering all Avalonia views and user-visible runtime messages in the Tools project, including image analysis, background removal, borderless windows, clipboard viewing, directory indexing, hash checking, icon conversion, image combining/comparison/splitting/thumbnailing, window inspection, metadata, monitor testing, OCR, pinning, QR codes, the ruler, video conversion, and video thumbnailing.

Supported cultures: `ar-YE`, `de`, `es`, `es-MX`, `fa-IR`, `fr`, `he-IL`, `hu`, `id-ID`, `it-IT`, `ja-JP`, `ko-KR`, `nl-NL`, `pl`, `pt-BR`, `pt-PT`, `ro`, `ru`, `tr`, `uk`, `vi-VN`, `zh-CN`, and `zh-TW`.

Run `Localization/Validate.ps1` from PowerShell to verify key completeness, placeholders, UTF-8 encoding, CRLF line endings, source references, and remaining literal Avalonia UI text.
