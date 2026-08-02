# Image Editor localization status

Image Editor translations use the shared `Strings.resx` resource set in this directory. Keys are scoped by their owning view or component.

Image effect names, descriptions, parameter labels, and options are intentionally excluded from localization.

## Localized areas

| Area | Resource prefix | Source files | Keys | Status |
| --- | --- | ---: | ---: | --- |
| Confirmation dialog | `ConfirmationDialogView_` | 2 | 5 | Complete |
| New image dialog | `NewImageDialogView_` | 1 | 9 | Complete |
| Start screen | `StartScreenDialogView_` | 2 | 11 | Complete |
| Emoji picker | `EmojiPickerDialogView_` | 2 | 9 | 20 cultures complete; Arabic, Persian, and Hebrew pending |
| Insert image dialog | `InsertImageDialogView_` | 2 | 9 | 20 cultures complete; Arabic, Persian, and Hebrew pending |
| Annotation toolbar | `AnnotationToolbar_` | 1 | 38 | 20 cultures complete; Arabic, Persian, and Hebrew pending |
| Color picker panel | `ColorPickerPanel_` | 1 | 1 | 20 cultures complete; Arabic, Persian, and Hebrew pending |
| Editor options panel | `EditorOptionsPanel_` | 1 | 3 | 20 cultures complete; Arabic, Persian, and Hebrew pending |
| Editor view | `EditorView_` | 1 | 33 | 20 cultures complete; Arabic, Persian, and Hebrew pending |
| Effect browser shell | `EffectBrowserPanel_` | 2 | 6 | 20 cultures complete; effect metadata excluded |
| Effect dialog shell | `SchemaDrivenEffectDialog_` | 1 | 3 | 20 cultures complete; effect metadata excluded |
| Toolbar customization dialog | `ToolbarCustomizationDialogView_` | 2 | 10 | 20 cultures complete; Arabic, Persian, and Hebrew pending |
| Zoom picker | `ZoomPickerDropdown_` | 1 | 1 | 20 cultures complete; Arabic, Persian, and Hebrew pending |

The current Image Editor resource set contains 138 keys. The latest batch added 113 scoped UI keys. Twenty cultures are complete for this batch; Arabic, Persian, and Hebrew retain .NET's default-resource fallback until their translations are completed, rather than duplicating English text into localized files.

`Validate.ps1` checks source references, key parity, values, placeholders, and reports the remaining untracked AXAML views.

Run from the repository root:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File ShareX.ImageEditor/Localization/Validate.ps1
```
