/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Security.Claims;
using HCL.CS.Domain.Enums;
using HCL.CS.Domain.Models.Endpoint.Response;

namespace HCL.CS.Domain.Models.Endpoint.Validation;

public class ValidatedBaseModel : ErrorResponseModel
{
    public Dictionary<string, string> RequestRawData { get; set; }

    public string ClientId { get; set; }

    public ClientsModel Client { get; set; }

    public ParsedSecretModel Secret { get; set; }

    public int AccessTokenExpiration { get; set; }

    public AccessTokenType AccessTokenType { get; set; }

    public ClaimsPrincipal Subject { get; set; }

    public string SessionId { get; set; }

    public TokenSettings TokenConfigOptions { get; set; }

    public AllowedScopesParserModel AllowedScopesParserModel { get; set; }

    public string RedirectUri { get; set; }

    public string EndpointBaseUrl { get; set; }

    public void SetClient(ClientsModel client, ParsedSecretModel secret = null)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
        Secret = secret;
        ClientId = client.ClientId;

        AccessTokenExpiration = client.AccessTokenExpiration;
        AccessTokenType = client.AccessTokenType;
    }
}
