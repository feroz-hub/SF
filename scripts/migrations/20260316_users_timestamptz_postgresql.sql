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
          AND table_name = 'HclCs_Users'
          AND column_name = 'CreatedOn'
          AND data_type = 'timestamp without time zone'
    ) THEN
        ALTER TABLE "HclCs_Users"
            ALTER COLUMN "DateOfBirth" TYPE timestamp with time zone USING "DateOfBirth" AT TIME ZONE 'UTC',
            ALTER COLUMN "LastPasswordChangedDate" TYPE timestamp with time zone USING "LastPasswordChangedDate" AT TIME ZONE 'UTC',
            ALTER COLUMN "LastLoginDateTime" TYPE timestamp with time zone USING "LastLoginDateTime" AT TIME ZONE 'UTC',
            ALTER COLUMN "LastLogoutDateTime" TYPE timestamp with time zone USING "LastLogoutDateTime" AT TIME ZONE 'UTC',
            ALTER COLUMN "CreatedOn" TYPE timestamp with time zone USING "CreatedOn" AT TIME ZONE 'UTC',
            ALTER COLUMN "ModifiedOn" TYPE timestamp with time zone USING "ModifiedOn" AT TIME ZONE 'UTC';
    END IF;
END $$;
