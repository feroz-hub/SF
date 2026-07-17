CREATE TABLE IF NOT EXISTS `HclCs_ExternalIdentities` (
    `Id` char(36) NOT NULL,
    `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
    `CreatedOn` datetime(6) NOT NULL,
    `ModifiedOn` datetime(6) NULL,
    `CreatedBy` varchar(255) NOT NULL,
    `ModifiedBy` varchar(255) NULL,
    `RowVersion` varbinary(16) NULL,
    `UserId` char(36) NOT NULL,
    `TenantId` varchar(128) NULL,
    `Provider` varchar(64) NOT NULL,
    `Issuer` varchar(256) NOT NULL,
    `Subject` varchar(256) NOT NULL,
    `Email` varchar(255) NOT NULL,
    `EmailVerified` tinyint(1) NOT NULL,
    `LinkedAt` datetime(6) NOT NULL,
    `LastSignInAt` datetime(6) NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_HclCs_ExternalIdentities_HclCs_Users_UserId`
        FOREIGN KEY (`UserId`) REFERENCES `HclCs_Users` (`Id`) ON DELETE RESTRICT
);

CREATE UNIQUE INDEX `IX_EXTID_PROVIDER_ISSUER_SUBJECT`
    ON `HclCs_ExternalIdentities` (`Provider`, `Issuer`, `Subject`);

CREATE INDEX `IX_EXTID_USERID`
    ON `HclCs_ExternalIdentities` (`UserId`);

CREATE INDEX `IX_EXTID_TENANT_EMAIL`
    ON `HclCs_ExternalIdentities` (`TenantId`, `Email`);
