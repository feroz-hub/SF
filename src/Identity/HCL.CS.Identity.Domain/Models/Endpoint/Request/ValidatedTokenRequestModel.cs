/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain.Models.Endpoint.Validation;

namespace HCL.CS.Domain.Models.Endpoint.Request;

public class ValidatedTokenRequestModel : ValidatedBaseModel
{
    public string Issuer { get; set; }

    public string Nonce { get; set; }

    public string State { get; set; }

    public string GrantType { get; set; }

    public string UserName { get; set; }

    public string RequestedRefreshToken { get; set; }

    public AuthorizationCodeModel AuthorizationCode { get; set; }

    public string AccessTokenToHash { get; set; }

    public string AuthorizationCodeToHash { get; set; }

    public string CodeVerifier { get; set; }

    public bool IsRequestFromAuthorizationEndpoint { get; set; } = false;

    public List<string> ResponseTypes { get; set; }

    public TokenDetailsModel TokenDetails { get; set; }
}
