[CmdletBinding()]
param()

# Checks that the user's accent color is only used through the brushes meant for each use
# (see ShareX.Avalonia/Theming/AccentPalette.cs):
#   ShareX.Brush.Accent / .Start      accent fills (backgrounds), never text, icons or borders
#   ShareX.Brush.Accent.Foreground    text and icons drawn on an accent fill
#   ShareX.Brush.Accent.Text          accent colored text drawn on a theme background (contrast enforced)
#   ShareX.Brush.Accent.Icon          accent colored icons drawn on a theme background (the accent as picked)
#   ShareX.Brush.Accent.Border        accent colored borders and indicators drawn on a theme background
# The accent can be any color, so the raw accent used as text disappears on some backgrounds, and a fixed color
# (such as White) on an accent fill disappears on some accents.

$ErrorActionPreference = 'Stop'

$repositoryDirectory = Split-Path -Parent $PSScriptRoot
$errors = [Collections.Generic.List[string]]::new()

# The theme files define the accent resources and their fills.
$excludedFiles = @(
    'ShareX.Avalonia/Theming/ShareXTheme.axaml'
)

$rawAccent = 'ShareX\.Brush\.Accent(\.Start|\.End)?|SystemAccentColor\w*|SystemControl\w*Accent\w*Brush'
$rawAccentResource = "\{(DynamicResource|StaticResource) ($rawAccent)\}"
$accentFill = "\{(DynamicResource|StaticResource) ($rawAccent)\}"
$onFillResource = '{DynamicResource ShareX.Brush.Accent.Foreground}'
$textProperties = 'Foreground|BorderBrush|Stroke|CaretBrush|TextElement\.Foreground'
# Style selectors whose target is an icon: an IconPresenter, or a TextBlock that draws a menu icon glyph.
$iconSelector = '(IconPresenter|TextBlock\.accent-menu-icon)[\w.:-]*$'

