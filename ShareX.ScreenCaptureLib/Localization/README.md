# ScreenCaptureLib localization status

ScreenCaptureLib uses the shared `Strings.resx` catalog in this directory for all translatable strings. Resource keys are scoped by their owning view or component.

| Area | Resource prefix | Keys | Status |
| --- | --- | ---: | --- |
| FFmpeg options window | `FFmpegOptionsWindow_` | 116 | Complete |
| Screen recording toolbar | `ScreenRecordWindow_` | 6 | Complete |
| Scrolling capture windows | `ScrollingCaptureWindow_` | 27 | Complete |
| Region capture | `BaseRegionForm_`, `RegionCaptureWindow_` | 2 | Complete |

All catalog keys are available in all supported cultures. Remaining WinForms views retain their form-specific `.resx` catalogs.

Run validation from the repository root:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File ValidateTranslations.ps1
```
