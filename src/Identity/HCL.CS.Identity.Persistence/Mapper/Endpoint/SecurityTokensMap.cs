/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HCL.CS.Domain.Entities.Endpoint;

namespace HCL.CS.Infrastructure.Data.Mapper.Endpoint;

public class SecurityTokensMap
{
    public SecurityTokensMap(EntityTypeBuilder<SecurityTokens> entityBuilder)
    {
        SecurityTokensMapping(entityBuilder);
    }

    private void SecurityTokensMapping(EntityTypeBuilder<SecurityTokens> entityBuilder)
    {
        if (entityBuilder != null)
        {
            entityBuilder.ToTable("HclCs_SecurityTokens");

            // Primary Key
            entityBuilder.HasKey(x => x.Id);

            // Table & Column Mappings
            entityBuilder.Property(x => x.ConsumedTime).HasColumnName("ConsumedTime");
            entityBuilder.Property(x => x.ConsumedAt).HasColumnName("ConsumedAt");
            entityBuilder.Property(x => x.CreationTime).HasColumnName("CreationTime");
            entityBuilder.Property(x => x.ExpiresAt).HasColumnName("ExpiresAt");
            entityBuilder.Property(x => x.TokenReuseDetected).HasColumnName("TokenReuseDetected");
            entityBuilder.Property(x => x.IsDeleted).IsRequired().HasColumnName("IsDeleted");
            entityBuilder.Property(x => x.CreatedOn).IsRequired().HasColumnName("CreatedOn");
            entityBuilder.Property(x => x.CreatedBy).IsRequired().HasMaxLength(255).HasColumnName("CreatedBy");
            entityBuilder.Property(x => x.ModifiedOn).HasColumnName("ModifiedOn");
            entityBuilder.Property(x => x.ModifiedBy).HasMaxLength(255).HasColumnName("ModifiedBy");
            entityBuilder.HasIndex(x => new { x.TokenType, x.Key }).HasDatabaseName("IX_SECTOK_TOKTYPE_KEY");
            entityBuilder.HasQueryFilter(m => EF.Property<bool>(m, "IsDeleted") == false);

            entityBuilder.Ignore(x => x.RowVersion);
        }
    }
}
