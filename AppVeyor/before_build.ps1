if ($env:APPVEYOR_PULL_REQUEST_NUMBER -eq $null)
{
    Invoke-WebRequest "$env:APIKeys" -OutFile "ShareX.UploadersLib\APIKeys\APIKeysLocal.cs"
}

nuget restore ShareX.sln

if ($env:CONFIGURATION -eq "WindowsStore")
{
    # Main project uses a different Framework version under the Windows Store configuration,
    # but NuGet restore doesn't knows, so have MsBuild do the job (which knows it)
    msbuild /t:restore /p:Configuration=WindowsStore ShareX\ShareX.csproj
}