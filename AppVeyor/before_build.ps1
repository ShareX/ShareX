if ($env:APPVEYOR_PULL_REQUEST_NUMBER -eq $null)
{
    Invoke-WebRequest "$env:APIKeys" -OutFile "ShareX.UploadersLib\APIKeys\APIKeysLocal.cs"
}

nuget restore ShareX.sln

# Remove when https://github.com/NuGet/Home/issues/4778 is fixed
$net40 =  @"
      "originalTargetFrameworks": [
        ".NETFramework,Version=v4.0"
      ],
"@

$net46 = @"
      "originalTargetFrameworks": [
        ".NETFramework,Version=v4.6"
      ],
"@

if ($env:CONFIGURATION -eq "WindowsStore")
{
    $fileContent = Get-Content "ShareX\obj\project.assets.json" -Raw
    $newFileContent = $fileContent -replace $net40, $net46
    Set-Content -Path "ShareX\obj\project.assets.json" -Value $newFileContent
}