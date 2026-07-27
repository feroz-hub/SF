using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using HclCsInstallerMVC.Infrastructure.Persistence.Data;

namespace HclCsInstallerMVC.Infrastructure.Persistence.Migrations.PostgreSql;

[DbContext(typeof(PostgreSqlApplicationDbcontext))]
[Migration("20260726060000_Phase2HclIdentityProfile")]
public sealed class Phase2HclIdentityProfile : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "DirectoryImmutableId",
            table: "HclCs_Users",
            type: "character varying(512)",
            maxLength: 512,
            nullable: true);
        migrationBuilder.AddColumn<string>(
            name: "EmployeeId",
            table: "HclCs_Users",
            type: "character varying(255)",
            maxLength: 255,
            nullable: true);
        migrationBuilder.AddColumn<string>(
            name: "UserPrincipalName",
            table: "HclCs_Users",
            type: "character varying(255)",
            maxLength: 255,
            nullable: true);
        migrationBuilder.AddColumn<string>(
            name: "DisplayName",
            table: "HclCs_Users",
            type: "character varying(255)",
            maxLength: 255,
            nullable: true);
        migrationBuilder.AddColumn<string>(
            name: "Department",
            table: "HclCs_Users",
            type: "character varying(255)",
            maxLength: 255,
            nullable: true);
        migrationBuilder.AddColumn<string>(
            name: "AuthenticationSource",
            table: "HclCs_Users",
            type: "character varying(32)",
            maxLength: 32,
            nullable: true);
        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "DirectoryLastValidatedAt",
            table: "HclCs_Users",
            type: "timestamp with time zone",
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
            """);

        migrationBuilder.CreateIndex(
            name: "IX_USERS_EMPLOYEE_ID",
            table: "HclCs_Users",
            column: "EmployeeId",
            filter: "\"EmployeeId\" IS NOT NULL");
        migrationBuilder.CreateIndex(
            name: "UX_USERS_DIRECTORY_IMMUTABLE_ID",
            table: "HclCs_Users",
            column: "DirectoryImmutableId",
            unique: true,
            filter: "\"DirectoryImmutableId\" IS NOT NULL");
        migrationBuilder.CreateIndex(
            name: "UX_USERS_USER_PRINCIPAL_NAME",
            table: "HclCs_Users",
            column: "UserPrincipalName",
            unique: true,
            filter: "\"UserPrincipalName\" IS NOT NULL");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_USERS_EMPLOYEE_ID",
            table: "HclCs_Users");
        migrationBuilder.DropIndex(
            name: "UX_USERS_DIRECTORY_IMMUTABLE_ID",
            table: "HclCs_Users");
        migrationBuilder.DropIndex(
            name: "UX_USERS_USER_PRINCIPAL_NAME",
            table: "HclCs_Users");

        migrationBuilder.DropColumn(name: "AuthenticationSource", table: "HclCs_Users");
        migrationBuilder.DropColumn(name: "Department", table: "HclCs_Users");
        migrationBuilder.DropColumn(name: "DirectoryImmutableId", table: "HclCs_Users");
        migrationBuilder.DropColumn(name: "DirectoryLastValidatedAt", table: "HclCs_Users");
        migrationBuilder.DropColumn(name: "DisplayName", table: "HclCs_Users");
        migrationBuilder.DropColumn(name: "EmployeeId", table: "HclCs_Users");
        migrationBuilder.DropColumn(name: "UserPrincipalName", table: "HclCs_Users");
    }
}
