#requires -Version 5.1

[CmdletBinding(SupportsShouldProcess = $true)]
param(
    [string]$RepositoryDirectory,
    [string]$LicenseNoticePath,
    [string[]]$IgnoreFolders = @('bin', 'obj', 'Properties', 'packages'),
    [string[]]$AllowFileExtensions = @('.cs'),
    [string[]]$IgnoreFileExtensions = @('.designer.cs'),
    [switch]$Check
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Get-SourceFiles
{
    param(
        [Parameter(Mandatory = $true)]
        [string]$Directory
    )

    foreach ($file in Get-ChildItem -LiteralPath $Directory -File)
    {
        $extensionAllowed = $false

        foreach ($extension in $AllowFileExtensions)
        {
            if ($file.Name.EndsWith($extension, [StringComparison]::OrdinalIgnoreCase))
            {
                $extensionAllowed = $true
                break
            }
        }

        if (-not $extensionAllowed)
        {
            continue
        }

        $extensionIgnored = $false

        foreach ($extension in $IgnoreFileExtensions)
        {
            if ($file.Name.EndsWith($extension, [StringComparison]::OrdinalIgnoreCase))
            {
                $extensionIgnored = $true
                break
            }
        }

        if (-not $extensionIgnored)
        {
            $file
        }
    }

    foreach ($subdirectory in Get-ChildItem -LiteralPath $Directory -Directory)
    {
        if ($ignoredFolderNames.Contains($subdirectory.Name))
        {
            continue
        }

        if (($subdirectory.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0)
        {
            continue
        }

        Get-SourceFiles -Directory $subdirectory.FullName
    }
}

function Read-SourceFile
{
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    $bytes = [IO.File]::ReadAllBytes($Path)
    $preambleLength = 0

    if ($bytes.Length -ge 4 -and
        $bytes[0] -eq 0x00 -and $bytes[1] -eq 0x00 -and
        $bytes[2] -eq 0xFE -and $bytes[3] -eq 0xFF)
    {
        $encoding = [Text.UTF32Encoding]::new($true, $true, $true)
        $preambleLength = 4
    }
    elseif ($bytes.Length -ge 4 -and
        $bytes[0] -eq 0xFF -and $bytes[1] -eq 0xFE -and
        $bytes[2] -eq 0x00 -and $bytes[3] -eq 0x00)
    {
        $encoding = [Text.UTF32Encoding]::new($false, $true, $true)
        $preambleLength = 4
    }
    elseif ($bytes.Length -ge 3 -and
        $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF)
    {
        $encoding = [Text.UTF8Encoding]::new($true, $true)
        $preambleLength = 3
    }
    elseif ($bytes.Length -ge 2 -and $bytes[0] -eq 0xFF -and $bytes[1] -eq 0xFE)
    {
        $encoding = [Text.UnicodeEncoding]::new($false, $true, $true)
        $preambleLength = 2
    }
    elseif ($bytes.Length -ge 2 -and $bytes[0] -eq 0xFE -and $bytes[1] -eq 0xFF)
    {
        $encoding = [Text.UnicodeEncoding]::new($true, $true, $true)
        $preambleLength = 2
    }
    else
    {
        $encoding = [Text.UTF8Encoding]::new($false, $true)
    }

    [pscustomobject]@{
        Content = $encoding.GetString($bytes, $preambleLength, $bytes.Length - $preambleLength)
        Encoding = $encoding
        Preamble = $encoding.GetPreamble()
    }
}

function Write-SourceFile
{
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path,
        [Parameter(Mandatory = $true)]
        [string]$Content,
        [Parameter(Mandatory = $true)]
        [Text.Encoding]$Encoding,
        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [byte[]]$Preamble
    )

    $contentBytes = $Encoding.GetBytes($Content)
    $outputBytes = [byte[]]::new($Preamble.Length + $contentBytes.Length)

    if ($Preamble.Length -gt 0)
    {
        [Array]::Copy($Preamble, 0, $outputBytes, 0, $Preamble.Length)
    }

    [Array]::Copy($contentBytes, 0, $outputBytes, $Preamble.Length, $contentBytes.Length)
    [IO.File]::WriteAllBytes($Path, $outputBytes)
}

$pathSeparators = [char[]]@(
    [IO.Path]::DirectorySeparatorChar,
    [IO.Path]::AltDirectorySeparatorChar)
$newlineCharacters = [char[]]@([char]13, [char]10)
$crlf = [string][char]13 + [char]10
$lf = [string][char]10
$cr = [string][char]13

if ([string]::IsNullOrWhiteSpace($RepositoryDirectory))
{
    $RepositoryDirectory = Split-Path -Parent $PSScriptRoot
}

if ([string]::IsNullOrWhiteSpace($LicenseNoticePath))
{
    $LicenseNoticePath = Join-Path $PSScriptRoot 'LicenseNotice.txt'
}

$repositoryPath = (Resolve-Path -LiteralPath $RepositoryDirectory).ProviderPath.TrimEnd(
    $pathSeparators)
$noticePath = (Resolve-Path -LiteralPath $LicenseNoticePath).ProviderPath
$licenseNotice = [IO.File]::ReadAllText($noticePath).TrimEnd($newlineCharacters)
$currentYear = [DateTime]::Now.Year.ToString([Globalization.CultureInfo]::InvariantCulture)
$licenseNotice = $licenseNotice.Replace('{year}', $currentYear)

if ([string]::IsNullOrWhiteSpace($licenseNotice))
{
    throw "License notice is empty: $noticePath"
}

$ignoredFolderNames = [Collections.Generic.HashSet[string]]::new(
    [StringComparer]::OrdinalIgnoreCase)

foreach ($folder in $IgnoreFolders)
{
    [void]$ignoredFolderNames.Add($folder)
}

$licenseRegionPattern = [regex]::new(
    '\A#region[\s\S]*?^#endregion[^\r\n]*(?:\r\n|\n|\r)*',
    [Text.RegularExpressions.RegexOptions]::Multiline)

$scannedCount = 0
$pendingCount = 0
$updatedCount = 0

foreach ($file in Get-SourceFiles -Directory $repositoryPath)
{
    $scannedCount++
    $source = Read-SourceFile -Path $file.FullName

    if ($source.Content.Contains($crlf))
    {
        $lineEnding = $crlf
    }
    elseif ($source.Content.Contains($lf))
    {
        $lineEnding = $lf
    }
    elseif ($source.Content.Contains($cr))
    {
        $lineEnding = $cr
    }
    else
    {
        $lineEnding = [Environment]::NewLine
    }

    $normalizedNotice = [regex]::Replace(
        $licenseNotice,
        '\r\n|\n|\r',
        $lineEnding)
    $existingRegion = $licenseRegionPattern.Match($source.Content)

    if ($existingRegion.Success)
    {
        $remainingContent = $source.Content.Substring($existingRegion.Length)
        $updatedContent = $normalizedNotice + $lineEnding + $lineEnding + $remainingContent
        $changeType = 'Updated'
    }
    else
    {
        $updatedContent = $normalizedNotice + $lineEnding + $lineEnding + $source.Content
        $changeType = 'Added'
    }

    if ($updatedContent -ceq $source.Content)
    {
        continue
    }

    $pendingCount++
    $relativePath = $file.FullName.Substring($repositoryPath.Length).TrimStart(
        $pathSeparators)

    if ($Check)
    {
        Write-Output "Needs update: $relativePath"
        continue
    }

    if ($PSCmdlet.ShouldProcess($relativePath, "$changeType license notice"))
    {
        Write-SourceFile -Path $file.FullName -Content $updatedContent -Encoding $source.Encoding -Preamble $source.Preamble
        $updatedCount++
        Write-Output ('{0}: {1}' -f $changeType, $relativePath)
    }
}

if ($Check)
{
    if ($pendingCount -gt 0)
    {
        Write-Error "$pendingCount of $scannedCount code files need a license notice update."
        exit 1
    }

    Write-Output "All $scannedCount code files have the current license notice."
    exit 0
}

Write-Output "Scanned $scannedCount code files; updated $updatedCount."
