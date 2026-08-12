[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'

$repositoryDirectory = $PSScriptRoot
$strictUtf8 = [Text.UTF8Encoding]::new($false, $true)
$legacyEncodings = @(
    [Text.Encoding]::GetEncoding(28591, [Text.EncoderExceptionFallback]::new(), [Text.DecoderExceptionFallback]::new())
    [Text.Encoding]::GetEncoding(1252, [Text.EncoderExceptionFallback]::new(), [Text.DecoderExceptionFallback]::new())
)
$errors = [Collections.Generic.List[string]]::new()

# LanguageHelper uses specific cultures, while neutral satellite catalogs are used where
# .NET resource fallback supports them. English is supplied by the default Strings.resx.
$languageCatalogMap = [ordered]@{
    'ar-YE' = 'ar-YE'
    'cs-CZ' = 'cs'
    'da-DK' = 'da'
    'de-DE' = 'de'
    'es-ES' = 'es'
    'es-MX' = 'es-MX'
    'fa-IR' = 'fa-IR'
    'fr-FR' = 'fr'
    'he-IL' = 'he-IL'
    'hi-IN' = 'hi'
    'hu-HU' = 'hu'
    'id-ID' = 'id-ID'
    'it-IT' = 'it-IT'
    'ja-JP' = 'ja-JP'
    'ko-KR' = 'ko-KR'
    'nl-NL' = 'nl-NL'
    'pl-PL' = 'pl'
    'pt-BR' = 'pt-BR'
    'pt-PT' = 'pt-PT'
    'ro-RO' = 'ro'
    'ru-RU' = 'ru'
    'sv-SE' = 'sv'
    'th-TH' = 'th'
    'tr-TR' = 'tr'
    'uk-UA' = 'uk'
    'vi-VN' = 'vi-VN'
    'zh-CN' = 'zh-CN'
    'zh-TW' = 'zh-TW'
}
$supportedCatalogCultures = @($languageCatalogMap.Values | Sort-Object -Unique)

$projects = @(
    [pscustomobject]@{
        Name = 'ShareX'
        ResourceBaseName = 'ShareX.Localization.Strings'
        DynamicPrefixes = @()
    }
    [pscustomobject]@{
        Name = 'ShareX.Avalonia'
        ResourceBaseName = 'ShareX.AvaloniaUI.Localization.Strings'
        DynamicPrefixes = @()
    }
    [pscustomobject]@{
        Name = 'ShareX.HelpersLib'
        ResourceBaseName = 'ShareX.HelpersLib.Localization.Strings'
        DynamicPrefixes = @(
            'AfterCaptureTasks_'
            'AfterUploadTasks_'
            'ArrowHeadDirection_'
            'BorderStyle_'
            'CustomUploaderDestinationType_'
            'CutOutEffectType_'
            'DrawImageSizeMode_'
            'EDataType_'
            'FileDestination_'
            'FileExistAction_'
            'GIFQuality_'
            'HotkeyType_'
            'ImageDestination_'
            'ImageEditorStartMode_'
            'ImgurThumbnailType_'
            'PastebinExpiration_'
            'PastebinPrivacy_'
            'PNGBitDepth_'
            'PrivateBinExpiration_'
            'PrivateBinFormat_'
            'ProxyMethod_'
            'RegionCaptureAction_'
            'ScreenRecordGIFEncoding_'
            'ScrollMethod_'
            'ShapeType_'
            'StepType_'
            'SupportedLanguage_'
            'TextDestination_'
            'ThumbnailTitleLocation_'
            'ThumbnailViewClickAction_'
            'ToastClickAction_'
            'UpdateChannel_'
            'URLSharingServices_'
            'UrlShortenerType_'
            'YouTubeVideoPrivacy_'
        )
    }
    [pscustomobject]@{
        Name = 'ShareX.HistoryLib'
        ResourceBaseName = 'ShareX.HistoryLib.Localization.Strings'
        DynamicPrefixes = @()
    }
    [pscustomobject]@{
        Name = 'ShareX.ImageEditor'
        ResourceBaseName = 'ShareX.ImageEditor.Localization.Strings'
        DynamicPrefixes = @(
            'EffectBrowserPanel_Category_'
            'EffectBrowserPanel_Effect_'
        )
    }
    [pscustomobject]@{
        Name = 'ShareX.ImageEffectsLib'
        ResourceBaseName = 'ShareX.ImageEffectsLib.Localization.Strings'
        DynamicPrefixes = @(
            'ImageEffect_'
            'ImageEffectProperty_'
            'ImageEffectPropertyDescription_'
            'ImageEffectEnum_'
        )
    }
    [pscustomobject]@{
        Name = 'ShareX.ScreenCaptureLib'
        ResourceBaseName = 'ShareX.ScreenCaptureLib.Localization.Strings'
        DynamicPrefixes = @()
    }
    [pscustomobject]@{
        Name = 'ShareX.Tools'
        ResourceBaseName = 'ShareX.Tools.Localization.Strings'
        DynamicPrefixes = @()
    }
    [pscustomobject]@{
        Name = 'ShareX.UploadersLib'
        ResourceBaseName = 'ShareX.UploadersLib.Localization.Strings'
        DynamicPrefixes = @(
            'AccountType_'
            'AmazonS3StorageClass_'
            'BoxShareAccessLevel_'
            'BrowserProtocol_'
            'CustomUploaderBody_'
            'CustomUploaderDestinationType_'
            'DestinationSettings_Field_'
            'FileDestination_'
            'FTPProtocol_'
            'ImageDestination_'
            'ImgurThumbnailType_'
            'LinkFormatEnum_'
            'PastebinExpiration_'
            'PastebinPrivacy_'
            'PrivateBinExpiration_'
            'PrivateBinFormat_'
            'TextDestination_'
            'URLSharingServices_'
            'UrlShortenerType_'
            'YouTubeVideoPrivacy_'
        )
    }
)

function Add-ValidationError([string]$message)
{
    $errors.Add($message)
}

function Read-ResourceCatalog([string]$path)
{
    $bytes = [IO.File]::ReadAllBytes($path)
    if ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF)
    {
        Add-ValidationError "Resource file has a UTF-8 BOM: $path"
    }

    $text = $strictUtf8.GetString($bytes)
    if (-not $text.EndsWith("`r`n", [StringComparison]::Ordinal))
    {
        Add-ValidationError "Resource file must use CRLF and end with a newline: $path"
    }
    if ($text.Replace("`r`n", '').Contains("`n"))
    {
        Add-ValidationError "Resource file contains bare LF newlines: $path"
    }
    if ($text -match '[\u0080-\u009F\uFFFD]')
    {
        Add-ValidationError "Resource file contains an invalid control or replacement character: $path"
    }
    if ($text -match '&apos\s+;|&\s+apos;')
    {
        Add-ValidationError "Resource file contains a malformed apostrophe entity: $path"
    }

    [xml]$document = $text
    $values = [ordered]@{}
    foreach ($item in $document.root.data)
    {
        $key = [string]$item.name
        $value = [string]$item.value
        if ($values.Contains($key))
        {
            Add-ValidationError "Duplicate resource key '$key' in $path"
            continue
        }

        foreach ($encoding in $legacyEncodings)
        {
            try
            {
                $decoded = $strictUtf8.GetString($encoding.GetBytes($value))
                if ($decoded -ne $value)
                {
                    Add-ValidationError "Resource value appears to contain reversible mojibake: $path ($key)"
                    break
                }
            }
            catch [Text.EncoderFallbackException]
            {
            }
            catch [Text.DecoderFallbackException]
            {
            }
        }

        $values[$key] = $value
    }

    return [pscustomobject]@{
        Keys = @($values.Keys)
        Values = $values
    }
}

