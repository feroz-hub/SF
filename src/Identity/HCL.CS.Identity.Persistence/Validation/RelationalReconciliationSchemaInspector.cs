/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
 */

using System.Data;
using System.Data.Common;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace HCL.CS.Infrastructure.Data.Validation;

public sealed class RelationalReconciliationSchemaInspector : IReconciliationSchemaInspector
{
    private readonly DbContext dbContext;

    public RelationalReconciliationSchemaInspector(DbContext dbContext)
    {
        this.dbContext = dbContext;
        ProviderName = dbContext.Database.ProviderName ?? "Unknown";
        SupportsProvider = dbContext.Database.IsNpgsql() || dbContext.Database.IsSqlite();
    }

    public string ProviderName { get; }
    public bool SupportsProvider { get; }

    public async Task<SchemaInspectionResult> InspectAsync(
        IReadOnlyList<string> migrationIds,
        CancellationToken cancellationToken = default)
    {
        if (!SupportsProvider)
        {
            return BuildResult(
                new[]
                {
                    new SchemaDifference(
                        "UnsupportedProvider",
                        ProviderName,
                        "PostgreSQL or SQLite",
                        ProviderName)
                },
                new[] { $"Provider '{ProviderName}' is not approved for reconciliation." },
                new[] { ProviderName });
        }

        var connection = dbContext.Database.GetDbConnection();
        var closeWhenComplete = connection.State != ConnectionState.Open;
        if (closeWhenComplete) await connection.OpenAsync(cancellationToken);
        try
        {
            return dbContext.Database.IsNpgsql()
                ? await InspectPostgreSqlAsync(connection, migrationIds, cancellationToken)
                : await InspectSqliteAsync(connection, migrationIds, cancellationToken);
        }
        finally
        {
            if (closeWhenComplete) await connection.CloseAsync();
        }
    }

    private async Task<SchemaInspectionResult> InspectPostgreSqlAsync(
        DbConnection connection,
        IReadOnlyList<string> migrationIds,
        CancellationToken cancellationToken)
    {
        var differences = new List<SchemaDifference>();
        var logs = new List<string> { "Inspecting PostgreSQL physical schema in the current schema." };
        var facts = new List<string> { ProviderName };

        if (migrationIds.Contains(DatabaseReconciliationService.Phase2MigrationId, StringComparer.Ordinal))
        {
            foreach (var column in Phase2Columns(postgreSql: true))
                await ValidatePostgreSqlColumnAsync(connection, column, differences, facts, cancellationToken);
            foreach (var index in Phase2Indexes(postgreSql: true))
                await ValidatePostgreSqlIndexAsync(connection, index, differences, facts, cancellationToken);
            await ValidateNoDuplicatesAsync(
                connection,
                "HclCs_Users",
                "DirectoryImmutableId",
                differences,
                facts,
                cancellationToken);
            await ValidateNoDuplicatesAsync(
                connection,
                "HclCs_Users",
                "UserPrincipalName",
                differences,
                facts,
                cancellationToken);
            logs.Add("Phase 2 columns, lengths, nullability, indexes, filters, and data compatibility inspected.");
        }

        if (migrationIds.Contains(DatabaseReconciliationService.Phase2BMigrationId, StringComparer.Ordinal))
        {
            await ValidatePostgreSqlColumnAsync(
                connection,
                new ColumnRequirement("HclCs_Users", "NormalizedEmail", "character varying", 255, null, null, false, null),
                differences,
                facts,
                cancellationToken);
            await ValidatePostgreSqlIndexAsync(
                connection,
                new IndexRequirement("HclCs_Users", "EmailIndex", new[] { "NormalizedEmail" }, true, null),
                differences,
                facts,
                cancellationToken);
            await ValidateNoDuplicatesAsync(
                connection,
                "HclCs_Users",
                "NormalizedEmail",
                differences,
                facts,
                cancellationToken);
            logs.Add("Phase 2B normalized-email column, unique index, PostgreSQL null semantics, and duplicate data inspected.");
        }

        if (migrationIds.Contains(DatabaseReconciliationService.Phase2CMigrationId, StringComparer.Ordinal))
        {
            foreach (var column in Phase2CColumns(postgreSql: true))
                await ValidatePostgreSqlColumnAsync(connection, column, differences, facts, cancellationToken);
            foreach (var primaryKey in Phase2CPrimaryKeys())
                await ValidatePostgreSqlPrimaryKeyAsync(connection, primaryKey, differences, facts, cancellationToken);
            foreach (var index in Phase2CIndexes())
                await ValidatePostgreSqlIndexAsync(connection, index, differences, facts, cancellationToken);
            await ValidatePostgreSqlForeignKeyAsync(
                connection,
                Phase2CExternalIdentityForeignKey(),
                differences,
                facts,
                cancellationToken);
            logs.Add("Phase 2C tables, all migration columns, primary keys, indexes, foreign key, and delete behaviour inspected.");
        }

        return BuildResult(differences, logs, facts);
    }

