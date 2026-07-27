/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

-- Phase 2 identity profile storage. All columns are nullable so existing user IDs
-- and the stable subject derived from HclCs_Users.Id remain unchanged.

ALTER TABLE "HclCs_Users" ADD COLUMN IF NOT EXISTS "DirectoryImmutableId" character varying(512) NULL;
ALTER TABLE "HclCs_Users" ADD COLUMN IF NOT EXISTS "EmployeeId" character varying(255) NULL;
ALTER TABLE "HclCs_Users" ADD COLUMN IF NOT EXISTS "UserPrincipalName" character varying(255) NULL;
ALTER TABLE "HclCs_Users" ADD COLUMN IF NOT EXISTS "DisplayName" character varying(255) NULL;
ALTER TABLE "HclCs_Users" ADD COLUMN IF NOT EXISTS "Department" character varying(255) NULL;
ALTER TABLE "HclCs_Users" ADD COLUMN IF NOT EXISTS "AuthenticationSource" character varying(32) NULL;
ALTER TABLE "HclCs_Users" ADD COLUMN IF NOT EXISTS "DirectoryLastValidatedAt" timestamp with time zone NULL;

-- IdentityProviderType is authoritative for provider origin. Directory identifiers
-- and employee attributes are intentionally not fabricated; LDAP fills them on
-- the next successful authentication.
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
        SELECT 1
        FROM "HclCs_Users"
        WHERE "DirectoryImmutableId" IS NOT NULL
        GROUP BY "DirectoryImmutableId"
        HAVING COUNT(*) > 1
    ) THEN
        RAISE EXCEPTION 'Duplicate DirectoryImmutableId values must be resolved before Phase 2 migration.';
    END IF;

    IF EXISTS (
        SELECT 1
        FROM "HclCs_Users"
        WHERE "UserPrincipalName" IS NOT NULL
        GROUP BY "UserPrincipalName"
        HAVING COUNT(*) > 1
    ) THEN
        RAISE EXCEPTION 'Duplicate UserPrincipalName values must be resolved before Phase 2 migration.';
    END IF;
END $$;

CREATE UNIQUE INDEX IF NOT EXISTS "UX_USERS_DIRECTORY_IMMUTABLE_ID"
    ON "HclCs_Users" ("DirectoryImmutableId")
    WHERE "DirectoryImmutableId" IS NOT NULL;

CREATE UNIQUE INDEX IF NOT EXISTS "UX_USERS_USER_PRINCIPAL_NAME"
    ON "HclCs_Users" ("UserPrincipalName")
    WHERE "UserPrincipalName" IS NOT NULL;

CREATE INDEX IF NOT EXISTS "IX_USERS_EMPLOYEE_ID"
    ON "HclCs_Users" ("EmployeeId")
    WHERE "EmployeeId" IS NOT NULL;
