# HelpersLib localization status

HelpersLib uses the shared `Strings.resx` catalog in this directory for its Avalonia windows and newer user-facing components. Resource keys are scoped by their owning window or component.

| Area | Resource prefix | Keys | Status |
| --- | --- | ---: | --- |
| Color picker window | `ColorPickerWindow_` | 34 | Complete |
| Error window | `ErrorWindow_` | 10 | Complete |
| Image viewer window | `ImageViewerWindow_` | 2 | Complete |
| Input box window | `InputBoxWindow_` | 2 | Complete |
| Output box window | `OutputBoxWindow_` | 4 | Complete |
| Print window | `PrintWindow_` | 10 | Complete |
| Downloader window | `DownloaderWindow_` | 4 | Complete |
| Update message window | `UpdateMessageWindow_` | 1 | Complete |
| Pixel information code menu | `CodeMenuEntryPixelInfo_` | 22 | Complete |
| Runtime and helper messages | Component-scoped prefixes | 40 | Complete |

All 129 keys are translated in all 25 supported cultures. The existing `Properties/Resources.resx` catalog remains responsible for its 390 already-translated legacy strings and 11 bitmap/icon resources. The `Controls` and `Colors` directories are intentionally excluded from this localization catalog.

Run validation from the repository root:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File ValidateTranslations.ps1
```
