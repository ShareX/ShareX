Set-StrictMode -Version Latest

function Read-TranslationResource([string]$Path)
{
    $settings = [Xml.XmlReaderSettings]::new()
    $settings.DtdProcessing = [Xml.DtdProcessing]::Prohibit
    $settings.IgnoreWhitespace = $true
    $settings.XmlResolver = $null
    $document = [Xml.XmlDocument]::new()
    $document.XmlResolver = $null
    $reader = [Xml.XmlReader]::Create($Path, $settings)
    try { $document.Load($reader) } finally { $reader.Dispose() }
    if ($document.DocumentElement.LocalName -cne 'root') { throw "Not a RESX resource: $Path" }
    $nodes = [Collections.Generic.Dictionary[string, Xml.XmlElement]]::new([StringComparer]::Ordinal)
    $values = [Collections.Generic.Dictionary[string, string]]::new([StringComparer]::Ordinal)
    foreach ($node in $document.DocumentElement.SelectNodes('data'))
    {
        $name = $node.GetAttribute('name')
        if ($nodes.ContainsKey($name)) { throw "Duplicate resource '$name': $Path" }
        $nodes.Add($name, $node)
        # Typed and binary resources remain in the XML, but are not translation tasks.
        if (-not $node.HasAttribute('type') -and -not $node.HasAttribute('mimetype'))
        {
            $value = $node.SelectSingleNode('value')
            if ($null -eq $value) { throw "Resource '$name' has no value: $Path" }
            $values.Add($name, $value.InnerText)
        }
    }
    return [pscustomobject]@{ Path = $Path; Document = $document; Nodes = $nodes; Values = $values }
}

function Format-TranslationResource([Xml.XmlDocument]$Document)
{
    $root = $Document.DocumentElement
    $elements = [Collections.Generic.List[Xml.XmlElement]]::new()
    $names = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    foreach ($node in $root.SelectNodes('data'))
    {
        if (-not $names.Add($node.GetAttribute('name'))) { throw "Duplicate resource '$($node.GetAttribute('name'))'." }
        $elements.Add($node)
    }
    if ($elements.Count -gt 0)
    {
        $insertBefore = $elements[$elements.Count - 1].NextSibling
        foreach ($element in $elements) { $null = $root.RemoveChild($element) }
        $elements.Sort([Comparison[Xml.XmlElement]]{
            param($left, $right)
            [StringComparer]::Ordinal.Compare($left.GetAttribute('name'), $right.GetAttribute('name'))
        })
        foreach ($element in $elements)
        {
            if ($null -eq $insertBefore) { $null = $root.AppendChild($element) }
            else { $null = $root.InsertBefore($element, $insertBefore) }
        }
    }
    $settings = [Xml.XmlWriterSettings]::new()
    $settings.Encoding = [Text.UTF8Encoding]::new($false)
    $settings.Indent = $true
    $settings.IndentChars = '  '
    $settings.NewLineChars = "`r`n"
    $settings.NewLineHandling = [Xml.NewLineHandling]::Replace
    $stream = [IO.MemoryStream]::new()
    try
    {
        $writer = [Xml.XmlWriter]::Create($stream, $settings)
        try
        {
            $Document.Save($writer)
            $writer.WriteWhitespace("`r`n")
            $writer.Flush()
        }
        finally { $writer.Dispose() }
        return ,$stream.ToArray()
    }
    finally { $stream.Dispose() }
}

function Get-TranslationSourceHash([string]$Value)
{
    return [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($Value))).ToLowerInvariant()
}

function Get-TranslationApprovals([string]$RepositoryRoot)
{
    $approvals = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    $path = Join-Path $RepositoryRoot 'Scripts/TranslationEnglishAllowlist.txt'
    if (Test-Path -LiteralPath $path)
    {
        foreach ($line in [IO.File]::ReadAllLines($path))
        {
            if ([string]::IsNullOrWhiteSpace($line) -or $line.StartsWith('#')) { continue }
            $parts = $line.Split('|')
            if ($parts.Count -ne 4) { throw "Malformed translation approval: $line" }
            foreach ($culture in $parts[3].Split(','))
            {
                $null = $approvals.Add("$($parts[0])|$culture|$($parts[1])|$($parts[2])")
            }
        }
    }
    return ,$approvals
}

