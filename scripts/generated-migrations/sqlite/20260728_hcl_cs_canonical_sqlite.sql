CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;

CREATE TABLE "HclCs_ApiResources" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_HclCs_ApiResources" PRIMARY KEY,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL,
    "RowVersion" BLOB NULL,
    "Name" TEXT NOT NULL,
    "DisplayName" TEXT NULL,
    "Description" TEXT NULL,
    "Enabled" INTEGER NOT NULL
);

CREATE TABLE "HclCs_AuditTrail" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_HclCs_AuditTrail" PRIMARY KEY,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ActionType" INTEGER NOT NULL,
    "TableName" TEXT NULL,
    "OldValue" TEXT NULL,
    "NewValue" TEXT NULL,
    "AffectedColumn" TEXT NULL,
    "ActionName" TEXT NULL
);

CREATE TABLE "HclCs_Clients" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_HclCs_Clients" PRIMARY KEY,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL,
    "RowVersion" BLOB NULL,
    "ClientId" TEXT NOT NULL,
    "ClientName" TEXT NULL,
    "ClientUri" TEXT NULL,
    "ClientIdIssuedAt" INTEGER NOT NULL,
    "ClientSecretExpiresAt" INTEGER NOT NULL,
    "ClientSecret" TEXT NULL,
    "LogoUri" TEXT NULL,
    "TermsOfServiceUri" TEXT NULL,
    "PolicyUri" TEXT NULL,
    "RefreshTokenExpiration" INTEGER NOT NULL,
    "AccessTokenExpiration" INTEGER NOT NULL,
    "IdentityTokenExpiration" INTEGER NOT NULL,
    "LogoutTokenExpiration" INTEGER NOT NULL,
    "AuthorizationCodeExpiration" INTEGER NOT NULL,
    "AccessTokenType" INTEGER NOT NULL,
    "RequirePkce" INTEGER NOT NULL,
    "IsPkceTextPlain" INTEGER NOT NULL,
    "RequireClientSecret" INTEGER NOT NULL,
    "IsFirstPartyApp" INTEGER NOT NULL,
    "AllowOfflineAccess" INTEGER NOT NULL,
    "AllowedScopes" TEXT NULL,
    "AllowAccessTokensViaBrowser" INTEGER NOT NULL,
    "ApplicationType" INTEGER NOT NULL,
    "AllowedSigningAlgorithm" TEXT NULL,
    "SupportedGrantTypes" TEXT NULL,
    "SupportedResponseTypes" TEXT NULL,
    "FrontChannelLogoutSessionRequired" INTEGER NOT NULL,
    "FrontChannelLogoutUri" TEXT NULL,
    "BackChannelLogoutSessionRequired" INTEGER NOT NULL,
    "BackChannelLogoutUri" TEXT NULL
);

CREATE TABLE "HclCs_IdentityResources" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_HclCs_IdentityResources" PRIMARY KEY,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL,
    "RowVersion" BLOB NULL,
    "Name" TEXT NOT NULL,
    "DisplayName" TEXT NULL,
    "Description" TEXT NULL,
    "Enabled" INTEGER NOT NULL,
    "Required" INTEGER NOT NULL,
    "Emphasize" INTEGER NOT NULL
);

CREATE TABLE "HclCs_Roles" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_HclCs_Roles" PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "NormalizedName" TEXT NOT NULL,
    "ConcurrencyStamp" TEXT NOT NULL,
    "Description" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL
);

CREATE TABLE "HclCs_SecurityQuestions" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_HclCs_SecurityQuestions" PRIMARY KEY,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL,
    "RowVersion" BLOB NULL,
    "Question" TEXT NOT NULL
);

CREATE TABLE "HclCs_SecurityTokens" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_HclCs_SecurityTokens" PRIMARY KEY,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL,
    "Key" TEXT NULL,
    "TokenType" TEXT NULL,
    "TokenValue" TEXT NULL,
    "ClientId" TEXT NULL,
    "SessionId" TEXT NULL,
    "SubjectId" TEXT NULL,
    "CreationTime" TEXT NOT NULL,
    "ExpiresAt" INTEGER NOT NULL,
    "ConsumedTime" TEXT NULL
);