function Get-FormatPlaceholders([string]$value)
{
    return @(
        [regex]::Matches($value, '(?<!\{)\{(?:\d+(?:[^}]*)|[A-Za-z_][A-Za-z0-9_]*)\}(?!\})') |
            ForEach-Object Value |
            Sort-Object -Unique
    )
}

function Get-CommandPlaceholders([string]$value)
{
    return @(
        [regex]::Matches($value, '\$[a-z_]+\$') |
            ForEach-Object Value |
            Sort-Object -Unique
    )
}

function Test-SequenceEqual([object[]]$left, [object[]]$right)
{
    if ($left.Count -ne $right.Count)
    {
        return $false
    }

    for ($index = 0; $index -lt $left.Count; $index++)
    {
        if ($left[$index] -cne $right[$index])
        {
            return $false
        }
    }

    return $true
}

function Test-UsesAnyPrefix([string]$key, [string[]]$prefixes)
{
    foreach ($prefix in $prefixes)
    {
        if ($key.StartsWith($prefix, [StringComparison]::Ordinal))
        {
            return $true
        }
    }

    return $false
}

function Test-AllowedAxamlLiteral([string]$value)
{
    if ($value.StartsWith('{', [StringComparison]::Ordinal) -or $value -notmatch '[A-Za-z]')
    {
        return $true
    }
    if ($value -match '^(Auto|Center|CenterOwner|CenterScreen|Disabled|False|Goldenrod|Left|Lime|None|Normal|Right|SemiBold|SizeAll|Stretch|Transparent|True|Uniform)$')
    {
        return $true
    }
    if ($value -match '^[A-Za-z]+(?:,[A-Za-z]+)+$' -or $value -match '^https?://example\.com/' -or $value -eq 'ShareX' -or $value -match '^[A-Z]$')
    {
        return $true
    }

    return $false
}

