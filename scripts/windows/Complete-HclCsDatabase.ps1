<# Run after the Installer has completed. Applies post-bootstrap PostgreSQL migrations idempotently. #>
$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest
$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
$config = Join-Path $RepoRoot ".windows\hcl-cs.env.ps1"
if (-not (Test-Path $config)) { throw "Run Initialize-HclCsLocal.ps1 first." }
. $config
try {
    $env:PGPASSWORD = $script:HclCsPgPassword
    Get-ChildItem (Join-Path $RepoRoot "scripts\migrations\*_postgresql.sql") |
        Sort-Object Name | ForEach-Object {
            Write-Host "Applying $($_.Name)"
            & $script:HclCsPsql -X -v ON_ERROR_STOP=1 `
                -h $script:HclCsPgHost -p $script:HclCsPgPort -U $script:HclCsPgUser `
                -d $script:HclCsPgDatabase -f $_.FullName
            if ($LASTEXITCODE -ne 0) { throw "Migration failed: $($_.Name)" }
        }
} finally {
    Remove-Item Env:PGPASSWORD -ErrorAction SilentlyContinue
}
Write-Host "HCL.CS migrations, including the SBOM client, are applied." -ForegroundColor Green
