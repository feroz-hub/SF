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
            entityBuilder.ToTable("HclCs_UserTokens");
            entityBuilder.Property(t => t.UserId).IsRequired().HasColumnName("UserId");
            entityBuilder.Property(t => t.Value).IsRequired().HasColumnName("Value");
        }
    }
}
