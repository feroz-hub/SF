using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HCL.CS.Domain.Entities.Api;

namespace HCL.CS.Infrastructure.Data.Mapper.Api;

public class RolesMap
{
    public RolesMap(EntityTypeBuilder<Roles> entityBuilder)
    {
        RolesMapping(entityBuilder);
    }

    private void RolesMapping(EntityTypeBuilder<Roles> entityBuilder)
    {
        if (entityBuilder != null)
        {
            // Primary Key
            entityBuilder.HasKey(t => t.Id);

            // Properties
            entityBuilder.Property(t => t.Name).IsRequired().HasMaxLength(255);
            entityBuilder.Property(t => t.NormalizedName).IsRequired().HasMaxLength(255);
            entityBuilder.Property(t => t.ConcurrencyStamp).IsRequired().HasMaxLength(255);

            // Table & Column Mappings
            entityBuilder.ToTable("HclCs_Roles");
            entityBuilder.Property(t => t.Id).HasColumnName("Id");
            entityBuilder.Property(t => t.Description).HasColumnName("Description");
            entityBuilder.Property(t => t.IsDeleted).IsRequired().HasColumnName("IsDeleted");
            entityBuilder.Property(t => t.CreatedOn).IsRequired().HasColumnName("CreatedOn");
            entityBuilder.Property(t => t.CreatedBy).IsRequired().HasMaxLength(255);
            entityBuilder.Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            entityBuilder.Property(t => t.ModifiedBy).HasMaxLength(255).HasColumnName("ModifiedBy");
            entityBuilder.HasQueryFilter(m => EF.Property<bool>(m, "IsDeleted") == false);
        }
    }
}
