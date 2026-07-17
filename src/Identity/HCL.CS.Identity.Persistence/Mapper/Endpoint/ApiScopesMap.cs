using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HCL.CS.Domain.Entities.Api;

namespace HCL.CS.Infrastructure.Data.Mapper.Endpoint;

public class ApiScopesMap
{
    public ApiScopesMap(EntityTypeBuilder<ApiScopes> entityBuilder)
    {
        ApiScopesMapping(entityBuilder);
    }

    private void ApiScopesMapping(EntityTypeBuilder<ApiScopes> entityBuilder)
    {
        if (entityBuilder != null)
        {
            entityBuilder.ToTable("HclCs_ApiScopes");

            // Primary Key
            entityBuilder.HasKey(x => x.Id);
            entityBuilder.HasIndex(t => new { t.ApiResourceId, t.Name }).HasDatabaseName("IX_APISCO_SCOID_NAME")
                .IsUnique();

            // Properties
            entityBuilder.Property(x => x.ApiResourceId).IsRequired();

            // Table & Column Mappings
            entityBuilder.Property(x => x.Name).IsRequired().HasMaxLength(255).HasColumnName("Name");
            entityBuilder.Property(x => x.DisplayName).HasMaxLength(255).HasColumnName("DisplayName");
            entityBuilder.Property(x => x.IsDeleted).IsRequired().HasColumnName("IsDeleted");
            entityBuilder.Property(x => x.CreatedOn).IsRequired().HasColumnName("CreatedOn");
            entityBuilder.Property(x => x.CreatedBy).IsRequired().HasMaxLength(255).HasColumnName("CreatedBy");
            entityBuilder.Property(x => x.ModifiedOn).HasColumnName("ModifiedOn");
            entityBuilder.Property(x => x.ModifiedBy).HasMaxLength(255).HasColumnName("ModifiedBy");

            // Mappings
            entityBuilder.HasMany(x => x.ApiScopeClaims).WithOne(x => x.ApiScope)
                .HasForeignKey(x => x.ApiScopeId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            entityBuilder.HasQueryFilter(m => EF.Property<bool>(m, "IsDeleted") == false);
        }
    }
}
