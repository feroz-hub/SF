using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zentra.Domain.Entities.Endpoint;

namespace Zentra.Infrastructure.Data.Mapper.Endpoint;

public class ClientsMap
{
    public ClientsMap(EntityTypeBuilder<Clients> entityBuilder)
    {
        ClientsMapping(entityBuilder);
    }

    private void ClientsMapping(EntityTypeBuilder<Clients> entityBuilder)
    {
        if (entityBuilder != null)
        {
            entityBuilder.ToTable("Zentra_Clients");

            // Primary Key
            entityBuilder.HasKey(x => x.Id);
            entityBuilder.HasIndex(x => new { x.ClientId, x.ClientSecret }).HasDatabaseName("IX_CLI_CLID_CLSEC")
                .IsUnique();

            // Properties
            entityBuilder.Property(x => x.ClientId).HasMaxLength(128).IsRequired();
            entityBuilder.Property(x => x.ClientName).HasMaxLength(255);
            entityBuilder.Property(x => x.ClientSecret).HasMaxLength(128);
            entityBuilder.Property(x => x.PreferredAudience).HasMaxLength(300);

            // Table & Column Mappings
            entityBuilder.Property(x => x.IsDeleted).IsRequired().HasColumnName("IsDeleted");
            entityBuilder.Property(x => x.CreatedOn).IsRequired().HasColumnName("CreatedOn");
            entityBuilder.Property(x => x.CreatedBy).IsRequired().HasMaxLength(255).HasColumnName("CreatedBy");
            entityBuilder.Property(x => x.ModifiedOn).HasColumnName("ModifiedOn");
            entityBuilder.Property(x => x.ModifiedBy).HasMaxLength(255).HasColumnName("ModifiedBy");

            // Mappings
            entityBuilder.HasMany(x => x.RedirectUris).WithOne(x => x.Client)
                .HasForeignKey(x => x.ClientId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            entityBuilder.HasMany(x => x.PostLogoutRedirectUris).WithOne(x => x.Client)
                .HasForeignKey(x => x.ClientId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            entityBuilder.HasQueryFilter(m => EF.Property<bool>(m, "IsDeleted") == false);
        }
    }
}
