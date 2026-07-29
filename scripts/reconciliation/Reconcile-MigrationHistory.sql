/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

-- Migration History Reconciliation Script for PostgreSQL Databases.
-- Run this script ONLY on existing databases that were initialized via legacy raw SQL scripts
-- (where physical columns/tables exist but __EFMigrationsHistory entries are missing).

DO $$
BEGIN
    -- Ensure __EFMigrationsHistory table exists
    CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
        "MigrationId" character varying(150) NOT NULL,
        "ProductVersion" character varying(32) NOT NULL,
        CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
    );

    -- 1. Reconcile 20220726113011_HclCsPostgreSqlV1 if HclCs_Users table exists
    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'HclCs_Users') THEN
        IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
            INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
            VALUES ('20220726113011_HclCsPostgreSqlV1', '8.0.11');
            RAISE NOTICE 'Reconciled 20220726113011_HclCsPostgreSqlV1 in __EFMigrationsHistory.';
        END IF;
    END IF;

    -- 2. Reconcile 20260726060000_Phase2HclIdentityProfile if DirectoryImmutableId column exists on HclCs_Users
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'HclCs_Users' AND column_name = 'DirectoryImmutableId'
    ) THEN
        IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260726060000_Phase2HclIdentityProfile') THEN
            INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
            VALUES ('20260726060000_Phase2HclIdentityProfile', '8.0.11');
            RAISE NOTICE 'Reconciled 20260726060000_Phase2HclIdentityProfile in __EFMigrationsHistory.';
        END IF;
    END IF;

    -- 3. Reconcile 20260728140000_Phase2CCoreInfrastructureSchema if HclCs_ExternalIdentities table exists
    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'HclCs_ExternalIdentities') THEN
        IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260728140000_Phase2CCoreInfrastructureSchema') THEN
            INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
            VALUES ('20260728140000_Phase2CCoreInfrastructureSchema', '8.0.11');
            RAISE NOTICE 'Reconciled 20260728140000_Phase2CCoreInfrastructureSchema in __EFMigrationsHistory.';
        END IF;
    END IF;
END $$;
