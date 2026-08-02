# ScreenCaptureLib localization status

ScreenCaptureLib uses the shared `Strings.resx` catalog in this directory for its Avalonia windows. Resource keys are scoped by their owning window.

| Area | Resource prefix | Keys | Status |
| --- | --- | ---: | --- |
| FFmpeg options window | `FFmpegOptionsWindow_` | 57 | Complete |
| Screen recording toolbar | `ScreenRecordWindow_` | 6 | Complete |
| Scrolling capture windows | `ScrollingCaptureWindow_` | 27 | Complete |

All 90 keys are translated in all 23 supported cultures. Existing WinForms views retain their form-specific `.resx` catalogs, while the legacy `Properties/Resources.resx` catalog remains responsible for its bitmap/cursor assets and already-localized legacy strings.

Run validation from the repository root:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File ShareX.ScreenCaptureLib/Localization/Validate.ps1
```
