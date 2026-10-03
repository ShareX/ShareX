param(
    [ValidateSet('Release', 'Debug')]
    [string] $Configuration = 'Release',
    [string] $EmojiFontPath
)

$ErrorActionPreference = 'Stop'
$repositoryPath = Split-Path -Parent $PSScriptRoot
$verificationPath = Join-Path $repositoryPath 'artifacts/editor-graphics-verification'
New-Item -ItemType Directory -Path $verificationPath -Force | Out-Null
$projectPath = Join-Path $verificationPath 'EditorGraphicsVerification.csproj'
$sourcePath = [System.Security.SecurityElement]::Escape((Join-Path $PSScriptRoot 'EditorGraphicsVerification/Program.cs'))
$editorPath = [System.Security.SecurityElement]::Escape((Join-Path $repositoryPath 'ShareX.ImageEditor/ShareX.ImageEditor.csproj'))

@"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="$sourcePath" />
    <ProjectReference Include="$editorPath" />
  </ItemGroup>
</Project>
"@ | Set-Content -LiteralPath $projectPath -Encoding utf8

$runtimeIdentifier = [System.Runtime.InteropServices.RuntimeInformation]::RuntimeIdentifier
$verificationArguments = @($verificationPath)
if ($EmojiFontPath) {
    $verificationArguments += (Resolve-Path -LiteralPath $EmojiFontPath).Path
}
& dotnet run --project $projectPath --configuration $Configuration -p:Platform=x64 "-p:RuntimeIdentifier=$runtimeIdentifier" -- @verificationArguments
if ($LASTEXITCODE -ne 0) {
    throw "Editor graphics verification failed (exit code $LASTEXITCODE)."
}
