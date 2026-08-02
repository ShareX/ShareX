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
