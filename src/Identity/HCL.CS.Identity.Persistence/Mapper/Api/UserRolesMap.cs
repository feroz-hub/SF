using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HCL.CS.Domain.Entities.Api;

namespace HCL.CS.Infrastructure.Data.Mapper.Api;

public class UserRolesMap
{
    public UserRolesMap(EntityTypeBuilder<UserRoles> entityBuilder)
    {
        UserRoleMapping(entityBuilder);
    }

    private void UserRoleMapping(EntityTypeBuilder<UserRoles> entityBuilder)
    {
        if (entityBuilder != null)
        {
            // Primary Key
            entityBuilder.HasKey(r => new { r.Id, r.UserId, r.RoleId });

            // Properties
            // Table & Column Mappings
            entityBuilder.ToTable("HclCs_UserRoles");
            entityBuilder.Property(t => t.Id).HasColumnName("Id");
            entityBuilder.Property(t => t.IsDeleted).IsRequired().HasColumnName("IsDeleted");
            entityBuilder.Property(t => t.CreatedOn).IsRequired().HasColumnName("CreatedOn");
            entityBuilder.Property(t => t.CreatedBy).IsRequired().HasMaxLength(255).HasColumnName("CreatedBy");
            entityBuilder.Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            entityBuilder.Property(t => t.ModifiedBy).HasMaxLength(255).HasColumnName("ModifiedBy");
            entityBuilder.HasQueryFilter(m => EF.Property<bool>(m, "IsDeleted") == false);
        }
    }
}
