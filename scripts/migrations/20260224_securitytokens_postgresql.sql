/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

ALTER TABLE "HclCs_SecurityTokens"
    ADD COLUMN IF NOT EXISTS "ConsumedAt" timestamp with time zone NULL;

ALTER TABLE "HclCs_SecurityTokens"
    ADD COLUMN IF NOT EXISTS "TokenReuseDetected" boolean NOT NULL DEFAULT FALSE;

CREATE INDEX IF NOT EXISTS "IX_SECTOK_TOKTYPE_KEY"
    ON "HclCs_SecurityTokens" ("TokenType", "Key");
