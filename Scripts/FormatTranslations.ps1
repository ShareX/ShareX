[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'

$repositoryDirectory = Split-Path -Parent $PSScriptRoot
$utf8WithoutBom = [Text.UTF8Encoding]::new($false)
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

function Format-TranslationResource([IO.FileInfo]$file)
{
    $readerSettings = [Xml.XmlReaderSettings]::new()
    $readerSettings.DtdProcessing = [Xml.DtdProcessing]::Prohibit
    $readerSettings.IgnoreWhitespace = $true
    $readerSettings.XmlResolver = $null

    $document = [Xml.XmlDocument]::new()
    $document.PreserveWhitespace = $false
    $document.XmlResolver = $null

    $reader = [Xml.XmlReader]::Create($file.FullName, $readerSettings)
    try
    {
        $document.Load($reader)
    }
    finally
    {
        $reader.Dispose()
    }

    $root = $document.DocumentElement
    if ($null -eq $root -or $root.LocalName -cne 'root')
    {
        throw "Translation resource does not have a RESX root element: $($file.FullName)"
    }

    $dataElements = [Collections.Generic.List[Xml.XmlElement]]::new()
    $names = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    foreach ($node in $root.ChildNodes)
    {
        if ($node.NodeType -eq [Xml.XmlNodeType]::Element -and $node.LocalName -ceq 'data' -and $node.NamespaceURI -ceq '')
        {
            $element = [Xml.XmlElement]$node
            $name = $element.GetAttribute('name')
            if (-not $names.Add($name))
            {
                throw "Translation resource contains duplicate name '$name': $($file.FullName)"
            }
            $dataElements.Add($element)
        }
    }

    if ($dataElements.Count -gt 0)
    {
        $insertBefore = $dataElements[$dataElements.Count - 1].NextSibling
        foreach ($element in $dataElements)
        {
            $null = $root.RemoveChild($element)
        }

        $dataElements.Sort([Comparison[Xml.XmlElement]]{
            param($left, $right)
            return [StringComparer]::Ordinal.Compare($left.GetAttribute('name'), $right.GetAttribute('name'))
        })

        foreach ($element in $dataElements)
        {
            if ($null -eq $insertBefore)
            {
                $null = $root.AppendChild($element)
            }
            else
            {
                $null = $root.InsertBefore($element, $insertBefore)
            }
        }
    }

    $writerSettings = [Xml.XmlWriterSettings]::new()
    $writerSettings.Encoding = $utf8WithoutBom
    $writerSettings.Indent = $true
    $writerSettings.IndentChars = '  '
    $writerSettings.NewLineChars = "`r`n"
    $writerSettings.NewLineHandling = [Xml.NewLineHandling]::Replace
    $writerSettings.OmitXmlDeclaration = $false

    $stream = [IO.MemoryStream]::new()
    try
    {
        $writer = [Xml.XmlWriter]::Create($stream, $writerSettings)
        try
        {
            $document.Save($writer)
            $writer.WriteWhitespace("`r`n")
            $writer.Flush()
        }
        finally
        {
            $writer.Dispose()
        }

        return $stream.ToArray()
    }
    finally
    {
        $stream.Dispose()
    }
}

$translationFiles = @(Get-TranslationResourceFiles)
foreach ($file in $translationFiles)
{
    $formattedFiles.Add([pscustomobject]@{
        File = $file
        Bytes = Format-TranslationResource $file
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
