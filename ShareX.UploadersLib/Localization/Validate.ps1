$ErrorActionPreference = 'Stop'

$localizationDirectory = $PSScriptRoot
$projectDirectory = Split-Path $localizationDirectory -Parent
$defaultPath = Join-Path $localizationDirectory 'Strings.resx'
$cultureFiles = @(Get-ChildItem -LiteralPath $localizationDirectory -Filter 'Strings.*.resx' | Sort-Object Name)
$strictUtf8 = [Text.UTF8Encoding]::new($false, $true)
$legacyEncodings = @(
    [Text.Encoding]::GetEncoding(28591, [Text.EncoderExceptionFallback]::new(), [Text.DecoderExceptionFallback]::new())
    [Text.Encoding]::GetEncoding(1252, [Text.EncoderExceptionFallback]::new(), [Text.DecoderExceptionFallback]::new())
)
$expectedCultures = @(
    'ar-YE', 'de', 'es', 'es-MX', 'fa-IR', 'fr', 'he-IL', 'hu', 'id-ID', 'it-IT',
    'ja-JP', 'ko-KR', 'nl-NL', 'pl', 'pt-BR', 'pt-PT', 'ro', 'ru', 'tr', 'uk',
    'vi-VN', 'zh-CN', 'zh-TW'
)
$dataDrivenPrefixes = @(
    'AccountType_',
    'AmazonS3StorageClass_',
    'BoxShareAccessLevel_',
    'BrowserProtocol_',
    'CustomUploaderBody_',
    'CustomUploaderDestinationType_',
    'DestinationSettings_Field_',
    'FileDestination_',
    'FTPProtocol_',
    'ImageDestination_',
    'ImgurThumbnailType_',
    'LinkFormatEnum_',
    'PastebinExpiration_',
    'PastebinPrivacy_',
    'PrivateBinExpiration_',
    'PrivateBinFormat_',
    'TextDestination_',
    'URLSharingServices_',
    'UrlShortenerType_',
    'YouTubeVideoPrivacy_'
)

function Read-Resources([string]$path)
{
    $bytes = [IO.File]::ReadAllBytes($path)
    if ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF)
    {
        throw "Resource file has a UTF-8 BOM: $path"
    }

    $text = $strictUtf8.GetString($bytes)
    if (-not $text.EndsWith("`r`n"))
    {
        throw "Resource file does not have a final CRLF newline: $path"
    }
    if ([regex]::IsMatch($text, '(?<!\r)\n'))
    {
        throw "Resource file contains a bare LF newline: $path"
    }
    if ($text -match '[\u0080-\u009F\uFFFD]')
    {
        throw "Resource file contains invalid control or replacement characters: $path"
    }
    if ($text -match '&apos\s+;|&\s+apos;')
    {
        throw "Resource file contains a malformed apostrophe entity: $path"
    }

    [xml]$document = $text
    $values = [ordered]@{}
    foreach ($item in $document.root.data)
    {
        $key = [string]$item.name
        $value = [string]$item.value
        if ($values.Contains($key))
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

        $values[$key] = $value
    }

    return [pscustomobject]@{
        Keys = @($values.Keys)
        Values = $values
    }
}

function Get-Placeholders([string]$value)
{
    return @([regex]::Matches($value, '(?<!\{)\{\d+(?:[^}]*)\}(?!\})') |
        ForEach-Object Value |
        Sort-Object)
}

function Test-SequenceEqual([object[]]$left, [object[]]$right)
{
    if ($left.Count -ne $right.Count)
    {
        return $false
    }

    for ($index = 0; $index -lt $left.Count; $index++)
    {
        if ($left[$index] -cne $right[$index])
        {
            return $false
        }
    }

    return $true
}

$errors = [Collections.Generic.List[string]]::new()
$default = Read-Resources $defaultPath

if ($default.Keys.Count -ne 493)
{
    $errors.Add("Expected 493 default keys, found $($default.Keys.Count).")
}

$actualCultures = @($cultureFiles | ForEach-Object { $_.BaseName.Substring('Strings.'.Length) })
foreach ($culture in $expectedCultures)
{
    if ($culture -notin $actualCultures)
    {
        $errors.Add("Missing localized resource catalog for '$culture'.")
    }
}
foreach ($culture in $actualCultures)
{
    if ($culture -notin $expectedCultures)
    {
        $errors.Add("Unexpected localized resource catalog for '$culture'.")
    }
}

foreach ($file in $cultureFiles)
{
    $localized = Read-Resources $file.FullName
    if (-not (Test-SequenceEqual $default.Keys $localized.Keys))
    {
        $errors.Add("$($file.Name): resource keys or their order differ from Strings.resx.")
    }

    foreach ($key in $default.Keys)
    {
        if (-not $localized.Values.Contains($key))
        {
            $errors.Add("$($file.Name): missing '$key'.")
            continue
        }
        if ([string]::IsNullOrWhiteSpace($localized.Values[$key]))
        {
            $errors.Add("$($file.Name): '$key' is empty.")
        }
        if (-not (Test-SequenceEqual (Get-Placeholders $default.Values[$key]) (Get-Placeholders $localized.Values[$key])))
        {
            $errors.Add("$($file.Name): '$key' has different format placeholders.")
        }
    }
}

