[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'

$repositoryDirectory = Split-Path -Parent $PSScriptRoot
Import-Module (Join-Path $PSScriptRoot 'TranslationResources.psm1') -Force
$formattedFiles = [Collections.Generic.List[object]]::new()

function Get-TranslationResourceFiles
{
    return @(
        Get-ChildItem -LiteralPath $repositoryDirectory -Directory |
            ForEach-Object {
                $localizationDirectory = Join-Path $_.FullName 'Localization'
                if (Test-Path -LiteralPath (Join-Path $localizationDirectory 'Strings.resx') -PathType Leaf)
                {
                    Get-ChildItem -LiteralPath $localizationDirectory -File -Filter 'Strings*.resx'
                }
            } |
            Sort-Object FullName
    )
}

$translationFiles = @(Get-TranslationResourceFiles)
foreach ($file in $translationFiles)
{
    $formattedFiles.Add([pscustomobject]@{
        File = $file
        Bytes = Format-TranslationResource (Read-TranslationResource $file.FullName).Document
    })
}

$updatedCount = 0
foreach ($formattedFile in $formattedFiles)
{
    $existingBytes = [IO.File]::ReadAllBytes($formattedFile.File.FullName)
    if (-not [Collections.StructuralComparisons]::StructuralEqualityComparer.Equals($existingBytes, $formattedFile.Bytes))
    {
        [IO.File]::WriteAllBytes($formattedFile.File.FullName, $formattedFile.Bytes)
        $updatedCount++
    }
}

Write-Host "Formatted $($translationFiles.Count) translation resource files ($updatedCount updated)."