    private async Task<SchemaInspectionResult> InspectSqliteAsync(
        DbConnection connection,
        IReadOnlyList<string> migrationIds,
        CancellationToken cancellationToken)
    {
        var differences = new List<SchemaDifference>();
        var logs = new List<string>
        {
            "Inspecting SQLite physical schema.",
            "SQLite does not preserve declared string-length or numeric precision metadata; affinity, constraints, indexes, and data compatibility are verified."
        };
        var facts = new List<string> { ProviderName };

        if (migrationIds.Contains(DatabaseReconciliationService.Phase2MigrationId, StringComparer.Ordinal))
        {
            foreach (var column in Phase2Columns(postgreSql: false))
                await ValidateSqliteColumnAsync(connection, column, differences, facts, cancellationToken);
            foreach (var index in Phase2Indexes(postgreSql: false))
                await ValidateSqliteIndexAsync(connection, index, differences, facts, cancellationToken);
            await ValidateNoDuplicatesAsync(
                connection,
                "HclCs_Users",
                "DirectoryImmutableId",
                differences,
                facts,
                cancellationToken);
            await ValidateNoDuplicatesAsync(
                connection,
                "HclCs_Users",
                "UserPrincipalName",
                differences,
                facts,
                cancellationToken);
        }

        if (migrationIds.Contains(DatabaseReconciliationService.Phase2BMigrationId, StringComparer.Ordinal))
        {
            await ValidateSqliteColumnAsync(
                connection,
                new ColumnRequirement("HclCs_Users", "NormalizedEmail", "TEXT", null, null, null, false, null),
                differences,
                facts,
                cancellationToken);
            await ValidateSqliteIndexAsync(
                connection,
                new IndexRequirement("HclCs_Users", "EmailIndex", new[] { "NormalizedEmail" }, true, null),
                differences,
                facts,
                cancellationToken);
            await ValidateNoDuplicatesAsync(
                connection,
                "HclCs_Users",
                "NormalizedEmail",
                differences,
                facts,
                cancellationToken);
        }

        if (migrationIds.Contains(DatabaseReconciliationService.Phase2CMigrationId, StringComparer.Ordinal))
        {
            foreach (var column in Phase2CColumns(postgreSql: false))
                await ValidateSqliteColumnAsync(connection, column, differences, facts, cancellationToken);
            foreach (var primaryKey in Phase2CPrimaryKeys())
                await ValidateSqlitePrimaryKeyAsync(connection, primaryKey, differences, facts, cancellationToken);
            foreach (var index in Phase2CIndexes())
                await ValidateSqliteIndexAsync(connection, index, differences, facts, cancellationToken);
            await ValidateSqliteForeignKeyAsync(
                connection,
                Phase2CExternalIdentityForeignKey(),
                differences,
                facts,
                cancellationToken);
        }

        logs.Add("SQLite Phase 2, Phase 2B, and Phase 2C physical schema inspection completed.");
        return BuildResult(differences, logs, facts);
    }

