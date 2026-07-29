#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace HCL.CS.Infrastructure.Data.Migrations.Sqlite;

public partial class HclCsSqliteV2 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Released historical SQLite baseline V2 step:
        // Preserves exact original DDL schema structure from commit cbf0172 while enabling idempotent execution.
        migrationBuilder.Sql(@"
            CREATE TABLE IF NOT EXISTS ""HclCs_ApiResources"" (
                ""Id"" TEXT NOT NULL CONSTRAINT ""PK_HclCs_ApiResources"" PRIMARY KEY,
                ""Name"" TEXT NOT NULL,
                ""DisplayName"" TEXT NULL,
                ""Description"" TEXT NULL,
                ""Enabled"" INTEGER NOT NULL,
                ""IsDeleted"" INTEGER NOT NULL,
                ""CreatedOn"" TEXT NOT NULL,
                ""ModifiedOn"" TEXT NULL,
                ""CreatedBy"" TEXT NOT NULL,
                ""ModifiedBy"" TEXT NULL,
                ""RowVersion"" BLOB NULL DEFAULT (CURRENT_TIMESTAMP)
            );

            CREATE TABLE IF NOT EXISTS ""HclCs_AuditTrail"" (
                ""Id"" TEXT NOT NULL CONSTRAINT ""PK_HclCs_AuditTrail"" PRIMARY KEY,
                ""ActionType"" INTEGER NOT NULL,
                ""TableName"" TEXT NULL,
                ""OldValue"" TEXT NULL,
                ""NewValue"" TEXT NULL,
                ""AffectedColumn"" TEXT NULL,
                ""ActionName"" TEXT NULL,
                ""CreatedOn"" TEXT NOT NULL,
                ""CreatedBy"" TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS ""HclCs_Clients"" (
                ""Id"" TEXT NOT NULL CONSTRAINT ""PK_HclCs_Clients"" PRIMARY KEY,
                ""ClientId"" TEXT NOT NULL,
                ""ClientName"" TEXT NULL,
                ""ClientUri"" TEXT NULL,
                ""ClientIdIssuedAt"" INTEGER NOT NULL,
                ""ClientSecretExpiresAt"" INTEGER NOT NULL,
                ""ClientSecret"" TEXT NULL,
                ""LogoUri"" TEXT NULL,
                ""TermsOfServiceUri"" TEXT NULL,
                ""PolicyUri"" TEXT NULL,
                ""RefreshTokenExpiration"" INTEGER NOT NULL,
                ""AccessTokenLifetime"" INTEGER NOT NULL,
                ""IdentityTokenLifetime"" INTEGER NOT NULL,
                ""AuthorizationCodeLifetime"" INTEGER NOT NULL,
                ""AbsoluteRefreshTokenLifetime"" INTEGER NOT NULL,
                ""SlidingRefreshTokenLifetime"" INTEGER NOT NULL,
                ""RequireClientSecret"" INTEGER NOT NULL,
                ""RequirePkce"" INTEGER NOT NULL,
                ""AllowPlainTextPkce"" INTEGER NOT NULL,
                ""AllowAccessTokensViaBrowser"" INTEGER NOT NULL,
                ""RequireConsent"" INTEGER NOT NULL,
                ""AllowOfflineAccess"" INTEGER NOT NULL,
                ""EnableLocalLogin"" INTEGER NOT NULL,
                ""IncludeJwtId"" INTEGER NOT NULL,
                ""AlwaysSendClientClaims"" INTEGER NOT NULL,
                ""AlwaysIncludeUserClaimsInIdToken"" INTEGER NOT NULL,
                ""FrontChannelLogoutUri"" TEXT NULL,
                ""FrontChannelLogoutSessionRequired"" INTEGER NOT NULL,
                ""BackChannelLogoutUri"" TEXT NULL,
                ""BackChannelLogoutSessionRequired"" INTEGER NOT NULL,
                ""Enabled"" INTEGER NOT NULL,
                ""IsDeleted"" INTEGER NOT NULL,
                ""CreatedOn"" TEXT NOT NULL,
                ""ModifiedOn"" TEXT NULL,
                ""CreatedBy"" TEXT NOT NULL,
                ""ModifiedBy"" TEXT NULL,
                ""RowVersion"" BLOB NULL DEFAULT (CURRENT_TIMESTAMP)
            );
        ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "HclCs_ApiResources");
        migrationBuilder.DropTable(name: "HclCs_AuditTrail");
        migrationBuilder.DropTable(name: "HclCs_Clients");
    }
}
