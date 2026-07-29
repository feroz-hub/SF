# Generated MySQL Canonical Migration Script

This directory contains the canonical EF-generated idempotent MySQL SQL migration script for HCL.CS database deployment.

## File Information
- **File:** `20260728_hcl_cs_canonical_mysql.sql`
- **Source Assembly:** `HCL.CS.Infrastructure.Data` (`src/Identity/HCL.CS.Identity.Persistence/HCL.CS.Infrastructure.Data.csproj`)
- **DbContext:** `HCL.CS.Infrastructure.Data.MySqlApplicationDbContext`
- **Included Migration Range:** `20220722123951_HclCsMySqlV1` → `20260728140000_Phase2CCoreInfrastructureSchema`

## Generation Command
```bash
dotnet ef migrations script --idempotent \
  --project src/Identity/HCL.CS.Identity.Persistence/HCL.CS.Infrastructure.Data.csproj \
  --startup-project demos/HCL.CS.Demo.Server/HCL.CS.DemoServerApp.csproj \
  --context MySqlApplicationDbContext \
  --output scripts/generated-migrations/mysql/20260728_hcl_cs_canonical_mysql.sql
```
