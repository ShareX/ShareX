# Image Editor localization status

Image Editor translations use the shared `Strings.resx` resource set in this directory. Keys are scoped by their owning view or component.

Image effect names and browser categories are localized. Effect descriptions, parameter labels, and options are intentionally excluded from localization.

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
| Editor options panel | `EditorOptionsPanel_` | 1 | 11 | Complete |
| Editor view | `EditorView_` | 5 | 46 | Complete |
| Effect browser | `EffectBrowserPanel_` | 3 | 241 | Complete; names and categories included, descriptions and options excluded |
| Effect dialog shell | `SchemaDrivenEffectDialog_` | 1 | 3 | Complete; effect metadata excluded |
| Toolbar customization dialog | `ToolbarCustomizationDialogView_` | 2 | 11 | Complete |
| Zoom picker | `ZoomPickerDropdown_` | 1 | 1 | Complete |
| Toolbar items | `ToolbarCustomizationItemViewModel_` | 1 | 24 | Complete |
| Main editor commands and status | `MainViewModel_` | 4 | 33 | Complete |
| Cursor type names | `CursorTypeDisplayNameConverter_` | 1 | 28 | Complete |
| Border style names | `BorderStyleDisplayConverter_` | 1 | 5 | Complete |
| Arrow style names | `ArrowStyleDisplayNameConverter_` | 1 | 5 | Complete |
| Text alignment names | `TextHorizontalAlignmentHelper_` | 1 | 3 | Complete |

The default Image Editor resource set contains 493 keys, with matching translations in all 27 localized cultures. Emoji catalog names and categories come directly from the embedded English catalog and are intentionally excluded from localization.

The repository-level `ValidateTranslations.ps1` checks supported languages, entry counts, key parity, values, placeholders, UTF-8 and CRLF formatting, generated designers, source references, and remaining literal Avalonia UI text across every localized project.

Run from the repository root:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File ValidateTranslations.ps1
```
