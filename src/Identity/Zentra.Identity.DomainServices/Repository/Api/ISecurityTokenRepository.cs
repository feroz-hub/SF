using System.Threading;
using Zentra.Domain.Enums;
using Zentra.Domain.Models.Api;
using Zentra.Domain.Models.Endpoint;

namespace Zentra.DomainServices.Repository.Api;

public interface ISecurityTokenRepository
{
    Task<IList<TokenModel>> GetSecurityTokenAsync(PagingModel page,
        SecurityTokenOption option,
        DateTime? fromdate = null,
        DateTime? todate = null,
        IList<string> clientIds = null,
        IList<string> userIds = null,
        CancellationToken cancellationToken = default);
}
