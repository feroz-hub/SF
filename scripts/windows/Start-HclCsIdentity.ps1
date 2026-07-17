$ErrorActionPreference = "Stop"
$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
$config = Join-Path $RepoRoot ".windows\hcl-cs.env.ps1"
if (-not (Test-Path $config)) { throw "Run Initialize-HclCsLocal.ps1 first." }
. $config
Push-Location $RepoRoot
try {
    & dotnet run --project .\demos\HCL.CS.Demo.Server\HCL.CS.DemoServerApp.csproj --no-launch-profile
} finally { Pop-Location }
