if ($env:APPVEYOR_PULL_REQUEST_NUMBER -eq $null)
{
    Invoke-WebRequest "$env:APIKeys" -OutFile "ShareX.UploadersLib\APIKeys\APIKeysLocal.cs"
}

nuget restore ShareX.sln
Remove-Item "obj\project.assets.json" # Remove when https://github.com/NuGet/Home/issues/4778 is fixed