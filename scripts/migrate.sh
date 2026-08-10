#!/usr/bin/env bash
set -euo pipefail

PROJECT="src/Identity/HCL.CS.Identity.Persistence/HCL.CS.Infrastructure.Data.csproj"
STARTUP="demos/HCL.CS.Demo.Server/HCL.CS.DemoServerApp.csproj"
CONTEXT="${HCL_CS_MIGRATION_CONTEXT:-PostgreSqlApplicationDbcontext}"

COMMAND="${1:-update}"
shift 1 || true

case "${COMMAND}" in
  list)
    echo "Listing EF Core migrations for context: ${CONTEXT}"
    dotnet ef migrations list --project "${PROJECT}" --startup-project "${STARTUP}" --context "${CONTEXT}" "$@"
    ;;
  script)
    OUTPUT="scripts/generated-migrations/postgresql/20260728_hcl_cs_canonical_postgresql.sql"
    echo "Generating idempotent SQL script to: ${OUTPUT}"
    mkdir -p "$(dirname "${OUTPUT}")"
    dotnet ef migrations script --idempotent --project "${PROJECT}" --startup-project "${STARTUP}" --context "${CONTEXT}" --output "${OUTPUT}" "$@"
    ;;
  update|database-update)
    echo "Applying EF Core migrations for context: ${CONTEXT}"
    dotnet ef database update --project "${PROJECT}" --startup-project "${STARTUP}" --context "${CONTEXT}" "$@"
    ;;
  *)
    dotnet ef database update --project "${PROJECT}" --startup-project "${STARTUP}" --context "${CONTEXT}" "${COMMAND}" "$@"
    ;;
esac
