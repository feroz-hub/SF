CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);


DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_ApiResources" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone NULL,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255) NULL,
        "Name" character varying(255) NOT NULL,
        "DisplayName" character varying(255) NULL,
        "Description" text NULL,
        "Enabled" boolean NOT NULL,
        CONSTRAINT "PK_HclCs_ApiResources" PRIMARY KEY ("Id")
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_AuditTrail" (
        "Id" uuid NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "CreatedBy" character varying(255) NOT NULL,
        "ActionType" integer NOT NULL,
        "TableName" character varying(255) NULL,
        "OldValue" text NULL,
        "NewValue" text NULL,
        "AffectedColumn" text NULL,
        "ActionName" text NULL,
        CONSTRAINT "PK_HclCs_AuditTrail" PRIMARY KEY ("Id")
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_Clients" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone NULL,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255) NULL,
        "ClientId" character varying(128) NOT NULL,
        "ClientName" character varying(255) NULL,
        "ClientUri" text NULL,
        "ClientIdIssuedAt" bigint NOT NULL,
        "ClientSecretExpiresAt" bigint NOT NULL,
        "ClientSecret" character varying(128) NULL,
        "LogoUri" text NULL,
        "TermsOfServiceUri" text NULL,
        "PolicyUri" text NULL,
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
        "AllowedScopes" text NULL,
        "AllowAccessTokensViaBrowser" boolean NOT NULL,
        "ApplicationType" integer NOT NULL,
        "AllowedSigningAlgorithm" text NULL,
        "SupportedGrantTypes" text NULL,
        "SupportedResponseTypes" text NULL,
        "FrontChannelLogoutSessionRequired" boolean NOT NULL,
        "FrontChannelLogoutUri" text NULL,
        "BackChannelLogoutSessionRequired" boolean NOT NULL,
        "BackChannelLogoutUri" text NULL,
        CONSTRAINT "PK_HclCs_Clients" PRIMARY KEY ("Id")
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_IdentityResources" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone NULL,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255) NULL,
        "Name" character varying(255) NOT NULL,
        "DisplayName" character varying(255) NULL,
        "Description" text NULL,
        "Enabled" boolean NOT NULL,
        "Required" boolean NOT NULL,
        "Emphasize" boolean NOT NULL,
        CONSTRAINT "PK_HclCs_IdentityResources" PRIMARY KEY ("Id")
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_Roles" (
        "Id" uuid NOT NULL,
        "Name" character varying(255) NOT NULL,
        "NormalizedName" character varying(255) NOT NULL,
        "ConcurrencyStamp" character varying(255) NOT NULL,
        "Description" text NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone NULL,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255) NULL,
        CONSTRAINT "PK_HclCs_Roles" PRIMARY KEY ("Id")
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_SecurityQuestions" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone NULL,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255) NULL,
        "Question" character varying(255) NOT NULL,
        CONSTRAINT "PK_HclCs_SecurityQuestions" PRIMARY KEY ("Id")
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_SecurityTokens" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp with time zone NOT NULL,
        "ModifiedOn" timestamp with time zone NULL,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255) NULL,
        "Key" text NULL,
        "TokenType" text NULL,
        "TokenValue" text NULL,
        "ClientId" text NULL,
        "SessionId" text NULL,
        "SubjectId" text NULL,
        "CreationTime" timestamp with time zone NOT NULL,
        "ExpiresAt" integer NOT NULL,
        "ConsumedTime" timestamp with time zone NULL,
        "ConsumedAt" timestamp with time zone NULL,
        "TokenReuseDetected" boolean NOT NULL DEFAULT FALSE,
        CONSTRAINT "PK_HclCs_SecurityTokens" PRIMARY KEY ("Id")
    );
    END IF;
END $$;

