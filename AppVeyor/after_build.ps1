if ($env:APPVEYOR_PULL_REQUEST_NUMBER -eq $null)
{
    & "ShareX.Setup\bin\$env:CONFIGURATION\ShareX.Setup.exe" -AppVeyor "$env:CONFIGURATION"
}