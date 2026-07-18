/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Security.Claims;

namespace HCL.CS.Service.Implementation.Endpoint.Comparers;

internal class ClaimsComparer : IEqualityComparer<Claim>
{
    public bool Equals(Claim sourceClaim, Claim targetClaim)
    {
        if (ReferenceEquals(sourceClaim, targetClaim)) return true;

        if (ReferenceEquals(sourceClaim, null) || ReferenceEquals(targetClaim, null)) return false;

        return sourceClaim.Type.ToLower() == targetClaim.Type.ToLower() &&
               sourceClaim.Value.ToLower() == targetClaim.Value.ToLower();
    }

    public int GetHashCode(Claim claim)
    {
        if (ReferenceEquals(claim, null)) return 0;

        var hashClaimType = claim.Type == null ? 0 : claim.Type.GetHashCode();
        var hashClaimValue = claim.Value.GetHashCode();
        return hashClaimType ^ hashClaimValue;
    }
}
