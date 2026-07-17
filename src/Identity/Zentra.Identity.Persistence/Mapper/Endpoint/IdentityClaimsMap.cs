using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zentra.Domain.Entities.Api;

namespace Zentra.Infrastructure.Data.Mapper.Endpoint;

public class IdentityClaimsMap
{
    public IdentityClaimsMap(EntityTypeBuilder<IdentityClaims> entityBuilder)
    {
        IdentityClaimsMapping(entityBuilder);
    }

    private void IdentityClaimsMapping(EntityTypeBuilder<IdentityClaims> entityBuilder)
    {
        if (entityBuilder != null)
        {
            entityBuilder.ToTable("Zentra_IdentityClaims");

            // Primary Key
            entityBuilder.HasKey(x => x.Id);
            entityBuilder.HasIndex(t => new { t.IdentityResourceId, t.Type })
                .HasDatabaseName("IX_IDRESCLM_IDRESID_TYPE").IsUnique();

            // Properties
            entityBuilder.Property(x => x.IdentityResourceId).IsRequired();

            entityBuilder.Property(x => x.Type).HasMaxLength(255).IsRequired();
            entityBuilder.Property(x => x.AliasType).HasMaxLength(255);

            // Table & Column Mappings
            entityBuilder.Property(x => x.IsDeleted).IsRequired().HasColumnName("IsDeleted");
            entityBuilder.Property(x => x.CreatedOn).IsRequired().HasColumnName("CreatedOn");
            entityBuilder.Property(x => x.CreatedBy).IsRequired().HasMaxLength(255).HasColumnName("CreatedBy");
            entityBuilder.Property(x => x.ModifiedOn).HasColumnName("ModifiedOn");
            entityBuilder.Property(x => x.ModifiedBy).HasMaxLength(255).HasColumnName("ModifiedBy");
            entityBuilder.HasQueryFilter(m => EF.Property<bool>(m, "IsDeleted") == false);
        }
    }
}
