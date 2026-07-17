using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zentra.Domain.Entities.Api;

namespace Zentra.Infrastructure.Data.Mapper.Endpoint;

public class IdentityResourcesMap
{
    public IdentityResourcesMap(EntityTypeBuilder<IdentityResources> entityBuilder)
    {
        IdentityResourcesMapping(entityBuilder);
    }

    private void IdentityResourcesMapping(EntityTypeBuilder<IdentityResources> entityBuilder)
    {
        if (entityBuilder != null)
        {
            entityBuilder.ToTable("Zentra_IdentityResources");

            // Primary Key
            entityBuilder.HasKey(x => x.Id);
            entityBuilder.HasIndex(t => t.Name).HasDatabaseName("IX_IDRES_NAME").IsUnique();

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
            entityBuilder.HasMany(x => x.IdentityClaims).WithOne(x => x.IdentityResource)
                .HasForeignKey(x => x.IdentityResourceId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            entityBuilder
                .HasQueryFilter(m => EF.Property<bool>(m, "IsDeleted") == false);
        }
    }
}
