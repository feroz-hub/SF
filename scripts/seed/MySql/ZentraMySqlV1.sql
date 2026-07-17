CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(95) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
);


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE TABLE `Zentra_ApiResources` (
        `Id` char(36) NOT NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        `CreatedOn` datetime(6) NOT NULL,
        `ModifiedOn` datetime(6) NULL,
        `CreatedBy` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ModifiedBy` varchar(255) CHARACTER SET utf8mb4 NULL,
        `RowVersion` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `Name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `DisplayName` varchar(255) CHARACTER SET utf8mb4 NULL,
        `Description` longtext CHARACTER SET utf8mb4 NULL,
        `Enabled` tinyint(1) NOT NULL,
        CONSTRAINT `PK_Zentra_ApiResources` PRIMARY KEY (`Id`)
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE TABLE `Zentra_AuditTrail` (
        `Id` char(36) NOT NULL,
        `CreatedOn` datetime(6) NOT NULL,
        `CreatedBy` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ActionType` int NOT NULL,
        `TableName` varchar(255) CHARACTER SET utf8mb4 NULL,
        `OldValue` longtext CHARACTER SET utf8mb4 NULL,
        `NewValue` longtext CHARACTER SET utf8mb4 NULL,
        `AffectedColumn` longtext CHARACTER SET utf8mb4 NULL,
        `ActionName` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_Zentra_AuditTrail` PRIMARY KEY (`Id`)
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE TABLE `Zentra_Clients` (
        `Id` char(36) NOT NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        `CreatedOn` datetime(6) NOT NULL,
        `ModifiedOn` datetime(6) NULL,
        `CreatedBy` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ModifiedBy` varchar(255) CHARACTER SET utf8mb4 NULL,
        `RowVersion` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `ClientId` varchar(128) CHARACTER SET utf8mb4 NOT NULL,
        `ClientName` varchar(255) CHARACTER SET utf8mb4 NULL,
        `ClientUri` longtext CHARACTER SET utf8mb4 NULL,
        `ClientIdIssuedAt` bigint NOT NULL,
        `ClientSecretExpiresAt` bigint NOT NULL,
        `ClientSecret` varchar(128) CHARACTER SET utf8mb4 NULL,
        `LogoUri` longtext CHARACTER SET utf8mb4 NULL,
        `TermsOfServiceUri` longtext CHARACTER SET utf8mb4 NULL,
        `PolicyUri` longtext CHARACTER SET utf8mb4 NULL,
        `RefreshTokenExpiration` int NOT NULL,
        `AccessTokenExpiration` int NOT NULL,
        `IdentityTokenExpiration` int NOT NULL,
        `LogoutTokenExpiration` int NOT NULL,
        `AuthorizationCodeExpiration` int NOT NULL,
        `AccessTokenType` int NOT NULL,
        `RequirePkce` tinyint(1) NOT NULL,
        `IsPkceTextPlain` tinyint(1) NOT NULL,
        `RequireClientSecret` tinyint(1) NOT NULL,
        `IsFirstPartyApp` tinyint(1) NOT NULL,
        `AllowOfflineAccess` tinyint(1) NOT NULL,
        `AllowedScopes` longtext CHARACTER SET utf8mb4 NULL,
        `AllowAccessTokensViaBrowser` tinyint(1) NOT NULL,
        `ApplicationType` int NOT NULL,
        `AllowedSigningAlgorithm` longtext CHARACTER SET utf8mb4 NULL,
        `SupportedGrantTypes` longtext CHARACTER SET utf8mb4 NULL,
        `SupportedResponseTypes` longtext CHARACTER SET utf8mb4 NULL,
        `FrontChannelLogoutSessionRequired` tinyint(1) NOT NULL,
        `FrontChannelLogoutUri` longtext CHARACTER SET utf8mb4 NULL,
        `BackChannelLogoutSessionRequired` tinyint(1) NOT NULL,
        `BackChannelLogoutUri` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_Zentra_Clients` PRIMARY KEY (`Id`)
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE TABLE `Zentra_IdentityResources` (
        `Id` char(36) NOT NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        `CreatedOn` datetime(6) NOT NULL,
        `ModifiedOn` datetime(6) NULL,
        `CreatedBy` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ModifiedBy` varchar(255) CHARACTER SET utf8mb4 NULL,
        `RowVersion` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `Name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `DisplayName` varchar(255) CHARACTER SET utf8mb4 NULL,
        `Description` longtext CHARACTER SET utf8mb4 NULL,
        `Enabled` tinyint(1) NOT NULL,
        `Required` tinyint(1) NOT NULL,
        `Emphasize` tinyint(1) NOT NULL,
        CONSTRAINT `PK_Zentra_IdentityResources` PRIMARY KEY (`Id`)
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE TABLE `Zentra_Roles` (
        `Id` char(36) NOT NULL,
        `Name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `NormalizedName` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ConcurrencyStamp` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `Description` longtext CHARACTER SET utf8mb4 NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        `CreatedOn` datetime(6) NOT NULL,
        `ModifiedOn` datetime(6) NULL,
        `CreatedBy` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ModifiedBy` varchar(255) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_Zentra_Roles` PRIMARY KEY (`Id`)
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE TABLE `Zentra_SecurityQuestions` (
        `Id` char(36) NOT NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        `CreatedOn` datetime(6) NOT NULL,
        `ModifiedOn` datetime(6) NULL,
        `CreatedBy` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ModifiedBy` varchar(255) CHARACTER SET utf8mb4 NULL,
        `RowVersion` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `Question` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_Zentra_SecurityQuestions` PRIMARY KEY (`Id`)
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE TABLE `Zentra_SecurityTokens` (
        `Id` char(36) NOT NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        `CreatedOn` datetime(6) NOT NULL,
        `ModifiedOn` datetime(6) NULL,
        `CreatedBy` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ModifiedBy` varchar(255) CHARACTER SET utf8mb4 NULL,
        `Key` longtext CHARACTER SET utf8mb4 NULL,
        `TokenType` longtext CHARACTER SET utf8mb4 NULL,
        `TokenValue` longtext CHARACTER SET utf8mb4 NULL,
        `ClientId` longtext CHARACTER SET utf8mb4 NULL,
        `SessionId` longtext CHARACTER SET utf8mb4 NULL,
        `SubjectId` longtext CHARACTER SET utf8mb4 NULL,
        `CreationTime` datetime(6) NOT NULL,
        `ExpiresAt` int NOT NULL,
        `ConsumedTime` datetime(6) NULL,
        `ConsumedAt` datetime(6) NULL,
        `TokenReuseDetected` tinyint(1) NOT NULL DEFAULT FALSE,
        CONSTRAINT `PK_Zentra_SecurityTokens` PRIMARY KEY (`Id`)
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE TABLE `Zentra_Users` (
        `Id` char(36) NOT NULL,
        `UserName` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `NormalizedUserName` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `Email` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `NormalizedEmail` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `EmailConfirmed` tinyint(1) NOT NULL,
        `PasswordHash` longtext CHARACTER SET utf8mb4 NOT NULL,
        `SecurityStamp` varchar(255) CHARACTER SET utf8mb4 NULL,
        `ConcurrencyStamp` varchar(255) CHARACTER SET utf8mb4 NULL,
        `PhoneNumber` varchar(15) CHARACTER SET utf8mb4 NULL,
        `PhoneNumberConfirmed` tinyint(1) NOT NULL,
        `TwoFactorEnabled` tinyint(1) NOT NULL,
        `LockoutEnd` datetime(6) NULL,
        `LockoutEnabled` tinyint(1) NOT NULL,
        `AccessFailedCount` int NOT NULL,
        `FirstName` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `LastName` varchar(255) CHARACTER SET utf8mb4 NULL,
        `DateOfBirth` datetime(6) NULL,
        `TwoFactorType` int NOT NULL,
        `LastPasswordChangedDate` datetime(6) NULL,
        `RequiresDefaultPasswordChange` tinyint(1) NULL,
        `LastLoginDateTime` datetime(6) NULL,
        `LastLogoutDateTime` datetime(6) NULL,
        `IdentityProviderType` int NOT NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        `CreatedOn` datetime(6) NOT NULL,
        `ModifiedOn` datetime(6) NULL,
        `CreatedBy` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ModifiedBy` varchar(255) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_Zentra_Users` PRIMARY KEY (`Id`)
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE TABLE `Zentra_ApiResourceClaims` (
        `Id` char(36) NOT NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        `CreatedOn` datetime(6) NOT NULL,
        `ModifiedOn` datetime(6) NULL,
        `CreatedBy` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ModifiedBy` varchar(255) CHARACTER SET utf8mb4 NULL,
        `RowVersion` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `ApiResourceId` char(36) NOT NULL,
        `Type` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_Zentra_ApiResourceClaims` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Zentra_ApiResourceClaims_Zentra_ApiResources_ApiResourceId` FOREIGN KEY (`ApiResourceId`) REFERENCES `Zentra_ApiResources` (`Id`) ON DELETE CASCADE
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE TABLE `Zentra_ApiScopes` (
        `Id` char(36) NOT NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        `CreatedOn` datetime(6) NOT NULL,
        `ModifiedOn` datetime(6) NULL,
        `CreatedBy` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ModifiedBy` varchar(255) CHARACTER SET utf8mb4 NULL,
        `RowVersion` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `ApiResourceId` char(36) NOT NULL,
        `Name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `DisplayName` varchar(255) CHARACTER SET utf8mb4 NULL,
        `Description` longtext CHARACTER SET utf8mb4 NULL,
        `Required` tinyint(1) NOT NULL,
        `Emphasize` tinyint(1) NOT NULL,
        CONSTRAINT `PK_Zentra_ApiScopes` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Zentra_ApiScopes_Zentra_ApiResources_ApiResourceId` FOREIGN KEY (`ApiResourceId`) REFERENCES `Zentra_ApiResources` (`Id`) ON DELETE CASCADE
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE TABLE `Zentra_ClientPostLogoutRedirectUris` (
        `Id` char(36) NOT NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        `CreatedOn` datetime(6) NOT NULL,
        `ModifiedOn` datetime(6) NULL,
        `CreatedBy` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ModifiedBy` varchar(255) CHARACTER SET utf8mb4 NULL,
        `RowVersion` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `ClientId` char(36) NOT NULL,
        `PostLogoutRedirectUri` varchar(510) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_Zentra_ClientPostLogoutRedirectUris` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Zentra_ClientPostLogoutRedirectUris_Zentra_Clients_ClientId` FOREIGN KEY (`ClientId`) REFERENCES `Zentra_Clients` (`Id`) ON DELETE CASCADE
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE TABLE `Zentra_ClientRedirectUris` (
        `Id` char(36) NOT NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        `CreatedOn` datetime(6) NOT NULL,
        `ModifiedOn` datetime(6) NULL,
        `CreatedBy` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ModifiedBy` varchar(255) CHARACTER SET utf8mb4 NULL,
        `RowVersion` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `ClientId` char(36) NOT NULL,
        `RedirectUri` varchar(510) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_Zentra_ClientRedirectUris` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Zentra_ClientRedirectUris_Zentra_Clients_ClientId` FOREIGN KEY (`ClientId`) REFERENCES `Zentra_Clients` (`Id`) ON DELETE CASCADE
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE TABLE `Zentra_IdentityClaims` (
        `Id` char(36) NOT NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        `CreatedOn` datetime(6) NOT NULL,
        `ModifiedOn` datetime(6) NULL,
        `CreatedBy` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ModifiedBy` varchar(255) CHARACTER SET utf8mb4 NULL,
        `RowVersion` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `IdentityResourceId` char(36) NOT NULL,
        `Type` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `AliasType` varchar(255) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_Zentra_IdentityClaims` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Zentra_IdentityClaims_Zentra_IdentityResources_IdentityResourceId` FOREIGN KEY (`IdentityResourceId`) REFERENCES `Zentra_IdentityResources` (`Id`) ON DELETE CASCADE
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE TABLE `Zentra_RoleClaims` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `RoleId` char(36) NOT NULL,
        `ClaimType` longtext CHARACTER SET utf8mb4 NULL,
        `ClaimValue` longtext CHARACTER SET utf8mb4 NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        `CreatedOn` datetime(6) NOT NULL,
        `ModifiedOn` datetime(6) NULL,
        `CreatedBy` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ModifiedBy` varchar(255) CHARACTER SET utf8mb4 NULL,
        `RowVersion` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        CONSTRAINT `PK_Zentra_RoleClaims` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Zentra_RoleClaims_Zentra_Roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `Zentra_Roles` (`Id`) ON DELETE RESTRICT
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE TABLE `Zentra_Notification` (
        `Id` char(36) NOT NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        `CreatedOn` datetime(6) NOT NULL,
        `ModifiedOn` datetime(6) NULL,
        `CreatedBy` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ModifiedBy` varchar(255) CHARACTER SET utf8mb4 NULL,
        `UserId` char(36) NOT NULL,
        `MessageId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `Type` int NOT NULL,
        `Activity` varchar(255) CHARACTER SET utf8mb4 NULL,
        `Status` int NOT NULL,
        `Sender` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `Recipient` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_Zentra_Notification` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Zentra_Notification_Zentra_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Zentra_Users` (`Id`) ON DELETE RESTRICT
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE TABLE `Zentra_PasswordHistory` (
        `Id` char(36) NOT NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        `CreatedOn` datetime(6) NOT NULL,
        `CreatedBy` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `UserID` char(36) NOT NULL,
        `ChangedOn` datetime(6) NOT NULL,
        `PasswordHash` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_Zentra_PasswordHistory` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Zentra_PasswordHistory_Zentra_Users_UserID` FOREIGN KEY (`UserID`) REFERENCES `Zentra_Users` (`Id`) ON DELETE RESTRICT
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE TABLE `Zentra_UserClaims` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `UserId` char(36) NOT NULL,
        `ClaimType` longtext CHARACTER SET utf8mb4 NULL,
        `ClaimValue` longtext CHARACTER SET utf8mb4 NULL,
        `IsAdminClaim` tinyint(1) NOT NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        `CreatedOn` datetime(6) NOT NULL,
        `ModifiedOn` datetime(6) NULL,
        `CreatedBy` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ModifiedBy` varchar(255) CHARACTER SET utf8mb4 NULL,
        `RowVersion` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        CONSTRAINT `PK_Zentra_UserClaims` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Zentra_UserClaims_Zentra_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Zentra_Users` (`Id`) ON DELETE RESTRICT
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE TABLE `Zentra_UserLogins` (
        `LoginProvider` varchar(256) CHARACTER SET utf8mb4 NOT NULL,
        `ProviderKey` varchar(256) CHARACTER SET utf8mb4 NOT NULL,
        `UserId` char(36) NOT NULL,
        `ProviderDisplayName` longtext CHARACTER SET utf8mb4 NULL,
        `Id` char(36) NOT NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        `CreatedOn` datetime(6) NOT NULL,
        `ModifiedOn` datetime(6) NULL,
        `CreatedBy` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ModifiedBy` varchar(255) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_Zentra_UserLogins` PRIMARY KEY (`LoginProvider`, `ProviderKey`, `UserId`),
        CONSTRAINT `FK_Zentra_UserLogins_Zentra_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Zentra_Users` (`Id`) ON DELETE RESTRICT
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE TABLE `Zentra_UserRoles` (
        `UserId` char(36) NOT NULL,
        `RoleId` char(36) NOT NULL,
        `Id` char(36) NOT NULL,
        `ValidFrom` datetime(6) NULL,
        `ValidTo` datetime(6) NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        `CreatedOn` datetime(6) NOT NULL,
        `ModifiedOn` datetime(6) NULL,
        `CreatedBy` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ModifiedBy` varchar(255) CHARACTER SET utf8mb4 NULL,
        `RowVersion` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        CONSTRAINT `PK_Zentra_UserRoles` PRIMARY KEY (`Id`, `UserId`, `RoleId`),
        CONSTRAINT `FK_Zentra_UserRoles_Zentra_Roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `Zentra_Roles` (`Id`) ON DELETE RESTRICT,
        CONSTRAINT `FK_Zentra_UserRoles_Zentra_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Zentra_Users` (`Id`) ON DELETE RESTRICT
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE TABLE `Zentra_UserSecurityQuestions` (
        `Id` char(36) NOT NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        `CreatedOn` datetime(6) NOT NULL,
        `ModifiedOn` datetime(6) NULL,
        `CreatedBy` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ModifiedBy` varchar(255) CHARACTER SET utf8mb4 NULL,
        `RowVersion` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `UserId` char(36) NOT NULL,
        `SecurityQuestionId` char(36) NOT NULL,
        `Answer` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_Zentra_UserSecurityQuestions` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Zentra_UserSecurityQuestions_Zentra_SecurityQuestions_SecurityQuesti~` FOREIGN KEY (`SecurityQuestionId`) REFERENCES `Zentra_SecurityQuestions` (`Id`) ON DELETE RESTRICT,
        CONSTRAINT `FK_Zentra_UserSecurityQuestions_Zentra_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Zentra_Users` (`Id`) ON DELETE RESTRICT
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE TABLE `Zentra_UserTokens` (
        `UserId` char(36) NOT NULL,
        `LoginProvider` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `Name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `Value` longtext CHARACTER SET utf8mb4 NOT NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        CONSTRAINT `PK_Zentra_UserTokens` PRIMARY KEY (`UserId`, `LoginProvider`, `Name`),
        CONSTRAINT `FK_Zentra_UserTokens_Zentra_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Zentra_Users` (`Id`) ON DELETE RESTRICT
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE TABLE `Zentra_ApiScopeClaims` (
        `Id` char(36) NOT NULL,
        `IsDeleted` tinyint(1) NOT NULL,
        `CreatedOn` datetime(6) NOT NULL,
        `ModifiedOn` datetime(6) NULL,
        `CreatedBy` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ModifiedBy` varchar(255) CHARACTER SET utf8mb4 NULL,
        `RowVersion` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `ApiScopeId` char(36) NOT NULL,
        `Type` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_Zentra_ApiScopeClaims` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Zentra_ApiScopeClaims_Zentra_ApiScopes_ApiScopeId` FOREIGN KEY (`ApiScopeId`) REFERENCES `Zentra_ApiScopes` (`Id`) ON DELETE CASCADE
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE UNIQUE INDEX `IX_APIRES_CLM_RESID_TYPE` ON `Zentra_ApiResourceClaims` (`ApiResourceId`, `Type`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE UNIQUE INDEX `IX_APIRES_NAME` ON `Zentra_ApiResources` (`Name`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE UNIQUE INDEX `IX_APISCO_CLM_SCOID_TYPE` ON `Zentra_ApiScopeClaims` (`ApiScopeId`, `Type`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE UNIQUE INDEX `IX_APISCO_SCOID_NAME` ON `Zentra_ApiScopes` (`ApiResourceId`, `Name`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE INDEX `IX_AUD_CBBY_ACTY` ON `Zentra_AuditTrail` (`CreatedBy`, `ActionType`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE INDEX `IX_AUD_CRON_ACTY` ON `Zentra_AuditTrail` (`CreatedOn`, `ActionType`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE INDEX `IX_AUD_CRON_CBBY` ON `Zentra_AuditTrail` (`CreatedOn`, `CreatedBy`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE UNIQUE INDEX `IX_Zentra_ClientPostLogoutRedirectUris_ClientId_PostLogoutRedirectU~` ON `Zentra_ClientPostLogoutRedirectUris` (`ClientId`, `PostLogoutRedirectUri`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE UNIQUE INDEX `IX_Zentra_ClientRedirectUris_ClientId_RedirectUri` ON `Zentra_ClientRedirectUris` (`ClientId`, `RedirectUri`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE INDEX `IX_SECTOK_TOKTYPE_KEY` ON `Zentra_SecurityTokens` (`TokenType`(64), `Key`(255));

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE UNIQUE INDEX `IX_CLI_CLID_CLSEC` ON `Zentra_Clients` (`ClientId`, `ClientSecret`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE UNIQUE INDEX `IX_IDRESCLM_IDRESID_TYPE` ON `Zentra_IdentityClaims` (`IdentityResourceId`, `Type`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE UNIQUE INDEX `IX_IDRES_NAME` ON `Zentra_IdentityResources` (`Name`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE INDEX `IX_NOTI_TYPE` ON `Zentra_Notification` (`Type`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE INDEX `IX_Zentra_Notification_UserId` ON `Zentra_Notification` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE INDEX `IX_Zentra_PasswordHistory_UserID` ON `Zentra_PasswordHistory` (`UserID`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE INDEX `IX_Zentra_RoleClaims_RoleId` ON `Zentra_RoleClaims` (`RoleId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE UNIQUE INDEX `RoleNameIndex` ON `Zentra_Roles` (`NormalizedName`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE UNIQUE INDEX `IX_SEC_QUESTION` ON `Zentra_SecurityQuestions` (`Question`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE INDEX `IX_Zentra_UserClaims_UserId` ON `Zentra_UserClaims` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE INDEX `IX_Zentra_UserLogins_UserId` ON `Zentra_UserLogins` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE INDEX `IX_Zentra_UserRoles_RoleId` ON `Zentra_UserRoles` (`RoleId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE INDEX `IX_Zentra_UserRoles_UserId` ON `Zentra_UserRoles` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE INDEX `EmailIndex` ON `Zentra_Users` (`NormalizedEmail`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE UNIQUE INDEX `UserNameIndex` ON `Zentra_Users` (`NormalizedUserName`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE INDEX `IX_USRSEC_QUEID` ON `Zentra_UserSecurityQuestions` (`SecurityQuestionId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    CREATE UNIQUE INDEX `IX_USRSEC_UID_QUEID` ON `Zentra_UserSecurityQuestions` (`UserId`, `SecurityQuestionId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220722123951_ZentraMySqlV1') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20220722123951_ZentraMySqlV1', '3.1.27');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