function Get-RelativePath([string]$path) {
    return [IO.Path]::GetRelativePath($repositoryDirectory, $path).Replace('\', '/')
}

function Add-Error([string]$file, [int]$line, [string]$message) {
    $errors.Add("${file}:${line}: $message")
}

function Get-LineNumber([string]$text, [int]$index) {
    return ([regex]::Matches($text.Substring(0, $index), "`n")).Count + 1
}

$sourceFiles = git -C $repositoryDirectory ls-files '*.axaml' '*.cs'
if ($LASTEXITCODE -ne 0) {
    throw 'git ls-files failed.'
}

foreach ($relativePath in $sourceFiles) {
    if ($excludedFiles -contains $relativePath) {
        continue
    }

    $path = Join-Path $repositoryDirectory $relativePath
    $text = [IO.File]::ReadAllText($path)

    if ($relativePath.EndsWith('.axaml')) {
        # 1. The raw accent as a text, icon or border color, as an attribute or a style setter.
        foreach ($match in [regex]::Matches($text, "\b($textProperties)=""$rawAccentResource""")) {
            Add-Error $relativePath (Get-LineNumber $text $match.Index) "$($match.Groups[1].Value) uses the raw accent; use ShareX.Brush.Accent.Text (text), ShareX.Brush.Accent.Icon (icons) or ShareX.Brush.Accent.Border (borders) on a background, or ShareX.Brush.Accent.Foreground on an accent fill."
        }

        foreach ($match in [regex]::Matches($text, "<Setter Property=""($textProperties)"" Value=""$rawAccentResource""")) {
            Add-Error $relativePath (Get-LineNumber $text $match.Index) "$($match.Groups[1].Value) setter uses the raw accent; use ShareX.Brush.Accent.Text, ShareX.Brush.Accent.Icon, ShareX.Brush.Accent.Border or ShareX.Brush.Accent.Foreground."
        }

        # 2. A style that fills with the accent and sets a text color other than the on-fill brush.
        foreach ($style in [regex]::Matches($text, '(?s)<Style Selector="(?<selector>[^"]*)">(?<body>(?:(?!<Style ).)*?)</Style>')) {
            $body = $style.Groups['body'].Value
            if ($body -notmatch "<Setter Property=""Background"" Value=""$accentFill""") {
                continue
            }

            foreach ($setter in [regex]::Matches($body, '<Setter Property="Foreground" Value="(?<value>[^"]*)"')) {
                $value = $setter.Groups['value'].Value
                if ($value -ne $onFillResource) {
                    Add-Error $relativePath (Get-LineNumber $text ($style.Index + $style.Groups['body'].Index + $setter.Index)) "Style '$($style.Groups['selector'].Value)' fills with the accent but sets Foreground to '$value'; use $onFillResource."
                }
            }
        }

        # 3. Text and icons on a theme background: icons use the icon brush and text uses the contrast-enforced text brush.
        foreach ($match in [regex]::Matches($text, '\{(DynamicResource|StaticResource) ShareX\.Brush\.Accent\.(?<kind>Text|Icon)\}')) {
            $tagStart = $text.LastIndexOf('<', $match.Index)
            $tag = [regex]::Match($text.Substring($tagStart), '^<\s*(?<name>[\w:.|]+)').Groups['name'].Value
            if ($tag -eq 'Setter') {
                $styleStart = $text.LastIndexOf('<Style ', $match.Index)
                $target = if ($styleStart -ge 0) { [regex]::Match($text.Substring($styleStart), 'Selector="(?<selector>[^"]*)"').Groups['selector'].Value } else { '' }
                $isIcon = $target -ne '' -and @($target.Split(',') | Where-Object { $_.Trim() -notmatch $iconSelector }).Count -eq 0
            }
            else {
                $target = "<$tag>"
                $isIcon = $tag -match 'IconPresenter$'
            }

            if ($match.Groups['kind'].Value -eq 'Text' -and $isIcon) {
                Add-Error $relativePath (Get-LineNumber $text $match.Index) "Icon '$target' uses ShareX.Brush.Accent.Text; use ShareX.Brush.Accent.Icon."
            }
            elseif ($match.Groups['kind'].Value -eq 'Icon' -and -not $isIcon) {
                Add-Error $relativePath (Get-LineNumber $text $match.Index) "'$target' is not an icon but uses ShareX.Brush.Accent.Icon; use ShareX.Brush.Accent.Text so the text stays readable."
            }
        }

        # 4. An element filled with the accent whose content sets a fixed or theme text color.
        try {
            [xml]$document = $text
        }
        catch {
            Add-Error $relativePath 1 "Could not be parsed as XML: $($_.Exception.Message)"
            continue
        }

        foreach ($element in $document.SelectNodes('//*[@Background]')) {
            if ($element.GetAttribute('Background') -notmatch "^$accentFill$") {
                continue
            }

            foreach ($child in $element.SelectNodes('.//*[@Foreground]')) {
                $value = $child.GetAttribute('Foreground')
                if ($value -ne $onFillResource) {
                    # XmlDocument has no line info, so report the first line holding the element name and value.
                    $needle = [regex]::Escape("Foreground=""$value""")
                    $lineMatch = [regex]::Match($text, $needle)
                    $line = if ($lineMatch.Success) { Get-LineNumber $text $lineMatch.Index } else { 1 }
                    Add-Error $relativePath $line "<$($child.LocalName)> sits on an accent-filled <$($element.LocalName)> but uses Foreground '$value'; use $onFillResource."
                }
            }
        }
    }
    else {
        # 5. Code that colors text or icons with the raw accent resource.
        foreach ($match in [regex]::Matches($text, "(Foreground|BorderBrush|CaretBrush)[^;\r\n]*""($rawAccent)""")) {
            Add-Error $relativePath (Get-LineNumber $text $match.Index) "$($match.Groups[1].Value) uses the raw accent resource '$($match.Groups[2].Value)'; use ShareX.Brush.Accent.Text, ShareX.Brush.Accent.Icon, ShareX.Brush.Accent.Border or ShareX.Brush.Accent.Foreground."
        }
    }
}

if ($errors.Count -gt 0) {
    foreach ($errorMessage in $errors) {
        Write-Error $errorMessage -ErrorAction Continue
    }

    throw "Accent contrast validation failed with $($errors.Count) error(s)."
}

Write-Host 'Accent contrast validation passed.'
