$content = Get-Content "D:\Dev\GitHub\ShareX\ShareX\ShareX\Properties\AssemblyInfo.cs"
$match = [regex]::Match($content, 'AssemblyVersion\(\"(.+?)\"\)')
if ($match.Success)
{
    $env:AppVersion = $match.Groups[1].Value
}
Write-Host "Test: ShareX $env:AppVersion"