    private async Task ValidatePostgreSqlColumnAsync(
        DbConnection connection,
        ColumnRequirement expected,
        ICollection<SchemaDifference> differences,
        ICollection<string> facts,
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT data_type, character_maximum_length, numeric_precision, numeric_scale,
                   is_nullable, column_default
            FROM information_schema.columns
            WHERE table_schema = current_schema()
              AND table_name = @table
              AND column_name = @column
            """;
        var row = await QuerySingleAsync(
            connection,
            sql,
            new Dictionary<string, object> { ["@table"] = expected.Table, ["@column"] = expected.Name },
            cancellationToken);
        var objectName = $"{expected.Table}.{expected.Name}";
        if (row is null)
        {
            differences.Add(new SchemaDifference("MissingColumn", objectName, expected.DataType, "missing"));
            facts.Add($"{objectName}=missing");
            return;
        }

        var actualType = ReadString(row, "data_type");
        var actualLength = ReadNullableInt(row, "character_maximum_length");
        var actualPrecision = ReadNullableInt(row, "numeric_precision");
        var actualScale = ReadNullableInt(row, "numeric_scale");
        var actualNullable = string.Equals(ReadString(row, "is_nullable"), "YES", StringComparison.Ordinal);
        var actualDefault = ReadNullableString(row, "column_default");
        facts.Add($"{objectName}:{actualType}:{actualLength}:{actualPrecision}:{actualScale}:{actualNullable}:{actualDefault}");

        AddMismatch(differences, "WrongDataType", objectName, expected.DataType, actualType);
        AddMismatch(differences, "WrongLength", objectName, expected.MaxLength, actualLength);
        AddMismatch(differences, "WrongPrecision", objectName, expected.Precision, actualPrecision);
        AddMismatch(differences, "WrongScale", objectName, expected.Scale, actualScale);
        AddMismatch(differences, "WrongNullability", objectName, expected.Nullable, actualNullable);
        if (!DefaultMatches(expected.DefaultContains, actualDefault))
        {
            differences.Add(new SchemaDifference(
                "WrongDefault",
                objectName,
                expected.DefaultContains ?? "none",
                actualDefault ?? "none"));
        }
    }

    private async Task ValidateSqliteColumnAsync(
        DbConnection connection,
        ColumnRequirement expected,
        ICollection<SchemaDifference> differences,
        ICollection<string> facts,
        CancellationToken cancellationToken)
    {
        var rows = await QueryAsync(
            connection,
            $"PRAGMA table_info({QuoteSqliteIdentifier(expected.Table)})",
            null,
            cancellationToken);
        var row = rows.FirstOrDefault(candidate =>
            string.Equals(ReadString(candidate, "name"), expected.Name, StringComparison.Ordinal));
        var objectName = $"{expected.Table}.{expected.Name}";
        if (row is null)
        {
            differences.Add(new SchemaDifference("MissingColumn", objectName, expected.DataType, "missing"));
            facts.Add($"{objectName}=missing");
            return;
        }

        var actualType = ReadString(row, "type").ToUpperInvariant();
        var actualNullable = ReadInt(row, "notnull") == 0;
        var actualDefault = ReadNullableString(row, "dflt_value");
        facts.Add($"{objectName}:{actualType}:{actualNullable}:{actualDefault}");
        AddMismatch(differences, "WrongDataType", objectName, expected.DataType.ToUpperInvariant(), actualType);
        AddMismatch(differences, "WrongNullability", objectName, expected.Nullable, actualNullable);
        if (!DefaultMatches(expected.DefaultContains, actualDefault))
        {
            differences.Add(new SchemaDifference(
                "WrongDefault",
                objectName,
                expected.DefaultContains ?? "none",
                actualDefault ?? "none"));
        }
    }

    private async Task ValidatePostgreSqlPrimaryKeyAsync(
        DbConnection connection,
        PrimaryKeyRequirement expected,
        ICollection<SchemaDifference> differences,
        ICollection<string> facts,
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT tc.constraint_name,
                   string_agg(kcu.column_name, ',' ORDER BY kcu.ordinal_position) AS columns
            FROM information_schema.table_constraints tc
            JOIN information_schema.key_column_usage kcu
              ON tc.constraint_name = kcu.constraint_name
             AND tc.table_schema = kcu.table_schema
            WHERE tc.table_schema = current_schema()
              AND tc.table_name = @table
              AND tc.constraint_type = 'PRIMARY KEY'
            GROUP BY tc.constraint_name
            """;
        var row = await QuerySingleAsync(
            connection,
            sql,
            new Dictionary<string, object> { ["@table"] = expected.Table },
            cancellationToken);
        var actual = row is null
            ? "missing"
            : $"{ReadString(row, "constraint_name")}:{ReadString(row, "columns")}";
        facts.Add($"pk:{expected.Table}:{actual}");
        var expectedValue = $"{expected.Name}:{string.Join(",", expected.Columns)}";
        if (!string.Equals(expectedValue, actual, StringComparison.Ordinal))
            differences.Add(new SchemaDifference("MissingPrimaryKey", expected.Table, expectedValue, actual));
    }

    private async Task ValidateSqlitePrimaryKeyAsync(
        DbConnection connection,
        PrimaryKeyRequirement expected,
        ICollection<SchemaDifference> differences,
        ICollection<string> facts,
        CancellationToken cancellationToken)
    {
        var rows = await QueryAsync(
            connection,
            $"PRAGMA table_info({QuoteSqliteIdentifier(expected.Table)})",
            null,
            cancellationToken);
        var columns = rows
            .Where(row => ReadInt(row, "pk") > 0)
            .OrderBy(row => ReadInt(row, "pk"))
            .Select(row => ReadString(row, "name"))
            .ToArray();
        facts.Add($"pk:{expected.Table}:{string.Join(",", columns)}");
        if (!columns.SequenceEqual(expected.Columns, StringComparer.Ordinal))
        {
            differences.Add(new SchemaDifference(
                "MissingPrimaryKey",
                expected.Table,
                string.Join(",", expected.Columns),
                columns.Length == 0 ? "missing" : string.Join(",", columns)));
        }
    }

