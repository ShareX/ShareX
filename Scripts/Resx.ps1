#requires -Version 7.2
<#
.SYNOPSIS
Export, find, import, or check ShareX translation strings without loading whole catalogs into an agent's context.
.EXAMPLE
pwsh -File Scripts/Resx.ps1 export -Project ShareX.Tools -Prefix AnimatedGifTrimmer_ -Path artifacts/gif.json
.EXAMPLE
pwsh -File Scripts/Resx.ps1 find -Text 'Pause' -Culture tr,fr
.EXAMPLE
pwsh -File Scripts/Resx.ps1 import -Path artifacts/gif.json -WhatIf
.EXAMPLE
pwsh -File Scripts/Resx.ps1 check -Project ShareX.Tools -Prefix AnimatedGifTrimmer_
#>
[CmdletBinding(SupportsShouldProcess)]
param(
    [Parameter(Mandatory, Position = 0)][ValidateSet('export', 'find', 'import', 'check')][string]$Command,
    [string[]]$Project,
    [string[]]$Prefix,
    [string[]]$Key,
    [string[]]$Culture,
    [string]$Text,
    [string]$Path,
    [switch]$IncludeExisting,
    [ValidateRange(1, 10000)][int]$MaxEntries = 40,
    [ValidateRange(0, 10)][int]$MaxCandidates = 2,
    [string]$RepositoryRoot = (Split-Path -Parent $PSScriptRoot)
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
Import-Module (Join-Path $PSScriptRoot 'TranslationResources.psm1') -Force
function Expand-List([string[]]$items)
{
    foreach ($item in $items)
    {
        foreach ($part in $item.Split(',')) { if (-not [string]::IsNullOrWhiteSpace($part)) { $part.Trim() } }
    }
}
# Native pwsh -File passes comma-separated options as a single string.
$Project = @(Expand-List $Project)
$Prefix = @(Expand-List $Prefix)
$Key = @(Expand-List $Key)
$Culture = @(Expand-List $Culture)
$repositoryDirectory = (Resolve-Path -LiteralPath $RepositoryRoot).Path
$projects = [Collections.Generic.Dictionary[string, string]]::new([StringComparer]::Ordinal)
$availableCultures = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
foreach ($directory in Get-ChildItem -LiteralPath $repositoryDirectory -Directory)
{
    $localization = Join-Path $directory.FullName 'Localization'
    if (Test-Path -LiteralPath (Join-Path $localization 'Strings.resx'))
    {
        $projects.Add($directory.Name, $localization)
        foreach ($file in Get-ChildItem -LiteralPath $localization -Filter 'Strings.*.resx')
        {
            $null = $availableCultures.Add($file.BaseName.Substring('Strings.'.Length))
        }
    }
}
if ($projects.Count -eq 0) { throw "No Localization/Strings.resx catalogs found in $repositoryDirectory." }
foreach ($name in $Project) { if (-not $projects.ContainsKey($name)) { throw "Unknown project '$name'." } }
foreach ($name in $Culture) { if (-not $availableCultures.Contains($name)) { throw "Unknown catalog culture '$name'." } }
$selectedProjects = if ($Project) { @($Project | Sort-Object -Unique) } else { @($projects.Keys | Sort-Object) }
$selectedCultures = if ($Culture) { @($Culture | Sort-Object -Unique) } else { @($availableCultures | Sort-Object) }
$approvals = Get-TranslationApprovals $repositoryDirectory
$catalogs = @{}
function Get-Catalog([string]$projectName, [string]$cultureName = '')
{
    $catalogPath = Join-Path $projects[$projectName] $(if ($cultureName) { "Strings.$cultureName.resx" } else { 'Strings.resx' })
    if (-not $catalogs.ContainsKey($catalogPath))
    {
        if (-not (Test-Path -LiteralPath $catalogPath)) { throw "Missing catalog: $catalogPath" }
        $catalogs[$catalogPath] = Read-TranslationResource $catalogPath
    }
    return $catalogs[$catalogPath]
}
function Get-Value($catalog, [string]$keyName)
{
    if ($catalog.Values.ContainsKey($keyName)) { return $catalog.Values[$keyName] }
    return $null
}
function Test-KeySelected([string]$keyName)
{
    if ($Key -and $keyName -cnotin $Key) { return $false }
    if ($Prefix)
    {
        foreach ($item in $Prefix) { if ($keyName.StartsWith($item, [StringComparison]::Ordinal)) { return $true } }
        return $false
    }
    return $true
}
function Write-JsonFile($value, [string]$outputPath)
{
    $json = ConvertTo-Json -InputObject $value -Depth 20
    [IO.File]::WriteAllText([IO.Path]::GetFullPath($outputPath), $json + "`n", [Text.UTF8Encoding]::new($false))
}

if ($Command -eq 'import')
{
    if ($Project -or $Prefix -or $Key -or $Culture -or $Text -or $IncludeExisting)
    { throw 'import reads its scope from the batch; filtering parameters are for export, find, and check.' }
    if (-not $Path) { throw 'import requires -Path.' }
    $batch = Read-TranslationBatch ([IO.Path]::GetFullPath($Path))
    if ($batch -isnot [Collections.IDictionary] -or $batch['version'] -ne 1 -or $batch['entries'] -isnot [array])
    { throw 'Expected a version 1 translation batch with an entries array.' }
    $seen = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    $changes = [Collections.Generic.List[object]]::new()
    foreach ($entry in $batch['entries'])
    {
        if ($entry -isnot [Collections.IDictionary]) { throw 'Every batch entry must be an object.' }
        $projectName = $entry['project']; $keyName = $entry['key']
        if ($projectName -isnot [string] -or -not $projects.ContainsKey($projectName)) { throw "Unknown batch project '$projectName'." }
        if ($keyName -isnot [string] -or -not $seen.Add("$projectName|$keyName")) { throw "Invalid or duplicate batch key '$projectName/$keyName'." }
        $sourceCatalog = Get-Catalog $projectName
        if (-not $sourceCatalog.Values.ContainsKey($keyName)) { throw "Unknown source key '$projectName/$keyName'." }
        $source = $sourceCatalog.Values[$keyName]
        if ($entry['source'] -isnot [string] -or $source -cne $entry['source']) { throw "Source changed: $projectName/$keyName. Export a fresh batch." }
        if ($entry['translations'] -isnot [Collections.IDictionary] -or $entry['existing'] -isnot [Collections.IDictionary])
        { throw "Missing translations or existing snapshot: $projectName/$keyName." }
        $reuse = $null
        if ($entry.Contains('reuseFrom'))
        {
            $reference = $entry['reuseFrom']
            if ($reference -isnot [Collections.IDictionary] -or $reference['project'] -isnot [string] -or
                -not $projects.ContainsKey($reference['project']) -or $reference['key'] -isnot [string])
            { throw "Invalid reuseFrom reference: $projectName/$keyName." }
            $reuse = Get-Catalog $reference['project']
            if (-not $reuse.Values.ContainsKey($reference['key']) -or $reuse.Values[$reference['key']] -cne $source)
            { throw "reuseFrom must have the same English source: $projectName/$keyName." }
        }
        foreach ($cultureName in $entry['translations'].Keys)
        {
            if (-not $availableCultures.Contains($cultureName)) { throw "Unknown batch culture '$cultureName'." }
            $value = $entry['translations'][$cultureName]
            if ($null -eq $value -and $null -ne $reuse)
            {
                $value = Get-Value (Get-Catalog $reference['project'] $cultureName) $reference['key']
                if ($null -eq $value) { throw "reuseFrom has no translation for $cultureName`: $projectName/$keyName." }
            }
            if ($null -eq $value) { continue } # Partially translated batches can be imported and resumed.
            if ($value -isnot [string]) { throw "Translation must be a string: $projectName/$keyName/$cultureName." }
            $problem = Get-TranslationProblem $source $value $projectName $keyName $cultureName $approvals
            if ($problem) { throw "$projectName/$keyName/$cultureName`: $problem" }
            $catalog = Get-Catalog $projectName $cultureName
            if ($catalog.Nodes.ContainsKey($keyName) -and -not $catalog.Values.ContainsKey($keyName))
            { throw "Target resource is not a string: $projectName/$keyName/$cultureName." }
            $current = Get-Value $catalog $keyName
            if ($current -ceq $value) { continue } # Re-importing an applied batch is a no-op.
            $expected = if ($entry['existing'].Contains($cultureName)) { $entry['existing'][$cultureName] } else { $null }
            if ($current -cne $expected) { throw "Translation changed: $projectName/$keyName/$cultureName. Export a fresh batch." }
            $changes.Add([pscustomobject]@{ Catalog = $catalog; Key = $keyName; Value = $value })
        }
    }
    # All source, snapshot, and value checks finish before any catalog is written.
    $changedCatalogs = @{}
    foreach ($change in $changes)
    {
        $catalog = $change.Catalog
        if (-not $catalog.Nodes.ContainsKey($change.Key))
        {
            $node = $catalog.Document.CreateElement('data')
            $node.SetAttribute('name', $change.Key)
            $node.SetAttribute('space', 'http://www.w3.org/XML/1998/namespace', 'preserve') | Out-Null
            $null = $node.AppendChild($catalog.Document.CreateElement('value'))
            $null = $catalog.Document.DocumentElement.AppendChild($node)
            $catalog.Nodes.Add($change.Key, $node)
        }
        $catalog.Nodes[$change.Key].SelectSingleNode('value').InnerText = $change.Value
        $changedCatalogs[$catalog.Path] = $catalog
    }
    $writes = @($changedCatalogs.Values | Sort-Object Path | ForEach-Object {
        [pscustomobject]@{ Path = $_.Path; Bytes = Format-TranslationResource $_.Document }
    })
    if ($writes.Count -gt 0 -and $PSCmdlet.ShouldProcess($repositoryDirectory, "Apply $($changes.Count) translations to $($writes.Count) catalogs"))
    {
        foreach ($write in $writes)
        {
            $temporaryPath = $write.Path + '.' + [Guid]::NewGuid().ToString('N') + '.tmp'
            try
            {
                [IO.File]::WriteAllBytes($temporaryPath, $write.Bytes)
                [IO.File]::Move($temporaryPath, $write.Path, $true)
            }
            finally { if ([IO.File]::Exists($temporaryPath)) { [IO.File]::Delete($temporaryPath) } }
            Write-Output "Updated $([IO.Path]::GetRelativePath($repositoryDirectory, $write.Path))"
        }
    }
    Write-Output "Import: $($changes.Count) changes in $($writes.Count) catalogs."
    return
}

$sources = [Collections.Generic.List[object]]::new()
$foundKeys = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
foreach ($projectName in $selectedProjects)
{
    $catalog = Get-Catalog $projectName
    foreach ($keyName in $catalog.Values.Keys | Sort-Object -CaseSensitive)
    {
        if (-not (Test-KeySelected $keyName)) { continue }
        $null = $foundKeys.Add($keyName)
        $sources.Add([pscustomobject]@{ Project = $projectName; Key = $keyName; Source = $catalog.Values[$keyName]; Node = $catalog.Nodes[$keyName] })
    }
}
foreach ($keyName in $Key) { if (-not $foundKeys.Contains($keyName)) { throw "No selected source key '$keyName'." } }
if ($Command -eq 'find')
{
    if (-not $Text -and -not $Key -and -not $Prefix) { throw 'find requires -Text, -Key, or -Prefix.' }
    $results = @($sources | Where-Object { -not $Text -or $_.Source.Contains($Text, [StringComparison]::OrdinalIgnoreCase) } |
        Sort-Object @{ Expression = { if ($Text -and $_.Source.Equals($Text, [StringComparison]::OrdinalIgnoreCase)) { 0 } else { 1 } } }, Project, Key |
        Select-Object -First $MaxEntries | ForEach-Object {
            $translations = [ordered]@{}
            foreach ($cultureName in $selectedCultures) { $translations[$cultureName] = Get-Value (Get-Catalog $_.Project $cultureName) $_.Key }
            [ordered]@{ project = $_.Project; key = $_.Key; source = $_.Source; translations = $translations }
        })
    $result = [ordered]@{ matches = $results; limit = $MaxEntries }
    if ($Path) { Write-JsonFile $result $Path; Write-Output "Found $($results.Count) matches; wrote $Path." }
    else { ConvertTo-Json -InputObject $result -Depth 20 }
    return
}

$entries = [Collections.Generic.List[object]]::new()
$problems = [Collections.Generic.List[string]]::new()
$checked = 0
foreach ($source in $sources)
{
    $translations = [ordered]@{}; $existing = [ordered]@{}
    foreach ($cultureName in $selectedCultures)
    {
        $catalog = Get-Catalog $source.Project $cultureName
        $value = Get-Value $catalog $source.Key
        $problem = Get-TranslationProblem $source.Source $value $source.Project $source.Key $cultureName $approvals
        $checked++
        if ($problem) { $problems.Add("$($source.Project)/$($source.Key)/$cultureName`: $problem") }
        if ($problem -or $IncludeExisting)
        {
            $translations[$cultureName] = $null
            if ($null -ne $value) { $existing[$cultureName] = $value }
        }
    }
    if ($translations.Count -gt 0)
    {
        $entries.Add([ordered]@{ project = $source.Project; key = $source.Key; source = $source.Source; existing = $existing; translations = $translations })
    }
}
if ($Command -eq 'check')
{
    foreach ($problem in $problems) { Write-Output $problem }
    if ($problems.Count -gt 0) { throw "Scoped check failed: $($problems.Count) of $checked translations need attention." }
    Write-Output "Scoped check passed: $checked translations, $($sources.Count) keys, $($selectedCultures.Count) cultures."
    return
}
if (-not $Path) { throw 'export requires -Path.' }
$selectedEntries = @($entries | Select-Object -First $MaxEntries)
if ($MaxCandidates -gt 0 -and $selectedEntries.Count -gt 0)
{
    $sourceIndex = [Collections.Generic.Dictionary[string, Collections.Generic.List[object]]]::new([StringComparer]::Ordinal)
    foreach ($projectName in $projects.Keys | Sort-Object)
    {
        $catalog = Get-Catalog $projectName
        foreach ($keyName in $catalog.Values.Keys | Sort-Object -CaseSensitive)
        {
            $value = $catalog.Values[$keyName]
            if (-not $sourceIndex.ContainsKey($value)) { $sourceIndex.Add($value, [Collections.Generic.List[object]]::new()) }
            $sourceIndex[$value].Add([pscustomobject]@{ Project = $projectName; Key = $keyName })
        }
    }
    foreach ($entry in $selectedEntries)
    {
        $candidates = [Collections.Generic.List[object]]::new()
        foreach ($candidate in $sourceIndex[$entry['source']])
        {
            if ($candidate.Project -ceq $entry['project'] -and $candidate.Key -ceq $entry['key']) { continue }
            $values = [ordered]@{}
            foreach ($cultureName in $entry['translations'].Keys)
            {
                $value = Get-Value (Get-Catalog $candidate.Project $cultureName) $candidate.Key
                if (-not (Get-TranslationProblem $entry['source'] $value $candidate.Project $candidate.Key $cultureName $approvals))
                { $values[$cultureName] = $value }
            }
            if ($values.Count -gt 0) { $candidates.Add([ordered]@{ project = $candidate.Project; key = $candidate.Key; translations = $values }) }
            if ($candidates.Count -ge $MaxCandidates) { break }
        }
        if ($candidates.Count -gt 0) { $entry['reuseCandidates'] = @($candidates.ToArray()) }
    }
}
foreach ($entry in $selectedEntries)
{
    $node = (Get-Catalog $entry['project']).Nodes[$entry['key']].SelectSingleNode('comment')
    if ($null -ne $node) { $entry['comment'] = $node.InnerText }
}
$batch = [ordered]@{ version = 1; entries = $selectedEntries }
Write-JsonFile $batch $Path
$pending = ($selectedEntries | ForEach-Object { $_['translations'].Count } | Measure-Object -Sum).Sum
Write-Output "Exported $($selectedEntries.Count) of $($entries.Count) keys ($pending translations) to $Path."
