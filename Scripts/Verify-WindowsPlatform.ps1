param(
    [ValidateSet('Release', 'Debug')]
    [string] $Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'
if (-not $IsWindows) {
    throw 'Windows platform verification must run on Windows with PowerShell 7.'
}

$repositoryPath = Split-Path -Parent $PSScriptRoot
$verificationPath = Join-Path $repositoryPath 'artifacts/windows-verification'
New-Item -ItemType Directory -Path $verificationPath -Force | Out-Null
$projectPath = Join-Path $verificationPath 'WindowsVerification.csproj'
$sourcePath = [System.Security.SecurityElement]::Escape((Join-Path $PSScriptRoot 'WindowsVerification/Program.cs'))
$platformPath = [System.Security.SecurityElement]::Escape((Join-Path $repositoryPath 'ShareX.Platform.Windows/ShareX.Platform.Windows.csproj'))

# Keep the throwaway project and its build outputs out of the solution and source tree.
@"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0-windows10.0.22621.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="$sourcePath" />
    <ProjectReference Include="$platformPath" />
  </ItemGroup>
</Project>
"@ | Set-Content -LiteralPath $projectPath -Encoding utf8

& dotnet run --project $projectPath --configuration $Configuration -p:Platform=x64
if ($LASTEXITCODE -ne 0) {
    throw "Windows platform verification failed (exit code $LASTEXITCODE)."
}
