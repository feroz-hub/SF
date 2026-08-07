# Generated PostgreSQL Canonical Migration Script

This directory contains the canonical, EF-generated idempotent PostgreSQL SQL migration script for HCL.CS database deployment.

## File Information
- **File:** `20260728_hcl_cs_canonical_postgresql.sql`
- **Source Assembly:** `HCL.CS.Infrastructure.Data` (`src/Identity/HCL.CS.Identity.Persistence/HCL.CS.Infrastructure.Data.csproj`)
- **DbContext:** `HCL.CS.Infrastructure.Data.PostgreSqlApplicationDbcontext`
- **Included Migration Range:** `20220726113011_HclCsPostgreSqlV1` → `20260728140000_Phase2CCoreInfrastructureSchema`

## Generation Command
To regenerate this script from the canonical EF Core migrations, run:

```bash
dotnet ef migrations script --idempotent \
  --project src/Identity/HCL.CS.Identity.Persistence/HCL.CS.Infrastructure.Data.csproj \
  --startup-project demos/HCL.CS.Demo.Server/HCL.CS.DemoServerApp.csproj \
  --context PostgreSqlApplicationDbcontext \
  --output scripts/generated-migrations/postgresql/20260728_hcl_cs_canonical_postgresql.sql
```

## Policy
- **Do NOT manually edit this file.** All DDL and schema changes must originate from EF Core migrations in `HCL.CS.Infrastructure.Data`.
- This file updates `__EFMigrationsHistory` automatically for every applied migration step.