CREATE TABLE "HclCs_Users" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_HclCs_Users" PRIMARY KEY,
    "UserName" TEXT NOT NULL,
    "NormalizedUserName" TEXT NOT NULL,
    "Email" TEXT NOT NULL,
    "NormalizedEmail" TEXT NOT NULL,
    "EmailConfirmed" INTEGER NOT NULL,
    "PasswordHash" TEXT NOT NULL,
    "SecurityStamp" TEXT NULL,
    "ConcurrencyStamp" TEXT NULL,
    "PhoneNumber" TEXT NULL,
    "PhoneNumberConfirmed" INTEGER NOT NULL,
    "TwoFactorEnabled" INTEGER NOT NULL,
    "LockoutEnd" TEXT NULL,
    "LockoutEnabled" INTEGER NOT NULL,
    "AccessFailedCount" INTEGER NOT NULL,
    "FirstName" TEXT NOT NULL,
    "LastName" TEXT NULL,
    "DateOfBirth" TEXT NULL,
    "TwoFactorType" INTEGER NOT NULL,
    "LastPasswordChangedDate" TEXT NULL,
    "RequiresDefaultPasswordChange" INTEGER NULL,
    "LastLoginDateTime" TEXT NULL,
    "LastLogoutDateTime" TEXT NULL,
    "IdentityProviderType" INTEGER NOT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL
);

CREATE TABLE "HclCs_ApiResourceClaims" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_HclCs_ApiResourceClaims" PRIMARY KEY,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL,
    "RowVersion" BLOB NULL,
    "ApiResourceId" TEXT NOT NULL,
    "Type" TEXT NOT NULL,
    CONSTRAINT "FK_HclCs_ApiResourceClaims_HclCs_ApiResources_ApiResourceId" FOREIGN KEY ("ApiResourceId") REFERENCES "HclCs_ApiResources" ("Id") ON DELETE CASCADE
);

CREATE TABLE "HclCs_ApiScopes" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_HclCs_ApiScopes" PRIMARY KEY,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL,
    "RowVersion" BLOB NULL,
    "ApiResourceId" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "DisplayName" TEXT NULL,
    "Description" TEXT NULL,
    "Required" INTEGER NOT NULL,
    "Emphasize" INTEGER NOT NULL,
    CONSTRAINT "FK_HclCs_ApiScopes_HclCs_ApiResources_ApiResourceId" FOREIGN KEY ("ApiResourceId") REFERENCES "HclCs_ApiResources" ("Id") ON DELETE CASCADE
);

CREATE TABLE "HclCs_ClientPostLogoutRedirectUris" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_HclCs_ClientPostLogoutRedirectUris" PRIMARY KEY,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL,
    "RowVersion" BLOB NULL,
    "ClientId" TEXT NOT NULL,
    "PostLogoutRedirectUri" TEXT NOT NULL,
    CONSTRAINT "FK_HclCs_ClientPostLogoutRedirectUris_HclCs_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "HclCs_Clients" ("Id") ON DELETE CASCADE
);

CREATE TABLE "HclCs_ClientRedirectUris" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_HclCs_ClientRedirectUris" PRIMARY KEY,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL,
    "RowVersion" BLOB NULL,
    "ClientId" TEXT NOT NULL,
    "RedirectUri" TEXT NOT NULL,
    CONSTRAINT "FK_HclCs_ClientRedirectUris_HclCs_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "HclCs_Clients" ("Id") ON DELETE CASCADE
);

CREATE TABLE "HclCs_IdentityClaims" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_HclCs_IdentityClaims" PRIMARY KEY,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL,
    "RowVersion" BLOB NULL,
    "IdentityResourceId" TEXT NOT NULL,
    "Type" TEXT NOT NULL,
    "AliasType" TEXT NULL,
    CONSTRAINT "FK_HclCs_IdentityClaims_HclCs_IdentityResources_IdentityResourceId" FOREIGN KEY ("IdentityResourceId") REFERENCES "HclCs_IdentityResources" ("Id") ON DELETE CASCADE
);

