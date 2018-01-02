if ($env:APPVEYOR_PULL_REQUEST_NUMBER -eq $null)
{
    Invoke-WebRequest "$env:APIKeys" -OutFile "ShareX.UploadersLib\APIKeys\APIKeysLocal.cs"
}

msbuild /t:restore /p:Configuration=$env:CONFIGURATION ShareX\ShareX.csproj
