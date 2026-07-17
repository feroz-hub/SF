using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HCL.CS.Domain.Entities.Api;

namespace HCL.CS.Infrastructure.Data.Mapper.Api;

public class ExternalAuthProviderConfigMap
{
    public ExternalAuthProviderConfigMap(EntityTypeBuilder<ExternalAuthProviderConfig> entityBuilder)
    {
        ExternalAuthProviderConfigMapping(entityBuilder);
    }

    private void ExternalAuthProviderConfigMapping(EntityTypeBuilder<ExternalAuthProviderConfig> entityBuilder)
    {
        if (entityBuilder != null)
        {
            // Primary Key
            entityBuilder.HasKey(t => t.Id);
            entityBuilder.HasIndex(t => t.ProviderName).IsUnique().HasDatabaseName("IX_EAPC_PROVIDER");
            entityBuilder.HasIndex(t => new { t.ProviderName, t.IsEnabled }).HasDatabaseName("IX_EAPC_PROVIDER_ENABLED");

            // Properties
            entityBuilder.Property(t => t.ProviderName).IsRequired().HasMaxLength(50);
            entityBuilder.Property(t => t.ProviderType).IsRequired();
            entityBuilder.Property(t => t.IsEnabled).IsRequired();
            entityBuilder.Property(t => t.ConfigJson).IsRequired();
            entityBuilder.Property(t => t.AutoProvisionEnabled).IsRequired();
            entityBuilder.Property(t => t.AllowedDomains).HasMaxLength(2000);
            entityBuilder.Property(t => t.LastTestedOn);
            entityBuilder.Property(t => t.LastTestSuccess);

            // Table & Column Mappings
            entityBuilder.ToTable("HclCs_ExternalAuthProviderConfig");
            entityBuilder.Property(t => t.Id).HasColumnName("Id");
            entityBuilder.Property(t => t.IsDeleted).IsRequired().HasColumnName("IsDeleted");
            entityBuilder.Property(t => t.CreatedOn).IsRequired().HasColumnName("CreatedOn");
            entityBuilder.Property(t => t.CreatedBy).IsRequired().HasMaxLength(255).HasColumnName("CreatedBy");
            entityBuilder.Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            entityBuilder.Property(t => t.ModifiedBy).HasMaxLength(255).HasColumnName("ModifiedBy");
            entityBuilder.HasQueryFilter(m => EF.Property<bool>(m, "IsDeleted") == false);

            entityBuilder.Ignore(x => x.RowVersion);
        }
    }
}
