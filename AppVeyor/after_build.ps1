if ($env:APPVEYOR_PULL_REQUEST_NUMBER -eq $null)
{
    if ($env:CONFIGURATION -eq "Release")
    {
        & "ShareX.Setup\bin\Release\ShareX.Setup.exe" -AppVeyorRelease
    }
    elseif ($env:APPVEYOR_REPO_TAG -eq $true)
    {
        if ($env:CONFIGURATION -eq "Steam")
        {
            & "ShareX.Setup\bin\Steam\ShareX.Setup.exe" -AppVeyorSteamRelease
        }
        elseif ($env:CONFIGURATION -eq "MicrosoftStore")
        {
            & "ShareX.Setup\bin\MicrosoftStore\ShareX.Setup.exe" -AppVeyorMicrosoftStoreRelease
        }
    }
    elseif ($env:CONFIGURATION -eq "Steam")
    {
        & "ShareX.Setup\bin\Steam\ShareX.Setup.exe" -AppVeyorSteam
    }
    elseif ($env:CONFIGURATION -eq "MicrosoftStore")
    {
        & "ShareX.Setup\bin\MicrosoftStore\ShareX.Setup.exe" -AppVeyorMicrosoftStore
    }
}