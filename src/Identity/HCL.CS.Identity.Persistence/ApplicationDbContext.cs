/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Threading;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Entities.Endpoint;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.DomainServices;
using HCL.CS.Infrastructure.Data.Mapper.Api;
using HCL.CS.Infrastructure.Data.Mapper.Endpoint;

namespace HCL.CS.Infrastructure.Data;

public class ApplicationDbContext :
    IdentityDbContext<Users, Roles, Guid, UserClaims, UserRoles, UserLogins, RoleClaims, UserTokens>,
    IApplicationDbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ClientRedirectUris> RedirectUris { get; set; }

    public virtual DbSet<ClientPostLogoutRedirectUris> PostLogoutRedirectUris { get; set; }
    public virtual DbSet<AuditTrail> AuditTrail { get; set; }

    public virtual DbSet<SecurityQuestions> SecurityQuestions { get; set; }

    public virtual DbSet<UserSecurityQuestions> UserSecurityQuestions { get; set; }

    public virtual DbSet<PasswordHistory> PasswordHistory { get; set; }

    public virtual DbSet<Notification> Notification { get; set; }

    public virtual DbSet<NotificationProviderConfig> NotificationProviderConfig { get; set; }

    public virtual DbSet<ExternalAuthProviderConfig> ExternalAuthProviderConfig { get; set; }

    public virtual DbSet<ExternalIdentities> ExternalIdentities { get; set; }

    public virtual DbSet<Clients> Clients { get; set; }

    public override DbSet<Roles> Roles { get; set; }

    public override DbSet<RoleClaims> RoleClaims { get; set; }

    public override DbSet<UserClaims> UserClaims { get; set; }

    public virtual DbSet<ApiResources> ApiResources { get; set; }

    public virtual DbSet<ApiResourceClaims> ApiResourceClaims { get; set; }

    public virtual DbSet<ApiScopes> ApiScopes { get; set; }

    public virtual DbSet<ApiScopeClaims> ApiScopeClaims { get; set; }

    public virtual DbSet<IdentityResources> IdentityResources { get; set; }

    public virtual DbSet<IdentityClaims> IdentityClaims { get; set; }

    public virtual DbSet<SecurityTokens> SecurityTokens { get; set; }

    DbSet<T> IApplicationDbContext.Set<T>()
    {
        return base.Set<T>();
    }

    public new async Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            ApplyAuditState(true);
            ApplySqliteRowVersionValues();
            NormalizeDateTimesToUtc();
            var changes = await base.SaveChangesAsync(true, cancellationToken);
            return BuildResult(changes);
        }
        catch (DbUpdateConcurrencyException)
        {
            return BuildFailedResult(ApiErrorCodes.ConcurrencyFailure, "Concurrency conflict while saving changes.");
        }
        catch (Exception ex)
        {
            return BuildFailedResult(ApiErrorCodes.InvalidOrNullObject, ex.Message);
        }
    }

    public async Task<FrameworkResult> SaveChangesWithHardDeleteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            ApplyAuditState(false);
            ApplySqliteRowVersionValues();
            NormalizeDateTimesToUtc();
            var changes = await base.SaveChangesAsync(true, cancellationToken);
            return BuildResult(changes);
        }
        catch (DbUpdateConcurrencyException)
        {
            return BuildFailedResult(ApiErrorCodes.ConcurrencyFailure, "Concurrency conflict while saving changes.");
        }
        catch (Exception ex)
        {
            return BuildFailedResult(ApiErrorCodes.InvalidOrNullObject, ex.Message);
        }
    }

    public void SetAddedStatus(object entry)
    {
        Entry(entry).State = EntityState.Added;
    }

    public void SetModifiedStatus(object entry)
    {
        Entry(entry).State = EntityState.Modified;
    }

    public void SetPropertyModifiedStatus(object entry, string property)
    {
        var entityEntry = Entry(entry);
        entityEntry.Property(property).IsModified = true;
    }

    public void SetConcurrencyOriginalValue(object entry, string dbConcurrencyStamp)
    {
        var entityEntry = Entry(entry);
        var concurrencyProperty = entityEntry.Metadata.FindProperty("ConcurrencyStamp");
        if (concurrencyProperty != null) entityEntry.Property("ConcurrencyStamp").OriginalValue = dbConcurrencyStamp;
    }

    public void SetConcurrencyStatus(object entry, string dbConcurrencyStamp)
    {
        var entityEntry = Entry(entry);
        SetConcurrencyOriginalValue(entry, dbConcurrencyStamp);
        entityEntry.State = EntityState.Modified;
    }

    public void SetRowVersionStatus(object entry, byte[] dbRowVersionStamp)
    {
        var entityEntry = Entry(entry);
        var rowVersionProperty = entityEntry.Metadata.FindProperty(nameof(BaseEntity.RowVersion));
        if (rowVersionProperty != null)
            entityEntry.Property(nameof(BaseEntity.RowVersion)).OriginalValue = dbRowVersionStamp;

        entityEntry.State = EntityState.Modified;
    }

    public override int SaveChanges()
    {
        ApplySqliteRowVersionValues();
        NormalizeDateTimesToUtc();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplySqliteRowVersionValues();
        NormalizeDateTimesToUtc();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        ApplyAuditState(true);
        ApplySqliteRowVersionValues();
        NormalizeDateTimesToUtc();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            relationship.DeleteBehavior = DeleteBehavior.Restrict;

        // API
        _ = new AuditTrailMap(modelBuilder.Entity<AuditTrail>());
        _ = new UsersMap(modelBuilder.Entity<Users>());
        _ = new SecurityQuestionsMap(modelBuilder.Entity<SecurityQuestions>());
        _ = new UserSecurityQuestionsMap(modelBuilder.Entity<UserSecurityQuestions>());
        _ = new PasswordHistoryMap(modelBuilder.Entity<PasswordHistory>());
        _ = new UserClaimsMap(modelBuilder.Entity<UserClaims>());
        _ = new UserLoginsMap(modelBuilder.Entity<UserLogins>());
        _ = new UserTokensMap(modelBuilder.Entity<UserTokens>());
        _ = new RolesMap(modelBuilder.Entity<Roles>());
        _ = new RoleClaimsMap(modelBuilder.Entity<RoleClaims>());
        _ = new UserRolesMap(modelBuilder.Entity<UserRoles>());
        _ = new NotificationMap(modelBuilder.Entity<Notification>());
        _ = new NotificationProviderConfigMap(modelBuilder.Entity<NotificationProviderConfig>());
        _ = new ExternalAuthProviderConfigMap(modelBuilder.Entity<ExternalAuthProviderConfig>());
        _ = new ExternalIdentitiesMap(modelBuilder.Entity<ExternalIdentities>());

        // Endpoint
        _ = new ClientsMap(modelBuilder.Entity<Clients>());
        _ = new ClientRedirectUrisMap(modelBuilder.Entity<ClientRedirectUris>());
        _ = new ClientPostLogoutRedirectUrisMap(modelBuilder.Entity<ClientPostLogoutRedirectUris>());
        _ = new IdentityResourcesMap(modelBuilder.Entity<IdentityResources>());
        _ = new IdentityClaimsMap(modelBuilder.Entity<IdentityClaims>());
        _ = new ApiResourcesMap(modelBuilder.Entity<ApiResources>());
        _ = new ApiResourceClaimsMap(modelBuilder.Entity<ApiResourceClaims>());
        _ = new ApiScopesMap(modelBuilder.Entity<ApiScopes>());
        _ = new ApiScopeClaimsMap(modelBuilder.Entity<ApiScopeClaims>());
        _ = new SecurityTokensMap(modelBuilder.Entity<SecurityTokens>());

        ApplyNullableStringMapping(modelBuilder);

        if (Database.IsNpgsql())
        {
            ApplyNpgsqlUserDateTimeMapping(modelBuilder);
            ApplyNpgsqlRowVersionMapping(modelBuilder);
        }
        else if (Database.IsSqlite()) ApplySqliteRowVersionMapping(modelBuilder);
    }

    private static void ApplyNullableStringMapping(ModelBuilder modelBuilder)
    {
        // Legacy schema has multiple optional TEXT columns; forcing nullable avoids runtime materialization failures.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        foreach (var property in entityType.GetProperties())
        {
            if (property.ClrType != typeof(string)) continue;

            if (property.IsPrimaryKey() || property.IsForeignKey()) continue;

            property.IsNullable = true;
        }
    }

    private static FrameworkResult BuildResult(int affectedRows)
    {
        if (affectedRows > 0) return new FrameworkResult { Status = ResultStatus.Succeeded };

        return BuildFailedResult(ApiErrorCodes.NoChangesWritten, "No changes written.");
    }

    private static FrameworkResult BuildFailedResult(string code, string description)
    {
        return new FrameworkResult
        {
            Status = ResultStatus.Failed,
            Errors = new List<FrameworkError>
            {
                new()
                {
                    Code = code,
                    Description = description
                }
            }
        };
    }

    private void ApplyAuditState(bool softDelete)
    {
        var utcNow = DateTime.UtcNow;
        var entries = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        foreach (var entry in entries)
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity.CreatedOn == default) entry.Entity.CreatedOn = utcNow;

                if (string.IsNullOrWhiteSpace(entry.Entity.CreatedBy)) entry.Entity.CreatedBy = "System";

                entry.Entity.ModifiedOn = null;
                if (entry.Entity.IsDeleted) entry.Entity.IsDeleted = false;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedOn = utcNow;
                if (string.IsNullOrWhiteSpace(entry.Entity.ModifiedBy))
                    entry.Entity.ModifiedBy = entry.Entity.CreatedBy;
            }
            else if (entry.State == EntityState.Deleted)
            {
                if (softDelete)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.ModifiedOn = utcNow;
                    if (string.IsNullOrWhiteSpace(entry.Entity.ModifiedBy))
                        entry.Entity.ModifiedBy = entry.Entity.CreatedBy;
                }
            }

        var userEntries = ChangeTracker.Entries<Users>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified)
            .ToList();

        foreach (var entry in userEntries)
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity.CreatedOn == default) entry.Entity.CreatedOn = utcNow;

                if (string.IsNullOrWhiteSpace(entry.Entity.CreatedBy)) entry.Entity.CreatedBy = "System";

                entry.Entity.ModifiedOn = null;
                entry.Entity.IsDeleted = false;
            }
            else
            {
                if (entry.Entity.ModifiedOn == null) entry.Entity.ModifiedOn = utcNow;

                if (string.IsNullOrWhiteSpace(entry.Entity.ModifiedBy))
                    entry.Entity.ModifiedBy = entry.Entity.CreatedBy;
            }
    }

    private void NormalizeDateTimesToUtc()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified or EntityState.Deleted)) continue;

            foreach (var property in entry.Properties)
            {
                var clrType = property.Metadata.ClrType;
                if (clrType != typeof(DateTime) && clrType != typeof(DateTime?)) continue;

                if (entry.State == EntityState.Modified && !property.IsModified) continue;

                if (property.CurrentValue is not DateTime value) continue;

                DateTime normalized;
                if (value.Kind == DateTimeKind.Unspecified)
                    normalized = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                else if (value.Kind == DateTimeKind.Local)
                    normalized = value.ToUniversalTime();
                else
                    continue;

                if (normalized.Kind != value.Kind || normalized.Ticks != value.Ticks)
                    property.CurrentValue = normalized;
            }
        }
    }

    private void ApplySqliteRowVersionValues()
    {
        if (!Database.IsSqlite()) return;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified)) continue;

            var rowVersion = entry.Metadata.FindProperty(nameof(BaseEntity.RowVersion));
            if (rowVersion == null) continue;

            var property = entry.Property(nameof(BaseEntity.RowVersion));
            property.CurrentValue = Guid.NewGuid().ToByteArray();
            property.IsModified = entry.State == EntityState.Modified;
        }
    }

    private static void ApplyNpgsqlUserDateTimeMapping(ModelBuilder modelBuilder)
    {
        var user = modelBuilder.Entity<Users>();

        user.Property(x => x.DateOfBirth).HasColumnType("timestamp with time zone");
        user.Property(x => x.LastPasswordChangedDate).HasColumnType("timestamp with time zone");
        user.Property(x => x.LastLoginDateTime).HasColumnType("timestamp with time zone");
        user.Property(x => x.LastLogoutDateTime).HasColumnType("timestamp with time zone");
        user.Property(x => x.CreatedOn).HasColumnType("timestamp with time zone");
        user.Property(x => x.ModifiedOn).HasColumnType("timestamp with time zone");
    }

    private static void ApplyNpgsqlRowVersionMapping(ModelBuilder modelBuilder)
    {
        var converter = new ValueConverter<byte[], long>(
            v => BitConverter.ToInt64(v, 0),
            v => BitConverter.GetBytes(v));

        var comparer = new ValueComparer<byte[]>(
            (left, right) =>
                left == right || (left != null && right != null && left.SequenceEqual(right)),
            value => value == null ? 0 : value.Aggregate(0, (hash, b) => HashCode.Combine(hash, b)),
            value => value == null ? null : value.ToArray());

        ConfigureNpgsqlRowVersion<SecurityQuestions>(modelBuilder, converter, comparer);
        ConfigureNpgsqlRowVersion<UserSecurityQuestions>(modelBuilder, converter, comparer);
        ConfigureNpgsqlRowVersion<PasswordHistory>(modelBuilder, converter, comparer);
        ConfigureNpgsqlRowVersion<UserClaims>(modelBuilder, converter, comparer);
        ConfigureNpgsqlRowVersion<RoleClaims>(modelBuilder, converter, comparer);
        ConfigureNpgsqlRowVersion<UserRoles>(modelBuilder, converter, comparer);
        ConfigureNpgsqlRowVersion<Notification>(modelBuilder, converter, comparer);
        ConfigureNpgsqlRowVersion<Clients>(modelBuilder, converter, comparer);
        ConfigureNpgsqlRowVersion<ClientRedirectUris>(modelBuilder, converter, comparer);
        ConfigureNpgsqlRowVersion<ClientPostLogoutRedirectUris>(modelBuilder, converter, comparer);
        ConfigureNpgsqlRowVersion<IdentityResources>(modelBuilder, converter, comparer);
        ConfigureNpgsqlRowVersion<IdentityClaims>(modelBuilder, converter, comparer);
        ConfigureNpgsqlRowVersion<ApiResources>(modelBuilder, converter, comparer);
        ConfigureNpgsqlRowVersion<ApiResourceClaims>(modelBuilder, converter, comparer);
        ConfigureNpgsqlRowVersion<ApiScopes>(modelBuilder, converter, comparer);
        ConfigureNpgsqlRowVersion<ApiScopeClaims>(modelBuilder, converter, comparer);
        ConfigureNpgsqlRowVersion<SecurityTokens>(modelBuilder, converter, comparer);
    }

    private static void ConfigureNpgsqlRowVersion<TEntity>(
        ModelBuilder modelBuilder,
        ValueConverter<byte[], long> converter,
        ValueComparer<byte[]> comparer)
        where TEntity : class
    {
        var property = modelBuilder.Entity<TEntity>()
            .Property<byte[]>(nameof(BaseEntity.RowVersion))
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .HasConversion(converter);

        property.Metadata.SetValueComparer(comparer);
    }

    private static void ApplySqliteRowVersionMapping(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var rowVersionProperty = entityType.FindProperty(nameof(BaseEntity.RowVersion));
            if (rowVersionProperty == null) continue;

            rowVersionProperty.IsNullable = true;
            rowVersionProperty.ValueGenerated = ValueGenerated.Never;
        }
    }
}
