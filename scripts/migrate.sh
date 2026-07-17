#!/usr/bin/env bash
set -euo pipefail

# Applies EF Core migrations for the persistence project.
dotnet ef database update \
  --project src/Identity/HCL.CS.Identity.Persistence/HCL.CS.Infrastructure.Data.csproj \
  --startup-project demos/HCL.CS.Demo.Server/HCL.CS.DemoServerApp.csproj \
  "$@"