DO $$
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
        "SecurityStamp" character varying(255) NULL,
        "ConcurrencyStamp" character varying(255) NULL,
        "PhoneNumber" character varying(15) NULL,
        "PhoneNumberConfirmed" boolean NOT NULL,
        "TwoFactorEnabled" boolean NOT NULL,
        "LockoutEnd" timestamp with time zone NULL,
        "LockoutEnabled" boolean NOT NULL,
        "AccessFailedCount" integer NOT NULL,
        "FirstName" character varying(255) NOT NULL,
        "LastName" character varying(255) NULL,
        "DateOfBirth" timestamp with time zone NULL,
        "TwoFactorType" integer NOT NULL,
        "LastPasswordChangedDate" timestamp with time zone NULL,
        "RequiresDefaultPasswordChange" boolean NULL,
        "LastLoginDateTime" timestamp with time zone NULL,
        "LastLogoutDateTime" timestamp with time zone NULL,
        "IdentityProviderType" integer NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp with time zone NOT NULL,
        "ModifiedOn" timestamp with time zone NULL,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255) NULL,
        CONSTRAINT "PK_HclCs_Users" PRIMARY KEY ("Id")
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_ApiResourceClaims" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone NULL,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255) NULL,
        "ApiResourceId" uuid NOT NULL,
        "Type" character varying(255) NOT NULL,
        CONSTRAINT "PK_HclCs_ApiResourceClaims" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_HclCs_ApiResourceClaims_HclCs_ApiResources_ApiResourceId" FOREIGN KEY ("ApiResourceId") REFERENCES "HclCs_ApiResources" ("Id") ON DELETE CASCADE
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_ApiScopes" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone NULL,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255) NULL,
        "ApiResourceId" uuid NOT NULL,
        "Name" character varying(255) NOT NULL,
        "DisplayName" character varying(255) NULL,
        "Description" text NULL,
        "Required" boolean NOT NULL,
        "Emphasize" boolean NOT NULL,
        CONSTRAINT "PK_HclCs_ApiScopes" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_HclCs_ApiScopes_HclCs_ApiResources_ApiResourceId" FOREIGN KEY ("ApiResourceId") REFERENCES "HclCs_ApiResources" ("Id") ON DELETE CASCADE
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_ClientPostLogoutRedirectUris" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone NULL,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255) NULL,
        "ClientId" uuid NOT NULL,
        "PostLogoutRedirectUri" character varying(510) NOT NULL,
        CONSTRAINT "PK_HclCs_ClientPostLogoutRedirectUris" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_HclCs_ClientPostLogoutRedirectUris_HclCs_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "HclCs_Clients" ("Id") ON DELETE CASCADE
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_ClientRedirectUris" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone NULL,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255) NULL,
        "ClientId" uuid NOT NULL,
        "RedirectUri" character varying(510) NOT NULL,
        CONSTRAINT "PK_HclCs_ClientRedirectUris" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_HclCs_ClientRedirectUris_HclCs_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "HclCs_Clients" ("Id") ON DELETE CASCADE
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_IdentityClaims" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone NULL,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255) NULL,
        "IdentityResourceId" uuid NOT NULL,
        "Type" character varying(255) NOT NULL,
        "AliasType" character varying(255) NULL,
        CONSTRAINT "PK_HclCs_IdentityClaims" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_HclCs_IdentityClaims_HclCs_IdentityResources_IdentityResourceId" FOREIGN KEY ("IdentityResourceId") REFERENCES "HclCs_IdentityResources" ("Id") ON DELETE CASCADE
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_RoleClaims" (
        "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
        "RoleId" uuid NOT NULL,
        "ClaimType" text NULL,
        "ClaimValue" text NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone NULL,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255) NULL,
        CONSTRAINT "PK_HclCs_RoleClaims" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_HclCs_RoleClaims_HclCs_Roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "HclCs_Roles" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_Notification" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone NULL,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255) NULL,
        "UserId" uuid NOT NULL,
        "MessageId" character varying(255) NOT NULL,
        "Type" integer NOT NULL,
        "Activity" character varying(255) NULL,
        "Status" integer NOT NULL,
        "Sender" character varying(255) NOT NULL,
        "Recipient" character varying(255) NOT NULL,
        CONSTRAINT "PK_HclCs_Notification" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_HclCs_Notification_HclCs_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "HclCs_Users" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $$;

DO $$
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
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_UserClaims" (
        "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
        "UserId" uuid NOT NULL,
        "ClaimType" text NULL,
        "ClaimValue" text NULL,
        "IsAdminClaim" boolean NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone NULL,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255) NULL,
        CONSTRAINT "PK_HclCs_UserClaims" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_HclCs_UserClaims_HclCs_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "HclCs_Users" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_UserLogins" (
        "LoginProvider" character varying(256) NOT NULL,
        "ProviderKey" character varying(256) NOT NULL,
        "UserId" uuid NOT NULL,
        "ProviderDisplayName" text NULL,
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone NULL,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255) NULL,
        CONSTRAINT "PK_HclCs_UserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey", "UserId"),
        CONSTRAINT "FK_HclCs_UserLogins_HclCs_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "HclCs_Users" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_UserRoles" (
        "UserId" uuid NOT NULL,
        "RoleId" uuid NOT NULL,
        "Id" uuid NOT NULL,
        "ValidFrom" timestamp without time zone NULL,
        "ValidTo" timestamp without time zone NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone NULL,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255) NULL,
        CONSTRAINT "PK_HclCs_UserRoles" PRIMARY KEY ("Id", "UserId", "RoleId"),
        CONSTRAINT "FK_HclCs_UserRoles_HclCs_Roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "HclCs_Roles" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_HclCs_UserRoles_HclCs_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "HclCs_Users" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_UserSecurityQuestions" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone NULL,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255) NULL,
        "UserId" uuid NOT NULL,
        "SecurityQuestionId" uuid NOT NULL,
        "Answer" character varying(255) NOT NULL,
        CONSTRAINT "PK_HclCs_UserSecurityQuestions" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_HclCs_UserSecurityQuestions_HclCs_SecurityQuestions_SecurityQuest~" FOREIGN KEY ("SecurityQuestionId") REFERENCES "HclCs_SecurityQuestions" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_HclCs_UserSecurityQuestions_HclCs_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "HclCs_Users" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $$;