$languageHelperPath = Join-Path $repositoryDirectory 'ShareX\LanguageHelper.cs'
$languageHelperText = [IO.File]::ReadAllText($languageHelperPath)
$actualApplicationCultures = @(
    [regex]::Matches($languageHelperText, 'cultureName\s*=\s*"([^"]+)"') |
        ForEach-Object { $_.Groups[1].Value } |
        Sort-Object -Unique
)
$expectedApplicationCultures = @(@('en-US') + @($languageCatalogMap.Keys) | Sort-Object -Unique)
if (-not (Test-SequenceEqual $expectedApplicationCultures $actualApplicationCultures))
{
    Add-ValidationError "LanguageHelper supported cultures differ from ValidateTranslations.ps1. Expected: $($expectedApplicationCultures -join ', '); actual: $($actualApplicationCultures -join ', ')."
}

$discoveredProjects = @(
    Get-ChildItem -LiteralPath $repositoryDirectory -Directory |
        Where-Object { Test-Path -LiteralPath (Join-Path $_.FullName 'Localization\Strings.resx') } |
        ForEach-Object Name |
        Sort-Object
)
$configuredProjects = @($projects.Name | Sort-Object)
if (-not (Test-SequenceEqual $configuredProjects $discoveredProjects))
{
    Add-ValidationError "Localized project inventory differs. Configured: $($configuredProjects -join ', '); discovered: $($discoveredProjects -join ', ')."
}

$summary = [Collections.Generic.List[object]]::new()
$totalDefaultEntries = 0
$totalLocalizedEntries = 0

