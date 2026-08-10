<#
Copyright (c) 2021 HCL CORPORATION.
All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
HCL is obtained. This is proprietary and confidential to HCL.
#>

$ErrorActionPreference = "Stop"
$ports = 3001, 5180, 7039
$connections = Get-NetTCPConnection -State Listen -LocalPort $ports -ErrorAction SilentlyContinue
$processIds = @($connections | Select-Object -ExpandProperty OwningProcess -Unique)
if ($processIds.Count -eq 0) {
    Write-Host "No HCL.CS native processes are listening on ports $($ports -join ', ')."
    exit 0
}
foreach ($processId in $processIds) {
    $process = Get-Process -Id $processId -ErrorAction SilentlyContinue
    if ($process) {
        Write-Host "Stopping $($process.ProcessName) (PID $processId)"
        Stop-Process -Id $processId
    }
}
