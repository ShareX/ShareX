# Avalonia localization status

Avalonia translations use the shared `Strings.resx` resource set in this directory. Resource keys are scoped by the source component name, for example `TaskSettingsWindow_Save`.

## Localized areas

| Area | Resource prefixes | Source files | Keys | Status |
| --- | --- | ---: | ---: | --- |
| Main window | `MainWindow_`, `MainMenuBuilder_`, `ThumbnailItemViewModel_` | 4 | 142 | Complete |
| Application settings | `ApplicationSettingsWindow_` | 3 | 174 | Complete |
| Hotkey settings | `HotkeySettingsWindow_` | 3 | 23 | Complete |
| Task settings | `TaskSettingsWindow_` | 4 | 254 | Complete |
| Drag and drop upload | `DragDropUploadWindow_` | 2 | 2 | Complete |
| Clipboard upload | `ClipboardUploadWindow_` | 2 | 8 | Complete |
| Notification | None (content supplied dynamically) | 4 | 0 | Reviewed |
| File exists | `FileExistWindow_` | 2 | 8 | Complete |
| Large file upload warning | `LargeFileUploadWarningWindow_` | 2 | 5 | Complete |
| Multi-upload confirmation | `MultiUploadConfirmationWindow_` | 2 | 5 | Complete |
| Debug log | `DebugLogWindow_` | 2 | 9 | Complete |
| First-time configuration | `FirstTimeConfigWindow_` | 2 | 11 | Complete |
| Image editor selector | `ImageEditorSelectorWindow_` | 2 | 9 | Complete |
| Shorten URL | `ShortenURLWindow_` | 2 | 6 | Complete |
| URL upload | `URLUploadWindow_` | 2 | 7 | Complete |
| Before upload | `BeforeUploadWindow_` | 2 | 10 | Complete |

`Validate.ps1` contains the tracked source-file manifest. It verifies that:

- every tracked source file exists;
- every direct `Strings` reference has a default resource;
- every tracked resource key is referenced by its owning area;
- every localized `.resx` file has the same keys as `Strings.resx`;
- values are non-empty and composite-format placeholders match the default value;
- untracked Avalonia `.axaml` views are reported as the remaining localization work.

Run it from the repository root:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File ShareX/Localization/Validate.ps1
```

## Localizing another area

1. Add directly scoped keys to `Strings.resx`, such as `AboutWindow_Title`.
2. Add translated values to every `Strings.<culture>.resx` file.
3. Use direct `Strings.<key>` references in AXAML and C#.
4. Add the area, resource prefixes, and participating source files to `$areas` in `Validate.ps1`.
5. Run the validator and build ShareX.
