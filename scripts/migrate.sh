#!/usr/bin/env bash
set -euo pipefail

# Applies EF Core migrations for the persistence project.
dotnet ef database update \
  --project src/Identity/Zentra.Identity.Persistence/Zentra.Infrastructure.Data.csproj \
  --startup-project demos/Zentra.Demo.Server/Zentra.DemoServerApp.csproj \
  "$@"
