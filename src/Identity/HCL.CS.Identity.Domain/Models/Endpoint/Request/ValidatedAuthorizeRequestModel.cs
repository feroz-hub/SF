/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Security.Claims;
using HCL.CS.Domain.Models.Endpoint.Validation;

namespace HCL.CS.Domain.Models.Endpoint.Request;

public class ValidatedAuthorizeRequestModel : ValidatedBaseModel
{
    public ValidatedAuthorizeRequestModel()
    {
        RequestedScopes = new List<string>();
        AuthenticationContextReferenceClasses = new List<string>();
    }

    public string ResponseType { get; set; }

    public string ResponseMode { get; set; }

    public string GrantType { get; set; }

    public List<string> RequestedScopes { get; set; }

    public string State { get; set; }

    public bool IsOpenIdRequest { get; set; } = false;

    public bool IsApiResourceRequest { get; set; }

    public string Nonce { get; set; }

    public List<string> AuthenticationContextReferenceClasses { get; set; }

    public IEnumerable<string> PromptModes { get; set; } = Enumerable.Empty<string>();

    public int? MaxAge { get; set; }

    public string CodeChallenge { get; set; }

    public string CodeChallengeMethod { get; set; }

    public ClaimsPrincipal User { get; set; }
}
