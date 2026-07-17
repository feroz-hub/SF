using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zentra.Domain.Entities.Api;

namespace Zentra.Infrastructure.Data.Mapper.Api;

public class UserTokensMap
{
    public UserTokensMap(EntityTypeBuilder<UserTokens> entityBuilder)
    {
        UserTokenMapping(entityBuilder);
    }

    private void UserTokenMapping(EntityTypeBuilder<UserTokens> entityBuilder)
    {
        if (entityBuilder != null)
        {
            // Primary Key
            entityBuilder.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

            // Properties
            entityBuilder.Property(t => t.LoginProvider).IsRequired().HasMaxLength(255);
            entityBuilder.Property(t => t.Name).IsRequired().HasMaxLength(255);

            // Table & Column Mappings
            entityBuilder.ToTable("Zentra_UserTokens");
            entityBuilder.Property(t => t.UserId).IsRequired().HasColumnName("UserId");
            entityBuilder.Property(t => t.Value).IsRequired().HasColumnName("Value");
        }
    }
}
