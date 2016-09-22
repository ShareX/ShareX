Update-AppveyorBuild -Version "$env:AppVersion.$env:APPVEYOR_BUILD_NUMBER"

if ($env:APPVEYOR_PULL_REQUEST_NUMBER -eq $null)
{
    Invoke-WebRequest "$env:APIKeys" -OutFile "ShareX.UploadersLib\APIKeys\APIKeysLocal.cs"
}

nuget restore ShareX.sln