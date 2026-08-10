/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Security.Claims;

namespace HCL.CS.Domain.Models.Endpoint;

public class ResultClaimsModel
{
    public virtual List<Claim> RoleClaims { get; set; }

    public virtual List<Claim> AudienceClaims { get; set; }

    public virtual List<Claim> IdentityTokenScopeClaims { get; set; }

    public virtual List<Claim> AccessTokenScopeClaims { get; set; }

    public virtual List<Claim> IdentityClaims { get; set; }

    public virtual List<Claim> TransactionClaims { get; set; }

    public virtual List<Claim> PermissionClaims { get; set; }

    /// <summary>Custom access-token claims that keep their claim type (e.g. capabilities). Not merged into permission claims.</summary>
    public virtual List<Claim> CustomAccessTokenClaims { get; set; }

    public List<Claim> AccessTokenClaims
    {
        get
        {
            var accessTokenClaims = new List<Claim>();
            accessTokenClaims.AddRange(IdentityTokenScopeClaims);
            // Standard identity claims requested through openid/profile/email are
            // also useful to resource servers. They remain scope-gated because
            // IdentityClaims is populated only from the requested resources.
            accessTokenClaims.AddRange(IdentityClaims);
            accessTokenClaims.AddRange(RoleClaims);
            accessTokenClaims.AddRange(TransactionClaims);
            accessTokenClaims.AddRange(PermissionClaims);
            if (CustomAccessTokenClaims != null)
                accessTokenClaims.AddRange(CustomAccessTokenClaims);
            return accessTokenClaims;
        }
    }
}