CREATE TABLE "HclCs_RoleClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_HclCs_RoleClaims" PRIMARY KEY AUTOINCREMENT,
    "RoleId" TEXT NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL,
    "RowVersion" BLOB NULL,
    CONSTRAINT "FK_HclCs_RoleClaims_HclCs_Roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "HclCs_Roles" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "HclCs_Notification" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_HclCs_Notification" PRIMARY KEY,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL,
    "UserId" TEXT NOT NULL,
    "MessageId" TEXT NOT NULL,
    "Type" INTEGER NOT NULL,
    "Activity" TEXT NULL,
    "Status" INTEGER NOT NULL,
    "Sender" TEXT NOT NULL,
    "Recipient" TEXT NOT NULL,
    CONSTRAINT "FK_HclCs_Notification_HclCs_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "HclCs_Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "HclCs_PasswordHistory" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_HclCs_PasswordHistory" PRIMARY KEY,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "UserID" TEXT NOT NULL,
    "ChangedOn" TEXT NOT NULL,
    "PasswordHash" TEXT NOT NULL,
    CONSTRAINT "FK_HclCs_PasswordHistory_HclCs_Users_UserID" FOREIGN KEY ("UserID") REFERENCES "HclCs_Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "HclCs_UserClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_HclCs_UserClaims" PRIMARY KEY AUTOINCREMENT,
    "UserId" TEXT NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    "IsAdminClaim" INTEGER NOT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL,
    "RowVersion" BLOB NULL,
    CONSTRAINT "FK_HclCs_UserClaims_HclCs_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "HclCs_Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "HclCs_UserLogins" (
    "LoginProvider" TEXT NOT NULL,
    "ProviderKey" TEXT NOT NULL,
    "UserId" TEXT NOT NULL,
    "ProviderDisplayName" TEXT NULL,
    "Id" TEXT NOT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL,
    CONSTRAINT "PK_HclCs_UserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey", "UserId"),
    CONSTRAINT "FK_HclCs_UserLogins_HclCs_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "HclCs_Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "HclCs_UserRoles" (
    "UserId" TEXT NOT NULL,
    "RoleId" TEXT NOT NULL,
    "Id" TEXT NOT NULL,
    "ValidFrom" TEXT NULL,
    "ValidTo" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL,
    "RowVersion" BLOB NULL,
    CONSTRAINT "PK_HclCs_UserRoles" PRIMARY KEY ("Id", "UserId", "RoleId"),
    CONSTRAINT "FK_HclCs_UserRoles_HclCs_Roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "HclCs_Roles" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_HclCs_UserRoles_HclCs_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "HclCs_Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "HclCs_UserSecurityQuestions" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_HclCs_UserSecurityQuestions" PRIMARY KEY,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL,
    "RowVersion" BLOB NULL,
    "UserId" TEXT NOT NULL,
    "SecurityQuestionId" TEXT NOT NULL,
    "Answer" TEXT NOT NULL,
    CONSTRAINT "FK_HclCs_UserSecurityQuestions_HclCs_SecurityQuestions_SecurityQuestionId" FOREIGN KEY ("SecurityQuestionId") REFERENCES "HclCs_SecurityQuestions" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_HclCs_UserSecurityQuestions_HclCs_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "HclCs_Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "HclCs_UserTokens" (
    "UserId" TEXT NOT NULL,
    "LoginProvider" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Value" TEXT NOT NULL,
    "IsDeleted" INTEGER NOT NULL,
    CONSTRAINT "PK_HclCs_UserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),
    CONSTRAINT "FK_HclCs_UserTokens_HclCs_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "HclCs_Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "HclCs_ApiScopeClaims" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_HclCs_ApiScopeClaims" PRIMARY KEY,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL,
    "RowVersion" BLOB NULL,
    "ApiScopeId" TEXT NOT NULL,
    "Type" TEXT NOT NULL,
    CONSTRAINT "FK_HclCs_ApiScopeClaims_HclCs_ApiScopes_ApiScopeId" FOREIGN KEY ("ApiScopeId") REFERENCES "HclCs_ApiScopes" ("Id") ON DELETE CASCADE
);

CREATE UNIQUE INDEX "IX_APIRES_CLM_RESID_TYPE" ON "HclCs_ApiResourceClaims" ("ApiResourceId", "Type");

CREATE UNIQUE INDEX "IX_APIRES_NAME" ON "HclCs_ApiResources" ("Name");

