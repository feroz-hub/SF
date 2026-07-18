/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

DO $$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM information_schema.columns
        WHERE table_schema = current_schema()
          AND table_name = 'HclCs_SecurityTokens'
          AND column_name = 'CreationTime'
          AND data_type = 'timestamp without time zone'
    ) THEN
        ALTER TABLE "HclCs_SecurityTokens"
            ALTER COLUMN "CreatedOn" TYPE timestamp with time zone USING "CreatedOn" AT TIME ZONE 'UTC',
            ALTER COLUMN "ModifiedOn" TYPE timestamp with time zone USING "ModifiedOn" AT TIME ZONE 'UTC',
            ALTER COLUMN "CreationTime" TYPE timestamp with time zone USING "CreationTime" AT TIME ZONE 'UTC',
            ALTER COLUMN "ConsumedTime" TYPE timestamp with time zone USING "ConsumedTime" AT TIME ZONE 'UTC',
            ALTER COLUMN "ConsumedAt" TYPE timestamp with time zone USING "ConsumedAt" AT TIME ZONE 'UTC';
    END IF;
END $$;
