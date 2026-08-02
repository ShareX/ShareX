$ErrorActionPreference = 'Stop'

$localizationDirectory = $PSScriptRoot
$projectDirectory = Split-Path $localizationDirectory -Parent
$defaultPath = Join-Path $localizationDirectory 'Strings.resx'
$emojiCatalogPath = Join-Path $projectDirectory 'Assets\emoji-catalog.json'
$cultureFiles = @(Get-ChildItem $localizationDirectory -Filter 'Strings.*.resx' | Sort-Object Name)
$strictUtf8 = [Text.UTF8Encoding]::new($false, $true)
$legacyEncodings = @(
    [Text.Encoding]::GetEncoding(28591, [Text.EncoderExceptionFallback]::new(), [Text.DecoderExceptionFallback]::new())
    [Text.Encoding]::GetEncoding(1252, [Text.EncoderExceptionFallback]::new(), [Text.DecoderExceptionFallback]::new())
)

$areas = @(
    [pscustomobject]@{
        Name = 'Confirmation dialog'
        Prefix = 'ConfirmationDialogView_'
        Sources = @(
            'Presentation/Views/ConfirmationDialogView.axaml'
            'Presentation/ViewModels/ConfirmationDialogViewModel.cs'
        )
    }
    [pscustomobject]@{
        Name = 'New image dialog'
        Prefix = 'NewImageDialogView_'
        Sources = @('Presentation/Views/NewImageDialogView.axaml')
    }
    [pscustomobject]@{
        Name = 'Start screen'
        Prefix = 'StartScreenDialogView_'
        Sources = @(
            'Presentation/Views/StartScreenDialogView.axaml'
            'Presentation/ViewModels/StartScreenDialogViewModel.cs'
        )
    }
    [pscustomobject]@{
        Name = 'Emoji picker'
        Prefix = 'EmojiPickerDialogView_'
        Sources = @(
            'Presentation/Views/EmojiPickerDialogView.axaml'
            'Presentation/ViewModels/EmojiPickerDialogViewModel.cs'
        )
    }
    [pscustomobject]@{
        Name = 'Insert image dialog'
        Prefix = 'InsertImageDialogView_'
        Sources = @(
            'Presentation/Views/InsertImageDialogView.axaml'
            'Presentation/ViewModels/InsertImageDialogViewModel.cs'
        )
    }
    [pscustomobject]@{
        Name = 'Annotation toolbar'
        Prefix = 'AnnotationToolbar_'
        Sources = @('Presentation/Controls/AnnotationToolbar.axaml')
    }
    [pscustomobject]@{
        Name = 'Color picker panel'
        Prefix = 'ColorPickerPanel_'
        Sources = @('Presentation/Controls/ColorPickerPanel.axaml')
    }
    [pscustomobject]@{
        Name = 'Editor options panel'
        Prefix = 'EditorOptionsPanel_'
        Sources = @('Presentation/Controls/EditorOptionsPanel.axaml')
    }
    [pscustomobject]@{
        Name = 'Editor view'
        Prefix = 'EditorView_'
        Sources = @(
            'Presentation/Views/EditorView.axaml'
            'Presentation/Views/EditorView.axaml.cs'
            'Presentation/Views/EditorView.EasterEggs.cs'
            'Presentation/Views/EditorView.ImageInsert.cs'
            'Presentation/Controllers/EditorInputController.cs'
        )
    }
    [pscustomobject]@{
        Name = 'Effect browser shell'
        Prefix = 'EffectBrowserPanel_'
        Sources = @(
            'Presentation/Controls/EffectBrowserPanel.axaml'
            'Presentation/Controls/EffectBrowserPanel.axaml.cs'
        )
    }
    [pscustomobject]@{
        Name = 'Effect dialog shell'
        Prefix = 'SchemaDrivenEffectDialog_'
        Sources = @('Presentation/Views/Dialogs/SchemaDrivenEffectDialog.axaml')
    }
    [pscustomobject]@{
        Name = 'Toolbar customization dialog'
        Prefix = 'ToolbarCustomizationDialogView_'
        Sources = @(
            'Presentation/Views/ToolbarCustomizationDialogView.axaml'
            'Presentation/ViewModels/ToolbarCustomizationDialogViewModel.cs'
        )
    }
    [pscustomobject]@{
        Name = 'Zoom picker'
        Prefix = 'ZoomPickerDropdown_'
        Sources = @('Presentation/Controls/ZoomPickerDropdown.axaml')
    }
)

