<#
Copyright (c) 2021 HCL CORPORATION.
All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
HCL is obtained. This is proprietary and confidential to HCL.
#>

<# Initializes HCL.CS for native Windows development with local PostgreSQL. #>
[CmdletBinding()]
param(
    [string]$PostgresHost = "localhost",
    [int]$PostgresPort = 5432,
    [string]$PostgresAdminUser = "postgres",
    [string]$DatabaseName = "hcl_cs",
    [string]$DatabaseUser = "hcl_cs",
    [switch]$SkipDependencyRestore
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest
$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path

function Get-PlainText([Security.SecureString]$Value) {
    $ptr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($Value)
    try { [Runtime.InteropServices.Marshal]::PtrToStringBSTR($ptr) }
    finally { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($ptr) }
}

function Find-Psql {
    $command = Get-Command psql.exe -ErrorAction SilentlyContinue
    if ($command) { return $command.Source }
    $root = Join-Path $env:ProgramFiles "PostgreSQL"
    if (Test-Path $root) {
        $candidate = Get-ChildItem $root -Filter psql.exe -Recurse -ErrorAction SilentlyContinue |
            Sort-Object FullName -Descending | Select-Object -First 1
        if ($candidate) { return $candidate.FullName }
    }
    throw "psql.exe was not found. Install PostgreSQL 16 and open a new PowerShell window."
}

function Assert-Identifier([string]$Value, [string]$Name) {
    if ($Value -notmatch '^[A-Za-z_][A-Za-z0-9_]*$') {
        throw "$Name must contain only letters, digits, and underscores and cannot start with a digit."
    }
}

function Invoke-Psql([string]$Psql, [string]$User, [string]$Database, [string]$Sql) {
    $Sql | & $Psql -X -v ON_ERROR_STOP=1 -h $PostgresHost -p $PostgresPort -U $User -d $Database
    if ($LASTEXITCODE -ne 0) { throw "PostgreSQL command failed." }
}

Assert-Identifier $DatabaseName "DatabaseName"
Assert-Identifier $DatabaseUser "DatabaseUser"
if (-not (Get-Command dotnet.exe -ErrorAction SilentlyContinue)) {
    throw "The .NET 8 SDK is required. Install it with: winget install Microsoft.DotNet.SDK.8"
}
if (-not ((& dotnet --list-sdks) -match '^8\.0\.')) {
    throw "A .NET 8 SDK is required by global.json."
}
if (-not (Get-Command npm.cmd -ErrorAction SilentlyContinue)) {
    throw "Node.js/npm is required. Install it with: winget install OpenJS.NodeJS.LTS"
}

$psql = Find-Psql
$adminPassword = Get-PlainText (Read-Host "PostgreSQL password for $PostgresAdminUser" -AsSecureString)
$databasePassword = Get-PlainText (Read-Host "Choose a local password for database role $DatabaseUser" -AsSecureString)
$escapedUser = $DatabaseUser.Replace("'", "''")
$escapedPassword = $databasePassword.Replace("'", "''")

try {
    $env:PGPASSWORD = $adminPassword
    $roleExists = (& $psql -X -tA -h $PostgresHost -p $PostgresPort -U $PostgresAdminUser -d postgres `
        -c "SELECT 1 FROM pg_roles WHERE rolname='$escapedUser'").Trim()
    if ($roleExists -ne "1") {
        Invoke-Psql $psql $PostgresAdminUser postgres `
            "CREATE ROLE `"$DatabaseUser`" LOGIN PASSWORD '$escapedPassword';"
    } else {
        Invoke-Psql $psql $PostgresAdminUser postgres `
            "ALTER ROLE `"$DatabaseUser`" WITH LOGIN PASSWORD '$escapedPassword';"
    }
    $dbExists = (& $psql -X -tA -h $PostgresHost -p $PostgresPort -U $PostgresAdminUser -d postgres `
        -c "SELECT 1 FROM pg_database WHERE datname='$($DatabaseName.Replace("'", "''"))'").Trim()
    if ($dbExists -ne "1") {
        Invoke-Psql $psql $PostgresAdminUser postgres `
            "CREATE DATABASE `"$DatabaseName`" OWNER `"$DatabaseUser`";"
    }
} finally {
    Remove-Item Env:PGPASSWORD -ErrorAction SilentlyContinue
    $adminPassword = $null
}

$windowsDir = Join-Path $RepoRoot ".windows"
$certificateDir = Join-Path $windowsDir "certificates"
New-Item -ItemType Directory -Force $certificateDir | Out-Null
& dotnet dev-certs https --trust
if ($LASTEXITCODE -ne 0) { throw "Unable to trust the ASP.NET Core development certificate." }
$certificatePath = Join-Path $certificateDir "hcl-cs-local.pem"
& dotnet dev-certs https -ep $certificatePath --format PEM --no-password
if ($LASTEXITCODE -ne 0) { throw "Unable to export the ASP.NET Core development certificate." }

$connectionPassword = $databasePassword.Replace('"', '""')
$connectionString = "Host=$PostgresHost;Port=$PostgresPort;Database=$DatabaseName;Username=$DatabaseUser;Password=`"$connectionPassword`""
function Quote-Ps([string]$Value) { "'" + $Value.Replace("'", "''") + "'" }
$envScript = @(
    "`$env:ASPNETCORE_ENVIRONMENT = 'Development'",
    "`$env:ASPNETCORE_URLS = 'https://localhost:5180'",
    "`$env:HCL_CS_DB_CONNECTION_STRING = $(Quote-Ps $connectionString)",
    "`$env:SystemSettings__DBConfig__Database = 'PostgreSQL'",
    "`$env:TokenSettings__TokenConfig__IssuerUri = 'https://localhost:5180'",
    "`$env:Security__Cors__AllowedOrigins__0 = 'https://localhost:3000'",
    "`$env:Security__Cors__AllowedOrigins__1 = 'https://localhost:3001'",
    "`$env:HCL_CS_DATA_PROTECTION_KEYS_PATH = $(Quote-Ps (Join-Path $windowsDir 'DataProtection-Keys'))",
    "`$script:HclCsPsql = $(Quote-Ps $psql)",
    "`$script:HclCsPgHost = $(Quote-Ps $PostgresHost)",
    "`$script:HclCsPgPort = $PostgresPort",
    "`$script:HclCsPgDatabase = $(Quote-Ps $DatabaseName)",
    "`$script:HclCsPgUser = $(Quote-Ps $DatabaseUser)",
    "`$script:HclCsPgPassword = $(Quote-Ps $databasePassword)"
)
Set-Content -Path (Join-Path $windowsDir "hcl-cs.env.ps1") -Value $envScript -Encoding utf8
$databasePassword = $null

if (-not $SkipDependencyRestore) {
    Push-Location $RepoRoot
    try { & dotnet restore .\HCL.CS.sln; if ($LASTEXITCODE -ne 0) { throw "dotnet restore failed" } }
    finally { Pop-Location }
    Push-Location (Join-Path $RepoRoot "HCL.CS-admin")
    try { & npm.cmd ci; if ($LASTEXITCODE -ne 0) { throw "HCL.CS Admin npm install failed" } }
    finally { Pop-Location }
}

& powershell.exe -ExecutionPolicy Bypass -File `
    (Join-Path $RepoRoot "HCL.CS-admin\scripts\setup-dev-https.ps1")
if ($LASTEXITCODE -ne 0) { throw "HCL.CS Admin HTTPS setup failed." }

Write-Host "HCL.CS native prerequisites are ready." -ForegroundColor Green
Write-Host "Next: run .\scripts\windows\Start-HclCsInstaller.ps1 and complete https://localhost:7039/setup"
Write-Host "Installer PostgreSQL database/user: $DatabaseName / $DatabaseUser (use the password you just chose)."
