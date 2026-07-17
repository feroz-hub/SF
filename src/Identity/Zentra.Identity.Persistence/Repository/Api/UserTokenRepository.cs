using System.Threading;
using Microsoft.EntityFrameworkCore;
using Zentra.Domain;
using Zentra.Domain.Entities.Api;
using Zentra.DomainServices;
using Zentra.DomainServices.Repository.Api;

namespace Zentra.Infrastructure.Data.Repository.Api;

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
