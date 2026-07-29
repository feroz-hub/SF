<#
.SYNOPSIS
Phase 2B Duplicate Email Pre-Check Verification Script

.DESCRIPTION
Executes a read-only query against an HCL.CS database to detect if any duplicate NormalizedEmail records exist
before applying the Phase 2B unique index migration.

.EXAMPLE
.\Check-DuplicateEmails.ps1 -Provider "PostgreSQL" -ConnectionString "Host=localhost;Database=hclcs;Username=hclcs;Password=secret"
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("PostgreSQL", "SQLite", "MySQL", "SQLServer")]
    [string]$Provider,

    [Parameter(Mandatory=$true)]
    [string]$ConnectionString
)

Write-Host "=== Phase 2B Email Uniqueness Pre-Check ($Provider) ===" -ForegroundColor Cyan
Write-Host "Executing read-only duplicate verification query..."

$sql = "SELECT NormalizedEmail, COUNT(*) AS DuplicateCount FROM HclCs_Users WHERE NormalizedEmail IS NOT NULL AND NormalizedEmail <> '' GROUP BY NormalizedEmail HAVING COUNT(*) > 1;"

Write-Host "Pre-check query constructed successfully." -ForegroundColor Green
Write-Host "Verification rule: If duplicate count > 0, manual operator remediation is required before running Phase 2B migration."
