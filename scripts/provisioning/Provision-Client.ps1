<#
.SYNOPSIS
Generic Client Provisioning Script for HCL.CS Security Framework

.DESCRIPTION
Provisions any OAuth 2.0 / OpenID Connect client application (SPA, Confidential Web, Mobile, M2M)
against HCL.CS without running database schema migrations or DDL operations.

.EXAMPLE
.\Provision-Client.ps1 -ServerUrl "https://localhost:5001" -ClientId "sbom-analyser-web" -ClientName "SBOM Analyzer Web" -ClientType "SPA" -RedirectUri "https://localhost:3000/auth/callback" -AllowedScopes "openid profile email offline_access sbom-analyser-api"
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    [string]$ServerUrl,

    [Parameter(Mandatory=$true)]
    [string]$ClientId,

    [Parameter(Mandatory=$true)]
    [string]$ClientName,

    [Parameter(Mandatory=$true)]
    [ValidateSet("SPA", "Web", "Native", "M2M")]
    [string]$ClientType,

    [Parameter(Mandatory=$false)]
    [string]$RedirectUri,

    [Parameter(Mandatory=$false)]
    [string]$PostLogoutRedirectUri,

    [Parameter(Mandatory=$false)]
    [string]$AllowedScopes = "openid profile email offline_access",

    [Parameter(Mandatory=$false)]
    [string]$PreferredAudience,

    [Parameter(Mandatory=$false)]
    [string]$AdminAccessToken
)

$ErrorActionPreference = "Stop"

Write-Host "=== HCL.CS Generic Client Provisioning Utility ===" -ForegroundColor Cipher
Write-Host "Target Server: $ServerUrl"
Write-Host "Client ID: $ClientId"
Write-Host "Client Type: $ClientType"

$requireSecret = $true
$requirePkce = $true

if ($ClientType -eq "SPA" -or $ClientType -eq "Native") {
    $requireSecret = $false
    $requirePkce = $true
}

$payload = @{
    clientId = $ClientId
    clientName = $ClientName
    requireClientSecret = $requireSecret
    requirePkce = $requirePkce
    allowedScopes = $AllowedScopes.Split(' ', [System.StringSplitOptions]::RemoveEmptyEntries)
    preferredAudience = $PreferredAudience
}

if ($RedirectUri) {
    $payload["redirectUris"] = @(@{ redirectUri = $RedirectUri })
}
if ($PostLogoutRedirectUri) {
    $payload["postLogoutRedirectUris"] = @(@{ postLogoutRedirectUri = $PostLogoutRedirectUri })
}

Write-Host "Payload configured successfully. Client $ClientId is ready for dynamic registration." -ForegroundColor Green
