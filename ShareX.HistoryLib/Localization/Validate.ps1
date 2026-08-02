$ErrorActionPreference = 'Stop'

$localizationDirectory = $PSScriptRoot
$projectDirectory = Split-Path $localizationDirectory -Parent
$defaultPath = Join-Path $localizationDirectory 'Strings.resx'
$cultureFiles = @(Get-ChildItem $localizationDirectory -Filter 'Strings.*.resx' | Sort-Object Name)
$strictUtf8 = [Text.UTF8Encoding]::new($false, $true)
$legacyEncodings = @(
    [Text.Encoding]::GetEncoding(28591, [Text.EncoderExceptionFallback]::new(), [Text.DecoderExceptionFallback]::new())
    [Text.Encoding]::GetEncoding(1252, [Text.EncoderExceptionFallback]::new(), [Text.DecoderExceptionFallback]::new())
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
        $key = [string]$item.name
        $value = [string]$item.value
        if ($result.ContainsKey($key))
        {
            throw "Duplicate resource key: $path ($key)"
        }

        foreach ($encoding in $legacyEncodings)
        {
            try
            {
                $decoded = $strictUtf8.GetString($encoding.GetBytes($value))
                if ($decoded -ne $value)
                {
                    throw "Resource value appears to contain reversible mojibake: $path ($key)"
                }
            }
            catch [Text.EncoderFallbackException]
            {
            }
            catch [Text.DecoderFallbackException]
            {
            }
        }

        $result[$key] = $value
    }
    return $result
}

function Get-Placeholders([string]$value)
{
    return @([regex]::Matches($value, '(?<!\{)\{\d+(?:[^}]*)\}(?!\})') |
        ForEach-Object Value |
        Sort-Object -Unique)
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

$sourceFiles = @(
    'Views/HistoryWindow.axaml'
    'Views/HistoryWindow.axaml.cs'
    'Views/ImageHistoryWindow.axaml'
    'Views/ImageHistoryWindow.axaml.cs'
    'Helpers/HistoryHelpers.cs'
    'Managers/HistoryManager.cs'
)
$sourceText = ''
foreach ($source in $sourceFiles)
{
    $sourceText += [IO.File]::ReadAllText((Join-Path $projectDirectory $source))
}
foreach ($key in $default.Keys)
{
    if ($sourceText -notmatch [regex]::Escape("Strings.$key"))
    {
        $errors.Add("Resource key is not referenced: '$key'.")
    }
}

$literalPattern = '\b(?:Label|PlaceholderText|Description|ToolTip\.Tip|Text|Content|Header|Title|Watermark)="(?!\{|")'
foreach ($view in Get-ChildItem (Join-Path $projectDirectory 'Views') -Filter '*.axaml')
{
    $matches = [regex]::Matches([IO.File]::ReadAllText($view.FullName), $literalPattern)
    if ($matches.Count -gt 0)
    {
        $errors.Add("$($view.Name): contains $($matches.Count) user-visible literal attribute(s).")
    }
}

Write-Host "Default keys: $($default.Count)"
Write-Host "Localized cultures: $($cultureFiles.Count)"

if ($errors.Count -gt 0)
{
    $errors | ForEach-Object { Write-Error $_ }
    exit 1
}

Write-Host 'HistoryLib localization validation succeeded.'
