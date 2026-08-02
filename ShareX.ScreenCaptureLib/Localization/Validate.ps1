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
    $bytes = [IO.File]::ReadAllBytes($path)
    if ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF)
    {
        throw "Resource file has a UTF-8 BOM: $path"
    }

    $text = $strictUtf8.GetString($bytes)
    if (-not $text.EndsWith("`n"))
    {
        throw "Resource file has no final newline: $path"
    }
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

function Get-CommandPlaceholders([string]$value)
{
    return @([regex]::Matches($value, '\$[a-z_]+\$') |
        ForEach-Object Value |
        Sort-Object -Unique)
}

$errors = [Collections.Generic.List[string]]::new()
$default = Read-Resources $defaultPath

if ($default.Count -ne 221)
{
    $errors.Add("Expected 221 default keys, found $($default.Count).")
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
        if (((Get-Placeholders $default[$key]) -join '|') -ne ((Get-Placeholders $localized[$key]) -join '|'))
        {
            $errors.Add("$($file.Name): '$key' has different format placeholders.")
        }
        if (((Get-CommandPlaceholders $default[$key]) -join '|') -ne ((Get-CommandPlaceholders $localized[$key]) -join '|'))
        {
            $errors.Add("$($file.Name): '$key' has different command placeholders.")
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

$sourceText = ''
foreach ($source in Get-ChildItem $projectDirectory -Recurse -File -Include '*.axaml','*.cs' |
    Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' -and $_.Name -notin @('Resources.Designer.cs', 'Strings.Designer.cs') })
{
    $sourceText += [IO.File]::ReadAllText($source.FullName)
}
foreach ($key in $default.Keys)
{
    if ($sourceText -notmatch [regex]::Escape("Strings.$key"))
    {
        $errors.Add("Resource key is not referenced: '$key'.")
    }
}

$literalPattern = '(?:Text|Content|Header|Title|Watermark|ToolTip\.Tip|PlaceholderText)="([^"]*)"'
foreach ($view in Get-ChildItem (Join-Path $projectDirectory 'Presentation') -Recurse -Filter '*.axaml')
{
    foreach ($match in [regex]::Matches([IO.File]::ReadAllText($view.FullName), $literalPattern))
    {
        $value = $match.Groups[1].Value
        if ($value -match '[A-Za-z]' -and
            $value -notmatch '^\{x:Static ' -and
            $value -notmatch '^(Auto|Center|CenterScreen|Disabled|False|Goldenrod|Lime|None|Normal|SemiBold|SizeAll|Transparent|True)$' -and
            $value -notmatch '^[A-Za-z]+(?:,[A-Za-z]+)+$')
        {
            $errors.Add("$($view.Name): contains user-visible literal '$value'.")
        }
    }
}

Write-Host "Default keys: $($default.Count)"
Write-Host "Localized cultures: $($cultureFiles.Count)"

if ($errors.Count -gt 0)
{
    $errors | ForEach-Object { Write-Error $_ }
    exit 1
}

Write-Host 'ScreenCaptureLib localization validation succeeded.'
