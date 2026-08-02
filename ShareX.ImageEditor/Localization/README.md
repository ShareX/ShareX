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
| Emoji catalog | `EmojiCatalog_` | 3 | 1604 | Turkish complete; other cultures use English fallback |
| Insert image dialog | `InsertImageDialogView_` | 2 | 9 | Complete |
| Annotation toolbar | `AnnotationToolbar_` | 1 | 38 | Complete |
| Color picker panel | `ColorPickerPanel_` | 1 | 1 | Complete |
| Editor options panel | `EditorOptionsPanel_` | 1 | 11 | Complete |
| Editor view | `EditorView_` | 5 | 46 | Complete |
| Effect browser shell | `EffectBrowserPanel_` | 2 | 6 | Complete; effect metadata excluded |
| Effect dialog shell | `SchemaDrivenEffectDialog_` | 1 | 3 | Complete; effect metadata excluded |
| Toolbar customization dialog | `ToolbarCustomizationDialogView_` | 2 | 11 | Complete |
| Zoom picker | `ZoomPickerDropdown_` | 1 | 1 | Complete |
| Toolbar items | `ToolbarCustomizationItemViewModel_` | 1 | 24 | Complete |
| Main editor commands and status | `MainViewModel_` | 4 | 33 | Complete |
| Cursor type names | `CursorTypeDisplayNameConverter_` | 1 | 28 | Complete |
| Border style names | `BorderStyleDisplayConverter_` | 1 | 5 | Complete |
| Arrow style names | `ArrowStyleDisplayNameConverter_` | 1 | 5 | Complete |
| Text alignment names | `TextHorizontalAlignmentHelper_` | 1 | 3 | Complete |

The default Image Editor resource set contains 1862 keys. Turkish contains all 1862 keys, including the complete 1604-key emoji catalog. The other 22 cultures contain all 258 ordinary UI keys and intentionally use the default English fallback for emoji catalog names and categories.

`Validate.ps1` checks source references, the Turkish-only emoji policy, key parity, values, placeholders, strict UTF-8 encoding, the emoji catalog JSON, and reports the remaining untracked AXAML views.

Run from the repository root:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File ShareX.ImageEditor/Localization/Validate.ps1
```
