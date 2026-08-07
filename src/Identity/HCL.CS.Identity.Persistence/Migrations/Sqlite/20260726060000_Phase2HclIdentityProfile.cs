using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using HCL.CS.Infrastructure.Data;

namespace HCL.CS.Infrastructure.Data.Migrations.Sqlite;

[DbContext(typeof(SqLiteApplicationDbContext))]
[Migration("20260726060000_Phase2HclIdentityProfile")]
public sealed class Phase2HclIdentityProfile : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "DirectoryImmutableId",
            table: "HclCs_Users",
            type: "TEXT",
            maxLength: 512,
            nullable: true);
        migrationBuilder.AddColumn<string>(
            name: "EmployeeId",
            table: "HclCs_Users",
            type: "TEXT",
            maxLength: 255,
            nullable: true);
        migrationBuilder.AddColumn<string>(
            name: "UserPrincipalName",
            table: "HclCs_Users",
            type: "TEXT",
            maxLength: 255,
            nullable: true);
        migrationBuilder.AddColumn<string>(
            name: "DisplayName",
            table: "HclCs_Users",
            type: "TEXT",
            maxLength: 255,
            nullable: true);
        migrationBuilder.AddColumn<string>(
            name: "Department",
            table: "HclCs_Users",
            type: "TEXT",
            maxLength: 255,
            nullable: true);
        migrationBuilder.AddColumn<string>(
            name: "AuthenticationSource",
            table: "HclCs_Users",
            type: "TEXT",
            maxLength: 32,
            nullable: true);
        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "DirectoryLastValidatedAt",
            table: "HclCs_Users",
            type: "TEXT",
            nullable: true);

        migrationBuilder.Sql(
            """
            UPDATE "HclCs_Users"
            SET "AuthenticationSource" = CASE "IdentityProviderType"
                WHEN 1 THEN 'LOCAL'
                WHEN 2 THEN 'LDAP'
                WHEN 3 THEN 'GOOGLE'
                ELSE NULL
            END
            WHERE "AuthenticationSource" IS NULL;
            """);

        migrationBuilder.CreateIndex(
            name: "IX_USERS_EMPLOYEE_ID",
            table: "HclCs_Users",
            column: "EmployeeId");
        migrationBuilder.CreateIndex(
            name: "UX_USERS_DIRECTORY_IMMUTABLE_ID",
            table: "HclCs_Users",
            column: "DirectoryImmutableId",
            unique: true);
        migrationBuilder.CreateIndex(
            name: "UX_USERS_USER_PRINCIPAL_NAME",
            table: "HclCs_Users",
            column: "UserPrincipalName",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "IX_USERS_EMPLOYEE_ID", table: "HclCs_Users");
        migrationBuilder.DropIndex(name: "UX_USERS_DIRECTORY_IMMUTABLE_ID", table: "HclCs_Users");
        migrationBuilder.DropIndex(name: "UX_USERS_USER_PRINCIPAL_NAME", table: "HclCs_Users");
        migrationBuilder.DropColumn(name: "AuthenticationSource", table: "HclCs_Users");
        migrationBuilder.DropColumn(name: "Department", table: "HclCs_Users");
        migrationBuilder.DropColumn(name: "DirectoryImmutableId", table: "HclCs_Users");
        migrationBuilder.DropColumn(name: "DirectoryLastValidatedAt", table: "HclCs_Users");
        migrationBuilder.DropColumn(name: "DisplayName", table: "HclCs_Users");
        migrationBuilder.DropColumn(name: "EmployeeId", table: "HclCs_Users");
        migrationBuilder.DropColumn(name: "UserPrincipalName", table: "HclCs_Users");
    }
}
