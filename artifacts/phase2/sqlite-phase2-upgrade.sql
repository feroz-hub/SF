BEGIN TRANSACTION;

ALTER TABLE "HclCs_Users" ADD "DirectoryImmutableId" TEXT NULL;

ALTER TABLE "HclCs_Users" ADD "EmployeeId" TEXT NULL;

ALTER TABLE "HclCs_Users" ADD "UserPrincipalName" TEXT NULL;

ALTER TABLE "HclCs_Users" ADD "DisplayName" TEXT NULL;

ALTER TABLE "HclCs_Users" ADD "Department" TEXT NULL;

ALTER TABLE "HclCs_Users" ADD "AuthenticationSource" TEXT NULL;

ALTER TABLE "HclCs_Users" ADD "DirectoryLastValidatedAt" TEXT NULL;

UPDATE "HclCs_Users"
SET "AuthenticationSource" = CASE "IdentityProviderType"
    WHEN 1 THEN 'LOCAL'
    WHEN 2 THEN 'LDAP'
    WHEN 3 THEN 'GOOGLE'
    ELSE NULL
END
WHERE "AuthenticationSource" IS NULL;

CREATE INDEX "IX_USERS_EMPLOYEE_ID" ON "HclCs_Users" ("EmployeeId");

CREATE UNIQUE INDEX "UX_USERS_DIRECTORY_IMMUTABLE_ID" ON "HclCs_Users" ("DirectoryImmutableId");

CREATE UNIQUE INDEX "UX_USERS_USER_PRINCIPAL_NAME" ON "HclCs_Users" ("UserPrincipalName");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260726060000_Phase2HclIdentityProfile', '8.0.11');

COMMIT;

