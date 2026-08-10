/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Threading;
using Microsoft.EntityFrameworkCore;
using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.Repository.Api;

namespace HCL.CS.Infrastructure.Data.Repository.Api;

internal class RoleRepository : BaseDispose, IRoleRepository
{
    private readonly IApplicationDbContext context;

    public RoleRepository(IApplicationDbContext context)
    {
        this.context = context;
    }

    public Task UpdateAsync(Roles entity)
    {
        context.Roles.Attach(entity);
        context.SetModifiedStatus(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Roles entity)
    {
        context.Roles.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<IList<Roles>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await context.Roles.AsNoTracking().ToListAsync(cancellationToken);
        return roles;
    }

    public async Task<bool> ExistsByNameIncludingDeletedAsync(string roleName, CancellationToken cancellationToken = default)
    {
        return await context.Roles
            .IgnoreQueryFilters()
            .AnyAsync(x => x.Name == roleName, cancellationToken);
    }

    public async Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}
