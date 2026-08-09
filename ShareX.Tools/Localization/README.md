# ShareX.Tools localization

`Strings.resx` is the default English catalog for the `ShareX.Tools` project. Each supported application culture has a complete satellite catalog named `Strings.<culture>.resx`.

The catalog contains 553 scoped keys covering all Avalonia views and user-visible runtime messages in the Tools project, including image analysis, background removal, borderless windows, clipboard viewing, directory indexing, hash checking, icon conversion, image combining/comparison/splitting/thumbnailing, window inspection, metadata, monitor testing, OCR, pinning, QR codes, the ruler, video conversion, and video thumbnailing.

Supported cultures: `ar-YE`, `cs-CZ`, `da-DK`, `de`, `es`, `es-MX`, `fa-IR`, `fr`, `he-IL`, `hi-IN`, `hu`, `id-ID`, `it-IT`, `ja-JP`, `ko-KR`, `nl-NL`, `pl`, `pt-BR`, `pt-PT`, `ro`, `ru`, `sv-SE`, `th-TH`, `tr`, `uk`, `vi-VN`, `zh-CN`, and `zh-TW`.

Run the repository-level `ValidateTranslations.ps1` from PowerShell to verify supported languages, entry counts, key completeness, placeholders, UTF-8 and CRLF formatting, generated designers, source references, and remaining literal Avalonia UI text across every localized project.
