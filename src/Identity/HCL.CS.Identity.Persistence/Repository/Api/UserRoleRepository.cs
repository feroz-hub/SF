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

internal class UserRoleRepository : BaseDispose, IUserRoleRepository
{
    private readonly IApplicationDbContext context;

    public UserRoleRepository(IApplicationDbContext context)
    {
        this.context = context;
    }

    public Task InsertAsync(UserRoles entity)
    {
        context.UserRoles.Add(entity);
        return Task.CompletedTask;
    }

    public virtual Task UpdateAsync(UserRoles entity)
    {
        context.UserRoles.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(IList<UserRoles> entityList)
    {
        foreach (var entity in entityList) context.UserRoles.Remove(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(UserRoles entity)
    {
        context.UserRoles.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<UserRoles> GetUserRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var entity = await context.UserRoles.Where(roleClaimEntity => roleClaimEntity.UserId == userId &&
                                                                      roleClaimEntity.RoleId == roleId)
            .FirstOrDefaultAsync(cancellationToken);
        return entity;
    }

    public async Task<IList<UserRoles>> GetUserRoleAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var entity = await context.UserRoles.Where(roleClaimEntity => roleClaimEntity.UserId == userId).ToListAsync(cancellationToken);
        return entity;
    }

    public async Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}