DO $$
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
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE TABLE "HclCs_ApiScopeClaims" (
        "Id" uuid NOT NULL,
        "IsDeleted" boolean NOT NULL,
        "CreatedOn" timestamp without time zone NOT NULL,
        "ModifiedOn" timestamp without time zone NULL,
        "CreatedBy" character varying(255) NOT NULL,
        "ModifiedBy" character varying(255) NULL,
        "ApiScopeId" uuid NOT NULL,
        "Type" character varying(255) NOT NULL,
        CONSTRAINT "PK_HclCs_ApiScopeClaims" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_HclCs_ApiScopeClaims_HclCs_ApiScopes_ApiScopeId" FOREIGN KEY ("ApiScopeId") REFERENCES "HclCs_ApiScopes" ("Id") ON DELETE CASCADE
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE UNIQUE INDEX "IX_APIRES_CLM_RESID_TYPE" ON "HclCs_ApiResourceClaims" ("ApiResourceId", "Type");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE UNIQUE INDEX "IX_APIRES_NAME" ON "HclCs_ApiResources" ("Name");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE UNIQUE INDEX "IX_APISCO_CLM_SCOID_TYPE" ON "HclCs_ApiScopeClaims" ("ApiScopeId", "Type");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE UNIQUE INDEX "IX_APISCO_SCOID_NAME" ON "HclCs_ApiScopes" ("ApiResourceId", "Name");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE INDEX "IX_AUD_CBBY_ACTY" ON "HclCs_AuditTrail" ("CreatedBy", "ActionType");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE INDEX "IX_AUD_CRON_ACTY" ON "HclCs_AuditTrail" ("CreatedOn", "ActionType");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE INDEX "IX_AUD_CRON_CBBY" ON "HclCs_AuditTrail" ("CreatedOn", "CreatedBy");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE UNIQUE INDEX "IX_HclCs_ClientPostLogoutRedirectUris_ClientId_PostLogoutRedirect~" ON "HclCs_ClientPostLogoutRedirectUris" ("ClientId", "PostLogoutRedirectUri");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE UNIQUE INDEX "IX_HclCs_ClientRedirectUris_ClientId_RedirectUri" ON "HclCs_ClientRedirectUris" ("ClientId", "RedirectUri");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE INDEX "IX_SECTOK_TOKTYPE_KEY" ON "HclCs_SecurityTokens" ("TokenType", "Key");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE UNIQUE INDEX "IX_CLI_CLID_CLSEC" ON "HclCs_Clients" ("ClientId", "ClientSecret");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE UNIQUE INDEX "IX_IDRESCLM_IDRESID_TYPE" ON "HclCs_IdentityClaims" ("IdentityResourceId", "Type");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE UNIQUE INDEX "IX_IDRES_NAME" ON "HclCs_IdentityResources" ("Name");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE INDEX "IX_NOTI_TYPE" ON "HclCs_Notification" ("Type");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE INDEX "IX_HclCs_Notification_UserId" ON "HclCs_Notification" ("UserId");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE INDEX "IX_HclCs_PasswordHistory_UserID" ON "HclCs_PasswordHistory" ("UserID");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE INDEX "IX_HclCs_RoleClaims_RoleId" ON "HclCs_RoleClaims" ("RoleId");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE UNIQUE INDEX "RoleNameIndex" ON "HclCs_Roles" ("NormalizedName");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE UNIQUE INDEX "IX_SEC_QUESTION" ON "HclCs_SecurityQuestions" ("Question");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE INDEX "IX_HclCs_UserClaims_UserId" ON "HclCs_UserClaims" ("UserId");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE INDEX "IX_HclCs_UserLogins_UserId" ON "HclCs_UserLogins" ("UserId");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE INDEX "IX_HclCs_UserRoles_RoleId" ON "HclCs_UserRoles" ("RoleId");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE INDEX "IX_HclCs_UserRoles_UserId" ON "HclCs_UserRoles" ("UserId");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE INDEX "EmailIndex" ON "HclCs_Users" ("NormalizedEmail");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE UNIQUE INDEX "UserNameIndex" ON "HclCs_Users" ("NormalizedUserName");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE INDEX "IX_USRSEC_QUEID" ON "HclCs_UserSecurityQuestions" ("SecurityQuestionId");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    CREATE UNIQUE INDEX "IX_USRSEC_UID_QUEID" ON "HclCs_UserSecurityQuestions" ("UserId", "SecurityQuestionId");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20220726113011_HclCsPostgreSqlV1') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20220726113011_HclCsPostgreSqlV1', '3.1.27');
    END IF;
END $$;
