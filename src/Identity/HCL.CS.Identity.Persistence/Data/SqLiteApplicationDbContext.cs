/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HCL.CS.Infrastructure.Data;

public class SqLiteApplicationDbContext : ApplicationDbContext
{
    public SqLiteApplicationDbContext()
    {
    }

    public SqLiteApplicationDbContext(DbContextOptions<SqLiteApplicationDbContext> options)
        : base(ChangeOptionsType(options))
    {
    }

    private static DbContextOptions<ApplicationDbContext> ChangeOptionsType(DbContextOptions<SqLiteApplicationDbContext> options)
    {
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        foreach (var extension in options.Extensions)
        {
            ((IDbContextOptionsBuilderInfrastructure)builder).AddOrUpdateExtension(extension);
        }
        return builder.Options;
    }
}
