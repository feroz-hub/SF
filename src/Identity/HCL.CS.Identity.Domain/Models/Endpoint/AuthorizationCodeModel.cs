/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Security.Claims;

namespace HCL.CS.Domain.Models.Endpoint;

public class AuthorizationCodeModel
{
    public virtual Guid Id { get; set; } = default;

    public virtual string State { get; set; }

    public virtual string ClientId { get; set; }

    public virtual string RedirectUri { get; set; }

    public bool IsOpenId { get; set; }

    public DateTime CreationTime { get; set; }

    public int Lifetime { get; set; }

    public ClaimsPrincipal Subject { get; set; }

    public string Nonce { get; set; }

    public string SessionId { get; set; }

    public string CodeChallenge { get; set; }

    public string CodeChallengeMethod { get; set; }

    public List<string> RequestedScopes { get; set; }

    public AllowedScopesParserModel AllowedScopesParserModel { get; set; }

    public TokenDetailsModel TokenDetails { get; set; }
}
