CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

CREATE TABLE "Zentra_ApiResources" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Zentra_ApiResources" PRIMARY KEY,
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

CREATE TABLE "Zentra_AuditTrail" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Zentra_AuditTrail" PRIMARY KEY,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ActionType" INTEGER NOT NULL,
    "TableName" TEXT NULL,
    "OldValue" TEXT NULL,
    "NewValue" TEXT NULL,
    "AffectedColumn" TEXT NULL,
    "ActionName" TEXT NULL
);

CREATE TABLE "Zentra_Clients" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Zentra_Clients" PRIMARY KEY,
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

CREATE TABLE "Zentra_IdentityResources" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Zentra_IdentityResources" PRIMARY KEY,
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

CREATE TABLE "Zentra_Roles" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Zentra_Roles" PRIMARY KEY,
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

CREATE TABLE "Zentra_SecurityQuestions" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Zentra_SecurityQuestions" PRIMARY KEY,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL,
    "RowVersion" BLOB NULL,
    "Question" TEXT NOT NULL
);

CREATE TABLE "Zentra_SecurityTokens" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Zentra_SecurityTokens" PRIMARY KEY,
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
    "ConsumedTime" TEXT NULL,
    "ConsumedAt" TEXT NULL,
    "TokenReuseDetected" INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE "Zentra_Users" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Zentra_Users" PRIMARY KEY,
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

CREATE TABLE "Zentra_ExternalIdentities" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Zentra_ExternalIdentities" PRIMARY KEY,
    "IsDeleted" INTEGER NOT NULL,
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
    CONSTRAINT "FK_Zentra_ExternalIdentities_Zentra_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Zentra_Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Zentra_ApiResourceClaims" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Zentra_ApiResourceClaims" PRIMARY KEY,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL,
    "RowVersion" BLOB NULL,
    "ApiResourceId" TEXT NOT NULL,
    "Type" TEXT NOT NULL,
    CONSTRAINT "FK_Zentra_ApiResourceClaims_Zentra_ApiResources_ApiResourceId" FOREIGN KEY ("ApiResourceId") REFERENCES "Zentra_ApiResources" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Zentra_ApiScopes" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Zentra_ApiScopes" PRIMARY KEY,
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
    CONSTRAINT "FK_Zentra_ApiScopes_Zentra_ApiResources_ApiResourceId" FOREIGN KEY ("ApiResourceId") REFERENCES "Zentra_ApiResources" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Zentra_ClientPostLogoutRedirectUris" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Zentra_ClientPostLogoutRedirectUris" PRIMARY KEY,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL,
    "RowVersion" BLOB NULL,
    "ClientId" TEXT NOT NULL,
    "PostLogoutRedirectUri" TEXT NOT NULL,
    CONSTRAINT "FK_Zentra_ClientPostLogoutRedirectUris_Zentra_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Zentra_Clients" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Zentra_ClientRedirectUris" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Zentra_ClientRedirectUris" PRIMARY KEY,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL,
    "RowVersion" BLOB NULL,
    "ClientId" TEXT NOT NULL,
    "RedirectUri" TEXT NOT NULL,
    CONSTRAINT "FK_Zentra_ClientRedirectUris_Zentra_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Zentra_Clients" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Zentra_IdentityClaims" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Zentra_IdentityClaims" PRIMARY KEY,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL,
    "RowVersion" BLOB NULL,
    "IdentityResourceId" TEXT NOT NULL,
    "Type" TEXT NOT NULL,
    "AliasType" TEXT NULL,
    CONSTRAINT "FK_Zentra_IdentityClaims_Zentra_IdentityResources_IdentityResourceId" FOREIGN KEY ("IdentityResourceId") REFERENCES "Zentra_IdentityResources" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Zentra_RoleClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Zentra_RoleClaims" PRIMARY KEY AUTOINCREMENT,
    "RoleId" TEXT NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL,
    "RowVersion" BLOB NULL,
    CONSTRAINT "FK_Zentra_RoleClaims_Zentra_Roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "Zentra_Roles" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Zentra_Notification" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Zentra_Notification" PRIMARY KEY,
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
    CONSTRAINT "FK_Zentra_Notification_Zentra_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Zentra_Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Zentra_PasswordHistory" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Zentra_PasswordHistory" PRIMARY KEY,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "UserID" TEXT NOT NULL,
    "ChangedOn" TEXT NOT NULL,
    "PasswordHash" TEXT NOT NULL,
    CONSTRAINT "FK_Zentra_PasswordHistory_Zentra_Users_UserID" FOREIGN KEY ("UserID") REFERENCES "Zentra_Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Zentra_UserClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Zentra_UserClaims" PRIMARY KEY AUTOINCREMENT,
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
    CONSTRAINT "FK_Zentra_UserClaims_Zentra_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Zentra_Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Zentra_UserLogins" (
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
    CONSTRAINT "PK_Zentra_UserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey", "UserId"),
    CONSTRAINT "FK_Zentra_UserLogins_Zentra_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Zentra_Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Zentra_UserRoles" (
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
    CONSTRAINT "PK_Zentra_UserRoles" PRIMARY KEY ("Id", "UserId", "RoleId"),
    CONSTRAINT "FK_Zentra_UserRoles_Zentra_Roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "Zentra_Roles" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Zentra_UserRoles_Zentra_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Zentra_Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Zentra_UserSecurityQuestions" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Zentra_UserSecurityQuestions" PRIMARY KEY,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL,
    "RowVersion" BLOB NULL,
    "UserId" TEXT NOT NULL,
    "SecurityQuestionId" TEXT NOT NULL,
    "Answer" TEXT NOT NULL,
    CONSTRAINT "FK_Zentra_UserSecurityQuestions_Zentra_SecurityQuestions_SecurityQuestionId" FOREIGN KEY ("SecurityQuestionId") REFERENCES "Zentra_SecurityQuestions" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Zentra_UserSecurityQuestions_Zentra_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Zentra_Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Zentra_UserTokens" (
    "UserId" TEXT NOT NULL,
    "LoginProvider" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Value" TEXT NOT NULL,
    "IsDeleted" INTEGER NOT NULL,
    CONSTRAINT "PK_Zentra_UserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),
    CONSTRAINT "FK_Zentra_UserTokens_Zentra_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Zentra_Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Zentra_ApiScopeClaims" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Zentra_ApiScopeClaims" PRIMARY KEY,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL,
    "RowVersion" BLOB NULL,
    "ApiScopeId" TEXT NOT NULL,
    "Type" TEXT NOT NULL,
    CONSTRAINT "FK_Zentra_ApiScopeClaims_Zentra_ApiScopes_ApiScopeId" FOREIGN KEY ("ApiScopeId") REFERENCES "Zentra_ApiScopes" ("Id") ON DELETE CASCADE
);

