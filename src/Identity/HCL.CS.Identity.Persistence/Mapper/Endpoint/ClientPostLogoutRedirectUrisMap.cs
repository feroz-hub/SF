using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HCL.CS.Domain.Entities.Endpoint;

namespace HCL.CS.Infrastructure.Data.Mapper.Endpoint;

public class ClientPostLogoutRedirectUrisMap
{
    public ClientPostLogoutRedirectUrisMap(EntityTypeBuilder<ClientPostLogoutRedirectUris> entityBuilder)
    {
        ClientPostLogoutRedirectUrisMapping(entityBuilder);
    }

    private void ClientPostLogoutRedirectUrisMapping(EntityTypeBuilder<ClientPostLogoutRedirectUris> entityBuilder)
    {
        if (entityBuilder != null)
        {
            entityBuilder.ToTable("HclCs_ClientPostLogoutRedirectUris");

            // Primary Key
            entityBuilder.HasKey(x => x.Id);
            entityBuilder.HasIndex(x => new { x.ClientId, x.PostLogoutRedirectUri }).IsUnique();

            // Properties
            entityBuilder.Property(x => x.PostLogoutRedirectUri).IsRequired().HasMaxLength(510)
                .HasColumnName("PostLogoutRedirectUri");

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