function Read-Resources([string]$path)
{
    $text = [IO.File]::ReadAllText($path, $strictUtf8)
    if ($text -match '[\u0080-\u009F\uFFFD]')
    {
        throw "Resource file contains invalid control or replacement characters: $path"
    }

    [xml]$document = $text
    $result = @{}
    foreach ($item in $document.root.data)
    {
        $value = [string]$item.value
        foreach ($encoding in $legacyEncodings)
        {
            try
            {
                $decoded = $strictUtf8.GetString($encoding.GetBytes($value))
                if ($decoded -ne $value)
                {
                    throw "Resource value appears to contain reversible mojibake: $path ($($item.name))"
                }
            }
            catch [Text.EncoderFallbackException]
            {
            }
            catch [Text.DecoderFallbackException]
            {
            }
        }
        $result[[string]$item.name] = $value
    }
    return $result
}

function Get-Placeholders([string]$value)
{
    return @([regex]::Matches($value, '(?<!\{)\{\d+(?:[^}]*)\}(?!\})') | ForEach-Object Value | Sort-Object -Unique)
}

$errors = [Collections.Generic.List[string]]::new()
$default = Read-Resources $defaultPath

$emojiCatalogText = [IO.File]::ReadAllText($emojiCatalogPath, $strictUtf8)
if ($emojiCatalogText -match '[\u0080-\u009F\u00C2\u00C3\u00E2\uFFFD]')
{
    $errors.Add('Emoji catalog contains mojibake or invalid control characters.')
}
try
{
    $null = $emojiCatalogText | ConvertFrom-Json
}
catch
{
    $errors.Add("Emoji catalog is not valid JSON: $($_.Exception.Message)")
}

if ($cultureFiles.Count -ne 23)
{
    $errors.Add("Expected 23 localized resource files, found $($cultureFiles.Count).")
}

foreach ($file in $cultureFiles)
{
    $localized = Read-Resources $file.FullName
    foreach ($key in $default.Keys)
    {
        if (-not $localized.ContainsKey($key))
        {
            $errors.Add("$($file.Name): missing '$key'.")
            continue
        }
        if ([string]::IsNullOrWhiteSpace($localized[$key]))
        {
            $errors.Add("$($file.Name): '$key' is empty.")
        }
        $defaultPlaceholders = (Get-Placeholders $default[$key]) -join '|'
        $localizedPlaceholders = (Get-Placeholders $localized[$key]) -join '|'
        if ($defaultPlaceholders -ne $localizedPlaceholders)
        {
            $errors.Add("$($file.Name): '$key' has different format placeholders.")
        }
    }
    foreach ($key in $localized.Keys)
    {
        if (-not $default.ContainsKey($key))
        {
            $errors.Add("$($file.Name): unexpected '$key'.")
        }
    }
}

$rows = foreach ($area in $areas)
{
    $text = ''
    foreach ($source in $area.Sources)
    {
        $path = Join-Path $projectDirectory $source.Replace('/', [IO.Path]::DirectorySeparatorChar)
        if (-not (Test-Path -LiteralPath $path))
        {
            $errors.Add("Missing source file: $source")
            continue
        }
        $text += [IO.File]::ReadAllText($path)
    }
    $keys = @($default.Keys | Where-Object { $_.StartsWith($area.Prefix, [StringComparison]::Ordinal) })
    if (-not $area.DataDriven)
    {
        foreach ($key in $keys)
        {
            if ($text -notmatch [regex]::Escape("Strings.$key"))
            {
                $errors.Add("$($area.Name): '$key' is not referenced.")
            }
        }
    }
    [pscustomobject]@{ Area = $area.Name; Sources = $area.Sources.Count; Keys = $keys.Count; Cultures = $cultureFiles.Count }
}

$rows | Format-Table -AutoSize

$trackedViews = @($areas.Sources | Where-Object { $_.EndsWith('.axaml') } | ForEach-Object { $_.Replace('\', '/') })
$allViews = @(Get-ChildItem $projectDirectory -Recurse -Filter '*.axaml' | ForEach-Object { $_.FullName.Substring($projectDirectory.Length + 1).Replace('\', '/') })
$untrackedViews = @($allViews | Where-Object { $_ -notin $trackedViews } | Sort-Object)

Write-Host "`nLocalized cultures: $($cultureFiles.Count)"
Write-Host "Untracked AXAML views: $($untrackedViews.Count)"
$untrackedViews | ForEach-Object { Write-Host "  $_" }

if ($errors.Count -gt 0)
{
    $errors | ForEach-Object { Write-Error $_ }
    exit 1
}

Write-Host "`nImage Editor localization validation succeeded."
