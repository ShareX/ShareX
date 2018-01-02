if ($env:APPVEYOR_PULL_REQUEST_NUMBER -eq $null)
{
    Invoke-WebRequest "$env:APIKeys" -OutFile "ShareX.UploadersLib\APIKeys\APIKeysLocal.cs"
}

nuget restore ShareX.sln
Get-ChildItem -rec | Where {$_.Name -match "project.assets.json"} | Remove-Item # Remove when https://github.com/NuGet/Home/issues/4778 is fixed