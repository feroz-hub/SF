/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.Entities.Endpoint;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.UnitOfWork.Endpoint;

namespace HCL.CS.Infrastructure.Data.UnitOfWork.Endpoint;

internal class ClientsUnitOfWork : BaseDispose, IClientsUnitOfWork
{
    private readonly IApplicationDbContext context;

    private IRepository<Clients> clientRepository;
    private IRepository<ClientPostLogoutRedirectUris> postLogoutRedirectUrisRepository;
    private IRepository<ClientRedirectUris> redirectUrisRepository;
    private IRepository<SecurityTokens> securityTokensRepository;
    private IRepository<AuditTrail> auditTrailRepository;

    public ClientsUnitOfWork(IApplicationDbContext context)
    {
        this.context = context;
    }

    public IRepository<Clients> ClientRepository
    {
        get
        {
            if (clientRepository != null) return clientRepository;
            clientRepository = new BaseRepository<Clients>(context);
            return clientRepository;
        }
    }

    public IRepository<ClientRedirectUris> RedirectUrisRepository
    {
        get
        {
            if (redirectUrisRepository != null) return redirectUrisRepository;
            redirectUrisRepository = new BaseRepository<ClientRedirectUris>(context);
            return redirectUrisRepository;
        }
    }

    public IRepository<ClientPostLogoutRedirectUris> PostLogoutRedirectUrisRepository
    {
        get
        {
            if (postLogoutRedirectUrisRepository != null) return postLogoutRedirectUrisRepository;
            postLogoutRedirectUrisRepository = new BaseRepository<ClientPostLogoutRedirectUris>(context);
            return postLogoutRedirectUrisRepository;
        }
    }

    public IRepository<SecurityTokens> SecurityTokensRepository
    {
        get
        {
            if (securityTokensRepository != null) return securityTokensRepository;
            securityTokensRepository = new BaseRepository<SecurityTokens>(context);
            return securityTokensRepository;
        }
    }

    public IRepository<AuditTrail> AuditTrailRepository
    {
        get
        {
            if (auditTrailRepository != null) return auditTrailRepository;
            auditTrailRepository = new BaseRepository<AuditTrail>(context);
            return auditTrailRepository;
        }
    }

    public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        var transaction = await ((ApplicationDbContext)context)
            .Database
            .BeginTransactionAsync(cancellationToken);
        return new EfUnitOfWorkTransaction(transaction);
    }

    public async Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<FrameworkResult> SaveChangesWithHardDeleteAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesWithHardDeleteAsync(cancellationToken);
    }
}

internal sealed class EfUnitOfWorkTransaction(
    Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction)
    : IUnitOfWorkTransaction
{
    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        return transaction.CommitAsync(cancellationToken);
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        return transaction.RollbackAsync(cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return transaction.DisposeAsync();
    }
}
