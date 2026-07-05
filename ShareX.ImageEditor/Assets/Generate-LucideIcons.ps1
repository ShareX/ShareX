[CmdletBinding()]
param(
    [string] $CodepointsPath,
    [string] $OutputPath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($CodepointsPath))
{
    $CodepointsPath = Join-Path $PSScriptRoot "lucide-codepoints.json"
}

if ([string]::IsNullOrWhiteSpace($OutputPath))
{
    $OutputPath = Join-Path $PSScriptRoot "..\Presentation\Theming\LucideIcons.cs"
}

$codepoints = Get-Content -LiteralPath $CodepointsPath -Raw | ConvertFrom-Json

if ($null -eq $codepoints -or $codepoints -isnot [PSCustomObject])
{
    throw "Expected '$CodepointsPath' to contain a JSON object."
}

$reservedKeywords = [System.Collections.Generic.HashSet[string]]::new(
    [string[]] @(
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
        "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else",
        "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for",
        "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock",
        "long", "namespace", "new", "null", "object", "operator", "out", "override", "params",
        "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short",
        "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true",
        "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual",
        "void", "volatile", "while"
    ),
    [System.StringComparer]::Ordinal
)

$entries = foreach ($property in $codepoints.PSObject.Properties)
{
    $identifier = $property.Name.Replace("-", "_")

    if ($identifier -cnotmatch "^[A-Za-z_][A-Za-z0-9_]*$")
    {
        throw "Icon name '$($property.Name)' cannot be converted to a valid C# identifier."
    }

    $codepoint = 0

    if (-not [int]::TryParse($property.Value.ToString(), [ref] $codepoint) -or
        $codepoint -lt 0 -or
        $codepoint -gt 0x10FFFF -or
        ($codepoint -ge 0xD800 -and $codepoint -le 0xDFFF))
    {
        throw "Icon '$($property.Name)' has invalid Unicode code point '$($property.Value)'."
    }

    [PSCustomObject] @{
        Identifier = $identifier
        Codepoint = $codepoint
    }
}

$duplicate = $entries | Group-Object -Property Identifier | Where-Object Count -gt 1 | Select-Object -First 1

if ($null -ne $duplicate)
{
    throw "Multiple icon names convert to the C# identifier '$($duplicate.Name)'."
}

$entries = $entries | Sort-Object -Property Identifier

$resolvedOutputPath = [System.IO.Path]::GetFullPath($OutputPath)
$outputDirectory = [System.IO.Path]::GetDirectoryName($resolvedOutputPath)

if (-not [System.IO.Directory]::Exists($outputDirectory))
{
    throw "Output directory '$outputDirectory' does not exist."
}

$namespaceDeclaration = "namespace ShareX.ImageEditor.Presentation.Theming;"
$lines = [System.Collections.Generic.List[string]]::new()

if ([System.IO.File]::Exists($resolvedOutputPath))
{
    $existingContent = [System.IO.File]::ReadAllText($resolvedOutputPath)
    $namespaceIndex = $existingContent.IndexOf($namespaceDeclaration, [System.StringComparison]::Ordinal)

    if ($namespaceIndex -gt 0)
    {
        $preservedHeader = $existingContent.Substring(0, $namespaceIndex).TrimEnd("`r", "`n")
        $lines.AddRange([string[]] ($preservedHeader -split "`r?`n"))
        $lines.Add("")
    }
}

$lines.AddRange([string[]] @(
    $namespaceDeclaration,
    "",
    "public static class LucideIcons",
    "{"
))

foreach ($entry in $entries)
{
    $identifier = $entry.Identifier

    if ($reservedKeywords.Contains($identifier))
    {
        $identifier = "@$identifier"
    }

    if ($entry.Codepoint -le 0xFFFF)
    {
        $escape = "\u{0:X4}" -f $entry.Codepoint
    }
    else
    {
        $escape = "\U{0:X8}" -f $entry.Codepoint
    }

    $lines.Add("    public const string $identifier = `"$escape`";")
}

$lines.Add("}")

$content = [string]::Join("`r`n", $lines)
$utf8WithBom = [System.Text.UTF8Encoding]::new($true)
[System.IO.File]::WriteAllText($resolvedOutputPath, $content, $utf8WithBom)

Write-Host "Generated $($entries.Count) icons in '$resolvedOutputPath'."
