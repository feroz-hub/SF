# Migration-history reconciliation

Do not insert migration-history rows with ad hoc SQL. Use the approved
`HCL.CS.DatabaseAdmin` operator CLI so reconciliation performs complete schema
inspection, defaults to a dry run, requires an explicit confirmation token,
uses a transaction, and verifies the database again before committing.

Configure the connection string in an environment variable; do not place it on
the command line.

```bash
export HCL_CS_RECONCILIATION_CONNECTION='<operator-supplied connection string>'

dotnet run --project tools/HCL.CS.DatabaseAdmin/HCL.CS.DatabaseAdmin.csproj -- \
  reconcile \
  --provider PostgreSql \
  --connection-env HCL_CS_RECONCILIATION_CONNECTION
```

If the dry run reports an exact compatible schema and returns a confirmation
token, repeat the command with both `--apply` and `--confirm`:

```bash
dotnet run --project tools/HCL.CS.DatabaseAdmin/HCL.CS.DatabaseAdmin.csproj -- \
  reconcile \
  --provider PostgreSql \
  --connection-env HCL_CS_RECONCILIATION_CONNECTION \
  --apply \
  --confirm '<token from the immediately preceding dry run>'
```

Reconciliation is not part of runtime startup, normal Installer execution, or
standard migration execution.
