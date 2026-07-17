$ErrorActionPreference = "Stop"
$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
$env:ASPNETCORE_ENVIRONMENT = "Development"
Push-Location $RepoRoot
try {
    & dotnet run --project .\installer\HCL.CS.Installer.Mvc\HclCsInstallerMVC.csproj --launch-profile https
} finally { Pop-Location }
