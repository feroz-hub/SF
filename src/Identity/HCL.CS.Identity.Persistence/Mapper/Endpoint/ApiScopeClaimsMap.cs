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

namespace HCL.CS.Infrastructure.Data.Mapper.Endpoint;

public class ApiScopeClaimsMap
{
    public ApiScopeClaimsMap(EntityTypeBuilder<ApiScopeClaims> entityBuilder)
    {
        ApiScopeClaimsMapping(entityBuilder);
    }

    private void ApiScopeClaimsMapping(EntityTypeBuilder<ApiScopeClaims> entityBuilder)
    {
        if (entityBuilder != null)
        {
            entityBuilder.ToTable("HclCs_ApiScopeClaims");

            // Primary Key
            entityBuilder.HasKey(x => x.Id);
            entityBuilder.HasIndex(t => new { t.ApiScopeId, t.Type }).HasDatabaseName("IX_APISCO_CLM_SCOID_TYPE")
                .IsUnique();

            // Properties
            entityBuilder.Property(x => x.ApiScopeId).IsRequired();
            entityBuilder.Property(x => x.Type).HasMaxLength(255).IsRequired();

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