CREATE UNIQUE INDEX "IX_APISCO_CLM_SCOID_TYPE" ON "HclCs_ApiScopeClaims" ("ApiScopeId", "Type");

CREATE UNIQUE INDEX "IX_APISCO_SCOID_NAME" ON "HclCs_ApiScopes" ("ApiResourceId", "Name");

CREATE INDEX "IX_AUD_CBBY_ACTY" ON "HclCs_AuditTrail" ("CreatedBy", "ActionType");

CREATE INDEX "IX_AUD_CRON_ACTY" ON "HclCs_AuditTrail" ("CreatedOn", "ActionType");

CREATE INDEX "IX_AUD_CRON_CBBY" ON "HclCs_AuditTrail" ("CreatedOn", "CreatedBy");

CREATE UNIQUE INDEX "IX_HclCs_ClientPostLogoutRedirectUris_ClientId_PostLogoutRedirectUri" ON "HclCs_ClientPostLogoutRedirectUris" ("ClientId", "PostLogoutRedirectUri");

CREATE UNIQUE INDEX "IX_HclCs_ClientRedirectUris_ClientId_RedirectUri" ON "HclCs_ClientRedirectUris" ("ClientId", "RedirectUri");

CREATE UNIQUE INDEX "IX_CLI_CLID_CLSEC" ON "HclCs_Clients" ("ClientId", "ClientSecret");

CREATE UNIQUE INDEX "IX_IDRESCLM_IDRESID_TYPE" ON "HclCs_IdentityClaims" ("IdentityResourceId", "Type");

CREATE UNIQUE INDEX "IX_IDRES_NAME" ON "HclCs_IdentityResources" ("Name");

CREATE INDEX "IX_NOTI_TYPE" ON "HclCs_Notification" ("Type");

CREATE INDEX "IX_HclCs_Notification_UserId" ON "HclCs_Notification" ("UserId");

CREATE INDEX "IX_HclCs_PasswordHistory_UserID" ON "HclCs_PasswordHistory" ("UserID");

CREATE INDEX "IX_HclCs_RoleClaims_RoleId" ON "HclCs_RoleClaims" ("RoleId");

CREATE UNIQUE INDEX "RoleNameIndex" ON "HclCs_Roles" ("NormalizedName");

CREATE UNIQUE INDEX "IX_SEC_QUESTION" ON "HclCs_SecurityQuestions" ("Question");

CREATE INDEX "IX_HclCs_UserClaims_UserId" ON "HclCs_UserClaims" ("UserId");

CREATE INDEX "IX_HclCs_UserLogins_UserId" ON "HclCs_UserLogins" ("UserId");

CREATE INDEX "IX_HclCs_UserRoles_RoleId" ON "HclCs_UserRoles" ("RoleId");

CREATE INDEX "IX_HclCs_UserRoles_UserId" ON "HclCs_UserRoles" ("UserId");

CREATE INDEX "EmailIndex" ON "HclCs_Users" ("NormalizedEmail");

CREATE UNIQUE INDEX "UserNameIndex" ON "HclCs_Users" ("NormalizedUserName");

CREATE INDEX "IX_USRSEC_QUEID" ON "HclCs_UserSecurityQuestions" ("SecurityQuestionId");

