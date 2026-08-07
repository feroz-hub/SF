/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

-- Read-Only Phase 2B Email Uniqueness Pre-Check Query
-- Executes against PostgreSQL, SQL Server, MySQL, or SQLite to detect duplicate normalized email records prior to applying Phase 2B migration index.

SELECT
    NormalizedEmail,
    COUNT(*) AS DuplicateCount
FROM HclCs_Users
WHERE NormalizedEmail IS NOT NULL AND NormalizedEmail <> ''
GROUP BY NormalizedEmail
HAVING COUNT(*) > 1;