function Get-TranslationProblem(
    [string]$Source, [AllowNull()][string]$Value, [string]$Project, [string]$Key, [string]$Culture,
    [Collections.Generic.HashSet[string]]$Approvals)
{
    if ([string]::IsNullOrWhiteSpace($Value)) { return 'Missing or empty translation' }
    if ($Value -match '[\u0080-\u009F\uFFFD]') { return 'Invalid control or replacement character' }
    foreach ($pattern in @('(?<!\{)\{(?:\d+(?:[^}]*)|[A-Za-z_][A-Za-z0-9_]*)\}(?!\})', '\$[a-z_]+\$'))
    {
        $sourceTokens = @([regex]::Matches($Source, $pattern) | ForEach-Object Value | Sort-Object -Unique -CaseSensitive)
        $valueTokens = @([regex]::Matches($Value, $pattern) | ForEach-Object Value | Sort-Object -Unique -CaseSensitive)
        if (($sourceTokens -join "`n") -cne ($valueTokens -join "`n")) { return 'Placeholder mismatch' }
    }
    $normalizedSource = [regex]::Replace($Source.Normalize().ToLowerInvariant(), '[\p{P}\p{Z}]', '')
    $normalizedValue = [regex]::Replace($Value.Normalize().ToLowerInvariant(), '[\p{P}\p{Z}]', '')
    if ($Source -match '[A-Za-z]' -and $normalizedSource -ceq $normalizedValue)
    {
        if (-not $Approvals.Contains("$Project|$Culture|$Key|$(Get-TranslationSourceHash $Source)"))
        {
            return 'English-equivalent value needs a reviewed allowlist entry'
        }
    }
    $scripts = @{
        'ar-YE' = '[\u0600-\u06FF]'; 'fa-IR' = '[\u0600-\u06FF]'; 'he-IL' = '[\u0590-\u05FF]'
        'hi' = '[\u0900-\u097F]'; 'ja-JP' = '[\u3040-\u30FF\u3400-\u9FFF]'
        'ko-KR' = '[\u1100-\u11FF\u3130-\u318F\uAC00-\uD7AF\u3400-\u9FFF]'
        'ru' = '[\u0400-\u04FF]'; 'th' = '[\u0E00-\u0E7F]'; 'uk' = '[\u0400-\u04FF]'
        'zh-CN' = '[\u3400-\u9FFF]'; 'zh-TW' = '[\u3400-\u9FFF]'
    }
    if ($scripts.ContainsKey($Culture) -and [regex]::Matches($Value, '[A-Za-z]{4,}').Count -ge 2 -and
        $Value -notmatch $scripts[$Culture]) { return 'Expected localized script is missing' }
    return $null
}

function Read-TranslationBatch([string]$Path)
{
    $text = [Text.UTF8Encoding]::new($false, $true).GetString([IO.File]::ReadAllBytes($Path)).TrimStart([char]0xFEFF)
    $document = [Text.Json.JsonDocument]::Parse($text)
    function Test-JsonMembers([Text.Json.JsonElement]$element)
    {
        if ($element.ValueKind -eq [Text.Json.JsonValueKind]::Object)
        {
            $names = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
            foreach ($property in $element.EnumerateObject())
            {
                if (-not $names.Add($property.Name)) { throw "Duplicate JSON member '$($property.Name)'." }
                Test-JsonMembers $property.Value
            }
        }
        elseif ($element.ValueKind -eq [Text.Json.JsonValueKind]::Array)
        {
            foreach ($item in $element.EnumerateArray()) { Test-JsonMembers $item }
        }
    }
    try { Test-JsonMembers $document.RootElement } finally { $document.Dispose() }
    return ConvertFrom-Json -InputObject $text -AsHashtable -Depth 100
}

Export-ModuleMember -Function Read-TranslationResource, Format-TranslationResource, Get-TranslationSourceHash,
    Get-TranslationApprovals, Get-TranslationProblem, Read-TranslationBatch
