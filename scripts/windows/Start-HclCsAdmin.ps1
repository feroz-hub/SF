$ErrorActionPreference = "Stop"
$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
$adminRoot = Join-Path $RepoRoot "HCL.CS-admin"
$hclConfig = Join-Path $RepoRoot ".windows\hcl-cs.env.ps1"
if (Test-Path $hclConfig) { . $hclConfig }
$env:NODE_EXTRA_CA_CERTS = Join-Path $RepoRoot ".windows\certificates\hcl-cs-local.pem"
if (-not (Test-Path (Join-Path $adminRoot ".env.local"))) {
    throw "Configure HCL.CS-admin\.env.local with the client values produced by the Installer."
}
Push-Location $adminRoot
try { & npm.cmd run dev:https }
finally { Pop-Location }
