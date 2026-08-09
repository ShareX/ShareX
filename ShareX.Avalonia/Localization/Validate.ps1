$ErrorActionPreference = 'Stop'

$localizationDirectory = $PSScriptRoot
$projectDirectory = Split-Path $localizationDirectory -Parent
$defaultPath = Join-Path $localizationDirectory 'Strings.resx'
$cultures = @(
    'ar-YE', 'de', 'es', 'es-MX', 'fa-IR', 'fr', 'he-IL', 'hu', 'id-ID', 'it-IT', 'ja-JP', 'ko-KR',
    'nl-NL', 'pl', 'pt-BR', 'pt-PT', 'ro', 'ru', 'tr', 'uk', 'vi-VN', 'zh-CN', 'zh-TW'
)
$trackedSources = @(
    'Controls\SettingsNavigation.axaml'
    'Input\CursorAssetLoader.cs'
)
$strictUtf8 = [Text.UTF8Encoding]::new($false, $true)
$legacyEncodings = @(
    [Text.Encoding]::GetEncoding(28591, [Text.EncoderExceptionFallback]::new(), [Text.DecoderExceptionFallback]::new())
    [Text.Encoding]::GetEncoding(1252, [Text.EncoderExceptionFallback]::new(), [Text.DecoderExceptionFallback]::new())
)
$errors = [Collections.Generic.List[string]]::new()

function Read-Resources([string]$path)
{
    $bytes = [IO.File]::ReadAllBytes($path)
    if ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF)
    {
        throw "Resource file has a UTF-8 BOM: $path"
    }
    $text = $strictUtf8.GetString($bytes)
    if (-not $text.EndsWith("`r`n", [StringComparison]::Ordinal))
    {
        throw "Resource file must use CRLF and end with a newline: $path"
    }
    if ($text.Replace("`r`n", '').Contains("`n"))
    {
        throw "Resource file contains bare LF newlines: $path"
    }
    if ($text -match '[\u0080-\u009F\u200E\u200F\u202A-\u202E\u2066-\u2069\uFFFD]')
    {
        throw "Resource file contains invalid control, direction-control, or replacement characters: $path"
    }

    [xml]$document = $text
    $result = [ordered]@{}
    foreach ($item in $document.root.data)
    {
        $key = [string]$item.name
        $value = [string]$item.value
        if ($result.Contains($key))
        {
            throw "Duplicate resource key '$key' in $path"
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
            catch [Text.EncoderFallbackException] { }
            catch [Text.DecoderFallbackException] { }
        }
        $result[$key] = $value
    }
    return $result
}

function Get-Placeholders([string]$value)
{
    return @([regex]::Matches($value, '(?<!\{)\{\d+(?:[^}]*)\}(?!\})') | ForEach-Object Value | Sort-Object -Unique)
}

$default = Read-Resources $defaultPath
if ($default.Count -ne 9)
{
    $errors.Add("Expected 9 default keys, found $($default.Count).")
}

$expectedPrefixCounts = [ordered]@{
    'SettingsNavigation_' = 1
    'CursorAssetLoader_' = 8
}
foreach ($entry in $expectedPrefixCounts.GetEnumerator())
{
    $count = @($default.Keys | Where-Object { $_.StartsWith($entry.Key, [StringComparison]::Ordinal) }).Count
    if ($count -ne $entry.Value)
    {
        $errors.Add("Prefix '$($entry.Key)' expected $($entry.Value) keys, found $count.")
    }
}

$cultureFiles = @(Get-ChildItem $localizationDirectory -Filter 'Strings.*.resx' | Sort-Object Name)
$expectedCultureFileNames = @($cultures | ForEach-Object { "Strings.$_.resx" } | Sort-Object)
$actualCultureFileNames = @($cultureFiles.Name | Sort-Object)
if (($actualCultureFileNames -join "`n") -ne ($expectedCultureFileNames -join "`n"))
{
    $errors.Add('Localized resource-file inventory differs from the supported-culture list.')
}

