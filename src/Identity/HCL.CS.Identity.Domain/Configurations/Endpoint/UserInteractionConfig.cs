using HCL.CS.Domain.Constants.Endpoint;

namespace HCL.CS.Domain.Configurations.Endpoint;

public class UserInteractionConfig
{
    public string LoginUrl { get; set; }

    public string LoginReturnUrlParameter { get; set; } =
        AuthenticationConstants.ApplicationUIConstants.DefaultRoutePathParams.Login;

    public string LogoutUrl { get; set; }

    public string LogoutIdParameter { get; set; } =
        AuthenticationConstants.ApplicationUIConstants.DefaultRoutePathParams.Logout;

    public string ErrorUrl { get; set; }

    public string ErrorIdParameter { get; set; } =
        AuthenticationConstants.ApplicationUIConstants.DefaultRoutePathParams.Error;
}
