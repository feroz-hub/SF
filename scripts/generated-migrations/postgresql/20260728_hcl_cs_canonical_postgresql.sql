CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_ApiResources" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255),
        "Name" character varying(255) NOT NULL,
        "DisplayName" character varying(255),
        "Description" text,
        "Enabled" boolean NOT NULL,
        CONSTRAINT "PK_HclCs_ApiResources" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_AuditTrail" (
        "Id" uuid NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "CreatedBy" character varying(255) NOT NULL,
        "ActionType" integer NOT NULL,
        "TableName" character varying(255),
        "OldValue" text,
        "NewValue" text,
        "AffectedColumn" text,
        "ActionName" text,
        CONSTRAINT "PK_HclCs_AuditTrail" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_Clients" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255),
        "ClientId" character varying(128) NOT NULL,
        "ClientName" character varying(255),
        "ClientUri" text,
        "ClientIdIssuedAt" bigint NOT NULL,
        "ClientSecretExpiresAt" bigint NOT NULL,
        "ClientSecret" character varying(128),
        "LogoUri" text,
        "TermsOfServiceUri" text,
        "PolicyUri" text,
        "RefreshTokenExpiration" integer NOT NULL,
        "AccessTokenExpiration" integer NOT NULL,
        "IdentityTokenExpiration" integer NOT NULL,
        "LogoutTokenExpiration" integer NOT NULL,
        "AuthorizationCodeExpiration" integer NOT NULL,
        "AccessTokenType" integer NOT NULL,
        "RequirePkce" boolean NOT NULL,
        "IsPkceTextPlain" boolean NOT NULL,
        "RequireClientSecret" boolean NOT NULL,
        "IsFirstPartyApp" boolean NOT NULL,
        "AllowOfflineAccess" boolean NOT NULL,
        "AllowedScopes" text,
        "AllowAccessTokensViaBrowser" boolean NOT NULL,
        "ApplicationType" integer NOT NULL,
        "AllowedSigningAlgorithm" text,
        "SupportedGrantTypes" text,
        "SupportedResponseTypes" text,
        "FrontChannelLogoutSessionRequired" boolean NOT NULL,
        "FrontChannelLogoutUri" text,
        "BackChannelLogoutSessionRequired" boolean NOT NULL,
        "BackChannelLogoutUri" text,
        CONSTRAINT "PK_HclCs_Clients" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_IdentityResources" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255),
        "Name" character varying(255) NOT NULL,
        "DisplayName" character varying(255),
        "Description" text,
        "Enabled" boolean NOT NULL,
        "Required" boolean NOT NULL,
        "Emphasize" boolean NOT NULL,
        CONSTRAINT "PK_HclCs_IdentityResources" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_Roles" (
        "Id" uuid NOT NULL,
        "Name" character varying(255) NOT NULL,
        "NormalizedName" character varying(255) NOT NULL,
        "ConcurrencyStamp" character varying(255) NOT NULL,
        "Description" text,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255),
        CONSTRAINT "PK_HclCs_Roles" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_SecurityQuestions" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255),
        "Question" character varying(255) NOT NULL,
        CONSTRAINT "PK_HclCs_SecurityQuestions" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_SecurityTokens" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255),
        "Key" text,
        "TokenType" text,
        "TokenValue" text,
        "ClientId" text,
        "SessionId" text,
        "SubjectId" text,
        "CreationTime" timestamp without time zone NOT NULL,
        "ExpiresAt" integer NOT NULL,
        "ConsumedTime" timestamp without time zone,
        CONSTRAINT "PK_HclCs_SecurityTokens" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_Users" (
        "Id" uuid NOT NULL,
        "UserName" character varying(255) NOT NULL,
        "NormalizedUserName" character varying(255) NOT NULL,
        "Email" character varying(255) NOT NULL,
        "NormalizedEmail" character varying(255) NOT NULL,
        "EmailConfirmed" boolean NOT NULL,
        "PasswordHash" text NOT NULL,
        "SecurityStamp" character varying(255),
        "ConcurrencyStamp" character varying(255),
        "PhoneNumber" character varying(15),
        "PhoneNumberConfirmed" boolean NOT NULL,
        "TwoFactorEnabled" boolean NOT NULL,
        "LockoutEnd" timestamp with time zone,
        "LockoutEnabled" boolean NOT NULL,
        "AccessFailedCount" integer NOT NULL,
        "FirstName" character varying(255) NOT NULL,
        "LastName" character varying(255),
        "DateOfBirth" timestamp without time zone,
        "TwoFactorType" integer NOT NULL,
        "LastPasswordChangedDate" timestamp without time zone,
        "RequiresDefaultPasswordChange" boolean,
        "LastLoginDateTime" timestamp without time zone,
        "LastLogoutDateTime" timestamp without time zone,
        "IdentityProviderType" integer NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255),
        CONSTRAINT "PK_HclCs_Users" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_ApiResourceClaims" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255),
        "ApiResourceId" uuid NOT NULL,
        "Type" character varying(255) NOT NULL,
        CONSTRAINT "PK_HclCs_ApiResourceClaims" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_HclCs_ApiResourceClaims_HclCs_ApiResources_ApiResourceId" FOREIGN KEY ("ApiResourceId") REFERENCES "HclCs_ApiResources" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_ApiScopes" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255),
        "ApiResourceId" uuid NOT NULL,
        "Name" character varying(255) NOT NULL,
        "DisplayName" character varying(255),
        "Description" text,
        "Required" boolean NOT NULL,
        "Emphasize" boolean NOT NULL,
        CONSTRAINT "PK_HclCs_ApiScopes" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_HclCs_ApiScopes_HclCs_ApiResources_ApiResourceId" FOREIGN KEY ("ApiResourceId") REFERENCES "HclCs_ApiResources" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_ClientPostLogoutRedirectUris" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255),
        "ClientId" uuid NOT NULL,
        "PostLogoutRedirectUri" character varying(510) NOT NULL,
        CONSTRAINT "PK_HclCs_ClientPostLogoutRedirectUris" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_HclCs_ClientPostLogoutRedirectUris_HclCs_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "HclCs_Clients" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_ClientRedirectUris" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255),
        "ClientId" uuid NOT NULL,
        "RedirectUri" character varying(510) NOT NULL,
        CONSTRAINT "PK_HclCs_ClientRedirectUris" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_HclCs_ClientRedirectUris_HclCs_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "HclCs_Clients" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_IdentityClaims" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255),
        "IdentityResourceId" uuid NOT NULL,
        "Type" character varying(255) NOT NULL,
        "AliasType" character varying(255),
        CONSTRAINT "PK_HclCs_IdentityClaims" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_HclCs_IdentityClaims_HclCs_IdentityResources_IdentityResourceId" FOREIGN KEY ("IdentityResourceId") REFERENCES "HclCs_IdentityResources" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_RoleClaims" (
        "Id" integer GENERATED BY DEFAULT AS IDENTITY,
        "RoleId" uuid NOT NULL,
        "ClaimType" text,
        "ClaimValue" text,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255),
        CONSTRAINT "PK_HclCs_RoleClaims" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_HclCs_RoleClaims_HclCs_Roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "HclCs_Roles" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_Notification" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255),
        "UserId" uuid NOT NULL,
        "MessageId" character varying(255) NOT NULL,
        "Type" integer NOT NULL,
        "Activity" character varying(255),
        "Status" integer NOT NULL,
        "Sender" character varying(255) NOT NULL,
        "Recipient" character varying(255) NOT NULL,
        CONSTRAINT "PK_HclCs_Notification" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_HclCs_Notification_HclCs_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "HclCs_Users" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_PasswordHistory" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "CreatedBy" character varying(255) NOT NULL,
        "UserID" uuid NOT NULL,
        "ChangedOn" timestamp without time zone NOT NULL,
        "PasswordHash" character varying(255) NOT NULL,
        CONSTRAINT "PK_HclCs_PasswordHistory" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_HclCs_PasswordHistory_HclCs_Users_UserID" FOREIGN KEY ("UserID") REFERENCES "HclCs_Users" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_UserClaims" (
        "Id" integer GENERATED BY DEFAULT AS IDENTITY,
        "UserId" uuid NOT NULL,
        "ClaimType" text,
        "ClaimValue" text,
        "IsAdminClaim" boolean NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255),
        CONSTRAINT "PK_HclCs_UserClaims" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_HclCs_UserClaims_HclCs_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "HclCs_Users" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_UserLogins" (
        "LoginProvider" character varying(256) NOT NULL,
        "ProviderKey" character varying(256) NOT NULL,
        "UserId" uuid NOT NULL,
        "ProviderDisplayName" text,
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255),
        CONSTRAINT "PK_HclCs_UserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey", "UserId"),
        CONSTRAINT "FK_HclCs_UserLogins_HclCs_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "HclCs_Users" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_UserRoles" (
        "UserId" uuid NOT NULL,
        "RoleId" uuid NOT NULL,
        "Id" uuid NOT NULL,
        "ValidFrom" timestamp without time zone,
        "ValidTo" timestamp without time zone,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255),
        CONSTRAINT "PK_HclCs_UserRoles" PRIMARY KEY ("Id", "UserId", "RoleId"),
        CONSTRAINT "FK_HclCs_UserRoles_HclCs_Roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "HclCs_Roles" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_HclCs_UserRoles_HclCs_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "HclCs_Users" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_UserSecurityQuestions" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255),
        "UserId" uuid NOT NULL,
        "SecurityQuestionId" uuid NOT NULL,
        "Answer" character varying(255) NOT NULL,
        CONSTRAINT "PK_HclCs_UserSecurityQuestions" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_HclCs_UserSecurityQuestions_HclCs_SecurityQuestions_SecurityQuest~" FOREIGN KEY ("SecurityQuestionId") REFERENCES "HclCs_SecurityQuestions" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_HclCs_UserSecurityQuestions_HclCs_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "HclCs_Users" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_UserTokens" (
        "UserId" uuid NOT NULL,
        "LoginProvider" character varying(255) NOT NULL,
        "Name" character varying(255) NOT NULL,
        "Value" text NOT NULL,
        "IsDeleted" boolean NOT NULL,
        CONSTRAINT "PK_HclCs_UserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),
        CONSTRAINT "FK_HclCs_UserTokens_HclCs_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "HclCs_Users" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_ApiScopeClaims" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255),
        "ApiScopeId" uuid NOT NULL,
        "Type" character varying(255) NOT NULL,
        CONSTRAINT "PK_HclCs_ApiScopeClaims" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_HclCs_ApiScopeClaims_HclCs_ApiScopes_ApiScopeId" FOREIGN KEY ("ApiScopeId") REFERENCES "HclCs_ApiScopes" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE UNIQUE INDEX "IX_APIRES_CLM_RESID_TYPE" ON "HclCs_ApiResourceClaims" ("ApiResourceId", "Type");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE UNIQUE INDEX "IX_APIRES_NAME" ON "HclCs_ApiResources" ("Name");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE UNIQUE INDEX "IX_APISCO_CLM_SCOID_TYPE" ON "HclCs_ApiScopeClaims" ("ApiScopeId", "Type");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE UNIQUE INDEX "IX_APISCO_SCOID_NAME" ON "HclCs_ApiScopes" ("ApiResourceId", "Name");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE INDEX "IX_AUD_CBBY_ACTY" ON "HclCs_AuditTrail" ("CreatedBy", "ActionType");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE INDEX "IX_AUD_CRON_ACTY" ON "HclCs_AuditTrail" ("CreatedOn", "ActionType");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE INDEX "IX_AUD_CRON_CBBY" ON "HclCs_AuditTrail" ("CreatedOn", "CreatedBy");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE UNIQUE INDEX "IX_HclCs_ClientPostLogoutRedirectUris_ClientId_PostLogoutRedirect~" ON "HclCs_ClientPostLogoutRedirectUris" ("ClientId", "PostLogoutRedirectUri");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE UNIQUE INDEX "IX_HclCs_ClientRedirectUris_ClientId_RedirectUri" ON "HclCs_ClientRedirectUris" ("ClientId", "RedirectUri");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE UNIQUE INDEX "IX_CLI_CLID_CLSEC" ON "HclCs_Clients" ("ClientId", "ClientSecret");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE UNIQUE INDEX "IX_IDRESCLM_IDRESID_TYPE" ON "HclCs_IdentityClaims" ("IdentityResourceId", "Type");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE UNIQUE INDEX "IX_IDRES_NAME" ON "HclCs_IdentityResources" ("Name");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE INDEX "IX_NOTI_TYPE" ON "HclCs_Notification" ("Type");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE INDEX "IX_HclCs_Notification_UserId" ON "HclCs_Notification" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE INDEX "IX_HclCs_PasswordHistory_UserID" ON "HclCs_PasswordHistory" ("UserID");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE INDEX "IX_HclCs_RoleClaims_RoleId" ON "HclCs_RoleClaims" ("RoleId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE UNIQUE INDEX "RoleNameIndex" ON "HclCs_Roles" ("NormalizedName");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE UNIQUE INDEX "IX_SEC_QUESTION" ON "HclCs_SecurityQuestions" ("Question");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE INDEX "IX_HclCs_UserClaims_UserId" ON "HclCs_UserClaims" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE INDEX "IX_HclCs_UserLogins_UserId" ON "HclCs_UserLogins" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE INDEX "IX_HclCs_UserRoles_RoleId" ON "HclCs_UserRoles" ("RoleId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE INDEX "IX_HclCs_UserRoles_UserId" ON "HclCs_UserRoles" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE INDEX "EmailIndex" ON "HclCs_Users" ("NormalizedEmail");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE UNIQUE INDEX "UserNameIndex" ON "HclCs_Users" ("NormalizedUserName");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE INDEX "IX_USRSEC_QUEID" ON "HclCs_UserSecurityQuestions" ("SecurityQuestionId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE UNIQUE INDEX "IX_USRSEC_UID_QUEID" ON "HclCs_UserSecurityQuestions" ("UserId", "SecurityQuestionId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20220726113011_HclCsPostgreSqlV1', '8.0.11');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260726060000_Phase2HclIdentityProfile') THEN
    ALTER TABLE "HclCs_Users" ADD "DirectoryImmutableId" character varying(512);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260726060000_Phase2HclIdentityProfile') THEN
    ALTER TABLE "HclCs_Users" ADD "EmployeeId" character varying(255);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260726060000_Phase2HclIdentityProfile') THEN
    ALTER TABLE "HclCs_Users" ADD "UserPrincipalName" character varying(255);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260726060000_Phase2HclIdentityProfile') THEN
    ALTER TABLE "HclCs_Users" ADD "DisplayName" character varying(255);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260726060000_Phase2HclIdentityProfile') THEN
    ALTER TABLE "HclCs_Users" ADD "Department" character varying(255);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260726060000_Phase2HclIdentityProfile') THEN
    ALTER TABLE "HclCs_Users" ADD "AuthenticationSource" character varying(32);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260726060000_Phase2HclIdentityProfile') THEN
    ALTER TABLE "HclCs_Users" ADD "DirectoryLastValidatedAt" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260726060000_Phase2HclIdentityProfile') THEN
    UPDATE "HclCs_Users"
    SET "AuthenticationSource" = CASE "IdentityProviderType"
        WHEN 1 THEN 'LOCAL'
        WHEN 2 THEN 'LDAP'
        WHEN 3 THEN 'GOOGLE'
        ELSE NULL
    END
    WHERE "AuthenticationSource" IS NULL;

    DO $$
    BEGIN
        IF EXISTS (
            SELECT 1 FROM "HclCs_Users"
            WHERE "DirectoryImmutableId" IS NOT NULL
            GROUP BY "DirectoryImmutableId"
            HAVING COUNT(*) > 1
        ) THEN
            RAISE EXCEPTION 'Duplicate DirectoryImmutableId values must be resolved before Phase 2 migration.';
        END IF;

        IF EXISTS (
            SELECT 1 FROM "HclCs_Users"
            WHERE "UserPrincipalName" IS NOT NULL
            GROUP BY "UserPrincipalName"
            HAVING COUNT(*) > 1
        ) THEN
            RAISE EXCEPTION 'Duplicate UserPrincipalName values must be resolved before Phase 2 migration.';
        END IF;
    END $$;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260726060000_Phase2HclIdentityProfile') THEN
    CREATE INDEX "IX_USERS_EMPLOYEE_ID" ON "HclCs_Users" ("EmployeeId") WHERE "EmployeeId" IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260726060000_Phase2HclIdentityProfile') THEN
    CREATE UNIQUE INDEX "UX_USERS_DIRECTORY_IMMUTABLE_ID" ON "HclCs_Users" ("DirectoryImmutableId") WHERE "DirectoryImmutableId" IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260726060000_Phase2HclIdentityProfile') THEN
    CREATE UNIQUE INDEX "UX_USERS_USER_PRINCIPAL_NAME" ON "HclCs_Users" ("UserPrincipalName") WHERE "UserPrincipalName" IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260726060000_Phase2HclIdentityProfile') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260726060000_Phase2HclIdentityProfile', '8.0.11');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260727130000_Phase2BLocalAuthenticationEmailUniqueness') THEN
    DROP INDEX "EmailIndex";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260727130000_Phase2BLocalAuthenticationEmailUniqueness') THEN
    CREATE UNIQUE INDEX "EmailIndex" ON "HclCs_Users" ("NormalizedEmail");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260727130000_Phase2BLocalAuthenticationEmailUniqueness') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260727130000_Phase2BLocalAuthenticationEmailUniqueness', '8.0.11');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260728140000_Phase2CCoreInfrastructureSchema') THEN
    ALTER TABLE "HclCs_SecurityTokens" ADD "ConsumedAt" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260728140000_Phase2CCoreInfrastructureSchema') THEN
    ALTER TABLE "HclCs_SecurityTokens" ADD "TokenReuseDetected" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260728140000_Phase2CCoreInfrastructureSchema') THEN
    ALTER TABLE "HclCs_Clients" ADD "PreferredAudience" character varying(300);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260728140000_Phase2CCoreInfrastructureSchema') THEN
    CREATE TABLE "HclCs_ExternalIdentities" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL DEFAULT FALSE,
        "CreatedOn" timestamp with time zone NOT NULL,
        "ModifiedOn" timestamp with time zone,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255),
        "RowVersion" bytea,
        "UserId" uuid NOT NULL,
        "TenantId" character varying(128),
        "Provider" character varying(64) NOT NULL,
        "Issuer" character varying(256) NOT NULL,
        "Subject" character varying(256) NOT NULL,
        "Email" character varying(255) NOT NULL,
        "EmailVerified" boolean NOT NULL,
        "LinkedAt" timestamp with time zone NOT NULL,
        "LastSignInAt" timestamp with time zone,
        CONSTRAINT "PK_HclCs_ExternalIdentities" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_HclCs_ExternalIdentities_HclCs_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "HclCs_Users" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260728140000_Phase2CCoreInfrastructureSchema') THEN
    CREATE TABLE "HclCs_NotificationProviderConfig" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL DEFAULT FALSE,
        "CreatedOn" timestamp with time zone NOT NULL,
        "ModifiedOn" timestamp with time zone,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255),
        "ProviderName" character varying(50) NOT NULL,
        "ChannelType" integer NOT NULL,
        "IsActive" boolean NOT NULL,
        "ConfigJson" text NOT NULL,
        "LastTestedOn" timestamp with time zone,
        "LastTestSuccess" boolean,
        CONSTRAINT "PK_HclCs_NotificationProviderConfig" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260728140000_Phase2CCoreInfrastructureSchema') THEN
    CREATE TABLE "HclCs_ExternalAuthProviderConfig" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL DEFAULT FALSE,
        "CreatedOn" timestamp with time zone NOT NULL,
        "ModifiedOn" timestamp with time zone,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255),
        "ProviderName" character varying(50) NOT NULL,
        "ProviderType" integer NOT NULL,
        "IsEnabled" boolean NOT NULL,
        "ConfigJson" text NOT NULL,
        "AutoProvisionEnabled" boolean NOT NULL DEFAULT FALSE,
        "AllowedDomains" character varying(2000),
        "LastTestedOn" timestamp with time zone,
        "LastTestSuccess" boolean,
        CONSTRAINT "PK_HclCs_ExternalAuthProviderConfig" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260728140000_Phase2CCoreInfrastructureSchema') THEN
    CREATE UNIQUE INDEX "IX_EXTID_PROVIDER_ISSUER_SUBJECT" ON "HclCs_ExternalIdentities" ("Provider", "Issuer", "Subject");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260728140000_Phase2CCoreInfrastructureSchema') THEN
    CREATE INDEX "IX_EXTID_USERID" ON "HclCs_ExternalIdentities" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260728140000_Phase2CCoreInfrastructureSchema') THEN
    CREATE INDEX "IX_EXTID_TENANT_EMAIL" ON "HclCs_ExternalIdentities" ("TenantId", "Email");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260728140000_Phase2CCoreInfrastructureSchema') THEN
    CREATE INDEX "IX_NPC_CHANNEL_TYPE" ON "HclCs_NotificationProviderConfig" ("ChannelType");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260728140000_Phase2CCoreInfrastructureSchema') THEN
    CREATE INDEX "IX_NPC_CHANNEL_ACTIVE" ON "HclCs_NotificationProviderConfig" ("ChannelType", "IsActive");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260728140000_Phase2CCoreInfrastructureSchema') THEN
    CREATE UNIQUE INDEX "IX_EAPC_PROVIDER" ON "HclCs_ExternalAuthProviderConfig" ("ProviderName");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260728140000_Phase2CCoreInfrastructureSchema') THEN
    CREATE INDEX "IX_EAPC_PROVIDER_ENABLED" ON "HclCs_ExternalAuthProviderConfig" ("ProviderName", "IsEnabled");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260728140000_Phase2CCoreInfrastructureSchema') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260728140000_Phase2CCoreInfrastructureSchema', '8.0.11');
    END IF;
END $EF$;
COMMIT;

