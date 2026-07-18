/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.Repository.Api;
using HCL.CS.DomainServices.UnitOfWork.Api;
using HCL.CS.Infrastructure.Data.Repository.Api;

namespace HCL.CS.Infrastructure.Data.UnitOfWork.Api;

internal class RoleManagementUnitOfWork : BaseDispose, IRoleManagementUnitOfWork
{
    public readonly IApplicationDbContext context;
    private IRoleClaimsRepository roleClaimsRepository;
    private IRoleRepository roleRepository;

    public RoleManagementUnitOfWork(IApplicationDbContext context)
    {
        this.context = context;
    }

    public IRoleRepository RoleRepository
    {
        get
        {
            if (roleRepository != null) return roleRepository;
            roleRepository = new RoleRepository(context);
            return roleRepository;
        }
    }

    public IRoleClaimsRepository RoleClaimsRepository
    {
        get
        {
            if (roleClaimsRepository != null) return roleClaimsRepository;
            roleClaimsRepository = new RoleClaimsRepository(context);
            return roleClaimsRepository;
        }
    }

    public Task SetAddedStatusAsync<T>(T entity)
    {
        context.SetAddedStatus(entity);
        return Task.CompletedTask;
    }

    public Task SetModifiedStatusAsync<T>(T entity, string concurrencyStamp)
    {
        context.SetConcurrencyStatus(entity, concurrencyStamp);
        return Task.CompletedTask;
    }

    public async Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}
