# ScreenCaptureLib localization status

ScreenCaptureLib uses the shared `Strings.resx` catalog in this directory for all translatable strings. New resource keys are scoped by their owning view or component; migrated legacy keys retain their existing names.

| Area | Resource prefix | Keys | Status |
| --- | --- | ---: | --- |
| FFmpeg options window | `FFmpegOptionsWindow_` | 116 | Complete |
| Screen recording toolbar | `ScreenRecordWindow_` | 6 | Complete |
| Scrolling capture windows | `ScrollingCaptureWindow_` | 27 | Complete |
| Region capture and image editor | Legacy keys and `ShapeManager_` | 134 | Complete |

All 283 keys are available in all 24 supported cultures. Existing WinForms views retain their form-specific `.resx` catalogs, while `Properties/Resources.resx` is limited to bitmap and cursor assets.

Run validation from the repository root:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File ValidateTranslations.ps1
```