foreach ($culture in $cultures)
{
    $path = Join-Path $localizationDirectory "Strings.$culture.resx"
    if (-not (Test-Path -LiteralPath $path))
    {
        $errors.Add("Missing localized resource file: Strings.$culture.resx")
        continue
    }

    $localized = Read-Resources $path
    if (($localized.Keys -join "`n") -ne ($default.Keys -join "`n"))
    {
        $errors.Add("Strings.$culture.resx keys or ordering differ from the default catalog.")
    }

    foreach ($key in $default.Keys)
    {
        if (-not $localized.Contains($key) -or [string]::IsNullOrWhiteSpace($localized[$key]))
        {
            $errors.Add("Strings.$culture.resx: '$key' is missing or empty.")
            continue
        }
        if (((Get-Placeholders $default[$key]) -join '|') -ne ((Get-Placeholders $localized[$key]) -join '|'))
        {
            $errors.Add("Strings.$culture.resx: '$key' has different format placeholders.")
        }
        if ($localized[$key] -ceq $default[$key])
        {
            $errors.Add("Strings.$culture.resx: '$key' unexpectedly retains the default English value.")
        }
    }
}

$sourceText = ''
foreach ($relativePath in $trackedSources)
{
    $path = Join-Path $projectDirectory $relativePath
    if (-not (Test-Path -LiteralPath $path))
    {
        $errors.Add("Missing tracked source file: $relativePath")
        continue
    }
    $sourceText += [IO.File]::ReadAllText($path) + "`n"
}

$referencedKeys = @([regex]::Matches($sourceText, '(?<![A-Za-z0-9_])Strings\.([A-Za-z0-9_]+)') |
    ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique)
foreach ($key in $referencedKeys)
{
    if (-not $default.Contains($key))
    {
        $errors.Add("Source references missing resource key '$key'.")
    }
}
foreach ($key in $default.Keys)
{
    if ($key -notin $referencedKeys)
    {
        $errors.Add("Resource key '$key' is not referenced by a tracked source file.")
    }
}

$allAxaml = Get-ChildItem $projectDirectory -Recurse -File -Filter '*.axaml' |
    Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' }
foreach ($file in $allAxaml)
{
    $text = [IO.File]::ReadAllText($file.FullName)
    foreach ($match in [regex]::Matches($text, '(?i)(?:Text|Content|Header|Title|Watermark|PlaceholderText|ToolTip\.Tip)="([^"]+)"'))
    {
        $value = $match.Groups[1].Value
        if (-not $value.StartsWith('{', [StringComparison]::Ordinal) -and $value -match '[A-Za-z]')
        {
            $errors.Add("Remaining literal Avalonia UI text in $($file.FullName): '$value'")
        }
    }
}

$cursorSource = [IO.File]::ReadAllText((Join-Path $projectDirectory 'Input\CursorAssetLoader.cs'))
if ($cursorSource -match 'throw\s+new\s+[A-Za-z]+Exception\s*\(\s*\$?"')
{
    $errors.Add('CursorAssetLoader.cs contains an unlocalized exception message.')
}
if ($sourceText -match 'using\s+R\s*=' -or $sourceText -match '(?<![A-Za-z0-9_])L\s*\(')
{
    $errors.Add('Found a forbidden resource alias or generic L() localization helper.')
}

$designerText = [IO.File]::ReadAllText((Join-Path $localizationDirectory 'Strings.Designer.cs'))
if ($designerText -notmatch 'ShareX\.AvaloniaUI\.Localization\.Strings')
{
    $errors.Add('Strings.Designer.cs has an incorrect ResourceManager base name.')
}
foreach ($key in $default.Keys)
{
    if ($designerText -notmatch "public static string $([regex]::Escape($key))\b")
    {
        $errors.Add("Strings.Designer.cs is missing property '$key'.")
    }
}

if ($errors.Count -gt 0)
{
    $errors | ForEach-Object { Write-Error $_ }
    exit 1
}

Write-Host "ShareX.Avalonia localization validation passed: $($default.Count) keys across $($cultures.Count) cultures."
