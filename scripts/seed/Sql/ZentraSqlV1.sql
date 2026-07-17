IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE TABLE [Zentra_ApiResources] (
        [Id] uniqueidentifier NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        [CreatedBy] nvarchar(255) NOT NULL,
        [ModifiedBy] nvarchar(255) NULL,
        [RowVersion] rowversion NULL,
        [Name] nvarchar(255) NOT NULL,
        [DisplayName] nvarchar(255) NULL,
        [Description] nvarchar(max) NULL,
        [Enabled] bit NOT NULL,
        CONSTRAINT [PK_Zentra_ApiResources] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE TABLE [Zentra_AuditTrail] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] nvarchar(255) NOT NULL,
        [ActionType] int NOT NULL,
        [TableName] nvarchar(255) NULL,
        [OldValue] nvarchar(max) NULL,
        [NewValue] nvarchar(max) NULL,
        [AffectedColumn] nvarchar(max) NULL,
        [ActionName] nvarchar(max) NULL,
        CONSTRAINT [PK_Zentra_AuditTrail] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE TABLE [Zentra_Clients] (
        [Id] uniqueidentifier NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        [CreatedBy] nvarchar(255) NOT NULL,
        [ModifiedBy] nvarchar(255) NULL,
        [RowVersion] rowversion NULL,
        [ClientId] nvarchar(128) NOT NULL,
        [ClientName] nvarchar(255) NULL,
        [ClientUri] nvarchar(max) NULL,
        [ClientIdIssuedAt] bigint NOT NULL,
        [ClientSecretExpiresAt] bigint NOT NULL,
        [ClientSecret] nvarchar(128) NULL,
        [LogoUri] nvarchar(max) NULL,
        [TermsOfServiceUri] nvarchar(max) NULL,
        [PolicyUri] nvarchar(max) NULL,
        [RefreshTokenExpiration] int NOT NULL,
        [AccessTokenExpiration] int NOT NULL,
        [IdentityTokenExpiration] int NOT NULL,
        [LogoutTokenExpiration] int NOT NULL,
        [AuthorizationCodeExpiration] int NOT NULL,
        [AccessTokenType] int NOT NULL,
        [RequirePkce] bit NOT NULL,
        [IsPkceTextPlain] bit NOT NULL,
        [RequireClientSecret] bit NOT NULL,
        [IsFirstPartyApp] bit NOT NULL,
        [AllowOfflineAccess] bit NOT NULL,
        [AllowedScopes] nvarchar(max) NULL,
        [AllowAccessTokensViaBrowser] bit NOT NULL,
        [ApplicationType] int NOT NULL,
        [AllowedSigningAlgorithm] nvarchar(max) NULL,
        [SupportedGrantTypes] nvarchar(max) NULL,
        [SupportedResponseTypes] nvarchar(max) NULL,
        [FrontChannelLogoutSessionRequired] bit NOT NULL,
        [FrontChannelLogoutUri] nvarchar(max) NULL,
        [BackChannelLogoutSessionRequired] bit NOT NULL,
        [BackChannelLogoutUri] nvarchar(max) NULL,
        CONSTRAINT [PK_Zentra_Clients] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE TABLE [Zentra_IdentityResources] (
        [Id] uniqueidentifier NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        [CreatedBy] nvarchar(255) NOT NULL,
        [ModifiedBy] nvarchar(255) NULL,
        [RowVersion] rowversion NULL,
        [Name] nvarchar(255) NOT NULL,
        [DisplayName] nvarchar(255) NULL,
        [Description] nvarchar(max) NULL,
        [Enabled] bit NOT NULL,
        [Required] bit NOT NULL,
        [Emphasize] bit NOT NULL,
        CONSTRAINT [PK_Zentra_IdentityResources] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE TABLE [Zentra_Roles] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(255) NOT NULL,
        [NormalizedName] nvarchar(255) NOT NULL,
        [ConcurrencyStamp] nvarchar(255) NOT NULL,
        [Description] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        [CreatedBy] nvarchar(255) NOT NULL,
        [ModifiedBy] nvarchar(255) NULL,
        CONSTRAINT [PK_Zentra_Roles] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE TABLE [Zentra_SecurityQuestions] (
        [Id] uniqueidentifier NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        [CreatedBy] nvarchar(255) NOT NULL,
        [ModifiedBy] nvarchar(255) NULL,
        [RowVersion] rowversion NULL,
        [Question] nvarchar(255) NOT NULL,
        CONSTRAINT [PK_Zentra_SecurityQuestions] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE TABLE [Zentra_SecurityTokens] (
        [Id] uniqueidentifier NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        [CreatedBy] nvarchar(255) NOT NULL,
        [ModifiedBy] nvarchar(255) NULL,
        [Key] nvarchar(max) NULL,
        [TokenType] nvarchar(max) NULL,
        [TokenValue] nvarchar(max) NULL,
        [ClientId] nvarchar(max) NULL,
        [SessionId] nvarchar(max) NULL,
        [SubjectId] nvarchar(max) NULL,
        [CreationTime] datetime2 NOT NULL,
        [ExpiresAt] int NOT NULL,
        [ConsumedTime] datetime2 NULL,
        [ConsumedAt] datetime2 NULL,
        [TokenReuseDetected] bit NOT NULL DEFAULT CAST(0 AS bit),
        CONSTRAINT [PK_Zentra_SecurityTokens] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE TABLE [Zentra_Users] (
        [Id] uniqueidentifier NOT NULL,
        [UserName] nvarchar(255) NOT NULL,
        [NormalizedUserName] nvarchar(255) NOT NULL,
        [Email] nvarchar(255) NOT NULL,
        [NormalizedEmail] nvarchar(255) NOT NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [SecurityStamp] nvarchar(255) NULL,
        [ConcurrencyStamp] nvarchar(255) NULL,
        [PhoneNumber] nvarchar(15) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        [FirstName] nvarchar(255) NOT NULL,
        [LastName] nvarchar(255) NULL,
        [DateOfBirth] datetime2 NULL,
        [TwoFactorType] int NOT NULL,
        [LastPasswordChangedDate] datetime2 NULL,
        [RequiresDefaultPasswordChange] bit NULL,
        [LastLoginDateTime] datetime2 NULL,
        [LastLogoutDateTime] datetime2 NULL,
        [IdentityProviderType] int NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        [CreatedBy] nvarchar(255) NOT NULL,
        [ModifiedBy] nvarchar(255) NULL,
        CONSTRAINT [PK_Zentra_Users] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE TABLE [Zentra_ApiResourceClaims] (
        [Id] uniqueidentifier NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        [CreatedBy] nvarchar(255) NOT NULL,
        [ModifiedBy] nvarchar(255) NULL,
        [RowVersion] rowversion NULL,
        [ApiResourceId] uniqueidentifier NOT NULL,
        [Type] nvarchar(255) NOT NULL,
        CONSTRAINT [PK_Zentra_ApiResourceClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Zentra_ApiResourceClaims_Zentra_ApiResources_ApiResourceId] FOREIGN KEY ([ApiResourceId]) REFERENCES [Zentra_ApiResources] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE TABLE [Zentra_ApiScopes] (
        [Id] uniqueidentifier NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        [CreatedBy] nvarchar(255) NOT NULL,
        [ModifiedBy] nvarchar(255) NULL,
        [RowVersion] rowversion NULL,
        [ApiResourceId] uniqueidentifier NOT NULL,
        [Name] nvarchar(255) NOT NULL,
        [DisplayName] nvarchar(255) NULL,
        [Description] nvarchar(max) NULL,
        [Required] bit NOT NULL,
        [Emphasize] bit NOT NULL,
        CONSTRAINT [PK_Zentra_ApiScopes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Zentra_ApiScopes_Zentra_ApiResources_ApiResourceId] FOREIGN KEY ([ApiResourceId]) REFERENCES [Zentra_ApiResources] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE TABLE [Zentra_ClientPostLogoutRedirectUris] (
        [Id] uniqueidentifier NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        [CreatedBy] nvarchar(255) NOT NULL,
        [ModifiedBy] nvarchar(255) NULL,
        [RowVersion] rowversion NULL,
        [ClientId] uniqueidentifier NOT NULL,
        [PostLogoutRedirectUri] nvarchar(510) NOT NULL,
        CONSTRAINT [PK_Zentra_ClientPostLogoutRedirectUris] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Zentra_ClientPostLogoutRedirectUris_Zentra_Clients_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [Zentra_Clients] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE TABLE [Zentra_ClientRedirectUris] (
        [Id] uniqueidentifier NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        [CreatedBy] nvarchar(255) NOT NULL,
        [ModifiedBy] nvarchar(255) NULL,
        [RowVersion] rowversion NULL,
        [ClientId] uniqueidentifier NOT NULL,
        [RedirectUri] nvarchar(510) NOT NULL,
        CONSTRAINT [PK_Zentra_ClientRedirectUris] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Zentra_ClientRedirectUris_Zentra_Clients_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [Zentra_Clients] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE TABLE [Zentra_IdentityClaims] (
        [Id] uniqueidentifier NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        [CreatedBy] nvarchar(255) NOT NULL,
        [ModifiedBy] nvarchar(255) NULL,
        [RowVersion] rowversion NULL,
        [IdentityResourceId] uniqueidentifier NOT NULL,
        [Type] nvarchar(255) NOT NULL,
        [AliasType] nvarchar(255) NULL,
        CONSTRAINT [PK_Zentra_IdentityClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Zentra_IdentityClaims_Zentra_IdentityResources_IdentityResourceId] FOREIGN KEY ([IdentityResourceId]) REFERENCES [Zentra_IdentityResources] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE TABLE [Zentra_RoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] uniqueidentifier NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        [CreatedBy] nvarchar(255) NOT NULL,
        [ModifiedBy] nvarchar(255) NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Zentra_RoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Zentra_RoleClaims_Zentra_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Zentra_Roles] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE TABLE [Zentra_Notification] (
        [Id] uniqueidentifier NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        [CreatedBy] nvarchar(255) NOT NULL,
        [ModifiedBy] nvarchar(255) NULL,
        [UserId] uniqueidentifier NOT NULL,
        [MessageId] nvarchar(255) NOT NULL,
        [Type] int NOT NULL,
        [Activity] nvarchar(255) NULL,
        [Status] int NOT NULL,
        [Sender] nvarchar(255) NOT NULL,
        [Recipient] nvarchar(255) NOT NULL,
        CONSTRAINT [PK_Zentra_Notification] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Zentra_Notification_Zentra_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Zentra_Users] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE TABLE [Zentra_PasswordHistory] (
        [Id] uniqueidentifier NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] nvarchar(255) NOT NULL,
        [UserID] uniqueidentifier NOT NULL,
        [ChangedOn] datetime2 NOT NULL,
        [PasswordHash] nvarchar(255) NOT NULL,
        CONSTRAINT [PK_Zentra_PasswordHistory] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Zentra_PasswordHistory_Zentra_Users_UserID] FOREIGN KEY ([UserID]) REFERENCES [Zentra_Users] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE TABLE [Zentra_UserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] uniqueidentifier NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        [IsAdminClaim] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        [CreatedBy] nvarchar(255) NOT NULL,
        [ModifiedBy] nvarchar(255) NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Zentra_UserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Zentra_UserClaims_Zentra_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Zentra_Users] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE TABLE [Zentra_UserLogins] (
        [LoginProvider] nvarchar(256) NOT NULL,
        [ProviderKey] nvarchar(256) NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [Id] uniqueidentifier NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        [CreatedBy] nvarchar(255) NOT NULL,
        [ModifiedBy] nvarchar(255) NULL,
        CONSTRAINT [PK_Zentra_UserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey], [UserId]),
        CONSTRAINT [FK_Zentra_UserLogins_Zentra_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Zentra_Users] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE TABLE [Zentra_UserRoles] (
        [UserId] uniqueidentifier NOT NULL,
        [RoleId] uniqueidentifier NOT NULL,
        [Id] uniqueidentifier NOT NULL,
        [ValidFrom] datetime2 NULL,
        [ValidTo] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        [CreatedBy] nvarchar(255) NOT NULL,
        [ModifiedBy] nvarchar(255) NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Zentra_UserRoles] PRIMARY KEY ([Id], [UserId], [RoleId]),
        CONSTRAINT [FK_Zentra_UserRoles_Zentra_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Zentra_Roles] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Zentra_UserRoles_Zentra_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Zentra_Users] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE TABLE [Zentra_UserSecurityQuestions] (
        [Id] uniqueidentifier NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        [CreatedBy] nvarchar(255) NOT NULL,
        [ModifiedBy] nvarchar(255) NULL,
        [RowVersion] rowversion NULL,
        [UserId] uniqueidentifier NOT NULL,
        [SecurityQuestionId] uniqueidentifier NOT NULL,
        [Answer] nvarchar(255) NOT NULL,
        CONSTRAINT [PK_Zentra_UserSecurityQuestions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Zentra_UserSecurityQuestions_Zentra_SecurityQuestions_SecurityQuestionId] FOREIGN KEY ([SecurityQuestionId]) REFERENCES [Zentra_SecurityQuestions] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Zentra_UserSecurityQuestions_Zentra_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Zentra_Users] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE TABLE [Zentra_UserTokens] (
        [UserId] uniqueidentifier NOT NULL,
        [LoginProvider] nvarchar(255) NOT NULL,
        [Name] nvarchar(255) NOT NULL,
        [Value] nvarchar(max) NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Zentra_UserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_Zentra_UserTokens_Zentra_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Zentra_Users] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE TABLE [Zentra_ApiScopeClaims] (
        [Id] uniqueidentifier NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        [CreatedBy] nvarchar(255) NOT NULL,
        [ModifiedBy] nvarchar(255) NULL,
        [RowVersion] rowversion NULL,
        [ApiScopeId] uniqueidentifier NOT NULL,
        [Type] nvarchar(255) NOT NULL,
        CONSTRAINT [PK_Zentra_ApiScopeClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Zentra_ApiScopeClaims_Zentra_ApiScopes_ApiScopeId] FOREIGN KEY ([ApiScopeId]) REFERENCES [Zentra_ApiScopes] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE UNIQUE INDEX [IX_APIRES_CLM_RESID_TYPE] ON [Zentra_ApiResourceClaims] ([ApiResourceId], [Type]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE UNIQUE INDEX [IX_APIRES_NAME] ON [Zentra_ApiResources] ([Name]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE UNIQUE INDEX [IX_APISCO_CLM_SCOID_TYPE] ON [Zentra_ApiScopeClaims] ([ApiScopeId], [Type]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE UNIQUE INDEX [IX_APISCO_SCOID_NAME] ON [Zentra_ApiScopes] ([ApiResourceId], [Name]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE INDEX [IX_AUD_CBBY_ACTY] ON [Zentra_AuditTrail] ([CreatedBy], [ActionType]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE INDEX [IX_AUD_CRON_ACTY] ON [Zentra_AuditTrail] ([CreatedOn], [ActionType]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE INDEX [IX_AUD_CRON_CBBY] ON [Zentra_AuditTrail] ([CreatedOn], [CreatedBy]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE UNIQUE INDEX [IX_Zentra_ClientPostLogoutRedirectUris_ClientId_PostLogoutRedirectUri] ON [Zentra_ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE UNIQUE INDEX [IX_Zentra_ClientRedirectUris_ClientId_RedirectUri] ON [Zentra_ClientRedirectUris] ([ClientId], [RedirectUri]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE INDEX [IX_SECTOK_TOKTYPE_KEY] ON [Zentra_SecurityTokens] ([TokenType], [Key]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE UNIQUE INDEX [IX_CLI_CLID_CLSEC] ON [Zentra_Clients] ([ClientId], [ClientSecret]) WHERE [ClientId] IS NOT NULL AND [ClientSecret] IS NOT NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE UNIQUE INDEX [IX_IDRESCLM_IDRESID_TYPE] ON [Zentra_IdentityClaims] ([IdentityResourceId], [Type]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE UNIQUE INDEX [IX_IDRES_NAME] ON [Zentra_IdentityResources] ([Name]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE INDEX [IX_NOTI_TYPE] ON [Zentra_Notification] ([Type]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE INDEX [IX_Zentra_Notification_UserId] ON [Zentra_Notification] ([UserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE INDEX [IX_Zentra_PasswordHistory_UserID] ON [Zentra_PasswordHistory] ([UserID]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE INDEX [IX_Zentra_RoleClaims_RoleId] ON [Zentra_RoleClaims] ([RoleId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE UNIQUE INDEX [RoleNameIndex] ON [Zentra_Roles] ([NormalizedName]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE UNIQUE INDEX [IX_SEC_QUESTION] ON [Zentra_SecurityQuestions] ([Question]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE INDEX [IX_Zentra_UserClaims_UserId] ON [Zentra_UserClaims] ([UserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE INDEX [IX_Zentra_UserLogins_UserId] ON [Zentra_UserLogins] ([UserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE INDEX [IX_Zentra_UserRoles_RoleId] ON [Zentra_UserRoles] ([RoleId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE INDEX [IX_Zentra_UserRoles_UserId] ON [Zentra_UserRoles] ([UserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE INDEX [EmailIndex] ON [Zentra_Users] ([NormalizedEmail]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE UNIQUE INDEX [UserNameIndex] ON [Zentra_Users] ([NormalizedUserName]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE INDEX [IX_USRSEC_QUEID] ON [Zentra_UserSecurityQuestions] ([SecurityQuestionId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    CREATE UNIQUE INDEX [IX_USRSEC_UID_QUEID] ON [Zentra_UserSecurityQuestions] ([UserId], [SecurityQuestionId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_ZentraSqlV1')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220722123632_ZentraSqlV1', N'3.1.27');
END;

GO

