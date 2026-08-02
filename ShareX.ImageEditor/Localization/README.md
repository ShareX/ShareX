# Image Editor localization status

Image Editor translations use the shared `Strings.resx` resource set in this directory. Keys are scoped by their owning view or component.

Image effect names, descriptions, parameter labels, and options are intentionally excluded from localization.

## Localized areas

| Area | Resource prefix | Source files | Keys | Status |
| --- | --- | ---: | ---: | --- |
| Confirmation dialog | `ConfirmationDialogView_` | 2 | 5 | Complete |
| New image dialog | `NewImageDialogView_` | 1 | 9 | Complete |
| Start screen | `StartScreenDialogView_` | 2 | 11 | Complete |

`Validate.ps1` checks source references, key parity, values, placeholders, and reports the remaining untracked AXAML views.

Run from the repository root:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File ShareX.ImageEditor/Localization/Validate.ps1
```
