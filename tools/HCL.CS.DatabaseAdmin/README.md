# HCL.CS database administration CLI

This executable is the approved operator surface for database migration and migration-history reconciliation.
It never accepts a connection string on the command line; use `--connection-env` to name the environment
variable containing the connection string.

Migration uses the same `DatabaseMigrationService` as the HCL.CS Installer, including duplicate-email
preflight checks.

Reconciliation is a dry run unless `--apply --confirm <token>` is supplied. Obtain the token from a compatible
dry run. The apply operation rechecks the schema inside an explicit transaction, writes only validated history
rows, performs post-write verification, and commits or rolls back as one unit.

Normal runtime startup and normal Installer execution never invoke reconciliation.
