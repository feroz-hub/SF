using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zentra.Domain.Entities.Api;

namespace Zentra.Infrastructure.Data.Mapper.Api;

public class UserLoginsMap
{
    public UserLoginsMap(EntityTypeBuilder<UserLogins> entityBuilder)
    {
        UserLoginMapping(entityBuilder);
    }

    private void UserLoginMapping(EntityTypeBuilder<UserLogins> entityBuilder)
    {
        if (entityBuilder != null)
        {
            // Primary Key
            entityBuilder.HasKey(t => t.Id);
            entityBuilder.HasKey(l => new { l.LoginProvider, l.ProviderKey, l.UserId });

            // Properties
            entityBuilder.Property(t => t.LoginProvider).IsRequired().HasMaxLength(256);
            entityBuilder.Property(t => t.ProviderKey).IsRequired().HasMaxLength(256);

            // Table & Column Mappings
            entityBuilder.ToTable("Zentra_UserLogins");
            entityBuilder.Property(t => t.UserId).IsRequired().HasColumnName("UserId");
            entityBuilder.Property(t => t.ProviderDisplayName).HasColumnName("ProviderDisplayName");
            entityBuilder.Property(t => t.IsDeleted).IsRequired().HasColumnName("IsDeleted");
            entityBuilder.Property(t => t.CreatedOn).IsRequired().HasColumnName("CreatedOn");
            entityBuilder.Property(t => t.CreatedBy).IsRequired().HasMaxLength(255).HasColumnName("CreatedBy");
            entityBuilder.Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            entityBuilder.Property(t => t.ModifiedBy).HasMaxLength(255).HasColumnName("ModifiedBy");
            entityBuilder.HasQueryFilter(m => EF.Property<bool>(m, "IsDeleted") == false);
        }
    }
}
