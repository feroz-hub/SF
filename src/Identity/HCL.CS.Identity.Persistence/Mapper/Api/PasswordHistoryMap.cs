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
