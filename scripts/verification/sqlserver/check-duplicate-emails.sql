-- Read-Only Phase 2B Email Uniqueness Pre-Check Query for SQL Server
-- Detects duplicate non-null normalized emails before attempting Phase 2B unique index migration.

SELECT
    [NormalizedEmail],
    COUNT(*) AS [DuplicateCount]
FROM [dbo].[HclCs_Users]
WHERE [NormalizedEmail] IS NOT NULL AND [NormalizedEmail] <> ''
GROUP BY [NormalizedEmail]
HAVING COUNT(*) > 1;