CREATE UNIQUE INDEX "IX_USRSEC_UID_QUEID" ON "HclCs_UserSecurityQuestions" ("UserId", "SecurityQuestionId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20220802110433_HclCsSqliteV1', '8.0.11');

COMMIT;

BEGIN TRANSACTION;


            CREATE TABLE IF NOT EXISTS "HclCs_ApiResources" (
                "Id" TEXT NOT NULL CONSTRAINT "PK_HclCs_ApiResources" PRIMARY KEY,
                "Name" TEXT NOT NULL,
                "DisplayName" TEXT NULL,
                "Description" TEXT NULL,
                "Enabled" INTEGER NOT NULL,
                "IsDeleted" INTEGER NOT NULL,
                "CreatedOn" TEXT NOT NULL,
                "ModifiedOn" TEXT NULL,
                "CreatedBy" TEXT NOT NULL,
                "ModifiedBy" TEXT NULL,
                "RowVersion" BLOB NULL DEFAULT (CURRENT_TIMESTAMP)
            );

            CREATE TABLE IF NOT EXISTS "HclCs_AuditTrail" (
                "Id" TEXT NOT NULL CONSTRAINT "PK_HclCs_AuditTrail" PRIMARY KEY,
                "ActionType" INTEGER NOT NULL,
                "TableName" TEXT NULL,
                "OldValue" TEXT NULL,
                "NewValue" TEXT NULL,
                "AffectedColumn" TEXT NULL,
                "ActionName" TEXT NULL,
                "CreatedOn" TEXT NOT NULL,
                "CreatedBy" TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS "HclCs_Clients" (
                "Id" TEXT NOT NULL CONSTRAINT "PK_HclCs_Clients" PRIMARY KEY,
                "ClientId" TEXT NOT NULL,
                "ClientName" TEXT NULL,
                "ClientUri" TEXT NULL,
                "ClientIdIssuedAt" INTEGER NOT NULL,
                "ClientSecretExpiresAt" INTEGER NOT NULL,
                "ClientSecret" TEXT NULL,
                "LogoUri" TEXT NULL,
                "TermsOfServiceUri" TEXT NULL,
                "PolicyUri" TEXT NULL,
                "RefreshTokenExpiration" INTEGER NOT NULL,
                "AccessTokenLifetime" INTEGER NOT NULL,
                "IdentityTokenLifetime" INTEGER NOT NULL,
                "AuthorizationCodeLifetime" INTEGER NOT NULL,
                "AbsoluteRefreshTokenLifetime" INTEGER NOT NULL,
                "SlidingRefreshTokenLifetime" INTEGER NOT NULL,
                "RequireClientSecret" INTEGER NOT NULL,
                "RequirePkce" INTEGER NOT NULL,
                "AllowPlainTextPkce" INTEGER NOT NULL,
                "AllowAccessTokensViaBrowser" INTEGER NOT NULL,
                "RequireConsent" INTEGER NOT NULL,
                "AllowOfflineAccess" INTEGER NOT NULL,
                "EnableLocalLogin" INTEGER NOT NULL,
                "IncludeJwtId" INTEGER NOT NULL,
                "AlwaysSendClientClaims" INTEGER NOT NULL,
                "AlwaysIncludeUserClaimsInIdToken" INTEGER NOT NULL,
                "FrontChannelLogoutUri" TEXT NULL,
                "FrontChannelLogoutSessionRequired" INTEGER NOT NULL,
                "BackChannelLogoutUri" TEXT NULL,
                "BackChannelLogoutSessionRequired" INTEGER NOT NULL,
                "Enabled" INTEGER NOT NULL,
                "IsDeleted" INTEGER NOT NULL,
                "CreatedOn" TEXT NOT NULL,
                "ModifiedOn" TEXT NULL,
                "CreatedBy" TEXT NOT NULL,
                "ModifiedBy" TEXT NULL,
                "RowVersion" BLOB NULL DEFAULT (CURRENT_TIMESTAMP)
            );
        

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20230428105853_HclCsSqliteV1', '8.0.11');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "HclCs_Users" ADD "DirectoryImmutableId" TEXT NULL;

ALTER TABLE "HclCs_Users" ADD "EmployeeId" TEXT NULL;

ALTER TABLE "HclCs_Users" ADD "UserPrincipalName" TEXT NULL;

ALTER TABLE "HclCs_Users" ADD "DisplayName" TEXT NULL;

ALTER TABLE "HclCs_Users" ADD "Department" TEXT NULL;

ALTER TABLE "HclCs_Users" ADD "AuthenticationSource" TEXT NULL;

ALTER TABLE "HclCs_Users" ADD "DirectoryLastValidatedAt" TEXT NULL;

UPDATE "HclCs_Users"
SET "AuthenticationSource" = CASE "IdentityProviderType"
    WHEN 1 THEN 'LOCAL'
    WHEN 2 THEN 'LDAP'
    WHEN 3 THEN 'GOOGLE'
    ELSE NULL
END
WHERE "AuthenticationSource" IS NULL;

CREATE INDEX "IX_USERS_EMPLOYEE_ID" ON "HclCs_Users" ("EmployeeId");

CREATE UNIQUE INDEX "UX_USERS_DIRECTORY_IMMUTABLE_ID" ON "HclCs_Users" ("DirectoryImmutableId");

CREATE UNIQUE INDEX "UX_USERS_USER_PRINCIPAL_NAME" ON "HclCs_Users" ("UserPrincipalName");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260726060000_Phase2HclIdentityProfile', '8.0.11');

COMMIT;

BEGIN TRANSACTION;

DROP INDEX "EmailIndex";

CREATE UNIQUE INDEX "EmailIndex" ON "HclCs_Users" ("NormalizedEmail");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260727130000_Phase2BLocalAuthenticationEmailUniqueness', '8.0.11');

COMMIT;

BEGIN TRANSACTION;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260728135000_FixSqliteBaselineDeduplication', '8.0.11');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "HclCs_SecurityTokens" ADD "ConsumedAt" TEXT NULL;

ALTER TABLE "HclCs_SecurityTokens" ADD "TokenReuseDetected" INTEGER NOT NULL DEFAULT 0;

ALTER TABLE "HclCs_Clients" ADD "PreferredAudience" TEXT NULL;

CREATE TABLE "HclCs_ExternalIdentities" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_HclCs_ExternalIdentities" PRIMARY KEY,
    "IsDeleted" INTEGER NOT NULL DEFAULT 0,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL,
    "RowVersion" BLOB NULL,
    "UserId" TEXT NOT NULL,
    "TenantId" TEXT NULL,
    "Provider" TEXT NOT NULL,
    "Issuer" TEXT NOT NULL,
    "Subject" TEXT NOT NULL,
    "Email" TEXT NOT NULL,
    "EmailVerified" INTEGER NOT NULL,
    "LinkedAt" TEXT NOT NULL,
    "LastSignInAt" TEXT NULL,
    CONSTRAINT "FK_HclCs_ExternalIdentities_HclCs_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "HclCs_Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "HclCs_NotificationProviderConfig" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_HclCs_NotificationProviderConfig" PRIMARY KEY,
    "IsDeleted" INTEGER NOT NULL DEFAULT 0,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL,
    "ProviderName" TEXT NOT NULL,
    "ChannelType" INTEGER NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "ConfigJson" TEXT NOT NULL,
    "LastTestedOn" TEXT NULL,
    "LastTestSuccess" INTEGER NULL
);

CREATE TABLE "HclCs_ExternalAuthProviderConfig" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_HclCs_ExternalAuthProviderConfig" PRIMARY KEY,
    "IsDeleted" INTEGER NOT NULL DEFAULT 0,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL,
    "ProviderName" TEXT NOT NULL,
    "ProviderType" INTEGER NOT NULL,
    "IsEnabled" INTEGER NOT NULL,
    "ConfigJson" TEXT NOT NULL,
    "AutoProvisionEnabled" INTEGER NOT NULL DEFAULT 0,
    "AllowedDomains" TEXT NULL,
    "LastTestedOn" TEXT NULL,
    "LastTestSuccess" INTEGER NULL
);

CREATE UNIQUE INDEX "IX_EXTID_PROVIDER_ISSUER_SUBJECT" ON "HclCs_ExternalIdentities" ("Provider", "Issuer", "Subject");

CREATE INDEX "IX_EXTID_USERID" ON "HclCs_ExternalIdentities" ("UserId");

CREATE INDEX "IX_EXTID_TENANT_EMAIL" ON "HclCs_ExternalIdentities" ("TenantId", "Email");

CREATE INDEX "IX_NPC_CHANNEL_TYPE" ON "HclCs_NotificationProviderConfig" ("ChannelType");

CREATE INDEX "IX_NPC_CHANNEL_ACTIVE" ON "HclCs_NotificationProviderConfig" ("ChannelType", "IsActive");

CREATE UNIQUE INDEX "IX_EAPC_PROVIDER" ON "HclCs_ExternalAuthProviderConfig" ("ProviderName");

CREATE INDEX "IX_EAPC_PROVIDER_ENABLED" ON "HclCs_ExternalAuthProviderConfig" ("ProviderName", "IsEnabled");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260728140000_Phase2CCoreInfrastructureSchema', '8.0.11');

COMMIT;