CREATE UNIQUE INDEX "IX_APIRES_CLM_RESID_TYPE" ON "Zentra_ApiResourceClaims" ("ApiResourceId", "Type");

CREATE UNIQUE INDEX "IX_APIRES_NAME" ON "Zentra_ApiResources" ("Name");

CREATE UNIQUE INDEX "IX_APISCO_CLM_SCOID_TYPE" ON "Zentra_ApiScopeClaims" ("ApiScopeId", "Type");

CREATE UNIQUE INDEX "IX_APISCO_SCOID_NAME" ON "Zentra_ApiScopes" ("ApiResourceId", "Name");

CREATE INDEX "IX_AUD_CBBY_ACTY" ON "Zentra_AuditTrail" ("CreatedBy", "ActionType");

CREATE INDEX "IX_AUD_CRON_ACTY" ON "Zentra_AuditTrail" ("CreatedOn", "ActionType");

CREATE INDEX "IX_AUD_CRON_CBBY" ON "Zentra_AuditTrail" ("CreatedOn", "CreatedBy");

CREATE UNIQUE INDEX "IX_Zentra_ClientPostLogoutRedirectUris_ClientId_PostLogoutRedirectUri" ON "Zentra_ClientPostLogoutRedirectUris" ("ClientId", "PostLogoutRedirectUri");

CREATE UNIQUE INDEX "IX_Zentra_ClientRedirectUris_ClientId_RedirectUri" ON "Zentra_ClientRedirectUris" ("ClientId", "RedirectUri");

CREATE UNIQUE INDEX "IX_CLI_CLID_CLSEC" ON "Zentra_Clients" ("ClientId", "ClientSecret");

CREATE INDEX "IX_SECTOK_TOKTYPE_KEY" ON "Zentra_SecurityTokens" ("TokenType", "Key");

CREATE UNIQUE INDEX "IX_IDRESCLM_IDRESID_TYPE" ON "Zentra_IdentityClaims" ("IdentityResourceId", "Type");

CREATE UNIQUE INDEX "IX_IDRES_NAME" ON "Zentra_IdentityResources" ("Name");

CREATE INDEX "IX_NOTI_TYPE" ON "Zentra_Notification" ("Type");

CREATE INDEX "IX_Zentra_Notification_UserId" ON "Zentra_Notification" ("UserId");

CREATE INDEX "IX_Zentra_PasswordHistory_UserID" ON "Zentra_PasswordHistory" ("UserID");

CREATE INDEX "IX_Zentra_RoleClaims_RoleId" ON "Zentra_RoleClaims" ("RoleId");

CREATE UNIQUE INDEX "RoleNameIndex" ON "Zentra_Roles" ("NormalizedName");

CREATE UNIQUE INDEX "IX_SEC_QUESTION" ON "Zentra_SecurityQuestions" ("Question");

CREATE INDEX "IX_Zentra_UserClaims_UserId" ON "Zentra_UserClaims" ("UserId");

CREATE UNIQUE INDEX "IX_EXTID_PROVIDER_ISSUER_SUBJECT" ON "Zentra_ExternalIdentities" ("Provider", "Issuer", "Subject");

CREATE INDEX "IX_EXTID_USERID" ON "Zentra_ExternalIdentities" ("UserId");

CREATE INDEX "IX_EXTID_TENANT_EMAIL" ON "Zentra_ExternalIdentities" ("TenantId", "Email");

CREATE INDEX "IX_Zentra_UserLogins_UserId" ON "Zentra_UserLogins" ("UserId");

CREATE INDEX "IX_Zentra_UserRoles_RoleId" ON "Zentra_UserRoles" ("RoleId");

CREATE INDEX "IX_Zentra_UserRoles_UserId" ON "Zentra_UserRoles" ("UserId");

CREATE INDEX "EmailIndex" ON "Zentra_Users" ("NormalizedEmail");

CREATE UNIQUE INDEX "UserNameIndex" ON "Zentra_Users" ("NormalizedUserName");

CREATE INDEX "IX_USRSEC_QUEID" ON "Zentra_UserSecurityQuestions" ("SecurityQuestionId");

CREATE UNIQUE INDEX "IX_USRSEC_UID_QUEID" ON "Zentra_UserSecurityQuestions" ("UserId", "SecurityQuestionId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20220802110433_ZentraSqliteV1', '3.1.27');
