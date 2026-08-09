# HistoryLib localization status

HistoryLib uses the shared `Strings.resx` catalog in this directory. Resource keys are scoped by their owning window or component.

| Area | Resource prefix | Keys | Status |
| --- | --- | ---: | --- |
| Shared history-window UI | `HistoryWindows_` | 87 | Complete |
| History window runtime messages | `HistoryWindow_` | 21 | Complete |
| Image-history runtime messages | `ImageHistoryWindow_` | 22 | Complete |
| Statistics output | `HistoryHelpers_` | 7 | Complete |
| History manager errors | `HistoryManager_` | 2 | Complete |

All 139 keys are translated in all 26 supported cultures. The former `Properties/Resources` catalog was consolidated here.

Run validation from the repository root:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File ValidateTranslations.ps1
```
