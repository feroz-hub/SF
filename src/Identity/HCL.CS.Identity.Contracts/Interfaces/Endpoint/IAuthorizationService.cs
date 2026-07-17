using HCL.CS.Domain;
using HCL.CS.Domain.Entities.Endpoint;
using HCL.CS.Domain.Models.Endpoint;
using HCL.CS.Domain.Models.Endpoint.Request;
using HCL.CS.Domain.Models.Endpoint.Response;

namespace HCL.CS.Service.Interfaces.Interfaces.Endpoint;

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
