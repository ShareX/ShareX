if ($env:APPVEYOR_PULL_REQUEST_NUMBER -eq $null)
{
    if ($env:Configuration -eq "Release")
    {
        & "ShareX.Setup\bin\Release\ShareX.Setup.exe" -AppVeyorRelease
    }
    elseif ($env:Configuration -eq "Steam")
    {
        & "ShareX.Setup\bin\Steam\ShareX.Setup.exe" -AppVeyorSteam
    }
}