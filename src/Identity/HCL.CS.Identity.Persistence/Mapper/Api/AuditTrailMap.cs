using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HCL.CS.Domain.Entities.Api;

namespace HCL.CS.Infrastructure.Data.Mapper.Api;

public class AuditTrailMap
{
    public AuditTrailMap(EntityTypeBuilder<AuditTrail> entityBuilder)
    {
        AuditMapping(entityBuilder);
    }

    private void AuditMapping(EntityTypeBuilder<AuditTrail> entityBuilder)
    {
        if (entityBuilder != null)
        {
            entityBuilder.ToTable("HclCs_AuditTrail");
            entityBuilder.HasKey(t => t.Id);
            entityBuilder.HasIndex(t => new { t.CreatedOn, t.CreatedBy }).HasDatabaseName("IX_AUD_CRON_CBBY");
            entityBuilder.HasIndex(t => new { t.CreatedOn, t.ActionType }).HasDatabaseName("IX_AUD_CRON_ACTY");
            entityBuilder.HasIndex(t => new { t.CreatedBy, t.ActionType }).HasDatabaseName("IX_AUD_CBBY_ACTY");

            entityBuilder.Property(t => t.ActionType).IsRequired().HasMaxLength(50);
            entityBuilder.Property(t => t.TableName).HasMaxLength(255);
            entityBuilder.Property(t => t.AffectedColumn);
            entityBuilder.Property(t => t.OldValue);
            entityBuilder.Property(t => t.NewValue);
            entityBuilder.Property(t => t.CreatedOn).IsRequired();
            entityBuilder.Property(t => t.CreatedBy).IsRequired().HasMaxLength(255);

            entityBuilder.Ignore(x => x.IsDeleted);
            entityBuilder.Ignore(x => x.ModifiedBy);
            entityBuilder.Ignore(x => x.ModifiedOn);
            entityBuilder.Ignore(x => x.RowVersion);
        }
    }
}