    private async Task ValidatePostgreSqlIndexAsync(
        DbConnection connection,
        IndexRequirement expected,
        ICollection<SchemaDifference> differences,
        ICollection<string> facts,
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT indexdef
            FROM pg_indexes
            WHERE schemaname = current_schema()
              AND tablename = @table
              AND indexname = @index
            """;
        var row = await QuerySingleAsync(
            connection,
            sql,
            new Dictionary<string, object> { ["@table"] = expected.Table, ["@index"] = expected.Name },
            cancellationToken);
        if (row is null)
        {
            differences.Add(new SchemaDifference("MissingIndex", expected.Name, DescribeIndex(expected), "missing"));
            facts.Add($"index:{expected.Name}=missing");
            return;
        }

        var definition = ReadString(row, "indexdef");
        facts.Add($"index:{expected.Name}:{definition}");
        var isUnique = definition.Contains("CREATE UNIQUE INDEX", StringComparison.OrdinalIgnoreCase);
        if (isUnique != expected.Unique)
            differences.Add(new SchemaDifference("WrongIndexUniqueness", expected.Name, expected.Unique.ToString(), isUnique.ToString()));
        var orderedColumns = ExtractPostgreSqlIndexColumns(definition);
        if (!orderedColumns.SequenceEqual(expected.Columns, StringComparer.Ordinal))
            differences.Add(new SchemaDifference("WrongIndexColumns", expected.Name, string.Join(",", expected.Columns), string.Join(",", orderedColumns)));
        var actualFilter = ExtractPostgreSqlIndexFilter(definition);
        if (!FiltersMatch(expected.Filter, actualFilter))
            differences.Add(new SchemaDifference("WrongIndexFilter", expected.Name, expected.Filter ?? "none", actualFilter ?? "none"));
    }

    private async Task ValidateSqliteIndexAsync(
        DbConnection connection,
        IndexRequirement expected,
        ICollection<SchemaDifference> differences,
        ICollection<string> facts,
        CancellationToken cancellationToken)
    {
        var indexes = await QueryAsync(
            connection,
            $"PRAGMA index_list({QuoteSqliteIdentifier(expected.Table)})",
            null,
            cancellationToken);
        var index = indexes.FirstOrDefault(row =>
            string.Equals(ReadString(row, "name"), expected.Name, StringComparison.Ordinal));
        if (index is null)
        {
            differences.Add(new SchemaDifference("MissingIndex", expected.Name, DescribeIndex(expected), "missing"));
            facts.Add($"index:{expected.Name}=missing");
            return;
        }

        var isUnique = ReadInt(index, "unique") == 1;
        var columns = (await QueryAsync(
                connection,
                $"PRAGMA index_info({QuoteSqliteIdentifier(expected.Name)})",
                null,
                cancellationToken))
            .OrderBy(row => ReadInt(row, "seqno"))
            .Select(row => ReadString(row, "name"))
            .ToArray();
        var sqlRow = await QuerySingleAsync(
            connection,
            "SELECT sql FROM sqlite_master WHERE type = 'index' AND name = @name",
            new Dictionary<string, object> { ["@name"] = expected.Name },
            cancellationToken);
        var definition = sqlRow is null ? string.Empty : ReadNullableString(sqlRow, "sql") ?? string.Empty;
        var filter = definition.Contains(" WHERE ", StringComparison.OrdinalIgnoreCase)
            ? definition[(definition.IndexOf(" WHERE ", StringComparison.OrdinalIgnoreCase) + 7)..].Trim()
            : null;
        facts.Add($"index:{expected.Name}:{isUnique}:{string.Join(",", columns)}:{filter}");
        if (isUnique != expected.Unique)
            differences.Add(new SchemaDifference("WrongIndexUniqueness", expected.Name, expected.Unique.ToString(), isUnique.ToString()));
        if (!columns.SequenceEqual(expected.Columns, StringComparer.Ordinal))
            differences.Add(new SchemaDifference("WrongIndexColumns", expected.Name, string.Join(",", expected.Columns), string.Join(",", columns)));
        if (!FiltersMatch(expected.Filter, filter))
            differences.Add(new SchemaDifference("WrongIndexFilter", expected.Name, expected.Filter ?? "none", filter ?? "none"));
    }

    private async Task ValidatePostgreSqlForeignKeyAsync(
        DbConnection connection,
        ForeignKeyRequirement expected,
        ICollection<SchemaDifference> differences,
        ICollection<string> facts,
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT kcu.column_name, ccu.table_name AS principal_table,
                   ccu.column_name AS principal_column, rc.delete_rule
            FROM information_schema.table_constraints tc
            JOIN information_schema.key_column_usage kcu
              ON tc.constraint_name = kcu.constraint_name
             AND tc.table_schema = kcu.table_schema
            JOIN information_schema.referential_constraints rc
              ON tc.constraint_name = rc.constraint_name
             AND tc.table_schema = rc.constraint_schema
            JOIN information_schema.constraint_column_usage ccu
              ON rc.unique_constraint_name = ccu.constraint_name
             AND rc.unique_constraint_schema = ccu.constraint_schema
            WHERE tc.table_schema = current_schema()
              AND tc.table_name = @table
              AND tc.constraint_name = @name
              AND tc.constraint_type = 'FOREIGN KEY'
            """;
        var row = await QuerySingleAsync(
            connection,
            sql,
            new Dictionary<string, object> { ["@table"] = expected.Table, ["@name"] = expected.Name },
            cancellationToken);
        ValidateForeignKeyRow(expected, row, differences, facts);
    }

