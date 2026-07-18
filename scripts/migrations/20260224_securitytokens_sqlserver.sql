/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

IF COL_LENGTH('HclCs_SecurityTokens', 'ConsumedAt') IS NULL
BEGIN
    ALTER TABLE [HclCs_SecurityTokens] ADD [ConsumedAt] datetime2 NULL;
END;
GO

IF COL_LENGTH('HclCs_SecurityTokens', 'TokenReuseDetected') IS NULL
BEGIN
    ALTER TABLE [HclCs_SecurityTokens] ADD [TokenReuseDetected] bit NOT NULL CONSTRAINT [DF_HclCs_SecurityTokens_TokenReuseDetected] DEFAULT (0);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_SECTOK_TOKTYPE_KEY' AND object_id = OBJECT_ID('HclCs_SecurityTokens'))
BEGIN
    CREATE INDEX [IX_SECTOK_TOKTYPE_KEY] ON [HclCs_SecurityTokens] ([TokenType], [Key]);
END;
GO
