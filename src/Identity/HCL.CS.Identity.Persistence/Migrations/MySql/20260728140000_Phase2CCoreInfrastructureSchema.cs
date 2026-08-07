/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HCL.CS.Infrastructure.Data.Migrations.MySql;

[DbContext(typeof(MySqlApplicationDbContext))]
[Migration("20260728140000_Phase2CCoreInfrastructureSchema")]
public partial class Phase2CCoreInfrastructureSchema : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "ConsumedAt",
            table: "HclCs_SecurityTokens",
            type: "datetime(6)",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "TokenReuseDetected",
            table: "HclCs_SecurityTokens",
            type: "tinyint(1)",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<string>(
            name: "PreferredAudience",
            table: "HclCs_Clients",
            type: "varchar(300)",
            maxLength: 300,
            nullable: true);

        migrationBuilder.CreateTable(
            name: "HclCs_ExternalIdentities",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "char(36)", nullable: false),
                IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                ModifiedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                RowVersion = table.Column<byte[]>(type: "longblob", nullable: true),
                UserId = table.Column<Guid>(type: "char(36)", nullable: false),
                TenantId = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true),
                Provider = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                Issuer = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                Subject = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                EmailVerified = table.Column<bool>(type: "tinyint(1)", nullable: false),
                LinkedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                LastSignInAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HclCs_ExternalIdentities", x => x.Id);
                table.ForeignKey(
                    name: "FK_HclCs_ExternalIdentities_HclCs_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "HclCs_Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "HclCs_NotificationProviderConfig",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "char(36)", nullable: false),
                IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                ModifiedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                ProviderName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                ChannelType = table.Column<int>(type: "int", nullable: false),
                IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ConfigJson = table.Column<string>(type: "longtext", nullable: false),
                LastTestedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                LastTestSuccess = table.Column<bool>(type: "tinyint(1)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HclCs_NotificationProviderConfig", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "HclCs_ExternalAuthProviderConfig",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "char(36)", nullable: false),
                IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                ModifiedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                ProviderName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                ProviderType = table.Column<int>(type: "int", nullable: false),
                IsEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ConfigJson = table.Column<string>(type: "longtext", nullable: false),
                AutoProvisionEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                AllowedDomains = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true),
                LastTestedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                LastTestSuccess = table.Column<bool>(type: "tinyint(1)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HclCs_ExternalAuthProviderConfig", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_EXTID_PROVIDER_ISSUER_SUBJECT",
            table: "HclCs_ExternalIdentities",
            columns: new[] { "Provider", "Issuer", "Subject" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_EXTID_USERID",
            table: "HclCs_ExternalIdentities",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_EXTID_TENANT_EMAIL",
            table: "HclCs_ExternalIdentities",
            columns: new[] { "TenantId", "Email" });

        migrationBuilder.CreateIndex(
            name: "IX_NPC_CHANNEL_TYPE",
            table: "HclCs_NotificationProviderConfig",
            column: "ChannelType");

        migrationBuilder.CreateIndex(
            name: "IX_NPC_CHANNEL_ACTIVE",
            table: "HclCs_NotificationProviderConfig",
            columns: new[] { "ChannelType", "IsActive" });

        migrationBuilder.CreateIndex(
            name: "IX_EAPC_PROVIDER",
            table: "HclCs_ExternalAuthProviderConfig",
            column: "ProviderName",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_EAPC_PROVIDER_ENABLED",
            table: "HclCs_ExternalAuthProviderConfig",
            columns: new[] { "ProviderName", "IsEnabled" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "HclCs_ExternalIdentities");
        migrationBuilder.DropTable(name: "HclCs_NotificationProviderConfig");
        migrationBuilder.DropTable(name: "HclCs_ExternalAuthProviderConfig");
        migrationBuilder.DropColumn(name: "ConsumedAt", table: "HclCs_SecurityTokens");
        migrationBuilder.DropColumn(name: "TokenReuseDetected", table: "HclCs_SecurityTokens");
        migrationBuilder.DropColumn(name: "PreferredAudience", table: "HclCs_Clients");
    }
}
