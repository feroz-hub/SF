/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

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