foreach ($project in $projects)
{
    $projectErrorCount = $errors.Count
    $projectDirectory = Join-Path $repositoryDirectory $project.Name
    $localizationDirectory = Join-Path $projectDirectory 'Localization'
    $defaultPath = Join-Path $localizationDirectory 'Strings.resx'
    $default = $null

    try
    {
        $default = Read-ResourceCatalog $defaultPath
    }
    catch
    {
        Add-ValidationError "$($project.Name): $($_.Exception.Message)"
    }

    $cultureFiles = @(Get-ChildItem -LiteralPath $localizationDirectory -Filter 'Strings.*.resx' | Sort-Object Name)
    $actualCultures = @($cultureFiles | ForEach-Object { $_.BaseName.Substring('Strings.'.Length) } | Sort-Object)
    if (-not (Test-SequenceEqual $supportedCatalogCultures $actualCultures))
    {
        Add-ValidationError "$($project.Name): localized catalog inventory differs. Expected: $($supportedCatalogCultures -join ', '); actual: $($actualCultures -join ', ')."
    }

    $localizedCounts = [Collections.Generic.List[int]]::new()
    foreach ($culture in $supportedCatalogCultures)
    {
        $culturePath = Join-Path $localizationDirectory "Strings.$culture.resx"
        if (-not (Test-Path -LiteralPath $culturePath -PathType Leaf))
        {
            Add-ValidationError "$($project.Name): missing Strings.$culture.resx."
            continue
        }

        try
        {
            $localized = Read-ResourceCatalog $culturePath
        }
        catch
        {
            Add-ValidationError "$($project.Name): $($_.Exception.Message)"
            continue
        }

        $localizedCounts.Add($localized.Keys.Count)
        $totalLocalizedEntries += $localized.Keys.Count

        if ($null -eq $default)
        {
            continue
        }
        if ($localized.Keys.Count -ne $default.Keys.Count)
        {
            Add-ValidationError "$($project.Name)/Strings.$culture.resx: entry count $($localized.Keys.Count) does not match the default count $($default.Keys.Count)."
        }
        foreach ($key in $default.Keys)
        {
            if (-not $localized.Values.Contains($key))
            {
                Add-ValidationError "$($project.Name)/Strings.$culture.resx: missing '$key'."
                continue
            }
            if ([string]::IsNullOrWhiteSpace($localized.Values[$key]))
            {
                Add-ValidationError "$($project.Name)/Strings.$culture.resx: '$key' is empty."
            }
            if (-not (Test-SequenceEqual (Get-FormatPlaceholders $default.Values[$key]) (Get-FormatPlaceholders $localized.Values[$key])))
            {
                Add-ValidationError "$($project.Name)/Strings.$culture.resx: '$key' has different format placeholders."
            }
            if (-not (Test-SequenceEqual (Get-CommandPlaceholders $default.Values[$key]) (Get-CommandPlaceholders $localized.Values[$key])))
            {
                Add-ValidationError "$($project.Name)/Strings.$culture.resx: '$key' has different command placeholders."
            }
        }
        foreach ($key in $localized.Keys)
        {
            if (-not $default.Values.Contains($key))
            {
                Add-ValidationError "$($project.Name)/Strings.$culture.resx: unexpected '$key'."
            }
        }
    }

    if ($null -ne $default)
    {
        $totalDefaultEntries += $default.Keys.Count
        foreach ($key in $default.Keys)
        {
            if ([string]::IsNullOrWhiteSpace($default.Values[$key]))
            {
                Add-ValidationError "$($project.Name)/Strings.resx: '$key' is empty."
            }
        }

        $designerPath = Join-Path $localizationDirectory 'Strings.Designer.cs'
        if (-not (Test-Path -LiteralPath $designerPath -PathType Leaf))
        {
            Add-ValidationError "$($project.Name): Strings.Designer.cs is missing."
        }
        else
        {
            $designerText = [IO.File]::ReadAllText($designerPath)
            $expectedResourceManager = 'ResourceManager("' + $project.ResourceBaseName + '"'
            if (-not $designerText.Contains($expectedResourceManager))
            {
                Add-ValidationError "$($project.Name): Strings.Designer.cs has an incorrect ResourceManager base name; expected '$($project.ResourceBaseName)'."
            }

            $designerKeys = @(
                [regex]::Matches($designerText, 'public static string ([A-Za-z_][A-Za-z0-9_]*)\b') |
                    ForEach-Object { $_.Groups[1].Value }
            )
            foreach ($key in $default.Keys)
            {
                if ($key -notin $designerKeys)
                {
                    Add-ValidationError "$($project.Name): Strings.Designer.cs is missing property '$key'."
                }
            }
        }

        $sourceFiles = @(
            Get-ChildItem -LiteralPath $projectDirectory -Recurse -File |
                Where-Object {
                    $_.Extension -in @('.axaml', '.cs') -and
                    $_.FullName -notmatch '[\\/](bin|obj|\.codex-build)[\\/]' -and
                    $_.Name -ne 'Strings.Designer.cs'
                }
        )
        $sourceText = ($sourceFiles | ForEach-Object { [IO.File]::ReadAllText($_.FullName) }) -join "`n"
        $referencedKeys = @(
            [regex]::Matches($sourceText, '(?:(?:Localization\.)?Strings\.|(?:localization|res):Strings\.)([A-Za-z_][A-Za-z0-9_]*)') |
                ForEach-Object { $_.Groups[1].Value } |
                Where-Object { $_ -notin @('Culture', 'ResourceManager') } |
                Sort-Object -Unique
        )

        foreach ($key in $referencedKeys)
        {
            if (-not $default.Values.Contains($key))
            {
                Add-ValidationError "$($project.Name): source references missing resource key '$key'."
            }
        }
        foreach ($key in $default.Keys)
        {
            if ($key -notin $referencedKeys -and -not (Test-UsesAnyPrefix $key $project.DynamicPrefixes))
            {
                Add-ValidationError "$($project.Name): resource key '$key' is not referenced."
            }
        }

        if ($sourceText -match 'using\s+[A-Za-z_][A-Za-z0-9_]*\s*=\s*(?:global::)?[A-Za-z0-9_.]+\.Localization\.Strings\s*;')
        {
            Add-ValidationError "$($project.Name): source contains a localization resource alias."
        }
        if ($sourceText -match '(?m)\b(?:string|object)\s+L\s*\(')
        {
            Add-ValidationError "$($project.Name): source contains an L() localization helper."
        }

        if ($project.Name -eq 'ShareX.ImageEditor')
        {
            foreach ($prefix in $project.DynamicPrefixes)
            {
                if (-not $sourceText.Contains($prefix))
                {
                    Add-ValidationError "$($project.Name): data-driven localization prefix '$prefix' is not used by source."
                }
            }
            if (-not $sourceText.Contains('Strings.ResourceManager.GetString(EffectPrefix + effectId'))
            {
                Add-ValidationError "$($project.Name): data-driven effect-browser localization lookup is missing."
            }

            $effectIds = @(
                Get-ChildItem -LiteralPath (Join-Path $projectDirectory 'Core\ImageEffects') -Recurse -Filter '*.cs' -File |
                    ForEach-Object {
                        $effectSource = [IO.File]::ReadAllText($_.FullName)
                        $idMatch = [regex]::Match($effectSource, 'public\s+override\s+string\s+Id\s*=>\s*"([^"]+)"')
                        $nameMatch = [regex]::Match($effectSource, 'public\s+override\s+string\s+Name\s*=>\s*"([^"]+)"')
                        if ($idMatch.Success -and $nameMatch.Success)
                        {
                            $idMatch.Groups[1].Value
                        }
                    } |
                    Sort-Object -Unique
            )
            $expectedEffectBrowserKeys = @(
                @(
                    'EffectBrowserPanel_Category_Manipulations'
                    'EffectBrowserPanel_Category_Adjustments'
                    'EffectBrowserPanel_Category_Filters'
                    'EffectBrowserPanel_Category_Drawings'
                ) + @($effectIds | ForEach-Object { "EffectBrowserPanel_Effect_$_" }) |
                    Sort-Object -Unique
            )
            $actualEffectBrowserKeys = @(
                $default.Keys |
                    Where-Object {
                        $_.StartsWith('EffectBrowserPanel_Category_', [StringComparison]::Ordinal) -or
                        $_.StartsWith('EffectBrowserPanel_Effect_', [StringComparison]::Ordinal)
                    } |
                    Sort-Object -Unique
            )
            if (-not (Test-SequenceEqual $expectedEffectBrowserKeys $actualEffectBrowserKeys))
            {
                $missingKeys = @($expectedEffectBrowserKeys | Where-Object { $_ -notin $actualEffectBrowserKeys })
                $unexpectedKeys = @($actualEffectBrowserKeys | Where-Object { $_ -notin $expectedEffectBrowserKeys })
                Add-ValidationError "$($project.Name): effect-browser resources differ from the discovered effect catalog. Missing: $($missingKeys -join ', '); unexpected: $($unexpectedKeys -join ', ')."
            }
        }
        elseif ($project.Name -eq 'ShareX.ImageEffectsLib')
        {
            foreach ($prefix in $project.DynamicPrefixes)
            {
                if (-not $sourceText.Contains($prefix))
                {
                    Add-ValidationError "$($project.Name): data-driven localization prefix '$prefix' is not used by source."
                }
            }
            if (-not $sourceText.Contains('Strings.ResourceManager.GetString(key'))
            {
                Add-ValidationError "$($project.Name): data-driven image-effect resource lookup is missing."
            }
        }
        elseif ($project.Name -eq 'ShareX.HelpersLib')
        {
            if (-not $sourceText.Contains('GetLocalizedDescription(Localization.Strings.ResourceManager)') -or
                -not $sourceText.Contains('GetLocalizedCategory(Localization.Strings.ResourceManager)'))
            {
                Add-ValidationError "$($project.Name): data-driven enum localization lookup is missing."
            }
        }
        elseif ($project.Name -eq 'ShareX.UploadersLib')
        {
            if (-not $sourceText.Contains('GetLocalizedDescription(Localization.Strings.ResourceManager)'))
            {
                Add-ValidationError "$($project.Name): data-driven enum localization lookup is missing."
            }
            if (-not $sourceText.Contains('"DestinationSettings_Field_" +') -or -not $sourceText.Contains('ResourceManager.GetString(resourceName)'))
            {
                Add-ValidationError "$($project.Name): data-driven destination-field localization lookup is missing."
            }
        }

        $literalPattern = '(?<![A-Za-z0-9_.])(?:Text|Content|Header|Title|Watermark|PlaceholderText|ToolTip\.Tip|OnContent|OffContent|AutomationProperties\.Name)="([^"]*)"'
        foreach ($view in $sourceFiles | Where-Object Extension -eq '.axaml')
        {
            foreach ($match in [regex]::Matches([IO.File]::ReadAllText($view.FullName), $literalPattern))
            {
                $value = $match.Groups[1].Value
                if (-not (Test-AllowedAxamlLiteral $value))
                {
                    $relativeViewPath = $view.FullName.Substring($repositoryDirectory.Length + 1)
                    Add-ValidationError "$relativeViewPath contains user-visible literal '$value'."
                }
            }
        }

        foreach ($source in $sourceFiles)
        {
            if ([IO.File]::ReadAllText($source.FullName).Contains('TODO: Translate'))
            {
                $relativeSourcePath = $source.FullName.Substring($repositoryDirectory.Length + 1)
                Add-ValidationError "$relativeSourcePath contains an unresolved translation TODO."
            }
        }

        if ($project.Name -eq 'ShareX.ScreenCaptureLib')
        {
            $formDirectory = Join-Path $projectDirectory 'Forms'
            $defaultFormFiles = @(
                Get-ChildItem -LiteralPath $formDirectory -File -Filter '*.resx' |
                    Where-Object { $_.BaseName -notmatch '\.' }
            )
            foreach ($formFile in $defaultFormFiles)
            {
                [xml]$formDocument = [IO.File]::ReadAllText($formFile.FullName)
                $localizableKeys = @(
                    $formDocument.root.data |
                        Where-Object { $_.name -match '(\.Text|\.ToolTip)$' } |
                        ForEach-Object { [string]$_.name }
                )
                foreach ($culture in $supportedCatalogCultures)
                {
                    $localizedFormPath = Join-Path $formDirectory "$($formFile.BaseName).$culture.resx"
                    if (-not (Test-Path -LiteralPath $localizedFormPath -PathType Leaf))
                    {
                        Add-ValidationError "$($project.Name)/$($formFile.BaseName): missing '$culture' form resource."
                        continue
                    }

                    [xml]$localizedFormDocument = [IO.File]::ReadAllText($localizedFormPath)
                    $localizedFormKeys = @($localizedFormDocument.root.data | ForEach-Object { [string]$_.name })
                    foreach ($key in $localizableKeys)
                    {
                        if ($key -notin $localizedFormKeys)
                        {
                            Add-ValidationError "$($project.Name)/$($formFile.BaseName).$culture.resx: missing '$key'."
                        }
                    }
                }
            }
        }
    }

    $translationCount = 'missing'
    if ($localizedCounts.Count -gt 0)
    {
        $minimumCount = ($localizedCounts | Measure-Object -Minimum).Minimum
        $maximumCount = ($localizedCounts | Measure-Object -Maximum).Maximum
        if ($minimumCount -eq $maximumCount -and $localizedCounts.Count -eq $supportedCatalogCultures.Count)
        {
            $translationCount = "$minimumCount each"
        }
        else
        {
            $translationCount = "$minimumCount-$maximumCount (mismatch)"
        }
    }

    $defaultCount = 0
    if ($null -ne $default)
    {
        $defaultCount = $default.Keys.Count
    }
    $summary.Add([pscustomobject]@{
        Project = $project.Name
        English = $defaultCount
        Translations = $translationCount
        Cultures = $cultureFiles.Count
        Total = $defaultCount + ($localizedCounts | Measure-Object -Sum).Sum
        Status = if ($errors.Count -eq $projectErrorCount) { 'Complete' } else { 'Failed' }
    })
}

Write-Host
Write-Host 'Translation entry counts'
$summary | Format-Table Project, English, Translations, Cultures, Total, Status -AutoSize

if ($errors.Count -gt 0)
{
    Write-Host
    foreach ($errorMessage in $errors)
    {
        Write-Error $errorMessage -ErrorAction Continue
    }

    throw "Translation validation failed with $($errors.Count) error(s)."
}

Write-Host
Write-Host "Translation validation succeeded for $($projects.Count) projects: $totalDefaultEntries English entries and $totalLocalizedEntries localized entries across $($supportedCatalogCultures.Count) cultures."
