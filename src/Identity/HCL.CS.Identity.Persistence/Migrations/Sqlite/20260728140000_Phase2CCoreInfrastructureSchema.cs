/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HCL.CS.Infrastructure.Data.Migrations.Sqlite;

[DbContext(typeof(SqLiteApplicationDbContext))]
[Migration("20260728140000_Phase2CCoreInfrastructureSchema")]
public sealed class Phase2CCoreInfrastructureSchema : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "ConsumedAt",
            table: "HclCs_SecurityTokens",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "TokenReuseDetected",
            table: "HclCs_SecurityTokens",
            type: "INTEGER",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "PreferredAudience",
            table: "HclCs_Clients",
            type: "TEXT",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "HclCs_ExternalIdentities",
            columns: table => new
            {
                Id = table.Column<string>(type: "TEXT", nullable: false),
                IsDeleted = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                CreatedOn = table.Column<string>(type: "TEXT", nullable: false),
                ModifiedOn = table.Column<string>(type: "TEXT", nullable: true),
                CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                ModifiedBy = table.Column<string>(type: "TEXT", nullable: true),
                RowVersion = table.Column<byte[]>(type: "BLOB", nullable: true),
                UserId = table.Column<string>(type: "TEXT", nullable: false),
                TenantId = table.Column<string>(type: "TEXT", nullable: true),
                Provider = table.Column<string>(type: "TEXT", nullable: false),
                Issuer = table.Column<string>(type: "TEXT", nullable: false),
                Subject = table.Column<string>(type: "TEXT", nullable: false),
                Email = table.Column<string>(type: "TEXT", nullable: false),
                EmailVerified = table.Column<int>(type: "INTEGER", nullable: false),
                LinkedAt = table.Column<string>(type: "TEXT", nullable: false),
                LastSignInAt = table.Column<string>(type: "TEXT", nullable: true)
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
                Id = table.Column<string>(type: "TEXT", nullable: false),
                IsDeleted = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                CreatedOn = table.Column<string>(type: "TEXT", nullable: false),
                ModifiedOn = table.Column<string>(type: "TEXT", nullable: true),
                CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                ModifiedBy = table.Column<string>(type: "TEXT", nullable: true),
                ProviderName = table.Column<string>(type: "TEXT", nullable: false),
                ChannelType = table.Column<int>(type: "INTEGER", nullable: false),
                IsActive = table.Column<int>(type: "INTEGER", nullable: false),
                ConfigJson = table.Column<string>(type: "TEXT", nullable: false),
                LastTestedOn = table.Column<string>(type: "TEXT", nullable: true),
                LastTestSuccess = table.Column<int>(type: "INTEGER", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HclCs_NotificationProviderConfig", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "HclCs_ExternalAuthProviderConfig",
            columns: table => new
            {
                Id = table.Column<string>(type: "TEXT", nullable: false),
                IsDeleted = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                CreatedOn = table.Column<string>(type: "TEXT", nullable: false),
                ModifiedOn = table.Column<string>(type: "TEXT", nullable: true),
                CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                ModifiedBy = table.Column<string>(type: "TEXT", nullable: true),
                ProviderName = table.Column<string>(type: "TEXT", nullable: false),
                ProviderType = table.Column<int>(type: "INTEGER", nullable: false),
                IsEnabled = table.Column<int>(type: "INTEGER", nullable: false),
                ConfigJson = table.Column<string>(type: "TEXT", nullable: false),
                AutoProvisionEnabled = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                AllowedDomains = table.Column<string>(type: "TEXT", nullable: true),
                LastTestedOn = table.Column<string>(type: "TEXT", nullable: true),
                LastTestSuccess = table.Column<int>(type: "INTEGER", nullable: true)
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
