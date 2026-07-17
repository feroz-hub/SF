using Zentra.Domain;
using Zentra.Domain.Entities.Endpoint;
using Zentra.Domain.Models.Endpoint;
using Zentra.Domain.Models.Endpoint.Request;
using Zentra.Domain.Models.Endpoint.Response;

namespace Zentra.Service.Interfaces.Interfaces.Endpoint;

public interface IAuthorizationService
{
    Task<string> SaveAuthorizationCodeAsync(AuthorizationCodeModel authCodeRequest);

    Task<FrameworkResult> DeleteAuthorizationCodeAsync(string authorizationCode);

    Task<AuthorizationCodeModel> GetAuthorizationCodeAsync(string authorizationCode);

    Task<Guid> SaveReturnUrlAsync(ValidatedAuthorizeRequestModel authCodeRequest);

    Task<Dictionary<string, string>> ValidateReturnUrlAsync(string requestId);

    Task<string> SaveVerificationCodeAsync(string name);

    Task<SecurityTokens> ValidateVerificationCodeAsync(string tokenValue);

    Task<FrameworkResult> DeleteSecurityTokenByIdAsync(Guid id);

    Task<FrameworkResult> DeleteSecurityTokenByTokenValueAsync(string tokenValue);

    Task<NavigationModel> CheckNavigationAsync(ValidatedAuthorizeRequestModel requestValidationModel);

    Task<AuthorizationResponseModel> ProcessAuthorizationCodeAsync(
        ValidatedAuthorizeRequestModel requestValidationModel);
}
