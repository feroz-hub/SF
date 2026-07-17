using System.Threading;
using HCL.CS.Domain.Enums;
using HCL.CS.Domain.Models.Api;
using HCL.CS.Domain.Models.Endpoint;

namespace HCL.CS.DomainServices.Repository.Api;

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
