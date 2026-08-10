-- Phase 2B: local identities require globally unique normalized email addresses.
-- Review and resolve any duplicate rows before applying this migration.

BEGIN;

DO $$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM "HclCs_Users"
        GROUP BY "NormalizedEmail"
        HAVING COUNT(*) > 1
    ) THEN
        RAISE EXCEPTION
            'Phase 2B migration blocked: duplicate HclCs_Users.NormalizedEmail values exist';
    END IF;
END
$$;

DROP INDEX IF EXISTS "EmailIndex";

CREATE UNIQUE INDEX "EmailIndex"
    ON "HclCs_Users" ("NormalizedEmail");

COMMIT;
