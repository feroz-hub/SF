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
[Migration("20260726060000_Phase2HclIdentityProfile")]
public partial class Phase2HclIdentityProfile : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "DirectoryImmutableId",
            table: "HclCs_Users",
            type: "longtext",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "EmployeeId",
            table: "HclCs_Users",
            type: "longtext",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "UserPrincipalName",
            table: "HclCs_Users",
            type: "longtext",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "DisplayName",
            table: "HclCs_Users",
            type: "longtext",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Department",
            table: "HclCs_Users",
            type: "longtext",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "AuthenticationSource",
            table: "HclCs_Users",
            type: "longtext",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "DirectoryLastValidatedAt",
            table: "HclCs_Users",
            type: "datetime(6)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "DirectoryImmutableId", table: "HclCs_Users");
        migrationBuilder.DropColumn(name: "EmployeeId", table: "HclCs_Users");
        migrationBuilder.DropColumn(name: "UserPrincipalName", table: "HclCs_Users");
        migrationBuilder.DropColumn(name: "DisplayName", table: "HclCs_Users");
        migrationBuilder.DropColumn(name: "Department", table: "HclCs_Users");
        migrationBuilder.DropColumn(name: "AuthenticationSource", table: "HclCs_Users");
        migrationBuilder.DropColumn(name: "DirectoryLastValidatedAt", table: "HclCs_Users");
    }
}
