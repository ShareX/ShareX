# ShareX.Avalonia localization status

ShareX.Avalonia uses the shared `Strings.resx` catalog in this directory for its user-facing interface and runtime diagnostics. Resource keys are scoped by their owning component.

| Area | Resource prefix | Keys | Status |
| --- | --- | ---: | --- |
| Settings navigation | `SettingsNavigation_` | 1 | Complete |
| Cursor asset diagnostics | `CursorAssetLoader_` | 8 | Complete |

All 9 keys are translated in all 23 supported cultures. Theme identifiers, asset URIs, font names, format tokens such as `$HEX`, and framework known-color names are stable technical data and are intentionally excluded.

`Validate.ps1` checks the supported-culture inventory, key and ordering parity, non-empty values, composite-format placeholders, strict UTF-8 and CRLF formatting, unintended English fallbacks, source references, and remaining literal Avalonia UI text.

Run validation from the repository root:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File ShareX.Avalonia/Localization/Validate.ps1
```
