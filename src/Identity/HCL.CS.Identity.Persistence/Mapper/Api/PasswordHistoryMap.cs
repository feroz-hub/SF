using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HCL.CS.Domain.Entities.Api;

namespace HCL.CS.Infrastructure.Data.Mapper.Api;

public class PasswordHistoryMap
{
    public PasswordHistoryMap(EntityTypeBuilder<PasswordHistory> entityBuilder)
    {
        PasswordHistoryMapping(entityBuilder);
    }

    private void PasswordHistoryMapping(EntityTypeBuilder<PasswordHistory> entityBuilder)
    {
        if (entityBuilder != null)
        {
            entityBuilder.ToTable("HclCs_PasswordHistory");

            // Primary Key
            entityBuilder.HasKey(t => t.Id);

            // Properties
            entityBuilder.Property(t => t.Id).HasColumnName("Id");
            entityBuilder.Property(t => t.PasswordHash).IsRequired().HasMaxLength(255);
            entityBuilder.Property(t => t.UserId).IsRequired().HasColumnName("UserID");
            entityBuilder.Property(t => t.IsDeleted).IsRequired().HasColumnName("IsDeleted");
            entityBuilder.Property(t => t.ChangedOn).IsRequired().HasColumnName("ChangedOn");
            entityBuilder.Property(t => t.CreatedOn).IsRequired().HasColumnName("CreatedOn");
            entityBuilder.Property(t => t.CreatedBy).IsRequired().HasMaxLength(255).HasColumnName("CreatedBy");
            entityBuilder.HasQueryFilter(m => EF.Property<bool>(m, "IsDeleted") == false);

            entityBuilder.Ignore(x => x.ModifiedBy);
            entityBuilder.Ignore(x => x.ModifiedOn);
            entityBuilder.Ignore(x => x.RowVersion);
        }
    }
}
