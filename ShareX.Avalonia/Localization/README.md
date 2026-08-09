# ShareX.Avalonia localization status

ShareX.Avalonia uses the shared `Strings.resx` catalog in this directory for its user-facing interface and runtime diagnostics. Resource keys are scoped by their owning component.

| Area | Resource prefix | Keys | Status |
| --- | --- | ---: | --- |
| Settings navigation | `SettingsNavigation_` | 1 | Complete |
| Cursor asset diagnostics | `CursorAssetLoader_` | 8 | Complete |

All 9 keys are translated in all 27 supported cultures. Theme identifiers, asset URIs, font names, format tokens such as `$HEX`, and framework known-color names are stable technical data and are intentionally excluded.

The repository-level `ValidateTranslations.ps1` checks the supported-culture inventory, entry counts, key parity, non-empty values, format placeholders, UTF-8 and CRLF formatting, generated designers, source references, and remaining literal Avalonia UI text across every localized project.

Run validation from the repository root:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File ValidateTranslations.ps1
```
