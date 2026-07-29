-- Read-Only Phase 2B Email Uniqueness Pre-Check Query for SQLite
-- Detects duplicate non-null normalized emails before attempting Phase 2B unique index migration.

SELECT
    "NormalizedEmail",
    COUNT(*) AS "DuplicateCount"
FROM "HclCs_Users"
WHERE "NormalizedEmail" IS NOT NULL AND "NormalizedEmail" <> ''
GROUP BY "NormalizedEmail"
HAVING COUNT(*) > 1;
