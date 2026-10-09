#requires -Version 7.2
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
Import-Module (Join-Path $PSScriptRoot 'TranslationResources.psm1') -Force
$helper = Join-Path $PSScriptRoot 'Resx.ps1'
$repositoryDirectory = Split-Path -Parent $PSScriptRoot
$testRoot = Join-Path $repositoryDirectory ('artifacts/resx-tests-' + [Guid]::NewGuid().ToString('N'))
$null = [IO.Directory]::CreateDirectory($testRoot)
$encoding = [Text.UTF8Encoding]::new($false)
$checks = 0

function Assert([bool]$condition, [string]$message)
{
    if (-not $condition) { throw "Assertion failed: $message" }
    $script:checks++
}
function Write-Json($batch)
{
    [IO.File]::WriteAllText($batchPath, (ConvertTo-Json -InputObject $batch -Depth 20), $encoding)
}
function Get-Snapshot
{
    $snapshot = @{}
    foreach ($file in Get-ChildItem -LiteralPath $testRoot -Filter '*.resx' -Recurse)
    { $snapshot[$file.FullName] = [Convert]::ToBase64String([IO.File]::ReadAllBytes($file.FullName)) }
    return $snapshot
}
function Assert-Unchanged($snapshot)
{
    foreach ($path in $snapshot.Keys)
    { Assert ($snapshot[$path] -ceq [Convert]::ToBase64String([IO.File]::ReadAllBytes($path))) "Unexpected write to $path" }
}
function Assert-Rejected($batch, [string]$messagePattern)
{
    Write-Json $batch
    $snapshot = Get-Snapshot
    $caught = $false
    try { & $helper import -RepositoryRoot $testRoot -Path $batchPath | Out-Null }
    catch { $caught = $true; Assert ($_.Exception.Message -match $messagePattern) "Unexpected failure: $($_.Exception.Message)" }
    Assert $caught 'Invalid import should fail'
    Assert-Unchanged $snapshot
}

$sourceXml = @'
<?xml version="1.0" encoding="utf-8"?>
<root>
  <!-- Preserve this XML comment and metadata. -->
  <resheader name="resmimetype"><value>text/microsoft-resx</value></resheader>
  <metadata name="Fixture"><value>unchanged</value></metadata>
  <data name="Widget_Message" xml:space="preserve"><value>Save {0}
Keep $filename$ &amp; text</value><comment>A multiline filename message.</comment></data>
  <data name="Widget_Pause" xml:space="preserve"><value>Pause</value></data>
  <data name="Widget_Ready" xml:space="preserve"><value>Ready</value></data>
  <data name="TypedAsset" type="System.String"><value>leave this typed resource alone</value></data>
