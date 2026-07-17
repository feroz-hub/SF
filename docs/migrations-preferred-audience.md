# PreferredAudience column (Zentra_Clients)

The optional client field **PreferredAudience** is used as the access token `aud` claim when set. The column must exist in the database.

## Option 1: Run installer migrations (recommended)

If you use the **Zentra Installer** (Zentra.Installer.Mvc):

1. Open the installer and go to the database setup / migration step.
2. Run migrations. The installer will:
   - Apply the standard EF migrations.
   - Run post-migration steps, including **EnsurePreferredAudienceColumnAsync**, which adds `PreferredAudience` to `Zentra_Clients` if it is missing (idempotent per provider).

No manual SQL is required.

## Option 2: Add the column manually

If you manage the database yourself (e.g. Identity API without running the installer), add the column with one of the following, depending on your provider.

**SQL Server**

```sql
IF COL_LENGTH('Zentra_Clients', 'PreferredAudience') IS NULL
    ALTER TABLE [Zentra_Clients] ADD [PreferredAudience] nvarchar(300) NULL;
```

**MySQL**

```sql
ALTER TABLE `Zentra_Clients` ADD COLUMN IF NOT EXISTS `PreferredAudience` varchar(300) NULL;
```

**PostgreSQL**

```sql
ALTER TABLE "Zentra_Clients" ADD COLUMN IF NOT EXISTS "PreferredAudience" varchar(300) NULL;
```

**SQLite**

```sql
ALTER TABLE "Zentra_Clients" ADD COLUMN "PreferredAudience" TEXT NULL;
```

(SQLite does not support `IF NOT EXISTS` for `ADD COLUMN`; run once or ignore “duplicate column name” if the column already exists.)

## Option 3: EF Core migrations from Persistence project

If you prefer to use EF Core migrations in the Identity solution:

1. Install the EF Core tools: `dotnet tool install --global dotnet-ef`
2. From the solution directory, set the startup project to the API (or the project that references Persistence and has the connection string):
   ```bash
   cd src/Identity/Zentra.Identity.API
   dotnet ef migrations add AddClientPreferredAudience --project ../Zentra.Identity.Persistence
   ```
3. Apply the migration:
   ```bash
   dotnet ef database update --project ../Zentra.Identity.Persistence
   ```

Note: The Identity API uses `ApplicationDbContext` from Zentra.Identity.Persistence. The **installer** uses its own DbContext and migration history; adding a migration in the Persistence project creates a separate migration history. For a single production database, prefer either the installer path (Option 1) or manual SQL (Option 2) so you don’t mix two migration histories.
