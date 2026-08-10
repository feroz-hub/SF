<#
Copyright (c) 2021 HCL CORPORATION.
All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
HCL is obtained. This is proprietary and confidential to HCL.
#>

$ErrorActionPreference = "Stop"
$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
$config = Join-Path $RepoRoot ".windows\hcl-cs.env.ps1"
if (-not (Test-Path $config)) { throw "Run Initialize-HclCsLocal.ps1 first." }
. $config
Push-Location $RepoRoot
try {
    & dotnet run --project .\demos\HCL.CS.Demo.Server\HCL.CS.DemoServerApp.csproj --no-launch-profile
} finally { Pop-Location }
