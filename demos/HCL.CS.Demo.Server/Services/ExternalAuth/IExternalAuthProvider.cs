using System.Security.Claims;

namespace HCL.CS.DemoServerApp.Services.ExternalAuth;

public interface IExternalAuthProvider
{
    string Provider { get; }

    bool CanHandle(string provider);

    ExternalIdentityPayload ParseIdentity(ClaimsPrincipal principal);
}
