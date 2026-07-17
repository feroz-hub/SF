using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HCL.CS.Domain.Entities.Api;

namespace HCL.CS.Infrastructure.Data.Mapper.Api;

public class ExternalIdentitiesMap
{
    public ExternalIdentitiesMap(EntityTypeBuilder<ExternalIdentities> entityBuilder)
    {
        ExternalIdentityMapping(entityBuilder);
    }

    private static void ExternalIdentityMapping(EntityTypeBuilder<ExternalIdentities> entityBuilder)
    {
        if (entityBuilder == null) return;

        entityBuilder.ToTable("HclCs_ExternalIdentities");

        entityBuilder.HasKey(x => x.Id);

        entityBuilder.Property(x => x.UserId).IsRequired();
        entityBuilder.Property(x => x.TenantId).HasMaxLength(128);
        entityBuilder.Property(x => x.Provider).IsRequired().HasMaxLength(64);
        entityBuilder.Property(x => x.Issuer).IsRequired().HasMaxLength(256);
        entityBuilder.Property(x => x.Subject).IsRequired().HasMaxLength(256);
        entityBuilder.Property(x => x.Email).IsRequired().HasMaxLength(255);
        entityBuilder.Property(x => x.EmailVerified).IsRequired();
        entityBuilder.Property(x => x.LinkedAt).IsRequired();
        entityBuilder.Property(x => x.LastSignInAt);
        entityBuilder.Property(x => x.IsDeleted).IsRequired();
        entityBuilder.Property(x => x.CreatedOn).IsRequired();
        entityBuilder.Property(x => x.CreatedBy).IsRequired().HasMaxLength(255);
        entityBuilder.Property(x => x.ModifiedBy).HasMaxLength(255);

        entityBuilder.HasOne<Users>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        entityBuilder.HasIndex(x => new { x.Provider, x.Issuer, x.Subject })
            .IsUnique()
            .HasDatabaseName("IX_EXTID_PROVIDER_ISSUER_SUBJECT");

        entityBuilder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_EXTID_USERID");

        entityBuilder.HasIndex(x => new { x.TenantId, x.Email })
            .HasDatabaseName("IX_EXTID_TENANT_EMAIL");

        entityBuilder.HasQueryFilter(x => EF.Property<bool>(x, "IsDeleted") == false);
    }
}
