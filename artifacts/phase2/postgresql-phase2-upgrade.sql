START TRANSACTION;

ALTER TABLE "HclCs_Users" ADD "DirectoryImmutableId" character varying(512);

ALTER TABLE "HclCs_Users" ADD "EmployeeId" character varying(255);

ALTER TABLE "HclCs_Users" ADD "UserPrincipalName" character varying(255);

ALTER TABLE "HclCs_Users" ADD "DisplayName" character varying(255);

ALTER TABLE "HclCs_Users" ADD "Department" character varying(255);

ALTER TABLE "HclCs_Users" ADD "AuthenticationSource" character varying(32);

ALTER TABLE "HclCs_Users" ADD "DirectoryLastValidatedAt" timestamp with time zone;

UPDATE "HclCs_Users"
SET "AuthenticationSource" = CASE "IdentityProviderType"
    WHEN 1 THEN 'LOCAL'
    WHEN 2 THEN 'LDAP'
    WHEN 3 THEN 'GOOGLE'
    ELSE NULL
END
WHERE "AuthenticationSource" IS NULL;

DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM "HclCs_Users"
        WHERE "DirectoryImmutableId" IS NOT NULL
        GROUP BY "DirectoryImmutableId"
        HAVING COUNT(*) > 1
    ) THEN
        RAISE EXCEPTION 'Duplicate DirectoryImmutableId values must be resolved before Phase 2 migration.';
    END IF;

    IF EXISTS (
        SELECT 1 FROM "HclCs_Users"
        WHERE "UserPrincipalName" IS NOT NULL
        GROUP BY "UserPrincipalName"
        HAVING COUNT(*) > 1
    ) THEN
        RAISE EXCEPTION 'Duplicate UserPrincipalName values must be resolved before Phase 2 migration.';
    END IF;
END $$;

CREATE INDEX "IX_USERS_EMPLOYEE_ID" ON "HclCs_Users" ("EmployeeId") WHERE "EmployeeId" IS NOT NULL;

CREATE UNIQUE INDEX "UX_USERS_DIRECTORY_IMMUTABLE_ID" ON "HclCs_Users" ("DirectoryImmutableId") WHERE "DirectoryImmutableId" IS NOT NULL;

CREATE UNIQUE INDEX "UX_USERS_USER_PRINCIPAL_NAME" ON "HclCs_Users" ("UserPrincipalName") WHERE "UserPrincipalName" IS NOT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260726060000_Phase2HclIdentityProfile', '8.0.11');

COMMIT;

