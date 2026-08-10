/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE TABLE [HclCs_ApiResources] (
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
        CONSTRAINT [PK_HclCs_ApiResources] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE TABLE [HclCs_AuditTrail] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] nvarchar(255) NOT NULL,
        [ActionType] int NOT NULL,
        [TableName] nvarchar(255) NULL,
        [OldValue] nvarchar(max) NULL,
        [NewValue] nvarchar(max) NULL,
        [AffectedColumn] nvarchar(max) NULL,
        [ActionName] nvarchar(max) NULL,
        CONSTRAINT [PK_HclCs_AuditTrail] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE TABLE [HclCs_Clients] (
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
        CONSTRAINT [PK_HclCs_Clients] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE TABLE [HclCs_IdentityResources] (
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
        CONSTRAINT [PK_HclCs_IdentityResources] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE TABLE [HclCs_Roles] (
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
        CONSTRAINT [PK_HclCs_Roles] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE TABLE [HclCs_SecurityQuestions] (
        [Id] uniqueidentifier NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        [CreatedBy] nvarchar(255) NOT NULL,
        [ModifiedBy] nvarchar(255) NULL,
        [RowVersion] rowversion NULL,
        [Question] nvarchar(255) NOT NULL,
        CONSTRAINT [PK_HclCs_SecurityQuestions] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE TABLE [HclCs_SecurityTokens] (
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
        CONSTRAINT [PK_HclCs_SecurityTokens] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE TABLE [HclCs_Users] (
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
        CONSTRAINT [PK_HclCs_Users] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE TABLE [HclCs_ApiResourceClaims] (
        [Id] uniqueidentifier NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        [CreatedBy] nvarchar(255) NOT NULL,
        [ModifiedBy] nvarchar(255) NULL,
        [RowVersion] rowversion NULL,
        [ApiResourceId] uniqueidentifier NOT NULL,
        [Type] nvarchar(255) NOT NULL,
        CONSTRAINT [PK_HclCs_ApiResourceClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HclCs_ApiResourceClaims_HclCs_ApiResources_ApiResourceId] FOREIGN KEY ([ApiResourceId]) REFERENCES [HclCs_ApiResources] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE TABLE [HclCs_ApiScopes] (
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
        CONSTRAINT [PK_HclCs_ApiScopes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HclCs_ApiScopes_HclCs_ApiResources_ApiResourceId] FOREIGN KEY ([ApiResourceId]) REFERENCES [HclCs_ApiResources] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE TABLE [HclCs_ClientPostLogoutRedirectUris] (
        [Id] uniqueidentifier NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        [CreatedBy] nvarchar(255) NOT NULL,
        [ModifiedBy] nvarchar(255) NULL,
        [RowVersion] rowversion NULL,
        [ClientId] uniqueidentifier NOT NULL,
        [PostLogoutRedirectUri] nvarchar(510) NOT NULL,
        CONSTRAINT [PK_HclCs_ClientPostLogoutRedirectUris] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HclCs_ClientPostLogoutRedirectUris_HclCs_Clients_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [HclCs_Clients] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE TABLE [HclCs_ClientRedirectUris] (
        [Id] uniqueidentifier NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        [CreatedBy] nvarchar(255) NOT NULL,
        [ModifiedBy] nvarchar(255) NULL,
        [RowVersion] rowversion NULL,
        [ClientId] uniqueidentifier NOT NULL,
        [RedirectUri] nvarchar(510) NOT NULL,
        CONSTRAINT [PK_HclCs_ClientRedirectUris] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HclCs_ClientRedirectUris_HclCs_Clients_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [HclCs_Clients] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE TABLE [HclCs_IdentityClaims] (
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
        CONSTRAINT [PK_HclCs_IdentityClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HclCs_IdentityClaims_HclCs_IdentityResources_IdentityResourceId] FOREIGN KEY ([IdentityResourceId]) REFERENCES [HclCs_IdentityResources] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE TABLE [HclCs_RoleClaims] (
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
        CONSTRAINT [PK_HclCs_RoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HclCs_RoleClaims_HclCs_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [HclCs_Roles] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE TABLE [HclCs_Notification] (
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
        CONSTRAINT [PK_HclCs_Notification] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HclCs_Notification_HclCs_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [HclCs_Users] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE TABLE [HclCs_PasswordHistory] (
        [Id] uniqueidentifier NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] nvarchar(255) NOT NULL,
        [UserID] uniqueidentifier NOT NULL,
        [ChangedOn] datetime2 NOT NULL,
        [PasswordHash] nvarchar(255) NOT NULL,
        CONSTRAINT [PK_HclCs_PasswordHistory] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HclCs_PasswordHistory_HclCs_Users_UserID] FOREIGN KEY ([UserID]) REFERENCES [HclCs_Users] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE TABLE [HclCs_UserClaims] (
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
        CONSTRAINT [PK_HclCs_UserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HclCs_UserClaims_HclCs_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [HclCs_Users] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE TABLE [HclCs_UserLogins] (
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
        CONSTRAINT [PK_HclCs_UserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey], [UserId]),
        CONSTRAINT [FK_HclCs_UserLogins_HclCs_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [HclCs_Users] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE TABLE [HclCs_UserRoles] (
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
        CONSTRAINT [PK_HclCs_UserRoles] PRIMARY KEY ([Id], [UserId], [RoleId]),
        CONSTRAINT [FK_HclCs_UserRoles_HclCs_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [HclCs_Roles] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HclCs_UserRoles_HclCs_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [HclCs_Users] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE TABLE [HclCs_UserSecurityQuestions] (
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
        CONSTRAINT [PK_HclCs_UserSecurityQuestions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HclCs_UserSecurityQuestions_HclCs_SecurityQuestions_SecurityQuestionId] FOREIGN KEY ([SecurityQuestionId]) REFERENCES [HclCs_SecurityQuestions] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HclCs_UserSecurityQuestions_HclCs_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [HclCs_Users] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE TABLE [HclCs_UserTokens] (
        [UserId] uniqueidentifier NOT NULL,
        [LoginProvider] nvarchar(255) NOT NULL,
        [Name] nvarchar(255) NOT NULL,
        [Value] nvarchar(max) NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_HclCs_UserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_HclCs_UserTokens_HclCs_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [HclCs_Users] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE TABLE [HclCs_ApiScopeClaims] (
        [Id] uniqueidentifier NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NULL,
        [CreatedBy] nvarchar(255) NOT NULL,
        [ModifiedBy] nvarchar(255) NULL,
        [RowVersion] rowversion NULL,
        [ApiScopeId] uniqueidentifier NOT NULL,
        [Type] nvarchar(255) NOT NULL,
        CONSTRAINT [PK_HclCs_ApiScopeClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HclCs_ApiScopeClaims_HclCs_ApiScopes_ApiScopeId] FOREIGN KEY ([ApiScopeId]) REFERENCES [HclCs_ApiScopes] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE UNIQUE INDEX [IX_APIRES_CLM_RESID_TYPE] ON [HclCs_ApiResourceClaims] ([ApiResourceId], [Type]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE UNIQUE INDEX [IX_APIRES_NAME] ON [HclCs_ApiResources] ([Name]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE UNIQUE INDEX [IX_APISCO_CLM_SCOID_TYPE] ON [HclCs_ApiScopeClaims] ([ApiScopeId], [Type]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE UNIQUE INDEX [IX_APISCO_SCOID_NAME] ON [HclCs_ApiScopes] ([ApiResourceId], [Name]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE INDEX [IX_AUD_CBBY_ACTY] ON [HclCs_AuditTrail] ([CreatedBy], [ActionType]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE INDEX [IX_AUD_CRON_ACTY] ON [HclCs_AuditTrail] ([CreatedOn], [ActionType]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE INDEX [IX_AUD_CRON_CBBY] ON [HclCs_AuditTrail] ([CreatedOn], [CreatedBy]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE UNIQUE INDEX [IX_HclCs_ClientPostLogoutRedirectUris_ClientId_PostLogoutRedirectUri] ON [HclCs_ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE UNIQUE INDEX [IX_HclCs_ClientRedirectUris_ClientId_RedirectUri] ON [HclCs_ClientRedirectUris] ([ClientId], [RedirectUri]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE INDEX [IX_SECTOK_TOKTYPE_KEY] ON [HclCs_SecurityTokens] ([TokenType], [Key]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE UNIQUE INDEX [IX_CLI_CLID_CLSEC] ON [HclCs_Clients] ([ClientId], [ClientSecret]) WHERE [ClientId] IS NOT NULL AND [ClientSecret] IS NOT NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE UNIQUE INDEX [IX_IDRESCLM_IDRESID_TYPE] ON [HclCs_IdentityClaims] ([IdentityResourceId], [Type]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE UNIQUE INDEX [IX_IDRES_NAME] ON [HclCs_IdentityResources] ([Name]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE INDEX [IX_NOTI_TYPE] ON [HclCs_Notification] ([Type]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE INDEX [IX_HclCs_Notification_UserId] ON [HclCs_Notification] ([UserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE INDEX [IX_HclCs_PasswordHistory_UserID] ON [HclCs_PasswordHistory] ([UserID]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE INDEX [IX_HclCs_RoleClaims_RoleId] ON [HclCs_RoleClaims] ([RoleId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE UNIQUE INDEX [RoleNameIndex] ON [HclCs_Roles] ([NormalizedName]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE UNIQUE INDEX [IX_SEC_QUESTION] ON [HclCs_SecurityQuestions] ([Question]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE INDEX [IX_HclCs_UserClaims_UserId] ON [HclCs_UserClaims] ([UserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE INDEX [IX_HclCs_UserLogins_UserId] ON [HclCs_UserLogins] ([UserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE INDEX [IX_HclCs_UserRoles_RoleId] ON [HclCs_UserRoles] ([RoleId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE INDEX [IX_HclCs_UserRoles_UserId] ON [HclCs_UserRoles] ([UserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE INDEX [EmailIndex] ON [HclCs_Users] ([NormalizedEmail]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE UNIQUE INDEX [UserNameIndex] ON [HclCs_Users] ([NormalizedUserName]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE INDEX [IX_USRSEC_QUEID] ON [HclCs_UserSecurityQuestions] ([SecurityQuestionId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    CREATE UNIQUE INDEX [IX_USRSEC_UID_QUEID] ON [HclCs_UserSecurityQuestions] ([UserId], [SecurityQuestionId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220722123632_HclCsSqlV1')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220722123632_HclCsSqlV1', N'3.1.27');
END;

GO

