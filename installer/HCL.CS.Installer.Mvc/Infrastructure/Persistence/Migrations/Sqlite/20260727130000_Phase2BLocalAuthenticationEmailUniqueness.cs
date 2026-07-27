/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HclCsInstallerMVC.Infrastructure.Persistence.Migrations.Sqlite;

public partial class Phase2BLocalAuthenticationEmailUniqueness : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "EmailIndex",
            table: "HclCs_Users");

        migrationBuilder.CreateIndex(
            name: "EmailIndex",
            table: "HclCs_Users",
            column: "NormalizedEmail",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "EmailIndex",
            table: "HclCs_Users");

        migrationBuilder.CreateIndex(
            name: "EmailIndex",
            table: "HclCs_Users",
            column: "NormalizedEmail");
    }
}
