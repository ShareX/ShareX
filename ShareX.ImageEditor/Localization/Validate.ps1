$ErrorActionPreference = 'Stop'

$localizationDirectory = $PSScriptRoot
$projectDirectory = Split-Path $localizationDirectory -Parent
$defaultPath = Join-Path $localizationDirectory 'Strings.resx'
$cultureFiles = @(Get-ChildItem $localizationDirectory -Filter 'Strings.*.resx' | Sort-Object Name)

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
)

function Read-Resources([string]$path)
{
    [xml]$document = Get-Content -LiteralPath $path -Raw
    $result = @{}
    foreach ($item in $document.root.data)
    {
        $result[[string]$item.name] = [string]$item.value
    }
    return $result
}

function Get-Placeholders([string]$value)
{
    return @([regex]::Matches($value, '(?<!\{)\{\d+(?:[^}]*)\}(?!\})') | ForEach-Object Value | Sort-Object -Unique)
}

$errors = [Collections.Generic.List[string]]::new()
$default = Read-Resources $defaultPath

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
    foreach ($key in $keys)
    {
        if ($text -notmatch [regex]::Escape("Strings.$key"))
        {
            $errors.Add("$($area.Name): '$key' is not referenced.")
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
