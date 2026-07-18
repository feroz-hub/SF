/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

IF OBJECT_ID('HclCs_ExternalIdentities', 'U') IS NULL
BEGIN
    CREATE TABLE [HclCs_ExternalIdentities] (
        [Id] uniqueidentifier NOT NULL CONSTRAINT [PK_HclCs_ExternalIdentities] PRIMARY KEY,
        [IsDeleted] bit NOT NULL CONSTRAINT [DF_HclCs_ExternalIdentities_IsDeleted] DEFAULT (0),
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        [CreatedBy] nvarchar(255) NOT NULL,
        [ModifiedBy] nvarchar(255) NULL,
        [RowVersion] rowversion,
        [UserId] uniqueidentifier NOT NULL,
        [TenantId] nvarchar(128) NULL,
        [Provider] nvarchar(64) NOT NULL,
        [Issuer] nvarchar(256) NOT NULL,
        [Subject] nvarchar(256) NOT NULL,
        [Email] nvarchar(255) NOT NULL,
        [EmailVerified] bit NOT NULL,
        [LinkedAt] datetime2 NOT NULL,
        [LastSignInAt] datetime2 NULL,
        CONSTRAINT [FK_HclCs_ExternalIdentities_HclCs_Users_UserId]
            FOREIGN KEY ([UserId]) REFERENCES [HclCs_Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EXTID_PROVIDER_ISSUER_SUBJECT' AND object_id = OBJECT_ID('HclCs_ExternalIdentities'))
BEGIN
    CREATE UNIQUE INDEX [IX_EXTID_PROVIDER_ISSUER_SUBJECT]
        ON [HclCs_ExternalIdentities] ([Provider], [Issuer], [Subject]);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EXTID_USERID' AND object_id = OBJECT_ID('HclCs_ExternalIdentities'))
BEGIN
    CREATE INDEX [IX_EXTID_USERID]
        ON [HclCs_ExternalIdentities] ([UserId]);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EXTID_TENANT_EMAIL' AND object_id = OBJECT_ID('HclCs_ExternalIdentities'))
BEGIN
    CREATE INDEX [IX_EXTID_TENANT_EMAIL]
        ON [HclCs_ExternalIdentities] ([TenantId], [Email]);
END;
GO
