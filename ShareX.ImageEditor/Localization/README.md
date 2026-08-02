# Image Editor localization status

Image Editor translations use the shared `Strings.resx` resource set in this directory. Keys are scoped by their owning view or component.

Image effect names, descriptions, parameter labels, and options are intentionally excluded from localization.

## Localized areas

| Area | Resource prefix | Source files | Keys | Status |
| --- | --- | ---: | ---: | --- |
| Confirmation dialog | `ConfirmationDialogView_` | 2 | 5 | Complete |
| New image dialog | `NewImageDialogView_` | 1 | 9 | Complete |
| Start screen | `StartScreenDialogView_` | 2 | 11 | Complete |
| Emoji picker | `EmojiPickerDialogView_` | 2 | 9 | Complete |
| Insert image dialog | `InsertImageDialogView_` | 2 | 9 | Complete |
| Annotation toolbar | `AnnotationToolbar_` | 1 | 38 | Complete |
| Color picker panel | `ColorPickerPanel_` | 1 | 1 | Complete |
| Editor options panel | `EditorOptionsPanel_` | 1 | 3 | Complete |
| Editor view | `EditorView_` | 1 | 33 | Complete |
| Effect browser shell | `EffectBrowserPanel_` | 2 | 6 | Complete; effect metadata excluded |
| Effect dialog shell | `SchemaDrivenEffectDialog_` | 1 | 3 | Complete; effect metadata excluded |
| Toolbar customization dialog | `ToolbarCustomizationDialogView_` | 2 | 10 | Complete |
| Zoom picker | `ZoomPickerDropdown_` | 1 | 1 | Complete |

The Image Editor resource set contains 245 keys. The first 138 keys are complete in all 23 cultures. The latest batch adds 107 scoped UI keys; Turkish is complete and exact translations from the current project were propagated to the remaining cultures. Unmatched values retain .NET's default-resource fallback until the next language pass, rather than duplicating English text into localized files.

`Validate.ps1` checks source references, key parity, values, placeholders, and reports the remaining untracked AXAML views.

Run from the repository root:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File ShareX.ImageEditor/Localization/Validate.ps1
```
