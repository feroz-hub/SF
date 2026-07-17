using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HCL.CS.Domain.Entities.Api;

namespace HCL.CS.Infrastructure.Data.Mapper.Api;

public class UserClaimsMap
{
    public UserClaimsMap(EntityTypeBuilder<UserClaims> entityBuilder)
    {
        UserClaimMapping(entityBuilder);
    }

    private void UserClaimMapping(EntityTypeBuilder<UserClaims> entityBuilder)
    {
        if (entityBuilder != null)
        {
            // Primary Key
            entityBuilder.HasKey(t => t.Id);

            // Table & Column Mappings
            entityBuilder.ToTable("HclCs_UserClaims");
            entityBuilder.Property(t => t.Id).HasColumnName("Id");
            entityBuilder.Property(t => t.UserId).IsRequired().HasColumnName("UserId");
            entityBuilder.Property(t => t.ClaimType).HasColumnName("ClaimType");
            entityBuilder.Property(t => t.ClaimValue).HasColumnName("ClaimValue");
            entityBuilder.Property(t => t.IsAdminClaim).HasColumnName("IsAdminClaim");
            entityBuilder.Property(t => t.IsDeleted).IsRequired().HasColumnName("IsDeleted");
            entityBuilder.Property(t => t.CreatedOn).IsRequired().HasColumnName("CreatedOn");
            entityBuilder.Property(t => t.CreatedBy).IsRequired().HasMaxLength(255).HasColumnName("CreatedBy");
            entityBuilder.Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            entityBuilder.Property(t => t.ModifiedBy).HasMaxLength(255).HasColumnName("ModifiedBy");
            entityBuilder.HasQueryFilter(m => EF.Property<bool>(m, "IsDeleted") == false);
        }
    }
}
