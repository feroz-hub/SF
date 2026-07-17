using Zentra.Domain;
using Zentra.Domain.Entities.Endpoint;
using Zentra.DomainServices;
using Zentra.DomainServices.UnitOfWork.Endpoint;

namespace Zentra.Infrastructure.Data.UnitOfWork.Endpoint;

internal class ClientsUnitOfWork : BaseDispose, IClientsUnitOfWork
{
    private readonly IApplicationDbContext context;

    private IRepository<Clients> clientRepository;
    private IRepository<ClientPostLogoutRedirectUris> postLogoutRedirectUrisRepository;
    private IRepository<ClientRedirectUris> redirectUrisRepository;
    private IRepository<SecurityTokens> securityTokensRepository;

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

    public async Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<FrameworkResult> SaveChangesWithHardDeleteAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesWithHardDeleteAsync(cancellationToken);
    }
}
