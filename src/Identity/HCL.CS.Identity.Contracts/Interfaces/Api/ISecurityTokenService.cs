using HCL.CS.Domain.Models.Api;
using HCL.CS.Domain.Models.Endpoint;

namespace HCL.CS.Service.Interfaces.Interfaces.Api;

public interface ISecurityTokenService
{
    Task<IList<TokenModel>> GetClientsActiveSecurityTokensAsync(IList<string> clientIds, PagingModel page = null);

    Task<IList<TokenModel>> GetUsersActiveSecurityTokensAsync(IList<string> userIds, PagingModel page = null);

    Task<IList<TokenModel>> GetActiveSecurityTokensAsync(DateTime fromdate, DateTime todate, PagingModel page = null);

    Task<IList<TokenModel>> GetAllSecurityTokensAsync(DateTime fromdate, DateTime todate, PagingModel page = null);
}
