$content = Get-Content "SharedAssemblyInfo.cs"
$match = [regex]::Match($content, 'AssemblyVersion\(\"(.+?)\"\)')
if ($match.Success)
{
    $env:AppVersion = $match.Groups[1].Value
    Update-AppveyorBuild -Version "$env:AppVersion.$env:APPVEYOR_BUILD_NUMBER"
}
else
{
    $env:AppVersion = "1.0.0"
}

Write-Host "Installing InnoSetup 6.2.0..." -ForegroundColor Cyan

$exePath = "$env:TEMP\innosetup-6.2.0.exe"

Write-Host "Downloading..."
(New-Object Net.WebClient).DownloadFile('https://files.jrsoftware.org/is/6/innosetup-6.2.0.exe', $exePath)

Write-Host "Installing..."
cmd /c start /wait $exePath /VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP-
Remove-Item $exePath

Write-Host "InnoSetup installed" -ForegroundColor Green