    private async Task ValidateSqliteForeignKeyAsync(
        DbConnection connection,
        ForeignKeyRequirement expected,
        ICollection<SchemaDifference> differences,
        ICollection<string> facts,
        CancellationToken cancellationToken)
    {
        var rows = await QueryAsync(
            connection,
            $"PRAGMA foreign_key_list({QuoteSqliteIdentifier(expected.Table)})",
            null,
            cancellationToken);
        var row = rows.FirstOrDefault(candidate =>
            string.Equals(ReadString(candidate, "from"), expected.Column, StringComparison.Ordinal)
            && string.Equals(ReadString(candidate, "table"), expected.PrincipalTable, StringComparison.Ordinal));
        if (row is null)
        {
            ValidateForeignKeyRow(expected, null, differences, facts);
            return;
        }

        var normalized = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
        {
            ["column_name"] = ReadString(row, "from"),
            ["principal_table"] = ReadString(row, "table"),
            ["principal_column"] = ReadString(row, "to"),
            ["delete_rule"] = ReadString(row, "on_delete")
        };
        ValidateForeignKeyRow(expected, normalized, differences, facts);
    }

    private static void ValidateForeignKeyRow(
        ForeignKeyRequirement expected,
        IReadOnlyDictionary<string, object> row,
        ICollection<SchemaDifference> differences,
        ICollection<string> facts)
    {
        if (row is null)
        {
            differences.Add(new SchemaDifference("MissingForeignKey", expected.Name, DescribeForeignKey(expected), "missing"));
            facts.Add($"fk:{expected.Name}=missing");
            return;
        }

        var actual = new ForeignKeyRequirement(
            expected.Name,
            expected.Table,
            ReadString(row, "column_name"),
            ReadString(row, "principal_table"),
            ReadString(row, "principal_column"),
            ReadString(row, "delete_rule"));
        facts.Add($"fk:{expected.Name}:{DescribeForeignKey(actual)}");
        if (!string.Equals(expected.Column, actual.Column, StringComparison.Ordinal)
            || !string.Equals(expected.PrincipalTable, actual.PrincipalTable, StringComparison.Ordinal)
            || !string.Equals(expected.PrincipalColumn, actual.PrincipalColumn, StringComparison.Ordinal))
        {
            differences.Add(new SchemaDifference("MissingForeignKey", expected.Name, DescribeForeignKey(expected), DescribeForeignKey(actual)));
        }
        if (!string.Equals(expected.DeleteRule, actual.DeleteRule, StringComparison.OrdinalIgnoreCase))
        {
            differences.Add(new SchemaDifference("WrongDeleteBehaviour", expected.Name, expected.DeleteRule, actual.DeleteRule));
        }
    }

    private async Task ValidateNoDuplicatesAsync(
        DbConnection connection,
        string table,
        string column,
        ICollection<SchemaDifference> differences,
        ICollection<string> facts,
        CancellationToken cancellationToken)
    {
        var sql =
            $"SELECT COUNT(*) AS duplicate_groups FROM (" +
            $"SELECT {QuoteIdentifier(column)} FROM {QuoteIdentifier(table)} " +
            $"WHERE {QuoteIdentifier(column)} IS NOT NULL " +
            $"GROUP BY {QuoteIdentifier(column)} HAVING COUNT(*) > 1) duplicate_values";
        var row = await QuerySingleAsync(connection, sql, null, cancellationToken);
        var duplicateGroups = row is null ? 0 : ReadInt(row, "duplicate_groups");
        facts.Add($"duplicates:{table}.{column}:{duplicateGroups}");
        if (duplicateGroups > 0)
        {
            differences.Add(new SchemaDifference(
                "DuplicateData",
                $"{table}.{column}",
                "0 duplicate groups",
                $"{duplicateGroups} duplicate group(s)"));
        }
    }

    private async Task<IReadOnlyDictionary<string, object>> QuerySingleAsync(
        DbConnection connection,
        string sql,
        IReadOnlyDictionary<string, object> parameters,
        CancellationToken cancellationToken)
    {
        return (await QueryAsync(connection, sql, parameters, cancellationToken)).FirstOrDefault();
    }

