# Avalonia localization status

Avalonia translations use the shared `Strings.resx` resource set in this directory. Resource keys are scoped by the source component name, for example `TaskSettingsWindow_Save`.

## Localized areas

| Area | Resource prefixes | Source files | Keys | Status |
| --- | --- | ---: | ---: | --- |
| Main window | `MainWindow_`, `MainMenuBuilder_`, `ThumbnailItemViewModel_` | 4 | 142 | Complete |
| Application settings | `ApplicationSettingsWindow_` | 3 | 174 | Complete |
| Hotkey settings | `HotkeySettingsWindow_` | 3 | 23 | Complete |
| Task settings | `TaskSettingsWindow_` | 4 | 255 | Complete |
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
| Actions toolbar editor | `ActionsToolbarEditorWindow_` | 2 | 7 | Complete |
| Actions toolbar | `ActionsToolbarWindow_` | 2 | 8 | Complete |
| After capture | `AfterCaptureWindow_` | 2 | 12 | Complete |
| After upload | `AfterUploadWindow_` | 2 | 15 | Complete |
| Auto capture | `AutoCaptureWindow_` | 2 | 14 | Complete |
| Custom uploader key/value editor | `CustomUploaderKeyValueEditor_` | 2 | 4 | Complete |
| Custom uploader settings | `CustomUploaderSettingsWindow_` | 4 | 112 | Complete |
| Quick task menu editor | `QuickTaskMenuEditorWindow_` | 2 | 23 | Complete |
| About | `AboutWindow_` | 2 | 17 | Complete |
| Core runtime messages | `TaskHelpers_`, `SettingManager_`, `WorkerTask_`, `TaskManager_` | 4 | 12 | Complete |

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
