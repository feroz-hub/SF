IF OBJECT_ID('Zentra_ExternalIdentities', 'U') IS NULL
BEGIN
    CREATE TABLE [Zentra_ExternalIdentities] (
        [Id] uniqueidentifier NOT NULL CONSTRAINT [PK_Zentra_ExternalIdentities] PRIMARY KEY,
        [IsDeleted] bit NOT NULL CONSTRAINT [DF_Zentra_ExternalIdentities_IsDeleted] DEFAULT (0),
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
        CONSTRAINT [FK_Zentra_ExternalIdentities_Zentra_Users_UserId]
            FOREIGN KEY ([UserId]) REFERENCES [Zentra_Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EXTID_PROVIDER_ISSUER_SUBJECT' AND object_id = OBJECT_ID('Zentra_ExternalIdentities'))
BEGIN
    CREATE UNIQUE INDEX [IX_EXTID_PROVIDER_ISSUER_SUBJECT]
        ON [Zentra_ExternalIdentities] ([Provider], [Issuer], [Subject]);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EXTID_USERID' AND object_id = OBJECT_ID('Zentra_ExternalIdentities'))
BEGIN
    CREATE INDEX [IX_EXTID_USERID]
        ON [Zentra_ExternalIdentities] ([UserId]);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EXTID_TENANT_EMAIL' AND object_id = OBJECT_ID('Zentra_ExternalIdentities'))
BEGIN
    CREATE INDEX [IX_EXTID_TENANT_EMAIL]
        ON [Zentra_ExternalIdentities] ([TenantId], [Email]);
END;
GO
