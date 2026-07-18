/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HCL.CS.Domain.Entities.Api;

namespace HCL.CS.Infrastructure.Data.Mapper.Api;

public class NotificationProviderConfigMap
{
    public NotificationProviderConfigMap(EntityTypeBuilder<NotificationProviderConfig> entityBuilder)
    {
        NotificationProviderConfigMapping(entityBuilder);
    }

    private void NotificationProviderConfigMapping(EntityTypeBuilder<NotificationProviderConfig> entityBuilder)
    {
        if (entityBuilder != null)
        {
            // Primary Key
            entityBuilder.HasKey(t => t.Id);
            entityBuilder.HasIndex(t => t.ChannelType).HasDatabaseName("IX_NPC_CHANNEL_TYPE");
            entityBuilder.HasIndex(t => new { t.ChannelType, t.IsActive }).HasDatabaseName("IX_NPC_CHANNEL_ACTIVE");

            // Properties
            entityBuilder.Property(t => t.ProviderName).IsRequired().HasMaxLength(50);
            entityBuilder.Property(t => t.ChannelType).IsRequired();
            entityBuilder.Property(t => t.IsActive).IsRequired();
            entityBuilder.Property(t => t.ConfigJson).IsRequired();
            entityBuilder.Property(t => t.LastTestedOn);
            entityBuilder.Property(t => t.LastTestSuccess);

            // Table & Column Mappings
            entityBuilder.ToTable("HclCs_NotificationProviderConfig");
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
