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

internal class UserTokenRepository : BaseDispose, IUserTokenRepository
{
    private readonly IApplicationDbContext context;

    public UserTokenRepository(IApplicationDbContext context)
    {
        this.context = context;
    }

    public Task DeleteAsync(IList<UserTokens> entityList)
    {
        foreach (var entity in entityList)
        {
            entity.IsDeleted = true;
            context.UserTokens.Remove(entity);
        }

        return Task.CompletedTask;
    }

    public async Task<IList<UserTokens>> GetUserTokenAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userToken = await context.UserTokens.Where(entity => entity.UserId == userId).ToListAsync(cancellationToken);
        if (userToken.Any()) return userToken;

        return null;
    }

    public async Task<IList<UserTokens>> GetUserTokenAsync(Guid userId, string name, string loginProvider, CancellationToken cancellationToken = default)
    {
        var userToken = await context.UserTokens.Where(entity =>
            entity.UserId == userId && entity.Name == name && entity.LoginProvider == loginProvider).ToListAsync(cancellationToken);
        if (userToken.Any()) return userToken;

        return null;
    }

    public async Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}
