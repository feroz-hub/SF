# Generated SQLite Canonical Migration Script

This directory contains the canonical EF-generated SQLite SQL migration script for HCL.CS database deployment.

## File Information
- **File:** `20260728_hcl_cs_canonical_sqlite.sql`
- **Source Assembly:** `HCL.CS.Infrastructure.Data` (`src/Identity/HCL.CS.Identity.Persistence/HCL.CS.Infrastructure.Data.csproj`)
- **DbContext:** `HCL.CS.Infrastructure.Data.SqLiteApplicationDbContext`
- **Included Migration Range:** `20220802110433_HclCsSqliteV1` → `20260728140000_Phase2CCoreInfrastructureSchema`

## Generation Command
```bash
dotnet ef migrations script \
  --project src/Identity/HCL.CS.Identity.Persistence/HCL.CS.Infrastructure.Data.csproj \
  --startup-project demos/HCL.CS.Demo.Server/HCL.CS.DemoServerApp.csproj \
  --context SqLiteApplicationDbContext \
  --output scripts/generated-migrations/sqlite/20260728_hcl_cs_canonical_sqlite.sql
```

## Deployment Model
- Applicable to fresh SQLite database creation or via `Database.MigrateAsync()` migration runner.
