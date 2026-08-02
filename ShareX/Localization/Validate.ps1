[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'

$localizationDirectory = $PSScriptRoot
$projectDirectory = Split-Path -Parent $localizationDirectory

# This is the source-of-truth manifest for completed Avalonia localization work.
$areas = @(
    [pscustomobject]@{
        Name = 'Main window'
        ResourcePrefixes = @('MainWindow_', 'MainMenuBuilder_', 'ThumbnailItemViewModel_')
        SourceFiles = @(
            'Presentation/MainWindow/MainWindow.axaml'
            'Presentation/MainWindow/MainWindow.axaml.cs'
            'Presentation/MainWindow/MainMenuBuilder.cs'
            'Presentation/MainWindow/ThumbnailItemViewModel.cs'
        )
    }
    [pscustomobject]@{
        Name = 'Application settings'
        ResourcePrefixes = @('ApplicationSettingsWindow_')
        SourceFiles = @(
            'Presentation/ApplicationSettings/ApplicationSettingsWindow.axaml'
            'Presentation/ApplicationSettings/ApplicationSettingsWindow.axaml.cs'
            'Presentation/ApplicationSettings/ApplicationSettingsViewModel.cs'
        )
    }
    [pscustomobject]@{
        Name = 'Hotkey settings'
        ResourcePrefixes = @('HotkeySettingsWindow_')
        SourceFiles = @(
            'Presentation/HotkeySettings/HotkeySettingsWindow.axaml'
            'Presentation/HotkeySettings/HotkeySettingsWindow.axaml.cs'
            'Presentation/HotkeySettings/HotkeySettingsModels.cs'
        )
    }
    [pscustomobject]@{
        Name = 'Task settings'
        ResourcePrefixes = @('TaskSettingsWindow_')
        SourceFiles = @(
            'Presentation/TaskSettings/TaskSettingsWindow.axaml'
            'Presentation/TaskSettings/TaskSettingsWindow.axaml.cs'
            'Presentation/TaskSettings/TaskSettingsPageBuilder.cs'
            'Presentation/TaskSettings/TaskSettingsViewModel.cs'
        )
    }
    [pscustomobject]@{
        Name = 'Drag and drop upload'
        ResourcePrefixes = @('DragDropUploadWindow_')
        SourceFiles = @(
            'Presentation/DragDropUpload/DragDropUploadWindow.axaml'
            'Presentation/DragDropUpload/DragDropUploadWindow.axaml.cs'
        )
    }
    [pscustomobject]@{
        Name = 'Clipboard upload'
        ResourcePrefixes = @('ClipboardUploadWindow_')
        SourceFiles = @(
            'Presentation/ClipboardUpload/ClipboardUploadWindow.axaml'
            'Presentation/ClipboardUpload/ClipboardUploadWindow.axaml.cs'
        )
    }
    [pscustomobject]@{
        Name = 'Notification'
        ResourcePrefixes = @()
        SourceFiles = @(
            'Presentation/Notification/NotificationWindow.axaml'
            'Presentation/Notification/NotificationWindow.cs'
            'Presentation/Notification/NotificationActionButton.cs'
            'Presentation/Notification/NotificationWindowConfig.cs'
        )
    }
    [pscustomobject]@{
        Name = 'File exists'
        ResourcePrefixes = @('FileExistWindow_')
        SourceFiles = @(
            'Presentation/FileExist/FileExistWindow.axaml'
            'Presentation/FileExist/FileExistWindow.axaml.cs'
        )
    }
    [pscustomobject]@{
        Name = 'Large file upload warning'
        ResourcePrefixes = @('LargeFileUploadWarningWindow_')
        SourceFiles = @(
            'Presentation/UploadConfirmation/LargeFileUploadWarningWindow.axaml'
            'Presentation/UploadConfirmation/LargeFileUploadWarningWindow.axaml.cs'
        )
    }
    [pscustomobject]@{
        Name = 'Multi-upload confirmation'
        ResourcePrefixes = @('MultiUploadConfirmationWindow_')
        SourceFiles = @(
            'Presentation/UploadConfirmation/MultiUploadConfirmationWindow.axaml'
            'Presentation/UploadConfirmation/MultiUploadConfirmationWindow.axaml.cs'
        )
    }
    [pscustomobject]@{
        Name = 'Debug log'
        ResourcePrefixes = @('DebugLogWindow_')
        SourceFiles = @(
            'Presentation/DebugLog/DebugLogWindow.axaml'
            'Presentation/DebugLog/DebugLogWindow.axaml.cs'
        )
    }
    [pscustomobject]@{
        Name = 'First-time configuration'
        ResourcePrefixes = @('FirstTimeConfigWindow_')
        SourceFiles = @(
            'Presentation/FirstTimeConfig/FirstTimeConfigWindow.axaml'
            'Presentation/FirstTimeConfig/FirstTimeConfigWindow.axaml.cs'
        )
    }
    [pscustomobject]@{
        Name = 'Image editor selector'
        ResourcePrefixes = @('ImageEditorSelectorWindow_')
        SourceFiles = @(
            'Presentation/ImageEditorSelector/ImageEditorSelectorWindow.axaml'
            'Presentation/ImageEditorSelector/ImageEditorSelectorWindow.axaml.cs'
        )
    }
    [pscustomobject]@{
        Name = 'Shorten URL'
        ResourcePrefixes = @('ShortenURLWindow_')
        SourceFiles = @(
            'Presentation/ShortenURL/ShortenURLWindow.axaml'
            'Presentation/ShortenURL/ShortenURLWindow.axaml.cs'
        )
    }
    [pscustomobject]@{
        Name = 'URL upload'
        ResourcePrefixes = @('URLUploadWindow_')
        SourceFiles = @(
            'Presentation/URLUpload/URLUploadWindow.axaml'
            'Presentation/URLUpload/URLUploadWindow.axaml.cs'
        )
    }
    [pscustomobject]@{
        Name = 'Before upload'
        ResourcePrefixes = @('BeforeUploadWindow_')
        SourceFiles = @(
            'Presentation/BeforeUpload/BeforeUploadWindow.axaml'
            'Presentation/BeforeUpload/BeforeUploadWindow.axaml.cs'
        )
    }
    [pscustomobject]@{
        Name = 'Actions toolbar editor'
        ResourcePrefixes = @('ActionsToolbarEditorWindow_')
        SourceFiles = @(
            'Presentation/ActionsToolbar/ActionsToolbarEditorWindow.axaml'
            'Presentation/ActionsToolbar/ActionsToolbarEditorWindow.axaml.cs'
        )
    }
    [pscustomobject]@{
        Name = 'Actions toolbar'
        ResourcePrefixes = @('ActionsToolbarWindow_')
        SourceFiles = @(
            'Presentation/ActionsToolbar/ActionsToolbarWindow.axaml'
            'Presentation/ActionsToolbar/ActionsToolbarWindow.axaml.cs'
        )
    }
    [pscustomobject]@{
        Name = 'After capture'
        ResourcePrefixes = @('AfterCaptureWindow_')
        SourceFiles = @(
            'Presentation/AfterCapture/AfterCaptureWindow.axaml'
            'Presentation/AfterCapture/AfterCaptureWindow.axaml.cs'
        )
    }
    [pscustomobject]@{
        Name = 'After upload'
        ResourcePrefixes = @('AfterUploadWindow_')
        SourceFiles = @(
            'Presentation/AfterUpload/AfterUploadWindow.axaml'
            'Presentation/AfterUpload/AfterUploadWindow.axaml.cs'
        )
    }
    [pscustomobject]@{
        Name = 'Auto capture'
        ResourcePrefixes = @('AutoCaptureWindow_')
        SourceFiles = @(
            'Presentation/AutoCapture/AutoCaptureWindow.axaml'
            'Presentation/AutoCapture/AutoCaptureWindow.axaml.cs'
        )
    }
    [pscustomobject]@{
        Name = 'Custom uploader key/value editor'
        ResourcePrefixes = @('CustomUploaderKeyValueEditor_')
        SourceFiles = @(
            'Presentation/CustomUploaderSettings/CustomUploaderKeyValueEditor.axaml'
            'Presentation/CustomUploaderSettings/CustomUploaderKeyValueEditor.axaml.cs'
        )
    }
    [pscustomobject]@{
        Name = 'Custom uploader settings'
        ResourcePrefixes = @('CustomUploaderSettingsWindow_')
        SourceFiles = @(
            'Presentation/CustomUploaderSettings/CustomUploaderSettingsWindow.axaml'
            'Presentation/CustomUploaderSettings/CustomUploaderSettingsWindow.axaml.cs'
            'Presentation/CustomUploaderSettings/CustomUploaderSettingsViewModel.cs'
            'Presentation/CustomUploaderSettings/CustomUploaderSettingsModels.cs'
        )
    }
    [pscustomobject]@{
        Name = 'Quick task menu editor'
        ResourcePrefixes = @('QuickTaskMenuEditorWindow_')
        SourceFiles = @(
            'Presentation/QuickTaskMenuEditor/QuickTaskMenuEditorWindow.axaml'
            'Presentation/QuickTaskMenuEditor/QuickTaskMenuEditorWindow.axaml.cs'
        )
    }
    [pscustomobject]@{
        Name = 'About'
        ResourcePrefixes = @('AboutWindow_')
        SourceFiles = @(
            'Presentation/About/AboutWindow.axaml'
            'Presentation/About/AboutWindow.axaml.cs'
        )
    }
    [pscustomobject]@{
        Name = 'Core runtime messages'
        ResourcePrefixes = @('TaskHelpers_', 'SettingManager_', 'WorkerTask_', 'TaskManager_')
        SourceFiles = @(
            'TaskHelpers.cs'
            'SettingManager.cs'
            'WorkerTask.cs'
            'TaskManager.cs'
        )
    }
)

function Resolve-SourcePath([string]$relativePath)
{
    $platformPath = $relativePath.Replace('/', [IO.Path]::DirectorySeparatorChar)
    return Join-Path $projectDirectory $platformPath
}

function Read-ResourceFile([string]$path)
{
    [xml]$document = Get-Content -LiteralPath $path -Raw -Encoding UTF8
    $entries = @($document.root.data)
    $duplicates = @($entries | Group-Object name | Where-Object Count -gt 1)

    if ($duplicates.Count -gt 0)
    {
        $names = $duplicates.Name -join ', '
        throw "Duplicate resource keys in '$path': $names"
    }

    $values = [Collections.Generic.Dictionary[string, string]]::new([StringComparer]::Ordinal)
    foreach ($entry in $entries)
    {
        $values.Add([string]$entry.name, [string]$entry.value)
    }

    return $values
}

function Get-Placeholders([string]$value)
{
    return @([regex]::Matches($value, '\{\d+\}') | ForEach-Object Value | Sort-Object -Unique)
}

function Uses-Prefix([string]$key, [string[]]$prefixes)
{
    foreach ($prefix in $prefixes)
    {
        if ($key.StartsWith($prefix, [StringComparison]::Ordinal))
        {
            return $true
        }
    }

    return $false
}

$errors = [Collections.Generic.List[string]]::new()
$defaultPath = Join-Path $localizationDirectory 'Strings.resx'
$defaultValues = Read-ResourceFile $defaultPath
$cultureFiles = @(Get-ChildItem -LiteralPath $localizationDirectory -Filter 'Strings.*.resx' | Sort-Object Name)

foreach ($cultureFile in $cultureFiles)
{
    $cultureValues = Read-ResourceFile $cultureFile.FullName

    foreach ($key in $defaultValues.Keys)
    {
        if (-not $cultureValues.ContainsKey($key))
        {
            $errors.Add("$($cultureFile.Name): missing '$key'")
            continue
        }

        if ([string]::IsNullOrWhiteSpace($cultureValues[$key]))
        {
            $errors.Add("$($cultureFile.Name): '$key' has an empty value")
        }

        $expectedPlaceholders = Get-Placeholders $defaultValues[$key]
        $actualPlaceholders = Get-Placeholders $cultureValues[$key]
        if (($expectedPlaceholders -join '|') -ne ($actualPlaceholders -join '|'))
        {
            $errors.Add("$($cultureFile.Name): '$key' has different format placeholders")
        }
    }

    foreach ($key in $cultureValues.Keys)
    {
        if (-not $defaultValues.ContainsKey($key))
        {
            $errors.Add("$($cultureFile.Name): unexpected key '$key'")
        }
    }
}

$status = foreach ($area in $areas)
{
    $areaKeys = @($defaultValues.Keys | Where-Object { Uses-Prefix $_ $area.ResourcePrefixes } | Sort-Object)
    $sourceText = [Text.StringBuilder]::new()

    foreach ($relativePath in $area.SourceFiles)
    {
        $sourcePath = Resolve-SourcePath $relativePath
        if (-not (Test-Path -LiteralPath $sourcePath -PathType Leaf))
        {
            $errors.Add("$($area.Name): source file does not exist: $relativePath")
            continue
        }

        [void]$sourceText.AppendLine((Get-Content -LiteralPath $sourcePath -Raw -Encoding UTF8))
    }

    $references = @(
        [regex]::Matches($sourceText.ToString(), '(?:res:Strings\.|Strings\.)([A-Za-z0-9_]+)') |
            ForEach-Object { $_.Groups[1].Value } |
            Where-Object { Uses-Prefix $_ $area.ResourcePrefixes } |
            Sort-Object -Unique
    )

    foreach ($reference in $references)
    {
        if (-not $defaultValues.ContainsKey($reference))
        {
            $errors.Add("$($area.Name): '$reference' is referenced but missing from Strings.resx")
        }
    }

    foreach ($key in $areaKeys)
    {
        if ($key -notin $references)
        {
            $errors.Add("$($area.Name): '$key' is not referenced by a tracked source file")
        }
    }

    [pscustomobject]@{
        Area = $area.Name
        Sources = $area.SourceFiles.Count
        Keys = $areaKeys.Count
        Cultures = $cultureFiles.Count
        Status = if ($areaKeys.Count -eq $references.Count) { 'Complete' } else { 'Check failed' }
    }
}

$trackedViews = @(
    $areas.SourceFiles |
        Where-Object { $_.EndsWith('.axaml', [StringComparison]::OrdinalIgnoreCase) } |
        ForEach-Object { $_.Replace('\', '/') }
)
$presentationDirectory = Join-Path $projectDirectory 'Presentation'
$projectDirectoryPrefix = $projectDirectory.TrimEnd('\', '/') + [IO.Path]::DirectorySeparatorChar
$untrackedViews = @(
    Get-ChildItem -LiteralPath $presentationDirectory -Recurse -Filter '*.axaml' |
        ForEach-Object {
            if (-not $_.FullName.StartsWith($projectDirectoryPrefix, [StringComparison]::OrdinalIgnoreCase))
            {
                throw "View is outside the project directory: $($_.FullName)"
            }

            $_.FullName.Substring($projectDirectoryPrefix.Length).Replace('\', '/')
        } |
        Where-Object { $_ -notin $trackedViews } |
        Sort-Object
)

$status | Format-Table -AutoSize
Write-Host
Write-Host "Localized cultures: $($cultureFiles.Count)"
Write-Host "Untracked Avalonia views: $($untrackedViews.Count)"
foreach ($view in $untrackedViews)
{
    Write-Host "  $view"
}

if ($errors.Count -gt 0)
{
    Write-Host
    foreach ($errorMessage in $errors)
    {
        Write-Error $errorMessage
    }

    throw "Localization validation failed with $($errors.Count) error(s)."
}

Write-Host
Write-Host 'Localization validation succeeded.'