</root>
'@
$reuseXml = '<root><data name="Reusable_Pause" xml:space="preserve"><value>Pause</value></data></root>'
foreach ($projectName in @('ShareX', 'ShareX.Tools'))
{
    $folder = Join-Path $testRoot "$projectName/Localization"
    $null = [IO.Directory]::CreateDirectory($folder)
    [IO.File]::WriteAllText((Join-Path $folder 'Strings.resx'), $(if ($projectName -eq 'ShareX.Tools') { $sourceXml } else { $reuseXml }), $encoding)
    foreach ($cultureName in @('fr', 'tr'))
    {
        $pause = if ($cultureName -eq 'fr') { 'Mettre en pause' } else { 'Duraklat' }
        $ready = if ($cultureName -eq 'fr') { 'Prêt' } else { 'Hazır' }
        $localized = if ($projectName -eq 'ShareX.Tools')
        { "<root><!-- retain localized comment --><metadata name=`"Fixture`"><value>unchanged</value></metadata><data name=`"Widget_Ready`" xml:space=`"preserve`"><value>$ready</value></data><data name=`"TypedAsset`" type=`"System.String`"><value>leave this typed resource alone</value></data></root>" }
        else { "<root><data name=`"Reusable_Pause`" xml:space=`"preserve`"><value>$pause</value></data></root>" }
        [IO.File]::WriteAllText((Join-Path $folder "Strings.$cultureName.resx"), $localized, $encoding)
    }
}
$batchPath = Join-Path $testRoot 'batch.json'
& $helper export -RepositoryRoot $testRoot -Project ShareX.Tools -Prefix Widget_ -Path $batchPath | Out-Null
$batch = Read-TranslationBatch $batchPath
Assert ($batch['entries'].Count -eq 2) 'Export should select only the missing keys'
$message = $batch['entries'][0]; $pauseEntry = $batch['entries'][1]
Assert ($message['key'] -ceq 'Widget_Message') 'Keys should be sorted'
Assert ($message['comment'] -ceq 'A multiline filename message.') 'Export should retain source context'
Assert ($message['existing'].Count -eq 0) 'Missing values should have an empty snapshot'
Assert ($pauseEntry['reuseCandidates'][0]['key'] -ceq 'Reusable_Pause') 'Exact-source reuse should be discovered'
$findPath = Join-Path $testRoot 'find.json'
& $helper find -RepositoryRoot $testRoot -Text 'Pause' -Culture tr -MaxEntries 1 -Path $findPath | Out-Null
$found = (Read-TranslationBatch $findPath)['matches']
Assert ($found.Count -eq 1 -and $found[0]['source'] -ceq 'Pause') 'Lookup should return a compact exact match'
Assert ($found[0]['translations']['tr'] -ceq 'Duraklat') 'Lookup should retrieve the selected culture'
$message['translations']['fr'] = "Enregistrer {0}`nConserver `$filename`$ & l’image <originale>"
$message['translations']['tr'] = "Kaydet {0}`n`$filename`$ & özgün <görüntü> korunsun"
$pauseEntry['reuseFrom'] = @{ project = 'ShareX'; key = 'Reusable_Pause' }
Write-Json $batch
$before = Get-Snapshot
& $helper import -RepositoryRoot $testRoot -Path $batchPath -WhatIf | Out-Null
Assert-Unchanged $before
# Import one completed locale first; the original batch can still resume safely.
$partial = ConvertFrom-Json -InputObject ([IO.File]::ReadAllText($batchPath)) -AsHashtable
$partial['entries'][0]['translations']['tr'] = $null
$partial['entries'][1].Remove('reuseFrom') | Out-Null
Write-Json $partial
& $helper import -RepositoryRoot $testRoot -Path $batchPath | Out-Null
$pendingPath = Join-Path $testRoot 'pending.json'
& $helper export -RepositoryRoot $testRoot -Project ShareX.Tools -Prefix Widget_ -Path $pendingPath | Out-Null
$pending = (Read-TranslationBatch $pendingPath)['entries']
Assert ($pending[0]['translations'].Count -eq 1 -and $pending[0]['translations'].Contains('tr')) 'Export should omit an already completed locale'
Assert ($pending[1]['translations'].Count -eq 2) 'Unfilled entries should remain pending'
Write-Json $batch
& $helper import -RepositoryRoot $testRoot -Path $batchPath | Out-Null
foreach ($cultureName in @('fr', 'tr'))
{
    $catalogPath = Join-Path $testRoot "ShareX.Tools/Localization/Strings.$cultureName.resx"
    $catalog = Read-TranslationResource $catalogPath
    Assert ($catalog.Values['Widget_Message'] -ceq $message['translations'][$cultureName]) 'Unicode, XML characters, placeholders, and newlines must round-trip'
    Assert ($catalog.Values['Widget_Pause'] -ceq $pauseEntry['reuseCandidates'][0]['translations'][$cultureName]) 'Selected translations should be reused'
    Assert ($catalog.Nodes['TypedAsset'].GetAttribute('type') -ceq 'System.String') 'Typed resources must be preserved'
    Assert ($catalog.Document.SelectSingleNode('/root/metadata/value').InnerText -ceq 'unchanged') 'Metadata must be preserved'
    Assert ($catalog.Document.SelectSingleNode('/root/comment()').Value -match 'retain localized comment') 'XML comments must be preserved'
    $bytes = [IO.File]::ReadAllBytes($catalogPath)
    $text = $encoding.GetString($bytes)
    Assert (-not ($bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF)) 'Catalogs must not have a BOM'
    Assert ($text.EndsWith("`r`n") -and -not $text.Replace("`r`n", '').Contains("`n")) 'Catalogs must use CRLF'
}
foreach ($path in $before.Keys | Where-Object { $_ -notmatch 'ShareX.Tools[\\/]Localization[\\/]Strings\.(fr|tr)\.resx$' })
{ Assert ($before[$path] -ceq [Convert]::ToBase64String([IO.File]::ReadAllBytes($path))) 'Unselected catalogs must be unchanged' }
$after = Get-Snapshot
& $helper import -RepositoryRoot $testRoot -Path $batchPath | Out-Null
Assert-Unchanged $after
& $helper check -RepositoryRoot $testRoot -Project ShareX.Tools -Prefix Widget_ | Out-Null
& $helper check -RepositoryRoot $testRoot -Project ShareX.Tools -Prefix Widget_ -Culture 'fr,tr' | Out-Null
& $helper export -RepositoryRoot $testRoot -Project ShareX.Tools -Prefix Widget_ -Path $batchPath | Out-Null
Assert ((Read-TranslationBatch $batchPath)['entries'].Count -eq 0) 'A completed scope should export an empty batch'

& $helper export -RepositoryRoot $testRoot -Project ShareX.Tools -Key Widget_Message -IncludeExisting -Path $batchPath | Out-Null
$originalBatch = [IO.File]::ReadAllText($batchPath)
function Fresh-Batch { ConvertFrom-Json -InputObject $originalBatch -AsHashtable }
$invalid = Fresh-Batch
$invalid['entries'][0]['translations']['fr'] = 'Enregistrer {0} $filename$ correctement'
$invalid['entries'][0]['translations']['tr'] = 'Eksik yer tutucu'
Assert-Rejected $invalid 'Placeholder mismatch'
$invalid = Fresh-Batch
$invalid['entries'][0]['source'] = 'Changed source'
Assert-Rejected $invalid 'Source changed'
$invalid = Fresh-Batch
$invalid['entries'][0]['translations']['tr'] = $invalid['entries'][0]['source']
Assert-Rejected $invalid 'English-equivalent'
$invalid = Fresh-Batch
$invalid['entries'][0]['translations']['zz'] = 'Invalid culture'
Assert-Rejected $invalid 'Unknown batch culture'
$invalid = Fresh-Batch
$invalid['entries'][0]['project'] = '../ShareX.Tools'
Assert-Rejected $invalid 'Unknown batch project'
$invalid = Fresh-Batch
$invalid['entries'][0]['translations']['tr'] = 123
Assert-Rejected $invalid 'must be a string'
$invalid = Fresh-Batch
$invalid['entries'] += $invalid['entries'][0]
Assert-Rejected $invalid 'duplicate batch key'
$invalid = Fresh-Batch
$invalid['entries'][0]['reuseFrom'] = @{ project = 'ShareX.Tools'; key = 'Widget_Ready' }
Assert-Rejected $invalid 'same English source'
$invalid = Fresh-Batch
$invalid['entries'][0]['existing']['tr'] = 'An older translation'
$invalid['entries'][0]['translations']['fr'] = 'Enregistrer {0} et $filename$ correctement'
$invalid['entries'][0]['translations']['tr'] = 'Doğru kaydet {0} ve $filename$'
Assert-Rejected $invalid 'Translation changed'
[IO.File]::WriteAllText($batchPath, '{"version":1,"version":1,"entries":[]}', $encoding)
$caught = $false
try { & $helper import -RepositoryRoot $testRoot -Path $batchPath | Out-Null }
catch { $caught = $true; Assert ($_.Exception.Message -match 'Duplicate JSON member') 'Duplicate JSON members should be diagnosed' }
Assert $caught 'Duplicate JSON members must fail'

$invalid = Fresh-Batch
$invalid['entries'][0]['key'] = 'widget_message'
Assert-Rejected $invalid 'Unknown source key'
$invalid = Fresh-Batch
$invalid['entries'][0]['translations']['tr'] = 'Kaydet {0} ve $filename$'
$typedPath = Join-Path $testRoot 'ShareX.Tools/Localization/Strings.tr.resx'
$typedCatalog = Read-TranslationResource $typedPath
$typedCatalog.Nodes['Widget_Message'].SetAttribute('type', 'System.String')
[IO.File]::WriteAllBytes($typedPath, (Format-TranslationResource $typedCatalog.Document))
Assert-Rejected $invalid 'not a string'

# Reviewed technical names remain valid in cultures that normally require another script.
$approvals = Get-TranslationApprovals $repositoryDirectory
$codecKey = 'FFmpegOptionsWindow_Codec_H264_Intel_Quick_Sync'
$codecName = 'H.264 Intel Quick Sync'
Assert (-not (Get-TranslationProblem $codecName $codecName 'ShareX.ScreenCaptureLib' $codecKey 'ja-JP' $approvals)) 'Approved technical names should not require localized script'
Assert ((Get-TranslationProblem $codecName $codecName 'ShareX.ScreenCaptureLib' 'Unapproved_Codec' 'ja-JP' $approvals) -match 'English-equivalent') 'Unapproved technical names must still fail'
Assert ((Get-TranslationProblem $codecName 'Other English phrase' 'ShareX.ScreenCaptureLib' $codecKey 'ja-JP' $approvals) -match 'localized script') 'Approval must not exempt a different English-only value'

# The shared serializer should produce exactly the established repository format.
$realPath = Join-Path $repositoryDirectory 'ShareX.Tools/Localization/Strings.tr.resx'
$formatted = Format-TranslationResource (Read-TranslationResource $realPath).Document
Assert ([Convert]::ToBase64String($formatted) -ceq [Convert]::ToBase64String([IO.File]::ReadAllBytes($realPath))) 'Formatting must match the existing formatter'
Write-Output "RESX helper tests passed: $checks checks. Fixtures: $testRoot"
