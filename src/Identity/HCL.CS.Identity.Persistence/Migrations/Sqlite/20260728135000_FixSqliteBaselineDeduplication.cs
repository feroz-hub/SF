/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.EntityFrameworkCore.Migrations;

namespace HCL.CS.Infrastructure.Data.Migrations.Sqlite;

public partial class FixSqliteBaselineDeduplication : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Corrective SQLite baseline deduplication step:
        // Ensures schema parity between databases created via 2022 baseline vs 2023 baseline before applying Phase 2 profile fields.
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}
