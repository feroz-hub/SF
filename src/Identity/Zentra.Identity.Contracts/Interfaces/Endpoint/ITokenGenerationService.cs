using Zentra.Domain;
using Zentra.Domain.Models;
using Zentra.Domain.Models.Endpoint;
using Zentra.Domain.Models.Endpoint.Request;
using Zentra.Domain.Models.Endpoint.Response;
using Zentra.Domain.Models.Endpoint.Validation;

namespace Zentra.Service.Interfaces.Interfaces.Endpoint;

public interface ITokenGenerationService
{
    Task<TokenResponseModel> ProcessTokenAsync(ValidatedTokenRequestModel tokenRequest);

    Task<TokenValidationModel> ValidateRefreshTokenAsync(string refreshTokenKey, ClientsModel client);

    Task<FrameworkResult> RemoveUserTokensAsync(string userId);

    Task<FrameworkResult> RevokeTokenAsync(ValidatedRevocationRequestModel revocationRequest);

    Task<string> GenerateBackChannelLogoutTokenAsync(BackChannelLogoutModel backChannelLogoutModel);
}