    private async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> QueryAsync(
        DbConnection connection,
        string sql,
        IReadOnlyDictionary<string, object> parameters,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        var currentTransaction = dbContext.Database.CurrentTransaction;
        if (currentTransaction is not null)
            command.Transaction = currentTransaction.GetDbTransaction();
        if (parameters is not null)
        {
            foreach (var parameter in parameters)
            {
                var dbParameter = command.CreateParameter();
                dbParameter.ParameterName = parameter.Key;
                dbParameter.Value = parameter.Value ?? DBNull.Value;
                command.Parameters.Add(dbParameter);
            }
        }

        var rows = new List<IReadOnlyDictionary<string, object>>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var row = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            for (var index = 0; index < reader.FieldCount; index++)
                row[reader.GetName(index)] = reader.IsDBNull(index) ? null : reader.GetValue(index);
            rows.Add(row);
        }
        return rows;
    }

    private static IReadOnlyList<ColumnRequirement> Phase2Columns(bool postgreSql)
    {
        var text = postgreSql ? "character varying" : "TEXT";
        var timestamp = postgreSql ? "timestamp with time zone" : "TEXT";
        return new[]
        {
            new ColumnRequirement("HclCs_Users", "DirectoryImmutableId", text, postgreSql ? 512 : null, null, null, true, null),
            new ColumnRequirement("HclCs_Users", "EmployeeId", text, postgreSql ? 255 : null, null, null, true, null),
            new ColumnRequirement("HclCs_Users", "UserPrincipalName", text, postgreSql ? 255 : null, null, null, true, null),
            new ColumnRequirement("HclCs_Users", "DisplayName", text, postgreSql ? 255 : null, null, null, true, null),
            new ColumnRequirement("HclCs_Users", "Department", text, postgreSql ? 255 : null, null, null, true, null),
            new ColumnRequirement("HclCs_Users", "AuthenticationSource", text, postgreSql ? 32 : null, null, null, true, null),
            new ColumnRequirement("HclCs_Users", "DirectoryLastValidatedAt", timestamp, null, null, null, true, null)
        };
    }

    private static IReadOnlyList<IndexRequirement> Phase2Indexes(bool postgreSql)
    {
        return new[]
        {
            new IndexRequirement(
                "HclCs_Users",
                "IX_USERS_EMPLOYEE_ID",
                new[] { "EmployeeId" },
                false,
                postgreSql ? "\"EmployeeId\" IS NOT NULL" : null),
            new IndexRequirement(
                "HclCs_Users",
                "UX_USERS_DIRECTORY_IMMUTABLE_ID",
                new[] { "DirectoryImmutableId" },
                true,
                postgreSql ? "\"DirectoryImmutableId\" IS NOT NULL" : null),
            new IndexRequirement(
                "HclCs_Users",
                "UX_USERS_USER_PRINCIPAL_NAME",
                new[] { "UserPrincipalName" },
                true,
                postgreSql ? "\"UserPrincipalName\" IS NOT NULL" : null)
        };
    }

    private static IReadOnlyList<ColumnRequirement> Phase2CColumns(bool postgreSql)
    {
        var uuid = postgreSql ? "uuid" : "TEXT";
        var text = postgreSql ? "text" : "TEXT";
        var varchar = postgreSql ? "character varying" : "TEXT";
        var timestamp = postgreSql ? "timestamp with time zone" : "TEXT";
        var boolean = postgreSql ? "boolean" : "INTEGER";
        var integer = postgreSql ? "integer" : "INTEGER";
        var bytes = postgreSql ? "bytea" : "BLOB";
        int? Length(int value) => postgreSql ? value : null;

        return new[]
        {
            new ColumnRequirement("HclCs_SecurityTokens", "ConsumedAt", timestamp, null, null, null, true, null),
            new ColumnRequirement("HclCs_SecurityTokens", "TokenReuseDetected", boolean, null, null, null, false, postgreSql ? "false" : "0"),
            new ColumnRequirement("HclCs_Clients", "PreferredAudience", varchar, Length(300), null, null, true, null),

            new ColumnRequirement("HclCs_ExternalIdentities", "Id", uuid, null, null, null, false, null),
            new ColumnRequirement("HclCs_ExternalIdentities", "IsDeleted", boolean, null, null, null, false, postgreSql ? "false" : "0"),
            new ColumnRequirement("HclCs_ExternalIdentities", "CreatedOn", timestamp, null, null, null, false, null),
            new ColumnRequirement("HclCs_ExternalIdentities", "ModifiedOn", timestamp, null, null, null, true, null),
            new ColumnRequirement("HclCs_ExternalIdentities", "CreatedBy", varchar, Length(255), null, null, false, null),
            new ColumnRequirement("HclCs_ExternalIdentities", "ModifiedBy", varchar, Length(255), null, null, true, null),
            new ColumnRequirement("HclCs_ExternalIdentities", "RowVersion", bytes, null, null, null, true, null),
            new ColumnRequirement("HclCs_ExternalIdentities", "UserId", uuid, null, null, null, false, null),
            new ColumnRequirement("HclCs_ExternalIdentities", "TenantId", varchar, Length(128), null, null, true, null),
            new ColumnRequirement("HclCs_ExternalIdentities", "Provider", varchar, Length(64), null, null, false, null),
            new ColumnRequirement("HclCs_ExternalIdentities", "Issuer", varchar, Length(256), null, null, false, null),
            new ColumnRequirement("HclCs_ExternalIdentities", "Subject", varchar, Length(256), null, null, false, null),
            new ColumnRequirement("HclCs_ExternalIdentities", "Email", varchar, Length(255), null, null, false, null),
            new ColumnRequirement("HclCs_ExternalIdentities", "EmailVerified", boolean, null, null, null, false, null),
            new ColumnRequirement("HclCs_ExternalIdentities", "LinkedAt", timestamp, null, null, null, false, null),
            new ColumnRequirement("HclCs_ExternalIdentities", "LastSignInAt", timestamp, null, null, null, true, null),

            new ColumnRequirement("HclCs_NotificationProviderConfig", "Id", uuid, null, null, null, false, null),
            new ColumnRequirement("HclCs_NotificationProviderConfig", "IsDeleted", boolean, null, null, null, false, postgreSql ? "false" : "0"),
            new ColumnRequirement("HclCs_NotificationProviderConfig", "CreatedOn", timestamp, null, null, null, false, null),
            new ColumnRequirement("HclCs_NotificationProviderConfig", "ModifiedOn", timestamp, null, null, null, true, null),
            new ColumnRequirement("HclCs_NotificationProviderConfig", "CreatedBy", varchar, Length(255), null, null, false, null),
            new ColumnRequirement("HclCs_NotificationProviderConfig", "ModifiedBy", varchar, Length(255), null, null, true, null),
            new ColumnRequirement("HclCs_NotificationProviderConfig", "ProviderName", varchar, Length(50), null, null, false, null),
            new ColumnRequirement("HclCs_NotificationProviderConfig", "ChannelType", integer, null, 32, 0, false, null),
            new ColumnRequirement("HclCs_NotificationProviderConfig", "IsActive", boolean, null, null, null, false, null),
            new ColumnRequirement("HclCs_NotificationProviderConfig", "ConfigJson", text, null, null, null, false, null),
            new ColumnRequirement("HclCs_NotificationProviderConfig", "LastTestedOn", timestamp, null, null, null, true, null),
            new ColumnRequirement("HclCs_NotificationProviderConfig", "LastTestSuccess", boolean, null, null, null, true, null),

            new ColumnRequirement("HclCs_ExternalAuthProviderConfig", "Id", uuid, null, null, null, false, null),
            new ColumnRequirement("HclCs_ExternalAuthProviderConfig", "IsDeleted", boolean, null, null, null, false, postgreSql ? "false" : "0"),
            new ColumnRequirement("HclCs_ExternalAuthProviderConfig", "CreatedOn", timestamp, null, null, null, false, null),
            new ColumnRequirement("HclCs_ExternalAuthProviderConfig", "ModifiedOn", timestamp, null, null, null, true, null),
            new ColumnRequirement("HclCs_ExternalAuthProviderConfig", "CreatedBy", varchar, Length(255), null, null, false, null),
            new ColumnRequirement("HclCs_ExternalAuthProviderConfig", "ModifiedBy", varchar, Length(255), null, null, true, null),
            new ColumnRequirement("HclCs_ExternalAuthProviderConfig", "ProviderName", varchar, Length(50), null, null, false, null),
            new ColumnRequirement("HclCs_ExternalAuthProviderConfig", "ProviderType", integer, null, 32, 0, false, null),
            new ColumnRequirement("HclCs_ExternalAuthProviderConfig", "IsEnabled", boolean, null, null, null, false, null),
            new ColumnRequirement("HclCs_ExternalAuthProviderConfig", "ConfigJson", text, null, null, null, false, null),
            new ColumnRequirement("HclCs_ExternalAuthProviderConfig", "AutoProvisionEnabled", boolean, null, null, null, false, postgreSql ? "false" : "0"),
            new ColumnRequirement("HclCs_ExternalAuthProviderConfig", "AllowedDomains", varchar, Length(2000), null, null, true, null),
            new ColumnRequirement("HclCs_ExternalAuthProviderConfig", "LastTestedOn", timestamp, null, null, null, true, null),
            new ColumnRequirement("HclCs_ExternalAuthProviderConfig", "LastTestSuccess", boolean, null, null, null, true, null)
        };
    }

    private static IReadOnlyList<PrimaryKeyRequirement> Phase2CPrimaryKeys()
    {
        return new[]
        {
            new PrimaryKeyRequirement("HclCs_ExternalIdentities", "PK_HclCs_ExternalIdentities", new[] { "Id" }),
            new PrimaryKeyRequirement("HclCs_NotificationProviderConfig", "PK_HclCs_NotificationProviderConfig", new[] { "Id" }),
            new PrimaryKeyRequirement("HclCs_ExternalAuthProviderConfig", "PK_HclCs_ExternalAuthProviderConfig", new[] { "Id" })
        };
    }

    private static IReadOnlyList<IndexRequirement> Phase2CIndexes()
    {
        return new[]
        {
            new IndexRequirement("HclCs_ExternalIdentities", "IX_EXTID_PROVIDER_ISSUER_SUBJECT", new[] { "Provider", "Issuer", "Subject" }, true, null),
            new IndexRequirement("HclCs_ExternalIdentities", "IX_EXTID_USERID", new[] { "UserId" }, false, null),
            new IndexRequirement("HclCs_ExternalIdentities", "IX_EXTID_TENANT_EMAIL", new[] { "TenantId", "Email" }, false, null),
            new IndexRequirement("HclCs_NotificationProviderConfig", "IX_NPC_CHANNEL_TYPE", new[] { "ChannelType" }, false, null),
            new IndexRequirement("HclCs_NotificationProviderConfig", "IX_NPC_CHANNEL_ACTIVE", new[] { "ChannelType", "IsActive" }, false, null),
            new IndexRequirement("HclCs_ExternalAuthProviderConfig", "IX_EAPC_PROVIDER", new[] { "ProviderName" }, true, null),
            new IndexRequirement("HclCs_ExternalAuthProviderConfig", "IX_EAPC_PROVIDER_ENABLED", new[] { "ProviderName", "IsEnabled" }, false, null)
        };
    }

    private static ForeignKeyRequirement Phase2CExternalIdentityForeignKey()
    {
        return new ForeignKeyRequirement(
            "FK_HclCs_ExternalIdentities_HclCs_Users_UserId",
            "HclCs_ExternalIdentities",
            "UserId",
            "HclCs_Users",
            "Id",
            "RESTRICT");
    }

    private static SchemaInspectionResult BuildResult(
        IEnumerable<SchemaDifference> differences,
        IEnumerable<string> logs,
        IEnumerable<string> facts)
    {
        var differenceArray = differences
            .OrderBy(difference => difference.Kind, StringComparer.Ordinal)
            .ThenBy(difference => difference.ObjectName, StringComparer.Ordinal)
            .ToArray();
        var fingerprintInput = string.Join(
            "\n",
            facts.OrderBy(fact => fact, StringComparer.Ordinal));
        return new SchemaInspectionResult
        {
            Differences = differenceArray,
            VerificationLogs = logs.ToArray(),
            Fingerprint = Convert.ToHexString(
                    SHA256.HashData(Encoding.UTF8.GetBytes(fingerprintInput)))
                .ToLowerInvariant()
        };
    }

    private static void AddMismatch<T>(
        ICollection<SchemaDifference> differences,
        string kind,
        string objectName,
        T expected,
        T actual)
    {
        if (expected is null) return;
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
            differences.Add(new SchemaDifference(kind, objectName, expected.ToString(), actual?.ToString() ?? "none"));
    }

    private static bool DefaultMatches(string expectedContains, string actual)
    {
        if (expectedContains is null) return string.IsNullOrWhiteSpace(actual);
        return actual?.Contains(expectedContains, StringComparison.OrdinalIgnoreCase) == true;
    }

    private static bool FiltersMatch(string expected, string actual)
    {
        return string.Equals(NormalizeSql(expected), NormalizeSql(actual), StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeSql(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : new string(value
                .Where(character => !char.IsWhiteSpace(character)
                                    && character is not '"' and not '(' and not ')')
                .ToArray());
    }

    private static string[] ExtractPostgreSqlIndexColumns(string definition)
    {
        var open = definition.IndexOf('(');
        var close = definition.IndexOf(')', open + 1);
        if (open < 0 || close < 0) return Array.Empty<string>();
        return definition[(open + 1)..close]
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(column => column.Trim('"'))
            .ToArray();
    }

    private static string ExtractPostgreSqlIndexFilter(string definition)
    {
        var whereIndex = definition.IndexOf(" WHERE ", StringComparison.OrdinalIgnoreCase);
        return whereIndex < 0 ? null : definition[(whereIndex + 7)..].Trim();
    }

    private static string DescribeIndex(IndexRequirement requirement)
    {
        return $"{(requirement.Unique ? "unique " : string.Empty)}({string.Join(",", requirement.Columns)})"
               + (requirement.Filter is null ? string.Empty : $" WHERE {requirement.Filter}");
    }

    private static string DescribeForeignKey(ForeignKeyRequirement requirement)
    {
        return $"{requirement.Table}.{requirement.Column}->{requirement.PrincipalTable}.{requirement.PrincipalColumn} DELETE {requirement.DeleteRule}";
    }

    private static string QuoteIdentifier(string identifier)
    {
        return $"\"{identifier.Replace("\"", "\"\"")}\"";
    }

    private static string QuoteSqliteIdentifier(string identifier)
    {
        return $"'{identifier.Replace("'", "''")}'";
    }

    private static string ReadString(IReadOnlyDictionary<string, object> row, string name)
    {
        return row.TryGetValue(name, out var value) ? value?.ToString() ?? string.Empty : string.Empty;
    }

    private static string ReadNullableString(IReadOnlyDictionary<string, object> row, string name)
    {
        return row.TryGetValue(name, out var value) ? value?.ToString() : null;
    }

    private static int ReadInt(IReadOnlyDictionary<string, object> row, string name)
    {
        return row.TryGetValue(name, out var value) && value is not null
            ? Convert.ToInt32(value)
            : 0;
    }

    private static int? ReadNullableInt(IReadOnlyDictionary<string, object> row, string name)
    {
        return row.TryGetValue(name, out var value) && value is not null
            ? Convert.ToInt32(value)
            : null;
    }

    private sealed record ColumnRequirement(
        string Table,
        string Name,
        string DataType,
        int? MaxLength,
        int? Precision,
        int? Scale,
        bool Nullable,
        string DefaultContains);

    private sealed record IndexRequirement(
        string Table,
        string Name,
        IReadOnlyList<string> Columns,
        bool Unique,
        string Filter);

    private sealed record PrimaryKeyRequirement(
        string Table,
        string Name,
        IReadOnlyList<string> Columns);

    private sealed record ForeignKeyRequirement(
        string Name,
        string Table,
        string Column,
        string PrincipalTable,
        string PrincipalColumn,
        string DeleteRule);
}
