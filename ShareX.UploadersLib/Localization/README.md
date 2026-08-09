# UploadersLib localization status

UploadersLib uses the shared `Strings.resx` catalog in this directory for all translatable user-interface and runtime text. Resource keys are scoped by their owning window, uploader, enum, or component.

| Area | Resource prefixes | Keys | Status |
| --- | --- | ---: | --- |
| Destination settings | `DestinationSettings_`, `DestinationSettingsWindow_` | 209 | Complete |
| Dialog windows | `EmailWindow_`, `OAuthListenerWindow_`, `ParserSelectWindow_`, `PuushLoginWindow_`, `ResponseWindow_`, `TextUploadWindow_`, `YouTubeVideoOptionsWindow_` | 64 | Complete |
| Destination, protocol, privacy, and format names | `FileDestination_`, `ImageDestination_`, `TextDestination_`, `UrlShortenerType_`, and related enum prefixes | 147 | Complete |
| Uploader errors and runtime messages | Uploader and helper component prefixes | 73 | Complete |

All 493 keys are available in the default English catalog and all 27 supported culture catalogs. Composite-format placeholders are kept in parity across every translation.

The repository-level `ValidateTranslations.ps1` checks supported languages, entry counts, key parity, non-empty values, format placeholders, UTF-8 and CRLF formatting, generated designers, source references, data-driven resource prefixes, and remaining literal Avalonia UI text across every localized project.

The existing `Properties/Resources.resx` catalog remains responsible for uploader icons, images, and the OAuth callback page asset. Translatable text is stored in this directory.

When adding or changing text:

1. Add a scoped key to `Strings.resx`.
2. Add the translated value to every `Strings.<culture>.resx` catalog.
3. Use direct `Strings.<key>` references in C# or `x:Static localization:Strings.<key>` in AXAML.
4. Preserve any composite-format placeholders from the English value.
5. Build `ShareX.UploadersLib` to verify the resources and AXAML.

Run validation and the project build from the repository root:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File ValidateTranslations.ps1
dotnet build ShareX.UploadersLib/ShareX.UploadersLib.csproj --no-restore
```