$sourceFiles = @(Get-ChildItem -LiteralPath $projectDirectory -Recurse -File |
    Where-Object {
        $_.Extension -in @('.axaml', '.cs') -and
        $_.FullName -notmatch '\\(bin|obj|Localization|Properties)\\'
    })
$sourceText = ($sourceFiles | ForEach-Object { [IO.File]::ReadAllText($_.FullName) }) -join "`n"

$referencePattern = '(?:(?:Localization\.)?Strings\.|localization:Strings\.)([A-Za-z_][A-Za-z0-9_]*)'
$referencedKeys = @([regex]::Matches($sourceText, $referencePattern) |
    ForEach-Object { $_.Groups[1].Value } |
    Where-Object { $_ -notin @('Culture', 'ResourceManager') } |
    Sort-Object -Unique)
foreach ($key in $referencedKeys)
{
    if (-not $default.Values.Contains($key))
    {
        $errors.Add("Source references missing resource key '$key'.")
    }
}

foreach ($key in $default.Keys)
{
    if ($key -in $referencedKeys)
    {
        continue
    }

    $isDataDriven = $false
    foreach ($prefix in $dataDrivenPrefixes)
    {
        if ($key.StartsWith($prefix, [StringComparison]::Ordinal))
        {
            $isDataDriven = $true
            break
        }
    }
    if (-not $isDataDriven)
    {
        $errors.Add("Resource key is not referenced and is not data-driven: '$key'.")
    }
}

if (-not $sourceText.Contains('GetLocalizedDescription(Localization.Strings.ResourceManager)'))
{
    $errors.Add('The data-driven enum localization lookup is missing.')
}
if (-not $sourceText.Contains('"DestinationSettings_Field_" +') -or
    -not $sourceText.Contains('ResourceManager.GetString(resourceName)'))
{
    $errors.Add('The data-driven destination-field localization lookup is missing.')
}

$literalPattern = '(?<![A-Za-z])(?:Text|Content|Header|Title|Watermark|ToolTip\.Tip|PlaceholderText)="([^"]*)"'
foreach ($view in Get-ChildItem (Join-Path $projectDirectory 'Presentation') -Recurse -Filter '*.axaml')
{
    foreach ($match in [regex]::Matches([IO.File]::ReadAllText($view.FullName), $literalPattern))
    {
        $value = $match.Groups[1].Value
        if ($value -match '[A-Za-z]' -and $value -notmatch '^\{')
        {
            $errors.Add("$($view.Name): contains user-visible literal '$value'.")
        }
    }
}

$runtimeLiteralPattern = '(?:Errors\.Add|throw new [A-Za-z]+Exception)\s*\(\s*"[A-Za-z]'
foreach ($source in $sourceFiles | Where-Object Extension -eq '.cs')
{
    if ([regex]::IsMatch([IO.File]::ReadAllText($source.FullName), $runtimeLiteralPattern))
    {
        $errors.Add("$($source.Name): contains a targeted user-visible runtime literal.")
    }
}

$assetResourcePath = Join-Path $projectDirectory 'Properties\Resources.resx'
[xml]$assetDocument = [IO.File]::ReadAllText($assetResourcePath, $strictUtf8)
$assetKeys = @($assetDocument.root.data |
    Where-Object { $_.type -or $_.mimetype } |
    ForEach-Object { [string]$_.name })
$legacyReferences = @([regex]::Matches($sourceText, '(?<![A-Za-z0-9_])Resources\.([A-Za-z_][A-Za-z0-9_]*)') |
    ForEach-Object { $_.Groups[1].Value } |
    Sort-Object -Unique)
foreach ($key in $legacyReferences)
{
    if ($key -notin $assetKeys)
    {
        $errors.Add("Source references legacy text resource 'Properties.Resources.$key'.")
    }
}

foreach ($source in $sourceFiles)
{
    if ([IO.File]::ReadAllText($source.FullName).Contains('TODO: Translate'))
    {
        $errors.Add("$($source.Name): contains an unresolved translation TODO.")
    }
}

Write-Host "Default keys: $($default.Keys.Count)"
Write-Host "Localized cultures: $($cultureFiles.Count)"
Write-Host "Directly referenced keys: $($referencedKeys.Count)"
Write-Host "Data-driven keys: $($default.Keys.Count - $referencedKeys.Count)"

if ($errors.Count -gt 0)
{
    $errors | ForEach-Object { Write-Error $_ }
    exit 1
}

Write-Host 'UploadersLib localization validation succeeded.'
