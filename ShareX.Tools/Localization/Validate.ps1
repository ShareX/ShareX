$ErrorActionPreference = 'Stop'

$localizationDirectory = $PSScriptRoot
$projectDirectory = Split-Path $localizationDirectory -Parent
$defaultPath = Join-Path $localizationDirectory 'Strings.resx'
$expectedCultures = @(
    'ar-YE', 'de', 'es', 'es-MX', 'fa-IR', 'fr', 'he-IL', 'hu', 'id-ID', 'it-IT', 'ja-JP', 'ko-KR',
    'nl-NL', 'pl', 'pt-BR', 'pt-PT', 'ro', 'ru', 'tr', 'uk', 'vi-VN', 'zh-CN', 'zh-TW'
)
$strictUtf8 = New-Object Text.UTF8Encoding($false, $true)
$legacyEncodings = @(
    [Text.Encoding]::GetEncoding(28591, (New-Object Text.EncoderExceptionFallback), (New-Object Text.DecoderExceptionFallback)),
    [Text.Encoding]::GetEncoding(1252, (New-Object Text.EncoderExceptionFallback), (New-Object Text.DecoderExceptionFallback))
)

function Read-Resources([string] $path) {
    $bytes = [IO.File]::ReadAllBytes($path)
    if ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF) {
        throw "Resource file has a UTF-8 BOM: $path"
    }

    $text = $strictUtf8.GetString($bytes)
    if (-not $text.EndsWith("`r`n")) {
        throw "Resource file has no final CRLF newline: $path"
    }
    if ($text -match '(?<!\r)\n') {
        throw "Resource file contains a bare LF newline: $path"
    }
    if ($text -match '[\u0080-\u009F\uFFFD]') {
        throw "Resource file contains an invalid control or replacement character: $path"
    }

    [xml] $document = $text
    $result = @{}
    foreach ($item in $document.root.data) {
        $key = [string] $item.name
        $value = [string] $item.value
        if ($result.ContainsKey($key)) {
            throw "Duplicate resource key: $path ($key)"
        }
        foreach ($encoding in $legacyEncodings) {
            try {
                $decoded = $strictUtf8.GetString($encoding.GetBytes($value))
                if ($decoded -ne $value) {
                    throw "Resource value appears to contain reversible mojibake: $path ($key)"
                }
            }
            catch [Text.EncoderFallbackException] {}
            catch [Text.DecoderFallbackException] {}
        }
        $result[$key] = $value
    }
    return $result
}

function Get-Placeholders([string] $value) {
    return @([regex]::Matches($value, '(?<!\{)\{\d+(?:[^}]*)\}(?!\})') | ForEach-Object Value | Sort-Object -Unique)
}

$errors = New-Object 'Collections.Generic.List[string]'
$default = Read-Resources $defaultPath
if ($default.Count -ne 553) {
    $errors.Add("Expected 553 default keys, found $($default.Count).")
}
$sharedActionKeys = @(
    'BackgroundRemoverWindow_Browse',
    'DirectoryIndexerWindow_Browse',
    'HashCheckerWindow_Browse',
    'IconConverterWindow_Browse',
    'ImageSplitterWindow_Browse',
    'ImageThumbnailerWindow_Browse',
    'MetadataWindow_Open',
    'VideoConverterWindow_Browse',
    'VideoThumbnailerWindow_Browse'
)

$cultureFiles = @(Get-ChildItem $localizationDirectory -Filter 'Strings.*.resx' | Sort-Object Name)
$actualCultures = @($cultureFiles | ForEach-Object { $_.BaseName.Substring('Strings.'.Length) })
if (($actualCultures -join '|') -ne (($expectedCultures | Sort-Object) -join '|')) {
    $errors.Add("Localized culture set differs. Expected: $($expectedCultures -join ', '); actual: $($actualCultures -join ', ').")
}

foreach ($file in $cultureFiles) {
    $localized = Read-Resources $file.FullName
    foreach ($key in $default.Keys) {
        if (-not $localized.ContainsKey($key)) {
            $errors.Add("$($file.Name): missing '$key'.")
            continue
        }
        if ([string]::IsNullOrWhiteSpace($localized[$key])) {
            $errors.Add("$($file.Name): '$key' is empty.")
        }
        if ($sharedActionKeys -contains $key -and $localized[$key] -eq $default[$key]) {
            $errors.Add("$($file.Name): '$key' still uses the English shared action label.")
        }
        if (((Get-Placeholders $default[$key]) -join '|') -ne ((Get-Placeholders $localized[$key]) -join '|')) {
            $errors.Add("$($file.Name): '$key' has different placeholders.")
        }
    }
    foreach ($key in $localized.Keys) {
        if (-not $default.ContainsKey($key)) {
            $errors.Add("$($file.Name): unexpected '$key'.")
        }
    }
}

$sourceFiles = @(Get-ChildItem $projectDirectory -Recurse -Include '*.axaml','*.cs' | Where-Object {
    $_.FullName -notmatch '\\(?:obj|bin|Localization|\.codex-build)\\'
})
$sourceText = ($sourceFiles | ForEach-Object { [IO.File]::ReadAllText($_.FullName) }) -join "`n"
foreach ($key in $default.Keys) {
    if ($sourceText -notmatch [regex]::Escape("Strings.$key")) {
        $errors.Add("Resource key is not referenced: '$key'.")
    }
}

$literalPattern = '(?<![A-Za-z0-9_.])(?:Text|Content|Header|Title|Watermark|PlaceholderText|ToolTip\.Tip|OnContent|OffContent|AutomationProperties\.Name)="([^"]*)"'
foreach ($view in $sourceFiles | Where-Object Extension -eq '.axaml') {
    foreach ($match in [regex]::Matches([IO.File]::ReadAllText($view.FullName), $literalPattern)) {
        $value = $match.Groups[1].Value
        if ($value -match '[A-Za-z]' -and
            $value -notmatch '^\{(?:x:Static|Binding|DynamicResource|StaticResource) ' -and
            $value -ne '{Binding}' -and
            $value -notmatch '^(Auto|Center|CenterOwner|CenterScreen|Disabled|False|Left|Normal|Right|SemiBold|Stretch|True|Uniform)$' -and
            $value -notmatch '^[A-Za-z]+(?:,[A-Za-z]+)+$') {
            $errors.Add("$($view.Name): contains user-visible literal '$value'.")
        }
    }
}

if ($sourceText -match 'using\s+\w+\s*=\s*ShareX\.Tools\.Localization\.Strings') {
    $errors.Add('Source contains a localization resource alias.')
}
if ($sourceText -match '(?m)\b(?:string|object)\s+L\s*\(') {
    $errors.Add('Source contains an L() localization helper.')
}

Write-Host "Default keys: $($default.Count)"
Write-Host "Localized cultures: $($cultureFiles.Count)"
if ($errors.Count -gt 0) {
    $errors | ForEach-Object { Write-Host $_ -ForegroundColor Red }
    exit 1
}
Write-Host 'ShareX.Tools localization validation succeeded.'
