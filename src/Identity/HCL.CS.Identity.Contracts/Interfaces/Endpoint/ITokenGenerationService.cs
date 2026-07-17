using HCL.CS.Domain;
using HCL.CS.Domain.Models;
using HCL.CS.Domain.Models.Endpoint;
using HCL.CS.Domain.Models.Endpoint.Request;
using HCL.CS.Domain.Models.Endpoint.Response;
using HCL.CS.Domain.Models.Endpoint.Validation;

namespace HCL.CS.Service.Interfaces.Interfaces.Endpoint;

public interface ITokenGenerationService
{
    Task<TokenResponseModel> ProcessTokenAsync(ValidatedTokenRequestModel tokenRequest);

    Task<TokenValidationModel> ValidateRefreshTokenAsync(string refreshTokenKey, ClientsModel client);

    Task<FrameworkResult> RemoveUserTokensAsync(string userId);

    Task<FrameworkResult> RevokeTokenAsync(ValidatedRevocationRequestModel revocationRequest);

    Task<string> GenerateBackChannelLogoutTokenAsync(BackChannelLogoutModel backChannelLogoutModel);
}
