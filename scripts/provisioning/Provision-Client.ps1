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
    [string]$AdminAccessToken = $env:HCL_CS_MANAGEMENT_ACCESS_TOKEN,

    [Parameter(Mandatory=$false)]
    [string]$ManagementEndpoint,

    [Parameter(Mandatory=$false)]
    [string]$SecretOutputPath
)

$ErrorActionPreference = "Stop"

try {
    if ([string]::IsNullOrWhiteSpace($AdminAccessToken)) {
        throw "An authenticated management access token is required. Pass -AdminAccessToken or set HCL_CS_MANAGEMENT_ACCESS_TOKEN."
    }

    if ([string]::IsNullOrWhiteSpace($ManagementEndpoint)) {
        $ManagementEndpoint = "$($ServerUrl.TrimEnd('/'))/Security/Api/Client/ProvisionClient"
    }

    $profiles = @{
        SPA = @{
            applicationType = 2
            requireClientSecret = $false
            requirePkce = $true
            supportedGrantTypes = @("authorization_code", "refresh_token")
            supportedResponseTypes = @("code")
        }
        Web = @{
            applicationType = 1
            requireClientSecret = $true
            requirePkce = $true
            supportedGrantTypes = @("authorization_code", "refresh_token")
            supportedResponseTypes = @("code")
        }
        Native = @{
            applicationType = 3
            requireClientSecret = $false
            requirePkce = $true
            supportedGrantTypes = @("authorization_code", "refresh_token")
            supportedResponseTypes = @("code")
        }
        M2M = @{
            applicationType = 4
            requireClientSecret = $true
            requirePkce = $false
            supportedGrantTypes = @("client_credentials")
            supportedResponseTypes = @()
        }
    }
    $profile = $profiles[$ClientType]

    $payload = @{
        clientId = $ClientId
        clientName = $ClientName
        applicationType = $profile.applicationType
        requireClientSecret = $profile.requireClientSecret
        requirePkce = $profile.requirePkce
        allowedScopes = $AllowedScopes.Split(
            ' ',
            [System.StringSplitOptions]::RemoveEmptyEntries)
        preferredAudience = $PreferredAudience
        supportedGrantTypes = $profile.supportedGrantTypes
        supportedResponseTypes = $profile.supportedResponseTypes
        allowOfflineAccess = $profile.supportedGrantTypes -contains "refresh_token"
        redirectUris = @()
        postLogoutRedirectUris = @()
    }

    if ($RedirectUri) {
        $payload.redirectUris = @(@{ redirectUri = $RedirectUri })
    }
    if ($PostLogoutRedirectUri) {
        $payload.postLogoutRedirectUris = @(
            @{ postLogoutRedirectUri = $PostLogoutRedirectUri })
    }

    Write-Host "Submitting authenticated HCL.CS client provisioning request."
    Write-Host "Target endpoint: $ManagementEndpoint"
    Write-Host "Client ID: $ClientId"
    Write-Host "Client type: $ClientType"

    $response = Invoke-RestMethod `
        -Method Post `
        -Uri $ManagementEndpoint `
        -Headers @{ Authorization = "Bearer $AdminAccessToken" } `
        -ContentType "application/json" `
        -Body ($payload | ConvertTo-Json -Depth 10 -Compress)

    $plaintextSecret = $response.clientSecret
    if (-not [string]::IsNullOrWhiteSpace($plaintextSecret) -and $SecretOutputPath) {
        $secretDirectory = Split-Path -Parent $SecretOutputPath
        if ($secretDirectory -and -not (Test-Path $secretDirectory)) {
            New-Item -ItemType Directory -Path $secretDirectory | Out-Null
        }
        Set-Content -LiteralPath $SecretOutputPath -Value $plaintextSecret -NoNewline
        if (-not $IsWindows) {
            & chmod 600 -- $SecretOutputPath
        }
        Write-Host "The one-time client secret was written to the requested protected output path."
    }

    if ($response.PSObject.Properties.Name -contains "clientSecret") {
        $response.clientSecret = if ([string]::IsNullOrWhiteSpace($plaintextSecret)) {
            $null
        } else {
            "[REDACTED]"
        }
    }

    $response | ConvertTo-Json -Depth 10
    exit 0
}
catch {
    $statusCode = $null
    if ($null -ne $_.Exception.Response -and
        $null -ne $_.Exception.Response.StatusCode) {
        $statusCode = [int]$_.Exception.Response.StatusCode
    }
    if ($statusCode) {
        Write-Error "Client provisioning failed with HTTP status $statusCode."
    } else {
        Write-Error "Client provisioning failed: $($_.Exception.Message)"
    }
    exit 1
}
