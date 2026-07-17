using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zentra.Domain.Entities.Api;

namespace Zentra.Infrastructure.Data.Mapper.Endpoint;

public class ApiResourcesMap
{
    public ApiResourcesMap(EntityTypeBuilder<ApiResources> entityBuilder)
    {
        ApiResourcesMapping(entityBuilder);
    }

    private void ApiResourcesMapping(EntityTypeBuilder<ApiResources> entityBuilder)
    {
        if (entityBuilder != null)
        {
            entityBuilder.ToTable("Zentra_ApiResources");

            // Primary Key
            entityBuilder.HasKey(x => x.Id);
            entityBuilder.HasIndex(t => t.Name).HasDatabaseName("IX_APIRES_NAME").IsUnique();

            // Properties
            entityBuilder.Property(x => x.Name).HasMaxLength(255).IsRequired();
            entityBuilder.Property(x => x.DisplayName).HasMaxLength(255);

            // Table & Column Mappings
            entityBuilder.Property(x => x.IsDeleted).IsRequired().HasColumnName("IsDeleted");
            entityBuilder.Property(x => x.CreatedOn).IsRequired().HasColumnName("CreatedOn");
            entityBuilder.Property(x => x.CreatedBy).IsRequired().HasMaxLength(255).HasColumnName("CreatedBy");
            entityBuilder.Property(x => x.ModifiedOn).HasColumnName("ModifiedOn");
            entityBuilder.Property(x => x.ModifiedBy).HasMaxLength(255).HasColumnName("ModifiedBy");

            // Mappings
            entityBuilder.HasMany(x => x.ApiResourceClaims).WithOne(x => x.ApiResource)
                .HasForeignKey(x => x.ApiResourceId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            entityBuilder.HasMany(x => x.ApiScopes).WithOne(x => x.ApiResource)
                .HasForeignKey(x => x.ApiResourceId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            entityBuilder.HasQueryFilter(m => EF.Property<bool>(m, "IsDeleted") == false);
        }
    }
}
