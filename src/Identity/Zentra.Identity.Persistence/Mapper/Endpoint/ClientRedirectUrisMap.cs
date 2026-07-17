using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zentra.Domain.Entities.Endpoint;

namespace Zentra.Infrastructure.Data.Mapper.Endpoint;

public class ClientRedirectUrisMap
{
    public ClientRedirectUrisMap(EntityTypeBuilder<ClientRedirectUris> entityBuilder)
    {
        ClientRedirectUrisMapping(entityBuilder);
    }

    private void ClientRedirectUrisMapping(EntityTypeBuilder<ClientRedirectUris> entityBuilder)
    {
        if (entityBuilder != null)
        {
            entityBuilder.ToTable("Zentra_ClientRedirectUris");

            // Primary Key
            entityBuilder.HasKey(x => x.Id);
            entityBuilder.HasIndex(x => new { x.ClientId, x.RedirectUri }).IsUnique();

            // Properties
            entityBuilder.Property(x => x.RedirectUri).IsRequired().HasMaxLength(510).HasColumnName("RedirectUri");

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
