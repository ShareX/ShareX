$ErrorActionPreference = 'Stop'

$localizationDirectory = $PSScriptRoot
$projectDirectory = Split-Path $localizationDirectory -Parent
$defaultPath = Join-Path $localizationDirectory 'Strings.resx'
$cultures = @(
    'ar-YE', 'de', 'es', 'es-MX', 'fa-IR', 'fr', 'he-IL', 'hu', 'id-ID', 'it-IT', 'ja-JP', 'ko-KR',
    'nl-NL', 'pl', 'pt-BR', 'pt-PT', 'ro', 'ru', 'tr', 'uk', 'vi-VN', 'zh-CN', 'zh-TW'
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
    if ($text -match '[\u0080-\u009F\uFFFD]')
    {
        throw "Resource file contains invalid control or replacement characters: $path"
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
if ($default.Count -ne 347)
{
    $errors.Add("Expected 347 default keys, found $($default.Count).")
}

$expectedPrefixCounts = [ordered]@{
    'GradientOptionsPanel_' = 4
    'ImageEffectOptionsPanel_' = 15
    'ImageEffectPackagerWindow_' = 16
    'ImageEffectsConfirmationWindow_' = 4
    'ImageEffectsWindow_' = 29
    'ImageEffectCategory_' = 4
    'ImageEffectDefault_' = 2
    'ImageEffectPropertyDescription_' = 28
    'ImageEffectProperty_' = 136
    'ImageEffectEnum_' = 58
}
foreach ($entry in $expectedPrefixCounts.GetEnumerator())
{
    $count = @($default.Keys | Where-Object { $_.StartsWith($entry.Key, [StringComparison]::Ordinal) }).Count
    if ($count -ne $entry.Value)
    {
        $errors.Add("Prefix '$($entry.Key)' expected $($entry.Value) keys, found $count.")
    }
}
$effectCount = @($default.Keys | Where-Object {
    $_.StartsWith('ImageEffect_', [StringComparison]::Ordinal) -and
    -not $_.StartsWith('ImageEffectProperty', [StringComparison]::Ordinal) -and
    -not $_.StartsWith('ImageEffectEnum_', [StringComparison]::Ordinal)
}).Count
if ($effectCount -ne 51)
{
    $errors.Add("Expected 51 effect-name keys, found $effectCount.")
}

$cultureFiles = @(Get-ChildItem $localizationDirectory -Filter 'Strings.*.resx' | Sort-Object Name)
if ($cultureFiles.Count -ne $cultures.Count)
{
    $errors.Add("Expected $($cultures.Count) localized resource files, found $($cultureFiles.Count).")
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
    }
}

$sourceText = Get-ChildItem $projectDirectory -Recurse -File -Include '*.cs', '*.axaml' |
    Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' -and $_.Name -ne 'Strings.Designer.cs' } |
    ForEach-Object { [IO.File]::ReadAllText($_.FullName) }
$sourceText = $sourceText -join "`n"
foreach ($prefix in @('ImageEffect_', 'ImageEffectProperty_', 'ImageEffectPropertyDescription_', 'ImageEffectEnum_'))
{
    if ($sourceText -notmatch [regex]::Escape($prefix))
    {
        $errors.Add("Runtime localization does not reference dynamic prefix '$prefix'.")
    }
}
if ($sourceText -match 'using\s+R\s*=' -or $sourceText -match '(?<![A-Za-z0-9_])L\s*\(')
{
    $errors.Add('Found a forbidden resource alias or generic L() localization helper.')
}

$designerText = [IO.File]::ReadAllText((Join-Path $localizationDirectory 'Strings.Designer.cs'))
if ($designerText -notmatch 'ShareX\.ImageEffectsLib\.Localization\.Strings')
{
    $errors.Add('Strings.Designer.cs has an incorrect ResourceManager base name.')
}

if ($errors.Count -gt 0)
{
    $errors | ForEach-Object { Write-Error $_ }
    exit 1
}

Write-Host "Image Effects localization validation passed: $($default.Count) keys across $($cultures.Count) cultures."
